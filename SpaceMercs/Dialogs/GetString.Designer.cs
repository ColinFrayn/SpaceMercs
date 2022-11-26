namespace SpaceMercs.Dialogs {
  partial class GetString {
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
      this.textBox = new System.Windows.Forms.TextBox();
      this.buttonAccept = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // textBox
      // 
      this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox.Location = new System.Drawing.Point(14, 17);
      this.textBox.MaxLength = 40;
      this.textBox.Name = "textBox";
      this.textBox.Size = new System.Drawing.Size(317, 26);
      this.textBox.TabIndex = 0;
      this.textBox.Text = "Input String";
      this.textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.textBox.WordWrap = false;
      this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
      // 
      // buttonAccept
      // 
      this.buttonAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.buttonAccept.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonAccept.Location = new System.Drawing.Point(14, 58);
      this.buttonAccept.Name = "buttonAccept";
      this.buttonAccept.Size = new System.Drawing.Size(75, 23);
      this.buttonAccept.TabIndex = 1;
      this.buttonAccept.Text = "Accept";
      this.buttonAccept.UseVisualStyleBackColor = true;
      this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
      // 
      // buttonCancel
      // 
      this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonCancel.Location = new System.Drawing.Point(259, 58);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 2;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // GetString
      // 
      this.AcceptButton = this.buttonAccept;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.buttonCancel;
      this.ClientSize = new System.Drawing.Size(346, 93);
      this.ControlBox = false;
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonAccept);
      this.Controls.Add(this.textBox);
      this.Name = "GetString";
      this.Text = "GetString";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.Button buttonAccept;
    private System.Windows.Forms.Button buttonCancel;
  }
}