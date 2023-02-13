// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;


namespace ImageSegmenter
{
    /// <summary>
    /// Dialog form to edit the application settings.
    /// </summary>
    public partial class frmAppSettings : Form
    {
        /// <summary>Separator definition for arrays in text fields.</summary>
        private const char SEPARATOR = ';';

        /// <summary>Reference to the AppSettings instance of the application.</summary>
        public AppSettings AppSettings;


        /// <summary>
        /// Creates the instance of the dialog form and loads all gui elements from the passed AppSettings reference.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object.</param>
        public frmAppSettings(AppSettings AppSettings)
        {
            InitializeComponent();
            this.AppSettings = new AppSettings(AppSettings);
            LoadSettingsToForm();
        }

        #region Form contents load and save

        /// <summary>
        /// Loads all fields of the AppSettings object to the corresponding GUI elements in the form for display and edit.
        /// </summary>
        private void LoadSettingsToForm()
        {
            tbPathToSourceImages.Text = AppSettings.PathToSourceImages;
            tbPathToSessionData.Text = AppSettings.PathToSessionData;
            tbPathToOutputDatset.Text = AppSettings.PathToOutputDataset;
            tbPathToPredictedMasks.Text = AppSettings.PathToPredictedMasks;
    
            tbSubDirImg.Text = AppSettings.SubDirImg;
            tbSubDirMask.Text = AppSettings.SubDirMask;
            tbSubDirInfo.Text = AppSettings.SubDirInfo;

            tbSubDirTrain.Text = AppSettings.SubDirTrain;
            tbSubDirVal.Text = AppSettings.SubDirVal;
            tbSubDirPredTest.Text = AppSettings.SubDirPredTest;

            tbPrefixImg.Text = AppSettings.PrefixImg;
            tbPrefixMask.Text = AppSettings.PrefixMask;
            tbPrefixInfo.Text = AppSettings.PrefixInfo;

            dtSegmClasses.Rows.Clear();
            foreach(SegmClassDef catDef in AppSettings.SegmClassDefs)
            {
                DataRow row = dtSegmClasses.NewRow();
                row["ID"] = catDef.ID;
                row["Name"] = catDef.Name;
                row["DrawOrder"] = catDef.DrawOrder;
                row["DrawColor"] = catDef.DrawColor;
             
                dtSegmClasses.Rows.Add(row);
            }

            nudImageOutputSizeWidth.Value = (decimal)AppSettings.ImageOutputSize.Width;
            nudImageOutputSizeHeight.Value = (decimal)AppSettings.ImageOutputSize.Height;
            nudTrainValRatio.Value = (decimal)AppSettings.TrainValRatio;
            ckbInfoOutput.Checked = AppSettings.InfoOutput;
            nudMaskDrawOrderMin.Value = (decimal)AppSettings.MaskDrawOrderMin;
            nudMaskDrawOrderMax.Value = (decimal)AppSettings.MaskDrawOrderMax;

            ckbTiltWithZoomOnly.Checked = AppSettings.TiltWithZoomOnly;
            ckbBrightnessContrastExclusive.Checked = AppSettings.BrightnessContrastExclusive;

            tbZoomFactors.Text = FloatVectorToString(AppSettings.ZoomFactors);
            tbTiltAngles.Text = IntVectorToString(AppSettings.TiltAngles);
            tbBrightnessFactors.Text = FloatVectorToString(AppSettings.BrightnessFactors);
            tbContrastEnhancement.Text = FloatVectorToString(AppSettings.ContrastEnhancements);
            tbNoiseAdders.Text = IntVectorToString(AppSettings.NoiseAdders);

            btnPolygonLineColor.BackColor = AppSettings.PolygonLineColor;
            btnPolygonPointColor.BackColor = AppSettings.PolygonPointColor;
            nudPolygonPointSize.Value = (decimal)AppSettings.PolygonPointSize;

            nudDrawMaskTransparency.Value = (decimal)AppSettings.DrawMaskTransparency;
            nudScrollZoneSize.Value = (decimal)AppSettings.ScrollZoneSize;
            nudScrollMoveFactor.Value = (decimal)AppSettings.ScrollMoveFactor;
            nudScrollStartMinCount.Value = (decimal)AppSettings.ScrollStartMinCount;
        }

        /// <summary>
        /// Load the contents of the GUI elements into the AppSettings object to update it with any edits. 
        /// </summary>
        private void LoadFormToSettings()
        {
            AppSettings.PathToSourceImages = tbPathToSourceImages.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.PathToSessionData = tbPathToSessionData.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.PathToOutputDataset = tbPathToOutputDatset.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.PathToPredictedMasks = tbPathToPredictedMasks.Text.TrimEnd(new char[] { '\\' }) + '\\';

            AppSettings.SubDirImg = tbSubDirImg.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.SubDirMask = tbSubDirMask.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.SubDirInfo = tbSubDirInfo.Text.TrimEnd(new char[] { '\\' }) + '\\';

            AppSettings.SubDirTrain = tbSubDirTrain.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.SubDirVal = tbSubDirVal.Text.TrimEnd(new char[] { '\\' }) + '\\';
            AppSettings.SubDirPredTest = tbSubDirPredTest.Text.TrimEnd(new char[] { '\\' }) + '\\';

            AppSettings.PrefixImg = tbPrefixImg.Text;
            AppSettings.PrefixMask = tbPrefixMask.Text;
            AppSettings.PrefixInfo = tbPrefixInfo.Text;

            AppSettings.CheckCreateDataFolders();

            AppSettings.SegmClassDefs = new SegmClassDef[dtSegmClasses.Rows.Count];
            for (int i=0; i< dtSegmClasses.Rows.Count; i++)
            {
                DataRow row = dtSegmClasses.Rows[i];
                int id = Convert.ToInt32(row["ID"]);
                string name = (string)row["Name"];
                int drawOrder = Convert.ToInt32(row["DrawOrder"]);
                Color drawColor = (Color)row["DrawColor"];

                AppSettings.SegmClassDefs[i] = new SegmClassDef(id, name, drawOrder, drawColor);
            }

            AppSettings.ImageOutputSize.Width = (int)nudImageOutputSizeWidth.Value;
            AppSettings.ImageOutputSize.Height = (int)nudImageOutputSizeHeight.Value;
            AppSettings.TrainValRatio = (int)nudTrainValRatio.Value;
            AppSettings.InfoOutput = ckbInfoOutput.Checked;
            AppSettings.MaskDrawOrderMin = (int)nudMaskDrawOrderMin.Value;
            AppSettings.MaskDrawOrderMax = (int)nudMaskDrawOrderMax.Value;

            AppSettings.TiltWithZoomOnly = ckbTiltWithZoomOnly.Checked;
            AppSettings.BrightnessContrastExclusive= ckbBrightnessContrastExclusive.Checked;

            AppSettings.ZoomFactors = StringToFloatVector(tbZoomFactors.Text, AppSettings.ZOOM_FACTOR_MIN, AppSettings.ZOOM_FACTOR_MAX);
            AppSettings.TiltAngles = StringToIntVector(tbTiltAngles.Text, AppSettings.TILT_ANGLE_MIN, AppSettings.TILT_ANGLE_MAX);
            AppSettings.BrightnessFactors = StringToFloatVector(tbBrightnessFactors.Text, AppSettings.BRIGHTNESS_FACTOR_MIN, AppSettings.BRIGHTNESS_FACTOR_MAX);
            AppSettings.ContrastEnhancements = StringToFloatVector(tbContrastEnhancement.Text, AppSettings.CONTRAST_ENHANCEMENT_MIN, AppSettings.CONTRAST_ENHANCEMENT_MAX);
            AppSettings.NoiseAdders = StringToIntVector(tbNoiseAdders.Text, AppSettings.NOISE_ADDER_MIN, AppSettings.NOISE_ADDER_MAX);

            AppSettings.PolygonLineColor = btnPolygonLineColor.BackColor;
            AppSettings.PolygonPointColor = btnPolygonPointColor.BackColor;
            AppSettings.PolygonPointSize = (float)nudPolygonPointSize.Value;

            AppSettings.DrawMaskTransparency = (int)nudDrawMaskTransparency.Value;
            AppSettings.ScrollZoneSize = (float)nudScrollZoneSize.Value;
            AppSettings.ScrollMoveFactor = (float)nudScrollMoveFactor.Value;
            AppSettings.ScrollStartMinCount = (int)nudScrollStartMinCount.Value;
        }
        #endregion Form contents load and save

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

            return s.Trim(new char[] { SEPARATOR,' ' });
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
                if (x[i]<MinLimit)
                {
                    MessageBox.Show(s[i] + " < "+MinLimit.ToString()+"!", "Range Min Error");
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

        #region OK Button Handling
        /// <summary>
        /// Helper function to save a color map to a CSV file, that can be used in the Jupyter notebook later.
        /// </summary>
        /// <param name="FileName">Full name of the file to save to.</param>
        private void SaveColorMapCsv(string FileName)
        {
            StreamWriter sw = new StreamWriter(FileName);
            for (int i = 0; i < 256; i++)
            {
                Color color = Color.White;
                if (i < AppSettings.SegmClassDefs.Length)
                    color = AppSettings.SegmClassDefs[i].DrawColor;
                sw.WriteLine(color.B.ToString() + ',' + color.G.ToString() + ',' + color.R.ToString() + ",255");
            }
            sw.Close();
        }

        /// <summary>
        /// Helper function to save the segmentation class names to a text file that can be used for the SegmClass definition in python.
        /// </summary>
        /// <param name="FileName">Full name of the file to save this text to.</param>
        private void SaveSegmClassNames(string FileName)
        {
            StreamWriter sw = new StreamWriter(FileName);
            foreach (SegmClassDef catDef in AppSettings.SegmClassDefs)
                sw.WriteLine(catDef.Name.ToUpper().Replace(' ', '_') + " = " + catDef.ID.ToString());
            sw.WriteLine();
            foreach (SegmClassDef catDef in AppSettings.SegmClassDefs)
                sw.WriteLine("'"+ catDef.Name + "', \\");
            sw.Close();
        }


        /// <summary>
        /// Ok button handler of the dialog. After some successful validations LoadFormToSettings() will be called to transfer all edits to the AppSettings object and the dialog will be closed with DialogResult.OK.
        /// If the validations failed, the dialog form stays open.
        /// </summary>
        /// <param name="sender">Sender of the notification, normally the OK button.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
            nudMaskDrawOrderMin_Validating(null, cancelEventArgs);
            if (cancelEventArgs.Cancel == true)
            {
                nudMaskDrawOrderMin.Focus();
                return;
            }
                
            nudMaskDrawOrderMax_Validating(null, cancelEventArgs);
            if (cancelEventArgs.Cancel == true)
            {
                nudMaskDrawOrderMax.Focus();
                return;
            }

            LoadFormToSettings();
            SaveColorMapCsv("jetcar_colormap.csv");
            SaveSegmClassNames("segm_classes.txt");
            this.DialogResult = DialogResult.OK;
            Close();
        }
        #endregion OK Button Handling

        #region Path Selections
        /// <summary>
        /// Browse button handler to select the path to the source images via an OpenFolderDialog.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnBrowsePathToSourceImages_Click(object sender, EventArgs e)
        {
            fbdSelectPath.Description = "Select Path To Source Images";
            fbdSelectPath.SelectedPath = tbPathToSourceImages.Text;
            if (fbdSelectPath.ShowDialog() == DialogResult.OK)
                tbPathToSourceImages.Text = fbdSelectPath.SelectedPath;
        }

        /// <summary>
        /// Browse button handler to select the path to the session data via an OpenFolderDialog.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnBrowsePathToSessionData_Click(object sender, EventArgs e)
        {
            fbdSelectPath.Description = "Select Path To Session Data";
            fbdSelectPath.SelectedPath = tbPathToSessionData.Text;
            if (fbdSelectPath.ShowDialog() == DialogResult.OK)
               tbPathToSessionData.Text = fbdSelectPath.SelectedPath;
        }

        /// <summary>
        /// Browse button handler to select the path to the output data set via an OpenFolderDialog.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnBrowsePathToOutputDataset_Click(object sender, EventArgs e)
        {
            fbdSelectPath.Description = "Select Path To Output Dataset";
            fbdSelectPath.SelectedPath = tbPathToOutputDatset.Text;
            if (fbdSelectPath.ShowDialog() == DialogResult.OK)
                tbPathToOutputDatset.Text = fbdSelectPath.SelectedPath;
        }

        /// <summary>
        /// Browse button handler to select the path to the prediction masks via an OpenFolderDialog.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnBrowsePathToPredictedMasks_Click(object sender, EventArgs e)
        {
            fbdSelectPath.Description = "Select Path To Predicted Masks";
            fbdSelectPath.SelectedPath = tbPathToPredictedMasks.Text;
            if (fbdSelectPath.ShowDialog() == DialogResult.OK)
                tbPathToPredictedMasks.Text = fbdSelectPath.SelectedPath;
        }
        #endregion Path Selections

        #region DataGridView events
        /// <summary>
        /// Event handler of the DataGridView CellClick events. In this case, clicking on a cell with color selection opens up a ColorDialog for choosing a different color. 
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments</param>
        private void dgvSegmClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 3) && (e.RowIndex >= 0))
            {
                colorDialog.Color = (Color)dgvSegmClasses[e.ColumnIndex, e.RowIndex].Value;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    dgvSegmClasses[e.ColumnIndex, e.RowIndex].Value = colorDialog.Color;
                }
            }         
        }

        /// <summary>
        /// The DataGridView CellFormatting event is used to create transparent cells where a color was assigned.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments</param>
        private void dgvSegmClasses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == 3) && (e.RowIndex >= 0) && (e.Value != null))
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
        private void dgvSegmClasses_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == 3) && (e.RowIndex >= 0) && (e.Value != null))
            {
                e.Graphics.FillRectangle(new SolidBrush(dgvSegmClasses.BackgroundColor), new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height));
                e.Graphics.FillRectangle(new SolidBrush((Color)e.Value), new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4, e.CellBounds.Width - 8, e.CellBounds.Height - 8));
            }
        }

        /// <summary>
        /// The DataGridView DataError is used to show integer conversion errors in the DrawOrder column.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void dgvSegmClasses_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if ((e.ColumnIndex == 2) && (e.RowIndex >= 0))
                MessageBox.Show("Not an integer!", "Data Error");
        }
        #endregion DataGridView events

        #region Draw order min max validation
        /// <summary>
        /// Determins the minimum and maximum of the values in the DrawOrder column and returns these values.
        /// </summary>
        /// <param name="Min">Resulting minimum value.</param>
        /// <param name="Max">Resulting maximum value.</param>
        private void GetDrawOrderMinMax(out int Min, out int Max)
        {
            Min = int.MaxValue;
            Max = int.MinValue;
            for (int i = 0; i < dtSegmClasses.Rows.Count; i++)
            {
                DataRow row = dtSegmClasses.Rows[i];
                int drawOrder = Convert.ToInt32(row["DrawOrder"]);

                Min = Math.Min(Min, drawOrder);
                Max = Math.Max(Max, drawOrder);
            }
        }

        /// <summary>
        /// Validating event handler of the NumericUpDown for the draw order minimum setting. It makes sure that the selected minimum is not bigger than the maximum.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void nudMaskDrawOrderMin_Validating(object sender, CancelEventArgs e)
        {
            int min, max;
            GetDrawOrderMinMax(out min, out max);
            if (nudMaskDrawOrderMin.Value > max)
            {
                MessageBox.Show("Value is greater than maximum in SegmClass Definitions resulting in empty masks!", "Mask Draw Order Min Error");
                e.Cancel = true;
            }
            else if (nudMaskDrawOrderMin.Value > nudMaskDrawOrderMax.Value)
            {
                MessageBox.Show("Value is greater than \"Mask Draw Order Max\" value resulting in empty masks!", "Mask Draw Order Min Error");
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Validating event handler of the NumericUpDown for the draw order maximum setting. It makes sure that the selected maximum is not smaller than the minimum.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void nudMaskDrawOrderMax_Validating(object sender, CancelEventArgs e)
        {
            int min, max;
            GetDrawOrderMinMax(out min, out max);
            if (nudMaskDrawOrderMax.Value < min)
            {
                MessageBox.Show("Value is less than minimum in SegmClass Definitions resulting in empty masks!", "Mask Draw Order Max Error");
                e.Cancel = true;
            }
            else if (nudMaskDrawOrderMax.Value < nudMaskDrawOrderMin.Value)
            {
                MessageBox.Show("Value is less than \"Mask Draw Order Min\" value resulting in empty masks!", "Mask Draw Order Max Error");
                e.Cancel = true;
            }
        }
        #endregion Draw order min max validation

        #region Polygon Color Selection
        /// <summary>
        /// Polygon line color button event handler to open a ColorDialog for the selection.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnPolygonLineColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = btnPolygonLineColor.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                btnPolygonLineColor.BackColor = colorDialog.Color;

        }

        /// <summary>
        /// Polygon point color button event handler to open a ColorDialog for the selection.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnPolygonPointColor_Click(object sender, EventArgs e)
        {
            colorDialog.Color = btnPolygonPointColor.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                btnPolygonPointColor.BackColor = colorDialog.Color;
        }
        #endregion Polygon Color Selection

        #region Vector text box validations
        /// <summary>
        /// Validation event handler of the TextBox contents for the zoom factors. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbZoomFactors_Validating(object sender, CancelEventArgs e)
        {
            if (StringToFloatVector(tbZoomFactors.Text, AppSettings.ZOOM_FACTOR_MIN, AppSettings.ZOOM_FACTOR_MAX) == null)
                e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the tilt angles. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbTiltAngles_Validating(object sender, CancelEventArgs e)
        {
            if (StringToIntVector(tbTiltAngles.Text, AppSettings.TILT_ANGLE_MIN, AppSettings.TILT_ANGLE_MAX) == null)
                e.Cancel = true;
        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the brightness factors. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbBrightnessFactors_Validating(object sender, CancelEventArgs e)
        {
            if (StringToFloatVector(tbBrightnessFactors.Text, AppSettings.BRIGHTNESS_FACTOR_MIN, AppSettings.BRIGHTNESS_FACTOR_MAX) == null)
                e.Cancel = true;

        }

        /// <summary>
        /// Validation event handler of the TextBox contents for the contrast enhancements. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbContrastEnhancement_Validating(object sender, CancelEventArgs e)
        {
            if (StringToFloatVector(tbContrastEnhancement.Text, AppSettings.CONTRAST_ENHANCEMENT_MIN, AppSettings.CONTRAST_ENHANCEMENT_MAX) == null)
                e.Cancel = true;
        }


        /// <summary>
        /// Validation event handler of the TextBox contents for the noise adders. The focus will stay on the cell until the conversion to a vector executes correctly.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void tbNoiseAdders_Validating(object sender, CancelEventArgs e)
        {
            if (StringToIntVector(tbNoiseAdders.Text, AppSettings.NOISE_ADDER_MIN, AppSettings.NOISE_ADDER_MAX) == null)
                e.Cancel = true;
        }
        #endregion Vector text box validations

    }
}
