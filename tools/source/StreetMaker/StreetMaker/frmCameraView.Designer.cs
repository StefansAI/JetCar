namespace StreetMaker
{
    partial class frmCameraView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCameraView));
            this.pbMaskImage = new System.Windows.Forms.PictureBox();
            this.pbCameraImg = new System.Windows.Forms.PictureBox();
            this.pnControl = new System.Windows.Forms.Panel();
            this.pnButtons = new System.Windows.Forms.Panel();
            this.lbIdx = new System.Windows.Forms.Label();
            this.btnFirst = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.pbPredictionImage = new System.Windows.Forms.PictureBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbMaskImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCameraImg)).BeginInit();
            this.pnControl.SuspendLayout();
            this.pnButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPredictionImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMaskImage
            // 
            this.pbMaskImage.Location = new System.Drawing.Point(459, 5);
            this.pbMaskImage.Name = "pbMaskImage";
            this.pbMaskImage.Size = new System.Drawing.Size(448, 448);
            this.pbMaskImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMaskImage.TabIndex = 6;
            this.pbMaskImage.TabStop = false;
            this.toolTip1.SetToolTip(this.pbMaskImage, "Generated object class map for the same scene as displayed in the camera view.");
            // 
            // pbCameraImg
            // 
            this.pbCameraImg.Location = new System.Drawing.Point(5, 5);
            this.pbCameraImg.Name = "pbCameraImg";
            this.pbCameraImg.Size = new System.Drawing.Size(448, 448);
            this.pbCameraImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCameraImg.TabIndex = 5;
            this.pbCameraImg.TabStop = false;
            this.toolTip1.SetToolTip(this.pbCameraImg, "Artificial image of a camera view of the street scene.");
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.pnButtons);
            this.pnControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnControl.Location = new System.Drawing.Point(0, 479);
            this.pnControl.Name = "pnControl";
            this.pnControl.Size = new System.Drawing.Size(1368, 40);
            this.pnControl.TabIndex = 4;
            // 
            // pnButtons
            // 
            this.pnButtons.Controls.Add(this.lbIdx);
            this.pnButtons.Controls.Add(this.btnFirst);
            this.pnButtons.Controls.Add(this.btnNext);
            this.pnButtons.Controls.Add(this.btnLast);
            this.pnButtons.Controls.Add(this.btnPrevious);
            this.pnButtons.Location = new System.Drawing.Point(463, 5);
            this.pnButtons.Name = "pnButtons";
            this.pnButtons.Size = new System.Drawing.Size(429, 29);
            this.pnButtons.TabIndex = 5;
            // 
            // lbIdx
            // 
            this.lbIdx.Location = new System.Drawing.Point(180, 3);
            this.lbIdx.Name = "lbIdx";
            this.lbIdx.Size = new System.Drawing.Size(71, 23);
            this.lbIdx.TabIndex = 4;
            this.lbIdx.Text = "-";
            this.lbIdx.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lbIdx, "Index of the current camera view and mask in the dataset.");
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(4, 3);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(75, 23);
            this.btnFirst.TabIndex = 3;
            this.btnFirst.Text = "First";
            this.toolTip1.SetToolTip(this.btnFirst, "Go to the very first camera view and mask of this dataset.");
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(257, 3);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.toolTip1.SetToolTip(this.btnNext, "Go to the next camera view and mask of this dataset.");
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(351, 3);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(75, 23);
            this.btnLast.TabIndex = 2;
            this.btnLast.Text = "Last";
            this.toolTip1.SetToolTip(this.btnLast, "Go to the very last camera view and mask of this dataset.");
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(99, 3);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 0;
            this.btnPrevious.Text = "Previous";
            this.toolTip1.SetToolTip(this.btnPrevious, "Go to the previous camera view and mask of this dataset.");
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // pbPredictionImage
            // 
            this.pbPredictionImage.Location = new System.Drawing.Point(913, 5);
            this.pbPredictionImage.Name = "pbPredictionImage";
            this.pbPredictionImage.Size = new System.Drawing.Size(448, 448);
            this.pbPredictionImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPredictionImage.TabIndex = 7;
            this.pbPredictionImage.TabStop = false;
            this.toolTip1.SetToolTip(this.pbPredictionImage, "Prediction object code mask from the training run. ");
            // 
            // lbStatus
            // 
            this.lbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbStatus.Location = new System.Drawing.Point(0, 459);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(1368, 20);
            this.lbStatus.TabIndex = 8;
            this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lbStatus, "File name of the current camera view image. Mask and prediction filenames are ide" +
        "ntical except the prefix.");
            // 
            // frmCameraView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 519);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.pbPredictionImage);
            this.Controls.Add(this.pbMaskImage);
            this.Controls.Add(this.pbCameraImg);
            this.Controls.Add(this.pnControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCameraView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCameraView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pbMaskImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCameraImg)).EndInit();
            this.pnControl.ResumeLayout(false);
            this.pnButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPredictionImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMaskImage;
        private System.Windows.Forms.PictureBox pbCameraImg;
        private System.Windows.Forms.Panel pnControl;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Panel pnButtons;
        private System.Windows.Forms.PictureBox pbPredictionImage;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbIdx;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}