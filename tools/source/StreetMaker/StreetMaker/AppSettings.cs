// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
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
using System.Drawing.Printing;

namespace StreetMaker
{

    /// <summary>
    /// Storage class for all customizable parameter for drawing and GUI. All public fields can be read from an XML file at startup and written to the XML file at any time.
    /// </summary>
    public class AppSettings
    {
        #region Public Constants
        /// <summary>Identifer string that is stored into the XML file.</summary>
        public const string XML_FILE_ID_STR = "StreetMaker AppSettings";
        /// <summary>Maximum allowed distance tolerance to accept the connection as good. No issue is flagged.</summary>
        public const double MAX_CONN_DIST_TOL = 0.01;
        /// <summary>Maximum allowed connector angle difference tolerance to accept the connection as good. No issue is flagged.</summary>
        public const double MAX_CONN_ANGLE_TOL = 0.1 * Math.PI / 180;
        /// <summary>Maximum value difference through conversions and rounding not flagged as value change. Used in property pages.</summary>
        public const double MAX_CHANGE_TOL = 0.001;

        /// <summary>Conversion factor from meter to millimeter. 1m=1000mm</summary>
        public const double METER_TO_MM = 1000f;
        /// <summary>Conversion factor from inch to millimeter. 1inch=25.4mm</summary>
        public const double INCH_TO_MM = 25.4f;
        /// <summary>Conversion factor from feet to inch. 1foot=12inch</summary>
        public const double FEET_TO_INCH = 12f;

        /// <summary>A3 paper size landscape width in mm</summary>
        public const double PAPER_SIZE_A3_WIDTH = 420;
        /// <summary>A3 paper size landscape height in mm</summary>
        public const double PAPER_SIZE_A3_HEIGHT = 297;

        /// <summary>Ledger paper size landscape width in mm</summary>
        public const double PAPER_SIZE_11x17_WIDTH = 17 * INCH_TO_MM;
        /// <summary>Ledger paper size landscape height in mm</summary>
        public const double PAPER_SIZE_11x17_HEIGHT = 11 * INCH_TO_MM;

        /// <summary>Minimum limit for BrightnessFactors values.</summary>
        public const float SIDE_STEPS_FACTOR_MIN = -0.5f;
        /// <summary>Maximum limit for BrightnessFactors values.</summary>
        public const float SIDE_STEPS_FACTOR_MAX = 0.5f;

        /// <summary>Minimum limit for BrightnessFactors values.</summary>
        public const float ANGLE_STEP_MIN = -15;
        /// <summary>Maximum limit for BrightnessFactors values.</summary>
        public const float ANGLE_STEP_MAX = 15;

        /// <summary>Minimum limit for BrightnessFactors values.</summary>
        public const float BRIGHTNESS_FACTOR_MIN = 0.1f;
        /// <summary>Maximum limit for BrightnessFactors values.</summary>
        public const float BRIGHTNESS_FACTOR_MAX = 2.0f;

        /// <summary>Minimum limit for ColorFactors values.</summary>
        public const float COLOR_FACTOR_MIN = 0.5f;
        /// <summary>Maximum limit for ColorFactors values.</summary>
        public const float COLOR_FACTOR_MAX = 2.0f;

        /// <summary>Minimum limit for NoiseAdders values.</summary>
        public const int NOISE_LEVEL_MIN = 0;
        /// <summary>Maximum limit for NoiseAdders values.</summary>
        public const int NOISE_LEVEL_MAX = 25;

        #endregion Public Constants

        #region Public Fields
        /// <summary>Full path to the base directory for storing street maps and datasets.</summary>
        public string PathToDataStorage = Application.StartupPath + @"\Data\";
        /// <summary>Sub directory to store all street maps..</summary>
        public string SubDirStreetmaps = @"Maps\";

        #region StreetMap
        /// <summary>Default drawing area width in mm. Creating a new drawing area uses this default value.</summary>
        public double DefaultDrawingWidth = 16 * FEET_TO_INCH * INCH_TO_MM;
        /// <summary>Default drawing area height in mm. Creating a new drawing area uses this default value.</summary>
        public double DefaultDrawingHeight = 10 * FEET_TO_INCH * INCH_TO_MM;

        #region Dimensions
        /// <summary>Standard width of all lanes in mm. Lines are normally drawn inside this lane width, except shared lines between 2 lanes. The effective width between depends on the type of the lines.</summary>
        public double LaneWidth = 120;
        /// <summary>Standard width of all lane limiting lines left and right in mm.</summary>
        public double LineWidth = 10;
        /// <summary>Space between double lines in mm.</summary>
        public double LineSpace = 5;
        /// <summary>Width of stop lines in mm.</summary>
        public double StopLineWidth = 15;
        /// <summary>Offset of the stop lines from the crossing lane limits in mm.</summary>
        public double StopLineOffset = 25;
        /// <summary>Width of a crosswalk in mm.</summary>
        public double CrosswalkWidth = 50;
        /// <summary>Width of the lines limiting a crosswalk in mm.</summary>
        public double CrosswalkLineWidth = 8;
        /// <summary>Width of the zebra strips of a crosswalk in mm.</summary>
        public double CrosswalkStripeWidth = 10;
        /// <summary>Border offset of a crosswalk in mm. This provides a gap between the crosswalk and a stop line or other elements.</summary>
        public double CrosswalkBorder = 8;
        /// <summary>Standard dash length in mm of dashed lines between lanes.</summary>
        public double DashLength = 40;
        /// <summary>Minimum inner radius of a lane in mm.</summary>
        public double MinInnerRadius = 120;
        /// <summary>Intersection inner radius in mm.</summary>
        public double CornerRadius = 100;
        /// <summary>Minimum curve angle of a curved street in degrees.</summary>
        public double MinCurveAngle = 45.0 / 4;
        /// <summary>Maximum curve angle of a curved street in degrees.</summary>
        public double MaxCurveAngle = 270;
        /// <summary>Curve angle step in degrees used in each hotkey increase or decrease step.</summary>
        public double AngleStep = 45.0 / 4;
        /// <summary>Step used in each hotkey increase or decrease step for straight or s-shape elements.</summary>
        public double LengthStep = 50;
        /// <summary>Minimum straight street length in mm.</summary>
        public double MinStraightLength = 50;
        /// <summary>Maximum width of arrow overlays in mm.</summary>
        public double MaxArrowOverlayWidth = 65;
        /// <summary>Maximum length of arrow overlays in mm.</summary>
        public double MaxArrowOverlayLength = 100;
        /// <summary>Minimum position step of arrow overlays in mm.</summary>
        public double MinArrowOverlayStep = 20;
        /// <summary>Maximum number of lanes in any street element left or right side.</summary>
        public int MaxLaneCountLeftRight = 3;
        /// <summary>Maximum number of center lanes in any street element.</summary>
        public int MaxLaneCountCenter = 1;
        /// <summary>Recess of crosswalks or stop lines from the inner area limits.</summary>
        public int IntersectionRecess = 5;
        /// <summary>The font size of Stop and Yield markings in the intersections.</summary>
        public double IntersectionFontSize = 30;
        /// <summary>The font size of P markings in the overlay.</summary>
        public double OverlayFontSize = 60;
        #endregion Dimensions

        #region Colors
        /// <summary>Color of the lane pavement.</summary>
        public Color LaneColor = Color.Black;
        /// <summary>Color of the white lines.</summary>
        public Color LineColorWhite = Color.White;
        /// <summary>Color of the yellow lines.</summary>
        public Color LineColorYellow = Color.Gold;
        /// <summary>Color of the stop line.</summary>
        public Color StopLineColor = Color.White;
        /// <summary>Color of the arrow overlays.</summary>
        public Color ArrowOverlayColor = Color.White;
        /// <summary>Color of the background outside of the streets.</summary>
        public Color BackgroundColor = Color.FromArgb(0xFF,0x79,0x69,0x59);
        #endregion Colors

        #endregion StreetMap


        #region Print Setup
        /// <summary>Page settings to be used in street map printing.</summary>
        public PageSettings PrintPageSettings = new PageSettings();
 
        #endregion Print Setup


        #region GUI
        #region Parameter
        /// <summary>Measurement unit to display all values in settings dialogs.</summary>
        public MeasurementUnit DisplayMeasurementUnit = MeasurementUnit.Millimeter;
        /// <summary>Default length of straight street elements when created from menu.</summary>
        public double DefaultStraightLength = 300;
        /// <summary>Default length of the junction streets of intersections when created from menu.</summary>
        public double DefaultJunctionLength = 100;
        /// <summary>Default curve angle of a curved street when created from menu.</summary>
        public double DefaultCurveAngle = 90;
        /// <summary>Default curve angle of an on-ramp or off-ramp when created from menu.</summary>
        public double DefaultRampCurveAngle = 45;
        /// <summary>Default inner radius of an on-ramp or off-ramp when created from menu.</summary>
        public double DefaultRampRadius = 300;
        /// <summary>Default inner radius of roundabout when created from menu.</summary>
        public double DefaultRoundaboutRadius = 150;

        /// <summary>Minimum distance from a lane connector center point to catch it with the mouse for moving or sizing.</summary>
        public double PointCatchDistance = 30;
        /// <summary>Minimum distance between 2 opposite connectors to draw a line for possible connection.</summary>
        public double ConnectionDrawDistance = 300;
        /// <summary>Minimum distance between 2 opposite connectors to snap them togeter.</summary>
        public double ConnectionSnapDistance = 90;
        /// <summary>Street and lane outline line width activated when hovering over it with mouse.</summary>
        public double StreetOutlineLineWidth = 3;
        /// <summary>Overlay outline line width activated when hovering over it with mouse.</summary>
        public double OverlayOutlineLineWidth = 2;
        #endregion Parameter

        #region Colors
        /// <summary>Street and lane outline line color activated when hovering over it with mouse.</summary>
        public Color StreetOutlineColor = Color.LightGray;
        /// <summary>Overlay outline line color activated when hovering over it with mouse.</summary>
        public Color OverlayOutlineColor = Color.DarkGray;
        /// <summary>Color of the in-bound connector marker activated when hovering over it with mouse.</summary>
        public Color ConnectorInColor = Color.Green;
        /// <summary>Color of the out-bound connector marker activated when hovering over it with mouse.</summary>
        public Color ConnectorOutColor = Color.Red;
        /// <summary>Color of the none-directional connector marker activated when hovering over it with mouse.</summary>
        public Color ConnectorNoDirColor = Color.DarkGray;

        /// <summary>Color of the line of a possible connection when the distance is less than ConnectionDrawDistance.</summary>
        public Color ConnectionDrawColor = Color.DarkGray;
        /// <summary>Color of the line of a possible connection when the distance is less than ConnectionSnapDistance.</summary>
        public Color ConnectionSnapColor = Color.LightGreen;
        #endregion Colors

        #region Hotkeys
        /// <summary>Hotkey definition to delete a street element or overlay.</summary>
        public Keys HotkeyDelete = Keys.Delete;
        /// <summary>Hotkey definition to abort adding a new street element or overlay.</summary>
        public Keys HotkeyAbort = Keys.Escape;
        /// <summary>Hotkey definition to disconnect a street element from all others to move it around.</summary>
        public Keys HotkeyDisconnect = Keys.D;
        /// <summary>Hotkey definition to rotate left a street element or overlay by AngleStep.</summary>
        public Keys HotkeyRotateLeft = Keys.E;
        /// <summary>Hotkey definition to rotate right a street element or overlay by AngleStep.</summary>
        public Keys HotkeyRotateRight = Keys.R;
        /// <summary>Hotkey definition to increase the size of a street element by one step.</summary>
        public Keys HotkeySizeIncrease = Keys.C;
        /// <summary>Hotkey definition to decrease the size of a street element by one step.</summary>
        public Keys HotkeySizeDecrease = Keys.X;
        /// <summary>Hotkey definition to enable basic sizing mode for dragging one connector with the mouse. Basic size mode only changes one parameter at a time like length and angle.</summary>
        public Keys HotkeySizeModeBase = Keys.S;
        /// <summary>Hotkey definition to enable extended sizing mode for dragging one connector with the mouse. Extended size mode changes 2 parameter at a time where possible, for instance radius and angle.</summary>
        public Keys HotkeySizeModeExt = Keys.A;
        /// <summary>Hotkey definition block the auto rotation function when moving a street element over the map.</summary>
        public Keys HotkeyBlockAutoRotate = Keys.Z;
        /// <summary>Hotkey definition to open the property page for the highlighted street element.</summary>
        public Keys HotkeyPropertyPage = Keys.P;
        #endregion Hotkeys
        #endregion GUI

        #region Data Generation
        #region Virtual Camera
        /// <summary>Theoretical Horizontal Field of View of the camera lens in Degrees without lens distortion.</summary>
        public double CameraHFOV = 120;
        /// <summary>Horizontal to vertical ratio of the camera image after distortion.</summary>
        public double CameraImageRatio = 1.333;
        /// <summary>Optical distortion coefficient 1 of the camera lens. Estimated value for 145 degree lens.</summary>
        public double CameraLensDistortion1 = 0.05;
        /// <summary>Optical distortion coefficient 2 of the camera lens. Estimated value for 145 degree lens.</summary>
        public double CameraLensDistortion2 = 0.25;
        /// <summary>Color correction factor of the camera for the red channel. Estimated value for 145 degree camera.</summary>
        public double CameraColorCorrRed = 1.18*0.8;
        /// <summary>Color correction factor of the camera for the green channel. Estimated value for 145 degree camera.</summary>
        public double CameraColorCorrGreen = 1.0*0.8;
        /// <summary>Color correction factor of the camera for the blue channel. Estimated value for 145 degree camera.</summary>
        public double CameraColorCorrBlue = 1.05*0.8;
        /// <summary>Height of the camera above ground.</summary>
        public double CameraHeight = 110;
        /// <summary>Angle of the optical axis of the camera.</summary>
        public double CameraAxisAngle = 47;
        /// <summary>Width of the output image of the virtual camera.</summary>
        public int CameraOutputWidth = 224;
        /// <summary>Height of the output image of the virtual camera.</summary>
        public int CameraOutputHeight = 224;
        /// <summary>Oversampling of the virtual camera of the original image before downsampling to output width.</summary>
        public int CameraOversampling = 3;
        /// <summary>Range in front of a view to include street elements to mark lanes with class codes or colors.</summary>
        public double MarkLaneMaxDistFront = 1500;
        /// <summary>Range to the side of a view to include street elements to mark lanes with class codes or colors.</summary>
        public double MarkLaneMaxDistSide = 1000;
        /// <summary>Angle to the side of a view to include street elements to mark lanes with class codes or colors.</summary>
        public double MarkLaneMaxDistSideAngle = 45;
        /// <summary>Maximum distance from camera view point to mark all details.</summary>
        public double MarkMaxDetailDistance = 1400;
        /// <summary>If true, overlays and intersection items are drawn in wrong direction.</summary>
        public bool DrawWrongDirItems = false;
        /// <summary>If true, class images are generated to show the classes in different colors.</summary>
        public bool IncludeClassImages = false;

        #endregion Virtual Camera

        #region Augmentation
        /// <summary>Ratio between training and validation output. 50 for instance means that about every 50th image,mask and info set will be placed into the validation folders instead of training folders.</summary>
        public int TrainValRatio = 50;
        /// <summary>If true, only center view sets will be placed into validation folders.</summary>
        public bool ValidateCenterViewsOnly = false;
        /// <summary>Ratio between Train/Val and Test output. 100 for instance means that about every 100th image,mask and info set will be copied into the test folder.</summary>
        public int TestOutRatio = 100;
        /// <summary>If true, only center view sets will be placed into test folders.</summary>
        public bool TestCenterViewsOnly = false;

        /// <summary>Step size for new images to be created along each lane.</summary>
        public double ImageStepSize = 100;

        /// <summary>If true, brightness calculation results will be offset to center the min/max range.</summary>
        public bool CenterBrightnessResults = true;

        /// <summary>Steps left and right from the lane center to create additional views off center.</summary>
        public float[] SideSteps = { -25, 0, 25 };
        /// <summary>Angle variations turning left and right from lane center view to create additional views.</summary>
        public float[] AngleSteps = { -5, 0, 5 };
        /// <summary>Different brightness variations for more augmentation</summary>
        public float[] BrightnessFactors = { 0.6f, 1.0f };
        /// <summary>Color variations to apply to each of the three basic colors RGB sequentially.</summary>
        public float[] ColorFactors = { 0.8f, 1.0f, 1.2f };
        /// <summary>Adding noise in different levels at the end.</summary>
        public int[] NoiseLevels = { 8, 8 };
        #endregion Augmentation

        #region Folders and File Prefixes
        /// <summary>Path to the folder to store all output dataset files, like images, mask and xml files.</summary>
        public string SubDirDataSet = @"DataSet\";

        /// <summary>Sub directory name under SubDirDataSet\ for augmented JPG images.</summary>
        public string SubDirImg = @"Img\";
        /// <summary>Sub directory name under SubDirDataSet\ for augmented PNG masks.</summary>
        public string SubDirMask = @"Mask\";
        /// <summary>Sub directory name under SubDirDataSet\ for augmented class image files.</summary>
        public string SubDirClassImg = @"ClassImg\";
        /// <summary>Sub directory name under SubDirDataSet\ for prediction results from the model.</summary>
        public string SubDirPred = @"Pred\";

        /// <summary>Sub directory name under SubDirDataSet\XXX\ for augmented files for training.</summary>
        public string SubDirTrain = @"Train\";
        /// <summary>Sub directory name under SubDirDataSet\XXX\ for augmented files for validation.</summary>
        public string SubDirVal = @"Val\";
        /// <summary>Sub directory name under SubDirDataSet\XXX\ for augmented files for test outputs.</summary>
        public string SubDirTest = @"Test\";

        /// <summary>File name prefix for JPG image files.</summary>
        public string PrefixImg = "Img_";
        /// <summary>File name prefix for PNG mask files.</summary>
        public string PrefixMask = "Mask_";
        /// <summary>File name prefix for class image files.</summary>
        public string PrefixClassImg = "Class_";
        /// <summary>File name prefix for prediction image files.</summary>
        public string PrefixPred = "Pred_";

        /// <summary>Name of the file to store the current active class definitions for python.</summary>
        public string ClassTextFileName = "JetCar_Classes.txt";
        /// <summary>Name of the file to store the current active class color map.</summary>
        public string ColorMapFileName = "JetCar_ColorMap.csv";

        #endregion Folders and File Prefixes
        #endregion Data Generation

        #endregion Public Fields

        #region Private Fields
        /// <summary>Full file name of the application settings XML file.</summary>
        private string fileName;
        /// <summary>True, if the file existed.</summary>
        private bool fileExisted;
        /// <summary>True, if the file could be loaded correctly.</summary>
        private bool fileLoadedCorrectly;
#endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the AppSettings class just with the defaults.
        /// </summary>
        public AppSettings()
        {
            fileLoadedCorrectly = false;
            fileExisted = false;
        }


        /// <summary>
        /// Creates an instance of the AppSettings class and reads in the XML file to all public fields if the file exists. If the file does not exist, if will be created and all defaulted fields will be written to it.
        /// </summary>
        /// <param name="FileName">Full file name of the application settings XML file.</param>
        public AppSettings(string FileName)
        {
            fileName = FileName;
            //SaveSettings();
            fileLoadedCorrectly = false;
            fileExisted = File.Exists(fileName);
            if (fileExisted)
                LoadSettings();
            else
                SaveSettings();
        }
        #endregion Constructor

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
        /// Conversion method from the internally used millimeter value to the display value in the given measurement unit. 
        /// </summary>
        /// <param name="MillimeterValue">Width or height in mm.</param>
        /// <param name="MeasurementUnit">Target unit to convert to.</param>
        /// <returns>Conversion result in the target unit.</returns>
        public static double ToUnit(double MillimeterValue, MeasurementUnit MeasurementUnit)
        {
            switch (MeasurementUnit)
            {
                case MeasurementUnit.Millimeter:
                    return MillimeterValue;

                case MeasurementUnit.Centimeter:
                    return MillimeterValue / 10;

                case MeasurementUnit.Meter:
                    return MillimeterValue / 1000;

                case MeasurementUnit.Inch:
                    return MillimeterValue / INCH_TO_MM;

                case MeasurementUnit.Feet:
                    return MillimeterValue / (FEET_TO_INCH * INCH_TO_MM);
            }
            return 0;
        }

        /// <summary>
        /// Conversion from a millimeter value to hundreds of an inch as double;
        /// </summary>
        /// <param name="MillimeterValue">A value in mm.</param>
        /// <returns>Millimeter value converted to hundreds of an inch.</returns>
        public static double ToHundredsOfInch(double MillimeterValue)
        {
            return (MillimeterValue / INCH_TO_MM) * 100;
        }

        /// <summary>
        /// Conversion from a millimeter value to hundreds of an inch as integer;
        /// </summary>
        /// <param name="MillimeterValue">A value in mm.</param>
        /// <returns>Millimeter value converted to hundreds of an inch.</returns>
        public static int ToHundredsOfInchInt(double MillimeterValue)
        {
            return (int)Math.Round(ToHundredsOfInch(MillimeterValue));
        }

        /// <summary>
        /// Conversion from hundreds of an inch to millimeter;
        /// </summary>
        /// <param name="HundredsOfInchValue">A value in hundreds of an inch</param>
        /// <returns>Input value converted to millimeter.</returns>
        public static double FromHundredsOfInch(double HundredsOfInchValue)
        {
            return (HundredsOfInchValue / 100 * INCH_TO_MM);
        }


        /// <summary>
        /// Conversion method from the display value in the target unit into the internally used miliimeter value.
        /// </summary>
        /// <param name="UnitValue">>Width or height in display value.</param>
        /// <param name="MeasurementUnit">Target unit to convert from.</param>
        /// <returns>Double representation in millimeter.</returns>
        public static double FromUnit(double UnitValue, MeasurementUnit MeasurementUnit)
        {
            switch (MeasurementUnit)
            {
                case MeasurementUnit.Millimeter:
                    return UnitValue;

                case MeasurementUnit.Centimeter:
                    return UnitValue * 10;

                case MeasurementUnit.Meter:
                    return UnitValue * 1000;

                case MeasurementUnit.Inch:
                    return UnitValue * AppSettings.INCH_TO_MM;

                case MeasurementUnit.Feet:
                    return UnitValue * FEET_TO_INCH * INCH_TO_MM;
            }
            return 0;
        }

        /// <summary>
        /// Reads the XML file given via the constructor and loads all public fields from that file. The property FileLoadedCorrectly will be set accordingly.
        /// </summary>
        public void LoadSettings()
        {
            fileExisted = File.Exists(fileName);
            if (fileExisted == false)
                return;

            fileLoadedCorrectly = false;
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(fileName);

                XmlNode nodeSettings = doc.SelectSingleNode("settings");
                string id = nodeSettings.SelectSingleNode("identifier").InnerText;
                if (id != XML_FILE_ID_STR)
                    throw new Exception("XML file identifier error!");

                PathToDataStorage = nodeSettings.SelectSingleNode("path_to_data_storage").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirStreetmaps = nodeSettings.SelectSingleNode("sub_dir_street_maps").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodeStreetMap = nodeSettings.SelectSingleNode("street_map");
                XmlNode nodeStreetDimensions = nodeStreetMap.SelectSingleNode("dimensions");
                DefaultDrawingWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("drawing_width").InnerText);
                DefaultDrawingHeight = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("drawing_height").InnerText);

                LaneWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("lane_width").InnerText);
                LineWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("line_width").InnerText);
                LineSpace = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("line_space").InnerText);
                StopLineWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("stop_line_width").InnerText);
                StopLineOffset = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("stop_line_offset").InnerText);

                CrosswalkWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("crosswalk_width").InnerText);
                CrosswalkLineWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("crosswalk_line_width").InnerText);
                CrosswalkStripeWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("crosswalk_stripe_width").InnerText);
                CrosswalkBorder = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("crosswalk_border").InnerText);

                DashLength = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("dash_length").InnerText);
                MinStraightLength = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("min_straight_length").InnerText);
                MinInnerRadius = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("min_inner_radius").InnerText);
                CornerRadius = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("corner_radius").InnerText);
                MinCurveAngle = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("min_curve_angle").InnerText);
                MaxCurveAngle = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("max_curve_angle").InnerText);
                AngleStep = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("angle_step").InnerText);
                LengthStep = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("length_step").InnerText);
                MaxArrowOverlayWidth = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("max_arrow_overlay_width").InnerText);
                MaxArrowOverlayLength = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("max_arrow_overlay_length").InnerText);
                MinArrowOverlayStep = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("min_arrow_overlay_step").InnerText);
                MaxLaneCountLeftRight = Convert.ToInt32(nodeStreetDimensions.SelectSingleNode("max_lane_count_left_right").InnerText);
                MaxLaneCountCenter = Convert.ToInt32(nodeStreetDimensions.SelectSingleNode("max_lane_count_center").InnerText);
                IntersectionRecess = Convert.ToInt32(nodeStreetDimensions.SelectSingleNode("intersection_recess").InnerText);
                IntersectionFontSize = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("intersection_font_size").InnerText);
                OverlayFontSize = Convert.ToDouble(nodeStreetDimensions.SelectSingleNode("overlay_font_size").InnerText);

                XmlNode nodeStreetColors = nodeStreetMap.SelectSingleNode("colors");
                LaneColor = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("lane_color").InnerText, 16));
                LineColorWhite = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("line_color_white").InnerText, 16));
                LineColorYellow = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("line_color_yellow").InnerText, 16));
                StopLineColor = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("stop_line_color").InnerText, 16));
                ArrowOverlayColor = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("arrow_overlay_color").InnerText, 16));
                BackgroundColor = Color.FromArgb(Convert.ToInt32(nodeStreetColors.SelectSingleNode("background_color").InnerText, 16));

                XmlNode nodePrintSettings = nodeSettings.SelectSingleNode("print_settings");
                XmlNode nodePrintPaperSource = nodePrintSettings.SelectSingleNode("paper_source");
                PrintPageSettings.PaperSource.RawKind = Convert.ToInt32(nodePrintPaperSource.SelectSingleNode("raw_kind").InnerText);
                PrintPageSettings.PaperSource.SourceName = nodePrintPaperSource.SelectSingleNode("name").InnerText;

                XmlNode nodePrintPaperSize = nodePrintSettings.SelectSingleNode("paper_size");
                int rawKind = Convert.ToInt32(nodePrintPaperSize.SelectSingleNode("raw_kind").InnerText);
                string paperName = nodePrintPaperSize.SelectSingleNode("name").InnerText;
                int paperWidth = Convert.ToInt32(nodePrintPaperSize.SelectSingleNode("width").InnerText);
                int paperHeight = Convert.ToInt32(nodePrintPaperSize.SelectSingleNode("height").InnerText);
                PrintPageSettings.PaperSize = new PaperSize(paperName, paperWidth, paperHeight);
                PrintPageSettings.PaperSize.RawKind = rawKind;

                XmlNode nodePrintMargins = nodePrintSettings.SelectSingleNode("margins");
                PrintPageSettings.Margins.Left = Convert.ToInt32(nodePrintMargins.SelectSingleNode("left").InnerText);
                PrintPageSettings.Margins.Top = Convert.ToInt32(nodePrintMargins.SelectSingleNode("top").InnerText);
                PrintPageSettings.Margins.Right = Convert.ToInt32(nodePrintMargins.SelectSingleNode("right").InnerText);
                PrintPageSettings.Margins.Bottom = Convert.ToInt32(nodePrintMargins.SelectSingleNode("bottom").InnerText);

                PrintPageSettings.Landscape = Convert.ToBoolean(nodePrintSettings.SelectSingleNode("landscape").InnerText);
                PrintPageSettings.Color = Convert.ToBoolean(nodePrintSettings.SelectSingleNode("color").InnerText);

                XmlNode nodeGui = nodeSettings.SelectSingleNode("gui");
                XmlNode nodeGuiParm = nodeGui.SelectSingleNode("parameter");
                DisplayMeasurementUnit = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), nodeGuiParm.SelectSingleNode("display_measurement_unit").InnerText);
                DefaultStraightLength = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_straight_length").InnerText);
                DefaultJunctionLength = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_junction_length").InnerText);
                DefaultCurveAngle = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_curve_angle").InnerText);
                DefaultRampCurveAngle = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_ramp_curve_angle").InnerText);
                DefaultRampRadius = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_ramp_radius").InnerText);
                DefaultRoundaboutRadius = Convert.ToDouble(nodeGuiParm.SelectSingleNode("default_roundabout_radius").InnerText);

                PointCatchDistance = Convert.ToDouble(nodeGuiParm.SelectSingleNode("point_catch_distance").InnerText);
                ConnectionDrawDistance = Convert.ToDouble(nodeGuiParm.SelectSingleNode("connection_draw_distance").InnerText);
                ConnectionSnapDistance = Convert.ToDouble(nodeGuiParm.SelectSingleNode("connection_snap_distance").InnerText);
                StreetOutlineLineWidth = Convert.ToDouble(nodeGuiParm.SelectSingleNode("street_outline_line_width").InnerText);
                OverlayOutlineLineWidth = Convert.ToDouble(nodeGuiParm.SelectSingleNode("overlay_outline_line_width").InnerText);

                XmlNode nodeGuiColors = nodeGui.SelectSingleNode("colors");
                StreetOutlineColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("street_outline_color").InnerText, 16));
                OverlayOutlineColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("overlay_outline_color").InnerText, 16));
                ConnectorInColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("connector_in_color").InnerText, 16));
                ConnectorOutColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("connector_out_color").InnerText, 16));
                ConnectorNoDirColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("connector_no_dir_color").InnerText, 16));
                ConnectionDrawColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("connection_draw_color").InnerText, 16));
                ConnectionSnapColor = Color.FromArgb(Convert.ToInt32(nodeGuiColors.SelectSingleNode("connection_snap_color").InnerText, 16));

                XmlNode nodeHotkeys = nodeGui.SelectSingleNode("hotkeys");
                HotkeyDelete = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("delete").InnerText);
                HotkeyAbort = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("abort").InnerText);
                HotkeyDisconnect = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("disconnect").InnerText);
                HotkeyRotateLeft = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("rotate_left").InnerText);
                HotkeyRotateRight = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("rotate_right").InnerText);
                HotkeySizeIncrease = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("size_increase").InnerText);
                HotkeySizeDecrease = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("size_decrease").InnerText);
                HotkeySizeModeBase = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("size_mode_base").InnerText);
                HotkeySizeModeExt = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("size_mode_ext").InnerText);
                HotkeyBlockAutoRotate = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("block_auto_rotate").InnerText);
                HotkeyPropertyPage = (Keys)Enum.Parse(typeof(Keys), nodeHotkeys.SelectSingleNode("property_page").InnerText);

                XmlNode nodeDataGen = nodeSettings.SelectSingleNode("data_generation");
                XmlNode nodeVirtualCam = nodeDataGen.SelectSingleNode("virtual_camera");

                CameraHFOV = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_hfov").InnerText);
                CameraImageRatio = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_image_ratio").InnerText);
                CameraLensDistortion1 = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_lens_distortion1").InnerText);
                CameraLensDistortion2 = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_lens_distortion2").InnerText);
                CameraColorCorrRed = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_color_correction_red").InnerText);
                CameraColorCorrGreen = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_color_correction_green").InnerText);
                CameraColorCorrBlue = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_color_correction_blue").InnerText);
                CameraHeight = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_height").InnerText);
                CameraAxisAngle = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("camera_axis_angle").InnerText);
                CameraOutputWidth = Convert.ToInt32(nodeVirtualCam.SelectSingleNode("camera_output_width").InnerText);
                CameraOutputHeight = Convert.ToInt32(nodeVirtualCam.SelectSingleNode("camera_output_height").InnerText);
                CameraOversampling = Convert.ToInt32(nodeVirtualCam.SelectSingleNode("camera_oversampling").InnerText);

                MarkLaneMaxDistFront = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("mark_lane_max_dist_front").InnerText);
                MarkLaneMaxDistSide = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("mark_lane_max_dist_side").InnerText);
                MarkLaneMaxDistSideAngle = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("mark_lane_max_dist_side_angle").InnerText);
                MarkMaxDetailDistance = Convert.ToDouble(nodeVirtualCam.SelectSingleNode("mark_max_detail_distance").InnerText);
                DrawWrongDirItems = Convert.ToBoolean(nodeVirtualCam.SelectSingleNode("draw_wrong_dir_items").InnerText);
                IncludeClassImages = Convert.ToBoolean(nodeVirtualCam.SelectSingleNode("include_class_images").InnerText);

                XmlNode nodeAugmentation = nodeDataGen.SelectSingleNode("augmentation");

                TrainValRatio = Convert.ToInt32(nodeAugmentation.SelectSingleNode("train_val_ratio").InnerText);
                ValidateCenterViewsOnly = Convert.ToBoolean(nodeAugmentation.SelectSingleNode("validate_center_views_only").InnerText);
                TestOutRatio = Convert.ToInt32(nodeAugmentation.SelectSingleNode("test_out_ratio").InnerText);
                TestCenterViewsOnly = Convert.ToBoolean(nodeAugmentation.SelectSingleNode("test_center_views_only").InnerText);
                CenterBrightnessResults = Convert.ToBoolean(nodeAugmentation.SelectSingleNode("center_brightness_results").InnerText);

                XmlNode nodeSideSteps = nodeAugmentation.SelectSingleNode("side_steps");
                XmlNodeList sideStepsItems = nodeSideSteps.SelectNodes("item");
                SideSteps = new float[sideStepsItems.Count];
                for (int i = 0; i < sideStepsItems.Count; i++)
                    SideSteps[i] = Convert.ToSingle(sideStepsItems[i].InnerText);

                XmlNode nodeAngleSteps = nodeAugmentation.SelectSingleNode("angle_steps");
                XmlNodeList angleStepsItems = nodeAngleSteps.SelectNodes("item");
                AngleSteps = new float[angleStepsItems.Count];
                for (int i = 0; i < angleStepsItems.Count; i++)
                    AngleSteps[i] = Convert.ToSingle(angleStepsItems[i].InnerText);

                XmlNode nodeBrightnessFactors = nodeAugmentation.SelectSingleNode("brightness_factors");
                XmlNodeList brightnessItems = nodeBrightnessFactors.SelectNodes("item");
                BrightnessFactors = new float[brightnessItems.Count];
                for (int i = 0; i < brightnessItems.Count; i++)
                    BrightnessFactors[i] = Convert.ToSingle(brightnessItems[i].InnerText);

                XmlNode nodeColorFactors = nodeAugmentation.SelectSingleNode("color_factors");
                XmlNodeList colorFactorsItems = nodeColorFactors.SelectNodes("item");
                ColorFactors = new float[colorFactorsItems.Count];
                for (int i = 0; i < colorFactorsItems.Count; i++)
                    ColorFactors[i] = Convert.ToSingle(colorFactorsItems[i].InnerText);

                XmlNode nodeNoiseLevels = nodeAugmentation.SelectSingleNode("noise_levels");
                XmlNodeList noiseLevelsItems = nodeNoiseLevels.SelectNodes("item");
                NoiseLevels = new int[noiseLevelsItems.Count];
                for (int i = 0; i < noiseLevelsItems.Count; i++)
                    NoiseLevels[i] = Convert.ToInt32(noiseLevelsItems[i].InnerText);

                XmlNode nodeDataPath = nodeDataGen.SelectSingleNode("data_path");

                SubDirDataSet = nodeDataPath.SelectSingleNode("output_dataset").InnerText.TrimEnd(new char[] { '\\' }) + '\\'; 

                XmlNode nodeSubDir = nodeDataPath.SelectSingleNode("sub_dir_level_1");
                SubDirImg = nodeSubDir.SelectSingleNode("image").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirMask = nodeSubDir.SelectSingleNode("mask").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirClassImg = nodeSubDir.SelectSingleNode("class_img").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirPred = nodeSubDir.SelectSingleNode("pred").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodeTrainVal = nodeDataPath.SelectSingleNode("sub_dir_level_2");
                SubDirTrain = nodeTrainVal.SelectSingleNode("train").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirVal = nodeTrainVal.SelectSingleNode("val").InnerText.TrimEnd(new char[] { '\\' }) + '\\';
                SubDirTest = nodeTrainVal.SelectSingleNode("test").InnerText.TrimEnd(new char[] { '\\' }) + '\\';

                XmlNode nodePrefix = nodeDataPath.SelectSingleNode("prefix");
                PrefixImg = nodePrefix.SelectSingleNode("image").InnerText;
                PrefixMask = nodePrefix.SelectSingleNode("mask").InnerText;
                PrefixClassImg = nodePrefix.SelectSingleNode("class_img").InnerText;
                PrefixPred = nodePrefix.SelectSingleNode("pred").InnerText;

                XmlNode nodeFnames = nodeDataPath.SelectSingleNode("file_names");
                ClassTextFileName = nodeFnames.SelectSingleNode("class_file_name").InnerText;
                ColorMapFileName = nodeFnames.SelectSingleNode("colormap_file_name").InnerText;

                XmlNode nodeSegmClass = nodeSettings.SelectSingleNode("segm_class_defs");
                XmlNodeList nodeItems = nodeSegmClass.SelectNodes("item");
                for (int i = 0; i < Math.Min(nodeItems.Count,SegmClassDefs.Defs.Length); i++)
                {
                    SegmClassDefs.Defs[i].Name = nodeItems[i].InnerText;
                    SegmClassDefs.Defs[i].DrawColor = Color.FromArgb(Convert.ToInt32(nodeItems[i].Attributes["draw_color"].Value, 16));
                }

                fileLoadedCorrectly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when loading Settings");
            }
        }

        /// <summary>
        /// Saves the current contents of all public fields to the XML file using the file name passed in the constructor.
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
                nodeSettings.AppendChild(doc.CreateElement("identifier")).AppendChild(doc.CreateTextNode(XML_FILE_ID_STR));
                nodeSettings.AppendChild(doc.CreateElement("path_to_data_storage")).AppendChild(doc.CreateTextNode(PathToDataStorage));
                nodeSettings.AppendChild(doc.CreateElement("sub_dir_street_maps")).AppendChild(doc.CreateTextNode(SubDirStreetmaps));

                XmlNode nodeStreetMap = nodeSettings.AppendChild(doc.CreateElement("street_map"));
                XmlNode nodeStreetDimensions = nodeStreetMap.AppendChild(doc.CreateElement("dimensions"));
                nodeStreetDimensions.AppendChild(doc.CreateElement("drawing_width")).AppendChild(doc.CreateTextNode(DefaultDrawingWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("drawing_height")).AppendChild(doc.CreateTextNode(DefaultDrawingHeight.ToString()));

                nodeStreetDimensions.AppendChild(doc.CreateElement("lane_width")).AppendChild(doc.CreateTextNode(LaneWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("line_width")).AppendChild(doc.CreateTextNode(LineWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("line_space")).AppendChild(doc.CreateTextNode(LineSpace.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("stop_line_width")).AppendChild(doc.CreateTextNode(StopLineWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("stop_line_offset")).AppendChild(doc.CreateTextNode(StopLineOffset.ToString()));

                nodeStreetDimensions.AppendChild(doc.CreateElement("crosswalk_width")).AppendChild(doc.CreateTextNode(CrosswalkWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("crosswalk_line_width")).AppendChild(doc.CreateTextNode(CrosswalkLineWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("crosswalk_stripe_width")).AppendChild(doc.CreateTextNode(CrosswalkStripeWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("crosswalk_border")).AppendChild(doc.CreateTextNode(CrosswalkBorder.ToString()));

                nodeStreetDimensions.AppendChild(doc.CreateElement("dash_length")).AppendChild(doc.CreateTextNode(DashLength.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("min_straight_length")).AppendChild(doc.CreateTextNode(MinStraightLength.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("min_inner_radius")).AppendChild(doc.CreateTextNode(MinInnerRadius.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("corner_radius")).AppendChild(doc.CreateTextNode(CornerRadius.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("min_curve_angle")).AppendChild(doc.CreateTextNode(MinCurveAngle.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("max_curve_angle")).AppendChild(doc.CreateTextNode(MaxCurveAngle.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("angle_step")).AppendChild(doc.CreateTextNode(AngleStep.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("length_step")).AppendChild(doc.CreateTextNode(LengthStep.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("max_arrow_overlay_width")).AppendChild(doc.CreateTextNode(MaxArrowOverlayWidth.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("max_arrow_overlay_length")).AppendChild(doc.CreateTextNode(MaxArrowOverlayLength.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("min_arrow_overlay_step")).AppendChild(doc.CreateTextNode(MinArrowOverlayStep.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("max_lane_count_left_right")).AppendChild(doc.CreateTextNode(MaxLaneCountLeftRight.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("max_lane_count_center")).AppendChild(doc.CreateTextNode(MaxLaneCountCenter.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("intersection_recess")).AppendChild(doc.CreateTextNode(IntersectionRecess.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("intersection_font_size")).AppendChild(doc.CreateTextNode(IntersectionFontSize.ToString()));
                nodeStreetDimensions.AppendChild(doc.CreateElement("overlay_font_size")).AppendChild(doc.CreateTextNode(OverlayFontSize.ToString()));

                XmlNode nodeStreetColors = nodeStreetMap.AppendChild(doc.CreateElement("colors"));
                nodeStreetColors.AppendChild(doc.CreateElement("lane_color")).AppendChild(doc.CreateTextNode(LaneColor.ToArgb().ToString("X8")));
                nodeStreetColors.AppendChild(doc.CreateElement("line_color_white")).AppendChild(doc.CreateTextNode(LineColorWhite.ToArgb().ToString("X8")));
                nodeStreetColors.AppendChild(doc.CreateElement("line_color_yellow")).AppendChild(doc.CreateTextNode(LineColorYellow.ToArgb().ToString("X8")));
                nodeStreetColors.AppendChild(doc.CreateElement("stop_line_color")).AppendChild(doc.CreateTextNode(StopLineColor.ToArgb().ToString("X8")));
                nodeStreetColors.AppendChild(doc.CreateElement("arrow_overlay_color")).AppendChild(doc.CreateTextNode(ArrowOverlayColor.ToArgb().ToString("X8")));
                nodeStreetColors.AppendChild(doc.CreateElement("background_color")).AppendChild(doc.CreateTextNode(BackgroundColor.ToArgb().ToString("X8")));

                XmlNode nodePrintSettings = nodeSettings.AppendChild(doc.CreateElement("print_settings"));
                XmlNode nodePrintPaperSource = nodePrintSettings.AppendChild(doc.CreateElement("paper_source"));
                nodePrintPaperSource.AppendChild(doc.CreateElement("name")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSource.SourceName));
                nodePrintPaperSource.AppendChild(doc.CreateElement("kind")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSource.Kind.ToString()));
                nodePrintPaperSource.AppendChild(doc.CreateElement("raw_kind")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSource.RawKind.ToString()));
                
                XmlNode nodePrintPaperSize = nodePrintSettings.AppendChild(doc.CreateElement("paper_size"));
                nodePrintPaperSize.AppendChild(doc.CreateElement("raw_kind")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSize.RawKind.ToString()));
                nodePrintPaperSize.AppendChild(doc.CreateElement("name")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSize.PaperName));
                nodePrintPaperSize.AppendChild(doc.CreateElement("width")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSize.Width.ToString()));
                nodePrintPaperSize.AppendChild(doc.CreateElement("height")).AppendChild(doc.CreateTextNode(PrintPageSettings.PaperSize.Height.ToString()));

                XmlNode nodePrintMargins = nodePrintSettings.AppendChild(doc.CreateElement("margins"));
                nodePrintMargins.AppendChild(doc.CreateElement("left")).AppendChild(doc.CreateTextNode(PrintPageSettings.Margins.Left.ToString()));
                nodePrintMargins.AppendChild(doc.CreateElement("top")).AppendChild(doc.CreateTextNode(PrintPageSettings.Margins.Top.ToString()));
                nodePrintMargins.AppendChild(doc.CreateElement("right")).AppendChild(doc.CreateTextNode(PrintPageSettings.Margins.Right.ToString()));
                nodePrintMargins.AppendChild(doc.CreateElement("bottom")).AppendChild(doc.CreateTextNode(PrintPageSettings.Margins.Bottom.ToString()));

                nodePrintSettings.AppendChild(doc.CreateElement("landscape")).AppendChild(doc.CreateTextNode(PrintPageSettings.Landscape.ToString()));
                nodePrintSettings.AppendChild(doc.CreateElement("color")).AppendChild(doc.CreateTextNode(PrintPageSettings.Color.ToString()));


                XmlNode nodeGui = nodeSettings.AppendChild(doc.CreateElement("gui"));
                XmlNode nodeGuiParm = nodeGui.AppendChild(doc.CreateElement("parameter"));
                nodeGuiParm.AppendChild(doc.CreateElement("display_measurement_unit")).AppendChild(doc.CreateTextNode(DisplayMeasurementUnit.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_straight_length")).AppendChild(doc.CreateTextNode(DefaultStraightLength.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_junction_length")).AppendChild(doc.CreateTextNode(DefaultJunctionLength.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_curve_angle")).AppendChild(doc.CreateTextNode(DefaultCurveAngle.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_ramp_curve_angle")).AppendChild(doc.CreateTextNode(DefaultRampCurveAngle.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_ramp_radius")).AppendChild(doc.CreateTextNode(DefaultRampRadius.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("default_roundabout_radius")).AppendChild(doc.CreateTextNode(DefaultRoundaboutRadius.ToString()));

                nodeGuiParm.AppendChild(doc.CreateElement("point_catch_distance")).AppendChild(doc.CreateTextNode(PointCatchDistance.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("connection_draw_distance")).AppendChild(doc.CreateTextNode(ConnectionDrawDistance.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("connection_snap_distance")).AppendChild(doc.CreateTextNode(ConnectionSnapDistance.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("street_outline_line_width")).AppendChild(doc.CreateTextNode(StreetOutlineLineWidth.ToString()));
                nodeGuiParm.AppendChild(doc.CreateElement("overlay_outline_line_width")).AppendChild(doc.CreateTextNode(OverlayOutlineLineWidth.ToString()));

                XmlNode nodeGuiColors = nodeGui.AppendChild(doc.CreateElement("colors"));
                nodeGuiColors.AppendChild(doc.CreateElement("street_outline_color")).AppendChild(doc.CreateTextNode(StreetOutlineColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("overlay_outline_color")).AppendChild(doc.CreateTextNode(OverlayOutlineColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("connector_in_color")).AppendChild(doc.CreateTextNode(ConnectorInColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("connector_out_color")).AppendChild(doc.CreateTextNode(ConnectorOutColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("connector_no_dir_color")).AppendChild(doc.CreateTextNode(ConnectorNoDirColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("connection_draw_color")).AppendChild(doc.CreateTextNode(ConnectionDrawColor.ToArgb().ToString("X8")));
                nodeGuiColors.AppendChild(doc.CreateElement("connection_snap_color")).AppendChild(doc.CreateTextNode(ConnectionSnapColor.ToArgb().ToString("X8")));

                XmlNode nodeHotkeys = nodeGui.AppendChild(doc.CreateElement("hotkeys"));
                nodeHotkeys.AppendChild(doc.CreateElement("delete")).AppendChild(doc.CreateTextNode(HotkeyDelete.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("abort")).AppendChild(doc.CreateTextNode(HotkeyAbort.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("disconnect")).AppendChild(doc.CreateTextNode(HotkeyDisconnect.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("rotate_left")).AppendChild(doc.CreateTextNode(HotkeyRotateLeft.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("rotate_right")).AppendChild(doc.CreateTextNode(HotkeyRotateRight.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("size_increase")).AppendChild(doc.CreateTextNode(HotkeySizeIncrease.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("size_decrease")).AppendChild(doc.CreateTextNode(HotkeySizeDecrease.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("size_mode_base")).AppendChild(doc.CreateTextNode(HotkeySizeModeBase.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("size_mode_ext")).AppendChild(doc.CreateTextNode(HotkeySizeModeExt.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("block_auto_rotate")).AppendChild(doc.CreateTextNode(HotkeyBlockAutoRotate.ToString()));
                nodeHotkeys.AppendChild(doc.CreateElement("property_page")).AppendChild(doc.CreateTextNode(HotkeyPropertyPage.ToString()));

                XmlNode nodeDataGen = nodeSettings.AppendChild(doc.CreateElement("data_generation"));
                XmlNode nodeVirtualCam = nodeDataGen.AppendChild(doc.CreateElement("virtual_camera"));

                nodeVirtualCam.AppendChild(doc.CreateElement("camera_hfov")).AppendChild(doc.CreateTextNode(CameraHFOV.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_image_ratio")).AppendChild(doc.CreateTextNode(CameraImageRatio.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_lens_distortion1")).AppendChild(doc.CreateTextNode(CameraLensDistortion1.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_lens_distortion2")).AppendChild(doc.CreateTextNode(CameraLensDistortion2.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_color_correction_red")).AppendChild(doc.CreateTextNode(CameraColorCorrRed.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_color_correction_green")).AppendChild(doc.CreateTextNode(CameraColorCorrGreen.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_color_correction_blue")).AppendChild(doc.CreateTextNode(CameraColorCorrBlue.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_height")).AppendChild(doc.CreateTextNode(CameraHeight.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_axis_angle")).AppendChild(doc.CreateTextNode(CameraAxisAngle.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_output_width")).AppendChild(doc.CreateTextNode(CameraOutputWidth.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_output_height")).AppendChild(doc.CreateTextNode(CameraOutputHeight.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("camera_oversampling")).AppendChild(doc.CreateTextNode(CameraOversampling.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("mark_lane_max_dist_front")).AppendChild(doc.CreateTextNode(MarkLaneMaxDistFront.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("mark_lane_max_dist_side")).AppendChild(doc.CreateTextNode(MarkLaneMaxDistSide.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("mark_lane_max_dist_side_angle")).AppendChild(doc.CreateTextNode(MarkLaneMaxDistSideAngle.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("mark_max_detail_distance")).AppendChild(doc.CreateTextNode(MarkMaxDetailDistance.ToString())); ;

                nodeVirtualCam.AppendChild(doc.CreateElement("draw_wrong_dir_items")).AppendChild(doc.CreateTextNode(DrawWrongDirItems.ToString()));
                nodeVirtualCam.AppendChild(doc.CreateElement("include_class_images")).AppendChild(doc.CreateTextNode(IncludeClassImages.ToString()));

                XmlNode nodeAugmentation = nodeDataGen.AppendChild(doc.CreateElement("augmentation"));

                nodeAugmentation.AppendChild(doc.CreateElement("train_val_ratio")).AppendChild(doc.CreateTextNode(TrainValRatio.ToString()));
                nodeAugmentation.AppendChild(doc.CreateElement("validate_center_views_only")).AppendChild(doc.CreateTextNode(ValidateCenterViewsOnly.ToString()));
                nodeAugmentation.AppendChild(doc.CreateElement("test_out_ratio")).AppendChild(doc.CreateTextNode(TestOutRatio.ToString()));
                nodeAugmentation.AppendChild(doc.CreateElement("test_center_views_only")).AppendChild(doc.CreateTextNode(TestCenterViewsOnly.ToString()));
                nodeAugmentation.AppendChild(doc.CreateElement("image_step_size")).AppendChild(doc.CreateTextNode(ImageStepSize.ToString()));
                nodeAugmentation.AppendChild(doc.CreateElement("center_brightness_results")).AppendChild(doc.CreateTextNode(CenterBrightnessResults.ToString()));

                XmlNode nodeSideSteps = nodeAugmentation.AppendChild(doc.CreateElement("side_steps"));
                foreach (float sideStep in SideSteps)
                    nodeSideSteps.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(sideStep.ToString()));

                XmlNode nodeAngleSteps = nodeAugmentation.AppendChild(doc.CreateElement("angle_steps"));
                foreach (float angleStep in AngleSteps)
                    nodeAngleSteps.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(angleStep.ToString()));

                XmlNode nodeBrightnessFactor = nodeAugmentation.AppendChild(doc.CreateElement("brightness_factors"));
                foreach (float brightnessValue in BrightnessFactors)
                    nodeBrightnessFactor.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(brightnessValue.ToString()));

                XmlNode nodeColorFactors = nodeAugmentation.AppendChild(doc.CreateElement("color_factors"));
                foreach (float colorFactor in ColorFactors)
                    nodeColorFactors.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(colorFactor.ToString()));

                XmlNode nodeNoiseLevels = nodeAugmentation.AppendChild(doc.CreateElement("noise_levels"));
                foreach (float noiseLevel in NoiseLevels)
                    nodeNoiseLevels.AppendChild(doc.CreateElement("item")).AppendChild(doc.CreateTextNode(noiseLevel.ToString()));

                XmlNode nodeDataPath = nodeDataGen.AppendChild(doc.CreateElement("data_path"));
                nodeDataPath.AppendChild(doc.CreateElement("output_dataset")).AppendChild(doc.CreateTextNode(SubDirDataSet));

                XmlNode nodeSubDir = nodeDataPath.AppendChild(doc.CreateElement("sub_dir_level_1"));
                nodeSubDir.AppendChild(doc.CreateElement("image")).AppendChild(doc.CreateTextNode(SubDirImg));
                nodeSubDir.AppendChild(doc.CreateElement("mask")).AppendChild(doc.CreateTextNode(SubDirMask));
                nodeSubDir.AppendChild(doc.CreateElement("class_img")).AppendChild(doc.CreateTextNode(SubDirClassImg));
                nodeSubDir.AppendChild(doc.CreateElement("pred")).AppendChild(doc.CreateTextNode(SubDirPred));

                XmlNode nodeTrainVal = nodeDataPath.AppendChild(doc.CreateElement("sub_dir_level_2"));
                nodeTrainVal.AppendChild(doc.CreateElement("train")).AppendChild(doc.CreateTextNode(SubDirTrain));
                nodeTrainVal.AppendChild(doc.CreateElement("val")).AppendChild(doc.CreateTextNode(SubDirVal));
                nodeTrainVal.AppendChild(doc.CreateElement("test")).AppendChild(doc.CreateTextNode(SubDirTest));

                XmlNode nodePrefix = nodeDataPath.AppendChild(doc.CreateElement("prefix"));
                nodePrefix.AppendChild(doc.CreateElement("image")).AppendChild(doc.CreateTextNode(PrefixImg));
                nodePrefix.AppendChild(doc.CreateElement("mask")).AppendChild(doc.CreateTextNode(PrefixMask));
                nodePrefix.AppendChild(doc.CreateElement("class_img")).AppendChild(doc.CreateTextNode(PrefixClassImg));
                nodePrefix.AppendChild(doc.CreateElement("pred")).AppendChild(doc.CreateTextNode(PrefixPred));

                XmlNode nodeFnames = nodeDataPath.AppendChild(doc.CreateElement("file_names"));
                nodeFnames.AppendChild(doc.CreateElement("class_file_name")).AppendChild(doc.CreateTextNode(ClassTextFileName));
                nodeFnames.AppendChild(doc.CreateElement("colormap_file_name")).AppendChild(doc.CreateTextNode(ColorMapFileName));

                XmlNode nodeSegmClass = nodeSettings.AppendChild(doc.CreateElement("segm_class_defs"));
                foreach (SegmClassDef scd in SegmClassDefs.Defs)
                {
                    XmlNode nodeItem = doc.CreateElement("item");
                    nodeSegmClass.AppendChild(nodeItem).AppendChild(doc.CreateTextNode(scd.Name));
                    nodeItem.Attributes.Append(doc.CreateAttribute("draw_color")).Value = scd.DrawColor.ToArgb().ToString("X8");
                }


                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error when saving Settings");
            }
        }
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Returns true, if the file existed.
        /// </summary>
        public bool FileExisted
        {
            get { return fileExisted; }
        }

        /// <summary>
        /// Returns true, if the file could be loaded correctly.
        /// </summary>
        public bool FileLoadedCorrectly
        {
            get { return fileLoadedCorrectly; }
        }
        #endregion Public Properties

    }
}
