using System;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
    partial class ModifyWeapon : Form {
        private readonly Weapon WeaponToModify;
        private double ModifyCost;
        private readonly double PriceMod;
        private double SuccessChance;
        private readonly Team PlayerTeam;
        private readonly Soldier? ThisSoldier1, ThisSoldier2;
        private readonly int Skill1, Skill2;
        public IEquippable? NewItem = null; // If this is set to non-null then it represents the new item after modification
        public bool Modified = false; // Did we attempt the upgrade? (i.e. should we remove the old item?) We need this in case the upgrade destroys the object, in which case NewItem will be null.
        public bool Destroyed = false; // Destroyed in attempting to upgrade?

        public ModifyWeapon(Weapon wp, double priceMod, int skill1, int skill2, Team t, Soldier? s1, Soldier? s2) {
            InitializeComponent();
            WeaponToModify = wp;
            PlayerTeam = t;
            ThisSoldier1 = s1;
            ThisSoldier2 = s2;
            Skill1 = skill1;
            Skill2 = skill2;
            Modified = false;
            Destroyed = false;
            PriceMod = priceMod;

            if (ThisSoldier1 == null) lbSoldier1.Visible = false;
            else {
                lbSoldier1.Visible = true;
                lbSoldier1.Text = $"{ThisSoldier1.Name} ({(wp.Type.IsMeleeWeapon ? Soldier.UtilitySkill.Bladesmith : Soldier.UtilitySkill.Gunsmith)} : {skill1})";
            }
            if (ThisSoldier2 == null) lbSoldier2.Visible = false;
            else {
                lbSoldier2.Visible = true;
                lbSoldier2.Text = ThisSoldier2.Name + " (Engineering : " + skill2 + ")";
            }

            lbName.Text = WeaponToModify.Type.Name;
            lbQuality.Text = Utils.LevelToDescription(WeaponToModify.Level);
            lbCurrentMod.Text = WeaponToModify.Mod?.Name ?? "None";

            cbModType.Items.Clear();
            foreach (WeaponMod mod in StaticData.WeaponMods) {
                if (mod.IsMelee == WeaponToModify.Type.IsMeleeWeapon &&
                    mod.Name != WeaponToModify.Mod?.Name) {
                    cbModType.Items.Add(mod.Name);
                }
            }
            cbModType.SelectedIndex = 0;

            UpdatedSelection();
        }

        private void UpdatedSelection() {
            string newMod = cbModType?.SelectedItem?.ToString() ?? string.Empty;
            WeaponMod? mod = StaticData.GetWeaponModByName(newMod);
            if (mod == null) { return; } 

            // Chance of this modification working and not damaging the weapon
            SuccessChance = (0.7 - 0.05 * (WeaponToModify.Level * 4 - (Skill1 + Skill2)) - 0.01 * (WeaponToModify.BaseType.BaseRarity));
            if (SuccessChance > 0.99) SuccessChance = 0.99;
            if (SuccessChance < 0.0) SuccessChance = 0.0;
            lbChance.Text = (SuccessChance * 100.0).ToString("N1") + "%";

            // Cost of this modification
            ModifyCost = (WeaponToModify.UnmodifiedCost * mod.CostMod + Const.ModificationCost) * PriceMod;
            lbCost.Text = ModifyCost.ToString("N2") + "cr";
            btModify.Enabled = (ModifyCost <= PlayerTeam.Cash);
        }

        private void btModify_Click(object sender, EventArgs e) {
            if (PlayerTeam.Cash < ModifyCost) {
                MessageBox.Show("This modification would cost more than your available cash reserves");
                return;
            }

            string newMod = cbModType?.SelectedItem?.ToString() ?? string.Empty;
            WeaponMod? mod = StaticData.GetWeaponModByName(newMod);
            if (mod == null) return;

            // Are you sure?
            if (MessageBox.Show("Adding a " + mod.Name + " will cost " + ModifyCost.ToString("N2") + " and has a " + (100 - (SuccessChance * 100.0)).ToString("N0") + "% chance of failure. A failed modification attempt may result in your item being downgraded or destroyed. Continue anyway?", "Really Modify?", MessageBoxButtons.YesNo) == DialogResult.No) return;

            // Attempt to modify the item
            Modified = true;
            PlayerTeam.Cash -= ModifyCost;
            SoundEffects.PlaySound("CashRegister");
            Random rand = new Random();
            double r = rand.NextDouble();
            if (r <= SuccessChance) {
                MessageBox.Show($"Great news! Your {WeaponToModify.Name} has been fitted with a {mod.Name}");
                NewItem = new Weapon(WeaponToModify);
                ((Weapon)NewItem).SetModifier(mod);
            }
            else {
                int iNewLevel = WeaponToModify.Level - (int)Math.Floor((r - SuccessChance) / 0.35);
                if (iNewLevel >= 0) {
                    NewItem = new Weapon(WeaponToModify, iNewLevel);
                }
                // Display result
                if (iNewLevel == WeaponToModify.Level) {
                    MessageBox.Show("Unfortunately the modification has not succeeded. Fortuantely, your " + WeaponToModify.Name + " was not damaged in the process");
                }
                else if (iNewLevel < 0) {
                    MessageBox.Show("Unfortunately the upgrade has failed and your " + WeaponToModify.Name + " has been destroyed");
                    Destroyed = true;
                }
                else {
                    MessageBox.Show("Unfortunately the upgrade has failed and your " + WeaponToModify.Name + " has been downgraded to " + Utils.LevelToDescription(iNewLevel));
                }
            }

            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cbModType_SelectedIndexChanged(object sender, EventArgs e) {
            UpdatedSelection();
        }
    }
}
