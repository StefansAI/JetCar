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
using System.Drawing.Drawing2D;
using System.Xml;
using System.Windows.Forms;

using System.Diagnostics;

namespace StreetMaker
{
    /// <summary>
    /// Class to handle different street marking overlays like direction arrows for turning and merging or a parking sign in form as a P.
    /// Overlays are automatically alligned with the driving direction in the center of the lane, which will then own this overlay and keep it at that position and direction.
    /// </summary>
    public class Overlay
    {
        #region Private Fields
        /// <summary>Maximum arraow overlay width to fit inside the lane between lines and some margin.</summary>
        private readonly double MaxArrowOverlayWidth;
        /// <summary>Maximum arraow overlay length to work well with the MaxArrowOverlayWidth.</summary>
        private readonly double MaxArrowOverlayLength;
        /// <summary>Minimum arrow overlay step size in the connector distance to allow better alignemnts between lanes.</summary>
        private readonly double MinArrowOverlayStep;
        /// <summary>Color of the overlay.</summary>
        private readonly Color Color;
        /// <summary>Color of the overlay outline in the GUI when highighted.</summary>
        private readonly Color OutlineColor;
        /// <summary>Line width of the overlay outline in the GUI when highlighted.</summary>
        private readonly double OutlineLineWidth;
        /// <summary>The font size of P markings in the overlay.</summary>
        private readonly double OverlayFontSize;

        /// <summary>Length of the straight arrow head.</summary>
        private readonly double straightArrowHeadLength;
        /// <summary>Recess of straight arrow head between outer tips and stem.</summary>
        private readonly double straightArrowHeadRecess;
        /// <summary>Total length of the straight arrow.</summary>
        private readonly double straightTotalLength;
        /// <summary>Width of the straight arrow.</summary>
        private readonly double straightArrowWidth;
        /// <summary>Width of the straight arrow stem at the arrow head.</summary>
        private readonly double straightStemWidthTop;
        /// <summary>Width of the straight arrow stem at the bottom.</summary>
        private readonly double straightStemWidthBottom;

        /// <summary>Length of the curved arrow head.</summary>
        private readonly double curvedArrowHeadLength;
        /// <summary>Vertical tip offset of the curved arrow head.</summary>
        private readonly double curvedArrowHeadTipOffs;
        /// <summary>Recess of curved arrow head between outer tips and stem.</summary>
        private readonly double curvedArrowHeadRecess;
        /// <summary>Total length of the cruved arrow.</summary>
        private readonly double curvedArrowTotalLength;
        /// <summary>Width of the curved arrow head.</summary>
        private readonly double curvedArrowHeadWidth;
        /// <summary>Width of the curved arrow stem at the arrow head.</summary>
        private readonly double curvedStemWidthTop;
        /// <summary>Width of the curved arrow stem at the bottom.</summary>
        private readonly double curvedStemWidthBottom;
        /// <summary>Offset between the tip of the curved arrow head and the center line of the stem.</summary>
        private readonly double curvedCenterTipOffs;
        /// <summary>Vertical offset of curvature start on stem for a curved arrow.</summary>
        private readonly double curvedCurveStartOffs;
        /// <summary>Horizontal offset of curvature start on stem for a curved arrow.</summary>
        private readonly double curvedBezierOffs;
        /// <summary>Vertical offset between straight arrow and curved arrow left or right.</summary>
        private readonly double curvedStraightOffset;

        /// <summary>Reference to the owner object of this overlay.</summary>
        private LaneElement owner;
        /// <summary>Distance between Connector[0].CenterP and overlay reference point (midPoint).</summary>
        private double refPointDistance;
        /// <summary>Angle difference between Connector[0].Angle and angle to the overlay reference point (midPoint).</summary>
        private double refPointAngleDelta;
        /// <summary>Angle difference between Connector[0].Angle and the direction of the overlay at the reference point (midPoint).</summary>
        private double directionAngleDelta;
        /// <summary>Middle point of the overlay used as reference point.</summary>
        private PointF midPoint;
        /// <summary>Start point of the overlay on the center line, for instance the tip of the straight arrow.</summary>
        private PointF startPoint;
        /// <summary>End point of the overlay on the center line, for instance the stem bottom of the straight arrow.</summary>
        private PointF endPoint;
        /// <summary>Reference point for mouse movement used for to calculate the movement delta.</summary>
        private PointF moveRefPoint;
        /// <summary>Length of the overlay.</summary>
        private double length;
        /// <summary>Width of the overlay.</summary>
        private double width;
        /// <summary>Current angle between Connector[0].CenterP and the midPoint in the drawing.</summary>
        private double midPointAngle;
        /// <summary>Current angle between Connector[0].CenterP and the overlay direction in the drawing.</summary>
        private double directionAngle;

        /// <summary>Refernce to the SegmClassDef object to be used with this instance.</summary>
        private SegmClassDef segmClassDef;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Type of this overlay.</summary>
        public readonly OverlayType OverlayType;
        /// <summary>Normally false to use the object Color in the Draw method. When set to true, the ClassCodeColor is used instead.</summary>
        public ColorMode ColorMode;
        #endregion Public Fields


        #region Constructor
        /// <summary>
        /// Creates an instance of the Overlay class from the given parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="OverlayType">Type of this overlay.</param>
        public Overlay(AppSettings AppSettings, OverlayType OverlayType) : this(AppSettings, OverlayType, null) { }

        /// <summary>
        /// Creates an instance of the Overlay class from the given parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="OverlayType">Type of this overlay.</param>
        /// <param name="Owner">Reference to the owner object of this overlay.</param>
        public Overlay(AppSettings AppSettings, OverlayType OverlayType, LaneElement Owner)
        {
            this.OverlayType = OverlayType;
            this.ColorMode = ColorMode.ImageColor;
            this.segmClassDef = SegmClassDefs.GetSegmClassDef(OverlayType);

            MaxArrowOverlayWidth = AppSettings.MaxArrowOverlayWidth;
            MaxArrowOverlayLength = AppSettings.MaxArrowOverlayLength;
            MinArrowOverlayStep = AppSettings.MinArrowOverlayStep;
            Color = AppSettings.ArrowOverlayColor;
            OutlineColor = AppSettings.OverlayOutlineColor;
            OutlineLineWidth = AppSettings.OverlayOutlineLineWidth;
            OverlayFontSize = AppSettings.OverlayFontSize;

            straightArrowHeadLength = MaxArrowOverlayLength / 3;
            straightArrowHeadRecess = MaxArrowOverlayLength / 20;
            straightTotalLength = MaxArrowOverlayLength / 1.3;
            straightArrowWidth = MaxArrowOverlayWidth / 4;
            straightStemWidthTop = MaxArrowOverlayWidth / 12;
            straightStemWidthBottom = MaxArrowOverlayWidth / 12;

            curvedArrowHeadLength = MaxArrowOverlayLength / 4.5;
            curvedArrowHeadTipOffs = MaxArrowOverlayLength / 20;
            curvedArrowHeadRecess = MaxArrowOverlayLength / 20;
            curvedArrowTotalLength = MaxArrowOverlayLength / 1.3;
            curvedArrowHeadWidth = MaxArrowOverlayWidth / 4;
            curvedStemWidthTop = MaxArrowOverlayWidth / 12;
            curvedStemWidthBottom = MaxArrowOverlayWidth / 12;
            curvedCenterTipOffs = MaxArrowOverlayWidth / 1.9;
            curvedCurveStartOffs = curvedArrowTotalLength / 4; //20;
            curvedBezierOffs = MaxArrowOverlayWidth / 4; //4;
            curvedStraightOffset = MaxArrowOverlayLength / 2.8;

            startPoint = new PointF(0, 0);
            midPoint = new PointF(0, 0);
            endPoint = new PointF(0, 0);
            moveRefPoint = new Point(0, 0);
            refPointDistance = MaxArrowOverlayLength;
            refPointAngleDelta = Utils.RIGHT_ANGLE_RADIAN;
            directionAngleDelta = Utils.RIGHT_ANGLE_RADIAN;
            if (OverlayType == OverlayType.ParkingSign)
            {
                length = 30;
                width = 20;
            }
            else if (OverlayType == OverlayType.ViewPoint)
            {
                length = 13;
                width = 9;
                MinArrowOverlayStep = 5;
            }
            else
            {
                length = straightTotalLength;
                width = straightArrowWidth;
                if ((OverlayType == OverlayType.ArrowStraightLeft) || (OverlayType == OverlayType.ArrowStraightRight))
                    length += curvedStraightOffset;
            }
            this.Owner = Owner;
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Set the midPoint to the passed value and update startPoint and endPoint.
        /// </summary>
        /// <param name="P">Corrdinates to set the midPoint to.</param>
        private void SetMidPoint(PointF P)
        {
            midPoint = P; 
            startPoint = Utils.GetPoint(midPoint, directionAngle, -length / 2);
            endPoint = Utils.GetPoint(midPoint, directionAngle, length / 2);
        }

        /// <summary>
        /// Event handler to handle connector change events of the reference connector as a result of movements or rotation to update angles and call SetMidPoint.
        /// </summary>
        /// <param name="sender">Reference to the connector that sent the event.</param>
        private void RefConnector_Changed(Connector sender)
        {
            if (RefConnector == null) return;
            midPointAngle = RefConnector.Angle + refPointAngleDelta;
            directionAngle = RefConnector.Angle + directionAngleDelta;
            SetMidPoint(Utils.GetPoint(RefConnector.CenterP, midPointAngle, refPointDistance));
        }

        /// <summary>
        /// Calculate and return the scaled outline polygon of the straight arrow overlay.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled outline polygon of the straight arrow overlay.</returns>
        private PointF[] GetStraightArrowPolygon(SizeF ScaleFactor)
        {
            PointF p1 = Utils.GetPoint(startPoint, directionAngle, straightArrowHeadLength);
            PointF p2 = Utils.GetPoint(p1, directionAngle, -straightArrowHeadRecess);
            //PointF p3 = Utils.GetPoint(startPoint, directionAngle, straightTotalLength);

            PointF[] poly = new PointF[8];
            poly[0] = startPoint;
            poly[1] = Utils.GetPoint(p1, directionAngle + Utils.RIGHT_ANGLE_RADIAN, straightArrowWidth);
            poly[2] = Utils.GetPoint(p2, directionAngle + Utils.RIGHT_ANGLE_RADIAN, straightStemWidthTop);
            poly[3] = Utils.GetPoint(endPoint, directionAngle + Utils.RIGHT_ANGLE_RADIAN, straightStemWidthBottom);
            poly[4] = Utils.GetPoint(endPoint, directionAngle + Utils.RIGHT_ANGLE_RADIAN, -straightStemWidthBottom);
            poly[5] = Utils.GetPoint(p2, directionAngle + Utils.RIGHT_ANGLE_RADIAN, -straightStemWidthTop);
            poly[6] = Utils.GetPoint(p1, directionAngle + Utils.RIGHT_ANGLE_RADIAN, -straightArrowWidth);
            poly[7] = startPoint;

            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Calculate and return the scaled outline polygon of the curved arrow overlay. This polygon does not render the curves at all and cannot be used for drawing.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="Left">True, if the arrow points to the left side or false to point to the right.</param>
        /// <param name="Offset">Vertical offset to shift the arrow along the stem center line.</param>
        /// <returns>Scaled outline polygon of the curved arrow overlay.</returns>
        private PointF[] GetCurvedArrowPolygon(SizeF ScaleFactor, bool Left, double Offset)
        {
            double angle = Left ? directionAngle - Utils.RIGHT_ANGLE_RADIAN : directionAngle + Utils.RIGHT_ANGLE_RADIAN;

            PointF p0 = Utils.GetPoint(startPoint, directionAngle, Offset);
            PointF p1 = Utils.GetPoint(p0, directionAngle, curvedArrowHeadWidth - curvedStemWidthTop);
            PointF p2 = Utils.GetPoint(p1, directionAngle, curvedArrowHeadTipOffs);
            PointF p3 = Utils.GetPoint(p1, directionAngle, 2 * curvedStemWidthTop);
            PointF p4 = Utils.GetPoint(p0, directionAngle, 2 * curvedArrowHeadWidth);
            PointF p5 = Utils.GetPoint(p4, directionAngle, curvedCurveStartOffs);
            PointF p6 = Utils.GetPoint(p0, directionAngle, curvedArrowTotalLength);

            PointF p10 = Utils.GetPoint(p1, angle, curvedCenterTipOffs - curvedArrowHeadLength + curvedArrowHeadRecess);
            PointF p11 = Utils.GetPoint(p0, angle, curvedCenterTipOffs - curvedArrowHeadLength);
            PointF p12 = Utils.GetPoint(p2, angle, curvedCenterTipOffs);
            PointF p13 = Utils.GetPoint(p4, angle, curvedCenterTipOffs - curvedArrowHeadLength);
            PointF p14 = Utils.GetPoint(p3, angle, curvedCenterTipOffs - curvedArrowHeadLength + curvedArrowHeadRecess);

            PointF p30 = Utils.GetPoint(p14, angle, -curvedBezierOffs);
            PointF p31 = Utils.GetPoint(p10, angle, -curvedBezierOffs);
            PointF p40 = Utils.GetPoint(p4, angle, curvedStemWidthTop);
            PointF p41 = Utils.GetPoint(p4, angle, -curvedStemWidthTop);

            PointF p50 = Utils.GetPoint(p5, angle, curvedStemWidthTop);
            PointF p51 = Utils.GetPoint(p5, angle, -curvedStemWidthTop);
            PointF p60 = Utils.GetPoint(p6, angle, curvedStemWidthBottom);
            PointF p61 = Utils.GetPoint(p6, angle, -curvedStemWidthBottom);

            //                              0    1    2    3    4    5    6    7    8    9    10   11   12   13
            PointF[] poly = new PointF[] { p14, p13, p12, p11, p10, p31, p41, p51, p61, p60, p50, p40, p30, p14 };

            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Calculate and return the scaled outline GraphicsPath of the curved arrow overlay, using GetCurvedArrowPolygon as base.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="Left">True, if the arrow points to the left side or false to point to the right.</param>
        /// <param name="Offset">Vertical offset to shift the arrow along the stem center line.</param>
        /// <returns>Scaled outline GraphicsPath of the curved arrow overlay.</returns>
        private GraphicsPath GetCurvedArrowPath(SizeF ScaleFactor, bool Left, double Offset)
        {
            PointF[] poly = GetCurvedArrowPolygon(ScaleFactor, Left, Offset);

            GraphicsPath path = new GraphicsPath();

            path.AddLine(poly[0], poly[1]);  
            path.AddLine(poly[1], poly[2]);
            path.AddLine(poly[2], poly[3]);
            path.AddLine(poly[3], poly[4]);
            path.AddBezier(poly[4], poly[5], poly[6], poly[7]);
            path.AddLine(poly[7], poly[8]);
            path.AddLine(poly[8], poly[9]);
            path.AddLine(poly[9], poly[10]);
            path.AddBezier(poly[10], poly[11], poly[12], poly[13]);

            return path;
        }

        /// <summary>
        /// Calculate and return the scaled outline polygon of the merge arrow overlay.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="Left">True, if the arrow points to the left side or false to point to the right.</param>
        /// <returns>Scaled outline polygon of the merge arrow overlay.</returns>
        private PointF[] GetMergeArrowPolygon(SizeF ScaleFactor, bool Left)
        {
            double angle = Left ? directionAngle + Utils.RIGHT_ANGLE_RADIAN : directionAngle - Utils.RIGHT_ANGLE_RADIAN;
            PointF[] poly = new PointF[8];
            PointF p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength/ -2.8);
            poly[0] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / -3.1); // -4.4);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / -4.5); // -7);
            poly[1] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / 10); // 5);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / -5); // -8.7);
            poly[2] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / -10);   //58);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / 5); // 3.0);
            poly[3] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / 5); // 3.3);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / 3); // 2.7);
            poly[4] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / 6); // 6.7);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / -8 ); // -13.7);
            poly[5] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / -5.5); // -7.3);

            p = Utils.GetPoint(midPoint, directionAngle, MaxArrowOverlayLength / 40); // 68);
            poly[6] = Utils.GetPoint(p, angle, MaxArrowOverlayWidth / -3.8);

            poly[7] = poly[0];

            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Calculate and return the scaled rectangular outline polygon of the parking sign overlay.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled outline polygon of the parking sign overlay.</returns>
        private PointF[] GetParkingSignOutline(SizeF ScaleFactor)
        {
            PointF p0 = Utils.GetPoint(midPoint, directionAngle, -length / 2);
            PointF[] poly = new PointF[5];
            poly[0] = Utils.GetPoint(p0, directionAngle - Utils.RIGHT_ANGLE_RADIAN, -width / 2);
            poly[1] = Utils.GetPoint(p0, directionAngle - Utils.RIGHT_ANGLE_RADIAN, +width / 2);
            poly[2] = Utils.GetPoint(poly[1], directionAngle, length);
            poly[3] = Utils.GetPoint(poly[2], directionAngle - Utils.RIGHT_ANGLE_RADIAN, -width);
            poly[4] = poly[0];

            return Utils.Scale(poly, ScaleFactor);
        }


        /// <summary>
        /// Calculate and return the scaled outline polygon of the view point overlay.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled outline polygon of the view point overlay.</returns>
        private PointF[] GetViewPointPolygon(SizeF ScaleFactor)
        {
            PointF p1 = Utils.GetPoint(midPoint, directionAngle, -length/2);
            PointF p2 = Utils.GetPoint(midPoint, directionAngle, length/2);

            PointF[] poly = new PointF[4];
            poly[0] = p1;
            poly[1] = Utils.GetPoint(p2, directionAngle + Utils.RIGHT_ANGLE_RADIAN, width/2);
            poly[2] = Utils.GetPoint(p2, directionAngle + Utils.RIGHT_ANGLE_RADIAN, -width/2);
            poly[3] = p1;
   
            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Returns the color to be used for drawing depending on the ColorMode.
        /// </summary>
        /// <returns>Color for drawing.</returns>
        private Color GetDrawColor()
        {
            switch (ColorMode)
            {
                case ColorMode.ImageColor: return Color;
                case ColorMode.ClassCode: return SegmClassDef.ClassCodeColor;
                case ColorMode.ClassColor: return SegmClassDef.DrawColor;
            }
            return Color;
        }

        #endregion Private Methods


        #region Public Methods
        /// <summary>
        /// Initialize the move of the overlay by storing the current reference point for move calculations.
        /// </summary>
        /// <param name="P">Coordinates of the mouse pointer inside the overlay.</param>
        public void StartMove(PointF P)
        {
            moveRefPoint = P;
        }

        /// <summary>
        /// Move the overlay to the new mouse pointer location relativ to the moveRefPoint.
        /// Since the target of the movement is not clear, remove the overlay from the current owner.
        /// </summary>
        /// <param name="NewLocation">New location in related to the moveRefPoint.</param>
        public void Move(PointF NewLocation)
        {
            Owner = null;
            SetMidPoint(midPoint + new SizeF(NewLocation.X - moveRefPoint.X, NewLocation.Y - moveRefPoint.Y));
            moveRefPoint = NewLocation;
        }

        /// <summary>
        /// Finalize the movement by assigning the new owner, which allows to center to its lane center and adjust the direction correctly.
        /// </summary>
        /// <param name="NewOwner">Reference to the new owner of the overlay.</param>
        public void EndMove(LaneElement NewOwner)
        {
            NewOwner.AdjustCenter(ref midPoint, ref directionAngle, ref refPointDistance, MinArrowOverlayStep);

            midPointAngle = Utils.GetAngle(NewOwner.Connectors[0].CenterP, midPoint);
            refPointAngleDelta = Utils.LimitRadian(midPointAngle - NewOwner.Connectors[0].Angle);
            directionAngleDelta = Utils.LimitRadian(directionAngle - NewOwner.Connectors[0].Angle);

            Owner = NewOwner;
        }

        /// <summary>
        /// Rotate the overlay by the given amount.
        /// </summary>
        /// <param name="DeltaAngle">Angle step in Degrees.</param>
        public void Rotate(double DeltaAngle)
        {
            DirectionAngleDelta = Utils.LimitRadian(directionAngleDelta + Utils.ToRadian(DeltaAngle));
            if (RefConnector == null)
            {
                directionAngle = Utils.LimitRadian(directionAngle + Utils.ToRadian(DeltaAngle));
                SetMidPoint(midPoint);
            }
        }

        /// <summary>
        /// Draws the scaled overlay to the Graphics object according to the drawing mode.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            Color drawColor = GetDrawColor();

            switch (OverlayType)
            {
                case OverlayType.ArrowLeftOnly:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, true, 0));
                    else if (DrawMode == DrawMode.Overlay)
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, true, 0));
                    break;

                case OverlayType.ArrowLeftRight:
                    if (DrawMode == DrawMode.Outline)
                    {
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, true, 0));
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, false, 0));
                    }
                    else if (DrawMode == DrawMode.Overlay)
                    {
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, true, 0));
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, false, 0));
                    }
                    break;

                case OverlayType.ArrowRightOnly:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, false, 0));
                    else if (DrawMode == DrawMode.Overlay)
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, false, 0));
                    break;

                case OverlayType.ArrowStraightLeft:
                    if (DrawMode == DrawMode.Outline)
                    {
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, true, curvedStraightOffset));
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetStraightArrowPolygon(ScaleFactor));
                    }
                    else if (DrawMode == DrawMode.Overlay)
                    {
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, true, curvedStraightOffset));
                        grfx.FillPolygon(new SolidBrush(drawColor), GetStraightArrowPolygon(ScaleFactor));
                    }
                    break;

                case OverlayType.ArrowStraightOnly:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetStraightArrowPolygon(ScaleFactor));
                    else if (DrawMode == DrawMode.Overlay)
                        grfx.FillPolygon(new SolidBrush(drawColor), GetStraightArrowPolygon(ScaleFactor));
                    break;

                case OverlayType.ArrowStraightRight:
                    if (DrawMode == DrawMode.Outline)
                    {
                        grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetCurvedArrowPath(ScaleFactor, false, curvedStraightOffset));
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetStraightArrowPolygon(ScaleFactor));
                    }
                    else if (DrawMode == DrawMode.Overlay)
                    {
                        grfx.FillPath(new SolidBrush(drawColor), GetCurvedArrowPath(ScaleFactor, false, curvedStraightOffset));
                        grfx.FillPolygon(new SolidBrush(drawColor), GetStraightArrowPolygon(ScaleFactor));
                    }
                    break;

                case OverlayType.ArrowMergeLeft:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetMergeArrowPolygon(ScaleFactor, true));
                    else if (DrawMode == DrawMode.Overlay)
                        grfx.FillPolygon(new SolidBrush(drawColor), GetMergeArrowPolygon(ScaleFactor, true));
                    break;

                case OverlayType.ArrowMergeRight:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetMergeArrowPolygon(ScaleFactor, false));
                    else if (DrawMode == DrawMode.Overlay)
                        grfx.FillPolygon(new SolidBrush(drawColor), GetMergeArrowPolygon(ScaleFactor, false));
                    break;

                case OverlayType.ParkingSign:
                    {
                        if (DrawMode == DrawMode.Outline)
                            grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetParkingSignOutline(ScaleFactor));

                        else if (DrawMode == DrawMode.Overlay)
                        {
                            string txt = "P";
                            float fontSize = (float)OverlayFontSize * ScaleFactor.Height;
                            Font font = new Font("Impact", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                            if (ColorMode == ColorMode.ImageColor)
                                Utils.DrawText(grfx, txt, Utils.Scale(midPoint, ScaleFactor), directionAngle + Utils.RIGHT_ANGLE_RADIAN, font, drawColor);
                            else
                            {

                                SizeF textSize = grfx.MeasureString(txt, font);
                                double w = textSize.Width * 0.58;
                                double h1 = textSize.Height * 0.70;
                                double h2 = textSize.Height * 0.62;
                                double angle = directionAngle + Utils.RIGHT_ANGLE_RADIAN;
                                PointF[] pp = new PointF[5];
                                pp[0] = Utils.GetPoint(midPoint, angle, -w / 1.9);
                                pp[0] = Utils.GetPoint(pp[0], angle - Utils.RIGHT_ANGLE_RADIAN, -h1 / 2);
                                pp[1] = Utils.GetPoint(pp[0], angle - Utils.RIGHT_ANGLE_RADIAN, h2);
                                pp[2] = Utils.GetPoint(pp[1], angle, w);
                                pp[3] = Utils.GetPoint(pp[0], angle, w);
                                pp[4] = pp[0];
                                grfx.FillPolygon(new SolidBrush(drawColor), pp);

//                                Utils.DrawText(grfx, txt, Utils.Scale(midPoint, ScaleFactor), directionAngle + Utils.RIGHT_ANGLE_RADIAN, font, Color.Blue);
                            }
                        }
                    }
                    break;

                case OverlayType.ViewPoint:
                    if (DrawMode == DrawMode.Outline)
                        grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetViewPointPolygon(ScaleFactor));
                    else if ((DrawMode == DrawMode.ViewPoint) && (ColorMode == ColorMode.ImageColor))
                    {
                        grfx.FillPolygon(new SolidBrush(Color), GetViewPointPolygon(ScaleFactor));
                        grfx.DrawPolygon(new Pen(Color.Red,2), GetViewPointPolygon(ScaleFactor));
                    }
                    break;
            }
        }


        /// <summary>
        /// Returns true, if the given point is inside of this object area. 
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <returns>True, if inside.</returns>
        public bool IsInside(PointF P)
        {
            SizeF scaleFactor = new SizeF(1, 1);
            switch (OverlayType)
            {
                case OverlayType.ArrowLeftOnly:
                    return Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, true, 0));

                case OverlayType.ArrowLeftRight:
                    return Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, true, 0)) || Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, false, 0));

                case OverlayType.ArrowRightOnly:
                    return Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, false, 0));

                case OverlayType.ArrowStraightLeft:
                    return Utils.IsInPolygon(P, GetStraightArrowPolygon(scaleFactor)) || Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, true, curvedStraightOffset));
 
                case OverlayType.ArrowStraightOnly:
                    return Utils.IsInPolygon(P, GetStraightArrowPolygon(scaleFactor));

                case OverlayType.ArrowStraightRight:
                    return Utils.IsInPolygon(P, GetStraightArrowPolygon(scaleFactor)) || Utils.IsInPolygon(P, GetCurvedArrowPolygon(scaleFactor, false, curvedStraightOffset));

                case OverlayType.ArrowMergeLeft:
                    return Utils.IsInPolygon(P, GetMergeArrowPolygon(scaleFactor, true));

                case OverlayType.ArrowMergeRight:
                    return Utils.IsInPolygon(P, GetMergeArrowPolygon(scaleFactor, false));

                case OverlayType.ParkingSign:
                    return Utils.IsInPolygon(P, GetParkingSignOutline(scaleFactor));

                case OverlayType.ViewPoint:
                    return Utils.IsInPolygon(P, GetViewPointPolygon(scaleFactor));
            }

            return false;
        }



        #region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));

            nodeItem.AppendChild(Doc.CreateElement("overlay_type")).AppendChild(Doc.CreateTextNode(OverlayType.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("ref_point_distance")).AppendChild(Doc.CreateTextNode(RefPointDistance.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("ref_point_angle_delta")).AppendChild(Doc.CreateTextNode(RefPointAngleDelta.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("direction_angle_delta")).AppendChild(Doc.CreateTextNode(DirectionAngleDelta.ToString()));
        }

        /// <summary>
        /// Reads the contents for one StreetElement class instance from an XML document at the specified node and returns the StreetElement object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <param name="AppSettings">Reference to the application settings object.</param>
        /// <param name="Owner">Reference to the owner object of this overlay.</param>
        /// <returns>Reference to the StreetElement created from the XML file contents.</returns>
        public static Overlay LoadFromXml(XmlDocument Doc, XmlNode nodeItem, AppSettings AppSettings)
        {
            try
            {
                OverlayType overlayType = (OverlayType)Enum.Parse(typeof(OverlayType), nodeItem.SelectSingleNode("overlay_type").InnerText);

                double refPointDistance = Convert.ToDouble(nodeItem.SelectSingleNode("ref_point_distance").InnerText);
                double refPointAngleDelta = Convert.ToDouble(nodeItem.SelectSingleNode("ref_point_angle_delta").InnerText);
                double directionAngleDelta = Convert.ToDouble(nodeItem.SelectSingleNode("direction_angle_delta").InnerText);

                Overlay overlay = new Overlay(AppSettings, overlayType);
                overlay.RefPointDistance = refPointDistance;
                overlay.RefPointAngleDelta = refPointAngleDelta;
                overlay.DirectionAngleDelta = directionAngleDelta;
                return overlay;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading XML file:" + ex.Message);
            }
            return null;
        }
        #endregion XML File Support

        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the reference to the owner object. Setting the owner to a new owner will disconnect from the previous owner, 
        /// removing itself from its overlay list and then connect to the new one by adding itself to that overlay list.
        /// </summary>
        public LaneElement Owner
        {
            get { return owner;  }
            set
            {
                if (owner != value)
                {
                    if (owner != null)
                    {
                        if (RefConnector != null)
                            RefConnector.Changed -= RefConnector_Changed;
                        owner.Overlays.Remove(this);
                    }

                    owner = value;

                    if (owner != null)
                    {
                        owner.Overlays.Add(this);
                        RefConnector_Changed(RefConnector);
                        if (RefConnector != null)
                            RefConnector.Changed += RefConnector_Changed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the reference connector from the owner or null if there is no owner object.
        /// </summary>
        public Connector RefConnector
        {
            get 
            {
                if (owner != null)
                    return owner.Connectors[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the RefPointDistance, the distance between the RefConnector.CenterP and the overlay reference point (midPoint).
        /// </summary>
        public double RefPointDistance
        {
            get { return refPointDistance; }
            set
            {
                if (refPointDistance != value)
                {
                    refPointDistance = value;
                    RefConnector_Changed(RefConnector);
                }
            }
        }

        /// <summary>
        /// Gets or sets the RefPointAngleDelta, the angle difference between RefConnector.Angle and the overlay reference point (midPoint).
        /// </summary>
        public double RefPointAngleDelta
        {
            get { return Utils.ToDegree(refPointAngleDelta); }
            set
            {
                refPointAngleDelta = Utils.ToRadian(value);
                RefConnector_Changed(RefConnector);
            }
        }

        /// <summary>
        /// Gets or sets the DirectionAngleDelta, the angle difference between RefConnector.Angle and the overlay direction at the reference point (midPoint).
        /// </summary>
        public double DirectionAngleDelta
        {
            get { return Utils.ToDegree(directionAngleDelta);  }
            set
            {
                directionAngleDelta = Utils.ToRadian(value);
                RefConnector_Changed(RefConnector);
            }
        }

        /// <summary>
        /// Gets the tefernce to the SegmClassDef object to be used with this instance.
        /// </summary>
        public SegmClassDef SegmClassDef
        {
            get { return segmClassDef;  }
        }

        /// <summary>
        /// Gets the current middle point of the overlay.</summary>
        public PointF MidPoint
        {
            get { return midPoint; }
        }

        /// <summary>
        /// Gets the current start point of the overlay on the center line, for instance the tip of the straight arrow.
        /// </summary>
        public PointF StartPoint
        {
            get { return startPoint; }
        }

        /// <summary>
        /// Gets the current end point of the overlay on the center line, for instance the stem bottom of the straight arrow.
        /// </summary>
        public PointF EndPoint
        {
            get { return endPoint; }
        }

        /// <summary>
        /// Gets the current direction angle in Radian.
        /// </summary>
        public double DirectionAngle
        {
            get { return directionAngle; }
        }
        #endregion Public Properties

    }
}
