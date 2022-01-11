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
    /// Dialog form to set up the intersection parameter.
    /// </summary>
    public partial class frmIntersectionSettings : Form
    {
        #region Private Fields
        /// <summary>Reference to the main form object.</summary>
        private frmStreetMakerMain MainForm;
        /// <summary>Reference to the original intersection object to be displayed or edited.</summary>
        private Intersection intersection;
        /// <summary>Reflects the result from checking all GUI values against the original parameter, set to true if any change is detected.</summary>
        private bool hasChanged;
        /// <summary>Array of new street descriptors created from GUI values, allowing to create a new version of the intersection.</summary>
        private StreetDescriptor[] newStreetDescriptor;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the intersection settings form to display and edit all parameter.
        /// </summary>
        /// <param name="MainForm">Reference to the main form object.</param>
        /// <param name="Intersection">Reference to the original intersection object to be displayed or edited.</param>
        public frmIntersectionSettings(frmStreetMakerMain MainForm, Intersection Intersection)
        {
            InitializeComponent();

            this.MainForm = MainForm;
            this.intersection = Intersection;
            LoadForm();
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// When closing this form, clear its reference in the main form to allow another settings dialog to open.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Form closing arguments</param>
        private void frmIntersectionSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.frmStreetElementSettings = null;
        }

        /// <summary>
        /// Set value and maximum of the NumericUpDown for the lane count.
        /// </summary>
        /// <param name="Ctrl">Reference to the control object that owns the NumericUpDown.</param>
        /// <param name="Name">Name of the NumericUpDown control to find.</param>
        /// <param name="Value">Value to set the NumericUpDown value to.</param>
        /// <param name="Maximum">Maximum limit to set the NumericUpDown Maximum to.</param>
        private void SetLaneNUD(Control Ctrl, string Name, int Value, int Maximum)
        {
            NumericUpDown nud = (NumericUpDown)Ctrl.Controls.Find(Name, false)[0];
            nud.Maximum = Maximum;
            nud.Value = Value;
        }

        /// <summary>
        /// Gets the value of the NumericUpDown for a lane count.
        /// </summary>
        /// <param name="Ctrl">Reference to the control object that owns the NumericUpDown.</param>
        /// <param name="Name">Name of the NumericUpDown control to find.</param>
        /// <returns>Integer representation of the NumericUpDown value field.</returns>
        private int GetLaneNUD(Control Ctrl, string Name)
        {
            NumericUpDown nud = (NumericUpDown)Ctrl.Controls.Find(Name, false)[0];
            return (int)nud.Value;
        }

        /// <summary>
        /// Initializes the ComboBox for a LineType selection with all LineType strings and sets the SelectedIndex to the passed value.
        /// </summary>
        /// <param name="Ctrl">Reference to the control object that owns the ComboBox.</param>
        /// <param name="Name">Name of the ComboBox control to find.</param>
        /// <param name="LineType">Line type to select in the ComboBox.</param>
        private void SetLineCB(Control Ctrl, string Name, LineType LineType)
        {
            ComboBox cb = (ComboBox)Ctrl.Controls.Find(Name, false)[0];
            cb.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(LineType)))
                cb.Items.Add(name);
            cb.SelectedIndex = (int)LineType;
        }

        /// <summary>
        /// Gets the selected LineType from the ComboBox on the form.
        /// </summary>
        /// <param name="Ctrl">Reference to the control object that owns the ComboBox.</param>
        /// <param name="Name">Name of the ComboBox control to find.</param>
        /// <returns>LineType representation of the selected index of the ComboBox.</returns>
        private LineType GetLineCB(Control Ctrl, string Name)
        {
            ComboBox cb = (ComboBox)Ctrl.Controls.Find(Name, false)[0];
            return (LineType)cb.SelectedIndex;
        }

        /// <summary>
        /// Loads all parameter of the referenced intersection object to the form controls to display and edit. 
        /// Each street descriptor is owned by it's own GroupBox. The GroupBoxes are enumerated for each StreetDescriptor, containing the same controls each. 
        /// All of them have the index for the StreetDescriptor in the name, so they can be easily found.
        /// </summary>
        private void LoadForm()
        {
            cbIntersectionType.SelectedIndex = intersection.StreetDescriptors.Length - 2;
            nudAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;
            nudAngle.Value = (decimal)Utils.ToDegree(intersection.Lane0Angle - Utils.RIGHT_ANGLE_RADIAN);
            for (int i=0; i< 4; i++)
            {
                int idx = i < intersection.StreetDescriptors.Length ? i : Math.Max(i - 2,0);
                string istr = i.ToString();
                Control gb = this.Controls.Find("gbStreet" + istr, false)[0];

                SetLaneNUD(gb, "nudLeftLaneCount" + istr, intersection.StreetDescriptors[idx].LaneCountLeft, MainForm.AppSettings.MaxLaneCountLeftRight);
                SetLineCB(gb, "cbLeftBorder" + istr, intersection.StreetDescriptors[idx].LeftBorderLine);
                SetLineCB(gb, "cbLeftLines" + istr, intersection.StreetDescriptors[idx].LeftLaneLine);

                SetLaneNUD(gb, "nudCenterLaneCount" + istr, intersection.StreetDescriptors[idx].LaneCountCenter, MainForm.AppSettings.MaxLaneCountCenter);
                SetLineCB(gb, "cbCenterDivider" + istr, intersection.StreetDescriptors[idx].DividerLine);
                SetLineCB(gb, "cbCenterDivider2" + istr, intersection.StreetDescriptors[idx].DividerLine2);

                SetLaneNUD(gb, "nudRightLaneCount" + istr, intersection.StreetDescriptors[idx].LaneCountRight, MainForm.AppSettings.MaxLaneCountLeftRight);
                SetLineCB(gb, "cbRightLines" + istr, intersection.StreetDescriptors[idx].RightLaneLine);
                SetLineCB(gb, "cbRightBorder" + istr, intersection.StreetDescriptors[idx].RightBorderLine);

                ComboBox cb = (ComboBox)gb.Controls.Find("cbCrosswalk" + istr, false)[0];
                cb.Items.Clear();
                foreach (string name in Enum.GetNames(typeof(CrosswalkType)))
                    cb.Items.Add(name);
                cb.SelectedIndex = (int)intersection.StreetDescriptors[idx].CrosswalkType;

                cb = (ComboBox)gb.Controls.Find("cbYieldStop" + istr, false)[0];
                cb.Items.Clear();
                foreach (string name in Enum.GetNames(typeof(StopYieldType)))
                    cb.Items.Add(name);
                cb.SelectedIndex = (int)intersection.StreetDescriptors[idx].StopYieldType;

                NumericUpDown nud = (NumericUpDown)gb.Controls.Find("nudLength" + istr, false)[0];
                nud.Maximum = (decimal)MainForm.StreetMap.DrawingSize.Width;
                nud.Minimum = (decimal)MainForm.AppSettings.MinStraightLength;
                nud.Value = (decimal)intersection.StreetDescriptors[idx].Length;
                nud.Increment = 50;
            }
        }

        /// <summary>
        /// Reads and converts back all values from the GUI elements into a new set of StreetDescriptors that can be used for creating a new Intersection object.
        /// Each back-converted value is checked against the original value and the hasChanges flag will be set to true, if the absolute difference between the values exceeds the tolerance definition.
        /// </summary>
        private void SaveForm()
        {
            newStreetDescriptor = new StreetDescriptor[cbIntersectionType.SelectedIndex + 2];
            hasChanged = newStreetDescriptor.Length != intersection.StreetDescriptors.Length;
            hasChanged |= Math.Abs((double)nudAngle.Value - Utils.ToDegree(intersection.Lane0Angle - Utils.RIGHT_ANGLE_RADIAN)) >= AppSettings.MAX_CHANGE_TOL;

            for (int i = 0; i < newStreetDescriptor.Length; i++)
            {
                string istr = i.ToString();
                Control gb = this.Controls.Find("gbStreet" + istr, false)[0];

                int laneCountLeft = GetLaneNUD(gb, "nudLeftLaneCount" + istr);
                LineType leftBorderLine = GetLineCB(gb, "cbLeftBorder" + istr);
                LineType leftLaneLine = GetLineCB(gb, "cbLeftLines" + istr);

                int laneCountCenter = GetLaneNUD(gb, "nudCenterLaneCount" + istr);
                LineType dividerLine = GetLineCB(gb, "cbCenterDivider" + istr);
                LineType dividerLine2 = GetLineCB(gb, "cbCenterDivider2" + istr);

                int laneCountRight = GetLaneNUD(gb, "nudRightLaneCount" + istr);
                LineType rightLaneLine = GetLineCB(gb, "cbRightLines" + istr);
                LineType rightBorderLine = GetLineCB(gb, "cbRightBorder" + istr);

                CrosswalkType crosswalk = (CrosswalkType)((ComboBox)gb.Controls.Find("cbCrosswalk" + istr, false)[0]).SelectedIndex;
                StopYieldType stopYield = (StopYieldType)((ComboBox)gb.Controls.Find("cbYieldStop" + istr, false)[0]).SelectedIndex;

                double length = (double)(((NumericUpDown)gb.Controls.Find("nudLength" + istr, false)[0]).Value);

                newStreetDescriptor[i] = new StreetDescriptor(MainForm.AppSettings, laneCountRight, laneCountCenter, laneCountLeft, crosswalk, stopYield, 
                                                            rightBorderLine, rightLaneLine, dividerLine, dividerLine2, leftLaneLine, leftBorderLine, length);

                if (hasChanged == false)
                    hasChanged = laneCountLeft != intersection.StreetDescriptors[i].LaneCountLeft || laneCountCenter != intersection.StreetDescriptors[i].LaneCountCenter ||
                             laneCountRight != intersection.StreetDescriptors[i].LaneCountRight || Math.Abs(length - intersection.StreetDescriptors[i].Length) >= AppSettings.MAX_CHANGE_TOL;

                if (hasChanged == false)
                    hasChanged = leftBorderLine != intersection.StreetDescriptors[i].LeftBorderLine || leftLaneLine != intersection.StreetDescriptors[i].LeftLaneLine ||
                             dividerLine != intersection.StreetDescriptors[i].DividerLine || dividerLine2 != intersection.StreetDescriptors[i].DividerLine2 ||
                             rightBorderLine != intersection.StreetDescriptors[i].RightBorderLine || rightLaneLine != intersection.StreetDescriptors[i].RightLaneLine ||
                             crosswalk != intersection.StreetDescriptors[i].CrosswalkType || stopYield != intersection.StreetDescriptors[i].StopYieldType;
            }    
        }


        /// <summary>
        /// Event handler of the cbIntersectionType SelectedIndex Changed event. This selector defines the number of streets leading to or through the intersection.
        /// </summary>
        /// <param name="sender">Reference to the sender object of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void cbIntersectionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbStreet2.Enabled = cbIntersectionType.SelectedIndex >= 1;
            gbStreet3.Enabled = cbIntersectionType.SelectedIndex >= 2;

        }

        /// <summary>
        /// Extracts the index part of the Control name. It is the last character of the name. Even though the sender is passed as object, it has to be a Control type to make it work.
        /// </summary>
        /// <param name="sender">Reference to the Control object with the index in the name.</param>
        /// <returns>The string containing the index character of the name of the Control.</returns>
        private string IdxStr(object sender)
        {
            return ((Control)sender).Name.Substring(((Control)sender).Name.Length - 1);
        }

        /// <summary>
        /// Common event handler for all left lane count NumericUpDown controls to enable or disable cbLeftLines ComboBoxes depending on the lane count.
        /// </summary>
        /// <param name="sender">NumericUpDown for the left lane count.</param>
        /// <param name="e">Event arguments</param>
        private void nudLeftLaneCount_ValueChanged(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).Parent.Controls.Find("cbLeftLines"+ IdxStr(sender), false)[0].Enabled = ((NumericUpDown)sender).Value > 1;
        }

        /// <summary>
        /// Common event handler for all center lane count NumericUpDown controls to enable or disable cbCenterDivider2 ComboBoxes depending on the lane count.
        /// </summary>
        /// <param name="sender">NumericUpDown for the center lane count.</param>
        /// <param name="e">Event arguments</param>
        private void nudCenterLaneCount_ValueChanged(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).Parent.Controls.Find("cbCenterDivider2" + IdxStr(sender), false)[0].Enabled = ((NumericUpDown)sender).Value > 0;
        }

        /// <summary>
        /// Common event handler for all CenterDivider ComboBox controls to default all cbCenterDivider2 selection to the same value if that one was disabled.
        /// </summary>
        /// <param name="sender">ComboBox for the CenterDivider selection</param>
        /// <param name="e">Event arguments</param>
        private void cbCenterDivider_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb2 = (ComboBox)((ComboBox)sender).Parent.Controls.Find("cbCenterDivider2" + IdxStr(sender), false)[0];
            if ((cb2.Enabled == false) && (cb2.Items.Count> ((ComboBox)sender).SelectedIndex))
                cb2.SelectedIndex = ((ComboBox)sender).SelectedIndex;
        }

        /// <summary>
        /// Common event handler for all center lane count NumericUpDown controls to enable or disable cbCenterDivider2 ComboBoxes depending on the lane count.
        /// </summary>
        /// <param name="sender">NumericUpDown for the center lane count.</param>
        /// <param name="e">Event arguments</param>
        private void nudRightLaneCount_ValueChanged(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).Parent.Controls.Find("cbRightLines" + IdxStr(sender), false)[0].Enabled = ((NumericUpDown)sender).Value > 1;
        }


        /// <summary>
        /// Disables the Main Form GUI, converts all values from this form by calling SaveForm, which also sets the hasChanged flag. 
        /// If that flag is true, a new Intersection object is created using the newStreetDescriptor array created by SaveForm.
        /// The original Intersection in the StreetMap object is then replaced with the new Intersection object in its place. 
        /// </summary>
        private void UpdateMainForm()
        {
            MainForm.SetEnabled(false);
            SaveForm();
            if (hasChanged == true)
            {
                Intersection newIntersection = new Intersection(MainForm.AppSettings, newStreetDescriptor);
                newIntersection.SetAngleAndLocation((double)nudAngle.Value, intersection.Lanes[0].Connectors[0].CenterP);
                MainForm.StreetMap.ReplaceStreetElement(intersection, newIntersection);
                intersection = newIntersection;
            }
            MainForm.SetEnabled(true);
        }

        /// <summary>
        /// OK button event handler to call UpdateMainForm and then close this form.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            UpdateMainForm();
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
        /// CheckBox event handler to change the increment steps of NumericUpDown depending on the Checked state.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void ckbFineSteps_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbFineSteps.Checked == false)
            {
                nudAngle.Increment = (decimal)MainForm.AppSettings.AngleStep;

                for (int i = 0; i < 4; i++)
                {
                    string istr = i.ToString();
                    Control gb = this.Controls.Find("gbStreet" + istr, false)[0];
                    ((NumericUpDown)gb.Controls.Find("nudLength" + istr, false)[0]).Increment = (decimal)MainForm.AppSettings.DefaultJunctionLength / 4;
                }
            }
            else
            {
                nudAngle.Increment = (decimal)0.001;

                for (int i = 0; i < 4; i++)
                {
                    string istr = i.ToString();
                    Control gb = this.Controls.Find("gbStreet" + istr, false)[0];
                    ((NumericUpDown)gb.Controls.Find("nudLength" + istr, false)[0]).Increment = (decimal)0.001;
                }
            }
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

        #endregion Private Methods
    }
}
