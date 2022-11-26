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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
      this.btRunMission = new System.Windows.Forms.Button();
      this.dgMissions = new System.Windows.Forms.DataGridView();
      this.colMission = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colGoal = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colOpp = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colDiff = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colLevels = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.lbNone = new System.Windows.Forms.Label();
      this.pbScan = new System.Windows.Forms.ProgressBar();
      ((System.ComponentModel.ISupportInitialize)(this.dgMissions)).BeginInit();
      this.SuspendLayout();
      // 
      // btRunMission
      // 
      this.btRunMission.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.btRunMission.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRunMission.Location = new System.Drawing.Point(173, 388);
      this.btRunMission.Name = "btRunMission";
      this.btRunMission.Size = new System.Drawing.Size(156, 33);
      this.btRunMission.TabIndex = 6;
      this.btRunMission.Text = "Run Mission";
      this.btRunMission.UseVisualStyleBackColor = true;
      this.btRunMission.Click += new System.EventHandler(this.btRunMission_Click);
      // 
      // dgMissions
      // 
      this.dgMissions.AllowUserToAddRows = false;
      this.dgMissions.AllowUserToDeleteRows = false;
      this.dgMissions.AllowUserToResizeRows = false;
      this.dgMissions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgMissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgMissions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMission,
            this.colGoal,
            this.colOpp,
            this.colDiff,
            this.colLevels});
      this.dgMissions.Dock = System.Windows.Forms.DockStyle.Top;
      this.dgMissions.Location = new System.Drawing.Point(0, 0);
      this.dgMissions.MultiSelect = false;
      this.dgMissions.Name = "dgMissions";
      this.dgMissions.ReadOnly = true;
      this.dgMissions.RowHeadersVisible = false;
      this.dgMissions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgMissions.ShowEditingIcon = false;
      this.dgMissions.Size = new System.Drawing.Size(504, 376);
      this.dgMissions.TabIndex = 5;
      this.dgMissions.SelectionChanged += new System.EventHandler(this.dgMissions_SelectionChanged);
      this.dgMissions.DoubleClick += new System.EventHandler(this.dgMissions_DoubleClick);
      // 
      // colMission
      // 
      this.colMission.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colMission.DefaultCellStyle = dataGridViewCellStyle1;
      this.colMission.HeaderText = "Mission";
      this.colMission.Name = "colMission";
      this.colMission.ReadOnly = true;
      // 
      // colGoal
      // 
      this.colGoal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colGoal.DefaultCellStyle = dataGridViewCellStyle2;
      this.colGoal.HeaderText = "Goal";
      this.colGoal.Name = "colGoal";
      this.colGoal.ReadOnly = true;
      this.colGoal.Width = 54;
      // 
      // colOpp
      // 
      this.colOpp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colOpp.DefaultCellStyle = dataGridViewCellStyle3;
      this.colOpp.HeaderText = "Enemy";
      this.colOpp.Name = "colOpp";
      this.colOpp.ReadOnly = true;
      this.colOpp.Width = 64;
      // 
      // colDiff
      // 
      this.colDiff.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colDiff.DefaultCellStyle = dataGridViewCellStyle4;
      this.colDiff.HeaderText = "Diff";
      this.colDiff.Name = "colDiff";
      this.colDiff.ReadOnly = true;
      this.colDiff.Width = 48;
      // 
      // colLevels
      // 
      this.colLevels.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.colLevels.DefaultCellStyle = dataGridViewCellStyle5;
      this.colLevels.HeaderText = "Levels";
      this.colLevels.Name = "colLevels";
      this.colLevels.ReadOnly = true;
      this.colLevels.Width = 63;
      // 
      // lbNone
      // 
      this.lbNone.AutoSize = true;
      this.lbNone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbNone.Location = new System.Drawing.Point(172, 187);
      this.lbNone.Name = "lbNone";
      this.lbNone.Size = new System.Drawing.Size(161, 20);
      this.lbNone.TabIndex = 7;
      this.lbNone.Text = "No Missions Available";
      // 
      // pbScan
      // 
      this.pbScan.Location = new System.Drawing.Point(32, 285);
      this.pbScan.Maximum = 20;
      this.pbScan.Name = "pbScan";
      this.pbScan.Size = new System.Drawing.Size(440, 51);
      this.pbScan.Step = 1;
      this.pbScan.TabIndex = 8;
      // 
      // ScanPlanet
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(504, 433);
      this.Controls.Add(this.pbScan);
      this.Controls.Add(this.lbNone);
      this.Controls.Add(this.btRunMission);
      this.Controls.Add(this.dgMissions);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ScanPlanet";
      this.Text = "ScanPlanet";
      ((System.ComponentModel.ISupportInitialize)(this.dgMissions)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

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
  }
}