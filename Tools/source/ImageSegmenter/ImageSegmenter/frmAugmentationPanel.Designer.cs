namespace ImageSegmenter
{
    partial class frmAugmentationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAugmentationPanel));
            this.label1 = new System.Windows.Forms.Label();
            this.nudZoom = new System.Windows.Forms.NumericUpDown();
            this.nudTilt = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nudBright = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudNoise = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nudContrast = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTilt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBright)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoise)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Zoom:";
            // 
            // nudZoom
            // 
            this.nudZoom.DecimalPlaces = 1;
            this.nudZoom.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudZoom.Location = new System.Drawing.Point(50, 7);
            this.nudZoom.Name = "nudZoom";
            this.nudZoom.Size = new System.Drawing.Size(59, 20);
            this.nudZoom.TabIndex = 1;
            this.nudZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudZoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudAny_KeyDown);
            // 
            // nudTilt
            // 
            this.nudTilt.Location = new System.Drawing.Point(50, 33);
            this.nudTilt.Name = "nudTilt";
            this.nudTilt.Size = new System.Drawing.Size(59, 20);
            this.nudTilt.TabIndex = 3;
            this.nudTilt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudAny_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tilt:";
            // 
            // nudBright
            // 
            this.nudBright.DecimalPlaces = 1;
            this.nudBright.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudBright.Location = new System.Drawing.Point(50, 59);
            this.nudBright.Name = "nudBright";
            this.nudBright.Size = new System.Drawing.Size(59, 20);
            this.nudBright.TabIndex = 5;
            this.nudBright.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBright.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudAny_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Bright:";
            // 
            // nudNoise
            // 
            this.nudNoise.Location = new System.Drawing.Point(50, 111);
            this.nudNoise.Name = "nudNoise";
            this.nudNoise.Size = new System.Drawing.Size(59, 20);
            this.nudNoise.TabIndex = 11;
            this.nudNoise.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudAny_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Noise:";
            // 
            // nudContrast
            // 
            this.nudContrast.DecimalPlaces = 1;
            this.nudContrast.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudContrast.Location = new System.Drawing.Point(50, 85);
            this.nudContrast.Name = "nudContrast";
            this.nudContrast.Size = new System.Drawing.Size(59, 20);
            this.nudContrast.TabIndex = 13;
            this.nudContrast.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nudAny_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Contrast:";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(6, 138);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(103, 23);
            this.btnApply.TabIndex = 14;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // frmAugmentationPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(114, 167);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.nudContrast);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudNoise);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudBright);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudTilt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudZoom);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAugmentationPanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Augmentation";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAugmentationPanel_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTilt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBright)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoise)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudZoom;
        private System.Windows.Forms.NumericUpDown nudTilt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudBright;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudNoise;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudContrast;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnApply;
    }
}