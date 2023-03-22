// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;


namespace StreetMaker
{
    /// <summary>
    /// Basic property definitions for segmentation classes. There should be one for each segmentation class in a project, stored in the XML file. 
    /// </summary>
    public class SegmClassDef
    {
        #region Public Readonly Fields
        /// <summary>.</summary>
        public readonly object ClassEnum;
        /// <summary>Identifier or code of the class.</summary>
        public int ClassCode;
        /// <summary>Human friendly name of the class, used for the GUI.</summary>
        public string Name;
        /// <summary>Color to be used in the GUI when displaying the polygon on the image or the filled area in the mask.</summary>
        public Color DrawColor;
        /// <summary>Approximated number of times used in a current StreetMap.</summary>
        public int UseCount;
        /// <summary>DrawOrder used for export to ImageSegmenter application.</summary>
        public int DrawOrder;
        #endregion Public Readonly Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the segmentation class definition, loading the passed parameter into its fields.
        /// </summary>
        /// <param name="ClassEnum">Enumeration of LineType, LaneDirType, OverlayType etc.</param>
        /// <param name="ClassCode">Identifier or code of the class.</param>
        /// <param name="Name">Human friendly name of the class, used for the GUI.</param>
        /// <param name="DrawColor">Color to be used in the GUI when displaying the polygon on the image or the filled area in the mask.</param>
        /// <param name="DrawOrder">Draw order used for ImageSegementer export</param>
        public SegmClassDef(object ClassEnum, int ClassCode, string Name, Color DrawColor, int DrawOrder)
        {
            this.ClassEnum = ClassEnum;
            this.ClassCode = ClassCode;
            this.Name = Name;
            this.DrawColor = DrawColor;
            this.UseCount = 0;
            this.DrawOrder = DrawOrder;
        }
        #endregion Constructor

        /// <summary>Gets the color representation of the class code of this object.</summary>
        public Color ClassCodeColor
        {
            get 
            {
                int code = Math.Max(ClassCode, 0);
                return Color.FromArgb(code, code, code); 
            }
        }
    }

    /// <summary>
    /// Segmentation Class Definitions of varies types used in this application.
    /// </summary>
    public class SegmClassDefs
    {
        /// <summary>
        /// Default segmentation class definitions as loaded initially.
        /// </summary>
        public static SegmClassDef[] Defs =
        {
            new SegmClassDef(null,                               0, "nothing",                     Color.FromArgb(unchecked((int)0xFF000000)), 10),     //  0
            new SegmClassDef(LineType.ShoulderLine,              1, "white_shoulder_line",         Color.FromArgb(unchecked((int)0xFF059FDA)),  9),     //  1
            new SegmClassDef(LineType.SingleWhiteSolid,          2, "white_solid_line",            Color.FromArgb(unchecked((int)0xFF02C8F2)),  9),     //  2
            new SegmClassDef(LineType.SingleWhiteDashed,         3, "white_dashed_line",           Color.FromArgb(unchecked((int)0xFF10F5C1)),  9),     //  3
            new SegmClassDef(LineType.DoubleWhiteSolid,          4, "white_double_line",           Color.FromArgb(unchecked((int)0xFF5FFCA6)),  9),     //  4
            new SegmClassDef(LineType.SingleYellowSolid,         5, "yellow_solid_line",           Color.FromArgb(unchecked((int)0xFFF5F023)),  9),     //  5
            new SegmClassDef(LineType.SingleYellowDashed,        6, "yellow_dashed_line",          Color.FromArgb(unchecked((int)0xFFE1D579)),  9),     //  6
            new SegmClassDef(LineType.DoubleYellowSolid,         7, "yellow_double_line",          Color.FromArgb(unchecked((int)0xFFD0CE8A)),  9),     //  7
            new SegmClassDef(LineType.DoubleYellowSolidDashed,   8, "yellow_solid_dashed_line",    Color.FromArgb(unchecked((int)0xFF88E278)),  9),     //  8
            new SegmClassDef(LineType.DoubleYellowDashedSolid,   9, "yellow_dashed_solid_line",    Color.FromArgb(unchecked((int)0xFFE1BD79)),  9),     //  9
            new SegmClassDef(LineType.Transparent,              10, "lane_limit_line",             Color.FromArgb(unchecked((int)0xFFD7D700)),  9),     // 10

            new SegmClassDef(LaneDirType.DrivingDir,            11, "lane_driving_dir",            Color.FromArgb(unchecked((int)0xFF00FF00)),  4),     // 11
            new SegmClassDef(LaneDirType.WrongDir,              12, "lane_wrong_dir",              Color.FromArgb(unchecked((int)0xFF8A0000)),  0),     // 12
            new SegmClassDef(LaneDirType.LeftTurnDir,           13, "lane_left_turn",              Color.FromArgb(unchecked((int)0xFF8BB803)),  1),     // 13
            new SegmClassDef(LaneDirType.RightTurnDir,          14, "lane_right_turn",             Color.FromArgb(unchecked((int)0xFF008040)),  2),     // 14
            new SegmClassDef(LaneDirType.SharedDir,             15, "lane_center",                 Color.FromArgb(unchecked((int)0xFF255A3A)),  3),     // 15
     
            new SegmClassDef(StopYieldType.YieldLine,           16, "yield_line",                  Color.FromArgb(unchecked((int)0xFFFF8000)),  5),     // 16
            new SegmClassDef(StopYieldType.YieldLineText,       17, "yield_text",                  Color.FromArgb(unchecked((int)0xFFD75C28)),  6),     // 17
            new SegmClassDef(StopYieldType.StopLine,            18, "stop_line",                   Color.FromArgb(unchecked((int)0xFFFF0000)),  5),     // 18
            new SegmClassDef(StopYieldType.StopLineText,        19, "stop_text",                   Color.FromArgb(unchecked((int)0xFFCE0000)),  6),     // 19

            new SegmClassDef(CrosswalkType.ParallelLines,       20, "crosswalk_lines",             Color.FromArgb(unchecked((int)0xFF7A057A)),  5),     // 20
            new SegmClassDef(CrosswalkType.ZebraStripes,        21, "crosswalk_zebra",             Color.FromArgb(unchecked((int)0xFFC03FC0)),  5),     // 21

            new SegmClassDef(OverlayType.ArrowStraightOnly,     22, "arrow_straight",              Color.FromArgb(unchecked((int)0xFFD7AC00)),  8),     // 22
            new SegmClassDef(OverlayType.ArrowStraightLeft,     23, "arrow_straight_left",         Color.FromArgb(unchecked((int)0xFFDF6862)),  8),     // 23
            new SegmClassDef(OverlayType.ArrowStraightRight,    24, "arrow_straight_right",        Color.FromArgb(unchecked((int)0xFFC00080)),  8),     // 24
            new SegmClassDef(OverlayType.ArrowLeftOnly,         25, "arrow_left_only",             Color.FromArgb(unchecked((int)0xFF82BF00)),  8),     // 25
            new SegmClassDef(OverlayType.ArrowRightOnly,        26, "arrow_right_only",            Color.FromArgb(unchecked((int)0xFF00C080)),  8),     // 26
            new SegmClassDef(OverlayType.ArrowLeftRight,        27, "arrow_left_right",            Color.FromArgb(unchecked((int)0xFF800080)),  8),     // 27
            new SegmClassDef(OverlayType.ArrowMergeLeft,        28, "merge_left",                  Color.FromArgb(unchecked((int)0xFF00C100)),  8),     // 28
            new SegmClassDef(OverlayType.ArrowMergeRight,       29, "merge_right",                 Color.FromArgb(unchecked((int)0xFFFF4000)),  8),     // 29
            new SegmClassDef(OverlayType.ParkingSign,           30, "parking_sign",                Color.FromArgb(unchecked((int)0xFF735AA5)),  7),     // 30
        };

        /// <summary>
        /// Get the reference to the SegmClassDef object associated with the passed ClassEnum. 
        /// </summary>
        /// <param name="ClassEnum">Enumeration to look for.</param>
        /// <returns>Reference to the SegmClassDef object containing this ClassEnum.</returns>
        public static SegmClassDef GetSegmClassDef(object ClassEnum)
        {
            if (ClassEnum == null)
            {
                foreach (SegmClassDef scd in Defs)
                    if (scd.ClassEnum == null)
                        return scd;
            }
            else
            {
                foreach (SegmClassDef scd in Defs)
                    if ((scd.ClassEnum != null) && (scd.ClassEnum.GetType() == ClassEnum.GetType()) && (scd.ClassEnum.ToString() == ClassEnum.ToString()))
                        return scd;
            }
            return Defs[0];
        }

        /// <summary>Reference to the SegmClassDef for Nothing.</summary>
        public static SegmClassDef ScdNothing = SegmClassDefs.GetSegmClassDef(null);
        /// <summary>Reference to the SegmClassDef for WrongDir.</summary>
        public static SegmClassDef ScdWrongDir = SegmClassDefs.GetSegmClassDef(LaneDirType.WrongDir);
        /// <summary>Reference to the SegmClassDef for DrivingDir.</summary>
        public static SegmClassDef ScdDrivingDir = SegmClassDefs.GetSegmClassDef(LaneDirType.DrivingDir);
        /// <summary>Reference to the SegmClassDef for SharedDir.</summary>
        public static SegmClassDef ScdCenterLane = SegmClassDefs.GetSegmClassDef(LaneDirType.SharedDir);
        /// <summary>Reference to the SegmClassDef for LeftTurnDir.</summary>
        public static SegmClassDef ScdLeftTurnDir = SegmClassDefs.GetSegmClassDef(LaneDirType.LeftTurnDir);
        /// <summary>Reference to the SegmClassDef for RightTurnDir.</summary>
        public static SegmClassDef ScdRightTurnDir = SegmClassDefs.GetSegmClassDef(LaneDirType.RightTurnDir);
        /// <summary>Reference to the SegmClassDef for Transparent.</summary>
        public static SegmClassDef ScdLaneLimitLine = SegmClassDefs.GetSegmClassDef(LineType.Transparent);

        /// <summary>
        /// Returns true, if the passed SegmClassDef is a DrivingDir, a LeftTurnDir or a RightTurnDir
        /// </summary>
        /// <param name="SCD">SegmClassDef to check</param>
        /// <returns>True, if one of the directions</returns>
        public static bool IsDriveLeftOrRight(SegmClassDef SCD)
        {
            return (SCD == ScdDrivingDir) || (SCD == ScdLeftTurnDir) || (SCD == ScdRightTurnDir);
        }

        /// <summary>
        /// Clear the use counts of all SegmClassDef objeects in Defs.
        /// </summary>
        public static void ClearUseCounts()
        {
            foreach (SegmClassDef scd in Defs)
                scd.UseCount = 0;

            ResetClassCodes();
        }

        /// <summary>
        /// Increment the use count for the SegmClassDef object specified via reference or ClassEnum.
        /// </summary>
        /// <param name="EnumOrSCD">EnumClass or reference of the object in Defs.</param>
        public static void IncUseCount(object EnumOrSCD)
        {
            SegmClassDef scd = null;
            if (EnumOrSCD is SegmClassDef)
                scd = EnumOrSCD as SegmClassDef;
            else
                scd = GetSegmClassDef(EnumOrSCD);

            if (scd != null)
                scd.UseCount++;
        }

        /// <summary>
        /// Reset all class codes in Defs by enumerating them in order.
        /// </summary>
        public static void ResetClassCodes()
        {
            for (int i = 0; i < Defs.Length; i++)
                Defs[i].ClassCode = i;
        }

        /// <summary>
        /// Optimize the class codes in Defs by only enumerating the the classes with use counts > 0.
        /// </summary>
        /// <returns>Number of used classes.</returns>
        public static int OptClassCodes()
        {
            int i = 0;
            // Make sure, these are always included
            ScdNothing.UseCount++;
            ScdDrivingDir.UseCount++;
            ScdWrongDir.UseCount++;
            ScdLeftTurnDir.UseCount++;
            ScdRightTurnDir.UseCount++;

            // Now, optimize all all
            foreach (SegmClassDef scd in Defs)
                if (scd.UseCount > 0)
                    scd.ClassCode = i++;
                else
                    scd.ClassCode = -1;
            return i;
        }

    }
}
