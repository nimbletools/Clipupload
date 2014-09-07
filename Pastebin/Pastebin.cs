using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using AddonHelper;

namespace Pastebin
{
  public class Pastebin : Addon
  {
    public Settings settings;

    public string PastebinAPIKey = "d88817b8669cb8b1e90c5a4e1ed4f64a";

    public bool UserLoggedIn = false;
    public string UserKey = "";
    public string UserName = "";
    public bool ShowPrivate;

    private Bitmap bmpIcon;

    public Pastebin()
    {
      this.bmpIcon = new Icon(AddonPath + "/Icon.ico").ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Pastebin",
        Author = "Angelo Geels",
        Icon = bmpIcon,
        URL = "https://pastebin.com/",
        URL_Author = "http://angelog.nl/"
      };
    }

    public override void Initialize()
    {
      this.settings = new Settings(AddonPath + "/settings.txt");

      LoadSettings();
    }

    public override void Uninitialize()
    {
    }

    public void LoadSettings()
    {
      UserKey = settings.GetString("UserKey");
      UserName = settings.GetString("UserName");
      ShowPrivate = settings.GetBool("ShowPrivate");
      UserLoggedIn = UserKey.Length == 32;
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsText || ClipboardContainsFileList,
        Text = "Pastebin",
        Image = this.bmpIcon
      }.AddSubEntry(new MenuEntry() {
        ShowShortInfo = false,
        Text = "Public",
        Action = new Action(Upload)
      }).AddSubEntry(new MenuEntry() {
        ShowShortInfo = false,
        Text = "Private",
        Action = new Action(UploadPrivate)
      }));

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsText || ClipboardContainsFileList,
        Text = "Pastebin Raw",
        Image = this.bmpIcon
      }.AddSubEntry(new MenuEntry() {
        ShowShortInfo = false,
        Text = "Public",
        Action = new Action(RawUpload),
      }).AddSubEntry(new MenuEntry() {
        ShowShortInfo = false,
        Text = "Private",
        Action = new Action(RawUploadPrivate),
      }));

      return ret.ToArray();
    }

    public override void Settings()
    {
      new FormSettings(this).ShowDialog();
    }

    public override ShellInfo[] ShowShell(string[] files)
    {
      List<ShellInfo> ret = new List<ShellInfo>();

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "Pastebin"),
        Image = this.bmpIcon,
        Identifier = "up"
      });

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "private Pastebin"),
        Image = this.bmpIcon,
        Identifier = "up_private"
      });

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "raw Pastebin"),
        Image = this.bmpIcon,
        Identifier = "up_raw"
      });

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "private raw Pastebin"),
        Image = this.bmpIcon,
        Identifier = "up_raw_private"
      });

      return ret.ToArray();
    }

    public override void ShellCalled(string identifier, string[] files)
    {
      if (identifier.StartsWith("up")) {
        StringCollection arr = new StringCollection();
        arr.AddRange(files);

        if (identifier == "up") {
          UploadFiles(arr, false, false);
        } else if (identifier == "up_private") {
          UploadFiles(arr, false, true);
        } else if (identifier == "up_raw") {
          UploadFiles(arr, true, false);
        } else if (identifier == "up_raw_private") {
          UploadFiles(arr, true, true);
        }
      }
    }

    public void UploadText(string Content, bool Raw, bool Private)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon("Addons/Pastebin/Icon.ico");

      WebClient wc = new WebClient();
      wc.Proxy = this.GetProxy();
      wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

      string result = "Bad API request, no result";
      string failReason = "";
      try {
        string args = "";

        args += "api_dev_key=" + PastebinAPIKey;
        args += "&api_option=paste";
        if (UserLoggedIn)
          args += "&api_user_key=" + UserKey;
        args += "&api_paste_code=" + LongDataEscape(Content);
        args += "&api_paste_private=" + (Private ? "1" : "0");

        result = wc.UploadString("http://" + "pastebin.com/api/api_post.php", args);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      Tray.Icon = defIcon;

      if (!result.Contains("Bad API request, ")) {
        if (Raw) {
          Uploaded("Text", result.Replace("pastebin.com/", "pastebin.com/raw.php?i=").Replace("http://", "https://"), Content.Length + " characters");
        } else {
          Uploaded("Text", result.Replace("http://", "https://"), Content.Length + " characters");
        }
      } else {
        Failed(failReason);
      }
    }

    public void UploadFiles(StringCollection files, bool Raw, bool Private)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon("Addons/Pastebin/Icon.ico");

      WebClient wc = new WebClient();
      wc.Proxy = this.GetProxy();
      wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

      string failReason = "";
      List<UploadedFile> uploadedFiles = new List<UploadedFile>();

      try {
        foreach (string file in files) {
          string result = "Bad API request, no result";
          string args = "";

          args += "api_dev_key=" + PastebinAPIKey;
          args += "&api_option=paste";
          if (UserLoggedIn)
            args += "&api_user_key=" + UserKey;
          args += "&api_paste_code=" + LongDataEscape(File.ReadAllText(file));
          args += "&api_paste_private=" + (Private ? "1" : "0");

          result = wc.UploadString("http://" + "pastebin.com/api/api_post.php", args);
          if (Raw) {
            result = result.Replace("pastebin.com/", "pastebin.com/raw.php?i=");
          }

          uploadedFiles.Add(new UploadedFile() {
            URL = result,
            Info = (new FileInfo(file).Length / 1000) + " kB"
          });
        }
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (failReason == "") {
        UploadedFiles(uploadedFiles);
      } else {
        Failed(failReason);
      }

      Tray.Icon = defIcon;
    }

    private void Up(bool Raw, bool Private)
    {
      if (Clipboard.ContainsText()) {
        UploadText(Clipboard.GetText(), Raw, Private);
      } else if (Clipboard.ContainsFileDropList()) {
        StringCollection files = Clipboard.GetFileDropList();
        if (files.Count == 1) {
          UploadText(File.ReadAllText(files[0]), Raw, Private);
        } else {
          UploadFiles(Clipboard.GetFileDropList(), Raw, Private);
        }
      }
    }

    public void Upload()
    {
      Up(false, false);
    }

    public void RawUpload()
    {
      Up(true, false);
    }

    public void UploadPrivate()
    {
      Up(false, true);
    }

    public void RawUploadPrivate()
    {
      Up(true, true);
    }
  }
}
