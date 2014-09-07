using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AddonHelper
{
  public partial class FormFilename : Form
  {
    public CustomFilenameInfo ReturnValue = null;

    public FormFilename()
    {
      InitializeComponent();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter) {
        buttonOK.PerformClick();
        return true;
      } else if (keyData == Keys.Escape) {
        buttonCancel.PerformClick();
        return true;
      }

      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      ReturnValue = new CustomFilenameInfo() {
        UserInput = textFilename.Text,
        Checkbox = checkBox.Checked
      };
      this.Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
    }

    private void FormFilename_Load(object sender, EventArgs e)
    {
      textFilename.Select(0, textFilename.Text.IndexOf('.'));
    }
  }
}
