namespace SpaceMercs.Dialogs {
    partial class FabricateItems {
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
            tcMain = new TabControl();
            tpConstruct = new TabPage();
            cbHideUnbuildable = new CheckBox();
            label1 = new Label();
            tbFilter = new TextBox();
            btConstruct = new Button();
            dgConstruct = new DataGridView();
            colItem = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            Mass = new DataGridViewTextBoxColumn();
            colAvail = new DataGridViewTextBoxColumn();
            cbItemType = new ComboBox();
            tpUpgrade = new TabPage();
            btDismantleAll = new Button();
            lbNoEngineering = new Label();
            btModify = new Button();
            btDismantle = new Button();
            btImprove = new Button();
            label2 = new Label();
            tbUpgradeFilter = new TextBox();
            cbUpgradeItemType = new ComboBox();
            dgInventory = new DataGridView();
            dgcItem = new DataGridViewTextBoxColumn();
            dgcLocation = new DataGridViewTextBoxColumn();
            dgcValue = new DataGridViewTextBoxColumn();
            dgcAvail = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            colFee = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            tcMain.SuspendLayout();
            tpConstruct.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgConstruct).BeginInit();
            tpUpgrade.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgInventory).BeginInit();
            SuspendLayout();
            // 
            // tcMain
            // 
            tcMain.Controls.Add(tpConstruct);
            tcMain.Controls.Add(tpUpgrade);
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
            // tpConstruct
            // 
            tpConstruct.Controls.Add(cbHideUnbuildable);
            tpConstruct.Controls.Add(label1);
            tpConstruct.Controls.Add(tbFilter);
            tpConstruct.Controls.Add(btConstruct);
            tpConstruct.Controls.Add(dgConstruct);
            tpConstruct.Controls.Add(cbItemType);
            tpConstruct.Location = new Point(4, 29);
            tpConstruct.Margin = new Padding(4, 3, 4, 3);
            tpConstruct.Name = "tpConstruct";
            tpConstruct.Size = new Size(636, 515);
            tpConstruct.TabIndex = 0;
            tpConstruct.Text = "Construction";
            tpConstruct.UseVisualStyleBackColor = true;
            // 
            // cbHideUnbuildable
            // 
            cbHideUnbuildable.AutoSize = true;
            cbHideUnbuildable.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            cbHideUnbuildable.Location = new Point(10, 475);
            cbHideUnbuildable.Margin = new Padding(4, 3, 4, 3);
            cbHideUnbuildable.Name = "cbHideUnbuildable";
            cbHideUnbuildable.Size = new Size(131, 20);
            cbHideUnbuildable.TabIndex = 5;
            cbHideUnbuildable.Text = "Hide Unbuildable";
            cbHideUnbuildable.UseVisualStyleBackColor = true;
            cbHideUnbuildable.CheckedChanged += cbHideUnbuildable_CheckedChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ImageAlign = ContentAlignment.MiddleRight;
            label1.Location = new Point(329, 16);
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
            tbFilter.Location = new Point(380, 13);
            tbFilter.Margin = new Padding(4, 3, 4, 3);
            tbFilter.Name = "tbFilter";
            tbFilter.Size = new Size(238, 26);
            tbFilter.TabIndex = 3;
            tbFilter.TextChanged += tbFilter_TextChanged;
            // 
            // btConstruct
            // 
            btConstruct.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btConstruct.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btConstruct.Location = new Point(267, 479);
            btConstruct.Margin = new Padding(4, 3, 4, 3);
            btConstruct.Name = "btConstruct";
            btConstruct.Size = new Size(102, 28);
            btConstruct.TabIndex = 2;
            btConstruct.Text = "Construct";
            btConstruct.UseVisualStyleBackColor = true;
            btConstruct.Click += btConstruct_Click;
            // 
            // dgConstruct
            // 
            dgConstruct.AllowUserToAddRows = false;
            dgConstruct.AllowUserToDeleteRows = false;
            dgConstruct.AllowUserToResizeRows = false;
            dgConstruct.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgConstruct.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgConstruct.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgConstruct.Columns.AddRange(new DataGridViewColumn[] { colItem, colType, Mass, colAvail });
            dgConstruct.Location = new Point(10, 53);
            dgConstruct.Margin = new Padding(4, 3, 4, 3);
            dgConstruct.MultiSelect = false;
            dgConstruct.Name = "dgConstruct";
            dgConstruct.ReadOnly = true;
            dgConstruct.RowHeadersVisible = false;
            dgConstruct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgConstruct.ShowEditingIcon = false;
            dgConstruct.Size = new Size(612, 415);
            dgConstruct.TabIndex = 1;
            dgConstruct.SortCompare += dgConstruct_SortCompare;
            dgConstruct.DoubleClick += dgConstruct_DoubleClick;
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
            // colType
            // 
            colType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colType.HeaderText = "Type";
            colType.Name = "colType";
            colType.ReadOnly = true;
            colType.Width = 68;
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
            // tpUpgrade
            // 
            tpUpgrade.Controls.Add(btDismantleAll);
            tpUpgrade.Controls.Add(lbNoEngineering);
            tpUpgrade.Controls.Add(btModify);
            tpUpgrade.Controls.Add(btDismantle);
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
            tpUpgrade.Text = "Engineering";
            tpUpgrade.UseVisualStyleBackColor = true;
            // 
            // btDismantleAll
            // 
            btDismantleAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btDismantleAll.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDismantleAll.Location = new Point(518, 474);
            btDismantleAll.Margin = new Padding(4, 3, 4, 3);
            btDismantleAll.Name = "btDismantleAll";
            btDismantleAll.Size = new Size(100, 28);
            btDismantleAll.TabIndex = 14;
            btDismantleAll.Text = "Dismantle All";
            btDismantleAll.UseVisualStyleBackColor = true;
            btDismantleAll.Click += btDismantleAll_Click;
            // 
            // lbNoEngineering
            // 
            lbNoEngineering.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lbNoEngineering.AutoSize = true;
            lbNoEngineering.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbNoEngineering.ForeColor = Color.Red;
            lbNoEngineering.ImageAlign = ContentAlignment.MiddleRight;
            lbNoEngineering.Location = new Point(13, 480);
            lbNoEngineering.Margin = new Padding(4, 0, 4, 0);
            lbNoEngineering.Name = "lbNoEngineering";
            lbNoEngineering.Size = new Size(404, 16);
            lbNoEngineering.TabIndex = 13;
            lbNoEngineering.Text = "Build an Engineering Bay to enable enhancement and modification!";
            lbNoEngineering.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btModify
            // 
            btModify.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btModify.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btModify.Location = new Point(224, 474);
            btModify.Margin = new Padding(4, 3, 4, 3);
            btModify.Name = "btModify";
            btModify.Size = new Size(118, 28);
            btModify.TabIndex = 12;
            btModify.Text = "Modify";
            btModify.UseVisualStyleBackColor = true;
            btModify.Click += btModify_Click;
            // 
            // btDismantle
            // 
            btDismantle.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btDismantle.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDismantle.Location = new Point(425, 474);
            btDismantle.Margin = new Padding(4, 3, 4, 3);
            btDismantle.Name = "btDismantle";
            btDismantle.Size = new Size(83, 28);
            btDismantle.TabIndex = 11;
            btDismantle.Text = "Dismantle";
            btDismantle.UseVisualStyleBackColor = true;
            btDismantle.Click += btDismantle_Click;
            // 
            // btImprove
            // 
            btImprove.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btImprove.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btImprove.Location = new Point(56, 474);
            btImprove.Margin = new Padding(4, 3, 4, 3);
            btImprove.Name = "btImprove";
            btImprove.Size = new Size(118, 28);
            btImprove.TabIndex = 9;
            btImprove.Text = "Enhance";
            btImprove.UseVisualStyleBackColor = true;
            btImprove.Click += btImprove_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.ImageAlign = ContentAlignment.MiddleRight;
            label2.Location = new Point(329, 16);
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
            tbUpgradeFilter.Location = new Point(380, 13);
            tbUpgradeFilter.Margin = new Padding(4, 3, 4, 3);
            tbUpgradeFilter.Name = "tbUpgradeFilter";
            tbUpgradeFilter.Size = new Size(238, 26);
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
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dgcItem.DefaultCellStyle = dataGridViewCellStyle3;
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
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle4;
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
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewTextBoxColumn3.HeaderText = "Race";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // colFee
            // 
            colFee.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colFee.HeaderText = "Fee";
            colFee.Name = "colFee";
            colFee.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle5;
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
            // 
            // colPrice
            // 
            colPrice.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colPrice.HeaderText = "Cost";
            colPrice.Name = "colPrice";
            colPrice.ReadOnly = true;
            // 
            // FabricateItems
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(644, 548);
            Controls.Add(tcMain);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            Name = "FabricateItems";
            Text = "Fabricate Items";
            tcMain.ResumeLayout(false);
            tpConstruct.ResumeLayout(false);
            tpConstruct.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgConstruct).EndInit();
            tpUpgrade.ResumeLayout(false);
            tpUpgrade.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgInventory).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tcMain;
        private TabPage tpConstruct;
        private Button btConstruct;
        private DataGridView dgConstruct;
        private ComboBox cbItemType;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn colFee;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn colPrice;
        private Label label1;
        private TextBox tbFilter;
        private TabPage tpUpgrade;
        private Label label2;
        private TextBox tbUpgradeFilter;
        private ComboBox cbUpgradeItemType;
        private DataGridView dgInventory;
        private Button btImprove;
        private Button btDismantle;
        private DataGridViewTextBoxColumn dgcItem;
        private DataGridViewTextBoxColumn dgcLocation;
        private DataGridViewTextBoxColumn dgcValue;
        private DataGridViewTextBoxColumn dgcAvail;
        private DataGridViewTextBoxColumn colItem;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn Mass;
        private DataGridViewTextBoxColumn colAvail;
        private CheckBox cbHideUnbuildable;
        private Button btModify;
        private Label lbNoEngineering;
        private Button btDismantleAll;
    }
}