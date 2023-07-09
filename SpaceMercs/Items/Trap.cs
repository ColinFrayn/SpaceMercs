using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Trap {
        public Point Location { get; private set; } // Isn't really used; only for storing location on load.
        public bool Hidden { get; private set; }
        public WeaponType.DamageType Type { get; private set; }
        public int Level { get; private set; }

        public Trap(Point pt, int diff, Random rnd) {
            Location = pt;
            Hidden = false;
            Level = diff;
            if (rnd.NextDouble() < 0.5) Level++;
            if (rnd.NextDouble() < 0.5 && Level > 1) Level--;
            int r = rnd.Next(100);
            if (r < 60) Type = WeaponType.DamageType.Physical;
            else if (r < 70) Type = WeaponType.DamageType.Electrical;
            else if (r < 80) Type = WeaponType.DamageType.Fire;
            else if (r < 90) Type = WeaponType.DamageType.Poison;
            else if (r < 95) Type = WeaponType.DamageType.Cold;
            else Type = WeaponType.DamageType.Acid;
        }
        public Trap(XmlNode xml) {
            int X = int.Parse(xml.Attributes["X"].Value);
            int Y = int.Parse(xml.Attributes["Y"].Value);
            Location = new Point(X, Y);
            Level = int.Parse(xml.Attributes["L"].Value);
            Hidden = (xml.SelectSingleNode("Hidden") is not null);
            string strType = xml.SelectNodeText("Type");
            if (!string.IsNullOrEmpty(strType)) {
                Type = (WeaponType.DamageType)Enum.Parse(typeof(WeaponType.DamageType), strType);
            }
            else Type = WeaponType.DamageType.Physical;
        }

        public void SaveToFile(StreamWriter file, Point pt) {
            file.WriteLine("<Trap X=\"" + pt.X + "\" Y=\"" + pt.Y + "\" L=\"" + Level + "\">");

            if (Hidden) file.WriteLine("<Hidden/>");

            if (Type != WeaponType.DamageType.Physical) file.WriteLine("<Type>" + Type + "</Type>");

            file.WriteLine("</Trap>");
        }

        public void SetLocation(Point pt) {
            Location = pt;
        }
        public void Hide() {
            Hidden = true;
        }
        public void Reveal() {
            Hidden = false;
        }
        public Dictionary<WeaponType.DamageType, double> GenerateDamage() {
            Dictionary<WeaponType.DamageType, double> AllDam = new Dictionary<WeaponType.DamageType, double>();
            double dam = Level;
            Random rand = new Random();
            dam *= Const.TrapDamageScale * (1.0 + rand.NextDouble());
            AllDam.Add(Type, dam);
            return AllDam;
        }
    }
}
