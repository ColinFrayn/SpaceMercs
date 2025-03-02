using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class OrbitalAO : AstronomicalObject {
        public double OrbitalDistance; // In metres
        public AstronomicalObject Parent { get; protected set; }
        protected List<Mission>? _MissionList = null;
        public IEnumerable<Mission> MissionList { get { return _MissionList?.AsReadOnly() ?? new List<Mission>().AsReadOnly(); } }
        public bool Scanned { get; private set; }
        public int CountMissions { get { return _MissionList?.Count ?? 0; } }
        public abstract Planet.PlanetType Type { get; }
        public Colony? Colony { 
            get {
                if (this is HabitableAO hao) return hao.Colony;
                return null;
            } 
        }

        public OrbitalAO() { }
        public OrbitalAO(double orbit, AstronomicalObject parent) {
            Parent = parent;
            OrbitalDistance = orbit;
        }
        public OrbitalAO(XmlNode xml, AstronomicalObject parent) : base(xml) {
            Parent = parent;
            OrbitalDistance = xml.SelectNodeDouble("Orbit", 0.0);
            LoadMissions(xml);
        }

        // Save this object to an Xml file
        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            base.SaveToFile(file, clock);
            SaveMissions(file);
            if (OrbitalDistance != 0.0) {
                file.WriteLine("<Orbit>" + Math.Round(OrbitalDistance, 0).ToString() + "</Orbit>");
            }
        }

        public virtual double DistanceFromStar() {
            return OrbitalDistance;
        }

        public void SetParent(AstronomicalObject parent) {
            Parent = parent;
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
            _MissionList ??= new List<Mission>();
            if (_MissionList.Contains(miss)) return;
            _MissionList.Add(miss);
            Scanned = true;
        }
        public void SetScanned() {
            Scanned = true;
        }


    }
}
