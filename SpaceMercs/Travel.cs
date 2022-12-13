using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using SpaceMercs.MainWindow;
using System.IO;
using System.Xml;
using Timer = System.Windows.Forms.Timer;

namespace SpaceMercs {
    // Deal with travelling between AstronomicalObjects
    class Travel {
        private readonly AstronomicalObject aoTravelFrom, aoTravelTo;
        private readonly double dTravelTime;
        private double dMissionElapsed, dElapsed, dDist;
        private double dSep; // Separation between two ships
        private bool bSurrendered = false;
        private readonly Team PlayerTeam;
        private Timer clockTick;
        private readonly MapView ParentView;
        private int EncounterCount = 0;
        private DateTime dtStart;
        private readonly Random rand = new Random();

        private class Shot {
            public bool Player { get; private set; }
            public int Attack { get; private set; }
            public int Life { get; private set; }
            private readonly double fx, fy, tx, ty;
            public Shot(bool pl, ShipRoomDesign rf, ShipRoomDesign rt, int att, double dSep, double aspect) {
                Player = pl;
                Attack = att;
                Life = (att / 2) + 2;
                double dSpace = dSep * Const.DrawBattleScale;

                if (Player) {
                    fx = (0.46 - dSpace) - ((rf.XPos + rf.Width / 2) * 0.005 / aspect);
                    tx = (0.54 + dSpace) + ((rt.XPos + rt.Width / 2) * 0.005 / aspect);
                }
                else {
                    fx = (0.54 + dSpace) + ((rf.XPos + rf.Width / 2) * 0.005 / aspect);
                    tx = (0.46 - dSpace) - ((rt.XPos + rf.Width / 2) * 0.005 / aspect);
                }
                fy = 0.58 + ((rf.YPos - rf.Height / 2) * 0.005);
                ty = 0.58 + ((rt.YPos - rf.Height / 2) * 0.005);
            }
            public void Display() {
                if (Life <= 0) return;
                GL.LineWidth(Attack);
                if (Player) GL.Color3(0.1, 0.3, 1.0);
                else GL.Color3(1.0, 0.3, 0.1);
                GL.Vertex3(fx, fy, 0.0);
                GL.Vertex3(tx, ty, 0.0);
                Life--;
            }
        }
        private readonly List<Shot> lShots = new List<Shot>();

        private class Frag {
            private double X, Y;
            private readonly double VX, VY;
            public double F { get; private set; }
            public Frag(double x, double y, double vx, double vy, double f) {
                X = x;
                Y = y;
                VX = vx;
                VY = vy;
                F = f;
            }
            public void Display(double aspect) {
                GL.Color3(0.2 + F * 0.8, 0.2 + F * 0.8, 0.2 + F * 0.8);
                GL.Vertex3(X - 0.0015 / aspect, Y - 0.0015, 0.0);
                GL.Vertex3(X + 0.0015 / aspect, Y - 0.0015, 0.0);
                GL.Vertex3(X + 0.0015 / aspect, Y + 0.0015, 0.0);
                GL.Vertex3(X - 0.0015 / aspect, Y + 0.0015, 0.0);
                X += VX;
                Y += VY;
                F = (F * 0.98) - 0.01;
                if (F <= 0.0) F = 0.0;
            }
        }
        private readonly List<Frag> lFrag = new List<Frag>();

        // Public
        public bool GameOver { get; private set; }

        // Getters
        private double dTickStep {
            get {
                if (dTravelTime < Const.SecondsPerDay) return 3600.0; // One hour
                else return dTravelTime / 24.0;
            }
        }
        public HabitableAO Destination {
            get {
                if (aoTravelTo.AOType == AstronomicalObject.AstronomicalObjectType.Star) return aoTravelTo.GetSystem().GetOutermostPlanet();
                else return (HabitableAO)aoTravelTo;
            }
        }

        public Travel(AstronomicalObject aoFrom, AstronomicalObject aoTo, double dTime, Team team, MapView parent) {
            aoTravelFrom = aoFrom;
            aoTravelTo = aoTo;
            dTravelTime = dTime;
            PlayerTeam = team;
            ParentView = parent;
            SetUp();
        }
        public Travel(XmlNode xml, Team team, MapView parent) {
            PlayerTeam = team;
            ParentView = parent;

            aoTravelFrom = team.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(xml.SelectSingleNode("AOFrom").InnerText);
            aoTravelTo = team.CurrentPosition.GetSystem().Sector.ParentMap.GetAOFromLocationString(xml.SelectSingleNode("AOTo").InnerText);

            dTravelTime = Double.Parse(xml.SelectSingleNode("Time").InnerText);
            dElapsed = Double.Parse(xml.SelectSingleNode("Elapsed").InnerText);
            dMissionElapsed = Double.Parse(xml.SelectSingleNode("MissionElapsed").InnerText);
            dSep = Double.Parse(xml.SelectSingleNode("Sep").InnerText);
            EncounterCount = Int32.Parse(xml.SelectSingleNode("EncounterCount").InnerText);
            dtStart = DateTime.FromBinary(long.Parse(xml.SelectSingleNode("Start").InnerText));
            bSurrendered = (xml.SelectSingleNode("Surrendered") != null);

            SetUp();
            clockTick.Stop();
        }
        private void SetUp() {
            dtStart = Const.dtTime;
            dDist = AstronomicalObject.CalculateDistance(aoTravelFrom, aoTravelTo);
            string strDist = Utils.PrintDistance(dDist);
            string strTime = String.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(dTravelTime));
            clockTick = new Timer();
            clockTick.Tick += new EventHandler(ClockTickProcessor);
            clockTick.Interval = 250;
            clockTick.Start();
            GameOver = false;
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Travel>");
            file.WriteLine("<AOFrom>" + aoTravelFrom.PrintCoordinates() + "</AOFrom>");
            file.WriteLine("<AOTo>" + aoTravelTo.PrintCoordinates() + "</AOTo>");
            file.WriteLine("<Time>" + dTravelTime + "</Time>");
            file.WriteLine("<Elapsed>" + dElapsed + "</Elapsed>");
            file.WriteLine("<MissionElapsed>" + dMissionElapsed + "</MissionElapsed>");
            file.WriteLine("<EncounterCount>" + EncounterCount + "</EncounterCount>");
            file.WriteLine("<Start>" + dtStart.ToBinary() + "</Start>");
            file.WriteLine("<Sep>" + dSep + "</Sep>");
            if (bSurrendered) file.WriteLine("<Surrendered/>");
            file.WriteLine("</Travel>");
        }

        // This is the method to run when the timer is raised.
        private void ClockTickProcessor(Object myObject, EventArgs myEventArgs) {
            if (ParentView.msgBox.Active) return;
            if (ParentView.TravelDetails == null) {
                clockTick.Stop();
                return;
            }
            if (PlayerTeam.CurrentMission != null) ResolveEncounter();
            else {
                lShots.Clear();
                if ((rand.NextDouble() * (1 + EncounterCount / 2.0)) < 0.2) {  // Reduce the chance of multiple encounters
                    clockTick.Stop();
                    PlayerTeam.SetCurrentMission(Encounter.CheckForInterception(aoTravelFrom, aoTravelTo, dTravelTime, PlayerTeam, dElapsed / dTravelTime));
                    if (PlayerTeam.CurrentMission != null) {
                        dMissionElapsed = 0.0;
                        EncounterCount++;
                        ParentView.RefreshView();
                        PlayerTeam.PlayerShip.InitialiseForBattle();
                        bSurrendered = false;
                        if (PlayerTeam.CurrentMission.Type == Mission.MissionType.BoardingParty) {
                            RunBoardingPartyMission(PlayerTeam.CurrentMission);
                            ParentView.RefreshView();
                            return;
                        }
                        if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) dSep = 18000.0;
                        clockTick.Start();
                        if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Ignore) PlayerTeam.SetCurrentMission(null);
                        return;
                    }
                    clockTick.Start();
                }
                dElapsed += dTickStep; // Move forward if we're not currently on a mission
                if (dElapsed >= dTravelTime) {
                    dElapsed = dTravelTime;
                    clockTick.Stop();
                    Const.dtTime = dtStart.AddSeconds(dElapsed);
                    ParentView.RefreshView();
                    ParentView.ArriveAt(aoTravelTo);
                    return;
                }
                Const.dtTime = dtStart.AddSeconds(dElapsed);
            }
            ParentView.RefreshView();
        }

        // Run a clock tick for the encounter
        private void ResolveEncounter() {
            if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Salvage) {
                dMissionElapsed += 60.0 * 60.0 * 6.0; // Add six hours
                if (dMissionElapsed > PlayerTeam.CurrentMission.TimeCost) dMissionElapsed = PlayerTeam.CurrentMission.TimeCost;
                Const.dtTime = dtStart.AddSeconds(dElapsed).AddSeconds(dMissionElapsed);
                if (dMissionElapsed >= PlayerTeam.CurrentMission.TimeCost) {
                    clockTick.Stop();
                    dtStart = dtStart.AddSeconds(PlayerTeam.CurrentMission.TimeCost);
                    Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission.ShipTarget.GenerateSalvage(false);
                    AnnounceSalvage(dSalvage);
                    dSalvage = PlayerTeam.AddItems(dSalvage);
                    if (Utils.CalculateMass(dSalvage) > 0.0) {
                        ParentView.msgBox.PopupMessage("Your cargo hold is full - couldn't collect all salvage. It has been jettisoned into space.");
                    }
                    PlayerTeam.CeaseMission();
                    clockTick.Start();
                }
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Repair) {
                dMissionElapsed += 60.0 * 60.0 * 6.0; // Add six hours
                if (dMissionElapsed > PlayerTeam.CurrentMission.TimeCost) dMissionElapsed = PlayerTeam.CurrentMission.TimeCost;
                Const.dtTime = dtStart.AddSeconds(dElapsed).AddSeconds(dMissionElapsed);
                if (dMissionElapsed >= PlayerTeam.CurrentMission.TimeCost) {
                    clockTick.Stop();
                    dtStart = dtStart.AddSeconds(PlayerTeam.CurrentMission.TimeCost);
                    PlayerTeam.Cash += PlayerTeam.CurrentMission.Reward;
                    ParentView.msgBox.PopupMessage("Repairs completed. You receive a reward of " + PlayerTeam.CurrentMission.Reward + " credits.");
                    PlayerTeam.CeaseMission();
                    clockTick.Start();
                }
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) {
                clockTick.Interval = 50;
                RunBattle();
            }
            else {
                PlayerTeam.CeaseMission();
                throw new Exception("Unexpected Mission Type : " + PlayerTeam.CurrentMission.Type);
            }
        }
        public void ResumeMissionAfterReload() {
            if (PlayerTeam.CurrentMission == null) {
                clockTick.Interval = 250;
                clockTick.Start();
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Salvage) {
                clockTick.Interval = 250;
                clockTick.Start();
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Repair) {
                clockTick.Interval = 250;
                clockTick.Start();
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) {
                clockTick.Interval = 50;
                clockTick.Start();
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.RepelBoarders) {
                RunRepelBoardersMission(PlayerTeam.CurrentMission);
            }
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.BoardingParty) {
                RunBoardingPartyMission(PlayerTeam.CurrentMission);
            }
        }

        // Run a step of a battle between the player ship and an enemy vessel
        private void RunBattle() {
            Random rand = new Random();
            if (dSep > 4000.0) dSep -= 100.0;

            // -- Player ship fires weapons
            foreach (int r in PlayerTeam.PlayerShip.AllWeaponRooms) {
                ShipWeapon sw = PlayerTeam.PlayerShip.GetEquipmentByRoomID(r) as ShipWeapon;
                if (sw == null) throw new Exception("Got non-weapon in player ship AllWeaponRooms");
                sw.Cooldown -= 0.05;
                if (sw.Cooldown <= 0.0 && sw.Range >= dSep) {
                    int Attack = PlayerTeam.PlayerShip.Attack + sw.Attack;
                    int Defence = PlayerTeam.CurrentMission.ShipTarget.Defence;
                    double Damage = (rand.NextDouble() * Attack) - (rand.NextDouble() * Defence);
                    double dmg = PlayerTeam.CurrentMission.ShipTarget.DamageShip(Damage);
                    sw.Cooldown = sw.Rate + (rand.NextDouble() * 0.1); // Reset cooldown, plus some randomness
                    ShipRoomDesign sT = PlayerTeam.CurrentMission.ShipTarget.PickRandomRoom(rand);
                    lShots.Add(new Shot(true, PlayerTeam.PlayerShip.Type.Rooms[r], sT, sw.Attack, dSep, ParentView.Aspect));
                    for (int n = 0; n <= (int)(dmg * 5.0); n++) {
                        lFrag.Add(new Frag((0.54 + dSep * Const.DrawBattleScale) + ((sT.XPos + sT.Width / 2) * 0.005 / ParentView.Aspect), 0.58 + ((sT.YPos - sT.Height / 2) * 0.005), (rand.NextDouble() - 0.5) * 0.003, (rand.NextDouble() - 0.5) * 0.003, rand.NextDouble() * rand.NextDouble()));
                    }
                }
            }

            // Enemy ship destroyed?
            if (PlayerTeam.CurrentMission.ShipTarget.Hull <= 0.0) {
                clockTick.Stop();
                ParentView.msgBox.PopupMessage("The enemy ship has been destroyed");
                Dictionary<IItem, int> dSalvage = PlayerTeam.CurrentMission.ShipTarget.GenerateSalvage(true);
                AnnounceSalvage(dSalvage);
                dSalvage = PlayerTeam.AddItems(dSalvage);
                if (Utils.CalculateMass(dSalvage) > 0.0) {
                    ParentView.msgBox.PopupMessage("Your cargo hold is full - couldn't collect all the salvage.\nAny excess has been jettisoned into space.");
                }
                PlayerTeam.CeaseMission();
                clockTick.Interval = 250;
                clockTick.Start();
                return;
            }

            // Enemy offers to surrender?
            if (!bSurrendered) {
                if ((PlayerTeam.CurrentMission.ShipTarget.HullFract + 0.4 + (rand.NextDouble() * 0.1) < PlayerTeam.PlayerShip.HullFract) || (PlayerTeam.PlayerShip.HullFract > 0.25 && PlayerTeam.CurrentMission.ShipTarget.HullFract < 0.1)) {
                    bSurrendered = true;
                    clockTick.Stop();
                    double dReward = Utils.RoundSF(PlayerTeam.CurrentMission.ShipTarget.EstimatedBountyValue * (rand.NextDouble() + 0.5), 3);
                    string strMessage = String.Format("The enemy ship captain offers to surrender and turn over a bounty of {0} credits.\nAccept his surrender?", dReward);
                    if (MessageBox.Show(strMessage, "Accept Surrender", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        PlayerTeam.Cash += dReward;
                        PlayerTeam.CeaseMission();
                        ParentView.msgBox.PopupMessage("The enemy captain hands over the bounty and flees the battle scene");
                        clockTick.Interval = 250;
                        clockTick.Start();
                        return;
                    }
                    clockTick.Start();
                }
            }

            // -- Enemy fires weapons
            foreach (int r in PlayerTeam.CurrentMission.ShipTarget.AllWeaponRooms) {
                ShipWeapon sw = PlayerTeam.CurrentMission.ShipTarget.GetEquipmentByRoomID(r) as ShipWeapon;
                if (sw == null) throw new Exception("Got non-weapon in attacking ship AllWeaponRooms");
                sw.Cooldown -= 0.05;
                if (sw.Cooldown <= 0.0 && sw.Range >= dSep) {
                    int Attack = PlayerTeam.CurrentMission.ShipTarget.Attack + sw.Attack;
                    int Defence = PlayerTeam.PlayerShip.Defence;
                    double Damage = (rand.NextDouble() * Attack) - (rand.NextDouble() * Defence);
                    double dmg = PlayerTeam.PlayerShip.DamageShip(Damage);
                    sw.Cooldown = sw.Rate + (rand.NextDouble() * 0.1); // Reset cooldown, plus some randomness
                    ShipRoomDesign sT = PlayerTeam.PlayerShip.PickRandomRoom(rand);
                    lShots.Add(new Shot(false, PlayerTeam.CurrentMission.ShipTarget.Type.Rooms[r], sT, sw.Attack, dSep, ParentView.Aspect));
                    for (int n = 0; n <= (int)(dmg * 5.0); n++) {
                        lFrag.Add(new Frag((0.46 - dSep * Const.DrawBattleScale) - ((sT.XPos + sT.Width / 2) * 0.005 / ParentView.Aspect), 0.58 + ((sT.YPos - sT.Height / 2) * 0.005), (rand.NextDouble() - 0.5) * 0.003, (rand.NextDouble() - 0.5) * 0.003, rand.NextDouble() * rand.NextDouble()));
                    }
                }
            }

            // Player Ship defeated?
            if (PlayerTeam.PlayerShip.Hull <= 0.0) {
                clockTick.Stop();
                ParentView.msgBox.PopupMessage("Your Ship has been destroyed!");
                PlayerTeam.CeaseMission();
                GameOver = true;
                return;
            }

            // Enemy attempts to board?
            if (dSep <= 4000.0 && PlayerTeam.PlayerShip.HullFract < (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.2) && PlayerTeam.CurrentMission.ShipTarget.HullFract > (Const.DEBUG_MORE_BOARDERS ? 0.2 : 0.3) && rand.NextDouble() > (Const.DEBUG_MORE_BOARDERS ? 0.4 : 0.7)) {
                clockTick.Stop();
                ParentView.msgBox.PopupMessage("The enemy are attempting to board your ship. Get ready to repel boarders");
                PlayerTeam.SetCurrentMission(Mission.CreateRepelBoardersMission(PlayerTeam.CurrentMission.RacialOpponent, PlayerTeam.CurrentMission.Diff, PlayerTeam.PlayerShip));
                RunRepelBoardersMission(PlayerTeam.CurrentMission);
            }
        }
        private void RunRepelBoardersMission(Mission miss) {
            clockTick.Stop();
            ParentView.RunMission(miss);
        }
        private void RunBoardingPartyMission(Mission miss) {
            clockTick.Stop();
            ParentView.RunMission(miss);
        }
        public void ResumeTravelling() {
            // If victory then continue travelling
            clockTick.Interval = 250;
            clockTick.Start();
        }

        // Draw the progress with whatever is happening
        public void Display(ShaderProgram prog) {
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);

            return;

            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            // Draw a large box in the centre of the screen
            GL.Disable(EnableCap.Texture2D);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color3(0.0, 0.0, 0.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.2, 0.2, 0.0);
            GL.Vertex3(0.8, 0.2, 0.0);
            GL.Vertex3(0.8, 0.8, 0.0);
            GL.Vertex3(0.2, 0.8, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.2, 0.2, 0.0);
            GL.Vertex3(0.8, 0.2, 0.0);
            GL.Vertex3(0.8, 0.8, 0.0);
            GL.Vertex3(0.2, 0.8, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //  Task specific stuff
            if (PlayerTeam.CurrentMission == null) DrawTravelProgress(prog);
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Repair) DrawRepair();
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.Salvage) DrawSalvage();
            else if (PlayerTeam.CurrentMission.Type == Mission.MissionType.ShipCombat) DrawBattle();

            // Display the description
            GL.PushMatrix();
            GL.Translate(0.5, 0.25, 0.0);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Scale(0.05 / ParentView.Aspect, 0.05, 0.05);
            GL.Rotate(180.0, Vector3d.UnitX);
            //tlTravel.Draw(TextLabel.Alignment.TopMiddle);
            GL.PopMatrix();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }
        private void DrawTravelProgress(ShaderProgram texProg) {
            // Set up the text
            string strDist = Utils.PrintDistance(AstronomicalObject.CalculateDistance(aoTravelFrom, aoTravelTo));
            string strTime = String.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(dTravelTime));
            //tlTravel.UpdateText(strDist + " " + strTime);

            // Progress Bar
            double dFract = dElapsed / dTravelTime;
            GL.Color3(0.0, 1.0, 0.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.7, 0.5, 0.0);
            GL.Vertex3(0.7, 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            // Source and Target AOs
            GL.PushMatrix();
            GL.Translate(0.25, 0.42, 0.0);
            GL.Scale(0.025 / ParentView.Aspect, 0.025, 0.025);
            GL.Rotate(90.0, Vector3d.UnitY);
            aoTravelFrom.DrawSelected(texProg, 6);
            GL.PopMatrix();
            GL.PushMatrix();
            GL.Translate(0.75, 0.42, 0.0);
            GL.Scale(0.025 / ParentView.Aspect, 0.025, 0.025);
            GL.Rotate(90.0, Vector3d.UnitY);
            aoTravelTo.DrawSelected(texProg, 6);
            GL.PopMatrix();
        }
        private void DrawSalvage() {
            // Set up the text
            string strTime = String.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(PlayerTeam.CurrentMission.TimeCost));
            //tlTravel.UpdateText("Salvaging " + strTime);
            // Progress Bar
            double dFract = dMissionElapsed / PlayerTeam.CurrentMission.TimeCost;
            GL.Color3(1.0, 0.0, 0.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.7, 0.5, 0.0);
            GL.Vertex3(0.7, 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
        private void DrawRepair() {
            // Set up the text
            string strTime = String.Format("({0:%d}d {0:%h}h {0:%m}m {0:%s}s)", TimeSpan.FromSeconds(PlayerTeam.CurrentMission.TimeCost));
            //tlTravel.UpdateText("Repairing " + strTime);
            // Progress Bar
            double dFract = dMissionElapsed / PlayerTeam.CurrentMission.TimeCost;
            GL.Color3(0.0, 0.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.5, 0.0);
            GL.Vertex3(0.3 + (dFract * 0.4), 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(0.3, 0.5, 0.0);
            GL.Vertex3(0.7, 0.5, 0.0);
            GL.Vertex3(0.7, 0.55, 0.0);
            GL.Vertex3(0.3, 0.55, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
        private void DrawBattle() {
            //if (PlayerTeam.CurrentMission.RacialOpponent == null) tlTravel.UpdateText("Battle versus unidentified alien ship");
            //else tlTravel.UpdateText("Battle versus " + PlayerTeam.CurrentMission.RacialOpponent.Name + " ship");

            // Setup and draw status text
            // Player Ship Stats:
            string strStatus = Math.Round(PlayerTeam.PlayerShip.Hull, 1) + "/" + Math.Round(PlayerTeam.PlayerShip.Type.MaxHull, 0);
            DrawFractBar(0.22, 0.35, 0.1, 0.02, PlayerTeam.PlayerShip.HullFract, Color.Green);
            GL.PushMatrix();
            GL.Translate(0.33, 0.34, 0.0);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Scale(0.04 / ParentView.Aspect, 0.04, 0.04);
            GL.Rotate(180.0, Vector3d.UnitX);
            TextRenderer.Draw(strStatus, Alignment.TopLeft);
            GL.PopMatrix();
            if (PlayerTeam.PlayerShip.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.PlayerShip.Shield, 1) + "/" + PlayerTeam.PlayerShip.MaxShield;
                DrawFractBar(0.22, 0.42, 0.1, 0.02, PlayerTeam.PlayerShip.ShieldFract, Color.Blue);
                GL.PushMatrix();
                GL.Translate(0.33, 0.41, 0.0);
                GL.Color3(1.0, 1.0, 1.0);
                GL.Scale(0.04 / ParentView.Aspect, 0.04, 0.04);
                GL.Rotate(180.0, Vector3d.UnitX);
                TextRenderer.Draw(strStatus, Alignment.TopLeft);
                GL.PopMatrix();
            }

            // Enemy Ship Stats:
            double stHull = Math.Max(0.0, Math.Round(PlayerTeam.CurrentMission.ShipTarget.Hull, 1));
            strStatus = stHull + "/" + Math.Round(PlayerTeam.CurrentMission.ShipTarget.Type.MaxHull, 0);
            DrawFractBar(0.68, 0.35, 0.1, 0.02, PlayerTeam.CurrentMission.ShipTarget.HullFract, Color.Green);
            GL.PushMatrix();
            GL.Translate(0.67, 0.34, 0.0);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Scale(0.04 / ParentView.Aspect, 0.04, 0.04);
            GL.Rotate(180.0, Vector3d.UnitX);
            TextRenderer.Draw(strStatus, Alignment.TopLeft);
            GL.PopMatrix();
            if (PlayerTeam.CurrentMission.ShipTarget.MaxShield > 0.0) {
                strStatus = Math.Round(PlayerTeam.CurrentMission.ShipTarget.Shield, 1) + "/" + PlayerTeam.CurrentMission.ShipTarget.MaxShield;
                DrawFractBar(0.68, 0.42, 0.1, 0.02, PlayerTeam.CurrentMission.ShipTarget.ShieldFract, Color.Blue);
                GL.PushMatrix();
                GL.Translate(0.67, 0.41, 0.0);
                GL.Color3(1.0, 1.0, 1.0);
                GL.Scale(0.04 / ParentView.Aspect, 0.04, 0.04);
                GL.Rotate(180.0, Vector3d.UnitX);
                TextRenderer.Draw(strStatus, Alignment.TopLeft);
                GL.PopMatrix();
            }

            // Display ships
            double dSpace = dSep * Const.DrawBattleScale;
            GL.PushMatrix();
            GL.Translate(0.46 - dSpace, 0.58, 0.0);
            GL.Scale(-0.005 / ParentView.Aspect, 0.005, 0.005);
            PlayerTeam.PlayerShip.DrawBattle();
            GL.PopMatrix();
            GL.PushMatrix();
            GL.Translate(0.54 + dSpace, 0.58, 0.0);
            GL.Scale(0.005 / ParentView.Aspect, 0.005, 0.005);
            PlayerTeam.CurrentMission.ShipTarget.DrawBattle();
            GL.PopMatrix();

            // Draw weapon shots (Blue = player, red = enemy, width = power) and Frag
            GL.Begin(BeginMode.Lines);
            foreach (Shot sh in lShots) {
                sh.Display();
            }
            GL.End();
            lShots.RemoveAll(s => s.Life <= 0);
            GL.Begin(BeginMode.Quads);
            foreach (Frag fr in lFrag) {
                fr.Display(ParentView.Aspect);
            }
            GL.End();
            lFrag.RemoveAll(f => f.F <= 0.0);
        }
        private void DrawFractBar(double dTLCX, double dTLCY, double dWidth, double dHeight, double dFract, Color cBar) {
            if (dFract < 0.0) dFract = 0.0;
            if (dFract > 1.0) dFract = 1.0;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color3(cBar);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(dTLCX, dTLCY, 0.0);
            GL.Vertex3(dTLCX + (dFract * dWidth), dTLCY, 0.0);
            GL.Vertex3(dTLCX + (dFract * dWidth), dTLCY + dHeight, 0.0);
            GL.Vertex3(dTLCX, dTLCY + dHeight, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Quads);
            GL.Vertex3(dTLCX, dTLCY, 0.0);
            GL.Vertex3(dTLCX + dWidth, dTLCY, 0.0);
            GL.Vertex3(dTLCX + dWidth, dTLCY + dHeight, 0.0);
            GL.Vertex3(dTLCX, dTLCY + dHeight, 0.0);
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        // Announce the salvage gained 
        private void AnnounceSalvage(Dictionary<IItem, int> dSalvage) {
            ParentView.msgBox.PopupMessage("Got " + dSalvage.Count + " item(s) of salvage");
        }

        public bool StopTimer() {
            if (clockTick == null) return false;
            if (clockTick.Enabled) {
                clockTick.Stop();
                return true;
            }
            return false;
        }
        public void StartTimer() {
            if (clockTick != null) clockTick.Start();
        }
    }
}
