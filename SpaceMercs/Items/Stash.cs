using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Stash {
        private readonly Dictionary<IItem, int> stash;
        public Point Location { get; private set; } // Isn't really used; only for storing location on load.
        public bool Hidden { get; private set; }
        public bool ContainsOnlyCorpses {
            get {
                if (IsEmpty) return false;
                foreach (IItem it in stash.Keys) {
                    if (it is not Corpse) return false;
                }
                return true;
            }
        }
        public int Count {
            get {
                if (stash == null) return 0;
                int count = 0;
                foreach (int n in stash.Values) count += n;
                return count;
            }
        }
        public bool IsEmpty { get { if (stash == null) return true; return (!stash.Any()); } }

        public Stash(Dictionary<IItem, int> dict, Point pt) {
            stash = new Dictionary<IItem, int>(dict);
            Location = pt;
            Hidden = false;
        }
        public Stash(Point pt) {
            stash = new Dictionary<IItem, int>();
            Location = pt;
            Hidden = false;
        }
        public Stash(XmlNode xml) {
            stash = new Dictionary<IItem, int>();
            int X = xml.GetAttributeInt("X");
            int Y = xml.GetAttributeInt("Y");
            Location = new Point(X, Y);
            foreach (XmlNode xn in xml.SelectNodesToList("StashItem")) {
                int n = xn.GetAttributeInt("N");
                IItem? it = Utils.LoadItem(xn.FirstChild);
                if (it is not null) stash.Add(it, n);
            }
            Hidden = (xml.SelectSingleNode("Hidden") != null);
        }
        public Stash(int lvl, Point pt, Random rnd) {
            stash = new Dictionary<IItem, int>();
            Location = pt;
            Hidden = false;
            double dnum = rnd.NextDouble() * (1 + Math.Max(lvl / 3, 3));
            int num = (int)Math.Round(dnum);
            if (num < 1) num = 1;
            for (int n = 0; n < num; n++) {
                IItem? eq = Utils.GenerateRandomItem(rnd, lvl);
                if (eq is not null) Add(eq);
            }
        }

        public void SaveToFile(StreamWriter file, Point pt) {
            file.WriteLine("<Stash X=\"" + pt.X + "\" Y=\"" + pt.Y + "\">");

            foreach (IItem it in stash.Keys) {
                file.WriteLine("<StashItem N=\"" + stash[it] + "\">");
                it.SaveToFile(file);
                file.WriteLine("</StashItem>");
            }
            if (Hidden) file.WriteLine("<Hidden/>");

            file.WriteLine("</Stash>");
        }

        public IEnumerable<IItem> Items() {
            return stash.Keys.ToList().AsReadOnly();
        }
        public int GetCount(IItem? it) {
            if (it == null) return 0;
            if (!stash.ContainsKey(it)) return 0;
            return stash[it];
        }
        public void Add(IItem? it, int count = 1) {
            if (it == null) return;
            if (stash.ContainsKey(it)) stash[it] += count;
            else stash.Add(it, count);
        }
        public void Add(Stash? st) {
            if (st == null) return;
            foreach (IItem it in st.stash.Keys) {
                if (stash.ContainsKey(it)) stash[it] += st.stash[it];
                else Add(it, st.stash[it]);
            }
        }
        public void Remove(IItem? it, int count = 1) {
            if (it == null) return;
            if (!stash.ContainsKey(it)) throw new Exception("Attempting to remove missing item from Stash");
            if (stash[it] < count) throw new Exception("Attempting to remove more items than exist in Stash");
            stash[it] -= count;
            if (stash[it] == 0) stash.Remove(it);
        }
        public void Clear() {
            stash.Clear();
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
    }
}
