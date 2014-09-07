namespace AddonHelper
{
  partial class FormFilename
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFilename));
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonOK = new System.Windows.Forms.Button();
      this.labelPrompt = new System.Windows.Forms.Label();
      this.textFilename = new System.Windows.Forms.TextBox();
      this.checkBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // buttonCancel
      // 
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.Location = new System.Drawing.Point(349, 92);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 3;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // buttonOK
      // 
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.Location = new System.Drawing.Point(268, 92);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(75, 23);
      this.buttonOK.TabIndex = 2;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
      // 
      // labelPrompt
      // 
      this.labelPrompt.Location = new System.Drawing.Point(12, 9);
      this.labelPrompt.Name = "labelPrompt";
      this.labelPrompt.Size = new System.Drawing.Size(412, 30);
      this.labelPrompt.TabIndex = 1;
      this.labelPrompt.Text = "labelPrompt";
      // 
      // textFilename
      // 
      this.textFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textFilename.Location = new System.Drawing.Point(12, 42);
      this.textFilename.Name = "textFilename";
      this.textFilename.Size = new System.Drawing.Size(412, 20);
      this.textFilename.TabIndex = 0;
      // 
      // checkBox
      // 
      this.checkBox.AutoSize = true;
      this.checkBox.Location = new System.Drawing.Point(12, 68);
      this.checkBox.Name = "checkBox";
      this.checkBox.Size = new System.Drawing.Size(73, 17);
      this.checkBox.TabIndex = 1;
      this.checkBox.Text = "checkBox";
      this.checkBox.UseVisualStyleBackColor = true;
      // 
      // FormFilename
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(436, 127);
      this.Controls.Add(this.checkBox);
      this.Controls.Add(this.textFilename);
      this.Controls.Add(this.labelPrompt);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.buttonCancel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormFilename";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Custom filename";
      this.Load += new System.EventHandler(this.FormFilename_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonOK;
    public System.Windows.Forms.Label labelPrompt;
    public System.Windows.Forms.TextBox textFilename;
    public System.Windows.Forms.CheckBox checkBox;
  }
}