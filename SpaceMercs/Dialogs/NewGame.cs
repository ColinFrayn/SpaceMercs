using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Mathematics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SpaceMercs.Dialogs {
    public partial class NewGame : Form {
        public int StarsPerSector { get { return tbStarsPerSector.Value; } }
        public int PlanetDensity { get { return tbPlanetDensity.Value; } }
        public int CivSize { get { return tbCivSize.Value; } }
        private int Unspent = Const.SpareAttributePoints;
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intellect { get; set; }
        public int Toughness { get; set; }
        public int Endurance { get; set; }
        public int Seed {
            get {
                try {
                    return Int32.Parse(tbSeed.Text);
                }
                catch {
                    Random rand = new Random();
                    return rand.Next(1000000);
                }
            }
        }
        public string PlayerName { get; set; }
        private readonly Race rc = StaticData.GetRaceByName("Human");

        // Set up everything
        public NewGame() {
            InitializeComponent();
            tbStarsPerSector.Minimum = Const.MinStarsPerSector;
            tbStarsPerSector.Maximum = Const.MaxStarsPerSector;
            tbStarsPerSector.Value = Const.DefaultStarsPerSector;
            tbPlanetDensity.Minimum = Const.MinPlanetDensity;
            tbPlanetDensity.Maximum = Const.MaxPlanetDensity;
            tbPlanetDensity.Value = Const.DefaultPlanetDensity;
            tbCivSize.Minimum = 1;
            tbCivSize.Maximum = Const.MinStarsPerSector;
            tbCivSize.Value = Const.MinStarsPerSector - 2;
            tbName.Text = "Unnamed";
            Strength = rc.Strength;
            Agility = rc.Agility;
            Intellect = rc.Intellect;
            Toughness = rc.Toughness;
            Endurance = rc.Endurance;
            Random rand = new Random();
            tbSeed.Text = rand.Next(1000000).ToString();
            SetupSkillButtons();
        }

        // Setup the skill buttons
        private void SetupSkillButtons() {

            lbUnspent.Text = Unspent.ToString();
            if (Unspent == 0) {
                btStrengthUp.Enabled = false;
                btAgilityUp.Enabled = false;
                btIntellectUp.Enabled = false;
                btToughnessUp.Enabled = false;
                btEnduranceUp.Enabled = false;
            }
            else {
                btStrengthUp.Enabled = true;
                btAgilityUp.Enabled = true;
                btIntellectUp.Enabled = true;
                btToughnessUp.Enabled = true;
                btEnduranceUp.Enabled = true;
            }

            // Strength
            lbStrength.Text = Strength.ToString();
            if (Strength + Const.MaximumSkillDeficit <= rc.Strength) btStrengthDown.Enabled = false;
            else btStrengthDown.Enabled = true;

            // Agility
            lbAgility.Text = Agility.ToString();
            if (Agility + Const.MaximumSkillDeficit <= rc.Agility) btAgilityDown.Enabled = false;
            else btAgilityDown.Enabled = true;

            // Intellect
            lbIntellect.Text = Intellect.ToString();
            if (Intellect + Const.MaximumSkillDeficit <= rc.Intellect) btIntellectDown.Enabled = false;
            else btIntellectDown.Enabled = true;

            // Toughness
            lbToughness.Text = Toughness.ToString();
            if (Toughness + Const.MaximumSkillDeficit <= rc.Toughness) btToughnessDown.Enabled = false;
            else btToughnessDown.Enabled = true;

            // Endurance
            lbEndurance.Text = Endurance.ToString();
            if (Endurance + Const.MaximumSkillDeficit <= rc.Endurance) btEnduranceDown.Enabled = false;
            else btEnduranceDown.Enabled = true;

        }

        // Close the window and generate the new map
        private void buttonGenerate_Click(object sender, EventArgs e) {
            if (Unspent > 0) {
                if (MessageBox.Show("You have unspent points. Really continue?", "Unspent points", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes) {
                    return;
                }
            }
            PlayerName = tbName.Text;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        // Alter skill points
        private void btStrengthUp_Click(object sender, EventArgs e) {
            if (Unspent == 0) return;
            Unspent--;
            Strength++;
            SetupSkillButtons();
        }
        private void btStrengthDown_Click(object sender, EventArgs e) {
            if (Strength + Const.MaximumSkillDeficit <= rc.Strength) return;
            Unspent++;
            Strength--;
            SetupSkillButtons();
        }
        private void btAgilityUp_Click(object sender, EventArgs e) {
            if (Unspent == 0) return;
            Unspent--;
            Agility++;
            SetupSkillButtons();
        }
        private void btAgilityDown_Click(object sender, EventArgs e) {
            if (Agility + Const.MaximumSkillDeficit <= rc.Agility) return;
            Unspent++;
            Agility--;
            SetupSkillButtons();
        }
        private void btIntellectUp_Click(object sender, EventArgs e) {
            if (Unspent == 0) return;
            Unspent--;
            Intellect++;
            SetupSkillButtons();
        }
        private void btIntellectDown_Click(object sender, EventArgs e) {
            if (Intellect + Const.MaximumSkillDeficit <= rc.Intellect) return;
            Unspent++;
            Intellect--;
            SetupSkillButtons();
        }
        private void btToughnessUp_Click(object sender, EventArgs e) {
            if (Unspent == 0) return;
            Unspent--;
            Toughness++;
            SetupSkillButtons();
        }
        private void btTougnessDown_Click(object sender, EventArgs e) {
            if (Toughness + Const.MaximumSkillDeficit <= rc.Toughness) return;
            Unspent++;
            Toughness--;
            SetupSkillButtons();
        }
        private void btEnduranceUp_Click(object sender, EventArgs e) {
            if (Unspent == 0) return;
            Unspent--;
            Endurance++;
            SetupSkillButtons();
        }
        private void btEnduranceDown_Click(object sender, EventArgs e) {
            if (Endurance + Const.MaximumSkillDeficit <= rc.Endurance) return;
            Unspent++;
            Endurance--;
            SetupSkillButtons();
        }

    }
}