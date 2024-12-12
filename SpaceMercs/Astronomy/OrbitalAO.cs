using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class OrbitalAO : AstronomicalObject {
        public double OrbitalDistance; // In metres
        public AstronomicalObject Parent { get; protected set; }

        public OrbitalAO() { }
        public OrbitalAO(double orbit, AstronomicalObject parent) {
            Parent = parent;
            OrbitalDistance = orbit;
        }
        public OrbitalAO(XmlNode xml, AstronomicalObject parent) : base(xml) {
            Parent = parent;
            OrbitalDistance = xml.SelectNodeDouble("Orbit", 0.0);
        }

        // Save this object to an Xml file
        public override void SaveToFile(StreamWriter file) {
            base.SaveToFile(file);
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
    }
}
