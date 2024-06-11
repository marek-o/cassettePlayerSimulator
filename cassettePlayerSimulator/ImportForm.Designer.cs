using static cassettePlayerSimulator.Translations;

namespace cassettePlayerSimulator
{
    partial class ImportForm
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBarPosition = new System.Windows.Forms.TrackBar();
            this.labelTapePosition = new System.Windows.Forms.Label();
            this.labelLength = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTapeLength = new System.Windows.Forms.Label();
            this.positionSelectorControl = new cassettePlayerSimulator.PositionSelectorControl();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(270, 255);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = _("Cancel");
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonImport
            // 
            this.buttonImport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonImport.Location = new System.Drawing.Point(189, 255);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(75, 23);
            this.buttonImport.TabIndex = 11;
            this.buttonImport.Text = _("Import");
            this.buttonImport.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = _("Input file:");
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(95, 35);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(57, 23);
            this.buttonSelectFile.TabIndex = 14;
            this.buttonSelectFile.Text = _("Select");
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Location = new System.Drawing.Point(95, 11);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.ReadOnly = true;
            this.textBoxFilename.Size = new System.Drawing.Size(251, 20);
            this.textBoxFilename.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 216);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = _("Tape position:");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = _("File length:");
            // 
            // trackBarPosition
            // 
            this.trackBarPosition.Location = new System.Drawing.Point(15, 64);
            this.trackBarPosition.Name = "trackBarPosition";
            this.trackBarPosition.Size = new System.Drawing.Size(331, 45);
            this.trackBarPosition.TabIndex = 18;
            this.trackBarPosition.Scroll += new System.EventHandler(this.trackBarPosition_Scroll);
            // 
            // labelTapePosition
            // 
            this.labelTapePosition.AutoSize = true;
            this.labelTapePosition.Location = new System.Drawing.Point(91, 216);
            this.labelTapePosition.Name = "labelTapePosition";
            this.labelTapePosition.Size = new System.Drawing.Size(22, 13);
            this.labelTapePosition.TabIndex = 19;
            this.labelTapePosition.Text = "--:--";
            // 
            // labelLength
            // 
            this.labelLength.AutoSize = true;
            this.labelLength.Location = new System.Drawing.Point(91, 193);
            this.labelLength.Name = "labelLength";
            this.labelLength.Size = new System.Drawing.Size(22, 13);
            this.labelLength.TabIndex = 20;
            this.labelLength.Text = "--:--";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = _("Tape length:");
            // 
            // labelTapeLength
            // 
            this.labelTapeLength.AutoSize = true;
            this.labelTapeLength.Location = new System.Drawing.Point(91, 239);
            this.labelTapeLength.Name = "labelTapeLength";
            this.labelTapeLength.Size = new System.Drawing.Size(22, 13);
            this.labelTapeLength.TabIndex = 22;
            this.labelTapeLength.Text = "--:--";
            // 
            // positionSelectorControl1
            // 
            this.positionSelectorControl.Location = new System.Drawing.Point(15, 115);
            this.positionSelectorControl.Name = "positionSelectorControl1";
            this.positionSelectorControl.Size = new System.Drawing.Size(335, 45);
            this.positionSelectorControl.TabIndex = 23;
            this.positionSelectorControl.Text = "positionSelectorControl1";
            // 
            // ImportForm
            // 
            this.AcceptButton = this.buttonImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(353, 290);
            this.Controls.Add(this.positionSelectorControl);
            this.Controls.Add(this.labelTapeLength);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelLength);
            this.Controls.Add(this.labelTapePosition);
            this.Controls.Add(this.trackBarPosition);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonImport);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = _("Importing file");
            this.Load += new System.EventHandler(this.ImportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.TextBox textBoxFilename;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBarPosition;
        private System.Windows.Forms.Label labelTapePosition;
        private System.Windows.Forms.Label labelLength;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelTapeLength;
        private PositionSelectorControl positionSelectorControl;
    }
}