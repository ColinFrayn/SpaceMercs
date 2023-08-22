//using System;

namespace SpaceMercs.Dialogs {
    partial class UseItem : Form {
        private readonly Soldier ThisSoldier;
        private bool bHasItems = false;
        public IEquippable? ChosenItem { get; private set; }

        public UseItem(Soldier s) {
            ThisSoldier = s;
            InitializeComponent();
            UpdateItems();
            ChosenItem = null;
            lbItems.SelectedIndex = -1;
            btUseItem.Enabled = false;
        }

        private record ItemPair(string Desc, IEquippable? Eq);

        private void UpdateItems() {
            lbItems.Items.Clear();
            lbItems.DisplayMember = "Desc";
            lbItems.ValueMember = "Eq";
            bHasItems = false;
            foreach (KeyValuePair<IItem, int> kvp in ThisSoldier.InventoryGrouped) {
                if (kvp.Key is not IEquippable eq) continue;
                ItemEffect? ie = eq.BaseType.ItemEffect;
                if (ie is not null) {
                    if (ie.AssociatedSkill == Soldier.UtilitySkill.Unspent || !ie.SkillRequired || ThisSoldier.GetUtilityLevel(ie.AssociatedSkill) > 0) {
                        string strReuse = "";
                        if (eq is Equipment eqi && eqi.BaseType.ItemEffect != null) {
                            if (eqi.BaseType.ItemEffect.SingleUse) strReuse = " sgl";
                            if (eqi.Recharge > 0) strReuse = $" ...Recharging({eqi.Recharge})";
                        }
                        string strQuantity = kvp.Value == 1 ? "" : $" [{kvp.Value}]";
                        lbItems.Items.Add(new ItemPair($"{eq.Name}{strQuantity}{strReuse}", eq));
                        bHasItems = true;

                        // Set this item in the list box to relate to the actual item type?
                        // TODO
                    }
                }
            }
            if (lbItems.Items.Count == 0) lbItems.Items.Add(new ItemPair("<None>", null));
        }

        private void btUseItem_Click(object sender, EventArgs e) {
            ChosenItem = (lbItems.SelectedItem is ItemPair ip) ? ip.Eq : null;
            if (ChosenItem is not null && ChosenItem is Equipment eq && eq.Recharge > 0) {
                ChosenItem = null;
                return;
            }
            this.Close();
        }

        private void lbItems_SelectedValueChanged(object sender, EventArgs e) {
            if (lbItems.SelectedIndex == -1 || !bHasItems) {
                btUseItem.Enabled = false;
                return;
            }
            IEquippable? it = (lbItems.SelectedItem is ItemPair ip) ? ip.Eq : null;
            if (it is not null && it is Equipment eq && eq.Recharge > 0) {
                btUseItem.Enabled = false;
                return;
            }
            btUseItem.Enabled = true;
        }
    }
}