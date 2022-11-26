namespace SpaceMercs.Dialogs {
  partial class UseItem {
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
      this.lbItems = new System.Windows.Forms.ListBox();
      this.btUseItem = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lbItems
      // 
      this.lbItems.Dock = System.Windows.Forms.DockStyle.Top;
      this.lbItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbItems.FormattingEnabled = true;
      this.lbItems.ItemHeight = 25;
      this.lbItems.Location = new System.Drawing.Point(0, 0);
      this.lbItems.Name = "lbItems";
      this.lbItems.Size = new System.Drawing.Size(282, 329);
      this.lbItems.TabIndex = 0;
      this.lbItems.SelectedValueChanged += new System.EventHandler(this.lbItems_SelectedValueChanged);
      // 
      // btUseItem
      // 
      this.btUseItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btUseItem.Location = new System.Drawing.Point(91, 343);
      this.btUseItem.Name = "btUseItem";
      this.btUseItem.Size = new System.Drawing.Size(100, 32);
      this.btUseItem.TabIndex = 1;
      this.btUseItem.Text = "Use item";
      this.btUseItem.UseVisualStyleBackColor = true;
      this.btUseItem.Click += new System.EventHandler(this.btUseItem_Click);
      // 
      // UseItem
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(282, 382);
      this.Controls.Add(this.btUseItem);
      this.Controls.Add(this.lbItems);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "UseItem";
      this.Text = "Use Item";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox lbItems;
    private System.Windows.Forms.Button btUseItem;
  }
}