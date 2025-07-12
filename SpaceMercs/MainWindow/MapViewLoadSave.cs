using System.IO;
using System.Xml;

namespace SpaceMercs.MainWindow {
    partial class MapView {
        static readonly string FileVersion = "1.0";

        // Load a game from an exported XML file
        private Tuple<Map, Team, Travel?> LoadGame(string strFile) {
            // Write the string to a file.
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(strFile);
            XmlNodeList xnl = xDoc.GetElementsByTagName("SpaceMercsSaveFile");
            if (xnl.Count == 0) {
                throw new Exception("Unknown File Type - Not a SpaceMercs save Game");
            }
            XmlNode xml = xnl.Item(0)!;
            string version = xml.GetAttributeText("Version");
            double thisVersion = double.Parse(FileVersion);
            double fileVersion = double.Parse(version);
            if (thisVersion != fileVersion) {
                throw new Exception($"Incorrect file version. Expected = {FileVersion}, found {version}.");
            }

            // Load in GUI details
            XmlNode xGUI = xml.SelectSingleNode("GUI") ?? throw new Exception("Could not find GUI details in save file");

            // Load GUI Details here
            bShowLabels = (xGUI.SelectSingleNode("ShowLabels") != null);
            bShowGridLines = (xGUI.SelectSingleNode("ShowMapGrid") != null);
            bFadeUnvisited = (xGUI.SelectSingleNode("FadeStars") != null);
            bShowRangeCircles = (xGUI.SelectSingleNode("ShowRangeCircles") != null);
            bShowTradeRoutes = (xGUI.SelectSingleNode("ShowTradeRoutes") != null);
            bShowFlags = (xGUI.SelectSingleNode("ShowFlags") != null);
            bShowColonies = (xGUI.SelectSingleNode("ShowColonies") != null);
            bShowPop = (xGUI.SelectSingleNode("ShowPopulation") != null);

            // Reset all races
            foreach (Race rc in StaticData.Races) rc.Reset();

            // Load in the clock
            DateTime newTime = DateTime.Parse(xml.SelectNodeText("Clock"));
            Clock.SetTime(newTime);

            // Load in Map data
            XmlNode? xMap = xml.SelectSingleNode("Map");
            if (xMap is null) {
                throw new Exception("Could not find Map details in save file");
            }
            Map newMap = new Map(xMap);

            // Load in player team
            XmlNode? xTeam = xml.SelectSingleNode("Team");
            if (xTeam is null) {
                throw new Exception("Could not find Player Team details in save file");
            }
            Team newTeam = new Team(xTeam, newMap);

            // Load additional race data
            foreach (XmlNode xr in xml.SelectNodesToList("Races/Race")) {
                string strName = xr.GetAttributeText("Name");
                Race r = StaticData.GetRaceByName(strName) ?? throw new Exception("Could not ID Race : " + strName);
                r.LoadAdditionalData(xr, newMap);
            }

            // Load in Travel details
            Travel? newTravel = null;
            XmlNode? xTravel = xml.SelectSingleNode("Travel");
            if (xTravel is not null) {
                newTravel = new Travel(xTravel, newTeam, this, Clock);
            }

            return new Tuple<Map, Team, Travel?>(newMap, newTeam, newTravel);
        }

        private (int? Seed, DateTime? clock) GetSaveGameDetails(string strFile) {
            try {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(strFile);
                XmlNodeList xnl = xDoc.GetElementsByTagName("SpaceMercsSaveFile");
                if (xnl.Count == 0) return (null, null);
                XmlNode xml = xnl.Item(0)!;

                // Load in the clock
                DateTime newTime = DateTime.Parse(xml.SelectNodeText("Clock"));

                // Load in Map data
                XmlNode? xMap = xml.SelectSingleNode("Map");
                int? seed = xMap?.GetAttributeInt("Seed", null);

                return (seed, newTime);
            }
            catch {
                return (null, null);
            }
        }

        private void SaveGame(string strFile) {
            // Check the file we're saving over, and do a sanity check
            try {
                if (File.Exists(strFile)) {
                    (int? seed, DateTime? clock) = GetSaveGameDetails(strFile);

                    if (!seed.HasValue || !clock.HasValue) {
                        msgBox.PopupConfirmation($"This file is not a valid {nameof(SpaceMercs)} save game\nYou may be overwriting a different file type\nAre you sure you want to continue?", () => SaveGame_Continue(strFile));
                        return;
                    }
                    if (seed.Value != GalaxyMap.MapSeed) {
                        msgBox.PopupConfirmation("Are you sure you want to overwrite this file?\nIt has a different map seed\nThis means it is a different game session\nReally overwrite?", () => SaveGame_Continue(strFile));
                        return;
                    }
                    if (clock > Clock.CurrentTime) {
                        msgBox.PopupConfirmation("The file you chose is more recent\nYou may be overwriting a later save\nAre you sure you want to continue?", () => SaveGame_Continue(strFile));
                        return;
                    }
                }
            }
            catch { } // Ignore

            SaveGame_Continue(strFile);
        }

        // Save the current game to an XML format (faking this with StreamWriter for simplicity)
        private void SaveGame_Continue(string strFile) {
            // Write the string to a file.
            using (StreamWriter file = new StreamWriter(strFile)) {
                file.WriteLine("<SpaceMercsSaveFile Version=\"" + FileVersion.ToString() + "\">");

                // Save GUI details
                file.WriteLine("<GUI>");
                if (bShowLabels) file.WriteLine("<ShowLabels/>");
                if (bShowGridLines) file.WriteLine("<ShowMapGrid/>");
                if (bShowRangeCircles) file.WriteLine("<ShowRangeCircles/>");
                if (bShowTradeRoutes) file.WriteLine("<ShowTradeRoutes/>");
                if (bFadeUnvisited) file.WriteLine("<FadeStars/>");
                if (bShowFlags) file.WriteLine("<ShowFlags/>");
                if (bShowColonies) file.WriteLine("<ShowColonies/>");
                if (bShowPop) file.WriteLine("<ShowPopulation/>");
                file.WriteLine("</GUI>");

                // Game clock
                file.WriteLine("<Clock>" + Clock.CurrentTime.ToString() + "</Clock>");

                // Save Map data
                GalaxyMap.SaveToFile(file, Clock);

                // Save player team to file
                PlayerTeam.SaveToFile(file);

                // Save race details
                file.WriteLine("<Races>");
                foreach (Race r in StaticData.Races) {
                    r.SaveAdditionalData(file);
                }
                file.WriteLine("</Races>");

                // If we're travelling, save that
                if (TravelDetails != null) TravelDetails.SaveToFile(file);

                file.WriteLine("</SpaceMercsSaveFile>");
                // All done!
                file.Close();
            }
        }
    }
}
