using Timer = System.Windows.Forms.Timer;

namespace SpaceMercs.Dialogs {
    internal partial class ResearchItem : Form {
        private readonly Team _playerTeam;
        private readonly Timer clockTick;
        private BaseItemType _typeToResearch;
        private int iProgress = 0;
        private double durationSeconds = 0d;

        public ResearchItem(Team team) {
            _playerTeam = team;
            InitializeComponent();
            clockTick = new Timer();
            clockTick.Tick += new EventHandler(UpdateResearchProgress);
            clockTick.Interval = 250;
            btResearch.Enabled = false;
            DisplayAvailableResearch();
        }

        private void StartResearch(BaseItemType item) {
            if (item?.Requirements is null) return;
            double cost = item.Requirements.CashCost;
            if (cost > _playerTeam.Cash) return;
            _playerTeam.Cash -= cost;
            // TODO Remove Materials
            pbResearch.Value = 0;
            pbResearch.Visible = true;
            btResearch.Text = "Researching...";
            btResearch.Enabled = false;
            durationSeconds = item.Requirements.Duration;
            _typeToResearch = item;
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
            if (_typeToResearch is null) throw new Exception("Found a null item to research!");
            StaticData.Races[0].CompleteResearch(_typeToResearch);
            // Announce it?
            // TODO
            DisplayAvailableResearch();
        }
        private void DisplayAvailableResearch() {
            pbResearch.Visible = false;
            btResearch.Text = "Research";
            btResearch.Enabled = false;
            dgResearchItems.Visible = true;
            dgResearchItems.Rows.Clear();
            string[] arrRowDest = new string[5];
            foreach (BaseItemType it in _playerTeam.ResearchableItems) {
                if (it.Requirements is null) continue; // Should never happen
                arrRowDest[0] = it.Name;
                double cost = it.Requirements.CashCost;
                double durationDays = it.Requirements.Duration;
                arrRowDest[1] = "<materials>"; // TODO
                arrRowDest[2] = $"{Math.Round(cost, 2)}cr";
                arrRowDest[3] = $"{Math.Round(durationDays, 2)}d";
                dgResearchItems.Rows.Add(arrRowDest);
                dgResearchItems.Rows[dgResearchItems.Rows.Count - 1].Tag = it;
            }
            dgResearchItems.ClearSelection();
        }

        // Travel, if possible
        private void btResearch_Click(object sender, EventArgs e) {
            if (dgResearchItems.SelectedRows.Count != 1) return;
            BaseItemType item = dgResearchItems.SelectedRows[0].Tag as BaseItemType ?? throw new Exception("Tech could not be found to research");
            StartResearch(item);
        }

        private void dgResearchItems_SelectionChanged(object sender, EventArgs e) {
            if (dgResearchItems.SelectedRows.Count == 1) {
                BaseItemType item = dgResearchItems.SelectedRows[0].Tag as BaseItemType ?? throw new Exception("Tech could not be found to research");
                btResearch.Enabled = item.Requirements?.MeetsRequirements(_playerTeam) == true;
            }
            else btResearch.Enabled = false;
        }
    }
}