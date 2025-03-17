namespace SpaceMercs.Dialogs {
  partial class ScanPlanet {
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
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            btRunMission = new Button();
            dgMissions = new DataGridView();
            colMission = new DataGridViewTextBoxColumn();
            colGoal = new DataGridViewTextBoxColumn();
            colOpp = new DataGridViewTextBoxColumn();
            colDiff = new DataGridViewTextBoxColumn();
            colLevels = new DataGridViewTextBoxColumn();
            lbNone = new Label();
            pbScan = new ProgressBar();
            btRegenerate = new Button();
            ((System.ComponentModel.ISupportInitialize)dgMissions).BeginInit();
            SuspendLayout();
            // 
            // btRunMission
            // 
            btRunMission.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btRunMission.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btRunMission.Location = new Point(202, 448);
            btRunMission.Margin = new Padding(4, 3, 4, 3);
            btRunMission.Name = "btRunMission";
            btRunMission.Size = new Size(182, 38);
            btRunMission.TabIndex = 6;
            btRunMission.Text = "Run Mission";
            btRunMission.UseVisualStyleBackColor = true;
            btRunMission.Click += btRunMission_Click;
            // 
            // dgMissions
            // 
            dgMissions.AllowUserToAddRows = false;
            dgMissions.AllowUserToDeleteRows = false;
            dgMissions.AllowUserToResizeRows = false;
            dgMissions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgMissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgMissions.Columns.AddRange(new DataGridViewColumn[] { colMission, colGoal, colOpp, colDiff, colLevels });
            dgMissions.Dock = DockStyle.Top;
            dgMissions.Location = new Point(0, 0);
            dgMissions.Margin = new Padding(4, 3, 4, 3);
            dgMissions.MultiSelect = false;
            dgMissions.Name = "dgMissions";
            dgMissions.ReadOnly = true;
            dgMissions.RowHeadersVisible = false;
            dgMissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgMissions.ShowEditingIcon = false;
            dgMissions.Size = new Size(588, 434);
            dgMissions.TabIndex = 5;
            dgMissions.SelectionChanged += dgMissions_SelectionChanged;
            dgMissions.DoubleClick += dgMissions_DoubleClick;
            // 
            // colMission
            // 
            colMission.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            colMission.DefaultCellStyle = dataGridViewCellStyle6;
            colMission.HeaderText = "Mission";
            colMission.Name = "colMission";
            colMission.ReadOnly = true;
            // 
            // colGoal
            // 
            colGoal.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle7.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            colGoal.DefaultCellStyle = dataGridViewCellStyle7;
            colGoal.HeaderText = "Goal";
            colGoal.Name = "colGoal";
            colGoal.ReadOnly = true;
            colGoal.Width = 56;
            // 
            // colOpp
            // 
            colOpp.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle8.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            colOpp.DefaultCellStyle = dataGridViewCellStyle8;
            colOpp.HeaderText = "Enemy";
            colOpp.Name = "colOpp";
            colOpp.ReadOnly = true;
            colOpp.Width = 68;
            // 
            // colDiff
            // 
            colDiff.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle9.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            colDiff.DefaultCellStyle = dataGridViewCellStyle9;
            colDiff.HeaderText = "Diff";
            colDiff.Name = "colDiff";
            colDiff.ReadOnly = true;
            colDiff.Width = 51;
            // 
            // colLevels
            // 
            colLevels.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle10.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            colLevels.DefaultCellStyle = dataGridViewCellStyle10;
            colLevels.HeaderText = "Levels";
            colLevels.Name = "colLevels";
            colLevels.ReadOnly = true;
            colLevels.Width = 64;
            // 
            // lbNone
            // 
            lbNone.AutoSize = true;
            lbNone.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbNone.Location = new Point(201, 216);
            lbNone.Margin = new Padding(4, 0, 4, 0);
            lbNone.Name = "lbNone";
            lbNone.Size = new Size(161, 20);
            lbNone.TabIndex = 7;
            lbNone.Text = "No Missions Available";
            // 
            // pbScan
            // 
            pbScan.Location = new Point(37, 329);
            pbScan.Margin = new Padding(4, 3, 4, 3);
            pbScan.Maximum = 24;
            pbScan.Name = "pbScan";
            pbScan.Size = new Size(513, 59);
            pbScan.Step = 1;
            pbScan.TabIndex = 8;
            // 
            // btRegenerate
            // 
            btRegenerate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btRegenerate.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btRegenerate.Location = new Point(462, 448);
            btRegenerate.Margin = new Padding(4, 3, 4, 3);
            btRegenerate.Name = "btRegenerate";
            btRegenerate.Size = new Size(113, 38);
            btRegenerate.TabIndex = 9;
            btRegenerate.Text = "Regenerate";
            btRegenerate.UseVisualStyleBackColor = true;
            btRegenerate.Click += btRegenerate_Click;
            // 
            // ScanPlanet
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 500);
            Controls.Add(btRegenerate);
            Controls.Add(pbScan);
            Controls.Add(lbNone);
            Controls.Add(btRunMission);
            Controls.Add(dgMissions);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ScanPlanet";
            Text = "ScanPlanet";
            ((System.ComponentModel.ISupportInitialize)dgMissions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btRunMission;
    private System.Windows.Forms.DataGridView dgMissions;
    private System.Windows.Forms.Label lbNone;
    private System.Windows.Forms.ProgressBar pbScan;
    private System.Windows.Forms.DataGridViewTextBoxColumn colMission;
    private System.Windows.Forms.DataGridViewTextBoxColumn colGoal;
    private System.Windows.Forms.DataGridViewTextBoxColumn colOpp;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDiff;
    private System.Windows.Forms.DataGridViewTextBoxColumn colLevels;
        private Button btRegenerate;
    }
}