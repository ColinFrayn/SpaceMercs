using System;

namespace SpaceMercs.Dialogs {
    public partial class RaceView : Form {
        Race rSelected;
        private readonly Team _team;

        public RaceView(Team team) {
            InitializeComponent();
            _team = team;
            rSelected = StaticData.HumanRace;
            cbRace.Items.Clear();
            foreach (Race rc in StaticData.Races) {
                cbRace.Items.Add(rc.Name);
            }
            cbRace.SelectedIndex = 0;
            SetupRaceDetails();
        }

        private void SetupRaceDetails() {
            cbRace.SelectedValue = rSelected.Name;
            pbColour.BackColor = rSelected.Colour;
            lbRelations.Text = rSelected.RelationsToString(_team);
            if (!rSelected.Known) {
                lbColoniesLabel.Visible = false;
                lbColonies.Visible = false;
                lbSystemsLabel.Visible = false;
                lbSystems.Visible = false;
                lbPopulationLabel.Visible = false;
                lbPopulation.Visible = false;
                lbHomeLabel.Visible = false;
                lbHome.Visible = false;
                tbDescription.Visible = false;
                lbExp.Visible = false;
            }
            else {
                lbColoniesLabel.Visible = true;
                lbColonies.Visible = true;
                lbSystemsLabel.Visible = true;
                lbSystems.Visible = true;
                lbPopulationLabel.Visible = true;
                lbPopulation.Visible = true;
                lbHomeLabel.Visible = true;
                lbHome.Visible = true;
                tbDescription.Visible = true;
                lbColonies.Text = rSelected.ColonyCount.ToString();
                lbSystems.Text = rSelected.SystemCount.ToString();
                lbPopulation.Text = rSelected.Population.ToString();
                lbHome.Text = "(" + Math.Round(rSelected.HomePlanet.GetSystem().MapPos.X, 2) + "," + Math.Round(rSelected.HomePlanet.GetSystem().MapPos.Y, 2) + ")";
                tbDescription.Text = rSelected.Description;
                int rel = _team.GetRelations(rSelected);
                if (rel < 5) {
                    lbExp.Visible = true;
                    double progress = _team.GetRelationsProgress(rSelected) * 100;
                    lbExp.Text = $"{progress:0}%";
                }
                else lbExp.Visible = false;
            }
        }

        private void cbRace_SelectedIndexChanged(object sender, EventArgs e) {
            int i = cbRace.SelectedIndex;
            rSelected = StaticData.Races[i];
            SetupRaceDetails();
        }
    }
}
