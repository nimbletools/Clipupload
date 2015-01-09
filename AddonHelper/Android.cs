using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using AddonHelper;
using System.Windows.Forms;
using System.Threading;

namespace AddonHelper
{
  public class AndroidDevice
  {
    public string SerialNumber;
    public string Type;
    public string Product;
    public string Model;
    public string Device;
  }

  public static class Android
  {
    public static bool Enabled = false;
    public static string TempPath = "/sdcard/";
    private static Process adbRecording = null;

    /// <summary>
    /// Check if all is OK with the Android Debug Bridge and if there's a device ready.
    /// </summary>
    /// <returns>True if all is OK.</returns>
    public static bool AllOK()
    {
      if (!Enabled) {
        return false;
      }

      if (!TempPath.EndsWith("/")) {
        TempPath += "/";
      }

      if (!File.Exists("adb.exe")) {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Ensure that there is an adb server running in the background.
    /// </summary>
    public static void EnsureServer()
    {
      Process proc = Process.Start(new ProcessStartInfo("adb.exe", "start-server") {
        UseShellExecute = false,
        CreateNoWindow = true
      });
    }

    /// <summary>
    /// Get a list of devices with USB debugging enabled currently connected to the computer.
    /// </summary>
    /// <returns>Array with information about the connected devices.</returns>
    public static AndroidDevice[] ListDevices()
    {
      Process proc = Process.Start(new ProcessStartInfo("adb.exe", "devices -l") {
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardOutput = true
      });
      string strList = proc.StandardOutput.ReadToEnd();
      string[] astrDevices = strList.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
      List<AndroidDevice> ret = new List<AndroidDevice>();
      foreach (string strDeviceLine in astrDevices) {
        string[] deviceInfo = strDeviceLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (deviceInfo[1] == "offline") {
          continue;
        }
        AndroidDevice device = new AndroidDevice();
        device.SerialNumber = deviceInfo[0];
        device.Type = deviceInfo[1];
        device.Product = deviceInfo[2].Substring("product:".Length);
        device.Model = deviceInfo[3].Substring("model:".Length);
        device.Device = deviceInfo[4].Substring("device:".Length);
        ret.Add(device);
      }
      return ret.ToArray();
    }

    /// <summary>
    /// Takes a screenshot on the device via adb, then pulls the screenshot to a temporary file and returns the filename to it.
    /// </summary>
    /// <param name="strDevice">The device serial number to use.</param>
    /// <returns>Path to a temporary png file of the screenshot.</returns>
    public static string PullScreenshot(string strDevice)
    {
      if (!AllOK()) {
        return "";
      }

      FormProgressBar pb = new FormProgressBar(null);
      pb.Text = "Taking Android screenshot...";
      pb.progressBar.Value = 0;
      pb.Show();

      string strAndroidPath = TempPath + "ClipuploadScreenshot.png";

      Process.Start(new ProcessStartInfo("adb.exe", "-s " + strDevice + " shell  screencap -p \"" + strAndroidPath + "\"") {
        UseShellExecute = false,
        CreateNoWindow = true
      }).WaitForExit();

      pb.progressBar.Value = 51;
      pb.progressBar.Value = 50;
      Application.DoEvents();

      string strTempFile = Path.GetTempFileName() + "_ClipuploadScreenshot.png";

      Process.Start(new ProcessStartInfo("adb.exe", "-s " + strDevice + " pull \"" + strAndroidPath + "\" \"" + strTempFile + "\"") {
        UseShellExecute = false,
        CreateNoWindow = true
      }).WaitForExit();

      pb.Close();

      return strTempFile;
    }

    /// <summary>
    /// Start a video capture on the device via adb.
    /// </summary>
    /// <param name="iBitrate">The bitrate. Multiply Mbps by 1000000 for this value. (12000000 = 12 Mbps)</param>
    public static void StartRecording(string strDevice, int iBitrate = 12000000)
    {
      string strAndroidPath = TempPath + "ClipuploadVideo.mp4";

      if (adbRecording != null) {
        throw new Exception("Already recording.");
      }

      adbRecording = Process.Start(new ProcessStartInfo("adb.exe", "-s " + strDevice + " shell screenrecord --bit-rate " + iBitrate + " \"" + strAndroidPath + "\"") {
        UseShellExecute = false,
        CreateNoWindow = true
      });
    }

    /// <summary>
    /// Stop the currently running video capture.
    /// </summary>
    public static void StopRecording()
    {
      if (adbRecording == null) {
        throw new Exception("Not yet recording.");
      }

      adbRecording.Kill();
      adbRecording = null;
    }

    /// <summary>
    /// Pull the last recorded video capture mp4 from the device to a temporary file and returns the filename to it.
    /// </summary>
    /// <returns>Path to a temporary mp4 file of the video.</returns>
    public static string PullRecording(string strDevice)
    {
      if (adbRecording != null) {
        throw new Exception("Still recording.");
      }

      // have to wait for half a second to make sure the device has properly closed the file
      Thread.Sleep(500);

      FormProgressBar pb = new FormProgressBar(null);
      pb.Text = "Pulling Android video...";
      pb.progressBar.Value = 0;
      pb.Show();

      string strAndroidPath = TempPath + "ClipuploadVideo.mp4";
      string strTempFile = Path.GetTempFileName() + "_ClipuploadVideo.mp4";

      Process.Start(new ProcessStartInfo("adb.exe", "-s " + strDevice + " pull \"" + strAndroidPath + "\" \"" + strTempFile + "\"") {
        UseShellExecute = false,
        CreateNoWindow = true
      }).WaitForExit();

      pb.Close();

      return strTempFile;
    }
  }
}
