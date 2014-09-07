using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClipBoard
{
  public partial class FormSettings : Form
  {
    ClipBoard mainClass;

    public FormSettings(ClipBoard mainClass)
    {
      InitializeComponent();

      this.mainClass = mainClass;

      {
        string[] parts = mainClass.shortCutDragModifiers.Split('+');
        foreach (string part in parts) {
          switch (part) {
            case "Ctrl": checkDragModCtrl.Checked = true; break;
            case "Alt": checkDragModAlt.Checked = true; break;
            case "Shift": checkDragModShift.Checked = true; break;
          }
        }
      }

      mainClass.PopulateKeysCombobox(comboDragKeys);

      comboDragKeys.SelectedItem = mainClass.shortCutDragKey;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      {
        string shortcutModifiers = "";
        if (checkDragModCtrl.Checked) shortcutModifiers += "+Ctrl";
        if (checkDragModAlt.Checked) shortcutModifiers += "+Alt";
        if (checkDragModShift.Checked) shortcutModifiers += "+Shift";
        shortcutModifiers = shortcutModifiers.Trim('+');

        mainClass.settings.SetString("ShortcutDragModifiers", shortcutModifiers);
        mainClass.settings.SetString("ShortcutDragKey", (string)comboDragKeys.SelectedItem != "None" ? (string)comboDragKeys.SelectedItem : "");
      }

      mainClass.settings.Save();

      mainClass.LoadSettings();

      this.Close();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
