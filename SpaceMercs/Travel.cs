using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using SpaceMercs.MainWindow;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    // Deal with travelling between AstronomicalObjects
    class Travel {
        private readonly AstronomicalObject aoTravelFrom, aoTravelTo;
        private readonly float fTravelTime;
        private float fMissionElapsed, fElapsed;
        private float fSep; // Separation between two ships
        private bool bSurrendered = false;
        private readonly Team PlayerTeam;
        private readonly MapView ParentView;
        private int EncounterCount = 0;
        private DateTime dtStart;
        private bool bPause = false;
        private readonly Random rand = new Random();
        private VertexBuffer? vbShot = null;
        private VertexArray? vaShot = null;

        private readonly Color4 ShotColPlayer = new Color4(0.1f, 0.3f, 1.0f, 1.0f);
        private readonly Color4 ShotColEnemy = new Color4(1.0f, 0.3f, 0.1f, 1.0f);
        private const float shipYpos = 0.48f;

        private class Shot {
            public bool Player { get; private set; }
            public int Attack { get; private set; }
            public int Life { get; private set; }
            public readonly float fx, fy, tx, ty;
            public Shot(bool pl, ShipRoomDesign rf, ShipRoomDesign rt, int att, float fSep, float aspect) {
                Player = pl;
                Attack = att;
                Life = (att * 5) + 15;
                float fSpace = fSep * Const.DrawBattleScale;

                if (Player) {
                    fx = (0.46f - fSpace) - ((rf.XPos + rf.Width / 2f) * 0.005f / aspect);
                    tx = (0.54f + fSpace) + ((rt.XPos + rt.Width / 2f) * 0.005f / aspect);
                }
                else {
                    fx = (0.54f + fSpace) + ((rf.XPos + rf.Width / 2f) * 0.005f / aspect);
                    tx = (0.46f - fSpace) - ((rt.XPos + rf.Width / 2f) * 0.005f / aspect);
                }
                fy = shipYpos + ((rf.YPos - rf.Height / 2f) * 0.005f);
                ty = shipYpos + ((rt.YPos - rf.Height / 2f) * 0.005f);
            }
            public void Update() {
                Life--;
            }
        }
        private readonly List<Shot> lShots = new List<Shot>();

        private class Frag {
            private float X, Y;
            private readonly float VX, VY;
            public float F { get; private set; }
            public Frag(float x, float y, float vx, float vy, float f) {
                X = x;
                Y = y;
                VX = vx;
                VY = vy;
                F = f;
            }
            public void Display(ShaderProgram prog, float aspect) {
                Matrix4 translateM = Matrix4.CreateTranslation(X - 0.0015f / aspect, Y - 0.0015f, 0f);
                Matrix4 scaleM = Matrix4.CreateScale(0.003f / aspect, 0.003f, 1f);
                prog.SetUniform("model", scaleM * translateM);
                prog.SetUniform("flatColour", new Vector4(0.2f + F * 0.8f, 0.2f + F * 0.8f, 0.2f + F * 0.8f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }
            public void Update() {
                X += VX / 10f;
                Y += VY / 10f;
                F = (F * 0.995f) - 0.002f;
                if (F <= 0.0f) F = 0.0f;
            }
        }
        private readonly List<Frag> lFrag = new List<Frag>();

        // Public
        public bool GameOver { get; private set; }
        public HabitableAO Destination {
            get {
                if (aoTravelTo.AOType == AstronomicalObject.AstronomicalObjectType.Star) return aoTravelTo.GetSystem().GetOutermostPlanet() ?? throw new Exception("Could not fidn suitable planet target for travel");
                else if (aoTravelTo is HabitableAO hao) return hao;
                throw new Exception("Travel Destination is not a valid target");
            }
        }

        public Travel(AstronomicalObject aoFrom, AstronomicalObject aoTo, float fTime, Team team, MapView parent) {
            aoTravelFrom = aoFrom;
            aoTravelTo = aoTo;
            fTravelTime = fTime;
            PlayerTeam = team;
            ParentView = parent;
            bPause = false;
            SetUp();
        }
        public Travel(XmlNode xml, Team team, MapView parent) {
            PlayerTeam = team;
            ParentView = parent;

            aoTravelFrom = team.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(xml.SelectNodeText("AOFrom")) ?? throw new Exception("Could not parse From location for travel node");
            aoTravelTo = team.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(xml.SelectNodeText("AOTo")) ?? throw new Exception("Could not parse To location for travel node");

            fTravelTime = float.Parse(xml.SelectNodeText("Time"));
            fElapsed = float.Parse(xml.SelectNodeText("Elapsed"));
            fMissionElapsed = float.Parse(xml.SelectNodeText("MissionElapsed"));
            fSep = float.Parse(xml.SelectNodeText("Sep"));
            EncounterCount = xml.SelectNodeInt("EncounterCount");
            dtStart = DateTime.FromBinary(long.Parse(xml.SelectNodeText("Start")));
            bSurrendered = (xml.SelectSingleNode("Surrendered") != null);

            SetUp();
            bPause = true;
        }

        private void SetUp() {
            dtStart = Const.dtTime;
            GameOver = false;
        }
        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Travel>");
            file.WriteLine("<AOFrom>" + aoTravelFrom.PrintCoordinates() + "</AOFrom>");
            file.WriteLine("<AOTo>" + aoTravelTo.PrintCoordinates() + "</AOTo>");
            file.WriteLine("<Time>" + fTravelTime + "</Time>");
            file.WriteLine("<Elapsed>" + fElapsed + "</Elapsed>");
            file.WriteLine("<MissionElapsed>" + fMissionElapsed + "</MissionElapsed>");
            file.WriteLine("<EncounterCount>" + EncounterCount + "</EncounterCount>");
            file.WriteLine("<Start>" + dtStart.ToBinary() + "</Start>");
            file.WriteLine("<Sep>" + fSep + "</Sep>");
            if (bSurrendered) file.WriteLine("<Surrendered/>");
            file.WriteLine("</Travel>");
        }

        // Clock tick update methods
        public void ClockTickProcessor() {
            if (bPause) return;
            if (ParentView.msgBox.Active) return;
            if (ParentView.TravelDetails == null) return;
            if (PlayerTeam.CurrentMission != null) {
                ResolveEncounter();
                return;
            }

            // Travelling somewhere
            lShots.Clear();
            if ((rand.NextDouble() * (1 + EncounterCount / 2.0)) < 0.2) {  // Reduce the chance of multiple encounters
                bPause = true;
                PlayerTeam.SetCurrentMission(Encounter.CheckForInterception(aoTravelFrom, aoTravelTo, fTravelTime, PlayerTeam, fElapsed / fTravelTime));
                if (PlayerTeam.CurrentMission != null) {
                    fMissionElapsed = 0.0f;
                    EncounterCount++;
                    PlayerTeam.PlayerShip.InitialiseForBattle();
                    bSurrendered = false;
                    if (PlayerTeam.CurrentMission.Type == Mission.MissionType.BoardingParty) {
                        RunBoardingPartyMission(PlayerTeam.CurrentMission);
                        return;
                    }
                    if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) fSep = 18000.0f;
                    bPause = false;
                    if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Ignore) PlayerTeam.SetCurrentMission(null);
                    return;
                }
                bPause = false;
            }

            // Move forward if we're not currently on a mission
            if (fTravelTime < Const.SecondsPerDay) fElapsed += 60f * 5f;
            else fElapsed += fTravelTime / 600f;
            if (fElapsed >= fTravelTime) {
                fElapsed = fTravelTime;
                bPause = true;
                Const.dtTime = dtStart.AddSeconds(fElapsed);
                ParentView.ArriveAt(aoTravelTo);
                return;
            }
            Const.dtTime = dtStart.AddSeconds(fElapsed);
        }
        private void ResolveEncounter() {
            if (PlayerTeam.CurrentMission?.Type == Mission.MissionType.Salvage) {
                fMissionElapsed += 60f * 10f; 
                if (fMissionElapsed > PlayerTeam.CurrentMission.TimeCost) fMissionElapsed = PlayerTeam.CurrentMission.TimeCost;
                Const.dtTime = dtStart.AddSeconds(fElapsed).AddSeconds(fMissionElapsed);
                if (fMissionElapsed >= PlayerTeam.CurrentMission.TimeCost) {
                    bPause = true;
                    dtStart = dtStart.AddSeconds(PlayerTeam.CurrentMission.TimeCost);
                    if (PlayerTeam.CurrentMission.ShipTarget is null) throw new Exception("ShipTarget is null in Salvage mission");
                    Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission.ShipTarget.GenerateSalvage(false);
                    AnnounceSalvage(dSalvage);
                    dSalvage = PlayerTeam.AddItems(dSalvage);
                    if (Utils.CalculateMass(dSalvage) > 0.0) {
                        ParentView.msgBox.PopupMessage("Your cargo hold is full - couldn't collect all salvage. It has been jettisoned into space.");
                    }
                    PlayerTeam.CeaseMission();
                    bPause = false;
                }
            }
            else if (PlayerTeam.CurrentMission?.Type == Mission.MissionType.Repair) {
                fMissionElapsed += 60f * 10f;
                if (fMissionElapsed > PlayerTeam.CurrentMission.TimeCost) fMissionElapsed = PlayerTeam.CurrentMission.TimeCost;
                Const.dtTime = dtStart.AddSeconds(fElapsed).AddSeconds(fMissionElapsed);
                if (fMissionElapsed >= PlayerTeam.CurrentMission.TimeCost) {
                    bPause = true;
                    dtStart = dtStart.AddSeconds(PlayerTeam.CurrentMission.TimeCost);
                    PlayerTeam.Cash += PlayerTeam.CurrentMission.Reward;
                    ParentView.msgBox.PopupMessage("Repairs completed. You receive a reward of " + PlayerTeam.CurrentMission.Reward + " credits.");
                    PlayerTeam.CeaseMission();
                    bPause = false;
                }
            }
            else if (PlayerTeam.CurrentMission?.Type == Mission.MissionType.ShipCombat) {
                RunBattle();
            }
            else {
                PlayerTeam.CeaseMission();
                throw new Exception("Unexpected Mission Type : " + PlayerTeam.CurrentMission?.Type ?? "none");
            }
        }
        public void ResumeMissionAfterReload() {
            if (PlayerTeam.CurrentMission?.Type == Mission.MissionType.RepelBoarders) {
                RunRepelBoardersMission(PlayerTeam.CurrentMission);
            }
            else if (PlayerTeam.CurrentMission?.Type == Mission.MissionType.BoardingParty) {
                RunBoardingPartyMission(PlayerTeam.CurrentMission);
            }
            else {
                bPause = false;
            }
        }

        // Run a step of a battle between the player ship and an enemy vessel
        private void RunBattle() {
            if (PlayerTeam.CurrentMission?.ShipTarget is null) return;
            Random rand = new Random();
            if (fSep > 4000.0f) fSep -= 10.0f;

            // -- Player ship fires weapons
            foreach (int r in PlayerTeam.PlayerShip.AllWeaponRooms) {
                if (PlayerTeam.PlayerShip.GetEquipmentByRoomID(r) is not ShipWeapon sw) throw new Exception("Got non-weapon in player ship AllWeaponRooms");
                sw.Cooldown -= 0.005;
                if (sw.Cooldown <= 0.0 && sw.Range >= fSep) {
                    int Attack = PlayerTeam.PlayerShip.Attack + sw.Attack;
                    int Defence = PlayerTeam.CurrentMission.ShipTarget.Defence;
                    double Damage = (rand.NextDouble() * Attack) - (rand.NextDouble() * Defence);
                    double dmg = PlayerTeam.CurrentMission.ShipTarget.DamageShip(Damage);
                    sw.Cooldown = sw.Rate + (rand.NextDouble() * 0.1); // Reset cooldown, plus some randomness
                    ShipRoomDesign sT = PlayerTeam.CurrentMission.ShipTarget.PickRandomRoom(rand);
                    lShots.Add(new Shot(true, PlayerTeam.PlayerShip.Type.Rooms[r], sT, sw.Attack, fSep, ParentView.Aspect));
                    for (int n = 0; n <= (int)(dmg * 5.0); n++) {
                        lFrag.Add(new Frag((0.54f + fSep * Const.DrawBattleScale) + ((sT.XPos + sT.Width / 2f) * 0.005f / ParentView.Aspect),
                                           shipYpos + ((sT.YPos - sT.Height / 2f) * 0.005f),
                                           ((float)rand.NextDouble() - 0.5f) * 0.003f,
                                           ((float)rand.NextDouble() - 0.5f) * 0.003f,
                                           (float)rand.NextDouble() * (float)rand.NextDouble()));
                    }
                }
            }

            // Enemy ship destroyed?
            if (PlayerTeam.CurrentMission.ShipTarget.Hull <= 0.0) {
                bPause = true;
                ParentView.msgBox.PopupMessage("The enemy ship has been destroyed");
                Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission.ShipTarget.GenerateSalvage(true);
                AnnounceSalvage(dSalvage);
                dSalvage = PlayerTeam.AddItems(dSalvage);
                if (Utils.CalculateMass(dSalvage) > 0.0) {
                    ParentView.msgBox.PopupMessage("Your cargo hold is full - couldn't collect all the salvage.\nAny excess has been jettisoned into space.");
                }
                PlayerTeam.CeaseMission();
                bPause = false;
                return;
            }

            // Enemy offers to surrender?
            if (!bSurrendered) {
                if ((PlayerTeam.CurrentMission.ShipTarget.HullFract + 0.4 + (rand.NextDouble() * 0.1) < PlayerTeam.PlayerShip.HullFract) || (PlayerTeam.PlayerShip.HullFract > 0.25 && PlayerTeam.CurrentMission.ShipTarget.HullFract < 0.1)) {
                    bSurrendered = true;
                    bPause = true;
                    double dReward = Utils.RoundSF(PlayerTeam.CurrentMission.ShipTarget.EstimatedBountyValue * (rand.NextDouble() + 0.5), 3);
                    string strMessage = String.Format("The enemy ship captain offers to surrender and turn over a bounty of {0} credits.\nAccept his surrender?", dReward);
                    if (MessageBox.Show(strMessage, "Accept Surrender", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        PlayerTeam.Cash += dReward;
                        PlayerTeam.CeaseMission();
                        ParentView.msgBox.PopupMessage("The enemy captain hands over the bounty and flees the battle scene");
                        bPause = false;
                        return;
                    }
                    bPause = false;
                }
            }

            // -- Enemy fires weapons
            foreach (int r in PlayerTeam.CurrentMission.ShipTarget.AllWeaponRooms) {
                ShipWeapon? sw = PlayerTeam.CurrentMission.ShipTarget.GetEquipmentByRoomID(r) as ShipWeapon;
                if (sw is null) throw new Exception("Got non-weapon in attacking ship AllWeaponRooms");
                sw.Cooldown -= 0.005;
                if (sw.Cooldown <= 0.0 && sw.Range >= fSep) {
                    int Attack = PlayerTeam.CurrentMission.ShipTarget.Attack + sw.Attack;
                    int Defence = PlayerTeam.PlayerShip.Defence;
                    double Damage = (rand.NextDouble() * Attack) - (rand.NextDouble() * Defence);
                    double dmg = PlayerTeam.PlayerShip.DamageShip(Damage);
                    sw.Cooldown = sw.Rate + (rand.NextDouble() * 0.1); // Reset cooldown, plus some randomness
                    ShipRoomDesign sT = PlayerTeam.PlayerShip.PickRandomRoom(rand);
                    lShots.Add(new Shot(false, PlayerTeam.CurrentMission.ShipTarget.Type.Rooms[r], sT, sw.Attack, fSep, ParentView.Aspect));
                    for (int n = 0; n <= (int)(dmg * 5.0); n++) {
                        lFrag.Add(new Frag((0.46f - fSep * Const.DrawBattleScale) - ((sT.XPos + sT.Width / 2f) * 0.005f / ParentView.Aspect),
                                           shipYpos + ((sT.YPos - sT.Height / 2f) * 0.005f),
                                           ((float)rand.NextDouble() - 0.5f) * 0.003f,
                                           ((float)rand.NextDouble() - 0.5f) * 0.003f,
                                           (float)rand.NextDouble() * (float)rand.NextDouble()));
                    }
                }
            }

            // Player Ship defeated?
            if (PlayerTeam.PlayerShip.Hull <= 0.0) {
                bPause = true;
                ParentView.msgBox.PopupMessage("Your Ship has been destroyed!");
                PlayerTeam.CeaseMission();
                GameOver = true;
                return;
            }

            // Enemy attempts to board?
            if (fSep <= 4000.0 && PlayerTeam.PlayerShip.HullFract < (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.2) && PlayerTeam.CurrentMission.ShipTarget.HullFract > (Const.DEBUG_MORE_BOARDERS ? 0.2 : 0.3) && rand.NextDouble() > (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.7)) {
                bPause = true;
                ParentView.msgBox.PopupMessage("The enemy are attempting to board your ship. Get ready to repel boarders", TriggerRepelBoardersMissionAction);
            }
        }
        private void TriggerRepelBoardersMissionAction() {
            if (PlayerTeam.CurrentMission is null) return;
            PlayerTeam.SetCurrentMission(Mission.CreateRepelBoardersMission(PlayerTeam.CurrentMission.RacialOpponent, PlayerTeam.CurrentMission.Diff, PlayerTeam.PlayerShip));
            RunRepelBoardersMission(PlayerTeam.CurrentMission);
        }
        private void RunRepelBoardersMission(Mission miss) {
            bPause = true;
            ParentView.RunMission(miss);
        }
        private void RunBoardingPartyMission(Mission miss) {
            bPause = true;
            ParentView.RunMission(miss);
        }
        public void ResumeTravelling() {
            bPause = false;
        }

        // Draw the progress with whatever is happening
        public void Display(ShaderProgram prog, ShaderProgram colprog) {
            Matrix4 projectionM = Matrix4.CreateOrthographicOffCenter(0.0f, 1.0f, 1.0f, 0.0f, -1.0f, 1.0f);
            prog.SetUniform("projection", projectionM);
            prog.SetUniform("view", Matrix4.Identity);
            prog.SetUniform("model", Matrix4.Identity);
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            GL.Disable(EnableCap.DepthTest);

            // Draw a large box in the centre of the screen
            Matrix4 translateM = Matrix4.CreateTranslation(0.2f, 0.2f, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(0.6f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            prog.SetUniform("flatColour", new Vector4(0f, 0f, 0f, 1.0f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1.0f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            //  Task specific stuff
            if (PlayerTeam.CurrentMission == null) DrawTravelProgress(prog);
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Repair) DrawRepair(prog);
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Salvage) DrawSalvage(prog);
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) DrawBattle(prog, colprog);
        }
        private void DrawTravelProgress(ShaderProgram prog) {
            // Set up the text
            string strDist = Utils.PrintDistance(AstronomicalObject.CalculateDistance(aoTravelFrom, aoTravelTo));
            string strTime = string.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(fTravelTime));
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = ParentView.Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.55f,
                ZPos = 0.015f,
                Scale = 0.05f
            };
            TextRenderer.DrawWithOptions(strDist + " " + strTime, tro);

            // Progress Bar
            float fract = (float)fElapsed / (float)fTravelTime;
            GraphicsFunctions.DrawFramedFractBar(prog, 0.3f, 0.4f, 0.4f, 0.05f, fract, new Vector4(0f, 1f, 0f, 1f));

            // Source and Target AOs
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            Matrix4 translateM = Matrix4.CreateTranslation(0.25f, 0.42f, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(1f / ParentView.Aspect, 1f, 1f);
            prog.SetUniform("view", scaleM * translateM);
            aoTravelFrom.DrawSelected(prog, 8);
            translateM = Matrix4.CreateTranslation(0.75f, 0.42f, 0f);
            prog.SetUniform("view", scaleM * translateM);
            aoTravelTo.DrawSelected(prog, 8);
        }
        private void DrawSalvage(ShaderProgram prog) {
            if (PlayerTeam.CurrentMission is null) return;
            // Set up the text
            string strTime = string.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(PlayerTeam.CurrentMission.TimeCost));
            string strText = "Salvaging " + strTime;
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = ParentView.Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.55f,
                ZPos = 0.015f,
                Scale = 0.05f
            };
            TextRenderer.DrawWithOptions(strText, tro);            
            // Progress Bar
            float fract = fMissionElapsed / PlayerTeam.CurrentMission.TimeCost;
            GraphicsFunctions.DrawFramedFractBar(prog, 0.3f, 0.4f, 0.4f, 0.05f, fract, new Vector4(1f, 0f, 0f, 1f));
        }
        private void DrawRepair(ShaderProgram prog) {
            if (PlayerTeam.CurrentMission is null) return;
            // Set up the text
            string strTime = String.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(PlayerTeam.CurrentMission.TimeCost));
            string strText = "Repairing " + strTime;
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = ParentView.Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.55f,
                ZPos = 0.015f,
                Scale = 0.05f
            };
            TextRenderer.DrawWithOptions(strText, tro);
            // Progress Bar
            float fract = fMissionElapsed / PlayerTeam.CurrentMission.TimeCost;
            GraphicsFunctions.DrawFramedFractBar(prog, 0.3f, 0.4f, 0.4f, 0.05f, fract, new Vector4(0f, 0f, 1f, 1f));
        }
        private void DrawBattle(ShaderProgram prog, ShaderProgram colprog) {
            if (PlayerTeam.PlayerShip is null) throw new Exception("Battle without Player Team Ship");
            if (PlayerTeam.CurrentMission?.ShipTarget is null) throw new Exception("Battle without Enemy Ship");
            string strText = "Battle versus unidentified alien ship";
            if (PlayerTeam.CurrentMission?.RacialOpponent is not null) strText = $"Battle versus {PlayerTeam.CurrentMission.RacialOpponent.Name} ship";
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.TopMiddle,
                Aspect = ParentView.Aspect,
                TextColour = Color.White,
                XPos = 0.5f,
                YPos = 0.65f,
                ZPos = 0.015f,
                Scale = 0.05f
            };
            TextRenderer.DrawWithOptions(strText, tro);

            // Setup and draw status text
            // Player Ship Stats:
            string strStatus = Math.Round(PlayerTeam.PlayerShip.Hull, 1) + "/" + Math.Round(PlayerTeam.PlayerShip.Type.MaxHull, 0);
            GraphicsFunctions.DrawFramedFractBar(prog, 0.22f, 0.35f, 0.1f, 0.02f, PlayerTeam.PlayerShip.HullFract, new Vector4(0f, 1f, 0f, 1f));
            tro.Scale = 0.04f;
            tro.XPos = 0.27f;
            tro.YPos = 0.37f;
            tro.Alignment = Alignment.TopMiddle;
            TextRenderer.DrawWithOptions(strStatus, tro);

            if (PlayerTeam.PlayerShip.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.PlayerShip.Shield, 1) + "/" + PlayerTeam.PlayerShip.MaxShield;
                GraphicsFunctions.DrawFramedFractBar(prog, 0.22f, 0.42f, 0.1f, 0.02f, (float)PlayerTeam.PlayerShip.ShieldFract, new Vector4(0f, 0f, 1f, 1f));
                tro.YPos = 0.41f;
                TextRenderer.DrawWithOptions(strStatus, tro);
            }

            // Enemy Ship Stats:
            double stHull = Math.Max(0.0, Math.Round(PlayerTeam.CurrentMission!.ShipTarget.Hull, 1));
            strStatus = stHull + "/" + Math.Round(PlayerTeam.CurrentMission.ShipTarget.Type.MaxHull, 0);
            GraphicsFunctions.DrawFramedFractBar(prog, 0.68f, 0.35f, 0.1f, 0.02f, PlayerTeam.CurrentMission.ShipTarget.HullFract, new Vector4(0f, 1f, 0f, 1f));
            tro.Scale = 0.04f;
            tro.XPos = 0.73f;
            tro.YPos = 0.37f;
            tro.Alignment = Alignment.TopMiddle;
            TextRenderer.DrawWithOptions(strStatus, tro);
            if (PlayerTeam.CurrentMission.ShipTarget.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.CurrentMission.ShipTarget.Shield, 1) + "/" + PlayerTeam.CurrentMission.ShipTarget.MaxShield;
                GraphicsFunctions.DrawFramedFractBar(prog, 0.68f, 0.42f, 0.1f, 0.02f, (float)PlayerTeam.CurrentMission.ShipTarget.ShieldFract, new Vector4(0f, 0f, 1f, 1f));
                tro.YPos = 0.41f;
                TextRenderer.DrawWithOptions(strStatus, tro);
            }

            // Display ships
            float fSpace = fSep * Const.DrawBattleScale;
            Matrix4 translateM = Matrix4.CreateTranslation(0.46f - fSpace, shipYpos, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(-0.005f / ParentView.Aspect, 0.005f, 0.005f);
            prog.SetUniform("view", scaleM * translateM);
            PlayerTeam.PlayerShip.DrawBattle(prog);
            translateM = Matrix4.CreateTranslation(0.54f + fSpace, shipYpos, 0f);
            scaleM = Matrix4.CreateScale(0.005f / ParentView.Aspect, 0.005f, 0.005f);
            prog.SetUniform("view", scaleM * translateM);
            PlayerTeam.CurrentMission.ShipTarget.DrawBattle(prog);

            // Draw weapon shots (Blue = player, red = enemy, width = power) and Frag
            UpdateShotLines();
            if (vaShot is not null && vbShot is not null && vbShot.VertexCount > 0 && lShots.Any()) {
                GL.UseProgram(colprog.ShaderProgramHandle);
                GL.BindVertexArray(vaShot.VertexArrayHandle);
                GL.DrawArrays(PrimitiveType.Lines, 0, vbShot.VertexCount);
                GL.BindVertexArray(0);
            }

            // Draw any frag items
            prog.SetUniform("view", Matrix4.Identity);
            foreach (Frag fr in lFrag) {
                fr.Display(prog, ParentView.Aspect);
                fr.Update();
            }
            lFrag.RemoveAll(f => f.F <= 0.0f);
        }
        private void UpdateShotLines() {
            if (!lShots.Any()) return;
            List<VertexPos2DCol> lines = new List<VertexPos2DCol>();
            foreach (Shot sh in lShots) {
                if (sh.Life <= 0) continue;
                float fract = (float)sh.Life / 20f;
                if (fract > 1.0f) fract = 1.0f;
                Color4 col = sh.Player ? ShotColPlayer : ShotColEnemy;
                col.R *= fract;
                col.G *= fract;
                col.B *= fract;
                lines.Add(new VertexPos2DCol(new Vector2(sh.fx, sh.fy), col));
                lines.Add(new VertexPos2DCol(new Vector2(sh.tx, sh.ty), col));
                sh.Update();
            }
            lShots.RemoveAll(s => s.Life <= 0);
            if (vbShot is null) {
                vbShot = new VertexBuffer(lines.ToArray(), BufferUsageHint.StreamDraw);
                vaShot = new VertexArray(vbShot);
            }
            else vbShot.SetData(lines.ToArray());
        }

        // Announce the salvage gained 
        private void AnnounceSalvage(Dictionary<IItem, int> dSalvage) {
            ParentView.msgBox.PopupMessage("Got " + dSalvage.Count + " item(s) of salvage");
        }

        public void Pause() {
            bPause = true;
        }
        public void Unpause() {
            bPause = false;
        }
    }
}
