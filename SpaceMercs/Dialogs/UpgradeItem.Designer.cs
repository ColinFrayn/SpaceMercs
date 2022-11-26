namespace SpaceMercs.Dialogs {
  partial class UpgradeItem {
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
      this.lbName = new System.Windows.Forms.Label();
      this.lbQuality = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.lbNewQuality = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lbCost = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.lbChance = new System.Windows.Forms.Label();
      this.btUpgrade = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.lbSoldier = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lbName
      // 
      this.lbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbName.Location = new System.Drawing.Point(23, 12);
      this.lbName.Name = "lbName";
      this.lbName.Size = new System.Drawing.Size(216, 28);
      this.lbName.TabIndex = 0;
      this.lbName.Text = "Name Of This Item";
      this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbQuality
      // 
      this.lbQuality.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbQuality.Location = new System.Drawing.Point(47, 82);
      this.lbQuality.Name = "lbQuality";
      this.lbQuality.Size = new System.Drawing.Size(169, 22);
      this.lbQuality.TabIndex = 1;
      this.lbQuality.Text = "Quality String";
      this.lbQuality.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(100, 64);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(62, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Item Quality";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(100, 121);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Upgrade To";
      // 
      // lbNewQuality
      // 
      this.lbNewQuality.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbNewQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbNewQuality.Location = new System.Drawing.Point(47, 139);
      this.lbNewQuality.Name = "lbNewQuality";
      this.lbNewQuality.Size = new System.Drawing.Size(169, 22);
      this.lbNewQuality.TabIndex = 3;
      this.lbNewQuality.Text = "Quality String";
      this.lbNewQuality.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(57, 199);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(28, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Cost";
      // 
      // lbCost
      // 
      this.lbCost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCost.Location = new System.Drawing.Point(35, 216);
      this.lbCost.Name = "lbCost";
      this.lbCost.Size = new System.Drawing.Size(73, 30);
      this.lbCost.TabIndex = 5;
      this.lbCost.Text = "8888.88";
      this.lbCost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(151, 199);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(88, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Success Chance";
      // 
      // lbChance
      // 
      this.lbChance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbChance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbChance.Location = new System.Drawing.Point(158, 216);
      this.lbChance.Name = "lbChance";
      this.lbChance.Size = new System.Drawing.Size(73, 30);
      this.lbChance.TabIndex = 7;
      this.lbChance.Text = "88%";
      this.lbChance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btUpgrade
      // 
      this.btUpgrade.Location = new System.Drawing.Point(28, 258);
      this.btUpgrade.Name = "btUpgrade";
      this.btUpgrade.Size = new System.Drawing.Size(81, 27);
      this.btUpgrade.TabIndex = 9;
      this.btUpgrade.Text = "Upgrade";
      this.btUpgrade.UseVisualStyleBackColor = true;
      this.btUpgrade.Click += new System.EventHandler(this.btUpgrade_Click);
      // 
      // btCancel
      // 
      this.btCancel.Location = new System.Drawing.Point(157, 257);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(81, 27);
      this.btCancel.TabIndex = 10;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // lbSoldier
      // 
      this.lbSoldier.Location = new System.Drawing.Point(15, 169);
      this.lbSoldier.Name = "lbSoldier";
      this.lbSoldier.Size = new System.Drawing.Size(235, 20);
      this.lbSoldier.TabIndex = 11;
      this.lbSoldier.Text = "Most Skilled Soldier Name and Skill";
      this.lbSoldier.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // UpgradeItem
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(263, 294);
      this.Controls.Add(this.lbSoldier);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btUpgrade);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.lbChance);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.lbCost);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lbNewQuality);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.lbQuality);
      this.Controls.Add(this.lbName);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UpgradeItem";
      this.Text = "UpgradeItem";
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lbName;
    private System.Windows.Forms.Label lbQuality;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbNewQuality;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lbCost;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label lbChance;
    private System.Windows.Forms.Button btUpgrade;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Label lbSoldier;
  }
}