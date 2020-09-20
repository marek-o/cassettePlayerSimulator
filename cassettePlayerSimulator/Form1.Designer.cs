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
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonLoadTape = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cassetteControl1 = new cassettePlayerSimulator.CassetteControl();
            this.cassetteButton1 = new cassettePlayerSimulator.CassetteButton();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Location = new System.Drawing.Point(504, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(93, 92);
            this.panel1.TabIndex = 0;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
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
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel2.Location = new System.Drawing.Point(603, 50);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(93, 92);
            this.panel2.TabIndex = 3;
            this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseUp);
            // 
            // cassetteControl1
            // 
            this.cassetteControl1.Location = new System.Drawing.Point(39, 12);
            this.cassetteControl1.Name = "cassetteControl1";
            this.cassetteControl1.Size = new System.Drawing.Size(422, 267);
            this.cassetteControl1.TabIndex = 5;
            // 
            // cassetteButton1
            // 
            this.cassetteButton1.Location = new System.Drawing.Point(39, 306);
            this.cassetteButton1.Name = "cassetteButton1";
            this.cassetteButton1.Size = new System.Drawing.Size(422, 90);
            this.cassetteButton1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cassetteButton1);
            this.Controls.Add(this.cassetteControl1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.buttonLoadTape);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonLoadTape;
        private System.Windows.Forms.Panel panel2;
        private CassetteControl cassetteControl1;
        private CassetteButton cassetteButton1;
    }
}

