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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dgSoldiers = new System.Windows.Forms.DataGridView();
      this.SoldierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SoldierRace = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SoldierLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.SoldierStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btInventory = new System.Windows.Forms.Button();
      this.gbSoldier = new System.Windows.Forms.GroupBox();
      this.btColour = new System.Windows.Forms.Button();
      this.btIncreaseSkill = new System.Windows.Forms.Button();
      this.btAddNewSkill = new System.Windows.Forms.Button();
      this.lbUnspent = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.pbExperience = new System.Windows.Forms.PictureBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.lbLevel = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.label20 = new System.Windows.Forms.Label();
      this.lbAttackTotal = new System.Windows.Forms.Label();
      this.label38 = new System.Windows.Forms.Label();
      this.lbDefenceTotal = new System.Windows.Forms.Label();
      this.lbHealthTotal = new System.Windows.Forms.Label();
      this.lbArmour = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.lbDeactivated = new System.Windows.Forms.Label();
      this.lbRFoot = new System.Windows.Forms.Label();
      this.lbRLeg = new System.Windows.Forms.Label();
      this.lbLFoot = new System.Windows.Forms.Label();
      this.lbLLeg = new System.Windows.Forms.Label();
      this.lbLHand = new System.Windows.Forms.Label();
      this.lbRHand = new System.Windows.Forms.Label();
      this.lbLArm = new System.Windows.Forms.Label();
      this.lbRArm = new System.Windows.Forms.Label();
      this.tbSkills = new System.Windows.Forms.TabControl();
      this.tpMilitary = new System.Windows.Forms.TabPage();
      this.lbWeaponSkills = new System.Windows.Forms.ListBox();
      this.tpUtility = new System.Windows.Forms.TabPage();
      this.lbUtilitySkills = new System.Windows.Forms.ListBox();
      this.lbChest = new System.Windows.Forms.Label();
      this.lbHead = new System.Windows.Forms.Label();
      this.lbEndurance = new System.Windows.Forms.Label();
      this.label16 = new System.Windows.Forms.Label();
      this.lbToughness = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.lbInsight = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.lbAgility = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.lbStrength = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lbEquipped = new System.Windows.Forms.ListBox();
      this.lbEncumber = new System.Windows.Forms.Label();
      this.btDrop = new System.Windows.Forms.Button();
      this.btEquip = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.lbInventory = new System.Windows.Forms.ListBox();
      this.lbCapacity = new System.Windows.Forms.Label();
      this.btDropAll = new System.Windows.Forms.Button();
      this.label13 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.lbWeight = new System.Windows.Forms.Label();
      this.btDismiss = new System.Windows.Forms.Button();
      this.btDeactivate = new System.Windows.Forms.Button();
      this.lbCash = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lbLocation = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.lbRoster = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.lbActive = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.lbShip = new System.Windows.Forms.Label();
      this.lbBerths = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.lbInsufficientBerths = new System.Windows.Forms.Label();
      this.cdPickColour = new System.Windows.Forms.ColorDialog();
      ((System.ComponentModel.ISupportInitialize)(this.dgSoldiers)).BeginInit();
      this.gbSoldier.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbExperience)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.tbSkills.SuspendLayout();
      this.tpMilitary.SuspendLayout();
      this.tpUtility.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dgSoldiers
      // 
      this.dgSoldiers.AllowUserToAddRows = false;
      this.dgSoldiers.AllowUserToDeleteRows = false;
      this.dgSoldiers.AllowUserToResizeColumns = false;
      this.dgSoldiers.AllowUserToResizeRows = false;
      this.dgSoldiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgSoldiers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SoldierName,
            this.SoldierRace,
            this.SoldierLevel,
            this.SoldierStatus});
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dgSoldiers.DefaultCellStyle = dataGridViewCellStyle1;
      this.dgSoldiers.Location = new System.Drawing.Point(12, 111);
      this.dgSoldiers.MultiSelect = false;
      this.dgSoldiers.Name = "dgSoldiers";
      this.dgSoldiers.ReadOnly = true;
      this.dgSoldiers.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgSoldiers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.dgSoldiers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgSoldiers.Size = new System.Drawing.Size(537, 502);
      this.dgSoldiers.TabIndex = 0;
      this.dgSoldiers.SelectionChanged += new System.EventHandler(this.dgSoldiers_SelectionChanged);
      // 
      // SoldierName
      // 
      this.SoldierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.SoldierName.HeaderText = "Soldier Name";
      this.SoldierName.Name = "SoldierName";
      this.SoldierName.ReadOnly = true;
      // 
      // SoldierRace
      // 
      this.SoldierRace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.SoldierRace.HeaderText = "Race";
      this.SoldierRace.Name = "SoldierRace";
      this.SoldierRace.ReadOnly = true;
      this.SoldierRace.Width = 58;
      // 
      // SoldierLevel
      // 
      this.SoldierLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.SoldierLevel.HeaderText = "Level";
      this.SoldierLevel.Name = "SoldierLevel";
      this.SoldierLevel.ReadOnly = true;
      this.SoldierLevel.Width = 58;
      // 
      // SoldierStatus
      // 
      this.SoldierStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.SoldierStatus.HeaderText = "Status";
      this.SoldierStatus.Name = "SoldierStatus";
      this.SoldierStatus.ReadOnly = true;
      this.SoldierStatus.Width = 62;
      // 
      // btInventory
      // 
      this.btInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btInventory.Location = new System.Drawing.Point(323, 624);
      this.btInventory.Name = "btInventory";
      this.btInventory.Size = new System.Drawing.Size(226, 83);
      this.btInventory.TabIndex = 7;
      this.btInventory.Text = "View Ship Storage";
      this.btInventory.UseVisualStyleBackColor = true;
      this.btInventory.Click += new System.EventHandler(this.btInventory_Click);
      // 
      // gbSoldier
      // 
      this.gbSoldier.Controls.Add(this.btColour);
      this.gbSoldier.Controls.Add(this.btIncreaseSkill);
      this.gbSoldier.Controls.Add(this.btAddNewSkill);
      this.gbSoldier.Controls.Add(this.lbUnspent);
      this.gbSoldier.Controls.Add(this.label17);
      this.gbSoldier.Controls.Add(this.pbExperience);
      this.gbSoldier.Controls.Add(this.groupBox2);
      this.gbSoldier.Controls.Add(this.lbDeactivated);
      this.gbSoldier.Controls.Add(this.lbRFoot);
      this.gbSoldier.Controls.Add(this.lbRLeg);
      this.gbSoldier.Controls.Add(this.lbLFoot);
      this.gbSoldier.Controls.Add(this.lbLLeg);
      this.gbSoldier.Controls.Add(this.lbLHand);
      this.gbSoldier.Controls.Add(this.lbRHand);
      this.gbSoldier.Controls.Add(this.lbLArm);
      this.gbSoldier.Controls.Add(this.lbRArm);
      this.gbSoldier.Controls.Add(this.tbSkills);
      this.gbSoldier.Controls.Add(this.lbChest);
      this.gbSoldier.Controls.Add(this.lbHead);
      this.gbSoldier.Controls.Add(this.lbEndurance);
      this.gbSoldier.Controls.Add(this.label16);
      this.gbSoldier.Controls.Add(this.lbToughness);
      this.gbSoldier.Controls.Add(this.label14);
      this.gbSoldier.Controls.Add(this.lbInsight);
      this.gbSoldier.Controls.Add(this.label12);
      this.gbSoldier.Controls.Add(this.lbAgility);
      this.gbSoldier.Controls.Add(this.label10);
      this.gbSoldier.Controls.Add(this.lbStrength);
      this.gbSoldier.Controls.Add(this.label6);
      this.gbSoldier.Controls.Add(this.groupBox1);
      this.gbSoldier.Location = new System.Drawing.Point(561, 11);
      this.gbSoldier.Name = "gbSoldier";
      this.gbSoldier.Size = new System.Drawing.Size(404, 696);
      this.gbSoldier.TabIndex = 8;
      this.gbSoldier.TabStop = false;
      this.gbSoldier.Text = "Soldier Details";
      // 
      // btColour
      // 
      this.btColour.Location = new System.Drawing.Point(346, 161);
      this.btColour.Name = "btColour";
      this.btColour.Size = new System.Drawing.Size(41, 23);
      this.btColour.TabIndex = 92;
      this.btColour.UseVisualStyleBackColor = true;
      this.btColour.Click += new System.EventHandler(this.btColour_Click);
      // 
      // btIncreaseSkill
      // 
      this.btIncreaseSkill.Location = new System.Drawing.Point(113, 651);
      this.btIncreaseSkill.Margin = new System.Windows.Forms.Padding(0);
      this.btIncreaseSkill.Name = "btIncreaseSkill";
      this.btIncreaseSkill.Size = new System.Drawing.Size(45, 37);
      this.btIncreaseSkill.TabIndex = 91;
      this.btIncreaseSkill.Text = "Boost Skill";
      this.btIncreaseSkill.UseVisualStyleBackColor = true;
      this.btIncreaseSkill.Click += new System.EventHandler(this.btIncreaseSkill_Click);
      // 
      // btAddNewSkill
      // 
      this.btAddNewSkill.Location = new System.Drawing.Point(59, 651);
      this.btAddNewSkill.Name = "btAddNewSkill";
      this.btAddNewSkill.Size = new System.Drawing.Size(51, 38);
      this.btAddNewSkill.TabIndex = 90;
      this.btAddNewSkill.Text = "Add New";
      this.btAddNewSkill.UseVisualStyleBackColor = true;
      this.btAddNewSkill.Click += new System.EventHandler(this.btAddNewSkill_Click);
      // 
      // lbUnspent
      // 
      this.lbUnspent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbUnspent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbUnspent.Location = new System.Drawing.Point(15, 667);
      this.lbUnspent.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbUnspent.Name = "lbUnspent";
      this.lbUnspent.Size = new System.Drawing.Size(36, 22);
      this.lbUnspent.TabIndex = 89;
      this.lbUnspent.Text = "88";
      this.lbUnspent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(10, 650);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(47, 13);
      this.label17.TabIndex = 88;
      this.label17.Text = "Unspent";
      // 
      // pbExperience
      // 
      this.pbExperience.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pbExperience.Location = new System.Drawing.Point(176, 192);
      this.pbExperience.Name = "pbExperience";
      this.pbExperience.Size = new System.Drawing.Size(211, 21);
      this.pbExperience.TabIndex = 87;
      this.pbExperience.TabStop = false;
      this.pbExperience.Paint += new System.Windows.Forms.PaintEventHandler(this.pbExperience_Paint);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.lbLevel);
      this.groupBox2.Controls.Add(this.label15);
      this.groupBox2.Controls.Add(this.label20);
      this.groupBox2.Controls.Add(this.lbAttackTotal);
      this.groupBox2.Controls.Add(this.label38);
      this.groupBox2.Controls.Add(this.lbDefenceTotal);
      this.groupBox2.Controls.Add(this.lbHealthTotal);
      this.groupBox2.Controls.Add(this.lbArmour);
      this.groupBox2.Controls.Add(this.label11);
      this.groupBox2.Controls.Add(this.label18);
      this.groupBox2.Location = new System.Drawing.Point(8, 180);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(148, 181);
      this.groupBox2.TabIndex = 86;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Primary Stats";
      // 
      // lbLevel
      // 
      this.lbLevel.BackColor = System.Drawing.SystemColors.Window;
      this.lbLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLevel.Location = new System.Drawing.Point(84, 21);
      this.lbLevel.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbLevel.Name = "lbLevel";
      this.lbLevel.Size = new System.Drawing.Size(50, 24);
      this.lbLevel.TabIndex = 90;
      this.lbLevel.Text = "888";
      this.lbLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label15.Location = new System.Drawing.Point(33, 26);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(41, 16);
      this.label15.TabIndex = 49;
      this.label15.Text = "Level";
      this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label20.Location = new System.Drawing.Point(15, 120);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(59, 16);
      this.label20.TabIndex = 89;
      this.label20.Text = "Defence";
      this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbAttackTotal
      // 
      this.lbAttackTotal.BackColor = System.Drawing.Color.Tomato;
      this.lbAttackTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbAttackTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbAttackTotal.Location = new System.Drawing.Point(84, 84);
      this.lbAttackTotal.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbAttackTotal.Name = "lbAttackTotal";
      this.lbAttackTotal.Size = new System.Drawing.Size(50, 24);
      this.lbAttackTotal.TabIndex = 80;
      this.lbAttackTotal.Text = "888";
      this.lbAttackTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label38
      // 
      this.label38.AutoSize = true;
      this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label38.Location = new System.Drawing.Point(29, 89);
      this.label38.Name = "label38";
      this.label38.Size = new System.Drawing.Size(45, 16);
      this.label38.TabIndex = 76;
      this.label38.Text = "Attack";
      this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbDefenceTotal
      // 
      this.lbDefenceTotal.BackColor = System.Drawing.SystemColors.Highlight;
      this.lbDefenceTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbDefenceTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbDefenceTotal.Location = new System.Drawing.Point(84, 115);
      this.lbDefenceTotal.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbDefenceTotal.Name = "lbDefenceTotal";
      this.lbDefenceTotal.Size = new System.Drawing.Size(50, 24);
      this.lbDefenceTotal.TabIndex = 61;
      this.lbDefenceTotal.Text = "888";
      this.lbDefenceTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbHealthTotal
      // 
      this.lbHealthTotal.BackColor = System.Drawing.Color.LimeGreen;
      this.lbHealthTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbHealthTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbHealthTotal.Location = new System.Drawing.Point(84, 53);
      this.lbHealthTotal.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbHealthTotal.Name = "lbHealthTotal";
      this.lbHealthTotal.Size = new System.Drawing.Size(50, 24);
      this.lbHealthTotal.TabIndex = 60;
      this.lbHealthTotal.Text = "888";
      this.lbHealthTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbArmour
      // 
      this.lbArmour.BackColor = System.Drawing.SystemColors.ControlLight;
      this.lbArmour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbArmour.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbArmour.Location = new System.Drawing.Point(84, 146);
      this.lbArmour.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbArmour.Name = "lbArmour";
      this.lbArmour.Size = new System.Drawing.Size(50, 24);
      this.lbArmour.TabIndex = 88;
      this.lbArmour.Text = "888";
      this.lbArmour.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(27, 57);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(47, 16);
      this.label11.TabIndex = 48;
      this.label11.Text = "Health";
      this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label18.Location = new System.Drawing.Point(23, 151);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(51, 16);
      this.label18.TabIndex = 87;
      this.label18.Text = "Armour";
      this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbDeactivated
      // 
      this.lbDeactivated.AutoSize = true;
      this.lbDeactivated.ForeColor = System.Drawing.Color.Red;
      this.lbDeactivated.Location = new System.Drawing.Point(7, 17);
      this.lbDeactivated.Name = "lbDeactivated";
      this.lbDeactivated.Size = new System.Drawing.Size(111, 13);
      this.lbDeactivated.TabIndex = 85;
      this.lbDeactivated.Text = "Soldier is deactivated!";
      // 
      // lbRFoot
      // 
      this.lbRFoot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbRFoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRFoot.Location = new System.Drawing.Point(282, 164);
      this.lbRFoot.Margin = new System.Windows.Forms.Padding(0);
      this.lbRFoot.Name = "lbRFoot";
      this.lbRFoot.Size = new System.Drawing.Size(26, 20);
      this.lbRFoot.TabIndex = 47;
      this.lbRFoot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbRLeg
      // 
      this.lbRLeg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbRLeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRLeg.Location = new System.Drawing.Point(282, 110);
      this.lbRLeg.Margin = new System.Windows.Forms.Padding(0);
      this.lbRLeg.Name = "lbRLeg";
      this.lbRLeg.Size = new System.Drawing.Size(26, 54);
      this.lbRLeg.TabIndex = 46;
      this.lbRLeg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbLFoot
      // 
      this.lbLFoot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLFoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLFoot.Location = new System.Drawing.Point(252, 164);
      this.lbLFoot.Margin = new System.Windows.Forms.Padding(0);
      this.lbLFoot.Name = "lbLFoot";
      this.lbLFoot.Size = new System.Drawing.Size(26, 20);
      this.lbLFoot.TabIndex = 45;
      this.lbLFoot.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbLLeg
      // 
      this.lbLLeg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLLeg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLLeg.Location = new System.Drawing.Point(252, 110);
      this.lbLLeg.Margin = new System.Windows.Forms.Padding(0);
      this.lbLLeg.Name = "lbLLeg";
      this.lbLLeg.Size = new System.Drawing.Size(26, 54);
      this.lbLLeg.TabIndex = 44;
      this.lbLLeg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbLHand
      // 
      this.lbLHand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLHand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLHand.Location = new System.Drawing.Point(225, 108);
      this.lbLHand.Margin = new System.Windows.Forms.Padding(0);
      this.lbLHand.Name = "lbLHand";
      this.lbLHand.Size = new System.Drawing.Size(26, 24);
      this.lbLHand.TabIndex = 43;
      this.lbLHand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbRHand
      // 
      this.lbRHand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbRHand.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRHand.Location = new System.Drawing.Point(309, 108);
      this.lbRHand.Margin = new System.Windows.Forms.Padding(0);
      this.lbRHand.Name = "lbRHand";
      this.lbRHand.Size = new System.Drawing.Size(26, 24);
      this.lbRHand.TabIndex = 42;
      this.lbRHand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbLArm
      // 
      this.lbLArm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLArm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLArm.Location = new System.Drawing.Point(225, 46);
      this.lbLArm.Margin = new System.Windows.Forms.Padding(0);
      this.lbLArm.Name = "lbLArm";
      this.lbLArm.Size = new System.Drawing.Size(26, 62);
      this.lbLArm.TabIndex = 41;
      this.lbLArm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbRArm
      // 
      this.lbRArm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbRArm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRArm.Location = new System.Drawing.Point(309, 46);
      this.lbRArm.Margin = new System.Windows.Forms.Padding(0);
      this.lbRArm.Name = "lbRArm";
      this.lbRArm.Size = new System.Drawing.Size(26, 62);
      this.lbRArm.TabIndex = 40;
      this.lbRArm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // tbSkills
      // 
      this.tbSkills.Controls.Add(this.tpMilitary);
      this.tbSkills.Controls.Add(this.tpUtility);
      this.tbSkills.Location = new System.Drawing.Point(9, 376);
      this.tbSkills.Name = "tbSkills";
      this.tbSkills.SelectedIndex = 0;
      this.tbSkills.Size = new System.Drawing.Size(156, 272);
      this.tbSkills.TabIndex = 39;
      this.tbSkills.SelectedIndexChanged += new System.EventHandler(this.tbSkills_SelectedIndexChanged);
      // 
      // tpMilitary
      // 
      this.tpMilitary.Controls.Add(this.lbWeaponSkills);
      this.tpMilitary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tpMilitary.Location = new System.Drawing.Point(4, 22);
      this.tpMilitary.Name = "tpMilitary";
      this.tpMilitary.Padding = new System.Windows.Forms.Padding(3);
      this.tpMilitary.Size = new System.Drawing.Size(148, 246);
      this.tpMilitary.TabIndex = 0;
      this.tpMilitary.Text = "Weapon Skills";
      this.tpMilitary.UseVisualStyleBackColor = true;
      // 
      // lbWeaponSkills
      // 
      this.lbWeaponSkills.FormattingEnabled = true;
      this.lbWeaponSkills.ItemHeight = 16;
      this.lbWeaponSkills.Location = new System.Drawing.Point(0, 2);
      this.lbWeaponSkills.Name = "lbWeaponSkills";
      this.lbWeaponSkills.Size = new System.Drawing.Size(145, 244);
      this.lbWeaponSkills.TabIndex = 0;
      // 
      // tpUtility
      // 
      this.tpUtility.Controls.Add(this.lbUtilitySkills);
      this.tpUtility.Location = new System.Drawing.Point(4, 22);
      this.tpUtility.Name = "tpUtility";
      this.tpUtility.Padding = new System.Windows.Forms.Padding(3);
      this.tpUtility.Size = new System.Drawing.Size(148, 246);
      this.tpUtility.TabIndex = 1;
      this.tpUtility.Text = "Utility Skills";
      this.tpUtility.UseVisualStyleBackColor = true;
      // 
      // lbUtilitySkills
      // 
      this.lbUtilitySkills.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbUtilitySkills.FormattingEnabled = true;
      this.lbUtilitySkills.ItemHeight = 16;
      this.lbUtilitySkills.Location = new System.Drawing.Point(0, 2);
      this.lbUtilitySkills.Name = "lbUtilitySkills";
      this.lbUtilitySkills.Size = new System.Drawing.Size(145, 244);
      this.lbUtilitySkills.TabIndex = 1;
      this.lbUtilitySkills.SelectedIndexChanged += new System.EventHandler(this.lbUtilitySkills_SelectedIndexChanged);
      // 
      // lbChest
      // 
      this.lbChest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbChest.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbChest.Location = new System.Drawing.Point(251, 46);
      this.lbChest.Margin = new System.Windows.Forms.Padding(0);
      this.lbChest.Name = "lbChest";
      this.lbChest.Size = new System.Drawing.Size(58, 64);
      this.lbChest.TabIndex = 35;
      this.lbChest.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbHead
      // 
      this.lbHead.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbHead.Location = new System.Drawing.Point(262, 18);
      this.lbHead.Margin = new System.Windows.Forms.Padding(0);
      this.lbHead.Name = "lbHead";
      this.lbHead.Size = new System.Drawing.Size(36, 24);
      this.lbHead.TabIndex = 34;
      this.lbHead.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbEndurance
      // 
      this.lbEndurance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbEndurance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbEndurance.Location = new System.Drawing.Point(95, 138);
      this.lbEndurance.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbEndurance.Name = "lbEndurance";
      this.lbEndurance.Size = new System.Drawing.Size(47, 22);
      this.lbEndurance.TabIndex = 29;
      this.lbEndurance.Text = "888";
      this.lbEndurance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label16.Location = new System.Drawing.Point(16, 141);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(73, 16);
      this.label16.TabIndex = 28;
      this.label16.Text = "Endurance";
      this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbToughness
      // 
      this.lbToughness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbToughness.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbToughness.Location = new System.Drawing.Point(95, 112);
      this.lbToughness.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbToughness.Name = "lbToughness";
      this.lbToughness.Size = new System.Drawing.Size(47, 22);
      this.lbToughness.TabIndex = 27;
      this.lbToughness.Text = "888";
      this.lbToughness.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label14.Location = new System.Drawing.Point(13, 115);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(76, 16);
      this.label14.TabIndex = 26;
      this.label14.Text = "Toughness";
      this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbInsight
      // 
      this.lbInsight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbInsight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbInsight.Location = new System.Drawing.Point(95, 86);
      this.lbInsight.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbInsight.Name = "lbInsight";
      this.lbInsight.Size = new System.Drawing.Size(47, 22);
      this.lbInsight.TabIndex = 25;
      this.lbInsight.Text = "888";
      this.lbInsight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.Location = new System.Drawing.Point(36, 89);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(53, 16);
      this.label12.TabIndex = 24;
      this.label12.Text = "Insight";
      this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbAgility
      // 
      this.lbAgility.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbAgility.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbAgility.Location = new System.Drawing.Point(95, 60);
      this.lbAgility.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbAgility.Name = "lbAgility";
      this.lbAgility.Size = new System.Drawing.Size(47, 22);
      this.lbAgility.TabIndex = 23;
      this.lbAgility.Text = "888";
      this.lbAgility.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(45, 63);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(44, 16);
      this.label10.TabIndex = 22;
      this.label10.Text = "Agility";
      this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbStrength
      // 
      this.lbStrength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbStrength.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbStrength.Location = new System.Drawing.Point(95, 34);
      this.lbStrength.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbStrength.Name = "lbStrength";
      this.lbStrength.Size = new System.Drawing.Size(47, 22);
      this.lbStrength.TabIndex = 21;
      this.lbStrength.Text = "888";
      this.lbStrength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(32, 37);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(57, 16);
      this.label6.TabIndex = 20;
      this.label6.Text = "Strength";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lbEquipped);
      this.groupBox1.Controls.Add(this.lbEncumber);
      this.groupBox1.Controls.Add(this.btDrop);
      this.groupBox1.Controls.Add(this.btEquip);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.lbInventory);
      this.groupBox1.Controls.Add(this.lbCapacity);
      this.groupBox1.Controls.Add(this.btDropAll);
      this.groupBox1.Controls.Add(this.label13);
      this.groupBox1.Controls.Add(this.label8);
      this.groupBox1.Controls.Add(this.lbWeight);
      this.groupBox1.Location = new System.Drawing.Point(176, 218);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(211, 464);
      this.groupBox1.TabIndex = 33;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Inventory";
      // 
      // lbEquipped
      // 
      this.lbEquipped.FormattingEnabled = true;
      this.lbEquipped.HorizontalScrollbar = true;
      this.lbEquipped.Location = new System.Drawing.Point(13, 273);
      this.lbEquipped.Name = "lbEquipped";
      this.lbEquipped.Size = new System.Drawing.Size(183, 121);
      this.lbEquipped.TabIndex = 45;
      this.lbEquipped.SelectedIndexChanged += new System.EventHandler(this.lbEquipped_SelectedIndexChanged);
      this.lbEquipped.DoubleClick += new System.EventHandler(this.lbEquipped_DoubleClick);
      // 
      // lbEncumber
      // 
      this.lbEncumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbEncumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbEncumber.Location = new System.Drawing.Point(144, 424);
      this.lbEncumber.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbEncumber.Name = "lbEncumber";
      this.lbEncumber.Size = new System.Drawing.Size(50, 26);
      this.lbEncumber.TabIndex = 44;
      this.lbEncumber.Text = "100%";
      this.lbEncumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btDrop
      // 
      this.btDrop.Location = new System.Drawing.Point(78, 240);
      this.btDrop.Name = "btDrop";
      this.btDrop.Size = new System.Drawing.Size(55, 23);
      this.btDrop.TabIndex = 2;
      this.btDrop.Text = "Drop";
      this.btDrop.UseVisualStyleBackColor = true;
      this.btDrop.Click += new System.EventHandler(this.btDrop_Click);
      // 
      // btEquip
      // 
      this.btEquip.Location = new System.Drawing.Point(13, 240);
      this.btEquip.Name = "btEquip";
      this.btEquip.Size = new System.Drawing.Size(55, 23);
      this.btEquip.TabIndex = 1;
      this.btEquip.Text = "Equip";
      this.btEquip.UseVisualStyleBackColor = true;
      this.btEquip.Click += new System.EventHandler(this.btEquip_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(141, 404);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(56, 15);
      this.label1.TabIndex = 43;
      this.label1.Text = "Encumb.";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbInventory
      // 
      this.lbInventory.FormattingEnabled = true;
      this.lbInventory.HorizontalScrollbar = true;
      this.lbInventory.Location = new System.Drawing.Point(13, 20);
      this.lbInventory.Name = "lbInventory";
      this.lbInventory.Size = new System.Drawing.Size(183, 212);
      this.lbInventory.TabIndex = 0;
      this.lbInventory.SelectedIndexChanged += new System.EventHandler(this.lbInventory_SelectedIndexChanged);
      this.lbInventory.DoubleClick += new System.EventHandler(this.lbInventory_DoubleClick);
      // 
      // lbCapacity
      // 
      this.lbCapacity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbCapacity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCapacity.Location = new System.Drawing.Point(61, 431);
      this.lbCapacity.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbCapacity.Name = "lbCapacity";
      this.lbCapacity.Size = new System.Drawing.Size(68, 22);
      this.lbCapacity.TabIndex = 42;
      this.lbCapacity.Text = "888.88 kg";
      this.lbCapacity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btDropAll
      // 
      this.btDropAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btDropAll.Location = new System.Drawing.Point(142, 240);
      this.btDropAll.Name = "btDropAll";
      this.btDropAll.Size = new System.Drawing.Size(55, 23);
      this.btDropAll.TabIndex = 31;
      this.btDropAll.Text = "Drop All";
      this.btDropAll.UseVisualStyleBackColor = true;
      this.btDropAll.Click += new System.EventHandler(this.btDropAllClick);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label13.Location = new System.Drawing.Point(8, 434);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(53, 15);
      this.label13.TabIndex = 41;
      this.label13.Text = "Capacity";
      this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(16, 407);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(45, 15);
      this.label8.TabIndex = 39;
      this.label8.Text = "Weight";
      this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbWeight
      // 
      this.lbWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbWeight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbWeight.Location = new System.Drawing.Point(61, 404);
      this.lbWeight.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbWeight.Name = "lbWeight";
      this.lbWeight.Size = new System.Drawing.Size(68, 22);
      this.lbWeight.TabIndex = 40;
      this.lbWeight.Text = "888.88 kg";
      this.lbWeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btDismiss
      // 
      this.btDismiss.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btDismiss.Location = new System.Drawing.Point(459, 50);
      this.btDismiss.Name = "btDismiss";
      this.btDismiss.Size = new System.Drawing.Size(90, 28);
      this.btDismiss.TabIndex = 32;
      this.btDismiss.Text = "Dismiss";
      this.btDismiss.UseVisualStyleBackColor = true;
      this.btDismiss.Click += new System.EventHandler(this.btDismiss_Click);
      // 
      // btDeactivate
      // 
      this.btDeactivate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btDeactivate.Location = new System.Drawing.Point(459, 20);
      this.btDeactivate.Name = "btDeactivate";
      this.btDeactivate.Size = new System.Drawing.Size(90, 28);
      this.btDeactivate.TabIndex = 30;
      this.btDeactivate.Text = "Deactivate";
      this.btDeactivate.UseVisualStyleBackColor = true;
      this.btDeactivate.Click += new System.EventHandler(this.btDeactivate_Click);
      // 
      // lbCash
      // 
      this.lbCash.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCash.Location = new System.Drawing.Point(78, 20);
      this.lbCash.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbCash.Name = "lbCash";
      this.lbCash.Size = new System.Drawing.Size(96, 22);
      this.lbCash.TabIndex = 10;
      this.lbCash.Text = "8888888.88";
      this.lbCash.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(35, 23);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(39, 16);
      this.label3.TabIndex = 9;
      this.label3.Text = "Cash";
      // 
      // lbLocation
      // 
      this.lbLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbLocation.Location = new System.Drawing.Point(78, 56);
      this.lbLocation.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbLocation.Name = "lbLocation";
      this.lbLocation.Size = new System.Drawing.Size(107, 22);
      this.lbLocation.TabIndex = 12;
      this.lbLocation.Text = "(888.88,888.88)";
      this.lbLocation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(15, 59);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(59, 16);
      this.label4.TabIndex = 11;
      this.label4.Text = "Location";
      // 
      // lbRoster
      // 
      this.lbRoster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbRoster.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRoster.Location = new System.Drawing.Point(314, 20);
      this.lbRoster.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbRoster.Name = "lbRoster";
      this.lbRoster.Size = new System.Drawing.Size(96, 22);
      this.lbRoster.TabIndex = 14;
      this.lbRoster.Text = "888 soldiers";
      this.lbRoster.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(258, 23);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(48, 16);
      this.label5.TabIndex = 13;
      this.label5.Text = "Roster";
      // 
      // lbActive
      // 
      this.lbActive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbActive.Location = new System.Drawing.Point(314, 56);
      this.lbActive.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbActive.Name = "lbActive";
      this.lbActive.Size = new System.Drawing.Size(96, 22);
      this.lbActive.TabIndex = 16;
      this.lbActive.Text = "888 soldiers";
      this.lbActive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(258, 59);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(45, 16);
      this.label7.TabIndex = 15;
      this.label7.Text = "Active";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(26, 633);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Ship";
      // 
      // lbShip
      // 
      this.lbShip.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbShip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbShip.Location = new System.Drawing.Point(67, 630);
      this.lbShip.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbShip.Name = "lbShip";
      this.lbShip.Size = new System.Drawing.Size(236, 22);
      this.lbShip.TabIndex = 5;
      this.lbShip.Text = "Ship Design Type";
      this.lbShip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lbBerths
      // 
      this.lbBerths.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lbBerths.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbBerths.Location = new System.Drawing.Point(67, 666);
      this.lbBerths.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
      this.lbBerths.Name = "lbBerths";
      this.lbBerths.Size = new System.Drawing.Size(41, 22);
      this.lbBerths.TabIndex = 18;
      this.lbBerths.Text = "888";
      this.lbBerths.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(15, 669);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(46, 16);
      this.label9.TabIndex = 17;
      this.label9.Text = "Berths";
      // 
      // lbInsufficientBerths
      // 
      this.lbInsufficientBerths.AutoSize = true;
      this.lbInsufficientBerths.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbInsufficientBerths.ForeColor = System.Drawing.Color.Red;
      this.lbInsufficientBerths.Location = new System.Drawing.Point(128, 669);
      this.lbInsufficientBerths.Name = "lbInsufficientBerths";
      this.lbInsufficientBerths.Size = new System.Drawing.Size(113, 16);
      this.lbInsufficientBerths.TabIndex = 19;
      this.lbInsufficientBerths.Text = "Insufficient Berths!";
      // 
      // cdPickColour
      // 
      this.cdPickColour.AnyColor = true;
      this.cdPickColour.Color = System.Drawing.Color.Blue;
      this.cdPickColour.SolidColorOnly = true;
      // 
      // TeamView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(976, 719);
      this.Controls.Add(this.gbSoldier);
      this.Controls.Add(this.lbInsufficientBerths);
      this.Controls.Add(this.lbBerths);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.lbActive);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.lbRoster);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.lbLocation);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.lbCash);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.btInventory);
      this.Controls.Add(this.lbShip);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.dgSoldiers);
      this.Controls.Add(this.btDeactivate);
      this.Controls.Add(this.btDismiss);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.Name = "TeamView";
      this.Text = "TeamView";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TeamView_FormClosing);
      this.Load += new System.EventHandler(this.TeamView_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dgSoldiers)).EndInit();
      this.gbSoldier.ResumeLayout(false);
      this.gbSoldier.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbExperience)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.tbSkills.ResumeLayout(false);
      this.tpMilitary.ResumeLayout(false);
      this.tpUtility.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dgSoldiers;
    private System.Windows.Forms.Button btInventory;
    private System.Windows.Forms.GroupBox gbSoldier;
    private System.Windows.Forms.Label lbCash;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lbLocation;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lbRoster;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label lbActive;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label lbShip;
    private System.Windows.Forms.Label lbBerths;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label lbInsufficientBerths;
    private System.Windows.Forms.DataGridViewTextBoxColumn SoldierName;
    private System.Windows.Forms.DataGridViewTextBoxColumn SoldierRace;
    private System.Windows.Forms.DataGridViewTextBoxColumn SoldierLevel;
    private System.Windows.Forms.DataGridViewTextBoxColumn SoldierStatus;
    private System.Windows.Forms.Button btDismiss;
    private System.Windows.Forms.Button btDropAll;
    private System.Windows.Forms.Button btDeactivate;
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
    private System.Windows.Forms.Label lbDeactivated;
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
    private System.Windows.Forms.ColorDialog cdPickColour;
    private System.Windows.Forms.Button btColour;
  }
}