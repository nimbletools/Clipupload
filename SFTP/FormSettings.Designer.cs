namespace SFTP
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
            if (disposing && (components != null))
            {
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
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.numLength = new System.Windows.Forms.NumericUpDown();
      this.checkShortMD5 = new System.Windows.Forms.CheckBox();
      this.checkUseMD5 = new System.Windows.Forms.CheckBox();
      this.textHttp = new System.Windows.Forms.TextBox();
      this.textPath = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.textPassword = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.textUsername = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.textServer = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.comboFormat = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.checkDragModCtrl = new System.Windows.Forms.CheckBox();
      this.checkDragModAlt = new System.Windows.Forms.CheckBox();
      this.checkDragModShift = new System.Windows.Forms.CheckBox();
      this.comboDragKeys = new System.Windows.Forms.ComboBox();
      this.comboPasteKeys = new System.Windows.Forms.ComboBox();
      this.checkPasteModShift = new System.Windows.Forms.CheckBox();
      this.checkPasteModAlt = new System.Windows.Forms.CheckBox();
      this.checkPasteModCtrl = new System.Windows.Forms.CheckBox();
      this.label9 = new System.Windows.Forms.Label();
      this.checkJpegCompression = new System.Windows.Forms.CheckBox();
      this.label10 = new System.Windows.Forms.Label();
      this.numJpegCompressionFilesize = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.button3 = new System.Windows.Forms.Button();
      this.numJpegCompressionRate = new System.Windows.Forms.NumericUpDown();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.numPort = new System.Windows.Forms.NumericUpDown();
      this.label12 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.checkAnimModCtrl = new System.Windows.Forms.CheckBox();
      this.checkAnimModAlt = new System.Windows.Forms.CheckBox();
      this.checkAnimModShift = new System.Windows.Forms.CheckBox();
      this.comboAnimKeys = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numJpegCompressionFilesize)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numJpegCompressionRate)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(396, 323);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 4;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button2.Location = new System.Drawing.Point(477, 323);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 5;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(20, 67);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(43, 13);
      this.label7.TabIndex = 23;
      this.label7.Text = "Length:";
      // 
      // numLength
      // 
      this.numLength.Location = new System.Drawing.Point(69, 65);
      this.numLength.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
      this.numLength.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.numLength.Name = "numLength";
      this.numLength.Size = new System.Drawing.Size(71, 20);
      this.numLength.TabIndex = 2;
      this.numLength.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
      // 
      // checkShortMD5
      // 
      this.checkShortMD5.AutoSize = true;
      this.checkShortMD5.Location = new System.Drawing.Point(69, 42);
      this.checkShortMD5.Name = "checkShortMD5";
      this.checkShortMD5.Size = new System.Drawing.Size(227, 17);
      this.checkShortMD5.TabIndex = 1;
      this.checkShortMD5.Text = "Use random string length as MD5 substring";
      this.checkShortMD5.UseVisualStyleBackColor = true;
      // 
      // checkUseMD5
      // 
      this.checkUseMD5.AutoSize = true;
      this.checkUseMD5.Location = new System.Drawing.Point(69, 19);
      this.checkUseMD5.Name = "checkUseMD5";
      this.checkUseMD5.Size = new System.Drawing.Size(185, 17);
      this.checkUseMD5.TabIndex = 0;
      this.checkUseMD5.Text = "Use MD5 instead of random string";
      this.checkUseMD5.UseVisualStyleBackColor = true;
      // 
      // textHttp
      // 
      this.textHttp.Location = new System.Drawing.Point(69, 163);
      this.textHttp.Name = "textHttp";
      this.textHttp.Size = new System.Drawing.Size(219, 20);
      this.textHttp.TabIndex = 6;
      // 
      // textPath
      // 
      this.textPath.Location = new System.Drawing.Point(69, 137);
      this.textPath.Name = "textPath";
      this.textPath.Size = new System.Drawing.Size(219, 20);
      this.textPath.TabIndex = 3;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(33, 166);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(30, 13);
      this.label6.TabIndex = 14;
      this.label6.Text = "Http:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(31, 140);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(32, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Path:";
      // 
      // textPassword
      // 
      this.textPassword.Location = new System.Drawing.Point(69, 111);
      this.textPassword.Name = "textPassword";
      this.textPassword.Size = new System.Drawing.Size(219, 20);
      this.textPassword.TabIndex = 2;
      this.textPassword.UseSystemPasswordChar = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(7, 114);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "Password:";
      // 
      // textUsername
      // 
      this.textUsername.Location = new System.Drawing.Point(69, 85);
      this.textUsername.Name = "textUsername";
      this.textUsername.Size = new System.Drawing.Size(219, 20);
      this.textUsername.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(5, 88);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(58, 13);
      this.label3.TabIndex = 13;
      this.label3.Text = "Username:";
      // 
      // textServer
      // 
      this.textServer.Location = new System.Drawing.Point(69, 13);
      this.textServer.Name = "textServer";
      this.textServer.Size = new System.Drawing.Size(219, 20);
      this.textServer.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(22, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(41, 13);
      this.label2.TabIndex = 12;
      this.label2.Text = "Server:";
      // 
      // comboFormat
      // 
      this.comboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboFormat.FormattingEnabled = true;
      this.comboFormat.Items.AddRange(new object[] {
            "PNG",
            "JPG",
            "GIF"});
      this.comboFormat.Location = new System.Drawing.Point(92, 19);
      this.comboFormat.Name = "comboFormat";
      this.comboFormat.Size = new System.Drawing.Size(136, 21);
      this.comboFormat.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(44, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(42, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Format:";
      // 
      // label8
      // 
      this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(12, 16);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(74, 13);
      this.label8.TabIndex = 25;
      this.label8.Text = "Drag shortcut:";
      // 
      // checkDragModCtrl
      // 
      this.checkDragModCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDragModCtrl.AutoSize = true;
      this.checkDragModCtrl.Location = new System.Drawing.Point(92, 16);
      this.checkDragModCtrl.Name = "checkDragModCtrl";
      this.checkDragModCtrl.Size = new System.Drawing.Size(40, 17);
      this.checkDragModCtrl.TabIndex = 0;
      this.checkDragModCtrl.Text = "Ctrl";
      this.checkDragModCtrl.UseVisualStyleBackColor = true;
      // 
      // checkDragModAlt
      // 
      this.checkDragModAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDragModAlt.AutoSize = true;
      this.checkDragModAlt.Location = new System.Drawing.Point(139, 16);
      this.checkDragModAlt.Name = "checkDragModAlt";
      this.checkDragModAlt.Size = new System.Drawing.Size(37, 17);
      this.checkDragModAlt.TabIndex = 1;
      this.checkDragModAlt.Text = "Alt";
      this.checkDragModAlt.UseVisualStyleBackColor = true;
      // 
      // checkDragModShift
      // 
      this.checkDragModShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkDragModShift.AutoSize = true;
      this.checkDragModShift.Location = new System.Drawing.Point(183, 16);
      this.checkDragModShift.Name = "checkDragModShift";
      this.checkDragModShift.Size = new System.Drawing.Size(46, 17);
      this.checkDragModShift.TabIndex = 2;
      this.checkDragModShift.Text = "Shift";
      this.checkDragModShift.UseVisualStyleBackColor = true;
      // 
      // comboDragKeys
      // 
      this.comboDragKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboDragKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboDragKeys.FormattingEnabled = true;
      this.comboDragKeys.Location = new System.Drawing.Point(92, 39);
      this.comboDragKeys.Name = "comboDragKeys";
      this.comboDragKeys.Size = new System.Drawing.Size(136, 21);
      this.comboDragKeys.TabIndex = 3;
      // 
      // comboPasteKeys
      // 
      this.comboPasteKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboPasteKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboPasteKeys.FormattingEnabled = true;
      this.comboPasteKeys.Location = new System.Drawing.Point(92, 139);
      this.comboPasteKeys.Name = "comboPasteKeys";
      this.comboPasteKeys.Size = new System.Drawing.Size(136, 21);
      this.comboPasteKeys.TabIndex = 7;
      // 
      // checkPasteModShift
      // 
      this.checkPasteModShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPasteModShift.AutoSize = true;
      this.checkPasteModShift.Location = new System.Drawing.Point(183, 116);
      this.checkPasteModShift.Name = "checkPasteModShift";
      this.checkPasteModShift.Size = new System.Drawing.Size(46, 17);
      this.checkPasteModShift.TabIndex = 6;
      this.checkPasteModShift.Text = "Shift";
      this.checkPasteModShift.UseVisualStyleBackColor = true;
      // 
      // checkPasteModAlt
      // 
      this.checkPasteModAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPasteModAlt.AutoSize = true;
      this.checkPasteModAlt.Location = new System.Drawing.Point(139, 116);
      this.checkPasteModAlt.Name = "checkPasteModAlt";
      this.checkPasteModAlt.Size = new System.Drawing.Size(37, 17);
      this.checkPasteModAlt.TabIndex = 5;
      this.checkPasteModAlt.Text = "Alt";
      this.checkPasteModAlt.UseVisualStyleBackColor = true;
      // 
      // checkPasteModCtrl
      // 
      this.checkPasteModCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkPasteModCtrl.AutoSize = true;
      this.checkPasteModCtrl.Location = new System.Drawing.Point(92, 116);
      this.checkPasteModCtrl.Name = "checkPasteModCtrl";
      this.checkPasteModCtrl.Size = new System.Drawing.Size(40, 17);
      this.checkPasteModCtrl.TabIndex = 4;
      this.checkPasteModCtrl.Text = "Ctrl";
      this.checkPasteModCtrl.UseVisualStyleBackColor = true;
      // 
      // label9
      // 
      this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(8, 116);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(78, 13);
      this.label9.TabIndex = 30;
      this.label9.Text = "Paste shortcut:";
      // 
      // checkJpegCompression
      // 
      this.checkJpegCompression.AutoSize = true;
      this.checkJpegCompression.Location = new System.Drawing.Point(7, 46);
      this.checkJpegCompression.Name = "checkJpegCompression";
      this.checkJpegCompression.Size = new System.Drawing.Size(158, 17);
      this.checkJpegCompression.TabIndex = 1;
      this.checkJpegCompression.Text = "Compress to Jpeg if required";
      this.checkJpegCompression.UseVisualStyleBackColor = true;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(42, 71);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(44, 13);
      this.label10.TabIndex = 32;
      this.label10.Text = "Filesize:";
      // 
      // numJpegCompressionFilesize
      // 
      this.numJpegCompressionFilesize.Location = new System.Drawing.Point(92, 69);
      this.numJpegCompressionFilesize.Maximum = new decimal(new int[] {
            1215752191,
            23,
            0,
            0});
      this.numJpegCompressionFilesize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numJpegCompressionFilesize.Name = "numJpegCompressionFilesize";
      this.numJpegCompressionFilesize.Size = new System.Drawing.Size(71, 20);
      this.numJpegCompressionFilesize.TabIndex = 2;
      this.numJpegCompressionFilesize.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(53, 97);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(33, 13);
      this.label11.TabIndex = 34;
      this.label11.Text = "Rate:";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button3);
      this.groupBox1.Controls.Add(this.numJpegCompressionRate);
      this.groupBox1.Controls.Add(this.checkJpegCompression);
      this.groupBox1.Controls.Add(this.label11);
      this.groupBox1.Controls.Add(this.comboFormat);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.label10);
      this.groupBox1.Controls.Add(this.numJpegCompressionFilesize);
      this.groupBox1.Location = new System.Drawing.Point(317, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(235, 127);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Image format";
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(197, 46);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(31, 23);
      this.button3.TabIndex = 36;
      this.button3.Text = "?";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // numJpegCompressionRate
      // 
      this.numJpegCompressionRate.Location = new System.Drawing.Point(92, 95);
      this.numJpegCompressionRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numJpegCompressionRate.Name = "numJpegCompressionRate";
      this.numJpegCompressionRate.Size = new System.Drawing.Size(71, 20);
      this.numJpegCompressionRate.TabIndex = 3;
      this.numJpegCompressionRate.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label13);
      this.groupBox2.Controls.Add(this.checkAnimModCtrl);
      this.groupBox2.Controls.Add(this.checkAnimModAlt);
      this.groupBox2.Controls.Add(this.checkAnimModShift);
      this.groupBox2.Controls.Add(this.comboAnimKeys);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.checkDragModCtrl);
      this.groupBox2.Controls.Add(this.comboPasteKeys);
      this.groupBox2.Controls.Add(this.checkDragModAlt);
      this.groupBox2.Controls.Add(this.checkPasteModShift);
      this.groupBox2.Controls.Add(this.checkDragModShift);
      this.groupBox2.Controls.Add(this.checkPasteModAlt);
      this.groupBox2.Controls.Add(this.comboDragKeys);
      this.groupBox2.Controls.Add(this.checkPasteModCtrl);
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Location = new System.Drawing.Point(317, 145);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(235, 166);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Shortcuts";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.checkUseMD5);
      this.groupBox3.Controls.Add(this.checkShortMD5);
      this.groupBox3.Controls.Add(this.numLength);
      this.groupBox3.Controls.Add(this.label7);
      this.groupBox3.Location = new System.Drawing.Point(12, 214);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(299, 97);
      this.groupBox3.TabIndex = 1;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Filename";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.numPort);
      this.groupBox4.Controls.Add(this.label12);
      this.groupBox4.Controls.Add(this.label2);
      this.groupBox4.Controls.Add(this.textServer);
      this.groupBox4.Controls.Add(this.label3);
      this.groupBox4.Controls.Add(this.textUsername);
      this.groupBox4.Controls.Add(this.label4);
      this.groupBox4.Controls.Add(this.textPassword);
      this.groupBox4.Controls.Add(this.textHttp);
      this.groupBox4.Controls.Add(this.label5);
      this.groupBox4.Controls.Add(this.textPath);
      this.groupBox4.Controls.Add(this.label6);
      this.groupBox4.Location = new System.Drawing.Point(12, 12);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(299, 196);
      this.groupBox4.TabIndex = 0;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Server";
      // 
      // numPort
      // 
      this.numPort.Location = new System.Drawing.Point(69, 39);
      this.numPort.Name = "numPort";
      this.numPort.Size = new System.Drawing.Size(105, 20);
      this.numPort.TabIndex = 16;
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(34, 41);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(29, 13);
      this.label12.TabIndex = 15;
      this.label12.Text = "Port:";
      // 
      // label13
      // 
      this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(12, 67);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(74, 13);
      this.label13.TabIndex = 35;
      this.label13.Text = "Anim shortcut:";
      // 
      // checkAnimModCtrl
      // 
      this.checkAnimModCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnimModCtrl.AutoSize = true;
      this.checkAnimModCtrl.Location = new System.Drawing.Point(92, 66);
      this.checkAnimModCtrl.Name = "checkAnimModCtrl";
      this.checkAnimModCtrl.Size = new System.Drawing.Size(40, 17);
      this.checkAnimModCtrl.TabIndex = 31;
      this.checkAnimModCtrl.Text = "Ctrl";
      this.checkAnimModCtrl.UseVisualStyleBackColor = true;
      // 
      // checkAnimModAlt
      // 
      this.checkAnimModAlt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnimModAlt.AutoSize = true;
      this.checkAnimModAlt.Location = new System.Drawing.Point(139, 66);
      this.checkAnimModAlt.Name = "checkAnimModAlt";
      this.checkAnimModAlt.Size = new System.Drawing.Size(37, 17);
      this.checkAnimModAlt.TabIndex = 32;
      this.checkAnimModAlt.Text = "Alt";
      this.checkAnimModAlt.UseVisualStyleBackColor = true;
      // 
      // checkAnimModShift
      // 
      this.checkAnimModShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkAnimModShift.AutoSize = true;
      this.checkAnimModShift.Location = new System.Drawing.Point(183, 66);
      this.checkAnimModShift.Name = "checkAnimModShift";
      this.checkAnimModShift.Size = new System.Drawing.Size(46, 17);
      this.checkAnimModShift.TabIndex = 33;
      this.checkAnimModShift.Text = "Shift";
      this.checkAnimModShift.UseVisualStyleBackColor = true;
      // 
      // comboAnimKeys
      // 
      this.comboAnimKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.comboAnimKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboAnimKeys.FormattingEnabled = true;
      this.comboAnimKeys.Location = new System.Drawing.Point(92, 89);
      this.comboAnimKeys.Name = "comboAnimKeys";
      this.comboAnimKeys.Size = new System.Drawing.Size(136, 21);
      this.comboAnimKeys.TabIndex = 34;
      // 
      // FormSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(564, 357);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.Name = "FormSettings";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "SFTP Settings";
      ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numJpegCompressionFilesize)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numJpegCompressionRate)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.CheckBox checkShortMD5;
        private System.Windows.Forms.CheckBox checkUseMD5;
        private System.Windows.Forms.TextBox textHttp;
        private System.Windows.Forms.TextBox textPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textUsername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkDragModCtrl;
        private System.Windows.Forms.CheckBox checkDragModAlt;
        private System.Windows.Forms.CheckBox checkDragModShift;
        private System.Windows.Forms.ComboBox comboDragKeys;
        private System.Windows.Forms.ComboBox comboPasteKeys;
        private System.Windows.Forms.CheckBox checkPasteModShift;
        private System.Windows.Forms.CheckBox checkPasteModAlt;
        private System.Windows.Forms.CheckBox checkPasteModCtrl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkJpegCompression;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numJpegCompressionFilesize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numJpegCompressionRate;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkAnimModCtrl;
        private System.Windows.Forms.CheckBox checkAnimModAlt;
        private System.Windows.Forms.CheckBox checkAnimModShift;
        private System.Windows.Forms.ComboBox comboAnimKeys;
    }
}