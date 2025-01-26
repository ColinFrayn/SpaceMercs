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
            ((System.ComponentModel.ISupportInitialize)pbColour).BeginInit();
            SuspendLayout();
            // 
            // cbRace
            // 
            cbRace.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            cbRace.FormattingEnabled = true;
            cbRace.Location = new Point(93, 14);
            cbRace.Margin = new Padding(4, 3, 4, 3);
            cbRace.Name = "cbRace";
            cbRace.Size = new Size(333, 28);
            cbRace.TabIndex = 0;
            cbRace.SelectedIndexChanged += cbRace_SelectedIndexChanged;
            // 
            // lbColourLabel
            // 
            lbColourLabel.AutoSize = true;
            lbColourLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbColourLabel.Location = new Point(518, 21);
            lbColourLabel.Margin = new Padding(4, 0, 4, 0);
            lbColourLabel.Name = "lbColourLabel";
            lbColourLabel.Size = new Size(46, 16);
            lbColourLabel.TabIndex = 1;
            lbColourLabel.Text = "Colour";
            // 
            // pbColour
            // 
            pbColour.Location = new Point(578, 14);
            pbColour.Margin = new Padding(4, 3, 4, 3);
            pbColour.Name = "pbColour";
            pbColour.Size = new Size(103, 31);
            pbColour.TabIndex = 2;
            pbColour.TabStop = false;
            // 
            // lbRelationsLabel
            // 
            lbRelationsLabel.AutoSize = true;
            lbRelationsLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbRelationsLabel.Location = new Point(8, 66);
            lbRelationsLabel.Margin = new Padding(4, 0, 4, 0);
            lbRelationsLabel.Name = "lbRelationsLabel";
            lbRelationsLabel.Size = new Size(64, 16);
            lbRelationsLabel.TabIndex = 3;
            lbRelationsLabel.Text = "Relations";
            // 
            // lbRelations
            // 
            lbRelations.BorderStyle = BorderStyle.FixedSingle;
            lbRelations.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbRelations.Location = new Point(93, 62);
            lbRelations.Margin = new Padding(6, 2, 6, 2);
            lbRelations.Name = "lbRelations";
            lbRelations.Size = new Size(148, 25);
            lbRelations.TabIndex = 4;
            lbRelations.Text = "Fairly Good";
            lbRelations.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbSystemsLabel
            // 
            lbSystemsLabel.AutoSize = true;
            lbSystemsLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbSystemsLabel.Location = new Point(14, 105);
            lbSystemsLabel.Margin = new Padding(4, 0, 4, 0);
            lbSystemsLabel.Name = "lbSystemsLabel";
            lbSystemsLabel.Size = new Size(59, 16);
            lbSystemsLabel.TabIndex = 5;
            lbSystemsLabel.Text = "Systems";
            // 
            // lbSystems
            // 
            lbSystems.BorderStyle = BorderStyle.FixedSingle;
            lbSystems.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbSystems.Location = new Point(93, 102);
            lbSystems.Margin = new Padding(6, 2, 6, 2);
            lbSystems.Name = "lbSystems";
            lbSystems.Size = new Size(49, 25);
            lbSystems.TabIndex = 6;
            lbSystems.Text = "88";
            lbSystems.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbColoniesLabel
            // 
            lbColoniesLabel.AutoSize = true;
            lbColoniesLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbColoniesLabel.Location = new Point(181, 105);
            lbColoniesLabel.Margin = new Padding(4, 0, 4, 0);
            lbColoniesLabel.Name = "lbColoniesLabel";
            lbColoniesLabel.Size = new Size(60, 16);
            lbColoniesLabel.TabIndex = 7;
            lbColoniesLabel.Text = "Colonies";
            // 
            // lbColonies
            // 
            lbColonies.BorderStyle = BorderStyle.FixedSingle;
            lbColonies.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbColonies.Location = new Point(260, 102);
            lbColonies.Margin = new Padding(6, 2, 6, 2);
            lbColonies.Name = "lbColonies";
            lbColonies.Size = new Size(49, 25);
            lbColonies.TabIndex = 8;
            lbColonies.Text = "88";
            lbColonies.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbHome
            // 
            lbHome.BorderStyle = BorderStyle.FixedSingle;
            lbHome.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbHome.Location = new Point(462, 62);
            lbHome.Margin = new Padding(6, 2, 6, 2);
            lbHome.Name = "lbHome";
            lbHome.Size = new Size(216, 25);
            lbHome.TabIndex = 10;
            lbHome.Text = "Location...";
            lbHome.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbHomeLabel
            // 
            lbHomeLabel.AutoSize = true;
            lbHomeLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbHomeLabel.Location = new Point(352, 66);
            lbHomeLabel.Margin = new Padding(4, 0, 4, 0);
            lbHomeLabel.Name = "lbHomeLabel";
            lbHomeLabel.Size = new Size(85, 16);
            lbHomeLabel.TabIndex = 9;
            lbHomeLabel.Text = "Home Planet";
            // 
            // tbDescription
            // 
            tbDescription.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            tbDescription.Location = new Point(14, 143);
            tbDescription.Margin = new Padding(4, 3, 4, 3);
            tbDescription.Multiline = true;
            tbDescription.Name = "tbDescription";
            tbDescription.Size = new Size(662, 313);
            tbDescription.TabIndex = 11;
            // 
            // lbRace
            // 
            lbRace.AutoSize = true;
            lbRace.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbRace.Location = new Point(36, 21);
            lbRace.Margin = new Padding(4, 0, 4, 0);
            lbRace.Name = "lbRace";
            lbRace.Size = new Size(40, 16);
            lbRace.TabIndex = 12;
            lbRace.Text = "Race";
            // 
            // lbPopulation
            // 
            lbPopulation.BorderStyle = BorderStyle.FixedSingle;
            lbPopulation.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbPopulation.Location = new Point(462, 102);
            lbPopulation.Margin = new Padding(6, 2, 6, 2);
            lbPopulation.Name = "lbPopulation";
            lbPopulation.Size = new Size(49, 25);
            lbPopulation.TabIndex = 14;
            lbPopulation.Text = "88";
            lbPopulation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbPopulationLabel
            // 
            lbPopulationLabel.AutoSize = true;
            lbPopulationLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbPopulationLabel.Location = new Point(383, 105);
            lbPopulationLabel.Margin = new Padding(4, 0, 4, 0);
            lbPopulationLabel.Name = "lbPopulationLabel";
            lbPopulationLabel.Size = new Size(71, 16);
            lbPopulationLabel.TabIndex = 13;
            lbPopulationLabel.Text = "Population";
            // 
            // lbExp
            // 
            lbExp.BorderStyle = BorderStyle.Fixed3D;
            lbExp.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbExp.Location = new Point(260, 62);
            lbExp.Margin = new Padding(6, 2, 6, 2);
            lbExp.Name = "lbExp";
            lbExp.Size = new Size(49, 25);
            lbExp.TabIndex = 15;
            lbExp.Text = "88.88%";
            lbExp.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // RaceView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(694, 465);
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
            Margin = new Padding(4, 3, 4, 3);
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
    }
}