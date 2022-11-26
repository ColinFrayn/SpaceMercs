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
      this.lbArmourType = new System.Windows.Forms.Label();
      this.btCancel = new System.Windows.Forms.Button();
      this.btSelect = new System.Windows.Forms.Button();
      this.lbValueLabel = new System.Windows.Forms.Label();
      this.lbValue = new System.Windows.Forms.Label();
      this.cbMaterialType = new System.Windows.Forms.ComboBox();
      this.lbMassLabel = new System.Windows.Forms.Label();
      this.lbMass = new System.Windows.Forms.Label();
      this.lbArmourLabel = new System.Windows.Forms.Label();
      this.lbArmour = new System.Windows.Forms.Label();
      this.lbProtection = new System.Windows.Forms.ListBox();
      this.lbDiffLabel = new System.Windows.Forms.Label();
      this.lbDiff = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lbArmourType
      // 
      this.lbArmourType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbArmourType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbArmourType.Location = new System.Drawing.Point(71, 9);
      this.lbArmourType.Name = "lbArmourType";
      this.lbArmourType.Size = new System.Drawing.Size(211, 28);
      this.lbArmourType.TabIndex = 1;
      this.lbArmourType.Text = "Armour Type";
      this.lbArmourType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btCancel
      // 
      this.btCancel.Location = new System.Drawing.Point(201, 318);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(81, 27);
      this.btCancel.TabIndex = 14;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // btSelect
      // 
      this.btSelect.Location = new System.Drawing.Point(72, 319);
      this.btSelect.Name = "btSelect";
      this.btSelect.Size = new System.Drawing.Size(81, 27);
      this.btSelect.TabIndex = 13;
      this.btSelect.Text = "Select";
      this.btSelect.UseVisualStyleBackColor = true;
      this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
      // 
      // lbValueLabel
      // 
      this.lbValueLabel.AutoSize = true;
      this.lbValueLabel.Location = new System.Drawing.Point(92, 98);
      this.lbValueLabel.Name = "lbValueLabel";
      this.lbValueLabel.Size = new System.Drawing.Size(74, 13);
      this.lbValueLabel.TabIndex = 12;
      this.lbValueLabel.Text = "Value Modifier";
      // 
      // lbValue
      // 
      this.lbValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbValue.Location = new System.Drawing.Point(172, 88);
      this.lbValue.Name = "lbValue";
      this.lbValue.Size = new System.Drawing.Size(73, 30);
      this.lbValue.TabIndex = 11;
      this.lbValue.Text = "8888.88";
      this.lbValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // cbMaterialType
      // 
      this.cbMaterialType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbMaterialType.FormattingEnabled = true;
      this.cbMaterialType.Location = new System.Drawing.Point(72, 53);
      this.cbMaterialType.Name = "cbMaterialType";
      this.cbMaterialType.Size = new System.Drawing.Size(210, 24);
      this.cbMaterialType.TabIndex = 15;
      this.cbMaterialType.SelectedIndexChanged += new System.EventHandler(this.cbMaterialType_SelectedIndexChanged);
      // 
      // lbMassLabel
      // 
      this.lbMassLabel.AutoSize = true;
      this.lbMassLabel.Location = new System.Drawing.Point(92, 135);
      this.lbMassLabel.Name = "lbMassLabel";
      this.lbMassLabel.Size = new System.Drawing.Size(72, 13);
      this.lbMassLabel.TabIndex = 17;
      this.lbMassLabel.Text = "Mass Modifier";
      // 
      // lbMass
      // 
      this.lbMass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbMass.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbMass.Location = new System.Drawing.Point(172, 125);
      this.lbMass.Name = "lbMass";
      this.lbMass.Size = new System.Drawing.Size(73, 30);
      this.lbMass.TabIndex = 16;
      this.lbMass.Text = "8888.88";
      this.lbMass.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbArmourLabel
      // 
      this.lbArmourLabel.AutoSize = true;
      this.lbArmourLabel.Location = new System.Drawing.Point(92, 173);
      this.lbArmourLabel.Name = "lbArmourLabel";
      this.lbArmourLabel.Size = new System.Drawing.Size(80, 13);
      this.lbArmourLabel.TabIndex = 19;
      this.lbArmourLabel.Text = "Armour Modifier";
      // 
      // lbArmour
      // 
      this.lbArmour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbArmour.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbArmour.Location = new System.Drawing.Point(172, 163);
      this.lbArmour.Name = "lbArmour";
      this.lbArmour.Size = new System.Drawing.Size(73, 30);
      this.lbArmour.TabIndex = 18;
      this.lbArmour.Text = "8888.88";
      this.lbArmour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbProtection
      // 
      this.lbProtection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbProtection.FormattingEnabled = true;
      this.lbProtection.ItemHeight = 16;
      this.lbProtection.Location = new System.Drawing.Point(43, 241);
      this.lbProtection.Name = "lbProtection";
      this.lbProtection.Size = new System.Drawing.Size(269, 68);
      this.lbProtection.TabIndex = 20;
      // 
      // lbDiffLabel
      // 
      this.lbDiffLabel.AutoSize = true;
      this.lbDiffLabel.Location = new System.Drawing.Point(92, 211);
      this.lbDiffLabel.Name = "lbDiffLabel";
      this.lbDiffLabel.Size = new System.Drawing.Size(47, 13);
      this.lbDiffLabel.TabIndex = 22;
      this.lbDiffLabel.Text = "Difficulty";
      // 
      // lbDiff
      // 
      this.lbDiff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbDiff.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbDiff.Location = new System.Drawing.Point(172, 201);
      this.lbDiff.Name = "lbDiff";
      this.lbDiff.Size = new System.Drawing.Size(73, 30);
      this.lbDiff.TabIndex = 21;
      this.lbDiff.Text = "8888.88";
      this.lbDiff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // SelectArmourMaterial
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(353, 357);
      this.Controls.Add(this.lbDiffLabel);
      this.Controls.Add(this.lbDiff);
      this.Controls.Add(this.lbProtection);
      this.Controls.Add(this.lbArmourLabel);
      this.Controls.Add(this.lbArmour);
      this.Controls.Add(this.lbMassLabel);
      this.Controls.Add(this.lbMass);
      this.Controls.Add(this.cbMaterialType);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btSelect);
      this.Controls.Add(this.lbValueLabel);
      this.Controls.Add(this.lbValue);
      this.Controls.Add(this.lbArmourType);
      this.Name = "SelectArmourMaterial";
      this.Text = "Select Armour Material";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lbArmourType;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btSelect;
    private System.Windows.Forms.Label lbValueLabel;
    private System.Windows.Forms.Label lbValue;
        private System.Windows.Forms.ComboBox cbMaterialType;
        private System.Windows.Forms.Label lbMassLabel;
        private System.Windows.Forms.Label lbMass;
        private System.Windows.Forms.Label lbArmourLabel;
        private System.Windows.Forms.Label lbArmour;
        private System.Windows.Forms.ListBox lbProtection;
        private System.Windows.Forms.Label lbDiffLabel;
        private System.Windows.Forms.Label lbDiff;
    }
}