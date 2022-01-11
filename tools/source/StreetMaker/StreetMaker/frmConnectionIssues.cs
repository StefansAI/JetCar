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
    /// Form to display a list of connection issues.
    /// </summary>
    public partial class frmConnectionIssues : Form
    {
        /// <summary>Reference to the main form of the application.</summary>
        private frmStreetMakerMain MainForm;

        /// <summary>
        /// Creates the instance of the connection issue form.
        /// </summary>
        /// <param name="MainForm">Reference to the main form of the application.</param>
        public frmConnectionIssues(frmStreetMakerMain MainForm)
        {
            InitializeComponent();
            this.MainForm = MainForm;
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Load the complete issue list and convert to text lines added into the display list.
        /// </summary>
        public void LoadIssues()
        {
            listBox1.Items.Clear();
            foreach(ConnectionIssue ci in MainForm.StreetMap.ConnectionIssues)
            {
                string s = "dist:" + ci.Distance.ToString("F3") + "  angle:" + ci.Angle.ToString("F1") + 
                            "  dx:" + ci.dx.ToString("F3") + "  dy:" + ci.dy.ToString("F3") + "  d_angle:" + Utils.ToDegree(ci.C1.Angle- ci.C0.Angle).ToString("F3") +
                            "   C0:" + ci.C0.CenterP.ToString() + "  C1:" + ci.C1.CenterP.ToString() + "  connected:" + ci.Connected.ToString();
                listBox1.Items.Add(s);
            }

            if (listBox1.Items.Count > 0)
            {
                if (listBox1.SelectedIndex == 0)
                    listBox1_SelectedIndexChanged(null,null);
                else
                    listBox1.SelectedIndex = 0;
            }
            this.Cursor = Cursors.Default;
        }


        /// <summary>
        /// Event handler to highlight the issue in the map just clicked on.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.StreetMap.SetActiveConnectionIssue(listBox1.SelectedIndex);
        }

        /// <summary>
        /// Form closed event to clear the reference in the main form.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void frmConnectionIssues_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.frmConnectionIssues = null;
        }
    }
}
