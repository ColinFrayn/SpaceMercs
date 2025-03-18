using OpenTK.Mathematics;
using Timer = System.Windows.Forms.Timer;

namespace SpaceMercs.Dialogs {
    internal partial class HyperspaceTravel : Form {
        private readonly HyperGate _hGate;
        private readonly Team _playerTeam;
        private readonly Timer clockTick;
        private readonly Action<AstronomicalObject> ArriveAt;
        private int iProgress = 0;
        private HyperGate? _hDest = null;
        private double durationSeconds = 0d;
        private readonly Race? _owner;
        private readonly GlobalClock _clock;
        private readonly Dictionary<Star, (double Distance, int Hops)> Targets = new();

        public HyperspaceTravel(HyperGate hGate, Team team, Action<AstronomicalObject> _arriveAt, GlobalClock clock) {
            _hGate = hGate;
            _playerTeam = team;
            _owner = hGate.GetSystem().Owner;
            _clock = clock;
            ArriveAt = _arriveAt;
            InitializeComponent();
            clockTick = new Timer();
            clockTick.Tick += new EventHandler(UpdateTravel);
            clockTick.Interval = 200;
            btHyperspaceTravel.Enabled = false;
            DisplayHyperspaceDestinations();
        }

        private double CalculateCost(double distLy) {
            double cost = Math.Pow(distLy, Const.HyperspaceCostDistanceExponent) * Math.Pow(_playerTeam.PlayerShip.Type.MaxHull, Const.HyperspaceCostHullExponent) / Const.HyperspaceCostScale;
            if (_owner is not null && _owner != StaticData.HumanRace) cost *= Utils.RelationsToHyperspaceCost(_playerTeam.GetRelations(_owner));
            return cost;
        }
        private void StartTravel(HyperGate targetGate) {
            double distLy = AstronomicalObject.CalculateDistance(_hGate, targetGate) / Const.LightYear;
            double cost = CalculateCost(distLy);
            if (cost > _playerTeam.Cash) return;
            _playerTeam.Cash -= cost;
            pbTravel.Value = 0;
            pbTravel.Visible = true;
            btHyperspaceTravel.Text = "In Hyperspace...";
            btHyperspaceTravel.Enabled = false;
            _hDest = targetGate;
            double durationYears = distLy / Const.HyperspaceGateTimeFactor;
            durationSeconds = durationYears * Const.SecondsPerYear;
            clockTick.Start();
        }
        private void UpdateTravel(object? myObject, EventArgs myEventArgs) {
            iProgress++;
            if (iProgress <= 24) _clock.AddSeconds(durationSeconds/24d);
            pbTravel.Value = Math.Min(iProgress, 24);
            if (iProgress > 25) {
                clockTick.Stop();
                Arrived();
            }
        }
        private void Arrived() {
            pbTravel.Visible = false;
            if (_hDest is null) throw new Exception("Arrived at null destination!");
            ArriveAt(_hDest);
            this.Close();
        }
        private void DisplayHyperspaceDestinations() {
            pbTravel.Visible = false;
            btHyperspaceTravel.Text = "Hyperspace";
            btHyperspaceTravel.Enabled = false;
            dgDestinations.Visible = true;
            dgDestinations.Rows.Clear();
            CalculateRoutes();
            string[] arrRowDest = new string[6];
            foreach (Star st in Targets.Keys) {
                (double distLy, int hops) = Targets[st];
                HyperGate targ = st?.GetHyperGate() ?? throw new Exception("TradeRoute system doesn't have a gate");
                double cost = CalculateCost(distLy);
                double durationYears = distLy / Const.HyperspaceGateTimeFactor;
                double durationDays = (int)(durationYears * 365.2422);
                Vector3 loc = st.GetMapLocation();
                arrRowDest[0] = st!.Name;
                arrRowDest[1] = $"({loc.X},{loc.Y})";
                arrRowDest[2] = $"{Math.Round(distLy, 2)}ly";
                arrRowDest[3] = $"{durationDays}d";
                arrRowDest[4] = hops.ToString();
                arrRowDest[5] = $"{Math.Round(cost,2)}cr";
                dgDestinations.Rows.Add(arrRowDest);
                dgDestinations.Rows[dgDestinations.Rows.Count - 1].Tag = targ;
            }
            dgDestinations.ClearSelection();
        }
        private void CalculateRoutes() {
            Targets.Clear();
            Star gateStar = _hGate.GetSystem();
            HashSet<Star> queue = [gateStar];
            while (queue.Count > 0) {
                Star current = queue.First();
                queue.Remove(current);
                double dist = 0d;
                int hops = 0;
                if (Targets.ContainsKey(current)) {
                    dist = Targets[current].Distance;
                    hops = Targets[current].Hops;
                }
                foreach (Star st in current.TradeRoutes) {
                    if (st == gateStar) continue;
                    double sdist = dist + AstronomicalObject.CalculateDistance(current, st) / Const.LightYear;
                    if (!Targets.ContainsKey(st)) {
                        Targets.Add(st, (sdist, hops + 1));
                        queue.Add(st);
                    }
                    else {
                        if (sdist < Targets[st].Distance || (sdist == Targets[st].Distance && hops + 1 < Targets[st].Hops)) {
                            Targets[st] = (sdist, hops + 1);
                            queue.Add(st);
                        }
                    }
                }
            }
            Targets.Remove(gateStar);
        }

        // Travel, if possible
        private void btHyperspaceTravel_Click(object sender, EventArgs e) {
            if (_playerTeam.CurrentPosition != _hGate) {
                MessageBox.Show("Your team is no longer located here!");
                return;
            }
            if (dgDestinations.SelectedRows.Count != 1) return;
            HyperGate dest = dgDestinations.SelectedRows[0].Tag as HyperGate ?? throw new Exception("Destination gate could not be found to run");
            StartTravel(dest);
        }

        private void dgDestinations_SelectionChanged(object sender, EventArgs e) {
            if (dgDestinations.SelectedRows.Count == 1) {
                HyperGate dest = dgDestinations.SelectedRows[0].Tag as HyperGate ?? throw new Exception("Destination gate could not be found to run");
                double distLy = AstronomicalObject.CalculateDistance(_hGate, dest) / Const.LightYear;
                double cost = distLy * _playerTeam.PlayerShip.Type.MaxHull / Const.HyperspaceCostScale;
                btHyperspaceTravel.Enabled = _playerTeam.Cash >= cost;
            }
            else btHyperspaceTravel.Enabled = false;
        }
    }
}