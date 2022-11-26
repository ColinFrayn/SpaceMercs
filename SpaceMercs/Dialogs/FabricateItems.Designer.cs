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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
      this.tcMain = new System.Windows.Forms.TabControl();
      this.tpConstruct = new System.Windows.Forms.TabPage();
      this.cbHideUnbuildable = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.tbFilter = new System.Windows.Forms.TextBox();
      this.btConstruct = new System.Windows.Forms.Button();
      this.dgConstruct = new System.Windows.Forms.DataGridView();
      this.colItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Mass = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colAvail = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cbItemType = new System.Windows.Forms.ComboBox();
      this.tpUpgrade = new System.Windows.Forms.TabPage();
      this.btDismantle = new System.Windows.Forms.Button();
      this.btImprove = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.tbUpgradeFilter = new System.Windows.Forms.TextBox();
      this.cbUpgradeItemType = new System.Windows.Forms.ComboBox();
      this.dgInventory = new System.Windows.Forms.DataGridView();
      this.dgcItem = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgcLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgcValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgcAvail = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colFee = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.tcMain.SuspendLayout();
      this.tpConstruct.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgConstruct)).BeginInit();
      this.tpUpgrade.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgInventory)).BeginInit();
      this.SuspendLayout();
      // 
      // tcMain
      // 
      this.tcMain.Controls.Add(this.tpConstruct);
      this.tcMain.Controls.Add(this.tpUpgrade);
      this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tcMain.Location = new System.Drawing.Point(0, 0);
      this.tcMain.Name = "tcMain";
      this.tcMain.SelectedIndex = 0;
      this.tcMain.Size = new System.Drawing.Size(552, 475);
      this.tcMain.TabIndex = 0;
      this.tcMain.SelectedIndexChanged += new System.EventHandler(this.tcMain_SelectedIndexChanged);
      // 
      // tpConstruct
      // 
      this.tpConstruct.Controls.Add(this.cbHideUnbuildable);
      this.tpConstruct.Controls.Add(this.label1);
      this.tpConstruct.Controls.Add(this.tbFilter);
      this.tpConstruct.Controls.Add(this.btConstruct);
      this.tpConstruct.Controls.Add(this.dgConstruct);
      this.tpConstruct.Controls.Add(this.cbItemType);
      this.tpConstruct.Location = new System.Drawing.Point(4, 29);
      this.tpConstruct.Name = "tpConstruct";
      this.tpConstruct.Size = new System.Drawing.Size(544, 442);
      this.tpConstruct.TabIndex = 0;
      this.tpConstruct.Text = "Construct";
      this.tpConstruct.UseVisualStyleBackColor = true;
      // 
      // cbHideUnbuildable
      // 
      this.cbHideUnbuildable.AutoSize = true;
      this.cbHideUnbuildable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbHideUnbuildable.Location = new System.Drawing.Point(9, 412);
      this.cbHideUnbuildable.Name = "cbHideUnbuildable";
      this.cbHideUnbuildable.Size = new System.Drawing.Size(132, 20);
      this.cbHideUnbuildable.TabIndex = 5;
      this.cbHideUnbuildable.Text = "Hide Unbuildable";
      this.cbHideUnbuildable.UseVisualStyleBackColor = true;
      this.cbHideUnbuildable.CheckedChanged += new System.EventHandler(this.cbHideUnbuildable_CheckedChanged);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.label1.Location = new System.Drawing.Point(282, 14);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(37, 16);
      this.label1.TabIndex = 4;
      this.label1.Text = "Filter";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tbFilter
      // 
      this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbFilter.Location = new System.Drawing.Point(326, 11);
      this.tbFilter.Name = "tbFilter";
      this.tbFilter.Size = new System.Drawing.Size(205, 26);
      this.tbFilter.TabIndex = 3;
      this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
      // 
      // btConstruct
      // 
      this.btConstruct.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btConstruct.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btConstruct.Location = new System.Drawing.Point(229, 415);
      this.btConstruct.Name = "btConstruct";
      this.btConstruct.Size = new System.Drawing.Size(87, 24);
      this.btConstruct.TabIndex = 2;
      this.btConstruct.Text = "Construct";
      this.btConstruct.UseVisualStyleBackColor = true;
      this.btConstruct.Click += new System.EventHandler(this.btConstruct_Click);
      // 
      // dgConstruct
      // 
      this.dgConstruct.AllowUserToAddRows = false;
      this.dgConstruct.AllowUserToDeleteRows = false;
      this.dgConstruct.AllowUserToResizeRows = false;
      this.dgConstruct.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgConstruct.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgConstruct.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgConstruct.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItem,
            this.colType,
            this.Mass,
            this.colAvail});
      this.dgConstruct.Location = new System.Drawing.Point(9, 46);
      this.dgConstruct.MultiSelect = false;
      this.dgConstruct.Name = "dgConstruct";
      this.dgConstruct.ReadOnly = true;
      this.dgConstruct.RowHeadersVisible = false;
      this.dgConstruct.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgConstruct.ShowEditingIcon = false;
      this.dgConstruct.Size = new System.Drawing.Size(525, 360);
      this.dgConstruct.TabIndex = 1;
      this.dgConstruct.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgConstruct_SortCompare);
      this.dgConstruct.DoubleClick += new System.EventHandler(this.dgConstruct_DoubleClick);
      // 
      // colItem
      // 
      this.colItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colItem.DefaultCellStyle = dataGridViewCellStyle7;
      this.colItem.HeaderText = "Item";
      this.colItem.Name = "colItem";
      this.colItem.ReadOnly = true;
      // 
      // colType
      // 
      this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colType.HeaderText = "Type";
      this.colType.Name = "colType";
      this.colType.ReadOnly = true;
      this.colType.Width = 68;
      // 
      // Mass
      // 
      this.Mass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Mass.DefaultCellStyle = dataGridViewCellStyle8;
      this.Mass.HeaderText = "Mass";
      this.Mass.Name = "Mass";
      this.Mass.ReadOnly = true;
      this.Mass.Width = 72;
      // 
      // colAvail
      // 
      this.colAvail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colAvail.HeaderText = "Avail";
      this.colAvail.Name = "colAvail";
      this.colAvail.ReadOnly = true;
      this.colAvail.Width = 67;
      // 
      // cbItemType
      // 
      this.cbItemType.FormattingEnabled = true;
      this.cbItemType.Location = new System.Drawing.Point(11, 11);
      this.cbItemType.Name = "cbItemType";
      this.cbItemType.Size = new System.Drawing.Size(153, 28);
      this.cbItemType.TabIndex = 0;
      this.cbItemType.SelectedIndexChanged += new System.EventHandler(this.cbItemType_SelectedIndexChanged);
      // 
      // tpUpgrade
      // 
      this.tpUpgrade.Controls.Add(this.btDismantle);
      this.tpUpgrade.Controls.Add(this.btImprove);
      this.tpUpgrade.Controls.Add(this.label2);
      this.tpUpgrade.Controls.Add(this.tbUpgradeFilter);
      this.tpUpgrade.Controls.Add(this.cbUpgradeItemType);
      this.tpUpgrade.Controls.Add(this.dgInventory);
      this.tpUpgrade.Location = new System.Drawing.Point(4, 29);
      this.tpUpgrade.Name = "tpUpgrade";
      this.tpUpgrade.Size = new System.Drawing.Size(544, 442);
      this.tpUpgrade.TabIndex = 4;
      this.tpUpgrade.Text = "Modify";
      this.tpUpgrade.UseVisualStyleBackColor = true;
      // 
      // btDismantle
      // 
      this.btDismantle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btDismantle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btDismantle.Location = new System.Drawing.Point(313, 412);
      this.btDismantle.Name = "btDismantle";
      this.btDismantle.Size = new System.Drawing.Size(101, 24);
      this.btDismantle.TabIndex = 11;
      this.btDismantle.Text = "Dismantle";
      this.btDismantle.UseVisualStyleBackColor = true;
      this.btDismantle.Click += new System.EventHandler(this.btDismantle_Click);
      // 
      // btImprove
      // 
      this.btImprove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btImprove.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btImprove.Location = new System.Drawing.Point(123, 412);
      this.btImprove.Name = "btImprove";
      this.btImprove.Size = new System.Drawing.Size(101, 24);
      this.btImprove.TabIndex = 9;
      this.btImprove.Text = "Improve";
      this.btImprove.UseVisualStyleBackColor = true;
      this.btImprove.Click += new System.EventHandler(this.btImprove_Click);
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.label2.Location = new System.Drawing.Point(282, 14);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 16);
      this.label2.TabIndex = 7;
      this.label2.Text = "Filter";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tbUpgradeFilter
      // 
      this.tbUpgradeFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tbUpgradeFilter.Location = new System.Drawing.Point(326, 11);
      this.tbUpgradeFilter.Name = "tbUpgradeFilter";
      this.tbUpgradeFilter.Size = new System.Drawing.Size(205, 26);
      this.tbUpgradeFilter.TabIndex = 6;
      this.tbUpgradeFilter.TextChanged += new System.EventHandler(this.tbUpgradeFilter_TextChanged);
      // 
      // cbUpgradeItemType
      // 
      this.cbUpgradeItemType.FormattingEnabled = true;
      this.cbUpgradeItemType.Location = new System.Drawing.Point(11, 11);
      this.cbUpgradeItemType.Name = "cbUpgradeItemType";
      this.cbUpgradeItemType.Size = new System.Drawing.Size(153, 28);
      this.cbUpgradeItemType.TabIndex = 5;
      this.cbUpgradeItemType.SelectedIndexChanged += new System.EventHandler(this.cbUpgradeItemType_SelectedIndexChanged);
      // 
      // dgInventory
      // 
      this.dgInventory.AllowUserToAddRows = false;
      this.dgInventory.AllowUserToDeleteRows = false;
      this.dgInventory.AllowUserToResizeRows = false;
      this.dgInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgInventory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgInventory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcItem,
            this.dgcLocation,
            this.dgcValue,
            this.dgcAvail});
      this.dgInventory.Location = new System.Drawing.Point(9, 46);
      this.dgInventory.MultiSelect = false;
      this.dgInventory.Name = "dgInventory";
      this.dgInventory.ReadOnly = true;
      this.dgInventory.RowHeadersVisible = false;
      this.dgInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgInventory.ShowEditingIcon = false;
      this.dgInventory.Size = new System.Drawing.Size(525, 360);
      this.dgInventory.TabIndex = 2;
      this.dgInventory.SelectionChanged += new System.EventHandler(this.dgInventory_SelectionChanged);
      this.dgInventory.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgInventory_SortCompare);
      this.dgInventory.DoubleClick += new System.EventHandler(this.dgInventory_DoubleClick);
      // 
      // dgcItem
      // 
      this.dgcItem.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dgcItem.DefaultCellStyle = dataGridViewCellStyle6;
      this.dgcItem.HeaderText = "Item";
      this.dgcItem.Name = "dgcItem";
      this.dgcItem.ReadOnly = true;
      // 
      // dgcLocation
      // 
      this.dgcLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dgcLocation.HeaderText = "Location";
      this.dgcLocation.Name = "dgcLocation";
      this.dgcLocation.ReadOnly = true;
      this.dgcLocation.Width = 95;
      // 
      // dgcValue
      // 
      this.dgcValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dgcValue.HeaderText = "Value";
      this.dgcValue.Name = "dgcValue";
      this.dgcValue.ReadOnly = true;
      this.dgcValue.Width = 75;
      // 
      // dgcAvail
      // 
      this.dgcAvail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dgcAvail.HeaderText = "Avail";
      this.dgcAvail.Name = "dgcAvail";
      this.dgcAvail.ReadOnly = true;
      this.dgcAvail.Width = 67;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle9;
      this.dataGridViewTextBoxColumn1.HeaderText = "Name";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dataGridViewTextBoxColumn2.HeaderText = "Level";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.ReadOnly = true;
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dataGridViewTextBoxColumn3.HeaderText = "Race";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.ReadOnly = true;
      // 
      // colFee
      // 
      this.colFee.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colFee.HeaderText = "Fee";
      this.colFee.Name = "colFee";
      this.colFee.ReadOnly = true;
      // 
      // dataGridViewTextBoxColumn6
      // 
      this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle10;
      this.dataGridViewTextBoxColumn6.HeaderText = "Ship";
      this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
      this.dataGridViewTextBoxColumn6.ReadOnly = true;
      // 
      // dataGridViewTextBoxColumn7
      // 
      this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dataGridViewTextBoxColumn7.HeaderText = "S/M/L/W";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      // 
      // colPrice
      // 
      this.colPrice.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colPrice.HeaderText = "Cost";
      this.colPrice.Name = "colPrice";
      this.colPrice.ReadOnly = true;
      // 
      // FabricateItems
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(552, 475);
      this.Controls.Add(this.tcMain);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "FabricateItems";
      this.Text = "Fabricate Items";
      this.tcMain.ResumeLayout(false);
      this.tpConstruct.ResumeLayout(false);
      this.tpConstruct.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgConstruct)).EndInit();
      this.tpUpgrade.ResumeLayout(false);
      this.tpUpgrade.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgInventory)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tcMain;
    private System.Windows.Forms.TabPage tpConstruct;
    private System.Windows.Forms.Button btConstruct;
    private System.Windows.Forms.DataGridView dgConstruct;
    private System.Windows.Forms.ComboBox cbItemType;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFee;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox tbFilter;
    private System.Windows.Forms.TabPage tpUpgrade;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tbUpgradeFilter;
    private System.Windows.Forms.ComboBox cbUpgradeItemType;
    private System.Windows.Forms.DataGridView dgInventory;
    private System.Windows.Forms.Button btImprove;
    private System.Windows.Forms.Button btDismantle;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcLocation;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcValue;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcAvail;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvail;
        private System.Windows.Forms.CheckBox cbHideUnbuildable;
    }
}