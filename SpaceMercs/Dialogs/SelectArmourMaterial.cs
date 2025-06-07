using System;

namespace SpaceMercs.Dialogs {
    partial class SelectArmourMaterial : Form {
        private readonly Dictionary<MaterialType, int> BaseMats;
        private readonly Team PlayerTeam;
        private readonly ArmourType ThisType;
        public MaterialType? SelectedMat { get; private set; }

        public SelectArmourMaterial(Dictionary<MaterialType, int> mats, ArmourType tp, Team pt) {
            BaseMats = new Dictionary<MaterialType, int>(mats);
            PlayerTeam = pt;
            ThisType = tp;

            InitializeComponent();
            lbArmourType.Text = tp.Name;
            cbMaterialType.Items.Clear();
            foreach (MaterialType mt in StaticData.Materials.Where(m => m.IsArmourMaterial && m.MaxLevel >= tp.MinMatLvl)) {
                int tot = ThisType.Size;
                if (BaseMats.ContainsKey(mt)) tot += BaseMats[mt];
                if (PlayerTeam.CountMaterial(mt) >= tot) {
                    cbMaterialType.Items.Add(mt.Name);
                }
            }
            if (cbMaterialType.Items.Count == 0) {
                cbMaterialType.Items.Add("None Available");
                btSelect.Enabled = false;
            }
            SetValues();
        }

        private void SetValues() {
            // Get selected material
            SelectedMat = StaticData.GetMaterialTypeByName(cbMaterialType.SelectedItem as string);
            if (SelectedMat == null) {
                lbArmour.Visible = false;
                lbMass.Visible = false;
                lbDiff.Visible = false;
                lbArmourLabel.Visible = false;
                lbMassLabel.Visible = false;
                lbDiffLabel.Visible = false;
                lbProtection.Visible = false;
                return;
            }
            lbArmour.Visible = true;
            lbArmour.Text = SelectedMat.ArmourMod.ToString();
            lbMass.Visible = true;
            lbMass.Text = SelectedMat.MassMod.ToString();
            lbDiff.Visible = true;
            double ccm = SelectedMat.ConstructionDiffModifier;
            lbDiff.Text = $"+{ccm.ToString("N1")}";
            lbArmourLabel.Visible = true;
            lbMassLabel.Visible = true;
            lbDiffLabel.Visible = true;
            lbProtection.Visible = true;
            lbProtection.Items.Clear();
            foreach (WeaponType.DamageType dt in SelectedMat.BonusArmour.Keys) {
                lbProtection.Items.Add(dt.ToString() + " : " + SelectedMat.BonusArmour[dt]);
            }
            if (lbProtection.Items.Count == 0) {
                lbProtection.Items.Add("<No extra protection>");
            }
        }

        private void btCancel_Click(object sender, EventArgs e) {
            SelectedMat = null;
            this.Close();
        }

        private void btSelect_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void cbMaterialType_SelectedIndexChanged(object sender, EventArgs e) {
            SetValues();
        }
    }
}
