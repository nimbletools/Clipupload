using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClipUpload4
{
  public partial class FormSettings : Form
  {
    FormMain mainClass;

    RegistryKey autostartRegKey;

    public FormSettings(FormMain mainClass)
    {
      this.mainClass = mainClass;

      InitializeComponent();

      // check for old startup registry entry
      RegistryKey keyOld = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
      if (keyOld.GetValue("ClipUpload3") != null) {
        keyOld.DeleteValue("ClipUpload3", false);
      }

      autostartRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

      checkAutostart.Checked = autostartRegKey.GetValue("ClipUpload4") != null;
      checkHideDonate.Checked = !mainClass.settings.GetBool("DonateVisible");

      checkProgressBar.Checked = mainClass.settings.GetBool("ProgressBar");
      checkPortableProgressbar.Checked = mainClass.settings.GetBool("PortableProgressBar");
      checkUpdates.Checked = mainClass.settings.GetBool("CheckForUpdates");

      checkBackupsEnable.Checked = mainClass.settings.GetBool("BackupsEnabled");
      textBackupsPath.Text = mainClass.settings.GetString("BackupsPath");
      textBackupsFormat.Text = mainClass.settings.GetString("BackupsFormat");

      checkRandomFilenameSuffixDate.Checked = mainClass.settings.GetBool("RandomFilenameSuffixDate");
      checkRandomFilenameCase.Checked = mainClass.settings.GetBool("RandomFilenameCase");

      checkUseProxy.Checked = mainClass.settings.GetBool("ProxyEnabled");
      textProxyHost.Text = mainClass.settings.GetString("ProxyHost");
      numProxyPort.Value = mainClass.settings.GetInt("ProxyPort");
      textProxyUsername.Text = mainClass.settings.GetString("ProxyUsername");
      textProxyPassword.Text = mainClass.settings.GetString("ProxyPassword");

      radioEditorBuiltin.Checked = mainClass.settings.GetInt("DragEditor") == 0;
      radioEditorProcess.Checked = mainClass.settings.GetInt("DragEditor") == 1;
      textDragExtraName.Text = mainClass.settings.GetString("DragExtraName");
      textDragExtraPath.Text = mainClass.settings.GetString("DragExtraPath");
      numAnimFPS.Value = mainClass.settings.GetInt("DragAnimFPS");
      checkAnimAutoStart.Checked = mainClass.settings.GetBool("DragAnimAuto");

      checkWatermark.Checked = mainClass.settings.GetBool("Watermark");
      textWatermarkText.Text = mainClass.settings.GetString("WatermarkText");
      numWatermarkFontSize.Value = mainClass.settings.GetInt("WatermarkFontSize");
      checkWatermarkFontBold.Checked = mainClass.settings.GetBool("WatermarkFontBold");
      string[] astrColor = mainClass.settings.GetString("WatermarkColor").Split(',');
      Color color = Color.FromArgb(int.Parse(astrColor[0]), int.Parse(astrColor[1]), int.Parse(astrColor[2]));
      picWatermarkColorPreview.BackColor = color;
      comboWatermarkPosition.SelectedIndex = mainClass.settings.GetInt("WatermarkLocation");
      numWatermarkTransparancy.Value = mainClass.settings.GetInt("WatermarkTransparency");

      int iSelectedFontFamily = 0;
      string strCurrentFamily = mainClass.settings.GetString("WatermarkFontFamily");
      int ctFamilies = FontFamily.Families.Length;
      string[] familyNames = new string[ctFamilies];
      for (int i = 0; i < ctFamilies; i++) {
        FontFamily fam = FontFamily.Families[i];
        familyNames[i] = fam.Name;
        if (strCurrentFamily == fam.Name) {
          iSelectedFontFamily = i;
        }
      }
      comboWatermarkFontFamily.Items.AddRange(familyNames.ToArray());
      comboWatermarkFontFamily.SelectedIndex = iSelectedFontFamily;

      checkResize.Checked = mainClass.settings.GetBool("Resize");
      numResizeWidth.Value = mainClass.settings.GetInt("ResizeWidth");
      numResizeHeight.Value = mainClass.settings.GetInt("ResizeHeight");

      checkShowDragScreenshot.Checked = mainClass.settings.GetBool("ShowDragScreenshot");
      checkShowDragAnimation.Checked = mainClass.settings.GetBool("ShowDragAnimation");
      checkShowShortHistory.Checked = mainClass.settings.GetBool("ShowShortHistory");
      checkShowSeparators.Checked = mainClass.settings.GetBool("ShowSeparators");
      checkShowShortInfo.Checked = mainClass.settings.GetBool("ShowShortInfo");

      checkAndroid.Checked = mainClass.settings.GetBool("Android");
      checkAndroidScreenshot.Checked = mainClass.settings.GetBool("AndroidScreenshots");
      checkAndroidVideo.Checked = mainClass.settings.GetBool("AndroidVideos");
      numAndroidVideoBitrate.Value = mainClass.settings.GetInt("AndroidVideoBitrate");
      textAndroidTempPath.Text = mainClass.settings.GetString("AndroidTempPath");
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (checkAutostart.Checked)
        autostartRegKey.SetValue("ClipUpload4", Application.ExecutablePath);
      else
        autostartRegKey.DeleteValue("ClipUpload4", false);

      mainClass.settings.SetBool("DonateVisible", !checkHideDonate.Checked);

      mainClass.settings.SetBool("ProgressBar", checkProgressBar.Checked);
      mainClass.settings.SetBool("PortableProgressBar", checkPortableProgressbar.Checked);
      mainClass.settings.SetBool("CheckForUpdates", checkUpdates.Checked);

      mainClass.settings.SetBool("BackupsEnabled", checkBackupsEnable.Checked);
      mainClass.settings.SetString("BackupsPath", textBackupsPath.Text);
      mainClass.settings.SetString("BackupsFormat", textBackupsFormat.Text);

      mainClass.settings.SetBool("RandomFilenameSuffixDate", checkRandomFilenameSuffixDate.Checked);
      mainClass.settings.SetBool("RandomFilenameCase", checkRandomFilenameCase.Checked);

      mainClass.settings.SetBool("ProxyEnabled", checkUseProxy.Checked);
      mainClass.settings.SetString("ProxyHost", textProxyHost.Text);
      mainClass.settings.SetInt("ProxyPort", (int)numProxyPort.Value);
      mainClass.settings.SetString("ProxyUsername", textProxyUsername.Text);
      mainClass.settings.SetString("ProxyPassword", textProxyPassword.Text);

      mainClass.settings.SetInt("DragEditor", radioEditorBuiltin.Checked ? 0 : 1);
      mainClass.settings.SetString("DragExtraName", textDragExtraName.Text);
      mainClass.settings.SetString("DragExtraPath", textDragExtraPath.Text);
      mainClass.settings.SetInt("DragAnimFPS", (int)numAnimFPS.Value);
      mainClass.settings.SetBool("DragAnimAuto", checkAnimAutoStart.Checked);

      mainClass.settings.SetBool("Watermark", checkWatermark.Checked);
      mainClass.settings.SetString("WatermarkText", textWatermarkText.Text);
      mainClass.settings.SetString("WatermarkFontFamily", comboWatermarkFontFamily.GetItemText(comboWatermarkFontFamily.SelectedItem));
      mainClass.settings.SetInt("WatermarkFontSize", (int)numWatermarkFontSize.Value);
      mainClass.settings.SetBool("WatermarkFontBold", checkWatermarkFontBold.Checked);
      Color c = picWatermarkColorPreview.BackColor;
      mainClass.settings.SetString("WatermarkColor", c.R + "," + c.G + "," + c.B);
      mainClass.settings.SetInt("WatermarkLocation", comboWatermarkPosition.SelectedIndex);
      mainClass.settings.SetInt("WatermarkTransparency", numWatermarkTransparancy.Value);

      mainClass.settings.SetBool("Resize", checkResize.Checked);
      mainClass.settings.SetInt("ResizeWidth", (int)numResizeWidth.Value);
      mainClass.settings.SetInt("ResizeHeight", (int)numResizeHeight.Value);

      mainClass.settings.SetBool("ShowDragScreenshot", checkShowDragScreenshot.Checked);
      mainClass.settings.SetBool("ShowDragAnimation", checkShowDragAnimation.Checked);
      mainClass.settings.SetBool("ShowShortHistory", checkShowShortHistory.Checked);
      mainClass.settings.SetBool("ShowSeparators", checkShowSeparators.Checked);
      mainClass.settings.SetBool("ShowShortInfo", checkShowShortInfo.Checked);

      mainClass.settings.SetBool("Android", checkAndroid.Checked);
      mainClass.settings.SetBool("AndroidScreenshots", checkAndroidScreenshot.Checked);
      mainClass.settings.SetBool("AndroidVideos", checkAndroidVideo.Checked);
      mainClass.settings.SetInt("AndroidVideoBitrate", (int)numAndroidVideoBitrate.Value);
      mainClass.settings.SetString("AndroidTempPath", textAndroidTempPath.Text);

      mainClass.settings.Save();

      this.mainClass.LoadSettings();
      this.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void button3_Click(object sender, EventArgs e)
    {
      MessageBox.Show(
        "If \"Edit process\" is enabled, it allows you to press the P key while dragging to open the current selection in an " +
        "application such as MSPaint or any other image editing software. This way, you can edit your image there and remove " +
        "unneeded parts or draw arrows around things that might be interesting. You can go full-out on these settings.", "Clipupload", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button4_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Executables (*.exe)|*.exe|Batch scripts (*.bat)|*.bat|All files (*.*)|*.*";
      if (dialog.ShowDialog() == DialogResult.OK) {
        textDragExtraPath.Text = dialog.FileName;
      }
    }

    private long Epoch()
    {
      return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }

    private Random rnd = new Random();
    private string RandomString(int len, string chars = "abcdefghijklmnopqrstuvwxyz0123456789")
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

    private void button5_Click(object sender, EventArgs e)
    {
      string backupDirMessage = "";
      try {
        backupDirMessage = "\nYour files will be backed up to '" + Path.GetFullPath(textBackupsPath.Text) + "'";
      } catch (Exception ex) {
        Program.Debug("Settings backup help button threw " + ex.GetType().FullName + ": '" + ex.Message + "'");
      }

      MessageBox.Show("This is the formatting of the filename that is going to be saved in the above path. You can use the following variables in your formatting:\n\n" +
                      "  %ADDON% - FTP/Imgur/...\n" +
                      "  %DATE% - " + DateTime.Now.ToString("d").Replace('/', '-') + "\n" +
                      "  %TIME% - " + DateTime.Now.ToString("t").Replace(':', '.') + "\n" +
                      "  %EPOCH% - " + Epoch() + "\n" +
                      "  %EPOCHX% - " + Epoch().ToString("x8") + "\n" +
                      "  %FILENAME% - " + RandomString(5) + ".png\n" +
                      backupDirMessage, "Backups", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void button6_Click(object sender, EventArgs e)
    {
      ColorDialog colorDialog = new ColorDialog();
      if (colorDialog.ShowDialog() == DialogResult.OK) {
        picWatermarkColorPreview.BackColor = colorDialog.Color;
      }
    }

    private void button7_Click(object sender, EventArgs e)
    {
      Process.Start(new ProcessStartInfo() {
        FileName = "srm.exe",
        Verb = "runas",
        Arguments = "install ClipUploadShellExtension.dll -codebase"
      });
    }
  }
}
