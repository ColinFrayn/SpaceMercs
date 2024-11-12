using SpaceMercs.Items;
using Timer = System.Windows.Forms.Timer;

namespace SpaceMercs.Dialogs {
    internal partial class ResearchItem : Form {
        private readonly Team _playerTeam;
        private readonly Timer clockTick;
        private IResearchable _typeToResearch;
        private int iProgress = 0;
        private double durationSeconds = 0d;
        private bool configuring = false;

        public ResearchItem(Team team) {
            _playerTeam = team;
            configuring = true;
            InitializeComponent();
            clockTick = new Timer();
            clockTick.Tick += new EventHandler(UpdateResearchProgress);
            clockTick.Interval = 250;
            btResearch.Enabled = false;
            DisplayAvailableResearch();
            configuring = false;
        }

        private void StartResearch(IResearchable item) {
            if (item?.Requirements is null) return;
            double cost = item.Requirements.CashCost;
            if (cost > _playerTeam.Cash) return;
            _playerTeam.Cash -= cost;

            foreach ((MaterialType mat, int count) in item.Requirements.RequiredMaterials) {
                _playerTeam.RemoveMaterial(mat, count);
            }

            pbResearch.Value = 0;
            pbResearch.Visible = true;
            btResearch.Text = "Researching...";
            btResearch.Enabled = false;
            durationSeconds = item.Requirements.Duration * Const.SecondsPerDay;
            _typeToResearch = item;
            iProgress = 0;
            clockTick.Start();
        }
        private void UpdateResearchProgress(object? myObject, EventArgs myEventArgs) {
            iProgress++;
            if (iProgress <= 24) Const.dtTime = Const.dtTime.AddSeconds(durationSeconds / 24.0);
            pbResearch.Value = Math.Min(iProgress, 24);
            if (iProgress > 25) {
                clockTick.Stop();
                ResearchCompleted();
            }
        }
        private void ResearchCompleted() {
            pbResearch.Visible = false;
            iProgress = 0;
            if (_typeToResearch is null) throw new Exception("Found a null item to research!");
            StaticData.Races[0].CompleteResearch(_typeToResearch);
            DisplayAvailableResearch();
        }
        private void DisplayAvailableResearch() {
            configuring = true;
            pbResearch.Visible = false;
            btResearch.Text = "Research";
            btResearch.Enabled = false;
            dgResearchItems.Visible = true;
            dgResearchItems.Rows.Clear();
            dgResearchItems.ClearSelection();
            string[] arrRowDest = new string[5];
            foreach (IResearchable it in _playerTeam.ResearchableItems) {
                if (it.Requirements is null) continue; // Should never happen
                arrRowDest[0] = it.Name;
                double cost = it.Requirements.CashCost;
                double durationDays = it.Requirements.Duration;
                bool hasMats = true;
                foreach (KeyValuePair<MaterialType, int> kvp in it.Requirements.RequiredMaterials) {
                    if (_playerTeam.CountMaterial(kvp.Key) < kvp.Value) hasMats = false;
                }
                arrRowDest[1] = it.GetType().Name;
                arrRowDest[2] = it.Requirements.RequiredMaterials.Count == 0 ? "-" : (hasMats ? "OK" : "<missing>");
                arrRowDest[3] = $"{Math.Round(cost, 2)}cr";
                arrRowDest[4] = $"{Math.Round(durationDays, 2)}d";
                dgResearchItems.Rows.Add(arrRowDest);
                var row = dgResearchItems.Rows[dgResearchItems.Rows.Count - 1];
                row.Tag = it;
                // If this row can be researched then set it bold, otherwise not
                if (it.Requirements.MeetsRequirements(_playerTeam)) {
                    row.DefaultCellStyle.Font = new Font(dgResearchItems.DefaultCellStyle.Font, FontStyle.Bold);
                }
                else {
                    row.DefaultCellStyle.Font = new Font(dgResearchItems.DefaultCellStyle.Font, FontStyle.Italic);
                }
            }
            configuring = false;
        }

        // Perform research, if possible
        private void btResearch_Click(object sender, EventArgs e) {
            if (configuring) return;
            if (dgResearchItems.SelectedRows.Count != 1) return;
            IResearchable item = dgResearchItems.SelectedRows[0].Tag as IResearchable ?? throw new Exception("Tech could not be found to research");
            StartResearch(item);
        }

        private void dgResearchItems_SelectionChanged(object sender, EventArgs e) {
            if (!configuring && dgResearchItems.SelectedRows.Count == 1) {
                if (dgResearchItems.SelectedRows[0].Tag is IResearchable item) {
                    btResearch.Enabled = item.Requirements?.MeetsRequirements(_playerTeam) == true;
                }
            }
            else btResearch.Enabled = false;
        }

        private void dgResearchItems_DoubleClick(object sender, EventArgs e) {
            if (configuring) return;
            if (dgResearchItems.SelectedRows.Count != 1) return;
            if (dgResearchItems.SelectedRows[0].Tag is IResearchable item) {
                string desc = $"{item.Name}\n{item.Description}\nRequirements:\n{item.Requirements?.Description}";
                MessageBox.Show(this, desc);
            }
        }
    }
}