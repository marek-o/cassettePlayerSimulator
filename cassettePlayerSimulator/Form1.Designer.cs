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
            this.cassetteButton4 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteButton5 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteButton6 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteButton2 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteButton1 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteButton3 = new cassettePlayerSimulator.CassetteButton();
            this.cassetteControl1 = new cassettePlayerSimulator.CassetteControl();
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
            // cassetteButton4
            // 
            this.cassetteButton4.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton4.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton4.Location = new System.Drawing.Point(397, 328);
            this.cassetteButton4.Name = "cassetteButton4";
            this.cassetteButton4.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton4.TabIndex = 12;
            // 
            // cassetteButton5
            // 
            this.cassetteButton5.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton5.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton5.Location = new System.Drawing.Point(320, 328);
            this.cassetteButton5.Name = "cassetteButton5";
            this.cassetteButton5.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton5.TabIndex = 11;
            // 
            // cassetteButton6
            // 
            this.cassetteButton6.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton6.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton6.Location = new System.Drawing.Point(243, 328);
            this.cassetteButton6.Name = "cassetteButton6";
            this.cassetteButton6.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton6.TabIndex = 10;
            // 
            // cassetteButton2
            // 
            this.cassetteButton2.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton2.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton2.Location = new System.Drawing.Point(166, 328);
            this.cassetteButton2.Name = "cassetteButton2";
            this.cassetteButton2.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton2.TabIndex = 9;
            // 
            // cassetteButton1
            // 
            this.cassetteButton1.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton1.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton1.Location = new System.Drawing.Point(89, 328);
            this.cassetteButton1.Name = "cassetteButton1";
            this.cassetteButton1.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton1.TabIndex = 8;
            // 
            // cassetteButton3
            // 
            this.cassetteButton3.ButtonState = cassettePlayerSimulator.CassetteButton.State.UP;
            this.cassetteButton3.ButtonType = cassettePlayerSimulator.CassetteButton.Type.BISTABLE;
            this.cassetteButton3.Location = new System.Drawing.Point(12, 328);
            this.cassetteButton3.Name = "cassetteButton3";
            this.cassetteButton3.Size = new System.Drawing.Size(80, 81);
            this.cassetteButton3.TabIndex = 7;
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
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cassetteButton4);
            this.Controls.Add(this.cassetteButton5);
            this.Controls.Add(this.cassetteButton6);
            this.Controls.Add(this.cassetteButton2);
            this.Controls.Add(this.cassetteButton1);
            this.Controls.Add(this.cassetteButton3);
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
        private CassetteButton cassetteButton3;
        private CassetteButton cassetteButton1;
        private CassetteButton cassetteButton2;
        private CassetteButton cassetteButton4;
        private CassetteButton cassetteButton5;
        private CassetteButton cassetteButton6;
    }
}

