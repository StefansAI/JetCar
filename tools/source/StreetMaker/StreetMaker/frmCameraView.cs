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
using System.IO;
using System.Drawing.Imaging;


namespace StreetMaker
{
    /// <summary>
    /// Form to display the camera views of image and class color bitmaps.
    /// </summary>
    public partial class frmCameraView : Form
    {
        #region Private Fields
        /// <summary>Reference to the main form object.</summary>
        private frmStreetMakerMain MainForm;
        /// <summary>Full path to the predicted images in FileMode.</summary>
        private string PredPath;
        /// <summary>Full path to the predicted images in FileMode for comparison.</summary>
        private string PredPathCmp;
        /// <summary>Array of all file names in the folder.</summary>
        private List<string> ImgFileNames;
        /// <summary>Current index of the image to be displayed.</summary>
        private int ImgIdx;
        /// <summary>Array of colors to be assigned to the mask codes in FileMode</summary>
        private Color[] colorPalette;
        /// <summary>Array of strings to map code values to names.</summary>
        private string[] classNames;
        /// <summary>Reference to the current code mask object.</summary>
        private Bitmap codeMask;
        /// <summary>Reference to the current prediction mask object</summary>
        private Bitmap predMask;
        /// <summary>Reference to the current prediction mask object for comparison</summary>
        private Bitmap predMaskCmp;
        /// <summary>True for special mode displaying the virtual camera from a view point.</summary>
        private bool viewPointMode;
        #endregion Private Fields

        #region Constructors
        /// <summary>
        /// Creates an instance of this form. The form will be initialized with FileMode=false, without buttons for loading bitmaps via BitmapLeft and BitmapRight properties.
        /// </summary>
        /// <param name="MainForm">Reference to the main form object.</param>
        public frmCameraView(frmStreetMakerMain MainForm)
        {
            InitializeComponent();
            this.MainForm = MainForm;
            PredPath = null;
            ImgFileNames = new List<string>();
            FileMode = false;
            viewPointMode = false;
            codeMask = null;
            predMask = null;
            predMaskCmp = null;
            SetPredictionVisible(0);
        }

        /// <summary>
        /// Creates an instance of this form. The form will be initialized with FileMode=true to load files from the dataset and navigating via the buttons.
        /// </summary>
        /// <param name="MainForm">Reference to the main form object.</param>
        /// <param name="SubDirs">Subdirectory name inside the image and mask folders, like "train" or "val"</param>
        public frmCameraView(frmStreetMakerMain MainForm, string[] SubDirs):this(MainForm)
        {
            FileMode = true;
            codeMask = null;
            predMask = null;
            predMaskCmp = null;
            string imgPath = MainForm.AppSettings.PathToDataStorage + MainForm.AppSettings.SubDirDataSet + MainForm.AppSettings.SubDirImg;
            PredPath = MainForm.AppSettings.PathToDataStorage + MainForm.AppSettings.SubDirDataSet + MainForm.AppSettings.SubDirPred;
            PredPathCmp = PredPath.TrimEnd(new char[] { '\\' }) + "Cmp";

            LoadPalette(MainForm.AppSettings.PathToDataStorage + MainForm.AppSettings.ColorMapFileName);
            LoadClassNames(MainForm.AppSettings.PathToDataStorage + MainForm.AppSettings.ClassTextFileName);

            foreach (string subDir in SubDirs)
                ImgFileNames.AddRange(Directory.GetFiles(imgPath+subDir));
            LoadImgAndMask(0);
        }
        #endregion Constructors

        #region Private Methods
        /// <summary>
        /// Form closing event used to clear the reference to this form in the main form.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void frmCameraView_FormClosing(object sender, FormClosingEventArgs e)
        {
            BitmapCameraView = null;
            BitmapMaskImage = null;
            BitmapPrediction = null;
            if (codeMask != null)
                codeMask.Dispose();

            MainForm.frmCameraView = null;
            e.Cancel = false;
        }

        /// <summary>
        /// Load the color values for all active class codes from the csv file stored at dataset generation.
        /// </summary>
        /// <param name="FileName">Full file name to the csv file containing the RGB values for the used classes.</param>
        private void LoadPalette(string FileName)
        {
            ColorPalette = new Color[256];
            for (int i = 0; i < 256; i++)
                ColorPalette[i] = Color.FromArgb(i, i, i);
            
            if (File.Exists(FileName))
            { 
                int idx = 0;
                StreamReader sr = new StreamReader(FileName);
                while (!sr.EndOfStream)
                {
                    string[] s = sr.ReadLine().Split(new char[] { ',' });
                    Color color = Color.FromArgb(Convert.ToInt32(s[2]), Convert.ToInt32(s[1]), Convert.ToInt32(s[0]));
                    ColorPalette[idx++] = color;
                }
                sr.Close();
            }
        }

        /// <summary>
        /// Load the class text file to get the names of all classes together with their code.
        /// </summary>
        /// <param name="FileName">Full path and name of the text file to load.</param>
        private void LoadClassNames(string FileName)
        {
            classNames = new string[256];
            for (int i = 0; i < 256; i++)
                classNames[i] = "Unknown_#" + i.ToString();

            if (File.Exists(FileName))
            {
                bool in_enum = false;
                int n = 0;
                StreamReader sr = new StreamReader(FileName);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.StartsWith("N_CLASSES"))
                    {
                        string[] s = line.Split(new char[] { '=' });
                        n = Convert.ToInt32(s[1].Trim());
                    }
                    else if (line.StartsWith("class SegmClass"))
                        in_enum = true;
                    else if (line.Contains("#Unused"))
                        in_enum = false;
                    else if (in_enum == true)
                    {
                        string[] s = line.Split(new char[] { '=' });
                        int idx = Convert.ToInt32(s[1].Trim());
                        classNames[idx] = s[0].Trim();
                     }
                }
                sr.Close();
            }
        }

        /// <summary>
        /// Set the prediction image visible or invisible and adjust form size accordingly.
        /// </summary>
        /// <param name="PredCount">0 for none, 1 or 2 to set the prediction images visible.</param>
        private void SetPredictionVisible(int PredCount)
        {
            switch (PredCount)
            {
                case 0:
                    ClientSize = new Size(pbMaskImage.Left + pbMaskImage.Width + 5, ClientSize.Height);
                    break;
                case 1:
                    ClientSize = new Size(pbPredictionImage.Left + pbPredictionImage.Width + 5, ClientSize.Height);
                    break;
                case 2:
                    ClientSize = new Size(pbPredictionImageCmp.Left + pbPredictionImageCmp.Width + 5, ClientSize.Height);
                    break;
            }
            pbPredictionImage.Visible = PredCount >= 1;
            pbPredictionImageCmp.Visible = PredCount >= 2;

            pnNavigation.Left = (ClientSize.Width - pnButtons.Width) / 2;
        }

        /// <summary>
        /// Load the 2 bitmaps from jpg and png files stored in the dataset folders for camera view and related class code mask.
        /// </summary>
        /// <param name="Idx">Index in the file list of the camera view to be used for file pair selection.</param>
        private void LoadImgAndMask(int Idx)
        {
            if (Idx < ImgFileNames.Count)
            {
                ImgIdx = Math.Min(Math.Max(Idx, 0), ImgFileNames.Count - 1);
                lbIdx.Text = ImgIdx.ToString();

                string fullImgFileName = ImgFileNames[ImgIdx];
                try
                {
                    Bitmap bmFile = (Bitmap)Bitmap.FromFile(fullImgFileName);
                    BitmapCameraView = new Bitmap(bmFile);
                    bmFile.Dispose();
                }
                catch
                {
                    BitmapCameraView = new Bitmap(MainForm.AppSettings.CameraOutputWidth, MainForm.AppSettings.CameraOutputHeight);
                }
                string imgFileName = Path.GetFileNameWithoutExtension(fullImgFileName).ToLower();
                lbStatus.Text = imgFileName;

                string maskFileName = imgFileName.Replace(MainForm.AppSettings.PrefixImg.ToLower(), MainForm.AppSettings.PrefixMask.ToLower());
                string[] s = Path.GetDirectoryName(fullImgFileName).Split(new char[] { Path.DirectorySeparatorChar });
                s[s.Length - 2] = MainForm.AppSettings.SubDirMask;
                string maskPath = Path.Combine(s).Replace(":", ":" + Path.DirectorySeparatorChar);
                maskFileName = maskPath + Path.DirectorySeparatorChar + maskFileName + ".png";
                try
                {
                    codeMask = (Bitmap)Bitmap.FromFile(maskFileName);                  
                }
                catch
                {
                    codeMask = new Bitmap(MainForm.AppSettings.CameraOutputWidth, MainForm.AppSettings.CameraOutputHeight, PixelFormat.Format8bppIndexed);
                }
                BitmapMaskImage = Process.ImageColorMap(codeMask, ColorPalette, 255);

                string predFileName = imgFileName.Replace(MainForm.AppSettings.PrefixImg.ToLower(), MainForm.AppSettings.PrefixPred.ToLower());
                string predFullFileName = PredPath + Path.DirectorySeparatorChar + predFileName + ".png";
                string predFullFileNameCmp = PredPathCmp + Path.DirectorySeparatorChar + predFileName + ".png";
                try
                {
                    predMask = (Bitmap)Bitmap.FromFile(predFullFileName);
                    BitmapPrediction = Process.ImageColorMap(predMask, ColorPalette, 255);
                    SetPredictionVisible(1);
                    lbPredCursor.Visible = true;

                    try
                    {
                        predMaskCmp = (Bitmap)Bitmap.FromFile(predFullFileNameCmp);
                        BitmapPredictionCmp = Process.ImageColorMap(predMaskCmp, ColorPalette, 255);
                        SetPredictionVisible(2);
                        lbPredCursorCmp.Visible = true;

                    }
                    catch
                    {
                        predMaskCmp = null;
                        BitmapPredictionCmp = null;
                        SetPredictionVisible(1);
                        lbPredCursorCmp.Visible = false;
                    }
                }
                catch
                {
                    predMask = null;
                    BitmapPrediction = null;
                    SetPredictionVisible(0);
                    lbPredCursor.Visible = false;
                    predMaskCmp = null;
                    BitmapPredictionCmp = null;
                    lbPredCursorCmp.Visible = false;

                }
            }
        }


        /// <summary>
        /// Button click event handler to select the first image pair in the file list.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnFirst_Click(object sender, EventArgs e)
        {
            LoadImgAndMask(0);
        }

        /// <summary>
        /// Button click event handler to select the previous image pair in the file list.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            LoadImgAndMask(ImgIdx-1);
        }

        /// <summary>
        /// Button click event handler to select the next image pair in the file list.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            LoadImgAndMask(ImgIdx + 1);

            if (ImgIdx >= (ImgFileNames.Count - 1))
                ckbScan.Checked = false;
        }

        /// <summary>
        /// Button click event handler to select the last image pair in the file list.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void btnLast_Click(object sender, EventArgs e)
        {
            LoadImgAndMask(ImgFileNames.Count - 1);
        }

        /// <summary>
        /// Checkbox change event handler to start or stop the scan timer.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void ckbScan_CheckedChanged(object sender, EventArgs e)
        {
            tmScan.Enabled = ckbScan.Checked;
        }


        /// <summary>
        /// Synchronized MouseMove event for all 3 views to display the value of the image or mask in the labels below the picture boxes.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void pbCameraImg_MouseMove(object sender, MouseEventArgs e)
        {
            if ((FileMode == true) || (viewPointMode == true))
                try
                {
                    // it is assumed, that all 3 bitmaps have the same size and all 3 picture boxes have their same size.
                    float xfactor = (float)BitmapCameraView.Width / pbCameraImg.Width;
                    float yfactor = (float)BitmapCameraView.Height / pbCameraImg.Height;
                    int x = Math.Min(Math.Max((int)(e.X * xfactor), 0), BitmapCameraView.Width - 1);
                    int y = Math.Min(Math.Max((int)(e.Y * yfactor), 0), BitmapCameraView.Height - 1);

                    Color color = BitmapCameraView.GetPixel(x, y);
                    lbImgCursor.Text = "Img: x=" + x.ToString() + "  y=" + y.ToString() + "  R=" + color.R.ToString() + " G=" + color.G.ToString() + "  B=" + color.B.ToString();

                    int maskCode = Process.Get8bitValue(codeMask, x, y);
                    string maskCodeName = classNames[maskCode];
                    lbMaskCursor.Text = "Mask: x=" + x.ToString() + "  y=" + y.ToString() + "  code=" + maskCode.ToString() + "  name=" + maskCodeName;

                    if (predMask != null)
                    {
                        int predCode = predMask.GetPixel(x, y).R; ;
                        string predCodeName = classNames[predCode];
                        lbPredCursor.Text = "Pred: x=" + x.ToString() + "  y=" + y.ToString() + "  code=" + predCode.ToString() + "  name=" + predCodeName;

                        if (maskCode != predCode)
                            lbMaskCursor.ForeColor = Color.Red;
                        else
                            lbMaskCursor.ForeColor = Color.Black;

                        if (predMaskCmp != null)
                        {
                            int predCodeCmp = predMaskCmp.GetPixel(x, y).R; ;
                            string predCodeCmpName = classNames[predCodeCmp];
                            lbPredCursorCmp.Text = "Pred: x=" + x.ToString() + "  y=" + y.ToString() + "  code=" + predCodeCmp.ToString() + "  name=" + predCodeCmpName;

                            if (maskCode != predCodeCmp)
                                lbPredCursorCmp.ForeColor = Color.Red;
                            else
                                lbPredCursorCmp.ForeColor = Color.Black;

                        }

                    }
                    else lbMaskCursor.ForeColor = Color.Black;

                    lbPredCursor.ForeColor = lbMaskCursor.ForeColor;
                }
                catch { pbCameraImg_MouseLeave(null, null); }
        }

        /// <summary>
        /// Mouse leave event handler of the picture boxes to clear the texts.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Event arguments.</param>
        private void pbCameraImg_MouseLeave(object sender, EventArgs e)
        {
            lbImgCursor.Text = "";
            lbMaskCursor.Text = "";
            lbPredCursor.Text = "";
            lbPredCursorCmp.Text = "";
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Adds the file name including full path to a list for later navigation and displays the file name without path in the status text.
        /// </summary>
        /// <param name="FullFileName">Full path and file name.</param>
        public void AddFileName(string FullFileName)
        {
            ImgFileNames.Add(FullFileName);
            StatusText = Path.GetFileName(FullFileName);
        }

        /// <summary>
        /// Switches from display mode without buttons to navigation mode with buttons and selects the first image in the list.
        /// </summary>
        public void SwitchToNavigation()
        {
            FileMode = true;
            prbGenerationProgress.Visible = false;
            LoadClassNames(MainForm.AppSettings.PathToDataStorage + MainForm.AppSettings.ClassTextFileName);
            LoadImgAndMask(0);
        }
        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Gets or sets the reference to the bitmap to be displayed on the left side as camera view. When a new reference is set, the previously assigned bitmap will be disposed.
        /// </summary>
        public Bitmap BitmapCameraView
        {
            get { return (Bitmap)pbCameraImg.Image; }
            set
            {
                Bitmap bmOld = (Bitmap)pbCameraImg.Image;
                pbCameraImg.Image = value;
                if (bmOld != null)
                    bmOld.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the bitmap to be displayed on the right of the camera view as mask image. When a new reference is set, the previously assigned bitmap will be disposed.
        /// </summary>
        public Bitmap BitmapMaskImage
        {
            get { return (Bitmap)pbMaskImage.Image; }
            set
            {
                Bitmap bmOld = (Bitmap)pbMaskImage.Image;
                pbMaskImage.Image = value;
                if (bmOld != null)
                    bmOld.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets the 8-bit code mask used to display cursor values when moving the mouse over image positions..
        /// </summary>
        public Bitmap CodeMask
        { 
            get { return codeMask; }
            set
            {
                Bitmap bmOld = codeMask;
                codeMask = value;
                if (bmOld != null)
                    bmOld.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the bitmap to be displayed on the very right side as predicted mask image. When a new reference is set, the previously assigned bitmap will be disposed.
        /// </summary>
        public Bitmap BitmapPrediction
        {
            get { return (Bitmap)pbPredictionImage.Image; }
            set
            {
                Bitmap bmOld = (Bitmap)pbPredictionImage.Image;
                pbPredictionImage.Image = value;
                if (bmOld != null)
                    bmOld.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets the reference to the bitmap to be displayed on the very right side as compare predicted mask image. When a new reference is set, the previously assigned bitmap will be disposed.
        /// </summary>
        public Bitmap BitmapPredictionCmp
        {
            get { return (Bitmap)pbPredictionImageCmp.Image; }
            set
            {
                Bitmap bmOld = (Bitmap)pbPredictionImageCmp.Image;
                pbPredictionImageCmp.Image = value;
                if (bmOld != null)
                    bmOld.Dispose();
            }
        }

        /// <summary>
        /// Get or set the file mode of this form. If true, the form displays the navigation buttons to fetch the bitmaps from files in the dataset. 
        /// If false, the properties BitmapLeft and BitmapRight can be used to display images.
        /// </summary>
        public bool FileMode
        {
            get { return pnNavigation.Visible; }
            set
            {
                pnNavigation.Visible = value;
                lbStatus.Text = "";
                if (value)
                    viewPointMode = false;

            }
        }

        /// <summary>
        /// Get or set the view point mode of the form. This mode is used to display a view from a specific point and the direction angle including mouse move class displays.
        /// </summary>
        public bool ViewPointMode
        {
            get { return viewPointMode; }
            set
            {
                viewPointMode = value;
                if (value)
                    FileMode = false;
            }
        }

        /// <summary>
        /// Gets or sets the status text under the bitmaps in FileMode=false mode.
        /// </summary>
        public string StatusText
        { 
            get { return lbStatus.Text; } 
            set { lbStatus.Text = value; }
        }

        /// <summary>
        /// Gets or sets the visibility of the prediction pictures. When set to 0, 1 or 2, the form size will be adjusted.
        /// </summary>
        public int PredictionVisibleCount
        {
            set
            {
                SetPredictionVisible(value);
            }
        }

        /// <summary>
        /// Gets or sets the ColorPalette for the class images
        /// </summary>
        public Color[] ColorPalette
        {
            get { return colorPalette; }
            set
            {
                if (value.Length == 256)
                    colorPalette = value;
            }
        }

        /// <summary>
        /// Gets or sets the class names array of strings containing the names of the classes
        /// </summary>
        public string[] ClassNames
        {
            get { return classNames; }
            set
            {
                if (value.Length == 256)
                    classNames = value;
            }
        }

        /// <summary>
        /// Displays the count on the lbCount label.
        /// </summary>
        public int Count
        {
            set
            {
                if (value > 0)
                {
                    lbCount.Text = "Dataset entries = " + value.ToString() + " / " + prbGenerationProgress.Maximum.ToString();
                    prbGenerationProgress.Value = value;
                }
                else
                {
                    lbCount.Text = "";
                    prbGenerationProgress.Visible = false;
                }
            }
        }

        /// <summary>
        /// Set the Maximum of the progress bar.
        /// </summary>
        public int MaxCount
        {
            set
            {
                prbGenerationProgress.Value = 0;
                prbGenerationProgress.Maximum = value;
                prbGenerationProgress.Visible = true;
            }
        }

        #endregion Public Properties

    }
}
