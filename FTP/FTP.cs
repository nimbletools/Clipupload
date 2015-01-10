using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Specialized;
using AddonHelper;

namespace FTP
{
  public class FTP : Addon
  {
    public Settings settings;

    public string ftpServer = "";
    public string ftpUsername = "";
    public string ftpPassword = "";
    public string ftpPath = "";
    public bool ftpPassive = false;
    public bool ftpBinary = true;
    public string ftpHttp = "";
    public string imageFormat = "PNG";
    public bool useMD5 = false;
    public bool shortMD5 = false;
    public int length = 8;

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

    public FTP()
    {
      Icon icon = new Icon(AddonPath + "/Icon.ico");
      this.bmpIcon = icon.ToBitmap();
      this.bmpIcon16 = new Icon(icon, new Size(16, 16)).ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "FTP",
        Author = "Angelo Geels",
        Icon = bmpIcon16,
        //URL = "http://clipupload.net/",
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
        settings.SetBool("Binary", true);

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

        settings.Save();
      }
    }

    public void LoadSettings()
    {
      MigrateSettings();

      ftpServer = settings.GetString("Server");
      ftpUsername = settings.GetString("Username");
      ftpPassword = base64Decode(settings.GetString("Password"));
      ftpPath = settings.GetString("Path");
      ftpPassive = settings.GetBool("Passive");
      ftpBinary = settings.GetBool("Binary");
      ftpHttp = settings.GetString("Http");

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
    }

    public override MenuEntry[] Menu()
    {
      List<MenuEntry> ret = new List<MenuEntry>();

      if (ftpServer != "" && ftpUsername != "" && ftpPath != "" && ftpHttp != "") {
        ret.Add(new MenuEntry() {
          IsDragItem = true,
          Text = "FTP",
          Image = this.bmpIcon16,
          Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
          ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary)); }),
          ShortcutModifiers = this.shortCutDragModifiers,
          ShortcutKey = this.shortCutDragKey
        });

        ret.Add(new MenuEntry() {
          IsAnimateItem = true,
          Text = "FTP",
          Image = this.bmpIcon16,
          Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback), true); }),
          ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary), true); }),
          ShortcutModifiers = this.shortCutAnimModifiers,
          ShortcutKey = this.shortCutAnimKey
        });

        ret.Add(new MenuEntry() {
          Visible = ClipboardContainsImage || ClipboardContainsText || ClipboardContainsFileList,
          Text = "FTP",
          Image = this.bmpIcon16,
          Action = new Action(delegate { Upload(false); }),
          ActionSecondary = new Action(delegate { Upload(true); }),
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
                  Image = this.bmpIcon16,
                  Action = new Action(delegate { AndroidScreenshot(strDeviceSerial, false); }),
                  ActionSecondary = new Action(delegate { AndroidScreenshot(strDeviceSerial, true); })
                },
                new MenuEntry() {
                  IsAndroidVideoItem = true,
                  Text = "Video",
                  Image = this.bmpIcon16,
                  Action = new Action(delegate { AndroidVideo(strDeviceSerial, false); }),
                  ActionSecondary = new Action(delegate { AndroidVideo(strDeviceSerial, true); })
                }
              })
            });
          }
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

      ret.Add(new ShellInfo() {
        Text = GetShellText(files, "FTP"),
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
      return OpenCustomFilenameDialog("Enter a custom filename:", false, "", filename, false);
    }

    public void UploadImage(Image img, bool askCustomFilename)
    {
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

          // And make sure the filename gets set correctly
          formatStr = "jpg";
        }
      }

      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length"));
      if (this.useMD5) {
        filename = MD5(filename + rnd.Next(1000, 9999).ToString());

        if (this.shortMD5)
          filename = filename.Substring(0, this.length);
      }
      filename += "." + formatStr;

      if (askCustomFilename) {
        CustomFilenameInfo cfi = OpenCFI(filename);
        if (cfi == null) {
          return;
        }
        filename = cfi.UserInput;
        //TODO: make overwrite work
        //overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      bool canceled = false;
      try {
        this.Backup(ms.GetBuffer(), filename);
        canceled = !UploadToFTP(ms, filename, overwrite);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (!canceled) {
        if (result) {
          Uploaded("Image", ftpHttp + filename, img.Width + " x " + img.Height);
        } else {
          Failed(failReason);
        }
      }

      img.Dispose();

      Tray.Icon = defIcon;
    }

    public void UploadAnimation(MemoryStream ms, bool askCustomFilename)
    {
      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length"));
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
        //TODO: make overwrite work
        //overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      bool canceled = false;
      try {
        if (!filename.EndsWith(".gif")) {
          filename += ".gif";
        }

        canceled = !UploadToFTP(ms, filename, overwrite);

        this.Backup(ms.GetBuffer(), filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (!canceled) {
        if (result) {
          Uploaded("Animation", ftpHttp + filename, (ms.Length / 1000) + " kB");
        } else {
          Failed(failReason);
        }
      }

      Tray.Icon = defIcon;
    }

    public void UploadText(string Text, bool askCustomFilename)
    {
      bool overwrite = false;
      string filename = this.RandomFilename(this.settings.GetInt("Length"));
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
        //TODO: make overwrite work
        //overwrite = cfi.Checkbox;
      }

      Icon defIcon = (Icon)Tray.Icon.Clone();
      Tray.Icon = new Icon(AddonPath + "/Icon.ico", new Size(16, 16));

      bool result = false;
      string failReason = "";

      byte[] textData = Encoding.UTF8.GetBytes(Text);

      bool canceled = false;
      try {
        canceled = !UploadToFTP(new MemoryStream(textData), filename, overwrite);

        this.Backup(textData, filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (!canceled) {
        if (result) {
          Uploaded("Text", ftpHttp + filename, Text.Length + " characters");
        } else {
          Failed(failReason);
        }
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

      bool canceled = false;

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

          canceled = !UploadToFTP(new MemoryStream(File.ReadAllBytes(file)), filename, overwrite);
          if (canceled)
            break;

          uploadedFiles.Add(new UploadedFile() {
            URL = ftpHttp + Uri.EscapeDataString(filename),
            Info = (new FileInfo(file).Length / 1000) + " kB"
          });
        }

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (!canceled) {
        if (result) {
          UploadedFiles(uploadedFiles);
        } else {
          Failed(failReason);
        }
      }

      Tray.Icon = defIcon;
    }

    public void AndroidScreenshot(string strDeviceSerial, bool askCustomFilename)
    {
      UploadImage(Image.FromFile(Android.PullScreenshot(strDeviceSerial)), askCustomFilename);
    }

    public void AndroidVideo(string strDeviceSerial, bool askCustomFilename)
    {
      FormAndroidRecord recorder = new FormAndroidRecord(strDeviceSerial);
      recorder.Callback = (string strFilename) => {
        StringCollection coll = new StringCollection();

        string filename = this.RandomFilename(this.settings.GetInt("Length"));
        if (this.useMD5) {
          filename = MD5(filename + rnd.Next(1000, 9999).ToString());

          if (this.shortMD5)
            filename = filename.Substring(0, this.length);
        }
        filename = Path.GetTempPath() + filename + ".mp4";
        File.Move(strFilename, filename);

        coll.Add(filename);
        UploadFiles(coll, askCustomFilename);
      };
      recorder.Show();
    }

    public bool UploadToFTP(MemoryStream ms, string filename, bool overwrite)
    {
      this.ProgressBar.Start(filename, ms.Length);

      string strPath = ftpPath;
      if (!strPath.StartsWith("/")) {
        strPath = "/" + strPath;
      }
      if (!strPath.EndsWith("/")) {
        strPath += "/";
      }

      FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ftpServer + strPath + filename);
      ftp.Proxy = null; //TODO: Ftp Proxy? (From SO: "If the specified proxy is an HTTP proxy, only the DownloadFile, ListDirectory, and ListDirectoryDetails commands are supported.")
      ftp.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
      ftp.Method = WebRequestMethods.Ftp.UploadFile;
      ftp.UsePassive = ftpPassive;
      ftp.UseBinary = ftpBinary;

      //TODO: Implement overwrite

      Stream stream = ftp.GetRequestStream();

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
          // Remove the file from the server..
          //TODO: Make this a setting?
          FtpWebRequest ftpDelete = (FtpWebRequest)FtpWebRequest.Create("ftp://" + ftpServer + strPath + filename);
          ftpDelete.Proxy = null;
          ftpDelete.Method = WebRequestMethods.Ftp.DeleteFile;
          ftpDelete.Credentials = ftp.Credentials;
          ftpDelete.UsePassive = ftpPassive;
          ftpDelete.UseBinary = ftpBinary;
          ftpDelete.GetResponse();
          ftpDelete.Abort();

          ftp.Abort();
          ms.Dispose();

          ftp = null;
          ms = null;

          return false;
        }
        this.ProgressBar.Set(i);
      }

      stream.Close();
      stream.Dispose();
      ftp.Abort();

      this.ProgressBar.Done();

      return true;
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
