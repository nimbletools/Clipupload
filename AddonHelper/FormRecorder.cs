using Gif.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AddonHelper
{
  public partial class FormRecorder : Form
  {
    [StructLayout(LayoutKind.Sequential)]
    private struct CURSORINFO
    {
      public Int32 cbSize;
      public Int32 flags;
      public IntPtr hCursor;
      public POINTAPI ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINTAPI
    {
      public int x;
      public int y;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorInfo(out CURSORINFO pci);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

    private const Int32 CURSOR_SHOWING = 0x0001;
    private const Int32 DI_NORMAL = 0x0003;

    public FormRecordingRect Rect;

    public bool Recording;
    public bool SnapBusy;
    public bool Encoding;

    public List<Bitmap> Frames = new List<Bitmap>();

    public float FPS = 30;
    public float RealFPS;
    public bool HQMode = true;

    public Action<DragCallback> Callback;

    [DllImport("dwmapi.dll", PreserveSig = false)]
    public static extern bool DwmIsCompositionEnabled();

    public bool DwmEnabled
    {
      get
      {
        if (Environment.OSVersion.Version.Major >= 6)
          return DwmIsCompositionEnabled();
        else
          return false;
      }
    }

    protected override bool ShowWithoutActivation
    {
      get { return this.Encoding; }
    }

    protected override CreateParams CreateParams
    {
      get
      {
        if (this.Encoding) {
          CreateParams p = base.CreateParams;
          p.ExStyle |= 8; // WS_EX_TOPMOST
          return p;
        }
        return base.CreateParams;
      }
    }

    public FormRecorder(FormRecordingRect formrect, Action<DragCallback> callback)
    {
      InitializeComponent();

      this.Rect = formrect;
      this.Callback = callback;
    }

    private void buttonStart_Click(object sender, EventArgs e)
    {
      this.buttonStart.Enabled = false;
      this.buttonStop.Enabled = true;
      this.buttonRetry.Enabled = true;
      this.numericFPS.Enabled = false;

      this.FPS = (float)this.numericFPS.Value;

      this.Recording = true;
      new Thread(new ThreadStart(this.SnapThread)).Start();
      this.timerCapture.Start();
    }

    private void buttonStop_Click(object sender, EventArgs e)
    {
      this.buttonStart.Enabled = true;
      this.buttonStop.Enabled = false;
      this.buttonRetry.Enabled = false;
      this.numericFPS.Enabled = true;

      this.Recording = false;
      this.timerCapture.Stop();

      while (this.SnapBusy) Thread.Sleep(1);
      GC.Collect();

      this.Rect.Close();

      this.buttonStart.Visible = false;
      this.buttonStop.Visible = false;
      this.buttonRetry.Visible = false;
      this.buttonCancel.Visible = false;
      this.labelFPS.Visible = false;
      this.numericFPS.Visible = false;

      this.Height = 51;
      this.TopMost = true;

      Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

      if (this.DwmEnabled) {
        this.Left = workingArea.Left + workingArea.Width - this.Width - 4;
        this.Top = workingArea.Top + workingArea.Height - this.Height - 4;
      } else {
        this.Left = workingArea.Left + workingArea.Width - this.Width;
        this.Top = workingArea.Top + workingArea.Height - this.Height;
      }

      this.progressBarEncoding.Maximum = this.Frames.Count;
      this.progressBarEncoding.Value = 0;
      this.progressBarEncoding.Visible = true;

      this.Hide();
      new FormGifEditor(this.Frames.ToArray(), this).Show();
    }

    public void StartEncoding()
    {
      this.Text = "Encoding gif";
      this.Encoding = true;
      this.Show();
      new Thread(new ThreadStart(this.EncodeThread)).Start();
    }

    List<double> av = new List<double>();
    private void EncodeThread()
    {
      MemoryStream mem = new MemoryStream();

      GifCreator gif = new GifCreator();
      gif.AlphaColor = Color.HotPink;
      gif.HQMode = this.HQMode;
      gif.SetFPS(this.RealFPS);

      gif.Start(mem);
      HPStopwatch watch = new HPStopwatch();
      watch.Start();
      Thread.Sleep(1);
      watch.Stop();

      watch.Start();
      gif.WriteFrames(this.Frames.ToArray(), delegate(int framenum)
      {
        watch.Stop();

        av.Add(watch.GetElapsedTimeInMicroseconds() / 1000);
        if (av.Count > 20)
          av.RemoveAt(0);

        double eta = 0;
        foreach (double num in av) eta += num;
        eta /= av.Count;

        eta *= this.Frames.Count - framenum;
        eta /= 1000;
        eta -= eta % 0.1;

        this.Invoke(new Action(delegate
        {
          this.progressBarEncoding.Value++;
          this.labelStatus.Text = "ETA: " + eta.ToString("0.0") + " seconds (" + ((double)mem.Length / 1024d / 1024d).ToString("0.00") + "MB)";
        }));

        watch.Start();
      });

      gif.Finish();

      this.Invoke(new Action(delegate
      {
        this.Close();
        this.Callback(new DragCallback() {
          Type = DragCallbackType.Gif,
          Animation = mem
        });
      }));
    }

    private void buttonRetry_Click(object sender, EventArgs e)
    {
      this.buttonStart.Enabled = true;
      this.buttonStop.Enabled = false;
      this.buttonRetry.Enabled = false;

      this.Recording = false;

      while (this.SnapBusy) Thread.Sleep(1);
      this.Frames.Clear();
      GC.Collect();
    }

    private void timerCapture_Tick(object sender, EventArgs e)
    {
      this.labelStatus.Text = (this.Frames.Count * (1f / this.FPS)).ToString("0.0") + " seconds, " + this.Frames.Count + " frames, " + this.RealFPS.ToString("0.0") + " FPS";
    }

    private void SnapThread()
    {
      this.SnapBusy = true;

      DateTime lastframe = DateTime.Now;
      DateTime reallastframe = DateTime.Now;
      float targetfps = 1000f / this.FPS;

      List<float> frametimes = new List<float>();
      while (true) {
        while ((DateTime.Now - lastframe).TotalMilliseconds <= targetfps && this.Recording) {
          Thread.Sleep(1);
          continue;
        }

        frametimes.Add((float)(DateTime.Now - reallastframe).TotalMilliseconds);
        if (frametimes.Count > 20)
          frametimes.RemoveAt(0);

        this.RealFPS = 0;
        foreach (float num in frametimes) this.RealFPS += num;
        this.RealFPS /= frametimes.Count;
        this.RealFPS = targetfps / this.RealFPS * this.FPS;

        reallastframe = DateTime.Now;

        lastframe = lastframe.AddMilliseconds(targetfps);

        Size s = new System.Drawing.Size(this.Rect.Width - this.Rect.BorderSize * 2, this.Rect.Height - this.Rect.BorderSize * 2);

        Bitmap frame = new Bitmap(s.Width, s.Height);
        Graphics g = Graphics.FromImage(frame);
        g.CopyFromScreen(new Point(this.Rect.Location.X + this.Rect.BorderSize, this.Rect.Location.Y + this.Rect.BorderSize), Point.Empty, s);

        CURSORINFO pci;
        pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));

        if (GetCursorInfo(out pci)) {
          if (pci.flags == CURSOR_SHOWING) {
            var hdc = g.GetHdc();
            DrawIconEx(hdc, pci.ptScreenPos.x - (this.Rect.Location.X + this.Rect.BorderSize), pci.ptScreenPos.y - (this.Rect.Location.Y + this.Rect.BorderSize), pci.hCursor, 0, 0, 0, IntPtr.Zero, DI_NORMAL);
            g.ReleaseHdc();
          }
        }

        if (!this.Recording)
          break;

        this.Frames.Add(frame);

        if (this.Frames.Count % 10 == 0)
          GC.Collect();
      }

      this.SnapBusy = false;
    }

    private void FormRecorder_Shown(object sender, EventArgs e)
    {
      this.Top = this.Rect.Top + this.Rect.Height / 2 - this.Height / 2;
      this.Left = this.Rect.Left + this.Rect.Width / 2 - this.Width / 2;
    }

    private void FormRecorder_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.Rect.Close();
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }
  }
}
