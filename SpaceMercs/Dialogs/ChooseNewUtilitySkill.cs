using System;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
  partial class ChooseNewUtilitySkill : Form {
    public Soldier.UtilitySkill ChosenSkill = Soldier.UtilitySkill.Unspent;
    public ChooseNewUtilitySkill(Soldier s) {
      InitializeComponent();
      lbSkills.Items.Clear();
      foreach (Soldier.UtilitySkill sk in s.UnknownUtilitySkills()) {
        lbSkills.Items.Add(sk);
      }
      btChoose.Enabled = false;
    }

    private void btChoose_Click(object sender, EventArgs e) {
      if (lbSkills.SelectedIndex < 0) return;
      if (MessageBox.Show(this,"Really learn this skill?", "Learn skill?", MessageBoxButtons.YesNo) == DialogResult.No) return;
      ChosenSkill = (Soldier.UtilitySkill)Enum.Parse(typeof(Soldier.UtilitySkill), lbSkills.SelectedItem.ToString());
      this.Close();
    }

    private void lbSkills_SelectedIndexChanged(object sender, EventArgs e) {
      btChoose.Enabled = true;
    }
  }
}
