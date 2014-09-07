namespace ClipBoard
{
    partial class FormSettings
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
      this.comboDragKeys = new System.Windows.Forms.ComboBox();
      this.checkDragModShift = new System.Windows.Forms.CheckBox();
      this.checkDragModAlt = new System.Windows.Forms.CheckBox();
      this.checkDragModCtrl = new System.Windows.Forms.CheckBox();
      this.label8 = new System.Windows.Forms.Label();
      this.button2 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // comboDragKeys
      // 
      this.comboDragKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboDragKeys.FormattingEnabled = true;
      this.comboDragKeys.Location = new System.Drawing.Point(90, 35);
      this.comboDragKeys.Name = "comboDragKeys";
      this.comboDragKeys.Size = new System.Drawing.Size(136, 21);
      this.comboDragKeys.TabIndex = 68;
      // 
      // checkDragModShift
      // 
      this.checkDragModShift.AutoSize = true;
      this.checkDragModShift.Location = new System.Drawing.Point(181, 12);
      this.checkDragModShift.Name = "checkDragModShift";
      this.checkDragModShift.Size = new System.Drawing.Size(46, 17);
      this.checkDragModShift.TabIndex = 67;
      this.checkDragModShift.Text = "Shift";
      this.checkDragModShift.UseVisualStyleBackColor = true;
      // 
      // checkDragModAlt
      // 
      this.checkDragModAlt.AutoSize = true;
      this.checkDragModAlt.Location = new System.Drawing.Point(137, 12);
      this.checkDragModAlt.Name = "checkDragModAlt";
      this.checkDragModAlt.Size = new System.Drawing.Size(37, 17);
      this.checkDragModAlt.TabIndex = 66;
      this.checkDragModAlt.Text = "Alt";
      this.checkDragModAlt.UseVisualStyleBackColor = true;
      // 
      // checkDragModCtrl
      // 
      this.checkDragModCtrl.AutoSize = true;
      this.checkDragModCtrl.Location = new System.Drawing.Point(90, 12);
      this.checkDragModCtrl.Name = "checkDragModCtrl";
      this.checkDragModCtrl.Size = new System.Drawing.Size(40, 17);
      this.checkDragModCtrl.TabIndex = 65;
      this.checkDragModCtrl.Text = "Ctrl";
      this.checkDragModCtrl.UseVisualStyleBackColor = true;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(10, 12);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(74, 13);
      this.label8.TabIndex = 73;
      this.label8.Text = "Drag shortcut:";
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(236, 310);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 64;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(155, 310);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 63;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // FormSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(323, 345);
      this.Controls.Add(this.comboDragKeys);
      this.Controls.Add(this.checkDragModShift);
      this.Controls.Add(this.checkDragModAlt);
      this.Controls.Add(this.checkDragModCtrl);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormSettings";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "ClipBoard settings";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboDragKeys;
        private System.Windows.Forms.CheckBox checkDragModShift;
        private System.Windows.Forms.CheckBox checkDragModAlt;
        private System.Windows.Forms.CheckBox checkDragModCtrl;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}