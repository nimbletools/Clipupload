using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AddonHelper
{
  public partial class FormRecordingRect : Form
  {
    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr window, int index);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public bool IsDragging;
    public int DragStartX;
    public int DragStartY;
    public int BorderSize;

    FormRecordingRectBar BarTop;
    FormRecordingRectBar BarDown;
    FormRecordingRectBar BarLeft;
    FormRecordingRectBar BarRight;

    public FormRecordingRect(Rectangle rect, int bs)
    {
      InitializeComponent();

      this.BarTop = new FormRecordingRectBar(this);
      this.BarDown = new FormRecordingRectBar(this);
      this.BarLeft = new FormRecordingRectBar(this);
      this.BarRight = new FormRecordingRectBar(this);

      this.SuspendLayout();

      this.BorderSize = bs;
      this.Left = rect.Left - bs;
      this.Top = rect.Top - bs;
      this.Width = rect.Width + bs * 2;
      this.Height = rect.Height + bs * 2;

      this.FormRecordingRect_LocationChanged(null, null);
      this.BarTop.Show();
      this.BarDown.Show();
      this.BarLeft.Show();
      this.BarRight.Show();

      int initialStyle = GetWindowLong(this.Handle, -20);
      SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);

      this.ResumeLayout(false);
    }

    private void FormRecordingRect_MouseDown(object sender, MouseEventArgs e)
    {
      this.IsDragging = true;
      this.DragStartX = e.X;
      this.DragStartY = e.Y;
    }

    private void FormRecordingRect_MouseUp(object sender, MouseEventArgs e)
    {
      this.IsDragging = false;
    }

    private void FormRecordingRect_MouseMove(object sender, MouseEventArgs e)
    {
      if (!this.IsDragging)
        return;

      Point m = Cursor.Position;

      this.SuspendLayout();
      this.Left = m.X - this.DragStartX;
      this.Top = m.Y - this.DragStartY;
      this.ResumeLayout(true);
    }

    private void FormRecordingRect_LocationChanged(object sender, EventArgs e)
    {
      this.BarTop.Left = this.Left;
      this.BarTop.Top = this.Top;
      this.BarTop.Width = this.Width;
      this.BarTop.Height = this.BorderSize;

      this.BarDown.Left = this.Left;
      this.BarDown.Top = this.Top + this.Height - this.BorderSize;
      this.BarDown.Width = this.Width;
      this.BarDown.Height = this.BorderSize;

      this.BarLeft.Left = this.Left;
      this.BarLeft.Top = this.Top;
      this.BarLeft.Width = this.BorderSize;
      this.BarLeft.Height = this.Height;

      this.BarRight.Left = this.Left + this.Width - this.BorderSize;
      this.BarRight.Top = this.Top;
      this.BarRight.Width = this.BorderSize;
      this.BarRight.Height = this.Height;

      this.BarTop.Update();
      this.BarDown.Update();
      this.BarLeft.Update();
      this.BarRight.Update();
      this.Update();
    }

    private void FormRecordingRect_Shown(object sender, EventArgs e)
    {
      this.FormRecordingRect_LocationChanged(null, null);
    }

    private void FormRecordingRect_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.BarTop.Close();
      this.BarDown.Close();
      this.BarLeft.Close();
      this.BarRight.Close();
    }

    private void FormRecordingRect_SizeChanged(object sender, EventArgs e)
    {
      this.FormRecordingRect_LocationChanged(null, null);
    }
  }
}
