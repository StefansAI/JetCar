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
    /// Dialog form to set up the multi lane street parameter.
    /// </summary>
    public partial class frmMultiStreetSettings : Form
    {
        #region Private Fields
        /// <summary>Reference to the main form object.</summary>
        private frmStreetMakerMain MainForm;
        /// <summary>Reference to the original multi lane object to be displayed or edited.</summary>
        private MultiLaneStreet MultiLaneStreet;
        /// <summary>Reference to the new multi lane object newly created from GUI values.</summary>
        private MultiLaneStreet newMultiLaneStreet;
        /// <summary>Reflects the result from checking all GUI values against the original parameter, set to true if any change is detected.</summary>
        private bool hasChanged;
        /// <summary>Normally true to close the dialog with the OK button, but could be set to false when parameter are not valid</summary>
        private bool canClose;
        /// <summary>Set to true after the form GUI elements are loaded.</summary>
        private bool loaded;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates the instance of this form.
        /// </summary>
        /// <param name="MainForm">Reference to the main form object.</param>
        /// <param name="MultiLaneStreet">Reference to the original multi lane object to be displayed or edited.</param>
        public frmMultiStreetSettings(frmStreetMakerMain MainForm, MultiLaneStreet MultiLaneStreet)
        {
            InitializeComponent();
            this.MainForm = MainForm;
            this.MultiLaneStreet = MultiLaneStreet;
            loaded = false;
            LoadForm();
        }
        #endregion Constructor

        #region Private Methods

        /// <summary>
        /// When closing this form, clear its reference in the main form to allow another settings dialog to open.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Form closing arguments</param>
        private void frmMultiStreetSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.frmStreetElementSettings = null;
        }

        /// <summary>
        /// Set value and maximum of the NumericUpDown for the lane count.
        /// </summary>
        /// <param name="Nud">Reference to the NumericUpDown object.</param>
        /// <param name="Value">Value to set the NumericUpDown value to.</param>
        /// <param name="Maximum">Maximum limit to set the NumericUpDown Maximum to.</param>
        private void SetLaneNUD(NumericUpDown Nud, int Value, int Maximum)
        {
            Nud.Maximum = Maximum;
            Nud.Value = Value;
        }


        /// <summary>
        /// Initializes the ComboBox for a LineType selection with all LineType strings and sets the SelectedIndex to the passed value.
        /// </summary>
        /// <param name="Cb">Reference to the ComboBox object.</param>
        /// <param name="LineType">Line type to select in the ComboBox.</param>
        private void SetLineCB(ComboBox Cb, LineType LineType)
        {
            Cb.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(LineType)))
                Cb.Items.Add(name);
            Cb.SelectedIndex = (int)LineType;
        }


        /// <summary>
        /// Loads all parameter of the referenced multi lane object to the form controls to display and edit. 
        /// </summary>
        private void LoadForm()
        {
            cbStreetType.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(StreetType)))
                cbStreetType.Items.Add(name);
            cbStreetType.SelectedIndex = (int)MultiLaneStreet.StreetType;

            nudAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
            nudAngle.Value = (decimal)Utils.ToDegree(MultiLaneStreet.Lanes[0].Connectors[0].Angle);

            nudLength.Maximum = (decimal)MainForm.StreetMap.DrawingSize.Width;
            nudLength.Minimum = (decimal)MainForm.AppSettings.MinStraightLength;
            nudLength.Increment = 50;
            nudLength.Value = (decimal)MultiLaneStreet.Lanes[0].Length;

            nudSOffset.Maximum = nudLength.Maximum / 2;
            nudSOffset.Minimum = -nudSOffset.Maximum;
            nudSOffset.Increment = (decimal)MainForm.AppSettings.LaneWidth / 4;
            nudSOffset.Value = (decimal)MultiLaneStreet.Lanes[0].SOffset;

            nudInnerRadius.Minimum = (decimal)MainForm.AppSettings.MinInnerRadius;
            nudInnerRadius.Maximum = 10 * nudInnerRadius.Minimum;
            nudInnerRadius.Increment = (decimal)MainForm.AppSettings.LaneWidth;
            nudInnerRadius.Value = (decimal)MultiLaneStreet.InnerRadius;

            nudCurveAngle.Minimum = (decimal)MainForm.AppSettings.MinCurveAngle;
            nudCurveAngle.Maximum = (decimal)MainForm.AppSettings.MaxCurveAngle;
            nudCurveAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
            nudCurveAngle.Value = (decimal)Math.Abs(Utils.ToDegree(MultiLaneStreet.Lanes[0].CurveAngle));

            SetLaneNUD(nudLeftLaneCount, MultiLaneStreet.LaneCountLeft, MainForm.AppSettings.MaxLaneCountLeftRight);
            SetLineCB(cbLeftBorder, MultiLaneStreet.LeftBorderLine);
            SetLineCB(cbLeftLines, MultiLaneStreet.LeftLaneLine);

            SetLaneNUD(nudCenterLaneCount, MultiLaneStreet.LaneCountCenter, MainForm.AppSettings.MaxLaneCountCenter);
            SetLineCB(cbCenterDivider, MultiLaneStreet.DividerLine);
            SetLineCB(cbCenterDivider2, MultiLaneStreet.DividerLine2);

            SetLaneNUD(nudRightLaneCount, MultiLaneStreet.LaneCountRight, MainForm.AppSettings.MaxLaneCountLeftRight);
            SetLineCB(cbRightLines, MultiLaneStreet.RightLaneLine);
            SetLineCB(cbRightBorder, MultiLaneStreet.RightBorderLine);

            cbRampType.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(RampType)))
                cbRampType.Items.Add(name);
            cbRampType.SelectedIndex = (int)MultiLaneStreet.RampType;

            nudRampRadius.Minimum = (decimal)MainForm.AppSettings.MinInnerRadius;
            nudRampRadius.Maximum = 10 * nudRampRadius.Minimum;
            nudRampRadius.Increment = (decimal)MainForm.AppSettings.LaneWidth;
            nudRampRadius.Value = (decimal)MultiLaneStreet.RampRadius;

            nudRampCurveAngle.Minimum = (decimal)MainForm.AppSettings.MinCurveAngle;
            nudRampCurveAngle.Maximum = (decimal)MainForm.AppSettings.MaxCurveAngle;
            nudRampCurveAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
            nudRampCurveAngle.Value = (decimal)Math.Abs(MultiLaneStreet.RampCurveAngle);

            loaded = true;
        }

        /// <summary>
        /// Reads and converts back all values from the GUI elements into a new multi lane street object.
        /// Each back-converted value is checked against the original value and the hasChanges flag will be set to true, 
        /// if the absolute difference between the values exceeds the tolerance definition.
        /// </summary>
        private void SaveForm()
        {
            hasChanged = cbStreetType.SelectedIndex != (int)MultiLaneStreet.StreetType;
            hasChanged |= Math.Abs((double)nudAngle.Value - Utils.ToDegree(MultiLaneStreet.Lanes[0].Connectors[0].Angle)) >= AppSettings.MAX_CHANGE_TOL;
            hasChanged |= Math.Abs((double)nudLength.Value - MultiLaneStreet.Lanes[0].Length) >= AppSettings.MAX_CHANGE_TOL;
            hasChanged |= Math.Abs((double)nudSOffset.Value - MultiLaneStreet.Lanes[0].SOffset) >= AppSettings.MAX_CHANGE_TOL;
            hasChanged |= Math.Abs((double)nudInnerRadius.Value - MultiLaneStreet.InnerRadius) >= AppSettings.MAX_CHANGE_TOL;
            hasChanged |= Math.Abs((double)nudCurveAngle.Value - MultiLaneStreet.Lanes[0].CurveAngle) >= AppSettings.MAX_CHANGE_TOL;

            int laneCountLeft = (int)nudLeftLaneCount.Value;
            LineType leftBorderLine = (LineType)cbLeftBorder.SelectedIndex;
            LineType leftLaneLine = (LineType)cbLeftLines.SelectedIndex;

            int laneCountCenter = (int)nudCenterLaneCount.Value;
            LineType dividerLine = (LineType)cbCenterDivider.SelectedIndex;
            LineType dividerLine2 = (LineType)cbCenterDivider2.SelectedIndex;

            int laneCountRight = (int)nudRightLaneCount.Value;
            LineType rightLaneLine = (LineType)cbRightLines.SelectedIndex; ;
            LineType rightBorderLine = (LineType)cbRightBorder.SelectedIndex;

            RampType rampType = (RampType)cbRampType.SelectedIndex;
            double rampRadius = (double)nudRampRadius.Value;
            double rampCurveAngle = (double)nudRampCurveAngle.Value;


            hasChanged |= laneCountLeft != MultiLaneStreet.LaneCountLeft || laneCountCenter != MultiLaneStreet.LaneCountCenter || laneCountRight != MultiLaneStreet.LaneCountRight;

            hasChanged |= leftBorderLine != MultiLaneStreet.LeftBorderLine || leftLaneLine != MultiLaneStreet.LeftLaneLine ||
                          dividerLine != MultiLaneStreet.DividerLine || dividerLine2 != MultiLaneStreet.DividerLine2 ||
                          rightBorderLine != MultiLaneStreet.RightBorderLine || rightLaneLine != MultiLaneStreet.RightLaneLine;

            hasChanged |= rampType != MultiLaneStreet.RampType || (Math.Abs(rampRadius - MultiLaneStreet.RampRadius) >= AppSettings.MAX_CHANGE_TOL) || (Math.Abs(rampCurveAngle - MultiLaneStreet.RampCurveAngle) >= AppSettings.MAX_CHANGE_TOL);

            if (hasChanged == true)
            {
                try
                {
                    newMultiLaneStreet = new MultiLaneStreet(MainForm.AppSettings, (StreetType)cbStreetType.SelectedIndex, laneCountRight, laneCountCenter, laneCountLeft,
                                                            rightBorderLine, rightLaneLine, dividerLine, dividerLine2, leftLaneLine, leftBorderLine, (double)nudInnerRadius.Value,
                                                            rampType, rampRadius, rampCurveAngle);
                    canClose = true;
                }
                catch
                {
                    MessageBox.Show("Could not apply the current parameter selection to this street element.\nPlease change parameter like number of lanes etc. until valid.","Warning");
                    canClose = false;
                }
            }
        }

        /// <summary>
        /// Disables the Main Form GUI, converts all values from this form by calling SaveForm, which also sets the hasChanged flag. 
        /// If that flag is true, the newMultiLaneStreet created by SaveForm will replace the original in the MainForm.StreetMap.
        /// </summary>
        private void UpdateMainForm()
        {
            if (loaded)
            {
                MainForm.SetEnabled(false);
                SaveForm();
                if ((canClose == true) && (hasChanged == true))
                {
                    newMultiLaneStreet.SetParameter((double)nudLength.Value, (double)nudCurveAngle.Value, (double)nudSOffset.Value);
                    newMultiLaneStreet.SetAngleAndLocation((double)nudAngle.Value, MultiLaneStreet.Lanes[0].Connectors[0].CenterP);
                    MainForm.StreetMap.ReplaceStreetElement(MultiLaneStreet, newMultiLaneStreet);
                    MultiLaneStreet = newMultiLaneStreet;
                }
                MainForm.SetEnabled(true);
            }
        }

        /// <summary>
        /// OK button event handler to call UpdateMainForm and then close this form.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateMainForm();
            if (canClose == true)
                Close();
        }

        /// <summary>
        /// Cancel button event handler to simply close this form without changing the original objects.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// ComboBox event handler to change the Street Type to straight, curved or s-shaped.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void cbStreetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            StreetType st = (StreetType)cbStreetType.SelectedIndex;
            nudLength.Enabled = (st != StreetType.CurveLeft) && (st != StreetType.CurveRight);
            nudCurveAngle.Enabled = (st == StreetType.CurveLeft) || (st == StreetType.CurveRight);
            nudInnerRadius.Enabled = nudCurveAngle.Enabled;
            nudSOffset.Enabled = !((st == StreetType.CurveLeft) || (st == StreetType.CurveRight) || (st == StreetType.Straight));
            cbRampType.Enabled = st == StreetType.Straight;
            cbRampType_SelectedIndexChanged(null,null);
        }

        /// <summary>
        /// ComboBox eveent handler for changing the ramp type.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void cbRampType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RampType rp = (RampType)cbRampType.SelectedIndex;
            nudRampRadius.Enabled = cbRampType.Enabled && (rp != RampType.None);
            nudRampCurveAngle.Enabled = nudRampRadius.Enabled;
        }

        /// <summary>
        /// NumericUpDown event handler to enable or disable cbLeftLines depending on the lane count value.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nudLeftLaneCount_ValueChanged(object sender, EventArgs e)
        {
            cbLeftLines.Enabled = nudLeftLaneCount.Value > 0;
        }

        /// <summary>
        /// NumericUpDown event handler to enable or disable cbCenterDivider2 depending on the lane count value.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nudCenterLaneCount_ValueChanged(object sender, EventArgs e)
        {
            cbCenterDivider2.Enabled = nudCenterLaneCount.Value > 0;
        }

        /// <summary>
        /// NumericUpDown event handler to enable or disable cbRightLines depending on the lane count value.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nudRightLaneCount_ValueChanged(object sender, EventArgs e)
        {
            cbRightLines.Enabled = nudRightLaneCount.Value > 0;
        }

        /// <summary>
        /// COmboBox event handler to synchronize cbCenterDivider2 selection, if that one is disabled.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void cbCenterDivider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((cbCenterDivider2.Enabled == false) && (cbCenterDivider2.Items.Count> cbCenterDivider.SelectedIndex))
                cbCenterDivider2.SelectedIndex = cbCenterDivider.SelectedIndex;
        }

        /// <summary>
        /// Common NumericUpDown event handler. Whenever the CheckBox for fine steps is checked, the call to UpdateMainForm immediately causes the StreetMap to change.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nud_ValueChanged(object sender, EventArgs e)
        {
            if (ckbFineSteps.Checked == true)
                UpdateMainForm();
        }


        /// <summary>
        /// CheckBox event handler to change the increment steps of NumericUpDown depending on the Checked state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ckbFineSteps_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbFineSteps.Checked == false)
            {
                nudAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
                nudLength.Increment = (decimal)MainForm.AppSettings.MinStraightLength / 4;
                nudSOffset.Increment = (decimal)MainForm.AppSettings.LaneWidth / 4;
                nudInnerRadius.Increment = (decimal)MainForm.AppSettings.LaneWidth;
                nudCurveAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
                nudRampRadius.Increment = (decimal)MainForm.AppSettings.LaneWidth;
                nudRampCurveAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
            }
            else
            {
                nudAngle.Increment = (decimal)0.001;
                nudLength.Increment = (decimal)0.001;
                nudSOffset.Increment = (decimal)0.001;
                nudInnerRadius.Increment = (decimal)0.001;
                nudCurveAngle.Increment = (decimal)0.001;
                nudRampRadius.Increment = (decimal)0.001;
                nudRampCurveAngle.Increment = (decimal)0.001;
            }
        }

        #endregion Private Methods
    }
}
