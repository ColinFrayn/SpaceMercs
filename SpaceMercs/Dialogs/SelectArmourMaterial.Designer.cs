namespace SpaceMercs.Dialogs {
    partial class SelectArmourMaterial {
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
            lbArmourType = new Label();
            btCancel = new Button();
            btSelect = new Button();
            cbMaterialType = new ComboBox();
            lbMassLabel = new Label();
            lbMass = new Label();
            lbArmourLabel = new Label();
            lbArmour = new Label();
            lbProtection = new ListBox();
            lbDiffLabel = new Label();
            lbDiff = new Label();
            SuspendLayout();
            // 
            // lbArmourType
            // 
            lbArmourType.BorderStyle = BorderStyle.FixedSingle;
            lbArmourType.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbArmourType.Location = new Point(119, 17);
            lbArmourType.Margin = new Padding(6, 0, 6, 0);
            lbArmourType.Name = "lbArmourType";
            lbArmourType.Size = new Size(351, 52);
            lbArmourType.TabIndex = 1;
            lbArmourType.Text = "Armour Type";
            lbArmourType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            btCancel.Location = new Point(334, 532);
            btCancel.Margin = new Padding(6, 5, 6, 5);
            btCancel.Name = "btCancel";
            btCancel.Size = new Size(134, 52);
            btCancel.TabIndex = 14;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = true;
            btCancel.Click += btCancel_Click;
            // 
            // btSelect
            // 
            btSelect.Location = new Point(120, 533);
            btSelect.Margin = new Padding(6, 5, 6, 5);
            btSelect.Name = "btSelect";
            btSelect.Size = new Size(134, 52);
            btSelect.TabIndex = 13;
            btSelect.Text = "Select";
            btSelect.UseVisualStyleBackColor = true;
            btSelect.Click += btSelect_Click;
            // 
            // cbMaterialType
            // 
            cbMaterialType.Font = new Font("Microsoft Sans Serif", 9.75F);
            cbMaterialType.FormattingEnabled = true;
            cbMaterialType.Location = new Point(120, 97);
            cbMaterialType.Margin = new Padding(6, 5, 6, 5);
            cbMaterialType.Name = "cbMaterialType";
            cbMaterialType.Size = new Size(347, 33);
            cbMaterialType.TabIndex = 15;
            cbMaterialType.SelectedIndexChanged += cbMaterialType_SelectedIndexChanged;
            // 
            // lbMassLabel
            // 
            lbMassLabel.AutoSize = true;
            lbMassLabel.Location = new Point(158, 180);
            lbMassLabel.Margin = new Padding(6, 0, 6, 0);
            lbMassLabel.Name = "lbMassLabel";
            lbMassLabel.Size = new Size(125, 25);
            lbMassLabel.TabIndex = 17;
            lbMassLabel.Text = "Mass Modifier";
            // 
            // lbMass
            // 
            lbMass.BorderStyle = BorderStyle.FixedSingle;
            lbMass.Font = new Font("Microsoft Sans Serif", 12F);
            lbMass.Location = new Point(287, 160);
            lbMass.Margin = new Padding(6, 0, 6, 0);
            lbMass.Name = "lbMass";
            lbMass.Size = new Size(121, 55);
            lbMass.TabIndex = 16;
            lbMass.Text = "8888.88";
            lbMass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbArmourLabel
            // 
            lbArmourLabel.AutoSize = true;
            lbArmourLabel.Location = new Point(138, 253);
            lbArmourLabel.Margin = new Padding(6, 0, 6, 0);
            lbArmourLabel.Name = "lbArmourLabel";
            lbArmourLabel.Size = new Size(145, 25);
            lbArmourLabel.TabIndex = 19;
            lbArmourLabel.Text = "Armour Modifier";
            // 
            // lbArmour
            // 
            lbArmour.BorderStyle = BorderStyle.FixedSingle;
            lbArmour.Font = new Font("Microsoft Sans Serif", 12F);
            lbArmour.Location = new Point(287, 233);
            lbArmour.Margin = new Padding(6, 0, 6, 0);
            lbArmour.Name = "lbArmour";
            lbArmour.Size = new Size(121, 55);
            lbArmour.TabIndex = 18;
            lbArmour.Text = "8888.88";
            lbArmour.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbProtection
            // 
            lbProtection.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbProtection.FormattingEnabled = true;
            lbProtection.Location = new Point(71, 383);
            lbProtection.Margin = new Padding(6, 5, 6, 5);
            lbProtection.Name = "lbProtection";
            lbProtection.Size = new Size(445, 104);
            lbProtection.TabIndex = 20;
            // 
            // lbDiffLabel
            // 
            lbDiffLabel.AutoSize = true;
            lbDiffLabel.Location = new Point(200, 325);
            lbDiffLabel.Margin = new Padding(6, 0, 6, 0);
            lbDiffLabel.Name = "lbDiffLabel";
            lbDiffLabel.Size = new Size(82, 25);
            lbDiffLabel.TabIndex = 22;
            lbDiffLabel.Text = "Difficulty";
            // 
            // lbDiff
            // 
            lbDiff.BorderStyle = BorderStyle.FixedSingle;
            lbDiff.Font = new Font("Microsoft Sans Serif", 12F);
            lbDiff.Location = new Point(287, 307);
            lbDiff.Margin = new Padding(6, 0, 6, 0);
            lbDiff.Name = "lbDiff";
            lbDiff.Size = new Size(121, 55);
            lbDiff.TabIndex = 21;
            lbDiff.Text = "8888.88";
            lbDiff.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SelectArmourMaterial
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(589, 615);
            Controls.Add(lbDiffLabel);
            Controls.Add(lbDiff);
            Controls.Add(lbProtection);
            Controls.Add(lbArmourLabel);
            Controls.Add(lbArmour);
            Controls.Add(lbMassLabel);
            Controls.Add(lbMass);
            Controls.Add(cbMaterialType);
            Controls.Add(btCancel);
            Controls.Add(btSelect);
            Controls.Add(lbArmourType);
            Margin = new Padding(6, 5, 6, 5);
            Name = "SelectArmourMaterial";
            Text = "Select Armour Material";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbArmourType;
        private Button btCancel;
        private Button btSelect;
        private ComboBox cbMaterialType;
        private Label lbMassLabel;
        private Label lbMass;
        private Label lbArmourLabel;
        private Label lbArmour;
        private ListBox lbProtection;
        private Label lbDiffLabel;
        private Label lbDiff;
    }
}