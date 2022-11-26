namespace SpaceMercs.Dialogs {
  partial class InventoryView {
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
      this.dgInventory = new System.Windows.Forms.DataGridView();
      this.Item = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Mass = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.MassTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btTransfer = new System.Windows.Forms.Button();
      this.btDestroy = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dgInventory)).BeginInit();
      this.SuspendLayout();
      // 
      // dgInventory
      // 
      this.dgInventory.AllowUserToAddRows = false;
      this.dgInventory.AllowUserToDeleteRows = false;
      this.dgInventory.AllowUserToResizeColumns = false;
      this.dgInventory.AllowUserToResizeRows = false;
      this.dgInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgInventory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Item,
            this.Type,
            this.Mass,
            this.Count,
            this.MassTotal});
      this.dgInventory.Location = new System.Drawing.Point(0, 0);
      this.dgInventory.MultiSelect = false;
      this.dgInventory.Name = "dgInventory";
      this.dgInventory.ReadOnly = true;
      this.dgInventory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgInventory.Size = new System.Drawing.Size(489, 468);
      this.dgInventory.TabIndex = 0;
      this.dgInventory.SelectionChanged += new System.EventHandler(this.dgInventory_SelectionChanged);
      this.dgInventory.DoubleClick += new System.EventHandler(this.dgInventory_DoubleClick);
      // 
      // Item
      // 
      this.Item.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.Item.HeaderText = "Item";
      this.Item.Name = "Item";
      this.Item.ReadOnly = true;
      // 
      // Type
      // 
      this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.Type.HeaderText = "Type";
      this.Type.Name = "Type";
      this.Type.ReadOnly = true;
      this.Type.Width = 56;
      // 
      // Mass
      // 
      this.Mass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.Mass.HeaderText = "Mass (each)";
      this.Mass.Name = "Mass";
      this.Mass.ReadOnly = true;
      this.Mass.Width = 90;
      // 
      // Count
      // 
      this.Count.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.Count.HeaderText = "Count";
      this.Count.Name = "Count";
      this.Count.ReadOnly = true;
      this.Count.Width = 60;
      // 
      // MassTotal
      // 
      this.MassTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.MassTotal.HeaderText = "Mass (total)";
      this.MassTotal.Name = "MassTotal";
      this.MassTotal.ReadOnly = true;
      this.MassTotal.Width = 86;
      // 
      // btTransfer
      // 
      this.btTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btTransfer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTransfer.Location = new System.Drawing.Point(21, 478);
      this.btTransfer.Name = "btTransfer";
      this.btTransfer.Size = new System.Drawing.Size(141, 28);
      this.btTransfer.TabIndex = 1;
      this.btTransfer.Text = "Transfer To Soldier";
      this.btTransfer.UseVisualStyleBackColor = true;
      this.btTransfer.Click += new System.EventHandler(this.btTransfer_Click);
      // 
      // btDestroy
      // 
      this.btDestroy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btDestroy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btDestroy.Location = new System.Drawing.Point(324, 478);
      this.btDestroy.Name = "btDestroy";
      this.btDestroy.Size = new System.Drawing.Size(141, 28);
      this.btDestroy.TabIndex = 2;
      this.btDestroy.Text = "Destroy";
      this.btDestroy.UseVisualStyleBackColor = true;
      this.btDestroy.Click += new System.EventHandler(this.btDestroy_Click);
      // 
      // InventoryView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(489, 518);
      this.Controls.Add(this.btDestroy);
      this.Controls.Add(this.btTransfer);
      this.Controls.Add(this.dgInventory);
      this.MaximizeBox = false;
      this.Name = "InventoryView";
      this.Text = "InventoryView";
      this.TopMost = true;
      this.Activated += new System.EventHandler(this.InventoryView_Activated);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InventoryView_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.dgInventory)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dgInventory;
    private System.Windows.Forms.Button btTransfer;
    private System.Windows.Forms.Button btDestroy;
    private System.Windows.Forms.DataGridViewTextBoxColumn Item;
    private System.Windows.Forms.DataGridViewTextBoxColumn Type;
    private System.Windows.Forms.DataGridViewTextBoxColumn Mass;
    private System.Windows.Forms.DataGridViewTextBoxColumn Count;
    private System.Windows.Forms.DataGridViewTextBoxColumn MassTotal;
  }
}