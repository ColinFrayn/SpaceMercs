namespace SpaceMercs.Dialogs {
  partial class ChooseCore {
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
            lbItems = new ListBox();
            btChooseCore = new Button();
            SuspendLayout();
            // 
            // lbItems
            // 
            lbItems.Dock = DockStyle.Top;
            lbItems.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbItems.FormattingEnabled = true;
            lbItems.Location = new Point(0, 0);
            lbItems.Margin = new Padding(5, 6, 5, 6);
            lbItems.Name = "lbItems";
            lbItems.Size = new Size(470, 596);
            lbItems.TabIndex = 0;
            lbItems.SelectedValueChanged += lbItems_SelectedValueChanged;
            // 
            // btChooseCore
            // 
            btChooseCore.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btChooseCore.Location = new Point(152, 660);
            btChooseCore.Margin = new Padding(5, 6, 5, 6);
            btChooseCore.Name = "btChooseCore";
            btChooseCore.Size = new Size(167, 62);
            btChooseCore.TabIndex = 1;
            btChooseCore.Text = "Use Item";
            btChooseCore.UseVisualStyleBackColor = true;
            btChooseCore.Click += btChooseCore_Click;
            // 
            // ChooseCore
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(470, 735);
            Controls.Add(btChooseCore);
            Controls.Add(lbItems);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(5, 6, 5, 6);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChooseCore";
            Text = "Use Item";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbItems;
    private System.Windows.Forms.Button btChooseCore;
  }
}