using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Clipupload
{
  // this class is here to fix a problem with Chrome and IE which marks every downloaded file with a
  // zone identifier, causing the program to fail loading the dll's dynamically causing the entire
  // program to crash
  public class FileUnblocker
  {
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool DeleteFile(string name);

    public static bool Unblock(string fileName)
    {
      return DeleteFile(fileName + ":Zone.Identifier");
    }
  }

  public class Addon
  {
    public Settings Stats;
    public string Path;
    public string PathEntry;
    public string FullTypeName;
    public bool Found = false;
    public bool Enabled = false;

    public Assembly Assembly;
    public Type Type;
    public AddonHelper.Addon ApplyObject;
    public AddonHelper.AddonInfo Info;

    public ListViewItem ListItem;

    public Addon(string strDir)
    {
      // find the right DLL file with the right info to load the assembly (previously declared in info.txt but now automated in order to deprecate info.txt)
      string[] files = Directory.GetFiles(strDir, "*.dll");

      // unblock all dll files, this is potentially required to load the assemblies in the first place
      foreach (string strFile in files) {
        FileUnblocker.Unblock(strFile);
      }

      // check every file for the presence of AddonHelper
      foreach (string strFile in files) {
        Assembly assembly = Assembly.LoadFrom(strFile);
        Type[] types = assembly.GetTypes();
        foreach (Type type in types) {
          if (type.BaseType == null) {
            continue;
          }
          if (type.BaseType.FullName == "AddonHelper.Addon") {
            // found it!
            Found = true;
            FullTypeName = type.FullName;
            PathEntry = strFile;
            break;
          }
        }
      }

      // if there's still an info.txt present
      if (File.Exists(strDir + "\\info.txt")) {
        // remember if it was enabled before
        Enabled = new Settings(strDir + "\\info.txt").GetBool("Enabled");

        // delete the old deprecated files
        File.Delete(strDir + "\\info.txt");
        if (File.Exists(strDir + "\\info.txt.clean")) {
          File.Delete(strDir + "\\info.txt.clean");
        }
      }

      // remember some fun statistics
      Stats = new Settings(strDir + "/stats.txt");
      if (!Stats.Contains("UploadCount")) {
        Stats.SetInt("UploadCount", 0);
        Stats.Save();
      }

      this.Path = strDir;
    }

    public void LoadAssembly()
    {
      if (File.Exists(PathEntry)) {
        try {
          Assembly = Assembly.LoadFrom(PathEntry);
          Type = Assembly.GetType(FullTypeName);
          ConstructorInfo constructor = this.Type.GetConstructor(Type.EmptyTypes);
          ApplyObject = (AddonHelper.Addon)constructor.Invoke(new object[] { });
          Info = ApplyObject.GetInfo();
        } catch (Exception ex) {
          string strExceptionMessage = (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
          MessageBox.Show("Clipupload addon loading exception caught: \"" + strExceptionMessage + "\"", "Clipupload", MessageBoxButtons.OK, MessageBoxIcon.Error);
          Program.Debug("Addon loading exception caught: \"" + strExceptionMessage + "\"\n\n" + ex.StackTrace);
        }
      } else {
        // this shouldn't happen
        MessageBox.Show("Assembly file \"" + PathEntry + "\" not found!");
      }
    }
  }
}
