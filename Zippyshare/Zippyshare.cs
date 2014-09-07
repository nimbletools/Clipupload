using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using System.IO;
using AddonHelper;

namespace Zippyshare
{
  public class Zippyshare : Addon
  {
    private ZippyshareSettings m_Settings = new ZippyshareSettings();
    private Bitmap m_BmpIcon;

    public Zippyshare()
    {
      this.m_BmpIcon = new Icon(AddonPath + "/Icon.ico").ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Zippyshare",
        Author = "Angelo Geels",
        Icon = m_BmpIcon,
        //URL = "http://clipupload.net/",
        URL_Author = "http://angelog.nl/"
      };
    }

    public override void Initialize()
    {
      DeserializeSettings(m_Settings, "settings.txt");
    }

    public override void Uninitialize()
    {

    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsText
          || ClipboardContainsImage
          || ClipboardContainsFileList,
        Text = "Zippyshare",
        Action = new Action(delegate { UploadFiles(Clipboard.GetFileDropList()); }),
        Image = this.m_BmpIcon
      });

      return ret.ToArray();
    }

    public override void Settings()
    {
      OpenQuickSettings(m_Settings, AddonPath + "/settings.txt", GetInfo().Name + " settings");
    }

    public override ShellInfo[] ShowShell(string[] files)
    {
      List<ShellInfo> ret = new List<ShellInfo>();

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "Zippyshare"),
        Image = m_BmpIcon,
        Identifier = "up"
      });

      return ret.ToArray();
    }

    public override void ShellCalled(string identifier, string[] files)
    {
      if (identifier == "up") {
        StringCollection arr = new StringCollection();
        arr.AddRange(files);
        UploadFiles(arr);
      }
    }

    void UploadFiles(StringCollection files)
    {
      List<UploadedFile> uploadedFiles = new List<UploadedFile>();

      foreach (string file in files) {
        byte[] buffer = File.ReadAllBytes(file);
        MemoryStream ms = new MemoryStream();
        ms.Write(buffer, 0, buffer.Length);
        ms.Seek(0, SeekOrigin.Begin);

        string url = "";
        string failReason = "";

        try {
          url = UploadToZippyshare(ms, Path.GetFileName(file));
        } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

        if (url != "") {
          uploadedFiles.Add(new UploadedFile() {
            URL = url,
            Info = (new FileInfo(file).Length / 1000) + " kB"
          });
        } else {
          Failed(failReason);
        }
      }

      UploadedFiles(uploadedFiles);
    }

    string UploadToZippyshare(MemoryStream ms, string filename)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      cszippy.SessionFetcher session = new cszippy.SessionFetcher();
      session.Fetch();

      cszippy.FileUploader uploader = new cszippy.FileUploader(session.GetServer(), session.GetUploadID(), filename, ms);
      uploader.Proxy = this.GetProxy();

      uploader.OnStart = (total) => {
        ProgressBar.Start(filename, total);
      };
      uploader.OnProgess = (sent, total) => {
        ProgressBar.Set(sent);
        return !ProgressBar.Canceled;
      };
      uploader.OnFinished = (success, error) => {
        Tray.Icon = defIcon;
        ProgressBar.Done();
      };

      return uploader.Upload();
    }
  }
}
