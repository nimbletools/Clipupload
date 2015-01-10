using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace AddonHelper
{
  public partial class FormAndroidRecord : Form
  {
    public Action<string> Callback;
    public string DeviceSerial;

    public bool Recording = false;

    public FormAndroidRecord(string strDevice)
    {
      InitializeComponent();

      DeviceSerial = strDevice;
    }

    void StartRecording()
    {
      Recording = true;

      buttonStart.Enabled = false;
      buttonStop.Enabled = true;
      labelStatus.Text = "Recording";
      labelStatus.ForeColor = Color.Red;

      Android.CallbackRecordingFailed = () => {
        this.Invoke(new Action(delegate
        {
          StopRecording();
          MessageBox.Show("Screen recording is not supported on this device.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          this.Close();
        }));
      };
      Android.StartRecording(DeviceSerial);
    }

    void StopRecording()
    {
      Recording = false;

      buttonStart.Enabled = true;
      buttonStop.Enabled = false;
      labelStatus.Text = "Idle";
      labelStatus.ForeColor = Color.Gray;

      Android.StopRecording();
    }

    private void buttonStart_Click(object sender, EventArgs e)
    {
      StartRecording();
    }

    private void buttonStop_Click(object sender, EventArgs e)
    {
      StopRecording();
      this.Hide();

      string strFilename = Android.PullRecording(DeviceSerial);
      if (Callback != null) {
        Callback(strFilename);
      }
    }

    private void buttonRetry_Click(object sender, EventArgs e)
    {
      StopRecording();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      if (Recording) {
        StopRecording();
      }

      base.OnFormClosing(e);
    }
  }
}
