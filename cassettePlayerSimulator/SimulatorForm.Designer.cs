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
            this.trackBarEffectsVolume = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBarSpeed = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarWow = new System.Windows.Forms.TrackBar();
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
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(474, 361);
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
            // trackBarEffectsVolume
            // 
            this.trackBarEffectsVolume.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.trackBarEffectsVolume.Location = new System.Drawing.Point(536, 276);
            this.trackBarEffectsVolume.Maximum = 100;
            this.trackBarEffectsVolume.Name = "trackBarEffectsVolume";
            this.trackBarEffectsVolume.Size = new System.Drawing.Size(212, 38);
            this.trackBarEffectsVolume.TabIndex = 13;
            this.trackBarEffectsVolume.Value = 100;
            this.trackBarEffectsVolume.Scroll += trackBarEffectsVolume_Scroll;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(456, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Effects volume:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(456, 316);
            this.label3.Size = new System.Drawing.Size(88, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Speed:";
            // 
            // trackBarSpeed
            // 
            this.trackBarSpeed.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.trackBarSpeed.Location = new System.Drawing.Point(536, 316);
            this.trackBarSpeed.Minimum = 50;
            this.trackBarSpeed.Maximum = 150;
            this.trackBarSpeed.Size = new System.Drawing.Size(212, 38);
            this.trackBarSpeed.TabIndex = 13;
            this.trackBarSpeed.Value = 100;
            this.trackBarSpeed.Scroll += trackBarSpeed_Scroll;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(456, 356);
            this.label2.Size = new System.Drawing.Size(88, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "Wow intensity:";
            // 
            // trackBarWow
            // 
            this.trackBarWow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.trackBarWow.Location = new System.Drawing.Point(536, 356);
            this.trackBarWow.Maximum = 100;
            this.trackBarWow.Size = new System.Drawing.Size(212, 38);
            this.trackBarWow.TabIndex = 13;
            this.trackBarWow.Value = 0;
            this.trackBarWow.Scroll += trackBarWow_Scroll;
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(762, 481);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.buttonCreateTape);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonAbout);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarEffectsVolume);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackBarSpeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBarWow);
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
        private System.Windows.Forms.TrackBar trackBarEffectsVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBarSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarWow;
    }
}

