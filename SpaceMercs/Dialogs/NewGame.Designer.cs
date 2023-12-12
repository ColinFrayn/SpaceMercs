namespace SpaceMercs.Dialogs {
    partial class NewGame {
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
            buttonGenerate = new Button();
            tbStarsPerSector = new TrackBar();
            label2 = new Label();
            tbPlanetDensity = new TrackBar();
            label3 = new Label();
            groupBox1 = new GroupBox();
            btEnduranceDown = new Button();
            btEnduranceUp = new Button();
            lbEndurance = new Label();
            btToughnessDown = new Button();
            btToughnessUp = new Button();
            lbToughness = new Label();
            btInsightDown = new Button();
            btInsightUp = new Button();
            lbInsight = new Label();
            btAgilityDown = new Button();
            btAgilityUp = new Button();
            lbAgility = new Label();
            btStrengthDown = new Button();
            btStrengthUp = new Button();
            lbStrength = new Label();
            label12 = new Label();
            lbUnspent = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            tbName = new TextBox();
            groupBox2 = new GroupBox();
            label11 = new Label();
            tbAlienCivSize = new TrackBar();
            tbCivSize = new TrackBar();
            label4 = new Label();
            label1 = new Label();
            tbSeed = new TextBox();
            ((System.ComponentModel.ISupportInitialize)tbStarsPerSector).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbPlanetDensity).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tbAlienCivSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tbCivSize).BeginInit();
            SuspendLayout();
            // 
            // buttonGenerate
            // 
            buttonGenerate.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            buttonGenerate.Location = new Point(415, 386);
            buttonGenerate.Margin = new Padding(4, 3, 4, 3);
            buttonGenerate.Name = "buttonGenerate";
            buttonGenerate.Size = new Size(167, 75);
            buttonGenerate.TabIndex = 0;
            buttonGenerate.Text = "Generate New Game";
            buttonGenerate.UseVisualStyleBackColor = true;
            buttonGenerate.Click += buttonGenerate_Click;
            // 
            // tbStarsPerSector
            // 
            tbStarsPerSector.AutoSize = false;
            tbStarsPerSector.Location = new Point(145, 83);
            tbStarsPerSector.Margin = new Padding(4, 3, 4, 3);
            tbStarsPerSector.Name = "tbStarsPerSector";
            tbStarsPerSector.Size = new Size(196, 44);
            tbStarsPerSector.TabIndex = 10;
            tbStarsPerSector.Value = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(44, 38);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(93, 16);
            label2.TabIndex = 13;
            label2.Text = "Planet Density";
            // 
            // tbPlanetDensity
            // 
            tbPlanetDensity.AutoSize = false;
            tbPlanetDensity.Location = new Point(145, 37);
            tbPlanetDensity.Margin = new Padding(4, 3, 4, 3);
            tbPlanetDensity.Name = "tbPlanetDensity";
            tbPlanetDensity.Size = new Size(196, 44);
            tbPlanetDensity.TabIndex = 12;
            tbPlanetDensity.Value = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(33, 84);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(104, 16);
            label3.TabIndex = 19;
            label3.Text = "Stars Per Sector";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btEnduranceDown);
            groupBox1.Controls.Add(btEnduranceUp);
            groupBox1.Controls.Add(lbEndurance);
            groupBox1.Controls.Add(btToughnessDown);
            groupBox1.Controls.Add(btToughnessUp);
            groupBox1.Controls.Add(lbToughness);
            groupBox1.Controls.Add(btInsightDown);
            groupBox1.Controls.Add(btInsightUp);
            groupBox1.Controls.Add(lbInsight);
            groupBox1.Controls.Add(btAgilityDown);
            groupBox1.Controls.Add(btAgilityUp);
            groupBox1.Controls.Add(lbAgility);
            groupBox1.Controls.Add(btStrengthDown);
            groupBox1.Controls.Add(btStrengthUp);
            groupBox1.Controls.Add(lbStrength);
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(lbUnspent);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(tbName);
            groupBox1.Location = new Point(14, 15);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(568, 208);
            groupBox1.TabIndex = 22;
            groupBox1.TabStop = false;
            groupBox1.Text = "Main Character";
            // 
            // btEnduranceDown
            // 
            btEnduranceDown.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btEnduranceDown.Location = new Point(383, 138);
            btEnduranceDown.Margin = new Padding(0);
            btEnduranceDown.Name = "btEnduranceDown";
            btEnduranceDown.Size = new Size(49, 27);
            btEnduranceDown.TabIndex = 23;
            btEnduranceDown.Text = "-";
            btEnduranceDown.UseVisualStyleBackColor = true;
            btEnduranceDown.Click += btEnduranceDown_Click;
            // 
            // btEnduranceUp
            // 
            btEnduranceUp.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btEnduranceUp.Location = new Point(384, 60);
            btEnduranceUp.Margin = new Padding(0);
            btEnduranceUp.Name = "btEnduranceUp";
            btEnduranceUp.Size = new Size(49, 27);
            btEnduranceUp.TabIndex = 22;
            btEnduranceUp.Text = "+";
            btEnduranceUp.UseVisualStyleBackColor = true;
            btEnduranceUp.Click += btEnduranceUp_Click;
            // 
            // lbEndurance
            // 
            lbEndurance.BorderStyle = BorderStyle.Fixed3D;
            lbEndurance.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbEndurance.Location = new Point(371, 90);
            lbEndurance.Margin = new Padding(4, 0, 4, 0);
            lbEndurance.Name = "lbEndurance";
            lbEndurance.Size = new Size(72, 46);
            lbEndurance.TabIndex = 21;
            lbEndurance.Text = "88";
            lbEndurance.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btToughnessDown
            // 
            btToughnessDown.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btToughnessDown.Location = new Point(295, 138);
            btToughnessDown.Margin = new Padding(0);
            btToughnessDown.Name = "btToughnessDown";
            btToughnessDown.Size = new Size(49, 27);
            btToughnessDown.TabIndex = 20;
            btToughnessDown.Text = "-";
            btToughnessDown.UseVisualStyleBackColor = true;
            btToughnessDown.Click += btTougnessDown_Click;
            // 
            // btToughnessUp
            // 
            btToughnessUp.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btToughnessUp.Location = new Point(296, 60);
            btToughnessUp.Margin = new Padding(0);
            btToughnessUp.Name = "btToughnessUp";
            btToughnessUp.Size = new Size(49, 27);
            btToughnessUp.TabIndex = 19;
            btToughnessUp.Text = "+";
            btToughnessUp.UseVisualStyleBackColor = true;
            btToughnessUp.Click += btToughnessUp_Click;
            // 
            // lbToughness
            // 
            lbToughness.BorderStyle = BorderStyle.Fixed3D;
            lbToughness.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbToughness.Location = new Point(284, 90);
            lbToughness.Margin = new Padding(4, 0, 4, 0);
            lbToughness.Name = "lbToughness";
            lbToughness.Size = new Size(72, 46);
            lbToughness.TabIndex = 18;
            lbToughness.Text = "88";
            lbToughness.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btInsightDown
            // 
            btInsightDown.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btInsightDown.Location = new Point(208, 138);
            btInsightDown.Margin = new Padding(0);
            btInsightDown.Name = "btInsightDown";
            btInsightDown.Size = new Size(49, 27);
            btInsightDown.TabIndex = 17;
            btInsightDown.Text = "-";
            btInsightDown.UseVisualStyleBackColor = true;
            btInsightDown.Click += btInsightDown_Click;
            // 
            // btInsightUp
            // 
            btInsightUp.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btInsightUp.Location = new Point(209, 60);
            btInsightUp.Margin = new Padding(0);
            btInsightUp.Name = "btInsightUp";
            btInsightUp.Size = new Size(49, 27);
            btInsightUp.TabIndex = 16;
            btInsightUp.Text = "+";
            btInsightUp.UseVisualStyleBackColor = true;
            btInsightUp.Click += btInsightUp_Click;
            // 
            // lbInsight
            // 
            lbInsight.BorderStyle = BorderStyle.Fixed3D;
            lbInsight.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbInsight.Location = new Point(196, 90);
            lbInsight.Margin = new Padding(4, 0, 4, 0);
            lbInsight.Name = "lbInsight";
            lbInsight.Size = new Size(72, 46);
            lbInsight.TabIndex = 15;
            lbInsight.Text = "88";
            lbInsight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btAgilityDown
            // 
            btAgilityDown.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btAgilityDown.Location = new Point(120, 138);
            btAgilityDown.Margin = new Padding(0);
            btAgilityDown.Name = "btAgilityDown";
            btAgilityDown.Size = new Size(49, 27);
            btAgilityDown.TabIndex = 14;
            btAgilityDown.Text = "-";
            btAgilityDown.UseVisualStyleBackColor = true;
            btAgilityDown.Click += btAgilityDown_Click;
            // 
            // btAgilityUp
            // 
            btAgilityUp.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btAgilityUp.Location = new Point(121, 60);
            btAgilityUp.Margin = new Padding(0);
            btAgilityUp.Name = "btAgilityUp";
            btAgilityUp.Size = new Size(49, 27);
            btAgilityUp.TabIndex = 13;
            btAgilityUp.Text = "+";
            btAgilityUp.UseVisualStyleBackColor = true;
            btAgilityUp.Click += btAgilityUp_Click;
            // 
            // lbAgility
            // 
            lbAgility.BorderStyle = BorderStyle.Fixed3D;
            lbAgility.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbAgility.Location = new Point(108, 90);
            lbAgility.Margin = new Padding(4, 0, 4, 0);
            lbAgility.Name = "lbAgility";
            lbAgility.Size = new Size(72, 46);
            lbAgility.TabIndex = 12;
            lbAgility.Text = "88";
            lbAgility.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btStrengthDown
            // 
            btStrengthDown.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btStrengthDown.Location = new Point(33, 138);
            btStrengthDown.Margin = new Padding(0);
            btStrengthDown.Name = "btStrengthDown";
            btStrengthDown.Size = new Size(49, 27);
            btStrengthDown.TabIndex = 11;
            btStrengthDown.Text = "-";
            btStrengthDown.UseVisualStyleBackColor = true;
            btStrengthDown.Click += btStrengthDown_Click;
            // 
            // btStrengthUp
            // 
            btStrengthUp.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btStrengthUp.Location = new Point(34, 60);
            btStrengthUp.Margin = new Padding(0);
            btStrengthUp.Name = "btStrengthUp";
            btStrengthUp.Size = new Size(49, 27);
            btStrengthUp.TabIndex = 10;
            btStrengthUp.Text = "+";
            btStrengthUp.UseVisualStyleBackColor = true;
            btStrengthUp.Click += btStrengthUp_Click;
            // 
            // lbStrength
            // 
            lbStrength.BorderStyle = BorderStyle.Fixed3D;
            lbStrength.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbStrength.Location = new Point(21, 90);
            lbStrength.Margin = new Padding(4, 0, 4, 0);
            lbStrength.Name = "lbStrength";
            lbStrength.Size = new Size(72, 46);
            lbStrength.TabIndex = 9;
            lbStrength.Text = "88";
            lbStrength.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.BorderStyle = BorderStyle.FixedSingle;
            label12.Location = new Point(472, 135);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(72, 20);
            label12.TabIndex = 8;
            label12.Text = "Unspent";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbUnspent
            // 
            lbUnspent.BorderStyle = BorderStyle.FixedSingle;
            lbUnspent.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbUnspent.Location = new Point(472, 90);
            lbUnspent.Margin = new Padding(4, 0, 4, 0);
            lbUnspent.Name = "lbUnspent";
            lbUnspent.Size = new Size(72, 46);
            lbUnspent.TabIndex = 7;
            lbUnspent.Text = "88";
            lbUnspent.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.BorderStyle = BorderStyle.FixedSingle;
            label10.Location = new Point(371, 170);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(72, 20);
            label10.TabIndex = 6;
            label10.Text = "Endurance";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.BorderStyle = BorderStyle.FixedSingle;
            label9.Location = new Point(284, 170);
            label9.Margin = new Padding(0);
            label9.Name = "label9";
            label9.Size = new Size(72, 20);
            label9.TabIndex = 5;
            label9.Text = "Toughness";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.BorderStyle = BorderStyle.FixedSingle;
            label8.Location = new Point(196, 170);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(72, 20);
            label8.TabIndex = 4;
            label8.Text = "Insight";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.BorderStyle = BorderStyle.FixedSingle;
            label7.Location = new Point(108, 170);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(72, 20);
            label7.TabIndex = 3;
            label7.Text = "Agility";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.BorderStyle = BorderStyle.FixedSingle;
            label6.Location = new Point(21, 170);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(72, 20);
            label6.TabIndex = 2;
            label6.Text = "Strength";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(21, 29);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(61, 15);
            label5.TabIndex = 1;
            label5.Text = "Full Name";
            // 
            // tbName
            // 
            tbName.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            tbName.Location = new Point(114, 23);
            tbName.Margin = new Padding(4, 3, 4, 3);
            tbName.Name = "tbName";
            tbName.Size = new Size(332, 22);
            tbName.TabIndex = 0;
            tbName.TextAlign = HorizontalAlignment.Center;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(tbAlienCivSize);
            groupBox2.Controls.Add(tbCivSize);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(tbStarsPerSector);
            groupBox2.Controls.Add(tbPlanetDensity);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(14, 230);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(380, 231);
            groupBox2.TabIndex = 23;
            groupBox2.TabStop = false;
            groupBox2.Text = "Galaxy Settings";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(33, 176);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(104, 16);
            label11.TabIndex = 23;
            label11.Text = "Alien Population";
            // 
            // tbAlienCivSize
            // 
            tbAlienCivSize.AutoSize = false;
            tbAlienCivSize.LargeChange = 5;
            tbAlienCivSize.Location = new Point(145, 175);
            tbAlienCivSize.Margin = new Padding(4, 3, 4, 3);
            tbAlienCivSize.Maximum = 25;
            tbAlienCivSize.Minimum = 5;
            tbAlienCivSize.Name = "tbAlienCivSize";
            tbAlienCivSize.Size = new Size(196, 44);
            tbAlienCivSize.TabIndex = 22;
            tbAlienCivSize.Value = 18;
            // 
            // tbCivSize
            // 
            tbCivSize.AutoSize = false;
            tbCivSize.LargeChange = 5;
            tbCivSize.Location = new Point(145, 129);
            tbCivSize.Margin = new Padding(4, 3, 4, 3);
            tbCivSize.Maximum = 25;
            tbCivSize.Minimum = 5;
            tbCivSize.Name = "tbCivSize";
            tbCivSize.Size = new Size(196, 44);
            tbCivSize.TabIndex = 18;
            tbCivSize.Value = 18;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(20, 130);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(117, 16);
            label4.TabIndex = 21;
            label4.Text = "Human Population";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(414, 248);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 25;
            label1.Text = "Seed";
            // 
            // tbSeed
            // 
            tbSeed.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            tbSeed.Location = new Point(458, 242);
            tbSeed.Margin = new Padding(4, 3, 4, 3);
            tbSeed.Name = "tbSeed";
            tbSeed.Size = new Size(123, 22);
            tbSeed.TabIndex = 24;
            tbSeed.TextAlign = HorizontalAlignment.Center;
            // 
            // NewGame
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(602, 473);
            Controls.Add(label1);
            Controls.Add(groupBox1);
            Controls.Add(tbSeed);
            Controls.Add(buttonGenerate);
            Controls.Add(groupBox2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "NewGame";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Generate a New Game";
            ((System.ComponentModel.ISupportInitialize)tbStarsPerSector).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbPlanetDensity).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tbAlienCivSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)tbCivSize).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGenerate;
        private TrackBar tbStarsPerSector;
        private Label label2;
        private TrackBar tbPlanetDensity;
        private Label label3;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private TextBox tbName;
        private Button btStrengthDown;
        private Button btStrengthUp;
        private Label lbStrength;
        private Label label12;
        private Label lbUnspent;
        private Button btEnduranceDown;
        private Button btEnduranceUp;
        private Label lbEndurance;
        private Button btToughnessDown;
        private Button btToughnessUp;
        private Label lbToughness;
        private Button btInsightDown;
        private Button btInsightUp;
        private Label lbInsight;
        private Button btAgilityDown;
        private Button btAgilityUp;
        private Label lbAgility;
        private Label label1;
        private TextBox tbSeed;
        private TrackBar tbCivSize;
        private Label label4;
        private Label label11;
        private TrackBar tbAlienCivSize;
    }
}