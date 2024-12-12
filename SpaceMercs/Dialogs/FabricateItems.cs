// 

namespace SpaceMercs.Dialogs {
    partial class FabricateItems : Form {
        private readonly Team PlayerTeam;

        public FabricateItems(Team t) {
            PlayerTeam = t;
            InitializeComponent();
            SetupTabs();
        }

        private void SetupTabs() {
            // Item types in Merchant and Foundry tabs
            cbItemType.Items.Clear();
            cbItemType.Items.Add("All");
            if (PlayerTeam.PlayerShip.HasArmoury) cbItemType.Items.Add("Weapons");
            if (PlayerTeam.PlayerShip.HasArmoury) cbItemType.Items.Add("Armour");
            if (PlayerTeam.PlayerShip.HasMedlab) cbItemType.Items.Add("Medical");
            if (PlayerTeam.PlayerShip.HasWorkshop) cbItemType.Items.Add("Equipment");
            cbItemType.SelectedIndex = 0;
            cbUpgradeItemType.Items.Clear();
            cbUpgradeItemType.Items.Add("All");
            if (PlayerTeam.PlayerShip.HasArmoury) cbUpgradeItemType.Items.Add("Weapons");
            if (PlayerTeam.PlayerShip.HasArmoury) cbUpgradeItemType.Items.Add("Armour");
            if (PlayerTeam.PlayerShip.HasMedlab) cbUpgradeItemType.Items.Add("Medical");
            if (PlayerTeam.PlayerShip.HasWorkshop) cbUpgradeItemType.Items.Add("Equipment");
            cbUpgradeItemType.SelectedIndex = 0;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dgConstruct.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            SetupConstructionDataGrid();
            SetupModifyTab();
        }

        // Setup data grid based on drop down setting
        private void SetupConstructionDataGrid() {
            string strType = (string)cbItemType.SelectedItem;
            dgConstruct.Rows.Clear();
            string strFilter = tbFilter.Text;
            // Add items to list based on construction capacity
            if (PlayerTeam.PlayerShip.HasArmoury) {
                if (strType.Equals("Weapons") || strType.Equals("All")) {
                    foreach (WeaponType tp in StaticData.WeaponTypes.Where(w => w.IsUsable)) {
                        if (tp.IsMeleeWeapon && PlayerTeam.HasSkill(Soldier.UtilitySkill.Bladesmith)) FilterAndAdd(tp, "Melee", strFilter);
                        else if (!tp.IsMeleeWeapon && PlayerTeam.HasSkill(Soldier.UtilitySkill.Gunsmith)) FilterAndAdd(tp, "Gun", strFilter);
                    }
                }
                if (PlayerTeam.HasSkill(Soldier.UtilitySkill.Armoursmith)) {
                    if (strType.Equals("Armour") || strType.Equals("All")) {
                        foreach (ArmourType tp in StaticData.ArmourTypes) {
                            FilterAndAdd(tp, "Armour", strFilter);
                        }
                    }
                }
            }
            if (PlayerTeam.PlayerShip.HasMedlab && PlayerTeam.HasSkill(Soldier.UtilitySkill.Medic)) {
                if (strType.Equals("Medical") || strType.Equals("All")) {
                    foreach (ItemType tp in StaticData.ItemTypes.Where(t => t.Source == ItemType.ItemSource.Medlab)) {
                        FilterAndAdd(tp, "Medical", strFilter);
                    }
                }
            }
            if (PlayerTeam.PlayerShip.HasWorkshop && PlayerTeam.HasSkill(Soldier.UtilitySkill.Engineer)) {
                if (strType.Equals("Equipment") || strType.Equals("All")) {
                    foreach (ItemType tp in StaticData.ItemTypes.Where(t => t.Source == ItemType.ItemSource.Workshop)) {
                        FilterAndAdd(tp, "Equipment", strFilter);
                    }
                }
            }
        }
        private void FilterAndAdd(ItemType thisItem, string strType, string strFilter) {
            if (String.IsNullOrEmpty(strFilter) || thisItem.Name.IndexOf(strFilter, StringComparison.InvariantCultureIgnoreCase) > -1
                                                || strType.IndexOf(strFilter, StringComparison.InvariantCultureIgnoreCase) > -1) {
                string[] arrRow = new string[4];
                arrRow[0] = thisItem.Name;
                arrRow[1] = strType;
                arrRow[2] = thisItem.Mass.ToString("N2") + "kg";
                Race playerRace = StaticData.Races[0];
                if (thisItem.RequiredRace != null && thisItem.RequiredRace != playerRace) return;
                if (!thisItem.CanBuild(playerRace)) return;

                // How many can we build?
                int count = 999;
                foreach (MaterialType mat in thisItem.Materials.Keys) {
                    int req = thisItem.Materials[mat];
                    int num = PlayerTeam.CountMaterial(mat);
                    int amount = num / req;
                    if (amount < count) count = amount;
                }
                // Filter out if count==0 and checkbox says not to display
                if (count == 0 && cbHideUnbuildable.Checked) return;
                // Add this row
                arrRow[3] = count.ToString();
                dgConstruct.Rows.Add(arrRow);
                dgConstruct.Rows[dgConstruct.Rows.Count - 1].Tag = thisItem;
            }
        }
        private void SetupModifyTab() {
            string strType = (string)cbUpgradeItemType.SelectedItem;
            dgInventory.Rows.Clear();
            string[] arrRow = new string[4];
            string strFilter = tbUpgradeFilter.Text;
            foreach (IItem eq in PlayerTeam.Inventory.Keys.Where(x => x is not Material)) {
                if (!PlayerTeam.PlayerShip.CanBuildItem(eq)) continue;
                if (!String.IsNullOrEmpty(strFilter) && !eq.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (strType.Equals("All") || (strType.Equals("Weapons") && eq is Weapon) || (strType.Equals("Armour") && eq is Armour) ||
                   (strType.Equals("Medical") && eq is Equipment eqpm && eqpm.BaseType.Source == ItemType.ItemSource.Medlab) ||
                   (strType.Equals("Equipment") && eq is Equipment eqpe && eqpe.BaseType.Source == ItemType.ItemSource.Workshop)) {
                    arrRow[0] = eq.Name;
                    arrRow[1] = "Ship Stores";
                    arrRow[2] = eq.Cost.ToString("N2");
                    arrRow[3] = PlayerTeam.Inventory[eq].ToString();
                    dgInventory.Rows.Add(arrRow);
                    dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = new Tuple<Soldier?, IItem, bool>(null, eq, false);
                }
            }
            foreach (Soldier s in PlayerTeam.SoldiersRO) {
                foreach (IItem eq in s.InventoryGrouped.Keys.Where(x => x is not Material)) {
                    if (!PlayerTeam.PlayerShip.CanBuildItem(eq)) continue;
                    if (!String.IsNullOrEmpty(strFilter) && !eq.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (strType.Equals("All") || (strType.Equals("Weapons") && eq is Weapon) || (strType.Equals("Armour") && eq is Armour) ||
                       (strType.Equals("Medical") && eq is Equipment eqpm && eqpm.BaseType.Source == ItemType.ItemSource.Medlab) ||
                       (strType.Equals("Equipment") && eq is Equipment eqpe && eqpe.BaseType.Source == ItemType.ItemSource.Workshop)) {
                        arrRow[0] = eq.Name;
                        arrRow[1] = s.Name;
                        arrRow[2] = eq.Cost.ToString("N2");
                        arrRow[3] = s.InventoryGrouped[eq].ToString();
                        dgInventory.Rows.Add(arrRow);
                        dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = new Tuple<Soldier?, IItem, bool>(s, eq, false);
                    }
                }
                if (PlayerTeam.PlayerShip.HasArmoury) {
                    if (PlayerTeam.HasSkill(Soldier.UtilitySkill.Armoursmith)) {
                        foreach (Armour ar in s.EquippedArmour) {
                            if (!String.IsNullOrEmpty(strFilter) && !ar.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                            if (strType.Equals("All") || strType.Equals("Armour")) {
                                arrRow[0] = ar.Name;
                                arrRow[1] = s.Name + " [Eq]";
                                arrRow[2] = ar.Cost.ToString("N2");
                                arrRow[3] = "1";
                                dgInventory.Rows.Add(arrRow);
                                dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = new Tuple<Soldier?, IItem, bool>(s, ar, true);
                            }
                        }
                    }
                    if (s.EquippedWeapon != null && PlayerTeam.PlayerShip.CanBuildItem(s.EquippedWeapon)) {
                        if (!String.IsNullOrEmpty(strFilter) && !s.EquippedWeapon.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                        if (strType.Equals("All") || strType.Equals("Weapon")) {
                            arrRow[0] = s.EquippedWeapon.Name;
                            arrRow[1] = s.Name + " [Eq]";
                            arrRow[2] = s.EquippedWeapon.Cost.ToString("N2");
                            arrRow[3] = "1";
                            dgInventory.Rows.Add(arrRow);
                            dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = new Tuple<Soldier?, IItem, bool>(s, s.EquippedWeapon, true);
                        }
                    }
                }
            }
            if (dgInventory.Rows.Count > 0) dgInventory.Rows[0].Selected = true;

            if (PlayerTeam.PlayerShip.HasEngineering) {
                btModify.Show();
                btImprove.Show();
                lbNoEngineering.Hide();
            }
            else {
                btModify.Hide();
                btImprove.Hide();
                lbNoEngineering.Show();
            }
        }

        // Button clicks
        private void btConstruct_Click(object sender, EventArgs e) {
            if (dgConstruct.SelectedRows.Count == 0) return;
            int iScroll = dgConstruct.FirstDisplayedScrollingRowIndex;
            if (dgConstruct.SelectedRows[0].Tag is not ItemType newType) return;
            int iRowNo = dgConstruct.SelectedRows[0].Index;

            // Make sure we can make it
            Dictionary<MaterialType, int> mats = new Dictionary<MaterialType, int>(newType.Materials);
            string strMissing = "Insufficient materials available. Requirements:";
            bool bOK = true;
            foreach (MaterialType mt in mats.Keys) {
                int num = mats[mt];
                int count = PlayerTeam.CountMaterial(mt);
                if (count < num) {
                    bOK = false;
                    strMissing += "\n" + mt.Name + " (" + count + "/" + num + ")";
                }
            }
            if (!bOK) {
                MessageBox.Show(strMissing);
                return;
            }

            // Select armour material
            MaterialType? armourMat = null;
            if (newType is ArmourType at) {
                // Choose armour material and add it to the list
                SelectArmourMaterial sam = new SelectArmourMaterial(mats, at, PlayerTeam);
                sam.ShowDialog(this);
                if (sam.SelectedMat == null) return; // Cancelled
                armourMat = sam.SelectedMat;
                if (mats.ContainsKey(armourMat)) mats[armourMat] += at.Size;
                else mats.Add(armourMat, at.Size);
            }

            // Get construction chance
            int maxlev = PlayerTeam.GetMaxSkillByItemType(newType);
            Soldier s = PlayerTeam.GetSoldierWithMaxSkillByItemType(newType);
            double chance = newType.ConstructionChance + (maxlev * Const.SkillConstructChanceModifier);
            if (armourMat is not null) chance += armourMat.ConstructionChanceModifier;
            double basechance = chance;
            if (chance > 99.0) chance = 99.0;

            // Are you sure?
            string strReally = "Really construct " + newType.Name + "?\nSoldier = " + s.Name + " (" + maxlev + ")\nChance of success = " + chance.ToString("N2") + "%";
            strReally += "\n\nMaterials:";
            foreach (MaterialType mt in mats.Keys) {
                strReally += mt.Name + " [" + mats[mt] + "]\n";
            }
            if (MessageBox.Show(strReally, "Really Construct?", MessageBoxButtons.YesNo) == DialogResult.No) return;

            // Delete raw materials
            foreach (KeyValuePair<MaterialType, int> tp in mats) {
                PlayerTeam.RemoveMaterial(tp.Key, tp.Value);
            }

            // Check if construction worked ok
            Random rnd = new Random();
            double r = rnd.NextDouble() * 100.0;
            if (r > chance) {
                MessageBox.Show("Construction failed! Base materials have been destroyed.");
                SetupConstructionDataGrid();
                dgConstruct.Rows[iRowNo].Selected = true;
                dgConstruct.FirstDisplayedScrollingRowIndex = iScroll;
                return;
            }

            // Boost item level randomly based on skill of fabricator
            int lvl = 0;
            if (newType is ArmourType || newType is WeaponType) {
                chance = basechance;
                do {
                    if ((lvl + 1) * 5 > maxlev) break;
                    chance -= 50.0 - (lvl * 20.0);
                    chance += (maxlev * 5.0);
                    r = rnd.NextDouble() * 100.0;
                    if (r < chance) lvl++;
                } while (r <= chance);
            }

            if (lvl > 0) MessageBox.Show("Construction succeeded! Quality is " + Utils.LevelToDescription(lvl));
            else MessageBox.Show("Construction succeeded!");

            IEquippable? newItem = newType switch {
                ArmourType at2 => new Armour(at2, armourMat!, lvl),
                WeaponType wt => new Weapon(wt, lvl),
                _ => new Equipment(newType)
            };

            // Add the new item
            PlayerTeam.AddItem(newItem, 1);
            SetupConstructionDataGrid();
            dgConstruct.Rows[iRowNo].Selected = true;
            dgConstruct.FirstDisplayedScrollingRowIndex = iScroll;
        }
        private void btImprove_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows[0].Tag is not Tuple<Soldier?, IItem, bool> tp) return;
            Soldier? s = tp.Item1;
            if (tp.Item2 is not IEquippable eq || eq is Equipment) return;
            bool bEquipped = tp.Item3;
            int maxlev = PlayerTeam.GetMaxSkillByItemType(eq.BaseType);
            if (maxlev == 0) {
                MessageBox.Show("Nobody has the required skill to perform that action!");
                return;
            }
            Soldier ssk = PlayerTeam.GetSoldierWithMaxSkillByItemType(eq.BaseType);
            UpgradeItem ui = new UpgradeItem(eq, 1.0, maxlev, PlayerTeam, ssk);
            ui.ShowDialog(this);
            if (ui.Upgraded) {
                if (s == null) {
                    PlayerTeam.RemoveItemFromStores(eq);
                    if (!ui.Destroyed && ui.NewItem != null) PlayerTeam.AddItem(ui.NewItem);
                }
                else {
                    if (bEquipped) {
                        s.Unequip(eq);
                    }
                    s.DestroyItem(eq);
                    if (!ui.Destroyed && ui.NewItem != null) {
                        s.AddItem(ui.NewItem);
                        if (bEquipped) s.Equip(ui.NewItem);
                    }
                }
                SetupModifyTab();
            }
        }
        private void btDismantle_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows[0].Tag is not Tuple<Soldier?, IItem, bool> tp) {
                MessageBox.Show("Error picking item to dismantle");
                return;
            }
            Soldier? s = tp.Item1;
            IItem it = tp.Item2;
            if (it is not IEquippable eq) return;
            bool bEquipped = tp.Item3;
            if (MessageBox.Show("Really dismantle your " + eq.Name + "? This will destroy it irreversibly!", "Really Dismantle?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            int maxlev = PlayerTeam.GetMaxSkillByItem(it);
            Dictionary<IItem, int> dRemains = Utils.DismantleEquipment(eq as IEquippable, maxlev);
            if (s is null) {
                PlayerTeam.RemoveItemFromStores(eq);
            }
            else {
                if (bEquipped) s.Unequip(eq);
                s.DestroyItem(eq);
            }
            string strRemains = "Materials obtained:";
            foreach (IItem r in dRemains.Keys) {
                PlayerTeam.AddItem(r, dRemains[r]);
                strRemains += "\n" + r.Name + " [" + dRemains[r] + "]";
            }
            if (dRemains.Count == 0) strRemains += "\nNone";
            MessageBox.Show(strRemains);
            SetupModifyTab();
        }
        private void btModify_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows[0].Tag is not Tuple<Soldier?, IItem, bool> tp) return;
            Soldier? s = tp.Item1;
            if (tp.Item2 is not Weapon wp || !wp.Type.Modifiable) return;
            bool bEquipped = tp.Item3;
            int maxLev = PlayerTeam.GetMaxSkillByItemType(wp.BaseType);
            int maxEngLev = PlayerTeam.MaxSkillLevel(Soldier.UtilitySkill.Engineer);
            if (maxLev == 0 || maxEngLev == 0) {
                Soldier.UtilitySkill skillReq = wp.Type.IsMeleeWeapon ? Soldier.UtilitySkill.Bladesmith : Soldier.UtilitySkill.Gunsmith;
                MessageBox.Show($"You do not have the required skills on board to perform that action!\nRequires Engineering and {skillReq}");
                return;
            }
            Soldier ssk = PlayerTeam.GetSoldierWithMaxSkillByItemType(wp.BaseType);
            Soldier sskEng = PlayerTeam.MaxSkillSoldier(Soldier.UtilitySkill.Engineer);
            ModifyWeapon mw = new ModifyWeapon(wp, 1.0, maxLev, maxEngLev, PlayerTeam, ssk, sskEng);
            mw.ShowDialog(this);
            if (mw.Modified) {
                if (s == null) {
                    PlayerTeam.RemoveItemFromStores(wp);
                    if (!mw.Destroyed && mw.NewItem != null) PlayerTeam.AddItem(mw.NewItem);
                }
                else {
                    if (bEquipped) {
                        s.Unequip(wp);
                    }
                    s.DestroyItem(wp);
                    if (!mw.Destroyed && mw.NewItem != null) {
                        s.AddItem(mw.NewItem);
                        if (bEquipped) s.Equip(mw.NewItem);
                    }
                }
                SetupModifyTab();
            }
        }

        // Other handlers
        private void tbFilter_TextChanged(object sender, EventArgs e) {
            SetupConstructionDataGrid();
        }
        private void tbUpgradeFilter_TextChanged(object sender, EventArgs e) {
            SetupModifyTab();
        }
        private void cbItemType_SelectedIndexChanged(object sender, EventArgs e) {
            SetupConstructionDataGrid();
        }
        private void cbUpgradeItemType_SelectedIndexChanged(object sender, EventArgs e) {
            SetupModifyTab();
        }
        private void dgInventory_SelectionChanged(object sender, EventArgs e) {
            btImprove.Enabled = false;
            btModify.Enabled = false;
            btDismantle.Enabled = false;
            if (dgInventory.SelectedRows.Count == 0) return;
            if (dgInventory.SelectedRows[0].Tag is not Tuple<Soldier?, IItem, bool> tp) return;
            IItem it = tp.Item2;
            if (it is not Material) btDismantle.Enabled = true;
            if (!PlayerTeam.PlayerShip.HasEngineering) return;
            if (it is IEquippable eq) {
                if ((eq is Weapon || eq is Armour) && eq.Level < Const.MaxItemLevel) btImprove.Enabled = true;
                if (eq is Weapon wp && wp.Type.Modifiable) btModify.Enabled = true;
            }
        }

        // Changed tab so update lists
        private void tcMain_SelectedIndexChanged(object sender, EventArgs e) {
            int iTabNo = tcMain.SelectedIndex;
            switch (iTabNo) {
                case 0: SetupConstructionDataGrid(); break;
                case 1: SetupModifyTab(); break;
                default: throw new NotImplementedException();
            }
        }

        // DataGridView sort handlers
        private void dgConstruct_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 1) { // Type
                e.SortResult = (e.CellValue1.ToString() ?? string.Empty).CompareTo(e.CellValue2.ToString() ?? string.Empty);
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 2) { // Mass
                e.SortResult = double.Parse(e.CellValue1.ToString()?.Replace("kg", "") ?? string.Empty).CompareTo(double.Parse(e.CellValue2.ToString()?.Replace("kg", "") ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 3) { // Avail
                e.SortResult = int.Parse(e.CellValue1.ToString() ?? string.Empty).CompareTo(int.Parse(e.CellValue2.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
        }
        private void dgInventory_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 2) { // Value
                e.SortResult = double.Parse(e.CellValue1.ToString() ?? string.Empty).CompareTo(double.Parse(e.CellValue2.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 3) { // Avail
                e.SortResult = int.Parse(e.CellValue1.ToString() ?? string.Empty).CompareTo(int.Parse(e.CellValue2.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void cbHideUnbuildable_CheckedChanged(object sender, EventArgs e) {
            SetupConstructionDataGrid();
        }

        private void dgConstruct_DoubleClick(object sender, EventArgs e) {
            if (dgConstruct.SelectedRows.Count != 1) return;
            if (dgConstruct.SelectedRows[0].Tag is not ItemType it) return;
            string desc = $"{it.Name}\n{it.Description}\n\nMaterials Required:\n";
            foreach (MaterialType mat in it.Materials.Keys) {
                int req = it.Materials[mat];
                desc += $"{mat.Name} * {req}\n";
            }
            MessageBox.Show(this, desc);
        }
        private void dgInventory_DoubleClick(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count != 1) return;
            if (dgInventory.SelectedRows[0].Tag is not Tuple<Soldier?, IItem, bool> tp) return;
            if (tp.Item2 is null) return;
            MessageBox.Show(this, tp.Item2.Description, tp.Item2.Name);
        }
    }
}
