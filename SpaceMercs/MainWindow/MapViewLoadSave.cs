using System.IO;
using System.Xml;

namespace SpaceMercs.MainWindow {
    partial class MapView {
        static readonly float FileVersion = 1.0f;

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
            float version = float.Parse(xml.Attributes["Version"].InnerText);
            if (version != FileVersion) {
                throw new Exception("Incorrect file version. Expected = " + FileVersion + ", found " + version + ".");
            }

            // Load in GUI details
            XmlNode xGUI = xml.SelectSingleNode("GUI") ?? throw new Exception("Could not find GUI details in save file");

            // Load GUI Details here
            if (xGUI.SelectSingleNode("ShowLabels") != null) bShowLabels = true;
            else bShowLabels = false;
            if (xGUI.SelectSingleNode("ShowMapGrid") != null) bShowGridLines = true;
            else bShowGridLines = false;
            if (xGUI.SelectSingleNode("FadeStars") != null) bFadeUnvisited = true;
            else bFadeUnvisited = false;
            if (xGUI.SelectSingleNode("ShowRangeCircles") != null) bShowRangeCircles = true;
            else bShowRangeCircles = false;
            if (xGUI.SelectSingleNode("ShowTradeRoutes") != null) bShowTradeRoutes = true;
            else bShowTradeRoutes = false;
            if (xGUI.SelectSingleNode("ShowFlags") != null) bShowFlags = true;
            else bShowFlags = false;
            if (xGUI.SelectSingleNode("ShowColonies") != null) bShowColonies = true;
            else bShowColonies = false;

            // Load in the clock
            DateTime newTime = DateTime.Parse(xml.SelectSingleNode("Clock").InnerText);
            Const.dtTime = newTime;

            // Load in Map data
            XmlNode xMap = xml.SelectSingleNode("Map");
            if (xMap == null) {
                throw new Exception("Could not find Map details in save file");
            }
            Map newMap = new Map(xMap);

            // Load in player team
            XmlNode xTeam = xml.SelectSingleNode("Team");
            if (xTeam == null) {
                throw new Exception("Could not find Player Team details in save file");
            }
            Team newTeam = new Team(xTeam, newMap);

            // Load additional race data
            XmlNode? xRaces = xml.SelectSingleNode("Races");
            foreach (XmlNode xr in xRaces.SelectNodes("Race")) {
                string strName = xr.Attributes["Name"].Value;
                Race r = StaticData.GetRaceByName(strName) ?? throw new Exception("Could not ID Race : " + strName);
                r.LoadAdditionalData(xr, newMap);
            }

            // Load in Travel details
            Travel? newTravel = null;
            XmlNode xTravel = xml.SelectSingleNode("Travel");
            if (xTravel != null) {
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
