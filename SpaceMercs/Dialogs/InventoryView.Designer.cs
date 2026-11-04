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
            dgInventory.AllowDrop = true;
            dgInventory.AllowUserToAddRows = false;
            dgInventory.AllowUserToDeleteRows = false;
            dgInventory.AllowUserToResizeColumns = false;
            dgInventory.AllowUserToResizeRows = false;
            dgInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgInventory.Columns.AddRange(new DataGridViewColumn[] { Item, Type, Mass, Count, MassTotal });
            dgInventory.Location = new Point(0, 0);
            dgInventory.Margin = new Padding(6, 5, 6, 5);
            dgInventory.MultiSelect = false;
            dgInventory.Name = "dgInventory";
            dgInventory.ReadOnly = true;
            dgInventory.RowHeadersWidth = 62;
            dgInventory.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgInventory.Size = new Size(814, 900);
            dgInventory.TabIndex = 0;
            dgInventory.SelectionChanged += dgInventory_SelectionChanged;
            dgInventory.DragDrop += dgInventory_DragDrop;
            dgInventory.DragEnter += dgInventory_DragEnter;
            dgInventory.DoubleClick += dgInventory_DoubleClick;
            dgInventory.MouseDown += dgInventory_MouseDown;
            dgInventory.MouseMove += dgInventory_MouseMove;
            dgInventory.MouseUp += dgInventory_MouseUp;
            // 
            // Item
            // 
            Item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Item.HeaderText = "Item";
            Item.MinimumWidth = 8;
            Item.Name = "Item";
            Item.ReadOnly = true;
            // 
            // Type
            // 
            Type.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Type.HeaderText = "Type";
            Type.MinimumWidth = 8;
            Type.Name = "Type";
            Type.ReadOnly = true;
            Type.Width = 85;
            // 
            // Mass
            // 
            Mass.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Mass.HeaderText = "Mass (each)";
            Mass.MinimumWidth = 8;
            Mass.Name = "Mass";
            Mass.ReadOnly = true;
            Mass.Width = 140;
            // 
            // Count
            // 
            Count.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Count.HeaderText = "Count";
            Count.MinimumWidth = 8;
            Count.Name = "Count";
            Count.ReadOnly = true;
            Count.Width = 96;
            // 
            // MassTotal
            // 
            MassTotal.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            MassTotal.HeaderText = "Mass (total)";
            MassTotal.MinimumWidth = 8;
            MassTotal.Name = "MassTotal";
            MassTotal.ReadOnly = true;
            MassTotal.Width = 140;
            // 
            // btTransfer
            // 
            btTransfer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btTransfer.Font = new Font("Microsoft Sans Serif", 9.75F);
            btTransfer.Location = new Point(34, 920);
            btTransfer.Margin = new Padding(6, 5, 6, 5);
            btTransfer.Name = "btTransfer";
            btTransfer.Size = new Size(234, 53);
            btTransfer.TabIndex = 1;
            btTransfer.Text = "Transfer To Soldier";
            btTransfer.UseVisualStyleBackColor = true;
            btTransfer.Click += btTransfer_Click;
            // 
            // btDestroy
            // 
            btDestroy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btDestroy.Font = new Font("Microsoft Sans Serif", 9.75F);
            btDestroy.Location = new Point(540, 920);
            btDestroy.Margin = new Padding(6, 5, 6, 5);
            btDestroy.Name = "btDestroy";
            btDestroy.Size = new Size(234, 53);
            btDestroy.TabIndex = 2;
            btDestroy.Text = "Destroy";
            btDestroy.UseVisualStyleBackColor = true;
            btDestroy.Click += btDestroy_Click;
            // 
            // lbTotalMass
            // 
            lbTotalMass.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lbTotalMass.BorderStyle = BorderStyle.FixedSingle;
            lbTotalMass.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbTotalMass.Location = new Point(309, 920);
            lbTotalMass.Margin = new Padding(6, 5, 6, 5);
            lbTotalMass.Name = "lbTotalMass";
            lbTotalMass.Size = new Size(100, 52);
            lbTotalMass.TabIndex = 3;
            lbTotalMass.Text = "8888.8 kg";
            lbTotalMass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbCapacity
            // 
            lbCapacity.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lbCapacity.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbCapacity.Location = new Point(411, 920);
            lbCapacity.Margin = new Padding(0, 5, 0, 5);
            lbCapacity.Name = "lbCapacity";
            lbCapacity.Size = new Size(106, 53);
            lbCapacity.TabIndex = 4;
            lbCapacity.Text = " / 8888 kg";
            lbCapacity.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // InventoryView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 997);
            Controls.Add(lbTotalMass);
            Controls.Add(lbCapacity);
            Controls.Add(btDestroy);
            Controls.Add(btTransfer);
            Controls.Add(dgInventory);
            Margin = new Padding(6, 5, 6, 5);
            MaximizeBox = false;
            Name = "InventoryView";
            Text = "InventoryView";
            TopMost = true;
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