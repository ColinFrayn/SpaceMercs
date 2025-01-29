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
        private float fSep; // Separation between two ships in battle
        private float yLeft = 0.0f, yRight = 0.0f;
        private float vLeft = 0f, vRight = 0f;
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
        private const float ShipYPos = 0.5f;
        private const float ShipYRange = 3000.0f;
        private const float ShipVMax = 50.0f;
        private const float ShipYProximity = 40.0f;
        private const float ShipYTargetSep = 1000.0f;
        private const float YScale = 4f;
        private const float RangeAccel = 0.8f;
        private const float ChaseAccel = 0.4f;


        private class Shot {
            public int Attack { get; private set; }
            public int Life { get; private set; }
            public Color4 Colour { get; private set; }
            private readonly float fx, fy, tx, ty;

            public Shot(Color4 col, int att, float fromx, float tox, float fromy, float toy) {
                Attack = att;
                Life = (att * 3) + 18;
                Colour = col;
                fx = fromx;
                tx = tox;
                fy = fromy;
                ty = toy;
            }
            public void Update() {
                Life--;
            }
            public Vector2 From { get { return new Vector2(fx, fy); } }
            public Vector2 To { get { return new Vector2(tx, ty); } }
        }
        private readonly List<Shot> lShots = new List<Shot>();

        private class Frag {
            private readonly float VX, VY;
            private float X, Y;
            private float F;

            public Frag(float x, float y, Random rand) {
                X = x;
                Y = y;
                VX = ((float)rand.NextDouble() - 0.5f) * 0.003f;
                VY = ((float)rand.NextDouble() - 0.5f) * 0.003f;
                F = (float)rand.NextDouble() * (float)rand.NextDouble();
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
                X += VX / 8f;
                Y += VY / 8f;
                F = (F * 0.995f) - 0.002f;
                if (F <= 0.0f) F = 0.0f;
            }
            public bool Faded => (F <= 0.0f);
        }
        private readonly List<Frag> lFrag = new List<Frag>();

        // Public
        public bool GameOver { get; private set; }
        public AstronomicalObject Destination {
            get {
                if (aoTravelTo is Star st) return st.GetOutermostPlanet() ?? throw new Exception("Could not find suitable planet target for travel");
                else if (aoTravelTo is HabitableAO hao) return hao;
                else if (aoTravelTo is HyperGate hg) return hg;
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
            string yLeftNode = xml.SelectNodeText("YLeft");
            if (!string.IsNullOrEmpty(yLeftNode)) yLeft = float.Parse(yLeftNode);
            string yRightNode = xml.SelectNodeText("YRight");
            if (!string.IsNullOrEmpty(yRightNode)) yRight = float.Parse(yRightNode);
            string vLeftNode = xml.SelectNodeText("VLeft");
            if (!string.IsNullOrEmpty(vLeftNode)) vLeft = float.Parse(vLeftNode);
            string vRightNode = xml.SelectNodeText("VRight");
            if (!string.IsNullOrEmpty(vRightNode)) vRight = float.Parse(vRightNode);
            fSep = float.Parse(xml.SelectNodeText("Sep"));
            EncounterCount = xml.SelectNodeInt("EncounterCount");
            dtStart = DateTime.FromBinary(long.Parse(xml.SelectNodeText("Start")));
            bSurrendered = (xml.SelectSingleNode("Surrendered") != null);

            SetUp();
            bPause = true;
        }

        private void SetUp() {
            dtStart = Const.dtTime;
            lFrag.Clear();
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
            file.WriteLine("<YLeft>" + yLeft + "</YLeft>");
            file.WriteLine("<YRight>" + yRight + "</YRight>");
            file.WriteLine("<VLeft>" + vLeft + "</VLeft>");
            file.WriteLine("<VRight>" + vRight + "</VRight>");
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

            // Travelling somewhere - check for an Encounter
            if ((rand.NextDouble() * (1 + EncounterCount)) < Const.EncounterFreqScale) {  // Reduce the chance of multiple encounters
                Mission? foundMission = Encounter.CheckForInterception(aoTravelFrom, aoTravelTo, fTravelTime, PlayerTeam, fElapsed / fTravelTime, ParentView.msgBox.PopupMessage);
                if (foundMission != null) {
                    EncounterCount++;
                    if (foundMission.Type == Mission.MissionType.Ignore) return;
                    bPause = true;
                    PlayerTeam.SetCurrentMission(foundMission);
                    if (PlayerTeam.CurrentMission!.Type == Mission.MissionType.BoardingParty) {
                        RunBoardingPartyMission(PlayerTeam.CurrentMission);
                        return;
                    }
                    fMissionElapsed = 0.0f;
                    if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) {
                        lShots.Clear();
                        lFrag.Clear();
                        PlayerTeam.PlayerShip.InitialiseForBattle();
                        bSurrendered = false;
                        fSep = 18000.0f;
                    }
                    else {
                        // Could be e.g. Salvage/Repair
                    }
                    bPause = false;
                    return;
                }
                bPause = false;
            }

            // Move forward if we're not currently on a mission
            if (aoTravelFrom.GetSystem() == aoTravelTo.GetSystem()) fElapsed += 60f * 5f;
            else fElapsed += (float)Const.SecondsPerDay * 2;

            ParentView.UpdateCurrentTime(dtStart.AddSeconds(Math.Min(fElapsed,fTravelTime)));

            if (fElapsed >= fTravelTime) {
                fElapsed = fTravelTime;
                bPause = true;
                ParentView.ArriveAt(Destination);
                if (PlayerTeam.PlayerShip.CanRepair) PlayerTeam.PlayerShip.RepairHull();
                return;
            }
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
                    Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission.ShipTarget.GenerateSalvage(false, PlayerTeam.CurrentMission?.RacialOpponent);
                    AnnounceSalvage(dSalvage);
                    Dictionary<IItem, int> dRemains = PlayerTeam.AddItems(dSalvage);
                    if (Utils.CalculateMass(dRemains) > 0.0) {
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
            if (fSep > 4000.0f) fSep -= 10.0f;
            float fDist = (float)Math.Sqrt((double)(fSep * fSep + (yLeft - yRight) * (yLeft - yRight)));
            float fSpace = fSep * Const.DrawBattleScale;

            MoveShips();

            // If the player ship is still alive...
            if ((PlayerTeam.CurrentMission?.ShipTarget?.Hull ?? 0.0) > 0.0) {
                RunPlayerShipTurn(fDist, fSpace);
            }

            // Was the enemy ship destroyed?
            if ((PlayerTeam.CurrentMission?.ShipTarget?.Hull ?? 0.0) <= 0.0) {
                if (lFrag.Any()) return;
                bPause = true;
                ParentView.msgBox.PopupMessage("The enemy ship has been destroyed");
                Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission!.ShipTarget.GenerateSalvage(true, PlayerTeam.CurrentMission?.RacialOpponent);
                AnnounceSalvage(dSalvage);
                Dictionary<IItem, int> dRemains = PlayerTeam.AddItems(dSalvage);
                if (Utils.CalculateMass(dRemains) > 0.0) {
                    ParentView.msgBox.PopupMessage("Your cargo hold is full - you couldn't collect all the salvage.\nAny excess has been jettisoned into space.");
                }
                Race? ra = PlayerTeam.CurrentMission?.RacialOpponent;
                if (ra is not null && !ra.Known) {
                    ParentView.msgBox.PopupMessage($"You pick apart the wreckage of the alien ship and identify individuals of a previously unknown alien race!\nBased on their appearance and technology, you name them {ra.Name}");
                    ra.SetAsKnownBy(PlayerTeam);
                }
                PlayerTeam.CeaseMission();
                bPause = false;
                return;
            }

            if (PlayerTeam.CurrentMission is null) return;

            // Enemy is still alive, so check if it can shoot
            RunEnemyShipTurn(fDist, fSpace);

            // Was the player Ship destroyed?
            if (PlayerTeam.PlayerShip.Hull <= 0.0) {
                bPause = true;
                ParentView.msgBox.PopupMessage("Your Ship has been destroyed!");
                PlayerTeam.CeaseMission();
                GameOver = true;
                return;
            }

            // Enemy attempts to board?
            if (fDist <= 4000.0 &&
                PlayerTeam.PlayerShip.HullFract < (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.2) &&
                PlayerTeam.CurrentMission.ShipTarget.HullFract > (Const.DEBUG_MORE_BOARDERS ? 0.2 : 0.3) &&
                rand.NextDouble() > (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.7)) {
                bPause = true;
                ParentView.msgBox.PopupMessage("The enemy are attempting to board your ship. Get ready to repel boarders", TriggerRepelBoardersMissionAction);
            }

            // Update both ships
            PlayerTeam.PlayerShip.BattleUpdate();
            PlayerTeam.CurrentMission.ShipTarget.BattleUpdate();
        }
        private void RunPlayerShipTurn(float fDist, float fSpace) {
            // Enemy offers to surrender?
            if (!bSurrendered && PlayerTeam!.CurrentMission!.ShipTarget!.HullFract < 0.35 && rand.NextDouble() < 0.1) {
                if (PlayerTeam.PlayerShip.HullFract > 0.7 || // You're in a much better condition than them
                    (PlayerTeam.PlayerShip.HullFract > 0.25 && PlayerTeam.CurrentMission.ShipTarget.HullFract < 0.1)) { // You're in a slightly better condition, but they're nearly dead
                    bSurrendered = true;
                    bPause = true;
                    double dReward = Utils.RoundSF(PlayerTeam.CurrentMission.ShipTarget.EstimatedBountyValue * (rand.NextDouble() + 3.5) / 5.0, 3);
                    Race? ra = PlayerTeam.CurrentMission?.RacialOpponent;
                    if (ra is not null && !ra.Known) {
                        string strMessage = $"The ship appears to surrender, and the crew identify themselves as individuals of a previously unknown alien race!\nThey claim the name of their species is {ra.Name}";
                        ra.SetAsKnownBy(PlayerTeam);
                        if (MessageBox.Show(strMessage, "Accept Surrender", MessageBoxButtons.YesNo) == DialogResult.Yes) { // REPLACE WITH msgBox
                            PlayerTeam.CeaseMission();
                            ParentView.msgBox.PopupMessage("The enemy ship flees the battle scene");
                            bPause = false;
                            return;
                        }
                    }
                    else {
                        string strMessage = string.Format("The enemy ship captain offers to surrender and turn over a bounty of {0} credits.\nAccept his surrender?", dReward);
                        if (MessageBox.Show(strMessage, "Accept Surrender", MessageBoxButtons.YesNo) == DialogResult.Yes) { // REPLACE WITH msgBox
                            PlayerTeam.Cash += dReward;
                            PlayerTeam.CeaseMission();
                            ParentView.msgBox.PopupMessage("The enemy captain hands over the bounty and flees the battle scene");
                            bPause = false;
                            return;
                        }
                    }
                    bPause = false;
                }
            }

            // -- Player ship fires weapons
            foreach (int r in PlayerTeam.PlayerShip.AllWeaponRooms) {
                if (PlayerTeam.PlayerShip.GetEquipmentByRoomID(r) is not ShipWeapon sw) throw new Exception("Got non-weapon in player ship AllWeaponRooms");
                sw.Cooldown -= 0.005;
                if (sw.Cooldown <= 0.0 && sw.Range >= fDist) {
                    double dmg = sw.FireWeapon(PlayerTeam.PlayerShip, PlayerTeam.CurrentMission!.ShipTarget, rand);
                    int nfrag = PlayerTeam!.CurrentMission!.ShipTarget!.Hull > 0 ? 1 : 10;
                    for (int i = 0; i < nfrag; i++) {
                        ShipRoomDesign rF = PlayerTeam.PlayerShip.Type.Rooms[r];
                        ShipRoomDesign rT = PlayerTeam.CurrentMission.ShipTarget.PickRandomRoom(rand);
                        float fx = (0.46f - fSpace) - ((rF.XPos + rF.Width / 2f) * 0.005f / ParentView.Aspect);
                        float tx = (0.54f + fSpace) + ((rT.XPos + rT.Width / 2f) * 0.005f / ParentView.Aspect);
                        float fy = ShipYPos + ((rF.YPos - rF.Height / 2f) * 0.005f) + (Const.DrawBattleScale * yLeft * YScale);
                        float ty = ShipYPos + ((rT.YPos - rT.Height / 2f) * 0.005f) + (Const.DrawBattleScale * yRight * YScale);
                        if (i == 0) lShots.Add(new Shot(ShotColPlayer, sw.Attack, fx, tx, fy, ty));
                        for (int n = 0; n <= 3 + (int)(dmg * 2.0); n++) {
                            lFrag.Add(new Frag(tx, ty, rand));
                        }
                    }
                }
            }
        }
        private void RunEnemyShipTurn(float fDist, float fSpace) {
            // -- Enemy fires weapons
            foreach (int r in PlayerTeam!.CurrentMission!.ShipTarget!.AllWeaponRooms) {
                ShipWeapon? sw = PlayerTeam.CurrentMission.ShipTarget.GetEquipmentByRoomID(r) as ShipWeapon;
                if (sw is null) throw new Exception("Got non-weapon in attacking ship AllWeaponRooms");
                sw.Cooldown -= 0.005;
                if (sw.Cooldown <= 0.0 && sw.Range >= fDist) {
                    double dmg = sw.FireWeapon(PlayerTeam.CurrentMission.ShipTarget, PlayerTeam.PlayerShip, rand);
                    int nfrag = PlayerTeam.PlayerShip.Hull > 0 ? 1 : 10;
                    for (int i = 0; i < nfrag; i++) {
                        ShipRoomDesign rF = PlayerTeam.CurrentMission.ShipTarget.Type.Rooms[r];
                        ShipRoomDesign rT = PlayerTeam.PlayerShip.PickRandomRoom(rand);
                        float fx = (0.54f + fSpace) + ((rF.XPos + rF.Width / 2f) * 0.005f / ParentView.Aspect);
                        float tx = (0.46f - fSpace) - ((rT.XPos + rT.Width / 2f) * 0.005f / ParentView.Aspect);
                        float fy = ShipYPos + ((rF.YPos - rF.Height / 2f) * 0.005f) + (Const.DrawBattleScale * yRight * YScale);
                        float ty = ShipYPos + ((rT.YPos - rT.Height / 2f) * 0.005f) + (Const.DrawBattleScale * yLeft * YScale);
                        if (i == 0) lShots.Add(new Shot(ShotColEnemy, sw.Attack, fx, tx, fy, ty));
                        for (int n = 0; n <= 3 + (int)(dmg * 2.0); n++) {
                            lFrag.Add(new Frag(tx, ty, rand));
                        }
                    }
                }
            }
        }
        private void MoveShips() {
            bool playerHasBetterRange = PlayerTeam.PlayerShip.WeaponRange > PlayerTeam.CurrentMission!.ShipTarget!.WeaponRange;

            // Player ship moves
            if (playerHasBetterRange) {
                if (Math.Abs(yLeft - yRight) < ShipYProximity) {
                    if (Math.Abs(yLeft) * 2 > ShipYRange) vLeft += (yLeft > 0) ? -RangeAccel : RangeAccel;
                    else vLeft += ((float)rand.NextDouble() * 6.0f * RangeAccel) - (3.0f * RangeAccel);
                }
                else if (Math.Abs(yLeft - yRight) < ShipYTargetSep) {
                    vLeft += ((float)rand.NextDouble() * 6.0f * RangeAccel) - (3.0f * RangeAccel);
                    //if (yRight > yLeft) vLeft -= RangeAccel;
                    //else vLeft += RangeAccel;
                }
                else vLeft *= 0.9f;
            }
            else {
                if (Math.Abs(yLeft - yRight) < ShipYProximity) {
                    if (Math.Abs(yLeft) * 2 > ShipYRange) vLeft -= (yLeft > 0) ? -ChaseAccel : ChaseAccel;
                    else vLeft += ((float)rand.NextDouble() * 6.0f * ChaseAccel) - (3.0f * ChaseAccel);
                }
                else if (yRight > yLeft) vLeft += ChaseAccel;
                else vLeft -= ChaseAccel;
            }
            if (vLeft > ShipVMax) vLeft = ShipVMax;
            else if (vLeft < -ShipVMax) vLeft = -ShipVMax;
            yLeft += vLeft;
            if (yLeft > ShipYRange) yLeft = ShipYRange;
            if (yLeft < -ShipYRange) yLeft = -ShipYRange;

            // Enemy Ship
            if (playerHasBetterRange) {
                if (Math.Abs(yLeft - yRight) < ShipYProximity) {
                    if (Math.Abs(yRight) * 2 > ShipYRange) vRight += (vRight > 0) ? -ChaseAccel : ChaseAccel;
                    else vRight += ((float)rand.NextDouble() * 6.0f * ChaseAccel) - (3.0f * ChaseAccel);
                }
                else if (yRight > yLeft) vRight -= ChaseAccel;
                else vRight += ChaseAccel;
            }
            else {
                if (Math.Abs(yLeft - yRight) < ShipYProximity) {
                    if (Math.Abs(yRight) * 2 > ShipYRange) vRight -= (vRight > 0) ? -RangeAccel : RangeAccel;
                    else vRight += ((float)rand.NextDouble() * 6.0f * RangeAccel) - (3.0f * RangeAccel);
                }
                else if (Math.Abs(yLeft - yRight) < ShipYTargetSep) {
                    vRight += ((float)rand.NextDouble() * 6.0f * RangeAccel) - (3.0f * RangeAccel);
                    //if (yRight > yLeft) vRight += RangeAccel;
                    //else vRight -= RangeAccel;
                }
                else vRight *= 0.9f;
            }
            if (vRight > ShipVMax) vRight = ShipVMax;
            else if (vRight < -ShipVMax) vRight = -ShipVMax;
            yRight += vRight;
            if (yRight > ShipYRange) yRight = ShipYRange;
            if (yRight < -ShipYRange) yRight = -ShipYRange;
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
            Matrix4 starScaleM = Matrix4.CreateScale(0.1f, 0.1f, 0.1f);
            prog.SetUniform("view", (aoTravelFrom is Star ? starScaleM : Matrix4.Identity) * scaleM * translateM);
            aoTravelFrom.DrawSelected(prog, 8);
            translateM = Matrix4.CreateTranslation(0.75f, 0.42f, 0f);
            prog.SetUniform("view", (aoTravelTo is Star ? starScaleM : Matrix4.Identity) * scaleM * translateM);
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
            if (PlayerTeam.CurrentMission?.RacialOpponent is not null && PlayerTeam.CurrentMission.RacialOpponent.Known) strText = $"Battle versus {PlayerTeam.CurrentMission.RacialOpponent.Name} ship";
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
            GraphicsFunctions.DrawFramedFractBar(prog, 0.22f, 0.25f, 0.1f, 0.02f, PlayerTeam.PlayerShip.HullFract, new Vector4(0f, 1f, 0f, 1f));
            tro.Scale = 0.04f;
            tro.XPos = 0.27f;
            tro.YPos = 0.27f;
            tro.Alignment = Alignment.TopMiddle;
            TextRenderer.DrawWithOptions(strStatus, tro);
            if (PlayerTeam.PlayerShip.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.PlayerShip.Shield, 1) + "/" + PlayerTeam.PlayerShip.MaxShield;
                GraphicsFunctions.DrawFramedFractBar(prog, 0.22f, 0.32f, 0.1f, 0.02f, (float)PlayerTeam.PlayerShip.ShieldFract, new Vector4(0f, 0f, 1f, 1f));
                tro.YPos = 0.34f;
                TextRenderer.DrawWithOptions(strStatus, tro);
            }

            // Enemy Ship Stats:
            double stHull = Math.Max(0.0, Math.Round(PlayerTeam.CurrentMission!.ShipTarget.Hull, 1));
            strStatus = stHull + "/" + Math.Round(PlayerTeam.CurrentMission.ShipTarget.Type.MaxHull, 0);
            GraphicsFunctions.DrawFramedFractBar(prog, 0.68f, 0.25f, 0.1f, 0.02f, PlayerTeam.CurrentMission.ShipTarget.HullFract, new Vector4(0f, 1f, 0f, 1f));
            tro.Scale = 0.04f;
            tro.XPos = 0.73f;
            tro.YPos = 0.27f;
            tro.Alignment = Alignment.TopMiddle;
            TextRenderer.DrawWithOptions(strStatus, tro);
            if (PlayerTeam.CurrentMission.ShipTarget.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.CurrentMission.ShipTarget.Shield, 1) + "/" + PlayerTeam.CurrentMission.ShipTarget.MaxShield;
                GraphicsFunctions.DrawFramedFractBar(prog, 0.68f, 0.32f, 0.1f, 0.02f, (float)PlayerTeam.CurrentMission.ShipTarget.ShieldFract, new Vector4(0f, 0f, 1f, 1f));
                tro.YPos = 0.34f;
                TextRenderer.DrawWithOptions(strStatus, tro);
            }

            // -- Display ships
            float fSpace = fSep * Const.DrawBattleScale;

            // Player Ship
            Matrix4 translateM = Matrix4.CreateTranslation(0.46f - fSpace, ShipYPos + yLeft * Const.DrawBattleScale * YScale, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(-0.005f / ParentView.Aspect, 0.005f, 0.005f);
            Matrix4 rotateM = Matrix4.CreateRotationZ(-(float)Math.Atan((yRight - yLeft) / (fSep * YScale)));
            prog.SetUniform("view", rotateM * scaleM * translateM);
            if (PlayerTeam.PlayerShip.Hull > 0) PlayerTeam.PlayerShip.DrawBattle(prog);
            // Enemy Ship
            translateM = Matrix4.CreateTranslation(0.54f + fSpace, ShipYPos + yRight * Const.DrawBattleScale * YScale, 0f);
            scaleM = Matrix4.CreateScale(0.005f / ParentView.Aspect, 0.005f, 0.005f);
            rotateM = Matrix4.CreateRotationZ((float)Math.Atan((yRight - yLeft) / (fSep * YScale)));
            prog.SetUniform("view", rotateM * scaleM * translateM);
            if (PlayerTeam.CurrentMission.ShipTarget.Hull > 0) PlayerTeam.CurrentMission.ShipTarget.DrawBattle(prog);

            // Draw weapon shots (Blue = player, red = enemy, width = power) and Frag
            UpdateShotLines();
            if (vaShot is not null && vbShot is not null && vbShot.VertexCount > 0 && lShots.Any()) {
                GL.UseProgram(colprog.ShaderProgramHandle);
                GL.BindVertexArray(vaShot.VertexArrayHandle);
                GL.DrawArrays(PrimitiveType.Lines, 0, vbShot.VertexCount);
                GL.BindVertexArray(0);
            }

            // Draw & update any frag items
            prog.SetUniform("view", Matrix4.Identity);
            foreach (Frag fr in lFrag) {
                fr.Display(prog, ParentView.Aspect);
                fr.Update();
            }
            lFrag.RemoveAll(f => f.Faded);
        }
        private void UpdateShotLines() {
            if (!lShots.Any()) return;
            List<VertexPos2DCol> lines = new List<VertexPos2DCol>();
            foreach (Shot sh in lShots) {
                if (sh.Life <= 0) continue;
                float fract = sh.Life / 20f;
                if (fract > 1.0f) fract = 1.0f;
                Color4 col = sh.Colour;
                col.R *= fract;
                col.G *= fract;
                col.B *= fract;
                lines.Add(new VertexPos2DCol(sh.From, col));
                lines.Add(new VertexPos2DCol(sh.To, col));
                sh.Update();
            }
            lShots.RemoveAll(s => s.Life <= 0);

            // Display the shots that are still valid
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
