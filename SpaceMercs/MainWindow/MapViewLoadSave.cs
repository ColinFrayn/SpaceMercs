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

            // Load in the clock
            DateTime newTime = DateTime.Parse(xml.SelectNodeText("Clock"));
            Const.dtTime = newTime;

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
                newTravel = new Travel(xTravel, newTeam, this);
            }

            return new Tuple<Map, Team, Travel?>(newMap, newTeam, newTravel);
        }

        // Save the current game to an XML format (faking this with StreamWriter for simplicity)
        private void SaveGame(string strFile) {
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
                file.WriteLine("</GUI>");

                // Game clock
                file.WriteLine("<Clock>" + Const.dtTime.ToString() + "</Clock>");

                // Save Map data
                GalaxyMap.SaveToFile(file);

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
