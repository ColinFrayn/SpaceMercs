namespace SpaceMercs.Dialogs {
  partial class ChooseStat {
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
      this.label1 = new System.Windows.Forms.Label();
      this.btStrength = new System.Windows.Forms.Button();
      this.btAgility = new System.Windows.Forms.Button();
      this.btIntelligence = new System.Windows.Forms.Button();
      this.btToughness = new System.Windows.Forms.Button();
      this.btEndurance = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(12, 211);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(203, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Please choose a stat to increase";
      // 
      // btStrength
      // 
      this.btStrength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btStrength.Location = new System.Drawing.Point(9, 5);
      this.btStrength.Name = "btStrength";
      this.btStrength.Size = new System.Drawing.Size(93, 61);
      this.btStrength.TabIndex = 1;
      this.btStrength.Text = "Strength";
      this.btStrength.UseVisualStyleBackColor = true;
      this.btStrength.Click += new System.EventHandler(this.btStrength_Click);
      // 
      // btAgility
      // 
      this.btAgility.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAgility.Location = new System.Drawing.Point(121, 5);
      this.btAgility.Name = "btAgility";
      this.btAgility.Size = new System.Drawing.Size(94, 61);
      this.btAgility.TabIndex = 2;
      this.btAgility.Text = "Agility";
      this.btAgility.UseVisualStyleBackColor = true;
      this.btAgility.Click += new System.EventHandler(this.btAgility_Click);
      // 
      // btIntelligence
      // 
      this.btIntelligence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btIntelligence.Location = new System.Drawing.Point(9, 72);
      this.btIntelligence.Name = "btIntelligence";
      this.btIntelligence.Size = new System.Drawing.Size(93, 61);
      this.btIntelligence.TabIndex = 3;
      this.btIntelligence.Text = "Intelligence";
      this.btIntelligence.UseVisualStyleBackColor = true;
      this.btIntelligence.Click += new System.EventHandler(this.btIntelligence_Click);
      // 
      // btToughness
      // 
      this.btToughness.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btToughness.Location = new System.Drawing.Point(121, 72);
      this.btToughness.Name = "btToughness";
      this.btToughness.Size = new System.Drawing.Size(94, 61);
      this.btToughness.TabIndex = 4;
      this.btToughness.Text = "Toughness";
      this.btToughness.UseVisualStyleBackColor = true;
      this.btToughness.Click += new System.EventHandler(this.btToughness_Click);
      // 
      // btEndurance
      // 
      this.btEndurance.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btEndurance.Location = new System.Drawing.Point(66, 139);
      this.btEndurance.Name = "btEndurance";
      this.btEndurance.Size = new System.Drawing.Size(93, 61);
      this.btEndurance.TabIndex = 5;
      this.btEndurance.Text = "Endurance";
      this.btEndurance.UseVisualStyleBackColor = true;
      this.btEndurance.Click += new System.EventHandler(this.btEndurance_Click);
      // 
      // ChooseStat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(224, 236);
      this.ControlBox = false;
      this.Controls.Add(this.btEndurance);
      this.Controls.Add(this.btToughness);
      this.Controls.Add(this.btIntelligence);
      this.Controls.Add(this.btAgility);
      this.Controls.Add(this.btStrength);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChooseStat";
      this.Text = "ChooseStat";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btStrength;
    private System.Windows.Forms.Button btAgility;
    private System.Windows.Forms.Button btIntelligence;
    private System.Windows.Forms.Button btToughness;
    private System.Windows.Forms.Button btEndurance;
  }
}