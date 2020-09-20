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
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonLoadTape = new System.Windows.Forms.Button();
            this.cassetteButtons = new cassettePlayerSimulator.CassetteButtons();
            this.cassetteControl1 = new cassettePlayerSimulator.CassetteControl();
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(762, 507);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl1);
            this.Controls.Add(this.buttonLoadTape);
            this.Controls.Add(this.buttonImport);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonLoadTape;
        private CassetteControl cassetteControl1;
        private CassetteButtons cassetteButtons;
    }
}

