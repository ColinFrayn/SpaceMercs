using System.Drawing.Drawing2D;

namespace SpaceMercs.Dialogs {
    partial class UpgradeItem : Form {
        private readonly IEquippable item;
        private readonly double UpgradeCost;
        private readonly double SuccessChance;
        private readonly Team PlayerTeam;
        private readonly Soldier? ThisSoldier;
        public IEquippable? NewItem = null; // If this is set to non-null then it represents the new item after modification
        public bool Upgraded = false; // Did we attempt the upgrade? (i.e. should we remove the old item?) We need this in case the upgrade destroys the object, in which case NewItem will be null.
        public bool Destroyed = false; // Destroyed in attempting to upgrade?

        public UpgradeItem(IEquippable eq, double PriceMod, int skill, int aiboost, Team t, Soldier? s) {
            InitializeComponent();
            item = eq;
            PlayerTeam = t;
            ThisSoldier = s;
            Upgraded = false;
            Destroyed = false;
            if (ThisSoldier == null) lbSoldier.Visible = false;
            else {
                lbSoldier.Visible = true;
                Soldier.UtilitySkill skType = Soldier.UtilitySkill.Armoursmith;
                if (eq is Weapon wp) {
                    if (wp.Type.IsMeleeWeapon) skType = Soldier.UtilitySkill.Bladesmith;
                    else skType = Soldier.UtilitySkill.Gunsmith;
                }
                lbSoldier.Text = $"{ThisSoldier.Name} ({skType} : {skill}{(aiboost > 0 ? "+" + aiboost : string.Empty)})";

            }
            lbName.Text = item.Name;
            lbQuality.Text = Utils.LevelToDescription(item.Level);
            lbNewQuality.Text = Utils.LevelToDescription(item.Level + 1);
            // Generate the item
            IEquippable? newItem = eq switch {
                Armour ar => new Armour(ar.Type, ar.Material, ar.Level + 1),
                Weapon wp => new Weapon(wp.Type, wp.Level + 1),
                _ => throw new Exception("Cannot upgrade this type!")
            };
            double ItemLevel = newItem.BuildDiff - 1; // Easier to upgrade to this level than it is to build it anew, hence -1
            double TotalSkill = skill + aiboost;
            SuccessChance = Utils.ConstructionChance(ItemLevel, TotalSkill);
            lbChance.Text = SuccessChance.ToString("N1") + "%";
            UpgradeCost = item.UpgradeCost * PriceMod;
            lbCost.Text = UpgradeCost.ToString("N2") + "cr";
            if (UpgradeCost > PlayerTeam.Cash || item.Level == Const.MaxItemLevel) btUpgrade.Enabled = false;
        }

        private void btUpgrade_Click(object sender, EventArgs e) {
            // Are you sure?
            if (MessageBox.Show("This will cost " + UpgradeCost.ToString("N2") + " and has a " + (100 - SuccessChance).ToString("N0") + "% chance of failure. A failed upgrade attempt will result in your item being downgraded or destroyed. Continue anyway?", "Really Upgrade?", MessageBoxButtons.YesNo) == DialogResult.No) return;

            // Attempt to upgrade the item
            Upgraded = true;
            PlayerTeam.Cash -= UpgradeCost;
            SoundEffects.PlaySound("CashRegister");
            Random rand = new Random();
            double r = rand.NextDouble() * 100d;
            int iNewLevel = item.Level;
            if (r <= SuccessChance) iNewLevel++;
            else iNewLevel -= (int)Math.Floor((r - SuccessChance) / 15d);
            if (iNewLevel >= 0) {
                if (item is Armour) NewItem = new Armour(item, iNewLevel);
                else NewItem = new Weapon(item, iNewLevel);
            }

            // Display result
            if (iNewLevel > item.Level) {
                MessageBox.Show("Congratulations! Your " + item.Name + " has been upgraded to " + Utils.LevelToDescription(iNewLevel) + "!");
            }
            else {
                if (iNewLevel == item.Level) {
                    MessageBox.Show("Unfortunately the upgrade has failed and your " + item.Name + " remains at a quality of " + Utils.LevelToDescription(iNewLevel));
                }
                else if (iNewLevel < 0) {
                    MessageBox.Show("Unfortunately the upgrade has failed and your " + item.Name + " has been destroyed");
                    Destroyed = true;
                }
                else {
                    MessageBox.Show("Unfortunately the upgrade has failed and your " + item.Name + " has been downgraded to " + Utils.LevelToDescription(iNewLevel));
                }
            }

            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
