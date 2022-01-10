// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace ImageSegmenter
{
    /// <summary>
    /// Storage class for global application settings. This class includes saving its contents to an XML file and loading from it.
    /// </summary>
    public class AppSettings
    {
        #region Public Constants
        /// <summary>Minimum limit for ZoomFactors values.</summary>
        public const float ZOOM_FACTOR_MIN = 0.5f;
        /// <summary>Maximum limit for ZoomFactors values.</summary>
        public const float ZOOM_FACTOR_MAX = 2.0f;

        /// <summary>Minimum limit for TiltAngles values.</summary>
        public const int TILT_ANGLE_MIN = -45;
        /// <summary>Maximum limit for TiltAngles values.</summary>
        public const int TILT_ANGLE_MAX = +45;

        /// <summary>Minimum limit for BrightnessFactors values.</summary>
        public const float BRIGHTNESS_FACTOR_MIN = 0.5f;
        /// <summary>Maximum limit for BrightnessFactors values.</summary>
        public const float BRIGHTNESS_FACTOR_MAX = 2.0f;

        /// <summary>Minimum limit for ContrastEnhancements values.</summary>
        public const float CONTRAST_ENHANCEMENT_MIN = 0;
        /// <summary>Maximum limit for ContrastEnhancements values.</summary>
        public const float CONTRAST_ENHANCEMENT_MAX = 1;

        /// <summary>Minimum limit for NoiseAdders values.</summary>
        public const int NOISE_ADDER_MIN = 0;
        /// <summary>Maximum limit for NoiseAdders values.</summary>
        public const int NOISE_ADDER_MAX = 25;
        #endregion Public Constants

        #region Private Fields
        /// <summary>Holds the full path and file name of the XML to read from or write to.</summary>
        private string FileNameAppSettings;
        /// <summary>True, when the XML file existed at creation of the object.</summary>
        private bool settingsFileExisted = false;
        /// <summary>True, when the XML file could be loaded without any error.</summary>
        private bool settingsFileLoadedCorrectly = false;
        #endregion Private Fields

        #region Public Fields
        #region Folders and File Prefixes
        /// <summary>Full path to the folder containing all source images.</summary>
        public string PathToSourceImages = @"Recording\";
        /// <summary>Full path to the folder to store the session XML files.</summary>
        public string PathToSessionData = @"Session\";
        /// <summary>Full path to the folder to store all output dataset files, like images, mask and xml files.</summary>
        public string PathToOutputDataset = @"DataSet\";
        /// <summary>Full path to the folder containing prediction masks produced by the Jupyter note book for result checking.</summary>
        public string PathToPredictedMasks = @"Prediction\";

        /// <summary>Sub directory name under PathToOutputDataset\ for augmented JPG images.</summary>
        public string SubDirImg = @"Img\";
        /// <summary>Sub directory name under PathToOutputDataset\ for augmented PNG masks.</summary>
        public string SubDirMask = @"Mask\";
        /// <summary>Sub directory name under PathToOutputDataset\ for augmented XML info files.</summary>
        public string SubDirInfo = @"Info\";

        /// <summary>Sub directory name under PathToOutputDataset\XXX\ for augmented files for training.</summary>
        public string SubDirTrain = @"Train\";
        /// <summary>Sub directory name under PathToOutputDataset\XXX\ for augmented files for validation.</summary>
        public string SubDirVal = @"Val\";

        /// <summary>File name prefix for JPG image files.</summary>
        public string PrefixImg = "Img_";
        /// <summary>File name prefix for PNG mask files.</summary>
        public string PrefixMask = "Mask_";
        /// <summary>File name prefix for XML info files.</summary>
        public string PrefixInfo = "Info_";
        #endregion Folders and File Prefixes

        #region Segmentation classes
        /// <summary>
        /// Default segmentation class definitions as loaded initially.
        /// </summary>
        public SegmClassDef[] SegmClassDefs =
        {
            new SegmClassDef( 0, "nothing",                     10, Color.FromArgb(0x00,0x00,0x00)),     //  0
            new SegmClassDef( 1, "white_solid_line",             9, Color.FromArgb(0xE7,0x0E,0x13)),     //  1
            new SegmClassDef( 2, "white_dashed_line",            9, Color.FromArgb(0xF7,0x66,0x0F)),     //  2
            new SegmClassDef( 3, "yellow_solid_line",            8, Color.FromArgb(0xBE,0x1E,0xFB)),     //  3
            new SegmClassDef( 4, "yellow_dashed_line",           8, Color.FromArgb(0xA2,0x5E,0xFD)),     //  4
            new SegmClassDef( 5, "lane_my_dir",                  3, Color.FromArgb(0x00,0xFF,0x00)),     //  5
            new SegmClassDef( 6, "lane_wrong_dir",               0, Color.FromArgb(0x8A,0x00,0x00)),     //  6
            new SegmClassDef( 7, "lane_left_turn",               1, Color.FromArgb(0x8B,0xB8,0x03)),     //  7
            new SegmClassDef( 8, "lane_right_turn",              2, Color.FromArgb(0x00,0x80,0x40)),     //  8
            new SegmClassDef( 9, "yield_sign",                   7, Color.FromArgb(0xFF,0x80,0x00)),     //  9
            new SegmClassDef(10, "stop_sign",                    7, Color.FromArgb(0xFF,0x00,0x00)),     // 10
            new SegmClassDef(11, "wait_line",                    5, Color.FromArgb(0x00,0xA6,0xA6)),     // 11
            new SegmClassDef(12, "arrow_straight",               6, Color.FromArgb(0xD7,0xAC,0x00)),     // 12
            new SegmClassDef(13, "arrow_straight_left",          6, Color.FromArgb(0xDF,0x68,0x62)),     // 13
            new SegmClassDef(14, "arrow_straight_right",         6, Color.FromArgb(0xC0,0x00,0x80)),     // 14
            new SegmClassDef(15, "arrow_left_straight_right",    6, Color.FromArgb(0xC0,0x80,0xC0)),     // 15
            new SegmClassDef(16, "arrow_left_only",              6, Color.FromArgb(0x82,0xBF,0x00)),     // 16
            new SegmClassDef(17, "arrow_right_only",             6, Color.FromArgb(0x00,0xC0,0x80)),     // 17
            new SegmClassDef(18, "arrow_left_right",             6, Color.FromArgb(0x80,0x00,0x80)),     // 18
            new SegmClassDef(19, "arrow_roundabout",             6, Color.FromArgb(0x00,0xC1,0x00)),     // 19
            new SegmClassDef(20, "merge_left",                   7, Color.FromArgb(0xFF,0x40,0x00)),     // 20
            new SegmClassDef(21, "lane_limit",                   4, Color.FromArgb(0xD7,0xD7,0x00)),     // 21
        };
        #endregion Segmentation classes

        #region Blur Area
        /// <summary>If a BlurAreaPolygon is defined, BlurEnabled=true forces to execute a very time intensive low pass over the polygon area each time an image is loaded.</summary>
        public bool BlurEnabled = false;
        /// <summary>Blur filter window size</summary>
        public Size BlurWindowSize = new Size(40, 40);
        /// <summary>Blur filter step size</summary>
        public Size BlurStepSize = new Size(4, 4);
        /// <summary>Polygon to surround the area to be blurred</summary>
        public PointF[] BlurAreaPolygon = new PointF[0];
        #endregion Blur Area

        #region DataSet output
        /// <summary>Image and mask output size for traing and validation.</summary>
        public Size ImageOutputSize = new Size(224, 224);
        /// <summary>Ratio between traing and validation output. 50 for instance means that about every 50th image,mask and info set will be placed into the validation folders instead of traing folders.</summary>
        public int TrainValRatio = 50;
        /// <summary>MaskDrawOrderMin and MaskDrawOrderMax allow creating a dataset excluding draw order levels outside of these limits.</summary>
        public int MaskDrawOrderMin = 0;
        /// <summary>MaskDrawOrderMin and MaskDrawOrderMax allow creating a dataset excluding draw order levels outside of these limits.</summary>
        public int MaskDrawOrderMax = 1000;
        /// <summary>If true, tilts and zoom levels are combined to produce images without black areas in augmentations.</summary>
        public bool TiltWithZoomOnly = true;
        /// <summary>If true, BrightnessFactors and ContrastEnhancements are not applied on top of eachother in augmentations.</summary>
        public bool BrightnessContrastExclusive = true;

        /// <summary>Zoom factor values to go through in an augmentation run.</summary>
        public float[] ZoomFactors = { 1.0f, 1.2f, 0.8f };
        /// <summary>Tilt angle values to go through in an augmentation run.</summary>
        public int[] TiltAngles = { 0, 5, 10, 15, -5, -10, -15 };
        /// <summary>Brightness factor values to go through in an augmentation run.</summary>
        public float[] BrightnessFactors = { 1f, 0.8f, 1.2f };
        /// <summary>Contrast enhancement values to go through in an augmentation run.</summary>
        public float[] ContrastEnhancements = { 0, 0.5f };
        /// <summary>Noise adder values to go through in an augmentation run.</summary>
        public int[] NoiseAdders = { 0, 2, 4 };
        #endregion DataSet output

        #region GUI parameter
        /// <summary>Color definition for the polygon lines drawn onto the image.</summary>
        public Color PolygonLineColor = Color.Red;
        /// <summary>Color definition for the polygon point makrers drawn onto the image.</summary>
        public Color PolygonPointColor = Color.Blue;
        /// <summary>Circle size of the polygon point markers.</summary>
        public float PolygonPointSize = 1.5f;

        /// <summary>Trasnparency value to be used for drawing areas to the mask image as value between 0 and 255.</summary>
        public int DrawMaskTransparency = 100;
        /// <summary>Zone around the bounds of the image, where the mouse position can trigger a scroll movement</summary>
        public float ScrollZoneSize = 2f;
        /// <summary>Scroll movement factor when scrolling is activated with each mouse move update.</summary>
        public float ScrollMoveFactor = 0.5f;
        /// <summary>When the mouse coordinates move into the scroll zone around the edges, a count down is triggered first before the scroll movement is initiated. 
        /// This allows moving from the image to a button without immediately scrolling away.</summary>
        public int ScrollStartMinCount = 7;
        #endregion GUI parameter

        #region GUI settings
        /// <summary>Value will be loaded to ToolStripMenuItem tsmiAutoLoadPredictedMask.Checked, which when true causes the automatic load of the prediction mask result, if available.</summary>
        public bool AutoLoadPredictedMask = true;
        /// <summary>Value will be loaded to CheckBox ckbLeftDrawActivePolygonOnly.Checked, which when true will cause to only draw the currently active polygon sintead of all.</summary>
        public bool LeftDrawActivePolygonOnly = false;
        /// <summary>Value will be loaded to CheckBox ckbLeftImageTargetSize.Checked, which when true will cause the left image to show target size resolution instead of full resolution.</summary>
        public bool LeftImageTargetSize = true;
        /// <summary>Value will be loaded to CheckBox ckbSyncLeftRight.Checked, which when true will cause several settings or movements between left and right image to be synchronized.</summary>
        public bool SyncLeftRight = true;
        /// <summary>Value will be loaded to CheckBox ckbEditPolygonAutoUpdateMask.Checked, which when true will cause an automatic mask update on the right image when editing a polygon on the left.</summary>
        public bool EditPolygonAutoUpdateMask = true;
        /// <summary>Value will be loaded to CheckBox ckbRightShowSelectedOnly.Checked, which when true will cause to display only the currently active polygon area on the right side instead of all.</summary>
        public bool RightShowSelectedOnly = false;
        /// <summary>Value will be loaded to CheckBox ckbRightShowOverlap.Checked, which when true will cause to display all polygon areas overlapping rather than overwriting each other in the draw order.</summary>
        public bool RightShowOverlap = false;
        /// <summary>Value will be loaded to CheckBox ckbShowPredictionMask.Checked, which when true will cause to display of the prediction mask in the right image rather than the mask created from the polygons.</summary>
        public bool ShowPredictionMask = true;
        /// <summary>Value will be loaded to CheckBox ckbAutoToggleMasks.Checked, which when true will cause a timer to toggle automatically between plygon mask and prediction mask.</summary>
        public bool AutoToggleMasks = true;
        #endregion GUI settings

        #endregion Public Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the AppSettings class including all defaults in the fields. If the XML file exists, it then loads all settings from the XML file overwriting the initial defaults.
        /// All directories and subdirectories will be created if they don't exist.
        /// If the XML file does not exists, it will be created by writing all defaults to that file.
        /// </summary>
        /// <param name="FileName">Full path and file name of the AppSettings XML file.</param>
        public AppSettings(string FileName)
        {
            FileNameAppSettings = FileName;

            string rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Python\tf-gpu\JetCar\Data\";
            PathToSourceImages = rootFolder + PathToSourceImages;
            PathToSessionData = rootFolder + PathToSessionData;
            PathToOutputDataset = rootFolder + PathToOutputDataset;
            PathToPredictedMasks = rootFolder + PathToPredictedMasks;

            if (File.Exists(FileNameAppSettings))
            {
                settingsFileExisted = true;
                LoadSettings();
            }
            else
                SaveSettings();

            CheckCreateDataFolders();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SourceSettings"></param>
        public AppSettings(AppSettings SourceSettings)
        {
            SourceSettings.CopyTo(this);
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// If not existing, creates the directory structure plus SubDir plus training and validation sub directories.
        /// </summary>
        /// <param name="SubDir"></param>
        private void CreateSubDirs(string SubDir)
        {
            if (Directory.Exists(PathToOutputDataset) == false)
                Directory.CreateDirectory(PathToOutputDataset);

            if (Directory.Exists(PathToOutputDataset + SubDir) == false)
                Directory.CreateDirectory(PathToOutputDataset + SubDir);

            if (Directory.Exists(PathToOutputDataset + SubDir + SubDirTrain) == false)
                Directory.CreateDirectory(PathToOutputDataset + SubDir + SubDirTrain);

            if (Directory.Exists(PathToOutputDataset + SubDir + SubDirVal) == false)
                Directory.CreateDirectory(PathToOutputDataset + SubDir + SubDirVal);
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Copies the contents of all public fields of this object to the target object.
        /// </summary>
        /// <param name="Target">Target AppSettings object to copy to.</param>
        public void CopyTo(AppSettings Target)
        {
            foreach (var s in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                Target.GetType().GetField(s.Name).SetValue(Target, s.GetValue(this));
        }

        /// <summary>
        /// Check the existance of the directories in the data folder structure and create all missing ones.
        /// </summary>
        public void CheckCreateDataFolders()
        {
            if (Directory.Exists(PathToSessionData) == false)
                Directory.CreateDirectory(PathToSessionData);

            if (Directory.Exists(PathToImageInfoFiles) == false)
                Directory.CreateDirectory(PathToImageInfoFiles);

            if (Directory.Exists(PathToPredictedMasks) == false)
                Directory.CreateDirectory(PathToPredictedMasks);

            CreateSubDirs(SubDirImg);
            CreateSubDirs(SubDirMask);
            CreateSubDirs(SubDirInfo);
        }


        /// <summary>
        /// Loads all public fields with the contents of the XML file specified by FileNameAppSettings and sets settingsFileLoadedCorrectly to true, if successful.
        /// In case of errors, an exception will bring up a MessageBox that an error occured. In that case, the XML file must be examined to find the issue.
        /// </summary>
        public void LoadSettings()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(FileNameAppSettings);
                XmlNode nodeSettings = doc.SelectSingleNode("settings");
                XmlNode nodeDataPath = nodeSettings.SelectSingleNode("data_path");
                PathToSourceImages = nodeDataPath.SelectSingleNode("source_images").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                PathToSessionData = nodeDataPath.SelectSingleNode("session_data").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                PathToOutputDataset = nodeDataPath.SelectSingleNode("output_dataset").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                PathToPredictedMasks = nodeDataPath.SelectSingleNode("predicted_masks").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodeSubDir = nodeSettings.SelectSingleNode("sub_dir_level_1");
                SubDirImg = nodeSubDir.SelectSingleNode("image").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirMask = nodeSubDir.SelectSingleNode("mask").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirInfo = nodeSubDir.SelectSingleNode("info").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodeTrainVal = nodeSettings.SelectSingleNode("sub_dir_level_2");
                SubDirTrain = nodeTrainVal.SelectSingleNode("train").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirVal = nodeTrainVal.SelectSingleNode("val").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodePrefix = nodeSettings.SelectSingleNode("prefix");
                PrefixImg = nodePrefix.SelectSingleNode("image").InnerText.TrimEnd(new char[] { '_' }) + '_';
                PrefixMask = nodePrefix.SelectSingleNode("mask").InnerText.TrimEnd(new char[] { '_' }) + '_';
                PrefixInfo = nodePrefix.SelectSingleNode("info").InnerText.TrimEnd(new char[] { '_' }) + '_';

                XmlNode nodeSegmClass = nodeSettings.SelectSingleNode("segm_class_defs");
                XmlNodeList nodeItems = nodeSegmClass.SelectNodes("item");
                SegmClassDefs = new SegmClassDef[nodeItems.Count];
                for (int i = 0; i < nodeItems.Count; i++)
                {
                    SegmClassDefs[i] = new SegmClassDef(
                        Convert.ToInt32(nodeItems[i].Attributes["id"].Value),
                        nodeItems[i].InnerText,
                        Convert.ToInt32(nodeItems[i].Attributes["draw_order"].Value),
                        Color.FromArgb(Convert.ToInt32(nodeItems[i].Attributes["draw_color"].Value, 16)));
                }

                XmlNode nodeOutputParm = nodeSettings.SelectSingleNode("output_parm");
                ImageOutputSize = new Size(Convert.ToInt32(nodeOutputParm.SelectSingleNode("image_width").InnerText), Convert.ToInt32(nodeOutputParm.SelectSingleNode("image_height").InnerText));

                XmlNode nodeBlurParm = nodeOutputParm.SelectSingleNode("blur_parm");
                BlurEnabled = Convert.ToBoolean(nodeBlurParm.SelectSingleNode("enabled").InnerText);
                BlurWindowSize = new Size(Convert.ToInt32(nodeBlurParm.SelectSingleNode("window_width").InnerText), Convert.ToInt32(nodeBlurParm.SelectSingleNode("window_height").InnerText));
                BlurStepSize = new Size(Convert.ToInt32(nodeBlurParm.SelectSingleNode("step_width").InnerText), Convert.ToInt32(nodeBlurParm.SelectSingleNode("step_height").InnerText));

                XmlNode nodePolygon = nodeBlurParm.SelectSingleNode("polygon");
                XmlNodeList pointPoints = nodePolygon.SelectNodes("point");
                BlurAreaPolygon = new PointF[pointPoints.Count];
                for (int i = 0; i < pointPoints.Count; i++)
                {
                    int idx = Convert.ToInt32(pointPoints[i].Attributes["idx"].Value);
                    float x = Convert.ToSingle(pointPoints[i].Attributes["x"].Value);
                    float y = Convert.ToSingle(pointPoints[i].Attributes["y"].Value);
                    BlurAreaPolygon[idx] = new PointF(x, y);
                }

                TrainValRatio = Convert.ToInt32(nodeOutputParm.SelectSingleNode("train_val_ratio").InnerText);
                MaskDrawOrderMin = Convert.ToInt32(nodeOutputParm.SelectSingleNode("mask_draw_order_min").InnerText);
                MaskDrawOrderMax = Convert.ToInt32(nodeOutputParm.SelectSingleNode("mask_draw_order_max").InnerText);

                XmlNode nodeZoomFactor = nodeOutputParm.SelectSingleNode("zoom_factor");
                XmlNodeList zoomItems = nodeZoomFactor.SelectNodes("item");
                ZoomFactors = new float[zoomItems.Count];
                for (int i = 0; i < zoomItems.Count; i++)
                    ZoomFactors[i] = Convert.ToSingle(zoomItems[i].InnerText);

                XmlNode nodeTiltAngles = nodeOutputParm.SelectSingleNode("tilt_angle");
                XmlNodeList tiltItems = nodeTiltAngles.SelectNodes("item");
                TiltAngles = new int[tiltItems.Count];
                for (int i = 0; i < tiltItems.Count; i++)
                    TiltAngles[i] = Convert.ToInt32(tiltItems[i].InnerText);

                XmlNode nodeBrightnessFactor = nodeOutputParm.SelectSingleNode("brightness_factor");
                XmlNodeList brightnessItems = nodeBrightnessFactor.SelectNodes("item");
                BrightnessFactors = new float[brightnessItems.Count];
                for (int i = 0; i < brightnessItems.Count; i++)
                    BrightnessFactors[i] = Convert.ToSingle(brightnessItems[i].InnerText);

                XmlNode nodeContrastEnhancement = nodeOutputParm.SelectSingleNode("contrast_enhancement");
                XmlNodeList contrastItems = nodeContrastEnhancement.SelectNodes("item");
                ContrastEnhancements = new float[contrastItems.Count];
                for (int i = 0; i < contrastItems.Count; i++)
                    ContrastEnhancements[i] = Convert.ToSingle(contrastItems[i].InnerText);

                XmlNode nodeNoiseAdder = nodeOutputParm.SelectSingleNode("noise_adder");
                XmlNodeList noiseItems = nodeNoiseAdder.SelectNodes("item");
                NoiseAdders = new int[noiseItems.Count];
                for (int i = 0; i < noiseItems.Count; i++)
                    NoiseAdders[i] = Convert.ToInt32(noiseItems[i].InnerText);

                XmlNode nodeGuiParm = nodeSettings.SelectSingleNode("gui_parm");
                PolygonLineColor = Color.FromArgb(Convert.ToInt32(nodeGuiParm.SelectSingleNode("polygon_line_color").InnerText, 16));
                PolygonPointColor = Color.FromArgb(Convert.ToInt32(nodeGuiParm.SelectSingleNode("polygon_point_color").InnerText, 16));
                PolygonPointSize = Convert.ToSingle(nodeGuiParm.SelectSingleNode("polygon_point_size").InnerText);
                DrawMaskTransparency = Convert.ToInt32(nodeGuiParm.SelectSingleNode("draw_mask_transparency").InnerText);
                ScrollZoneSize = Convert.ToSingle(nodeGuiParm.SelectSingleNode("scroll_zone_size").InnerText);
                ScrollMoveFactor = Convert.ToSingle(nodeGuiParm.SelectSingleNode("scroll_move_factor").InnerText);
                ScrollStartMinCount = Convert.ToInt32(nodeGuiParm.SelectSingleNode("scroll_start_min_count").InnerText);

                XmlNode nodeGuiSettings = nodeSettings.SelectSingleNode("gui_settings");
                AutoLoadPredictedMask = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("auto_load_predicted_mask").InnerText);
                LeftDrawActivePolygonOnly = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("left_draw_active_polygon_only").InnerText);
                LeftImageTargetSize = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("left_image_target_size").InnerText);
                SyncLeftRight = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("sync_left_right").InnerText);
                EditPolygonAutoUpdateMask = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("edit_polygon_auto_update_mask").InnerText);
                RightShowSelectedOnly = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("right_show_selected_only").InnerText);
                RightShowOverlap = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("right_show_overlap").InnerText);
                ShowPredictionMask = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("show_predicted_mask").InnerText);
                AutoToggleMasks = Convert.ToBoolean(nodeGuiSettings.SelectSingleNode("auto_toggle_masks").InnerText);

                settingsFileLoadedCorrectly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when loading Settings");
            }
        }

        /// <summary>
        /// Write the contents of all public fields to the XML file specified in FileNameAppSettings.
        /// In case of errors, an exception will bring up a MessageBox that an error occured. The issue might be related to disc write problems.
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlNode nodeSettings = doc.AppendChild(doc.CreateElement("settings"));
                XmlNode nodeDataPath = nodeSettings.AppendChild(doc.CreateElement("data_path"));
                nodeDataPath.AppendChild(doc.CreateElement("source_images")).AppendChild(doc.CreateTextNode(PathToSourceImages));
                nodeDataPath.AppendChild(doc.CreateElement("session_data")).AppendChild(doc.CreateTextNode(PathToSessionData));
                nodeDataPath.AppendChild(doc.CreateElement("output_dataset")).AppendChild(doc.CreateTextNode(PathToOutputDataset));
                nodeDataPath.AppendChild(doc.CreateElement("predicted_masks")).AppendChild(doc.CreateTextNode(PathToPredictedMasks));

                XmlNode nodeSubDir = nodeSettings.AppendChild(doc.CreateElement("sub_dir_level_1"));
                nodeSubDir.AppendChild(doc.CreateElement("image")).AppendChild(doc.CreateTextNode(SubDirImg));
                nodeSubDir.AppendChild(doc.CreateElement("mask")).AppendChild(doc.CreateTextNode(SubDirMask));
                nodeSubDir.AppendChild(doc.CreateElement("info")).AppendChild(doc.CreateTextNode(SubDirInfo));

                XmlNode nodeTrainVal = nodeSettings.AppendChild(doc.CreateElement("sub_dir_level_2"));
                nodeTrainVal.AppendChild(doc.CreateElement("train")).AppendChild(doc.CreateTextNode(SubDirTrain));
                nodeTrainVal.AppendChild(doc.CreateElement("val")).AppendChild(doc.CreateTextNode(SubDirVal));

                XmlNode nodePrefix = nodeSettings.AppendChild(doc.CreateElement("prefix"));
                nodePrefix.AppendChild(doc.CreateElement("image")).AppendChild(doc.CreateTextNode(PrefixImg));
                nodePrefix.AppendChild(doc.CreateElement("mask")).AppendChild(doc.CreateTextNode(PrefixMask));
                nodePrefix.AppendChild(doc.CreateElement("info")).AppendChild(doc.CreateTextNode(PrefixInfo));

                XmlNode nodeSegmClass = nodeSettings.AppendChild(doc.CreateElement("segm_class_defs"));
                foreach (SegmClassDef catDef in SegmClassDefs)
                {
                    XmlNode nodeItem = doc.CreateElement("item");
                    nodeSegmClass.AppendChild(nodeItem).AppendChild(doc.CreateTextNode(catDef.Name));
                    nodeItem.Attributes.Append(doc.CreateAttribute("id")).Value = catDef.ID.ToString();
                    nodeItem.Attributes.Append(doc.CreateAttribute("draw_order")).Value = catDef.DrawOrder.ToString();
                    nodeItem.Attributes.Append(doc.CreateAttribute("draw_color")).Value = catDef.DrawColor.ToArgb().ToString("X8");
                }

                XmlNode nodeOutputParm = nodeSettings.AppendChild(doc.CreateElement("output_parm"));
                nodeOutputParm.AppendChild(doc.CreateElement("image_width")).AppendChild(doc.CreateTextNode(ImageOutputSize.Width.ToString()));
                nodeOutputParm.AppendChild(doc.CreateElement("image_height")).AppendChild(doc.CreateTextNode(ImageOutputSize.Height.ToString()));
                nodeOutputParm.AppendChild(doc.CreateElement("train_val_ratio")).AppendChild(doc.CreateTextNode(TrainValRatio.ToString()));
                nodeOutputParm.AppendChild(doc.CreateElement("mask_draw_order_min")).AppendChild(doc.CreateTextNode(MaskDrawOrderMin.ToString()));
                nodeOutputParm.AppendChild(doc.CreateElement("mask_draw_order_max")).AppendChild(doc.CreateTextNode(MaskDrawOrderMax.ToString()));

                XmlNode nodeBlurParm = nodeOutputParm.AppendChild(doc.CreateElement("blur_parm"));
                nodeBlurParm.AppendChild(doc.CreateElement("enabled")).AppendChild(doc.CreateTextNode(BlurEnabled.ToString()));
                nodeBlurParm.AppendChild(doc.CreateElement("window_width")).AppendChild(doc.CreateTextNode(BlurWindowSize.Width.ToString()));
                nodeBlurParm.AppendChild(doc.CreateElement("window_height")).AppendChild(doc.CreateTextNode(BlurWindowSize.Height.ToString()));
                nodeBlurParm.AppendChild(doc.CreateElement("step_width")).AppendChild(doc.CreateTextNode(BlurStepSize.Width.ToString()));
                nodeBlurParm.AppendChild(doc.CreateElement("step_height")).AppendChild(doc.CreateTextNode(BlurStepSize.Height.ToString()));

                XmlNode nodePolygon = nodeBlurParm.AppendChild(doc.CreateElement("polygon"));
                for (int i = 0; i < BlurAreaPolygon.Length; i++)
                {
                    XmlNode nodePoint = doc.CreateElement("point");
                    nodePolygon.AppendChild(nodePoint);
                    nodePoint.Attributes.Append(doc.CreateAttribute("idx")).Value = i.ToString();
                    nodePoint.Attributes.Append(doc.CreateAttribute("x")).Value = BlurAreaPolygon[i].X.ToString("F7");
                    nodePoint.Attributes.Append(doc.CreateAttribute("y")).Value = BlurAreaPolygon[i].Y.ToString("F7");
                }

                nodeOutputParm.AppendChild(doc.CreateElement("tilt_with_zoom_only")).AppendChild(doc.CreateTextNode(TiltWithZoomOnly.ToString()));
                nodeOutputParm.AppendChild(doc.CreateElement("brightness_contrast_exclusive")).AppendChild(doc.CreateTextNode(BrightnessContrastExclusive.ToString()));

                XmlNode nodeZoomFactor = nodeOutputParm.AppendChild(doc.CreateElement("zoom_factor"));
                foreach (float zoomValue in ZoomFactors)
                    nodeZoomFactor.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(zoomValue.ToString("F1")));

                XmlNode nodeTiltAngles = nodeOutputParm.AppendChild(doc.CreateElement("tilt_angle"));
                foreach (float tiltValue in TiltAngles)
                    nodeTiltAngles.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(tiltValue.ToString()));

                XmlNode nodeBrightnessFactor = nodeOutputParm.AppendChild(doc.CreateElement("brightness_factor"));
                foreach (float brightnessValue in BrightnessFactors)
                    nodeBrightnessFactor.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(brightnessValue.ToString("F1")));

                XmlNode nodeContrastEnhancement = nodeOutputParm.AppendChild(doc.CreateElement("contrast_enhancement"));
                foreach (float contrastValue in ContrastEnhancements)
                    nodeContrastEnhancement.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(contrastValue.ToString("F1")));

                XmlNode nodeNoiseAdder = nodeOutputParm.AppendChild(doc.CreateElement("noise_adder"));
                foreach (int noiseValue in NoiseAdders)
                    nodeNoiseAdder.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(noiseValue.ToString()));

                XmlNode nodeGuiParm = nodeSettings.AppendChild(doc.CreateElement("gui_parm"));
                nodeGuiParm.AppendChild(doc.CreateElement("polygon_line_color")).AppendChild(doc.CreateTextNode(PolygonLineColor.ToArgb().ToString("X8")));
                nodeGuiParm.AppendChild(doc.CreateElement("polygon_point_color")).AppendChild(doc.CreateTextNode(PolygonPointColor.ToArgb().ToString("X8")));
                nodeGuiParm.AppendChild(doc.CreateElement("polygon_point_size")).AppendChild(doc.CreateTextNode(PolygonPointSize.ToString("F1")));
                nodeGuiParm.AppendChild(doc.CreateElement("draw_mask_transparency")).AppendChild(doc.CreateTextNode(DrawMaskTransparency.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("scroll_zone_size")).AppendChild(doc.CreateTextNode(ScrollZoneSize.ToString("F1")));
                nodeGuiParm.AppendChild(doc.CreateElement("scroll_move_factor")).AppendChild(doc.CreateTextNode(ScrollMoveFactor.ToString("F2")));
                nodeGuiParm.AppendChild(doc.CreateElement("scroll_start_min_count")).AppendChild(doc.CreateTextNode(ScrollStartMinCount.ToString()));

                XmlNode nodeGuiSettings = nodeSettings.AppendChild(doc.CreateElement("gui_settings"));
                nodeGuiSettings.AppendChild(doc.CreateElement("auto_load_predicted_mask")).AppendChild(doc.CreateTextNode(AutoLoadPredictedMask.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("left_draw_active_polygon_only")).AppendChild(doc.CreateTextNode(LeftDrawActivePolygonOnly.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("left_image_target_size")).AppendChild(doc.CreateTextNode(LeftImageTargetSize.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("sync_left_right")).AppendChild(doc.CreateTextNode(SyncLeftRight.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("edit_polygon_auto_update_mask")).AppendChild(doc.CreateTextNode(EditPolygonAutoUpdateMask.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("right_show_selected_only")).AppendChild(doc.CreateTextNode(RightShowSelectedOnly.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("right_show_overlap")).AppendChild(doc.CreateTextNode(RightShowOverlap.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("show_predicted_mask")).AppendChild(doc.CreateTextNode(ShowPredictionMask.ToString()));
                nodeGuiSettings.AppendChild(doc.CreateElement("auto_toggle_masks")).AppendChild(doc.CreateTextNode(AutoToggleMasks.ToString()));

                doc.Save(FileNameAppSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when saving Settings");
            }
        }
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the full path to the image info session XML files.
        /// </summary>
        public string PathToImageInfoFiles
        {
            get { return PathToSessionData + SubDirInfo; }
        }

        /// <summary>
        /// Get the status of the existance of the AppSettings XML file.
        /// </summary>
        public bool SettingsFileExisted
        {
            get { return settingsFileExisted; }
        }

        /// <summary>
        /// Gets the status of successfully loading of the XML AppSettings file. 
        /// </summary>
        public bool SettingsFileLoadedCorrectly
        {
            get { return settingsFileLoadedCorrectly; }
        }
        #endregion Public Properties

    }
}
