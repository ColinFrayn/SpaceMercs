namespace SpaceMercs.Dialogs {
  partial class EquipmentView {
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
            gbSoldier = new GroupBox();
            btIncreaseSkill = new Button();
            btAddNewSkill = new Button();
            lbUnspent = new Label();
            label17 = new Label();
            pbExperience = new PictureBox();
            groupBox2 = new GroupBox();
            lbStamina = new Label();
            label3 = new Label();
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
            label13 = new Label();
            label8 = new Label();
            lbWeight = new Label();
            btScavenge = new Button();
            btPickUpAll = new Button();
            btPickUpSelected = new Button();
            dgFloor = new DataGridView();
            Item = new DataGridViewTextBoxColumn();
            Count = new DataGridViewTextBoxColumn();
            groupBox3 = new GroupBox();
            gbSoldier.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbExperience).BeginInit();
            groupBox2.SuspendLayout();
            tbSkills.SuspendLayout();
            tpMilitary.SuspendLayout();
            tpUtility.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgFloor).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // gbSoldier
            // 
            gbSoldier.Controls.Add(btIncreaseSkill);
            gbSoldier.Controls.Add(btAddNewSkill);
            gbSoldier.Controls.Add(lbUnspent);
            gbSoldier.Controls.Add(label17);
            gbSoldier.Controls.Add(pbExperience);
            gbSoldier.Controls.Add(groupBox2);
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
            gbSoldier.Location = new Point(7, 10);
            gbSoldier.Margin = new Padding(4, 3, 4, 3);
            gbSoldier.Name = "gbSoldier";
            gbSoldier.Padding = new Padding(4, 3, 4, 3);
            gbSoldier.Size = new Size(471, 803);
            gbSoldier.TabIndex = 8;
            gbSoldier.TabStop = false;
            gbSoldier.Text = "Soldier Details";
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
            lbUnspent.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            groupBox2.Controls.Add(lbStamina);
            groupBox2.Controls.Add(label3);
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
            groupBox2.Location = new Point(9, 189);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(173, 239);
            groupBox2.TabIndex = 86;
            groupBox2.TabStop = false;
            groupBox2.Text = "Primary Stats";
            // 
            // lbStamina
            // 
            lbStamina.BackColor = Color.Gold;
            lbStamina.BorderStyle = BorderStyle.FixedSingle;
            lbStamina.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbStamina.Location = new Point(88, 196);
            lbStamina.Margin = new Padding(6, 2, 6, 2);
            lbStamina.Name = "lbStamina";
            lbStamina.Size = new Size(66, 27);
            lbStamina.TabIndex = 92;
            lbStamina.Text = "888/88";
            lbStamina.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(15, 202);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(56, 16);
            label3.TabIndex = 91;
            label3.Text = "Stamina";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbLevel
            // 
            lbLevel.BackColor = SystemColors.Window;
            lbLevel.BorderStyle = BorderStyle.FixedSingle;
            lbLevel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbLevel.Location = new Point(88, 21);
            lbLevel.Margin = new Padding(6, 2, 6, 2);
            lbLevel.Name = "lbLevel";
            lbLevel.Size = new Size(66, 27);
            lbLevel.TabIndex = 90;
            lbLevel.Text = "888";
            lbLevel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(31, 27);
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
            label20.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label20.Location = new Point(13, 132);
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
            lbAttackTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbAttackTotal.Location = new Point(88, 92);
            lbAttackTotal.Margin = new Padding(6, 2, 6, 2);
            lbAttackTotal.Name = "lbAttackTotal";
            lbAttackTotal.Size = new Size(66, 27);
            lbAttackTotal.TabIndex = 80;
            lbAttackTotal.Text = "888";
            lbAttackTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label38.Location = new Point(27, 98);
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
            lbDefenceTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbDefenceTotal.Location = new Point(88, 127);
            lbDefenceTotal.Margin = new Padding(6, 2, 6, 2);
            lbDefenceTotal.Name = "lbDefenceTotal";
            lbDefenceTotal.Size = new Size(66, 27);
            lbDefenceTotal.TabIndex = 61;
            lbDefenceTotal.Text = "888";
            lbDefenceTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbHealthTotal
            // 
            lbHealthTotal.BackColor = Color.LimeGreen;
            lbHealthTotal.BorderStyle = BorderStyle.FixedSingle;
            lbHealthTotal.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbHealthTotal.Location = new Point(88, 57);
            lbHealthTotal.Margin = new Padding(6, 2, 6, 2);
            lbHealthTotal.Name = "lbHealthTotal";
            lbHealthTotal.Size = new Size(66, 27);
            lbHealthTotal.TabIndex = 60;
            lbHealthTotal.Text = "888";
            lbHealthTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbArmour
            // 
            lbArmour.BackColor = SystemColors.ControlLight;
            lbArmour.BorderStyle = BorderStyle.FixedSingle;
            lbArmour.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbArmour.Location = new Point(88, 161);
            lbArmour.Margin = new Padding(6, 2, 6, 2);
            lbArmour.Name = "lbArmour";
            lbArmour.Size = new Size(66, 27);
            lbArmour.TabIndex = 88;
            lbArmour.Text = "888";
            lbArmour.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.Location = new Point(25, 62);
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
            label18.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label18.Location = new Point(21, 167);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(50, 16);
            label18.TabIndex = 87;
            label18.Text = "Armour";
            label18.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbRFoot
            // 
            lbRFoot.BorderStyle = BorderStyle.FixedSingle;
            lbRFoot.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbRLeg.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbLFoot.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbLLeg.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbLHand.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbRHand.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbLArm.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbRArm.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            tpMilitary.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbUtilitySkills.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbUtilitySkills.FormattingEnabled = true;
            lbUtilitySkills.Location = new Point(0, 2);
            lbUtilitySkills.Margin = new Padding(4, 3, 4, 3);
            lbUtilitySkills.Name = "lbUtilitySkills";
            lbUtilitySkills.Size = new Size(168, 276);
            lbUtilitySkills.TabIndex = 1;
            lbUtilitySkills.SelectedIndexChanged += lbUtilitySkills_SelectedIndexChanged;
            // 
            // lbChest
            // 
            lbChest.BorderStyle = BorderStyle.FixedSingle;
            lbChest.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbHead.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbEndurance.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbEndurance.Location = new Point(111, 151);
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
            label16.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label16.Location = new Point(19, 155);
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
            lbToughness.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbToughness.Location = new Point(111, 121);
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
            label14.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label14.Location = new Point(15, 125);
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
            lbInsight.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbInsight.Location = new Point(111, 91);
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
            label12.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.Location = new Point(42, 95);
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
            lbAgility.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbAgility.Location = new Point(111, 61);
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
            label10.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(52, 65);
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
            lbStrength.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbStrength.Location = new Point(111, 31);
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
            label6.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(37, 35);
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
            lbEquipped.AllowDrop = true;
            lbEquipped.FormattingEnabled = true;
            lbEquipped.HorizontalScrollbar = true;
            lbEquipped.Location = new Point(15, 315);
            lbEquipped.Margin = new Padding(4, 3, 4, 3);
            lbEquipped.Name = "lbEquipped";
            lbEquipped.Size = new Size(213, 139);
            lbEquipped.TabIndex = 45;
            lbEquipped.SelectedIndexChanged += lbEquipped_SelectedIndexChanged;
            lbEquipped.DragDrop += lbEquipped_DragDrop;
            lbEquipped.DragEnter += lbEquipped_DragEnter;
            lbEquipped.DoubleClick += lbEquipped_DoubleClick;
            lbEquipped.MouseDown += lbEquipped_MouseDown;
            lbEquipped.MouseMove += lbEquipped_MouseMove;
            lbEquipped.MouseUp += lbEquipped_MouseUp;
            // 
            // lbEncumber
            // 
            lbEncumber.BorderStyle = BorderStyle.FixedSingle;
            lbEncumber.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            btDrop.Location = new Point(132, 277);
            btDrop.Margin = new Padding(4, 3, 4, 3);
            btDrop.Name = "btDrop";
            btDrop.Size = new Size(94, 27);
            btDrop.TabIndex = 2;
            btDrop.Text = "Drop";
            btDrop.UseVisualStyleBackColor = true;
            btDrop.Click += btDrop_Click;
            // 
            // btEquip
            // 
            btEquip.Location = new Point(15, 277);
            btEquip.Margin = new Padding(4, 3, 4, 3);
            btEquip.Name = "btEquip";
            btEquip.Size = new Size(104, 27);
            btEquip.TabIndex = 1;
            btEquip.Text = "Equip";
            btEquip.UseVisualStyleBackColor = true;
            btEquip.Click += btEquip_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbInventory.AllowDrop = true;
            lbInventory.FormattingEnabled = true;
            lbInventory.HorizontalScrollbar = true;
            lbInventory.Location = new Point(15, 23);
            lbInventory.Margin = new Padding(4, 3, 4, 3);
            lbInventory.Name = "lbInventory";
            lbInventory.Size = new Size(213, 244);
            lbInventory.TabIndex = 0;
            lbInventory.SelectedIndexChanged += lbInventory_SelectedIndexChanged;
            lbInventory.DragDrop += lbInventory_DragDrop;
            lbInventory.DragEnter += lbInventory_DragEnter;
            lbInventory.DoubleClick += lbInventory_DoubleClick;
            lbInventory.MouseDown += lbInventory_MouseDown;
            lbInventory.MouseMove += lbInventory_MouseMove;
            lbInventory.MouseUp += lbInventory_MouseUp;
            // 
            // lbCapacity
            // 
            lbCapacity.BorderStyle = BorderStyle.FixedSingle;
            lbCapacity.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbCapacity.Location = new Point(71, 497);
            lbCapacity.Margin = new Padding(6, 2, 6, 2);
            lbCapacity.Name = "lbCapacity";
            lbCapacity.Size = new Size(79, 25);
            lbCapacity.TabIndex = 42;
            lbCapacity.Text = "888.88 kg";
            lbCapacity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            label8.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
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
            lbWeight.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbWeight.Location = new Point(71, 466);
            lbWeight.Margin = new Padding(6, 2, 6, 2);
            lbWeight.Name = "lbWeight";
            lbWeight.Size = new Size(79, 25);
            lbWeight.TabIndex = 40;
            lbWeight.Text = "888.88 kg";
            lbWeight.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btScavenge
            // 
            btScavenge.Location = new Point(653, 768);
            btScavenge.Margin = new Padding(4, 3, 4, 3);
            btScavenge.Name = "btScavenge";
            btScavenge.Size = new Size(115, 37);
            btScavenge.TabIndex = 7;
            btScavenge.Text = "Scavenge";
            btScavenge.UseVisualStyleBackColor = true;
            btScavenge.Click += btScavenge_Click;
            // 
            // btPickUpAll
            // 
            btPickUpAll.Location = new Point(7, 753);
            btPickUpAll.Margin = new Padding(4, 3, 4, 3);
            btPickUpAll.Name = "btPickUpAll";
            btPickUpAll.Size = new Size(115, 37);
            btPickUpAll.TabIndex = 6;
            btPickUpAll.Text = "Take All";
            btPickUpAll.UseVisualStyleBackColor = true;
            btPickUpAll.Click += btPickUpAll_Click;
            // 
            // btPickUpSelected
            // 
            btPickUpSelected.Location = new Point(800, 768);
            btPickUpSelected.Margin = new Padding(4, 3, 4, 3);
            btPickUpSelected.Name = "btPickUpSelected";
            btPickUpSelected.Size = new Size(115, 37);
            btPickUpSelected.TabIndex = 5;
            btPickUpSelected.Text = "Take Selected";
            btPickUpSelected.UseVisualStyleBackColor = true;
            btPickUpSelected.Click += btPickUpSelected_Click;
            // 
            // dgFloor
            // 
            dgFloor.AllowDrop = true;
            dgFloor.AllowUserToAddRows = false;
            dgFloor.AllowUserToDeleteRows = false;
            dgFloor.AllowUserToResizeColumns = false;
            dgFloor.AllowUserToResizeRows = false;
            dgFloor.Anchor = AnchorStyles.None;
            dgFloor.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgFloor.Columns.AddRange(new DataGridViewColumn[] { Item, Count });
            dgFloor.Location = new Point(516, 45);
            dgFloor.Margin = new Padding(4, 3, 4, 3);
            dgFloor.Name = "dgFloor";
            dgFloor.ReadOnly = true;
            dgFloor.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgFloor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgFloor.Size = new Size(396, 708);
            dgFloor.TabIndex = 1;
            dgFloor.CellContentClick += dgFloor_CellContentClick;
            dgFloor.SelectionChanged += dgFloor_SelectionChanged;
            dgFloor.DragDrop += dgFloor_DragDrop;
            dgFloor.DragEnter += dgFloor_DragEnter;
            dgFloor.DoubleClick += dgFloor_DoubleClick;
            dgFloor.MouseDown += dgFloor_MouseDown;
            dgFloor.MouseMove += dgFloor_MouseMove;
            dgFloor.MouseUp += dgFloor_MouseUp;
            // 
            // Item
            // 
            Item.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Item.HeaderText = "Item";
            Item.Name = "Item";
            Item.ReadOnly = true;
            // 
            // Count
            // 
            Count.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Count.HeaderText = "Count";
            Count.Name = "Count";
            Count.ReadOnly = true;
            Count.Width = 65;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btPickUpAll);
            groupBox3.Location = new Point(498, 15);
            groupBox3.Margin = new Padding(4, 3, 4, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(4, 3, 4, 3);
            groupBox3.Size = new Size(429, 801);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "On the Floor";
            // 
            // EquipmentView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(941, 830);
            Controls.Add(btPickUpSelected);
            Controls.Add(btScavenge);
            Controls.Add(gbSoldier);
            Controls.Add(dgFloor);
            Controls.Add(groupBox3);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EquipmentView";
            Text = "EquipmentView";
            FormClosing += EquipmentView_FormClosing;
            Load += EquipmentView_Load;
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
            ((System.ComponentModel.ISupportInitialize)dgFloor).EndInit();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox gbSoldier;
    private System.Windows.Forms.Label lbEndurance;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label lbToughness;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label lbInsight;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Label lbAgility;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label lbStrength;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lbChest;
    private System.Windows.Forms.Label lbHead;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btDrop;
    private System.Windows.Forms.Button btEquip;
    private System.Windows.Forms.ListBox lbInventory;
    private System.Windows.Forms.Label lbEncumber;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbCapacity;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label lbWeight;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TabControl tbSkills;
    private System.Windows.Forms.TabPage tpMilitary;
    private System.Windows.Forms.TabPage tpUtility;
    private System.Windows.Forms.Label lbRFoot;
    private System.Windows.Forms.Label lbRLeg;
    private System.Windows.Forms.Label lbLFoot;
    private System.Windows.Forms.Label lbLLeg;
    private System.Windows.Forms.Label lbLHand;
    private System.Windows.Forms.Label lbRHand;
    private System.Windows.Forms.Label lbLArm;
    private System.Windows.Forms.Label lbRArm;
    private System.Windows.Forms.ListBox lbWeaponSkills;
    private System.Windows.Forms.ListBox lbUtilitySkills;
    private System.Windows.Forms.ListBox lbEquipped;
    private System.Windows.Forms.Label lbAttackTotal;
    private System.Windows.Forms.Label label38;
    private System.Windows.Forms.Label lbDefenceTotal;
    private System.Windows.Forms.Label lbHealthTotal;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label lbLevel;
    private System.Windows.Forms.Label label20;
    private System.Windows.Forms.Label lbArmour;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.PictureBox pbExperience;
    private System.Windows.Forms.Button btIncreaseSkill;
    private System.Windows.Forms.Button btAddNewSkill;
    private System.Windows.Forms.Label lbUnspent;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Button btScavenge;
    private System.Windows.Forms.Button btPickUpAll;
    private System.Windows.Forms.Button btPickUpSelected;
    private System.Windows.Forms.DataGridView dgFloor;
    private System.Windows.Forms.DataGridViewTextBoxColumn Item;
    private System.Windows.Forms.DataGridViewTextBoxColumn Count;
    private System.Windows.Forms.GroupBox groupBox3;
        private Label lbStamina;
        private Label label3;
    }
}