namespace ImageSegmenter
{
    partial class frmAppSettings
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAppSettings));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowsePathToPredictedMasks = new System.Windows.Forms.Button();
            this.tbPathToPredictedMasks = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.btnBrowsePathToOutputDataset = new System.Windows.Forms.Button();
            this.tbPathToOutputDatset = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnBrowsePathToSessionData = new System.Windows.Forms.Button();
            this.tbPathToSessionData = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowsePathToSourceImages = new System.Windows.Forms.Button();
            this.tbPathToSourceImages = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.fbdSelectPath = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbSubDirInfo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbSubDirMask = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSubDirImg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbSubDirVal = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSubDirTrain = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbPrefixInfo = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbPrefixMask = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbPrefixImg = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dsGridViews = new System.Data.DataSet();
            this.dtSegmClasses = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dgvSegmClasses = new System.Windows.Forms.DataGridView();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.ckbBrightnessContrastExclusive = new System.Windows.Forms.CheckBox();
            this.ckbTiltWithZoomOnly = new System.Windows.Forms.CheckBox();
            this.nudMaskDrawOrderMax = new System.Windows.Forms.NumericUpDown();
            this.nudMaskDrawOrderMin = new System.Windows.Forms.NumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.tbContrastEnhancement = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.nudImageOutputSizeHeight = new System.Windows.Forms.NumericUpDown();
            this.nudImageOutputSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.nudTrainValRatio = new System.Windows.Forms.NumericUpDown();
            this.tbNoiseAdders = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbBrightnessFactors = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tbTiltAngles = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.tbZoomFactors = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.nudPolygonPointSize = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.nudScrollStartMinCount = new System.Windows.Forms.NumericUpDown();
            this.nudScrollMoveFactor = new System.Windows.Forms.NumericUpDown();
            this.nudScrollZoneSize = new System.Windows.Forms.NumericUpDown();
            this.nudDrawMaskTransparency = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.btnPolygonPointColor = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.btnPolygonLineColor = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drawOrderDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.drawColorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsGridViews)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSegmClasses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSegmClasses)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskDrawOrderMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskDrawOrderMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageOutputSizeHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageOutputSizeWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrainValRatio)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPolygonPointSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollStartMinCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollMoveFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollZoneSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDrawMaskTransparency)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBrowsePathToPredictedMasks);
            this.groupBox1.Controls.Add(this.tbPathToPredictedMasks);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.btnBrowsePathToOutputDataset);
            this.groupBox1.Controls.Add(this.tbPathToOutputDatset);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnBrowsePathToSessionData);
            this.groupBox1.Controls.Add(this.tbPathToSessionData);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnBrowsePathToSourceImages);
            this.groupBox1.Controls.Add(this.tbPathToSourceImages);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(919, 135);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Path";
            // 
            // btnBrowsePathToPredictedMasks
            // 
            this.btnBrowsePathToPredictedMasks.Location = new System.Drawing.Point(821, 98);
            this.btnBrowsePathToPredictedMasks.Name = "btnBrowsePathToPredictedMasks";
            this.btnBrowsePathToPredictedMasks.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePathToPredictedMasks.TabIndex = 11;
            this.btnBrowsePathToPredictedMasks.Text = "Browse";
            this.btnBrowsePathToPredictedMasks.UseVisualStyleBackColor = true;
            this.btnBrowsePathToPredictedMasks.Click += new System.EventHandler(this.btnBrowsePathToPredictedMasks_Click);
            // 
            // tbPathToPredictedMasks
            // 
            this.tbPathToPredictedMasks.Location = new System.Drawing.Point(138, 100);
            this.tbPathToPredictedMasks.Name = "tbPathToPredictedMasks";
            this.tbPathToPredictedMasks.Size = new System.Drawing.Size(666, 20);
            this.tbPathToPredictedMasks.TabIndex = 10;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(19, 103);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(106, 13);
            this.label30.TabIndex = 9;
            this.label30.Text = "Path to Pred. Masks:";
            // 
            // btnBrowsePathToOutputDataset
            // 
            this.btnBrowsePathToOutputDataset.Location = new System.Drawing.Point(821, 69);
            this.btnBrowsePathToOutputDataset.Name = "btnBrowsePathToOutputDataset";
            this.btnBrowsePathToOutputDataset.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePathToOutputDataset.TabIndex = 8;
            this.btnBrowsePathToOutputDataset.Text = "Browse";
            this.btnBrowsePathToOutputDataset.UseVisualStyleBackColor = true;
            this.btnBrowsePathToOutputDataset.Click += new System.EventHandler(this.btnBrowsePathToOutputDataset_Click);
            // 
            // tbPathToOutputDatset
            // 
            this.tbPathToOutputDatset.Location = new System.Drawing.Point(138, 71);
            this.tbPathToOutputDatset.Name = "tbPathToOutputDatset";
            this.tbPathToOutputDatset.Size = new System.Drawing.Size(666, 20);
            this.tbPathToOutputDatset.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Path to Output Dataset:";
            // 
            // btnBrowsePathToSessionData
            // 
            this.btnBrowsePathToSessionData.Location = new System.Drawing.Point(821, 43);
            this.btnBrowsePathToSessionData.Name = "btnBrowsePathToSessionData";
            this.btnBrowsePathToSessionData.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePathToSessionData.TabIndex = 5;
            this.btnBrowsePathToSessionData.Text = "Browse";
            this.btnBrowsePathToSessionData.UseVisualStyleBackColor = true;
            this.btnBrowsePathToSessionData.Click += new System.EventHandler(this.btnBrowsePathToSessionData_Click);
            // 
            // tbPathToSessionData
            // 
            this.tbPathToSessionData.Location = new System.Drawing.Point(138, 45);
            this.tbPathToSessionData.Name = "tbPathToSessionData";
            this.tbPathToSessionData.Size = new System.Drawing.Size(666, 20);
            this.tbPathToSessionData.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Path to Session Data:";
            // 
            // btnBrowsePathToSourceImages
            // 
            this.btnBrowsePathToSourceImages.Location = new System.Drawing.Point(821, 17);
            this.btnBrowsePathToSourceImages.Name = "btnBrowsePathToSourceImages";
            this.btnBrowsePathToSourceImages.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePathToSourceImages.TabIndex = 2;
            this.btnBrowsePathToSourceImages.Text = "Browse";
            this.btnBrowsePathToSourceImages.UseVisualStyleBackColor = true;
            this.btnBrowsePathToSourceImages.Click += new System.EventHandler(this.btnBrowsePathToSourceImages_Click);
            // 
            // tbPathToSourceImages
            // 
            this.tbPathToSourceImages.Location = new System.Drawing.Point(138, 19);
            this.tbPathToSourceImages.Name = "tbPathToSourceImages";
            this.tbPathToSourceImages.Size = new System.Drawing.Size(666, 20);
            this.tbPathToSourceImages.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path to Source Images:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(361, 853);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(525, 853);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbSubDirInfo);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbSubDirMask);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbSubDirImg);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(10, 152);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(294, 102);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sub Directories Level 1";
            // 
            // tbSubDirInfo
            // 
            this.tbSubDirInfo.Location = new System.Drawing.Point(138, 71);
            this.tbSubDirInfo.Name = "tbSubDirInfo";
            this.tbSubDirInfo.Size = new System.Drawing.Size(133, 20);
            this.tbSubDirInfo.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Sub Dir Info:";
            // 
            // tbSubDirMask
            // 
            this.tbSubDirMask.Location = new System.Drawing.Point(138, 45);
            this.tbSubDirMask.Name = "tbSubDirMask";
            this.tbSubDirMask.Size = new System.Drawing.Size(133, 20);
            this.tbSubDirMask.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Sub Dir Mask:";
            // 
            // tbSubDirImg
            // 
            this.tbSubDirImg.Location = new System.Drawing.Point(138, 19);
            this.tbSubDirImg.Name = "tbSubDirImg";
            this.tbSubDirImg.Size = new System.Drawing.Size(133, 20);
            this.tbSubDirImg.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Sub Dir Image:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbSubDirVal);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.tbSubDirTrain);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(323, 152);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(294, 102);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sub Directories Level 2";
            // 
            // tbSubDirVal
            // 
            this.tbSubDirVal.Location = new System.Drawing.Point(138, 45);
            this.tbSubDirVal.Name = "tbSubDirVal";
            this.tbSubDirVal.Size = new System.Drawing.Size(133, 20);
            this.tbSubDirVal.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Sub Dir Validation:";
            // 
            // tbSubDirTrain
            // 
            this.tbSubDirTrain.Location = new System.Drawing.Point(138, 19);
            this.tbSubDirTrain.Name = "tbSubDirTrain";
            this.tbSubDirTrain.Size = new System.Drawing.Size(133, 20);
            this.tbSubDirTrain.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Sub Dir Train:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbPrefixInfo);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.tbPrefixMask);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.tbPrefixImg);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Location = new System.Drawing.Point(635, 152);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(294, 102);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "File Name Prefix";
            // 
            // tbPrefixInfo
            // 
            this.tbPrefixInfo.Location = new System.Drawing.Point(138, 71);
            this.tbPrefixInfo.Name = "tbPrefixInfo";
            this.tbPrefixInfo.Size = new System.Drawing.Size(133, 20);
            this.tbPrefixInfo.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Prefix Info:";
            // 
            // tbPrefixMask
            // 
            this.tbPrefixMask.Location = new System.Drawing.Point(138, 45);
            this.tbPrefixMask.Name = "tbPrefixMask";
            this.tbPrefixMask.Size = new System.Drawing.Size(133, 20);
            this.tbPrefixMask.TabIndex = 5;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Prefix Mask:";
            // 
            // tbPrefixImg
            // 
            this.tbPrefixImg.Location = new System.Drawing.Point(138, 19);
            this.tbPrefixImg.Name = "tbPrefixImg";
            this.tbPrefixImg.Size = new System.Drawing.Size(133, 20);
            this.tbPrefixImg.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Prefix Image:";
            // 
            // dsGridViews
            // 
            this.dsGridViews.DataSetName = "NewDataSet";
            this.dsGridViews.Tables.AddRange(new System.Data.DataTable[] {
            this.dtSegmClasses});
            // 
            // dtSegmClasses
            // 
            this.dtSegmClasses.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn3,
            this.dataColumn2,
            this.dataColumn4});
            this.dtSegmClasses.TableName = "SegmClasses";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "ID";
            this.dataColumn1.DataType = typeof(uint);
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Name";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "DrawOrder";
            this.dataColumn2.DataType = typeof(uint);
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "DrawColor";
            this.dataColumn4.DataType = typeof(object);
            // 
            // dgvSegmClasses
            // 
            this.dgvSegmClasses.AutoGenerateColumns = false;
            this.dgvSegmClasses.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvSegmClasses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSegmClasses.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.drawOrderDataGridViewTextBoxColumn,
            this.drawColorDataGridViewTextBoxColumn});
            this.dgvSegmClasses.DataMember = "SegmClasses";
            this.dgvSegmClasses.DataSource = this.dsGridViews;
            this.dgvSegmClasses.Location = new System.Drawing.Point(22, 19);
            this.dgvSegmClasses.Name = "dgvSegmClasses";
            this.dgvSegmClasses.RowHeadersVisible = false;
            this.dgvSegmClasses.Size = new System.Drawing.Size(440, 543);
            this.dgvSegmClasses.TabIndex = 6;
            this.dgvSegmClasses.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSegmClasses_CellClick);
            this.dgvSegmClasses.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvSegmClasses_CellFormatting);
            this.dgvSegmClasses.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvSegmClasses_CellPainting);
            this.dgvSegmClasses.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvSegmClasses_DataError);
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.DataPropertyName = "DrawColor";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle2.NullValue")));
            this.dataGridViewImageColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewImageColumn1.HeaderText = "DrawColor";
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewImageColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "DrawColor";
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Transparent;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn1.HeaderText = "DrawColor";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.Width = 80;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgvSegmClasses);
            this.groupBox5.Location = new System.Drawing.Point(10, 260);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(483, 573);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "SegmClass Definition";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.ckbBrightnessContrastExclusive);
            this.groupBox6.Controls.Add(this.ckbTiltWithZoomOnly);
            this.groupBox6.Controls.Add(this.nudMaskDrawOrderMax);
            this.groupBox6.Controls.Add(this.nudMaskDrawOrderMin);
            this.groupBox6.Controls.Add(this.label28);
            this.groupBox6.Controls.Add(this.label29);
            this.groupBox6.Controls.Add(this.tbContrastEnhancement);
            this.groupBox6.Controls.Add(this.label27);
            this.groupBox6.Controls.Add(this.nudImageOutputSizeHeight);
            this.groupBox6.Controls.Add(this.nudImageOutputSizeWidth);
            this.groupBox6.Controls.Add(this.nudTrainValRatio);
            this.groupBox6.Controls.Add(this.tbNoiseAdders);
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.tbBrightnessFactors);
            this.groupBox6.Controls.Add(this.label18);
            this.groupBox6.Controls.Add(this.tbTiltAngles);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.tbZoomFactors);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Location = new System.Drawing.Point(499, 260);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(430, 338);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Output Parameter";
            // 
            // ckbBrightnessContrastExclusive
            // 
            this.ckbBrightnessContrastExclusive.AutoSize = true;
            this.ckbBrightnessContrastExclusive.Location = new System.Drawing.Point(138, 172);
            this.ckbBrightnessContrastExclusive.Name = "ckbBrightnessContrastExclusive";
            this.ckbBrightnessContrastExclusive.Size = new System.Drawing.Size(165, 17);
            this.ckbBrightnessContrastExclusive.TabIndex = 28;
            this.ckbBrightnessContrastExclusive.Text = "Brightness Contrast Exclusive";
            this.ckbBrightnessContrastExclusive.UseVisualStyleBackColor = true;
            // 
            // ckbTiltWithZoomOnly
            // 
            this.ckbTiltWithZoomOnly.AutoSize = true;
            this.ckbTiltWithZoomOnly.Location = new System.Drawing.Point(138, 149);
            this.ckbTiltWithZoomOnly.Name = "ckbTiltWithZoomOnly";
            this.ckbTiltWithZoomOnly.Size = new System.Drawing.Size(119, 17);
            this.ckbTiltWithZoomOnly.TabIndex = 27;
            this.ckbTiltWithZoomOnly.Text = "Tilt With Zoom Only";
            this.ckbTiltWithZoomOnly.UseVisualStyleBackColor = true;
            // 
            // nudMaskDrawOrderMax
            // 
            this.nudMaskDrawOrderMax.Location = new System.Drawing.Point(345, 71);
            this.nudMaskDrawOrderMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMaskDrawOrderMax.Name = "nudMaskDrawOrderMax";
            this.nudMaskDrawOrderMax.Size = new System.Drawing.Size(62, 20);
            this.nudMaskDrawOrderMax.TabIndex = 26;
            this.nudMaskDrawOrderMax.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudMaskDrawOrderMax.Validating += new System.ComponentModel.CancelEventHandler(this.nudMaskDrawOrderMax_Validating);
            // 
            // nudMaskDrawOrderMin
            // 
            this.nudMaskDrawOrderMin.Location = new System.Drawing.Point(136, 71);
            this.nudMaskDrawOrderMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudMaskDrawOrderMin.Name = "nudMaskDrawOrderMin";
            this.nudMaskDrawOrderMin.Size = new System.Drawing.Size(62, 20);
            this.nudMaskDrawOrderMin.TabIndex = 25;
            this.nudMaskDrawOrderMin.Validating += new System.ComponentModel.CancelEventHandler(this.nudMaskDrawOrderMin_Validating);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(223, 74);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(116, 13);
            this.label28.TabIndex = 24;
            this.label28.Text = "Mask Draw Order Max:";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(19, 74);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(110, 13);
            this.label29.TabIndex = 23;
            this.label29.Text = "Mask Draw Order Min";
            // 
            // tbContrastEnhancement
            // 
            this.tbContrastEnhancement.Location = new System.Drawing.Point(136, 281);
            this.tbContrastEnhancement.Name = "tbContrastEnhancement";
            this.tbContrastEnhancement.Size = new System.Drawing.Size(271, 20);
            this.tbContrastEnhancement.TabIndex = 22;
            this.tbContrastEnhancement.Validating += new System.ComponentModel.CancelEventHandler(this.tbContrastEnhancement_Validating);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(19, 284);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(118, 13);
            this.label27.TabIndex = 21;
            this.label27.Text = "Contrast Enhancement:";
            // 
            // nudImageOutputSizeHeight
            // 
            this.nudImageOutputSizeHeight.Location = new System.Drawing.Point(345, 19);
            this.nudImageOutputSizeHeight.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudImageOutputSizeHeight.Minimum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudImageOutputSizeHeight.Name = "nudImageOutputSizeHeight";
            this.nudImageOutputSizeHeight.Size = new System.Drawing.Size(62, 20);
            this.nudImageOutputSizeHeight.TabIndex = 20;
            this.nudImageOutputSizeHeight.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // nudImageOutputSizeWidth
            // 
            this.nudImageOutputSizeWidth.Location = new System.Drawing.Point(136, 19);
            this.nudImageOutputSizeWidth.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.nudImageOutputSizeWidth.Minimum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.nudImageOutputSizeWidth.Name = "nudImageOutputSizeWidth";
            this.nudImageOutputSizeWidth.Size = new System.Drawing.Size(62, 20);
            this.nudImageOutputSizeWidth.TabIndex = 19;
            this.nudImageOutputSizeWidth.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // nudTrainValRatio
            // 
            this.nudTrainValRatio.Location = new System.Drawing.Point(136, 45);
            this.nudTrainValRatio.Name = "nudTrainValRatio";
            this.nudTrainValRatio.Size = new System.Drawing.Size(62, 20);
            this.nudTrainValRatio.TabIndex = 18;
            // 
            // tbNoiseAdders
            // 
            this.tbNoiseAdders.Location = new System.Drawing.Point(136, 307);
            this.tbNoiseAdders.Name = "tbNoiseAdders";
            this.tbNoiseAdders.Size = new System.Drawing.Size(271, 20);
            this.tbNoiseAdders.TabIndex = 14;
            this.tbNoiseAdders.Validating += new System.ComponentModel.CancelEventHandler(this.tbNoiseAdders_Validating);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(19, 310);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(73, 13);
            this.label19.TabIndex = 13;
            this.label19.Text = "Noise Adders:";
            // 
            // tbBrightnessFactors
            // 
            this.tbBrightnessFactors.Location = new System.Drawing.Point(136, 255);
            this.tbBrightnessFactors.Name = "tbBrightnessFactors";
            this.tbBrightnessFactors.Size = new System.Drawing.Size(271, 20);
            this.tbBrightnessFactors.TabIndex = 12;
            this.tbBrightnessFactors.Validating += new System.ComponentModel.CancelEventHandler(this.tbBrightnessFactors_Validating);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(19, 258);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(97, 13);
            this.label18.TabIndex = 11;
            this.label18.Text = "Brightness Factors:";
            // 
            // tbTiltAngles
            // 
            this.tbTiltAngles.Location = new System.Drawing.Point(136, 229);
            this.tbTiltAngles.Name = "tbTiltAngles";
            this.tbTiltAngles.Size = new System.Drawing.Size(271, 20);
            this.tbTiltAngles.TabIndex = 10;
            this.tbTiltAngles.Validating += new System.ComponentModel.CancelEventHandler(this.tbTiltAngles_Validating);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(19, 232);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(59, 13);
            this.label17.TabIndex = 9;
            this.label17.Text = "Tilt Angles:";
            // 
            // tbZoomFactors
            // 
            this.tbZoomFactors.Location = new System.Drawing.Point(136, 203);
            this.tbZoomFactors.Name = "tbZoomFactors";
            this.tbZoomFactors.Size = new System.Drawing.Size(271, 20);
            this.tbZoomFactors.TabIndex = 8;
            this.tbZoomFactors.Validating += new System.ComponentModel.CancelEventHandler(this.tbZoomFactors_Validating);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(19, 206);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(75, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Zoom Factors:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(19, 127);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(107, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Image Augmentation:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(19, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Train/Val Ratio:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(228, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Image Height:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Image Width:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.nudPolygonPointSize);
            this.groupBox7.Controls.Add(this.label26);
            this.groupBox7.Controls.Add(this.nudScrollStartMinCount);
            this.groupBox7.Controls.Add(this.nudScrollMoveFactor);
            this.groupBox7.Controls.Add(this.nudScrollZoneSize);
            this.groupBox7.Controls.Add(this.nudDrawMaskTransparency);
            this.groupBox7.Controls.Add(this.label25);
            this.groupBox7.Controls.Add(this.label24);
            this.groupBox7.Controls.Add(this.label23);
            this.groupBox7.Controls.Add(this.btnPolygonPointColor);
            this.groupBox7.Controls.Add(this.label22);
            this.groupBox7.Controls.Add(this.btnPolygonLineColor);
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Location = new System.Drawing.Point(499, 604);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(430, 229);
            this.groupBox7.TabIndex = 9;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "GUI Parameter";
            // 
            // nudPolygonPointSize
            // 
            this.nudPolygonPointSize.DecimalPlaces = 1;
            this.nudPolygonPointSize.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudPolygonPointSize.Location = new System.Drawing.Point(195, 70);
            this.nudPolygonPointSize.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPolygonPointSize.Name = "nudPolygonPointSize";
            this.nudPolygonPointSize.Size = new System.Drawing.Size(62, 20);
            this.nudPolygonPointSize.TabIndex = 21;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(19, 72);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(98, 13);
            this.label26.TabIndex = 20;
            this.label26.Text = "Polygon Point Size:";
            // 
            // nudScrollStartMinCount
            // 
            this.nudScrollStartMinCount.Location = new System.Drawing.Point(195, 198);
            this.nudScrollStartMinCount.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudScrollStartMinCount.Name = "nudScrollStartMinCount";
            this.nudScrollStartMinCount.Size = new System.Drawing.Size(62, 20);
            this.nudScrollStartMinCount.TabIndex = 19;
            // 
            // nudScrollMoveFactor
            // 
            this.nudScrollMoveFactor.DecimalPlaces = 2;
            this.nudScrollMoveFactor.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScrollMoveFactor.Location = new System.Drawing.Point(195, 172);
            this.nudScrollMoveFactor.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudScrollMoveFactor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScrollMoveFactor.Name = "nudScrollMoveFactor";
            this.nudScrollMoveFactor.Size = new System.Drawing.Size(62, 20);
            this.nudScrollMoveFactor.TabIndex = 18;
            this.nudScrollMoveFactor.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // nudScrollZoneSize
            // 
            this.nudScrollZoneSize.DecimalPlaces = 1;
            this.nudScrollZoneSize.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScrollZoneSize.Location = new System.Drawing.Point(195, 146);
            this.nudScrollZoneSize.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudScrollZoneSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudScrollZoneSize.Name = "nudScrollZoneSize";
            this.nudScrollZoneSize.Size = new System.Drawing.Size(62, 20);
            this.nudScrollZoneSize.TabIndex = 17;
            this.nudScrollZoneSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // nudDrawMaskTransparency
            // 
            this.nudDrawMaskTransparency.Location = new System.Drawing.Point(195, 108);
            this.nudDrawMaskTransparency.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudDrawMaskTransparency.Name = "nudDrawMaskTransparency";
            this.nudDrawMaskTransparency.Size = new System.Drawing.Size(62, 20);
            this.nudDrawMaskTransparency.TabIndex = 16;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(19, 200);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(112, 13);
            this.label25.TabIndex = 14;
            this.label25.Text = "Scroll Start Min Count:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(19, 174);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(99, 13);
            this.label24.TabIndex = 12;
            this.label24.Text = "Scroll Move Factor:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(19, 110);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(129, 13);
            this.label23.TabIndex = 11;
            this.label23.Text = "Draw Mask Transparency";
            // 
            // btnPolygonPointColor
            // 
            this.btnPolygonPointColor.Location = new System.Drawing.Point(195, 44);
            this.btnPolygonPointColor.Name = "btnPolygonPointColor";
            this.btnPolygonPointColor.Size = new System.Drawing.Size(62, 20);
            this.btnPolygonPointColor.TabIndex = 10;
            this.btnPolygonPointColor.UseVisualStyleBackColor = true;
            this.btnPolygonPointColor.Click += new System.EventHandler(this.btnPolygonPointColor_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(19, 48);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(102, 13);
            this.label22.TabIndex = 9;
            this.label22.Text = "Polygon Point Color:";
            // 
            // btnPolygonLineColor
            // 
            this.btnPolygonLineColor.Location = new System.Drawing.Point(195, 18);
            this.btnPolygonLineColor.Name = "btnPolygonLineColor";
            this.btnPolygonLineColor.Size = new System.Drawing.Size(62, 20);
            this.btnPolygonLineColor.TabIndex = 8;
            this.btnPolygonLineColor.UseVisualStyleBackColor = true;
            this.btnPolygonLineColor.Click += new System.EventHandler(this.btnPolygonLineColor_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(19, 148);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(87, 13);
            this.label20.TabIndex = 6;
            this.label20.Text = "Scroll Zone Size:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(19, 22);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(98, 13);
            this.label21.TabIndex = 4;
            this.label21.Text = "Polygon Line Color:";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "DrawColor";
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn2.HeaderText = "DrawColor";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn2.Width = 80;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "DrawColor";
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn3.HeaderText = "DrawColor";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "DrawColor";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn4.HeaderText = "DrawColor";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "DrawColor";
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn5.HeaderText = "DrawColor";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn5.Width = 80;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Width = 50;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // drawOrderDataGridViewTextBoxColumn
            // 
            this.drawOrderDataGridViewTextBoxColumn.DataPropertyName = "DrawOrder";
            this.drawOrderDataGridViewTextBoxColumn.HeaderText = "DrawOrder";
            this.drawOrderDataGridViewTextBoxColumn.Name = "drawOrderDataGridViewTextBoxColumn";
            this.drawOrderDataGridViewTextBoxColumn.Width = 80;
            // 
            // drawColorDataGridViewTextBoxColumn
            // 
            this.drawColorDataGridViewTextBoxColumn.DataPropertyName = "DrawColor";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.drawColorDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.drawColorDataGridViewTextBoxColumn.HeaderText = "DrawColor";
            this.drawColorDataGridViewTextBoxColumn.Name = "drawColorDataGridViewTextBoxColumn";
            this.drawColorDataGridViewTextBoxColumn.ReadOnly = true;
            this.drawColorDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.drawColorDataGridViewTextBoxColumn.Width = 80;
            // 
            // frmAppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(941, 891);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAppSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsGridViews)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtSegmClasses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSegmClasses)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskDrawOrderMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaskDrawOrderMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageOutputSizeHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudImageOutputSizeWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTrainValRatio)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPolygonPointSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollStartMinCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollMoveFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScrollZoneSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDrawMaskTransparency)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowsePathToSourceImages;
        private System.Windows.Forms.TextBox tbPathToSourceImages;
        private System.Windows.Forms.FolderBrowserDialog fbdSelectPath;
        private System.Windows.Forms.Button btnBrowsePathToOutputDataset;
        private System.Windows.Forms.TextBox tbPathToOutputDatset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnBrowsePathToSessionData;
        private System.Windows.Forms.TextBox tbPathToSessionData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSubDirImg;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSubDirInfo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbSubDirMask;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbSubDirVal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSubDirTrain;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbPrefixInfo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPrefixMask;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbPrefixImg;
        private System.Windows.Forms.Label label11;
        private System.Data.DataSet dsGridViews;
        private System.Data.DataTable dtSegmClasses;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn4;
        private System.Windows.Forms.DataGridView dgvSegmClasses;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tbTiltAngles;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbZoomFactors;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox tbBrightnessFactors;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbNoiseAdders;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnPolygonPointColor;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btnPolygonLineColor;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.NumericUpDown nudImageOutputSizeHeight;
        private System.Windows.Forms.NumericUpDown nudImageOutputSizeWidth;
        private System.Windows.Forms.NumericUpDown nudTrainValRatio;
        private System.Windows.Forms.NumericUpDown nudScrollStartMinCount;
        private System.Windows.Forms.NumericUpDown nudScrollMoveFactor;
        private System.Windows.Forms.NumericUpDown nudScrollZoneSize;
        private System.Windows.Forms.NumericUpDown nudDrawMaskTransparency;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.NumericUpDown nudPolygonPointSize;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbContrastEnhancement;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.NumericUpDown nudMaskDrawOrderMax;
        private System.Windows.Forms.NumericUpDown nudMaskDrawOrderMin;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Button btnBrowsePathToPredictedMasks;
        private System.Windows.Forms.TextBox tbPathToPredictedMasks;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox ckbTiltWithZoomOnly;
        private System.Windows.Forms.CheckBox ckbBrightnessContrastExclusive;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn drawOrderDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn drawColorDataGridViewTextBoxColumn;
    }
}