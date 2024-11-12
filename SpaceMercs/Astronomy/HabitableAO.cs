using OpenTK.Windowing.Common.Input;
using SpaceMercs.Graphics;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class HabitableAO : OrbitalAO {
        public Colony? Colony { get; private set; }
        public int BaseSize {
            get {
                if (Colony == null) return 0;
                return Colony.BaseSize;
            }
        }
        public Planet.PlanetType Type;
        protected List<Mission>? _MissionList = null;
        public IEnumerable<Mission> MissionList { get { return _MissionList?.AsReadOnly() ?? new List<Mission>().AsReadOnly(); } }
        public bool Scanned { get; private set; }
        public int CountMissions { get { return _MissionList?.Count ?? 0; } }
        public double OrbitalPeriod; // Period of orbit (seconds)

        public HabitableAO() {}
        public HabitableAO(XmlNode xml, AstronomicalObject parent) : base(xml, parent) { 
            OrbitalPeriod = xml.SelectNodeDouble("PRot");
            Type = (Planet.PlanetType)Enum.Parse(typeof(Planet.PlanetType), xml.SelectNodeText("Type"));
            XmlNode? xmlc = xml.SelectSingleNode("Colony");
            if (xmlc != null) SetColony(new Colony(xmlc, this));
            LoadMissions(xml);
        }

        public override void SaveToFile(StreamWriter file) {
            file.WriteLine("<Type>" + Type.ToString() + "</Type>");
            file.WriteLine("<PRot>" + Math.Round(OrbitalPeriod, 0).ToString() + "</PRot>");
            SaveMissions(file);
            Colony?.SaveToFile(file);
            base.SaveToFile(file);
        }

        protected void LoadMissions(XmlNode xml) {
            Scanned = (xml.SelectSingleNode("Scanned") is not null);
            IEnumerable<XmlNode> nodes = xml.SelectNodesToList("Missions/Mission");
            if (!nodes.Any()) return;
            _MissionList = new List<Mission>();
            foreach (XmlNode xm in nodes) {
                _MissionList.Add(new Mission(xm, this));
            }
        }
        protected void SaveMissions(StreamWriter file) {
            if (Scanned || CountMissions > 0) file.WriteLine(" <Scanned/>");
            if (_MissionList == null) return;
            file.WriteLine(" <Missions>");
            foreach (Mission m in _MissionList) m.SaveToFile(file);
            file.WriteLine(" </Missions>");
        }
        public void RemoveMission(Mission miss) {
            if (_MissionList == null || miss == null) return;
            if (_MissionList.Contains(miss)) {
                _MissionList.Remove(miss);
                return;
            }
            // In case the mission pointers got messed up, check for an identical mission and delete it
            foreach (Mission m in _MissionList) {
                if (m.IsSameMissionAs(miss)) {
                    _MissionList.Remove(m);
                    return;
                }
            }
        }
        public void AddMission(Mission miss) {
            if (miss == null) return;
            if (_MissionList == null) _MissionList = new List<Mission>();
            if (_MissionList.Contains(miss)) return;
            _MissionList.Add(miss);
            Scanned = true;
        }

        public double TDiff(Race rc) {
            double tdiff = Temperature - rc.BaseTemp;
            if (tdiff > 0.0) tdiff *= 2.0;
            return Math.Abs(tdiff);
        }
        public void SetColony(Colony cl) {
            Colony = cl;
            cl.Owner.AddColony(cl);
        }
        public void SetupBase(Race rc, int iSize) {
            if (iSize < 1) return;
            Colony = new Colony(rc, iSize, Seed, this);
        }
        public void ExpandBase(Colony.BaseType bt) {
            if (Colony is not null) Colony.ExpandBase(bt);
        }
        public int ExpandBase(Race rc, Random rand) {
            if (Colony == null) {
                Colony = new Colony(rc, 1, rand.Next(1000000), this);
                return 1;
            }
            return Colony.ExpandBase(rand);
        }
        public Planet ParentPlanet() {
            if (this is Planet pln) return pln;
            else if (this is Moon mn && mn.Parent is Planet pln2) return pln2;
            throw new Exception($"Unexpected HAO type : {this.GetType()}");
        }

        // Draw icons showing whether or not this body has a base on it and, if so, then what type.
        public abstract void DrawBaseIcon(ShaderProgram prog);
    }
}
