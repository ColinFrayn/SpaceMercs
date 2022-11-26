using System;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
  partial class ChooseStat : Form {
    private readonly Soldier ThisSoldier = null;

    public ChooseStat(Soldier s) {
      ThisSoldier = s;
      InitializeComponent();
    }

    private void btStrength_Click(object sender, EventArgs e) {
      ThisSoldier.IncreaseStat(StatType.Strength);
      this.Close();
    }

    private void btAgility_Click(object sender, EventArgs e) {
      ThisSoldier.IncreaseStat(StatType.Agility);
      this.Close();
    }

    private void btIntelligence_Click(object sender, EventArgs e) {
      ThisSoldier.IncreaseStat(StatType.Intelligence);
      this.Close();
    }

    private void btToughness_Click(object sender, EventArgs e) {
      ThisSoldier.IncreaseStat(StatType.Toughness);
      this.Close();
    }

    private void btEndurance_Click(object sender, EventArgs e) {
      ThisSoldier.IncreaseStat(StatType.Endurance);
      this.Close();
    }
  }
}
