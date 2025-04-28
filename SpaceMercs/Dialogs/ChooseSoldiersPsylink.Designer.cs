namespace SpaceMercs.Dialogs {
  partial class ChooseSoldiersPsylink {
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
            btLaunch = new Button();
            btAbort = new Button();
            label1 = new Label();
            lbSquadSize = new Label();
            lbPsylink = new ListBox();
            btPsylink = new Button();
            label2 = new Label();
            lbPsylinkCapacity = new Label();
            Selected = new DataGridViewCheckBoxColumn();
            btPsylinked = new DataGridViewButtonColumn();
            dgcName = new DataGridViewTextBoxColumn();
            dgcRace = new DataGridViewTextBoxColumn();
            dgcLevel = new DataGridViewTextBoxColumn();
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
            dgSoldiers.Columns.AddRange(new DataGridViewColumn[] { Selected, btPsylinked, dgcName, dgcRace, dgcLevel });
            dgSoldiers.Location = new Point(0, 33);
            dgSoldiers.Margin = new Padding(4, 3, 4, 3);
            dgSoldiers.Name = "dgSoldiers";
            dgSoldiers.ReadOnly = true;
            dgSoldiers.RowHeadersVisible = false;
            dgSoldiers.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgSoldiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgSoldiers.Size = new Size(492, 402);
            dgSoldiers.TabIndex = 0;
            dgSoldiers.CellClick += dgSoldiers_CellClick;
            dgSoldiers.CellContentClick += dgSoldiers_CellContentClick;
            dgSoldiers.SelectionChanged += dgSoldiers_SelectionChanged;
            dgSoldiers.Click += dgSoldiers_Click;
            // 
            // btLaunch
            // 
            btLaunch.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btLaunch.Location = new Point(12, 458);
            btLaunch.Margin = new Padding(4, 3, 4, 3);
            btLaunch.Name = "btLaunch";
            btLaunch.Size = new Size(130, 50);
            btLaunch.TabIndex = 1;
            btLaunch.Text = "Launch";
            btLaunch.UseVisualStyleBackColor = true;
            btLaunch.Click += btLaunch_Click;
            // 
            // btAbort
            // 
            btAbort.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btAbort.Location = new Point(351, 458);
            btAbort.Margin = new Padding(4, 3, 4, 3);
            btAbort.Name = "btAbort";
            btAbort.Size = new Size(130, 50);
            btAbort.TabIndex = 2;
            btAbort.Text = "Abort";
            btAbort.UseVisualStyleBackColor = true;
            btAbort.Click += btAbort_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(147, 8);
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
            lbSquadSize.Location = new Point(291, 4);
            lbSquadSize.Margin = new Padding(4, 0, 4, 0);
            lbSquadSize.Name = "lbSquadSize";
            lbSquadSize.Size = new Size(56, 26);
            lbSquadSize.TabIndex = 4;
            lbSquadSize.Text = "88";
            lbSquadSize.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbPsylink
            // 
            lbPsylink.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbPsylink.FormattingEnabled = true;
            lbPsylink.Location = new Point(12, 530);
            lbPsylink.Name = "lbPsylink";
            lbPsylink.Size = new Size(321, 151);
            lbPsylink.TabIndex = 5;
            lbPsylink.Click += lbPsylink_Click;
            lbPsylink.SelectedIndexChanged += lbPsylink_SelectedIndexChanged;
            // 
            // btPsylink
            // 
            btPsylink.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btPsylink.Location = new Point(191, 463);
            btPsylink.Margin = new Padding(4, 3, 4, 3);
            btPsylink.Name = "btPsylink";
            btPsylink.Size = new Size(110, 40);
            btPsylink.TabIndex = 6;
            btPsylink.Text = "Psylink";
            btPsylink.UseVisualStyleBackColor = true;
            btPsylink.Click += btPsylink_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(363, 566);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(106, 16);
            label2.TabIndex = 7;
            label2.Text = "Psylink Capacity";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbPsylinkCapacity
            // 
            lbPsylinkCapacity.BorderStyle = BorderStyle.FixedSingle;
            lbPsylinkCapacity.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbPsylinkCapacity.Location = new Point(385, 595);
            lbPsylinkCapacity.Margin = new Padding(4, 0, 4, 0);
            lbPsylinkCapacity.Name = "lbPsylinkCapacity";
            lbPsylinkCapacity.Size = new Size(56, 39);
            lbPsylinkCapacity.TabIndex = 8;
            lbPsylinkCapacity.Text = "88";
            lbPsylinkCapacity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Selected
            // 
            Selected.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            Selected.HeaderText = "Selected";
            Selected.Name = "Selected";
            Selected.ReadOnly = true;
            Selected.Width = 57;
            // 
            // btPsylinked
            // 
            btPsylinked.HeaderText = "Psylinked";
            btPsylinked.Name = "btPsylinked";
            btPsylinked.ReadOnly = true;
            btPsylinked.Text = "Psylink";
            btPsylinked.Width = 65;
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
            // ChooseSoldiersPsylink
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(493, 692);
            Controls.Add(lbPsylinkCapacity);
            Controls.Add(label2);
            Controls.Add(btPsylink);
            Controls.Add(lbPsylink);
            Controls.Add(lbSquadSize);
            Controls.Add(label1);
            Controls.Add(btAbort);
            Controls.Add(btLaunch);
            Controls.Add(dgSoldiers);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseSoldiersPsylink";
            Text = "ChooseSoldiers";
            TopMost = true;
            FormClosing += ChooseSoldiersPsylink_FormClosing;
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
        private ListBox lbPsylink;
        private Button btPsylink;
        private Label label2;
        private Label lbPsylinkCapacity;
        private DataGridViewCheckBoxColumn Selected;
        private DataGridViewButtonColumn btPsylinked;
        private DataGridViewTextBoxColumn dgcName;
        private DataGridViewTextBoxColumn dgcRace;
        private DataGridViewTextBoxColumn dgcLevel;
    }
}