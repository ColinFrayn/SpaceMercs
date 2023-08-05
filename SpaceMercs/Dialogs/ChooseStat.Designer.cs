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
            label1 = new Label();
            btStrength = new Button();
            btAgility = new Button();
            btIntelligence = new Button();
            btToughness = new Button();
            btEndurance = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(34, 243);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(202, 16);
            label1.TabIndex = 0;
            label1.Text = "Please choose a stat to increase";
            // 
            // btStrength
            // 
            btStrength.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btStrength.Location = new Point(10, 6);
            btStrength.Margin = new Padding(4, 3, 4, 3);
            btStrength.Name = "btStrength";
            btStrength.Size = new Size(108, 70);
            btStrength.TabIndex = 1;
            btStrength.Text = "Strength";
            btStrength.UseVisualStyleBackColor = true;
            btStrength.Click += btStrength_Click;
            // 
            // btAgility
            // 
            btAgility.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btAgility.Location = new Point(141, 6);
            btAgility.Margin = new Padding(4, 3, 4, 3);
            btAgility.Name = "btAgility";
            btAgility.Size = new Size(110, 70);
            btAgility.TabIndex = 2;
            btAgility.Text = "Agility";
            btAgility.UseVisualStyleBackColor = true;
            btAgility.Click += btAgility_Click;
            // 
            // btIntelligence
            // 
            btIntelligence.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btIntelligence.Location = new Point(10, 83);
            btIntelligence.Margin = new Padding(4, 3, 4, 3);
            btIntelligence.Name = "btIntelligence";
            btIntelligence.Size = new Size(108, 70);
            btIntelligence.TabIndex = 3;
            btIntelligence.Text = "Intelligence";
            btIntelligence.UseVisualStyleBackColor = true;
            btIntelligence.Click += btIntelligence_Click;
            // 
            // btToughness
            // 
            btToughness.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btToughness.Location = new Point(141, 83);
            btToughness.Margin = new Padding(4, 3, 4, 3);
            btToughness.Name = "btToughness";
            btToughness.Size = new Size(110, 70);
            btToughness.TabIndex = 4;
            btToughness.Text = "Toughness";
            btToughness.UseVisualStyleBackColor = true;
            btToughness.Click += btToughness_Click;
            // 
            // btEndurance
            // 
            btEndurance.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btEndurance.Location = new Point(77, 160);
            btEndurance.Margin = new Padding(4, 3, 4, 3);
            btEndurance.Name = "btEndurance";
            btEndurance.Size = new Size(108, 70);
            btEndurance.TabIndex = 5;
            btEndurance.Text = "Endurance";
            btEndurance.UseVisualStyleBackColor = true;
            btEndurance.Click += btEndurance_Click;
            // 
            // ChooseStat
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(261, 272);
            ControlBox = false;
            Controls.Add(btEndurance);
            Controls.Add(btToughness);
            Controls.Add(btIntelligence);
            Controls.Add(btAgility);
            Controls.Add(btStrength);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseStat";
            Text = "ChooseStat";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btStrength;
        private Button btAgility;
        private Button btIntelligence;
        private Button btToughness;
        private Button btEndurance;
    }
}