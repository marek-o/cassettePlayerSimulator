namespace cassettePlayerSimulator
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
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonLoadTape = new System.Windows.Forms.Button();
            this.labelDebug = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.spoolControl1 = new cassettePlayerSimulator.SpoolControl();
            this.cassetteButtons = new cassettePlayerSimulator.CassetteButtons();
            this.cassetteControl1 = new cassettePlayerSimulator.CassetteControl();
            this.spoolControl2 = new cassettePlayerSimulator.SpoolControl();
            this.SuspendLayout();
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(504, 148);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // buttonLoadTape
            // 
            this.buttonLoadTape.Location = new System.Drawing.Point(504, 177);
            this.buttonLoadTape.Name = "buttonLoadTape";
            this.buttonLoadTape.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadTape.TabIndex = 2;
            this.buttonLoadTape.Text = "Load tape";
            this.buttonLoadTape.UseVisualStyleBackColor = true;
            this.buttonLoadTape.Click += new System.EventHandler(this.buttonLoadTape_Click);
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(501, 22);
            this.labelDebug.Name = "labelDebug";
            this.labelDebug.Size = new System.Drawing.Size(57, 13);
            this.labelDebug.TabIndex = 7;
            this.labelDebug.Text = "debug info";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timerAnimation
            // 
            this.timerAnimation.Enabled = true;
            this.timerAnimation.Interval = 16;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // spoolControl1
            // 
            this.spoolControl1.Location = new System.Drawing.Point(134, 109);
            this.spoolControl1.Name = "spoolControl1";
            this.spoolControl1.Size = new System.Drawing.Size(50, 50);
            this.spoolControl1.TabIndex = 8;
            // 
            // cassetteButtons
            // 
            this.cassetteButtons.Location = new System.Drawing.Point(39, 311);
            this.cassetteButtons.Name = "cassetteButtons";
            this.cassetteButtons.Size = new System.Drawing.Size(422, 123);
            this.cassetteButtons.TabIndex = 6;
            // 
            // cassetteControl1
            // 
            this.cassetteControl1.Location = new System.Drawing.Point(39, 12);
            this.cassetteControl1.Name = "cassetteControl1";
            this.cassetteControl1.Size = new System.Drawing.Size(422, 267);
            this.cassetteControl1.TabIndex = 5;
            // 
            // spoolControl2
            // 
            this.spoolControl2.Location = new System.Drawing.Point(304, 109);
            this.spoolControl2.Name = "spoolControl2";
            this.spoolControl2.Size = new System.Drawing.Size(50, 50);
            this.spoolControl2.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(762, 507);
            this.Controls.Add(this.spoolControl2);
            this.Controls.Add(this.spoolControl1);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl1);
            this.Controls.Add(this.buttonLoadTape);
            this.Controls.Add(this.buttonImport);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonLoadTape;
        private CassetteControl cassetteControl1;
        private CassetteButtons cassetteButtons;
        private System.Windows.Forms.Label labelDebug;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerAnimation;
        private SpoolControl spoolControl1;
        private SpoolControl spoolControl2;
    }
}

