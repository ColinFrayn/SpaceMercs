namespace SpaceMercs.Dialogs {
    partial class HyperspaceTravel {
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
            btHyperspaceTravel = new Button();
            dgDestinations = new DataGridView();
            colSystem = new DataGridViewTextBoxColumn();
            colCoords = new DataGridViewTextBoxColumn();
            colDistance = new DataGridViewTextBoxColumn();
            colTime = new DataGridViewTextBoxColumn();
            colCost = new DataGridViewTextBoxColumn();
            pbTravel = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)dgDestinations).BeginInit();
            SuspendLayout();
            // 
            // btRunMission
            // 
            btHyperspaceTravel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btHyperspaceTravel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btHyperspaceTravel.Location = new Point(202, 448);
            btHyperspaceTravel.Margin = new Padding(4, 3, 4, 3);
            btHyperspaceTravel.Name = "btHyperspaceTravel";
            btHyperspaceTravel.Size = new Size(182, 38);
            btHyperspaceTravel.TabIndex = 6;
            btHyperspaceTravel.Text = "Hyperspace Travel";
            btHyperspaceTravel.UseVisualStyleBackColor = true;
            btHyperspaceTravel.Click += btHyperspaceTravel_Click;
            // 
            // dgDestinations
            // 
            dgDestinations.AllowUserToAddRows = false;
            dgDestinations.AllowUserToDeleteRows = false;
            dgDestinations.AllowUserToResizeRows = false;
            dgDestinations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgDestinations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgDestinations.Columns.AddRange(new DataGridViewColumn[] { colSystem, colCoords, colDistance, colTime, colCost });
            dgDestinations.Dock = DockStyle.Top;
            dgDestinations.Location = new Point(0, 0);
            dgDestinations.Margin = new Padding(4, 3, 4, 3);
            dgDestinations.MultiSelect = false;
            dgDestinations.Name = "dgDestinations";
            dgDestinations.ReadOnly = true;
            dgDestinations.RowHeadersVisible = false;
            dgDestinations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgDestinations.ShowEditingIcon = false;
            dgDestinations.Size = new Size(588, 434);
            dgDestinations.TabIndex = 5;
            dgDestinations.SelectionChanged += dgDestinations_SelectionChanged;
            // 
            // colSystem
            // 
            colSystem.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colSystem.DefaultCellStyle = dataGridViewCellStyle1;
            colSystem.HeaderText = "System";
            colSystem.Name = "colSystem";
            colSystem.ReadOnly = true;
            // 
            // colCoords
            // 
            colCoords.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            colCoords.DefaultCellStyle = dataGridViewCellStyle2;
            colCoords.HeaderText = "Coords";
            colCoords.Name = "colCoords";
            colCoords.ReadOnly = true;
            colCoords.Width = 70;
            // 
            // colDistance
            // 
            colDistance.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colDistance.DefaultCellStyle = dataGridViewCellStyle3;
            colDistance.HeaderText = "Distance";
            colDistance.Name = "colDistance";
            colDistance.ReadOnly = true;
            colDistance.Width = 77;
            // 
            // colCost
            // 
            colCost.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colCost.DefaultCellStyle = dataGridViewCellStyle4;
            colCost.HeaderText = "Cost";
            colCost.Name = "colCost";
            colCost.ReadOnly = true;
            colCost.Width = 56;
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colTime.DefaultCellStyle = dataGridViewCellStyle5;
            colTime.HeaderText = "Time";
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            colTime.Width = 58;
            // 
            // pbTravel
            // 
            pbTravel.Location = new Point(36, 310);
            pbTravel.Margin = new Padding(4, 3, 4, 3);
            pbTravel.Maximum = 24;
            pbTravel.Name = "pbTravel";
            pbTravel.Size = new Size(513, 59);
            pbTravel.Step = 1;
            pbTravel.TabIndex = 9;
            // 
            // HyperspaceTravel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 500);
            Controls.Add(pbTravel);
            Controls.Add(btHyperspaceTravel);
            Controls.Add(dgDestinations);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HyperspaceTravel";
            Text = "Hyperspace Travel";
            ((System.ComponentModel.ISupportInitialize)dgDestinations).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btHyperspaceTravel;
        private DataGridView dgDestinations;
        private DataGridViewTextBoxColumn colSystem;
        private DataGridViewTextBoxColumn colCoords;
        private DataGridViewTextBoxColumn colDistance;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colCost;
        private ProgressBar pbTravel;
    }
}