namespace Ttp.Meteor.MeteorMonoPrint
{
    partial class FormMeteorMonoPrint
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMeteorMonoPrint));
            this.buttonLoadImage = new System.Windows.Forms.Button();
            this.buttonStartPrint = new System.Windows.Forms.Button();
            this.buttonStopPrint = new System.Windows.Forms.Button();
            this.pictureBoxPrintData = new System.Windows.Forms.PictureBox();
            this.numericUpDownFrequency = new System.Windows.Forms.NumericUpDown();
            this.labelkHz = new System.Windows.Forms.Label();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.labelCopies = new System.Windows.Forms.Label();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.IPCStatus = new System.Windows.Forms.TextBox();
            this.statusStripImage = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusImageDimensions = new System.Windows.Forms.ToolStripStatusLabel();
            this.numericUpDownCopies = new System.Windows.Forms.NumericUpDown();
            this.checkBoxHeadTemperatureControl = new System.Windows.Forms.CheckBox();
            this.checkBoxAuxTemperatureControl = new System.Windows.Forms.CheckBox();
            this.groupBoxTemperatures = new System.Windows.Forms.GroupBox();
            this.numericUpDownAuxTemperatureSetPoint = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownHeadTemperatureSetPoint = new System.Windows.Forms.NumericUpDown();
            this.groupBoxStatus = new System.Windows.Forms.GroupBox();
            this.groupBoxControl = new System.Windows.Forms.GroupBox();
            this.labelYTop = new System.Windows.Forms.Label();
            this.checkBoxEnableHeadPower = new System.Windows.Forms.CheckBox();
            this.numericUpDownYTop = new System.Windows.Forms.NumericUpDown();
            this.groupBoxSetup = new System.Windows.Forms.GroupBox();
            this.radioButtonDiscrete = new System.Windows.Forms.RadioButton();
            this.radioButtonSeamless = new System.Windows.Forms.RadioButton();
            this.groupBoxResolution = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelBpp = new System.Windows.Forms.FlowLayoutPanel();
            this.radioButton1bpp = new System.Windows.Forms.RadioButton();
            this.radioButton2bpp = new System.Windows.Forms.RadioButton();
            this.radioButton4bpp = new System.Windows.Forms.RadioButton();
            this.timerMeteorStatus = new System.Windows.Forms.Timer(this.components);
            this.openFileDialogLoadImage = new System.Windows.Forms.OpenFileDialog();
            this.groupBoxPrintClock = new System.Windows.Forms.GroupBox();
            this.radioButtonInternalEncoder = new System.Windows.Forms.RadioButton();
            this.radioButtonExternalEncoder = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).BeginInit();
            this.groupBoxPreview.SuspendLayout();
            this.statusStripImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).BeginInit();
            this.groupBoxTemperatures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAuxTemperatureSetPoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeadTemperatureSetPoint)).BeginInit();
            this.groupBoxStatus.SuspendLayout();
            this.groupBoxControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).BeginInit();
            this.groupBoxSetup.SuspendLayout();
            this.groupBoxResolution.SuspendLayout();
            this.flowLayoutPanelBpp.SuspendLayout();
            this.groupBoxPrintClock.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLoadImage
            // 
            this.buttonLoadImage.Location = new System.Drawing.Point(43, 43);
            this.buttonLoadImage.Name = "buttonLoadImage";
            this.buttonLoadImage.Size = new System.Drawing.Size(122, 23);
            this.buttonLoadImage.TabIndex = 1;
            this.buttonLoadImage.Text = "Load Image";
            this.buttonLoadImage.UseVisualStyleBackColor = true;
            this.buttonLoadImage.Click += new System.EventHandler(this.buttonLoadImage_Click);
            // 
            // buttonStartPrint
            // 
            this.buttonStartPrint.Enabled = false;
            this.buttonStartPrint.Location = new System.Drawing.Point(42, 96);
            this.buttonStartPrint.Name = "buttonStartPrint";
            this.buttonStartPrint.Size = new System.Drawing.Size(121, 23);
            this.buttonStartPrint.TabIndex = 3;
            this.buttonStartPrint.Text = "Start Print";
            this.buttonStartPrint.UseVisualStyleBackColor = true;
            this.buttonStartPrint.Click += new System.EventHandler(this.buttonStartPrint_Click);
            // 
            // buttonStopPrint
            // 
            this.buttonStopPrint.Enabled = false;
            this.buttonStopPrint.Location = new System.Drawing.Point(43, 124);
            this.buttonStopPrint.Name = "buttonStopPrint";
            this.buttonStopPrint.Size = new System.Drawing.Size(120, 23);
            this.buttonStopPrint.TabIndex = 4;
            this.buttonStopPrint.Text = "Stop Print";
            this.buttonStopPrint.UseVisualStyleBackColor = true;
            this.buttonStopPrint.Click += new System.EventHandler(this.buttonStopPrint_Click);
            // 
            // pictureBoxPrintData
            // 
            this.pictureBoxPrintData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPrintData.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxPrintData.Location = new System.Drawing.Point(222, 24);
            this.pictureBoxPrintData.Name = "pictureBoxPrintData";
            this.pictureBoxPrintData.Size = new System.Drawing.Size(521, 513);
            this.pictureBoxPrintData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPrintData.TabIndex = 1;
            this.pictureBoxPrintData.TabStop = false;
            // 
            // numericUpDownFrequency
            // 
            this.numericUpDownFrequency.DecimalPlaces = 1;
            this.numericUpDownFrequency.Location = new System.Drawing.Point(121, 42);
            this.numericUpDownFrequency.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownFrequency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDownFrequency.Name = "numericUpDownFrequency";
            this.numericUpDownFrequency.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownFrequency.TabIndex = 2;
            this.numericUpDownFrequency.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelkHz
            // 
            this.labelkHz.AutoSize = true;
            this.labelkHz.Location = new System.Drawing.Point(171, 44);
            this.labelkHz.Name = "labelkHz";
            this.labelkHz.Size = new System.Drawing.Size(26, 13);
            this.labelkHz.TabIndex = 4;
            this.labelkHz.Text = "kHz";
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Location = new System.Drawing.Point(7, 19);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.ReadOnly = true;
            this.textBoxStatus.Size = new System.Drawing.Size(185, 20);
            this.textBoxStatus.TabIndex = 1;
            this.textBoxStatus.TabStop = false;
            this.textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelCopies
            // 
            this.labelCopies.AutoSize = true;
            this.labelCopies.Location = new System.Drawing.Point(22, 46);
            this.labelCopies.Name = "labelCopies";
            this.labelCopies.Size = new System.Drawing.Size(39, 13);
            this.labelCopies.TabIndex = 7;
            this.labelCopies.Text = "Copies";
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPreview.Controls.Add(this.statusStripImage);
            this.groupBoxPreview.Controls.Add(this.pictureBoxPrintData);
            this.groupBoxPreview.Location = new System.Drawing.Point(217, 3);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Size = new System.Drawing.Size(772, 574);
            this.groupBoxPreview.TabIndex = 8;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Preview:";
            // 
            // IPCStatus
            // 
            this.IPCStatus.Location = new System.Drawing.Point(11, 45);
            this.IPCStatus.Name = "IPCStatus";
            this.IPCStatus.Size = new System.Drawing.Size(100, 20);
            this.IPCStatus.TabIndex = 3;
            // 
            // statusStripImage
            // 
            this.statusStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFilename,
            this.toolStripStatusImageDimensions});
            this.statusStripImage.Location = new System.Drawing.Point(3, 549);
            this.statusStripImage.Name = "statusStripImage";
            this.statusStripImage.ShowItemToolTips = true;
            this.statusStripImage.Size = new System.Drawing.Size(766, 22);
            this.statusStripImage.SizingGrip = false;
            this.statusStripImage.TabIndex = 2;
            // 
            // toolStripStatusFilename
            // 
            this.toolStripStatusFilename.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
            this.toolStripStatusFilename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusFilename.Name = "toolStripStatusFilename";
            this.toolStripStatusFilename.Size = new System.Drawing.Size(751, 17);
            this.toolStripStatusFilename.Spring = true;
            this.toolStripStatusFilename.Text = "No image loaded";
            this.toolStripStatusFilename.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusImageDimensions
            // 
            this.toolStripStatusImageDimensions.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
            this.toolStripStatusImageDimensions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusImageDimensions.Name = "toolStripStatusImageDimensions";
            this.toolStripStatusImageDimensions.Size = new System.Drawing.Size(0, 17);
            this.toolStripStatusImageDimensions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownCopies
            // 
            this.numericUpDownCopies.Location = new System.Drawing.Point(67, 44);
            this.numericUpDownCopies.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownCopies.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCopies.Name = "numericUpDownCopies";
            this.numericUpDownCopies.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownCopies.TabIndex = 2;
            this.numericUpDownCopies.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // checkBoxHeadTemperatureControl
            // 
            this.checkBoxHeadTemperatureControl.AutoSize = true;
            this.checkBoxHeadTemperatureControl.Location = new System.Drawing.Point(47, 22);
            this.checkBoxHeadTemperatureControl.Name = "checkBoxHeadTemperatureControl";
            this.checkBoxHeadTemperatureControl.Size = new System.Drawing.Size(52, 17);
            this.checkBoxHeadTemperatureControl.TabIndex = 0;
            this.checkBoxHeadTemperatureControl.Text = "Head";
            this.checkBoxHeadTemperatureControl.UseVisualStyleBackColor = true;
            this.checkBoxHeadTemperatureControl.CheckedChanged += new System.EventHandler(this.checkBoxHeadTemperatureControl_CheckedChanged);
            // 
            // checkBoxAuxTemperatureControl
            // 
            this.checkBoxAuxTemperatureControl.AutoSize = true;
            this.checkBoxAuxTemperatureControl.Location = new System.Drawing.Point(47, 46);
            this.checkBoxAuxTemperatureControl.Name = "checkBoxAuxTemperatureControl";
            this.checkBoxAuxTemperatureControl.Size = new System.Drawing.Size(64, 17);
            this.checkBoxAuxTemperatureControl.TabIndex = 2;
            this.checkBoxAuxTemperatureControl.Text = "Auxiliary";
            this.checkBoxAuxTemperatureControl.UseVisualStyleBackColor = true;
            this.checkBoxAuxTemperatureControl.CheckedChanged += new System.EventHandler(this.checkBoxAuxTemperatureControl_CheckedChanged);
            // 
            // groupBoxTemperatures
            // 
            this.groupBoxTemperatures.Controls.Add(this.checkBoxHeadTemperatureControl);
            this.groupBoxTemperatures.Controls.Add(this.checkBoxAuxTemperatureControl);
            this.groupBoxTemperatures.Controls.Add(this.numericUpDownAuxTemperatureSetPoint);
            this.groupBoxTemperatures.Controls.Add(this.numericUpDownHeadTemperatureSetPoint);
            this.groupBoxTemperatures.Location = new System.Drawing.Point(5, 442);
            this.groupBoxTemperatures.Name = "groupBoxTemperatures";
            this.groupBoxTemperatures.Size = new System.Drawing.Size(206, 76);
            this.groupBoxTemperatures.TabIndex = 6;
            this.groupBoxTemperatures.TabStop = false;
            this.groupBoxTemperatures.Text = "Temperature Setpoints (°C)";
            // 
            // numericUpDownAuxTemperatureSetPoint
            // 
            this.numericUpDownAuxTemperatureSetPoint.Enabled = false;
            this.numericUpDownAuxTemperatureSetPoint.Location = new System.Drawing.Point(112, 45);
            this.numericUpDownAuxTemperatureSetPoint.Name = "numericUpDownAuxTemperatureSetPoint";
            this.numericUpDownAuxTemperatureSetPoint.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownAuxTemperatureSetPoint.TabIndex = 3;
            this.numericUpDownAuxTemperatureSetPoint.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // numericUpDownHeadTemperatureSetPoint
            // 
            this.numericUpDownHeadTemperatureSetPoint.Enabled = false;
            this.numericUpDownHeadTemperatureSetPoint.Location = new System.Drawing.Point(112, 21);
            this.numericUpDownHeadTemperatureSetPoint.Name = "numericUpDownHeadTemperatureSetPoint";
            this.numericUpDownHeadTemperatureSetPoint.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownHeadTemperatureSetPoint.TabIndex = 1;
            this.numericUpDownHeadTemperatureSetPoint.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // groupBoxStatus
            // 
            this.groupBoxStatus.Controls.Add(this.IPCStatus);
            this.groupBoxStatus.Controls.Add(this.textBoxStatus);
            this.groupBoxStatus.Location = new System.Drawing.Point(5, 3);
            this.groupBoxStatus.Name = "groupBoxStatus";
            this.groupBoxStatus.Size = new System.Drawing.Size(206, 73);
            this.groupBoxStatus.TabIndex = 1;
            this.groupBoxStatus.TabStop = false;
            this.groupBoxStatus.Text = "Status:";
            // 
            // groupBoxControl
            // 
            this.groupBoxControl.Controls.Add(this.labelYTop);
            this.groupBoxControl.Controls.Add(this.checkBoxEnableHeadPower);
            this.groupBoxControl.Controls.Add(this.numericUpDownYTop);
            this.groupBoxControl.Controls.Add(this.buttonLoadImage);
            this.groupBoxControl.Controls.Add(this.buttonStartPrint);
            this.groupBoxControl.Controls.Add(this.buttonStopPrint);
            this.groupBoxControl.Location = new System.Drawing.Point(5, 82);
            this.groupBoxControl.Name = "groupBoxControl";
            this.groupBoxControl.Size = new System.Drawing.Size(206, 158);
            this.groupBoxControl.TabIndex = 2;
            this.groupBoxControl.TabStop = false;
            this.groupBoxControl.Text = "Print Control";
            // 
            // labelYTop
            // 
            this.labelYTop.AutoSize = true;
            this.labelYTop.Location = new System.Drawing.Point(42, 73);
            this.labelYTop.Name = "labelYTop";
            this.labelYTop.Size = new System.Drawing.Size(53, 13);
            this.labelYTop.TabIndex = 6;
            this.labelYTop.Text = "Y position";
            // 
            // checkBoxEnableHeadPower
            // 
            this.checkBoxEnableHeadPower.AutoSize = true;
            this.checkBoxEnableHeadPower.Location = new System.Drawing.Point(43, 21);
            this.checkBoxEnableHeadPower.Name = "checkBoxEnableHeadPower";
            this.checkBoxEnableHeadPower.Size = new System.Drawing.Size(121, 17);
            this.checkBoxEnableHeadPower.TabIndex = 0;
            this.checkBoxEnableHeadPower.Text = "Enable Head Power";
            this.checkBoxEnableHeadPower.UseVisualStyleBackColor = true;
            this.checkBoxEnableHeadPower.CheckedChanged += new System.EventHandler(this.checkBoxEnableHeadPower_CheckedChanged);
            // 
            // numericUpDownYTop
            // 
            this.numericUpDownYTop.Location = new System.Drawing.Point(98, 71);
            this.numericUpDownYTop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownYTop.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownYTop.Name = "numericUpDownYTop";
            this.numericUpDownYTop.Size = new System.Drawing.Size(67, 20);
            this.numericUpDownYTop.TabIndex = 2;
            // 
            // groupBoxSetup
            // 
            this.groupBoxSetup.Controls.Add(this.radioButtonDiscrete);
            this.groupBoxSetup.Controls.Add(this.radioButtonSeamless);
            this.groupBoxSetup.Controls.Add(this.numericUpDownCopies);
            this.groupBoxSetup.Controls.Add(this.labelCopies);
            this.groupBoxSetup.Location = new System.Drawing.Point(5, 246);
            this.groupBoxSetup.Name = "groupBoxSetup";
            this.groupBoxSetup.Size = new System.Drawing.Size(206, 72);
            this.groupBoxSetup.TabIndex = 3;
            this.groupBoxSetup.TabStop = false;
            this.groupBoxSetup.Text = "Preload Data Path Repeats";
            // 
            // radioButtonDiscrete
            // 
            this.radioButtonDiscrete.AutoSize = true;
            this.radioButtonDiscrete.Location = new System.Drawing.Point(114, 21);
            this.radioButtonDiscrete.Name = "radioButtonDiscrete";
            this.radioButtonDiscrete.Size = new System.Drawing.Size(64, 17);
            this.radioButtonDiscrete.TabIndex = 1;
            this.radioButtonDiscrete.TabStop = true;
            this.radioButtonDiscrete.Text = "Discrete";
            this.radioButtonDiscrete.UseVisualStyleBackColor = true;
            // 
            // radioButtonSeamless
            // 
            this.radioButtonSeamless.AutoSize = true;
            this.radioButtonSeamless.Checked = true;
            this.radioButtonSeamless.Location = new System.Drawing.Point(21, 21);
            this.radioButtonSeamless.Name = "radioButtonSeamless";
            this.radioButtonSeamless.Size = new System.Drawing.Size(70, 17);
            this.radioButtonSeamless.TabIndex = 1;
            this.radioButtonSeamless.TabStop = true;
            this.radioButtonSeamless.Text = "Seamless";
            this.radioButtonSeamless.UseVisualStyleBackColor = true;
            // 
            // groupBoxResolution
            // 
            this.groupBoxResolution.Controls.Add(this.flowLayoutPanelBpp);
            this.groupBoxResolution.Location = new System.Drawing.Point(5, 384);
            this.groupBoxResolution.Name = "groupBoxResolution";
            this.groupBoxResolution.Size = new System.Drawing.Size(206, 51);
            this.groupBoxResolution.TabIndex = 5;
            this.groupBoxResolution.TabStop = false;
            this.groupBoxResolution.Text = "Resolution";
            // 
            // flowLayoutPanelBpp
            // 
            this.flowLayoutPanelBpp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanelBpp.AutoSize = true;
            this.flowLayoutPanelBpp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton1bpp);
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton2bpp);
            this.flowLayoutPanelBpp.Controls.Add(this.radioButton4bpp);
            this.flowLayoutPanelBpp.Location = new System.Drawing.Point(18, 16);
            this.flowLayoutPanelBpp.Name = "flowLayoutPanelBpp";
            this.flowLayoutPanelBpp.Size = new System.Drawing.Size(165, 23);
            this.flowLayoutPanelBpp.TabIndex = 1;
            // 
            // radioButton1bpp
            // 
            this.radioButton1bpp.AutoSize = true;
            this.radioButton1bpp.Checked = true;
            this.radioButton1bpp.Location = new System.Drawing.Point(3, 3);
            this.radioButton1bpp.Name = "radioButton1bpp";
            this.radioButton1bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton1bpp.TabIndex = 0;
            this.radioButton1bpp.TabStop = true;
            this.radioButton1bpp.Text = "1bpp";
            this.radioButton1bpp.UseVisualStyleBackColor = true;
            // 
            // radioButton2bpp
            // 
            this.radioButton2bpp.AutoSize = true;
            this.radioButton2bpp.Location = new System.Drawing.Point(58, 3);
            this.radioButton2bpp.Name = "radioButton2bpp";
            this.radioButton2bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton2bpp.TabIndex = 0;
            this.radioButton2bpp.Text = "2bpp";
            this.radioButton2bpp.UseVisualStyleBackColor = true;
            // 
            // radioButton4bpp
            // 
            this.radioButton4bpp.AutoSize = true;
            this.radioButton4bpp.Location = new System.Drawing.Point(113, 3);
            this.radioButton4bpp.Name = "radioButton4bpp";
            this.radioButton4bpp.Size = new System.Drawing.Size(49, 17);
            this.radioButton4bpp.TabIndex = 0;
            this.radioButton4bpp.Text = "4bpp";
            this.radioButton4bpp.UseVisualStyleBackColor = true;
            // 
            // timerMeteorStatus
            // 
            this.timerMeteorStatus.Enabled = true;
            this.timerMeteorStatus.Interval = 250;
            this.timerMeteorStatus.Tick += new System.EventHandler(this.timerMeteorStatus_Tick);
            // 
            // openFileDialogLoadImage
            // 
            this.openFileDialogLoadImage.DefaultExt = "bmp";
            this.openFileDialogLoadImage.Filter = "Bitmap files|*.bmp|JPEG files|*.jpg|TIFF files|*.tif";
            this.openFileDialogLoadImage.Title = "Please select the print image ...";
            // 
            // groupBoxPrintClock
            // 
            this.groupBoxPrintClock.Controls.Add(this.numericUpDownFrequency);
            this.groupBoxPrintClock.Controls.Add(this.radioButtonInternalEncoder);
            this.groupBoxPrintClock.Controls.Add(this.radioButtonExternalEncoder);
            this.groupBoxPrintClock.Controls.Add(this.labelkHz);
            this.groupBoxPrintClock.Location = new System.Drawing.Point(5, 305);
            this.groupBoxPrintClock.Name = "groupBoxPrintClock";
            this.groupBoxPrintClock.Size = new System.Drawing.Size(206, 72);
            this.groupBoxPrintClock.TabIndex = 4;
            this.groupBoxPrintClock.TabStop = false;
            this.groupBoxPrintClock.Text = "Print Clock";
            // 
            // radioButtonInternalEncoder
            // 
            this.radioButtonInternalEncoder.AutoSize = true;
            this.radioButtonInternalEncoder.Location = new System.Drawing.Point(21, 42);
            this.radioButtonInternalEncoder.Name = "radioButtonInternalEncoder";
            this.radioButtonInternalEncoder.Size = new System.Drawing.Size(102, 17);
            this.radioButtonInternalEncoder.TabIndex = 1;
            this.radioButtonInternalEncoder.TabStop = true;
            this.radioButtonInternalEncoder.Text = "Internal encoder";
            this.radioButtonInternalEncoder.UseVisualStyleBackColor = true;
            // 
            // radioButtonExternalEncoder
            // 
            this.radioButtonExternalEncoder.AutoSize = true;
            this.radioButtonExternalEncoder.Location = new System.Drawing.Point(21, 19);
            this.radioButtonExternalEncoder.Name = "radioButtonExternalEncoder";
            this.radioButtonExternalEncoder.Size = new System.Drawing.Size(105, 17);
            this.radioButtonExternalEncoder.TabIndex = 1;
            this.radioButtonExternalEncoder.TabStop = true;
            this.radioButtonExternalEncoder.Text = "External encoder";
            this.radioButtonExternalEncoder.UseVisualStyleBackColor = true;
            // 
            // FormMeteorMonoPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1001, 580);
            this.Controls.Add(this.groupBoxPrintClock);
            this.Controls.Add(this.groupBoxSetup);
            this.Controls.Add(this.groupBoxControl);
            this.Controls.Add(this.groupBoxResolution);
            this.Controls.Add(this.groupBoxStatus);
            this.Controls.Add(this.groupBoxTemperatures);
            this.Controls.Add(this.groupBoxPreview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(677, 559);
            this.Name = "FormMeteorMonoPrint";
            this.Text = "MeteorMonoPrint";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMeteorMonoPrint_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPrintData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrequency)).EndInit();
            this.groupBoxPreview.ResumeLayout(false);
            this.groupBoxPreview.PerformLayout();
            this.statusStripImage.ResumeLayout(false);
            this.statusStripImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopies)).EndInit();
            this.groupBoxTemperatures.ResumeLayout(false);
            this.groupBoxTemperatures.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAuxTemperatureSetPoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeadTemperatureSetPoint)).EndInit();
            this.groupBoxStatus.ResumeLayout(false);
            this.groupBoxStatus.PerformLayout();
            this.groupBoxControl.ResumeLayout(false);
            this.groupBoxControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYTop)).EndInit();
            this.groupBoxSetup.ResumeLayout(false);
            this.groupBoxSetup.PerformLayout();
            this.groupBoxResolution.ResumeLayout(false);
            this.groupBoxResolution.PerformLayout();
            this.flowLayoutPanelBpp.ResumeLayout(false);
            this.flowLayoutPanelBpp.PerformLayout();
            this.groupBoxPrintClock.ResumeLayout(false);
            this.groupBoxPrintClock.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonLoadImage;
        private System.Windows.Forms.Button buttonStartPrint;
        private System.Windows.Forms.Button buttonStopPrint;
        internal System.Windows.Forms.PictureBox pictureBoxPrintData;
        private System.Windows.Forms.NumericUpDown numericUpDownFrequency;
        private System.Windows.Forms.Label labelkHz;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Label labelCopies;
        private System.Windows.Forms.GroupBox groupBoxPreview;
        private System.Windows.Forms.NumericUpDown numericUpDownCopies;
        private System.Windows.Forms.CheckBox checkBoxHeadTemperatureControl;
        private System.Windows.Forms.CheckBox checkBoxAuxTemperatureControl;
        private System.Windows.Forms.GroupBox groupBoxTemperatures;
        private System.Windows.Forms.NumericUpDown numericUpDownAuxTemperatureSetPoint;
        private System.Windows.Forms.NumericUpDown numericUpDownHeadTemperatureSetPoint;
        private System.Windows.Forms.GroupBox groupBoxStatus;
        private System.Windows.Forms.GroupBox groupBoxControl;
        private System.Windows.Forms.GroupBox groupBoxSetup;
        private System.Windows.Forms.RadioButton radioButtonDiscrete;
        private System.Windows.Forms.RadioButton radioButtonSeamless;
        private System.Windows.Forms.GroupBox groupBoxResolution;
        private System.Windows.Forms.Timer timerMeteorStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialogLoadImage;
        private System.Windows.Forms.CheckBox checkBoxEnableHeadPower;
        private System.Windows.Forms.StatusStrip statusStripImage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFilename;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusImageDimensions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBpp;
        private System.Windows.Forms.RadioButton radioButton1bpp;
        private System.Windows.Forms.RadioButton radioButton2bpp;
        private System.Windows.Forms.RadioButton radioButton4bpp;
        private System.Windows.Forms.NumericUpDown numericUpDownYTop;
        private System.Windows.Forms.Label labelYTop;
        private System.Windows.Forms.GroupBox groupBoxPrintClock;
        private System.Windows.Forms.RadioButton radioButtonInternalEncoder;
        private System.Windows.Forms.RadioButton radioButtonExternalEncoder;
        private System.Windows.Forms.TextBox IPCStatus;
    }
}

