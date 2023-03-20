// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreetMaker
{
    /// <summary>
    /// Dialog form to edit the application settings.
    /// </summary>
    public partial class frmAppSettingsSetup : Form
    {
        /// <summary>Separator definition for arrays in text fields.</summary>
        private const char SEPARATOR = ';';

        #region Private Fields
        /// <summary>Reference to the main form object.</summary>
        private frmStreetMakerMain MainForm;
        /// <summary>Local copy of the AppSettings object used for editing.</summary>
        private AppSettings editSettings;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the AppSettings Setup Form for editing the AppSettings.
        /// </summary>
        /// <param name="MainForm">Reference to the main form object.</param>
        public frmAppSettingsSetup(frmStreetMakerMain MainForm)
        {
            InitializeComponent();
            this.MainForm = MainForm;
            this.editSettings = new AppSettings();
            MainForm.AppSettings.CopyTo(this.editSettings);
            LoadForm();
        }
        #endregion Constructor


        #region Special conversions
        /// <summary>
        /// Converts a float vector into string representation.
        /// </summary>
        /// <param name="Vector">One dimensional float array.</param>
        /// <returns>String representation of the vector.</returns>
        private string FloatVectorToString(float[] Vector)
        {
            string s = "";
            foreach (float x in Vector)
                s += x.ToString("F1") + SEPARATOR + ' ';

            return s.Trim(new char[] { SEPARATOR, ' ' });
        }

        /// <summary>
        /// Converts a float vector into string representation.
        /// </summary>
        /// <param name="Vector">One dimensional integer array.</param>
        /// <returns>String representation of the vector.</returns>
        private string IntVectorToString(int[] Vector)
        {
            string s = "";
            foreach (int x in Vector)
                s += x.ToString() + SEPARATOR + ' ';

            return s.Trim(new char[] { SEPARATOR, ' ' });
        }

        /// <summary>
        /// Converts the text string into a one dimensional float array.
        /// </summary>
        /// <param name="Text">String containing one or more floating point numbers, separated via the SEPARATOR character.</param>
        /// <param name="MinLimit">Minimum limit allowed for any of the values.</param>
        /// <param name="MaxLimit">Maximum limit allowed for any of the values.</param>
        /// <returns>One dimensional float array converted from the string or null if any value was out of range or on any conversion error.</returns>
        private float[] StringToFloatVector(string Text, float MinLimit, float MaxLimit)
        {
            string[] s = Text.Trim(new char[] { SEPARATOR, ' ' }).Split(new char[] { SEPARATOR });
            float[] x = new float[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    x[i] = Convert.ToSingle(s[i].Trim());
                }
                catch
                {
                    MessageBox.Show(s[i] + " is not a float!", "Conversion Error");
                    return null;
                }
                if (x[i] < MinLimit)
                {
                    MessageBox.Show(s[i] + " < " + MinLimit.ToString() + "!", "Range Min Error");
                    return null;
                }
                if (x[i] > MaxLimit)
                {
                    MessageBox.Show(s[i] + " > " + MaxLimit.ToString() + "!", "Range Max Error");
                    return null;
                }

            }
            return x;
        }

        /// <summary>
        /// Converts the text string into a one dimensional integer array.
        /// </summary>
        /// <param name="Text">String containing one or more integer numbers, separated via the SEPARATOR character.</param>
        /// <param name="MinLimit">Minimum limit allowed for any of the values.</param>
        /// <param name="MaxLimit">Maximum limit allowed for any of the values.</param>
        /// <returns>One dimensional integer array converted from the string or null if any value was out of range or on any conversion error.</returns>
        private int[] StringToIntVector(string Text, int MinLimit, int MaxLimit)
        {
            string[] s = Text.Trim(new char[] { SEPARATOR, ' ' }).Split(new char[] { SEPARATOR });
            int[] x = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                try
                {
                    x[i] = Convert.ToInt32(s[i].Trim());
                }
                catch
                {
                    MessageBox.Show(s[i] + " is not an integer!", "Conversion Error");
                    return null;
                }
                if (x[i] < MinLimit)
                {
                    MessageBox.Show(s[i] + " < " + MinLimit.ToString() + "!", "Range Min Error");
                    return null;
                }
                if (x[i] > MaxLimit)
                {
                    MessageBox.Show(s[i] + " > " + MaxLimit.ToString() + "!", "Range Max Error");
                    return null;
                }
            }
            return x;
        }
        #endregion Special conversions


        #region Private Method
        /// <summary>
        /// Load all customizable values from the editSettings object to the GUI elements of this form.
        /// </summary>
        private void LoadForm()
        { 
            cbMeasurementUnit.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(MeasurementUnit)))
                cbMeasurementUnit.Items.Add(name);
            cbMeasurementUnit.SelectedIndex = (int)editSettings.DisplayMeasurementUnit;

            nudMinCurveAngle.Value = (decimal)editSettings.MinCurveAngle;
            nudMaxCurveAngle.Value = (decimal)editSettings.MaxCurveAngle;
            nudAngleStep.Value = (decimal)editSettings.AngleStep;
            nudMaxLaneCountLeftRight.Value = (decimal)editSettings.MaxLaneCountLeftRight;
            nudMaxLaneCountCenter.Value = (decimal)editSettings.MaxLaneCountCenter;

            nudDefaultCurveAngle.Value = (decimal)editSettings.DefaultCurveAngle;
            nudDefaultRampCurveAngle.Value = (decimal)editSettings.DefaultRampCurveAngle;

            nudStreetOutlineLineWidth.Value = (decimal)editSettings.StreetOutlineLineWidth;
            nudOverlayOutlineLineWidth.Value = (decimal)editSettings.OverlayOutlineLineWidth;
            nudViewPointLength.Value = (decimal)editSettings.ViewPointLength;
            nudViewPointWidth.Value = (decimal)editSettings.ViewPointWidth;

            btnLaneColor.BackColor = editSettings.LaneColor;
            btnLineColorWhite.BackColor = editSettings.LineColorWhite;
            btnLineColorYellow.BackColor = editSettings.LineColorYellow;
            btnStopLineColor.BackColor = editSettings.StopLineColor;
            btnArrowOverlayColor.BackColor = editSettings.ArrowOverlayColor;
            btnBackgroundColor.BackColor = editSettings.BackgroundColor;

            btnStreetOutlineColor.BackColor = editSettings.StreetOutlineColor;
            btnOverlayOutlineColor.BackColor = editSettings.OverlayOutlineColor;
            btnConnectorInColor.BackColor = editSettings.ConnectorInColor;
            btnConnectorOutColor.BackColor = editSettings.ConnectorOutColor;
            btnConnectorNoDirColor.BackColor = editSettings.ConnectorNoDirColor;
            btnConnectionDrawColor.BackColor = editSettings.ConnectionDrawColor;
            btnConnectionSnapColor.BackColor = editSettings.ConnectionSnapColor;
            btnViewPointOutlineColor.BackColor = editSettings.ViewPointOutlineColor;

            SetHotkeyCB(cbHotkeyDelete,editSettings.HotkeyDelete);
            SetHotkeyCB(cbHotkeyAbort,editSettings.HotkeyAbort);
            SetHotkeyCB(cbHotkeyDisconnect,editSettings.HotkeyDisconnect);
            SetHotkeyCB(cbHotkeyRotateLeft,editSettings.HotkeyRotateLeft);
            SetHotkeyCB(cbHotkeyRotateRight,editSettings.HotkeyRotateRight);
            SetHotkeyCB(cbHotkeySizeIncrease,editSettings.HotkeySizeIncrease);
            SetHotkeyCB(cbHotkeySizeDecrease,editSettings.HotkeySizeDecrease);
            SetHotkeyCB(cbHotkeySizeModeBase,editSettings.HotkeySizeModeBase);
            SetHotkeyCB(cbHotkeySizeModeExt,editSettings.HotkeySizeModeExt);
            SetHotkeyCB(cbHotkeyBlockAutoRotate,editSettings.HotkeyBlockAutoRotate);
            SetHotkeyCB(cbHotkeyPropertyPage,editSettings.HotkeyPropertyPage);

            tbSubDirStreetmaps.Text = editSettings.SubDirStreetmaps;
            tbPathToDataStorage.Text = editSettings.PathToDataStorage;

            // second page

            tbDataSetSubDir.Text = editSettings.SubDirDataSet;
            tbSubDirImg.Text = editSettings.SubDirImg;
            tbSubDirMask.Text = editSettings.SubDirMask;
            tbSubDirClassImg.Text = editSettings.SubDirClassImg;
            tbSubDirPred.Text = editSettings.SubDirPred;

            tbSubDirTrain.Text = editSettings.SubDirTrain;
            tbSubDirVal.Text = editSettings.SubDirVal;
            tbSubDirTest.Text = editSettings.SubDirTest;

            tbPrefixImg.Text = editSettings.PrefixImg;
            tbPrefixMask.Text = editSettings.PrefixMask;
            tbPrefixClassImg.Text = editSettings.PrefixClassImg;
            tbPrefixPred.Text = editSettings.PrefixPred;

            tbClassTextFileName.Text = editSettings.ClassTextFileName;
            tbColorMapFileName.Text = editSettings.ColorMapFileName;

            nudCameraHFOV.Value = (decimal)editSettings.CameraHFOV;
            nudCameraImageRatio.Value = (decimal)editSettings.CameraImageRatio;
            nudCameraLensDist1.Value = (decimal)editSettings.CameraLensDistortion1;
            nudCameraLensDist2.Value = (decimal)editSettings.CameraLensDistortion2;

            nudColorCorrRed.Value = (decimal)editSettings.CameraColorCorrRed;
            nudColorCorrGreen.Value = (decimal)editSettings.CameraColorCorrGreen;
            nudColorCorrBlue.Value = (decimal)editSettings.CameraColorCorrBlue;

            nudCameraOversampling.Value = (decimal)editSettings.CameraOversampling;
            nudCameraOutputWidth.Value = (decimal)editSettings.CameraOutputWidth;
            nudCameraOutputHeight.Value = (decimal)editSettings.CameraOutputHeight;

            nudCameraAxisAngle.Value = (decimal)editSettings.CameraAxisAngle;

            ckbDrawWrongDirItems.Checked = editSettings.DrawWrongDirItems;

            nudTrainValRatio.Value = (decimal)editSettings.TrainValRatio;
            ckbValidateCenterViewsOnly.Checked = editSettings.ValidateCenterViewsOnly;
            nudTestOutRatio.Value = (decimal)editSettings.TestOutRatio;
            ckbTestCenterViewsOnly.Checked = editSettings.TestCenterViewsOnly;
            ckbCenterBrightnessResults.Checked = editSettings.CenterBrightnessResults;

            tbSideSteps.Text = FloatVectorToString(editSettings.SideSteps);
            tbAngleSteps.Text = FloatVectorToString(editSettings.AngleSteps);
            tbBrightnessFactors.Text = FloatVectorToString(editSettings.BrightnessFactors);
            tbColorFactors.Text = FloatVectorToString(editSettings.ColorFactors);
            tbNoiseLevels.Text = IntVectorToString(editSettings.NoiseLevels);

            dtSegmClassDefs.Rows.Clear();
            foreach (SegmClassDef scd in SegmClassDefs.Defs)
            {
                DataRow row = dtSegmClassDefs.NewRow();
                row["ClassCode"] = scd.ClassCode;
                row["Name"] = scd.Name;
                row["DrawColor"] = scd.DrawColor;
                row["UseCount"] = scd.UseCount;

                dtSegmClassDefs.Rows.Add(row);
            }
            lbResultingImageCount.Text = MainForm.StreetMap.GetDatasetImageCount(editSettings).ToString();
        }

        /// <summary>
        /// Save the values from the GUI elements of this form to the customizable fields of the editSettings form.
        /// </summary>
        private void SaveForm()
        {
            editSettings.DisplayMeasurementUnit = (MeasurementUnit)cbMeasurementUnit.SelectedIndex;
            editSettings.MinCurveAngle = (double)nudMinCurveAngle.Value;
            editSettings.MaxCurveAngle = (double)nudMaxCurveAngle.Value;

            editSettings.AngleStep = (double)nudAngleStep.Value;
            editSettings.MaxLaneCountLeftRight = (int)nudMaxLaneCountLeftRight.Value;
            editSettings.MaxLaneCountCenter = (int)nudMaxLaneCountCenter.Value;

            editSettings.DefaultCurveAngle = (double)nudDefaultCurveAngle.Value;
            editSettings.DefaultRampCurveAngle = (double)nudDefaultRampCurveAngle.Value;

            editSettings.StreetOutlineLineWidth = (double)nudStreetOutlineLineWidth.Value;
            editSettings.OverlayOutlineLineWidth =(double)nudOverlayOutlineLineWidth.Value;
            editSettings.ViewPointLength = (double)nudViewPointLength.Value;
            editSettings.ViewPointWidth = (double)nudViewPointWidth.Value;

            editSettings.LaneColor = btnLaneColor.BackColor;
            editSettings.LineColorWhite = btnLineColorWhite.BackColor;
            editSettings.LineColorYellow = btnLineColorYellow.BackColor;
            editSettings.StopLineColor = btnStopLineColor.BackColor;
            editSettings.ArrowOverlayColor = btnArrowOverlayColor.BackColor;
            editSettings.BackgroundColor = btnBackgroundColor.BackColor;

            editSettings.StreetOutlineColor = btnStreetOutlineColor.BackColor;
            editSettings.OverlayOutlineColor = btnOverlayOutlineColor.BackColor;
            editSettings.ConnectorInColor = btnConnectorInColor.BackColor;
            editSettings.ConnectorOutColor = btnConnectorOutColor.BackColor;
            editSettings.ConnectorNoDirColor = btnConnectorNoDirColor.BackColor;
            editSettings.ConnectionDrawColor = btnConnectionDrawColor.BackColor;
            editSettings.ConnectionSnapColor = btnConnectionSnapColor.BackColor;
            editSettings.ViewPointOutlineColor = btnViewPointOutlineColor.BackColor;

            editSettings.HotkeyDelete = (Keys)Enum.Parse(typeof(Keys), cbHotkeyDelete.SelectedItem.ToString());
            editSettings.HotkeyAbort = (Keys)Enum.Parse(typeof(Keys), cbHotkeyAbort.SelectedItem.ToString());
            editSettings.HotkeyDisconnect = (Keys)Enum.Parse(typeof(Keys), cbHotkeyDisconnect.SelectedItem.ToString());
            editSettings.HotkeyRotateLeft = (Keys)Enum.Parse(typeof(Keys), cbHotkeyRotateLeft.SelectedItem.ToString());
            editSettings.HotkeyRotateRight = (Keys)Enum.Parse(typeof(Keys), cbHotkeyRotateRight.SelectedItem.ToString());
            editSettings.HotkeySizeIncrease = (Keys)Enum.Parse(typeof(Keys), cbHotkeySizeIncrease.SelectedItem.ToString());
            editSettings.HotkeySizeDecrease = (Keys)Enum.Parse(typeof(Keys), cbHotkeySizeDecrease.SelectedItem.ToString());
            editSettings.HotkeySizeModeBase = (Keys)Enum.Parse(typeof(Keys), cbHotkeySizeModeBase.SelectedItem.ToString());
            editSettings.HotkeySizeModeExt = (Keys)Enum.Parse(typeof(Keys), cbHotkeySizeModeExt.SelectedItem.ToString());
            editSettings.HotkeyBlockAutoRotate = (Keys)Enum.Parse(typeof(Keys), cbHotkeyBlockAutoRotate.SelectedItem.ToString());
            editSettings.HotkeyPropertyPage = (Keys)Enum.Parse(typeof(Keys), cbHotkeyPropertyPage.SelectedItem.ToString());


            editSettings.SubDirStreetmaps = tbSubDirStreetmaps.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.PathToDataStorage = tbPathToDataStorage.Text.TrimEnd(new char[] { '\\' }) + '\\';

            // second page

            editSettings.SubDirDataSet = tbDataSetSubDir.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirImg = tbSubDirImg.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirMask = tbSubDirMask.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirClassImg = tbSubDirClassImg.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirPred = tbSubDirPred.Text.TrimEnd(new char[] { '\\' }) + '\\';

            editSettings.SubDirTrain = tbSubDirTrain.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirVal = tbSubDirVal.Text.TrimEnd(new char[] { '\\' }) + '\\';
            editSettings.SubDirTest = tbSubDirTest.Text.TrimEnd(new char[] { '\\' }) + '\\';

            editSettings.PrefixImg = tbPrefixImg.Text;
            editSettings.PrefixMask = tbPrefixMask.Text;
            editSettings.PrefixClassImg = tbPrefixClassImg.Text;
            editSettings.PrefixPred = tbPrefixPred.Text;

            editSettings.ClassTextFileName = tbClassTextFileName.Text;
            editSettings.ColorMapFileName = tbColorMapFileName.Text;

            editSettings.CameraHFOV = (double)nudCameraHFOV.Value;
            editSettings.CameraLensDistortion1 = (double)nudCameraLensDist1.Value;
            editSettings.CameraLensDistortion2 = (double)nudCameraLensDist2.Value;
            editSettings.CameraAxisAngle = (double)nudCameraAxisAngle.Value;

            editSettings.CameraColorCorrRed = (double)nudColorCorrRed.Value;
            editSettings.CameraColorCorrGreen = (double)nudColorCorrGreen.Value;
            editSettings.CameraColorCorrBlue = (double)nudColorCorrBlue.Value;

            editSettings.CameraOversampling = (int)nudCameraOversampling.Value;
            editSettings.CameraOutputWidth =  (int)nudCameraOutputWidth.Value;
            editSettings.CameraOutputHeight = (int)nudCameraOutputHeight.Value;

            editSettings.DrawWrongDirItems = ckbDrawWrongDirItems.Checked;

            editSettings.TrainValRatio = (int)nudTrainValRatio.Value;
            editSettings.ValidateCenterViewsOnly = ckbValidateCenterViewsOnly.Checked;
            editSettings.TestOutRatio = (int)nudTestOutRatio.Value;
            editSettings.TestCenterViewsOnly = ckbTestCenterViewsOnly.Checked;
            editSettings.CenterBrightnessResults = ckbCenterBrightnessResults.Checked;

            editSettings.SideSteps = StringToFloatVector(tbSideSteps.Text, (float)editSettings.LaneWidth * AppSettings.SIDE_STEPS_FACTOR_MIN, (float)editSettings.LaneWidth * AppSettings.SIDE_STEPS_FACTOR_MAX);
            editSettings.AngleSteps = StringToFloatVector(tbAngleSteps.Text, AppSettings.ANGLE_STEP_MIN, AppSettings.ANGLE_STEP_MAX);
            editSettings.BrightnessFactors = StringToFloatVector(tbBrightnessFactors.Text, AppSettings.BRIGHTNESS_FACTOR_MIN, AppSettings.BRIGHTNESS_FACTOR_MAX);
            editSettings.ColorFactors = StringToFloatVector(tbColorFactors.Text, AppSettings.COLOR_FACTOR_MIN, AppSettings.COLOR_FACTOR_MAX);
            editSettings.NoiseLevels = StringToIntVector(tbNoiseLevels.Text, AppSettings.NOISE_LEVEL_MIN, AppSettings.NOISE_LEVEL_MAX);

            for (int i = 0; i < dtSegmClassDefs.Rows.Count; i++)
            {
                DataRow row = dtSegmClassDefs.Rows[i];
                SegmClassDefs.Defs[i].Name = (string)row["Name"];
                SegmClassDefs.Defs[i].DrawColor = (Color)row["DrawColor"];
            }
        }

        /// <summary>
        /// Conversion method from the internally used millimeter value to the display value in the given measurement unit. 
        /// </summary>
        /// <param name="SizeValue">Width or height in mm.</param>
        /// <returns>Decimal conversion result.</returns>
        private decimal ToDecimal(double SizeValue)
        {
            return (decimal)AppSettings.ToUnit(SizeValue, (MeasurementUnit)cbMeasurementUnit.SelectedIndex);
        }

        /// <summary>
        /// Conversion method from the display value in the target unit into the internally used miliimeter value.
        /// </summary>
        /// <param name="SizeValue">>Width or height in display value.</param>
        /// <returns>Double representation in millimeter.</returns>
        private double ToDouble(decimal SizeValue)
        {
            return Math.Round(AppSettings.FromUnit((double)SizeValue, (MeasurementUnit)cbMeasurementUnit.SelectedIndex),3);
        }

        /// <summary>
        /// Recursively callable method to set the DecimalPlaces property of NumericUpDown objects listed in the ControlCollection.
        /// The DecimalPlaces property will only be set in all those NumericUpDown object, that have an integer 0 assigned to its Tag property.
        /// </summary>
        /// <param name="Ctrls">Controls list to check for NumericUpDown objects.</param>
        /// <param name="DecimalPlaces">Number of DecimalPlaces to set to.</param>
        private void ChangeDecimals(Control.ControlCollection Ctrls, int DecimalPlaces)
        {
            foreach (Control ctrl in Ctrls)
            {
                NumericUpDown nud = ctrl as NumericUpDown;
                if (nud != null)
                {
                    if ((nud.Tag != null) && (nud.Tag.ToString() == "0"))
                        nud.DecimalPlaces = DecimalPlaces;
                }
                else ChangeDecimals(ctrl.Controls, DecimalPlaces);
            }
        }

        /// <summary>
        /// Goes through all Controls owned by this form and all next levels of owned controls to find all 
        /// NumericUpDown objects that have the integer of 0 assigned to the Tag property to set the DecimalPlaces to a value that retains the millimeter resolution.
        /// </summary>
        private void ChangeDecimals()
        {
            double mm = AppSettings.ToUnit(1, (MeasurementUnit)cbMeasurementUnit.SelectedIndex);
            double d = Math.Log10(1 / mm);

            ChangeDecimals(Controls, (int)(d+1));
        }

        /// <summary>
        /// Fills the given ComboBox with all members of the Keys enumeration and selects the one that is passed as argument.
        /// </summary>
        /// <param name="CB">Reference to the ComboBox object to be set.</param>
        /// <param name="Key">Key value to select in the ComboBox.</param>
        private void SetHotkeyCB(ComboBox CB, Keys Key)
        {
            CB.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(Keys)))
            {
                CB.Items.Add(name);
                if (Key.ToString() == name)
                    CB.SelectedIndex = CB.Items.Count - 1;
            }
        }

        /// <summary>
        /// ComboBox Selected Index Changed Event Handler to convert all unit related NumericUpDown values into the selected unit.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void cbMeasurementUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDecimals();
            nudDefaultDrawingWidth.Value = ToDecimal(editSettings.DefaultDrawingWidth);
            nudDefaultDrawingHeight.Value = ToDecimal(editSettings.DefaultDrawingHeight);

            nudLaneWidth.Value = ToDecimal(editSettings.LaneWidth);
            nudLineWidth.Value = ToDecimal(editSettings.LineWidth);
            nudLineSpace.Value = ToDecimal(editSettings.LineSpace);
            nudStopLineWidth.Value = ToDecimal(editSettings.StopLineWidth);
            nudStopLineOffset.Value = ToDecimal(editSettings.StopLineOffset);

            nudCrosswalkWidth.Value = ToDecimal(editSettings.CrosswalkWidth);
            nudCrosswalkLineWidth.Value = ToDecimal(editSettings.CrosswalkLineWidth);
            nudCrosswalkStripeWidth.Value = ToDecimal(editSettings.CrosswalkStripeWidth);
            nudCrosswalkBorder.Value = ToDecimal(editSettings.CrosswalkBorder);

            nudDashLength.Value = ToDecimal(editSettings.DashLength);
            nudMinStraightLength.Value = ToDecimal(editSettings.MinStraightLength);
            nudMinInnerRadius.Value = ToDecimal(editSettings.MinInnerRadius);
            nudCornerRadius.Value = ToDecimal(editSettings.CornerRadius);

            nudLengthStep.Value = ToDecimal(editSettings.LengthStep);
            nudMaxArrowOverlayWidth.Value = ToDecimal(editSettings.MaxArrowOverlayWidth);
            nudMaxArrowOverlayLength.Value = ToDecimal(editSettings.MaxArrowOverlayLength);
            nudMinArrowOverlayStep.Value = ToDecimal(editSettings.MinArrowOverlayStep);


            nudDefaultStraightLength.Value = ToDecimal(editSettings.DefaultStraightLength);
            nudDefaultJunctionLength.Value = ToDecimal(editSettings.DefaultJunctionLength);
            nudDefaultRampRadius.Value = ToDecimal(editSettings.DefaultRampRadius);
            nudDefaultRoundaboutRadius.Value = ToDecimal(editSettings.DefaultRoundaboutRadius);

            nudPointCatchDistance.Value = ToDecimal(editSettings.PointCatchDistance);
            nudConnectionDrawDistance.Value = ToDecimal(editSettings.ConnectionDrawDistance);
            nudConnectionSnapDistance.Value = ToDecimal(editSettings.ConnectionSnapDistance);

            nudCameraHeight.Value = ToDecimal(editSettings.CameraHeight);
            nudMarkLaneMaxDistFront.Value = ToDecimal(editSettings.MarkLaneMaxDistFront);
            nudMarkLaneMaxDistSide.Value = ToDecimal(editSettings.MarkLaneMaxDistSide);
            nudMarkMaxDetailDistance.Value = ToDecimal(editSettings.MarkMaxDetailDistance);
            nudImageStepSize.Value = ToDecimal(editSettings.ImageStepSize);

        }

        /// <summary>
        /// Update lbResultingImageCount and lbTranValTest with the image count predictions from the current parameter and values.
        /// </summary>
        private void UpdateDatasetImageCount()
        {
            int imgCount = MainForm.StreetMap.GetDatasetImageCount(editSettings);
            lbResultingImageCount.Text = imgCount.ToString();
            int valCount = imgCount / (int)nudTrainValRatio.Value;
            lbTranValTest.Text = (imgCount - valCount).ToString() + "/" + valCount.ToString() + "/" + (imgCount / (int)nudTestOutRatio.Value).ToString();
        }

        #region NumericUpDown Events
        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultDrawingWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultDrawingWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultDrawingWidth = ToDouble(nudDefaultDrawingWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultDrawingHeight value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultDrawingHeight_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultDrawingHeight = ToDouble(nudDefaultDrawingHeight.Value);
        }


        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the LaneWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudLaneWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.LaneWidth = ToDouble(nudLaneWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the LineWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudLineWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.LineWidth = ToDouble(nudLineWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the LineSpace value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudLineSpace_ValueChanged(object sender, EventArgs e)
        {
            editSettings.LineSpace = ToDouble(nudLineSpace.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the StopLineWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudStopLineWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.StopLineWidth = ToDouble(nudStopLineWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the StopLineOffset value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudStopLineOffset_ValueChanged(object sender, EventArgs e)
        {
            editSettings.StopLineOffset = ToDouble(nudStopLineOffset.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CrosswalkWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCrosswalkWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CrosswalkWidth = ToDouble(nudCrosswalkWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CrosswalkLineWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCrosswalkLineWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CrosswalkLineWidth = ToDouble(nudCrosswalkLineWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CrosswalkStripeWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCrosswalkStripeWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CrosswalkStripeWidth = ToDouble(nudCrosswalkStripeWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CrosswalkBorder value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCrosswalkBorder_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CrosswalkBorder = ToDouble(nudCrosswalkBorder.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DashLength value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDashLength_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DashLength = ToDouble(nudDashLength.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MinStraightLength value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMinStraightLength_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MinStraightLength = ToDouble(nudMinStraightLength.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MinInnerRadius value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMinInnerRadius_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MinInnerRadius = ToDouble(nudMinInnerRadius.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CornerRadius value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCornerRadius_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CornerRadius = ToDouble(nudCornerRadius.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MinCurveAngle value double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMinCurveAngle_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MinCurveAngle = (double)nudMinCurveAngle.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MaxCurveAngle value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMaxCurveAngle_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MaxCurveAngle = (double)nudMaxCurveAngle.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the AngleStep value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudAngleStep_ValueChanged(object sender, EventArgs e)
        {
            editSettings.AngleStep = (double)nudAngleStep.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the LengthStep value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudLengthStep_ValueChanged(object sender, EventArgs e)
        {
            editSettings.LengthStep = ToDouble(nudLengthStep.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MaxArrowOverlayWidth value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMaxArrowOverlayWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MaxArrowOverlayWidth = ToDouble(nudMaxArrowOverlayWidth.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MaxArrowOverlayLength value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMaxArrowOverlayLength_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MaxArrowOverlayLength = ToDouble(nudMaxArrowOverlayLength.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MinArrowOverlayStep value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMinArrowOverlayStep_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MinArrowOverlayStep = ToDouble(nudMinArrowOverlayStep.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MaxLaneCountLeftRight value to integer.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMaxLaneCountLeftRight_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MaxLaneCountLeftRight = (int)nudMaxLaneCountLeftRight.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MaxLaneCountCenter value to integer.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMaxLaneCountCenter_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MaxLaneCountCenter = (int)nudMaxLaneCountCenter.Value;
        }

        /// <summary>
        /// General Button Click Event Handler for all color buttons to bring up the ColorDialog to set a color, which is written back to the sender button BackColor..
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnColor_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                colorDialog1.Color = ((Button)sender).BackColor;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                    ((Button)sender).BackColor = colorDialog1.Color;
            }
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultStraightLength value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultStraightLength_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultStraightLength = ToDouble(nudDefaultStraightLength.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultJunctionLength value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultJunctionLength_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultJunctionLength = ToDouble(nudDefaultJunctionLength.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultCurveAngle value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultCurveAngle_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultCurveAngle = (double)nudDefaultCurveAngle.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultRampCurveAngle value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultRampCurveAngle_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultRampCurveAngle = (double)nudDefaultRampCurveAngle.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultRampRadius value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultRampRadius_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultRampRadius = ToDouble(nudDefaultRampRadius.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the DefaultRoundaboutRadius value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudDefaultRoundaboutRadius_ValueChanged(object sender, EventArgs e)
        {
            editSettings.DefaultRoundaboutRadius = ToDouble(nudDefaultRoundaboutRadius.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the PointCatchDistance value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudPointCatchDistance_ValueChanged(object sender, EventArgs e)
        {
            editSettings.PointCatchDistance = ToDouble(nudPointCatchDistance.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the ConnectionDrawDistance value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudConnectionDrawDistance_ValueChanged(object sender, EventArgs e)
        {
            editSettings.ConnectionDrawDistance = ToDouble(nudConnectionDrawDistance.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the ConnectionSnapDistance value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudConnectionSnapDistance_ValueChanged(object sender, EventArgs e)
        {
            editSettings.ConnectionSnapDistance = ToDouble(nudConnectionSnapDistance.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the StreetOutlineLineWidth value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudStreetOutlineLineWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.StreetOutlineLineWidth = (double)nudStreetOutlineLineWidth.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the OverlayOutlineLineWidth value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudOverlayOutlineLineWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.OverlayOutlineLineWidth = (double)nudOverlayOutlineLineWidth.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraHFOV value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraHFOV_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraHFOV = (double)nudCameraHFOV.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraImageRatio value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraImageRatio_ValueChanged(object sender, EventArgs e)
        {
            double d = (double)nudCameraImageRatio.Value;
            if (Math.Abs(d - 4.0 / 3.0) < 0.01)
                d = 4.0 / 3.0;
            editSettings.CameraImageRatio = d;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraLensDist1 value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraLensDist1_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraLensDistortion1 = (double)nudCameraLensDist1.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraLensDist2 value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraLensDist2_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraLensDistortion2 = (double)nudCameraLensDist2.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraHeight value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraHeight_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraHeight = ToDouble(nudCameraHeight.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraAxisAngle value to double.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraAxisAngle_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraAxisAngle = (double)nudCameraAxisAngle.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraOutputWidth value to int.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraOutputWidth_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraOutputWidth = (int)nudCameraOutputWidth.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraOutputHeight value to int.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraOutputHeight_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraOutputHeight = (int)nudCameraOutputHeight.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the CameraOversampling value to int.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCameraOversampling_ValueChanged(object sender, EventArgs e)
        {
            editSettings.CameraOversampling = (int)nudCameraOversampling.Value;
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MarkLaneMaxDistFront value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMarkLaneMaxDistFront_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MarkLaneMaxDistFront = ToDouble(nudMarkLaneMaxDistFront.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the MarkLaneMaxDistSide value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMarkLaneMaxDistSide_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MarkLaneMaxDistSide = ToDouble(nudMarkLaneMaxDistSide.Value);
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the arkMaxDetailDistance value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudMarkMaxDetailDistance_ValueChanged(object sender, EventArgs e)
        {
            editSettings.MarkMaxDetailDistance = ToDouble(nudMarkMaxDetailDistance.Value);
        }


        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the TrainValRatio value to int.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudTrainValRatio_ValueChanged(object sender, EventArgs e)
        {
            editSettings.TrainValRatio = (int)nudTrainValRatio.Value;
            UpdateDatasetImageCount();
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the TestOutRatio value to int.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudTestOutRatio_ValueChanged(object sender, EventArgs e)
        {
            editSettings.TestOutRatio = (int)nudTestOutRatio.Value;
            UpdateDatasetImageCount();
        }

        /// <summary>
        /// NumericUpDown Value Changed Event Handler to convert the ImageStepSize value to the internal Millimeter value.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void nudImageStepSize_ValueChanged(object sender, EventArgs e)
        {
            editSettings.ImageStepSize = ToDouble(nudImageStepSize.Value);
            UpdateDatasetImageCount();
        }
        #endregion NumericUpDown Events

        /// <summary>
        /// Validation event handler of the TextBox contents for the side steps. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbSideSteps_Validating(object sender, CancelEventArgs e)
        {
            float[] values = StringToFloatVector(tbSideSteps.Text, (float)editSettings.LaneWidth * AppSettings.SIDE_STEPS_FACTOR_MIN, (float)editSettings.LaneWidth * AppSettings.SIDE_STEPS_FACTOR_MAX);
            if (values != null)
            {
                editSettings.SideSteps = values;
                UpdateDatasetImageCount();
            }
            else e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the angle steps. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbAngleSteps_Validating(object sender, CancelEventArgs e)
        {
            float[] values = StringToFloatVector(tbAngleSteps.Text, AppSettings.ANGLE_STEP_MIN, AppSettings.ANGLE_STEP_MAX);
            if (values != null)
            {
                editSettings.AngleSteps = values;
                UpdateDatasetImageCount();
            }
            else e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the brightness factors. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbBrightnessFactors_Validating(object sender, CancelEventArgs e)
        {
            float[] values = StringToFloatVector(tbBrightnessFactors.Text, AppSettings.BRIGHTNESS_FACTOR_MIN, AppSettings.BRIGHTNESS_FACTOR_MAX);
            if ( values != null)
            {
                editSettings.BrightnessFactors = values;
                UpdateDatasetImageCount();
            }
            else e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the color factors. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbColorFactors_Validating(object sender, CancelEventArgs e)
        {
            float[] values = StringToFloatVector(tbColorFactors.Text, AppSettings.COLOR_FACTOR_MIN, AppSettings.COLOR_FACTOR_MAX);
            if (values != null)
            {
                editSettings.ColorFactors = values;
                UpdateDatasetImageCount();
            }
            else e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the noise levels. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbNoiseLevels_Validating(object sender, CancelEventArgs e)
        {
            int[] values = StringToIntVector(tbNoiseLevels.Text, AppSettings.NOISE_LEVEL_MIN, AppSettings.NOISE_LEVEL_MAX);
            if (values != null)
            {
                editSettings.NoiseLevels = values;
                UpdateDatasetImageCount();
            }
            else e.Cancel = true;
        }

        /// <summary>
        /// OK Button Click Event Handler to save the form GUI values to the editSettings object and to copy its contents to the main form AppSettings object before closing this form.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveForm();
            editSettings.CopyTo(MainForm.AppSettings);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Cancel Button Click Event Handler to simply close the form without saving anything.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// SedDefaults Button Click Event Handler to create a new AppSettings object with all defaults, assign that to editSettings and call LoadForm to update the GUI.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnSetDefaults_Click(object sender, EventArgs e)
        {
            editSettings = new AppSettings();
            LoadForm();
        }

        #endregion Private Method


        #region DataGridView events
        /// <summary>
        /// Event handler of the DataGridView CellClick events. In this case, clicking on a cell with color selection opens up a ColorDialog for choosing a different color. 
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments</param>
        private void dgvSegmClassDefs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 2) && (e.RowIndex >= 0))
            {
                colorDialog1.Color = (Color)dgvSegmClassDefs[e.ColumnIndex, e.RowIndex].Value;
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    dgvSegmClassDefs[e.ColumnIndex, e.RowIndex].Value = colorDialog1.Color;
                }
            }
        }

        /// <summary>
        /// The DataGridView CellFormatting event is used to create transparent cells where a color was assigned.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments</param>
        private void dgvSegmClassDefs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 2) && (e.RowIndex >= 0) /*&& (e.Value != null) */&& (e.Value is Color))
            {
                e.CellStyle.SelectionBackColor = Color.Transparent;
                e.CellStyle.BackColor = Color.Transparent;
                e.Value = "";
                e.FormattingApplied = true;
            }
        }

        /// <summary>
        /// The DataGridView CellPainting event is used to draw a filled rectangle in the cell with the assigned color.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void dgvSegmClassDefs_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == 2) && (e.RowIndex >= 0) /*&& (e.Value != null)*/ && (e.Value is Color))
            {
                e.Graphics.FillRectangle(new SolidBrush(dgvSegmClassDefs.BackgroundColor), new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height));
                e.Graphics.FillRectangle(new SolidBrush((Color)e.Value), new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4, e.CellBounds.Width - 8, e.CellBounds.Height - 8));
            }
        }


        #endregion DataGridView events

    }
}
