namespace AG_Interface
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.OpenAGPrinthead = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.IPCStatus = new System.Windows.Forms.TextBox();
            this.PDFConvert = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Bufferdata = new System.Windows.Forms.PictureBox();
            this.ResetBuffers = new System.Windows.Forms.Button();
            this.SetContrastBtn = new System.Windows.Forms.Button();
            this.ContrastValueUpDown = new System.Windows.Forms.NumericUpDown();
            this.TriggerGenerator0 = new System.Windows.Forms.Button();
            this.TriggerGenerator1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bufferdata)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastValueUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenAGPrinthead
            // 
            this.OpenAGPrinthead.Location = new System.Drawing.Point(12, 14);
            this.OpenAGPrinthead.Name = "OpenAGPrinthead";
            this.OpenAGPrinthead.Size = new System.Drawing.Size(115, 23);
            this.OpenAGPrinthead.TabIndex = 0;
            this.OpenAGPrinthead.Text = "Open AGPrinthead";
            this.OpenAGPrinthead.UseVisualStyleBackColor = true;
            this.OpenAGPrinthead.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(261, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(342, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(423, 15);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "System ready?";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // listBox1
            // 
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 83);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(628, 314);
            this.listBox1.TabIndex = 6;
            // 
            // IPCStatus
            // 
            this.IPCStatus.Location = new System.Drawing.Point(138, 16);
            this.IPCStatus.Name = "IPCStatus";
            this.IPCStatus.Size = new System.Drawing.Size(115, 20);
            this.IPCStatus.TabIndex = 8;
            this.IPCStatus.Text = "IPCStatus";
            // 
            // PDFConvert
            // 
            this.PDFConvert.Location = new System.Drawing.Point(659, 15);
            this.PDFConvert.Name = "PDFConvert";
            this.PDFConvert.Size = new System.Drawing.Size(202, 23);
            this.PDFConvert.TabIndex = 9;
            this.PDFConvert.Text = "PDF Converter Test";
            this.PDFConvert.UseVisualStyleBackColor = true;
            this.PDFConvert.Click += new System.EventHandler(this.PDFConvert_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(867, 17);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(578, 20);
            this.lblVersion.TabIndex = 10;
            this.lblVersion.Text = "PDF Converter version";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(659, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(786, 518);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Bufferdata
            // 
            this.Bufferdata.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Bufferdata.Location = new System.Drawing.Point(12, 408);
            this.Bufferdata.Name = "Bufferdata";
            this.Bufferdata.Size = new System.Drawing.Size(628, 154);
            this.Bufferdata.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Bufferdata.TabIndex = 12;
            this.Bufferdata.TabStop = false;
            // 
            // ResetBuffers
            // 
            this.ResetBuffers.Location = new System.Drawing.Point(522, 15);
            this.ResetBuffers.Name = "ResetBuffers";
            this.ResetBuffers.Size = new System.Drawing.Size(118, 23);
            this.ResetBuffers.TabIndex = 15;
            this.ResetBuffers.Text = "ResetBuffers";
            this.ResetBuffers.UseVisualStyleBackColor = true;
            this.ResetBuffers.Click += new System.EventHandler(this.ResetBuffers_Click);
            // 
            // SetContrastBtn
            // 
            this.SetContrastBtn.Location = new System.Drawing.Point(12, 44);
            this.SetContrastBtn.Name = "SetContrastBtn";
            this.SetContrastBtn.Size = new System.Drawing.Size(75, 23);
            this.SetContrastBtn.TabIndex = 16;
            this.SetContrastBtn.Text = "Set Contrast";
            this.SetContrastBtn.UseVisualStyleBackColor = true;
            this.SetContrastBtn.Click += new System.EventHandler(this.SetContrastBtn_Click);
            // 
            // ContrastValueUpDown
            // 
            this.ContrastValueUpDown.Location = new System.Drawing.Point(93, 47);
            this.ContrastValueUpDown.Maximum = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.ContrastValueUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            this.ContrastValueUpDown.Name = "ContrastValueUpDown";
            this.ContrastValueUpDown.Size = new System.Drawing.Size(45, 20);
            this.ContrastValueUpDown.TabIndex = 17;
            this.ContrastValueUpDown.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.ContrastValueUpDown.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // TriggerGenerator0
            // 
            this.TriggerGenerator0.Location = new System.Drawing.Point(144, 47);
            this.TriggerGenerator0.Name = "TriggerGenerator0";
            this.TriggerGenerator0.Size = new System.Drawing.Size(124, 23);
            this.TriggerGenerator0.TabIndex = 18;
            this.TriggerGenerator0.Text = "Trigger Generator 0";
            this.TriggerGenerator0.UseVisualStyleBackColor = true;
            this.TriggerGenerator0.Click += new System.EventHandler(this.TriggerGenerator0_Click);
            // 
            // TriggerGenerator1
            // 
            this.TriggerGenerator1.Location = new System.Drawing.Point(274, 47);
            this.TriggerGenerator1.Name = "TriggerGenerator1";
            this.TriggerGenerator1.Size = new System.Drawing.Size(110, 23);
            this.TriggerGenerator1.TabIndex = 19;
            this.TriggerGenerator1.Text = "Trigger Generator 1";
            this.TriggerGenerator1.UseVisualStyleBackColor = true;
            this.TriggerGenerator1.Click += new System.EventHandler(this.TriggerGenerator1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1457, 574);
            this.Controls.Add(this.TriggerGenerator1);
            this.Controls.Add(this.TriggerGenerator0);
            this.Controls.Add(this.ContrastValueUpDown);
            this.Controls.Add(this.SetContrastBtn);
            this.Controls.Add(this.ResetBuffers);
            this.Controls.Add(this.Bufferdata);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.PDFConvert);
            this.Controls.Add(this.IPCStatus);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.OpenAGPrinthead);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "InkJet Status";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Bufferdata)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ContrastValueUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenAGPrinthead;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox IPCStatus;
        private System.Windows.Forms.Button PDFConvert;
        private System.Windows.Forms.TextBox lblVersion;
        internal System.Windows.Forms.PictureBox pictureBox1;
        protected System.Windows.Forms.PictureBox Bufferdata;
        private System.Windows.Forms.Button ResetBuffers;
        private System.Windows.Forms.Button SetContrastBtn;
        private System.Windows.Forms.NumericUpDown ContrastValueUpDown;
        private System.Windows.Forms.Button TriggerGenerator0;
        private System.Windows.Forms.Button TriggerGenerator1;
    }
}

