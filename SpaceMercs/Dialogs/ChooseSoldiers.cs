using System;

namespace SpaceMercs.Dialogs {
    partial class ChooseSoldiers : Form {
        private readonly Team PlayerTeam;
        private readonly int MaxSize;
        public List<Soldier> Soldiers = new List<Soldier>();

        public ChooseSoldiers(Team t, int sz, bool bCanAbort) {
            PlayerTeam = t;
            MaxSize = sz;
            InitializeComponent();
            lbSquadSize.Text = MaxSize.ToString();
            Soldiers.Clear();
            btAbort.Enabled = bCanAbort;
            if (!bCanAbort) this.ControlBox = false;
            ShowTeamSoldiers();
            UpdateSelection();
            dgSoldiers.RefreshEdit();
        }

        private void ShowTeamSoldiers() {
            dgSoldiers.Rows.Clear();
            object[] arrRow = new object[4];
            Font defaultFont = dgSoldiers.DefaultCellStyle.Font;
            int nsel = 0;
            foreach (Soldier s in PlayerTeam.SoldiersRO) {
                if (!s.IsActive) continue;
                nsel++;
                arrRow[0] = (nsel <= MaxSize);
                arrRow[1] = s.Name;
                arrRow[2] = s.Race.Name;
                arrRow[3] = s.Level.ToString();
                dgSoldiers.Rows.Add(arrRow);
                dgSoldiers.Rows[dgSoldiers.Rows.Count - 1].Tag = s;
            }
            CheckLoadouts();
        }

        private void btLaunch_Click(object sender, EventArgs e) {
            Soldiers.Clear();
            foreach (DataGridViewRow row in dgSoldiers.Rows) {
                bool bSelected = Convert.ToBoolean(row.Cells["Selected"].Value);
                if (bSelected && row.Tag is Soldier s) Soldiers.Add(s);
            }
            if (Soldiers.Count == 0 || Soldiers.Count > MaxSize) return;
            this.Close();
        }

        private void btAbort_Click(object sender, EventArgs e) {
            Soldiers.Clear();
            this.Close();
        }

        private void dgSoldiers_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0) dgSoldiers.Rows[e.RowIndex].Cells[0].Value = !Convert.ToBoolean(dgSoldiers.Rows[e.RowIndex].Cells[0].Value);
            UpdateSelection();
            dgSoldiers.RefreshEdit();
        }

        private void UpdateSelection() {
            int count = 0;
            foreach (DataGridViewRow row in dgSoldiers.Rows) {
                if (Convert.ToBoolean(row.Cells["Selected"].Value)) {
                    count++;
                    row.Selected = true;
                }
                else row.Selected = false;
            }
            if (count == 0 || count > MaxSize) btLaunch.Enabled = false;
            else btLaunch.Enabled = true;
            dgSoldiers.ClearSelection();
            CheckLoadouts();
        }

        private void CheckLoadouts() {
            // Check if our loadouts are complete
            bool bComplete = true;
            foreach (DataGridViewRow row in dgSoldiers.Rows) {
                bool bSelected = Convert.ToBoolean(row.Cells["Selected"].Value);
                if (bSelected && row.Tag is Soldier s) {
                    if (!s.LoadoutComplete) {
                        bComplete = false;
                        break;
                    }
                }
            }
            if (bComplete) {
                btRefill.Enabled = false;
                btRefill.Visible = false;
                lbLoadout.Visible = false;
            }
            else {
                btRefill.Enabled = true;
                btRefill.Visible = true;
                lbLoadout.Visible = true;
            }
        }

        private void dgSoldiers_SelectionChanged(object sender, EventArgs e) {
            dgSoldiers.ClearSelection();
        }

        private void btRefill_Click(object sender, EventArgs e) {
            foreach (DataGridViewRow row in dgSoldiers.Rows) {
                bool bSelected = Convert.ToBoolean(row.Cells["Selected"].Value);
                if (bSelected && row.Tag is Soldier s) {
                    if (!s.LoadoutComplete) {
                        s.Refill();
                    }
                }
            }
            CheckLoadouts();
        }
    }
}
