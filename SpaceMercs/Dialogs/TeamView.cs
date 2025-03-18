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
            if (lbInventory.SelectedIndex >= 0) return s.InventoryGrouped.Keys.ElementAt(lbInventory.SelectedIndex);
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
                lbInventory.Enabled = true;
                lbEquipped.Enabled = true;
                lbDeactivated.Visible = false;
            }
            else {
                btDeactivate.Text = "Activate";
                if (s.aoLocation == PlayerTeam.CurrentPosition) btDeactivate.Enabled = true;
                else btDeactivate.Enabled = false;
                lbInventory.Enabled = false;
                lbEquipped.Enabled = false;
                lbDeactivated.Visible = true;
            }

            // Stats
            lbStrength.Text = s.Strength.ToString();
            ttSoldier.SetToolTip(this.lbStrength, $"{s.BaseStrength} (base) {(s.StatBonuses(StatType.Strength) >= 0 ? "+" : "-")} " + Math.Abs(s.StatBonuses(StatType.Strength)) + " (items)");
            lbAgility.Text = s.Agility.ToString();
            ttSoldier.SetToolTip(this.lbAgility, $"{s.BaseAgility} (base) {(s.StatBonuses(StatType.Agility) >= 0 ? "+" : "-")} " + Math.Abs(s.StatBonuses(StatType.Agility)) + " (items)");
            lbInsight.Text = s.Insight.ToString();
            ttSoldier.SetToolTip(this.lbInsight, $"{s.BaseInsight} (base) {(s.StatBonuses(StatType.Insight) >= 0 ? "+" : "-")} " + Math.Abs(s.StatBonuses(StatType.Insight)) + " (items)");
            lbToughness.Text = s.Toughness.ToString();
            ttSoldier.SetToolTip(this.lbToughness, $"{s.BaseToughness} (base) {(s.StatBonuses(StatType.Toughness) >= 0 ? "+" : "-")} " + Math.Abs(s.StatBonuses(StatType.Toughness)) + " (items)");
            lbEndurance.Text = s.Endurance.ToString();
            ttSoldier.SetToolTip(this.lbEndurance, $"{s.BaseEndurance} (base) {(s.StatBonuses(StatType.Endurance) >= 0 ? "+" : "-")} " + Math.Abs(s.StatBonuses(StatType.Endurance)) + " (items)");

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
            lbHealthTotal.Text = Math.Round(s.MaxHealth,0).ToString();
            ttSoldier.SetToolTip(this.lbHealthTotal, s.BaseHealth + " (base) + " + s.StatBonuses(StatType.Health) + " (items)");
            lbAttackTotal.Text = Math.Round(s.Attack,0).ToString();
            double bfi = s.StatBonuses(StatType.Attack);
            ttSoldier.SetToolTip(this.lbAttackTotal, s.BaseAttack + " (base) + " + bfi + " (items)" + ((s.EquippedWeapon != null) ? (" + " + s.GetSoldierSkillWithWeapon(s.EquippedWeapon.Type) + " (weapon skills)") : ""));
            lbDefenceTotal.Text = Math.Round(s.Defence,0).ToString();
            double enc = Math.Round(s.Encumbrance * Const.EncumbranceDefencePenalty, 0);
            string encStr = enc > 0d ? $" - {enc} (weight)" : string.Empty;
            ttSoldier.SetToolTip(this.lbDefenceTotal, $"{s.BaseDefence} (base) + {s.StatBonuses(StatType.Defence)} (items) + {s.GetUtilityLevel(Soldier.UtilitySkill.Avoidance)} (skills){encStr}");
            lbArmour.Text = s.BaseArmour.ToString("N1");
            string strArmour = (100.0 - (Utils.ArmourReduction(s.BaseArmour) * 100.0)).ToString("N2") + "% base damage reduction";
            Dictionary<WeaponType.DamageType, double> AllRes = s.GetAllResistances();
            if (AllRes.Any()) strArmour += Environment.NewLine + "Bonus Resistances:";
            foreach (WeaponType.DamageType tp in AllRes.Keys) {
                strArmour += Environment.NewLine + tp.ToString() + " : " + (int)Math.Round(AllRes[tp]) + "%";
            }
            ttSoldier.SetToolTip(this.lbArmour, strArmour);
            lbStamina.Text = $"{s.MaxStamina:N0}";
            ttSoldier.SetToolTip(this.lbStamina, $"{s.BaseStamina} (base) + {s.StatBonuses(StatType.Stamina)} (items) / {s.StaminaRegen} (recharge)");
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
            lbInventory.Items.Clear();
            foreach (IItem eq in s.InventoryGrouped.Keys) {
                lbInventory.Items.Add(eq.Name + (s.InventoryGrouped[eq] > 1 ? " [" + s.InventoryGrouped[eq] + "]" : ""));
            }
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
                iPrevIndex = lbInventory.SelectedIndex;
                s.DropAll(it);
            }
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && lbInventory.Items.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(it)) lbInventory.SelectedIndex = iPrevIndex;
                else lbInventory.SelectedIndex = Math.Max(0, iPrevIndex - 1);
            }
            ivForm?.UpdateInventory();
        }
        private void btEquip_Click(object sender, EventArgs e) {
            Soldier s = SelectedSoldier() ?? throw new Exception("Selected soldier was null");
            if (SelectedItem() is not IEquippable eq) return;
            int iPrevIndex = -1;
            if (lbInventory.SelectedIndex >= 0) {
                iPrevIndex = lbInventory.SelectedIndex;
                s.Equip(eq);
            }
            else s.Unequip(eq);
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && lbInventory.Items.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(eq)) lbInventory.SelectedIndex = iPrevIndex;
                else lbInventory.SelectedIndex = Math.Max(0, iPrevIndex - 1);
            }
            ivForm?.UpdateInventory();
        }
        private void btDrop_Click(object sender, EventArgs e) {
            Soldier? s = SelectedSoldier();
            if (s is null) return;
            IItem? it = SelectedItem();
            if (it is null) return;
            int iPrevIndex = -1;
            if (lbEquipped.SelectedIndex >= 0 && it is IEquippable eq) s.Unequip(eq);
            else iPrevIndex = lbInventory.SelectedIndex;
            s.DropItem(it);
            ShowSelectedSoldierDetails();
            if (iPrevIndex >= 0 && lbInventory.Items.Count > 0) {
                if (s.InventoryGrouped.ContainsKey(it)) lbInventory.SelectedIndex = iPrevIndex;
                else lbInventory.SelectedIndex = Math.Max(0, iPrevIndex - 1);
            }
            ivForm?.UpdateInventory();
        }
        private void lbInventory_SelectedIndexChanged(object sender, EventArgs e) {
            // Update buttons based on selection
            int i = lbInventory.SelectedIndex;
            if (i < 0) return;
            if (SelectedItem() is Weapon || SelectedItem() is Armour) btEquip.Enabled = true;
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
            lbInventory.SelectedIndex = -1;
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
            if (s.GetRawUtilityLevel(sk) >= 10) throw new Exception("Attempting to increase Utility skill when skill is already at or above maximum level");
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
                    if (s.GetRawUtilityLevel(sk) >= 10) btIncreaseSkill.Enabled = false;
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
                if (s.GetRawUtilityLevel(sk) >= 10) btIncreaseSkill.Enabled = false;
                else btIncreaseSkill.Enabled = true;
            }
        }

        private void lbInventory_DoubleClick(object sender, EventArgs e) {
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
    }
}
