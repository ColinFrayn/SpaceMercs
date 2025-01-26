namespace SpaceMercs.Dialogs {
    partial class ColonyView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            tcMain = new TabControl();
            tpMerchant = new TabPage();
            cbAffordable = new CheckBox();
            btRandomiseMerchant = new Button();
            lbTeamCashMerch = new Label();
            label3 = new Label();
            label1 = new Label();
            tbFilter = new TextBox();
            btBuyMerchant = new Button();
            dgMerchant = new DataGridView();
            colItem = new DataGridViewTextBoxColumn();
            colCost = new DataGridViewTextBoxColumn();
            Mass = new DataGridViewTextBoxColumn();
            colAvail = new DataGridViewTextBoxColumn();
            cbItemType = new ComboBox();
            tpMercenaries = new TabPage();
            lbTeamCashMercs = new Label();
            label12 = new Label();
            btRandomiseMercs = new Button();
            btHire = new Button();
            dgMercenaries = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            colFee = new DataGridViewTextBoxColumn();
            tpMissions = new TabPage();
            btRandomiseMissions = new Button();
            btAccept = new Button();
            dgMissions = new DataGridView();
            colMission = new DataGridViewTextBoxColumn();
            colGoal = new DataGridViewTextBoxColumn();
            colOpp = new DataGridViewTextBoxColumn();
            colDiff = new DataGridViewTextBoxColumn();
            colLevels = new DataGridViewTextBoxColumn();
            colReward = new DataGridViewTextBoxColumn();
            tpShips = new TabPage();
            lbTeamCashShips = new Label();
            label13 = new Label();
            btUpgrade = new Button();
            dgShips = new DataGridView();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            tpUpgrade = new TabPage();
            btModify = new Button();
            cbEquipped = new CheckBox();
            lbTeamCashFoundry = new Label();
            label14 = new Label();
            btSellAll = new Button();
            btDismantle = new Button();
            btSell = new Button();
            btImprove = new Button();
            label2 = new Label();
            tbUpgradeFilter = new TextBox();
            cbUpgradeItemType = new ComboBox();
            dgInventory = new DataGridView();
            dgcItem = new DataGridViewTextBoxColumn();
            dgcLocation = new DataGridViewTextBoxColumn();
            dgcValue = new DataGridViewTextBoxColumn();
            dgcAvail = new DataGridViewTextBoxColumn();
            tpDetails = new TabPage();
            lbNextGrowth = new Label();
            label8 = new Label();
            lbLastGrowth = new Label();
            label6 = new Label();
            lbMetropolis = new Label();
            lbResidential = new Label();
            lbMilitary = new Label();
            lbTrade = new Label();
            lbResearch = new Label();
            lbColonySize = new Label();
            label4 = new Label();
            lbColonyName = new Label();
            groupBox1 = new GroupBox();
            groupBox4 = new GroupBox();
            lbSystemName = new Label();
            label10 = new Label();
            lbStarType = new Label();
            label11 = new Label();
            lbPlanetType = new Label();
            label9 = new Label();
            lbLocation = new Label();
            label7 = new Label();
            lbOwner = new Label();
            label15 = new Label();
            tcMain.SuspendLayout();
            tpMerchant.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgMerchant).BeginInit();
            tpMercenaries.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgMercenaries).BeginInit();
            tpMissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgMissions).BeginInit();
            tpShips.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgShips).BeginInit();
            tpUpgrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgInventory).BeginInit();
            tpDetails.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // tcMain
            // 
            tcMain.Controls.Add(tpMerchant);
            tcMain.Controls.Add(tpMercenaries);
            tcMain.Controls.Add(tpMissions);
            tcMain.Controls.Add(tpShips);
            tcMain.Controls.Add(tpUpgrade);
            tcMain.Controls.Add(tpDetails);
            tcMain.Dock = DockStyle.Fill;
            tcMain.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            tcMain.Location = new Point(0, 0);
            tcMain.Margin = new Padding(4, 3, 4, 3);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new Size(644, 548);
            tcMain.TabIndex = 0;
            tcMain.SelectedIndexChanged += tcMain_SelectedIndexChanged;
            // 
            // tpMerchant
            // 
            tpMerchant.Controls.Add(cbAffordable);
            tpMerchant.Controls.Add(btRandomiseMerchant);
            tpMerchant.Controls.Add(lbTeamCashMerch);
            tpMerchant.Controls.Add(label3);
            tpMerchant.Controls.Add(label1);
            tpMerchant.Controls.Add(tbFilter);
            tpMerchant.Controls.Add(btBuyMerchant);
            tpMerchant.Controls.Add(dgMerchant);
            tpMerchant.Controls.Add(cbItemType);
            tpMerchant.Location = new Point(4, 29);
            tpMerchant.Margin = new Padding(4, 3, 4, 3);
            tpMerchant.Name = "tpMerchant";
            tpMerchant.Size = new Size(636, 515);
            tpMerchant.TabIndex = 0;
            tpMerchant.Text = "Merchants";
            tpMerchant.UseVisualStyleBackColor = true;
            // 
            // cbAffordable
            // 
            cbAffordable.AutoSize = true;
            cbAffordable.CheckAlign = ContentAlignment.MiddleRight;
            cbAffordable.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            cbAffordable.Location = new Point(207, 17);
            cbAffordable.Name = "cbAffordable";
            cbAffordable.Size = new Size(124, 20);
            cbAffordable.TabIndex = 8;
            cbAffordable.Text = "Show Affordable";
            cbAffordable.UseVisualStyleBackColor = true;
            cbAffordable.CheckedChanged += cbAffordable_CheckedChanged;
            // 
            // btRandomiseMerchant
            // 
            btRandomiseMerchant.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btRandomiseMerchant.Location = new Point(518, 480);
            btRandomiseMerchant.Margin = new Padding(4, 3, 4, 3);
            btRandomiseMerchant.Name = "btRandomiseMerchant";
            btRandomiseMerchant.Size = new Size(105, 27);
            btRandomiseMerchant.TabIndex = 7;
            btRandomiseMerchant.Text = "Randomise";
            btRandomiseMerchant.UseVisualStyleBackColor = true;
            btRandomiseMerchant.Click += btRandomiseMerchant_Click;
            // 
            // lbTeamCashMerch
            // 
            lbTeamCashMerch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbTeamCashMerch.BorderStyle = BorderStyle.FixedSingle;
            lbTeamCashMerch.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbTeamCashMerch.ImageAlign = ContentAlignment.MiddleRight;
            lbTeamCashMerch.Location = new Point(61, 479);
            lbTeamCashMerch.Margin = new Padding(5);
            lbTeamCashMerch.Name = "lbTeamCashMerch";
            lbTeamCashMerch.Padding = new Padding(2);
            lbTeamCashMerch.Size = new Size(100, 27);
            lbTeamCashMerch.TabIndex = 6;
            lbTeamCashMerch.Text = "888888.88cr";
            lbTeamCashMerch.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.ImageAlign = ContentAlignment.MiddleRight;
            label3.Location = new Point(13, 483);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(38, 16);
            label3.TabIndex = 5;
            label3.Text = "Cash";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ImageAlign = ContentAlignment.MiddleRight;
            label1.Location = new Point(356, 16);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(36, 16);
            label1.TabIndex = 4;
            label1.Text = "Filter";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tbFilter
            // 
            tbFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbFilter.Location = new Point(400, 13);
            tbFilter.Margin = new Padding(4, 3, 4, 3);
            tbFilter.Name = "tbFilter";
            tbFilter.Size = new Size(218, 26);
            tbFilter.TabIndex = 3;
            tbFilter.TextChanged += tbFilter_TextChanged;
            // 
            // btBuyMerchant
            // 
            btBuyMerchant.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btBuyMerchant.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btBuyMerchant.Location = new Point(263, 479);
            btBuyMerchant.Margin = new Padding(4, 3, 4, 3);
            btBuyMerchant.Name = "btBuyMerchant";
            btBuyMerchant.Size = new Size(102, 28);
            btBuyMerchant.TabIndex = 2;
            btBuyMerchant.Text = "Buy";
            btBuyMerchant.UseVisualStyleBackColor = true;
            btBuyMerchant.Click += btBuyFromMerchant_Click;
            // 
            // dgMerchant
            // 
            dgMerchant.AllowUserToAddRows = false;
            dgMerchant.AllowUserToDeleteRows = false;
            dgMerchant.AllowUserToResizeRows = false;
            dgMerchant.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgMerchant.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgMerchant.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgMerchant.Columns.AddRange(new DataGridViewColumn[] { colItem, colCost, Mass, colAvail });
            dgMerchant.Location = new Point(10, 53);
            dgMerchant.Margin = new Padding(4, 3, 4, 3);
            dgMerchant.MultiSelect = false;
            dgMerchant.Name = "dgMerchant";
            dgMerchant.ReadOnly = true;
            dgMerchant.RowHeadersVisible = false;
            dgMerchant.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgMerchant.ShowEditingIcon = false;
            dgMerchant.Size = new Size(612, 415);
            dgMerchant.TabIndex = 1;
            dgMerchant.SortCompare += dgMerchant_SortCompare;
            dgMerchant.DoubleClick += dgMerchant_DoubleClick;
            // 
            // colItem
            // 
            colItem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            colItem.DefaultCellStyle = dataGridViewCellStyle1;
            colItem.HeaderText = "Item";
            colItem.Name = "colItem";
            colItem.ReadOnly = true;
            // 
            // colCost
            // 
            colCost.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colCost.HeaderText = "Cost";
            colCost.Name = "colCost";
            colCost.ReadOnly = true;
            colCost.Width = 67;
            // 
            // Mass
            // 
            Mass.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            Mass.DefaultCellStyle = dataGridViewCellStyle2;
            Mass.HeaderText = "Mass";
            Mass.Name = "Mass";
            Mass.ReadOnly = true;
            Mass.Width = 72;
            // 
            // colAvail
            // 
            colAvail.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colAvail.HeaderText = "Avail";
            colAvail.Name = "colAvail";
            colAvail.ReadOnly = true;
            colAvail.Width = 67;
            // 
            // cbItemType
            // 
            cbItemType.FormattingEnabled = true;
            cbItemType.Location = new Point(13, 13);
            cbItemType.Margin = new Padding(4, 3, 4, 3);
            cbItemType.Name = "cbItemType";
            cbItemType.Size = new Size(178, 28);
            cbItemType.TabIndex = 0;
            cbItemType.SelectedIndexChanged += cbItemType_SelectedIndexChanged;
            // 
            // tpMercenaries
            // 
            tpMercenaries.Controls.Add(lbTeamCashMercs);
            tpMercenaries.Controls.Add(label12);
            tpMercenaries.Controls.Add(btRandomiseMercs);
            tpMercenaries.Controls.Add(btHire);
            tpMercenaries.Controls.Add(dgMercenaries);
            tpMercenaries.Location = new Point(4, 29);
            tpMercenaries.Margin = new Padding(4, 3, 4, 3);
            tpMercenaries.Name = "tpMercenaries";
            tpMercenaries.Size = new Size(636, 515);
            tpMercenaries.TabIndex = 1;
            tpMercenaries.Text = "Mercenaries";
            tpMercenaries.UseVisualStyleBackColor = true;
            // 
            // lbTeamCashMercs
            // 
            lbTeamCashMercs.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbTeamCashMercs.BorderStyle = BorderStyle.FixedSingle;
            lbTeamCashMercs.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbTeamCashMercs.ImageAlign = ContentAlignment.MiddleRight;
            lbTeamCashMercs.Location = new Point(61, 479);
            lbTeamCashMercs.Margin = new Padding(5);
            lbTeamCashMercs.Name = "lbTeamCashMercs";
            lbTeamCashMercs.Padding = new Padding(2);
            lbTeamCashMercs.Size = new Size(100, 27);
            lbTeamCashMercs.TabIndex = 8;
            lbTeamCashMercs.Text = "888888.88cr";
            lbTeamCashMercs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label12.ImageAlign = ContentAlignment.MiddleRight;
            label12.Location = new Point(13, 483);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(38, 16);
            label12.TabIndex = 7;
            label12.Text = "Cash";
            label12.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btRandomiseMercs
            // 
            btRandomiseMercs.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btRandomiseMercs.Location = new Point(518, 479);
            btRandomiseMercs.Margin = new Padding(4, 3, 4, 3);
            btRandomiseMercs.Name = "btRandomiseMercs";
            btRandomiseMercs.Size = new Size(105, 27);
            btRandomiseMercs.TabIndex = 6;
            btRandomiseMercs.Text = "Randomise";
            btRandomiseMercs.UseVisualStyleBackColor = true;
            btRandomiseMercs.Click += btRandomiseMercs_Click;
            // 
            // btHire
            // 
            btHire.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btHire.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btHire.Location = new Point(223, 479);
            btHire.Margin = new Padding(4, 3, 4, 3);
            btHire.Name = "btHire";
            btHire.Size = new Size(189, 28);
            btHire.TabIndex = 3;
            btHire.Text = "Hire";
            btHire.UseVisualStyleBackColor = true;
            btHire.Click += btHireMercenary_Click;
            // 
            // dgMercenaries
            // 
            dgMercenaries.AllowUserToAddRows = false;
            dgMercenaries.AllowUserToDeleteRows = false;
            dgMercenaries.AllowUserToResizeRows = false;
            dgMercenaries.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgMercenaries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgMercenaries.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgMercenaries.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, colFee });
            dgMercenaries.Location = new Point(10, 14);
            dgMercenaries.Margin = new Padding(4, 3, 4, 3);
            dgMercenaries.MultiSelect = false;
            dgMercenaries.Name = "dgMercenaries";
            dgMercenaries.ReadOnly = true;
            dgMercenaries.RowHeadersVisible = false;
            dgMercenaries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgMercenaries.ShowEditingIcon = false;
            dgMercenaries.Size = new Size(612, 455);
            dgMercenaries.TabIndex = 2;
            dgMercenaries.SortCompare += dgMercenaries_SortCompare;
            dgMercenaries.DoubleClick += dgMercenaries_DoubleClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewTextBoxColumn1.HeaderText = "Name";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn2.HeaderText = "Level";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.Width = 71;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn3.HeaderText = "Race";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            dataGridViewTextBoxColumn3.Width = 72;
            // 
            // colFee
            // 
            colFee.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colFee.HeaderText = "Fee";
            colFee.Name = "colFee";
            colFee.ReadOnly = true;
            colFee.Width = 62;
            // 
            // tpMissions
            // 
            tpMissions.Controls.Add(btRandomiseMissions);
            tpMissions.Controls.Add(btAccept);
            tpMissions.Controls.Add(dgMissions);
            tpMissions.Location = new Point(4, 29);
            tpMissions.Margin = new Padding(4, 3, 4, 3);
            tpMissions.Name = "tpMissions";
            tpMissions.Size = new Size(636, 515);
            tpMissions.TabIndex = 2;
            tpMissions.Text = "Mission Board";
            tpMissions.UseVisualStyleBackColor = true;
            // 
            // btRandomiseMissions
            // 
            btRandomiseMissions.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btRandomiseMissions.Location = new Point(516, 475);
            btRandomiseMissions.Margin = new Padding(4, 3, 4, 3);
            btRandomiseMissions.Name = "btRandomiseMissions";
            btRandomiseMissions.Size = new Size(105, 27);
            btRandomiseMissions.TabIndex = 5;
            btRandomiseMissions.Text = "Randomise";
            btRandomiseMissions.UseVisualStyleBackColor = true;
            btRandomiseMissions.Click += btRandomiseMissions_Click;
            // 
            // btAccept
            // 
            btAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btAccept.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btAccept.Location = new Point(226, 475);
            btAccept.Margin = new Padding(4, 3, 4, 3);
            btAccept.Name = "btAccept";
            btAccept.Size = new Size(182, 28);
            btAccept.TabIndex = 4;
            btAccept.Text = "Accept";
            btAccept.UseVisualStyleBackColor = true;
            btAccept.Click += btRunMission_Click;
            // 
            // dgMissions
            // 
            dgMissions.AllowUserToAddRows = false;
            dgMissions.AllowUserToDeleteRows = false;
            dgMissions.AllowUserToResizeRows = false;
            dgMissions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgMissions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgMissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgMissions.Columns.AddRange(new DataGridViewColumn[] { colMission, colGoal, colOpp, colDiff, colLevels, colReward });
            dgMissions.Location = new Point(14, 17);
            dgMissions.Margin = new Padding(4, 3, 4, 3);
            dgMissions.MultiSelect = false;
            dgMissions.Name = "dgMissions";
            dgMissions.ReadOnly = true;
            dgMissions.RowHeadersVisible = false;
            dgMissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgMissions.ShowEditingIcon = false;
            dgMissions.Size = new Size(606, 448);
            dgMissions.TabIndex = 2;
            dgMissions.SortCompare += dgMissions_SortCompare;
            dgMissions.DoubleClick += dgMissions_DoubleClick;
            // 
            // colMission
            // 
            colMission.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            colMission.DefaultCellStyle = dataGridViewCellStyle4;
            colMission.HeaderText = "Mission";
            colMission.Name = "colMission";
            colMission.ReadOnly = true;
            // 
            // colGoal
            // 
            colGoal.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            colGoal.DefaultCellStyle = dataGridViewCellStyle5;
            colGoal.HeaderText = "Goal";
            colGoal.Name = "colGoal";
            colGoal.ReadOnly = true;
            colGoal.Width = 68;
            // 
            // colOpp
            // 
            colOpp.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            colOpp.DefaultCellStyle = dataGridViewCellStyle6;
            colOpp.HeaderText = "Enemy";
            colOpp.Name = "colOpp";
            colOpp.ReadOnly = true;
            colOpp.Width = 83;
            // 
            // colDiff
            // 
            colDiff.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colDiff.HeaderText = "Diff";
            colDiff.Name = "colDiff";
            colDiff.ReadOnly = true;
            colDiff.Width = 59;
            // 
            // colLevels
            // 
            colLevels.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colLevels.HeaderText = "Size";
            colLevels.Name = "colLevels";
            colLevels.ReadOnly = true;
            colLevels.Width = 65;
            // 
            // colReward
            // 
            colReward.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colReward.HeaderText = "Reward";
            colReward.Name = "colReward";
            colReward.ReadOnly = true;
            colReward.Width = 89;
            // 
            // tpShips
            // 
            tpShips.Controls.Add(lbTeamCashShips);
            tpShips.Controls.Add(label13);
            tpShips.Controls.Add(btUpgrade);
            tpShips.Controls.Add(dgShips);
            tpShips.Location = new Point(4, 29);
            tpShips.Margin = new Padding(4, 3, 4, 3);
            tpShips.Name = "tpShips";
            tpShips.Size = new Size(636, 515);
            tpShips.TabIndex = 3;
            tpShips.Text = "Ship Vendor";
            tpShips.UseVisualStyleBackColor = true;
            // 
            // lbTeamCashShips
            // 
            lbTeamCashShips.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbTeamCashShips.BorderStyle = BorderStyle.FixedSingle;
            lbTeamCashShips.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbTeamCashShips.ImageAlign = ContentAlignment.MiddleRight;
            lbTeamCashShips.Location = new Point(60, 478);
            lbTeamCashShips.Margin = new Padding(5);
            lbTeamCashShips.Name = "lbTeamCashShips";
            lbTeamCashShips.Padding = new Padding(2);
            lbTeamCashShips.Size = new Size(100, 27);
            lbTeamCashShips.TabIndex = 10;
            lbTeamCashShips.Text = "888888.88cr";
            lbTeamCashShips.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label13.ImageAlign = ContentAlignment.MiddleRight;
            label13.Location = new Point(12, 482);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(38, 16);
            label13.TabIndex = 9;
            label13.Text = "Cash";
            label13.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btUpgrade
            // 
            btUpgrade.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btUpgrade.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btUpgrade.Location = new Point(223, 477);
            btUpgrade.Margin = new Padding(4, 3, 4, 3);
            btUpgrade.Name = "btUpgrade";
            btUpgrade.Size = new Size(189, 28);
            btUpgrade.TabIndex = 6;
            btUpgrade.Text = "Upgrade";
            btUpgrade.UseVisualStyleBackColor = true;
            btUpgrade.Click += btUpgradeShip_Click;
            // 
            // dgShips
            // 
            dgShips.AllowUserToAddRows = false;
            dgShips.AllowUserToDeleteRows = false;
            dgShips.AllowUserToResizeRows = false;
            dgShips.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgShips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgShips.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgShips.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7, colPrice });
            dgShips.Location = new Point(10, 8);
            dgShips.Margin = new Padding(4, 3, 4, 3);
            dgShips.MultiSelect = false;
            dgShips.Name = "dgShips";
            dgShips.ReadOnly = true;
            dgShips.RowHeadersVisible = false;
            dgShips.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgShips.ShowEditingIcon = false;
            dgShips.Size = new Size(612, 462);
            dgShips.TabIndex = 5;
            dgShips.SortCompare += dgShips_SortCompare;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle7.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewTextBoxColumn6.HeaderText = "Ship";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn7.HeaderText = "S/M/L/W";
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            dataGridViewTextBoxColumn7.Width = 94;
            // 
            // colPrice
            // 
            colPrice.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colPrice.HeaderText = "Cost";
            colPrice.Name = "colPrice";
            colPrice.ReadOnly = true;
            colPrice.Width = 67;
            // 
            // tpUpgrade
            // 
            tpUpgrade.Controls.Add(btModify);
            tpUpgrade.Controls.Add(cbEquipped);
            tpUpgrade.Controls.Add(lbTeamCashFoundry);
            tpUpgrade.Controls.Add(label14);
            tpUpgrade.Controls.Add(btSellAll);
            tpUpgrade.Controls.Add(btDismantle);
            tpUpgrade.Controls.Add(btSell);
            tpUpgrade.Controls.Add(btImprove);
            tpUpgrade.Controls.Add(label2);
            tpUpgrade.Controls.Add(tbUpgradeFilter);
            tpUpgrade.Controls.Add(cbUpgradeItemType);
            tpUpgrade.Controls.Add(dgInventory);
            tpUpgrade.Location = new Point(4, 29);
            tpUpgrade.Margin = new Padding(4, 3, 4, 3);
            tpUpgrade.Name = "tpUpgrade";
            tpUpgrade.Size = new Size(636, 515);
            tpUpgrade.TabIndex = 4;
            tpUpgrade.Text = "Foundry";
            tpUpgrade.UseVisualStyleBackColor = true;
            // 
            // btModify
            // 
            btModify.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btModify.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btModify.Location = new Point(181, 475);
            btModify.Margin = new Padding(4, 3, 4, 3);
            btModify.Name = "btModify";
            btModify.Size = new Size(80, 28);
            btModify.TabIndex = 16;
            btModify.Text = "Modify";
            btModify.UseVisualStyleBackColor = true;
            btModify.Click += btModifyWeapon_Click;
            // 
            // cbEquipped
            // 
            cbEquipped.AutoSize = true;
            cbEquipped.CheckAlign = ContentAlignment.MiddleRight;
            cbEquipped.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            cbEquipped.Location = new Point(222, 15);
            cbEquipped.Name = "cbEquipped";
            cbEquipped.Size = new Size(121, 20);
            cbEquipped.TabIndex = 15;
            cbEquipped.Text = "Show Equipped";
            cbEquipped.UseVisualStyleBackColor = true;
            cbEquipped.CheckedChanged += cbEquipped_CheckedChanged;
            // 
            // lbTeamCashFoundry
            // 
            lbTeamCashFoundry.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbTeamCashFoundry.BorderStyle = BorderStyle.FixedSingle;
            lbTeamCashFoundry.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbTeamCashFoundry.ImageAlign = ContentAlignment.MiddleRight;
            lbTeamCashFoundry.Location = new Point(63, 476);
            lbTeamCashFoundry.Margin = new Padding(5);
            lbTeamCashFoundry.Name = "lbTeamCashFoundry";
            lbTeamCashFoundry.Padding = new Padding(2);
            lbTeamCashFoundry.Size = new Size(100, 27);
            lbTeamCashFoundry.TabIndex = 14;
            lbTeamCashFoundry.Text = "888888.88cr";
            lbTeamCashFoundry.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label14.ImageAlign = ContentAlignment.MiddleRight;
            label14.Location = new Point(15, 480);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(38, 16);
            label14.TabIndex = 13;
            label14.Text = "Cash";
            label14.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btSellAll
            // 
            btSellAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btSellAll.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btSellAll.Location = new Point(542, 475);
            btSellAll.Margin = new Padding(4, 3, 4, 3);
            btSellAll.Name = "btSellAll";
            btSellAll.Size = new Size(80, 28);
            btSellAll.TabIndex = 12;
            btSellAll.Text = "Sell All";
            btSellAll.UseVisualStyleBackColor = true;
            btSellAll.Click += btSellAll_Click;
            // 
            // btDismantle
            // 
            btDismantle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btDismantle.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDismantle.Location = new Point(362, 475);
            btDismantle.Margin = new Padding(4, 3, 4, 3);
            btDismantle.Name = "btDismantle";
            btDismantle.Size = new Size(80, 28);
            btDismantle.TabIndex = 11;
            btDismantle.Text = "Dismantle";
            btDismantle.UseVisualStyleBackColor = true;
            btDismantle.Click += btDismantleEquippable_Click;
            // 
            // btSell
            // 
            btSell.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btSell.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btSell.Location = new Point(452, 475);
            btSell.Margin = new Padding(4, 3, 4, 3);
            btSell.Name = "btSell";
            btSell.Size = new Size(80, 28);
            btSell.TabIndex = 10;
            btSell.Text = "Sell One";
            btSell.UseVisualStyleBackColor = true;
            btSell.Click += btSellItem_Click;
            // 
            // btImprove
            // 
            btImprove.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btImprove.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btImprove.Location = new Point(272, 475);
            btImprove.Margin = new Padding(4, 3, 4, 3);
            btImprove.Name = "btImprove";
            btImprove.Size = new Size(80, 28);
            btImprove.TabIndex = 9;
            btImprove.Text = "Improve";
            btImprove.UseVisualStyleBackColor = true;
            btImprove.Click += btImproveWeaponOrArmour_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ImageAlign = ContentAlignment.MiddleRight;
            label2.Location = new Point(364, 16);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(36, 16);
            label2.TabIndex = 7;
            label2.Text = "Filter";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tbUpgradeFilter
            // 
            tbUpgradeFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbUpgradeFilter.Location = new Point(404, 13);
            tbUpgradeFilter.Margin = new Padding(4, 3, 4, 3);
            tbUpgradeFilter.Name = "tbUpgradeFilter";
            tbUpgradeFilter.Size = new Size(214, 26);
            tbUpgradeFilter.TabIndex = 6;
            tbUpgradeFilter.TextChanged += tbUpgradeFilter_TextChanged;
            // 
            // cbUpgradeItemType
            // 
            cbUpgradeItemType.FormattingEnabled = true;
            cbUpgradeItemType.Location = new Point(13, 13);
            cbUpgradeItemType.Margin = new Padding(4, 3, 4, 3);
            cbUpgradeItemType.Name = "cbUpgradeItemType";
            cbUpgradeItemType.Size = new Size(178, 28);
            cbUpgradeItemType.TabIndex = 5;
            cbUpgradeItemType.SelectedIndexChanged += cbUpgradeItemType_SelectedIndexChanged;
            // 
            // dgInventory
            // 
            dgInventory.AllowUserToAddRows = false;
            dgInventory.AllowUserToDeleteRows = false;
            dgInventory.AllowUserToResizeRows = false;
            dgInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgInventory.Columns.AddRange(new DataGridViewColumn[] { dgcItem, dgcLocation, dgcValue, dgcAvail });
            dgInventory.Location = new Point(10, 53);
            dgInventory.Margin = new Padding(4, 3, 4, 3);
            dgInventory.Name = "dgInventory";
            dgInventory.ReadOnly = true;
            dgInventory.RowHeadersVisible = false;
            dgInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgInventory.ShowEditingIcon = false;
            dgInventory.Size = new Size(612, 415);
            dgInventory.TabIndex = 2;
            dgInventory.SelectionChanged += dgInventory_SelectionChanged;
            dgInventory.SortCompare += dgInventory_SortCompare;
            dgInventory.DoubleClick += dgInventory_DoubleClick;
            // 
            // dgcItem
            // 
            dgcItem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle8.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dgcItem.DefaultCellStyle = dataGridViewCellStyle8;
            dgcItem.HeaderText = "Item";
            dgcItem.Name = "dgcItem";
            dgcItem.ReadOnly = true;
            // 
            // dgcLocation
            // 
            dgcLocation.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgcLocation.HeaderText = "Location";
            dgcLocation.Name = "dgcLocation";
            dgcLocation.ReadOnly = true;
            dgcLocation.Width = 95;
            // 
            // dgcValue
            // 
            dgcValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgcValue.HeaderText = "Value";
            dgcValue.Name = "dgcValue";
            dgcValue.ReadOnly = true;
            dgcValue.Width = 75;
            // 
            // dgcAvail
            // 
            dgcAvail.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgcAvail.HeaderText = "Avail";
            dgcAvail.Name = "dgcAvail";
            dgcAvail.ReadOnly = true;
            dgcAvail.Width = 67;
            // 
            // tpDetails
            // 
            tpDetails.Controls.Add(lbNextGrowth);
            tpDetails.Controls.Add(label8);
            tpDetails.Controls.Add(lbLastGrowth);
            tpDetails.Controls.Add(label6);
            tpDetails.Controls.Add(lbMetropolis);
            tpDetails.Controls.Add(lbResidential);
            tpDetails.Controls.Add(lbMilitary);
            tpDetails.Controls.Add(lbTrade);
            tpDetails.Controls.Add(lbResearch);
            tpDetails.Controls.Add(lbColonySize);
            tpDetails.Controls.Add(label4);
            tpDetails.Controls.Add(lbColonyName);
            tpDetails.Controls.Add(groupBox1);
            tpDetails.Controls.Add(groupBox4);
            tpDetails.Location = new Point(4, 29);
            tpDetails.Margin = new Padding(4, 3, 4, 3);
            tpDetails.Name = "tpDetails";
            tpDetails.Size = new Size(636, 515);
            tpDetails.TabIndex = 5;
            tpDetails.Text = "Details";
            tpDetails.UseVisualStyleBackColor = true;
            // 
            // lbNextGrowth
            // 
            lbNextGrowth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbNextGrowth.BorderStyle = BorderStyle.FixedSingle;
            lbNextGrowth.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbNextGrowth.ImageAlign = ContentAlignment.MiddleRight;
            lbNextGrowth.Location = new Point(120, 426);
            lbNextGrowth.Margin = new Padding(5);
            lbNextGrowth.Name = "lbNextGrowth";
            lbNextGrowth.Padding = new Padding(2);
            lbNextGrowth.Size = new Size(193, 27);
            lbNextGrowth.TabIndex = 12;
            lbNextGrowth.Text = "88th December 2188";
            lbNextGrowth.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label8.ImageAlign = ContentAlignment.MiddleRight;
            label8.Location = new Point(22, 432);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(78, 16);
            label8.TabIndex = 11;
            label8.Text = "Next Growth";
            label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbLastGrowth
            // 
            lbLastGrowth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbLastGrowth.BorderStyle = BorderStyle.FixedSingle;
            lbLastGrowth.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbLastGrowth.ImageAlign = ContentAlignment.MiddleRight;
            lbLastGrowth.Location = new Point(120, 377);
            lbLastGrowth.Margin = new Padding(5);
            lbLastGrowth.Name = "lbLastGrowth";
            lbLastGrowth.Padding = new Padding(2);
            lbLastGrowth.Size = new Size(193, 27);
            lbLastGrowth.TabIndex = 10;
            lbLastGrowth.Text = "88th December 2188";
            lbLastGrowth.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label6.ImageAlign = ContentAlignment.MiddleRight;
            label6.Location = new Point(22, 383);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(76, 16);
            label6.TabIndex = 9;
            label6.Text = "Last Growth";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbMetropolis
            // 
            lbMetropolis.AutoSize = true;
            lbMetropolis.Location = new Point(72, 297);
            lbMetropolis.Margin = new Padding(4, 0, 4, 0);
            lbMetropolis.Name = "lbMetropolis";
            lbMetropolis.Size = new Size(82, 20);
            lbMetropolis.TabIndex = 7;
            lbMetropolis.Text = "Metropolis";
            // 
            // lbResidential
            // 
            lbResidential.AutoSize = true;
            lbResidential.Location = new Point(46, 263);
            lbResidential.Margin = new Padding(4, 0, 4, 0);
            lbResidential.Name = "lbResidential";
            lbResidential.Size = new Size(129, 20);
            lbResidential.TabIndex = 6;
            lbResidential.Text = "Residential Zone";
            // 
            // lbMilitary
            // 
            lbMilitary.AutoSize = true;
            lbMilitary.Location = new Point(63, 230);
            lbMilitary.Margin = new Padding(4, 0, 4, 0);
            lbMilitary.Name = "lbMilitary";
            lbMilitary.Size = new Size(98, 20);
            lbMilitary.TabIndex = 5;
            lbMilitary.Text = "Military Base";
            // 
            // lbTrade
            // 
            lbTrade.AutoSize = true;
            lbTrade.Location = new Point(61, 196);
            lbTrade.Margin = new Padding(4, 0, 4, 0);
            lbTrade.Name = "lbTrade";
            lbTrade.Size = new Size(102, 20);
            lbTrade.TabIndex = 4;
            lbTrade.Text = "Trade Centre";
            // 
            // lbResearch
            // 
            lbResearch.AutoSize = true;
            lbResearch.Location = new Point(51, 163);
            lbResearch.Margin = new Padding(4, 0, 4, 0);
            lbResearch.Name = "lbResearch";
            lbResearch.Size = new Size(119, 20);
            lbResearch.TabIndex = 3;
            lbResearch.Text = "Research Base";
            // 
            // lbColonySize
            // 
            lbColonySize.BorderStyle = BorderStyle.FixedSingle;
            lbColonySize.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbColonySize.Location = new Point(161, 106);
            lbColonySize.Margin = new Padding(0);
            lbColonySize.Name = "lbColonySize";
            lbColonySize.RightToLeft = RightToLeft.Yes;
            lbColonySize.Size = new Size(37, 32);
            lbColonySize.TabIndex = 2;
            lbColonySize.Text = "8";
            lbColonySize.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(47, 112);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.RightToLeft = RightToLeft.Yes;
            label4.Size = new Size(92, 20);
            label4.TabIndex = 1;
            label4.Text = "Colony Size";
            // 
            // lbColonyName
            // 
            lbColonyName.BorderStyle = BorderStyle.FixedSingle;
            lbColonyName.Location = new Point(128, 12);
            lbColonyName.Margin = new Padding(4, 0, 4, 0);
            lbColonyName.Name = "lbColonyName";
            lbColonyName.Size = new Size(379, 39);
            lbColonyName.TabIndex = 0;
            lbColonyName.Text = "Colony Name";
            lbColonyName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox1.Location = new Point(23, 73);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(201, 276);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Colony Details";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(lbOwner);
            groupBox4.Controls.Add(label15);
            groupBox4.Controls.Add(lbSystemName);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(lbStarType);
            groupBox4.Controls.Add(label11);
            groupBox4.Controls.Add(lbPlanetType);
            groupBox4.Controls.Add(label9);
            groupBox4.Controls.Add(lbLocation);
            groupBox4.Controls.Add(label7);
            groupBox4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            groupBox4.Location = new Point(257, 73);
            groupBox4.Margin = new Padding(4, 3, 4, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(4, 3, 4, 3);
            groupBox4.Size = new Size(352, 276);
            groupBox4.TabIndex = 9;
            groupBox4.TabStop = false;
            groupBox4.Text = "Location Details";
            // 
            // lbSystemName
            // 
            lbSystemName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbSystemName.BorderStyle = BorderStyle.FixedSingle;
            lbSystemName.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbSystemName.ImageAlign = ContentAlignment.MiddleRight;
            lbSystemName.Location = new Point(152, 72);
            lbSystemName.Margin = new Padding(5);
            lbSystemName.Name = "lbSystemName";
            lbSystemName.Padding = new Padding(2);
            lbSystemName.Size = new Size(186, 27);
            lbSystemName.TabIndex = 20;
            lbSystemName.Text = "Current Location";
            lbSystemName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label10.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.ImageAlign = ContentAlignment.MiddleRight;
            label10.Location = new Point(14, 74);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(120, 24);
            label10.TabIndex = 19;
            label10.Text = "System Name";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbStarType
            // 
            lbStarType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbStarType.BorderStyle = BorderStyle.FixedSingle;
            lbStarType.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbStarType.ImageAlign = ContentAlignment.MiddleRight;
            lbStarType.Location = new Point(152, 110);
            lbStarType.Margin = new Padding(5);
            lbStarType.Name = "lbStarType";
            lbStarType.Padding = new Padding(2);
            lbStarType.Size = new Size(186, 27);
            lbStarType.TabIndex = 18;
            lbStarType.Text = "Current Location";
            lbStarType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label11.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label11.ImageAlign = ContentAlignment.MiddleRight;
            label11.Location = new Point(15, 112);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(120, 24);
            label11.TabIndex = 17;
            label11.Text = "Star Type";
            label11.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbPlanetType
            // 
            lbPlanetType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbPlanetType.BorderStyle = BorderStyle.FixedSingle;
            lbPlanetType.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbPlanetType.ImageAlign = ContentAlignment.MiddleRight;
            lbPlanetType.Location = new Point(152, 147);
            lbPlanetType.Margin = new Padding(5);
            lbPlanetType.Name = "lbPlanetType";
            lbPlanetType.Padding = new Padding(2);
            lbPlanetType.Size = new Size(186, 27);
            lbPlanetType.TabIndex = 16;
            lbPlanetType.Text = "Current Location";
            lbPlanetType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label9.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label9.ImageAlign = ContentAlignment.MiddleRight;
            label9.Location = new Point(14, 151);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(121, 18);
            label9.TabIndex = 15;
            label9.Text = "Planet Type";
            label9.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbLocation
            // 
            lbLocation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbLocation.BorderStyle = BorderStyle.FixedSingle;
            lbLocation.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbLocation.ImageAlign = ContentAlignment.MiddleRight;
            lbLocation.Location = new Point(152, 35);
            lbLocation.Margin = new Padding(5);
            lbLocation.Name = "lbLocation";
            lbLocation.Padding = new Padding(2);
            lbLocation.Size = new Size(186, 27);
            lbLocation.TabIndex = 14;
            lbLocation.Text = "Current Location";
            lbLocation.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label7.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label7.ImageAlign = ContentAlignment.MiddleRight;
            label7.Location = new Point(14, 39);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(120, 20);
            label7.TabIndex = 13;
            label7.Text = "Location";
            label7.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbOwner
            // 
            lbOwner.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbOwner.BorderStyle = BorderStyle.FixedSingle;
            lbOwner.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbOwner.ImageAlign = ContentAlignment.MiddleRight;
            lbOwner.Location = new Point(152, 183);
            lbOwner.Margin = new Padding(5);
            lbOwner.Name = "lbOwner";
            lbOwner.Padding = new Padding(2);
            lbOwner.Size = new Size(186, 27);
            lbOwner.TabIndex = 22;
            lbOwner.Text = "Placeholder Text";
            lbOwner.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label15.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label15.ImageAlign = ContentAlignment.MiddleRight;
            label15.Location = new Point(14, 187);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(121, 18);
            label15.TabIndex = 21;
            label15.Text = "Owning Race";
            label15.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ColonyView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(644, 548);
            Controls.Add(tcMain);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ColonyView";
            Text = "Colony Services";
            tcMain.ResumeLayout(false);
            tpMerchant.ResumeLayout(false);
            tpMerchant.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgMerchant).EndInit();
            tpMercenaries.ResumeLayout(false);
            tpMercenaries.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgMercenaries).EndInit();
            tpMissions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgMissions).EndInit();
            tpShips.ResumeLayout(false);
            tpShips.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgShips).EndInit();
            tpUpgrade.ResumeLayout(false);
            tpUpgrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgInventory).EndInit();
            tpDetails.ResumeLayout(false);
            tpDetails.PerformLayout();
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tcMain;
        private TabPage tpMerchant;
        private TabPage tpMercenaries;
        private TabPage tpMissions;
        private TabPage tpShips;
        private Button btBuyMerchant;
        private DataGridView dgMerchant;
        private ComboBox cbItemType;
        private Button btHire;
        private DataGridView dgMercenaries;
        private Button btAccept;
        private DataGridView dgMissions;
        private Button btUpgrade;
        private DataGridView dgShips;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn colFee;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn colPrice;
        private DataGridViewTextBoxColumn colItem;
        private DataGridViewTextBoxColumn colCost;
        private DataGridViewTextBoxColumn Mass;
        private DataGridViewTextBoxColumn colAvail;
        private Label label1;
        private TextBox tbFilter;
        private TabPage tpUpgrade;
        private Label label2;
        private TextBox tbUpgradeFilter;
        private ComboBox cbUpgradeItemType;
        private DataGridView dgInventory;
        private Button btImprove;
        private Button btSell;
        private Button btDismantle;
        private DataGridViewTextBoxColumn dgcItem;
        private DataGridViewTextBoxColumn dgcLocation;
        private DataGridViewTextBoxColumn dgcValue;
        private DataGridViewTextBoxColumn dgcAvail;
        private Label lbTeamCashMerch;
        private Label label3;
        private Button btRandomiseMissions;
        private Button btRandomiseMercs;
        private Button btRandomiseMerchant;
        private DataGridViewTextBoxColumn colMission;
        private DataGridViewTextBoxColumn colGoal;
        private DataGridViewTextBoxColumn colOpp;
        private DataGridViewTextBoxColumn colDiff;
        private DataGridViewTextBoxColumn colLevels;
        private DataGridViewTextBoxColumn colReward;
        private Button btSellAll;
        private TabPage tpDetails;
        private Label lbColonyName;
        private Label lbColonySize;
        private Label label4;
        private Label lbResidential;
        private Label lbMilitary;
        private Label lbTrade;
        private Label lbResearch;
        private Label lbMetropolis;
        private GroupBox groupBox1;
        private Label lbNextGrowth;
        private Label label8;
        private Label lbLastGrowth;
        private Label label6;
        private GroupBox groupBox4;
        private Label lbStarType;
        private Label label11;
        private Label lbPlanetType;
        private Label label9;
        private Label lbLocation;
        private Label label7;
        private Label lbSystemName;
        private Label label10;
        private CheckBox cbAffordable;
        private Label lbTeamCashMercs;
        private Label label12;
        private Label lbTeamCashShips;
        private Label label13;
        private Label lbTeamCashFoundry;
        private Label label14;
        private CheckBox cbEquipped;
        private Button btModify;
        private Label lbOwner;
        private Label label15;
    }
}