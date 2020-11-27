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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timerAnimation = new System.Windows.Forms.Timer(this.components);
            this.counter1 = new cassettePlayerSimulator.Counter();
            this.cassetteButtons = new cassettePlayerSimulator.CassetteButtons();
            this.cassetteControl1 = new cassettePlayerSimulator.CassetteControl();
            this.listBox = new System.Windows.Forms.ListBox();
            this.buttonSaveList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(495, 166);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 1;
            this.buttonImport.Text = "Import";
            this.buttonImport.UseVisualStyleBackColor = true;
            this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.Location = new System.Drawing.Point(492, 40);
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
            this.timerAnimation.Interval = 10;
            this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
            // 
            // counter1
            // 
            this.counter1.Location = new System.Drawing.Point(17, 299);
            this.counter1.Name = "counter1";
            this.counter1.Size = new System.Drawing.Size(139, 51);
            this.counter1.TabIndex = 8;
            this.counter1.Text = "counter1";
            // 
            // cassetteButtons
            // 
            this.cassetteButtons.Location = new System.Drawing.Point(17, 346);
            this.cassetteButtons.Name = "cassetteButtons";
            this.cassetteButtons.Size = new System.Drawing.Size(422, 123);
            this.cassetteButtons.TabIndex = 6;
            // 
            // cassetteControl1
            // 
            this.cassetteControl1.BackColor = System.Drawing.Color.Gray;
            this.cassetteControl1.CassetteInserted = false;
            this.cassetteControl1.Location = new System.Drawing.Point(17, 18);
            this.cassetteControl1.Name = "cassetteControl1";
            this.cassetteControl1.Size = new System.Drawing.Size(422, 275);
            this.cassetteControl1.TabIndex = 5;
            // 
            // listBox
            // 
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 50;
            this.listBox.Location = new System.Drawing.Point(593, 89);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(294, 204);
            this.listBox.TabIndex = 10;
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox_DrawItem);
            this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
            // 
            // buttonSaveList
            // 
            this.buttonSaveList.Location = new System.Drawing.Point(593, 40);
            this.buttonSaveList.Name = "buttonSaveList";
            this.buttonSaveList.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveList.TabIndex = 11;
            this.buttonSaveList.Text = "Save list";
            this.buttonSaveList.UseVisualStyleBackColor = true;
            this.buttonSaveList.Click += new System.EventHandler(this.buttonSaveList_Click);
            // 
            // SimulatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(899, 481);
            this.Controls.Add(this.buttonSaveList);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.counter1);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.cassetteButtons);
            this.Controls.Add(this.cassetteControl1);
            this.Controls.Add(this.buttonImport);
            this.MaximizeBox = false;
            this.Name = "SimulatorForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Cassette Player Simulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonImport;
        private CassetteControl cassetteControl1;
        private CassetteButtons cassetteButtons;
        private System.Windows.Forms.Label labelDebug;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerAnimation;
        private Counter counter1;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button buttonSaveList;
    }
}

