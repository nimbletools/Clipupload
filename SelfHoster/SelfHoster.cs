using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AddonHelper;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Specialized;

namespace SelfHoster
{
  public class SelfHoster : Addon
  {
    public Settings settings;

    public string host_interface = "0.0.0.0";
    public int host_port = 80;

    public string imageFormat = "PNG";
    public bool useMD5 = false;
    public bool shortMD5 = false;
    public int length = 8;

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
    private Bitmap bmpIcon16;

    public string web_path = "";
    public string web_http = "";
    public bool web_accesslog = false;
    public bool web_accessNotify = false;
    public bool web_accessDelete = false;

    public WebServer web_server = new WebServer();

    public SelfHoster()
    {
      Icon icon = new Icon(AddonPath + "/Icon.ico");
      this.bmpIcon = icon.ToBitmap();
      this.bmpIcon16 = new Icon(icon, new Size(16, 16)).ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Self Hoster",
        Author = "Angelo Geels",
        Icon = bmpIcon16,
        //URL = "http://clipupload.net/",
        URL_Author = "http://angelog.nl/"
      };
    }

    public override void Initialize()
    {
      this.settings = new Settings(AddonPath + "/settings.txt");

      web_path = AddonPath + "/Files";

      if (!Directory.Exists(web_path)) {
        Directory.CreateDirectory(web_path);
        using (StreamWriter writer = File.CreateText(web_path + "/index.html")) {
          writer.WriteLine(
@"<!DOCTYPE html>
<html>
  <head>
    <title>ClipUpload Self-Hoster Addon</title>
    <style>
      body {
        font-family: Sans-serif;
        font-size: 12px;
        background: #ddd;
        color: #222;
      }
      #container {
        width: 900px;
        margin: 0 auto;
      }
      h1 {
        text-align: center;
      }
    </style>
  </head>
  <body>
    <div id=""container"">
      <h1>ClipUpload Self-Hoster Addon</h1>
      <p>This is the <i>index.html</i> file located in <i>" + web_path + @"/</i>. All files uploaded with the <i>Self Hoster</i> addon will be placed in this folder. Feel free
         to modify this index page or even delete it if you wish.</p>
      <p><a href=""http://clipupload.net/"">ClipUpload</a> is a program to easily take screenshots and text snippets and instantly upload them to a file host of one's choice such
         as Imgur or Dropbox, as well as allowing for custom addon support for users who's favorite host isn't supported by default.</p>
    </div>
  </body>
</html>");
        }
      }

      LoadSettings();
    }

    public override void Uninitialize()
    {
      try {
        web_server.Stop();
      } catch { }
    }

    public void StartServer()
    {
      web_server.ws_strInterface = host_interface;
      web_server.ws_iPort = host_port;
      web_server.ws_strRootPath = web_path;
      web_server.ws_strAccessLog = web_accesslog ? AddonPath + "/Access.log" : "";
      web_server.ws_onAccess = new Action<string, string, string, bool>(OnAccess);
      web_server.Start();
    }

    public void LoadSettings()
    {
      host_interface = settings.GetString("HostInterface");
      host_port = settings.GetInt("HostPort");
      web_http = settings.GetString("HostHTTP");
      web_accesslog = settings.GetBool("HostAccessLog");
      web_accessNotify = settings.GetBool("HostNotify");
      web_accessDelete = settings.GetBool("HostDelete");

      imageFormat = settings.GetString("Format");
      useMD5 = settings.GetBool("UseMD5");
      shortMD5 = settings.GetBool("ShortMD5");
      length = settings.GetInt("Length");

      jpegCompression = settings.GetBool("JpegCompression");
      jpegCompressionFilesize = settings.GetInt("JpegCompressionFilesize");
      jpegCompressionRate = settings.GetInt("JpegCompressionRate");

      shortCutDragModifiers = settings.GetString("ShortcutDragModifiers");
      shortCutDragKey = settings.GetString("ShortcutDragKey");
      shortCutAnimModifiers = settings.GetString("ShortcutAnimModifiers");
      shortCutAnimKey = settings.GetString("ShortcutAnimKey");
      shortCutPasteModifiers = settings.GetString("ShortcutPasteModifiers");
      shortCutPasteKey = settings.GetString("ShortcutPasteKey");

      StartServer();
    }

    public void OnAccess(string strIP, string strURL, string strPath, bool bFileExists)
    {
      if (web_accessNotify && strURL != "/favicon.ico") {
        string strMessage = strIP + " - \"" + strURL + "\"";
        ToolTipIcon icon = ToolTipIcon.Info;
        if (!bFileExists) {
          strMessage += "\nThe file didn't exist!";
          icon = ToolTipIcon.Error;
        }
        Tray.ShowBalloonTip(2000, "Self Hoster Access", strMessage, icon);
      }

      if (web_accessDelete && bFileExists) {
        //TODO: Find out if this is safe :)
        // The webserver class makes sure that the path /should/ be safe, but I haven't done any pentesting on it yet..
        try {
          File.Delete(strPath);
        } catch { }
      }
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      ret.Add(new MenuEntry() {
        IsDragItem = true,
        Text = "Self Hoster",
        Image = this.bmpIcon16,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
        ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary)); }),
        ShortcutKey = this.shortCutDragKey
      });

      ret.Add(new MenuEntry() {
        IsAnimateItem = true,
        Text = "Self Hoster",
        Image = this.bmpIcon16,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback), true); }),
        ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary), true); }),
        ShortcutModifiers = this.shortCutAnimModifiers,
        ShortcutKey = this.shortCutAnimKey
      });

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsImage || ClipboardContainsFileList || ClipboardContainsText,
        Text = "Self Hoster",
        Image = this.bmpIcon16,
        Action = new Action(delegate { Upload(false); }),
        ActionSecondary = new Action(delegate { Upload(true); }),
        ShortcutModifiers = this.shortCutPasteModifiers,
        ShortcutKey = this.shortCutPasteKey
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

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "Self Hoster"),
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
        UploadFiles(arr, false);
      }
    }

    public void DragCallback(DragCallback callback)
    {
      switch (callback.Type) {
        case DragCallbackType.Image:
          UploadImage(callback.Image, callback.CustomFilename);
          break;

        case DragCallbackType.Animation:
          UploadAnimation(callback.Animation, callback.CustomFilename);
          break;
      }
    }

    public void DragCallbackSecondary(DragCallback callback)
    {
      callback.CustomFilename = true;
      DragCallback(callback);
    }

    CustomFilenameInfo OpenCFI(string filename)
    {
      return OpenCustomFilenameDialog("Enter a custom filename:", true, "Overwrite file if it exists", filename, false);
    }

    public void UploadImage(Image img, bool askCustomFilename)
    {
      ImageFormat format = ImageFormat.Png;
      string formatStr = imageFormat.ToLower();

      switch (imageFormat.ToLower()) {
        case "png": format = ImageFormat.Png; break;
        case "jpg": format = ImageFormat.Jpeg; break;
        case "gif": format = ImageFormat.Gif; break;
      }

      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length")).ToLower();
      if (this.useMD5) {
        filename = MD5(filename + rnd.Next(1000, 9999).ToString());

        if (this.shortMD5)
          filename = filename.Substring(0, this.length);
      }

      bool jpegCompressed = false;
      if (jpegCompression && Control.ModifierKeys != Keys.Shift) {
        MemoryStream ms = new MemoryStream();
        img.Save(ms, format);

        if (ms.Length / 1000 > jpegCompressionFilesize) {
          ms.Dispose();

          // Set up the encoder, codec and params
          System.Drawing.Imaging.Encoder jpegEncoder = System.Drawing.Imaging.Encoder.Compression;
          ImageCodecInfo jpegCodec = this.GetEncoder(ImageFormat.Jpeg);
          EncoderParameters jpegParams = new EncoderParameters();
          jpegParams.Param[0] = new EncoderParameter(jpegEncoder, jpegCompressionRate);

          // Now save it with the new encoder
          filename += ".jpg";

          // custom filename for jpg... ugly to put this twice but it works.
          if (askCustomFilename) {
            CustomFilenameInfo cfi = OpenCFI(filename);
            if (cfi == null) {
              return;
            }
            filename = cfi.UserInput;
            overwrite = cfi.Checkbox;
            askCustomFilename = false;
          }

          if (overwrite && File.Exists(web_path + "/" + filename)) {
            File.Delete(web_path + "/" + filename);
          }
          img.Save(web_path + "/" + filename, jpegCodec, jpegParams);
          jpegCompressed = true; // UGLYYYYYYYYYYYYY
        } else {
          filename += "." + formatStr;
        }

        ms.Dispose();
      } else {
        filename += "." + formatStr;
      }

      if (askCustomFilename) {
        CustomFilenameInfo cfi = OpenCFI(filename);
        if (cfi == null) {
          return;
        }
        filename = cfi.UserInput;
        overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      try {
        img = this.ImagePipeline(img);

        this.Backup(web_path + "/" + filename);
        if (!jpegCompressed) { // FUKCING UGLYYYYYY FUCKKK
          if (overwrite && File.Exists(web_path + "/" + filename)) {
            File.Delete(web_path + "/" + filename);
          }
          img.Save(web_path + "/" + filename, format);
        }

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Image", web_http + filename, img.Width + " x " + img.Height);
      } else {
        Failed(failReason);
      }

      img.Dispose();

      Tray.Icon = defIcon;
    }

    public void UploadAnimation(MemoryStream ms, bool askCustomFilename)
    {
      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length")).ToLower();
      if (this.useMD5) {
        filename = MD5(filename + rnd.Next(1000, 9999).ToString());

        if (this.shortMD5)
          filename = filename.Substring(0, this.length);
      }
      filename += ".gif";

      if (askCustomFilename) {
        CustomFilenameInfo cfi = OpenCFI(filename);
        if (cfi == null) {
          return;
        }
        filename = cfi.UserInput;
        overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      try {
        if (overwrite && File.Exists(web_path + "/" + filename)) {
          File.Delete(web_path + "/" + filename);
        }

        FileStream fs = File.OpenWrite(web_path + "/" + filename);
        fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
        fs.Close();
        fs.Dispose();

        this.Backup(web_path + "/" + filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Animation", web_http + filename, (ms.Length / 1000) + " kB");
      } else {
        Failed(failReason);
      }

      Tray.Icon = defIcon;
    }

    public void UploadText(string Text, bool askCustomFilename)
    {
      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length")).ToLower();
      if (this.useMD5) {
        filename = MD5(filename + rnd.Next(1000, 9999).ToString());

        if (this.shortMD5)
          filename = filename.Substring(0, this.length);
      }
      filename += ".txt";

      if (askCustomFilename) {
        CustomFilenameInfo cfi = OpenCFI(filename);
        if (cfi == null) {
          return;
        }
        filename = cfi.UserInput;
        overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      try {
        if (overwrite && File.Exists(web_path + "/" + filename)) {
          File.Delete(web_path + "/" + filename);
        }

        File.WriteAllText(web_path + "/" + filename, Text);

        this.Backup(web_path + "/" + filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Text", web_http + filename, Text.Length + " characters");
      } else {
        Failed(failReason);
      }

      Tray.Icon = defIcon;
    }

    public void UploadFiles(StringCollection files, bool askCustomFilename)
    {
      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";
      List<UploadedFile> uploadedFiles = new List<UploadedFile>();

      try {
        foreach (string file in files) {
          string filename = file.Split('/', '\\').Last();
          bool overwrite = false;

          if (askCustomFilename) {
            CustomFilenameInfo cfi = OpenCFI(filename);
            if (cfi == null) {
              continue;
            }
            filename = cfi.UserInput;
            overwrite = cfi.Checkbox;
          }

          if (overwrite && File.Exists(web_path + "/" + filename)) {
            File.Delete(web_path + "/" + filename);
          }

          File.Copy(file, web_path + "/" + filename);

          uploadedFiles.Add(new UploadedFile() {
            URL = web_http + Uri.EscapeDataString(filename),
            Info = (new FileInfo(file).Length / 1000) + " kB"
          });
        }

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        UploadedFiles(uploadedFiles);
      } else {
        Failed(failReason);
      }

      Tray.Icon = defIcon;
    }

    public void Upload(bool askCustomFilename)
    {
      if (Clipboard.ContainsImage())
        UploadImage(Clipboard.GetImage(), askCustomFilename);
      else if (Clipboard.ContainsText())
        UploadText(Clipboard.GetText(), askCustomFilename);
      else if (Clipboard.ContainsFileDropList()) {
        StringCollection files = Clipboard.GetFileDropList();
        if (files.Count == 1 && (files[0].EndsWith(".png") || files[0].EndsWith(".jpg") || files[0].EndsWith(".gif")))
          UploadImage(Image.FromFile(files[0]), askCustomFilename);
        else
          UploadFiles(files, askCustomFilename && files.Count == 1);
      }
    }
  }
}
