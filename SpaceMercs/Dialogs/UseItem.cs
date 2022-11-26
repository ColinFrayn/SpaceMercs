using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
  partial class UseItem : Form {
    private readonly Soldier ThisSoldier;
    private bool bHasItems = false;
    public IEquippable ChosenItem { get; private set; }

    public UseItem(Soldier s) {
      ThisSoldier = s;
      InitializeComponent();
      UpdateItems();
      ChosenItem = null;
      lbItems.SelectedIndex = -1;
      btUseItem.Enabled = false;
    }

    class ItemPair {
      public string Desc { get; set; }
      public IEquippable Eq { get; set; }
    }

    private void UpdateItems() {
      lbItems.Items.Clear();
      lbItems.DisplayMember = "Desc";
      lbItems.ValueMember = "Eq";
      bHasItems = false;
      foreach (KeyValuePair<IItem, int> kvp in ThisSoldier.InventoryRO) {
        if (!(kvp.Key is IEquippable eq)) continue;
        ItemEffect ie = eq.BaseType.ItemEffect;
        if (ie != null) { 
          if (ie.AssociatedSkill == Soldier.UtilitySkill.Unspent || !ie.SkillRequired || ThisSoldier.GetUtilityLevel(ie.AssociatedSkill) > 0) {
            string strReuse = "";
            if (eq is Equipment eqi && eqi.BaseType.ItemEffect != null) {
              if (eqi.BaseType.ItemEffect.SingleUse) strReuse = " sgl";
            }
            lbItems.Items.Add(new ItemPair { Desc = $"{eq.Name} [{kvp.Value}]" + strReuse, Eq = eq });
            bHasItems = true;
            // Set this item in the list box to relate to the actual item type
            // TODO
          }
        }
      }
      if (lbItems.Items.Count == 0) lbItems.Items.Add(new ItemPair { Desc = "<None>", Eq = null });
    }

    private void btUseItem_Click(object sender, EventArgs e) {
      ChosenItem = (lbItems.SelectedItem as ItemPair)?.Eq;
      this.Close();
    }

    private void lbItems_SelectedValueChanged(object sender, EventArgs e) {
      if (lbItems.SelectedIndex == -1 || !bHasItems) {
        btUseItem.Enabled = false;
        return;
      }
      btUseItem.Enabled = true;
    }
  }
}
