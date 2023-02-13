namespace ImageSegmenter
{
    partial class frmProcessConfirmation
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nudStartImageNumber = new System.Windows.Forms.NumericUpDown();
            this.ckbClearOutputDirs = new System.Windows.Forms.CheckBox();
            this.nudEndImageNumber = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartImageNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndImageNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(151, 108);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(32, 108);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start at Image Number:";
            // 
            // nudStartImageNumber
            // 
            this.nudStartImageNumber.Location = new System.Drawing.Point(170, 12);
            this.nudStartImageNumber.Name = "nudStartImageNumber";
            this.nudStartImageNumber.Size = new System.Drawing.Size(56, 20);
            this.nudStartImageNumber.TabIndex = 4;
            this.nudStartImageNumber.ValueChanged += new System.EventHandler(this.nudStartImageNumber_ValueChanged);
            // 
            // ckbClearOutputDirs
            // 
            this.ckbClearOutputDirs.AutoSize = true;
            this.ckbClearOutputDirs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbClearOutputDirs.Checked = true;
            this.ckbClearOutputDirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbClearOutputDirs.Location = new System.Drawing.Point(32, 74);
            this.ckbClearOutputDirs.Name = "ckbClearOutputDirs";
            this.ckbClearOutputDirs.Size = new System.Drawing.Size(151, 17);
            this.ckbClearOutputDirs.TabIndex = 5;
            this.ckbClearOutputDirs.Text = "Clear all Output Directories";
            this.ckbClearOutputDirs.UseVisualStyleBackColor = true;
            // 
            // nudEndImageNumber
            // 
            this.nudEndImageNumber.Location = new System.Drawing.Point(170, 38);
            this.nudEndImageNumber.Name = "nudEndImageNumber";
            this.nudEndImageNumber.Size = new System.Drawing.Size(56, 20);
            this.nudEndImageNumber.TabIndex = 7;
            this.nudEndImageNumber.ValueChanged += new System.EventHandler(this.nudEndImageNumber_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "End at Image Number:";
            // 
            // frmProcessConfirmation
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(260, 155);
            this.ControlBox = false;
            this.Controls.Add(this.nudEndImageNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ckbClearOutputDirs);
            this.Controls.Add(this.nudStartImageNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmProcessConfirmation";
            this.Text = "Process All Confirmation";
            ((System.ComponentModel.ISupportInitialize)(this.nudStartImageNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudEndImageNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudStartImageNumber;
        private System.Windows.Forms.CheckBox ckbClearOutputDirs;
        private System.Windows.Forms.NumericUpDown nudEndImageNumber;
        private System.Windows.Forms.Label label2;
    }
}