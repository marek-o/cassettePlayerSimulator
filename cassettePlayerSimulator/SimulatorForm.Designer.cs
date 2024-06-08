namespace cassettePlayerSimulator
{
    partial class SimulatorForm
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
            this.buttonImport = new System.Windows.Forms.Button();
            this.labelDebug = new System.Windows.Forms.Label();
            this.timerDebug = new System.Windows.Forms.Timer(this.components);
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.counter = new cassettePlayerSimulator.Counter();
            this.cassetteButtons = new cassettePlayerSimulator.CassetteButtons();
            this.cassetteControl = new cassettePlayerSimulator.CassetteControl();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonCreateTape = new System.Windows.Forms.Button();
            this.buttonAbout = new System.Windows.Forms.Button();
            this.labelPosition = new System.Windows.Forms.Label();
            this.trackBarPosition = new System.Windows.Forms.TrackBar();
            this.slidersPanel = new System.Windows.Forms.Panel();
            this.trackBarEffectsVolume = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBarSpeed = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarWow = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBarFlutter = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBarDistortion = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBarHiss = new System.Windows.Forms.TrackBar();
            this.buttonResetDistortionParameters = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Enabled = false;
            this.buttonImport.Location = new System.Drawing.Point(608, 33);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(142, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import into current tape...";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.BackColor = System.Drawing.Color.Transparent;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(0, 361);
            this.labelDebug.Name = "labelDebug";
            this.labelDebug.Size = new System.Drawing.Size(57, 13);
            this.labelDebug.TabIndex = 7;
            this.labelDebug.Text = "debug info";
            // 
            // timerDebug
            // 
            this.timerDebug.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Enabled = true;
            this.timerAnimation.Interval = 10;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // counter
            // 
            this.counter.Location = new System.Drawing.Point(17, 299);
            this.counter.Name = "counter";
            this.counter.Size = new System.Drawing.Size(141, 57);
            this.counter.TabIndex = 8;
            this.counter.Text = "counter1";
            // 
            // cassetteButtons
            // 
            this.cassetteButtons.Location = new System.Drawing.Point(17, 346);
            this.cassetteButtons.Name = "cassetteButtons";
            this.cassetteButtons.Size = new System.Drawing.Size(422, 123);
            this.cassetteButtons.TabIndex = 6;
            // 
            // cassetteControl
            // 
            this.cassetteControl.Location = new System.Drawing.Point(17, 18);
            this.cassetteControl.Name = "cassetteControl";
            this.cassetteControl.Size = new System.Drawing.Size(342, 251);
            this.cassetteControl.TabIndex = 5;
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(456, 62);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(294, 199);
            this.listBox.TabIndex = 10;
            // 
            // buttonCreateTape
            // 
            this.buttonCreateTape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateTape.Location = new System.Drawing.Point(456, 33);
            this.buttonCreateTape.Name = "buttonCreateTape";
            this.buttonCreateTape.Size = new System.Drawing.Size(146, 23);
            this.buttonCreateTape.TabIndex = 12;
            this.buttonCreateTape.Text = "Create tape...";
            this.buttonCreateTape.UseVisualStyleBackColor = true;
            this.buttonCreateTape.Click += new System.EventHandler(this.buttonCreateTape_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAbout.Location = new System.Drawing.Point(608, 8);
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(142, 23);
            this.buttonAbout.TabIndex = 12;
            this.buttonAbout.Text = "About...";
            this.buttonAbout.UseVisualStyleBackColor = true;
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // labelPosition
            // 
            this.labelPosition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.labelPosition.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 12.0f);
            this.labelPosition.Location = new System.Drawing.Point(0, 450);
            this.labelPosition.Size = new System.Drawing.Size(50, 20);
            this.labelPosition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.labelPosition.TabIndex = 14;
            this.labelPosition.Text = "00:00";
            // 
            // trackBarPosition
            // 
            this.trackBarPosition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.trackBarPosition.Location = new System.Drawing.Point(50, 450);
            this.trackBarPosition.Maximum = 1000000;
            this.trackBarPosition.Size = new System.Drawing.Size(380, 38);
            this.trackBarPosition.TabIndex = 13;
            this.trackBarPosition.Value = 100;
            this.trackBarPosition.Scroll += trackBarPosition_Scroll;
            //
            this.slidersPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.slidersPanel.Location = new System.Drawing.Point(456, 276);
            this.slidersPanel.Size = new System.Drawing.Size(300, 183);
            this.slidersPanel.Controls.Add(label6);
            this.slidersPanel.Controls.Add(trackBarHiss);
            this.slidersPanel.Controls.Add(label5);
            this.slidersPanel.Controls.Add(trackBarDistortion);
            this.slidersPanel.Controls.Add(label4);
            this.slidersPanel.Controls.Add(trackBarFlutter);
            this.slidersPanel.Controls.Add(label2);
            this.slidersPanel.Controls.Add(trackBarWow);
            this.slidersPanel.Controls.Add(label3);
            this.slidersPanel.Controls.Add(trackBarSpeed);
            this.slidersPanel.Controls.Add(label1);
            this.slidersPanel.Controls.Add(trackBarEffectsVolume);
            // 
            // trackBarEffectsVolume
            // 
            this.trackBarEffectsVolume.Location = new System.Drawing.Point(80, 0);
            this.trackBarEffectsVolume.Maximum = 100;
            this.trackBarEffectsVolume.Name = "trackBarEffectsVolume";
            this.trackBarEffectsVolume.Size = new System.Drawing.Size(212, 25);
            this.trackBarEffectsVolume.TabIndex = 13;
            this.trackBarEffectsVolume.Value = 50;
            this.trackBarEffectsVolume.Scroll += trackBarEffectsVolume_Scroll;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Effects volume:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 30);
            this.label3.Size = new System.Drawing.Size(88, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Speed:";
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.Location = new System.Drawing.Point(80, 30);
            this.trackBarSpeed.Minimum = 50;
            this.trackBarSpeed.Maximum = 150;
            this.trackBarSpeed.Size = new System.Drawing.Size(212, 30);
            this.trackBarSpeed.TabIndex = 13;
            this.trackBarSpeed.Value = 100;
            this.trackBarSpeed.Scroll += trackBarDistortionParameters_Scroll;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 60);
            this.label2.Size = new System.Drawing.Size(88, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "Wow intensity:";
            // 
            // trackBarWow
            // 
            this.trackBarWow.Location = new System.Drawing.Point(80, 60);
            this.trackBarWow.Maximum = 100;
            this.trackBarWow.Size = new System.Drawing.Size(212, 30);
            this.trackBarWow.TabIndex = 13;
            this.trackBarWow.Value = 0;
            this.trackBarWow.Scroll += trackBarDistortionParameters_Scroll;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 90);
            this.label4.Size = new System.Drawing.Size(88, 15);
            this.label4.TabIndex = 14;
            this.label4.Text = "Flutter intensity:";
            // 
            // trackBarFlutter
            // 
            this.trackBarFlutter.Location = new System.Drawing.Point(80, 90);
            this.trackBarFlutter.Maximum = 100;
            this.trackBarFlutter.Size = new System.Drawing.Size(212, 30);
            this.trackBarFlutter.TabIndex = 13;
            this.trackBarFlutter.Value = 0;
            this.trackBarFlutter.Scroll += trackBarDistortionParameters_Scroll;
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 120);
            this.label5.Size = new System.Drawing.Size(88, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Distortion:";
            // 
            this.trackBarDistortion.Location = new System.Drawing.Point(80, 120);
            this.trackBarDistortion.Maximum = 100;
            this.trackBarDistortion.Size = new System.Drawing.Size(212, 30);
            this.trackBarDistortion.TabIndex = 13;
            this.trackBarDistortion.Value = 0;
            this.trackBarDistortion.Scroll += trackBarDistortionParameters_Scroll;
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 150);
            this.label6.Size = new System.Drawing.Size(88, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "Hiss:";
            // 
            this.trackBarHiss.Location = new System.Drawing.Point(80, 150);
            this.trackBarHiss.Maximum = 100;
            this.trackBarHiss.Size = new System.Drawing.Size(212, 30);
            this.trackBarHiss.TabIndex = 13;
            this.trackBarHiss.Value = 0;
            this.trackBarHiss.Scroll += trackBarDistortionParameters_Scroll;
            // 
            // buttonResetSpeedParameters
            // 
            this.buttonResetDistortionParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResetDistortionParameters.Location = new System.Drawing.Point(456, 461);
            this.buttonResetDistortionParameters.Size = new System.Drawing.Size(146, 23);
            this.buttonResetDistortionParameters.Text = "Reset distortion effects";
            this.buttonResetDistortionParameters.UseVisualStyleBackColor = true;
            this.buttonResetDistortionParameters.Click += new System.EventHandler(this.buttonResetDistortionParameters_Click);
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.Size = new System.Drawing.Size(792, 550);
            this.MinimumSize = new System.Drawing.Size(600, 550);
            this.Controls.Add(this.buttonCreateTape);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.labelPosition);
            this.Controls.Add(this.trackBarPosition);
            this.Controls.Add(this.slidersPanel);
            this.Controls.Add(this.buttonResetDistortionParameters);
            this.Icon = global::cassettePlayerSimulator.Properties.Resources.icon;
            this.Name = "SimulatorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Cassette Player Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SimulatorForm_FormClosing);
            this.Resize += new System.EventHandler(this.SimulatorForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImport;
        private CassetteControl cassetteControl;
        private CassetteButtons cassetteButtons;
        private System.Windows.Forms.Label labelDebug;
        private System.Windows.Forms.Timer timerDebug;
        private System.Windows.Forms.Timer timerAnimation;
        private Counter counter;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonCreateTape;
        private System.Windows.Forms.Button buttonAbout;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.TrackBar trackBarPosition;
        private System.Windows.Forms.Panel slidersPanel;
        private System.Windows.Forms.TrackBar trackBarEffectsVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBarSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarWow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar trackBarFlutter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBarDistortion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar trackBarHiss;
        private System.Windows.Forms.Button buttonResetDistortionParameters;
    }
}

