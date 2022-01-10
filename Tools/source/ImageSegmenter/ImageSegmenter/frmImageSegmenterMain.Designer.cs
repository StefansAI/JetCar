namespace ImageSegmenter
{
    partial class frmImageSegmenterMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImageSegmenterMain));
            this.msMainMenu = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveCurrentState = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetttings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditAppSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAutoLoadPredictedMask = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProcess = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGotoFirstProcessedImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiGotoLastProcessedImage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveLastProcessed = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAugmentationPanel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiProcessAllImageAugmentation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrevious = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNext = new System.Windows.Forms.ToolStripMenuItem();
            this.pnControl = new System.Windows.Forms.Panel();
            this.ckbShowSteeringAngles = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.ckbAutoToggleMasks = new System.Windows.Forms.CheckBox();
            this.ckbShowPredictionMask = new System.Windows.Forms.CheckBox();
            this.nudStepSize = new System.Windows.Forms.NumericUpDown();
            this.nudWindowSize = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.lbCurrentImageIdx = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ckbEdgeSnap = new System.Windows.Forms.CheckBox();
            this.ckbEditPolygonAutoUpdateMask = new System.Windows.Forms.CheckBox();
            this.ckbSyncLeftRight = new System.Windows.Forms.CheckBox();
            this.ckbRightShowOverlap = new System.Windows.Forms.CheckBox();
            this.lbRemainingCount = new System.Windows.Forms.Label();
            this.lbProcessedCount = new System.Windows.Forms.Label();
            this.lbInputFileName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDeleteSegmClass = new System.Windows.Forms.Button();
            this.cbCurrentSegmClasses = new System.Windows.Forms.ComboBox();
            this.btnEditPolygon = new System.Windows.Forms.Button();
            this.ckbRightShowSelectedOnly = new System.Windows.Forms.CheckBox();
            this.lbCursorValuesRight = new System.Windows.Forms.Label();
            this.ckbLeftImageTargetSize = new System.Windows.Forms.CheckBox();
            this.btnFinishSegmClass = new System.Windows.Forms.Button();
            this.ckbLeftDrawActivePolygonOnly = new System.Windows.Forms.CheckBox();
            this.btnNewPolygon = new System.Windows.Forms.Button();
            this.cbSegmClass = new System.Windows.Forms.ComboBox();
            this.lbCursorValuesLeft = new System.Windows.Forms.Label();
            this.pnLeft = new System.Windows.Forms.Panel();
            this.pbLeft = new System.Windows.Forms.PixelPictureBox();
            this.pnRight = new System.Windows.Forms.Panel();
            this.pbRight = new System.Windows.Forms.PixelPictureBox();
            this.pnMarginLeft = new System.Windows.Forms.Panel();
            this.pnMarginRight = new System.Windows.Forms.Panel();
            this.tmAutoMaskUpdateTrigger = new System.Windows.Forms.Timer(this.components);
            this.tmAutoMaskUpdateClear = new System.Windows.Forms.Timer(this.components);
            this.tmToggleMasks = new System.Windows.Forms.Timer(this.components);
            this.msMainMenu.SuspendLayout();
            this.pnControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStepSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWindowSize)).BeginInit();
            this.pnLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
            this.pnRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).BeginInit();
            this.SuspendLayout();
            // 
            // msMainMenu
            // 
            this.msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiSetttings,
            this.tsmiProcess,
            this.tsmiPrevious,
            this.tsmiNext});
            this.msMainMenu.Location = new System.Drawing.Point(0, 0);
            this.msMainMenu.Name = "msMainMenu";
            this.msMainMenu.Size = new System.Drawing.Size(1884, 24);
            this.msMainMenu.TabIndex = 0;
            this.msMainMenu.Text = "menuStrip1";
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSaveCurrentState,
            this.toolStripMenuItem1,
            this.tsmiExit});
            this.tsmiFile.Name = "tsmiFile";
            this.tsmiFile.Size = new System.Drawing.Size(37, 20);
            this.tsmiFile.Text = "File";
            // 
            // tsmiSaveCurrentState
            // 
            this.tsmiSaveCurrentState.Name = "tsmiSaveCurrentState";
            this.tsmiSaveCurrentState.Size = new System.Drawing.Size(170, 22);
            this.tsmiSaveCurrentState.Text = "Save Current State";
            this.tsmiSaveCurrentState.Click += new System.EventHandler(this.tsmiSaveCurrentState_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(167, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(170, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // tsmiSetttings
            // 
            this.tsmiSetttings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEditAppSettings,
            this.toolStripMenuItem2,
            this.tsmiAutoLoadPredictedMask});
            this.tsmiSetttings.Name = "tsmiSetttings";
            this.tsmiSetttings.Size = new System.Drawing.Size(65, 20);
            this.tsmiSetttings.Text = "Setttings";
            // 
            // tsmiEditAppSettings
            // 
            this.tsmiEditAppSettings.Name = "tsmiEditAppSettings";
            this.tsmiEditAppSettings.Size = new System.Drawing.Size(213, 22);
            this.tsmiEditAppSettings.Text = "Edit App Settings";
            this.tsmiEditAppSettings.Click += new System.EventHandler(this.tsmiEditAppSettings_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(210, 6);
            // 
            // tsmiAutoLoadPredictedMask
            // 
            this.tsmiAutoLoadPredictedMask.CheckOnClick = true;
            this.tsmiAutoLoadPredictedMask.Name = "tsmiAutoLoadPredictedMask";
            this.tsmiAutoLoadPredictedMask.Size = new System.Drawing.Size(213, 22);
            this.tsmiAutoLoadPredictedMask.Text = "Auto Load Predicted Mask";
            this.tsmiAutoLoadPredictedMask.Click += new System.EventHandler(this.tsmiAutoLoadPredictedMask_Click);
            // 
            // tsmiProcess
            // 
            this.tsmiProcess.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiGotoFirstProcessedImage,
            this.tsmiGotoLastProcessedImage,
            this.tsmiRemoveLastProcessed,
            this.toolStripMenuItem5,
            this.tsmiAugmentationPanel,
            this.toolStripMenuItem4,
            this.tsmiProcessAllImageAugmentation});
            this.tsmiProcess.Name = "tsmiProcess";
            this.tsmiProcess.Size = new System.Drawing.Size(59, 20);
            this.tsmiProcess.Text = "Process";
            // 
            // tsmiGotoFirstProcessedImage
            // 
            this.tsmiGotoFirstProcessedImage.Name = "tsmiGotoFirstProcessedImage";
            this.tsmiGotoFirstProcessedImage.Size = new System.Drawing.Size(245, 22);
            this.tsmiGotoFirstProcessedImage.Text = "Goto First Processed Image";
            this.tsmiGotoFirstProcessedImage.Click += new System.EventHandler(this.tsmiGotoFirstProcessedImage_Click);
            // 
            // tsmiGotoLastProcessedImage
            // 
            this.tsmiGotoLastProcessedImage.Name = "tsmiGotoLastProcessedImage";
            this.tsmiGotoLastProcessedImage.Size = new System.Drawing.Size(245, 22);
            this.tsmiGotoLastProcessedImage.Text = "Goto Last Processed Image";
            this.tsmiGotoLastProcessedImage.Click += new System.EventHandler(this.tsmiGotoLastProcessedImage_Click);
            // 
            // tsmiRemoveLastProcessed
            // 
            this.tsmiRemoveLastProcessed.Name = "tsmiRemoveLastProcessed";
            this.tsmiRemoveLastProcessed.Size = new System.Drawing.Size(245, 22);
            this.tsmiRemoveLastProcessed.Text = "Remove Last Processed";
            this.tsmiRemoveLastProcessed.Click += new System.EventHandler(this.tsmiRemoveLastProcessed_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(242, 6);
            // 
            // tsmiAugmentationPanel
            // 
            this.tsmiAugmentationPanel.Name = "tsmiAugmentationPanel";
            this.tsmiAugmentationPanel.Size = new System.Drawing.Size(245, 22);
            this.tsmiAugmentationPanel.Text = "Augmentation Panel";
            this.tsmiAugmentationPanel.Click += new System.EventHandler(this.tsmiAugmentationPanel_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(242, 6);
            // 
            // tsmiProcessAllImageAugmentation
            // 
            this.tsmiProcessAllImageAugmentation.Name = "tsmiProcessAllImageAugmentation";
            this.tsmiProcessAllImageAugmentation.Size = new System.Drawing.Size(245, 22);
            this.tsmiProcessAllImageAugmentation.Text = "Process all Image Augmentation";
            this.tsmiProcessAllImageAugmentation.Click += new System.EventHandler(this.tsmiProcessAllImageAugmentation_Click);
            // 
            // tsmiPrevious
            // 
            this.tsmiPrevious.Name = "tsmiPrevious";
            this.tsmiPrevious.Size = new System.Drawing.Size(64, 20);
            this.tsmiPrevious.Text = "Previous";
            this.tsmiPrevious.Click += new System.EventHandler(this.tsmiPrevious_Click);
            // 
            // tsmiNext
            // 
            this.tsmiNext.Name = "tsmiNext";
            this.tsmiNext.Size = new System.Drawing.Size(44, 20);
            this.tsmiNext.Text = "Next";
            this.tsmiNext.Click += new System.EventHandler(this.tsmiNext_Click);
            // 
            // pnControl
            // 
            this.pnControl.Controls.Add(this.ckbShowSteeringAngles);
            this.pnControl.Controls.Add(this.button1);
            this.pnControl.Controls.Add(this.ckbAutoToggleMasks);
            this.pnControl.Controls.Add(this.ckbShowPredictionMask);
            this.pnControl.Controls.Add(this.nudStepSize);
            this.pnControl.Controls.Add(this.nudWindowSize);
            this.pnControl.Controls.Add(this.button2);
            this.pnControl.Controls.Add(this.lbCurrentImageIdx);
            this.pnControl.Controls.Add(this.label5);
            this.pnControl.Controls.Add(this.ckbEdgeSnap);
            this.pnControl.Controls.Add(this.ckbEditPolygonAutoUpdateMask);
            this.pnControl.Controls.Add(this.ckbSyncLeftRight);
            this.pnControl.Controls.Add(this.ckbRightShowOverlap);
            this.pnControl.Controls.Add(this.lbRemainingCount);
            this.pnControl.Controls.Add(this.lbProcessedCount);
            this.pnControl.Controls.Add(this.lbInputFileName);
            this.pnControl.Controls.Add(this.label3);
            this.pnControl.Controls.Add(this.label2);
            this.pnControl.Controls.Add(this.label1);
            this.pnControl.Controls.Add(this.btnDeleteSegmClass);
            this.pnControl.Controls.Add(this.cbCurrentSegmClasses);
            this.pnControl.Controls.Add(this.btnEditPolygon);
            this.pnControl.Controls.Add(this.ckbRightShowSelectedOnly);
            this.pnControl.Controls.Add(this.lbCursorValuesRight);
            this.pnControl.Controls.Add(this.ckbLeftImageTargetSize);
            this.pnControl.Controls.Add(this.btnFinishSegmClass);
            this.pnControl.Controls.Add(this.ckbLeftDrawActivePolygonOnly);
            this.pnControl.Controls.Add(this.btnNewPolygon);
            this.pnControl.Controls.Add(this.cbSegmClass);
            this.pnControl.Controls.Add(this.lbCursorValuesLeft);
            this.pnControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnControl.Location = new System.Drawing.Point(0, 920);
            this.pnControl.Name = "pnControl";
            this.pnControl.Size = new System.Drawing.Size(1884, 83);
            this.pnControl.TabIndex = 1;
            // 
            // ckbShowSteeringAngles
            // 
            this.ckbShowSteeringAngles.AutoSize = true;
            this.ckbShowSteeringAngles.Location = new System.Drawing.Point(113, 30);
            this.ckbShowSteeringAngles.Name = "ckbShowSteeringAngles";
            this.ckbShowSteeringAngles.Size = new System.Drawing.Size(95, 17);
            this.ckbShowSteeringAngles.TabIndex = 54;
            this.ckbShowSteeringAngles.Text = "Show Steering";
            this.ckbShowSteeringAngles.UseVisualStyleBackColor = true;
            this.ckbShowSteeringAngles.CheckedChanged += new System.EventHandler(this.ckbShowSteeringAngles_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1722, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 53;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ckbAutoToggleMasks
            // 
            this.ckbAutoToggleMasks.AutoSize = true;
            this.ckbAutoToggleMasks.Checked = true;
            this.ckbAutoToggleMasks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbAutoToggleMasks.Location = new System.Drawing.Point(1501, 57);
            this.ckbAutoToggleMasks.Name = "ckbAutoToggleMasks";
            this.ckbAutoToggleMasks.Size = new System.Drawing.Size(118, 17);
            this.ckbAutoToggleMasks.TabIndex = 52;
            this.ckbAutoToggleMasks.Text = "Auto Toggle Masks";
            this.ckbAutoToggleMasks.UseVisualStyleBackColor = true;
            this.ckbAutoToggleMasks.CheckedChanged += new System.EventHandler(this.ckbAutoToggleMasks_CheckedChanged);
            // 
            // ckbShowPredictionMask
            // 
            this.ckbShowPredictionMask.AutoSize = true;
            this.ckbShowPredictionMask.Checked = true;
            this.ckbShowPredictionMask.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbShowPredictionMask.Location = new System.Drawing.Point(1501, 32);
            this.ckbShowPredictionMask.Name = "ckbShowPredictionMask";
            this.ckbShowPredictionMask.Size = new System.Drawing.Size(132, 17);
            this.ckbShowPredictionMask.TabIndex = 51;
            this.ckbShowPredictionMask.Text = "Show Prediction Mask";
            this.ckbShowPredictionMask.UseVisualStyleBackColor = true;
            this.ckbShowPredictionMask.CheckedChanged += new System.EventHandler(this.ckbShowPredictionMask_CheckedChanged);
            // 
            // nudStepSize
            // 
            this.nudStepSize.Location = new System.Drawing.Point(1803, 58);
            this.nudStepSize.Name = "nudStepSize";
            this.nudStepSize.Size = new System.Drawing.Size(74, 20);
            this.nudStepSize.TabIndex = 28;
            this.nudStepSize.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudStepSize.Visible = false;
            // 
            // nudWindowSize
            // 
            this.nudWindowSize.Location = new System.Drawing.Point(1803, 29);
            this.nudWindowSize.Name = "nudWindowSize";
            this.nudWindowSize.Size = new System.Drawing.Size(74, 20);
            this.nudWindowSize.TabIndex = 50;
            this.nudWindowSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudWindowSize.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1722, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 26;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lbCurrentImageIdx
            // 
            this.lbCurrentImageIdx.AutoSize = true;
            this.lbCurrentImageIdx.Location = new System.Drawing.Point(69, 31);
            this.lbCurrentImageIdx.Name = "lbCurrentImageIdx";
            this.lbCurrentImageIdx.Size = new System.Drawing.Size(10, 13);
            this.lbCurrentImageIdx.TabIndex = 24;
            this.lbCurrentImageIdx.Text = "-";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Image Idx:";
            // 
            // ckbEdgeSnap
            // 
            this.ckbEdgeSnap.AutoSize = true;
            this.ckbEdgeSnap.Checked = true;
            this.ckbEdgeSnap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbEdgeSnap.Location = new System.Drawing.Point(847, 2);
            this.ckbEdgeSnap.Name = "ckbEdgeSnap";
            this.ckbEdgeSnap.Size = new System.Drawing.Size(79, 17);
            this.ckbEdgeSnap.TabIndex = 22;
            this.ckbEdgeSnap.Text = "Edge Snap";
            this.ckbEdgeSnap.UseVisualStyleBackColor = true;
            // 
            // ckbEditPolygonAutoUpdateMask
            // 
            this.ckbEditPolygonAutoUpdateMask.AutoSize = true;
            this.ckbEditPolygonAutoUpdateMask.Checked = true;
            this.ckbEditPolygonAutoUpdateMask.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbEditPolygonAutoUpdateMask.Location = new System.Drawing.Point(1005, 57);
            this.ckbEditPolygonAutoUpdateMask.Name = "ckbEditPolygonAutoUpdateMask";
            this.ckbEditPolygonAutoUpdateMask.Size = new System.Drawing.Size(177, 17);
            this.ckbEditPolygonAutoUpdateMask.TabIndex = 21;
            this.ckbEditPolygonAutoUpdateMask.Text = "Edit Polygon Auto Update Mask";
            this.ckbEditPolygonAutoUpdateMask.UseVisualStyleBackColor = true;
            // 
            // ckbSyncLeftRight
            // 
            this.ckbSyncLeftRight.AutoSize = true;
            this.ckbSyncLeftRight.Checked = true;
            this.ckbSyncLeftRight.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbSyncLeftRight.Location = new System.Drawing.Point(1005, 30);
            this.ckbSyncLeftRight.Name = "ckbSyncLeftRight";
            this.ckbSyncLeftRight.Size = new System.Drawing.Size(108, 17);
            this.ckbSyncLeftRight.TabIndex = 20;
            this.ckbSyncLeftRight.Text = "Sync Left && Right";
            this.ckbSyncLeftRight.UseVisualStyleBackColor = true;
            // 
            // ckbRightShowOverlap
            // 
            this.ckbRightShowOverlap.AutoSize = true;
            this.ckbRightShowOverlap.Location = new System.Drawing.Point(1295, 57);
            this.ckbRightShowOverlap.Name = "ckbRightShowOverlap";
            this.ckbRightShowOverlap.Size = new System.Drawing.Size(91, 17);
            this.ckbRightShowOverlap.TabIndex = 19;
            this.ckbRightShowOverlap.Text = "Show overlap";
            this.ckbRightShowOverlap.UseVisualStyleBackColor = true;
            this.ckbRightShowOverlap.CheckedChanged += new System.EventHandler(this.ckbRightShowOverlap_CheckedChanged);
            // 
            // lbRemainingCount
            // 
            this.lbRemainingCount.AutoSize = true;
            this.lbRemainingCount.Location = new System.Drawing.Point(176, 58);
            this.lbRemainingCount.Name = "lbRemainingCount";
            this.lbRemainingCount.Size = new System.Drawing.Size(10, 13);
            this.lbRemainingCount.TabIndex = 18;
            this.lbRemainingCount.Text = "-";
            // 
            // lbProcessedCount
            // 
            this.lbProcessedCount.AutoSize = true;
            this.lbProcessedCount.Location = new System.Drawing.Point(69, 58);
            this.lbProcessedCount.Name = "lbProcessedCount";
            this.lbProcessedCount.Size = new System.Drawing.Size(10, 13);
            this.lbProcessedCount.TabIndex = 17;
            this.lbProcessedCount.Text = "-";
            // 
            // lbInputFileName
            // 
            this.lbInputFileName.AutoSize = true;
            this.lbInputFileName.Location = new System.Drawing.Point(69, 6);
            this.lbInputFileName.Name = "lbInputFileName";
            this.lbInputFileName.Size = new System.Drawing.Size(10, 13);
            this.lbInputFileName.TabIndex = 16;
            this.lbInputFileName.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Remaining:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Processed:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Input File:";
            // 
            // btnDeleteSegmClass
            // 
            this.btnDeleteSegmClass.Enabled = false;
            this.btnDeleteSegmClass.Location = new System.Drawing.Point(800, 53);
            this.btnDeleteSegmClass.Name = "btnDeleteSegmClass";
            this.btnDeleteSegmClass.Size = new System.Drawing.Size(120, 23);
            this.btnDeleteSegmClass.TabIndex = 10;
            this.btnDeleteSegmClass.Text = "Delete Segm Class";
            this.btnDeleteSegmClass.UseVisualStyleBackColor = true;
            this.btnDeleteSegmClass.Click += new System.EventHandler(this.btnDeleteSegmClass_Click);
            // 
            // cbCurrentSegmClasses
            // 
            this.cbCurrentSegmClasses.FormattingEnabled = true;
            this.cbCurrentSegmClasses.Location = new System.Drawing.Point(670, 28);
            this.cbCurrentSegmClasses.Name = "cbCurrentSegmClasses";
            this.cbCurrentSegmClasses.Size = new System.Drawing.Size(250, 21);
            this.cbCurrentSegmClasses.TabIndex = 9;
            this.cbCurrentSegmClasses.SelectedIndexChanged += new System.EventHandler(this.cbCurrentSegmClasses_SelectedIndexChanged);
            // 
            // btnEditPolygon
            // 
            this.btnEditPolygon.Enabled = false;
            this.btnEditPolygon.Location = new System.Drawing.Point(670, 53);
            this.btnEditPolygon.Name = "btnEditPolygon";
            this.btnEditPolygon.Size = new System.Drawing.Size(120, 23);
            this.btnEditPolygon.TabIndex = 8;
            this.btnEditPolygon.Text = "Edit Polygon";
            this.btnEditPolygon.UseVisualStyleBackColor = true;
            this.btnEditPolygon.Click += new System.EventHandler(this.btnEditPolygon_Click);
            // 
            // ckbRightShowSelectedOnly
            // 
            this.ckbRightShowSelectedOnly.AutoSize = true;
            this.ckbRightShowSelectedOnly.Location = new System.Drawing.Point(1295, 30);
            this.ckbRightShowSelectedOnly.Name = "ckbRightShowSelectedOnly";
            this.ckbRightShowSelectedOnly.Size = new System.Drawing.Size(118, 17);
            this.ckbRightShowSelectedOnly.TabIndex = 7;
            this.ckbRightShowSelectedOnly.Text = "Show selected only";
            this.ckbRightShowSelectedOnly.UseVisualStyleBackColor = true;
            this.ckbRightShowSelectedOnly.CheckedChanged += new System.EventHandler(this.ckbRightShowOnlySelected_CheckedChanged);
            // 
            // lbCursorValuesRight
            // 
            this.lbCursorValuesRight.Location = new System.Drawing.Point(961, 0);
            this.lbCursorValuesRight.Name = "lbCursorValuesRight";
            this.lbCursorValuesRight.Size = new System.Drawing.Size(893, 19);
            this.lbCursorValuesRight.TabIndex = 6;
            this.lbCursorValuesRight.Text = "-";
            this.lbCursorValuesRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ckbLeftImageTargetSize
            // 
            this.ckbLeftImageTargetSize.AutoSize = true;
            this.ckbLeftImageTargetSize.Location = new System.Drawing.Point(232, 57);
            this.ckbLeftImageTargetSize.Name = "ckbLeftImageTargetSize";
            this.ckbLeftImageTargetSize.Size = new System.Drawing.Size(115, 17);
            this.ckbLeftImageTargetSize.TabIndex = 5;
            this.ckbLeftImageTargetSize.Text = "Target Size Bitmap";
            this.ckbLeftImageTargetSize.UseVisualStyleBackColor = true;
            this.ckbLeftImageTargetSize.CheckedChanged += new System.EventHandler(this.ckbLeftImageTargetSize_CheckedChanged);
            // 
            // btnFinishSegmClass
            // 
            this.btnFinishSegmClass.Enabled = false;
            this.btnFinishSegmClass.Location = new System.Drawing.Point(497, 53);
            this.btnFinishSegmClass.Name = "btnFinishSegmClass";
            this.btnFinishSegmClass.Size = new System.Drawing.Size(120, 23);
            this.btnFinishSegmClass.TabIndex = 4;
            this.btnFinishSegmClass.Text = "Finish Segm Class";
            this.btnFinishSegmClass.UseVisualStyleBackColor = true;
            this.btnFinishSegmClass.Click += new System.EventHandler(this.btnFinishSegmClass_Click);
            // 
            // ckbLeftDrawActivePolygonOnly
            // 
            this.ckbLeftDrawActivePolygonOnly.AutoSize = true;
            this.ckbLeftDrawActivePolygonOnly.Location = new System.Drawing.Point(232, 30);
            this.ckbLeftDrawActivePolygonOnly.Name = "ckbLeftDrawActivePolygonOnly";
            this.ckbLeftDrawActivePolygonOnly.Size = new System.Drawing.Size(105, 17);
            this.ckbLeftDrawActivePolygonOnly.TabIndex = 3;
            this.ckbLeftDrawActivePolygonOnly.Text = "Draw active only";
            this.ckbLeftDrawActivePolygonOnly.UseVisualStyleBackColor = true;
            this.ckbLeftDrawActivePolygonOnly.CheckedChanged += new System.EventHandler(this.ckbLeftDrawActivePolygonOnly_CheckedChanged);
            // 
            // btnNewPolygon
            // 
            this.btnNewPolygon.Enabled = false;
            this.btnNewPolygon.Location = new System.Drawing.Point(366, 53);
            this.btnNewPolygon.Name = "btnNewPolygon";
            this.btnNewPolygon.Size = new System.Drawing.Size(120, 23);
            this.btnNewPolygon.TabIndex = 2;
            this.btnNewPolygon.Text = "New Polygon";
            this.btnNewPolygon.UseVisualStyleBackColor = true;
            this.btnNewPolygon.Click += new System.EventHandler(this.btnNewPolygon_Click);
            // 
            // cbSegmClass
            // 
            this.cbSegmClass.FormattingEnabled = true;
            this.cbSegmClass.Location = new System.Drawing.Point(366, 28);
            this.cbSegmClass.Name = "cbSegmClass";
            this.cbSegmClass.Size = new System.Drawing.Size(250, 21);
            this.cbSegmClass.TabIndex = 1;
            // 
            // lbCursorValuesLeft
            // 
            this.lbCursorValuesLeft.Location = new System.Drawing.Point(366, 0);
            this.lbCursorValuesLeft.Name = "lbCursorValuesLeft";
            this.lbCursorValuesLeft.Size = new System.Drawing.Size(465, 19);
            this.lbCursorValuesLeft.TabIndex = 0;
            this.lbCursorValuesLeft.Text = "-";
            this.lbCursorValuesLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnLeft
            // 
            this.pnLeft.Controls.Add(this.pbLeft);
            this.pnLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnLeft.Location = new System.Drawing.Point(30, 24);
            this.pnLeft.Name = "pnLeft";
            this.pnLeft.Size = new System.Drawing.Size(896, 896);
            this.pnLeft.TabIndex = 3;
            // 
            // pbLeft
            // 
            this.pbLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbLeft.Location = new System.Drawing.Point(0, 0);
            this.pbLeft.Name = "pbLeft";
            this.pbLeft.Size = new System.Drawing.Size(896, 896);
            this.pbLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLeft.TabIndex = 1;
            this.pbLeft.TabStop = false;
            this.pbLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pbLeft_Paint);
            this.pbLeft.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseDoubleClick);
            this.pbLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseDown);
            this.pbLeft.MouseLeave += new System.EventHandler(this.pbLeft_MouseLeave);
            this.pbLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseMove);
            this.pbLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbLeft_MouseUp);
            // 
            // pnRight
            // 
            this.pnRight.Controls.Add(this.pbRight);
            this.pnRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnRight.Location = new System.Drawing.Point(958, 24);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(896, 896);
            this.pnRight.TabIndex = 4;
            // 
            // pbRight
            // 
            this.pbRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbRight.Location = new System.Drawing.Point(0, 0);
            this.pbRight.Name = "pbRight";
            this.pbRight.Size = new System.Drawing.Size(896, 896);
            this.pbRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRight.TabIndex = 2;
            this.pbRight.TabStop = false;
            this.pbRight.MouseLeave += new System.EventHandler(this.pbRight_MouseLeave);
            this.pbRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbRight_MouseMove);
            // 
            // pnMarginLeft
            // 
            this.pnMarginLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnMarginLeft.Location = new System.Drawing.Point(0, 24);
            this.pnMarginLeft.Name = "pnMarginLeft";
            this.pnMarginLeft.Size = new System.Drawing.Size(30, 896);
            this.pnMarginLeft.TabIndex = 5;
            // 
            // pnMarginRight
            // 
            this.pnMarginRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnMarginRight.Location = new System.Drawing.Point(1854, 24);
            this.pnMarginRight.Name = "pnMarginRight";
            this.pnMarginRight.Size = new System.Drawing.Size(30, 896);
            this.pnMarginRight.TabIndex = 6;
            // 
            // tmAutoMaskUpdateTrigger
            // 
            this.tmAutoMaskUpdateTrigger.Interval = 250;
            this.tmAutoMaskUpdateTrigger.Tick += new System.EventHandler(this.tmAutoMaskUpdateTrigger_Tick);
            // 
            // tmAutoMaskUpdateClear
            // 
            this.tmAutoMaskUpdateClear.Interval = 1000;
            this.tmAutoMaskUpdateClear.Tick += new System.EventHandler(this.tmAutoMaskUpdateClear_Tick);
            // 
            // tmToggleMasks
            // 
            this.tmToggleMasks.Interval = 1000;
            this.tmToggleMasks.Tick += new System.EventHandler(this.tmToggleMasks_Tick);
            // 
            // frmImageSegmenterMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1884, 1003);
            this.Controls.Add(this.pnRight);
            this.Controls.Add(this.pnLeft);
            this.Controls.Add(this.pnMarginRight);
            this.Controls.Add(this.pnMarginLeft);
            this.Controls.Add(this.pnControl);
            this.Controls.Add(this.msMainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.msMainMenu;
            this.Name = "frmImageSegmenterMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Segmenter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmImageSegmenterMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmImageSegmenterMain_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmImageSegmenterMain_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmImageSegmenterMain_KeyUp);
            this.msMainMenu.ResumeLayout(false);
            this.msMainMenu.PerformLayout();
            this.pnControl.ResumeLayout(false);
            this.pnControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStepSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWindowSize)).EndInit();
            this.pnLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
            this.pnRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.Panel pnControl;
        private System.Windows.Forms.Panel pnLeft;
        
        private System.Windows.Forms.Panel pnRight;
        
        private System.Windows.Forms.Label lbCursorValuesLeft;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetttings;
        private System.Windows.Forms.ToolStripMenuItem tsmiNext;
        private System.Windows.Forms.ComboBox cbSegmClass;
        private System.Windows.Forms.Button btnNewPolygon;
        private System.Windows.Forms.CheckBox ckbLeftDrawActivePolygonOnly;
        private System.Windows.Forms.Button btnFinishSegmClass;
        private System.Windows.Forms.CheckBox ckbLeftImageTargetSize;
        private System.Windows.Forms.ToolStripMenuItem tsmiPrevious;
        private System.Windows.Forms.Label lbCursorValuesRight;
        private System.Windows.Forms.CheckBox ckbRightShowSelectedOnly;
        private System.Windows.Forms.Panel pnMarginLeft;
        private System.Windows.Forms.Panel pnMarginRight;
        private System.Windows.Forms.Button btnEditPolygon;
        private System.Windows.Forms.Button btnDeleteSegmClass;
        private System.Windows.Forms.ComboBox cbCurrentSegmClasses;
        private System.Windows.Forms.Label lbRemainingCount;
        private System.Windows.Forms.Label lbProcessedCount;
        private System.Windows.Forms.Label lbInputFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveCurrentState;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.CheckBox ckbRightShowOverlap;
        private System.Windows.Forms.CheckBox ckbSyncLeftRight;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditAppSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.CheckBox ckbEditPolygonAutoUpdateMask;
        private System.Windows.Forms.Timer tmAutoMaskUpdateTrigger;
        private System.Windows.Forms.Timer tmAutoMaskUpdateClear;
        private System.Windows.Forms.CheckBox ckbEdgeSnap;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem tsmiGotoFirstProcessedImage;
        private System.Windows.Forms.ToolStripMenuItem tsmiGotoLastProcessedImage;
        private System.Windows.Forms.Label lbCurrentImageIdx;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripMenuItem tsmiProcess;
        private System.Windows.Forms.ToolStripMenuItem tsmiAugmentationPanel;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tsmiProcessAllImageAugmentation;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveLastProcessed;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.NumericUpDown nudWindowSize;
        private System.Windows.Forms.NumericUpDown nudStepSize;
        private System.Windows.Forms.PixelPictureBox pbLeft;
        private System.Windows.Forms.PixelPictureBox pbRight;
        private System.Windows.Forms.ToolStripMenuItem tsmiAutoLoadPredictedMask;
        private System.Windows.Forms.CheckBox ckbShowPredictionMask;
        private System.Windows.Forms.Timer tmToggleMasks;
        private System.Windows.Forms.CheckBox ckbAutoToggleMasks;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox ckbShowSteeringAngles;
    }
}

