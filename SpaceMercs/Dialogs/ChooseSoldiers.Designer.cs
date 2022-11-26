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
      this.dgSoldiers = new System.Windows.Forms.DataGridView();
      this.Selected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.dgcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgcRace = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgcLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btLaunch = new System.Windows.Forms.Button();
      this.btAbort = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.lbSquadSize = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.dgSoldiers)).BeginInit();
      this.SuspendLayout();
      // 
      // dgSoldiers
      // 
      this.dgSoldiers.AllowUserToAddRows = false;
      this.dgSoldiers.AllowUserToDeleteRows = false;
      this.dgSoldiers.AllowUserToResizeColumns = false;
      this.dgSoldiers.AllowUserToResizeRows = false;
      this.dgSoldiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgSoldiers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selected,
            this.dgcName,
            this.dgcRace,
            this.dgcLevel});
      this.dgSoldiers.Location = new System.Drawing.Point(2, 29);
      this.dgSoldiers.Name = "dgSoldiers";
      this.dgSoldiers.ReadOnly = true;
      this.dgSoldiers.RowHeadersVisible = false;
      this.dgSoldiers.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgSoldiers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgSoldiers.Size = new System.Drawing.Size(443, 348);
      this.dgSoldiers.TabIndex = 0;
      this.dgSoldiers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgSoldiers_CellContentClick);
      this.dgSoldiers.SelectionChanged += new System.EventHandler(this.dgSoldiers_SelectionChanged);
      // 
      // Selected
      // 
      this.Selected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
      this.Selected.HeaderText = "Selected";
      this.Selected.Name = "Selected";
      this.Selected.ReadOnly = true;
      this.Selected.Width = 55;
      // 
      // dgcName
      // 
      this.dgcName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.dgcName.HeaderText = "Name";
      this.dgcName.MinimumWidth = 20;
      this.dgcName.Name = "dgcName";
      this.dgcName.ReadOnly = true;
      // 
      // dgcRace
      // 
      this.dgcRace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dgcRace.HeaderText = "Race";
      this.dgcRace.Name = "dgcRace";
      this.dgcRace.ReadOnly = true;
      this.dgcRace.Width = 58;
      // 
      // dgcLevel
      // 
      this.dgcLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.dgcLevel.HeaderText = "Level";
      this.dgcLevel.Name = "dgcLevel";
      this.dgcLevel.ReadOnly = true;
      this.dgcLevel.Width = 58;
      // 
      // btLaunch
      // 
      this.btLaunch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btLaunch.Location = new System.Drawing.Point(36, 392);
      this.btLaunch.Name = "btLaunch";
      this.btLaunch.Size = new System.Drawing.Size(128, 44);
      this.btLaunch.TabIndex = 1;
      this.btLaunch.Text = "Launch";
      this.btLaunch.UseVisualStyleBackColor = true;
      this.btLaunch.Click += new System.EventHandler(this.btLaunch_Click);
      // 
      // btAbort
      // 
      this.btAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAbort.Location = new System.Drawing.Point(282, 392);
      this.btAbort.Name = "btAbort";
      this.btAbort.Size = new System.Drawing.Size(122, 44);
      this.btAbort.TabIndex = 2;
      this.btAbort.Text = "Abort";
      this.btAbort.UseVisualStyleBackColor = true;
      this.btAbort.Click += new System.EventHandler(this.btAbort_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(129, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(137, 16);
      this.label1.TabIndex = 3;
      this.label1.Text = "Maximum Squad Size";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbSquadSize
      // 
      this.lbSquadSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbSquadSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbSquadSize.Location = new System.Drawing.Point(271, 3);
      this.lbSquadSize.Name = "lbSquadSize";
      this.lbSquadSize.Size = new System.Drawing.Size(48, 23);
      this.lbSquadSize.TabIndex = 4;
      this.lbSquadSize.Text = "88";
      this.lbSquadSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // ChooseSoldiers
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(448, 448);
      this.Controls.Add(this.lbSquadSize);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btAbort);
      this.Controls.Add(this.btLaunch);
      this.Controls.Add(this.dgSoldiers);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChooseSoldiers";
      this.Text = "ChooseSoldiers";
      this.TopMost = true;
      ((System.ComponentModel.ISupportInitialize)(this.dgSoldiers)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

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
  }
}