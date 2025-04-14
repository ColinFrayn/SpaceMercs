using Timer = System.Windows.Forms.Timer;

namespace SpaceMercs.Dialogs {
    internal partial class ScanPlanet : Form {
        private readonly OrbitalAO _aoScan;
        private readonly Team _playerTeam;
        private readonly Timer clockTick;
        private readonly Func<Mission, bool> StartMission;
        private int iProgress = 0;
        private readonly GlobalClock _clock;

        public ScanPlanet(OrbitalAO aoScan, Team team, Func<Mission, bool> _startMission, GlobalClock clock) {
            _aoScan = aoScan;
            _playerTeam = team;
            _clock = clock;
            StartMission = _startMission;
            InitializeComponent();
            clockTick = new Timer();
            clockTick.Tick += new EventHandler(UpdateScan);
            clockTick.Interval = 250;
            btRegenerate.Enabled = Const.DEBUG_RANDOMISE_VENDORS && _aoScan.Scanned;
            btRegenerate.Visible = Const.DEBUG_RANDOMISE_VENDORS && _aoScan.Scanned;

            if (!_aoScan.Scanned) {
                lbNone.Visible = false;
                dgMissions.Visible = false;
                pbScan.Visible = false;
                btRunMission.Text = _aoScan switch {
                    Planet _ => "Scan Planet",
                    Moon _ => "Scan Moon",
                    SpaceHulk _ => "Scan Hulk",
                    _ => throw new NotImplementedException($"Cannot scan type : {_aoScan?.GetType()}")
                };
            }
            else DisplayMissionList();
        }

        private void RunScan() {
            // Scan for new missions
            pbScan.Value = 0;
            pbScan.Visible = true;
            btRunMission.Text = "Scanning...";
            btRunMission.Enabled = false;
            clockTick.Start();
        }
        private void UpdateScan(object? myObject, EventArgs myEventArgs) {
            iProgress++;
            if (iProgress <= 24) _clock.AddHours(1);
            pbScan.Value = Math.Min(iProgress, 24);
            if (iProgress > 25) {
                clockTick.Stop();
                ScanComplete();
            }
        }
        private void ScanComplete() {
            Random rnd = new Random();
            int nm = 2 + rnd.Next(3);
            _aoScan.ClearMissions();
            if (_aoScan.Type == Planet.PlanetType.Oceanic) nm++;
            if (_aoScan.Type == Planet.PlanetType.Gas) nm--;
            if (_aoScan is Planet pl) {
                if (pl.IsPrecursor) {
                    nm = 0;
                    pl.SetupPrecursorMissions(rnd);
                }
                else nm++;
            }
            else if (_aoScan is SpaceHulk sh) {
                nm = 0;
                sh.SetupSpaceHulkMissions(rnd);
            }
            else if (nm < 2) nm = 2;
            for (int n = 0; n < nm; n++) {
                Mission m = Mission.CreateRandomScannerMission(_aoScan, rnd);
                _aoScan.AddMission(m);
            }
            _aoScan.SetScanned();
            btRegenerate.Enabled = Const.DEBUG_RANDOMISE_VENDORS;
            btRegenerate.Visible = Const.DEBUG_RANDOMISE_VENDORS;
            pbScan.Visible = false;
            DisplayMissionList();
        }
        private void DisplayMissionList() {
            pbScan.Visible = false;
            btRunMission.Text = "Run Mission";
            btRunMission.Enabled = false;
            if (!_aoScan.MissionList.Any()) {
                dgMissions.Visible = false;
                lbNone.Visible = true;
                return;
            }
            lbNone.Visible = false;
            dgMissions.Visible = true;
            dgMissions.Rows.Clear();
            string[] arrRowMiss = new string[5];
            foreach (Mission miss in _aoScan.MissionList) {
                arrRowMiss[0] = miss.Summary;
                arrRowMiss[1] = Utils.MissionGoalToString(miss.Goal);
                string secondary = miss.SecondaryEnemy is null ? string.Empty : " [+]";
                if (miss.RacialOpponent != null) {
                    if (miss.RacialOpponent.Known) arrRowMiss[2] = miss.RacialOpponent.Name + " " + miss.PrimaryEnemy.Name + secondary;
                    else arrRowMiss[2] = "Alien " + miss.PrimaryEnemy.Name + secondary;
                }
                else arrRowMiss[2] = miss.PrimaryEnemy.Name + miss.SwarmLevelText + secondary;
                arrRowMiss[3] = miss.Diff.ToString();
                arrRowMiss[4] = Utils.MapSizeToDescription(miss.Size) + (miss.LevelCount > 1 ? " * " + miss.LevelCount.ToString() : "");

                dgMissions.Rows.Add(arrRowMiss);
                dgMissions.Rows[dgMissions.Rows.Count - 1].Tag = miss;
            }
            dgMissions.ClearSelection();
        }

        // Run selected mission *or* run a scan if no scan has yet been done
        private void btRunMission_Click(object sender, EventArgs e) {
            if (_playerTeam.CurrentPosition != _aoScan) {
                MessageBox.Show("Your team is no longer located here!");
                return;
            }
            if (!_aoScan.Scanned) {
                RunScan();
                return;
            }
            if (dgMissions.SelectedRows.Count != 1) return;
            Mission miss = dgMissions.SelectedRows[0].Tag as Mission ?? throw new Exception("Mission could not be found to run");
            if (StartMission(miss)) {
                _aoScan.RemoveMission(miss);
                this.Close();
            }
        }

        private void dgMissions_SelectionChanged(object sender, EventArgs e) {
            if (dgMissions.SelectedRows.Count == 1) btRunMission.Enabled = true;
            else btRunMission.Enabled = false;
        }

        private void dgMissions_DoubleClick(object sender, EventArgs e) {
            if (dgMissions.SelectedRows.Count != 1) return;
            Mission miss = dgMissions.SelectedRows[0].Tag as Mission ?? throw new Exception("Could not convert to Mission");
            MessageBox.Show(this, miss.GetDescription(), "Mission Details");
        }

        private void btRegenerate_Click(object sender, EventArgs e) {
            ScanComplete();
        }
    }
}