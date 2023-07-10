using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    class Colony {
        [Flags]
        public enum BaseType { None = 0x0, Outpost = 0x1, Military = 0x2, Research = 0x4, Colony = 0x8, Trading = 0x10, Metropolis = 0x20 };
        public BaseType Base { get; protected set; }
        public int BaseSize {
            get {
                int Count = 0;
                if ((Base & BaseType.Outpost) != 0) Count++;
                if ((Base & BaseType.Military) != 0) Count++;
                if ((Base & BaseType.Research) != 0) Count++;
                if ((Base & BaseType.Colony) != 0) Count++;
                if ((Base & BaseType.Trading) != 0) Count++;
                if ((Base & BaseType.Metropolis) != 0) Count++;
                return Count;
            }
        }
        public Race Owner { get; private set; }
        public HabitableAO Location { get; private set; }
        public double CostModifier {
            get {
                if (Base == BaseType.Outpost) return 3.0;
                double Modifier = 1.6 - (BaseSize * 0.1);
                if ((Base & BaseType.Trading) != 0) Modifier *= 0.95;
                return Modifier;
            }
        }
        public DateTime dtLastGrowth { get; private set; }
        public DateTime dtNextGrowth { get; private set; }
        public bool CanGrow {
            get {
                if (BaseSize == 6) return false;
                if (Location is Moon && BaseSize >= 5) return false;
                return true;
            }
        }

        private readonly List<Soldier> Mercenaries = new List<Soldier>();
        private readonly List<Mission> Missions = new List<Mission>();
        private readonly Dictionary<IItem, int> Inventory = new Dictionary<IItem, int>();
        private DateTime dtLastUpdate = DateTime.MinValue;

        public Colony(Race rc, int iSize, int seed, HabitableAO loc) {
            Owner = rc;
            Random rand = new Random(seed + 1);
            Base = BaseType.None;
            if (iSize == 5) Base = BaseType.Outpost | BaseType.Military | BaseType.Research | BaseType.Colony | BaseType.Trading;
            else if (iSize == 6) Base = BaseType.Outpost | BaseType.Military | BaseType.Research | BaseType.Colony | BaseType.Trading | BaseType.Metropolis;
            else {
                Base = BaseType.Outpost;
                for (int n = 1; n < iSize; n++) {
                    BaseType btOld = Base;
                    do {
                        BaseType bt = (BaseType)(2 << (rand.Next(4)));
                        if ((Base & bt) == 0) Base |= bt;
                    } while (Base == btOld);
                }
            }
            Location = loc;
            dtLastGrowth = Const.dtTime;
            if (CanGrow) dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            else dtNextGrowth = DateTime.MaxValue;

            rc.AddColony(this);
        }
        public Colony(XmlNode xml, HabitableAO loc) {
            Location = loc;
            string raceName = xml.SelectNodeText("Owner");
            Owner = StaticData.GetRaceByName(raceName) ?? throw new Exception("Could not ID colony owning race : " + raceName);
            Owner.AddColony(this);
            string baseType = xml.SelectNodeText("BaseType");
            if (!string.IsNullOrEmpty(baseType)) Base = (BaseType)Enum.Parse(typeof(BaseType), baseType);
            Base |= BaseType.Outpost; // All colonies have it
            dtLastUpdate = DateTime.FromBinary(long.Parse(xml.SelectNodeText("LastUpdate")));

            string strLastGrowth = xml.SelectNodeText("LastGrowth");
            if (!string.IsNullOrEmpty(strLastGrowth)) dtLastGrowth = DateTime.FromBinary(long.Parse(strLastGrowth));
            else dtLastGrowth = Const.dtStart;

            string strNextGrowth = xml.SelectNodeText("NextGrowth");
            if (!string.IsNullOrEmpty(strNextGrowth)) dtNextGrowth = DateTime.FromBinary(long.Parse(strNextGrowth));
            else if (CanGrow) dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            else dtNextGrowth = DateTime.MaxValue;

            Mercenaries.Clear();
            foreach (XmlNode xm in xml.SelectNodesToList("Mercenaries/Soldier")) {
                Soldier s = new Soldier(xm, null);
                Mercenaries.Add(s);
            }

            Missions.Clear();
            foreach (XmlNode xm in xml.SelectNodesToList("Missions/Soldier")) {
                Mission m = new Mission(xm, loc);
                Missions.Add(m);
            }

            XmlNode? xmli = xml.SelectSingleNode("Inventory");
            Inventory.Clear();
            if (xmli != null) {
                foreach (XmlNode xi in xmli.ChildNodes) {
                    int count = xi.GetAttributeInt("Count");
                    IItem? eq;
                    try {
                        eq = Utils.LoadItem(xi.FirstChild);
                    }
                    catch (Exception) {
                        // Try re-loading as if it's a material
                        if (xi.FirstChild!.Name.Equals("Equipment")) {
                            eq = new Material(xi.FirstChild);
                        }
                        else throw;
                    }
                    if (eq is not null) Inventory.Add(eq, count);
                }
            }
        }

        public void SaveToFile(StreamWriter file) {
            file.WriteLine("<Colony>");
            file.WriteLine(" <BaseType>" + Base.ToString() + "</BaseType>");
            file.WriteLine(" <Owner>" + Owner.Name + "</Owner>");
            file.WriteLine(" <LastUpdate>" + dtLastUpdate.ToBinary() + "</LastUpdate>");
            file.WriteLine(" <LastGrowth>" + dtLastGrowth.ToBinary() + "</LastGrowth>");
            file.WriteLine(" <NextGrowth>" + dtNextGrowth.ToBinary() + "</NextGrowth>");

            if (Mercenaries.Count > 0) {
                file.WriteLine(" <Mercenaries>");
                foreach (Soldier s in Mercenaries) s.SaveToFile(file);
                file.WriteLine(" </Mercenaries>");
            }

            if (Missions.Count > 0) {
                file.WriteLine(" <Missions>");
                foreach (Mission m in Missions) m.SaveToFile(file);
                file.WriteLine(" </Missions>");
            }

            if (Inventory.Count > 0) {
                file.WriteLine(" <Inventory>");
                foreach (IItem it in Inventory.Keys) {
                    file.WriteLine("  <Inv Count=\"" + Inventory[it] + "\">");
                    it.SaveToFile(file);
                    file.WriteLine("  </Inv>");
                }
                file.WriteLine(" </Inventory>");
            }

            file.WriteLine("</Colony>");
        }
        public void ExpandBase(BaseType bt) {
            Base |= bt;
        }
        public int ExpandBase(Random rand) {
            double tdiff = Location.TDiff(Owner);
            if (BaseSize == 5) {
                if (rand.NextDouble() * 50.0 > tdiff) {
                    ExpandBase(BaseType.Metropolis);
                    return 1;
                }
            }
            else {
                BaseType bt = (BaseType)(1 << (rand.Next(5)));
                if ((Base & bt) == 0) {
                    ExpandBase(bt);
                    return 1;
                }
            }
            return 0;
        }
        public void CheckGrowth() {
            if (!CanGrow) return;
            while (Const.dtTime > dtNextGrowth) {
                ForceExpandBase();
                dtLastGrowth = dtNextGrowth;
                dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            }
        }
        private void ForceExpandBase() {
            if (BaseSize == 6) return;
            if (BaseSize == 5) {
                ExpandBase(BaseType.Metropolis);
                return;
            }
            Random rand = new Random(Location.GetHashCode());
            do {
                BaseType bt = (BaseType)(1 << (rand.Next(5)));
                if ((Base & bt) == 0) {
                    ExpandBase(bt);
                    return;
                }
            } while (true);
        }

        // Periodic stock update
        public void UpdateStock(Team t) {
            // Check how much time has elapsed
            TimeSpan ts = Const.dtTime - dtLastUpdate;
            double dDays = ts.TotalDays;

            // Remove any mercenaries with names identical to soldiers in the team
            HashSet<string> hsTeamNames = new HashSet<string>();
            foreach (Soldier s in t.SoldiersRO) hsTeamNames.Add(s.Name);
            List<Soldier> lMercs = new List<Soldier>(Mercenaries);
            foreach (Soldier m in lMercs) {
                if (hsTeamNames.Contains(m.Name)) Mercenaries.Remove(m);
            }

            if (dDays < 0.1) return; // Tiny gap - do nothing
            if (dDays > 400) {
                // Very long gap - just refresh all
                Mercenaries.Clear();
                Missions.Clear();
                Inventory.Clear();
                PopulateInventory(Const.MerchantStockResetDuration);
            }
            else {
                // Intermediate gap - expire some, randomly
                Random rand = new Random();
                for (int n = 0; n <= (int)dDays; n++) {
                    if (Mercenaries.Count > 0 && rand.NextDouble() > 0.95) Mercenaries.RemoveAt(rand.Next(Mercenaries.Count));
                    if (Missions.Count > 0 && rand.NextDouble() > 0.95) Missions.RemoveAt(rand.Next(Missions.Count));
                    UpdateInventory();
                }
            }

            // Now fill the gaps
            PopulateMercenaries();
            PopulateMissions();
            dtLastUpdate = Const.dtTime;
        }
        private void PopulateMissions() {
            Random rand = new Random();
            int total = BaseSize + rand.Next(5) + rand.Next(5) - rand.Next(5);
            if ((Base & BaseType.Military) != 0) total += 2;
            if ((Base & BaseType.Research) != 0) total++;
            if (total > Const.MaxColonyMissions) total = Const.MaxColonyMissions;
            if (total < 0) total = 0;
            if (Missions.Count >= total) return;
            for (int n = 0; n < total; n++) {
                Missions.Add(Mission.CreateRandomColonyMission(this, rand));
            }
        }
        private void PopulateMercenaries() {
            Random rand = new Random();
            int total = BaseSize + rand.Next(4) + rand.Next(4) - rand.Next(4);
            if ((Base & BaseType.Military) != 0) total += 3;
            if ((Base & BaseType.Metropolis) != 0) total += 2;
            if (total > Const.MaxColonyMercenaries) total = Const.MaxColonyMercenaries;
            if (total < 0) total = 0;
            if (Mercenaries.Count >= total) return;
            HashSet<string> hsNames = new HashSet<string>();
            for (int n = 0; n < (total - Mercenaries.Count); n++) {
                int ntries = 0;
                do {
                    Soldier s = Soldier.GenerateRandomMercenary(this, rand);
                    if (!hsNames.Contains(s.Name)) {
                        Mercenaries.Add(s);
                        hsNames.Add(s.Name);
                        break;
                    }
                    ntries++;
                } while (ntries < 10);
            }
        }

        // Fill the inventory with deliveries for the given number of days
        private void PopulateInventory(int days) {
            Random rand = new Random();
            foreach (ItemType eq in StaticData.ItemTypes) {
                double rarity = eq.Rarity * (BaseSize + 1.0) * (BaseSize + 1.0) / 100.0;
                // Modify rarity by colony details & add this item if required
                if ((Base & BaseType.Military) == 0) rarity /= 2.0;
                if ((Base & BaseType.Colony) == 0) rarity /= 2.0;
                if ((Base & BaseType.Metropolis) != 0) rarity *= 2.0;
                if ((Base & BaseType.Research) != 0) rarity = Math.Pow(rarity, 0.7);
                AddItem(new Equipment(eq), rarity, days, rand);
            }

            // Now add all weapons
            foreach (WeaponType wt in StaticData.WeaponTypes) {
                if (!wt.IsUsable) continue;
                for (int Level = 0; Level < 4; Level++) {
                    Weapon wp = new Weapon(wt, Level);
                    double rarity = wp.Rarity * (BaseSize + 1.0) * (BaseSize + 1.0) / 100.0;
                    // Modify rarity by colony details & add this weapon if required
                    if ((Base & BaseType.Colony) == 0) rarity /= 1.5;
                    if ((Base & BaseType.Metropolis) != 0) rarity *= 2.0;
                    if ((Base & BaseType.Military) == 0) rarity *= 2.0;
                    if ((Base & BaseType.Research) != 0) rarity = Math.Pow(rarity, 0.7);
                    AddItem(wp, rarity, days, rand);
                }
            }

            // Now add all armour types
            foreach (ArmourType atp in StaticData.ArmourTypes) {
                foreach (MaterialType mat in StaticData.Materials) {
                    for (int Level = 0; Level < 4; Level++) {
                        if (!mat.IsArmourMaterial) continue;
                        Armour ar = new Armour(atp, mat, Level);
                        double rarity = ar.Rarity * (BaseSize + 1.0) * (BaseSize + 1.0) / 100.0;
                        // Modify rarity by colony details & add this armour if required
                        if ((Base & BaseType.Colony) == 0) rarity /= 1.5;
                        if ((Base & BaseType.Metropolis) != 0) rarity *= 2.0;
                        if ((Base & BaseType.Military) == 0) rarity *= 2.0;
                        if ((Base & BaseType.Research) != 0) rarity = Math.Pow(rarity, 0.7);
                        AddItem(ar, rarity, days, rand);
                    }
                }
            }
        }
        private void AddItem(IItem eq, double rarity, int days, Random rand) {
            // Calculate how many we get
            int count = 0;
            for (int n = 0; n < days; n++) {
                double dRand = rand.NextDouble() * 10.0;
                if (dRand <= rarity) {
                    dRand += 0.1;
                    double frac = (rarity / dRand);
                    int dcount = (int)Math.Floor(frac);
                    double dRemainder = frac - Math.Floor(frac);
                    if (rand.NextDouble() < dRemainder) dcount++;
                    if (dcount < 1) dcount = 1;
                    count += dcount;
                }
            }
            // Adjust the stock accordingly
            if (count == 0) return;
            if (Inventory.ContainsKey(eq)) Inventory[eq] += count;
            else Inventory.Add(eq, count);
        }
        public void RemoveItem(IItem eq) {
            if (!Inventory.ContainsKey(eq)) return;
            Inventory[eq]--;
            if (Inventory[eq] == 0) Inventory.Remove(eq);
        }

        // Update the inventory for one day
        private void UpdateInventory() {
            Random rand = new Random();
            List<IItem> lItems = new List<IItem>(Inventory.Keys);
            foreach (IItem eq in lItems) {
                double dSale = Math.Sqrt((double)Inventory[eq]) * 1.01 * rand.NextDouble();
                int iSale = (int)Math.Floor(dSale);
                Inventory[eq] -= iSale;
                if (Inventory[eq] <= 0) Inventory.Remove(eq);
            }
            PopulateInventory(1);
        }

        // Draw on the system view
        public void DrawBaseIcon(ShaderProgram prog, float scale) {
            GL.Disable(EnableCap.DepthTest);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1f));
            Matrix4 pScaleM = Matrix4.CreateScale(scale * 1.0f);

            // Might be a metropolis
            if (BaseSize == 6) {
                Matrix4 pmScaleM = Matrix4.CreateScale(1.1f);
                Matrix4 pmTranslateM = Matrix4.CreateTranslation(-0.55f, -0.55f, 0f);
                prog.SetUniform("model", pmScaleM * pmTranslateM * pScaleM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Lines.BindAndDraw();
            }

            Matrix4 pTranslateM = Matrix4.CreateTranslation(-0.5f, -0.5f, 0f);
            prog.SetUniform("model", pTranslateM * pScaleM);

            // Square framework
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();

            // Colony type indicators
            Matrix4 piScaleM = Matrix4.CreateScale(0.2f);
            Matrix4 bgScaleM = Matrix4.CreateScale(0.25f);
            Matrix4 piRotateM = Matrix4.CreateRotationZ((float)Math.PI / 4f);
            if ((Base & BaseType.Colony) != 0) {
                Matrix4 piTranslateM = Matrix4.CreateTranslation(-0.5f, -0.5f, 0f);
                prog.SetUniform("model", piRotateM * bgScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
                prog.SetUniform("model", piRotateM * piScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0f, 1f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
            }
            if ((Base & BaseType.Trading) != 0) {
                Matrix4 piTranslateM = Matrix4.CreateTranslation(0.5f, -0.5f, 0f);
                prog.SetUniform("model", piRotateM * bgScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
                prog.SetUniform("model", piRotateM * piScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(1f, 1f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
            }
            if ((Base & BaseType.Research) != 0) {
                Matrix4 piTranslateM = Matrix4.CreateTranslation(-0.5f, 0.5f, 0f);
                prog.SetUniform("model", piRotateM * bgScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
                prog.SetUniform("model", piRotateM * piScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0f, 0f, 1f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
            }
            if ((Base & BaseType.Military) != 0) {
                Matrix4 piTranslateM = Matrix4.CreateTranslation(0.5f, 0.5f, 0f);
                prog.SetUniform("model", piRotateM * bgScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
                prog.SetUniform("model", piRotateM * piScaleM * piTranslateM * pScaleM);
                prog.SetUniform("flatColour", new Vector4(1f, 0f, 0f, 1f));
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.FlatCentred.BindAndDraw();
            }
            GL.Enable(EnableCap.DepthTest);
        }

        // Utility stuff
        public int GetAvailability(IItem eq) {
            if (Inventory.ContainsKey(eq)) return Inventory[eq];
            return 0;
        }
        public void RemoveMission(Mission miss) {
            if (miss == null || Missions == null) return;
            if (!Missions.Contains(miss)) return;
            Missions.Remove(miss);
        }
        public void AddMission(Mission miss) {
            if (miss == null || Missions == null) return;
            if (Missions.Contains(miss)) return;
            Missions.Add(miss);
        }
        public void RemoveMercenary(Soldier merc) {
            if (!Mercenaries.Contains(merc)) throw new Exception("Attemptign to delete non-existent Mercenary");
            Mercenaries.Remove(merc);
        }
        public void ResetMissions() {
            Missions.Clear();
            PopulateMissions();
        }
        public void ResetMercenaries() {
            Mercenaries.Clear();
            PopulateMercenaries();
        }
        public void ResetStock() {
            Inventory.Clear();
            PopulateInventory(Const.MerchantStockResetDuration);
        }
        public bool CanBuildShipType(ShipType tp) {
            if (tp.Weapon > 1 && (Base & BaseType.Military) == 0) return false;
            if (tp.Small > 6 && (Base & BaseType.Trading) == 0) return false;
            if (tp.MaxHull > BaseSize * 15.0) return false;
            return true;
        }
        private int GetNextGrowthPeriod() { // In days
            if (!CanGrow) return 1000000;
            Random rand = new Random(Location.GetHashCode());
            int dt = (int)(BaseSize * BaseSize * (Const.DaysPerYear + 50.0 * rand.NextDouble()));
            dt += rand.Next(250) + rand.Next(250);
            return dt;
        }

        // Iterators
        public IEnumerable<Soldier> MercenariesList() {
            foreach (Soldier s in Mercenaries) {
                yield return s;
            }
        }
        public IEnumerable<Mission> MissionsList() {
            foreach (Mission m in Missions) {
                yield return m;
            }
        }
        public IEnumerable<IItem> InventoryList() {
            foreach (IItem eq in Inventory.Keys) {
                yield return eq;
            }
        }
    }
}
