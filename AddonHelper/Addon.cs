using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SWF = System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections.Specialized;

namespace AddonHelper
{
  public enum DragCallbackType { Image, Animation, Gif, None }
  public class DragCallback
  {
    /// <summary>
    /// What kind of callback this is
    /// </summary>
    public DragCallbackType Type;

    /// <summary>
    /// The image that the user dragged
    /// </summary>
    public Image Image;

    /// <summary>
    /// A MemoryStream containing the buffer of the animation
    /// </summary>
    public MemoryStream Animation;

    /// <summary>
    /// Whether to ask for custom filename (must be handled by addons)
    /// </summary>
    public bool CustomFilename;
  }

  /// <summary>
  /// Used for Menu() call
  /// </summary>
  public class MenuEntry
  {
    public bool IsDragItem = false;
    public bool IsAnimateItem = false;
    public bool ShowShortInfo = true;

    public bool IsAndroid = false;
    public bool IsAndroidScreenshotItem = false;
    public bool IsAndroidVideoItem = false;

    public bool Visible = true;

    public string Text = "";
    public Image Image = null;
    public Action Action = null;
    public Action ActionSecondary = null;
    public string ShortcutModifiers = "";
    public string ShortcutKey = "";

    public List<MenuEntry> SubEntries = new List<MenuEntry>();

    /// <summary>
    /// Function that allows for a continues stream of code because it returns 'this'
    /// </summary>
    public MenuEntry AddSubEntry(MenuEntry entry)
    {
      SubEntries.Add(entry);
      return this;
    }
  }

  /// <summary>
  /// Used for GetInfo() call
  /// </summary>
  public class AddonInfo
  {
    /// <summary>
    /// The name of the addon.
    /// </summary>
    public string Name;
    /// <summary>
    /// The author of the addon.
    /// </summary>
    public string Author;
    /// <summary>
    /// Icon to use that represents this addon.
    /// </summary>
    public Image Icon;
    /// <summary>
    /// A generic URL to get more info on the service that this addon provides.
    /// </summary>
    public string URL;
    /// <summary>
    /// A URL to the author's website.
    /// </summary>
    public string URL_Author;
  }

  /// <summary>
  /// Used for file drop lists
  /// </summary>
  public class UploadedFile
  {
    public string URL = "";
    public string Info = "";
  }

  /// <summary>
  /// Attribute for int and long values to set the minimum/maximum values
  /// </summary>
  public class NumericSetting : Attribute
  {
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    public NumericSetting(int min, int max)
    {
      MinValue = min;
      MaxValue = max;
    }
  }

  public class ShellInfo
  {
    /// <summary>
    /// Whether or not this item is visible in the shell extension menu
    /// </summary>
    public bool Visible = true;
    /// <summary>
    /// The text that shows up for this shell extension
    /// </summary>
    public string Text;
    /// <summary>
    /// The image that shows up left of the shell extension
    /// </summary>
    public Image Image;
    /// <summary>
    /// An identifier that will be passed to your addon via the ShellCalled() method
    /// </summary>
    public string Identifier;
  }

  public class CustomFilenameInfo
  {
    /// <summary>
    /// The text the user wrote in the dialog
    /// </summary>
    public string UserInput;
    /// <summary>
    /// A boolean corresponding to the optional checkbox value in the dialog
    /// </summary>
    public bool Checkbox;
  }

  public abstract class Addon
  {
    public NotifyIcon Tray = null;
    public static Random rnd = new Random();
    private Settings appSettings = new Settings("settings.txt");

    public string AddonPath = "";

    public bool ClipboardContainsImage = false;
    public bool ClipboardContainsFileList = false;
    public bool ClipboardContainsText = false;

    // Abstract functions
    public abstract AddonInfo GetInfo();
    public abstract void Initialize();
    public abstract void Uninitialize();
    public abstract MenuEntry[] Menu();
    public abstract void Settings();
    //public abstract void Removed();

    // Virtual functions (compatibility as well as optional methods)
    public virtual ShellInfo[] ShowShell(string[] files) { return new ShellInfo[0]; }
    public virtual void ShellCalled(string identifier, string[] files) { }

    public Addon()
    {
      AddonPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(this.GetType()).Location);
    }

    /// <summary>
    /// Internal initialization hook, should only be called once.
    /// </summary>
    public void InternalInitialize(NotifyIcon tray)
    {
      Tray = tray;
    }

    /// <summary>
    /// Writes to debug.txt
    /// </summary>
    /// <param name="str">Line of text</param>
    public void Debug(string str)
    {
      StreamWriter writer;
      if (File.Exists("debug.txt"))
        writer = File.AppendText("debug.txt");
      else
        writer = new StreamWriter(File.Create("debug.txt"));

      writer.WriteLine("[" + this.GetType().Name + " - " + DateTime.Now.ToString() + "] " + str);
      writer.Close();
      writer.Dispose();
    }

    /// <summary>
    /// Returns the linux epoch time (seconds passed since 1/1/1970)
    /// </summary>
    /// <returns>Epoch time as a long</returns>
    public long Epoch()
    {
      return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    /// <summary>
    /// Transforms an image based on global upload settings
    /// </summary>
    /// <param name="img">The base image</param>
    public Image ImagePipeline(Image img)
    {
      appSettings.Reload();
      Graphics gfx = Graphics.FromImage(img);

      if (appSettings.GetBool("Resize")) {
        int iMaxWidth = appSettings.GetInt("ResizeWidth");
        int iMaxHeight = appSettings.GetInt("ResizeHeight");

        int iWidth = img.Width;
        int iHeight = img.Height;

        if (iWidth > iMaxWidth) {
          iHeight = (int)(((double)iMaxWidth / (double)iWidth) * iHeight);
          iWidth = iMaxWidth;
        }

        if (iHeight > iMaxHeight) {
          iWidth = (int)(((double)iMaxHeight / (double)iHeight) * iWidth);
          iHeight = iMaxHeight;
        }

        Image imgOld = img;
        img = new Bitmap(iWidth, iHeight);
        gfx.Dispose();
        gfx = Graphics.FromImage(img);
        gfx.DrawImage(imgOld, new Rectangle(0, 0, iWidth, iHeight));
        imgOld.Dispose();
      }

      // 2nd anniversary easter egg, cats!
      if (appSettings.Contains("Cats")) {
        try {
          // First, get a random cat picture
          // Can be fetched from placekitten.com/g/<width>/<height>
          WebClient wc = new WebClient();
          Image catImage = Image.FromStream(new MemoryStream(wc.DownloadData("http://placekitten.com/g/" + img.Width + "/" + img.Height)));

          // Create attributes that transform the image before pasting
          ImageAttributes imgAttributes = new ImageAttributes();
          imgAttributes.SetColorMatrix(new ColorMatrix() { Matrix33 = 0.5f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap); // Matrix33 = alpha

          // Now we draw the cat picture with the attributes
          gfx.DrawImage(catImage, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
        } catch { Debug("Failed to put anniversary cat :("); }
      }

      if (appSettings.GetBool("Watermark")) {
        string strText = appSettings.GetString("WatermarkText");
        string strFontFamily = appSettings.GetString("WatermarkFontFamily");
        float fFontSize = appSettings.GetFloat("WatermarkFontSize");
        bool bFontBold = appSettings.GetBool("WatermarkFontBold");
        string[] astrColor = appSettings.GetString("WatermarkColor").Split(',');
        int iLocation = appSettings.GetInt("WatermarkLocation");
        int iTransparency = appSettings.GetInt("WatermarkTransparency");

        FontFamily fntfamFamily = FontFamily.GenericSansSerif;
        foreach (FontFamily fam in FontFamily.Families) {
          if (fam.Name == strFontFamily) {
            fntfamFamily = fam;
            break;
          }
        }

        Font fntFont = new Font(fntfamFamily, fFontSize, bFontBold ? FontStyle.Bold : FontStyle.Regular);
        Brush brsBrush = new SolidBrush(Color.FromArgb(iTransparency, int.Parse(astrColor[0]), int.Parse(astrColor[1]), int.Parse(astrColor[2])));
        PointF pntPoint = PointF.Empty;

        switch (iLocation) {
          case 0: pntPoint = new PointF(10, 10); break; // Top left
          case 1: pntPoint = new PointF(img.Width - gfx.MeasureString(strText, fntFont).Width - 10, 10); break; // Top right
          case 2: pntPoint = new PointF(10, img.Height - gfx.MeasureString(strText, fntFont).Height - 10); break; // Bottom left
          case 3: pntPoint = new PointF(img.Width - gfx.MeasureString(strText, fntFont).Width - 10, img.Height - gfx.MeasureString(strText, fntFont).Height - 10); break; // Bottom right
        }

        gfx.DrawString(strText, fntFont, brsBrush, pntPoint);
      }

      gfx.Dispose();

      return img;
    }

    /// <summary>
    /// Enables or disables shortcuts in ClipUpload.
    /// </summary>
    /// <param name="enabled">Bool to enable or disable</param>
    public static void SetShortcuts(bool enabled)
    {
      Form form = null;
      foreach (Form f in Application.OpenForms) {
        if (f.GetType().Name == "FormMain") {
          form = f;
          break;
        }
      }

      form.Invoke(new Action(delegate
      {
        FieldInfo fiListener = form.GetType().GetField("keyboardListener");
        PropertyInfo piEnabled = fiListener.FieldType.GetProperty("Enabled");
        piEnabled.SetValue(fiListener.GetValue(form), enabled, null);
      }));
    }

    private string formatFilename(string origFilename)
    {
      string fnm = appSettings.GetString("BackupsFormat");
      fnm = fnm.Replace("%ADDON%", this.GetType().Name);
      fnm = fnm.Replace("%DATE%", DateTime.Now.ToString("d").Replace('/', '-'));
      fnm = fnm.Replace("%TIME%", DateTime.Now.ToString("t").Replace(':', '.'));
      fnm = fnm.Replace("%EPOCH%", Epoch().ToString());
      fnm = fnm.Replace("%EPOCHX%", Epoch().ToString("x8"));
      fnm = fnm.Replace("%FILENAME%", origFilename);

      char[] invalidChars = Path.GetInvalidFileNameChars();
      foreach (char c in invalidChars)
        fnm = fnm.Replace(c.ToString(), "");

      return fnm;
    }

    /// <summary>
    /// Make a backup of the given source file
    /// </summary>
    /// <param name="sourceFile">The source filename</param>
    public void Backup(string sourceFile)
    {
      if (appSettings.GetBool("BackupsEnabled")) {
        string path = appSettings.GetString("BackupsPath");

        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);

        string filename = Path.GetFileName(sourceFile);
        string fnm = formatFilename(filename);
        File.Copy(sourceFile, path + "/" + fnm);
      }
    }

    /// <summary>
    /// Make a backup of the given file buffer as the current filename
    /// </summary>
    /// <param name="buffer">The file buffer</param>
    /// <param name="filename">The filename</param>
    public void Backup(byte[] buffer, string filename)
    {
      if (appSettings.GetBool("BackupsEnabled")) {
        string path = appSettings.GetString("BackupsPath");

        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);

        string fnm = formatFilename(filename);
        File.WriteAllBytes(path + "/" + fnm, buffer);
      }
    }

    /// <summary>
    /// Call this when a file has been uploaded, this will copy the URL and add it to the log as well as show a notification
    /// </summary>
    /// <param name="type">The type of thing that's being uploaded (eg. "Image", "Text", "Files")</param>
    /// <param name="URL">The URL</param>
    /// <param name="info">Short info for short upload history in menu</param>
    public void Uploaded(string type, string URL, string info)
    {
      string strMessage = type + " uploaded and URL" + (info.Contains('\n') ? "s" : "") + " copied to clipboard.";
      if (URL != "") {
        Clipboard.SetText(URL);
      } else {
        strMessage = type + " uploaded.";
      }
      Tray.ShowBalloonTip(1000, this.GetType().Name, strMessage, ToolTipIcon.Info);
      if (info != "") {
        AddLog(URL, info);
      }
    }

    /// <summary>
    /// Call this when multiple files have been uploaded (FileDropList), this will copy the final copy and show a notification
    /// </summary>
    /// <param name="finalCopy"></param>
    public void UploadedFiles(List<UploadedFile> files)
    {
      if (files.Count == 0) {
        return;
      }
      string finalCopy = "";
      bool bHasURL = false;
      bool bHasInfo = false;
      foreach (UploadedFile file in files) {
        if (file.URL != "") {
          bHasURL = true;
        }
        if (file.Info != "") {
          bHasInfo = true;
        }
        if (bHasURL && bHasInfo) {
          AddLog(file.URL, file.Info);
        }
        finalCopy += file.URL + "\r\n";
      }
      Clipboard.SetText(finalCopy.Trim('\r', '\n'));
      string mult = files.Count != 1 ? "s" : "";
      string strMessage = files.Count + " file" + mult + " uploaded and URL" + mult + " copied to clipboard.";
      if (!bHasURL) {
        strMessage = files.Count + " file" + mult + " uploaded.";
      }
      Tray.ShowBalloonTip(1000, this.GetType().Name, strMessage, ToolTipIcon.Info);
    }

    /// <summary>
    /// Call this when a file failed to be uploaded, this will hide any progress bar present and show a notification
    /// </summary>
    /// <param name="failReason">Reason of failure, such as an exception message</param>
    public void Failed(string failReason)
    {
      this.ProgressBar.Done();
      Tray.ShowBalloonTip(1000, this.GetType().Name, "Something went wrong, try again.\nFail reason: '" + failReason + "'", ToolTipIcon.Error);
    }

    /// <summary>
    /// Add a recently uploaded file to the upload log
    /// </summary>
    /// <param name="URL">The URL</param>
    /// <param name="info">Additional info that will appear in the recent uploads list</param>
    public void AddLog(string URL, string info)
    {
      Form form = null;
      foreach (Form f in Application.OpenForms) {
        if (f.GetType().Name == "FormMain") {
          form = f;
          break;
        }
      }

      MethodInfo mi = form.GetType().GetMethod("JustUploaded");
      mi.Invoke(form, new object[] { URL, info, this.GetType(), AddonPath });

      StreamWriter writer;
      if (File.Exists("uploadlog.txt"))
        writer = File.AppendText("uploadlog.txt");
      else
        writer = new StreamWriter(File.Create("uploadlog.txt"));

      writer.WriteLine(DateTime.Now.ToString("G") + "|" + URL);
      writer.Close();
      writer.Dispose();
    }

    /// <summary>
    /// Drag to upload
    /// </summary>
    /// <param name="doneDragging">Dragging callback</param>
    public void Drag(Action<DragCallback> doneDragging, bool bAnimationOnly = false)
    {
      FormDrag formDrag = new FormDrag(this);
      formDrag.DoneDragging = doneDragging;
      formDrag.AnimationOnly = bAnimationOnly;
      formDrag.ShowDialog();
    }

    /// <summary>
    /// Populate a keys combobox with possible keys to press
    /// </summary>
    /// <param name="comboBox">The combobox</param>
    public void PopulateKeysCombobox(ComboBox comboBox)
    {
      string[] keys = Enum.GetNames(typeof(Keys));
      foreach (string key in keys)
        comboBox.Items.Add(key);
    }

    /// <summary>
    /// Generate an MD5 hash of the given string
    /// </summary>
    /// <param name="S">The string</param>
    /// <returns>The MD5 hash of the given string</returns>
    public string MD5(string S)
    {
      MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
      byte[] bOut = x.ComputeHash(Encoding.UTF8.GetBytes(S));
      StringBuilder sb = new StringBuilder();
      foreach (byte b in bOut)
        sb.Append(b.ToString("x2").ToLower());
      return sb.ToString();
    }

    /// <summary>
    /// Generate an MD5 hash of the given buffer
    /// </summary>
    /// <param name="b">The buffer</param>
    /// <returns>The MD5 hash of the given buffer</returns>
    public string MD5(byte[] b)
    {
      MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
      byte[] bOut = x.ComputeHash(b);
      StringBuilder sb = new StringBuilder();
      foreach (byte bt in bOut)
        sb.Append(bt.ToString("x2").ToLower());
      return sb.ToString();
    }

    /// <summary>
    /// Returns a random string based on the length and the allowed characters given
    /// </summary>
    /// <param name="len">The length</param>
    /// <param name="chars">Allowed characters</param>
    /// <returns>Random string based on requirements</returns>
    public static string RandomString(int len, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
    {
      string ret = "";
      for (int i = 0; i < len; i++) {
        string a = chars[rnd.Next(chars.Length)].ToString();
        if (rnd.Next(2) == 0)
          ret += a.ToUpper();
        else
          ret += a;
      }
      return ret;
    }

    /// <summary>
    /// Returns a random filename based on the length and the allowed characters given
    /// </summary>
    /// <param name="len">The length</param>
    /// <param name="chars">Allowed characters</param>
    /// <returns>Random string based on requirements</returns>
    public string RandomFilename(int len, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
    {
      string strRet = RandomString(len, chars);
      if (appSettings.GetBool("RandomFilenameSuffixDate")) {
        strRet = DateTime.Now.ToString("yyyyMMdd") + strRet;
      }
      if (!appSettings.GetBool("RandomFilenameCase")) {
        strRet = strRet.ToLower();
      }
      return strRet;
    }

    /// <summary>
    /// Encode a string in base64
    /// </summary>
    /// <param name="input">The string to be encoded</param>
    /// <returns>The encoded base64 string</returns>
    public string base64Encode(string input)
    {
      return Convert.ToBase64String(Encoding.ASCII.GetBytes(input));
    }

    /// <summary>
    /// Decode a base64 string
    /// </summary>
    /// <param name="input">The base64 string to be decoded</param>
    /// <returns>The decoded string</returns>
    public string base64Decode(string input)
    {
      return Encoding.ASCII.GetString(Convert.FromBase64String(input));
    }

    /// <summary>
    /// Alternative of <see cref="M:System.Uri.EscapeDataString(System.String)"/> to allow for much longer data to be escaped
    /// </summary>
    /// <param name="Str">The string to be escaped</param>
    /// <returns>The escaped string</returns>
    public string LongDataEscape(string Str)
    {
      string Output = "";
      int ByteCount = 32766;
      if (Str.Length > ByteCount) {
        for (int i = 0; i < Str.Length; i += ByteCount) {
          if (Str.Length - i < ByteCount)
            Output += Uri.EscapeDataString(Str.Substring(i, Str.Length - i));
          else
            Output += Uri.EscapeDataString(Str.Substring(i, ByteCount));
        }
      } else
        Output = Uri.EscapeDataString(Str);
      return Output;
    }

    /// <summary>
    /// Get a string between 2 other strings from the source string
    /// </summary>
    /// <param name="Source">The source string</param>
    /// <param name="Str1">The first string, for example: "&lt;link&gt;"</param>
    /// <param name="Str2">The second string, for example: "&lt;/link&gt;"</param>
    /// <returns>Returns the string between Str1 and Str2</returns>
    public string GetBetween(string Source, string Str1, string Str2)
    {
      return Source.Split(new string[] { Str1, Str2 }, StringSplitOptions.None)[Source.Contains(Str1) ? 1 : 0];
    }

    /// <summary>
    /// Get a WebProxy object from the global proxy settings in the ClipUpload application
    /// </summary>
    /// <returns>The WebProxy object</returns>
    public WebProxy GetProxy()
    {
      if (appSettings.GetBool("ProxyEnabled")) {
        string hostName = appSettings.GetString("ProxyHost");
        int hostPort = appSettings.GetInt("ProxyPort");

        WebProxy ret = new WebProxy(hostName, hostPort);

        string credentialsUsername = appSettings.GetString("ProxyUsername");
        string credentialsPassword = appSettings.GetString("ProxyPassword");

        if (credentialsUsername != "" || credentialsPassword != "") {
          ret.Credentials = new NetworkCredential(credentialsUsername, credentialsPassword);
        } else {
          ret.UseDefaultCredentials = true;
        }

        return ret;
      } else {
        return null;
      }
    }

    /// <summary>
    /// Returns the image encoder for the given ImageFormat
    /// </summary>
    /// <param name="format">The ImageFormat to get the encoder for</param>
    /// <returns>The ImageCodecInfo object</returns>
    public ImageCodecInfo GetEncoder(ImageFormat format)
    {
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
      foreach (ImageCodecInfo codec in codecs) {
        if (codec.FormatID == format.Guid)
          return codec;
      }
      return null;
    }

    private class SettingsInfo
    {
      public Control Control;
      public FieldInfo Field;
    }

    /// <summary>
    /// Quickly open a settings window with the given object as the settings, see codecat.nl/?p=5 for an example
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="filename">Filename to save it in (usually AddonPath + "/settings.txt")</param>
    /// <param name="title">The title of the settings window (usually GetInfo().Name + " settings")</param>
    public void OpenQuickSettings(object obj, string filename, string title)
    {
      Type type = obj.GetType();
      FieldInfo[] fields = type.GetFields();

      Form formSettings = new Form();
      formSettings.ClientSize = new Size(391, 473);
      formSettings.FormBorderStyle = FormBorderStyle.FixedSingle;
      formSettings.MaximizeBox = false;
      formSettings.MinimizeBox = false;
      formSettings.ShowIcon = false;
      formSettings.Text = title;
      formSettings.StartPosition = FormStartPosition.CenterScreen;

      Button buttonCancel = new Button();
      buttonCancel.Anchor = (AnchorStyles.Right | AnchorStyles.Bottom);
      buttonCancel.Location = new Point(304, 438);
      buttonCancel.Size = new Size(75, 23);
      buttonCancel.Text = "Cancel";
      formSettings.Controls.Add(buttonCancel);

      Button buttonOK = new Button();
      buttonOK.Anchor = (AnchorStyles.Right | AnchorStyles.Bottom);
      buttonOK.Location = new Point(223, 438);
      buttonOK.Size = new Size(75, 23);
      buttonOK.Text = "OK";
      formSettings.Controls.Add(buttonOK);

      List<SettingsInfo> arr = new List<SettingsInfo>();

      int left = 12;
      int top = 9;

      foreach (FieldInfo field in fields) {
        string strName = field.Name;
        string strDescription = "";
        decimal iMin = -99999999;
        decimal iMax = 99999999;

        object[] attributes = field.GetCustomAttributes(false);
        foreach (object attr in attributes) {
          if (attr.GetType() == typeof(System.ComponentModel.DescriptionAttribute)) {
            strDescription = ((System.ComponentModel.DescriptionAttribute)attr).Description;
          }

          if (attr.GetType() == typeof(NumericSetting)) {
            NumericSetting numSet = (NumericSetting)attr;
            iMin = numSet.MinValue;
            iMax = numSet.MaxValue;
          }
        }

        Control control = null;

        if (field.FieldType == typeof(string) || field.FieldType == typeof(float)) {
          Label label = new Label();
          label.Location = new Point(left, top);
          label.AutoSize = true;
          label.Text = strDescription;
          formSettings.Controls.Add(label);
          top += 16;

          TextBox textbox = new TextBox();
          textbox.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
          textbox.Location = new Point(left, top);
          textbox.Size = new Size(formSettings.Width - 30, 20);
          textbox.Text = field.GetValue(obj).ToString();
          formSettings.Controls.Add(textbox);
          top += 23;

          control = textbox;
        } else if (field.FieldType == typeof(bool)) {
          CheckBox checkbox = new CheckBox();
          checkbox.Location = new Point(left, top);
          checkbox.AutoSize = true;
          checkbox.Text = strDescription;
          checkbox.Checked = (bool)field.GetValue(obj);
          formSettings.Controls.Add(checkbox);
          top += 23;

          control = checkbox;
        } else if (field.FieldType == typeof(int) || field.FieldType == typeof(long)) {
          NumericUpDown num = new NumericUpDown();
          num.Location = new Point(left, top);
          num.Size = new Size(120, 20);
          num.Minimum = -9999999999;
          num.Maximum = 9999999999;
          if (field.FieldType == typeof(int)) {
            num.Value = (int)field.GetValue(obj);
          } else if (field.FieldType == typeof(long)) {
            num.Value = (long)field.GetValue(obj);
          }
          formSettings.Controls.Add(num);
          top += 23;

          control = num;
        } else {
          throw new NotImplementedException("Quicksettings type not implemented in AddonHelper: \"" + field.FieldType.FullName + "\"");
        }

        if (control != null) {
          arr.Add(new SettingsInfo() {
            Control = control,
            Field = field
          });
        }
      }

      buttonCancel.Click += new EventHandler(delegate
      {
        formSettings.Close();
      });

      buttonOK.Click += new EventHandler(delegate
      {
        foreach (SettingsInfo info in arr) {
          if (info.Field.FieldType == typeof(string)) {
            info.Field.SetValue(obj, (info.Control as TextBox).Text);
          } else if (info.Field.FieldType == typeof(bool)) {
            info.Field.SetValue(obj, (info.Control as CheckBox).Checked);
          } else if (info.Field.FieldType == typeof(int)) {
            info.Field.SetValue(obj, (int)(info.Control as NumericUpDown).Value);
          } else if (info.Field.FieldType == typeof(long)) {
            info.Field.SetValue(obj, (long)(info.Control as NumericUpDown).Value);
          } else if (info.Field.FieldType == typeof(float)) {
            float f = 0;
            if (float.TryParse((info.Control as TextBox).Text, out f)) {
              info.Field.SetValue(obj, f);
            }
          }
        }
        SerializeSettings(obj, filename);
        formSettings.Close();
      });

      formSettings.ShowDialog();
      formSettings.Dispose();
    }

    /// <summary>
    /// Serialize settings to file from object
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="filename">The filename (usually AddonPath + "/settings.txt")</param>
    public void SerializeSettings(object obj, string filename)
    {
      Type type = obj.GetType();
      Settings settings = new Settings(filename);
      FieldInfo[] fields = type.GetFields();
      foreach (FieldInfo field in fields) {
        settings.SetString(field.Name, field.GetValue(obj).ToString());
      }
      settings.Save();
    }

    /// <summary>
    /// Deserialize settings from file to object
    /// </summary>
    /// <param name="obj">The object</param>
    /// <param name="filename">The filename (usually AddonPath + "/settings.txt")</param>
    public void DeserializeSettings(object obj, string filename)
    {
      Type type = obj.GetType();
      Settings settings = new Settings(filename);
      FieldInfo[] fields = type.GetFields();
      foreach (FieldInfo field in fields) {
        if (settings.Contains(field.Name)) {
          if (field.FieldType == typeof(string)) {
            field.SetValue(obj, settings.GetString(field.Name));
          } else if (field.FieldType == typeof(bool)) {
            field.SetValue(obj, settings.GetBool(field.Name));
          } else if (field.FieldType == typeof(int)) {
            field.SetValue(obj, settings.GetInt(field.Name));
          } else if (field.FieldType == typeof(long)) {
            field.SetValue(obj, settings.GetLong(field.Name));
          } else if (field.FieldType == typeof(float)) {
            field.SetValue(obj, settings.GetFloat(field.Name));
          }
        }
      }
    }

    /// <summary>
    /// Open a customizable user input dialog, commonly used for providing custom filenames and a file overwrite checkbox, but can also be used for other things
    /// </summary>
    /// <param name="prompt">The text to prompt the user with</param>
    /// <param name="showCheckbox">Whether to show a checkbox or not</param>
    /// <param name="checkboxText">The text to show on the checkbox</param>
    /// <param name="def">The default value for the text input</param>
    /// <param name="defCheckbox">The default value for the checkbox</param>
    /// <param name="title">The title of the dialog window</param>
    /// <returns>A CustomFilenameInfo object containing user input, or null if the user clicked Cancel</returns>
    public CustomFilenameInfo OpenCustomFilenameDialog(
      string prompt = "Enter a custom filename:",
      bool showCheckbox = true,
      string checkboxText = "Overwrite file if it exists",
      string def = "",
      bool defCheckbox = true,
      string title = "Custom filename")
    {
      FormFilename form = new FormFilename();
      form.labelPrompt.Text = prompt;
      form.checkBox.Visible = showCheckbox;
      form.checkBox.Text = checkboxText;
      form.textFilename.Text = def;
      form.checkBox.Checked = defCheckbox;
      form.Text = title;

      if (form.ShowDialog() == DialogResult.OK) {
        return form.ReturnValue;
      } else {
        return null;
      }
    }

    public string GetShellText(string[] files, string to, string verb = "Upload")
    {
      if (files.Length == 1) {
        return verb + " " + Path.GetFileName(files[0]) + " to " + to;
      } else {
        return verb + " " + files.Length + " files to " + to;
      }
    }

    public class cProgressBar
    {
      public FormProgressBar Form;

      private string filename = "";
      private long filesize = 0;
      private long lastLocation = 0;
      private bool showSpeed = true;
      private Stopwatch speedTimer = new Stopwatch();

      private bool done = false;
      public bool Canceled = false;

      private Settings appSettings = new Settings("settings.txt");

      /// <summary>
      /// Shows a progressbar window.
      /// </summary>
      /// <param name="Filename">The filename</param>
      /// <param name="Filesize">The filesize</param>
      public void Start(string Filename, long Filesize)
      {
        this.Start(Filename, Filesize, true);
      }

      /// <summary>
      /// Shows a progressbar window.
      /// </summary>
      /// <param name="Filename">The filename</param>
      /// <param name="Filesize">The filesize</param>
      /// <param name="DisplaySpeed">Whether or not to display the speed</param>
      public void Start(string Filename, long Filesize, bool DisplaySpeed)
      {
        Addon.SetShortcuts(false);

        this.reset();
        if (Filesize == 0) return;

        if (!appSettings.GetBool("ProgressBar")) {
          return;
        }

        this.Form = new FormProgressBar(appSettings);
        this.Form.FormClosing += new FormClosingEventHandler(cancelUpload);

        new Thread(new ThreadStart(delegate
        {
          this.filename = Filename;
          this.filesize = (int)Filesize; //TODO: Handle Filesize > int.MaxValue
          this.showSpeed = DisplaySpeed;
          this.updateStatus(0);

          Application.Run(this.Form);
        })).Start();
      }

      private void reset()
      {
        if (this.Form != null && !this.Form.IsDisposed) {
          this.Form.Close();
        }
        this.Form = null;

        this.filename = "";
        this.filesize = 0;
        this.lastLocation = 0;
        this.speedTimer = new Stopwatch();
        this.speedTimer.Start();

        this.done = false;
        this.Canceled = false;
      }

      private void cancelUpload(object sender, FormClosingEventArgs e)
      {
        if (!this.done) {
          this.Canceled = true;
        }
      }

      private double percentage(long currentLocation)
      {
        return Math.Min(100d, 100d / (double)this.filesize * (double)currentLocation);
      }

      private double percentage(long currentLocation, int decimals)
      {
        return Math.Round(this.percentage(currentLocation), decimals);
      }

      private void updateStatus(long currentLocation)
      {
        this.lastLocation = currentLocation;

        string uploadRate = "";
        if (this.showSpeed) {
          long bytesTransfered = currentLocation - this.lastLocation;
          double bytesPerSecond = Math.Round(this.percentage(currentLocation) / 100d * (double)this.filesize) / ((double)this.speedTimer.ElapsedMilliseconds / 1000d);
          double uploadRateBps = Math.Round(bytesPerSecond, 2);
          double uploadRateKBps = Math.Round(uploadRateBps / 1024, 2);
          double uploadRateMBps = Math.Round(uploadRateKBps / 1024, 2);

          uploadRate = uploadRateBps + " B/s";
          if (uploadRateBps > 1024)
            uploadRate = uploadRateKBps + " KB/s";
          if (uploadRateKBps > 1024)
            uploadRate = uploadRateMBps + " MB/s";
        }


        this.Form.Text = this.filename + " - " + this.percentage(currentLocation, 0) + "%" + (this.showSpeed ? " - " + uploadRate : "");

        int percVal = (int)this.percentage(currentLocation);
        this.Form.progressBar.Value = Math.Min(100, percVal + 1); // First set it to it's value + 1 to bypass the Aero animation
        this.Form.progressBar.Value = percVal;
      }

      /// <summary>
      /// Set the current progress
      /// </summary>
      /// <param name="currentLocation">The current location in the file</param>
      public void Set(long currentLocation)
      {
        if (!Canceled && this.Form != null) {
          try {
            this.Form.Invoke(new Action(delegate
            {
              this.updateStatus(currentLocation);
            }));
          } catch { }
        }
      }

      /// <summary>
      /// Close the progressbar window
      /// </summary>
      public void Done()
      {
        Addon.SetShortcuts(true);

        if (!Canceled && this.Form != null) {
          this.done = true;
          try {
            this.Form.Invoke(new Action(delegate
            {
              this.Form.Close();
            }));
          } catch { }
        }
      }
    }

    /// <summary>
    /// Used to invoke a progressbar window
    /// </summary>
    public cProgressBar ProgressBar = new cProgressBar();

    public class cClipboard
    {
      public int RetryTime = 10; // 10ms should be good enough
      public int MaxRetries = 500; // every retry is 10ms, so 100 retries is 1s

      public bool ContainsImage()
      {
        int retries = 0;
        while (true) {
          try {
            bool ret = SWF.Clipboard.ContainsImage();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return false;
          }
          Thread.Sleep(RetryTime);
        }
      }
      public bool ContainsText()
      {
        int retries = 0;
        while (true) {
          try {
            bool ret = SWF.Clipboard.ContainsText();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return false;
          }
          Thread.Sleep(RetryTime);
        }
      }
      public bool ContainsFileDropList()
      {
        int retries = 0;
        while (true) {
          try {
            bool ret = SWF.Clipboard.ContainsFileDropList();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return false;
          }
          Thread.Sleep(RetryTime);
        }
      }

      public void SetText(string strText)
      {
        int retries = 0;
        while (true) {
          try {
            SWF.Clipboard.SetText(strText);
            return;
          } catch { }
          if (++retries >= MaxRetries) {
            return; // :(
          }
          Thread.Sleep(RetryTime);
        }
      }
      public void SetImage(Image img)
      {
        int retries = 0;
        while (true) {
          try {
            SWF.Clipboard.SetImage(img);
            return;
          } catch { }
          if (++retries >= MaxRetries) {
            return; // :(
          }
          Thread.Sleep(RetryTime);
        }
      }

      public Image GetImage()
      {
        int retries = 0;
        while (true) {
          try {
            Image ret = SWF.Clipboard.GetImage();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return null; // :(
          }
          Thread.Sleep(RetryTime);
        }
      }
      public string GetText()
      {
        int retries = 0;
        while (true) {
          try {
            string ret = SWF.Clipboard.GetText();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return ""; // :(
          }
          Thread.Sleep(RetryTime);
        }
      }
      public StringCollection GetFileDropList()
      {
        int retries = 0;
        while (true) {
          try {
            StringCollection ret = SWF.Clipboard.GetFileDropList();
            return ret;
          } catch { }
          if (++retries >= MaxRetries) {
            return new StringCollection(); // :(
          }
          Thread.Sleep(RetryTime);
        }
      }
    }

    /// <summary>
    /// Used as a replacement for the generic "Clipboard" class, which can be unstable at times - cClipboard fixes this problem
    /// </summary>
    public cClipboard Clipboard = new cClipboard();
  }
}
