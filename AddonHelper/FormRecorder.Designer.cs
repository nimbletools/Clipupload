namespace AddonHelper {
    partial class FormRecorder {
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecorder));
      this.buttonStart = new System.Windows.Forms.Button();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonRetry = new System.Windows.Forms.Button();
      this.timerCapture = new System.Windows.Forms.Timer(this.components);
      this.numericFPS = new System.Windows.Forms.NumericUpDown();
      this.labelFPS = new System.Windows.Forms.Label();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.statusStrip = new System.Windows.Forms.StatusStrip();
      this.progressBarEncoding = new System.Windows.Forms.ToolStripProgressBar();
      this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
      ((System.ComponentModel.ISupportInitialize)(this.numericFPS)).BeginInit();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonStart
      // 
      this.buttonStart.Image = ((System.Drawing.Image)(resources.GetObject("buttonStart.Image")));
      this.buttonStart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonStart.Location = new System.Drawing.Point(12, 12);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(61, 58);
      this.buttonStart.TabIndex = 1;
      this.buttonStart.Text = "Record";
      this.buttonStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // buttonStop
      // 
      this.buttonStop.Enabled = false;
      this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
      this.buttonStop.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonStop.Location = new System.Drawing.Point(79, 12);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(61, 58);
      this.buttonStop.TabIndex = 2;
      this.buttonStop.Text = "Stop";
      this.buttonStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // buttonRetry
      // 
      this.buttonRetry.Enabled = false;
      this.buttonRetry.Image = ((System.Drawing.Image)(resources.GetObject("buttonRetry.Image")));
      this.buttonRetry.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonRetry.Location = new System.Drawing.Point(146, 12);
      this.buttonRetry.Name = "buttonRetry";
      this.buttonRetry.Size = new System.Drawing.Size(61, 58);
      this.buttonRetry.TabIndex = 3;
      this.buttonRetry.Text = "Redo";
      this.buttonRetry.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonRetry.UseVisualStyleBackColor = true;
      this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
      // 
      // timerCapture
      // 
      this.timerCapture.Tick += new System.EventHandler(this.timerCapture_Tick);
      // 
      // numericFPS
      // 
      this.numericFPS.Location = new System.Drawing.Point(213, 76);
      this.numericFPS.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
      this.numericFPS.Name = "numericFPS";
      this.numericFPS.Size = new System.Drawing.Size(61, 20);
      this.numericFPS.TabIndex = 6;
      this.numericFPS.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
      // 
      // labelFPS
      // 
      this.labelFPS.AutoSize = true;
      this.labelFPS.Location = new System.Drawing.Point(143, 78);
      this.labelFPS.Name = "labelFPS";
      this.labelFPS.Size = new System.Drawing.Size(64, 13);
      this.labelFPS.TabIndex = 7;
      this.labelFPS.Text = "Target FPS:";
      // 
      // buttonCancel
      // 
      this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
      this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonCancel.Location = new System.Drawing.Point(213, 12);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(61, 58);
      this.buttonCancel.TabIndex = 3;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // statusStrip
      // 
      this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBarEncoding,
            this.labelStatus});
      this.statusStrip.Location = new System.Drawing.Point(0, 107);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(286, 22);
      this.statusStrip.SizingGrip = false;
      this.statusStrip.TabIndex = 10;
      // 
      // progressBarEncoding
      // 
      this.progressBarEncoding.Name = "progressBarEncoding";
      this.progressBarEncoding.Size = new System.Drawing.Size(100, 16);
      this.progressBarEncoding.Visible = false;
      // 
      // labelStatus
      // 
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(153, 17);
      this.labelStatus.Text = "0.0 seconds, 0 frames, 0 FPS";
      // 
      // FormRecorder
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(286, 129);
      this.Controls.Add(this.statusStrip);
      this.Controls.Add(this.labelFPS);
      this.Controls.Add(this.numericFPS);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonRetry);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.buttonStart);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormRecorder";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Animation control";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRecorder_FormClosing);
      this.Shown += new System.EventHandler(this.FormRecorder_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.numericFPS)).EndInit();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Timer timerCapture;
        private System.Windows.Forms.NumericUpDown numericFPS;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBarEncoding;
    }
}