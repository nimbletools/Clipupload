using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AddonHelper {
    public partial class FormRecordingRectBar : Form {
        public bool IsDragging;
        public int DragStartX;
        public int DragStartY;
        public int BorderSize;

        public FormRecordingRect Rect;
        public FormRecordingRectBar(FormRecordingRect rect) {
            InitializeComponent();

            this.Rect = rect;
            this.Cursor = Cursors.SizeAll;
        }

        private void FormRecordingRect_MouseDown(object sender, MouseEventArgs e) {
            this.IsDragging = true;
            this.DragStartX = e.X;
            this.DragStartY = e.Y;
        }

        private void FormRecordingRect_MouseUp(object sender, MouseEventArgs e) {
            this.IsDragging = false;
        }

        private void FormRecordingRect_MouseMove(object sender, MouseEventArgs e) {
            if (!this.IsDragging)
                return;

            if (e.Button != MouseButtons.Left) {
                this.IsDragging = false;
                return;
            }

            Point m = Cursor.Position;

            if (ModifierKeys.HasFlag(Keys.Control)) {
                this.Rect.Size = new Size(m.X - this.Rect.Left, m.Y - this.Rect.Top);
                this.DragStartX = e.X;
                this.DragStartY = e.Y;
            } else {
                this.Rect.Left += e.X - this.DragStartX;
                this.Rect.Top += e.Y - this.DragStartY;
            }
        }
    }
}
