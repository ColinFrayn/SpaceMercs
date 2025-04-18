using System;

namespace SpaceMercs.Dialogs {
    partial class ChooseSoldiersPsylink : Form {
        private readonly Team PlayerTeam;
        private readonly int MaxSize;
        public List<Soldier> Soldiers = new List<Soldier>();
        public List<Soldier> Psylinked = new List<Soldier>();
        private readonly int psylinkSlots = 0;
        private bool bAccept = false;

        public ChooseSoldiersPsylink(Team t, int sz, bool bCanAbort) {
            PlayerTeam = t;
            MaxSize = sz;
            InitializeComponent();
            lbSquadSize.Text = MaxSize.ToString();
            Soldiers.Clear();
            Psylinked.Clear();
            Soldiers.AddRange(PlayerTeam.SoldiersRO.Take(MaxSize));
            psylinkSlots = t.PlayerShip.PsylinkSpaces;
            lbPsylinkCapacity.Text = psylinkSlots.ToString();
            btAbort.Enabled = bCanAbort;
            btPsylink.Enabled = false;
            if (!bCanAbort) this.ControlBox = false;
            ShowTeamSoldiers();
            UpdateSelection();
            dgSoldiers.RefreshEdit();
        }

        private void ShowTeamSoldiers() {
            dgSoldiers.Rows.Clear();
            object[] arrRow = new object[4];
            foreach (Soldier s in PlayerTeam.SoldiersRO) {
                if (!s.IsActive) continue;
                if (Psylinked.Contains(s)) continue;
                arrRow[0] = Soldiers.Contains(s);
                arrRow[1] = s.Name;
                arrRow[2] = s.Race.Name;
                arrRow[3] = s.Level.ToString();
                dgSoldiers.Rows.Add(arrRow);
                dgSoldiers.Rows[dgSoldiers.Rows.Count - 1].Tag = s;
            }
        }

        private void btLaunch_Click(object sender, EventArgs e) {
            Soldiers.Clear();
            foreach (DataGridViewRow row in dgSoldiers.Rows) {
                bool bSelected = Convert.ToBoolean(row.Cells["Selected"].Value);
                if (bSelected && row.Tag is Soldier s) Soldiers.Add(s);
            }
            if (Soldiers.Count == 0 || Soldiers.Count > MaxSize) return;
            bAccept = true;
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
            //dgSoldiers.ClearSelection();
        }

        private void dgSoldiers_SelectionChanged(object sender, EventArgs e) {
            if (dgSoldiers.SelectedRows.Count == 0) {
                btPsylink.Enabled = false;
                return;
            }
            if (Psylinked.Count >= psylinkSlots) {
                btPsylink.Enabled = false;
                return;
            }
            btPsylink.Enabled = true;
        }

        private void btPsylink_Click(object sender, EventArgs e) {
            if (dgSoldiers.SelectedRows.Count > 0) {
                // Psylink a soldier
                if (dgSoldiers.SelectedRows.Count > 1) {
                    // Weird
                    return;
                }
                if (dgSoldiers.SelectedRows[0].Tag is Soldier s) {
                    if (Psylinked.Count < psylinkSlots) {
                        Psylinked.Add(s);
                        lbPsylink.Items.Add(s);
                        ShowTeamSoldiers();
                        return;
                    }
                }
            }
            else if (lbPsylink.SelectedItems.Count > 0) {
                // Unlink a soldier
                if (lbPsylink.SelectedItems.Count > 1) {
                    // Weird
                    return;
                }
                if (lbPsylink.SelectedItems[0] is Soldier s) {
                    Psylinked.Remove(s);
                    lbPsylink.Items.Remove(s);
                    ShowTeamSoldiers();
                    return;
                }
            }
        }

        private void lbPsylink_SelectedIndexChanged(object sender, EventArgs e) {
            if (lbPsylink.SelectedItems.Count == 0) {
                btPsylink.Enabled = false;
                return;
            }
            btPsylink.Enabled = true;
        }

        private void lbPsylink_Click(object sender, EventArgs e) {
            dgSoldiers.ClearSelection();
            btPsylink.Text = "Unlink";
        }

        private void dgSoldiers_Click(object sender, EventArgs e) {
            btPsylink.Text = "Psylink";
            lbPsylink.ClearSelected();
        }

        private void ChooseSoldiersPsylink_FormClosing(object sender, FormClosingEventArgs e) {
            if (!bAccept) {
                Soldiers.Clear();
                Psylinked.Clear();
            }
        }
    }
}
