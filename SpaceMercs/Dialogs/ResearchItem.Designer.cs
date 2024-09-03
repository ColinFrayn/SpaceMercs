namespace SpaceMercs.Dialogs {
    partial class ResearchItem {
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
            btResearch = new Button();
            dgResearchItems = new DataGridView();
            pbResearch = new ProgressBar();
            colTech = new DataGridViewTextBoxColumn();
            colType = new DataGridViewTextBoxColumn();
            colMaterials = new DataGridViewTextBoxColumn();
            colCost = new DataGridViewTextBoxColumn();
            colTime = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgResearchItems).BeginInit();
            SuspendLayout();
            // 
            // btResearch
            // 
            btResearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btResearch.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btResearch.Location = new Point(202, 448);
            btResearch.Margin = new Padding(4, 3, 4, 3);
            btResearch.Name = "btResearch";
            btResearch.Size = new Size(182, 38);
            btResearch.TabIndex = 6;
            btResearch.Text = "Research";
            btResearch.UseVisualStyleBackColor = true;
            btResearch.Click += btResearch_Click;
            // 
            // dgResearchItems
            // 
            dgResearchItems.AllowUserToAddRows = false;
            dgResearchItems.AllowUserToDeleteRows = false;
            dgResearchItems.AllowUserToResizeRows = false;
            dgResearchItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgResearchItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgResearchItems.Columns.AddRange(new DataGridViewColumn[] { colTech, colType, colMaterials, colCost, colTime });
            dgResearchItems.Dock = DockStyle.Top;
            dgResearchItems.Location = new Point(0, 0);
            dgResearchItems.Margin = new Padding(4, 3, 4, 3);
            dgResearchItems.MultiSelect = false;
            dgResearchItems.Name = "dgResearchItems";
            dgResearchItems.ReadOnly = true;
            dgResearchItems.RowHeadersVisible = false;
            dgResearchItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgResearchItems.ShowEditingIcon = false;
            dgResearchItems.Size = new Size(588, 434);
            dgResearchItems.TabIndex = 5;
            dgResearchItems.SelectionChanged += dgResearchItems_SelectionChanged;
            dgResearchItems.DoubleClick += dgResearchItems_DoubleClick;
            // 
            // pbResearch
            // 
            pbResearch.Location = new Point(36, 310);
            pbResearch.Margin = new Padding(4, 3, 4, 3);
            pbResearch.Maximum = 24;
            pbResearch.Name = "pbResearch";
            pbResearch.Size = new Size(513, 59);
            pbResearch.Step = 1;
            pbResearch.TabIndex = 9;
            // 
            // colTech
            // 
            colTech.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colTech.DefaultCellStyle = dataGridViewCellStyle1;
            colTech.HeaderText = "Tech";
            colTech.Name = "colTech";
            colTech.ReadOnly = true;
            // 
            // colType
            // 
            colType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            colType.HeaderText = "Type";
            colType.Name = "colType";
            colType.ReadOnly = true;
            colType.Width = 56;
            // 
            // colMaterials
            // 
            colMaterials.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colMaterials.DefaultCellStyle = dataGridViewCellStyle2;
            colMaterials.HeaderText = "Materials";
            colMaterials.Name = "colMaterials";
            colMaterials.ReadOnly = true;
            colMaterials.Width = 80;
            // 
            // colCost
            // 
            colCost.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colCost.DefaultCellStyle = dataGridViewCellStyle3;
            colCost.HeaderText = "Cost";
            colCost.Name = "colCost";
            colCost.ReadOnly = true;
            colCost.Width = 56;
            // 
            // colTime
            // 
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            colTime.DefaultCellStyle = dataGridViewCellStyle4;
            colTime.HeaderText = "Time";
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            colTime.Width = 58;
            // 
            // ResearchItem
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(588, 500);
            Controls.Add(pbResearch);
            Controls.Add(btResearch);
            Controls.Add(dgResearchItems);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ResearchItem";
            Text = "Research New Technology";
            ((System.ComponentModel.ISupportInitialize)dgResearchItems).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btResearch;
        private DataGridView dgResearchItems;
        private ProgressBar pbResearch;
        private DataGridViewTextBoxColumn colTech;
        private DataGridViewTextBoxColumn colType;
        private DataGridViewTextBoxColumn colMaterials;
        private DataGridViewTextBoxColumn colCost;
        private DataGridViewTextBoxColumn colTime;
    }
}