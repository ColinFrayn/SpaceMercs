namespace SpaceMercs.Dialogs {
    partial class RaceView {
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
            cbRace = new ComboBox();
            lbColourLabel = new Label();
            pbColour = new PictureBox();
            lbRelationsLabel = new Label();
            lbRelations = new Label();
            lbSystemsLabel = new Label();
            lbSystems = new Label();
            lbColoniesLabel = new Label();
            lbColonies = new Label();
            lbHome = new Label();
            lbHomeLabel = new Label();
            tbDescription = new TextBox();
            lbRace = new Label();
            lbPopulation = new Label();
            lbPopulationLabel = new Label();
            lbExp = new Label();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pbColour).BeginInit();
            SuspendLayout();
            // 
            // cbRace
            // 
            cbRace.Font = new Font("Microsoft Sans Serif", 12F);
            cbRace.FormattingEnabled = true;
            cbRace.Location = new Point(133, 23);
            cbRace.Margin = new Padding(6, 5, 6, 5);
            cbRace.Name = "cbRace";
            cbRace.Size = new Size(474, 37);
            cbRace.TabIndex = 0;
            cbRace.SelectedIndexChanged += cbRace_SelectedIndexChanged;
            // 
            // lbColourLabel
            // 
            lbColourLabel.AutoSize = true;
            lbColourLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbColourLabel.Location = new Point(740, 35);
            lbColourLabel.Margin = new Padding(6, 0, 6, 0);
            lbColourLabel.Name = "lbColourLabel";
            lbColourLabel.Size = new Size(70, 25);
            lbColourLabel.TabIndex = 1;
            lbColourLabel.Text = "Colour";
            // 
            // pbColour
            // 
            pbColour.Location = new Point(826, 23);
            pbColour.Margin = new Padding(6, 5, 6, 5);
            pbColour.Name = "pbColour";
            pbColour.Size = new Size(147, 52);
            pbColour.TabIndex = 2;
            pbColour.TabStop = false;
            // 
            // lbRelationsLabel
            // 
            lbRelationsLabel.AutoSize = true;
            lbRelationsLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbRelationsLabel.Location = new Point(11, 110);
            lbRelationsLabel.Margin = new Padding(6, 0, 6, 0);
            lbRelationsLabel.Name = "lbRelationsLabel";
            lbRelationsLabel.Size = new Size(92, 25);
            lbRelationsLabel.TabIndex = 3;
            lbRelationsLabel.Text = "Relations";
            // 
            // lbRelations
            // 
            lbRelations.BorderStyle = BorderStyle.FixedSingle;
            lbRelations.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbRelations.Location = new Point(133, 103);
            lbRelations.Margin = new Padding(9, 3, 9, 3);
            lbRelations.Name = "lbRelations";
            lbRelations.Size = new Size(211, 40);
            lbRelations.TabIndex = 4;
            lbRelations.Text = "Fairly Good";
            lbRelations.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbSystemsLabel
            // 
            lbSystemsLabel.AutoSize = true;
            lbSystemsLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbSystemsLabel.Location = new Point(20, 175);
            lbSystemsLabel.Margin = new Padding(6, 0, 6, 0);
            lbSystemsLabel.Name = "lbSystemsLabel";
            lbSystemsLabel.Size = new Size(88, 25);
            lbSystemsLabel.TabIndex = 5;
            lbSystemsLabel.Text = "Systems";
            // 
            // lbSystems
            // 
            lbSystems.BorderStyle = BorderStyle.FixedSingle;
            lbSystems.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbSystems.Location = new Point(133, 170);
            lbSystems.Margin = new Padding(9, 3, 9, 3);
            lbSystems.Name = "lbSystems";
            lbSystems.Size = new Size(69, 40);
            lbSystems.TabIndex = 6;
            lbSystems.Text = "88";
            lbSystems.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbColoniesLabel
            // 
            lbColoniesLabel.AutoSize = true;
            lbColoniesLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbColoniesLabel.Location = new Point(259, 175);
            lbColoniesLabel.Margin = new Padding(6, 0, 6, 0);
            lbColoniesLabel.Name = "lbColoniesLabel";
            lbColoniesLabel.Size = new Size(89, 25);
            lbColoniesLabel.TabIndex = 7;
            lbColoniesLabel.Text = "Colonies";
            // 
            // lbColonies
            // 
            lbColonies.BorderStyle = BorderStyle.FixedSingle;
            lbColonies.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbColonies.Location = new Point(371, 170);
            lbColonies.Margin = new Padding(9, 3, 9, 3);
            lbColonies.Name = "lbColonies";
            lbColonies.Size = new Size(69, 40);
            lbColonies.TabIndex = 8;
            lbColonies.Text = "88";
            lbColonies.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbHome
            // 
            lbHome.BorderStyle = BorderStyle.FixedSingle;
            lbHome.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbHome.Location = new Point(660, 103);
            lbHome.Margin = new Padding(9, 3, 9, 3);
            lbHome.Name = "lbHome";
            lbHome.Size = new Size(308, 40);
            lbHome.TabIndex = 10;
            lbHome.Text = "Location...";
            lbHome.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbHomeLabel
            // 
            lbHomeLabel.AutoSize = true;
            lbHomeLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbHomeLabel.Location = new Point(503, 110);
            lbHomeLabel.Margin = new Padding(6, 0, 6, 0);
            lbHomeLabel.Name = "lbHomeLabel";
            lbHomeLabel.Size = new Size(124, 25);
            lbHomeLabel.TabIndex = 9;
            lbHomeLabel.Text = "Home Planet";
            // 
            // tbDescription
            // 
            tbDescription.Font = new Font("Microsoft Sans Serif", 9.75F);
            tbDescription.Location = new Point(20, 238);
            tbDescription.Margin = new Padding(6, 5, 6, 5);
            tbDescription.Multiline = true;
            tbDescription.Name = "tbDescription";
            tbDescription.Size = new Size(944, 519);
            tbDescription.TabIndex = 11;
            // 
            // lbRace
            // 
            lbRace.AutoSize = true;
            lbRace.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbRace.Location = new Point(51, 35);
            lbRace.Margin = new Padding(6, 0, 6, 0);
            lbRace.Name = "lbRace";
            lbRace.Size = new Size(57, 25);
            lbRace.TabIndex = 12;
            lbRace.Text = "Race";
            // 
            // lbPopulation
            // 
            lbPopulation.BorderStyle = BorderStyle.FixedSingle;
            lbPopulation.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbPopulation.Location = new Point(660, 170);
            lbPopulation.Margin = new Padding(9, 3, 9, 3);
            lbPopulation.Name = "lbPopulation";
            lbPopulation.Size = new Size(69, 40);
            lbPopulation.TabIndex = 14;
            lbPopulation.Text = "88";
            lbPopulation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbPopulationLabel
            // 
            lbPopulationLabel.AutoSize = true;
            lbPopulationLabel.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbPopulationLabel.Location = new Point(547, 175);
            lbPopulationLabel.Margin = new Padding(6, 0, 6, 0);
            lbPopulationLabel.Name = "lbPopulationLabel";
            lbPopulationLabel.Size = new Size(104, 25);
            lbPopulationLabel.TabIndex = 13;
            lbPopulationLabel.Text = "Population";
            // 
            // lbExp
            // 
            lbExp.BorderStyle = BorderStyle.Fixed3D;
            lbExp.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbExp.Location = new Point(373, 103);
            lbExp.Margin = new Padding(9, 3, 9, 3);
            lbExp.Name = "lbExp";
            lbExp.Size = new Size(81, 42);
            lbExp.TabIndex = 15;
            lbExp.Text = "88.8%";
            lbExp.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F);
            label1.Location = new Point(348, 110);
            label1.Margin = new Padding(0);
            label1.Name = "label1";
            label1.Size = new Size(24, 25);
            label1.TabIndex = 16;
            label1.Text = "+";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RaceView
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(991, 775);
            Controls.Add(label1);
            Controls.Add(lbExp);
            Controls.Add(lbPopulation);
            Controls.Add(lbPopulationLabel);
            Controls.Add(lbRace);
            Controls.Add(tbDescription);
            Controls.Add(lbHome);
            Controls.Add(lbHomeLabel);
            Controls.Add(lbColonies);
            Controls.Add(lbColoniesLabel);
            Controls.Add(lbSystems);
            Controls.Add(lbSystemsLabel);
            Controls.Add(lbRelations);
            Controls.Add(lbRelationsLabel);
            Controls.Add(pbColour);
            Controls.Add(lbColourLabel);
            Controls.Add(cbRace);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(6, 5, 6, 5);
            Name = "RaceView";
            Text = "RaceView";
            ((System.ComponentModel.ISupportInitialize)pbColour).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cbRace;
        private Label lbColourLabel;
        private PictureBox pbColour;
        private Label lbRelationsLabel;
        private Label lbRelations;
        private Label lbSystemsLabel;
        private Label lbSystems;
        private Label lbColoniesLabel;
        private Label lbColonies;
        private Label lbHome;
        private Label lbHomeLabel;
        private TextBox tbDescription;
        private Label lbRace;
        private Label lbPopulation;
        private Label lbPopulationLabel;
        private Label lbExp;
        private Label label1;
    }
}