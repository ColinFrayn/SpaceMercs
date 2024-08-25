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
            lbSkills = new ListBox();
            btChoose = new Button();
            SuspendLayout();
            // 
            // lbSkills
            // 
            lbSkills.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbSkills.FormattingEnabled = true;
            lbSkills.ItemHeight = 16;
            lbSkills.Location = new Point(0, 0);
            lbSkills.Margin = new Padding(4, 3, 4, 3);
            lbSkills.Name = "lbSkills";
            lbSkills.Size = new Size(261, 308);
            lbSkills.TabIndex = 0;
            lbSkills.SelectedIndexChanged += lbSkills_SelectedIndexChanged;
            lbSkills.DoubleClick += lbSkills_DoubleClick;
            // 
            // btChoose
            // 
            btChoose.Location = new Point(63, 336);
            btChoose.Margin = new Padding(4, 3, 4, 3);
            btChoose.Name = "btChoose";
            btChoose.Size = new Size(136, 31);
            btChoose.TabIndex = 1;
            btChoose.Text = "Choose This";
            btChoose.UseVisualStyleBackColor = true;
            btChoose.Click += btChoose_Click;
            // 
            // ChooseNewUtilitySkill
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(261, 380);
            Controls.Add(btChoose);
            Controls.Add(lbSkills);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseNewUtilitySkill";
            Text = "ChooseNewUtilitySkill";
            ResumeLayout(false);
        }

        #endregion

        private ListBox lbSkills;
        private Button btChoose;
    }
}