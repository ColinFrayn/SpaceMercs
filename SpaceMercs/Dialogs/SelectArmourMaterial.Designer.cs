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
            lbArmourType.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbArmourType.Location = new Point(83, 10);
            lbArmourType.Margin = new Padding(4, 0, 4, 0);
            lbArmourType.Name = "lbArmourType";
            lbArmourType.Size = new Size(246, 32);
            lbArmourType.TabIndex = 1;
            lbArmourType.Text = "Armour Type";
            lbArmourType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            btCancel.Location = new Point(234, 319);
            btCancel.Margin = new Padding(4, 3, 4, 3);
            btCancel.Name = "btCancel";
            btCancel.Size = new Size(94, 31);
            btCancel.TabIndex = 14;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = true;
            btCancel.Click += btCancel_Click;
            // 
            // btSelect
            // 
            btSelect.Location = new Point(84, 320);
            btSelect.Margin = new Padding(4, 3, 4, 3);
            btSelect.Name = "btSelect";
            btSelect.Size = new Size(94, 31);
            btSelect.TabIndex = 13;
            btSelect.Text = "Select";
            btSelect.UseVisualStyleBackColor = true;
            btSelect.Click += btSelect_Click;
            // 
            // cbMaterialType
            // 
            cbMaterialType.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            cbMaterialType.FormattingEnabled = true;
            cbMaterialType.Location = new Point(84, 58);
            cbMaterialType.Margin = new Padding(4, 3, 4, 3);
            cbMaterialType.Name = "cbMaterialType";
            cbMaterialType.Size = new Size(244, 24);
            cbMaterialType.TabIndex = 15;
            cbMaterialType.SelectedIndexChanged += cbMaterialType_SelectedIndexChanged;
            // 
            // lbMassLabel
            // 
            lbMassLabel.AutoSize = true;
            lbMassLabel.Location = new Point(114, 108);
            lbMassLabel.Margin = new Padding(4, 0, 4, 0);
            lbMassLabel.Name = "lbMassLabel";
            lbMassLabel.Size = new Size(82, 15);
            lbMassLabel.TabIndex = 17;
            lbMassLabel.Text = "Mass Modifier";
            // 
            // lbMass
            // 
            lbMass.BorderStyle = BorderStyle.FixedSingle;
            lbMass.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbMass.Location = new Point(201, 96);
            lbMass.Margin = new Padding(4, 0, 4, 0);
            lbMass.Name = "lbMass";
            lbMass.Size = new Size(85, 34);
            lbMass.TabIndex = 16;
            lbMass.Text = "8888.88";
            lbMass.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbArmourLabel
            // 
            lbArmourLabel.AutoSize = true;
            lbArmourLabel.Location = new Point(100, 152);
            lbArmourLabel.Margin = new Padding(4, 0, 4, 0);
            lbArmourLabel.Name = "lbArmourLabel";
            lbArmourLabel.Size = new Size(96, 15);
            lbArmourLabel.TabIndex = 19;
            lbArmourLabel.Text = "Armour Modifier";
            // 
            // lbArmour
            // 
            lbArmour.BorderStyle = BorderStyle.FixedSingle;
            lbArmour.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbArmour.Location = new Point(201, 140);
            lbArmour.Margin = new Padding(4, 0, 4, 0);
            lbArmour.Name = "lbArmour";
            lbArmour.Size = new Size(85, 34);
            lbArmour.TabIndex = 18;
            lbArmour.Text = "8888.88";
            lbArmour.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbProtection
            // 
            lbProtection.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbProtection.FormattingEnabled = true;
            lbProtection.ItemHeight = 16;
            lbProtection.Location = new Point(50, 230);
            lbProtection.Margin = new Padding(4, 3, 4, 3);
            lbProtection.Name = "lbProtection";
            lbProtection.Size = new Size(313, 68);
            lbProtection.TabIndex = 20;
            // 
            // lbDiffLabel
            // 
            lbDiffLabel.AutoSize = true;
            lbDiffLabel.Location = new Point(141, 195);
            lbDiffLabel.Margin = new Padding(4, 0, 4, 0);
            lbDiffLabel.Name = "lbDiffLabel";
            lbDiffLabel.Size = new Size(55, 15);
            lbDiffLabel.TabIndex = 22;
            lbDiffLabel.Text = "Difficulty";
            // 
            // lbDiff
            // 
            lbDiff.BorderStyle = BorderStyle.FixedSingle;
            lbDiff.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbDiff.Location = new Point(201, 184);
            lbDiff.Margin = new Padding(4, 0, 4, 0);
            lbDiff.Name = "lbDiff";
            lbDiff.Size = new Size(85, 34);
            lbDiff.TabIndex = 21;
            lbDiff.Text = "8888.88";
            lbDiff.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SelectArmourMaterial
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(412, 369);
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
            Margin = new Padding(4, 3, 4, 3);
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