using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceMercs.Dialogs {
  class CommonDialog : Form {
    public virtual void ClockTick() {}
    public virtual void DrawAll() {}
  }
}
