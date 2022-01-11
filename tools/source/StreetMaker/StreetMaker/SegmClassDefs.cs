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
        #endregion Public Readonly Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the segmentation class definition, loading the passed parameter into its fields.
        /// </summary>
        /// <param name="ClassCode">Identifier or code of the class.</param>
        /// <param name="Name">Human friendly name of the class, used for the GUI.</param>
        /// <param name="DrawColor">Color to be used in the GUI when displaying the polygon on the image or the filled area in the mask.</param>
        public SegmClassDef(object ClassEnum, int ClassCode, string Name, Color DrawColor)
        {
            this.ClassEnum = ClassEnum;
            this.ClassCode = ClassCode;
            this.Name = Name;
            this.DrawColor = DrawColor;
            this.UseCount = 0;
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

    public class SegmClassDefs
    {

        /// <summary>
        /// Default segmentation class definitions as loaded initially.
        /// </summary>
        public static SegmClassDef[] Defs =
        {
            new SegmClassDef(null,                               0, "nothing",                     Color.FromArgb(unchecked((int)0xFF000000))),     //  0
            new SegmClassDef(LineType.ShoulderLine,              1, "white_shoulder_line",         Color.FromArgb(unchecked((int)0xFF059FDA))),     //  1
            new SegmClassDef(LineType.SingleWhiteSolid,          2, "white_solid_line",            Color.FromArgb(unchecked((int)0xFF02C8F2))),     //  2
            new SegmClassDef(LineType.SingleWhiteDashed,         3, "white_dashed_line",           Color.FromArgb(unchecked((int)0xFF10F5C1))),     //  3
            new SegmClassDef(LineType.DoubleWhiteSolid,          4, "white_double_line",           Color.FromArgb(unchecked((int)0xFF5FFCA6))),     //  4
            new SegmClassDef(LineType.SingleYellowSolid,         5, "yellow_solid_line",           Color.FromArgb(unchecked((int)0xFFF5F023))),     //  5
            new SegmClassDef(LineType.SingleYellowDashed,        6, "yellow_dashed_line",          Color.FromArgb(unchecked((int)0xFFE1D579))),     //  6
            new SegmClassDef(LineType.DoubleYellowSolid,         7, "yellow_double_line",          Color.FromArgb(unchecked((int)0xFFD0CE8A))),     //  7
            new SegmClassDef(LineType.DoubleYellowSolidDashed,   8, "yellow_solid_dashed_line",    Color.FromArgb(unchecked((int)0xFF88E278))),     //  8
            new SegmClassDef(LineType.DoubleYellowDashedSolid,   9, "yellow_dashed_solid_line",    Color.FromArgb(unchecked((int)0xFFE1BD79))),     //  9
            new SegmClassDef(LineType.Transparent,              10, "lane_limit_line",             Color.FromArgb(unchecked((int)0xFFD7D700))),     // 10

            new SegmClassDef(LaneDirType.DrivingDir,            11, "lane_driving_dir",            Color.FromArgb(unchecked((int)0xFF00FF00))),     // 11
            new SegmClassDef(LaneDirType.WrongDir,              12, "lane_wrong_dir",              Color.FromArgb(unchecked((int)0xFF8A0000))),     // 12
            new SegmClassDef(LaneDirType.LeftTurnDir,           13, "lane_left_turn",              Color.FromArgb(unchecked((int)0xFF8BB803))),     // 13
            new SegmClassDef(LaneDirType.RightTurnDir,          14, "lane_right_turn",             Color.FromArgb(unchecked((int)0xFF008040))),     // 14
            new SegmClassDef(LaneDirType.SharedDir,             15, "lane_center",                 Color.FromArgb(unchecked((int)0xFF255A3A))),     // 15
     
            new SegmClassDef(StopYieldType.YieldLine,           16, "yield_line",                  Color.FromArgb(unchecked((int)0xFFFF8000))),     // 16
            new SegmClassDef(StopYieldType.YieldLineText,       17, "yield_text",                  Color.FromArgb(unchecked((int)0xFFD75C28))),     // 17
            new SegmClassDef(StopYieldType.StopLine,            18, "stop_line",                   Color.FromArgb(unchecked((int)0xFFFF0000))),     // 18
            new SegmClassDef(StopYieldType.StopLineText,        19, "stop_text",                   Color.FromArgb(unchecked((int)0xFFCE0000))),     // 19

            new SegmClassDef(CrosswalkType.ParallelLines,       20, "crosswalk_lines",             Color.FromArgb(unchecked((int)0xFF7A057A))),     // 20
            new SegmClassDef(CrosswalkType.ZebraStripes,        21, "crosswalk_zebra",             Color.FromArgb(unchecked((int)0xFFC03FC0))),     // 21

            new SegmClassDef(OverlayType.ArrowStraightOnly,     22, "arrow_straight",              Color.FromArgb(unchecked((int)0xFFD7AC00))),     // 22
            new SegmClassDef(OverlayType.ArrowStraightLeft,     23, "arrow_straight_left",         Color.FromArgb(unchecked((int)0xFFDF6862))),     // 23
            new SegmClassDef(OverlayType.ArrowStraightRight,    24, "arrow_straight_right",        Color.FromArgb(unchecked((int)0xFFC00080))),     // 24
            new SegmClassDef(OverlayType.ArrowLeftOnly,         25, "arrow_left_only",             Color.FromArgb(unchecked((int)0xFF82BF00))),     // 25
            new SegmClassDef(OverlayType.ArrowRightOnly,        26, "arrow_right_only",            Color.FromArgb(unchecked((int)0xFF00C080))),     // 26
            new SegmClassDef(OverlayType.ArrowLeftRight,        27, "arrow_left_right",            Color.FromArgb(unchecked((int)0xFF800080))),     // 27
            new SegmClassDef(OverlayType.ArrowMergeLeft,        28, "merge_left",                  Color.FromArgb(unchecked((int)0xFF00C100))),     // 28
            new SegmClassDef(OverlayType.ArrowMergeRight,       29, "merge_right",                 Color.FromArgb(unchecked((int)0xFFFF4000))),     // 29
            new SegmClassDef(OverlayType.ParkingSign,           30, "parking_sign",                Color.FromArgb(unchecked((int)0xFF735AA5))),     // 30
        };

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


        public static SegmClassDef ScdNothing = SegmClassDefs.GetSegmClassDef(null);
        public static SegmClassDef ScdWrongDir = SegmClassDefs.GetSegmClassDef(LaneDirType.WrongDir);
        public static SegmClassDef ScdDrivingDir = SegmClassDefs.GetSegmClassDef(LaneDirType.DrivingDir);
        public static SegmClassDef ScdCenterLane = SegmClassDefs.GetSegmClassDef(LaneDirType.SharedDir);
        public static SegmClassDef ScdLeftTurnDir = SegmClassDefs.GetSegmClassDef(LaneDirType.LeftTurnDir);
        public static SegmClassDef ScdRightTurnDir = SegmClassDefs.GetSegmClassDef(LaneDirType.RightTurnDir);
        public static SegmClassDef ScdLaneLimitLine = SegmClassDefs.GetSegmClassDef(LineType.Transparent);

        public static void ClearUseCounts()
        {
            foreach (SegmClassDef scd in Defs)
                scd.UseCount = 0;

            ResetClassCodes();
        }

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

        public static void ResetClassCodes()
        {
            for (int i = 0; i < Defs.Length; i++)
                Defs[i].ClassCode = i;
        }

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
