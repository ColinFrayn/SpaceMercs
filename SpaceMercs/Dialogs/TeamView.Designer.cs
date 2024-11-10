namespace SpaceMercs.Dialogs {
    partial class TeamView {
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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            dgSoldiers = new DataGridView();
            SoldierName = new DataGridViewTextBoxColumn();
            SoldierRace = new DataGridViewTextBoxColumn();
            SoldierLevel = new DataGridViewTextBoxColumn();
            SoldierStatus = new DataGridViewTextBoxColumn();
            btInventory = new Button();
            gbSoldier = new GroupBox();
            btColour = new Button();
            btIncreaseSkill = new Button();
            btAddNewSkill = new Button();
            lbUnspent = new Label();
            label17 = new Label();
            pbExperience = new PictureBox();
            groupBox2 = new GroupBox();
            lbLevel = new Label();
            label15 = new Label();
            label20 = new Label();
            lbAttackTotal = new Label();
            label38 = new Label();
            lbDefenceTotal = new Label();
            lbHealthTotal = new Label();
            lbArmour = new Label();
            label11 = new Label();
            label18 = new Label();
            lbDeactivated = new Label();
            lbRFoot = new Label();
            lbRLeg = new Label();
            lbLFoot = new Label();
            lbLLeg = new Label();
            lbLHand = new Label();
            lbRHand = new Label();
            lbLArm = new Label();
            lbRArm = new Label();
            tbSkills = new TabControl();
            tpMilitary = new TabPage();
            lbWeaponSkills = new ListBox();
            tpUtility = new TabPage();
            lbUtilitySkills = new ListBox();
            lbChest = new Label();
            lbHead = new Label();
            lbEndurance = new Label();
            label16 = new Label();
            lbToughness = new Label();
            label14 = new Label();
            lbInsight = new Label();
            label12 = new Label();
            lbAgility = new Label();
            label10 = new Label();
            lbStrength = new Label();
            label6 = new Label();
            groupBox1 = new GroupBox();
            lbEquipped = new ListBox();
            lbEncumber = new Label();
            btDrop = new Button();
            btEquip = new Button();
            label1 = new Label();
            lbInventory = new ListBox();
            lbCapacity = new Label();
            btDropAll = new Button();
            label13 = new Label();
            label8 = new Label();
            lbWeight = new Label();
            btDismiss = new Button();
            btDeactivate = new Button();
            lbCash = new Label();
            label3 = new Label();
            lbLocation = new Label();
            label4 = new Label();
            lbRoster = new Label();
            label5 = new Label();
            lbActive = new Label();
            label7 = new Label();
            label2 = new Label();
            lbShip = new Label();
            lbBerths = new Label();
            label9 = new Label();
            lbInsufficientBerths = new Label();
            cdPickColour = new ColorDialog();
            btUpgradeStat = new Button();
            ((System.ComponentModel.ISupportInitialize)dgSoldiers).BeginInit();
            gbSoldier.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbExperience).BeginInit();
            groupBox2.SuspendLayout();
            tbSkills.SuspendLayout();
            tpMilitary.SuspendLayout();
            tpUtility.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // dgSoldiers
            // 
            dgSoldiers.AllowUserToAddRows = false;
            dgSoldiers.AllowUserToDeleteRows = false;
            dgSoldiers.AllowUserToResizeColumns = false;
            dgSoldiers.AllowUserToResizeRows = false;
            dgSoldiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgSoldiers.Columns.AddRange(new DataGridViewColumn[] { SoldierName, SoldierRace, SoldierLevel, SoldierStatus });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgSoldiers.DefaultCellStyle = dataGridViewCellStyle4;
            dgSoldiers.Location = new Point(14, 128);
            dgSoldiers.Margin = new Padding(4, 3, 4, 3);
            dgSoldiers.MultiSelect = false;
            dgSoldiers.Name = "dgSoldiers";
            dgSoldiers.ReadOnly = true;
            dgSoldiers.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgSoldiers.ScrollBars = ScrollBars.Vertical;
            dgSoldiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgSoldiers.Size = new Size(626, 579);
            dgSoldiers.TabIndex = 0;
            dgSoldiers.SelectionChanged += dgSoldiers_SelectionChanged;
            // 
            // SoldierName
            // 
            SoldierName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            SoldierName.HeaderText = "Soldier Name";
            SoldierName.Name = "SoldierName";
            SoldierName.ReadOnly = true;
            // 
            // SoldierRace
            // 
            SoldierRace.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SoldierRace.HeaderText = "Race";
            SoldierRace.Name = "SoldierRace";
            SoldierRace.ReadOnly = true;
            SoldierRace.Width = 57;
            // 
            // SoldierLevel
            // 
            SoldierLevel.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SoldierLevel.HeaderText = "Level";
            SoldierLevel.Name = "SoldierLevel";
            SoldierLevel.ReadOnly = true;
            SoldierLevel.Width = 59;
            // 
            // SoldierStatus
            // 
            SoldierStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SoldierStatus.HeaderText = "Status";
            SoldierStatus.Name = "SoldierStatus";
            SoldierStatus.ReadOnly = true;
            SoldierStatus.Width = 64;
            // 
            // btInventory
            // 
            btInventory.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            btInventory.Location = new Point(377, 720);
            btInventory.Margin = new Padding(4, 3, 4, 3);
            btInventory.Name = "btInventory";
            btInventory.Size = new Size(264, 96);
            btInventory.TabIndex = 7;
            btInventory.Text = "View Ship Storage";
            btInventory.UseVisualStyleBackColor = true;
            btInventory.Click += btInventory_Click;
            // 
            // gbSoldier
            // 
            gbSoldier.Controls.Add(btUpgradeStat);
            gbSoldier.Controls.Add(btColour);
            gbSoldier.Controls.Add(btIncreaseSkill);
            gbSoldier.Controls.Add(btAddNewSkill);
            gbSoldier.Controls.Add(lbUnspent);
            gbSoldier.Controls.Add(label17);
            gbSoldier.Controls.Add(pbExperience);
            gbSoldier.Controls.Add(groupBox2);
            gbSoldier.Controls.Add(lbDeactivated);
            gbSoldier.Controls.Add(lbRFoot);
            gbSoldier.Controls.Add(lbRLeg);
            gbSoldier.Controls.Add(lbLFoot);
            gbSoldier.Controls.Add(lbLLeg);
            gbSoldier.Controls.Add(lbLHand);
            gbSoldier.Controls.Add(lbRHand);
            gbSoldier.Controls.Add(lbLArm);
            gbSoldier.Controls.Add(lbRArm);
            gbSoldier.Controls.Add(tbSkills);
            gbSoldier.Controls.Add(lbChest);
            gbSoldier.Controls.Add(lbHead);
            gbSoldier.Controls.Add(lbEndurance);
            gbSoldier.Controls.Add(label16);
            gbSoldier.Controls.Add(lbToughness);
            gbSoldier.Controls.Add(label14);
            gbSoldier.Controls.Add(lbInsight);
            gbSoldier.Controls.Add(label12);
            gbSoldier.Controls.Add(lbAgility);
            gbSoldier.Controls.Add(label10);
            gbSoldier.Controls.Add(lbStrength);
            gbSoldier.Controls.Add(label6);
            gbSoldier.Controls.Add(groupBox1);
            gbSoldier.Location = new Point(654, 13);
            gbSoldier.Margin = new Padding(4, 3, 4, 3);
            gbSoldier.Name = "gbSoldier";
            gbSoldier.Padding = new Padding(4, 3, 4, 3);
            gbSoldier.Size = new Size(471, 803);
            gbSoldier.TabIndex = 8;
            gbSoldier.TabStop = false;
            gbSoldier.Text = "Soldier Details";
            // 
            // btColour
            // 
            btColour.Location = new Point(404, 186);
            btColour.Margin = new Padding(4, 3, 4, 3);
            btColour.Name = "btColour";
            btColour.Size = new Size(48, 27);
            btColour.TabIndex = 92;
            btColour.UseVisualStyleBackColor = true;
            btColour.Click += btColour_Click;
            // 
            // btIncreaseSkill
            // 
            btIncreaseSkill.Location = new Point(132, 751);
            btIncreaseSkill.Margin = new Padding(0);
            btIncreaseSkill.Name = "btIncreaseSkill";
            btIncreaseSkill.Size = new Size(52, 43);
            btIncreaseSkill.TabIndex = 91;
            btIncreaseSkill.Text = "Boost Skill";
            btIncreaseSkill.UseVisualStyleBackColor = true;
            btIncreaseSkill.Click += btIncreaseSkill_Click;
            // 
            // btAddNewSkill
            // 
            btAddNewSkill.Location = new Point(69, 751);
            btAddNewSkill.Margin = new Padding(4, 3, 4, 3);
            btAddNewSkill.Name = "btAddNewSkill";
            btAddNewSkill.Size = new Size(59, 44);
            btAddNewSkill.TabIndex = 90;
            btAddNewSkill.Text = "Add New";
            btAddNewSkill.UseVisualStyleBackColor = true;
            btAddNewSkill.Click += btAddNewSkill_Click;
            // 
            // lbUnspent
            // 
            lbUnspent.BorderStyle = BorderStyle.FixedSingle;
            lbUnspent.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbUnspent.Location = new Point(18, 770);
            lbUnspent.Margin = new Padding(6, 2, 6, 2);
            lbUnspent.Name = "lbUnspent";
            lbUnspent.Size = new Size(42, 25);
            lbUnspent.TabIndex = 89;
            lbUnspent.Text = "88";
            lbUnspent.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(12, 750);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(51, 15);
            label17.TabIndex = 88;
            label17.Text = "Unspent";
            // 
            // pbExperience
            // 
            pbExperience.BorderStyle = BorderStyle.FixedSingle;
            pbExperience.Location = new Point(205, 222);
            pbExperience.Margin = new Padding(4, 3, 4, 3);
            pbExperience.Name = "pbExperience";
            pbExperience.Size = new Size(246, 24);
            pbExperience.TabIndex = 87;
            pbExperience.TabStop = false;
            pbExperience.Paint += pbExperience_Paint;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lbLevel);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(label20);
            groupBox2.Controls.Add(lbAttackTotal);
            groupBox2.Controls.Add(label38);
            groupBox2.Controls.Add(lbDefenceTotal);
            groupBox2.Controls.Add(lbHealthTotal);
            groupBox2.Controls.Add(lbArmour);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label18);
            groupBox2.Location = new Point(9, 208);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(173, 209);
            groupBox2.TabIndex = 86;
            groupBox2.TabStop = false;
            groupBox2.Text = "Primary Stats";
            // 
            // lbLevel
            // 
            lbLevel.BackColor = SystemColors.Window;
            lbLevel.BorderStyle = BorderStyle.FixedSingle;
            lbLevel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbLevel.Location = new Point(98, 24);
            lbLevel.Margin = new Padding(6, 2, 6, 2);
            lbLevel.Name = "lbLevel";
            lbLevel.Size = new Size(58, 27);
            lbLevel.TabIndex = 90;
            lbLevel.Text = "888";
            lbLevel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label15.Location = new Point(38, 30);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(40, 16);
            label15.TabIndex = 49;
            label15.Text = "Level";
            label15.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label20.Location = new Point(18, 138);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(58, 16);
            label20.TabIndex = 89;
            label20.Text = "Defence";
            label20.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbAttackTotal
            // 
            lbAttackTotal.BackColor = Color.Tomato;
            lbAttackTotal.BorderStyle = BorderStyle.FixedSingle;
            lbAttackTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbAttackTotal.Location = new Point(98, 97);
            lbAttackTotal.Margin = new Padding(6, 2, 6, 2);
            lbAttackTotal.Name = "lbAttackTotal";
            lbAttackTotal.Size = new Size(58, 27);
            lbAttackTotal.TabIndex = 80;
            lbAttackTotal.Text = "888";
            lbAttackTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label38.Location = new Point(34, 103);
            label38.Margin = new Padding(4, 0, 4, 0);
            label38.Name = "label38";
            label38.Size = new Size(44, 16);
            label38.TabIndex = 76;
            label38.Text = "Attack";
            label38.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbDefenceTotal
            // 
            lbDefenceTotal.BackColor = SystemColors.Highlight;
            lbDefenceTotal.BorderStyle = BorderStyle.FixedSingle;
            lbDefenceTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbDefenceTotal.Location = new Point(98, 133);
            lbDefenceTotal.Margin = new Padding(6, 2, 6, 2);
            lbDefenceTotal.Name = "lbDefenceTotal";
            lbDefenceTotal.Size = new Size(58, 27);
            lbDefenceTotal.TabIndex = 61;
            lbDefenceTotal.Text = "888";
            lbDefenceTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbHealthTotal
            // 
            lbHealthTotal.BackColor = Color.LimeGreen;
            lbHealthTotal.BorderStyle = BorderStyle.FixedSingle;
            lbHealthTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbHealthTotal.Location = new Point(98, 61);
            lbHealthTotal.Margin = new Padding(6, 2, 6, 2);
            lbHealthTotal.Name = "lbHealthTotal";
            lbHealthTotal.Size = new Size(58, 27);
            lbHealthTotal.TabIndex = 60;
            lbHealthTotal.Text = "888";
            lbHealthTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbArmour
            // 
            lbArmour.BackColor = SystemColors.ControlLight;
            lbArmour.BorderStyle = BorderStyle.FixedSingle;
            lbArmour.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbArmour.Location = new Point(98, 168);
            lbArmour.Margin = new Padding(6, 2, 6, 2);
            lbArmour.Name = "lbArmour";
            lbArmour.Size = new Size(58, 27);
            lbArmour.TabIndex = 88;
            lbArmour.Text = "888";
            lbArmour.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label11.Location = new Point(31, 66);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(46, 16);
            label11.TabIndex = 48;
            label11.Text = "Health";
            label11.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label18.Location = new Point(27, 174);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(50, 16);
            label18.TabIndex = 87;
            label18.Text = "Armour";
            label18.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbDeactivated
            // 
            lbDeactivated.AutoSize = true;
            lbDeactivated.ForeColor = Color.Red;
            lbDeactivated.Location = new Point(171, 19);
            lbDeactivated.Margin = new Padding(4, 0, 4, 0);
            lbDeactivated.Name = "lbDeactivated";
            lbDeactivated.Size = new Size(121, 15);
            lbDeactivated.TabIndex = 85;
            lbDeactivated.Text = "Soldier is deactivated!";
            lbDeactivated.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbRFoot
            // 
            lbRFoot.BorderStyle = BorderStyle.FixedSingle;
            lbRFoot.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbRFoot.Location = new Point(329, 189);
            lbRFoot.Margin = new Padding(0);
            lbRFoot.Name = "lbRFoot";
            lbRFoot.Size = new Size(30, 23);
            lbRFoot.TabIndex = 47;
            lbRFoot.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbRLeg
            // 
            lbRLeg.BorderStyle = BorderStyle.FixedSingle;
            lbRLeg.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbRLeg.Location = new Point(329, 127);
            lbRLeg.Margin = new Padding(0);
            lbRLeg.Name = "lbRLeg";
            lbRLeg.Size = new Size(30, 62);
            lbRLeg.TabIndex = 46;
            lbRLeg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbLFoot
            // 
            lbLFoot.BorderStyle = BorderStyle.FixedSingle;
            lbLFoot.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbLFoot.Location = new Point(294, 189);
            lbLFoot.Margin = new Padding(0);
            lbLFoot.Name = "lbLFoot";
            lbLFoot.Size = new Size(30, 23);
            lbLFoot.TabIndex = 45;
            lbLFoot.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbLLeg
            // 
            lbLLeg.BorderStyle = BorderStyle.FixedSingle;
            lbLLeg.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbLLeg.Location = new Point(294, 127);
            lbLLeg.Margin = new Padding(0);
            lbLLeg.Name = "lbLLeg";
            lbLLeg.Size = new Size(30, 62);
            lbLLeg.TabIndex = 44;
            lbLLeg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbLHand
            // 
            lbLHand.BorderStyle = BorderStyle.FixedSingle;
            lbLHand.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbLHand.Location = new Point(262, 125);
            lbLHand.Margin = new Padding(0);
            lbLHand.Name = "lbLHand";
            lbLHand.Size = new Size(30, 27);
            lbLHand.TabIndex = 43;
            lbLHand.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbRHand
            // 
            lbRHand.BorderStyle = BorderStyle.FixedSingle;
            lbRHand.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbRHand.Location = new Point(360, 125);
            lbRHand.Margin = new Padding(0);
            lbRHand.Name = "lbRHand";
            lbRHand.Size = new Size(30, 27);
            lbRHand.TabIndex = 42;
            lbRHand.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbLArm
            // 
            lbLArm.BorderStyle = BorderStyle.FixedSingle;
            lbLArm.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbLArm.Location = new Point(262, 53);
            lbLArm.Margin = new Padding(0);
            lbLArm.Name = "lbLArm";
            lbLArm.Size = new Size(30, 71);
            lbLArm.TabIndex = 41;
            lbLArm.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbRArm
            // 
            lbRArm.BorderStyle = BorderStyle.FixedSingle;
            lbRArm.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbRArm.Location = new Point(360, 53);
            lbRArm.Margin = new Padding(0);
            lbRArm.Name = "lbRArm";
            lbRArm.Size = new Size(30, 71);
            lbRArm.TabIndex = 40;
            lbRArm.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbSkills
            // 
            tbSkills.Controls.Add(tpMilitary);
            tbSkills.Controls.Add(tpUtility);
            tbSkills.Location = new Point(10, 434);
            tbSkills.Margin = new Padding(4, 3, 4, 3);
            tbSkills.Name = "tbSkills";
            tbSkills.SelectedIndex = 0;
            tbSkills.Size = new Size(182, 314);
            tbSkills.TabIndex = 39;
            tbSkills.SelectedIndexChanged += tbSkills_SelectedIndexChanged;
            // 
            // tpMilitary
            // 
            tpMilitary.Controls.Add(lbWeaponSkills);
            tpMilitary.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            tpMilitary.Location = new Point(4, 24);
            tpMilitary.Margin = new Padding(4, 3, 4, 3);
            tpMilitary.Name = "tpMilitary";
            tpMilitary.Padding = new Padding(4, 3, 4, 3);
            tpMilitary.Size = new Size(174, 286);
            tpMilitary.TabIndex = 0;
            tpMilitary.Text = "Weapon Skills";
            tpMilitary.UseVisualStyleBackColor = true;
            // 
            // lbWeaponSkills
            // 
            lbWeaponSkills.FormattingEnabled = true;
            lbWeaponSkills.ItemHeight = 16;
            lbWeaponSkills.Location = new Point(0, 2);
            lbWeaponSkills.Margin = new Padding(4, 3, 4, 3);
            lbWeaponSkills.Name = "lbWeaponSkills";
            lbWeaponSkills.Size = new Size(168, 276);
            lbWeaponSkills.TabIndex = 0;
            // 
            // tpUtility
            // 
            tpUtility.Controls.Add(lbUtilitySkills);
            tpUtility.Location = new Point(4, 24);
            tpUtility.Margin = new Padding(4, 3, 4, 3);
            tpUtility.Name = "tpUtility";
            tpUtility.Padding = new Padding(4, 3, 4, 3);
            tpUtility.Size = new Size(174, 286);
            tpUtility.TabIndex = 1;
            tpUtility.Text = "Utility Skills";
            tpUtility.UseVisualStyleBackColor = true;
            // 
            // lbUtilitySkills
            // 
            lbUtilitySkills.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbUtilitySkills.FormattingEnabled = true;
            lbUtilitySkills.ItemHeight = 16;
            lbUtilitySkills.Location = new Point(0, 2);
            lbUtilitySkills.Margin = new Padding(4, 3, 4, 3);
            lbUtilitySkills.Name = "lbUtilitySkills";
            lbUtilitySkills.Size = new Size(168, 276);
            lbUtilitySkills.TabIndex = 1;
            lbUtilitySkills.SelectedIndexChanged += lbUtilitySkills_SelectedIndexChanged;
            lbUtilitySkills.DoubleClick += lbUtilitySkills_DoubleClick;
            // 
            // lbChest
            // 
            lbChest.BorderStyle = BorderStyle.FixedSingle;
            lbChest.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbChest.Location = new Point(293, 53);
            lbChest.Margin = new Padding(0);
            lbChest.Name = "lbChest";
            lbChest.Size = new Size(67, 74);
            lbChest.TabIndex = 35;
            lbChest.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbHead
            // 
            lbHead.BorderStyle = BorderStyle.FixedSingle;
            lbHead.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbHead.Location = new Point(306, 21);
            lbHead.Margin = new Padding(0);
            lbHead.Name = "lbHead";
            lbHead.Size = new Size(42, 27);
            lbHead.TabIndex = 34;
            lbHead.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbEndurance
            // 
            lbEndurance.BorderStyle = BorderStyle.FixedSingle;
            lbEndurance.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbEndurance.Location = new Point(111, 159);
            lbEndurance.Margin = new Padding(6, 2, 6, 2);
            lbEndurance.Name = "lbEndurance";
            lbEndurance.Size = new Size(54, 25);
            lbEndurance.TabIndex = 29;
            lbEndurance.Text = "888";
            lbEndurance.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label16.Location = new Point(19, 163);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(72, 16);
            label16.TabIndex = 28;
            label16.Text = "Endurance";
            label16.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbToughness
            // 
            lbToughness.BorderStyle = BorderStyle.FixedSingle;
            lbToughness.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbToughness.Location = new Point(111, 129);
            lbToughness.Margin = new Padding(6, 2, 6, 2);
            lbToughness.Name = "lbToughness";
            lbToughness.Size = new Size(54, 25);
            lbToughness.TabIndex = 27;
            lbToughness.Text = "888";
            lbToughness.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label14.Location = new Point(15, 133);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(75, 16);
            label14.TabIndex = 26;
            label14.Text = "Toughness";
            label14.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbInsight
            // 
            lbInsight.BorderStyle = BorderStyle.FixedSingle;
            lbInsight.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbInsight.Location = new Point(111, 99);
            lbInsight.Margin = new Padding(6, 2, 6, 2);
            lbInsight.Name = "lbInsight";
            lbInsight.Size = new Size(54, 25);
            lbInsight.TabIndex = 25;
            lbInsight.Text = "888";
            lbInsight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label12.Location = new Point(42, 103);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(45, 16);
            label12.TabIndex = 24;
            label12.Text = "Insight";
            label12.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbAgility
            // 
            lbAgility.BorderStyle = BorderStyle.FixedSingle;
            lbAgility.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbAgility.Location = new Point(111, 69);
            lbAgility.Margin = new Padding(6, 2, 6, 2);
            lbAgility.Name = "lbAgility";
            lbAgility.Size = new Size(54, 25);
            lbAgility.TabIndex = 23;
            lbAgility.Text = "888";
            lbAgility.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label10.Location = new Point(52, 73);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(43, 16);
            label10.TabIndex = 22;
            label10.Text = "Agility";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbStrength
            // 
            lbStrength.BorderStyle = BorderStyle.FixedSingle;
            lbStrength.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lbStrength.Location = new Point(111, 39);
            lbStrength.Margin = new Padding(6, 2, 6, 2);
            lbStrength.Name = "lbStrength";
            lbStrength.Size = new Size(54, 25);
            lbStrength.TabIndex = 21;
            lbStrength.Text = "888";
            lbStrength.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(37, 43);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(56, 16);
            label6.TabIndex = 20;
            label6.Text = "Strength";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbEquipped);
            groupBox1.Controls.Add(lbEncumber);
            groupBox1.Controls.Add(btDrop);
            groupBox1.Controls.Add(btEquip);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lbInventory);
            groupBox1.Controls.Add(lbCapacity);
            groupBox1.Controls.Add(btDropAll);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(lbWeight);
            groupBox1.Location = new Point(205, 252);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(246, 535);
            groupBox1.TabIndex = 33;
            groupBox1.TabStop = false;
            groupBox1.Text = "Inventory";
            // 
            // lbEquipped
            // 
            lbEquipped.FormattingEnabled = true;
            lbEquipped.HorizontalScrollbar = true;
            lbEquipped.ItemHeight = 15;
            lbEquipped.Location = new Point(15, 315);
            lbEquipped.Margin = new Padding(4, 3, 4, 3);
            lbEquipped.Name = "lbEquipped";
            lbEquipped.Size = new Size(213, 139);
            lbEquipped.TabIndex = 45;
            lbEquipped.SelectedIndexChanged += lbEquipped_SelectedIndexChanged;
            lbEquipped.DoubleClick += lbEquipped_DoubleClick;
            // 
            // lbEncumber
            // 
            lbEncumber.BorderStyle = BorderStyle.FixedSingle;
            lbEncumber.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbEncumber.Location = new Point(168, 489);
            lbEncumber.Margin = new Padding(6, 2, 6, 2);
            lbEncumber.Name = "lbEncumber";
            lbEncumber.Size = new Size(58, 30);
            lbEncumber.TabIndex = 44;
            lbEncumber.Text = "100%";
            lbEncumber.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btDrop
            // 
            btDrop.Location = new Point(91, 277);
            btDrop.Margin = new Padding(4, 3, 4, 3);
            btDrop.Name = "btDrop";
            btDrop.Size = new Size(64, 27);
            btDrop.TabIndex = 2;
            btDrop.Text = "Store";
            btDrop.UseVisualStyleBackColor = true;
            btDrop.Click += btDrop_Click;
            // 
            // btEquip
            // 
            btEquip.Location = new Point(15, 277);
            btEquip.Margin = new Padding(4, 3, 4, 3);
            btEquip.Name = "btEquip";
            btEquip.Size = new Size(64, 27);
            btEquip.TabIndex = 1;
            btEquip.Text = "Equip";
            btEquip.UseVisualStyleBackColor = true;
            btEquip.Click += btEquip_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(164, 466);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 43;
            label1.Text = "Encumb.";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbInventory
            // 
            lbInventory.FormattingEnabled = true;
            lbInventory.HorizontalScrollbar = true;
            lbInventory.ItemHeight = 15;
            lbInventory.Location = new Point(15, 23);
            lbInventory.Margin = new Padding(4, 3, 4, 3);
            lbInventory.Name = "lbInventory";
            lbInventory.Size = new Size(213, 244);
            lbInventory.TabIndex = 0;
            lbInventory.SelectedIndexChanged += lbInventory_SelectedIndexChanged;
            lbInventory.DoubleClick += lbInventory_DoubleClick;
            // 
            // lbCapacity
            // 
            lbCapacity.BorderStyle = BorderStyle.FixedSingle;
            lbCapacity.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbCapacity.Location = new Point(71, 497);
            lbCapacity.Margin = new Padding(6, 2, 6, 2);
            lbCapacity.Name = "lbCapacity";
            lbCapacity.Size = new Size(79, 25);
            lbCapacity.TabIndex = 42;
            lbCapacity.Text = "888.88 kg";
            lbCapacity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btDropAll
            // 
            btDropAll.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            btDropAll.Location = new Point(166, 277);
            btDropAll.Margin = new Padding(4, 3, 4, 3);
            btDropAll.Name = "btDropAll";
            btDropAll.Size = new Size(64, 27);
            btDropAll.TabIndex = 31;
            btDropAll.Text = "Store All";
            btDropAll.UseVisualStyleBackColor = true;
            btDropAll.Click += btDropAllClick;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label13.Location = new Point(9, 501);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(53, 15);
            label13.TabIndex = 41;
            label13.Text = "Capacity";
            label13.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label8.Location = new Point(19, 470);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(45, 15);
            label8.TabIndex = 39;
            label8.Text = "Weight";
            label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbWeight
            // 
            lbWeight.BorderStyle = BorderStyle.FixedSingle;
            lbWeight.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbWeight.Location = new Point(71, 466);
            lbWeight.Margin = new Padding(6, 2, 6, 2);
            lbWeight.Name = "lbWeight";
            lbWeight.Size = new Size(79, 25);
            lbWeight.TabIndex = 40;
            lbWeight.Text = "888.88 kg";
            lbWeight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btDismiss
            // 
            btDismiss.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDismiss.Location = new Point(536, 58);
            btDismiss.Margin = new Padding(4, 3, 4, 3);
            btDismiss.Name = "btDismiss";
            btDismiss.Size = new Size(105, 32);
            btDismiss.TabIndex = 32;
            btDismiss.Text = "Dismiss";
            btDismiss.UseVisualStyleBackColor = true;
            btDismiss.Click += btDismiss_Click;
            // 
            // btDeactivate
            // 
            btDeactivate.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btDeactivate.Location = new Point(536, 23);
            btDeactivate.Margin = new Padding(4, 3, 4, 3);
            btDeactivate.Name = "btDeactivate";
            btDeactivate.Size = new Size(105, 32);
            btDeactivate.TabIndex = 30;
            btDeactivate.Text = "Deactivate";
            btDeactivate.UseVisualStyleBackColor = true;
            btDeactivate.Click += btDeactivate_Click;
            // 
            // lbCash
            // 
            lbCash.BorderStyle = BorderStyle.FixedSingle;
            lbCash.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbCash.Location = new Point(91, 23);
            lbCash.Margin = new Padding(6, 2, 6, 2);
            lbCash.Name = "lbCash";
            lbCash.Size = new Size(112, 25);
            lbCash.TabIndex = 10;
            lbCash.Text = "8888888.88";
            lbCash.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(41, 27);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(38, 16);
            label3.TabIndex = 9;
            label3.Text = "Cash";
            // 
            // lbLocation
            // 
            lbLocation.BorderStyle = BorderStyle.FixedSingle;
            lbLocation.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbLocation.Location = new Point(91, 65);
            lbLocation.Margin = new Padding(6, 2, 6, 2);
            lbLocation.Name = "lbLocation";
            lbLocation.Size = new Size(124, 25);
            lbLocation.TabIndex = 12;
            lbLocation.Text = "(888.88,888.88)";
            lbLocation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(18, 68);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(58, 16);
            label4.TabIndex = 11;
            label4.Text = "Location";
            // 
            // lbRoster
            // 
            lbRoster.BorderStyle = BorderStyle.FixedSingle;
            lbRoster.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbRoster.Location = new Point(366, 23);
            lbRoster.Margin = new Padding(6, 2, 6, 2);
            lbRoster.Name = "lbRoster";
            lbRoster.Size = new Size(112, 25);
            lbRoster.TabIndex = 14;
            lbRoster.Text = "888 soldiers";
            lbRoster.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label5.Location = new Point(301, 27);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(47, 16);
            label5.TabIndex = 13;
            label5.Text = "Roster";
            // 
            // lbActive
            // 
            lbActive.BorderStyle = BorderStyle.FixedSingle;
            lbActive.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbActive.Location = new Point(366, 65);
            lbActive.Margin = new Padding(6, 2, 6, 2);
            lbActive.Name = "lbActive";
            lbActive.Size = new Size(112, 25);
            lbActive.TabIndex = 16;
            lbActive.Text = "888 soldiers";
            lbActive.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label7.Location = new Point(301, 68);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(44, 16);
            label7.TabIndex = 15;
            label7.Text = "Active";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(30, 730);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(34, 16);
            label2.TabIndex = 1;
            label2.Text = "Ship";
            // 
            // lbShip
            // 
            lbShip.BorderStyle = BorderStyle.FixedSingle;
            lbShip.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbShip.Location = new Point(78, 727);
            lbShip.Margin = new Padding(6, 2, 6, 2);
            lbShip.Name = "lbShip";
            lbShip.Size = new Size(275, 25);
            lbShip.TabIndex = 5;
            lbShip.Text = "Ship Design Type";
            lbShip.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbBerths
            // 
            lbBerths.BorderStyle = BorderStyle.FixedSingle;
            lbBerths.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbBerths.Location = new Point(78, 768);
            lbBerths.Margin = new Padding(6, 2, 6, 2);
            lbBerths.Name = "lbBerths";
            lbBerths.Size = new Size(48, 25);
            lbBerths.TabIndex = 18;
            lbBerths.Text = "888";
            lbBerths.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(18, 772);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(45, 16);
            label9.TabIndex = 17;
            label9.Text = "Berths";
            // 
            // lbInsufficientBerths
            // 
            lbInsufficientBerths.AutoSize = true;
            lbInsufficientBerths.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lbInsufficientBerths.ForeColor = Color.Red;
            lbInsufficientBerths.Location = new Point(149, 772);
            lbInsufficientBerths.Margin = new Padding(4, 0, 4, 0);
            lbInsufficientBerths.Name = "lbInsufficientBerths";
            lbInsufficientBerths.Size = new Size(112, 16);
            lbInsufficientBerths.TabIndex = 19;
            lbInsufficientBerths.Text = "Insufficient Berths!";
            // 
            // cdPickColour
            // 
            cdPickColour.AnyColor = true;
            cdPickColour.Color = Color.Blue;
            cdPickColour.SolidColorOnly = true;
            // 
            // btUpgradeStat
            // 
            btUpgradeStat.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            btUpgradeStat.Location = new Point(121, 14);
            btUpgradeStat.Margin = new Padding(0);
            btUpgradeStat.Name = "btUpgradeStat";
            btUpgradeStat.Size = new Size(34, 24);
            btUpgradeStat.TabIndex = 93;
            btUpgradeStat.Text = "+8";
            btUpgradeStat.UseVisualStyleBackColor = true;
            btUpgradeStat.Click += btUpgradeStat_Click;
            // 
            // TeamView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1139, 830);
            Controls.Add(gbSoldier);
            Controls.Add(lbInsufficientBerths);
            Controls.Add(lbBerths);
            Controls.Add(label9);
            Controls.Add(lbActive);
            Controls.Add(label7);
            Controls.Add(lbRoster);
            Controls.Add(label5);
            Controls.Add(lbLocation);
            Controls.Add(label4);
            Controls.Add(lbCash);
            Controls.Add(label3);
            Controls.Add(btInventory);
            Controls.Add(lbShip);
            Controls.Add(label2);
            Controls.Add(dgSoldiers);
            Controls.Add(btDeactivate);
            Controls.Add(btDismiss);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "TeamView";
            Text = "TeamView";
            FormClosing += TeamView_FormClosing;
            Load += TeamView_Load;
            ((System.ComponentModel.ISupportInitialize)dgSoldiers).EndInit();
            gbSoldier.ResumeLayout(false);
            gbSoldier.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbExperience).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tbSkills.ResumeLayout(false);
            tpMilitary.ResumeLayout(false);
            tpUtility.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgSoldiers;
        private Button btInventory;
        private GroupBox gbSoldier;
        private Label lbCash;
        private Label label3;
        private Label lbLocation;
        private Label label4;
        private Label lbRoster;
        private Label label5;
        private Label lbActive;
        private Label label7;
        private Label label2;
        private Label lbShip;
        private Label lbBerths;
        private Label label9;
        private Label lbInsufficientBerths;
        private DataGridViewTextBoxColumn SoldierName;
        private DataGridViewTextBoxColumn SoldierRace;
        private DataGridViewTextBoxColumn SoldierLevel;
        private DataGridViewTextBoxColumn SoldierStatus;
        private Button btDismiss;
        private Button btDropAll;
        private Button btDeactivate;
        private Label lbEndurance;
        private Label label16;
        private Label lbToughness;
        private Label label14;
        private Label lbInsight;
        private Label label12;
        private Label lbAgility;
        private Label label10;
        private Label lbStrength;
        private Label label6;
        private Label lbChest;
        private Label lbHead;
        private GroupBox groupBox1;
        private Button btDrop;
        private Button btEquip;
        private ListBox lbInventory;
        private Label lbEncumber;
        private Label label1;
        private Label lbCapacity;
        private Label label13;
        private Label lbWeight;
        private Label label8;
        private TabControl tbSkills;
        private TabPage tpMilitary;
        private TabPage tpUtility;
        private Label lbRFoot;
        private Label lbRLeg;
        private Label lbLFoot;
        private Label lbLLeg;
        private Label lbLHand;
        private Label lbRHand;
        private Label lbLArm;
        private Label lbRArm;
        private ListBox lbWeaponSkills;
        private ListBox lbUtilitySkills;
        private ListBox lbEquipped;
        private Label lbDeactivated;
        private Label lbAttackTotal;
        private Label label38;
        private Label lbDefenceTotal;
        private Label lbHealthTotal;
        private Label label15;
        private Label label11;
        private GroupBox groupBox2;
        private Label lbLevel;
        private Label label20;
        private Label lbArmour;
        private Label label18;
        private PictureBox pbExperience;
        private Button btIncreaseSkill;
        private Button btAddNewSkill;
        private Label lbUnspent;
        private Label label17;
        private ColorDialog cdPickColour;
        private Button btColour;
        private Button btUpgradeStat;
    }
}