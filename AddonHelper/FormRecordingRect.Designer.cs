﻿namespace AddonHelper {
    partial class FormRecordingRect {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // FormRecordingRect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.HotPink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRecordingRect";
            this.Opacity = 0.5D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FormRecordingRect";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.HotPink;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRecordingRect_FormClosing);
            this.Shown += new System.EventHandler(this.FormRecordingRect_Shown);
            this.LocationChanged += new System.EventHandler(this.FormRecordingRect_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.FormRecordingRect_SizeChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormRecordingRect_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormRecordingRect_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormRecordingRect_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}