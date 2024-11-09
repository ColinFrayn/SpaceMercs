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
            dgInventory = new DataGridView();
            Item = new DataGridViewTextBoxColumn();
            Type = new DataGridViewTextBoxColumn();
            Mass = new DataGridViewTextBoxColumn();
            Count = new DataGridViewTextBoxColumn();
            MassTotal = new DataGridViewTextBoxColumn();
            btTransfer = new Button();
            btDestroy = new Button();
            lbTotalMass = new Label();
            lbCapacity = new Label();
            ((System.ComponentModel.ISupportInitialize)dgInventory).BeginInit();
            SuspendLayout();
            // 
            // dgInventory
            // 
            dgInventory.AllowUserToAddRows = false;
            dgInventory.AllowUserToDeleteRows = false;
            dgInventory.AllowUserToResizeColumns = false;
            dgInventory.AllowUserToResizeRows = false;
            dgInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgInventory.Columns.AddRange(new DataGridViewColumn[] { Item, Type, Mass, Count, MassTotal });
            dgInventory.Location = new Point(0, 0);
            dgInventory.Margin = new Padding(4, 3, 4, 3);
            dgInventory.MultiSelect = false;
            dgInventory.Name = "dgInventory";
            dgInventory.ReadOnly = true;
            dgInventory.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgInventory.Size = new Size(570, 540);
            dgInventory.TabIndex = 0;
            dgInventory.SelectionChanged += dgInventory_SelectionChanged;
            dgInventory.DoubleClick += dgInventory_DoubleClick;
            // 
            // Item
            // 
            Item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Item.HeaderText = "Item";
            Item.Name = "Item";
            Item.ReadOnly = true;
            // 
            // Type
            // 
            Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Type.HeaderText = "Type";
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Width = 56;
            // 
            // Mass
            // 
            Mass.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Mass.HeaderText = "Mass (each)";
            Mass.Name = "Mass";
            Mass.ReadOnly = true;
            Mass.Width = 95;
            // 
            // Count
            // 
            Count.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Count.HeaderText = "Count";
            Count.Name = "Count";
            Count.ReadOnly = true;
            Count.Width = 65;
            // 
            // MassTotal
            // 
            MassTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MassTotal.HeaderText = "Mass (total)";
            MassTotal.Name = "MassTotal";
            MassTotal.ReadOnly = true;
            MassTotal.Width = 94;
            // 
            // btTransfer
            // 
            btTransfer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btTransfer.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btTransfer.Location = new Point(24, 552);
            btTransfer.Margin = new Padding(4, 3, 4, 3);
            btTransfer.Name = "btTransfer";
            btTransfer.Size = new Size(164, 32);
            btTransfer.TabIndex = 1;
            btTransfer.Text = "Transfer To Soldier";
            btTransfer.UseVisualStyleBackColor = true;
            btTransfer.Click += btTransfer_Click;
            // 
            // btDestroy
            // 
            btDestroy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btDestroy.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDestroy.Location = new Point(378, 552);
            btDestroy.Margin = new Padding(4, 3, 4, 3);
            btDestroy.Name = "btDestroy";
            btDestroy.Size = new Size(164, 32);
            btDestroy.TabIndex = 2;
            btDestroy.Text = "Destroy";
            btDestroy.UseVisualStyleBackColor = true;
            btDestroy.Click += btDestroy_Click;
            // 
            // lbTotalMass
            // 
            lbTotalMass.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lbTotalMass.BorderStyle = BorderStyle.FixedSingle;
            lbTotalMass.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbTotalMass.Location = new Point(221, 552);
            lbTotalMass.Margin = new Padding(4, 3, 4, 3);
            lbTotalMass.Name = "lbTotalMass";
            lbTotalMass.Size = new Size(66, 32);
            lbTotalMass.TabIndex = 3;
            lbTotalMass.Text = "8888.8 kg";
            lbTotalMass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCapacity
            // 
            lbCapacity.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lbCapacity.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbCapacity.Location = new Point(288, 552);
            lbCapacity.Margin = new Padding(0, 3, 0, 3);
            lbCapacity.Name = "lbCapacity";
            lbCapacity.Size = new Size(74, 32);
            lbCapacity.TabIndex = 4;
            lbCapacity.Text = " / 8888 kg";
            lbCapacity.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // InventoryView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(570, 598);
            Controls.Add(lbTotalMass);
            Controls.Add(lbCapacity);
            Controls.Add(btDestroy);
            Controls.Add(btTransfer);
            Controls.Add(dgInventory);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "InventoryView";
            Text = "InventoryView";
            TopMost = true;
            Activated += InventoryView_Activated;
            FormClosing += InventoryView_FormClosing;
            ((System.ComponentModel.ISupportInitialize)dgInventory).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgInventory;
        private Button btTransfer;
        private Button btDestroy;
        private DataGridViewTextBoxColumn Item;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn Mass;
        private DataGridViewTextBoxColumn Count;
        private DataGridViewTextBoxColumn MassTotal;
        private Label lbTotalMass;
        private Label lbCapacity;
    }
}