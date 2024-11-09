using System;
using System.Linq;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
    partial class InventoryView : Form {
        private readonly TeamView tvParent;
        private readonly Team PlayerTeam;

        public InventoryView(TeamView tv, Team pt) {
            tvParent = tv;
            PlayerTeam = pt;
            InitializeComponent();
            UpdateInventory();
            SetButtons();
        }

        private void SetButtons() {
            btDestroy.Enabled = false;
            btTransfer.Enabled = false;
            if (dgInventory.SelectedRows.Count > 0) {
                btDestroy.Enabled = true;
                Soldier? selected = tvParent.SelectedSoldier();
                if (selected is not null && selected.IsActive) btTransfer.Enabled = true;
            }
        }

        public void UpdateInventory(IItem? iLast = null) {
            dgInventory.Rows.Clear();
            string[] arrRow = new string[5];
            double mass = 0d;
            foreach (IItem eq in PlayerTeam.Inventory.Keys) {
                arrRow[0] = eq.Name;
                arrRow[1] = eq.GetType().Name; //eq is Armour ? "Armour" : (eq is Weapon ? "Weapon" : "Item");
                arrRow[2] = Math.Round(eq.Mass, 2) + "kg";
                arrRow[3] = PlayerTeam.Inventory[eq].ToString();
                double thisMass = eq.Mass * PlayerTeam.Inventory[eq];
                arrRow[4] = Math.Round(thisMass, 2) + "kg";
                dgInventory.Rows.Add(arrRow);
                dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = eq;
                if (eq == iLast) dgInventory.Rows[dgInventory.Rows.Count - 1].Selected = true;
                mass += thisMass;
            }
            lbTotalMass.Text = $"{Math.Round(mass, 1)} kg";
            lbCapacity.Text = $"/ {PlayerTeam.PlayerShip.Type.Capacity} kg";
        }

        private void btTransfer_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count == 0) return;
            Soldier? s = tvParent.SelectedSoldier();
            if (s is null) return;
            if (dgInventory.SelectedRows[0].Tag is not IItem eq) {
                throw new Exception("Could not identify item in data grid");
            }
            if (!PlayerTeam.Inventory.ContainsKey(eq)) { UpdateInventory(); return; }
            s.AddItem(eq, 1);
            PlayerTeam.RemoveItemFromStores(eq, 1);
            UpdateInventory(eq);
            tvParent.ShowSelectedSoldierDetails();
        }

        private void btDestroy_Click(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count == 0) return;
            if (dgInventory.SelectedRows[0].Tag is not IItem eq) {
                throw new Exception("Could not identify item in data grid");
            }
            if (!PlayerTeam.Inventory.ContainsKey(eq)) { UpdateInventory(); return; }
            if (MessageBox.Show("Really destroy " + eq.Name + "?", "Are you sure?", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;
            PlayerTeam.RemoveItemFromStores(eq, 1);
            UpdateInventory();
        }

        private void dgInventory_SelectionChanged(object sender, EventArgs e) {
            SetButtons();
        }

        private void InventoryView_FormClosing(object sender, FormClosingEventArgs e) {
            tvParent.CloseInventory();
        }

        // Update buttons etc. based on selections in the TeamView panel
        public void UpdateAll() {
            SetButtons();
        }

        private void InventoryView_Activated(object sender, EventArgs e) {
            UpdateInventory();
        }

        private void dgInventory_DoubleClick(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count == 0) return;
            if (dgInventory.SelectedRows[0].Tag is IItem eq) {
                MessageBox.Show(eq.Description);
            }
        }
    }
}