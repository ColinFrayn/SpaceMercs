namespace SpaceMercs.Dialogs {
    partial class ModifyWeapon {
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
            lbName = new Label();
            lbQuality = new Label();
            label2 = new Label();
            label1 = new Label();
            lbCurrentMod = new Label();
            label3 = new Label();
            lbCost = new Label();
            label5 = new Label();
            lbChance = new Label();
            btModify = new Button();
            btCancel = new Button();
            lbSoldier2 = new Label();
            lbSoldier1 = new Label();
            label4 = new Label();
            cbModType = new ComboBox();
            lbDescription = new Label();
            SuspendLayout();
            // 
            // lbName
            // 
            lbName.BorderStyle = BorderStyle.FixedSingle;
            lbName.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbName.Location = new Point(27, 14);
            lbName.Margin = new Padding(4, 0, 4, 0);
            lbName.Name = "lbName";
            lbName.Size = new Size(252, 32);
            lbName.TabIndex = 0;
            lbName.Text = "Name Of This Weapon";
            lbName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbQuality
            // 
            lbQuality.BorderStyle = BorderStyle.FixedSingle;
            lbQuality.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbQuality.Location = new Point(55, 73);
            lbQuality.Margin = new Padding(4, 0, 4, 0);
            lbQuality.Name = "lbQuality";
            lbQuality.Size = new Size(197, 25);
            lbQuality.TabIndex = 1;
            lbQuality.Text = "Quality String";
            lbQuality.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(117, 52);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(72, 15);
            label2.TabIndex = 2;
            label2.Text = "Item Quality";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(116, 110);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(75, 15);
            label1.TabIndex = 4;
            label1.Text = "Current Mod";
            // 
            // lbCurrentMod
            // 
            lbCurrentMod.BorderStyle = BorderStyle.FixedSingle;
            lbCurrentMod.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbCurrentMod.Location = new Point(55, 130);
            lbCurrentMod.Margin = new Padding(4, 0, 4, 0);
            lbCurrentMod.Name = "lbCurrentMod";
            lbCurrentMod.Size = new Size(197, 25);
            lbCurrentMod.TabIndex = 3;
            lbCurrentMod.Text = "Quality String";
            lbCurrentMod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(70, 370);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 6;
            label3.Text = "Cost";
            // 
            // lbCost
            // 
            lbCost.BorderStyle = BorderStyle.FixedSingle;
            lbCost.Font = new Font("Microsoft Sans Serif", 12F);
            lbCost.Location = new Point(45, 389);
            lbCost.Margin = new Padding(4, 0, 4, 0);
            lbCost.Name = "lbCost";
            lbCost.Size = new Size(85, 34);
            lbCost.TabIndex = 5;
            lbCost.Text = "8888.88";
            lbCost.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(180, 370);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(91, 15);
            label5.TabIndex = 8;
            label5.Text = "Success Chance";
            // 
            // lbChance
            // 
            lbChance.BorderStyle = BorderStyle.FixedSingle;
            lbChance.Font = new Font("Microsoft Sans Serif", 12F);
            lbChance.Location = new Point(181, 389);
            lbChance.Margin = new Padding(4, 0, 4, 0);
            lbChance.Name = "lbChance";
            lbChance.Size = new Size(85, 34);
            lbChance.TabIndex = 7;
            lbChance.Text = "88%";
            lbChance.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btModify
            // 
            btModify.Location = new Point(37, 435);
            btModify.Margin = new Padding(4, 3, 4, 3);
            btModify.Name = "btModify";
            btModify.Size = new Size(94, 31);
            btModify.TabIndex = 9;
            btModify.Text = "Modify";
            btModify.UseVisualStyleBackColor = true;
            btModify.Click += btModify_Click;
            // 
            // btCancel
            // 
            btCancel.Location = new Point(180, 434);
            btCancel.Margin = new Padding(4, 3, 4, 3);
            btCancel.Name = "btCancel";
            btCancel.Size = new Size(94, 31);
            btCancel.TabIndex = 10;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = true;
            btCancel.Click += btCancel_Click;
            // 
            // lbSoldier2
            // 
            lbSoldier2.Location = new Point(18, 340);
            lbSoldier2.Margin = new Padding(4, 0, 4, 0);
            lbSoldier2.Name = "lbSoldier2";
            lbSoldier2.Size = new Size(274, 23);
            lbSoldier2.TabIndex = 11;
            lbSoldier2.Text = "Most Skilled Soldier Name and Skill";
            lbSoldier2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbSoldier1
            // 
            lbSoldier1.Location = new Point(18, 317);
            lbSoldier1.Margin = new Padding(4, 0, 4, 0);
            lbSoldier1.Name = "lbSoldier1";
            lbSoldier1.Size = new Size(274, 23);
            lbSoldier1.TabIndex = 12;
            lbSoldier1.Text = "Most Skilled Soldier Name and Skill";
            lbSoldier1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(124, 167);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(59, 15);
            label4.TabIndex = 14;
            label4.Text = "New Mod";
            // 
            // cbModType
            // 
            cbModType.FormattingEnabled = true;
            cbModType.Location = new Point(64, 185);
            cbModType.Margin = new Padding(4, 3, 4, 3);
            cbModType.Name = "cbModType";
            cbModType.Size = new Size(178, 23);
            cbModType.TabIndex = 15;
            cbModType.SelectedIndexChanged += cbModType_SelectedIndexChanged;
            // 
            // lbDescription
            // 
            lbDescription.BorderStyle = BorderStyle.FixedSingle;
            lbDescription.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbDescription.Location = new Point(27, 221);
            lbDescription.Margin = new Padding(4, 0, 4, 0);
            lbDescription.Name = "lbDescription";
            lbDescription.Size = new Size(252, 85);
            lbDescription.TabIndex = 16;
            lbDescription.Text = "Description Here";
            lbDescription.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ModifyWeapon
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(307, 478);
            Controls.Add(lbDescription);
            Controls.Add(cbModType);
            Controls.Add(label4);
            Controls.Add(lbSoldier1);
            Controls.Add(lbSoldier2);
            Controls.Add(btCancel);
            Controls.Add(btModify);
            Controls.Add(label5);
            Controls.Add(lbChance);
            Controls.Add(label3);
            Controls.Add(lbCost);
            Controls.Add(label1);
            Controls.Add(lbCurrentMod);
            Controls.Add(label2);
            Controls.Add(lbQuality);
            Controls.Add(lbName);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ModifyWeapon";
            Text = "ModifyWeapon";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbName;
        private Label lbQuality;
        private Label label2;
        private Label label1;
        private Label lbCurrentMod;
        private Label label3;
        private Label lbCost;
        private Label label5;
        private Label lbChance;
        private Button btModify;
        private Button btCancel;
        private Label lbSoldier2;
        private Label lbSoldier1;
        private Label label4;
        private ComboBox cbModType;
        private Label lbDescription;
    }
}