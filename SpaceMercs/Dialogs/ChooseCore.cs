//using System;

namespace SpaceMercs.Dialogs {
    partial class ChooseCore : Form {
        public MissionItem? ChosenItem { get; private set; }
        private readonly Team playerTeam;
        private bool bHasItems = false;

        public ChooseCore(Team team, bool bShowHulkCores, bool bShowPrecursorCores) {
            InitializeComponent();
            playerTeam = team;
            bHasItems = false;
            UpdateItems(bShowHulkCores, bShowPrecursorCores);
            ChosenItem = null;
            lbItems.SelectedIndex = -1;
            btChooseCore.Enabled = false;
        }

        private record ItemPair(string Desc, MissionItem? Mi);

        private void UpdateItems(bool bShowHulkCores, bool bShowPrecursorCores) {
            lbItems.Items.Clear();
            lbItems.DisplayMember = "Desc";
            lbItems.ValueMember = "Eq";
            bHasItems = false;
            Dictionary<MissionItem, int> cores = new();
            foreach (KeyValuePair<IItem, int> kvp in playerTeam.Inventory) {
                if (kvp.Key is not MissionItem mi) continue;
                if ((mi.IsSpaceHulkCore && bShowHulkCores) || (mi.IsPrecursorCore && bShowPrecursorCores)) {
                    if (cores.ContainsKey(mi)) cores[mi] += kvp.Value;
                    else cores.Add(mi, kvp.Value);
                    bHasItems = true;
                }
            }
            foreach (Soldier s in playerTeam.SoldiersRO.Where(s => s.aoLocation == playerTeam.CurrentPosition)) {
                foreach (KeyValuePair<IItem, int> kvp in s.InventoryGrouped) {
                    if (kvp.Key is not MissionItem mi) continue;
                    if ((mi.IsSpaceHulkCore && bShowHulkCores) || (mi.IsPrecursorCore && bShowPrecursorCores)) {
                        if (cores.ContainsKey(mi)) cores[mi] += kvp.Value;
                        else cores.Add(mi, kvp.Value);
                        bHasItems = true;
                    }
                }
            }

            foreach ((MissionItem mi, int count) in cores) {
                string strQuantity = count == 1 ? "" : $" [{count}]";
                lbItems.Items.Add(new ItemPair($"{mi.Name} L{mi.Level}{strQuantity}", mi));
            }
            if (cores.Count == 0) lbItems.Items.Add(new ItemPair("<None>", null));
        }

        private void btChooseCore_Click(object sender, EventArgs e) {
            ChosenItem = (lbItems.SelectedItem is ItemPair ip) ? ip.Mi : null;
            this.Close();
        }

        private void lbItems_SelectedValueChanged(object sender, EventArgs e) {
            if (lbItems.SelectedIndex == -1 || !bHasItems) {
                btChooseCore.Enabled = false;
                return;
            }
            MissionItem? mi = (lbItems.SelectedItem is ItemPair ip) ? ip.Mi : null;
            btChooseCore.Enabled = (mi is not null);
        }
    }
}