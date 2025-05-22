using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;
using System.IO;
using System.Xml;

namespace SpaceMercs {
    public class Colony {
        [Flags]
        public enum BaseType { None = 0x0, Outpost = 0x1, Military = 0x2, Research = 0x4, Colony = 0x8, Trading = 0x10, Metropolis = 0x20 };
        private BaseType Base;
        public int BaseSize {
            get {
                int Count = 0;
                if (HasBaseType(BaseType.Outpost)) Count++;
                if (HasBaseType(BaseType.Military)) Count++;
                if (HasBaseType(BaseType.Research)) Count++;
                if (HasBaseType(BaseType.Colony)) Count++;
                if (HasBaseType(BaseType.Trading)) Count++;
                if (HasBaseType(BaseType.Metropolis)) Count++;
                return Count;
            }
        }
        public Race Owner { get; private set; }
        public HabitableAO Location { get; private set; }
        public double CostModifier {
            get {
                if (Base == BaseType.Outpost) return 2.0;
                double Modifier = 1.6 - (BaseSize * 0.1);
                if (HasBaseType(BaseType.Trading)) Modifier *= 0.95;
                return Modifier;
            }
        }
        public DateTime dtLastGrowth { get; private set; }
        public DateTime dtNextGrowth { get; private set; }
        public double SeedProgress { get; private set; }
        private readonly List<Soldier> Mercenaries = new List<Soldier>();
        private readonly List<Mission> Missions = new List<Mission>();
        private readonly Dictionary<IItem, int> Inventory = new Dictionary<IItem, int>();
        private DateTime dtLastVisit = DateTime.MinValue;

        public bool CanGrow {
            get {
                if (BaseSize == 6) return false;
                if (Location is Moon) return false; // Maximum moon colony size is now clamped at 1
                return true;
            }
        }
        public bool CanSeed { // Seed a new colony?
            get {
                if (Location is Moon) return false;
                if (BaseSize < 3) return false;
                if (!HasBaseType(BaseType.Colony)) return false;
                return true;
            }
        }
        public bool CanRepairShips {
            get {
                return
                    HasBaseType(BaseType.Colony) ||
                    HasBaseType(BaseType.Military) ||
                    HasBaseType(BaseType.Metropolis);
            }
        }
        public bool HasBaseType(BaseType type) {
            if (type == BaseType.None) return true;
            return (Base & type) != 0;
        }

        public Colony(Race rc, int iSize, int seed, HabitableAO loc, GlobalClock clock) {
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
                        if (HasBaseType(bt)) Base |= bt;
                    } while (Base == btOld);
                }
            }
            Location = loc;
            dtLastGrowth = clock.CurrentTime;
            rc.AddColony(this);
            if (CanGrow) dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            else dtNextGrowth = DateTime.MaxValue;
            dtLastVisit = DateTime.MinValue;
        }
        public Colony(XmlNode xml, HabitableAO loc) {
            Location = loc;
            string raceName = xml.SelectNodeText("Owner");
            Owner = StaticData.GetRaceByName(raceName) ?? throw new Exception("Could not ID colony owning race : " + raceName);
            Owner.AddColony(this);
            string baseType = xml.SelectNodeText("BaseType");
            if (loc is Planet && !string.IsNullOrEmpty(baseType)) {
                if (Int32.TryParse(baseType, out int baseInt)) {
                    Base = (BaseType)baseInt;
                }
                else { 
                    Base = (BaseType)Enum.Parse(typeof(BaseType), baseType);
                }
            }
            Base |= BaseType.Outpost; // All colonies have it
            dtLastVisit = DateTime.FromBinary(long.Parse(xml.SelectNodeText("LastUpdate","0")));

            SeedProgress = xml.SelectNodeDouble("SeedProgress", 0d);

            string strLastGrowth = xml.SelectNodeText("LastGrowth", string.Empty);
            if (!string.IsNullOrEmpty(strLastGrowth)) dtLastGrowth = DateTime.FromBinary(long.Parse(strLastGrowth));
            else dtLastGrowth = Const.StartingDate;

            string strNextGrowth = xml.SelectNodeText("NextGrowth", string.Empty);
            if (!string.IsNullOrEmpty(strNextGrowth)) dtNextGrowth = DateTime.FromBinary(long.Parse(strNextGrowth));
            else if (CanGrow) dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            else dtNextGrowth = DateTime.MaxValue;

            Mercenaries.Clear();
            foreach (XmlNode xm in xml.SelectNodesToList("Mercenaries/Soldier")) {
                Soldier s = new Soldier(xm, null);
                Mercenaries.Add(s);
            }

            Missions.Clear();
            foreach (XmlNode xm in xml.SelectNodesToList("Missions/Mission")) {
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
                    catch (Exception ex) {
                        // Try re-loading as if it's a material
                        if (xi.FirstChild!.Name.Equals("Equipment")) {
                            eq = new Material(xi.FirstChild);
                        }
                        else throw new Exception($"Error loading inventory for Colony {Location.Name} : {ex.Message}");
                    }
                    if (eq is not null) Inventory.TryAdd(eq, count);
                }
            }
        }

        public void SaveToFile(StreamWriter file, GlobalClock clock) {
            file.WriteLine("<Colony>");
            if (Base != BaseType.Outpost) file.WriteLine(" <BaseType>" + (int)Base + "</BaseType>");
            file.WriteLine(" <Owner>" + Owner.Name + "</Owner>");
            if (dtLastVisit > DateTime.MinValue) file.WriteLine(" <LastUpdate>" + dtLastVisit.ToBinary() + "</LastUpdate>");
            if (dtLastGrowth > Const.StartingDate) file.WriteLine(" <LastGrowth>" + dtLastGrowth.ToBinary() + "</LastGrowth>");
            if (Location is not Moon) file.WriteLine(" <NextGrowth>" + dtNextGrowth.ToBinary() + "</NextGrowth>");
            if (SeedProgress > 0d) file.WriteLine(" <SeedProgress>" + SeedProgress.ToString("F4") + "</SeedProgress>");

            TimeSpan ts = clock.CurrentTime - dtLastVisit;
            // Only bother writing the inventory if it's likely to be relevant
            if (ts.TotalDays < Const.LongEnoughGapToResetColonyInventory) {
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
            }
            file.WriteLine("</Colony>");
        }
        public void ExpandBase(BaseType bt) {
            Base |= bt;
        }
        public int ExpandBase(Random rand) {
            double tdiff = Location.TDiff(Owner);

            if (Location is Moon) return 0; // Can't expand a moon base

            if (BaseSize == 5) {
                if (rand.NextDouble() * 50.0 > tdiff) {
                    ExpandBase(BaseType.Metropolis);
                    return 1;
                }
            }
            else {
                BaseType bt = (BaseType)(1 << (rand.Next(5)));
                if (!HasBaseType(bt)) {
                    ExpandBase(bt);
                    return 1;
                }
            }
            return 0;
        }
        internal void CheckGrowth(GlobalClock clock) {
            if (!CanGrow) return;
            while (clock.CurrentTime > dtNextGrowth) {
                ForceExpandBase();
                Location.GetSystem().CheckBuildTradeRoutes(Owner);
                dtLastGrowth = dtNextGrowth;
                dtNextGrowth = dtLastGrowth + TimeSpan.FromDays(GetNextGrowthPeriod());
            }
        }
        public void UpdateGrowthProgress(TimeSpan tDiff, GlobalClock clock) {
            dtNextGrowth -= tDiff;
            CheckGrowth(clock);
        }
        internal void UpdateSeedProgress(GUIMessageBox msgBox, TimeSpan tDiff, GlobalClock clock) {
            if (!CanSeed) return;
            Random rand = new Random();

            // Colony growth rate and seeding should slow down as pop gets larger, or else it will get exponential
            SeedProgress += BaseSize * Const.ColonySeedRate * tDiff.TotalSeconds * 50d / (Const.SecondsPerYear * Math.Max(10, Owner.Population));
            if (SeedProgress >= Const.ColonySeedTarget) {
                if (Location.GetSystem().MaybeAddNewColony(Owner, rand, clock)) {
                    // Built a new colony in this system
                    if (Owner.IsPlayer) {
                        string sysName = Location.GetSystem().Name;
                        if (string.IsNullOrEmpty(sysName)) sysName = Location.GetSystem().PrintCoordinates();
                        msgBox.PopupMessage($"The {Owner.Name} Race has founded a new colony in system {sysName}");
                    }
                    SeedProgress = 0d;
                }
                else SeedProgress = Const.ColonySeedTarget * 0.8; // No seeding this time, so step back a bit
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
                if (!HasBaseType(bt)) {
                    ExpandBase(bt);
                    return;
                }
            } while (true);
        }

        // Periodic stock update
        public void UpdateStock(Team t, GlobalClock clock) {
            // Check how much time has elapsed
            TimeSpan ts = clock.CurrentTime - dtLastVisit;
            double dDays = ts.TotalDays;

            // Larger colonies have a more rapid turnover; small colonies don't            
            dDays = (dDays * BaseSize) / 2d;

            if (dDays < 0.1) return; // Tiny gap - do nothing
            if (dDays > Const.LongEnoughGapToResetColonyInventory) {
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
            PopulateMissions(t);
            dtLastVisit = clock.CurrentTime;

            // Remove any mercenaries with names identical to soldiers in the team
            HashSet<string> hsTeamNames = new HashSet<string>();
            foreach (Soldier s in t.SoldiersRO) hsTeamNames.Add(s.Name);
            List<Soldier> lMercs = new List<Soldier>(Mercenaries);
            foreach (Soldier m in lMercs) {
                if (hsTeamNames.Contains(m.Name)) Mercenaries.Remove(m);
            }
        }
        private void PopulateMissions(Team playerTeam) {
            Random rand = new Random();
            int total = BaseSize + rand.Next(3) + rand.Next(3) + 2;
            if (HasBaseType(BaseType.Military)) total += 2;
            if (HasBaseType(BaseType.Research)) total++;
            if (total > Const.MaxColonyMissions) total = Const.MaxColonyMissions;
            if (total < 0) total = 0;
            if (Missions.Count >= total) return;
            for (int n = 0; n < total; n++) {
                Missions.Add(Mission.CreateRandomColonyMission(this, rand, playerTeam));
            }
        }
        private void PopulateMercenaries() {
            Random rand = new Random();
            int total = BaseSize + BaseSize + rand.Next(2) + rand.Next(2) - 1;
            if (HasBaseType(BaseType.Military)) total += 4 + rand.Next(2) + rand.Next(2);
            if (HasBaseType(BaseType.Metropolis)) total += 2 + rand.Next(2) + rand.Next(2);
            if (total > Const.MaxColonyMercenaries) total = Const.MaxColonyMercenaries;
            if (total < 0) total = 0;
            if (Mercenaries.Count >= total) return;
            HashSet<string> hsNames = new HashSet<string>();
            int newCount = total - Mercenaries.Count;
            for (int n = 0; n < newCount; n++) {
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
        public int GetRandomMissionDifficulty(Random rand) {
            int diff = Location.GetRandomMissionDifficulty(rand);
            return diff;
        }
        public int GetAverageMissionExperience() {
            // Get the average mission difficulty by faking a random variable
            int diff = Location.GetRandomMissionDifficulty(() => 0.5);
            return Mission.GetAverageMissionExperience(diff);
        }

        // Fill the inventory with deliveries for the given number of days
        private void PopulateInventory(int days) {
            Random rand = new Random();
            int civSize = Owner.Population;

            // Add specific utility items
            foreach (ItemType eq in StaticData.ItemTypes.Where(it => it.CanBuild(Owner))) {
                double rarity = eq.Rarity * BaseSize * (BaseSize + 1.0) / 100.0;
                // Modify rarity by colony details & add this item if required
                if (!HasBaseType(BaseType.Military)) rarity /= 2d;
                if (HasBaseType(BaseType.Metropolis)) rarity *= 2d;
                if (HasBaseType(BaseType.Trading)) rarity = Math.Pow(rarity, 0.7d);
                if (HasBaseType(BaseType.Research)) rarity = Math.Pow(rarity, 0.7d);
                AddSuitableNumberOfItems(new Equipment(eq), rarity, days, rand);
            }

            // Now add all weapons
            foreach (WeaponType wt in StaticData.WeaponTypes.Where(it => it.CanBuild(Owner) && (it.Requirements?.MinLevel??1) < BaseSize*4)) {
                if (!wt.IsUsable) continue;
                for (int Level = 0; Level < Math.Min(BaseSize, 3) ; Level++) {
                    Weapon wp = new Weapon(wt, Level);
                    double rarity = wp.Rarity * BaseSize * (BaseSize + 1.0) / 100.0;
                    // Modify rarity by colony details & add this weapon if required
                    if (!HasBaseType(BaseType.Colony)) rarity /= 1.5d;
                    if (HasBaseType(BaseType.Metropolis)) rarity *= 2d;
                    if (!HasBaseType(BaseType.Military)) rarity /= 4d;
                    if (HasBaseType(BaseType.Trading)) rarity = Math.Pow(rarity, 0.7d);
                    if (HasBaseType(BaseType.Research)) rarity = Math.Pow(rarity, 0.7d);
                    AddSuitableNumberOfItems(wp, rarity, days, rand);
                }
            }

            // Now add all armour types
            int maxLev = BaseSize + 1;
            if (HasBaseType(BaseType.Trading)) maxLev++;
            foreach (ArmourType atp in StaticData.ArmourTypes.Where(it => it.CanBuild(Owner) && (it.Requirements?.MinLevel ?? 1) < BaseSize * 4)) {
                foreach (MaterialType mat in StaticData.Materials.Where(mat => mat.CanBuild(Owner) && mat.MaxLevel <= maxLev && mat.MaxLevel <= BaseSize + 1)) {
                    if (!mat.IsArmourMaterial) continue;
                    if (mat.IsScavenged) continue;
                    if (mat.MaxLevel < atp.MinMatLvl) continue;
                    for (int Level = 0; Level < Math.Min(BaseSize, Math.Min(3, mat.MaxLevel)); Level++) {
                        Armour ar = new Armour(atp, mat, Level);
                        double rarity = ar.Rarity * (BaseSize + 1.0) * (BaseSize + 1.0) / 100.0;
                        // Modify rarity by colony details & add this armour if required
                        if (!HasBaseType(BaseType.Military)) rarity /= 4d;
                        if (HasBaseType(BaseType.Metropolis)) rarity *= 2.0;
                        if (HasBaseType(BaseType.Research)) rarity = Math.Pow(rarity, 0.7);
                        if (HasBaseType(BaseType.Trading)) rarity = Math.Pow(rarity, 0.7d);
                        AddSuitableNumberOfItems(ar, rarity, days, rand);
                    }
                }
            }

            // Add some raw materials
            double matScale = 1d;
            if (HasBaseType(BaseType.Metropolis)) matScale += 1d;
            bool trading = HasBaseType(BaseType.Trading);
            if (trading) matScale += 3d;
            matScale += (double)(BaseSize * BaseSize) / 8d;
            double rarityExponent = 1d - (trading ? 0.2d : 0d) - ((double)BaseSize / 40d);
            foreach (MaterialType mat in StaticData.Materials.Where(mat => mat.CanBuild(Owner))) {
                if (mat.IsScavenged) continue;
                double rarity = Math.Pow(mat.Rarity, rarityExponent);
                AddSuitableNumberOfItems(new Material(mat), rarity * matScale, days, rand);
            }
        }
        private void AddSuitableNumberOfItems(IItem eq, double rarity, int days, Random rand) {
            // Calculate how many we get
            int count = 0;
            for (int n = 0; n < days; n++) {
                double dRand = rand.NextDouble() * 10d;
                if (dRand <= rarity) {
                    dRand += 0.01;
                    double frac = (rarity / dRand);
                    int dcount = (int)Math.Floor(frac);
                    if (eq is Equipment eqp && eqp.Level > 3) dcount = 1; // Don't get multiples of very rare objects
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
            if (HasBaseType(BaseType.Colony)) {
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
            if (HasBaseType(BaseType.Trading)) {
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
            if (HasBaseType(BaseType.Research)) {
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
            if (HasBaseType(BaseType.Military)) {
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
            if (Inventory.TryGetValue(eq, out int count)) return count;
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
        public void ResetMissions(Team playerTeam) {
            Missions.Clear();
            PopulateMissions(playerTeam);
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
            if (tp.Weapon > 1 && !HasBaseType(BaseType.Military)) return false;
            if (tp.Small > 6 && !HasBaseType(BaseType.Trading)) return false;
            if (tp.MaxHull > BaseSize * 15.0) return false;
            if (tp.RequiredRace != null && tp.RequiredRace != Owner) return false;
            return true;
        }
        private int GetNextGrowthPeriod() { // In days
            if (!CanGrow) return (int)Const.Million; // i.e. never

            // Repeatable random seed
            Random rand = new Random(Location.GetHashCode() + BaseSize);

            // Base amount of time, in days, before the next growth
            double dt = Math.Pow(BaseSize, Const.GrowthExponent) * (Const.DaysPerYear * (Const.GrowthScale + rand.NextDouble()));

            // Calculate temperature diff from ideal, abs value, doubled for +ve because hotter temperatures get difficult more quickly
            double tdiff = Location.TDiff(Owner);
            
            // Tougher to grow if far from ideal temp
            if (tdiff > Const.GrowthTempOffset) {
                dt *= Math.Pow(Const.GrowthTempBase, (tdiff - Const.GrowthTempOffset) / Const.GrowthTempScale);
            }

            // Much easier to grow with trade routes set up
            if (Location.GetSystem().TradeRoutes.Any()) {
                dt *= Const.TradeRouteColonyGrowthRate;
            }

            // Slow down as the civ gets larger, or growth will get far too rapid
            dt *= Math.Sqrt(Math.Max(40, Owner.Population) / 40d);

            return (int)dt;
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
