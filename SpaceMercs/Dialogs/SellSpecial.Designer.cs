namespace SpaceMercs.Dialogs {
    partial class SellSpecial {
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
            lbArmourType = new Label();
            btCancel = new Button();
            btSell = new Button();
            cbMaxQuality = new ComboBox();
            lbArmourLabel = new Label();
            lbItemCount = new Label();
            lbDiffLabel = new Label();
            lbProceeds = new Label();
            label1 = new Label();
            cbWeapons = new CheckBox();
            cbItems = new CheckBox();
            cbMaterials = new CheckBox();
            cbArmour = new CheckBox();
            cbIncludeInventory = new CheckBox();
            groupBox1 = new GroupBox();
            SuspendLayout();
            // 
            // lbArmourType
            // 
            lbArmourType.BorderStyle = BorderStyle.FixedSingle;
            lbArmourType.Font = new Font("Microsoft Sans Serif", 9.75F);
            lbArmourType.Location = new Point(119, 17);
            lbArmourType.Margin = new Padding(6, 0, 6, 0);
            lbArmourType.Name = "lbArmourType";
            lbArmourType.Size = new Size(265, 52);
            lbArmourType.TabIndex = 1;
            lbArmourType.Text = "Sell Options";
            lbArmourType.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btCancel
            // 
            btCancel.Location = new Point(301, 460);
            btCancel.Margin = new Padding(6, 5, 6, 5);
            btCancel.Name = "btCancel";
            btCancel.Size = new Size(134, 52);
            btCancel.TabIndex = 14;
            btCancel.Text = "Cancel";
            btCancel.UseVisualStyleBackColor = true;
            btCancel.Click += btCancel_Click;
            // 
            // btSell
            // 
            btSell.Location = new Point(87, 462);
            btSell.Margin = new Padding(6, 5, 6, 5);
            btSell.Name = "btSell";
            btSell.Size = new Size(134, 52);
            btSell.TabIndex = 13;
            btSell.Text = "Sell";
            btSell.UseVisualStyleBackColor = true;
            btSell.Click += btSell_Click;
            // 
            // cbMaxQuality
            // 
            cbMaxQuality.Font = new Font("Microsoft Sans Serif", 9.75F);
            cbMaxQuality.FormattingEnabled = true;
            cbMaxQuality.Location = new Point(234, 115);
            cbMaxQuality.Margin = new Padding(6, 5, 6, 5);
            cbMaxQuality.Name = "cbMaxQuality";
            cbMaxQuality.Size = new Size(200, 33);
            cbMaxQuality.TabIndex = 15;
            cbMaxQuality.SelectedIndexChanged += cbMaxQuality_SelectedIndexChanged;
            // 
            // lbArmourLabel
            // 
            lbArmourLabel.AutoSize = true;
            lbArmourLabel.Location = new Point(16, 398);
            lbArmourLabel.Margin = new Padding(6, 0, 6, 0);
            lbArmourLabel.Name = "lbArmourLabel";
            lbArmourLabel.Size = new Size(101, 25);
            lbArmourLabel.TabIndex = 19;
            lbArmourLabel.Text = "Item Count";
            // 
            // lbItemCount
            // 
            lbItemCount.BorderStyle = BorderStyle.FixedSingle;
            lbItemCount.Font = new Font("Microsoft Sans Serif", 12F);
            lbItemCount.Location = new Point(120, 380);
            lbItemCount.Margin = new Padding(6, 0, 6, 0);
            lbItemCount.Name = "lbItemCount";
            lbItemCount.Size = new Size(72, 55);
            lbItemCount.TabIndex = 18;
            lbItemCount.Text = "8888";
            lbItemCount.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbDiffLabel
            // 
            lbDiffLabel.AutoSize = true;
            lbDiffLabel.Location = new Point(220, 398);
            lbDiffLabel.Margin = new Padding(6, 0, 6, 0);
            lbDiffLabel.Name = "lbDiffLabel";
            lbDiffLabel.Size = new Size(84, 25);
            lbDiffLabel.TabIndex = 22;
            lbDiffLabel.Text = "Proceeds";
            // 
            // lbProceeds
            // 
            lbProceeds.BorderStyle = BorderStyle.FixedSingle;
            lbProceeds.Font = new Font("Microsoft Sans Serif", 12F);
            lbProceeds.Location = new Point(308, 380);
            lbProceeds.Margin = new Padding(6, 0, 6, 0);
            lbProceeds.Name = "lbProceeds";
            lbProceeds.Size = new Size(153, 55);
            lbProceeds.TabIndex = 21;
            lbProceeds.Text = "88888.88 cr";
            lbProceeds.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(64, 118);
            label1.Margin = new Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new Size(167, 28);
            label1.TabIndex = 23;
            label1.Text = "Maximum Quality";
            // 
            // cbWeapons
            // 
            cbWeapons.AutoSize = true;
            cbWeapons.CheckAlign = ContentAlignment.MiddleRight;
            cbWeapons.Font = new Font("Segoe UI", 9.75F);
            cbWeapons.Location = new Point(121, 267);
            cbWeapons.Margin = new Padding(4, 5, 4, 5);
            cbWeapons.Name = "cbWeapons";
            cbWeapons.Size = new Size(119, 32);
            cbWeapons.TabIndex = 24;
            cbWeapons.Text = "Weapons";
            cbWeapons.TextAlign = ContentAlignment.MiddleRight;
            cbWeapons.UseVisualStyleBackColor = true;
            cbWeapons.CheckedChanged += cbWeapons_CheckedChanged;
            // 
            // cbItems
            // 
            cbItems.AutoSize = true;
            cbItems.CheckAlign = ContentAlignment.MiddleRight;
            cbItems.Font = new Font("Segoe UI", 9.75F);
            cbItems.Location = new Point(301, 267);
            cbItems.Margin = new Padding(4, 5, 4, 5);
            cbItems.Name = "cbItems";
            cbItems.Size = new Size(85, 32);
            cbItems.TabIndex = 25;
            cbItems.Text = "Items";
            cbItems.TextAlign = ContentAlignment.MiddleRight;
            cbItems.UseVisualStyleBackColor = true;
            cbItems.CheckedChanged += cbItems_CheckedChanged;
            // 
            // cbMaterials
            // 
            cbMaterials.AutoSize = true;
            cbMaterials.CheckAlign = ContentAlignment.MiddleRight;
            cbMaterials.Font = new Font("Segoe UI", 9.75F);
            cbMaterials.Location = new Point(269, 312);
            cbMaterials.Margin = new Padding(4, 5, 4, 5);
            cbMaterials.Name = "cbMaterials";
            cbMaterials.Size = new Size(118, 32);
            cbMaterials.TabIndex = 26;
            cbMaterials.Text = "Materials";
            cbMaterials.TextAlign = ContentAlignment.MiddleRight;
            cbMaterials.UseVisualStyleBackColor = true;
            cbMaterials.CheckedChanged += cbMaterials_CheckedChanged;
            // 
            // cbArmour
            // 
            cbArmour.AutoSize = true;
            cbArmour.CheckAlign = ContentAlignment.MiddleRight;
            cbArmour.Font = new Font("Segoe UI", 9.75F);
            cbArmour.Location = new Point(136, 312);
            cbArmour.Margin = new Padding(4, 5, 4, 5);
            cbArmour.Name = "cbArmour";
            cbArmour.Size = new Size(105, 32);
            cbArmour.TabIndex = 27;
            cbArmour.Text = "Armour";
            cbArmour.TextAlign = ContentAlignment.MiddleRight;
            cbArmour.UseVisualStyleBackColor = true;
            cbArmour.CheckedChanged += cbArmour_CheckedChanged;
            // 
            // cbIncludeInventory
            // 
            cbIncludeInventory.AutoSize = true;
            cbIncludeInventory.CheckAlign = ContentAlignment.MiddleRight;
            cbIncludeInventory.Font = new Font("Segoe UI", 9.75F);
            cbIncludeInventory.Location = new Point(141, 183);
            cbIncludeInventory.Margin = new Padding(4, 5, 4, 5);
            cbIncludeInventory.Name = "cbIncludeInventory";
            cbIncludeInventory.Size = new Size(256, 32);
            cbIncludeInventory.TabIndex = 28;
            cbIncludeInventory.Text = "Include Soldier Inventory";
            cbIncludeInventory.TextAlign = ContentAlignment.MiddleRight;
            cbIncludeInventory.UseVisualStyleBackColor = true;
            cbIncludeInventory.CheckedChanged += cbIncludeInventory_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Location = new Point(19, 233);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 5, 4, 5);
            groupBox1.Size = new Size(443, 132);
            groupBox1.TabIndex = 29;
            groupBox1.TabStop = false;
            groupBox1.Text = "Item Types";
            // 
            // SellSpecial
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(494, 535);
            Controls.Add(cbIncludeInventory);
            Controls.Add(cbArmour);
            Controls.Add(cbMaterials);
            Controls.Add(cbItems);
            Controls.Add(cbWeapons);
            Controls.Add(label1);
            Controls.Add(lbDiffLabel);
            Controls.Add(lbProceeds);
            Controls.Add(lbArmourLabel);
            Controls.Add(lbItemCount);
            Controls.Add(cbMaxQuality);
            Controls.Add(btCancel);
            Controls.Add(btSell);
            Controls.Add(lbArmourType);
            Controls.Add(groupBox1);
            Margin = new Padding(6, 5, 6, 5);
            Name = "SellSpecial";
            Text = "Select Armour Material";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbArmourType;
        private Button btCancel;
        private Button btSell;
        private ComboBox cbMaxQuality;
        private Label lbArmourLabel;
        private Label lbItemCount;
        private Label lbDiffLabel;
        private Label lbProceeds;
        private Label label1;
        private CheckBox cbWeapons;
        private CheckBox cbItems;
        private CheckBox cbMaterials;
        private CheckBox cbArmour;
        private CheckBox cbIncludeInventory;
        private GroupBox groupBox1;
    }
}