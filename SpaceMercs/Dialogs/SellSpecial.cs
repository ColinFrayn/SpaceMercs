namespace SpaceMercs.Dialogs {
    partial class SellSpecial : Form {
        private readonly Team _playerTeam;
        private readonly Colony _colony;
        private int count = 0;
        private double proceeds = 0d;

        public SellSpecial(Team playerTeam, Colony col) {
            _playerTeam = playerTeam;
            _colony = col;
            InitializeComponent();
            cbMaxQuality.Items.Clear();
            cbMaxQuality.Items.Add("Basic");
            cbMaxQuality.Items.Add("Good");
            cbMaxQuality.Items.Add("Fine");
            cbMaxQuality.Items.Add("Superb");
            cbMaxQuality.SelectedIndex = 0;
            cbWeapons.Checked = true;
            cbArmour.Checked = true;
            cbItems.Checked = false;
            cbMaterials.Checked = false;
            cbIncludeInventory.Checked = true;
            SetValues();
        }

        private void SetValues() {
            Dictionary<IItem, int> inv = new Dictionary<IItem, int>(_playerTeam.Inventory);
            if (cbIncludeInventory.Checked) {
                foreach (Soldier s in _playerTeam.SoldiersRO) {
                    if (s.aoLocation == _colony.Location) {
                        foreach ((IItem it, int quantity) in s.InventoryGrouped) {
                            int invQ = quantity;
                            if (s.Loadout.TryGetValue(it, out int loq)) {
                                invQ -= loq;
                                if (invQ <= 0) continue;
                            }
                            if (inv.ContainsKey(it)) {
                                inv[it] += invQ;
                            }
                            else inv.Add(it, invQ);
                        }
                    }
                }
            }
            int maxLevel = cbMaxQuality.SelectedIndex;
            count = 0;
            proceeds = 0d;
            double costMod = Const.SellDiscount * _colony.CostModifier / _playerTeam.GetPriceModifier(_colony.Owner, _colony.Location.GetSystem());
            foreach ((IItem it, int quantity) in inv) {
                if (!ItemIsIncluded(it, maxLevel)) continue;
                count += quantity;
                proceeds += it.Cost * quantity * costMod;
            }

            lbItemCount.Text = count.ToString();
            lbProceeds.Text = $"{proceeds:N2} cr";
        }

        private void btCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btSell_Click(object sender, EventArgs e) {
            if (MessageBox.Show($"Really sell these {count} items for {proceeds.ToString("N2")} cr?", "Really Sell?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            int maxLevel = cbMaxQuality.SelectedIndex;
            if (cbIncludeInventory.Checked) {
                foreach (Soldier s in _playerTeam.SoldiersRO) {
                    if (s.aoLocation == _colony.Location) {
                        Dictionary<IItem, int> sinv = new Dictionary<IItem, int>(s.InventoryGrouped);
                        foreach ((IItem it, int quantity) in sinv) {
                            if (!ItemIsIncluded(it, maxLevel)) continue;
                            int invQ = quantity;
                            if (s.Loadout.TryGetValue(it, out int loq)) {
                                invQ -= loq;
                                if (invQ <= 0) continue;                                
                            }
                            // Sell here
                            s.DestroyItem(it, invQ);
                        }
                    }
                }
            }
            Dictionary<IItem, int> inv = new Dictionary<IItem, int>(_playerTeam.Inventory);
            foreach ((IItem it, int quantity) in inv) {
                if (!ItemIsIncluded(it, maxLevel)) continue;
                _playerTeam.RemoveItemFromStores(it, quantity);
            }
            _playerTeam.Cash += proceeds;
            SoundEffects.PlaySound("CashRegister");
            count = 0;
            proceeds = 0d;
            this.Close();
        }
        private bool ItemIsIncluded(IItem it, int maxLevel) {
            if (it is Corpse || it is MissionItem) return false;
            if (it is Weapon && !cbWeapons.Checked) return false;
            if (it is Armour && !cbArmour.Checked) return false;
            if (it is Equipment && !cbItems.Checked) return false;
            if (it is Material && !cbMaterials.Checked) return false;
            if (it is Armour ar && ar.Level > maxLevel) return false;
            if (it is Weapon wp && wp.Level > maxLevel) return false;
            return true;
        }

        private void cbMaxQuality_SelectedIndexChanged(object sender, EventArgs e) {
            SetValues();
        }

        private void cbIncludeInventory_CheckedChanged(object sender, EventArgs e) {
            SetValues();
        }

        private void cbWeapons_CheckedChanged(object sender, EventArgs e) {
            SetValues();
        }

        private void cbArmour_CheckedChanged(object sender, EventArgs e) {
            SetValues();
        }

        private void cbItems_CheckedChanged(object sender, EventArgs e) {
            SetValues();
        }

        private void cbMaterials_CheckedChanged(object sender, EventArgs e) {
            SetValues();
        }
    }
}
