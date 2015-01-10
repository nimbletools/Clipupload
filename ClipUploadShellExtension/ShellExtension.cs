using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpShell.SharpContextMenu;
using SharpShell.Attributes;
using ClipUpload4;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Diagnostics;

namespace ClipUploadShellExtension
{
  [ComVisible(true)]
  [Guid("740808E1-0C44-48D9-9A35-A262EAB77846")]
  [COMServerAssociation(AssociationType.Class, "*")]
  public class ClipUploadShellEx : SharpContextMenu
  {
    protected override bool CanShowMenu()
    {
      return true;
    }

    protected override ContextMenuStrip CreateMenu()
    {
      ContextMenuStrip ret = new ContextMenuStrip();

      string strPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(this.GetType()).Location) + "\\";

      AddonManager.Path = strPath;
      AddonManager.Settings = new Settings(strPath + "settings.txt");
      AddonManager.LoadAddons();

      ToolStripMenuItem tsiRoot = null;

      int ctAddons = 0;
      foreach (Addon addon in AddonManager.Addons) {
        if (!addon.Enabled || addon.ApplyObject == null) {
          continue;
        }

        AddonHelper.ShellInfo[] shellInfos = addon.ApplyObject.ShowShell(SelectedItemPaths.ToArray());
        Addon addonTemp = addon;

        int c = 0;
        foreach (AddonHelper.ShellInfo shellInfo in shellInfos) {
          if (!shellInfo.Visible) {
            continue;
          }

          if (tsiRoot == null) {
            tsiRoot = new ToolStripMenuItem("Upload with Clipupload", ClipUploadShellExtension.Properties.Resources.cu4icon_small);
            ret.Items.Add(tsiRoot);
          }

          if (++ctAddons > 1 && ++c == 1) {
            tsiRoot.DropDownItems.Add(new ToolStripSeparator());
          }

          string identifier = shellInfo.Identifier;

          ToolStripItem tsi = tsiRoot.DropDownItems.Add(shellInfo.Text);
          tsi.Image = shellInfo.Image;
          tsi.Click += (sender, args) => {
            try {
              TcpClient client = new TcpClient();
              client.Connect("127.0.0.1", 33404);
              using (StreamWriter writer = new StreamWriter(client.GetStream())) {
                writer.WriteLine(addonTemp.Path.Replace(strPath, ""));
                writer.WriteLine(identifier);
                foreach (string strFile in SelectedItemPaths) {
                  writer.WriteLine(strFile);
                }
                writer.WriteLine("END");
              }
            } catch (Exception ex) {
              string strError = "Failed to connect to Clipupload IPC: \"" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + "\"";
              LogError(strError);
              MessageBox.Show(strError, "Clipupload", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          };
        }
      }

      return ret;
    }
  }
}
