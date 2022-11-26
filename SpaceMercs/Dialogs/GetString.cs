using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
  public partial class GetString : Form {
    public string strText = "";
    public GetString() {
      InitializeComponent();
    }

    public void UpdateString(string strNew) {
      strText = strNew;
      textBox.Text = strNew;
    }

    private void textBox_TextChanged(object sender, EventArgs e) {
      strText = textBox.Text;
    }

    private void buttonAccept_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e) {
      this.Close();
    }
  }
}
