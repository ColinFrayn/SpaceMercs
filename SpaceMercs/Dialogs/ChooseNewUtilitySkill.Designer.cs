namespace SpaceMercs.Dialogs {
  partial class ChooseNewUtilitySkill {
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
      this.lbSkills = new System.Windows.Forms.ListBox();
      this.btChoose = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lbSkills
      // 
      this.lbSkills.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbSkills.FormattingEnabled = true;
      this.lbSkills.ItemHeight = 16;
      this.lbSkills.Location = new System.Drawing.Point(0, 0);
      this.lbSkills.Name = "lbSkills";
      this.lbSkills.Size = new System.Drawing.Size(224, 276);
      this.lbSkills.TabIndex = 0;
      this.lbSkills.SelectedIndexChanged += new System.EventHandler(this.lbSkills_SelectedIndexChanged);
      // 
      // btChoose
      // 
      this.btChoose.Location = new System.Drawing.Point(54, 291);
      this.btChoose.Name = "btChoose";
      this.btChoose.Size = new System.Drawing.Size(117, 27);
      this.btChoose.TabIndex = 1;
      this.btChoose.Text = "Choose This";
      this.btChoose.UseVisualStyleBackColor = true;
      this.btChoose.Click += new System.EventHandler(this.btChoose_Click);
      // 
      // ChooseNewUtilitySkill
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(224, 329);
      this.Controls.Add(this.btChoose);
      this.Controls.Add(this.lbSkills);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChooseNewUtilitySkill";
      this.Text = "ChooseNewUtilitySkill";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox lbSkills;
    private System.Windows.Forms.Button btChoose;
  }
}