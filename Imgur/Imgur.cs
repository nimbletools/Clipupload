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
using System.Diagnostics;
using MrAG.OAuth;

namespace Imgur
{
  public class Imgur : Addon
  {
    public Settings settings;

    public string imgurClientID = "1afa43da0ac83a9";
    public string imgurClientSecret = "990e744d81e134250a860b5b28354f99fa28799a";

    public bool authenticated;
    public bool isPro;
    public string username;

    public string imageFormat = "PNG";

    public string shortCutDragModifiers = "";
    public string shortCutDragKey = "";
    public string shortCutAnimModifiers = "";
    public string shortCutAnimKey = "";
    public string shortCutPasteModifiers = "";
    public string shortCutPasteKey = "";

    public bool jpegCompression = false;
    public int jpegCompressionFilesize = 1000;
    public int jpegCompressionRate = 75;

    private Bitmap bmpIcon;

    public OAuth2 oauth;

    public Imgur()
    {
      this.bmpIcon = new Icon(AddonPath + "/Icon.ico").ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Imgur",
        Author = "Angelo Geels",
        Icon = bmpIcon,
        URL = "https://imgur.com/",
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

    public void MigrateSettings()
    {
      if (settings.Contains("ShortcutModifiers")) {
        // Migrate old 3.00 config file
        settings.SetString("ShortcutDragModifiers", settings.GetString("ShortcutModifiers"));
        settings.SetString("ShortcutDragKey", settings.GetString("ShortcutKey"));
        settings.SetString("ShortcutPasteModifiers", "");
        settings.SetString("ShortcutPasteKey", "");

        settings.Delete("ShortcutModifiers");
        settings.Delete("ShortcutKey");

        settings.Save();
      }

      if (!settings.Contains("AccessToken")) {
        // Migrate old 3.10 config file
        settings.SetString("Username", "");
        settings.SetBool("IsPro", false);
        settings.SetString("AccessToken", "");
        settings.SetString("AccessTokenSecret", "");

        settings.SetBool("JpegCompression", false);
        settings.SetInt("JpegCompressionFilesize", 1000);
        settings.SetInt("JpegCompressionRate", 75);

        settings.Save();
      }

      if (!settings.Contains("RefreshNeeded")) {
        // Migrate old 3.12 config file
        // this migration will delete access token as well, because it's invalidated anyhow since we're now using OAuth2
        settings.SetString("AccessToken", "");
        settings.SetLong("RefreshNeeded", 0);
        settings.SetString("RefreshToken", "");

        settings.Delete("AccessTokenSecret");

        settings.Save();
      }
    }

    public void LoadSettings()
    {
      MigrateSettings();

      imageFormat = settings.GetString("Format");

      shortCutDragModifiers = settings.GetString("ShortcutDragModifiers");
      shortCutDragKey = settings.GetString("ShortcutDragKey");
      shortCutAnimModifiers = settings.GetString("ShortcutAnimModifiers");
      shortCutAnimKey = settings.GetString("ShortcutAnimKey");
      shortCutPasteModifiers = settings.GetString("ShortcutPasteModifiers");
      shortCutPasteKey = settings.GetString("ShortcutPasteKey");

      jpegCompression = settings.GetBool("JpegCompression");
      jpegCompressionFilesize = settings.GetInt("JpegCompressionFilesize");
      jpegCompressionRate = settings.GetInt("JpegCompressionRate");

      this.oauth = new OAuth2("https://api.imgur.com/oauth2/", this.imgurClientID, this.imgurClientSecret);
      this.oauth.Proxy = GetProxy();
      this.oauth.ServiceIcon = new Icon(AddonPath + "/Icon.ico");
      this.oauth.ServiceName = "Imgur";

      this.username = settings.GetString("Username");
      this.isPro = settings.GetBool("IsPro");
      this.oauth.AccessToken = settings.GetString("AccessToken");
      this.oauth.RefreshNeeded = new DateTime(settings.GetLong("RefreshNeeded"));
      this.oauth.RefreshToken = settings.GetString("RefreshToken");
      authenticated = username != "" && this.oauth.AccessToken != "";
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        IsDragItem = true,
        Text = "Imgur",
        Image = this.bmpIcon,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
        ShortcutModifiers = this.shortCutDragModifiers,
        ShortcutKey = this.shortCutDragKey
      });

      ret.Add(new MenuEntry() {
        IsAnimateItem = true,
        Text = "Imgur",
        Image = this.bmpIcon,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback), true); }),
        ShortcutModifiers = this.shortCutAnimModifiers,
        ShortcutKey = this.shortCutAnimKey
      });

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsImage || ClipboardContainsFileList,
        Text = "Imgur",
        Image = this.bmpIcon,
        Action = new Action(Upload),
        ShortcutModifiers = this.shortCutPasteModifiers,
        ShortcutKey = this.shortCutPasteKey
      });

      if (Android.AllOK()) {
        AndroidDevice[] devices = Android.ListDevices();
        foreach (AndroidDevice device in devices) {
          string strDeviceSerial = device.SerialNumber;
          ret.Add(new MenuEntry() {
            IsAndroid = true,
            ShowShortInfo = false,
            Text = device.Model,
            SubEntries = new List<MenuEntry>(new MenuEntry[] {
                new MenuEntry() {
                  IsAndroidScreenshotItem = true,
                  Text = "Screenshot",
                  Image = this.bmpIcon,
                  Action = new Action(delegate { AndroidScreenshot(strDeviceSerial, false); }),
                  ActionSecondary = new Action(delegate { AndroidScreenshot(strDeviceSerial, true); })
                }
              })
          });
        }
      }

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
          Text = GetShellText(files, "Imgur"),
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

        case DragCallbackType.Animation:
          UploadAnimation(callback.Animation);
          break;
      }
    }

    public void UploadImage(Image img)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      MemoryStream ms = new MemoryStream();

      ImageFormat format = ImageFormat.Png;
      string formatStr = imageFormat.ToLower();

      switch (formatStr) {
        case "png": format = ImageFormat.Png; break;
        case "jpg": format = ImageFormat.Jpeg; break;
        case "gif": format = ImageFormat.Gif; break;
      }

      img = this.ImagePipeline(img);

      img.Save(ms, format);

      if (jpegCompression && Control.ModifierKeys != Keys.Shift) {
        if (ms.Length / 1000 > jpegCompressionFilesize) {
          ms.Dispose();
          ms = new MemoryStream();

          // Set up the encoder, codec and params
          System.Drawing.Imaging.Encoder jpegEncoder = System.Drawing.Imaging.Encoder.Compression;
          ImageCodecInfo jpegCodec = this.GetEncoder(ImageFormat.Jpeg);
          EncoderParameters jpegParams = new EncoderParameters();
          jpegParams.Param[0] = new EncoderParameter(jpegEncoder, jpegCompressionRate);

          // Now save it with the new encoder
          img.Save(ms, jpegCodec, jpegParams);

          // And make sure the filename gets set correctly (for the Imgur addon this is only for the backups)
          formatStr = "jpg";
        }
      }

      string failReason = "";
      string url = "";
      try {
        url = this.UploadToImgur(ms, img.Width, img.Height);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (url != "CANCELED" && url != "")
        this.Backup(ms.GetBuffer(), url.Split('/', '\\').Last());
      else
        this.Backup(ms.GetBuffer(), this.RandomFilename(5) + "." + formatStr);

      if (url != "CANCELED") {
        if (url != "") {
          Uploaded("Image", url, img.Width + " x " + img.Height);
        } else {
          Failed(failReason);
        }
      }

      img.Dispose();

      Tray.Icon = defIcon;
    }

    public void UploadAnimation(MemoryStream ms)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      string failReason = "";
      string url = "";
      try {
        url = this.UploadToImgur(ms, -1, -1);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (url != "CANCELED" && url != "")
        this.Backup(ms.GetBuffer(), url.Split('/', '\\').Last());
      else
        this.Backup(ms.GetBuffer(), this.RandomFilename(5) + ".gif");

      if (url != "CANCELED") {
        if (url != "") {
          Uploaded("Animation", url, (ms.Length / 1000) + " kB");
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

          Image img = Image.FromFile(file);
          MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));

          string url = this.UploadToImgur(ms, img.Width, img.Height);
          img.Dispose();

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

    public void AndroidScreenshot(string strDeviceSerial, bool askCustomFilename)
    {
      UploadImage(Image.FromFile(Android.PullScreenshot(strDeviceSerial)));
    }

    public string UploadToImgur(MemoryStream ms, int width, int height)
    {
      byte[] writeData = Encoding.ASCII.GetBytes("image=" + LongDataEscape(Convert.ToBase64String(ms.ToArray())));

      this.ProgressBar.Start("Imgur", writeData.Length);

      HttpWebRequest req = null;
      string url = "";

      if (this.authenticated) {
        req = this.oauth.AuthenticatedWebRequest("https://api.imgur.com/3/image");
      } else {
        req = (HttpWebRequest)HttpWebRequest.Create("https://api.imgur.com/3/image");
        req.Headers[HttpRequestHeader.Authorization] = "Client-ID " + imgurClientID;
      }
      req.Proxy = this.GetProxy();
      req.Method = "POST";
      req.ContentType = "application/x-www-form-urlencoded";
      req.ContentLength = writeData.Length;
      req.UserAgent = "ClipUpload 3/clipupload.net";

      Stream stream = req.GetRequestStream();

      ms.Dispose();
      ms = new MemoryStream(writeData);
      int sr = 1024;
      for (int i = 0; i < ms.Length; i += 1024) {
        if (ms.Length - i < 1024)
          sr = (int)ms.Length - i;
        else
          sr = 1024;

        byte[] buffer = new byte[sr];
        ms.Seek((long)i, SeekOrigin.Begin);
        ms.Read(buffer, 0, sr);
        stream.Write(buffer, 0, sr);

        if (this.ProgressBar.Canceled) {
          req.Abort();
          ms.Dispose();

          req = null;
          ms = null;

          return "CANCELED";
        }
        this.ProgressBar.Set(i);
      }

      WebResponse response = req.GetResponse();
      StreamReader reader = new StreamReader(response.GetResponseStream());
      bool bSuccess = false;
      dynamic result = JSON.JsonDecode(reader.ReadToEnd(), ref bSuccess);

      if (bSuccess && result["success"]) {
        url = result["data"]["link"];
      }

      this.ProgressBar.Done();

      return url.Replace("http://", "https://");
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
