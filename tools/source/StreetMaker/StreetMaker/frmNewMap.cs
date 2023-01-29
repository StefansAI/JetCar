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
    /// Very simple dialog form to enter the map size.
    /// </summary>
    public partial class frmNewMap : Form
    {
        #region Private Fields
        /// <summary>Drawing width in mm.</summary>
        private double drawingWidth;
        /// <summary>Drawing height in mm.</summary>
        private double drawingHeight;
        /// <summary>Minimum width in mm.</summary>
        private double minimumWidth;
        /// <summary>Minimum height in mm.</summary>
        private double minimumHeight;
        /// <summary>Flag to block the update at re-entrance of the event handler.</summary>
        private bool blockUpdate;
        #endregion Private Fields


        #region Constructor
        /// <summary>
        /// Creates an instance of the form.
        /// </summary>
        /// <param name="DrawingSize">Default drawing size to start with.</param>
        /// <param name="MeasurementUnit">Meter or Feet conversion.</param>
        public frmNewMap(string Text, SizeF DrawingSize, SizeF MinimumSize, MeasurementUnit MeasurementUnit)
        {
            InitializeComponent();
            this.Text = Text;
            blockUpdate = true;
            drawingWidth = DrawingSize.Width;
            drawingHeight = DrawingSize.Height;
            minimumWidth = MinimumSize.Width;
            minimumHeight = MinimumSize.Height;
            cbMeasurementUnit.Items.Clear();
            foreach (string name in Enum.GetNames(typeof(MeasurementUnit)))
                cbMeasurementUnit.Items.Add(name);
            cbMeasurementUnit.SelectedIndex = (int)MeasurementUnit;
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Goes through all Controls owned by this form and all next levels of owned controls to find all 
        /// NumericUpDown objects that have the integer of 0 assigned to the Tag property to set the DecimalPlaces to a value that retains the millimeter resolution.
        /// </summary>
        private void ChangeDecimals()
        {
            double mm = AppSettings.ToUnit(1, (MeasurementUnit)cbMeasurementUnit.SelectedIndex);
            double d = Math.Log10(1 / mm);

            nudWidth.DecimalPlaces = (int)(d + 1);
            nudHeight.DecimalPlaces = (int)(d + 1);
        }



        /// <summary>
        /// ComboBox event handler to change the measurement scale. The display values will be converted from mm into the target scale.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void cbUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDecimals();
            blockUpdate = true;
            nudWidth.Minimum = 0;
            nudHeight.Minimum = 0;
            nudWidth.Value = ToDecimal(drawingWidth);
            nudHeight.Value = ToDecimal(drawingHeight);
            nudWidth.Minimum = ToDecimal(minimumWidth);
            nudHeight.Minimum = ToDecimal(minimumHeight);
            blockUpdate = false;
        }

        /// <summary>
        /// NumericUpDown event handler used to convert the displayed value from the displayed value to mm.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (blockUpdate == false)
                drawingWidth = ToDouble(nudWidth.Value);
        }

        /// <summary>
        /// NumericUpDown event handler used to convert the displayed value from the displayed value to mm.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            if (blockUpdate == false)
                drawingHeight = ToDouble(nudHeight.Value);
        }

        /// <summary>
        /// OK button event handler to close the form.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
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
        #endregion Private Methods


        #region Public Properties

        /// <summary>
        /// Gets the drawing size result from the form.
        /// </summary>
        public SizeF DrawingSize
        {
            get { return new SizeF((float)drawingWidth, (float)drawingHeight); }
        }
        #endregion Public Properties


    }
}
