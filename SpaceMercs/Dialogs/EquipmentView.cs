using System.Text;
using System.Drawing.Text;

namespace SpaceMercs.Dialogs {
    partial class EquipmentView : Form {
        private InventoryView ivForm = null;
        private readonly ToolTip ttSoldier = new ToolTip();
        private readonly Soldier ThisSoldier;
        private readonly Stash ThisStash;

        public EquipmentView(Soldier? s, Stash st) {
            ThisSoldier = s;
            ThisStash = st;
            InitializeComponent();
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            UpdateAllDetails();
            btScavenge.Enabled = false;
            ttSoldier.SetToolTip(this.pbExperience, "Experience gained this level");
        }

        public IItem?SelectedItem() {
            if (ThisSoldier == null) return null;
            if (lbInventory.SelectedIndex >= 0) return ThisSoldier.InventoryRO.Keys.ElementAt(lbInventory.SelectedIndex);
            if (lbEquipped.SelectedIndex >= 0) {
                int iIndex = lbEquipped.SelectedIndex;
                if (ThisSoldier.EquippedWeapon != null && iIndex == lbEquipped.Items.Count - 1) return ThisSoldier.EquippedWeapon;
                return ThisSoldier.EquippedArmour[iIndex];
            }
            return null;
        }

        public void UpdateAllDetails() {
            if (ThisSoldier == null) return;

            // Stats
            lbStrength.Text = ThisSoldier.Strength.ToString();
            ttSoldier.SetToolTip(this.lbStrength, ThisSoldier.BaseStrength + " (base) + " + ThisSoldier.StatBonuses(StatType.Strength) + " (items)");
            lbAgility.Text = ThisSoldier.Agility.ToString();
            ttSoldier.SetToolTip(this.lbAgility, ThisSoldier.BaseAgility + " (base) + " + ThisSoldier.StatBonuses(StatType.Agility) + " (items)");
            lbIntellect.Text = ThisSoldier.Intellect.ToString();
            ttSoldier.SetToolTip(this.lbIntellect, ThisSoldier.BaseIntellect + " (base) + " + ThisSoldier.StatBonuses(StatType.Intelligence) + " (items)");
            lbToughness.Text = ThisSoldier.Toughness.ToString();
            ttSoldier.SetToolTip(this.lbToughness, ThisSoldier.BaseToughness + " (base) + " + ThisSoldier.StatBonuses(StatType.Toughness) + " (items)");
            lbEndurance.Text = ThisSoldier.Endurance.ToString();
            ttSoldier.SetToolTip(this.lbEndurance, ThisSoldier.BaseEndurance + " (base) + " + ThisSoldier.StatBonuses(StatType.Endurance) + " (items)");

            // Display armour presence by colours
            lbHead.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Head));
            lbChest.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Chest));
            lbRArm.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Arms));
            lbLArm.BackColor = lbRArm.BackColor;
            lbRHand.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Hands));
            lbLHand.BackColor = lbRHand.BackColor;
            lbRLeg.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Legs));
            lbLLeg.BackColor = lbRLeg.BackColor;
            lbRFoot.BackColor = ArmourToColour(ThisSoldier.GetArmourAtLocation(BodyPart.Feet));
            lbLFoot.BackColor = lbRFoot.BackColor;

            // Prime skills
            lbLevel.Text = ThisSoldier.Level.ToString();
            lbHealthTotal.Text = ThisSoldier.MaxHealth.ToString();
            ttSoldier.SetToolTip(this.lbHealthTotal, ThisSoldier.BaseHealth + " (base) + " + ThisSoldier.StatBonuses(StatType.Health) + " (items)");
            lbAttackTotal.Text = ThisSoldier.Attack.ToString();
            double bfi = ThisSoldier.StatBonuses(StatType.Attack);
            if (ThisSoldier.EquippedWeapon != null) bfi += ThisSoldier.EquippedWeapon.AttackBonus;
            ttSoldier.SetToolTip(this.lbAttackTotal, ThisSoldier.BaseAttack + " (base) + " + bfi + " (items)" + ((ThisSoldier.EquippedWeapon != null) ? (" + " + ThisSoldier.GetSoldierSkillWithWeapon(ThisSoldier.EquippedWeapon.Type) + " (weapon skills)") : ""));
            lbDefenceTotal.Text = ThisSoldier.Defence.ToString();
            ttSoldier.SetToolTip(this.lbDefenceTotal, ThisSoldier.BaseDefence + " (base) + " + ThisSoldier.StatBonuses(StatType.Defence) + " (items) + " + ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Avoidance) + " (skills)");
            lbArmour.Text = ThisSoldier.BaseArmour.ToString("N1");
            string strArmour = (100.0 - (Utils.ArmourReduction(ThisSoldier.BaseArmour) * 100.0)).ToString("N2") + "% base damage reduction";
            Dictionary<WeaponType.DamageType, double> AllRes = ThisSoldier.GetAllResistances();
            if (AllRes.Any()) strArmour += Environment.NewLine + "Bonus Resistances:";
            foreach (WeaponType.DamageType tp in AllRes.Keys) {
                strArmour += Environment.NewLine + tp.ToString() + " : " + (int)Math.Round(AllRes[tp]) + "%";
            }
            ttSoldier.SetToolTip(this.lbArmour, strArmour);
            pbExperience.Refresh(); // Display the experience progress bar

            // Weapon skills
            lbWeaponSkills.Items.Clear();
            foreach (WeaponType wp in ThisSoldier.SkilledWeapons) {
                int lvl = ThisSoldier.GetSoldierSkillWithWeapon(wp);
                lbWeaponSkills.Items.Add(wp.Name + " [" + lvl + "]");
            }

            // Utility Skills
            lbUtilitySkills.Items.Clear();
            foreach (Soldier.UtilitySkill sk in Enum.GetValues(typeof(Soldier.UtilitySkill))) {
                int lvl = ThisSoldier.GetUtilityLevel(sk);
                if (lvl > 0 && sk != Soldier.UtilitySkill.Unspent) {
                    int bonus = lvl - ThisSoldier.GetRawUtilityLevel(sk);
                    lbUtilitySkills.Items.Add(sk + " [" + (lvl - bonus) + "]" + (bonus > 0 ? "+" + bonus : ""));
                }
            }
            lbUnspent.Text = ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Unspent).ToString();
            tbSkills_SelectedIndexChanged(null, null); // We might as well use the existing function. Params are ignored anyway...

            // Inventory
            lbInventory.Items.Clear();
            foreach (IItem eq in ThisSoldier.InventoryRO.Keys) {
                lbInventory.Items.Add(eq.Name + (ThisSoldier.InventoryRO[eq] > 1 ? " [" + ThisSoldier.InventoryRO[eq] + "]" : ""));
            }
            lbEquipped.Items.Clear();
            foreach (Armour ar in ThisSoldier.EquippedArmour) {
                lbEquipped.Items.Add(ar.Name);
            }
            if (ThisSoldier.EquippedWeapon != null) lbEquipped.Items.Add(ThisSoldier.EquippedWeapon.Name);
            lbWeight.Text = ThisSoldier.CalculateInventoryMass().ToString("N2") + " kg";
            lbCapacity.Text = Math.Round(ThisSoldier.MaximumCarry, 2).ToString("N2") + " kg";
            lbEncumber.Text = Math.Round(ThisSoldier.Encumbrance * 100.0, 0).ToString("N2") + "%";
            if (ThisSoldier.Encumbrance >= 1.0) lbEncumber.BackColor = Color.Red;
            else if (ThisSoldier.Encumbrance > 0.5) lbEncumber.BackColor = Color.FromArgb(255, 255, (int)((1.0 - ThisSoldier.Encumbrance) * 511.0), 0);
            else if (ThisSoldier.Encumbrance > 0.0) lbEncumber.BackColor = Color.FromArgb(255, (int)(ThisSoldier.Encumbrance * 511.0), 255, 0);
            else lbEncumber.BackColor = Color.FromArgb(255, 0, 255, 0);
            btDrop.Enabled = false;
            btEquip.Enabled = false;

            // Update inventory if it's open, so that it's up-to-date (just to make sure!)
            if (ivForm != null) ivForm.UpdateAll();

            // Stash on the floor
            dgFloor.Rows.Clear();
            string[] arrRow = new string[2];
            foreach (IItem eq in ThisStash.Items()) {
                arrRow[0] = eq.Name;
                //arrRow[1] = eq is Armour ? "Armour" : (eq is Weapon ? "Weapon" : "Item");
                //arrRow[2] = Math.Round(eq.Mass, 2) + "kg";
                arrRow[1] = ThisStash.GetCount(eq).ToString();
                //arrRow[4] = Math.Round(eq.Mass * Stack[eq], 2) + "kg";
                dgFloor.Rows.Add(arrRow);
                dgFloor.Rows[dgFloor.Rows.Count - 1].Tag = eq;
            }
        }

        private Color ArmourToColour(Armour ar) {
            if (ar == null) return Color.Black;
            if (ar.BaseArmour == 0.0) return Color.Gray;
            return Color.Green;
        }

        private void btEquip_Click(object sender, EventArgs e) {
            if (!(SelectedItem() is IEquippable eq)) return;
            int iPrevIndex = -1;
            if (lbInventory.SelectedIndex >= 0) {
                iPrevIndex = lbInventory.SelectedIndex;
                ThisSoldier.Equip(eq);
            }
            else ThisSoldier.Unequip(eq);
            UpdateAllDetails();
            if (iPrevIndex >= 0 && lbInventory.Items.Count > 0) {
                if (ThisSoldier.InventoryRO.ContainsKey(eq)) lbInventory.SelectedIndex = iPrevIndex;
                else lbInventory.SelectedIndex = Math.Max(0, iPrevIndex - 1);
            }
            if (ivForm != null) ivForm.UpdateInventory();
        }
        private void btDrop_Click(object sender, EventArgs e) {
            IItem? it = SelectedItem();
            int iPrevIndex = -1;
            if (lbEquipped.SelectedIndex >= 0 && (it is IEquippable)) ThisSoldier.Unequip((IEquippable)it);
            else iPrevIndex = lbInventory.SelectedIndex;
            ThisStash.Add(it);
            ThisSoldier.DestroyItem(it, 1);
            UpdateAllDetails();
            if (iPrevIndex >= 0 && lbInventory.Items.Count > 0) {
                if (ThisSoldier.InventoryRO.ContainsKey(it)) lbInventory.SelectedIndex = iPrevIndex;
                else lbInventory.SelectedIndex = Math.Max(0, iPrevIndex - 1);
            }
            if (ivForm != null) ivForm.UpdateInventory();
        }
        private void lbInventory_SelectedIndexChanged(object sender, EventArgs e) {
            // Update buttons based on selection
            int i = lbInventory.SelectedIndex;
            if (i < 0) return;
            IItem? it = SelectedItem();
            if (it is Weapon || it is Armour) btEquip.Enabled = true;
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
        private void EquipmentView_FormClosing(object sender, FormClosingEventArgs e) {
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

        private void EquipmentView_Load(object sender, EventArgs e) {
            // Set up the delays for the ToolTip.
            ttSoldier.AutoPopDelay = 5000;
            ttSoldier.InitialDelay = 400;
            ttSoldier.ReshowDelay = 400;
            ttSoldier.ShowAlways = false;
        }

        private void pbExperience_Paint(object sender, PaintEventArgs e) {
            // Clear the background.
            e.Graphics.Clear(Color.White);

            float fraction = (float)ThisSoldier.Experience / (float)ThisSoldier.ExperienceRequiredToReachNextLevel();
            int wid = (int)(fraction * pbExperience.ClientSize.Width);
            e.Graphics.FillRectangle(Brushes.Red, 0, 0, wid, pbExperience.ClientSize.Height);

            // Draw the text.
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (StringFormat sf = new StringFormat()) {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                Font f = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                e.Graphics.DrawString(ThisSoldier.Experience.ToString() + " / " + ThisSoldier.ExperienceRequiredToReachNextLevel().ToString(), f, Brushes.Black, pbExperience.ClientRectangle, sf);
            }
        }

        private void btAddNewSkill_Click(object sender, EventArgs e) {
            int nut = ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            if (nut == 0) return;
            if (ThisSoldier.HasAllUtilitySkills()) return; // Shouldn't ever get here
            ChooseNewUtilitySkill cnus = new ChooseNewUtilitySkill(ThisSoldier);
            cnus.ShowDialog(this);
            if (cnus.ChosenSkill == Soldier.UtilitySkill.Unspent) return;
            ThisSoldier.AddUtilitySkill(cnus.ChosenSkill);
            ThisSoldier.AddUtilitySkill(Soldier.UtilitySkill.Unspent, -1);
            UpdateAllDetails();
        }

        private void btIncreaseSkill_Click(object sender, EventArgs e) {
            if (lbUtilitySkills.SelectedIndex < 0) return;
            int nut = ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            if (nut == 0) return;
            string stsk = lbUtilitySkills.SelectedItem.ToString();
            if (stsk.Contains("[")) stsk = stsk.Substring(0, stsk.IndexOf("[") - 1);
            Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
            if (ThisSoldier.GetRawUtilityLevel(sk) >= ThisSoldier.Level) return; // Should never get here
            if (MessageBox.Show("Really increase this skill?", "Increase skill?", MessageBoxButtons.YesNo) == DialogResult.No) return;
            ThisSoldier.AddUtilitySkill(sk);
            ThisSoldier.AddUtilitySkill(Soldier.UtilitySkill.Unspent, -1);
            UpdateAllDetails();
        }

        private void tbSkills_SelectedIndexChanged(object sender, EventArgs e) {
            // Hide buttons if on weapon skills
            if (tbSkills.SelectedIndex == 0) {
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
                int nut = ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
                lbUnspent.Text = nut.ToString();
                btAddNewSkill.Enabled = (nut > 0 && !ThisSoldier.HasAllUtilitySkills());
                btIncreaseSkill.Enabled = false;
                if ((nut > 0) && (lbUtilitySkills.SelectedIndex >= 0)) {
                    // Disable if existing skill is already max level (== Player's level)
                    // Otherwise Enable
                    Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), (string)lbUtilitySkills.SelectedItem);
                    if (ThisSoldier.GetRawUtilityLevel(sk) >= ThisSoldier.Level) btIncreaseSkill.Enabled = false;
                    else btIncreaseSkill.Enabled = true;
                }
            }
        }

        private void lbUtilitySkills_SelectedIndexChanged(object sender, EventArgs e) {
            int nut = ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Unspent);
            btIncreaseSkill.Enabled = false;
            if ((nut > 0) && (lbUtilitySkills.SelectedIndex >= 0)) {
                // Disable if existing skill is already max level (== Player's level)
                // Otherwise Enable
                string stsk = lbUtilitySkills.SelectedItem.ToString();
                if (stsk.Contains("[")) stsk = stsk.Substring(0, stsk.IndexOf("[") - 1);
                Soldier.UtilitySkill sk = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), stsk);
                if (ThisSoldier.GetRawUtilityLevel(sk) >= ThisSoldier.Level) btIncreaseSkill.Enabled = false;
                else btIncreaseSkill.Enabled = true;
            }

        }

        private void btScavenge_Click(object sender, EventArgs e) {
            if (dgFloor.SelectedRows.Count == 0) return;
            Dictionary<IItem, int> Scavenged = new Dictionary<IItem, int>();
            for (int i = 0; i < dgFloor.SelectedRows.Count; i++) {
                IItem it = (IItem)dgFloor.SelectedRows[i].Tag;
                if (!(it is Corpse)) {
                    MessageBox.Show("You can only scavenge corpses!", "Skin error");
                    return;
                }
                Corpse cp = (Corpse)it;
                if (cp.IsSoldier) {
                    MessageBox.Show("You can't scavenge soldier corpses!", "Skin error");
                    return;
                }

                // Generate stuff
                Random rand = new Random();
                for (int n = 0; n < ThisStash.GetCount(it); n++) {
                    List<IItem> stuff = ((Corpse)it).Scavenge(ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Scavenging), rand);
                    foreach (IItem sc in stuff) {
                        ThisSoldier.AddItem(sc);
                        if (Scavenged.ContainsKey(sc)) Scavenged[sc]++;
                        else Scavenged.Add(sc, 1);
                    }
                }

                ThisStash.Remove(it);
            }
            UpdateAllDetails();
            StringBuilder sb = new StringBuilder();
            if (!Scavenged.Any()) {
                sb.AppendLine("You found nothing useful");
            }
            else {
                sb.AppendLine("Scavenging Results:");
                foreach (IItem it in Scavenged.Keys) {
                    sb.AppendFormat("{0} [{1}]", it.Name, Scavenged[it]);
                    sb.AppendLine();
                }
            }
            MessageBox.Show(sb.ToString(), "Scavenge Results");
        }

        private void btPickUpSelected_Click(object sender, EventArgs e) {
            if (dgFloor.SelectedRows.Count == 0) return;
            HashSet<IItem> hsIt = new HashSet<IItem>();
            for (int i = 0; i < dgFloor.SelectedRows.Count; i++) {
                IItem it = (IItem)dgFloor.SelectedRows[i].Tag;
                hsIt.Add(it);
            }
            foreach (IItem it in hsIt) {
                ThisStash.Remove(it);
                ThisSoldier.AddItem(it, 1);
            }
            UpdateAllDetails();
            // Re-select the same items
            foreach (DataGridViewRow row in dgFloor.Rows) {
                IItem it = (IItem)row.Tag;
                if (hsIt.Contains(it)) row.Selected = true;
                else row.Selected = false;
            }
        }

        private void btPickUpAll_Click(object sender, EventArgs e) {
            if (dgFloor.SelectedRows.Count == 0) return;
            foreach (IItem it in ThisStash.Items()) {
                ThisSoldier.AddItem(it, ThisStash.GetCount(it));
            }
            ThisStash.Clear();
            UpdateAllDetails();
        }

        private void SetScavengeButton() {
            if (dgFloor.SelectedRows.Count == 0 || ThisSoldier.GetUtilityLevel(Soldier.UtilitySkill.Scavenging) == 0) {
                btScavenge.Enabled = false;
                return;
            }
            for (int i = 0; i < dgFloor.SelectedRows.Count; i++) {
                IItem it = (IItem)dgFloor.SelectedRows[i].Tag;
                if (!(it is Corpse)) {
                    btScavenge.Enabled = false;
                    return;
                }
            }
            btScavenge.Enabled = true;
        }

        private void dgFloor_SelectionChanged(object sender, EventArgs e) {
            SetScavengeButton();
        }
        private void dgFloor_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            SetScavengeButton();
        }

        private void lbInventory_DoubleClick(object sender, EventArgs e) {
            if (SelectedItem() == null) return;
            MessageBox.Show(SelectedItem().Desc);
        }

        private void lbEquipped_DoubleClick(object sender, EventArgs e) {
            if (SelectedItem() == null) return;
            MessageBox.Show(SelectedItem().Desc);
        }

        private void dgFloor_DoubleClick(object sender, EventArgs e) {
            if (dgFloor.SelectedRows.Count != 1) return;
            IItem it = (IItem)(dgFloor.SelectedRows[0].Tag);
            MessageBox.Show(it.Desc);
        }
    }
}