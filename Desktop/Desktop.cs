using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Specialized;
using AddonHelper;
using System.IO;

namespace Desktop
{
  public class Desktop : Addon
  {
    public Settings settings;

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

    public Desktop()
    {
      Icon icon = new Icon(AddonPath + "/Icon.ico");
      this.bmpIcon = icon.ToBitmap();
      this.bmpIcon16 = new Icon(icon, new Size(16, 16)).ToBitmap();
    }

    public override AddonInfo GetInfo()
    {
      return new AddonInfo() {
        Name = "Desktop",
        Author = "Angelo Geels",
        Icon = bmpIcon16,
        //URL = "https://nimble.tools/clipupload",
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

      ret.Add(new MenuEntry() {
        IsDragItem = true,
        Text = "Desktop",
        Image = this.bmpIcon16,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback)); }),
        ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary)); }),
        ShortcutModifiers = this.shortCutDragModifiers,
        ShortcutKey = this.shortCutDragKey
      });

      ret.Add(new MenuEntry() {
        IsAnimateItem = true,
        Text = "Desktop",
        Image = this.bmpIcon16,
        Action = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallback), true); }),
        ActionSecondary = new Action(delegate { this.Drag(new Action<DragCallback>(DragCallbackSecondary), true); }),
        ShortcutModifiers = this.shortCutAnimModifiers,
        ShortcutKey = this.shortCutAnimKey
      });

      ret.Add(new MenuEntry() {
        Visible = ClipboardContainsImage || ClipboardContainsFileList || ClipboardContainsText,
        Text = "Desktop",
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
        Text = GetShellText(files, "Desktop", "Copy"),
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

    string DesktopPath
    {
      get { return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); }
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

          if (overwrite && File.Exists(DesktopPath + "/" + filename)) {
            File.Delete(DesktopPath + "/" + filename);
          }
          img.Save(DesktopPath + "/" + filename, jpegCodec, jpegParams);
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

        this.Backup(DesktopPath + "/" + filename);
        if (!jpegCompressed) {
          if (overwrite && File.Exists(DesktopPath + "/" + filename)) {
            File.Delete(DesktopPath + "/" + filename);
          }
          img.Save(DesktopPath + "/" + filename, format);
        }

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Image", DesktopPath + "/" + filename, "");
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
        if (overwrite && File.Exists(DesktopPath + "/" + filename)) {
          File.Delete(DesktopPath + "/" + filename);
        }

        FileStream fs = File.OpenWrite(DesktopPath + "/" + filename);
        fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
        fs.Close();
        fs.Dispose();

        this.Backup(DesktopPath + "/" + filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Animation", "", "");
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
        if (overwrite && File.Exists(DesktopPath + "/" + filename)) {
          File.Delete(DesktopPath + "/" + filename);
        }

        File.WriteAllText(DesktopPath + "/" + filename, Text);

        this.Backup(DesktopPath + "/" + filename);

        result = true;
      } catch (Exception ex) { failReason = (ex.InnerException != null ? ex.InnerException.Message : ex.Message); }

      if (result) {
        Uploaded("Text", "", "");
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

          if (overwrite && File.Exists(DesktopPath + "/" + filename)) {
            File.Delete(DesktopPath + "/" + filename);
          }

          File.Copy(file, DesktopPath + "/" + filename);

          uploadedFiles.Add(new UploadedFile());
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
