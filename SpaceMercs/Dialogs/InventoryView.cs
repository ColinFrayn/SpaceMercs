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
            AllowDrop = true;
            DragEnter += dgInventory_DragEnter;
            DragOver += dgInventory_DragEnter;
            DragDrop += dgInventory_DragDrop;
            dgInventory.DragOver += dgInventory_DragEnter;
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
            if (PlayerTeam.Inventory.ContainsKey(eq)) UpdateInventory(eq);
            else UpdateInventory();
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

        private void dgInventory_DoubleClick(object sender, EventArgs e) {
            if (dgInventory.SelectedRows.Count == 0) return;
            if (dgInventory.SelectedRows[0].Tag is IItem eq) {
                MessageBox.Show(eq.Description, eq.Name);
            }
        }

        private Rectangle dragBoxFromMouseDown;
        private readonly Size dragSize = SystemInformation.DragSize;

        private void dgInventory_MouseDown(object sender, MouseEventArgs e) {
            int row = dgInventory.HitTest(e.X, e.Y).RowIndex;
            if (row < 0 || dgInventory.Rows[row].Tag is not IItem) {
                dragBoxFromMouseDown = Rectangle.Empty;
                return;
            }
            dgInventory.ClearSelection();
            dgInventory.Rows[row].Selected = true;
            dragBoxFromMouseDown = new Rectangle(
                new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)),
                dragSize);
        }

        private void dgInventory_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            if (dragBoxFromMouseDown == Rectangle.Empty || dragBoxFromMouseDown.Contains(e.X, e.Y)) return;
            if (dgInventory.SelectedRows.Count != 1) return;
            if (dgInventory.SelectedRows[0].Tag is not IItem it) return;
            TeamView.DataAndSource das = new TeamView.DataAndSource() {
                dob = it,
                source = TeamView.WindowSource.Stores,
                EntireStack = (ModifierKeys & Keys.Control) != 0
            };
            dgInventory.DoDragDrop(das, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void dgInventory_MouseUp(object sender, MouseEventArgs e) {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dgInventory_DragEnter(object? sender, DragEventArgs e) {
            bool allowed = e.Data?.GetData(typeof(TeamView.DataAndSource)) is TeamView.DataAndSource;
            e.Effect = allowed ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.None;
        }

        private void dgInventory_DragDrop(object? sender, DragEventArgs e) {
            if (e.Data?.GetData(typeof(TeamView.DataAndSource)) is not TeamView.DataAndSource das) return;
            if (das.dob is not IItem it) return;
            if (das.source == TeamView.WindowSource.Stores) return;
            Soldier? s = tvParent.SelectedSoldier();
            if (s is null || !s.IsActive) return;
            if (it is IEquippable eq && das.source == TeamView.WindowSource.Equipment) s.Unequip(eq);
            if (das.EntireStack || (e.KeyState & 8) != 0) s.DropAll(it);
            else s.DropItem(it);
            UpdateInventory(it);
            tvParent.ShowSelectedSoldierDetails();
        }
    }
}