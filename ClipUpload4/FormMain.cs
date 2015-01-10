using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Collections;
using GlobalHook;
using GlobalHook.WinApi;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using Nimble.JSON;
using ClipUpload4.Properties;

namespace ClipUpload4
{
  public partial class FormMain : Form
  {
    public class ShortcutInfo
    {
      public Action Action;
      public bool ModCtrl = false;
      public bool ModAlt = false;
      public bool ModShift = false;
      public Keys Key = 0;
    }

    internal class LogInfo
    {
      public string URL;
      public string Info;
      public Addon Addon;
      public string AddonPath;
      public Image IconBitmap;
    }

    public List<ShortcutInfo> Shortcuts = new List<ShortcutInfo>();
    public Settings settings;

    bool mustExit = false;
    bool mustHide = false;

    public string Version;

    public KeyboardHookListener keyboardListener;
    public KeyEventHandler keyboardHandler;

    internal List<LogInfo> LogInfos = new List<LogInfo>();

    public AddonHelper.Addon.cClipboard Clipboard = new AddonHelper.Addon.cClipboard();

    public FormMain()
    {
      InitializeComponent();

      AddonHelper.Android.AllOK();
    }

    public WebProxy GetProxy()
    {
      if (settings.GetBool("ProxyEnabled")) {
        string hostName = settings.GetString("ProxyHost");
        int hostPort = settings.GetInt("ProxyPort");

        WebProxy ret = new WebProxy(hostName, hostPort);

        string credentialsUsername = settings.GetString("ProxyUsername");
        string credentialsPassword = settings.GetString("ProxyPassword");

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

    public void JustUploaded(string url, string info, Type typeAddon, string addonPath)
    {
      Addon addon = null;
      foreach (Addon a in AddonManager.Addons) {
        if (a.ApplyObject != null && a.ApplyObject.GetType() == typeAddon) {
          addon = a;
          break;
        }
      }

      if (addon != null) {
        int iNewCount = addon.Stats.GetInt("UploadCount") + 1;
        addon.Stats.SetInt("UploadCount", iNewCount);
        addon.Stats.Save();
        addon.ListItem.SubItems[1].Text = iNewCount.ToString();
      }

      LogInfos.Add(new LogInfo() {
        URL = url,
        Info = info,
        Addon = addon,
        AddonPath = addonPath,
        IconBitmap = (File.Exists(addonPath + "/Icon.ico") ? new Icon(addonPath + "/Icon.ico").ToBitmap() : null)
      });

      if (LogInfos.Count > 5) { // todo not hardcode
        LogInfos.RemoveRange(0, LogInfos.Count - 5); // todo not hardcode
      }
    }

    void IPC_Server()
    {
      TcpListener listener = new TcpListener(IPAddress.Loopback, 33404);
      listener.Start();

      while (true) {
        TcpClient client = listener.AcceptTcpClient();
        try {
          using (StreamReader reader = new StreamReader(client.GetStream())) {
            string strAddonPath = reader.ReadLine();
            string strAddonIdentifier = reader.ReadLine();
            string strLine = reader.ReadLine();
            List<string> files = new List<string>();
            while (strLine != "END") {
              files.Add(strLine);
              strLine = reader.ReadLine();
            }
            foreach (Addon addon in AddonManager.Addons) {
              if (addon.Path == strAddonPath) {
                this.Invoke(new Action(delegate
                {
                  addon.ApplyObject.ShellCalled(strAddonIdentifier, files.ToArray());
                }));
                break;
              }
            }
          }
        } catch (Exception ex) {
          Program.Debug("IPC Server exception thrown: \"" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + "\"");
          break;
        }
      }

      listener.Stop();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      string path = Environment.CommandLine.Trim(' ', '"');

      if (!(path.Contains('\\') || path.Contains('/'))) {
        Console.WriteLine("Please start Clipupload from Explorer, or add the absolute path.");
        mustExit = true;
        this.Close();
        return;
      }

      string curDir = Directory.GetCurrentDirectory();

      Directory.SetCurrentDirectory(path.Substring(0, path.LastIndexOf('\\')));

      this.LoadSettings();
      this.LoadAddons();
      this.UpdateList();

      this.Text = "ClipUpload " + Version;

      if (this.settings.GetBool("CheckForUpdates")) {
        try {
          WebClient wc = new WebClient();
          wc.Proxy = GetProxy();
          string strResult = wc.DownloadString("https://nimble.tools/ping/update?product=2&cptversion=0");
          dynamic res = Json.JsonDecode(strResult);
          if (res["status"] != "OK") {
            throw new Exception();
          }
          if (res["version"] != Version) {
            int iLatestMinor = int.Parse(res["version"].Split('.').Last());
            int iMinor = int.Parse(Version.Split('.').Last());
            if (iLatestMinor > iMinor) {
              this.Text += " (outdated)";
              this.Tray.ShowBalloonTip(5, "ClipUpload Update", "A new update for ClipUpload is available, version " + res["version"] + ". Visit https://nimble.tools/clipupload to get the new version.", ToolTipIcon.Info);
            } else {
              this.Text += " (pre-release)";
              this.Tray.ShowBalloonTip(5, "ClipUpload Pre-release", "You are running on a pre-release version of ClipUpload, version " + Version + ". The latest stable version is " + res["version"] + ". Please report all bugs.", ToolTipIcon.Warning);
            }
          }
        } catch (Exception ex) {
          Program.Debug("Update check threw " + ex.GetType().FullName + ": '" + ex.Message + "'");
        }
      }

      if (curDir != Directory.GetCurrentDirectory()) {
        mustHide = true;
      }

      keyboardHandler = new KeyEventHandler(keyboardListener_KeyDown);

      this.keyboardListener = new KeyboardHookListener(new GlobalHooker());
      this.keyboardListener.Enabled = true;
      this.keyboardListener.KeyDown += keyboardHandler;

      new Thread(new ThreadStart(IPC_Server)).Start();
    }

    public void MigrateSettings()
    {
      //NOTE: This function is ONLY for 3.xx to 4.xx and should not be used for any more config updates.
      //      Config updates are now handled through settings.txt.clean file comparisons against the live configs.
      //      This function should eventually be DEPRECATED, as well as all the similar functions in addons!

      if (this.settings.GetString("Version") == "3.00") {
        // Migrate from old 3.00 config file
        this.settings.SetString("Version", "3.01");
        this.settings.SetBool("ProgressBar", true);
        this.settings.SetBool("PortableProgressBar", false);

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.01") {
        // Migrate from old 3.01 config file
        this.settings.SetString("Version", "3.02");
        this.settings.SetBool("ProxyEnabled", false);
        this.settings.SetString("ProxyHost", "");
        this.settings.SetInt("ProxyPort", 8080);
        this.settings.SetString("ProxyUsername", "");
        this.settings.SetString("ProxyPassword", "");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.02") {
        // Migrate from old 3.02 config file
        this.settings.SetString("Version", "3.03");
        this.settings.SetBool("DragExtra", false);
        this.settings.SetString("DragExtraName", "Paint");
        this.settings.SetString("DragExtraPath", "C:\\Windows\\system32\\mspaint.exe");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.03") {
        // Migrate from old 3.03 config file
        this.settings.SetString("Version", "3.04");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.04") {
        // Migrate from old 3.04 config file
        this.settings.SetString("Version", "3.10");
        this.settings.SetInt("DragEditor", 1);
        this.settings.SetInt("DragAnimFPS", 10);
        this.settings.SetBool("BackupsEnabled", false);
        this.settings.SetString("BackupsPath", "Backup");
        this.settings.SetString("BackupsFormat", "%DATE% %TIME% %FILENAME%");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.10") {
        // Migrate from old 3.10 config file
        this.settings.SetString("Version", "3.11");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.11") {
        // Migrate from old 3.11 config file
        this.settings.SetString("Version", "3.12");

        this.settings.Save();
      }

      if (this.settings.GetString("Version") == "3.12") {
        // Migrate from old 3.12 config file
        this.settings.SetString("Version", "4.00");
        this.settings.SetBool("Watermark", false);
        this.settings.SetString("WatermarkText", "");
        this.settings.SetString("WatermarkFontFamily", "Arial");
        this.settings.SetInt("WatermarkFontSize", 12);
        this.settings.SetBool("WatermarkFontBold", false);
        this.settings.SetString("WatermarkColor", "0,0,0");
        this.settings.SetInt("WatermarkLocation", 3);
        this.settings.SetInt("WatermarkTransparency", 128);

        this.settings.SetBool("Resize", false);
        this.settings.SetInt("ResizeWidth", 1920);
        this.settings.SetInt("ResizeHeight", 1080);

        this.settings.SetBool("RandomFilenameSuffixDate", true);
        this.settings.SetBool("RandomFilenameCase", true);

        this.settings.SetBool("ShowDragScreenshot", true);
        this.settings.SetBool("ShowDragAnimation", false);
        this.settings.SetBool("ShowShortHistory", true);
        this.settings.SetBool("ShowSeparators", true);
        this.settings.SetBool("ShowShortInfo", false);

        this.settings.Save();
      }
    }

    public void CleanConfigs()
    {
      // get all clean filenames (settings.txt.clean and possibly others)
      string[] cleanConfigs = Directory.GetFiles(".", "*.txt.clean", SearchOption.AllDirectories);
      foreach (string cleanConfig in cleanConfigs) {
        // get the live filename
        string strPathConfig = cleanConfig.Substring(0, cleanConfig.Length - ".clean".Length);

        // if no live config file yet
        if (!File.Exists(strPathConfig)) {
          // copy .txt.clean to .txt!
          File.Copy(cleanConfig, strPathConfig);
        } else {
          // otherwise, let's start comparing them
          Settings settingsClean = new Settings(cleanConfig);
          Settings settingsLive = new Settings(strPathConfig);

          int iUpdates = 0;
          // for every key in the clean file
          foreach (KeyValuePair<string, string> entry in settingsClean.Keys) {
            // if it doesn't exist in the live file
            if (!settingsLive.Contains(entry.Key)) {
              // add it to the live file
              settingsLive.SetString(entry.Key, entry.Value);
              iUpdates++;
            }

            // if it is of name "Version" or "AddonsURL", make sure it's the right updated value
            if (entry.Key == "Version" || entry.Key == "AddonsURL") {
              if (settingsLive.GetString(entry.Key) != entry.Value) {
                settingsLive.SetString(entry.Key, entry.Value);
                iUpdates++;
              }
            }
          }

          // eventually save
          if (iUpdates > 0) {
            settingsLive.Save();
          }
        }
      }

      GC.Collect();
    }

    public void LoadSettings()
    {
      this.CleanConfigs();

      this.settings = new Settings("settings.txt");
      this.MigrateSettings(); //TODO: Deprecate this!

      this.Version = this.settings.GetString("Version");
      this.panelDonate.Visible = this.settings.GetBool("DonateVisible");

      AddonHelper.Android.Enabled = this.settings.GetBool("Android");
      if (AddonHelper.Android.Enabled) {
        AddonHelper.Android.EnsureServer();
      }
    }

    public void LoadShortcuts()
    {
      this.Shortcuts.Clear();
      foreach (Addon addon in AddonManager.Addons) {
        if (!addon.Enabled) {
          continue;
        }

        AddonHelper.MenuEntry[] MenuItems = addon.ApplyObject.Menu();

        foreach (AddonHelper.MenuEntry MenuItem in MenuItems) {
          if (MenuItem.ShortcutModifiers != "" && MenuItem.ShortcutKey != "") {
            string[] parts = MenuItem.ShortcutModifiers.Split('+');
            bool reqCtrl = false, reqAlt = false, reqShift = false;
            foreach (string part in parts) {
              switch (part.ToLower().Trim()) {
                case "ctrl": reqCtrl = true; break;
                case "alt": reqAlt = true; break;
                case "shift": reqShift = true; break;
              }
            }

            if (new List<string>(Enum.GetNames(typeof(Keys)).AsEnumerable<string>()).Contains(MenuItem.ShortcutKey)) {
              Keys reqKey = (Keys)typeof(Keys).GetField(MenuItem.ShortcutKey).GetValue(null);
              this.Shortcuts.Add(new ShortcutInfo() {
                Action = (Action)MenuItem.Action.Clone(),
                ModCtrl = reqCtrl,
                ModAlt = reqAlt,
                ModShift = reqShift,
                Key = reqKey
              });
            }
          }
        }
      }
    }

    public void SaveAddons()
    {
      string strEnabledAddons = "";
      foreach (Addon addon in AddonManager.Addons) {
        if (addon.Enabled) {
          strEnabledAddons += addon.Path + ";";
        }
      }
      settings.SetString("EnabledAddons", strEnabledAddons);
      settings.Save();

      button1.Enabled = false;
      LoadShortcuts();
    }

    public void LoadAddons()
    {
      AddonManager.Tray = this.Tray;
      AddonManager.LoadAddons();

      bool bMigrated = false;
      foreach (Addon addon in AddonManager.Addons) {
        if (!addon.Enabled) {
          addon.Enabled = settings.GetString("EnabledAddons").Contains(addon.Path + ";");
        } else {
          bMigrated = true;
        }
        if (addon.Enabled) {
          addon.ApplyObject.Initialize();
        }
      }

      if (this.settings.GetString("EnabledAddons") == "(init)") {
        string strEnabledAddons = "";
        foreach (Addon addon in AddonManager.Addons) {
          if (!bMigrated || addon.Enabled) {
            strEnabledAddons += addon.Path + ";";
          }
        }
        settings.SetString("EnabledAddons", strEnabledAddons);
        settings.Save();
      }

      LoadShortcuts();
    }

    public void UpdateList()
    {
      ImageList imgList = new ImageList();
      imgList.ImageSize = new Size(16, 16);
      imgList.ColorDepth = ColorDepth.Depth32Bit;

      listAddons.Items.Clear();

      int c = 0;
      foreach (Addon addon in AddonManager.Addons) {
        if (addon.Info.Icon == null) {
          // default icon if none other is provided
          imgList.Images.Add(this.Icon.ToBitmap());
        } else {
          imgList.Images.Add(addon.Info.Icon);
        }

        ListViewItem lvi = listAddons.Items.Add(addon.Info.Name);
        lvi.SubItems.Add(addon.Stats.GetString("UploadCount"));
        lvi.SubItems.Add(addon.Info.Author);
        lvi.ImageIndex = c;
        lvi.UseItemStyleForSubItems = false;

        addon.ListItem = lvi;

        ListViewItem.ListViewSubItem enabled = lvi.SubItems.Add(addon.Enabled ? "Yes" : "No");
        enabled.ForeColor = addon.Enabled ? Color.FromArgb(50, 200, 50) : Color.FromArgb(200, 50, 50);

        c++;
      }

      listAddons.SmallImageList = imgList;
    }

    private void button4_Click(object sender, EventArgs e)
    {
      KillMe();
    }

    public void KillMe()
    {
      mustExit = true;
      this.Close();
    }

    private void listAddons_MouseUp(object sender, MouseEventArgs e)
    {
      if (listAddons.SelectedItems.Count == 1 && e.Button == MouseButtons.Right) {
        Addon addon = AddonManager.Addons[listAddons.SelectedItems[0].Index];

        ContextMenuStrip cms = new ContextMenuStrip();
        ToolStripItem tsi;

        tsi = cms.Items.Add(addon.Enabled ? "Disable" : "Enable");
        tsi.Image = addon.Enabled ? this.iconList.Images[2] : this.iconList.Images[3];
        tsi.Click += new EventHandler(delegate
        {
          addon.Enabled = !addon.Enabled;

          if (addon.Enabled) {
            addon.ApplyObject.Initialize();
          } else {
            addon.ApplyObject.Uninitialize();
          }

          UpdateList();
          button1.Enabled = true;
        });

        if (addon.Enabled) {
          tsi = cms.Items.Add("Settings");
          tsi.Image = this.iconList.Images[0];
          tsi.Click += new EventHandler(delegate
          {
            addon.ApplyObject.Settings();
          });
        }

        if (addon.Info.URL != null) {
          tsi = cms.Items.Add("Website");
          tsi.Image = this.iconList.Images[7];
          tsi.Click += new EventHandler(delegate
          {
            Process.Start(addon.Info.URL);
          });
        }

        if (addon.Info.URL_Author != null) {
          tsi = cms.Items.Add("Author's website");
          tsi.Image = this.iconList.Images[8];
          tsi.Click += new EventHandler(delegate
          {
            Process.Start(addon.Info.URL_Author);
          });
        }

        /*tsi = cms.Items.Add("Remove");
        tsi.Click += new EventHandler(delegate
        {
            addon.ApplyObject.Removed();

            addon.Assembly = null;
            Addons.Remove(addon);
        });*/

        listAddons.ContextMenuStrip = cms;
      }
    }

    private void listAddons_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (listAddons.SelectedItems.Count == 1) {
        Addon addon = AddonManager.Addons[listAddons.SelectedItems[0].Index];
        if (addon.Enabled) {
          addon.ApplyObject.Settings();
        }
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      UpdateList();

      Hide();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      SaveAddons();

      Hide();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      SaveAddons();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!mustExit && e.CloseReason != CloseReason.WindowsShutDown) {
        e.Cancel = true;
        Hide();
      } else {
        foreach (Addon addon in AddonManager.Addons) {
          addon.ApplyObject.Uninitialize();
        }
      }
    }

    private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
    {
      Tray.Visible = false;
      if (AddonHelper.Android.Enabled) {
        AddonHelper.Android.KillServer();
      }
      Environment.Exit(0);
    }

    void keyboardListener_KeyDown(object sender, KeyEventArgs e)
    {
      bool control = ModifierKeys.HasFlag(Keys.Control);
      bool shift = ModifierKeys.HasFlag(Keys.Shift);
      bool alt = ModifierKeys.HasFlag(Keys.Alt);

      foreach (ShortcutInfo shortcut in this.Shortcuts) {
        bool modifiersOK = (control || !shortcut.ModCtrl) &&
                           (shift || !shortcut.ModShift) &&
                           (alt || !shortcut.ModAlt);
        if (modifiersOK && e.KeyCode == shortcut.Key) {
          e.SuppressKeyPress = true;
          // fix for events in drag window not working after using shortcuts :I
          new Thread(() => {
            this.Invoke(new Action(() => {
              shortcut.Action();
            }));
          }).Start();
          break;
        }
      }
    }

    bool ClipboardContainsImage = false;
    bool ClipboardContainsFileList = false;
    bool ClipboardContainsText = false;

    bool AddMenuEntryToCollection(ToolStripItemCollection collection, AddonHelper.MenuEntry entry)
    {
      if (!entry.Visible) {
        return false;
      }

      string strText = entry.Text;

      if (entry.IsDragItem) {
        if (!this.settings.GetBool("ShowDragScreenshot")) {
          return false;
        }
        strText += " (Drag screenshot)";
      } else if (entry.IsAnimateItem) {
        if (!this.settings.GetBool("ShowDragAnimation")) {
          return false;
        }
        strText += " (Drag gif)";
      } else if (entry.IsAndroidScreenshotItem) {
        if (!this.settings.GetBool("AndroidScreenshots")) {
          return false;
        }
      } else if (entry.IsAndroidVideoItem) {
        if (!this.settings.GetBool("AndroidVideos")) {
          return false;
        }
      } else if (entry.ShowShortInfo && settings.GetBool("ShowShortInfo")) {
        string strShort = "";

        if (ClipboardContainsImage) {
          strShort = "Image";
        }

        if (ClipboardContainsFileList) {
          strShort = Clipboard.GetFileDropList().Count + " files";
        }

        if (ClipboardContainsText) {
          int bytes = Clipboard.GetText().Length;
          string strBytes = bytes.ToString();
          string measure = "bytes";
          if (bytes > 1000 * 1000) {
            strBytes = Math.Round(bytes / 1000.0d / 1000.0d, 2).ToString();
            measure = "MB";
          } else if (bytes > 1000) {
            strBytes = Math.Round(bytes / 1000.0d, 2).ToString();
            measure = "KB";
          }
          strShort = "Text: " + strBytes + " " + measure;
        }

        strText += " (" + strShort + ")";
      }

      ToolStripMenuItem tsi = (ToolStripMenuItem)collection.Add(strText);

      if (entry.IsAndroid) {
        tsi.Image = Resources.AndroidIcon;
      } else {
        tsi.Image = entry.Image;
      }

      if (entry.Action != null) {
        Action temp = entry.Action;
        Action tempSecondary = entry.ActionSecondary;
        tsi.MouseUp += new MouseEventHandler((sender, args) => {
          if (args.Button == MouseButtons.Left) {
            temp.Invoke();
          } else if (tempSecondary != null && args.Button == MouseButtons.Right) {
            tempSecondary.Invoke();
          }
        });
      }

      if (entry.ShortcutModifiers != "" && entry.ShortcutKey != "") {
        tsi.ShortcutKeyDisplayString = entry.ShortcutModifiers + "+" + entry.ShortcutKey.Trim('+');
      }

      foreach (AddonHelper.MenuEntry subEntry in entry.SubEntries) {
        AddMenuEntryToCollection(tsi.DropDownItems, subEntry);
      }

      return true;
    }

    private void Tray_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Right) {
        return;
      }

      GC.Collect();

      ContextMenuStrip cms = new ContextMenuStrip();

      ClipboardContainsImage = Clipboard.ContainsImage();
      ClipboardContainsFileList = Clipboard.ContainsFileDropList();
      ClipboardContainsText = Clipboard.ContainsText();

      foreach (Addon addon in AddonManager.Addons) {
        if (!addon.Enabled || addon.ApplyObject == null) {
          continue;
        }

        int MenuItemsAdded = 0;

        addon.ApplyObject.ClipboardContainsImage = ClipboardContainsImage;
        addon.ApplyObject.ClipboardContainsFileList = ClipboardContainsFileList;
        addon.ApplyObject.ClipboardContainsText = ClipboardContainsText;

        AddonHelper.MenuEntry[] MenuItems = addon.ApplyObject.Menu();
        for (int i = 0; i < MenuItems.Length; i++) {
          if (!MenuItems[i].Visible) {
            continue;
          }
          if (AddMenuEntryToCollection(cms.Items, MenuItems[i])) {
            MenuItemsAdded++;
          }
        }

        if (MenuItemsAdded > 0 && this.settings.GetBool("ShowSeparators")) {
          cms.Items.Add(new ToolStripSeparator());
        }
      }

      if (!this.settings.GetBool("ShowSeparators")) {
        // we need to add a separator here because otherwise there will be no separator between addons and history/options
        cms.Items.Add(new ToolStripSeparator());
      }

      ToolStripItem tsi;

      if (this.settings.GetBool("ShowShortHistory") && LogInfos.Count > 0) {
        foreach (LogInfo logInfo in LogInfos.Reverse<LogInfo>()) {
          LogInfo temp = logInfo;

          string filename = "";
          try {
            filename = Path.GetFileName(temp.URL);
          } catch { }

          tsi = (ToolStripMenuItem)cms.Items.Add(filename + " - " + temp.Info);
          tsi.Click += new EventHandler(delegate { Clipboard.SetText(temp.URL); });
          tsi.Image = temp.IconBitmap;
        }
        cms.Items.Add(new ToolStripSeparator());
      }

      tsi = (ToolStripMenuItem)cms.Items.Add("Settings");
      tsi.Image = this.iconList.Images[0];
      tsi.Click += new EventHandler(delegate { Show(); });

      tsi = (ToolStripMenuItem)cms.Items.Add("Upload Log");
      tsi.Image = this.iconList.Images[1];
      tsi.Click += new EventHandler(delegate { button7_Click(null, null); });

      if (settings.GetBool("BackupsEnabled")) {
        tsi = (ToolStripMenuItem)cms.Items.Add("Backups");
        tsi.Image = this.iconList.Images[4];
        tsi.Click += new EventHandler(delegate
        {
          Process.Start("explorer", "\"" + Path.GetFullPath(settings.GetString("BackupsPath")) + "\"");
        });
      }

      tsi = (ToolStripMenuItem)cms.Items.Add("Exit");
      tsi.Click += new EventHandler(delegate { KillMe(); });

      // 5th anniversary
      if (!this.settings.Contains("Cats")) {
        DateTime now = DateTime.Now;
        if (now.Day == 14 && now.Month == 7 && now.Year == 2015) {
          cms.Items.Add(new ToolStripSeparator());

          tsi = (ToolStripMenuItem)cms.Items.Add("Happy 5th Anniversary, Clipupload!");
          tsi.Image = this.iconList.Images[6];
          tsi.Click += new EventHandler(delegate
          {
            this.settings.SetBool("Cats", true);
            this.settings.Save();
            MessageBox.Show("You may now upload cat watermarks. Enjoy.", "Clipupload 5th Anniversary", MessageBoxButtons.OK, MessageBoxIcon.Information);
          });
        }
      } else {
        cms.Items.Add(new ToolStripSeparator());

        tsi = (ToolStripMenuItem)cms.Items.Add("Disable anniversary cats");
        tsi.Image = this.iconList.Images[6];
        tsi.Click += new EventHandler(delegate
        {
          this.settings.Delete("Cats");
          this.settings.Save();
          MessageBox.Show("And the cat watermarks are now gone.", "Clipupload 5th Anniversary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
      }

      Tray.ContextMenuStrip = cms;
    }

    private void Tray_BalloonTipClicked(object sender, EventArgs e)
    {
      // When the notify balloon tip is shown, this will always be called when the icon is clicked,
      // which cancels out the above event. Thus, we redirect this one in that case to the above method.
      Tray_MouseDown(null, new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));
    }

    private void Tray_DoubleClick(object sender, EventArgs e)
    {
      Show();
    }

    private void button5_Click(object sender, EventArgs e)
    {
      new FormSettings(this).ShowDialog();
    }

    private void Form1_Activated(object sender, EventArgs e)
    {
      if (mustHide) {
        Hide();
        mustHide = false;
      }
    }

    private void button6_Click(object sender, EventArgs e)
    {
      Process.Start(settings.GetString("AddonsURL"));
    }

    private void button7_Click(object sender, EventArgs e)
    {
      new FormUploadLog(this).ShowDialog();
    }

    private void pictureDonate_Click(object sender, EventArgs e)
    {
      Process.Start("http://sourceforge.net/donate/index.php?group_id=340379");
    }
  }
}
