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
using System.Threading;

namespace Facebook
{
  public class Facebook : Addon
  {
    public Settings settings;

    public string facebookAppID = "294548907266945";
    public string facebookName = "";
    public FacebookClient facebookClient;

    public string shortCutDragModifiers = "";
    public string shortCutDragKey = "";
    public string shortCutPasteModifiers = "";
    public string shortCutPasteKey = "";

    private Image bmpIcon;

    public Facebook()
    {
      this.bmpIcon = Image.FromFile(AddonPath + "/Icon.ico");
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Facebook",
        Author = "Angelo Geels",
        Icon = bmpIcon,
        URL = "https://facebook.com/",
        URL_Author = "http://angelog.nl/"
      };
    }

    public override void Initialize()
    {
      this.settings = new Settings(AddonPath + "/settings.txt");

      FacebookClient.DefaultVersion = "v2.1";
      this.facebookClient = new FacebookClient() {
        IsSecureConnection = true
      };

      LoadSettings();

      if (this.facebookClient.AccessToken != "") {
        // don't block the main thread by loading the user's facebook name in a new thread
        new Thread(new ThreadStart(delegate
        {
          try {
            this.facebookName = (this.facebookClient.Get("/me") as dynamic).name;
          } catch {
            // If it crashes here, then the user probably doesn't have a working internet connection.
            // Not sure what else to do here besides turning off the addon itself...
            this.facebookClient.AccessToken = "";
          }
        })).Start();
      }
    }

    public override void Uninitialize()
    {
    }

    public void LoadSettings()
    {
      facebookClient.AccessToken = settings.GetString("AccessToken");

      shortCutDragModifiers = settings.GetString("ShortcutDragModifiers");
      shortCutDragKey = settings.GetString("ShortcutDragKey");
      shortCutPasteModifiers = settings.GetString("ShortcutPasteModifiers");
      shortCutPasteKey = settings.GetString("ShortcutPasteKey");
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        Visible = this.facebookClient.AccessToken != "",
        IsDragItem = true,
        Text = "Facebook",
        Image = this.bmpIcon,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
        ShortcutModifiers = this.shortCutDragModifiers,
        ShortcutKey = this.shortCutDragKey
      });

      ret.Add(new MenuEntry() {
        Visible = this.facebookClient.AccessToken != "" && (ClipboardContainsImage || ClipboardContainsFileList),
        Text = "Facebook",
        Image = this.bmpIcon,
        Action = new Action(Upload),
        ShortcutModifiers = this.shortCutPasteModifiers,
        ShortcutKey = this.shortCutPasteKey
      });

      ret.Add(new MenuEntry() {
        Visible = this.facebookClient.AccessToken == "",
        ShowShortInfo = false,
        Text = "Authenticate Facebook",
        Image = this.bmpIcon,
        Action = new Action(Settings)
      });

      return ret.ToArray();
    }

    public override void Settings()
    {
      new FormSettings(this).ShowDialog();
    }

    public override ShellInfo[] ShowShell(string[] files)
    {
      List<ShellInfo> ret = new List<ShellInfo>();

      bool allImages = true;

      foreach (string file in files) {
        string strExt = Path.GetExtension(file);
        if (strExt != ".png" && strExt != ".jpg" && strExt != ".gif" && strExt != ".jpeg") {
          allImages = false;
          break;
        }
      }

      if (allImages) {
        ret.Add(new ShellInfo() {
          Text = GetShellText(files, "Facebook"),
          Image = this.bmpIcon,
          Identifier = "up"
        });
      }

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

    public void DragCallback(DragCallback callback)
    {
      switch (callback.Type) {
        case DragCallbackType.Image:
          UploadImage(callback.Image);
          break;
      }
    }

    public void UploadImage(Image img)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      MemoryStream ms = new MemoryStream();

      img.Save(ms, ImageFormat.Png);
      string strImageInfo = img.Width + " x " + img.Height;
      img.Dispose();

      string url = "";
      string failReason = "";
      try {
        url = this.UploadToFacebook(ms);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (url != "")
        this.Backup(ms.GetBuffer(), url.Split('/', '\\').Last() + ".png");
      else
        this.Backup(ms.GetBuffer(), this.RandomFilename(5) + ".png");

      if (url != "CANCELED") {
        if (url != "") {
          Uploaded("Image", url, strImageInfo);
        } else {
          Failed(failReason);
        }
      }

      Tray.Icon = defIcon;
    }

    public void UploadFiles(StringCollection files)
    {
      if (files.Count == 0)
        return;

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      string failReason = "";
      List<UploadedFile> uploadedFiles = new List<UploadedFile>();

      bool canceled = false;

      try {
        foreach (string file in files) {
          if (!(file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".gif")))
            continue;

          MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));

          string url = this.UploadToFacebook(ms);
          if (url == "CANCELED") {
            canceled = true;
            break;
          }

          if (url != "") {
            uploadedFiles.Add(new UploadedFile() {
              URL = url,
              Info = (new FileInfo(file).Length / 1000) + " kB"
            });
          }
        }
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (!canceled) {
        if (uploadedFiles.Count > 0) {
          UploadedFiles(uploadedFiles);
        } else {
          Failed(failReason);
        }
      }

      Tray.Icon = defIcon;
    }

    public string UploadToFacebook(MemoryStream ms)
    {
      // TODO: Make progressbar functional for C# Facebook SDK.
      //       Limitation in current C# Facebook SDK makes us unable to track progress of photo uploads.
      this.ProgressBar.Start("Facebook", ms.Length);

      string url = "";

      ms.Seek(0, SeekOrigin.Begin);
      long iLength = ms.Length;

      FacebookMediaStream img = new FacebookMediaStream();
      img.ContentType = "image/png";
      img.FileName = this.RandomFilename(5) + ".png";
      img.SetValue(ms);

      Dictionary<string, object> photoParams = new Dictionary<string, object>();
      // TODO: Some way to attach a message to an image
      //photoParams["message"] = message;
      photoParams["image"] = img;
      dynamic ret = this.facebookClient.Post("/me/photos", photoParams);

      string photoID = ret.id;
      url = "https://www.facebook.com/photo.php?fbid=" + photoID;

      this.ProgressBar.Done();

      return url;
    }

    public void Upload()
    {
      if (Clipboard.ContainsImage())
        UploadImage(Clipboard.GetImage());
      else if (Clipboard.ContainsFileDropList())
        UploadFiles(Clipboard.GetFileDropList());
    }
  }
}
