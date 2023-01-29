namespace StreetMaker
{
    partial class frmMultiStreetSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiStreetSettings));
            this.cbStreetType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudLength = new System.Windows.Forms.NumericUpDown();
            this.lbLength = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudSOffset = new System.Windows.Forms.NumericUpDown();
            this.cbCenterDivider2 = new System.Windows.Forms.ComboBox();
            this.label46 = new System.Windows.Forms.Label();
            this.cbRightBorder = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbLeftBorder = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbRightLines = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nudRightLaneCount = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.cbCenterDivider = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbLeftLines = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nudCenterLaneCount = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nudLeftLaneCount = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nudInnerRadius = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.nudAngle = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.nudCurveAngle = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.cbRampType = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.nudRampCurveAngle = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.nudRampRadius = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.ckbFineSteps = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRightLaneCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCenterLaneCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeftLaneCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInnerRadius)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurveAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRampCurveAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRampRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // cbStreetType
            // 
            this.cbStreetType.FormattingEnabled = true;
            this.cbStreetType.Location = new System.Drawing.Point(105, 34);
            this.cbStreetType.Name = "cbStreetType";
            this.cbStreetType.Size = new System.Drawing.Size(121, 21);
            this.cbStreetType.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cbStreetType, "Displays the current type of the street element and allows changing to a complete" +
        "ly different one. Changing the type might require lane adjustments.");
            this.cbStreetType.SelectedIndexChanged += new System.EventHandler(this.cbStreetType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type:";
            // 
            // nudLength
            // 
            this.nudLength.DecimalPlaces = 3;
            this.nudLength.Location = new System.Drawing.Point(127, 87);
            this.nudLength.Name = "nudLength";
            this.nudLength.Size = new System.Drawing.Size(77, 20);
            this.nudLength.TabIndex = 7;
            this.toolTip1.SetToolTip(this.nudLength, "Straight length of the street element. Does not apply to curves.");
            this.nudLength.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // lbLength
            // 
            this.lbLength.AutoSize = true;
            this.lbLength.Location = new System.Drawing.Point(22, 89);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(43, 13);
            this.lbLength.TabIndex = 6;
            this.lbLength.Text = "Length:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "S-Offset:";
            // 
            // nudSOffset
            // 
            this.nudSOffset.DecimalPlaces = 3;
            this.nudSOffset.Location = new System.Drawing.Point(127, 113);
            this.nudSOffset.Name = "nudSOffset";
            this.nudSOffset.Size = new System.Drawing.Size(77, 20);
            this.nudSOffset.TabIndex = 7;
            this.toolTip1.SetToolTip(this.nudSOffset, "Offset from the center line at the end of an S-shaped street element only.");
            this.nudSOffset.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // cbCenterDivider2
            // 
            this.cbCenterDivider2.FormattingEnabled = true;
            this.cbCenterDivider2.Location = new System.Drawing.Point(104, 324);
            this.cbCenterDivider2.Name = "cbCenterDivider2";
            this.cbCenterDivider2.Size = new System.Drawing.Size(121, 21);
            this.cbCenterDivider2.TabIndex = 22;
            this.toolTip1.SetToolTip(this.cbCenterDivider2, "Divider lane type left side of the Center Lane, when present. It is normally Doub" +
        "le Yellow Dashed Solid Line.");
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(21, 327);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(72, 13);
            this.label46.TabIndex = 23;
            this.label46.Text = "Center Div. 2:";
            // 
            // cbRightBorder
            // 
            this.cbRightBorder.FormattingEnabled = true;
            this.cbRightBorder.Location = new System.Drawing.Point(104, 404);
            this.cbRightBorder.Name = "cbRightBorder";
            this.cbRightBorder.Size = new System.Drawing.Size(121, 21);
            this.cbRightBorder.TabIndex = 18;
            this.toolTip1.SetToolTip(this.cbRightBorder, "Border type on the right side of the right lanes in driving direction. Normally i" +
        "t is the ShoulderLine, but can also be set to other line types.");
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 407);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Right Border:";
            // 
            // cbLeftBorder
            // 
            this.cbLeftBorder.FormattingEnabled = true;
            this.cbLeftBorder.Location = new System.Drawing.Point(104, 217);
            this.cbLeftBorder.Name = "cbLeftBorder";
            this.cbLeftBorder.Size = new System.Drawing.Size(121, 21);
            this.cbLeftBorder.TabIndex = 16;
            this.toolTip1.SetToolTip(this.cbLeftBorder, "Border type on the left side of the left lanes in driving direction. Normally it " +
        "is the ShoulderLine, but can also be set to other line types.");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Left Border:";
            // 
            // cbRightLines
            // 
            this.cbRightLines.FormattingEnabled = true;
            this.cbRightLines.Location = new System.Drawing.Point(104, 377);
            this.cbRightLines.Name = "cbRightLines";
            this.cbRightLines.Size = new System.Drawing.Size(121, 21);
            this.cbRightLines.TabIndex = 10;
            this.toolTip1.SetToolTip(this.cbRightLines, "Line type of the lines between multiple right lanes in driving direction.");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 380);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Right Lines:";
            // 
            // nudRightLaneCount
            // 
            this.nudRightLaneCount.Location = new System.Drawing.Point(146, 351);
            this.nudRightLaneCount.Name = "nudRightLaneCount";
            this.nudRightLaneCount.Size = new System.Drawing.Size(36, 20);
            this.nudRightLaneCount.TabIndex = 9;
            this.nudRightLaneCount.Tag = "0";
            this.toolTip1.SetToolTip(this.nudRightLaneCount, "Number of lanes on the right side in the driving direction.");
            this.nudRightLaneCount.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudRightLaneCount.ValueChanged += new System.EventHandler(this.nudRightLaneCount_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 353);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Right Lane Count:";
            // 
            // cbCenterDivider
            // 
            this.cbCenterDivider.FormattingEnabled = true;
            this.cbCenterDivider.Location = new System.Drawing.Point(104, 297);
            this.cbCenterDivider.Name = "cbCenterDivider";
            this.cbCenterDivider.Size = new System.Drawing.Size(121, 21);
            this.cbCenterDivider.TabIndex = 6;
            this.toolTip1.SetToolTip(this.cbCenterDivider, "Lane type dividing both directions or to the right side of the Center Lane, when " +
        "present. It is often a Yellow Solid Line.");
            this.cbCenterDivider.SelectedIndexChanged += new System.EventHandler(this.cbCenterDivider_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 300);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Center Divider:";
            // 
            // cbLeftLines
            // 
            this.cbLeftLines.FormattingEnabled = true;
            this.cbLeftLines.Location = new System.Drawing.Point(104, 244);
            this.cbLeftLines.Name = "cbLeftLines";
            this.cbLeftLines.Size = new System.Drawing.Size(121, 21);
            this.cbLeftLines.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cbLeftLines, "Line type of the lines between multiple left lanes, the lanes left of the driving" +
        " direction. It is mostly White Dashed Line.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 247);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Left Lines:";
            // 
            // nudCenterLaneCount
            // 
            this.nudCenterLaneCount.Location = new System.Drawing.Point(146, 271);
            this.nudCenterLaneCount.Name = "nudCenterLaneCount";
            this.nudCenterLaneCount.Size = new System.Drawing.Size(36, 20);
            this.nudCenterLaneCount.TabIndex = 5;
            this.nudCenterLaneCount.Tag = "0";
            this.toolTip1.SetToolTip(this.nudCenterLaneCount, "Number of Center Lanes between left and right lanes.");
            this.nudCenterLaneCount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudCenterLaneCount.ValueChanged += new System.EventHandler(this.nudCenterLaneCount_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 273);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Center Lane Count:";
            // 
            // nudLeftLaneCount
            // 
            this.nudLeftLaneCount.Location = new System.Drawing.Point(146, 191);
            this.nudLeftLaneCount.Name = "nudLeftLaneCount";
            this.nudLeftLaneCount.Size = new System.Drawing.Size(36, 20);
            this.nudLeftLaneCount.TabIndex = 3;
            this.nudLeftLaneCount.Tag = "0";
            this.toolTip1.SetToolTip(this.nudLeftLaneCount, "Number of lanes left from the driving direction.");
            this.nudLeftLaneCount.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.nudLeftLaneCount.ValueChanged += new System.EventHandler(this.nudLeftLaneCount_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(21, 193);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Left Lane Count:";
            // 
            // nudInnerRadius
            // 
            this.nudInnerRadius.DecimalPlaces = 3;
            this.nudInnerRadius.Location = new System.Drawing.Point(127, 139);
            this.nudInnerRadius.Name = "nudInnerRadius";
            this.nudInnerRadius.Size = new System.Drawing.Size(77, 20);
            this.nudInnerRadius.TabIndex = 25;
            this.toolTip1.SetToolTip(this.nudInnerRadius, "Inner radius of a curve. Only valid for curved streetelements.");
            this.nudInnerRadius.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(22, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Inner Radius:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(151, 530);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 27;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "The Cancel button will simply close this dialog without applying any changes.");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(33, 530);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 26;
            this.btnOk.Text = "Ok";
            this.toolTip1.SetToolTip(this.btnOk, "The OK button will save any changes to the selected street element and exit.");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // nudAngle
            // 
            this.nudAngle.DecimalPlaces = 3;
            this.nudAngle.Increment = new decimal(new int[] {
            225,
            0,
            0,
            65536});
            this.nudAngle.Location = new System.Drawing.Point(127, 61);
            this.nudAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudAngle.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.nudAngle.Name = "nudAngle";
            this.nudAngle.Size = new System.Drawing.Size(77, 20);
            this.nudAngle.TabIndex = 29;
            this.toolTip1.SetToolTip(this.nudAngle, "Angle of the complete element on the drawing in degrees.");
            this.nudAngle.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(22, 63);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 13);
            this.label12.TabIndex = 28;
            this.label12.Text = "Angle:";
            // 
            // nudCurveAngle
            // 
            this.nudCurveAngle.DecimalPlaces = 3;
            this.nudCurveAngle.Location = new System.Drawing.Point(127, 165);
            this.nudCurveAngle.Name = "nudCurveAngle";
            this.nudCurveAngle.Size = new System.Drawing.Size(77, 20);
            this.nudCurveAngle.TabIndex = 31;
            this.toolTip1.SetToolTip(this.nudCurveAngle, "Circular angle of a curved street element. Not valid for othere types.");
            this.nudCurveAngle.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(22, 167);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(68, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "Curve Angle:";
            // 
            // cbRampType
            // 
            this.cbRampType.FormattingEnabled = true;
            this.cbRampType.Location = new System.Drawing.Point(104, 431);
            this.cbRampType.Name = "cbRampType";
            this.cbRampType.Size = new System.Drawing.Size(121, 21);
            this.cbRampType.TabIndex = 32;
            this.toolTip1.SetToolTip(this.cbRampType, "Defines any additional on or off ramp lane at the right side.");
            this.cbRampType.SelectedIndexChanged += new System.EventHandler(this.cbRampType_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 434);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 33;
            this.label14.Text = "On/Off-Ramp:";
            // 
            // nudRampCurveAngle
            // 
            this.nudRampCurveAngle.DecimalPlaces = 3;
            this.nudRampCurveAngle.Enabled = false;
            this.nudRampCurveAngle.Location = new System.Drawing.Point(127, 484);
            this.nudRampCurveAngle.Name = "nudRampCurveAngle";
            this.nudRampCurveAngle.Size = new System.Drawing.Size(77, 20);
            this.nudRampCurveAngle.TabIndex = 37;
            this.toolTip1.SetToolTip(this.nudRampCurveAngle, "The on or off ramp is a curved lane with the curve angle defined here.");
            this.nudRampCurveAngle.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(22, 486);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(99, 13);
            this.label15.TabIndex = 36;
            this.label15.Text = "Ramp Curve Angle:";
            // 
            // nudRampRadius
            // 
            this.nudRampRadius.DecimalPlaces = 3;
            this.nudRampRadius.Enabled = false;
            this.nudRampRadius.Location = new System.Drawing.Point(127, 458);
            this.nudRampRadius.Name = "nudRampRadius";
            this.nudRampRadius.Size = new System.Drawing.Size(77, 20);
            this.nudRampRadius.TabIndex = 35;
            this.toolTip1.SetToolTip(this.nudRampRadius, "The on or off ramp is a curved lane with the inner radius defined here.");
            this.nudRampRadius.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(22, 460);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(74, 13);
            this.label16.TabIndex = 34;
            this.label16.Text = "Ramp Radius:";
            // 
            // ckbFineSteps
            // 
            this.ckbFineSteps.AutoSize = true;
            this.ckbFineSteps.Location = new System.Drawing.Point(128, 11);
            this.ckbFineSteps.Name = "ckbFineSteps";
            this.ckbFineSteps.Size = new System.Drawing.Size(76, 17);
            this.ckbFineSteps.TabIndex = 38;
            this.ckbFineSteps.Text = "Fine Steps";
            this.toolTip1.SetToolTip(this.ckbFineSteps, "When checked, the dialog can stay open and fine changes will be applied immediate" +
        "ly.");
            this.ckbFineSteps.UseVisualStyleBackColor = true;
            this.ckbFineSteps.CheckedChanged += new System.EventHandler(this.ckbFineSteps_CheckedChanged);
            // 
            // frmMultiStreetSettings
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(253, 565);
            this.Controls.Add(this.ckbFineSteps);
            this.Controls.Add(this.nudRampCurveAngle);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.nudRampRadius);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.cbRampType);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.nudCurveAngle);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nudAngle);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.nudInnerRadius);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cbCenterDivider2);
            this.Controls.Add(this.label46);
            this.Controls.Add(this.nudSOffset);
            this.Controls.Add(this.cbRightBorder);
            this.Controls.Add(this.nudLength);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbLeftBorder);
            this.Controls.Add(this.lbLength);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbRightLines);
            this.Controls.Add(this.cbStreetType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nudLeftLaneCount);
            this.Controls.Add(this.nudRightLaneCount);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbCenterDivider);
            this.Controls.Add(this.nudCenterLaneCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbLeftLines);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMultiStreetSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Street Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMultiStreetSettings_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRightLaneCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCenterLaneCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeftLaneCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudInnerRadius)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCurveAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRampCurveAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRampRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbStreetType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudLength;
        private System.Windows.Forms.Label lbLength;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudSOffset;
        private System.Windows.Forms.ComboBox cbCenterDivider2;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.ComboBox cbRightBorder;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbLeftBorder;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbRightLines;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudRightLaneCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbCenterDivider;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbLeftLines;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudCenterLaneCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudLeftLaneCount;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudInnerRadius;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.NumericUpDown nudAngle;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nudCurveAngle;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbRampType;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nudRampCurveAngle;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nudRampRadius;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox ckbFineSteps;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}