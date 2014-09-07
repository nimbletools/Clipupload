namespace AddonHelper {
    partial class FormGifEditor {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGifEditor));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
      this.buttonDelete = new System.Windows.Forms.Button();
      this.buttonEdit = new System.Windows.Forms.Button();
      this.buttonRender = new System.Windows.Forms.Button();
      this.comboQuality = new System.Windows.Forms.ComboBox();
      this.trackScale = new System.Windows.Forms.TrackBar();
      this.buttonDeleteEdit = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.buttonCopy = new System.Windows.Forms.Button();
      this.buttonUpload = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackScale)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(692, 385);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // hScrollBar1
      // 
      this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hScrollBar1.Location = new System.Drawing.Point(0, 385);
      this.hScrollBar1.Name = "hScrollBar1";
      this.hScrollBar1.Size = new System.Drawing.Size(692, 20);
      this.hScrollBar1.TabIndex = 1;
      this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
      // 
      // buttonDelete
      // 
      this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDelete.Location = new System.Drawing.Point(176, 408);
      this.buttonDelete.Name = "buttonDelete";
      this.buttonDelete.Size = new System.Drawing.Size(123, 23);
      this.buttonDelete.TabIndex = 2;
      this.buttonDelete.Text = "Delete Frame";
      this.buttonDelete.UseVisualStyleBackColor = true;
      this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
      // 
      // buttonEdit
      // 
      this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonEdit.Location = new System.Drawing.Point(176, 437);
      this.buttonEdit.Name = "buttonEdit";
      this.buttonEdit.Size = new System.Drawing.Size(123, 23);
      this.buttonEdit.TabIndex = 3;
      this.buttonEdit.Text = "Edit Frame";
      this.buttonEdit.UseVisualStyleBackColor = true;
      this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
      // 
      // buttonRender
      // 
      this.buttonRender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonRender.Location = new System.Drawing.Point(12, 408);
      this.buttonRender.Name = "buttonRender";
      this.buttonRender.Size = new System.Drawing.Size(158, 23);
      this.buttonRender.TabIndex = 10;
      this.buttonRender.Text = "Render frame preview";
      this.buttonRender.UseVisualStyleBackColor = true;
      this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
      // 
      // comboQuality
      // 
      this.comboQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.comboQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboQuality.FormattingEnabled = true;
      this.comboQuality.Items.AddRange(new object[] {
            "High quality",
            "Low quality"});
      this.comboQuality.Location = new System.Drawing.Point(568, 408);
      this.comboQuality.Name = "comboQuality";
      this.comboQuality.Size = new System.Drawing.Size(112, 21);
      this.comboQuality.TabIndex = 9;
      this.comboQuality.SelectedIndexChanged += new System.EventHandler(this.comboQuality_SelectedIndexChanged);
      // 
      // trackScale
      // 
      this.trackScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.trackScale.Location = new System.Drawing.Point(561, 435);
      this.trackScale.Maximum = 100;
      this.trackScale.Name = "trackScale";
      this.trackScale.Size = new System.Drawing.Size(126, 45);
      this.trackScale.TabIndex = 8;
      this.trackScale.TickStyle = System.Windows.Forms.TickStyle.None;
      this.trackScale.Value = 100;
      this.trackScale.Scroll += new System.EventHandler(this.trackScale_Scroll);
      // 
      // buttonDeleteEdit
      // 
      this.buttonDeleteEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonDeleteEdit.Location = new System.Drawing.Point(12, 466);
      this.buttonDeleteEdit.Name = "buttonDeleteEdit";
      this.buttonDeleteEdit.Size = new System.Drawing.Size(158, 23);
      this.buttonDeleteEdit.TabIndex = 7;
      this.buttonDeleteEdit.Text = "Delete Edits";
      this.buttonDeleteEdit.UseVisualStyleBackColor = true;
      this.buttonDeleteEdit.Click += new System.EventHandler(this.buttonDeleteEdit_Click);
      // 
      // buttonCancel
      // 
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.Location = new System.Drawing.Point(605, 466);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 6;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      // 
      // buttonCopy
      // 
      this.buttonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.buttonCopy.Location = new System.Drawing.Point(12, 437);
      this.buttonCopy.Name = "buttonCopy";
      this.buttonCopy.Size = new System.Drawing.Size(158, 23);
      this.buttonCopy.TabIndex = 5;
      this.buttonCopy.Text = "Copy Edits to next frame";
      this.buttonCopy.UseVisualStyleBackColor = true;
      this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
      // 
      // buttonUpload
      // 
      this.buttonUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonUpload.Location = new System.Drawing.Point(524, 466);
      this.buttonUpload.Name = "buttonUpload";
      this.buttonUpload.Size = new System.Drawing.Size(75, 23);
      this.buttonUpload.TabIndex = 4;
      this.buttonUpload.Text = "Upload";
      this.buttonUpload.UseVisualStyleBackColor = true;
      this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(520, 411);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(42, 13);
      this.label1.TabIndex = 11;
      this.label1.Text = "Quality:";
      // 
      // label2
      // 
      this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(525, 437);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(37, 13);
      this.label2.TabIndex = 11;
      this.label2.Text = "Scale:";
      // 
      // FormGifEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(692, 501);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonUpload);
      this.Controls.Add(this.buttonEdit);
      this.Controls.Add(this.trackScale);
      this.Controls.Add(this.comboQuality);
      this.Controls.Add(this.buttonRender);
      this.Controls.Add(this.buttonDelete);
      this.Controls.Add(this.buttonDeleteEdit);
      this.Controls.Add(this.hScrollBar1);
      this.Controls.Add(this.buttonCopy);
      this.Controls.Add(this.pictureBox1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MinimumSize = new System.Drawing.Size(165, 500);
      this.Name = "FormGifEditor";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Gif Editor - Frame 0/0";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.trackScale)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonDeleteEdit;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboQuality;
        private System.Windows.Forms.TrackBar trackScale;
        private System.Windows.Forms.Button buttonRender;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}