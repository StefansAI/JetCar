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

namespace StreetMaker
{
    /// <summary>
    /// Enumeration of shapes for lanes, lines and streets.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>A straight shape.</summary>
        Straight,
        /// <summary>A curved shape.</summary>
        Curve,
        /// <summary>An S-shape.</summary>
        S_Shape,
    }

    /// <summary>
    /// Enumeration of lane connectors.
    /// </summary>
    public enum ConnectorMode
    {
        /// <summary>Direction goes in at this connector.</summary>
        In,
        /// <summary>Direction goes out from this connector.</summary>
        Out,
        /// <summary>There is no direction at this connector.</summary>
        NoDir,
        /// <summary>This connector stays hidden.</summary>
        Hidden
    }

    /// <summary>
    /// Enumeration of draw modes for seperating different layers.
    /// </summary>
    public enum DrawMode
    {
        /// <summary>Only draw the outline of the shape.</summary>
        Outline,
        /// <summary>Restore the background of the shape.</summary>
        Background,
        /// <summary>Only draw the base layer of the shape, like the lane.</summary>
        BaseLayer,
        /// <summary>Only draw the top layer of the shape, like lines.</summary>
        TopLayer,
        /// <summary>Only draw the overlays on top of the shape, like arrows and signs.</summary>
        Overlay,
        /// <summary>Only draw the view points on top of the shape and other overlays.</summary>
        ViewPoint
    }

    /// <summary>
    /// Enumeration of lane limit lines.
    /// </summary>
    public enum LineType
    {
        /// <summary>No line at all.</summary>
        None,
        /// <summary>Line is transparent or invisible.</summary>
        Transparent,
        /// <summary>Lane limit is a single white solid line to the shoulder.</summary>
        ShoulderLine,
        /// <summary>Lane limit is a single white solid line.</summary>
        SingleWhiteSolid,
        /// <summary>Lane limit is a single white dashed line.</summary>
        SingleWhiteDashed,
        /// <summary>Lane limit is a single yellow solid line.</summary>
        SingleYellowSolid,
        /// <summary>Lane limit is a single yellow dashed line.</summary>
        SingleYellowDashed,
        /// <summary>Lane limit is a double white solid line.</summary>
        DoubleWhiteSolid,
        /// <summary>Lane limit is a double yellow solid line.</summary>
        DoubleYellowSolid,
        /// <summary>Lane limit is a double yellow solid/dashed line.</summary>
        DoubleYellowSolidDashed,
        /// <summary>Lane limit is a double yellow dashed/solid line.</summary>
        DoubleYellowDashedSolid,

    }

    /// <summary>
    /// Enumeration of overlay types.
    /// </summary>
    public enum OverlayType
    {
        /// <summary>Direction arrow to turn left only.</summary>
        ArrowLeftOnly,
        /// <summary>Direction arrow to turn left or right only.</summary>
        ArrowLeftRight,
        /// <summary>Direction arrow to turn right only.</summary>
        ArrowRightOnly,
        /// <summary>Direction arrow to go straight or left only.</summary>
        ArrowStraightLeft,
        /// <summary>Direction arrow to go straight only.</summary>
        ArrowStraightOnly,
        /// <summary>Direction arrow to go straight or right only.</summary>
        ArrowStraightRight,
        /// <summary>Direction arrow to merge left.</summary>
        ArrowMergeLeft,
        /// <summary>Direction arrow to merge right.</summary>
        ArrowMergeRight,
        /// <summary>Simple parking sign.</summary>
        ParkingSign,
        /// <summary>A special overlay to mark a view point for the camera view when creating a dataset.</summary>
        ViewPoint
    }

    /// <summary>
    /// Enumeration of street shapes.
    /// </summary>
    public enum StreetType
    {
        /// <summary>Street is straight.</summary>
        Straight,
        /// <summary>Street is curved to the right.</summary>
        CurveRight,
        /// <summary>Street is curved to the left.</summary>
        CurveLeft,
        /// <summary>Street is s-shaped to the right.</summary>
        S_Right,
        /// <summary>Street is s-shaped to the left.</summary>
        S_Left,
        /// <summary>Lane is splitting into multiple lanes right in s-shapes.</summary>
        Lane_Split_Right,
        /// <summary>Lanes are coming together in s-shapes.</summary>
        Lane_Union_Right,
        /// <summary>Lane is splitting into multiple lanes right in s-shapes.</summary>
        Lane_Split_Both,
        /// <summary>Lanes are coming together in s-shapes.</summary>
        Lane_Union_Both,
        /// <summary>Street is splitting left and right in s-shapes.</summary>
        Center_Split,
        /// <summary>Street is coming together in s-shapes.</summary>
        Center_Union
    }

    /// <summary>
    /// Enumeration for different street edit modes.
    /// </summary>
    public enum StreetEditMode
    {
        /// <summary>No active edit mode.</summary>
        Nothing,
        /// <summary>Currently adding and placing a new street element.</summary>
        AddNewStreetElement,
        /// <summary>Move a previously placed street element.</summary>
        MoveActiveStreetElement,
        /// <summary>Size a previously placed street element.</summary>
        SizeActiveStreetElement,
        /// <summary>Currently adding and placing a new overlay.</summary>
        AddNewOverlay,
        /// <summary>Move a previously placed overlay.</summary>
        MoveActiveOverlay
    }

    /// <summary>
    /// Enumeration for stop and yield line types.
    /// </summary>
    public enum StopYieldType
    {
        /// <summary>No stop or yield line.</summary>
        None,
        /// <summary>Yield line only without text.</summary>
        YieldLine,
        /// <summary>Yield line with additional text.</summary>
        YieldLineText,
        /// <summary>Stop line only without text.</summary>
        StopLine,
        /// <summary>Stop line with additional text.</summary>
        StopLineText
    }

    /// <summary>
    /// Enumeration for crosswalk types.
    /// </summary>
    public enum CrosswalkType
    {
        /// <summary>No crosswalk.</summary>
        None,
        /// <summary>CrosswalkType marked just parallel lines.</summary>
        ParallelLines,
        /// <summary>CrosswalkType marked with zebra stripes.</summary>
        ZebraStripes
    }

    /// <summary>
    /// Enumeration for ramp types.
    /// </summary>
    public enum RampType
    {
        /// <summary>No ramp.</summary>
        None,
        /// <summary>Exit or off ramp. Just a lane spliting off too the right in a defined radius.</summary>
        ExitRamp,
        /// <summary>Entrance or on ramp. A curved lane merging into the right lane.</summary>
        Entrance
    }

    /// <summary>
    /// Enumeration for measurement units.
    /// </summary>
    public enum MeasurementUnit
    {
        /// <summary>Use millimeter.</summary>
        Millimeter,
        /// <summary>Use centimeter.</summary>
        Centimeter,
        /// <summary>Use meter.</summary>
        Meter,
        /// <summary>Use inch.</summary>
        Inch,
        /// <summary>Use feet.</summary>
        Feet
    }


    public enum LaneDirType
    {
        DrivingDir,
        LeftTurnDir,
        RightTurnDir,
        WrongDir,
        SharedDir
    }

    public enum ColorMode
    {
        ImageColor,
        ClassCode,
        ClassColor
    }
}
