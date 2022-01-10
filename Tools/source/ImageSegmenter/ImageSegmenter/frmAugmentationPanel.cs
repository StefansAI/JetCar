// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
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

namespace ImageSegmenter
{
    /// <summary>
    /// Dialog form to test out different augmentation values.
    /// </summary>
    public partial class frmAugmentationPanel : Form
    {
        /// <summary>Reference to the main form.</summary>
        private frmImageSegmenterMain Main;

        #region Constructor
        /// <summary>
        /// Creates the dialog form instance and keeps the reference to the main form to later inform when closing.
        /// </summary>
        /// <param name="Main">Reference to the main form.</param>
        public frmAugmentationPanel(frmImageSegmenterMain Main)
        {
            InitializeComponent();
            this.Main = Main;
            nudZoom.Minimum = (decimal)AppSettings.ZOOM_FACTOR_MIN;
            nudZoom.Maximum = (decimal)AppSettings.ZOOM_FACTOR_MAX;
            nudTilt.Minimum = (decimal)AppSettings.TILT_ANGLE_MIN;
            nudTilt.Maximum = (decimal)AppSettings.TILT_ANGLE_MAX;
            nudBright.Minimum = (decimal)AppSettings.BRIGHTNESS_FACTOR_MIN;
            nudBright.Maximum = (decimal)AppSettings.BRIGHTNESS_FACTOR_MAX;
            nudContrast.Minimum = (decimal)AppSettings.CONTRAST_ENHANCEMENT_MIN;
            nudContrast.Maximum = (decimal)AppSettings.CONTRAST_ENHANCEMENT_MAX;
            nudNoise.Minimum = (decimal)AppSettings.NOISE_ADDER_MIN;
            nudNoise.Maximum = (decimal)AppSettings.NOISE_ADDER_MAX;
        }
        #endregion Constructor

        #region Event Handler
        /// <summary>
        /// Dialog form closing event used to inform the main form that this dialog is gone and to restore the original state of image, mask and polygons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmAugmentationPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Main.AugmentationPanel = null;
            Main.ForceWorkingImageOriginal();
            Main.UpdateButtonAndMenuEnable();
        }

        /// <summary>
        /// KeyDown event handler for all NumericUpDown controls on the form. Pressing the enter key will apply the change.
        /// </summary>
        /// <param name="sender">Sender of the notfication</param>
        /// <param name="e">Specific event arguments.</param>
        private void nudAny_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnApply_Click(null, null);
        }

        /// <summary>
        /// Apply Button event handler to process the augmentation with the selected values.
        /// </summary>
        /// <param name="sender">Sender of the notfication</param>
        /// <param name="e">Standard event arguments</param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Main.ProcessAugmentation((float)nudZoom.Value, (int)nudTilt.Value, (float)nudBright.Value, (float)nudContrast.Value, (int)nudNoise.Value);
            Enabled = true;
        }
        #endregion Event Handler

    }
}
