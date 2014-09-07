using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AddonHelper
{
  public partial class FormGifEditor : Form
  {
    public Bitmap[] Frames;
    public Bitmap[] FrameEdits;
    public FormRecorder Recorder;

    public FormGifEditor(Bitmap[] frames, FormRecorder rec)
    {
      InitializeComponent();

      this.Recorder = rec;
      this.Frames = frames;
      this.FrameEdits = new Bitmap[this.Frames.Length];

      this.hScrollBar1.Maximum = frames.Length + this.hScrollBar1.LargeChange - 1;

      int spacew = this.Width - this.pictureBox1.Width;
      int spaceh = this.Height - this.pictureBox1.Height;

      this.Size = new Size(this.Frames[0].Width + spacew, this.Frames[0].Height + spaceh);
      this.RefreshImage();

      this.comboQuality.SelectedIndex = 0;
    }

    private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      if (e.NewValue == e.OldValue)
        return;

      int i = e.NewValue;
      if (i >= this.Frames.Length) i = this.Frames.Length - 1;

      this.RefreshImage(i);
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      this.Close();
      this.Recorder.Close();
    }

    private void buttonUpload_Click(object sender, EventArgs e)
    {
      int w = (int)((float)this.Frames[0].Width * ((float)this.trackScale.Value / 100f));
      int h = (int)((float)this.Frames[0].Height * ((float)this.trackScale.Value / 100f));

      // fix offset for gif encoder, i dont know why, i dont know how, go fuck yourself.
      if (w % 4 != 0) w += 4 - w % 4;
      if (h % 4 != 0) h += 4 - h % 4;

      for (int i = 0; i < this.Frames.Length; i++) {
        if (this.trackScale.Value != 100 || this.Frames[0].Width != w || this.Frames[0].Height != h) {
          Bitmap resized = new Bitmap(w, h);
          Graphics g2 = Graphics.FromImage(resized);
          g2.DrawImage(this.Frames[i], 0, 0, w, h);
          g2.Dispose();

          Bitmap old = this.Frames[i];
          this.Frames[i] = resized;
          old.Dispose();
        }

        if (this.FrameEdits[i] == null)
          continue;

        Bitmap curframe = this.Frames[i];
        Graphics g = Graphics.FromImage(curframe);
        g.DrawImage(this.FrameEdits[i], 0, 0, w, h);
        g.Dispose();

        this.FrameEdits[i].Dispose();
      }

      this.Recorder.HQMode = this.comboQuality.Text == "High quality";

      this.Recorder.Frames = new List<Bitmap>(this.Frames);
      this.Recorder.StartEncoding();
      this.Close();
    }

    private void buttonEdit_Click(object sender, EventArgs e)
    {
      int framenum = this.hScrollBar1.Value;
      if (framenum >= this.Frames.Length) framenum = this.Frames.Length - 1;

      string filename = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp\\" + Addon.RandomString(16) + ".png";

      try {
        MemoryStream mem = new MemoryStream();
        this.pictureBox1.Image.Save(mem, ImageFormat.Png);
        File.WriteAllBytes(filename, mem.ToArray());
        mem.Dispose();
      } catch (Exception ex) {
        MessageBox.Show("Sorry, Windows GDI is derping, we cannot do this.. (" + (ex.InnerException != null ? ex.InnerException.Message : ex.Message) + ")");
        return;
      }

      Process p = Process.Start("mspaint.exe", filename);
      p.WaitForExit();

      Bitmap img = (Bitmap)Bitmap.FromFile(filename);
      Bitmap editimg = new Bitmap(img.Width, img.Height);

      BitmapData newdata = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
      BitmapData realdata = this.Frames[framenum].LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
      BitmapData editdata = editimg.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

      IntPtr newptr = newdata.Scan0;
      IntPtr realptr = realdata.Scan0;
      IntPtr editptr = editdata.Scan0;

      byte[] newarr = new byte[Math.Abs(newdata.Stride) * img.Height];
      byte[] realarr = new byte[Math.Abs(realdata.Stride) * img.Height];
      byte[] editarr = new byte[Math.Abs(realdata.Stride) * img.Height];

      System.Runtime.InteropServices.Marshal.Copy(newptr, newarr, 0, editarr.Length);
      System.Runtime.InteropServices.Marshal.Copy(realptr, realarr, 0, realarr.Length);
      System.Runtime.InteropServices.Marshal.Copy(editptr, editarr, 0, editarr.Length);

      for (int i = 0; i < newarr.Length; i += 4) {
        if (BitConverter.ToInt32(newarr, i) != BitConverter.ToInt32(realarr, i)) {
          editarr[i + 0] = newarr[i + 0];
          editarr[i + 1] = newarr[i + 1];
          editarr[i + 2] = newarr[i + 2];
          editarr[i + 3] = newarr[i + 3];
        }
      }

      Marshal.Copy(editarr, 0, editptr, editarr.Length);

      img.UnlockBits(newdata);
      editimg.UnlockBits(realdata);
      this.Frames[framenum].UnlockBits(realdata);

      this.FrameEdits[this.hScrollBar1.Value] = editimg;

      img.Dispose();

      this.RefreshImage();
    }

    private void buttonDelete_Click(object sender, EventArgs e)
    {
      if (this.Frames.Length == 1)
        return;

      int framenum = this.hScrollBar1.Value;
      if (framenum >= this.Frames.Length) framenum = this.Frames.Length - 1;

      Bitmap[] bmpnew = new Bitmap[this.Frames.Length - 1];
      Bitmap[] bmpedit = new Bitmap[bmpnew.Length];
      for (int i = 0; i < this.Frames.Length; i++) {
        if (i == framenum)
          continue;

        bmpnew[i > this.hScrollBar1.Value ? i - 1 : i] = this.Frames[i];
        bmpedit[i > this.hScrollBar1.Value ? i - 1 : i] = this.FrameEdits[i];
      }

      this.Frames = bmpnew;
      this.FrameEdits = bmpedit;
      this.hScrollBar1.Maximum = this.Frames.Length + this.hScrollBar1.LargeChange - 1;
      this.RefreshImage();
    }

    private void buttonCopy_Click(object sender, EventArgs e)
    {
      int framenum = this.hScrollBar1.Value;
      if (framenum >= this.Frames.Length) framenum = this.Frames.Length - 1;

      int framenum2 = this.hScrollBar1.Value + 1;
      if (framenum2 >= this.Frames.Length) framenum2 = this.Frames.Length - 1;

      if (framenum == framenum2 || this.FrameEdits[framenum] == null)
        return;

      this.FrameEdits[framenum2] = new Bitmap(this.FrameEdits[framenum]);
      this.hScrollBar1.Value++;
      this.RefreshImage();
    }

    private void buttonDeleteEdit_Click(object sender, EventArgs e)
    {
      int framenum = this.hScrollBar1.Value;
      if (framenum >= this.Frames.Length) framenum = this.Frames.Length - 1;

      if (this.FrameEdits[framenum] == null)
        return;

      this.FrameEdits[framenum].Dispose();
      this.FrameEdits[framenum] = null;

      this.RefreshImage(framenum);
    }

    private void RefreshImage(int i = -1)
    {
      if (i == -1)
        i = this.hScrollBar1.Value;

      if (i >= this.Frames.Length) i = this.Frames.Length - 1;

      if (this.pictureBox1.Image != null)
        this.pictureBox1.Image.Dispose();

      int w = (int)((float)this.Frames[0].Width * ((float)this.trackScale.Value / 100f));
      int h = (int)((float)this.Frames[0].Height * ((float)this.trackScale.Value / 100f));

      if (w == 0) w = 1;
      if (h == 0) h = 1;

      Bitmap bmp = new Bitmap(w, h);

      {
        Graphics g = Graphics.FromImage(bmp);

        g.DrawImage(this.Frames[i], 0, 0, bmp.Width, bmp.Height);

        if (this.FrameEdits[i] != null)
          g.DrawImage(this.FrameEdits[i], 0, 0, bmp.Width, bmp.Height);

        g.Dispose();
      }

      this.pictureBox1.Image = bmp;

      this.Text = "Gif Editor " + (i + 1) + "/" + this.Frames.Length;
    }

    private void comboQuality_SelectedIndexChanged(object sender, EventArgs e)
    {
      //this.RefreshImage();
    }

    private void trackScale_Scroll(object sender, EventArgs e)
    {
      this.RefreshImage();
    }

    private void buttonRender_Click(object sender, EventArgs e)
    {
      this.Enabled = false;

      int i = this.hScrollBar1.Value;
      if (i >= this.Frames.Length) i = this.Frames.Length - 1;

      if (this.pictureBox1.Image != null)
        this.pictureBox1.Image.Dispose();

      int w = (int)((float)this.Frames[0].Width * ((float)this.trackScale.Value / 100f));
      int h = (int)((float)this.Frames[0].Height * ((float)this.trackScale.Value / 100f));

      // fix offset for gif encoder, i dont know why, i dont know how, once again, go fuck yourself.
      if (w % 4 != 0) w += 4 - w % 4;
      if (h % 4 != 0) h += 4 - h % 4;

      Bitmap bmp = new Bitmap(w, h);
      Graphics g = Graphics.FromImage(bmp);

      g.DrawImage(this.Frames[i], 0, 0, bmp.Width, bmp.Height);

      if (this.FrameEdits[i] != null)
        g.DrawImage(this.FrameEdits[i], 0, 0, bmp.Width, bmp.Height);

      g.Dispose();

      MemoryStream mem = new MemoryStream();
      GifCreator gif = new GifCreator();
      gif.HQMode = this.comboQuality.Text == "High quality";
      gif.Start(mem);
      gif.WriteFrames(new Bitmap[] { bmp }, null);
      gif.Finish();

      mem.Position = 0;
      bmp = (Bitmap)Image.FromStream(mem);

      this.pictureBox1.Image = bmp;

      this.Enabled = true;
    }
  }
}
