namespace SpaceMercs.Dialogs {
  partial class ChooseSoldiers {
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
            dgSoldiers = new DataGridView();
            Selected = new DataGridViewCheckBoxColumn();
            dgcName = new DataGridViewTextBoxColumn();
            dgcRace = new DataGridViewTextBoxColumn();
            dgcLevel = new DataGridViewTextBoxColumn();
            btLaunch = new Button();
            btAbort = new Button();
            label1 = new Label();
            lbSquadSize = new Label();
            btRefill = new Button();
            lbLoadout = new Label();
            ((System.ComponentModel.ISupportInitialize)dgSoldiers).BeginInit();
            SuspendLayout();
            // 
            // dgSoldiers
            // 
            dgSoldiers.AllowUserToAddRows = false;
            dgSoldiers.AllowUserToDeleteRows = false;
            dgSoldiers.AllowUserToResizeColumns = false;
            dgSoldiers.AllowUserToResizeRows = false;
            dgSoldiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgSoldiers.Columns.AddRange(new DataGridViewColumn[] { Selected, dgcName, dgcRace, dgcLevel });
            dgSoldiers.Location = new Point(2, 33);
            dgSoldiers.Margin = new Padding(4, 3, 4, 3);
            dgSoldiers.Name = "dgSoldiers";
            dgSoldiers.ReadOnly = true;
            dgSoldiers.RowHeadersVisible = false;
            dgSoldiers.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgSoldiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgSoldiers.Size = new Size(517, 402);
            dgSoldiers.TabIndex = 0;
            dgSoldiers.CellContentClick += dgSoldiers_CellContentClick;
            dgSoldiers.SelectionChanged += dgSoldiers_SelectionChanged;
            // 
            // Selected
            // 
            Selected.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            Selected.HeaderText = "Selected";
            Selected.Name = "Selected";
            Selected.ReadOnly = true;
            Selected.Width = 57;
            // 
            // dgcName
            // 
            dgcName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgcName.HeaderText = "Name";
            dgcName.MinimumWidth = 20;
            dgcName.Name = "dgcName";
            dgcName.ReadOnly = true;
            // 
            // dgcRace
            // 
            dgcRace.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgcRace.HeaderText = "Race";
            dgcRace.Name = "dgcRace";
            dgcRace.ReadOnly = true;
            dgcRace.Width = 57;
            // 
            // dgcLevel
            // 
            dgcLevel.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgcLevel.HeaderText = "Level";
            dgcLevel.Name = "dgcLevel";
            dgcLevel.ReadOnly = true;
            dgcLevel.Width = 59;
            // 
            // btLaunch
            // 
            btLaunch.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btLaunch.Location = new Point(30, 452);
            btLaunch.Margin = new Padding(4, 3, 4, 3);
            btLaunch.Name = "btLaunch";
            btLaunch.Size = new Size(149, 51);
            btLaunch.TabIndex = 1;
            btLaunch.Text = "Launch";
            btLaunch.UseVisualStyleBackColor = true;
            btLaunch.Click += btLaunch_Click;
            // 
            // btAbort
            // 
            btAbort.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btAbort.Location = new Point(345, 452);
            btAbort.Margin = new Padding(4, 3, 4, 3);
            btAbort.Name = "btAbort";
            btAbort.Size = new Size(142, 51);
            btAbort.TabIndex = 2;
            btAbort.Text = "Abort";
            btAbort.UseVisualStyleBackColor = true;
            btAbort.Click += btAbort_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(150, 8);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(136, 16);
            label1.TabIndex = 3;
            label1.Text = "Maximum Squad Size";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbSquadSize
            // 
            lbSquadSize.BorderStyle = BorderStyle.FixedSingle;
            lbSquadSize.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbSquadSize.Location = new Point(316, 3);
            lbSquadSize.Margin = new Padding(4, 0, 4, 0);
            lbSquadSize.Name = "lbSquadSize";
            lbSquadSize.Size = new Size(56, 26);
            lbSquadSize.TabIndex = 4;
            lbSquadSize.Text = "88";
            lbSquadSize.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btRefill
            // 
            btRefill.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btRefill.Location = new Point(215, 477);
            btRefill.Margin = new Padding(4, 3, 4, 3);
            btRefill.Name = "btRefill";
            btRefill.Size = new Size(101, 23);
            btRefill.TabIndex = 5;
            btRefill.Text = "Refill";
            btRefill.UseVisualStyleBackColor = true;
            btRefill.Click += btRefill_Click;
            // 
            // lbLoadout
            // 
            lbLoadout.AutoSize = true;
            lbLoadout.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbLoadout.ForeColor = Color.Red;
            lbLoadout.Location = new Point(197, 457);
            lbLoadout.Margin = new Padding(4, 0, 4, 0);
            lbLoadout.Name = "lbLoadout";
            lbLoadout.Size = new Size(128, 16);
            lbLoadout.TabIndex = 6;
            lbLoadout.Text = "Loadout Incomplete!";
            lbLoadout.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ChooseSoldiers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(523, 517);
            Controls.Add(lbLoadout);
            Controls.Add(btRefill);
            Controls.Add(lbSquadSize);
            Controls.Add(label1);
            Controls.Add(btAbort);
            Controls.Add(btLaunch);
            Controls.Add(dgSoldiers);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseSoldiers";
            Text = "ChooseSoldiers";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)dgSoldiers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgSoldiers;
    private System.Windows.Forms.Button btLaunch;
    private System.Windows.Forms.Button btAbort;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbSquadSize;
    private System.Windows.Forms.DataGridViewCheckBoxColumn Selected;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcName;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcRace;
    private System.Windows.Forms.DataGridViewTextBoxColumn dgcLevel;
        private Button btRefill;
        private Label lbLoadout;
    }
}