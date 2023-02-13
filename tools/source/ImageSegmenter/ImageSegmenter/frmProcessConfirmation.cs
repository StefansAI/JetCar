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
    /// Dialog form for some selections for processing all augmentations. 
    /// </summary>
    public partial class frmProcessConfirmation : Form
    {
        /// <summary>
        /// Creates the dialog form and assigns the passed maximum value as maximum to the start index selection.
        /// </summary>
        /// <param name="MaxNumber">Maximum number of processed images available</param>
        public frmProcessConfirmation(int MaxNumber)
        {
            InitializeComponent();
            nudEndImageNumber.Value = (decimal)MaxNumber;
            nudEndImageNumber.Maximum = (decimal)MaxNumber;
        }

        /// <summary>
        /// Start number change handler to adjust end minimum.
        /// </summary>
        /// <param name="sender">Sender of the notification</param>
        /// <param name="e">Standard event arguments.</param>
        private void nudStartImageNumber_ValueChanged(object sender, EventArgs e)
        {
            nudEndImageNumber.Minimum = nudStartImageNumber.Value;
        }

        /// <summary>
        /// End number change handler to adjust end minimum.
        /// </summary>
        /// <param name="sender">Sender of the notification</param>
        /// <param name="e">Standard event arguments.</param>
        private void nudEndImageNumber_ValueChanged(object sender, EventArgs e)
        {
            nudStartImageNumber.Maximum = nudEndImageNumber.Value;
        }

        /// <summary>
        /// Gets the image index to start with.
        /// </summary>
        public int StartImageNumber
        {
            get { return (int)nudStartImageNumber.Value;  }
        }

        /// <summary>
        /// Gets the image index to end with.
        /// </summary>
        public int EndImageNumber
        {
            get { return (int)nudEndImageNumber.Value; }
        }

        /// <summary>
        /// Gets the checkbox checked value to clear all output directories first.
        /// </summary>
        public bool CLearAllOutputDirs
        {
            get { return ckbClearOutputDirs.Checked;  }
        }

        /// <summary>
        /// OK button handler of the dialog to close the form.
        /// </summary>
        /// <param name="sender">Sender of the notification</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

     }
}
