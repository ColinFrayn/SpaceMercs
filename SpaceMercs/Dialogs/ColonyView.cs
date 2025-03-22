using System.Text;

namespace SpaceMercs.Dialogs {
    partial class ColonyView : Form {
        private readonly Team PlayerTeam;
        private readonly Colony cl;
        private readonly double PriceMod = 1.0; // Price modifier based on the colony owner relations with the team
        private readonly int Relations;
        private readonly Color clExists = Color.Black;
        private readonly Color clDoesntExist = Color.LightGray;
        private readonly Func<Mission, bool> StartMission;

        private record SaleItem {
            public IItem? Item { get; private set; }
            public Soldier? Soldier { get; private set; }
            public bool Equipped { get; private set; }
            public int Count { get; private set; }
            public SaleItem(IItem it, Soldier? s, bool eq, int c) {
                Item = it;
                Soldier = s;
                Equipped = eq;
                Count = c;
            }
        }
        class SaleItemEqualityComparer : IEqualityComparer<SaleItem> {
            public bool Equals(SaleItem? b1, SaleItem? b2) {
                if (ReferenceEquals(b1, b2)) return true;

                if (b2 is null || b1 is null) return false;

                return b1.Item == b2.Item
                    && b1.Soldier == b2.Soldier
                    && b1.Equipped == b2.Equipped;
            }
            public int GetHashCode(SaleItem si) {
                return HashCode.Combine(si.Item, si.Soldier, si.Equipped);
            }
        }

        public ColonyView(Team t, Func<Mission, bool> _StartMission, GlobalClock clock) {
            PlayerTeam = t;
            StartMission = _StartMission;
            if (PlayerTeam.CurrentPosition is not HabitableAO hao || hao.Colony is null) throw new Exception("Null colony in ColonyView!");
            cl = hao.Colony;
            cl.UpdateStock(PlayerTeam, clock); // Make sure we have updated everything since the last time we visited
            PriceMod = PlayerTeam.GetPriceModifier(cl.Owner, cl.Location.GetSystem());
            Relations = PlayerTeam.GetRelations(cl.Owner);
            InitializeComponent();
            SetupTabs();
            lbImproveRelations.Text = $"You must improve relations with the {cl.Owner.Name} race first!";
        }

        private void SetupTabs() {
            // Item types in Merchant and Foundry tabs
            cbItemType.Items.Clear();
            cbItemType.Items.Add("All");
            cbItemType.Items.Add("Weapons");
            cbItemType.Items.Add("Armour");
            cbItemType.Items.Add("Medical");
            cbItemType.Items.Add("Equipment");
            cbItemType.Items.Add("Materials");
            cbItemType.SelectedIndex = 0;
            cbUpgradeItemType.Items.Add("All");
            cbUpgradeItemType.Items.Add("Weapons");
            cbUpgradeItemType.Items.Add("Armour");
            cbUpgradeItemType.Items.Add("Medical");
            cbUpgradeItemType.Items.Add("Equipment");
            cbUpgradeItemType.Items.Add("Materials");
            cbUpgradeItemType.Items.Add("Mission Items");
            cbUpgradeItemType.SelectedIndex = 0;

            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dgMerchant.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            SetupMerchantDataGrid();
            SetupMercenariesTab();
            SetupMissionsTab();
            SetupShipsTab();
            SetupFoundryTab();
        }

        // Setup data grid based on drop down setting
        private void SetupMerchantDataGrid() {
            if (Relations < -2) {
                tpMerchant.Hide();
                lbImproveRelations.Show();
                return;
            }
            tpMerchant.Show();
            lbImproveRelations.Hide();
            string? strType = cbItemType?.SelectedItem?.ToString() ?? string.Empty;
            bool bAffordable = cbAffordable.Checked;
            dgMerchant.Rows.Clear();
            string[] arrRow = new string[4];
            string strFilter = tbFilter.Text;
            foreach (IItem eq in cl.InventoryList()) {
                if (!string.IsNullOrEmpty(strFilter) && !eq.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (strType.Equals("All") || (strType.Equals("Weapons") && eq is Weapon) || (strType.Equals("Armour") && eq is Armour) ||
                   (strType.Equals("Medical") && eq is Equipment eqp && eqp.BaseType.Source == ItemType.ItemSource.Medlab) ||
                   (strType.Equals("Materials") && eq is Material) ||
                   (strType.Equals("Equipment") && eq is Equipment eqp2 && eqp2.BaseType.Source == ItemType.ItemSource.Workshop)) {
                    double cost = cl.CostModifier * eq.Cost * PriceMod;
                    if (cost > PlayerTeam.Cash && bAffordable) continue;
                    arrRow[0] = eq.Name;
                    arrRow[1] = cost.ToString("N2");
                    arrRow[2] = eq.Mass.ToString("N2") + "kg";
                    int count = cl.GetAvailability(eq);
                    arrRow[3] = count > 999 ? "999" : count.ToString();
                    dgMerchant.Rows.Add(arrRow);
                    dgMerchant.Rows[dgMerchant.Rows.Count - 1].Tag = eq;
                }
            }
            lbTeamCashMerch.Text = PlayerTeam.Cash.ToString("N2") + "cr";
            btBuyMerchant.Enabled = dgMerchant.Rows.Count > 0;
            if (dgMerchant.SortOrder != SortOrder.None && dgMerchant.SortedColumn is not null) {
                dgMerchant.Sort(dgMerchant.SortedColumn, dgMerchant.SortOrder == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            }
        }
        private void SetupMercenariesTab() {
            dgMercenaries.Rows.Clear();
            if (Relations < 0) {
                tpMercenaries.Hide();
                lbImproveRelations.Show();
                lbImproveRelations.Text = $"You must improve relations with the {cl.Owner.Name} race first!";
                return;
            }
            tpMercenaries.Show();
            lbImproveRelations.Hide();
            string[] arrRowMerc = new string[4];
            foreach (Soldier merc in cl.MercenariesList()) {
                arrRowMerc[0] = merc.Name;
                arrRowMerc[1] = merc.Level.ToString();
                arrRowMerc[2] = merc.Race.Name;
                arrRowMerc[3] = (merc.HireCost() * PriceMod).ToString("N2");
                dgMercenaries.Rows.Add(arrRowMerc);
                dgMercenaries.Rows[dgMercenaries.Rows.Count - 1].Tag = merc;
            }
            lbTeamCashMercs.Text = PlayerTeam.Cash.ToString("N2") + "cr";
            btHire.Enabled = dgMercenaries.Rows.Count > 0;
        }
        private void SetupMissionsTab() {
            dgMissions.Rows.Clear();
            string[] arrRowMiss = new string[6];
            foreach (Mission miss in cl.MissionsList()) {
                arrRowMiss[0] = miss.Summary;
                arrRowMiss[1] = Utils.MissionGoalToString(miss.Goal);
                string secondary = miss.SecondaryEnemy is null ? string.Empty : " [+]";
                if (miss.RacialOpponent != null) arrRowMiss[2] = miss.RacialOpponent.Name + " " + miss.PrimaryEnemy.Name + secondary;
                else arrRowMiss[2] = miss.PrimaryEnemy.Name + miss.SwarmLevelText + secondary;
                arrRowMiss[3] = miss.Diff.ToString();
                arrRowMiss[4] = Utils.MapSizeToDescription(miss.Size) + (miss.LevelCount > 1 ? " * " + miss.LevelCount.ToString() : "");
                arrRowMiss[5] = miss.Goal == Mission.MissionGoal.Gather ? "Variable" : miss.Reward.ToString("N2") + "cr";
                dgMissions.Rows.Add(arrRowMiss);
                dgMissions.Rows[dgMissions.Rows.Count - 1].Tag = miss;
            }
            btAccept.Enabled = dgMissions.Rows.Count > 0;
        }
        private void SetupShipsTab() {
            dgShips.Rows.Clear();
            if (Relations < 1) {
                tpShips.Hide();
                lbImproveRelations.Show();
                lbImproveRelations.Text = $"You must improve relations with the {cl.Owner.Name} race first!";
                return;
            }
            tpShips.Show();
            lbImproveRelations.Hide();

            double SalvageValue = PlayerTeam.PlayerShip.CalculateSalvageValue();
            string[] arrRowShip = new string[3];
            foreach (ShipType st in StaticData.ShipTypes) {
                if (st == PlayerTeam.PlayerShip.Type) continue;
                if (!cl.CanBuildShipType(st)) continue;
                arrRowShip[0] = st.Name;
                arrRowShip[1] = st.RoomConfigString;
                arrRowShip[2] = ((st.Cost * PriceMod) - SalvageValue).ToString("N2");
                dgShips.Rows.Add(arrRowShip);
                dgShips.Rows[dgShips.Rows.Count - 1].Tag = st;
            }
            lbTeamCashShips.Text = PlayerTeam.Cash.ToString("N2") + "cr";
            btUpgrade.Enabled = dgShips.Rows.Count > 0;

        }
        private void SetupFoundryTab(List<SaleItem>? tpLast = null) {
            if (Relations < -1) {
                tpUpgrade.Hide();
                lbImproveRelations.Show();
                lbImproveRelations.Text = $"You must improve relations with the {cl.Owner.Name} race first!";
                return;
            }
            tpUpgrade.Show();
            lbImproveRelations.Hide();
            HashSet<SaleItem> hsLast;
            if (tpLast == null) hsLast = new HashSet<SaleItem>(new SaleItemEqualityComparer());
            else hsLast = new HashSet<SaleItem>(tpLast, new SaleItemEqualityComparer());
            string strType = cbUpgradeItemType.SelectedItem?.ToString() ?? throw new Exception("Could not identify selected item string");
            List<DataGridViewRow> lSelected = new List<DataGridViewRow>();
            int scroll = dgInventory.FirstDisplayedScrollingRowIndex;
            bool bIncludeEquipped = cbEquipped.Checked;

            dgInventory.Rows.Clear();
            string[] arrRow = new string[4];
            string strFilter = tbUpgradeFilter.Text;
            foreach (IItem eq in PlayerTeam.Inventory.Keys) {
                if (!string.IsNullOrEmpty(strFilter) && !eq.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                if (ShouldDisplayInFoundry(strType, eq)) {
                    arrRow[0] = eq.Name;
                    arrRow[1] = "Ship Stores";
                    arrRow[2] = (eq.Cost * Const.SellDiscount * cl.CostModifier / PriceMod).ToString("N2");
                    arrRow[3] = PlayerTeam.Inventory[eq].ToString();
                    dgInventory.Rows.Add(arrRow);
                    SaleItem si = new SaleItem(eq, null, false, PlayerTeam.Inventory[eq]);
                    dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = si;
                    if (hsLast.Contains(si)) lSelected.Add(dgInventory.Rows[dgInventory.Rows.Count - 1]);
                }
            }
            foreach (Soldier s in PlayerTeam.SoldiersRO) {
                foreach (IItem eq in s.InventoryGrouped.Keys) {
                    if (!string.IsNullOrEmpty(strFilter) && !eq.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                    if (ShouldDisplayInFoundry(strType, eq)) {
                        arrRow[0] = eq.Name;
                        arrRow[1] = s.Name;
                        arrRow[2] = (eq.Cost * Const.SellDiscount * cl.CostModifier / PriceMod).ToString("N2");
                        arrRow[3] = s.InventoryGrouped[eq].ToString();
                        dgInventory.Rows.Add(arrRow);
                        SaleItem si = new SaleItem(eq, s, false, s.InventoryGrouped[eq]);
                        dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = si;
                        if (hsLast.Contains(si)) lSelected.Add(dgInventory.Rows[dgInventory.Rows.Count - 1]);
                    }
                }
                if (bIncludeEquipped) {
                    foreach (Armour ar in s.EquippedArmour) {
                        if (!string.IsNullOrEmpty(strFilter) && !ar.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                        if (strType.Equals("All") || strType.Equals("Armour")) {
                            arrRow[0] = ar.Name;
                            arrRow[1] = s.Name + " [Eq]";
                            arrRow[2] = (ar.Cost * Const.SellDiscount * cl.CostModifier / PriceMod).ToString("N2");
                            arrRow[3] = "1";
                            dgInventory.Rows.Add(arrRow);
                            SaleItem si = new SaleItem(ar, s, true, 1);
                            dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = si;
                            if (hsLast.Contains(si)) lSelected.Add(dgInventory.Rows[dgInventory.Rows.Count - 1]);
                        }
                    }
                    if (s.EquippedWeapon != null) {
                        if (!string.IsNullOrEmpty(strFilter) && !s.EquippedWeapon.Name.Contains(strFilter, StringComparison.InvariantCultureIgnoreCase)) continue;
                        if (strType.Equals("All") || strType.Equals("Weapon")) {
                            arrRow[0] = s.EquippedWeapon.Name;
                            arrRow[1] = s.Name + " [Eq]";
                            arrRow[2] = (s.EquippedWeapon.Cost * Const.SellDiscount * cl.CostModifier / PriceMod).ToString("N2");
                            arrRow[3] = "1";
                            dgInventory.Rows.Add(arrRow);
                            SaleItem si = new SaleItem(s.EquippedWeapon, s, true, 1);
                            dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = si;
                            if (hsLast.Contains(si)) lSelected.Add(dgInventory.Rows[dgInventory.Rows.Count - 1]);
                        }
                    }
                }
            }
            dgInventory.ClearSelection();
            foreach (DataGridViewRow row in lSelected) row.Selected = true;
            if (scroll >= 0 && scroll < dgInventory.Rows.Count) dgInventory.FirstDisplayedScrollingRowIndex = scroll;
            lbTeamCashFoundry.Text = PlayerTeam.Cash.ToString("N2") + "cr";
            if (dgInventory.SortOrder != SortOrder.None && dgInventory.SortedColumn is not null) {
                dgInventory.Sort(dgInventory.SortedColumn, dgInventory.SortOrder == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            }
        }

        private static bool ShouldDisplayInFoundry(string strType, IItem eq) {
            if (strType.Equals("All")) return true;
            if (strType.Equals("Weapons") && eq is Weapon) return true;
            if (strType.Equals("Armour") && eq is Armour) return true;
            if (strType.Equals("Materials") && eq is Material) return true;
            if (strType.Equals("Mission Items") && eq is MissionItem) return true;
            if (strType.Equals("Medical") && eq is Equipment eq2 && eq2.BaseType.Source == ItemType.ItemSource.Medlab) return true;
            if (strType.Equals("Equipment") && eq is Equipment eq3 && eq3.BaseType.Source == ItemType.ItemSource.Workshop) return true;
            return false;
        }

        // Button clicks
        private void btBuyFromMerchant_Click(object sender, EventArgs e) {
            if (dgMerchant.SelectedRows.Count == 0) return;
            int iScroll = dgMerchant.SelectedRows[0].Index - dgMerchant.FirstDisplayedScrollingRowIndex;
            if (dgMerchant.SelectedRows[0].Tag is not IItem eq) return;
            double Cost = cl.CostModifier * eq.Cost * PriceMod;
            if (Cost > PlayerTeam.Cash) {
                MessageBox.Show("You cannot afford to buy that item!");
                return;
            }
            // Buy it
            PlayerTeam.Cash -= Cost;
            PlayerTeam.AddItem(eq, 1);
            SoundEffects.PlaySound("CashRegister");
            cl.RemoveItem(eq);
            // Reinitialise the data grid after purchase to reflect the new state
            SetupMerchantDataGrid();
            // Re-highlight the correct row, if it still exists (might have bought the last one, or only showing affordable and can no longer afford)
            foreach (DataGridViewRow row in dgMerchant.Rows) {
                if (row.Tag is IItem it) {
                    if (it.Name == eq.Name) {
                        row.Selected = true;
                        dgMerchant.FirstDisplayedScrollingRowIndex = Math.Max(row.Index - iScroll, 0);
                        break;
                    }
                }
            }
        }
        private void btHireMercenary_Click(object sender, EventArgs e) {
            if (dgMercenaries.SelectedRows.Count == 0) return;
            if (dgMercenaries.SelectedRows[0].Tag is not Soldier merc) return;
            double Cost = merc.HireCost() * PriceMod;
            if (Cost > PlayerTeam.Cash) {
                MessageBox.Show("You cannot afford to hire that soldier!");
                return;
            }

            // Check if we have enough berths
            int nSpace = PlayerTeam.GetSpareBerths();
            if (nSpace < 1) {
                if (MessageBox.Show("You don't have sufficient accommodation for this new soldier, so you won't be able to take off. Continue anyway?", "No More Space", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            // Recruit the soldier
            MessageBox.Show("You have hired the soldier " + merc.Name);
            cl.RemoveMercenary(merc);
            PlayerTeam.AddSoldier(merc);
            PlayerTeam.Cash -= Cost;
            SoundEffects.PlaySound("CashRegister");
            SetupMercenariesTab();
        }
        private void btRunMission_Click(object sender, EventArgs e) {
            if (dgMissions.SelectedRows.Count == 0) return;
            if (dgMissions.SelectedRows[0].Tag is not Mission miss) return;
            if (StartMission(miss)) {
                cl.RemoveMission(miss);
                this.Close();
            }
        }
        private void btUpgradeShip_Click(object sender, EventArgs e) {
            if (dgShips.SelectedRows.Count == 0) return;
            double SalvageValue = Math.Round(PlayerTeam.PlayerShip.CalculateSalvageValue(), 2);
            if (dgShips.SelectedRows[0].Tag is not ShipType st) {
                MessageBox.Show("Null ship type");
                return;
            }

            if (st == PlayerTeam.PlayerShip.Type) {
                MessageBox.Show("This is the same as the ship you already own!");
                return;
            }

            double Cost = Math.Round((st.Cost * PriceMod) - SalvageValue, 2);
            if (Cost > PlayerTeam.Cash) {
                MessageBox.Show("You cannot afford to upgrade to that model!");
                return;
            }

            // Downgrading to a less valuable ship type. WTF?
            if (PlayerTeam.PlayerShip.Type.Cost > st.Cost) {
                if (MessageBox.Show("This would be a downgrade. Your current ship is already better. Continue anyway?", "Really Downgrade?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            if (MessageBox.Show("Really upgrade your ship to a " + st.Name + "? Total Cost = " + Cost + " credits (includes " + SalvageValue + " credits salvage value from existing ship)", "Really Buy Ship?", MessageBoxButtons.YesNo) == DialogResult.No) return;

            // Do the upgrade
            PlayerTeam.SetTeamShip(st);

            PlayerTeam.Cash -= Cost;
            SoundEffects.PlaySound("CashRegister");
            SetupShipsTab();
        }
        private void btImproveWeaponOrArmour_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count != 1) return;
            if (dgInventory.SelectedRows[0].Tag is not SaleItem tp || tp.Item is null) return;
            Soldier? s = tp.Soldier;
            if (tp.Item is not IEquippable eq) return;
            bool bEquipped = tp.Equipped;
            int relations = PlayerTeam.GetRelations(cl.Owner);
            int lvl = (cl.BaseSize * 2) + relations - 2;
            UpgradeItem ui = new UpgradeItem(eq, PriceMod * cl.CostModifier, lvl, 0, PlayerTeam, null);
            ui.ShowDialog(this.Owner);
            if (ui.Upgraded) {
                if (s is null) {
                    PlayerTeam.RemoveItemFromStores(eq);
                    if (!ui.Destroyed && ui.NewItem != null) PlayerTeam.AddItem(ui.NewItem);
                }
                else {
                    if (bEquipped) s.Unequip(eq);
                    s.DestroyItem(eq);
                    if (!ui.Destroyed && ui.NewItem != null) {
                        s.AddItem(ui.NewItem);
                        if (bEquipped) s.Equip(ui.NewItem);
                    }
                }
                if (ui.Destroyed) SetupFoundryTab();
                else if (ui.NewItem is not null) SetupFoundryTab(new List<SaleItem>() { new SaleItem(ui.NewItem, s, bEquipped, 1) });
            }
        }
        private void btSellItem_Click(object sender, EventArgs e) {
            SellSelectedItems(false);
        }
        private void btSellAll_Click(object sender, EventArgs e) {
            SellSelectedItems(true);
        }
        private void btDismantleEquippable_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count == 0) return;
            // Get a list of everything to sell
            SaleItem tpFirst = dgInventory.SelectedRows[0].Tag as SaleItem ?? throw new Exception("No item selected for dismantling");
            if (tpFirst.Item is null) throw new Exception("Selected row has no item associated with it");
            // Ask if the player is sure
            if (dgInventory.SelectedRows.Count == 1) {
                if (MessageBox.Show($"Really dismantle your {tpFirst.Item.Name}? This will destroy it irreversibly!", "Really Dismantle?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            else {
                if (MessageBox.Show($"Really dismantle these {dgInventory.SelectedRows.Count} items? This will destroy them irreversibly!", "Really Dismantle?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            Dictionary<IItem, int> dRemains = new Dictionary<IItem, int>();
            List<SaleItem> lSI = new List<SaleItem>();
            int count = 0;
            int relations = PlayerTeam.GetRelations(cl.Owner);
            int lvl = (cl.BaseSize * 2) + relations - 2;
            foreach (DataGridViewRow row in dgInventory.SelectedRows) {
                if (row.Tag is not SaleItem tp || tp.Item is not IEquippable eqp) return;
                lSI.Add(tp);
                Soldier? s = tp.Soldier;
                IItem? eq = tp.Item;
                bool bEquipped = tp.Equipped;
                Dictionary<IItem, int> dr = Utils.DismantleEquipment(eqp, lvl);
                foreach (IItem it in dr.Keys) {
                    if (dRemains.ContainsKey(it)) dRemains[it] += dr[it];
                    else dRemains.Add(it, dr[it]);
                }
                if (s == null) PlayerTeam.RemoveItemFromStores(eq);
                else {
                    if (bEquipped) s.Unequip(eqp);
                    s.DestroyItem(eq);
                }
                count++;
            }
            // Inform the user what we got back
            if (count == 0) {
                MessageBox.Show("None of these items could be dismantled");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Dismantled " + count + " item" + (count == 1 ? "" : "s"));
            sb.AppendLine("Materials obtained:");
            foreach (IItem r in dRemains.Keys) {
                PlayerTeam.AddItem(r, dRemains[r]);
                sb.AppendLine(r.Name + " [" + dRemains[r] + "]");
            }
            if (dRemains.Count == 0) sb.AppendLine("None");
            MessageBox.Show(sb.ToString());
            SetupFoundryTab(lSI);
        }
        private void SellSelectedItems(bool bAll) {
            if (dgInventory.SelectedRows.Count == 0) return;
            bool bQuery = false;
            double TotalValue = 0.0;
            int nitem = 0;
            // Get a list of everything to sell
            List<SaleItem> tpSelected = new List<SaleItem>();
            foreach (DataGridViewRow row in dgInventory.SelectedRows) {
                if (row.Tag is not SaleItem tp || tp.Item is not IItem eq) continue;
                tpSelected.Add(tp);
                double SalePrice = (bAll ? tp.Count : 1.0) * eq.Cost * Const.SellDiscount * cl.CostModifier / PriceMod;
                if ((eq is IEquippable ieq && ieq.Level > 0) || SalePrice > 10.0) bQuery = true;
                TotalValue += SalePrice;
                nitem += (bAll ? tp.Count : 1);
            }
            if (tpSelected.Count == 0 || tpSelected[0].Item is null) {
                if (nitem > 1 || TotalValue > 10.0) bQuery = true;
            }
            // Do we neeed to ask if the player is sure?
            if (bQuery) {
                if (nitem == 1) {
                    if (TotalValue > 100.0) {
                        if (MessageBox.Show($"Really sell your {tpSelected[0]!.Item!.Name} for {TotalValue.ToString("N2")}?", "Really Sell?", MessageBoxButtons.YesNo) == DialogResult.No) return;
                    }
                }
                else {
                    if (MessageBox.Show($"Really sell these {nitem} items for {TotalValue.ToString("N2")}?", "Really Sell?", MessageBoxButtons.YesNo) == DialogResult.No) return;
                }
            }
            // Remove items from player
            foreach (DataGridViewRow row in dgInventory.SelectedRows) {
                if (row.Tag is not SaleItem tp) continue;
                IItem eq = tp.Item ?? throw new Exception("Found incomprehensible equipment in DGViewRow");
                Soldier? s = tp.Soldier;
                bool bEquipped = tp.Equipped;
                if (s is null) PlayerTeam.RemoveItemFromStores(eq, bAll ? tp.Count : 1);
                else {
                    if (bEquipped) s.Unequip(eq as IEquippable);
                    s.DestroyItem(eq, bAll ? tp.Count : 1);
                }
            }
            PlayerTeam.Cash += TotalValue;
            SoundEffects.PlaySound("CashRegister");
            if (bAll) SetupFoundryTab(null);
            else SetupFoundryTab(tpSelected);
        }
        private void btModifyWeapon_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count != 1) return;
            if (dgInventory.SelectedRows[0].Tag is not SaleItem tp || tp.Item is not Weapon wp) return;
            if (!wp.Type.Modifiable) return;
            Soldier? s = tp.Soldier;
            bool bEquipped = tp.Equipped;
            int relations = PlayerTeam.GetRelations(cl.Owner);
            ModifyWeapon mw = new ModifyWeapon(wp, PriceMod * cl.CostModifier, (cl.BaseSize * 2) + relations - 2, (cl.BaseSize * 2) + relations - 2, 0, PlayerTeam, null, null);
            mw.ShowDialog(this.Owner);
            if (mw.Modified) {
                if (s is null) {
                    PlayerTeam.RemoveItemFromStores(wp);
                    if (!mw.Destroyed && mw.NewItem != null) PlayerTeam.AddItem(mw.NewItem);
                }
                else {
                    if (bEquipped) s.Unequip(wp);
                    s.DestroyItem(wp);
                    if (!mw.Destroyed && mw.NewItem != null) {
                        s.AddItem(mw.NewItem);
                        if (bEquipped) s.Equip(mw.NewItem);
                    }
                }
                if (mw.Destroyed) SetupFoundryTab();
                else if (mw.NewItem is not null) SetupFoundryTab(new List<SaleItem>() { new SaleItem(mw.NewItem, s, bEquipped, 1) });
            }
        }

        // Double click to get further details on specific entries
        private void dgMercenaries_DoubleClick(object sender, EventArgs e) {
            if (dgMercenaries.CurrentRow?.Tag is not Soldier merc) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(merc.Name);
            sb.AppendLine($"Level {merc.Level} {merc.Gender} {merc.Race.Name}");
            sb.AppendLine("Strength = " + merc.BaseStrength);
            sb.AppendLine("Agility = " + merc.BaseAgility);
            sb.AppendLine("Insight = " + merc.BaseInsight);
            sb.AppendLine("Toughness = " + merc.BaseToughness);
            sb.AppendLine("Endurance = " + merc.BaseEndurance);
            sb.AppendLine("");
            sb.AppendLine("Equipment:");
            foreach (Armour ar in merc.EquippedArmour) {
                sb.AppendLine(ar.Name);
            }
            if (merc.EquippedWeapon != null) sb.AppendLine(merc.EquippedWeapon.Name);
            sb.AppendLine("");
            sb.AppendLine("Inventory:");
            foreach (IItem eq in merc.InventoryGrouped.Keys) {
                if (merc.InventoryGrouped[eq] == 1) sb.AppendLine(eq.Name);
                else sb.AppendLine(eq.Name + " [" + merc.InventoryGrouped[eq] + "]");
            }

            MessageBox.Show(this, sb.ToString(), "Mercenary " + merc.Name);
        }
        private void dgMissions_DoubleClick(object sender, EventArgs e) {
            if (dgMissions?.CurrentRow?.Tag is not Mission miss) return;
            MessageBox.Show(this, miss.GetDescription(), "Mission Details");
        }
        private void dgMerchant_DoubleClick(object sender, EventArgs e) {
            if (dgMerchant.SelectedRows.Count == 0) return;
            if (dgMerchant.SelectedRows[0].Tag is IItem eq) {
                MessageBox.Show(eq.Description, eq.Name);
            }
        }
        private void dgInventory_DoubleClick(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count != 1) return;
            if (dgInventory.SelectedRows[0].Tag is not SaleItem tp || tp.Item is null) return;
            MessageBox.Show(tp.Item.Description, tp.Item.Name);
        }

        // Other handlers
        private void tbFilter_TextChanged(object sender, EventArgs e) {
            SetupMerchantDataGrid();
        }
        private void tbUpgradeFilter_TextChanged(object sender, EventArgs e) {
            SetupFoundryTab();
        }
        private void cbItemType_SelectedIndexChanged(object sender, EventArgs e) {
            SetupMerchantDataGrid();
        }
        private void cbUpgradeItemType_SelectedIndexChanged(object sender, EventArgs e) {
            SetupFoundryTab();
        }
        private void btRandomiseMissions_Click(object sender, EventArgs e) {
            cl.ResetMissions();
            SetupMissionsTab();
        }
        private void btRandomiseMercs_Click(object sender, EventArgs e) {
            cl.ResetMercenaries();
            SetupMercenariesTab();
        }
        private void btRandomiseMerchant_Click(object sender, EventArgs e) {
            cl.ResetStock();
            SetupMerchantDataGrid();
        }
        private void dgInventory_SelectionChanged(object sender, EventArgs e) {
            btSell.Enabled = btDismantle.Enabled = (dgInventory.SelectedRows.Count > 0);
            btImprove.Enabled = false;
            btModify.Enabled = false;
            if (dgInventory.SelectedRows.Count == 1) { // Can improve only one weapon/armour
                if (dgInventory.SelectedRows[0].Tag is not SaleItem tp || tp.Item is not IEquippable eq) return;
                if ((eq is Weapon || eq is Armour) && eq.Level < Const.MaxItemLevel) btImprove.Enabled = true;
                if (eq is Weapon wp && wp.Type.Modifiable) btModify.Enabled = true;
            }
        }
        private void cbAffordable_CheckedChanged(object sender, EventArgs e) {
            SetupMerchantDataGrid();
        }
        private void cbEquipped_CheckedChanged(object sender, EventArgs e) {
            SetupFoundryTab();
        }

        // Changed tab so update lists
        private void tcMain_SelectedIndexChanged(object sender, EventArgs e) {
            int iTabNo = tcMain.SelectedIndex;
            lbImproveRelations.Hide();
            switch (iTabNo) {
                case 0: SetupMerchantDataGrid(); break;
                case 1: SetupMercenariesTab(); break;
                case 2: SetupMissionsTab(); break;
                case 3: SetupShipsTab(); break;
                case 4: SetupFoundryTab(); break;
                case 5: break;
                default: throw new NotImplementedException();
            }
        }

        // DataGridView sort handlers
        private void dgMerchant_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 1) { // Cost
                e.SortResult = double.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(double.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 2) { // Mass
                e.SortResult = double.Parse(e.CellValue1?.ToString()?.Replace("kg", "") ?? string.Empty).CompareTo(double.Parse(e.CellValue2?.ToString()?.Replace("kg", "") ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 3) { // Avail
                e.SortResult = int.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(int.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }

        }
        private void dgMercenaries_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 1) { // Level
                e.SortResult = int.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(int.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 3) { // Fee
                e.SortResult = double.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(double.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
        }
        private void dgMissions_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (dgMissions.Rows[e.RowIndex1].Tag is not Mission miss1) return;
            if (dgMissions.Rows[e.RowIndex2].Tag is not Mission miss2) return;
            if (e.Column.Index == 3) { // Diff
                e.SortResult = miss1.Diff.CompareTo(miss2.Diff);
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 4) { // Size
                if (miss1.Size == miss2.Size) e.SortResult = miss1.LevelCount.CompareTo(miss2.LevelCount);
                else e.SortResult = miss1.Size.CompareTo(miss2.Size);
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 5) { // Reward
                e.SortResult = miss1.Reward.CompareTo(miss2.Reward);
                e.Handled = true;//pass by the default sorting
            }
        }
        private void dgShips_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 2) { // Cost
                e.SortResult = double.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(double.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
        }
        private void dgInventory_SortCompare(object sender, DataGridViewSortCompareEventArgs e) {
            if (e.Column.Index == 2) { // Value
                e.SortResult = double.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(double.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
            if (e.Column.Index == 3) { // Avail
                e.SortResult = int.Parse(e.CellValue1?.ToString() ?? string.Empty).CompareTo(int.Parse(e.CellValue2?.ToString() ?? string.Empty));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void ColonyView_Load(object sender, EventArgs e) {
            btRandomiseMissions.Enabled = Const.DEBUG_RANDOMISE_VENDORS;
            btRandomiseMissions.Visible = Const.DEBUG_RANDOMISE_VENDORS;
            btRandomiseMercs.Enabled = Const.DEBUG_RANDOMISE_VENDORS;
            btRandomiseMercs.Visible = Const.DEBUG_RANDOMISE_VENDORS;
            btRandomiseMerchant.Enabled = Const.DEBUG_RANDOMISE_VENDORS;
            btRandomiseMerchant.Visible = Const.DEBUG_RANDOMISE_VENDORS;
            if (string.IsNullOrEmpty(cl.Location.Name)) lbColonyName.Text = "Unnamed Colony";
            else lbColonyName.Text = cl.Location.Name;
            lbColonySize.Text = cl.BaseSize.ToString();
            if (cl.HasBaseType(Colony.BaseType.Colony)) lbResidential.ForeColor = clExists;
            else lbResidential.ForeColor = clDoesntExist;
            if (cl.HasBaseType(Colony.BaseType.Research)) lbResearch.ForeColor = clExists;
            else lbResearch.ForeColor = clDoesntExist;
            if (cl.HasBaseType(Colony.BaseType.Trading)) lbTrade.ForeColor = clExists;
            else lbTrade.ForeColor = clDoesntExist;
            if (cl.HasBaseType(Colony.BaseType.Military)) lbMilitary.ForeColor = clExists;
            else lbMilitary.ForeColor = clDoesntExist;
            if (cl.HasBaseType(Colony.BaseType.Metropolis)) lbMetropolis.ForeColor = clExists;
            else lbMetropolis.ForeColor = clDoesntExist;
            lbLastGrowth.Text = cl.dtLastGrowth.ToString("D");
            if (!cl.CanGrow) lbNextGrowth.Text = "n/a";
            else lbNextGrowth.Text = cl.dtNextGrowth.ToString("D");
            lbLocation.Text = cl.Location.PrintCoordinates();
            if (string.IsNullOrEmpty(cl.Location.GetSystem().Name)) lbSystemName.Text = "Unnamed";
            else lbSystemName.Text = cl.Location.GetSystem().Name;
            lbStarType.Text = cl.Location.GetSystem().StarType.ToString();
            lbPlanetType.Text = cl.Location.Type.ToString();
            lbOwner.Text = cl.Owner.Name;
        }

        private void ColonyView_Shown(object sender, EventArgs e) {
            // We've just opened the window. Setup teh Merchants datagrid as hidden if required.
            if (Relations < -2) {
                tpMerchant.Hide();
                lbImproveRelations.Show();
                return;
            }
            tpMerchant.Show();
            lbImproveRelations.Hide();
        }
    }
}
