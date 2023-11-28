// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ImageSegmenter
{
    /// <summary>
    /// Main form of the ImageSegmenter application.
    /// </summary>
    public partial class frmImageSegmenterMain : Form
    {
        #region Private Constants
        /// <summary>Application name to be shown in the form text.</summary>
        private const string APP_NAME = "Image Segmenter";
        /// <summary>File name for the application settings.</summary>
        private const string APP_SETTINGS_FILE_NAME = "ImageSegmenter.Cfg";
        /// <summary>When true, the original images, normally never changed, will be saved after blurring is applied.</summary>
        private const bool SAVE_BLURED_ORGINAL_IMAGE = false;
        #endregion Private Constants

        #region Private Fields
        /// <summary>Reference to AppSettings object.</summary>
        private AppSettings AppSettings;
        /// <summary>Reference to the Session object.</summary>
        private Session Session;

        /// <summary>Bitmap object with the original image contents loaded from a file via the Session object. This image can be much larger than the output requires.</summary>
        private Bitmap bmOriginalImg;
        /// <summary>Bitmap object with an image, that may have been modified from the original image (bmOriginalImg).</summary>
        private Bitmap bmWorkingImg;
        /// <summary>Downsampled version of bmWoorkingImg to fit the output size.</summary>
        private Bitmap bmTargetSizeImg;
        /// <summary>Image made from drawing the filled polygons in their colors to display the rsulting segmentaion mask.</summary>
        private Bitmap bmImageMask;
        /// <summary>Bitmap with only filled polygons drawn as segmentation class code only to convert into a PNG file later.</summary>
        private Bitmap bmCodeMask;
        /// <summary>bmCodeMask reduced to PixelFormat.Format8bppIndexed for saving as PNG file.</summary>
        private Bitmap bm8bitMask;
        /// <summary>Bitmap read from a prediction PNG file, which is 24bit RGB with the segmentation class codes in R,G,B.</summary>
        private Bitmap bmPredMaskCode;
        /// <summary>bmPredMaskCode re-colored to segmentation class colors for display.</summary>
        private Bitmap bmPredMaskColored;
        /// <summary>Zoom factor for the left image display. Zoom is controlled via mouse wheel.</summary>
        private float ZoomLeft;
        /// <summary>Minimum for ZoomLeft</summary>
        private float ZoomLeftMin;
        /// <summary>Mouse coordinates in the left side image.</summary>
        private Point CoordLeft;
        /// <summary>Zoom factor for the right image display. Zoom is controlled via mouse wheel.</summary>
        private float ZoomRight;
        /// <summary>Minimum for ZoomLeft</summary>
        private float ZoomRightMin;
        /// <summary>Mouse coordinates in the right side image.</summary>
        private Point CoordRight;

        /// <summary>Normalized polygon currently edited on the left side.</summary>
        private PointF[] NormalizedPolygon;
        /// <summary>Scaled version of NormalizedPolygon to draw on the left side while editing.</summary>
        private PointF[] DrawPolygon;
        /// <summary></summary>
        private bool SegmClassesEdited;
        /// <summary>True, when a new polygon is in the process of beeing entered on the left side.</summary>
        private bool EnterNewPolygon;
        /// <summary>True, when the currently selected polygon is edited on the left side.</summary>
        private bool EditCurrentPolygon;
        /// <summary>True, when the currently selected polygon is to be highlighted as filled out area on the left side</summary>
        private bool HighlightCurrentPolygon;
        /// <summary>Index of the currently selected polygon.</summary>
        private int HighlightCurrentPolygonIdx;
        /// <summary>Index of the point of the currently selected polygon to be edited.</summary>
        private int EditPolygonPointIdx;
        /// <summary>Delay count up counter before scrolling is activated on the left side. This delay allows moving the mouse quickly over the scroll bounds to other GUI elements without immediately triggering scrolling.</summary>
        private int LeftScrollZoneCount;
        /// <summary>Delay count up counter before scrolling is activated on the right side. This delay allows moving the mouse quickly over the scroll bounds to other GUI elements without immediately triggering scrolling.</summary>
        private int RightScrollZoneCount;
        /// <summary>Holds the KeyCode of any key pressed until released.</summary>
        private Keys CurrentKeyDown;
        /// <summary>Index of the WorkingSteeringDirs entry currently edited.</summary>
        private int EditSteeringDirIdx;

        /// <summary>List of the original segmentaion class objects for this image as for instance loaded from an XML file.</summary>
        private List<SegmClass> OriginalSegmClasses;
        /// <summary>Working version of the segmentation class list can be identical to OriginalSegmClasses or modified through augmentaion, editing etc.</summary>
        private List<SegmClass> WorkingSegmClasses;

        /// <summary>Original steering directions for this image for instance loaded from file.</summary>
        private SteeringDirection[] OriginalSteeringDirs;
        /// <summary>Working version of OriginalSteeringDirs can be identical to OriginalSteeringDirs or modified through augmentaion, editing etc.</summary>
        private SteeringDirection[]  WorkingSteeringDirs;

        /// <summary>Refernce to the augmentation panel object when open or null.</summary>
        internal frmAugmentationPanel AugmentationPanel;

        /// <summary></summary>
        private Bitmap bmAugmentationInput;
        private Bitmap bmTiltZoomResult;
        private Bitmap bmTiltZoomResultTargetSize;
        private Bitmap bmBrightnessResult;
        private Bitmap bmContrastResult;
        private float lastZoomFactor;
        private int lastTiltAngle;
        private float lastBrightnessFactor;
        private float lastContrastEnhancement;

        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the main form fro this application.
        /// </summary>
        public frmImageSegmenterMain()
        {
            InitializeComponent();

            AppSettings = new AppSettings(Application.StartupPath + "\\"+ APP_SETTINGS_FILE_NAME);
            Session = new Session(AppSettings.PathToSourceImages, AppSettings.PathToSessionData);

            for (int i = 0; i < AppSettings.SegmClassDefs.Length; i++)
                cbSegmClass.Items.Add(AppSettings.SegmClassDefs[i].Name);

            cbSegmClass.SelectedIndex = 0;
            OriginalSegmClasses = new List<SegmClass>();
            WorkingSegmClasses = OriginalSegmClasses;
            OriginalSteeringDirs = InitialSteeringDirs();
            WorkingSteeringDirs = OriginalSteeringDirs;

            pbLeft.MouseWheel += new MouseEventHandler(pbLeft_MouseWheel);
            ZoomLeft = 1;
            ZoomLeftMin = 1;
            CoordLeft = new Point(-1, -1);
            EditPolygonPointIdx = -1;
            CurrentKeyDown = Keys.None;
            EditSteeringDirIdx=-1;
        
            pbRight.MouseWheel += new MouseEventHandler(pbRight_MouseWheel);
            pbRight.BackColor = Color.Transparent;
            ZoomRight = 1;
            ZoomRightMin = 1;
            CoordRight = new Point(-1, -1);

            tsmiNext.Enabled = (Session.CurrentImageIdx < Session.ProcessedCount) || (Session.RemainingCount > 0);
            tsmiPrevious.Enabled = Session.CurrentImageIdx > 0;
            lbCurrentImageIdx.Text = Session.CurrentImageIdx.ToString();
            lbProcessedCount.Text = Session.ProcessedCount.ToString();
            lbRemainingCount.Text = Session.RemainingCount.ToString();
            lbRemainingCount.Text = Session.RemainingCount.ToString();

            tsmiAutoLoadPredictedMask.Checked = AppSettings.AutoLoadPredictedMask;
            ckbLeftDrawActivePolygonOnly.Checked = AppSettings.LeftDrawActivePolygonOnly;
            ckbLeftImageTargetSize.Checked = AppSettings.LeftImageTargetSize;
            ckbSyncLeftRight.Checked = AppSettings.SyncLeftRight;
            ckbEditPolygonAutoUpdateMask.Checked = AppSettings.EditPolygonAutoUpdateMask;
            ckbRightShowSelectedOnly.Checked = AppSettings.RightShowSelectedOnly;
            ckbRightShowOverlap.Checked = AppSettings.RightShowOverlap;
            ckbShowPredictionMask.Checked = AppSettings.ShowPredictionMask;
            ckbAutoToggleMasks.Checked = AppSettings.AutoToggleMasks;
        }
        #endregion Constructor

        #region Form Events
        /// <summary>
        /// Even raised right after the form is shown on the screen. It is used to inform the user about issues with loading the AppSettings XML file or that the XML file didn't exist.
        /// In both cases the AppSettings dialog is opened to give the user the chance to check and change any setting, specifically the directories.
        /// If no problem occured, the current image of the session and any existing segmentation classes are loaded.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void frmImageSegmenterMain_Shown(object sender, EventArgs e)
        {
            if (AppSettings.SettingsFileExisted == false)
            {
                MessageBox.Show("File " + APP_SETTINGS_FILE_NAME + " not found!\nPlease check and update directory settings.", "Attention!");
                tsmiEditAppSettings_Click(null, null);
            }
            else if (AppSettings.SettingsFileLoadedCorrectly == false)
            {
                MessageBox.Show("File " + APP_SETTINGS_FILE_NAME + " was corrupted!\nPlease check and update all settings.", "Attention!");
                tsmiEditAppSettings_Click(null, null);
            }
            else
            {
                LoadBitmapAndSegmClasses(Session.GetCurrent);
            }
        }


        /// <summary>
        /// Event raised when this form and the application are closing. It is used to save the AppSettings and the segmentation classes for this image.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void frmImageSegmenterMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAppSettings();
            SaveSegmClasses();
        }

        /// <summary>
        /// KeyDown event handler of the form used to capture the current key code in CurrentKeyDown.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void frmImageSegmenterMain_KeyDown(object sender, KeyEventArgs e)
        {
            CurrentKeyDown = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        /// <summary>
        /// KeyUp event handler of the form used to clear the current key code in CurrentKeyDown.
        /// </summary>
        /// <param name="sender">Sender of the notification.</param>
        /// <param name="e">Specific event arguments.</param>
        private void frmImageSegmenterMain_KeyUp(object sender, KeyEventArgs e)
        {
            CurrentKeyDown = Keys.None;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        #endregion Form Events

        #region Common Methods
        /// <summary>
        /// Creates and returns an initialized SteeringDirection array with values for left, straight and right.
        /// </summary>
        /// <returns>SteeringDirection array created.</returns>
        private SteeringDirection[] InitialSteeringDirs()
        {
            return new SteeringDirection[] { new SteeringDirection("left", Color.Green, 0), new SteeringDirection("straight", Color.DarkRed, 0), new SteeringDirection("right", Color.DarkBlue, 0) };
        }

        /// <summary>
        /// Assign a Bitmap object as new bmWorkingImg and default all dependent fields and settings.
        /// </summary>
        /// <param name="Img">Bitmap object to assign</param>
        private void SetWorkingImage(Bitmap Img)
        {
            bmWorkingImg = Img;
            bmTargetSizeImg = Process.DownSample(Img, AppSettings.ImageOutputSize.Width, AppSettings.ImageOutputSize.Height);
            pbLeft.Bounds = new Rectangle(0, 0, pnLeft.Width, pnLeft.Height);
            pbRight.Bounds = new Rectangle(0, 0, pnRight.Width, pnRight.Height);
            ckbLeftImageTargetSize_CheckedChanged(null, null);
            pbRight.BackgroundImage = bmTargetSizeImg;
            SetRightImage(new Bitmap(bmTargetSizeImg.Width, bmTargetSizeImg.Height));
            cbCurrentSegmClasses.Items.Clear();
            cbCurrentSegmClasses.Text = "";
            ResetFlags();
            lbInputFileName.Text = Session.CurrentImageFileName;
            lbCurrentImageIdx.Text = Session.CurrentImageIdx.ToString();
            lbProcessedCount.Text = Session.ProcessedCount.ToString();
            lbRemainingCount.Text = Session.RemainingCount.ToString();
        }

        /// <summary>
        /// Re-assign bmOriginalImg to bmWorkingImg and OriginalSegmClasses to WorkingSegmClasses.
        /// </summary>
        public void ForceWorkingImageOriginal()
        {
            if (WorkingSegmClasses != OriginalSegmClasses)
                WorkingSegmClasses = OriginalSegmClasses;

            if (bmWorkingImg != bmOriginalImg)
                SetWorkingImage(bmOriginalImg);
        }


        /// <summary>
        /// Saves the passed list of segmentation class objects to an XML file using the given file name.
        /// </summary>
        /// <param name="FullXmlFilename">Full path and file name with extension to the XML file to write to.</param>
        /// <param name="OriginalImgFileName">File name of the original image without path to be stored as info in the XML file.</param>
        /// <param name="ProcessedImgFileName">File name of the processed image without path to be stored as info in the XML file.</param>
        /// <param name="Image">Reference to the image to get the size info.</param>
        /// <param name="SegmClasses">List of segmentation class objects for this image.</param>
        /// <param name="SteeringDirs">Steering directions to be stored with it.</param>
        private void SaveSegmClassesToXml(string FullXmlFilename, string OriginalImgFileName, string ProcessedImgFileName, Bitmap Image, List<SegmClass> SegmClasses, SteeringDirection[] SteeringDirs)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlNode nodeAnnotation = doc.AppendChild(doc.CreateElement("annotation"));
            XmlNode nodeFile = nodeAnnotation.AppendChild(doc.CreateElement("file"));
            nodeFile.AppendChild(doc.CreateElement("original_image_filename")).AppendChild(doc.CreateTextNode(OriginalImgFileName));
            nodeFile.AppendChild(doc.CreateElement("processed_image_filename")).AppendChild(doc.CreateTextNode(ProcessedImgFileName));

            XmlNode nodeSize = nodeAnnotation.AppendChild(doc.CreateElement("size"));
            nodeSize.AppendChild(doc.CreateElement("width")).AppendChild(doc.CreateTextNode(Image.Width.ToString()));
            nodeSize.AppendChild(doc.CreateElement("height")).AppendChild(doc.CreateTextNode(Image.Height.ToString()));
            nodeSize.AppendChild(doc.CreateElement("depth")).AppendChild(doc.CreateTextNode(3.ToString()));

            XmlNode nodeDirs = nodeAnnotation.AppendChild(doc.CreateElement("steering_angle"));
            foreach (SteeringDirection steerDir in SteeringDirs)
                steerDir.WriteToXml(doc, nodeDirs);

            XmlNode nodeObject = nodeAnnotation.AppendChild(doc.CreateElement("object"));
            foreach (SegmClass cat in SegmClasses)
                cat.WriteToXml(doc, nodeObject);

            doc.Save(FullXmlFilename);
        }

        /// <summary>
        /// Loads all segmentation class objects from XML file and returns the list of them. In addition it puts out the steering directions from the file.
        /// </summary>
        /// <param name="FullXmlFilename">Full path and file name with extension to the XML file to read from</param>
        /// <param name="ImgFileName">The image file name is just for checking, if it matches the one in the file.</param>
        /// <param name="SteeringDirs">Output of the steering directions read from the file.</param>
        /// <returns>List of segmentation class objects read from the XML file.</returns>
        private List<SegmClass> LoadSegmClassesFromXml(string FullXmlFilename, string ImgFileName, out SteeringDirection[] SteeringDirs)
        {
            List<SegmClass> segmClasses = new List<SegmClass>();
            SteeringDirs = InitialSteeringDirs();

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(FullXmlFilename);
                XmlNode nodeAnnotation = doc.SelectSingleNode("annotation");
                XmlNode nodeFile = nodeAnnotation.SelectSingleNode("file");

                XmlNode nodeFilename = nodeFile.SelectSingleNode("image_filename");
                if (nodeFilename != null)
                {
                    string imgFileName = nodeFile.SelectSingleNode("image_filename").InnerText;
                    if (ImgFileName != imgFileName)
                    {
                        if (MessageBox.Show(ImgFileName + "!=" + imgFileName, "Current image filename doesn't match filename in XML file", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return segmClasses;
                    }
                }
                else
                {
                    string imgFileName = nodeFile.SelectSingleNode("processed_image_filename").InnerText;
                    if (ImgFileName != imgFileName)
                    {
                        if (MessageBox.Show(ImgFileName + "!=" + imgFileName, "Current image filename doesn't match filename in XML file", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                            return segmClasses;
                    }
                }


                XmlNode nodeDirs = nodeAnnotation.SelectSingleNode("steering_angle");
                if (nodeDirs != null)
                    try
                    {
                        XmlNodeList nodeSteerItems = nodeDirs.SelectNodes("item");
                        foreach (XmlNode node in nodeSteerItems)
                        {
                            SteeringDirection steerDir = SteeringDirection.ReadFromXml(doc, node);
                            for (int i = 0; i < SteeringDirs.Length; i++)
                                if (SteeringDirs[i].Name == steerDir.Name)
                                    SteeringDirs[i] = steerDir;
                        }
                    }
                    catch { }


                XmlNode nodeObject = nodeAnnotation.SelectSingleNode("object");
                XmlNodeList nodeItems = nodeObject.SelectNodes("item");
                foreach (XmlNode node in nodeItems)
                {
                    SegmClass cat = SegmClass.ReadFromXml(doc, node);
                    if (cat.Def.Name == AppSettings.SegmClassDefs[cat.Def.ID].Name)
                        cat.Def = AppSettings.SegmClassDefs[cat.Def.ID];
                    segmClasses.Add(cat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading segmentation classes");
            }

            return segmClasses;
        }



        /// <summary>
        /// Save the list of segementation classes to the XML file for this image.
        /// </summary>
        private void SaveSegmClasses()
        {
            if ((bmOriginalImg != null) && (Session.CurrentImageFileName != null) && (Session.CurrentImageFileName != "") && (File.Exists(Session.CurrentImageFileNameFull) == true))
            {
                SaveSegmClassesToXml(AppSettings.PathToImageInfoFiles + Session.CurrentImageFileName + ".xml", Session.CurrentImageFileName, Session.CurrentImageFileName, bmOriginalImg, OriginalSegmClasses, OriginalSteeringDirs);
            }
        }

        /// <summary>
        /// Load all segmentation classes for the current image file and initialize defaults.
        /// </summary>
        private void LoadSegmClasses()
        {
            string fileName = AppSettings.PathToImageInfoFiles + Session.CurrentImageFileName + ".xml";
            if ((File.Exists(fileName) == true))
                OriginalSegmClasses = LoadSegmClassesFromXml(fileName, Session.CurrentImageFileName, out OriginalSteeringDirs);
            else
                OriginalSegmClasses = new List<SegmClass>();

            WorkingSegmClasses = OriginalSegmClasses;
            cbCurrentSegmClasses.Items.Clear();
            cbCurrentSegmClasses.Text = "";
            foreach (SegmClass cat in WorkingSegmClasses)
                cbCurrentSegmClasses.Items.Add(cat.Def.Name);

            btnEditPolygon.Enabled = WorkingSegmClasses.Count > 0;
            btnDeleteSegmClass.Enabled = WorkingSegmClasses.Count > 0;

            DrawMasks();
            pbLeft.Invalidate();

        }

        /// <summary>
        /// Loads bmOriginalImg from a JPG file using the passed LoadBitmapFunction. bmOriginalImg is also assigned to bmWorkingImg via SetWorkingImage() and 
        /// the segmentation classes for this image are also loaded if exist. if there also is an existing prediction mask it will be loaded automatically if this feature is enabled.
        /// </summary>
        /// <param name="LoadBitmapFunction">One of the Bitmap load functions of the Session class, like GetCurrent, GetNext or GetPrevious</param>
        private void LoadBitmapAndSegmClasses(Session.LoadBitmapFunction LoadBitmapFunction)
        {
            SetEnabled(false);
            SaveSegmClasses();                      // save any previous segmentation classes if existed
            bmOriginalImg = null;
            Bitmap bmLoad = LoadBitmapFunction();   // execute the load function 
            if (bmLoad != null)
            {
                bmOriginalImg = new Bitmap(bmLoad); // if loading was succesfull, create a local copy of the loaded image
                bmLoad.Dispose();                   // and dispose the loaded, so the file handle is freed

                // in case any blur functionality is enabled, apply it here right after loading the image.
                if ((AppSettings.BlurEnabled == true) && (AppSettings.BlurAreaPolygon != null) && (AppSettings.BlurAreaPolygon.Length>2))
                {
                    PointF[] poly = new PointF[AppSettings.BlurAreaPolygon.Length];
                    for (int i = 0; i < poly.Length; i++)
                        poly[i] = new PointF(AppSettings.BlurAreaPolygon[i].X, AppSettings.BlurAreaPolygon[i].Y); // == 0 ? 0 : AppSettings.BlurAreaPolygon[i].Y + 0.03f);
                    bmOriginalImg = Process.BlurArea(bmOriginalImg, poly, AppSettings.BlurWindowSize, AppSettings.BlurStepSize);
                }
                SetWorkingImage(bmOriginalImg);     // assign bmOriginalImg to bmWorkingImg and initialize everything related
                LoadSegmClasses();                  // load the segmentation classes for this image

                bmPredMaskCode = null;              // reset prediction masks and related settings
                bmPredMaskColored = null;
                ckbShowPredictionMask.Enabled = false;
                ckbAutoToggleMasks.Enabled = false;

                if (tsmiAutoLoadPredictedMask.Checked == true)      // if enabled, try to load the prediction mask for this image
                {
                    string fullPredFname = AppSettings.PathToPredictedMasks + Session.CurrentImageFileName + ".png";
                    if (File.Exists(fullPredFname) == true)
                    {
                        bmPredMaskCode = (Bitmap)Image.FromFile(fullPredFname);     // the 8bit PNG file is returned here as 24bit R,G,B Bitmap with the 8 bit code copied to all 3 colors.
                        bmPredMaskColored = Process.ImageColorMap(bmPredMaskCode, AppSettings.SegmClassDefs, AppSettings.DrawMaskTransparency); // Re-color to segmentation class colors
                        ckbShowPredictionMask.Enabled = true;
                        ckbAutoToggleMasks.Enabled = true;
                        if (ckbShowPredictionMask.Checked == true)
                            SetRightImage(bmPredMaskColored);                       // if enabled, display the prediction mask on the right side
                        ckbAutoToggleMasks_CheckedChanged(null, null);
                    }
                }
            }
            ResetFlags();
            SetEnabled(true);
        }

        /// <summary>
        /// Transfer GUI settings to the AppSettings object and save it all to the XML file.
        /// </summary>
        private void SaveAppSettings()
        {
            AppSettings.AutoLoadPredictedMask = tsmiAutoLoadPredictedMask.Checked;
            AppSettings.LeftDrawActivePolygonOnly = ckbLeftDrawActivePolygonOnly.Checked;
            AppSettings.LeftImageTargetSize = ckbLeftImageTargetSize.Checked;
            AppSettings.SyncLeftRight = ckbSyncLeftRight.Checked;
            AppSettings.EditPolygonAutoUpdateMask = ckbEditPolygonAutoUpdateMask.Checked;
            AppSettings.RightShowSelectedOnly = ckbRightShowSelectedOnly.Checked;
            AppSettings.RightShowOverlap = ckbRightShowOverlap.Checked;
            AppSettings.SaveSettings();
        }

        /// <summary>
        /// General method to disable or enable the user control elemements and change the cursor.
        /// </summary>
        /// <param name="Value">Enable value to set to.</param>
        private void SetEnabled(bool Value)
        {
            pnControl.Enabled = Value;
            msMainMenu.Enabled = Value;
            if (Value)
                Cursor = Cursors.Default;
            else
                Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Updates the Enabled properties of ToolStripMenuItems, for instance to not allow the user to navigate away from the current image while editing a class.
        /// </summary>
        /// <param name="Value">False to disable all, True to enable conditionally</param>
        private void SetNavigationMenuEnabled(bool Value)
        {
            tsmiFile.Enabled = Value;
            tsmiSetttings.Enabled = Value;
            tsmiProcess.Enabled = Value;
            tsmiNext.Enabled = Value && ((Session.CurrentImageIdx < Session.ProcessedCount) || (Session.RemainingCount > 0));
            tsmiPrevious.Enabled = Value && (Session.CurrentImageIdx > 0);
            tsmiGotoFirstProcessedImage.Enabled = Value && (Session.ProcessedCount > 0);
            tsmiGotoLastProcessedImage.Enabled = Value && (Session.ProcessedCount > 0);
            tsmiRemoveLastProcessed.Enabled = Value && (Session.ProcessedCount > 0);
        }

        /// <summary>
        /// Enables or disables GUI elements depending on the current user interaction status
        /// </summary>
        public void UpdateButtonAndMenuEnable()
        {
            if (AugmentationPanel != null)      // if the augmentation dialog is open, disable editing
            {
                btnNewPolygon.Enabled = false;
                btnFinishSegmClass.Enabled = false;
                btnEditPolygon.Enabled = false;
                btnDeleteSegmClass.Enabled = false;
                cbCurrentSegmClasses.Enabled = false;

                Text = APP_NAME + " -> Test Augmentation Levels";
            }
            else if (EnterNewPolygon == true)        
            {
                btnNewPolygon.Enabled = true;
                btnFinishSegmClass.Enabled = true;
                btnEditPolygon.Enabled = false;
                btnDeleteSegmClass.Enabled = false;
                cbCurrentSegmClasses.Enabled = false;

                SetNavigationMenuEnabled(false);
                Text = APP_NAME + " -> Enter New Polygon";
            }
            else if (EditCurrentPolygon == true)
            {
                btnNewPolygon.Enabled = false;
                btnFinishSegmClass.Enabled = false;
                btnEditPolygon.Enabled = true;
                btnDeleteSegmClass.Enabled = false;
                cbCurrentSegmClasses.Enabled = false;

                SetNavigationMenuEnabled(false);
                Text = APP_NAME + " -> Edit Current Polygon";
            }
            else
            {
                btnNewPolygon.Enabled = true;
                btnFinishSegmClass.Enabled = false;
                btnEditPolygon.Enabled = WorkingSegmClasses.Count > 0;
                btnDeleteSegmClass.Enabled = WorkingSegmClasses.Count > 0;
                cbCurrentSegmClasses.Enabled = WorkingSegmClasses.Count > 0;

                SetNavigationMenuEnabled(true);
                Text = APP_NAME;
            }
        }

        /// <summary>
        /// Reset all edit action flags.
        /// </summary>
        private void ResetFlags()
        {
            SegmClassesEdited = false;
            EnterNewPolygon = false;
            EditCurrentPolygon = false;
            HighlightCurrentPolygon = false;
            HighlightCurrentPolygonIdx = -1;

            UpdateButtonAndMenuEnable();
        }

        /// <summary>
        /// Creates new mask bitmaps and draws the filled segmentation class polygons in different ways for their uses.
        /// bmCodeMask will be filled using the class code in R,G and B, which is not usable for display, but for getting the code for any pixel.
        /// bmImageMask will be filled using the draw color for the polygon area to be shown in the right PictureBox.
        /// bm8bitMask is the 8bit monochrome version of the bmCodeMask, that can be stored as PNG segmentation mask.
        /// </summary>
        private void DrawMasks()
        {
            // create the thre new bitmap objects
            bmCodeMask = new Bitmap(AppSettings.ImageOutputSize.Width, AppSettings.ImageOutputSize.Height);
            bmImageMask = new Bitmap(AppSettings.ImageOutputSize.Width, AppSettings.ImageOutputSize.Height);
            bm8bitMask = new Bitmap(AppSettings.ImageOutputSize.Width, AppSettings.ImageOutputSize.Height, PixelFormat.Format8bppIndexed);

            // First determine the min and max value of the DrawOrder used in this image
            int minOrder = int.MaxValue;
            int maxOrder = int.MinValue;
            foreach (SegmClass cat in WorkingSegmClasses)
            {
                minOrder = Math.Min(minOrder, cat.Def.DrawOrder);
                maxOrder = Math.Max(maxOrder, cat.Def.DrawOrder);
            }
            // Limit the range further if requested
            minOrder = Math.Max(minOrder, AppSettings.MaskDrawOrderMin);
            maxOrder = Math.Min(maxOrder, AppSettings.MaskDrawOrderMax);

            // Graphics objects can only be created for th RGB bitmaps
            Graphics grfxImageMask = Graphics.FromImage(bmImageMask);
            Graphics grfxCodeMask = Graphics.FromImage(bmCodeMask);
            // Fill in the polygon areas as colors or as code values
            for (int drawOrder = minOrder; drawOrder <= maxOrder; drawOrder++)
                for (int i = 0; i < WorkingSegmClasses.Count; i++)
                    if (drawOrder == WorkingSegmClasses[i].Def.DrawOrder)
                    {
                        PointF[] poly = SegmClass.GetDrawPolygon(WorkingSegmClasses[i].NormalizedPolygon, bmImageMask.Size);
                        grfxCodeMask.FillPolygon(new SolidBrush(Color.FromArgb(WorkingSegmClasses[i].Def.ID, WorkingSegmClasses[i].Def.ID, WorkingSegmClasses[i].Def.ID)), poly);
                        if (((ckbRightShowOverlap.Checked == true) && (ckbRightShowSelectedOnly.Checked == false)) || (ckbRightShowSelectedOnly.Checked == true) && (i == cbCurrentSegmClasses.SelectedIndex))
                            grfxImageMask.FillPolygon(new SolidBrush(Color.FromArgb(AppSettings.DrawMaskTransparency, WorkingSegmClasses[i].Def.DrawColor)), poly);
                    }

            // If there is a blur polygon, draw it here too
            if ((AppSettings.BlurEnabled == true) && (AppSettings.BlurAreaPolygon != null) && (AppSettings.BlurAreaPolygon.Length > 2))
            {
                PointF[] poly = SegmClass.GetDrawPolygon(AppSettings.BlurAreaPolygon, bmImageMask.Size);
                grfxCodeMask.FillPolygon(new SolidBrush(Color.FromArgb(0, 0, 0)), poly);
                grfxImageMask.FillPolygon(new SolidBrush(Color.FromArgb(AppSettings.DrawMaskTransparency, Color.FromArgb(0, 0, 0))), poly);
            }

            // Now plow through the code version and set the pixel values in the 8bit version 
            Rectangle rect = new Rectangle(0, 0, bm8bitMask.Width, bm8bitMask.Height);
            BitmapData data = bm8bitMask.LockBits(rect, ImageLockMode.ReadOnly, bm8bitMask.PixelFormat);

            unsafe
            {
                byte* ptr = (byte*)data.Scan0.ToPointer();

                if ((ckbRightShowSelectedOnly.Checked == true) || (ckbRightShowOverlap.Checked == true))
                    for (int y = 0; y < bm8bitMask.Height; y++)
                        for (int x = 0; x < bm8bitMask.Width; x++)
                            *ptr++ = bmCodeMask.GetPixel(x, y).R;
                else
                    for (int y = 0; y < bm8bitMask.Height; y++)
                        for (int x = 0; x < bm8bitMask.Width; x++)
                        {
                            int code = bmCodeMask.GetPixel(x, y).R;
                            *ptr++ = (byte)code;
                            //if (code != 0)
                                bmImageMask.SetPixel(x, y, Color.FromArgb(AppSettings.DrawMaskTransparency, AppSettings.SegmClassDefs[code].DrawColor));
                        }

            }
            bm8bitMask.UnlockBits(data);

            // The 8bit version needs a new color palette lookup table with 1:1 translation, so it's altered with the standard color lookup table
            ColorPalette pal = bm8bitMask.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(i, i, i);
            bm8bitMask.Palette = pal;

            // Now assign the correct color mask version to the right side
            Rectangle r = pbRight.Bounds;
            SetRightImage(bmImageMask);
            pbRight.Bounds = r;
        }

        #endregion Common Methods

        #region Menu Event Handler
        /// <summary>
        /// ToolStripMenuItem click event handler to save the current state to files. It saves the segmentation classes for the current image and the session info.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiSaveCurrentState_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            SaveSegmClasses();
            Session.SaveSessionInfo();
            SetEnabled(true);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to exit the application. 
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to open the AppSettings edit dialog.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiEditAppSettings_Click(object sender, EventArgs e)
        {
            frmAppSettings frmAppSettings = new frmAppSettings(AppSettings);
            if (frmAppSettings.ShowDialog() == DialogResult.OK)
            {
                bool newSession = (AppSettings.PathToSourceImages != frmAppSettings.AppSettings.PathToSourceImages) ||
                                  (AppSettings.PathToSessionData != frmAppSettings.AppSettings.PathToSessionData);
                frmAppSettings.AppSettings.CopyTo(AppSettings);
                if (newSession == true)
                    Session = new Session(AppSettings.PathToSourceImages, AppSettings.PathToSessionData);

                SaveAppSettings();
            }
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to re-load everything after the checked status had been changed.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiAutoLoadPredictedMask_Click(object sender, EventArgs e)
        {
            LoadBitmapAndSegmClasses(Session.GetCurrent);
        }


        /// <summary>
        /// ToolStripMenuItem click event handler to load the first image of the session.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiGotoFirstProcessedImage_Click(object sender, EventArgs e)
        {
            Session.CurrentImageIdx = 0;
            LoadBitmapAndSegmClasses(Session.GetCurrent);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to load the last image of the session.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiGotoLastProcessedImage_Click(object sender, EventArgs e)
        {
            Session.CurrentImageIdx = Session.ProcessedCount - 1;
            LoadBitmapAndSegmClasses(Session.GetCurrent);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to remove the last image from the session.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiRemoveLastProcessed_Click(object sender, EventArgs e)
        {
            LoadBitmapAndSegmClasses(Session.RemoveLastProcessed);
        }


        /// <summary>
        /// ToolStripMenuItem click event handler to display the augmentation panel.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiAugmentationPanel_Click(object sender, EventArgs e)
        {
            if (AugmentationPanel != null)
                AugmentationPanel.BringToFront();
            else
            {
                AugmentationPanel = new frmAugmentationPanel(this);
                AugmentationPanel.Location = new Point(this.Location.X + this.Width - AugmentationPanel.Width, this.Location.Y + 24);
                AugmentationPanel.Show();
                UpdateButtonAndMenuEnable();
            }
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to process all augmentations.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiProcessAllImageAugmentation_Click(object sender, EventArgs e)
        {
            frmProcessConfirmation processConfirmation = new frmProcessConfirmation(Session.ProcessedCount-1);
            if (processConfirmation.ShowDialog() == DialogResult.Cancel)
                return;

            SetEnabled(false);
            if (processConfirmation.CLearAllOutputDirs == true)
                ClearOutputDirs();

            ProcessAllImageAugmentations(processConfirmation.StartImageNumber, processConfirmation.EndImageNumber);

            SetEnabled(true);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to load the next image of the session.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiNext_Click(object sender, EventArgs e)
        {
            LoadBitmapAndSegmClasses(Session.GetNext);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to load the previous image of the session.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiPrevious_Click(object sender, EventArgs e)
        {
            LoadBitmapAndSegmClasses(Session.GetPrevious);
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to open the online documentation.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiOnlineDoc_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/StefansAI/JetCar/blob/main/docs/Data%20Preparation.md");
        }


        /// <summary>
        /// ToolStripMenuItem click event handler to open the JetCar project page.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiJetCarProject_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/StefansAI/JetCar/");
        }

        /// <summary>
        /// ToolStripMenuItem click event handler to open the about box.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            (new AboutBox1()).Show();
        }
        #endregion Menu Event Handler

        #region Left Image

        /// <summary>
        /// MouseLeave event handler of the left PictureBox to reset CoordLeft and LeftScrollZoneCount.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments</param>
        private void pbLeft_MouseLeave(object sender, EventArgs e)
        {
            CoordLeft = new Point(-1, -1);
            lbCursorValuesLeft.Text = "X: ----  Y: ---- Value: -----";
            LeftScrollZoneCount = 0;
        }

        /// <summary>
        /// If CheckBox ckbEdgeSnap is checked and the point P is close to an edge, return a point snapped to the edge. Otherwise return the point as is.
        /// </summary>
        /// <param name="P">Point coordinates to check.</param>
        /// <returns>Same point as passed to check or new point snapped to an edge</returns>
        private Point SnapToEdges(Point P)
        {
            if (ckbEdgeSnap.Checked == true)
            {
                int x = P.X;
                int y = P.Y;
                int snapMargin = (int)(3 * ZoomLeft);

                if (x < snapMargin)
                    x = 0;
                else if (x > pbLeft.Width - snapMargin)
                    x = pbLeft.Width;

                if (y < snapMargin)
                    y = 0;
                else if (y > pbLeft.Height - snapMargin)
                    y = pbLeft.Height;

                return new Point(x, y);
            }
            else return P;
        }

        /// <summary>
        /// MouseMove event handler of the left PictureBox performs several functions. If close to an edge when zoomed in, it handles scrolling. 
        /// For entering a new polygon point or changing an existing polygon this handler moves the location of the polygon point with the mouse move. 
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Mouse specific arguments</param>
        private void pbLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (pbLeft.Image == null)
                return;

            try
            {
                // convert mouse coordinates into pixel coordinates in the bitmap
                CoordLeft = new Point((int)(e.X / ZoomLeft), (int)(e.Y / ZoomLeft));
                Color color = ((Bitmap)pbLeft.Image).GetPixel(CoordLeft.X, CoordLeft.Y);
                lbCursorValuesLeft.Text = "X: " + CoordLeft.X.ToString() + "  Y: " + CoordLeft.Y.ToString() + " Color: " + color.ToString();
            }
            catch
            {
                lbCursorValuesLeft.Text = "X: " + CoordLeft.X.ToString() + "  Y: " + CoordLeft.Y.ToString() + " Color: ----";
            }

            // If the picture box size is larger than the panel it is on, the scrolling is possible
            if ((pbLeft.Width > pnLeft.Width) || (pbLeft.Height > pnLeft.Height))
            {
                int windowX = e.X + pbLeft.Location.X;
                int windowY = e.Y + pbLeft.Location.Y;
                int x = pbLeft.Location.X;
                int y = pbLeft.Location.Y;
                int moveMargin = (int)(AppSettings.ScrollZoneSize * ZoomLeft + 0.5f);
                int moveDelta = (int)(AppSettings.ScrollMoveFactor * ZoomLeft + 0.5f);

                // Calculate possible new location of picturebox
                if (windowX < moveMargin)
                    x = Math.Min(x + moveDelta, 0);
                else if (windowX > pnLeft.Width - moveMargin)
                    x = Math.Max(x - moveDelta, pnLeft.Width - pbLeft.Width);

                if (windowY < moveMargin)
                    y = Math.Min(y + moveDelta, 0);
                else if (windowY > pnLeft.Height - moveMargin)
                    y = Math.Max(y - moveDelta, pnLeft.Height - pbLeft.Height);

                // If new location is different than current one, wait until LeftScrollZoneCount crosses the threshold before really move to new location.
                if ((pbLeft.Location.X != x) || (pbLeft.Location.Y != y))
                {
                    if (++LeftScrollZoneCount > AppSettings.ScrollStartMinCount)
                    {
                        LeftScrollZoneCount = AppSettings.ScrollStartMinCount;
                        pbLeft.Location = new Point(x, y);
                        pbLeft.Refresh();
                        if (ckbSyncLeftRight.Checked == true)
                            pbRight.Location = pbLeft.Location;
                    }
                }
                else
                    LeftScrollZoneCount = 0;        // always reset for next scroll delay
            }
            else LeftScrollZoneCount = 0;           // always reset for next scroll delay

            // When entering a new polygon, update the last point in the polygon to the current mouse location
            if (EnterNewPolygon == true)
            {
                NormalizedPolygon[NormalizedPolygon.Length - 1] = SegmClass.GetNormalizedPoint(new PointF(e.X, e.Y), pbLeft.Size);
                DrawPolygon[DrawPolygon.Length - 1] = new Point(e.X, e.Y);
                pbLeft.Invalidate();
            }

            // When editing a point of an existing polygon, move that point
            if ((EditCurrentPolygon == true) && (EditPolygonPointIdx >= 0))
            {
                NormalizedPolygon[EditPolygonPointIdx] = SegmClass.GetNormalizedPoint(new PointF(e.X, e.Y), pbLeft.Size);
                DrawPolygon[EditPolygonPointIdx] = new Point(e.X, e.Y);
                pbLeft.Invalidate();
                EditPolygonChange();
            }

            // WHen the left mouse button is pressed while moving, it could mean the steering vector has to be updated.
            if (e.Button == MouseButtons.Left)
            {
                if ((EnterNewPolygon == false) && (EditCurrentPolygon == false) && (ckbShowSteeringAngles.Checked == true) && (EditSteeringDirIdx >= 0))
                {
                    if (EditSteeringDirIdx == 3)
                        for (int i = 0; i < WorkingSteeringDirs.Length; i++)
                            WorkingSteeringDirs[i].DrawingEndPoint = e.Location;
                    else
                        WorkingSteeringDirs[EditSteeringDirIdx].DrawingEndPoint = e.Location;

                    pbLeft.Invalidate();
                }
            }
            else
            {
                EditSteeringDirIdx = -1;
            }
        }

        /// <summary>
        /// Mouse button down event handler for the left PictureBox. The left mouse button normally finalizes a polygon point move, picks up a point for move or might insert a new point. 
        /// The right mouse button is used to delete a polygon point.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Mouse specific arguments</param>
        private void pbLeft_MouseDown(object sender, MouseEventArgs e)
        {
            // Left mouse button means finalize a polygon point move or insert a new point
            if (e.Button == MouseButtons.Left)
            {
                if (EnterNewPolygon == true)
                {
                    // Potentially snap and fix the last moved point and add a new point to be moved next from now on
                    NormalizedPolygon[NormalizedPolygon.Length - 1] = SegmClass.GetNormalizedPoint(SnapToEdges(e.Location), pbLeft.Size);
                    PointF[] oldPoly = NormalizedPolygon;
                    NormalizedPolygon = new PointF[oldPoly.Length + 1];
                    Array.Copy(oldPoly, 0, NormalizedPolygon, 0, oldPoly.Length);
                    NormalizedPolygon[NormalizedPolygon.Length - 1] = NormalizedPolygon[NormalizedPolygon.Length - 2];

                    DrawPolygon = SegmClass.GetDrawPolygon(NormalizedPolygon, pbLeft.Size);
                    pbLeft.Invalidate();

                }
                else if (EditCurrentPolygon == true)
                {
                    // Mouse down in edit mode means finding the point that might be pointed to by the mouse or even insert a new point at the mouse position
                    EditPolygonPointIdx = -1;
                    float snapMargin = AppSettings.PolygonPointSize * ZoomLeft;
                    for (int i = 0; i < DrawPolygon.Length; i++)
                        if ((Math.Abs(e.X - DrawPolygon[i].X) < snapMargin) && (Math.Abs(e.Y - DrawPolygon[i].Y) < snapMargin))
                        {
                            // Polygon point found close enough to the mouse position
                            EditPolygonPointIdx = i;
                            pbLeft.Invalidate();
                            return;
                        }

                    // Since a specific point of the polygon could not be reached, insert a new point in the polygon line segement that could fit the mouse position.
                    snapMargin /= 2; 
                    int n = DrawPolygon.Length;
                    for (int i = 0; i < n; i++)
                    {
                        float x0 = DrawPolygon[i].X;
                        float x1 = DrawPolygon[(i + 1) % n].X;
                        float y0 = DrawPolygon[i].Y;
                        float y1 = DrawPolygon[(i + 1) % n].Y;
                        if ((e.X>Math.Min(x0,x1)-snapMargin) && (e.X < Math.Max(x0, x1) + snapMargin) && (e.Y > Math.Min(y0, y1) - snapMargin) && (e.Y < Math.Max(y0, y1) + snapMargin))
                        {
                            bool valid = false;
                            float dx = (float)e.X - x0;
                            float dy = (float)e.Y - y0;
                            float DX = x1 - x0;
                            float DY = y1 - y0;
                            if ((Math.Abs(dx) < snapMargin) && (Math.Abs(DX) < snapMargin))
                                valid = true;
                            else if ((Math.Abs(dy) < snapMargin) && (Math.Abs(DY) < snapMargin))
                                valid = true;
                            else
                            {
                                if (Math.Abs(DX) < Math.Abs(DY))
                                {
                                    float slopeXY = DX / DY;
                                    float dxs = dy * slopeXY;
                                    valid = Math.Abs(dx - dxs) < snapMargin;
                                }
                                else
                                {
                                    float slopeYX = DY / DX;
                                    float dys = dx * slopeYX;
                                    valid = Math.Abs(dy - dys) < snapMargin;
                                }
                            }
                            if (valid == true)
                            {
                                // a segemnt had been found and a new point can be inserted here
                                PointF[] oldPoly = NormalizedPolygon;
                                NormalizedPolygon = new PointF[oldPoly.Length + 1];
                                Array.Copy(oldPoly, 0, NormalizedPolygon, 0, i + 1);
                                NormalizedPolygon[i + 1] = SegmClass.GetNormalizedPoint(e.Location, pbLeft.Size);
                                if (i < n - 1)
                                    Array.Copy(oldPoly, i + 1, NormalizedPolygon, i + 2, n - i - 1);

                                DrawPolygon = SegmClass.GetDrawPolygon(NormalizedPolygon, pbLeft.Size);

                                EditPolygonPointIdx = i + 1;
                                pbLeft.Invalidate();
                                EditPolygonChange();
                                return;
                            }
                        }
                    }

                }
                else if (ckbShowSteeringAngles.Checked == true)
                {
                    // When arrived here, change one or all of the steering directions.
                    EditSteeringDirIdx = -1;
                    float snapMargin = AppSettings.PolygonPointSize * ZoomLeft;
                    
                    if ((CurrentKeyDown == Keys.S) || (CurrentKeyDown == Keys.Left))
                    {
                        if ((Math.Abs(e.X - WorkingSteeringDirs[0].DrawingEndPoint.X) < snapMargin) && (Math.Abs(e.Y - WorkingSteeringDirs[0].DrawingEndPoint.Y) < snapMargin))
                        {
                            EditSteeringDirIdx = 0;
                            return;
                        }
                    }
                    if ((CurrentKeyDown == Keys.E) || (CurrentKeyDown == Keys.Up))
                    {
                        if ((Math.Abs(e.X - WorkingSteeringDirs[1].DrawingEndPoint.X) < snapMargin) && (Math.Abs(e.Y - WorkingSteeringDirs[1].DrawingEndPoint.Y) < snapMargin))
                        {
                            EditSteeringDirIdx = 1;
                            return;
                        }
                    }
                    if ((CurrentKeyDown == Keys.D) || (CurrentKeyDown == Keys.Right))
                    {
                        if ((Math.Abs(e.X - WorkingSteeringDirs[2].DrawingEndPoint.X) < snapMargin) && (Math.Abs(e.Y - WorkingSteeringDirs[2].DrawingEndPoint.Y) < snapMargin))
                        {
                            EditSteeringDirIdx = 2;
                            return;
                        }
                    }
                    if ((CurrentKeyDown == Keys.A) || (CurrentKeyDown == Keys.Down))
                    {
                        EditSteeringDirIdx = 3;
                        return;
                    }

                    for (int i=0; i<WorkingSteeringDirs.Length; i++)
                    {
                        if ((Math.Abs(e.X - WorkingSteeringDirs[i].DrawingEndPoint.X) < snapMargin) && (Math.Abs(e.Y - WorkingSteeringDirs[i].DrawingEndPoint.Y) < snapMargin))
                        {
                            EditSteeringDirIdx = i;
                            return;
                        }
                    }
                }
            }
            else if ((e.Button == MouseButtons.Right) && (NormalizedPolygon.Length > 2))
            {
                // Right mouse button means delete a point, the last point when entering a new polygon
                if (EnterNewPolygon == true)
                {
                    Array.Resize<PointF>(ref NormalizedPolygon, NormalizedPolygon.Length - 1);
                    Array.Resize<PointF>(ref DrawPolygon, DrawPolygon.Length - 1);
                }
                else if (EditCurrentPolygon == true)
                {
                    // When editing a polygon, find the point close enough to the mouse coordinates to be deleted
                    EditPolygonPointIdx = -1;
                    float snapMargin = ZoomLeft;
                    for (int i = 0; i < DrawPolygon.Length; i++)
                        if ((Math.Abs(e.X - DrawPolygon[i].X) < snapMargin) && (Math.Abs(e.Y - DrawPolygon[i].Y) < snapMargin))
                        {
                            PointF[] oldPoly = NormalizedPolygon;
                            NormalizedPolygon = new PointF[oldPoly.Length - 1];
                            if (i > 0)
                                Array.Copy(oldPoly, 0, NormalizedPolygon, 0, i);
                            if (i < NormalizedPolygon.Length)
                                Array.Copy(oldPoly, i + 1, NormalizedPolygon, i, NormalizedPolygon.Length - i);

                            DrawPolygon = SegmClass.GetDrawPolygon(NormalizedPolygon, pbLeft.Size);
                            pbLeft.Invalidate();
                            EditPolygonChange();
                            return;
                        }
                }
            }
        }

        /// <summary>
        /// Mouse button up event handler for the left PictureBox. When editing a polygon, the point was selected via MouseDown and moved via MouseMove. MouseUp finalizes the move
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Mouse event arguments</param>
        private void pbLeft_MouseUp(object sender, MouseEventArgs e)
        {
            if ((EditCurrentPolygon == true) && (EditPolygonPointIdx >= 0))
            {
                Point p = SnapToEdges(e.Location);
                DrawPolygon[EditPolygonPointIdx] = p;
                NormalizedPolygon[EditPolygonPointIdx] = SegmClass.GetNormalizedPoint(p, pbLeft.Size);
                EditPolygonPointIdx = -1;
                pbLeft.Invalidate();
                EditPolygonChange();
            }
        }

        /// <summary>
        /// Mouse double click event handler of the left PictureBox. When netering a new ploygon, the double click finilizes the edit with the last point.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Mouse event arguments</param>
        private void pbLeft_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (EnterNewPolygon == true)
            {
                // remove any double entries that might exist from double clicking or multiple clicking at the last points
                while ((DrawPolygon.Length > 2) && (DrawPolygon[DrawPolygon.Length - 1] == DrawPolygon[DrawPolygon.Length - 2]))
                {
                    Array.Resize<PointF>(ref NormalizedPolygon, NormalizedPolygon.Length - 1);
                    Array.Resize<PointF>(ref DrawPolygon, DrawPolygon.Length - 1);
                }

                EnterNewPolygon = false;
                btnFinishSegmClass.Enabled = true;
            }
        }

        /// <summary>
        /// Mouse wheel event handler of the left PictureBox. It is used to zoom in and out keeping the current cursor position in about the same place.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments.</param>
        private void pbLeft_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pbLeft.Image == null) return;

            int idxX = (int)(e.X / ZoomLeft);
            int idxY = (int)(e.Y / ZoomLeft);
            float delta = e.Delta / 240.0f;
            float zoomLast = ZoomLeft;
            ZoomLeft = Math.Max(Math.Min(ZoomLeft + delta, 32 * ZoomLeftMin), ZoomLeftMin);
            if (ZoomLeft > ZoomLeftMin)
            {
                int width = (int)(pbLeft.Image.Width * ZoomLeft + 0.5f);
                int height = (int)(pbLeft.Image.Height * ZoomLeft + 0.5f);
                int x = Math.Min(pbLeft.Left + e.X - (int)(idxX * ZoomLeft + 0.5f), 0);
                int y = Math.Min(pbLeft.Top + e.Y - (int)(idxY * ZoomLeft + 0.5f), 0);
                pbLeft.Size = new Size(width, height);
                pbLeft.Location = new Point(x, y);
            }
            else
            {
                pbLeft.Size = pnLeft.Size;
                pbLeft.Location = new Point(0, 0);
            }

            // if synchronization checkbox is checked, set the right PictureBox to same size and location.
            if (ckbSyncLeftRight.Checked == true)
            {
                pbRight.Size = pbLeft.Size;
                pbRight.Location = pbLeft.Location;
            }

            // Update drawing polygon to new size
            if ((ZoomLeft != zoomLast) && (NormalizedPolygon != null))
                DrawPolygon = SegmClass.GetDrawPolygon(NormalizedPolygon, pbLeft.Size);
        }

        /// <summary>
        /// Paint event handler of left PictureBox. Draw all segmentation class polygons in their colors and a name text to the PictureBox.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments</param>
        private void pbLeft_Paint(object sender, PaintEventArgs e)
        {
            if ((DrawPolygon != null) && (DrawPolygon.Length > 1))
            {
                // if there is an active DrawPolygon ready to draw, do it with the PolygonLineColor.
                e.Graphics.DrawPolygon(new Pen(AppSettings.PolygonLineColor), DrawPolygon);

                if (EditCurrentPolygon == true)
                {
                    // when editing an exitsing polygon, also display the circles around each point
                    float d = AppSettings.PolygonPointSize * ZoomLeft;
                    float r = d / 2;
                    foreach (PointF p in DrawPolygon)
                        e.Graphics.DrawEllipse(new Pen(AppSettings.PolygonPointColor), p.X - r, p.Y - r, d, d);
                }
            }

            if (ckbShowSteeringAngles.Checked == true)
            {
                // draw steering directions
                float d = AppSettings.PolygonPointSize * ZoomLeft;
                float r = d / 2;
                foreach (SteeringDirection steerDir in WorkingSteeringDirs)
                {
                    steerDir.DrawingSize = pbLeft.Size;
                    Pen pen = new Pen(steerDir.DrawColor, 3);
                    pen.DashStyle = DashStyle.Dash;
                    e.Graphics.DrawLine(pen, steerDir.DrawingPoints[0].X, steerDir.DrawingPoints[0].Y, steerDir.DrawingPoints[1].X, steerDir.DrawingPoints[1].Y);
                    e.Graphics.DrawEllipse(new Pen(steerDir.DrawColor), steerDir.DrawingPoints[1].X - r, steerDir.DrawingPoints[1].Y - r, d, d);
                    e.Graphics.DrawString(steerDir.Name, pnLeft.Font, new SolidBrush(steerDir.DrawColor), steerDir.DrawingPoints[1].X, steerDir.DrawingPoints[1].Y - d - 2);
                }
            }

            if ((WorkingSegmClasses != null) && (WorkingSegmClasses.Count > 0))
            {
                if (ckbLeftDrawActivePolygonOnly.Checked == false)
                {
                    // Draw all segmentation class polygons in their class colors with name text, except the currently selected one, since it is already drawn above
                    for (int i = 0; i < WorkingSegmClasses.Count; i++)
                    {
                        WorkingSegmClasses[i].DrawSize = pbLeft.Size;

                        // fill out the selected one
                        if ((HighlightCurrentPolygon == true) && (i == HighlightCurrentPolygonIdx /*cbCurrentSegmClasses.SelectedIndex*/))
                            e.Graphics.FillPolygon(new SolidBrush(AppSettings.PolygonLineColor), WorkingSegmClasses[i].DrawPolygon);

                        if ((EditCurrentPolygon == false) || (i != cbCurrentSegmClasses.SelectedIndex))
                        {
                            e.Graphics.DrawPolygon(WorkingSegmClasses[i].DrawPen, WorkingSegmClasses[i].DrawPolygon);
                            e.Graphics.DrawString(WorkingSegmClasses[i].Def.Name, pnLeft.Font, WorkingSegmClasses[i].DrawBrush, WorkingSegmClasses[i].DrawTextLocation.X, WorkingSegmClasses[i].DrawTextLocation.Y);
                        }
                    }
                }
                else if ((EnterNewPolygon == false) && (EditCurrentPolygon == false))
                {
                    // draw only the slected one and no other
                    int i = cbCurrentSegmClasses.SelectedIndex;
                    if ((i >= 0) && (i < WorkingSegmClasses.Count))
                    {
                        WorkingSegmClasses[i].DrawSize = pbLeft.Size;
                        e.Graphics.DrawPolygon(WorkingSegmClasses[i].DrawPen, WorkingSegmClasses[i].DrawPolygon);
                        e.Graphics.DrawString(WorkingSegmClasses[i].Def.Name, pnLeft.Font, WorkingSegmClasses[i].DrawBrush, WorkingSegmClasses[i].DrawTextLocation.X, WorkingSegmClasses[i].DrawTextLocation.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Assign a Bitmap to the left PictureBox image and reset settings to defaults.
        /// </summary>
        /// <param name="bmSet">Reference to the Bitmap object to assign to pbLeft.Image. </param>
        private void SetLeftImage(Bitmap bmSet)
        {
            if (bmSet == null) return;
            pbLeft.Image = bmSet;
            pbLeft.Location = new Point(0, 0);
            pbLeft.Width = pnLeft.Width;
            pbLeft.Height = pnLeft.Height;
            ZoomLeftMin = Math.Min((float)pbLeft.Width / pbLeft.Image.Width, (float)pbLeft.Height / pbLeft.Image.Height);
            ZoomLeft = ZoomLeftMin;
        }

        /// <summary>
        /// Triggers automatic mask update after a polygon changed from editing, if enabled.
        /// </summary>
        private void EditPolygonChange()
        {
            if (ckbEditPolygonAutoUpdateMask.Checked == true)
            {
                tmAutoMaskUpdateTrigger.Enabled = false;        // stop timer
                tmAutoMaskUpdateTrigger.Enabled = true;         // and restart, so it is not constantly updating while moving
                lbCursorValuesRight.Text = "Polygon Changing!";
            }
        }

        /// <summary>
        /// Timer tick event of tmAutoMaskUpdateTrigger. This event is used to update the mask drawing on the right side when the polygon is edited on the left.
        /// The timer delays the mask drawing enough, until the mouse move has stopped for a moment. This prevents constant re-drawing while moving.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments</param>
        private void tmAutoMaskUpdateTrigger_Tick(object sender, EventArgs e)
        {
            tmAutoMaskUpdateTrigger.Enabled = false;                // stop both timers
            tmAutoMaskUpdateClear.Enabled = false;
            if (EditCurrentPolygon == true)
            {
                lbCursorValuesRight.Text = "Mask updated!";
                WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].Def = AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex];
                WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].NormalizedPolygon = NormalizedPolygon;       // update segmentation class object with latest edits
                DrawMasks();                                        // Draw all masks
                tmAutoMaskUpdateClear.Enabled = true;               // start the clear timer to remove the message
            }
        }

        /// <summary>
        /// Timer tick event of tmAutoMaskUpdateClear. Simply clear the text from the tmAutoMaskUpdateTrigger timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmAutoMaskUpdateClear_Tick(object sender, EventArgs e)
        {
            tmAutoMaskUpdateClear.Enabled = false;
            lbCursorValuesRight.Text = "";
        }

        #endregion Left Image

        #region Right Image
        /// <summary>
        /// MouseLeave event handler of the right PictureBox to reset CoordRight and RightScrollZoneCount.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments</param>
        private void pbRight_MouseLeave(object sender, EventArgs e)
        {
            CoordRight = new Point(-1, -1);
            lbCursorValuesRight.Text = "X: ----  Y: ---- Value: -----";
            RightScrollZoneCount = 0;
        }

        /// <summary>
        /// MouseMove event handler of the right PictureBox. Fetch the segmentation class codes from the code mask and if possible from the prediction mask.
        /// Perfrom the same scrolling as in pbLeft_MouseMove.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Mouse specific arguments</param>
        private void pbRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (pbRight.Image == null)
                return;

            try
            {
                CoordRight = new Point((int)(e.X / ZoomRight), (int)(e.Y / ZoomRight));
                lbCursorValuesRight.Text = "X: " + CoordRight.X.ToString() + "  Y: " + CoordRight.Y.ToString();
                if (bmCodeMask != null)
                {
                    // Get the segmentation class code from the code bitmap bmCodeMask, where the filled polygons were drawn with R,G, and B set to class code
                    Color color = bmCodeMask.GetPixel(CoordRight.X, CoordRight.Y);
                    int ID = color.R;                                   // getting one color part is enough
                    lbCursorValuesRight.Text += " ID: " + ID.ToString();
                    if (ID < AppSettings.SegmClassDefs.Length)
                        lbCursorValuesRight.Text += "   SegmClass: " + AppSettings.SegmClassDefs[ID].Name;

                    if (bmPredMaskCode != null)
                    {
                        // bmPredMaskCode has the same coding as above
                        color = bmPredMaskCode.GetPixel(CoordRight.X, CoordRight.Y);
                        ID = color.R;
                        lbCursorValuesRight.Text += "   PredMask ID: " + ID.ToString();
                        if (ID < AppSettings.SegmClassDefs.Length)
                            lbCursorValuesRight.Text += "   SegmClass: " + AppSettings.SegmClassDefs[ID].Name;
                    }
                }
            }
            catch
            {
                // no action on exception
            }

            if ((pbRight.Width > pnRight.Width) || (pbRight.Height > pnRight.Height))
            {
                int windowX = e.X + pbRight.Location.X;
                int windowY = e.Y + pbRight.Location.Y;
                int x = pbRight.Location.X;
                int y = pbRight.Location.Y;
                int moveMargin = (int)(AppSettings.ScrollZoneSize * ZoomRight + 0.5f);
                int moveDelta = (int)(AppSettings.ScrollMoveFactor * ZoomRight + 0.5f);

                if (windowX < moveMargin)
                    x = Math.Min(x + moveDelta, 0);
                else if (windowX > pnRight.Width - moveMargin)
                    x = Math.Max(x - moveDelta, pnRight.Width - pbRight.Width);

                if (windowY < moveMargin)
                    y = Math.Min(y + moveDelta, 0);
                else if (windowY > pnRight.Height - moveMargin)
                    y = Math.Max(y - moveDelta, pnRight.Height - pbRight.Height);

                if ((pbRight.Location.X != x) || (pbRight.Location.Y != y))
                {
                    if (++RightScrollZoneCount > AppSettings.ScrollStartMinCount)
                    {
                        pbRight.Location = new Point(x, y);
                        if (ckbSyncLeftRight.Checked == true)
                            pbLeft.Location = pbRight.Location;
                    }
                }
                else
                    RightScrollZoneCount = 0;

            }
            else
                RightScrollZoneCount = 0;

        }


        /// <summary>
        /// Mouse wheel event handler of the right PictureBox. It is used to zoom in and out keeping the current cursor position in about the same place.
        /// </summary>
        /// <param name="sender">Sender of notification</param>
        /// <param name="e">Standard event arguments.</param>
        private void pbRight_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pbRight.Image == null) return;

            int idxX = (int)(e.X / ZoomRight);
            int idxY = (int)(e.Y / ZoomRight);
            float delta = e.Delta / 240.0f;
            ZoomRight = Math.Max(Math.Min(ZoomRight + delta, 32 * ZoomRightMin), ZoomRightMin);

            if (ZoomRight > ZoomRightMin)
            {
                pbRight.Size = new Size((int)(pbRight.Image.Width * ZoomRight + 0.5f), (int)(pbRight.Image.Height * ZoomRight + 0.5f));
                pbRight.Location = new Point(Math.Min(pbRight.Left + e.X - (int)(idxX * ZoomRight), 0), Math.Min(pbRight.Top + e.Y - (int)(idxY * ZoomRight), 0));
            }
            else
            {
                pbRight.Size = pnRight.Size;
                pbRight.Location = new Point(0, 0);
            }

            if (ckbSyncLeftRight.Checked == true)
            {
                pbLeft.Size = pbRight.Size;
                pbLeft.Location = pbRight.Location;
            }
        }

        /// <summary>
        /// Assign a Bitmap to the right PictureBox image and reset settings to defaults.
        /// </summary>
        /// <param name="bmSet">Reference to the Bitmap object to assign to pbLeft.Image. </param>
        private void SetRightImage(Bitmap bmSet)
        {
            if (bmSet == null) return;
            pbRight.Image = bmSet;
            pbRight.Location = new Point(0, 0);
            pbRight.Width = pnRight.Width;
            pbRight.Height = pnRight.Height;
            ZoomRightMin = Math.Min((float)pbRight.Width / pbRight.Image.Width, (float)pbRight.Height / pbRight.Image.Height);
            ZoomRight = ZoomRightMin;
        }

        #endregion Right Image

        #region Left Side Controls Event Handler
        /// <summary>
        /// CheckBox changed event handler to display or hide the steering directions.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbShowSteeringAngles_CheckedChanged(object sender, EventArgs e)
        {
            pbLeft.Invalidate();
        }

        /// <summary>
        /// CheckBox changed event handler to switch the image between full resolution and target size.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbLeftImageTargetSize_CheckedChanged(object sender, EventArgs e)
        {
            Rectangle r = pbLeft.Bounds;
            if (ckbLeftImageTargetSize.Checked == true)
                SetLeftImage(bmTargetSizeImg);
            else
                SetLeftImage(bmWorkingImg);
            pbLeft.Bounds = r;
        }

        /// <summary>
        /// CheckBox changed event handler to switch between displaying all polygons or only the selected one.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbLeftDrawActivePolygonOnly_CheckedChanged(object sender, EventArgs e)
        {
            pbLeft.Invalidate();
            if ((ckbSyncLeftRight.Checked == true) && (ckbRightShowSelectedOnly.Checked != ckbLeftDrawActivePolygonOnly.Checked))
                ckbRightShowSelectedOnly.Checked = ckbLeftDrawActivePolygonOnly.Checked;
        }

        /// <summary>
        /// Button click event handler to start entering a new polygon.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnNewPolygon_Click(object sender, EventArgs e)
        {
            NormalizedPolygon = new PointF[1];
            NormalizedPolygon[0] = new PointF(0, 0);
            DrawPolygon = new PointF[1];
            DrawPolygon[0] = new PointF(0, 0);
            EnterNewPolygon = true;
            EditCurrentPolygon = false;
            HighlightCurrentPolygon = false;
            UpdateButtonAndMenuEnable();
            EditPolygonPointIdx = -1;
            pbLeft.Invalidate();
        }

        /// <summary>
        /// Button click event handler to finish entering a new polygon and to add it in a new segmentation class object.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnFinishSegmClass_Click(object sender, EventArgs e)
        {
            EnterNewPolygon = false;
            EditCurrentPolygon = false;
            HighlightCurrentPolygon = false;
            EditPolygonPointIdx = -1;
 
            if ((NormalizedPolygon != null) && (NormalizedPolygon.Length > 2))
            {
                SegmClass newEntry = new SegmClass(AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex], NormalizedPolygon);
                WorkingSegmClasses.Add(newEntry);
                cbCurrentSegmClasses.Items.Add(newEntry.Def.Name);
                cbCurrentSegmClasses.SelectedIndex = cbCurrentSegmClasses.Items.Count - 1;
                HighlightCurrentPolygon = false;
                pbLeft.Invalidate();
                DrawMasks();
                SegmClassesEdited = true;
            }

            UpdateButtonAndMenuEnable();
            NormalizedPolygon = null;
            DrawPolygon = null;
        }


        /// <summary>
        /// Button click event handler to start editing the polygon selected by the ComboBox cbCurrentSegmClasses.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnEditPolygon_Click(object sender, EventArgs e)
        {
            EditPolygonPointIdx = -1;
            if ((cbCurrentSegmClasses.Items.Count == 0) || (cbCurrentSegmClasses.SelectedIndex < 0)) return;
            EditCurrentPolygon = !EditCurrentPolygon;
            if (EditCurrentPolygon == true)
            {
                btnEditPolygon.Text = "End Edit";
                UpdateButtonAndMenuEnable();
                NormalizedPolygon = WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].NormalizedPolygon;
                DrawPolygon = WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].DrawPolygon;
                cbSegmClass.SelectedIndex = WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].Def.ID;
            }
            else
            {
                btnEditPolygon.Text = "Edit Polygon";
                tmAutoMaskUpdateTrigger.Enabled = false;
                tmAutoMaskUpdateClear.Enabled = false;
                if (WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].Def.Name != AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex].Name)
                {
                    if (MessageBox.Show("Do you want to change the current segmentation class \"" + WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].Def.Name + "\" to \"" + AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex].Name + "\"?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cbCurrentSegmClasses.Items[cbCurrentSegmClasses.SelectedIndex] = AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex].Name;
                        WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].Def = AppSettings.SegmClassDefs[cbSegmClass.SelectedIndex];
                    }
                }
                WorkingSegmClasses[cbCurrentSegmClasses.SelectedIndex].NormalizedPolygon = NormalizedPolygon;
                NormalizedPolygon = null;
                DrawPolygon = null;
                UpdateButtonAndMenuEnable();
                DrawMasks();
                SegmClassesEdited = true;
            }
            HighlightCurrentPolygon = false;
            pbLeft.Invalidate();
        }


        /// <summary>
        /// Button click event handler to delete the segmentation class object selected by the ComboBox cbCurrentSegmClasses.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void btnDeleteSegmClass_Click(object sender, EventArgs e)
        {
            if ((cbCurrentSegmClasses.Items.Count == 0) || (cbCurrentSegmClasses.SelectedIndex < 0)) return;
            if (MessageBox.Show("Do you really want to delete the selected segmentation class \"" + cbCurrentSegmClasses.SelectedItem + "\" from the list?", "Confirmation", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int idx = cbCurrentSegmClasses.SelectedIndex;
                WorkingSegmClasses.RemoveAt(idx);
                cbCurrentSegmClasses.Items.RemoveAt(idx);
                if (cbCurrentSegmClasses.Items.Count == 0)
                    cbCurrentSegmClasses.Text = "";
                pbLeft.Invalidate();
                DrawMasks();
                SegmClassesEdited = true;
            }
        }

        /// <summary>
        /// ComboBox SelectedIndexChanged event handler of cbCurrentSegmClasses to select highlight and select one of the segmentation class objects out the existing list.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void cbCurrentSegmClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ckbLeftDrawActivePolygonOnly.Checked == false)
            {
                HighlightCurrentPolygon = true;
                HighlightCurrentPolygonIdx = cbCurrentSegmClasses.SelectedIndex;
            }
            pbLeft.Invalidate();

            if (ckbRightShowSelectedOnly.Checked == true)
                DrawMasks();
        }
        #endregion Left Side Controls Event Handler

        #region Right Side Controls Event Handler

        /// <summary>
        /// CheckBox changed event handler to switch between displaying all polygon areas on thr right or only the selected one.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbRightShowOnlySelected_CheckedChanged(object sender, EventArgs e)
        {
            DrawMasks();
            if ((ckbSyncLeftRight.Checked == true) && (ckbLeftDrawActivePolygonOnly.Checked != ckbRightShowSelectedOnly.Checked))
                ckbLeftDrawActivePolygonOnly.Checked = ckbRightShowSelectedOnly.Checked;
        }

        /// <summary>
        /// CheckBox changed event handler to switch between displaying all polygons with overlapping areas or exclusive overwriting eachother in the draw order.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbRightShowOverlap_CheckedChanged(object sender, EventArgs e)
        {
            DrawMasks();
        }

        /// <summary>
        /// CheckBox changed event handler to switch manually between mask drawn from segmentation classes or generated prediction maask on the right.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbShowPredictionMask_CheckedChanged(object sender, EventArgs e)
        {
            Rectangle r = pbRight.Bounds;
            if (ckbShowPredictionMask.Checked == true)
                SetRightImage(bmPredMaskColored);
            else
                SetRightImage(bmImageMask);
            pbRight.Bounds = r;
        }

        /// <summary>
        /// CheckBox changed event handler to enable or disable timer controlled switching between mask drawn from segmentation classes or generated prediction mask on the right.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void ckbAutoToggleMasks_CheckedChanged(object sender, EventArgs e)
        {
            tmToggleMasks.Enabled = ckbAutoToggleMasks.Checked; // && ckbShowPredictionMask.Enabled;
        }

        /// <summary>
        /// Timer tick event handler of tmToggleMasks to alternate between mask drawn from segmentation classes or generated prediction mask.
        /// </summary>
        /// <param name="sender">Sender of notification.</param>
        /// <param name="e">Standard event arguments.</param>
        private void tmToggleMasks_Tick(object sender, EventArgs e)
        {
            if (ckbShowPredictionMask.Enabled == true)
                ckbShowPredictionMask.Checked = !ckbShowPredictionMask.Checked;
            else
                tmToggleMasks.Enabled = false;
        }

        #endregion Right Side Controls Event Handler

        #region Augmentation

        /// <summary>
        /// Clears a directory by deleting all files in it.
        /// </summary>
        /// <param name="Dir">Full directory path.</param>
        private void ClearDirectory(string Dir)
        {
            if (Directory.Exists(Dir) == true)
            {
                string[] fileNames = Directory.GetFiles(Dir);
                foreach (string fileName in fileNames)
                    File.Delete(fileName);
            }
            else Directory.CreateDirectory(Dir);
        }

        /// <summary>
        /// Clears all output directories by deleting all files in those directories.
        /// </summary>
        private void ClearOutputDirs()
        {
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirImg + AppSettings.SubDirTrain);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirImg + AppSettings.SubDirVal);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirImg + AppSettings.SubDirPredTest);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirMask + AppSettings.SubDirTrain);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirMask + AppSettings.SubDirVal);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirMask + AppSettings.SubDirPredTest);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo + AppSettings.SubDirTrain);
            ClearDirectory(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo + AppSettings.SubDirVal);
            if (AppSettings.InfoOutput == false)
            {
                Directory.Delete(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo + AppSettings.SubDirTrain);
                Directory.Delete(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo + AppSettings.SubDirVal);
                Directory.Delete(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo);
            }
        }

        /// <summary>
        /// Returns a string by concatenating the string representations of passed values to be used as base file name for images, mask and info.
        /// </summary>
        /// <param name="ImgNo">Image number or index in the session.</param>
        /// <param name="ZoomFactor">Current zoom factor of the augmentation.</param>
        /// <param name="TiltAngle">Current tilt angle of the augmentation.</param>
        /// <param name="BrightnessFactor">Current b of the brithness factor of the augmentation.</param>
        /// <param name="ContrastEnhancement">Current contrast enhancement value of the augmentation.</param>
        /// <param name="NoiseAdderIdx">Current noise adder index of the augmentation instead of the noise adder value. Noise values can be the same for a number of augmentations.</param>
        /// <returns>A string built from all input values to be used as base file name.</returns>
        private string BuildBaseFileName(int ImgNo, float ZoomFactor, int TiltAngle, float BrightnessFactor, float ContrastEnhancement, int NoiseAdderIdx)
        {
            string s = "";
            if (ImgNo >= 0)
                s += ImgNo.ToString("D4") + "_";

            if (ZoomFactor >= 0)
                s += "z" + ZoomFactor.ToString("F2") + "_";

            if ((TiltAngle >= 0) && (TiltAngle <= 90))
                s += "t" + TiltAngle.ToString("D3") + "_";
            else if ((TiltAngle < 0) && (TiltAngle >= -90))
                s += "t" + TiltAngle.ToString("D2") + "_";

            if (BrightnessFactor >= 0)
                s += "b" + BrightnessFactor.ToString("F1") + "_";

            if (ContrastEnhancement >= 0)
                s += "c" + ContrastEnhancement.ToString("F1") + "_";

            if (NoiseAdderIdx >= 0)
                s += "n" + NoiseAdderIdx.ToString("D2") + "_";

            return s.TrimEnd(new char[] { '_' });
        }

        /// <summary>
        /// Processes one augmentation output with the passed parameter and assigns the result to the left PictureBox.
        /// </summary>
        /// <param name="ZoomFactor">Current zoom factor of the augmentation.</param>
        /// <param name="TiltAngle">Current tilt angle of the augmentation.</param>
        /// <param name="BrightnessFactor">Current b of the brithness factor of the augmentation.</param>
        /// <param name="ContrastEnhancement">Current contrast enhancement value of the augmentation.</param>
        /// <param name="NoiseAdder">Current noise adder value of the augmentation.</param>
        public void ProcessAugmentation(float ZoomFactor, int TiltAngle, float BrightnessFactor, float ContrastEnhancement, int NoiseAdder)
        {
            SetEnabled(false);
            bool updated = false;
            if ((bmOriginalImg != bmAugmentationInput) || (ZoomFactor != lastZoomFactor) || (TiltAngle != lastTiltAngle) || (bmTiltZoomResult == null) || (bmTiltZoomResultTargetSize == null))
            {
                bmTiltZoomResult = Process.TiltImage(bmOriginalImg, TiltAngle, ZoomFactor);
                SetWorkingImage(bmTiltZoomResult);
                WorkingSegmClasses = Process.TiltPolygons(OriginalSegmClasses, TiltAngle, ZoomFactor);
                DrawMasks();

                bmAugmentationInput = bmOriginalImg;
                bmTiltZoomResultTargetSize = bmTargetSizeImg;
                lastZoomFactor = ZoomFactor;
                lastTiltAngle = TiltAngle;
                updated = true;
            }

            if ((updated == true) || (BrightnessFactor != lastBrightnessFactor) || (bmBrightnessResult == null))
            {
                bmBrightnessResult = Process.ImageBrightness(bmTiltZoomResultTargetSize, BrightnessFactor);
                lastBrightnessFactor = BrightnessFactor;
                updated = true;
            }

            if ((updated == true) || (ContrastEnhancement != lastContrastEnhancement) || (bmContrastResult == null))
            {
                bmContrastResult = Process.ImageContrast(bmBrightnessResult, ContrastEnhancement);
                lastContrastEnhancement = ContrastEnhancement;
            }

            bmTargetSizeImg = Process.ImageNoise(bmContrastResult, NoiseAdder);
            pbLeft.Image = bmTargetSizeImg;
            SetEnabled(true);
        }

        /// <summary>
        /// Processes one augmentation output with the passed parameter and assigns the result to the left PictureBox.
        /// </summary>
        /// <param name="ImgNo">Image number to be used in the file name.</param>
        /// <param name="ZoomFactor">Current zoom factor of the augmentation.</param>
        /// <param name="TiltAngle">Current tilt angle of the augmentation.</param>
        /// <param name="BrightnessFactor">Current b of the brithness factor of the augmentation.</param>
        /// <param name="ContrastEnhancement">Current contrast enhancement value of the augmentation.</param>
        /// <param name="NoiseAdder">Current noise adder value of the augmentation.</param>
        /// <param name="NoiseIdx">Index of the current noise application to be used in the file name.</param>
        /// <param name="Rnd">Reference to the random object used.</param>
        private void ProcessAugmentationAndSave(int ImgNo, float ZoomFactor, int TiltAngle, float BrightnessFactor, float ContrastEnhancement, int NoiseAdder, int NoiseIdx, Random Rnd)
        {
            string baseName = BuildBaseFileName(ImgNo, ZoomFactor, TiltAngle, BrightnessFactor, ContrastEnhancement, NoiseIdx);
            Text = APP_NAME + " -> processing: " + baseName;
            Refresh();
            ProcessAugmentation(ZoomFactor, TiltAngle, BrightnessFactor, ContrastEnhancement, NoiseAdder);
            pbLeft.Refresh();
            pbRight.Refresh();
            SetEnabled(false);

            string trainVal = AppSettings.SubDirTrain;
            if (AppSettings.TrainValRatio == 0)
            {
                if ((ZoomFactor == 1) && (TiltAngle == 0) && (BrightnessFactor == 1) && (ContrastEnhancement == 0) && (NoiseAdder == 0))
                    trainVal = AppSettings.SubDirVal;
            }
            else if (Rnd.Next(AppSettings.TrainValRatio) == 0)
                trainVal = AppSettings.SubDirVal;

            bmTargetSizeImg.Save(AppSettings.PathToOutputDataset + AppSettings.SubDirImg + trainVal + AppSettings.PrefixImg + baseName + ".jpg");
            bm8bitMask.Save(AppSettings.PathToOutputDataset + AppSettings.SubDirMask + trainVal + AppSettings.PrefixMask + baseName + ".png");
            if (AppSettings.InfoOutput == true)
                SaveSegmClassesToXml(AppSettings.PathToOutputDataset + AppSettings.SubDirInfo + trainVal + AppSettings.PrefixInfo + baseName + ".xml", Session.CurrentImageFileName, AppSettings.PrefixImg + baseName + ".jpg", bmTargetSizeImg, WorkingSegmClasses,  WorkingSteeringDirs);

            Text = APP_NAME;
        }

        /// <summary>
        /// Process all image augmentations and create all JPG,PNG and XML files in the output dataset folders fro traing and validation.
        /// </summary>
        /// <param name="StartImgIdx">Index of the first image to start processing</param>
        /// <param name="EndImgIdx">Index of the last image to finish processing</param>
        private void ProcessAllImageAugmentations(int StartImgIdx, int EndImgIdx)
        { 
            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int idxImg = StartImgIdx; idxImg <= EndImgIdx; idxImg++) 
            {
                bmOriginalImg = Session.GetProcessedImage(idxImg);
                if (bmOriginalImg == null)
                    break;

                if (SAVE_BLURED_ORGINAL_IMAGE == true)
                {
                    if ((AppSettings.BlurEnabled == true) && (AppSettings.BlurAreaPolygon != null) && (AppSettings.BlurAreaPolygon.Length > 2))
                    {
                        PointF[] poly = new PointF[AppSettings.BlurAreaPolygon.Length];
                        for (int i = 0; i < poly.Length; i++)
                            poly[i] = new PointF(AppSettings.BlurAreaPolygon[i].X, AppSettings.BlurAreaPolygon[i].Y); // == 0 ? 0 : AppSettings.BlurAreaPolygon[i].Y + 0.03f);
                        Bitmap bmBlured = Process.BlurArea(bmOriginalImg, poly, AppSettings.BlurWindowSize, AppSettings.BlurStepSize);
                        bmOriginalImg.Dispose();
                        bmOriginalImg = bmBlured;
                        bmOriginalImg.Save(Session.CurrentImageFileNameFull);
                    }
                }

                SetWorkingImage(bmOriginalImg);
                LoadSegmClasses();
                // Store the downsampled image for the prediction in the test subfolder
                Bitmap bmTest = Process.DownSample(bmOriginalImg, AppSettings.ImageOutputSize.Width, AppSettings.ImageOutputSize.Height);
                bmTest.Save(AppSettings.PathToOutputDataset + AppSettings.SubDirImg + AppSettings.SubDirPredTest + Session.CurrentImageFileName+".jpg");
                bmTest.Dispose();
                // Store the related mask for StreetMaker only, when dataset is mixed with StreetMaker dataset so it can be viewed there
                bm8bitMask.Save(AppSettings.PathToOutputDataset + AppSettings.SubDirMask + AppSettings.SubDirPredTest + Session.CurrentImageFileName + ".png");
                
                for (int idxZoom = 0; idxZoom < AppSettings.ZoomFactors.Length; idxZoom++)
                {
                    float zoomFactor = AppSettings.ZoomFactors[idxZoom];
                    for (int idxTilt = 0; idxTilt < AppSettings.TiltAngles.Length; idxTilt++)
                    {
                        int tiltAngle = AppSettings.TiltAngles[idxTilt];
                        if (((Math.Abs(tiltAngle) >= 15) && (zoomFactor >= 1.3f)) || 
                            ((Math.Abs(tiltAngle) >= 10) && (zoomFactor >= 1.2f)) || 
                            ((Math.Abs(tiltAngle) >= 5) && (zoomFactor >= 1.1f)) || 
                            ((Math.Abs(tiltAngle) == 0) && (zoomFactor >= 1.0f)))
                        {
                            for (int idxBright = 0; idxBright < AppSettings.BrightnessFactors.Length; idxBright++)
                            {
                                float brightnessFactor = AppSettings.BrightnessFactors[idxBright];
                                for (int idxNoise = 0; idxNoise < AppSettings.NoiseAdders.Length; idxNoise++)
                                {
                                    int noiseAdder = AppSettings.NoiseAdders[idxNoise];
                                    ProcessAugmentationAndSave(idxImg, zoomFactor, tiltAngle, brightnessFactor, 0, noiseAdder, idxNoise, rnd);
                                }
                            }
                            for (int idxContrast = 0; idxContrast < AppSettings.ContrastEnhancements.Length; idxContrast++)
                            {
                                float contrastEnhancement = AppSettings.ContrastEnhancements[idxContrast];
                                for (int idxNoise = 0; idxNoise < AppSettings.NoiseAdders.Length; idxNoise++)
                                {
                                    int noiseAdder = AppSettings.NoiseAdders[idxNoise];
                                    ProcessAugmentationAndSave(idxImg, zoomFactor, tiltAngle, 1, contrastEnhancement, noiseAdder, idxNoise, rnd);
                                }
                            }
                            Application.DoEvents();
                        }
                    }
                }
            }
        }
        #endregion Augmentation
 
        private void button2_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            for (int i = 0; i < OriginalSegmClasses.Count; i++)
                if (OriginalSegmClasses[i].Def.ID == 0)
                {
                    Bitmap bmBlured = Process.BlurArea(bmOriginalImg, OriginalSegmClasses[i].NormalizedPolygon, new Size((int)nudWindowSize.Value, (int)nudWindowSize.Value), new Size((int)nudStepSize.Value, (int)nudStepSize.Value));
                    SetWorkingImage(bmBlured);
                    DrawMasks();

                    AppSettings.BlurWindowSize = new Size((int)nudWindowSize.Value, (int)nudWindowSize.Value);
                    AppSettings.BlurStepSize = new Size((int)nudStepSize.Value, (int)nudStepSize.Value);
                    AppSettings.BlurAreaPolygon = OriginalSegmClasses[i].NormalizedPolygon;
                    AppSettings.SaveSettings();
                    break;
                }
            SetEnabled(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((AppSettings.BlurAreaPolygon != null) && (AppSettings.BlurAreaPolygon.Length > 2))
            {
                PointF[] poly = new PointF[AppSettings.BlurAreaPolygon.Length];
                for (int i = 0; i < poly.Length; i++)
                    poly[i] = new PointF(AppSettings.BlurAreaPolygon[i].X, AppSettings.BlurAreaPolygon[i].Y == 0 ? 0 : AppSettings.BlurAreaPolygon[i].Y + 0.03f);
                Bitmap bmBlured = Process.BlurArea(bmOriginalImg, poly, AppSettings.BlurWindowSize, AppSettings.BlurStepSize);
                bmOriginalImg.Dispose();
                bmOriginalImg = bmBlured;
                bmOriginalImg.Save(Session.CurrentImageFileNameFull);
            }
        }


    }
}
