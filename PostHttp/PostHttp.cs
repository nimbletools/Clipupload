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

namespace PostHttp
{
  public class PostHttp : Addon
  {
    public Settings settings;

    public string imageFormat = "PNG";
    public string endpointURL = "";
    public string endpointName = "";
    public string endpointAuthorization = "";

    public bool jpegCompression = false;
    public int jpegCompressionFilesize = 1000;
    public int jpegCompressionRate = 75;

    public string shortCutDragModifiers = "";
    public string shortCutDragKey = "";
    public string shortCutAnimModifiers = "";
    public string shortCutAnimKey = "";
    public string shortCutPasteModifiers = "";
    public string shortCutPasteKey = "";

    private Bitmap bmpIcon;
    private Bitmap bmpIcon16;

    public PostHttp()
    {
      this.settings = new Settings(AddonPath + "/settings.txt");
      Icon icon = new Icon(AddonPath + "/Icon.ico");
      this.bmpIcon = icon.ToBitmap();
      this.bmpIcon16 = new Icon(icon, new Size(16, 16)).ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "PostHttp",
        Author = "Angelo Geels",
        Icon = bmpIcon16,
        URL = settings.GetString("EndpointURLInfo"),
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

      if (!settings.Contains("JpegCompression")) {
        // Migrate old 3.10 config file
        settings.SetBool("JpegCompression", false);
        settings.SetInt("JpegCompressionFilesize", 1000);
        settings.SetInt("JpegCompressionRate", 75);

        settings.SetString("EndpointAuthorization", "");

        settings.Save();
      }
    }

    public void LoadSettings()
    {
      MigrateSettings();

      imageFormat = settings.GetString("Format");
      endpointURL = settings.GetString("EndpointURL");
      endpointName = settings.GetString("EndpointName");
      endpointAuthorization = settings.GetString("EndpointAuthorization");

      jpegCompression = settings.GetBool("JpegCompression");
      jpegCompressionFilesize = settings.GetInt("JpegCompressionFilesize");
      jpegCompressionRate = settings.GetInt("JpegCompressionRate");

      shortCutDragModifiers = settings.GetString("ShortcutDragModifiers");
      shortCutDragKey = settings.GetString("ShortcutDragKey");
      shortCutAnimModifiers = settings.GetString("ShortcutAnimModifiers");
      shortCutAnimKey = settings.GetString("ShortcutAnimKey");
      shortCutPasteModifiers = settings.GetString("ShortcutPasteModifiers");
      shortCutPasteKey = settings.GetString("ShortcutPasteKey");
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      if (endpointName != "" && endpointURL != "") {
        ret.Add(new MenuEntry() {
          IsDragItem = true,
          Text = this.endpointName,
          Image = this.bmpIcon16,
          Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
          ShortcutModifiers = this.shortCutDragModifiers,
          ShortcutKey = this.shortCutDragKey
        });

        ret.Add(new MenuEntry() {
          IsAnimateItem = true,
          Text = this.endpointName,
          Image = this.bmpIcon16,
          Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback), true); }),
          ShortcutModifiers = this.shortCutAnimModifiers,
          ShortcutKey = this.shortCutAnimKey
        });

        ret.Add(new MenuEntry() {
          Visible = ClipboardContainsImage || ClipboardContainsFileList || ClipboardContainsText,
          Text = this.endpointName,
          Image = this.bmpIcon16,
          Action = new Action(Upload),
          ShortcutModifiers = this.shortCutPasteModifiers,
          ShortcutKey = this.shortCutPasteKey
        });
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

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, settings.GetString("EndpointName")),
        Image = this.bmpIcon16,
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

      switch (imageFormat.ToLower()) {
        case "png": format = ImageFormat.Png; break;
        case "jpg": format = ImageFormat.Jpeg; break;
        case "gif": format = ImageFormat.Gif; break;
      }

      img = this.ImagePipeline(img);

      img.Save(ms, format);

      if (jpegCompression) {
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

          // And make sure the filename gets set correctly
          formatStr = "jpg";
        }
      }

      string failReason = "";
      string result = "";
      try {
        result = UploadToEndPoint(ms, this.endpointName);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result != "CANCELED") {
        string filename = "";

        if (result.StartsWith("http")) {
          Uploaded("Image", result, img.Width + " x " + img.Height);
          filename = result.Split('/', '\\').Last();
        } else {
          Failed(failReason);
        }

        if (filename != "")
          this.Backup(ms.GetBuffer(), filename);
        else
          this.Backup(ms.GetBuffer(), this.RandomFilename(5) + "." + formatStr);
      }

      img.Dispose();

      Tray.Icon = defIcon;
    }

    public void UploadAnimation(MemoryStream ms)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      string failReason = "";
      string result = "";
      try {
        result = UploadToEndPoint(ms, this.endpointName);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result != "CANCELED") {
        string filename = "";

        if (result.StartsWith("http")) {
          Uploaded("Animation", result, (ms.Length / 1000) + " kB");
          filename = result.Split('/', '\\').Last();
        } else {
          Failed(failReason);
        }

        if (filename != "")
          this.Backup(ms.GetBuffer(), filename);
        else
          this.Backup(ms.GetBuffer(), this.RandomFilename(5) + ".gif");
      }

      Tray.Icon = defIcon;
    }

    public void UploadText(string Text)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      string failReason = "";
      string result = "";
      try {
        result = UploadToEndPoint(new MemoryStream(Encoding.ASCII.GetBytes(Text)), this.endpointName);
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result != "CANCELED") {
        string filename = "";

        if (result.StartsWith("http")) {
          Uploaded("Text", result, Text.Length + " characters");
          filename = result.Split('/', '\\').Last();
        } else {
          Failed(failReason);
        }

        if (filename != "")
          this.Backup(Encoding.ASCII.GetBytes(Text), filename);
        else
          this.Backup(Encoding.ASCII.GetBytes(Text), this.RandomFilename(5) + "." + imageFormat.ToLower());
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
          MemoryStream ms = new MemoryStream(File.ReadAllBytes(file));

          string result = UploadToEndPoint(ms, file);
          if (result == "CANCELED") {
            canceled = true;
            break;
          }

          if (result.StartsWith("http")) {
            uploadedFiles.Add(new UploadedFile() {
              URL = result,
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

    public string UploadToEndPoint(MemoryStream ms, string filename)
    {
      string Filename = Uri.EscapeDataString(filename.Split('/', '\\').Last());
      byte[] writeData = Encoding.ASCII.GetBytes((filename == this.endpointName ? "" : "filename=" + Filename) +
                                                 (this.endpointAuthorization != "" ? "&auth=" + LongDataEscape(this.base64Encode(this.endpointAuthorization)) : "") +
                                                 "&file=" + LongDataEscape(Convert.ToBase64String(ms.ToArray())));

      this.ProgressBar.Start(filename, writeData.Length);

      string result = "";

      HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(this.endpointURL);
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
      result = reader.ReadToEnd();

      this.ProgressBar.Done();

      return result;
    }

    public void Upload()
    {
      if (Clipboard.ContainsImage())
        UploadImage(Clipboard.GetImage());
      else if (Clipboard.ContainsText())
        UploadText(Clipboard.GetText());
      else if (Clipboard.ContainsFileDropList())
        UploadFiles(Clipboard.GetFileDropList());
    }
  }
}
