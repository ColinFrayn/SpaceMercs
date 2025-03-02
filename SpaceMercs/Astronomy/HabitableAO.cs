using SpaceMercs.Graphics;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public abstract class HabitableAO : OrbitalAO {
        public new Colony? Colony { get; private set; }
        public int BaseSize {
            get {
                if (Colony == null) return 0;
                return Colony.BaseSize;
            }
        }
        protected Planet.PlanetType _type;
        public override Planet.PlanetType Type => _type;
        public double OrbitalPeriod; // Period of orbit (seconds)

        public HabitableAO() {}
        public HabitableAO(XmlNode xml, AstronomicalObject parent) : base(xml, parent) { 
            OrbitalPeriod = xml.SelectNodeDouble("PRot");
            _type = (Planet.PlanetType)Enum.Parse(typeof(Planet.PlanetType), xml.SelectNodeText("Type"));
            XmlNode? xmlc = xml.SelectSingleNode("Colony");
            if (xmlc != null) SetColony(new Colony(xmlc, this));
        }

        public override void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine("<Type>" + Type.ToString() + "</Type>");
            file.WriteLine("<PRot>" + Math.Round(OrbitalPeriod, 0).ToString() + "</PRot>");
            Colony?.SaveToFile(file, clock);
            base.SaveToFile(file, clock);
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
        public void SetupBase(Race rc, int iSize, GlobalClock clock) {
            if (iSize < 1) return;
            Colony = new Colony(rc, iSize, Seed, this, clock);
        }
        public void ExpandBase(Colony.BaseType bt) {
            if (Colony is not null) Colony.ExpandBase(bt);
        }
        public int ExpandBase(Race rc, Random rand, GlobalClock clock) {
            if (Colony == null) {
                Colony = new Colony(rc, 1, rand.Next(1000000), this, clock);
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
