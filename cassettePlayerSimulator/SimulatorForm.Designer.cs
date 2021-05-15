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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimulatorForm));
            this.buttonImport = new System.Windows.Forms.Button();
            this.labelDebug = new System.Windows.Forms.Label();
            this.timerDebug = new System.Windows.Forms.Timer(this.components);
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.counter = new cassettePlayerSimulator.Counter();
            this.cassetteButtons = new cassettePlayerSimulator.CassetteButtons();
            this.cassetteControl = new cassettePlayerSimulator.CassetteControl();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonCreateTape = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonImport
            // 
            this.buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonImport.Enabled = false;
            this.buttonImport.Location = new System.Drawing.Point(608, 18);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(142, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import into current tape";
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
            this.cassetteControl.BackColor = System.Drawing.Color.Gray;
            this.cassetteControl.Location = new System.Drawing.Point(17, 18);
            this.cassetteControl.Name = "cassetteControl";
            this.cassetteControl.Size = new System.Drawing.Size(342, 251);
            this.cassetteControl.TabIndex = 5;
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(456, 47);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(294, 199);
            this.listBox.TabIndex = 10;
            // 
            // buttonCreateTape
            // 
            this.buttonCreateTape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCreateTape.Location = new System.Drawing.Point(456, 18);
            this.buttonCreateTape.Name = "buttonCreateTape";
            this.buttonCreateTape.Size = new System.Drawing.Size(146, 23);
            this.buttonCreateTape.TabIndex = 12;
            this.buttonCreateTape.Text = "Create tape";
            this.buttonCreateTape.UseVisualStyleBackColor = true;
            this.buttonCreateTape.Click += new System.EventHandler(this.buttonCreateTape_Click);
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(762, 481);
            this.Controls.Add(this.buttonCreateTape);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.counter);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl);
            this.Controls.Add(this.buttonImport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
    }
}

