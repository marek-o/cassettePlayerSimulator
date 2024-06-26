﻿using static cassettePlayerSimulator.Translations;

namespace cassettePlayerSimulator
{
    partial class EditTapeForm
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
            this.radioButtonC10 = new System.Windows.Forms.RadioButton();
            this.radioButtonC30 = new System.Windows.Forms.RadioButton();
            this.radioButtonC60 = new System.Windows.Forms.RadioButton();
            this.radioButtonC90 = new System.Windows.Forms.RadioButton();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxLabelA = new System.Windows.Forms.TextBox();
            this.textBoxLabelB = new System.Windows.Forms.TextBox();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.buttonColor = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelColor = new System.Windows.Forms.Panel();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonC10
            // 
            this.radioButtonC10.AutoSize = true;
            this.radioButtonC10.Location = new System.Drawing.Point(6, 22);
            this.radioButtonC10.Name = "radioButtonC10";
            this.radioButtonC10.Size = new System.Drawing.Size(122, 17);
            this.radioButtonC10.TabIndex = 0;
            this.radioButtonC10.TabStop = true;
            this.radioButtonC10.Text = "C10 (5+5 " + _("minutes") + ", 100MB)";
            this.radioButtonC10.UseVisualStyleBackColor = true;
            // 
            // radioButtonC30
            // 
            this.radioButtonC30.AutoSize = true;
            this.radioButtonC30.Location = new System.Drawing.Point(6, 45);
            this.radioButtonC30.Name = "radioButtonC30";
            this.radioButtonC30.Size = new System.Drawing.Size(122, 17);
            this.radioButtonC30.TabIndex = 0;
            this.radioButtonC30.TabStop = true;
            this.radioButtonC30.Text = "C30 (15+15 " + _("minutes") + ", 300MB)";
            this.radioButtonC30.UseVisualStyleBackColor = true;
            // 
            // radioButtonC60
            // 
            this.radioButtonC60.AutoSize = true;
            this.radioButtonC60.Location = new System.Drawing.Point(6, 68);
            this.radioButtonC60.Name = "radioButtonC60";
            this.radioButtonC60.Size = new System.Drawing.Size(122, 17);
            this.radioButtonC60.TabIndex = 1;
            this.radioButtonC60.TabStop = true;
            this.radioButtonC60.Text = "C60 (30+30 " + _("minutes") + ", 600MB)";
            this.radioButtonC60.UseVisualStyleBackColor = true;
            // 
            // radioButtonC90
            // 
            this.radioButtonC90.AutoSize = true;
            this.radioButtonC90.Location = new System.Drawing.Point(6, 91);
            this.radioButtonC90.Name = "radioButtonC90";
            this.radioButtonC90.Size = new System.Drawing.Size(122, 17);
            this.radioButtonC90.TabIndex = 2;
            this.radioButtonC90.TabStop = true;
            this.radioButtonC90.Text = "C90 (45+45 " + _("minutes") + ", 900MB)";
            this.radioButtonC90.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox.Controls.Add(this.radioButtonC10);
            this.groupBox.Controls.Add(this.radioButtonC90);
            this.groupBox.Controls.Add(this.radioButtonC30);
            this.groupBox.Controls.Add(this.radioButtonC60);
            this.groupBox.Location = new System.Drawing.Point(12, 12);
            this.groupBox.Name = "groupBox1";
            this.groupBox.Size = new System.Drawing.Size(316, 122);
            this.groupBox.TabIndex = 3;
            this.groupBox.TabStop = false;
            this.groupBox.Text = _("Length");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 146);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = _("Label (side A):");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = _("Label (side B):");
            // 
            // textBoxLabelA
            // 
            this.textBoxLabelA.Location = new System.Drawing.Point(113, 146);
            this.textBoxLabelA.Name = "textBoxLabelA";
            this.textBoxLabelA.Size = new System.Drawing.Size(215, 20);
            this.textBoxLabelA.TabIndex = 6;
            // 
            // textBoxLabelB
            // 
            this.textBoxLabelB.Location = new System.Drawing.Point(113, 175);
            this.textBoxLabelB.Name = "textBoxLabelB";
            this.textBoxLabelB.Size = new System.Drawing.Size(215, 20);
            this.textBoxLabelB.TabIndex = 7;
            // 
            // buttonColor
            // 
            this.buttonColor.Location = new System.Drawing.Point(11, 201);
            this.buttonColor.Name = "buttonColor";
            this.buttonColor.Size = new System.Drawing.Size(100, 23);
            this.buttonColor.TabIndex = 8;
            this.buttonColor.Text = _("Select color");
            this.buttonColor.UseVisualStyleBackColor = true;
            this.buttonColor.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // buttonCreate
            // 
            this.buttonCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCreate.Location = new System.Drawing.Point(172, 249);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 9;
            this.buttonCreate.Text = _("Create");
            this.buttonCreate.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(253, 249);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = _("Cancel");
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // panelColor
            // 
            this.panelColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelColor.Location = new System.Drawing.Point(117, 202);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(102, 23);
            this.panelColor.TabIndex = 11;
            // 
            // EditTapeForm
            // 
            this.AcceptButton = this.buttonCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(339, 285);
            this.Controls.Add(this.panelColor);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.buttonColor);
            this.Controls.Add(this.textBoxLabelB);
            this.Controls.Add(this.textBoxLabelA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTapeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = _("Creating empty tape");
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonC10;
        private System.Windows.Forms.RadioButton radioButtonC30;
        private System.Windows.Forms.RadioButton radioButtonC60;
        private System.Windows.Forms.RadioButton radioButtonC90;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxLabelA;
        private System.Windows.Forms.TextBox textBoxLabelB;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button buttonColor;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panelColor;
    }
}