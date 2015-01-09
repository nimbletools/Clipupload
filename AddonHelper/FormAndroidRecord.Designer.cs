namespace AddonHelper
{
  partial class FormAndroidRecord
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
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
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAndroidRecord));
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonStop = new System.Windows.Forms.Button();
      this.buttonStart = new System.Windows.Forms.Button();
      this.buttonRetry = new System.Windows.Forms.Button();
      this.labelStatus = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // buttonCancel
      // 
      this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
      this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonCancel.Location = new System.Drawing.Point(213, 12);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(61, 58);
      this.buttonCancel.TabIndex = 6;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // buttonStop
      // 
      this.buttonStop.Enabled = false;
      this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
      this.buttonStop.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonStop.Location = new System.Drawing.Point(79, 12);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(61, 58);
      this.buttonStop.TabIndex = 5;
      this.buttonStop.Text = "Stop";
      this.buttonStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // buttonStart
      // 
      this.buttonStart.Image = ((System.Drawing.Image)(resources.GetObject("buttonStart.Image")));
      this.buttonStart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonStart.Location = new System.Drawing.Point(12, 12);
      this.buttonStart.Name = "buttonStart";
      this.buttonStart.Size = new System.Drawing.Size(61, 58);
      this.buttonStart.TabIndex = 4;
      this.buttonStart.Text = "Record";
      this.buttonStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonStart.UseVisualStyleBackColor = true;
      this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
      // 
      // buttonRetry
      // 
      this.buttonRetry.Enabled = false;
      this.buttonRetry.Image = ((System.Drawing.Image)(resources.GetObject("buttonRetry.Image")));
      this.buttonRetry.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
      this.buttonRetry.Location = new System.Drawing.Point(146, 12);
      this.buttonRetry.Name = "buttonRetry";
      this.buttonRetry.Size = new System.Drawing.Size(61, 58);
      this.buttonRetry.TabIndex = 7;
      this.buttonRetry.Text = "Redo";
      this.buttonRetry.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      this.buttonRetry.UseVisualStyleBackColor = true;
      this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
      // 
      // labelStatus
      // 
      this.labelStatus.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelStatus.ForeColor = System.Drawing.Color.Gray;
      this.labelStatus.Location = new System.Drawing.Point(280, 41);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(119, 34);
      this.labelStatus.TabIndex = 8;
      this.labelStatus.Text = "Idle";
      this.labelStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // FormAndroidRecord
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(397, 84);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.buttonRetry);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.buttonStart);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormAndroidRecord";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Record Android screen";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonStop;
    private System.Windows.Forms.Button buttonStart;
    private System.Windows.Forms.Button buttonRetry;
    private System.Windows.Forms.Label labelStatus;

  }
}