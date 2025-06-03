using System.Collections.ObjectModel;
using System.Drawing.Text;

namespace SpaceMercs.Dialogs {
    partial class TeamView : Form {
        private readonly Team PlayerTeam;
        private readonly ToolTip ttSoldier = new ToolTip();
        private InventoryView? ivForm = null;

        public TeamView(Team t) {
            PlayerTeam = t;
            InitializeComponent();
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dgSoldiers.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            ShowTeamSummary();
            ShowTeamSoldiers();
            ShowSelectedSoldierDetails();
        }

        private void ShowTeamSummary() {
            lbCash.Text = Math.Round(PlayerTeam.Cash, 2).ToString();
            lbLocation.Text = "(" + Math.Round(PlayerTeam.CurrentPosition.GetSystem().MapPos.X, 2) + "," + Math.Round(PlayerTeam.CurrentPosition.GetSystem().MapPos.Y, 2) + ")";
            int iRoster = PlayerTeam.SoldierCount;
            lbRoster.Text = iRoster.ToString() + " soldier" + (iRoster > 1 ? "s" : "");
            int iActive = PlayerTeam.ActiveSoldiers;
            lbActive.Text = iActive.ToString() + " soldier" + (iActive > 1 ? "s" : "");
            lbShip.Text = PlayerTeam.PlayerShip.Type.Name;
            int iBerths = PlayerTeam.PlayerShip.TotalBerths;
            lbBerths.Text = iBerths.ToString();
            if (iBerths < iActive) lbInsufficientBerths.Visible = true;
            else lbInsufficientBerths.Visible = false;
        }

        private void ShowTeamSoldiers() {
            dgSoldiers.Rows.Clear();
            if (!PlayerTeam.SoldiersRO.Any()) return;
            string[] arrRow = new string[4];
            Font defaultFont = dgSoldiers.DefaultCellStyle.Font;
            foreach (Soldier s in PlayerTeam.SoldiersRO) {
                arrRow[0] = s.Name;
                arrRow[1] = s.Race.Name;
                arrRow[2] = s.Level.ToString();
                if (s.IsActive) arrRow[3] = "Active";
                else {
                    if (s.aoLocation == PlayerTeam.CurrentPosition) arrRow[3] = "Inactive";
                    else arrRow[3] = s.aoLocation?.PrintCoordinates() ?? string.Empty;
                }
                dgSoldiers.Rows.Add(arrRow);
                dgSoldiers.Rows[dgSoldiers.Rows.Count - 1].Tag = s;
                if (s.IsActive) dgSoldiers.Rows[dgSoldiers.Rows.Count - 1].DefaultCellStyle.Font = new Font(defaultFont, FontStyle.Bold);
            }
            dgSoldiers.Rows[0].Selected = true;
        }

        public Soldier? SelectedSoldier() {
            if (dgSoldiers.SelectedRows.Count == 0) return null;
            return dgSoldiers.SelectedRows[0].Tag as Soldier;
        }
        public IItem? SelectedItem() {
            Soldier? s = SelectedSoldier();
            if (s is null) return null;
            if (dgInventory.SelectedRows.Count > 0 && dgInventory.SelectedRows[0].Tag is IItem it) {
                return it;
            }
            if (lbEquipped.SelectedIndex >= 0) {
                int iIndex = lbEquipped.SelectedIndex;
                if (s.EquippedWeapon != null && iIndex == lbEquipped.Items.Count - 1) return s.EquippedWeapon;
                return s.EquippedArmour[iIndex];
            }
            return null;
        }

        public void ShowSelectedSoldierDetails() {
            Soldier? s = SelectedSoldier();
            if (s is null) return;

            // Soldier management
            if (s.IsActive) {
                btDeactivate.Text = "Deactivate";
                btDeactivate.Enabled = true;
                dgInventory.Enabled = true;
                lbEquipped.Enabled = true;
                lbDeactivated.Visible = false;
            }
            else {
                btDeactivate.Text = "Activate";
                if (s.aoLocation == PlayerTeam.CurrentPosition) btDeactivate.Enabled = true;
                else btDeactivate.Enabled = false;
                dgInventory.Enabled = false;
                lbEquipped.Enabled = false;
                lbDeactivated.Visible = true;
            }

            // Stats
            lbStrength.Text = s.Strength.ToString();
            ttSoldier.SetToolTip(this.lbStrength, s.StrengthExplanation);
            lbAgility.Text = s.Agility.ToString();
            ttSoldier.SetToolTip(this.lbAgility, s.AgilityExplanation);
            lbInsight.Text = s.Insight.ToString();
            ttSoldier.SetToolTip(this.lbInsight, s.InsightExplanation);
            lbToughness.Text = s.Toughness.ToString();
            ttSoldier.SetToolTip(this.lbToughness, s.ToughnessExplanation);
            lbEndurance.Text = s.Endurance.ToString();
            ttSoldier.SetToolTip(this.lbEndurance, s.EnduranceExplanation);

            // Display armour presence by colours
            lbHead.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Head));
            lbChest.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Chest));
            lbRArm.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Arms));
            lbLArm.BackColor = lbRArm.BackColor;
            lbRHand.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Hands));
            lbLHand.BackColor = lbRHand.BackColor;
            lbRLeg.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Legs));
            lbLLeg.BackColor = lbRLeg.BackColor;
            lbRFoot.BackColor = ArmourToColour(s.GetArmourAtLocation(BodyPart.Feet));
            lbLFoot.BackColor = lbRFoot.BackColor;
            btColour.BackColor = s.PrimaryColor;

            // Prime skills
            lbLevel.Text = s.Level.ToString();
            lbHealthTotal.Text = Math.Round(s.MaxHealth, 0).ToString();
            ttSoldier.SetToolTip(this.lbHealthTotal, s.HealthExplanation);
            lbAttackTotal.Text = Math.Round(s.Attack, 0).ToString();
            ttSoldier.SetToolTip(this.lbAttackTotal, s.AttackExplanation);
            lbDefenceTotal.Text = Math.Round(s.Defence, 0).ToString();
            ttSoldier.SetToolTip(this.lbDefenceTotal, s.DefenceExplanation);
            lbArmour.Text = s.BaseArmour.ToString("N1");
            ttSoldier.SetToolTip(this.lbArmour, s.ArmourExplanation);
            lbStamina.Text = $"{s.MaxStamina:N0}";
            ttSoldier.SetToolTip(this.lbStamina, s.StaminaExplanation);
            pbExperience.Refresh(); // Display the experience progress bar
            btUpgradeStat.Enabled = s.PointsToSpend > 0;
            btUpgradeStat.Visible = s.PointsToSpend > 0;
            btUpgradeStat.Text = $"+{s.PointsToSpend}";

            // Weapon skills
            lbWeaponSkills.Items.Clear();
            foreach (WeaponType.WeaponClass wp in s.SkilledWeaponClasses) {
                int lvl = s.GetSoldierSkillWithWeaponClass(wp);
                lbWeaponSkills.Items.Add($"{wp} [{lvl}]");
            }

            // Utility Skills
            lbUtilitySkills.Items.Clear();
            foreach (Soldier.UtilitySkill sk in Enum.GetValues(typeof(Soldier.UtilitySkill))) {
                int lvl = s.GetUtilityLevel(sk);
                if (lvl != 0 && sk != Soldier.UtilitySkill.Unspent) {
                    int bonus = lvl - s.GetRawUtilityLevel(sk);
                    lbUtilitySkills.Items.Add($"{sk} [{lvl - bonus}] {(bonus > 0 ? "+" : string.Empty)}{(bonus != 0 ? bonus : string.Empty)}");
                }
            }
            lbUnspent.Text = s.GetUtilityLevel(Soldier.UtilitySkill.Unspent).ToString();
            tbSkills_SelectedIndexChanged(new object(), EventArgs.Empty); // We might as well use the existing function. Params are ignored anyway...

            // Inventory
            dgInventory.Rows.Clear();
            ReadOnlyDictionary<IItem, int> loadout = s.Loadout;
            Dictionary<IItem, int> allItems = new(s.InventoryGrouped);
            // Add in placeholders for desired items that we don't have
            foreach (IItem it in loadout.Keys) {
                if (!allItems.ContainsKey(it)) allItems.TryAdd(it, 0);
            }
            string[] arrRow = new string[3];
            foreach (IItem eq in allItems.Keys) {
                arrRow[0] = eq.Name;
                arrRow[1] = allItems[eq].ToString();
                if (loadout.TryGetValue(eq, out int quantity)) {
                    arrRow[2] = quantity.ToString();
                }
                else {
                    arrRow[2] = string.Empty;
                }
                dgInventory.Rows.Add(arrRow);
                dgInventory.Rows[dgInventory.Rows.Count - 1].Tag = eq;
                if (s.CountItem(eq) < quantity) {
                    dgInventory.Rows[dgInventory.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Red;
                }
                else {
                    dgInventory.Rows[dgInventory.Rows.Count - 1].DefaultCellStyle.BackColor = Color.White;
                }
            }
            dgInventory.ClearSelection();

            lbEquipped.Items.Clear();
            foreach (Armour ar in s.EquippedArmour) {
                lbEquipped.Items.Add(ar.Name);
            }
            if (s.EquippedWeapon != null) lbEquipped.Items.Add(s.EquippedWeapon.Name);
            lbWeight.Text = s.CalculateInventoryMass().ToString("N2") + " kg";
            lbCapacity.Text = Math.Round(s.MaximumCarry, 2).ToString() + " kg";
            lbEncumber.Text = Math.Round(s.Encumbrance * 100.0, 0).ToString() + "%";
            if (s.Encumbrance >= 1.0) lbEncumber.BackColor = Color.Red;
            else if (s.Encumbrance > 0.5) lbEncumber.BackColor = Color.FromArgb(255, 255, (int)((1.0 - s.Encumbrance) * 511.0), 0);
            else if (s.Encumbrance > 0.0) lbEncumber.BackColor = Color.FromArgb(255, (int)(s.Encumbrance * 511.0), 255, 0);
            else lbEncumber.BackColor = Color.FromArgb(255, 0, 255, 0);
            btDrop.Enabled = false;
            btEquip.Enabled = false;
            if (s.InventoryGrouped.Count == 0 || !s.IsActive) btDropAll.Enabled = false;
            else btDropAll.Enabled = true;

            // Update inventory if it's open, so that it's up-to-date (just to make sure!)
            ivForm?.UpdateAll();
        }
        private void SelectInventoryItem(IItem item) {
            dgInventory.ClearSelection();
            foreach (DataGridViewRow row in dgInventory.Rows) {
                if (row.Tag is IItem it) {
                    if (it == item) {
                        row.Selected = true;
                    }
                }
            }
        }

        private static Color ArmourToColour(Armour? ar) {
            if (ar is null) return Color.Black;
            if (ar.BaseArmour == 0.0) return Color.Gray;
            return Color.Green;
        }

        // Soldier management
        private void dgSoldiers_SelectionChanged(object sender, EventArgs e) {
            ShowSelectedSoldierDetails();
        }
        private void btDeactivate_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            if (s.IsActive) s.Deactivate();
            else s.Activate();
            ShowSelectedSoldierDetails();
            Font defaultFont = dgSoldiers.DefaultCellStyle.Font;
            if (s.IsActive) dgSoldiers.SelectedRows[0].DefaultCellStyle.Font = new Font(defaultFont, FontStyle.Bold);
            else dgSoldiers.SelectedRows[0].DefaultCellStyle.Font = defaultFont;
            if (s.IsActive) dgSoldiers.SelectedRows[0].Cells[3].Value = "Active";
            else dgSoldiers.SelectedRows[0].Cells[3].Value = "Inactive";
            ShowTeamSummary();
        }
        private void btDismiss_Click(object sender, EventArgs e) {
            if (PlayerTeam.SoldierCount == 1) {
                MessageBox.Show("You cannot dismiss your last soldier!");
                return;
            }
            ShowSelectedSoldierDetails();
            ShowTeamSummary();
        }

        // Inventory stuff
        private void btInventory_Click(object sender, EventArgs e) {
            if (ivForm != null && ivForm.IsDisposed) ivForm = null;
            if (ivForm == null) {
                ivForm = new InventoryView(this, PlayerTeam);
                ivForm.Show(this);
            }
        }
        private void btDropAllClick(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            IItem? it = SelectedItem();
            if (it is null) return;
            int iPrevIndex = -1;
            if (lbEquipped.SelectedIndex >= 0 && it is IEquippable eq) {
                s.Unequip(eq);
                s.DropItem(it);
            }
            else {
                iPrevIndex = dgInventory.CurrentCell?.RowIndex - 1 ?? -1;
                foreach (DataGridViewRow row in dgInventory.Rows) {
                    if (row.Selected == true && row.Tag is IItem itd) {
                        s.DropAll(itd);
                    }
                }
            }
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && dgInventory.Rows.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(it)) SelectInventoryItem(it);
                else dgInventory.Rows[Math.Min(dgInventory.Rows.Count - 1, Math.Max(0, iPrevIndex))].Selected = true;
            }
            ivForm?.UpdateInventory();
        }
        private void btEquip_Click(object sender, EventArgs e) {
            Soldier s = SelectedSoldier() ?? throw new Exception("Selected soldier was null");
            if (SelectedItem() is not IEquippable eq) return;
            int iPrevIndex = -1;
            if (dgInventory.SelectedRows.Count > 0) {
                iPrevIndex = dgInventory.CurrentCell?.RowIndex - 1 ?? -1;
                s.Equip(eq);
            }
            else s.Unequip(eq);
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && dgInventory.Rows.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(eq)) SelectInventoryItem(eq);
                else dgInventory.Rows[Math.Max(0, iPrevIndex)].Selected = true;
            }
            ivForm?.UpdateInventory();
        }
        private void btDrop_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            IItem? it = SelectedItem();
            if (it is null) return;
            int iPrevIndex = -1;
            if (lbEquipped.SelectedIndex >= 0 && it is IEquippable eq) {
                s.Unequip(eq);
                s.DropItem(eq);
            }
            else {
                iPrevIndex = dgInventory.CurrentCell?.RowIndex - 1 ?? -1;
                foreach (DataGridViewRow row in dgInventory.Rows) {
                    if (row.Selected == true && row.Tag is IItem itd) {
                        s.DropItem(itd);
                    }
                }
            }
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && dgInventory.Rows.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(it)) SelectInventoryItem(it);
                else dgInventory.Rows[Math.Min(dgInventory.Rows.Count-1, Math.Max(0, iPrevIndex))].Selected = true;
            }
            ivForm?.UpdateInventory();
        }
        private void dgInventory_SelectedIndexChanged(object sender, EventArgs e) {
            // Update buttons based on selection
            int i = dgInventory.CurrentCell?.RowIndex - 1 ?? -1;
            if (i < 0) return;
            if (dgInventory.SelectedRows.Count == 1 && (SelectedItem() is Weapon || SelectedItem() is Armour)) btEquip.Enabled = true;
            else btEquip.Enabled = false;
            btEquip.Text = "Equip";
            btDrop.Enabled = true;
            lbEquipped.SelectedIndex = -1;
        }
        private void lbEquipped_SelectedIndexChanged(object sender, EventArgs e) {
            // Update buttons based on selection
            int i = lbEquipped.SelectedIndex;
            if (i < 0) return;
            btEquip.Enabled = true;
            btEquip.Text = "Unequip";
            btDrop.Enabled = true;
            dgInventory.ClearSelection();
        }

        // ----- Miscellaneous form handlers -----
        private void TeamView_FormClosing(object sender, FormClosingEventArgs e) {
            // Close InventoryView if it exists
            if (ivForm != null && !ivForm.IsDisposed) {
                ivForm.Dispose();
                ivForm.Close();
                ivForm = null;
            }
        }

        // Indicate that the InventoryView window has closed
        public void CloseInventory() {
            ivForm = null;
        }

        private void TeamView_Load(object sender, EventArgs e) {
            // Set up the delays for the ToolTip.
            ttSoldier.AutoPopDelay = 5000;
            ttSoldier.InitialDelay = 400;
            ttSoldier.ReshowDelay = 400;
            ttSoldier.ShowAlways = false;
        }

        private void pbExperience_Paint(object sender, PaintEventArgs e) {
            // Clear the background.
            e.Graphics.Clear(Color.White);

            Soldier s = SelectedSoldier() ?? throw new Exception("No selected soldier to paint");
            int lastLevel = Soldier.ExperienceRequiredToReachLevel(s.Level);
            int nextLevel = s.ExperienceRequiredToReachNextLevel();
            float fraction = (float)(s.Experience - lastLevel) / (float)(nextLevel - lastLevel);
            int wid = (int)(fraction * pbExperience.ClientSize.Width);
            e.Graphics.FillRectangle(Brushes.Red, 0, 0, wid, pbExperience.ClientSize.Height);

            // Draw the text.
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (StringFormat sf = new StringFormat()) {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                Font f = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                e.Graphics.DrawString((s.Experience - lastLevel).ToString() + " / " + (nextLevel - lastLevel).ToString(), f, Brushes.Black, pbExperience.ClientRectangle, sf);
            }
        }

        private void btAddNewSkill_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            int nut = s.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            if (nut == 0) return;
            if (s.HasAllUtilitySkills()) throw new Exception("Attempting to add utility skill when soldier already has them all!");
            ChooseNewUtilitySkill cnus = new ChooseNewUtilitySkill(s);
            cnus.ShowDialog(this);
            if (cnus.ChosenSkill == Soldier.UtilitySkill.Unspent) return;
            s.AddUtilitySkill(cnus.ChosenSkill);
            s.AddUtilitySkill(Soldier.UtilitySkill.Unspent, -1);
            ShowSelectedSoldierDetails();
        }
        private void btIncreaseSkill_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            if (lbUtilitySkills.SelectedIndex < 0) throw new Exception("Attempting to add skill level to unselected utility skill");
            int nut = s.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            if (nut == 0) return;
            string stsk = lbUtilitySkills?.SelectedItem?.ToString() ?? string.Empty;
            if (stsk.Contains('[')) stsk = stsk.Substring(0, stsk.IndexOf("[") - 1);
            Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
            if (s.GetRawUtilityLevel(sk) >= s.Level) throw new Exception("Attempting to increase Utility skill when skill is already at or above Soldier level");
            if (s.GetRawUtilityLevel(sk) >= Const.MaxUtilitySkill) throw new Exception("Attempting to increase Utility skill when skill is already at or above maximum level");
            if (MessageBox.Show("Really increase this skill?", "Increase skill?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            s.AddUtilitySkill(sk);
            s.AddUtilitySkill(Soldier.UtilitySkill.Unspent, -1);
            ShowSelectedSoldierDetails();
        }
        private void tbSkills_SelectedIndexChanged(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            // Hide buttons if on weapon skills
            if (s is null || tbSkills.SelectedIndex == 0) {
                lbUnspent.Visible = false;
                label17.Visible = false;
                btAddNewSkill.Visible = false;
                btIncreaseSkill.Visible = false;
                return;
            }
            else {
                lbUnspent.Visible = true;
                label17.Visible = true;
                btAddNewSkill.Visible = true;
                btIncreaseSkill.Visible = true;
                int nut = s.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
                lbUnspent.Text = nut.ToString();
                btAddNewSkill.Enabled = (nut > 0 && !s.HasAllUtilitySkills());
                btIncreaseSkill.Enabled = false;
                if ((nut > 0) && (lbUtilitySkills.SelectedIndex >= 0)) {
                    // Disable if existing skill is already max level (== Player's level)
                    // Otherwise Enable
                    string stsk = lbUtilitySkills?.SelectedItem?.ToString() ?? string.Empty;
                    if (stsk.Contains('[')) stsk = stsk.Substring(0, stsk.IndexOf("[") - 1);
                    Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
                    if (s.GetRawUtilityLevel(sk) >= s.Level) btIncreaseSkill.Enabled = false;
                    if (s.GetRawUtilityLevel(sk) >= Const.MaxUtilitySkill) btIncreaseSkill.Enabled = false;
                    else btIncreaseSkill.Enabled = true;
                }
            }
        }
        private void lbUtilitySkills_SelectedIndexChanged(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            int nut = s.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            btIncreaseSkill.Enabled = false;
            if ((nut > 0) && (lbUtilitySkills.SelectedIndex >= 0)) {
                // Disable if existing skill is already max level (== Player's level)
                // Otherwise Enable
                string stsk = lbUtilitySkills?.SelectedItem?.ToString() ?? string.Empty;
                if (stsk.Contains('[')) stsk = stsk.Substring(0, stsk.IndexOf('[') - 1);
                Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
                if (s.GetRawUtilityLevel(sk) >= s.Level) btIncreaseSkill.Enabled = false;
                if (s.GetRawUtilityLevel(sk) >= Const.MaxUtilitySkill) btIncreaseSkill.Enabled = false;
                else btIncreaseSkill.Enabled = true;
            }
        }

        private void dgInventory_DoubleClick(object sender, EventArgs e) {
            IItem? it = SelectedItem();
            if (it is null) return;
            MessageBox.Show(it.Description, it.Name);
        }

        private void lbEquipped_DoubleClick(object sender, EventArgs e) {
            IItem? it = SelectedItem();
            if (it is null) return;
            MessageBox.Show(it.Description, it.Name);
        }

        private void btColour_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            DialogResult dr = cdPickColour.ShowDialog();
            if (dr == DialogResult.OK) {
                btColour.BackColor = cdPickColour.Color;
                s.SetPrimaryColour(cdPickColour.Color);
            }
        }

        private void lbUtilitySkills_DoubleClick(object sender, EventArgs e) {
            if (lbUtilitySkills.SelectedIndex < 0) return;
            string stsk = lbUtilitySkills?.SelectedItem?.ToString() ?? string.Empty;
            if (stsk.Contains('[')) stsk = stsk.Substring(0, stsk.IndexOf('[') - 1);
            Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
            string desc = Utils.UtilitySkillToDesc(sk);
            MessageBox.Show(desc);
        }

        private void btUpgradeStat_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            if (s.PointsToSpend == 0) return;
            s.UpgradeStat();
            ShowSelectedSoldierDetails();
        }

        private void dgInventory_KeyUp(object sender, KeyEventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            IItem? it = SelectedItem();
            if (it is null) return;
            if (dgInventory.SelectedRows.Count == 0) return;

            if (e.KeyCode is Keys.Add or Keys.Subtract) {
                int quantity = 0;
                if (e.KeyCode == Keys.Add) {
                    quantity = s.AddLoadout(it);
                }
                if (e.KeyCode == Keys.Subtract) {
                    quantity = s.RemoveLoadout(it);
                }
                var row = dgInventory.SelectedRows[0];
                if (quantity > 0) row.Cells[2].Value = quantity.ToString();
                else row.Cells[2].Value = string.Empty;
                if (s.CountItem(it) < quantity) {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
                else {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
                ShowSelectedSoldierDetails();
                SelectInventoryItem(it);
            }
        }

        // Drag-Drop functionality

        private enum WindowSource { Equipment, Inventory };
        private class DataAndSource {
            public IItem dob { get; init; }
            public WindowSource source { get; init; }
        }
        private Rectangle dragBoxFromMouseDown;
        private int indexOfItemUnderMouseToDrag;
        private Size dragSize = SystemInformation.DragSize;

        private void lbEquipped_MouseDown(object sender, MouseEventArgs e) {
            indexOfItemUnderMouseToDrag = lbEquipped.IndexFromPoint(e.X, e.Y);
            if (indexOfItemUnderMouseToDrag != ListBox.NoMatches) {
                dragBoxFromMouseDown = new Rectangle(
                    new Point(e.X - (dragSize.Width / 2),
                              e.Y - (dragSize.Height / 2)),
                    dragSize);
            }
            else {
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void lbEquipped_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y)) {
                    IItem? it = null;
                    Soldier? thisSoldier = SelectedSoldier();
                    if (thisSoldier is null) return;
                    if (thisSoldier.EquippedWeapon != null && indexOfItemUnderMouseToDrag == lbEquipped.Items.Count - 1) it = thisSoldier.EquippedWeapon;
                    else it = thisSoldier.EquippedArmour[indexOfItemUnderMouseToDrag];
                    if (it is not null) {
                        DataAndSource das = new DataAndSource() { dob = it, source = WindowSource.Equipment };
                        lbEquipped.DoDragDrop(das, DragDropEffects.All);
                    }
                }
            }
        }

        private void lbEquipped_MouseUp(object sender, MouseEventArgs e) {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void lbEquipped_DragEnter(object sender, DragEventArgs e) {
            DataAndSource? das = e.Data?.GetData(typeof(DataAndSource)) as DataAndSource;
            if (das?.dob is Weapon or Armour) {
                e.Effect = DragDropEffects.Move;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void lbEquipped_DragDrop(object sender, DragEventArgs e) {
            if (e.Data?.GetData(typeof(DataAndSource)) is not DataAndSource das) return;
            if (das.dob is not IItem it) return;
            if (das.source == WindowSource.Equipment) return;
            Soldier? thisSoldier = SelectedSoldier();
            if (thisSoldier is null) return;
            if (it is IEquippable eq) {
                thisSoldier.Equip(eq);
            }
            ShowSelectedSoldierDetails();
            if (ivForm != null) ivForm.UpdateInventory();
        }

        private void dgInventory_MouseDown(object sender, MouseEventArgs e) {
            if (dgInventory.SelectedRows.Count == 1 && dgInventory.SelectedRows[0].Tag is IItem) {
                dragBoxFromMouseDown = new Rectangle(
                    new Point(e.X - (dragSize.Width / 2),
                              e.Y - (dragSize.Height / 2)),
                    dragSize);
            }
            else {
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void dgInventory_MouseMove(object sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) {
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y)) {
                    if (dgInventory.SelectedRows.Count != 1) return;
                    if (dgInventory.SelectedRows[0].Tag is not IItem it) return;
                    DataAndSource das = new DataAndSource() { dob = it, source = WindowSource.Inventory };
                    dgInventory.DoDragDrop(das, DragDropEffects.All);
                }
            }
        }

        private void dgInventory_MouseUp(object sender, MouseEventArgs e) {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dgInventory_DragEnter(object sender, DragEventArgs e) {
            if (e.Data?.GetDataPresent(typeof(DataAndSource)) == true) e.Effect = DragDropEffects.Move;
            else e.Effect = DragDropEffects.None;
        }

        private void dgInventory_DragDrop(object sender, DragEventArgs e) {
            if (e.Data?.GetData(typeof(DataAndSource)) is not DataAndSource das) return;
            if (das.dob is not IItem it) return;
            if (das.source == WindowSource.Inventory) return;
            Soldier? thisSoldier = SelectedSoldier();
            if (thisSoldier is null) return;
            if (it is IEquippable eq && das.source == WindowSource.Equipment) thisSoldier.Unequip(eq);
            ShowSelectedSoldierDetails();
            if (ivForm != null) ivForm.UpdateInventory();
        }

        private void btInventory_DragEnter(object sender, DragEventArgs e) {
            if (e.Data?.GetDataPresent(typeof(DataAndSource)) == true) e.Effect = DragDropEffects.Move;
            else e.Effect = DragDropEffects.None;
        }

        private void btInventory_DragDrop(object sender, DragEventArgs e) {
            if (e.Data?.GetData(typeof(DataAndSource)) is not DataAndSource das) return;
            if (das.dob is not IItem it) return;
            Soldier? thisSoldier = SelectedSoldier();
            if (thisSoldier is null) return;
            if (it is IEquippable eq && das.source == WindowSource.Equipment) thisSoldier.Unequip(eq);
            thisSoldier.DropItem(it);
            ShowSelectedSoldierDetails();
        }
    }
}
