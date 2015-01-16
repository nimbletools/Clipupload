using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Clipupload
{
  public class AddonManager
  {
    public static string Path = "";
    public static NotifyIcon Tray = null;
    public static Settings Settings = null;

    public static List<Addon> Addons = new List<Addon>();

    public static void LoadAddons()
    {
      string[] dirs = Directory.GetDirectories(Path + "Addons");
      Addons.Clear();
      foreach (string dir in dirs) {
        Addon addon = new Addon(dir);
        if (!addon.Found) {
          continue;
        }
        if (!addon.Enabled && Settings != null) {
          addon.Enabled = Settings.GetString("EnabledAddons").Contains((Path != "" ? addon.Path.Replace(Path, "") : addon.Path) + ";");
        }
        addon.LoadAssembly();
        addon.ApplyObject.InternalInitialize(Tray);
        Addons.Add(addon);
      }
    }
  }
}
