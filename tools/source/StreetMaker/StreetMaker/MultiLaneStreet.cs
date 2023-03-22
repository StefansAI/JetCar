// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

// Activate to enable Debug.WriteLine for debugging resizing
//#define DEBUG_WRITE_LINES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace StreetMaker
{

    /// <summary>
    /// Class to manage multiple lanes in both directions plus a possible center lane in a number of different shapes and configurations as defined in the StreetTyp enumeration.
    /// </summary>
    public class MultiLaneStreet : StreetElement
    {
        #region Public Constants
        /// <summary>String to identify this class in the XML file.</summary>
        public const string XML_TYPE_STR = "multi_lane_street";
        #endregion Public Constants

        #region Public Readonly Fields
        /// <summary>StreetType of this street.</summary>
        public readonly StreetType StreetType;
        /// <summary>Number of Lanes on the right side, or in our direction.</summary>
        public readonly int LaneCountRight;
        /// <summary>Number of Lanes in the center used as birectional turn lanes.</summary>
        public readonly int LaneCountCenter;
        /// <summary>Number of Lanes on the left side, or in opposite direction.</summary>
        public readonly int LaneCountLeft;
        /// <summary>LineType of the right border line.</summary>
        public readonly LineType RightBorderLine;
        /// <summary>LineType of the right lines between the lanes.</summary>
        public readonly LineType RightLaneLine;
        /// <summary>LineType of the center divider lines between the opposite lanes or a center lane.</summary>
        public readonly LineType DividerLine;
        /// <summary>LineType of the center divider lines between the center lane and the left lanes.</summary>
        public readonly LineType DividerLine2;
        /// <summary>LineType of the left lines between the lanes.</summary>
        public readonly LineType LeftLaneLine;
        /// <summary>LineType of the left border line.</summary>
        public readonly LineType LeftBorderLine;
        /// <summary>Total lane count.</summary>
        public readonly int LaneCount;
        /// <summary>Type of off-ramp or on-ramp at the most right lane, if any.</summary>
        public readonly RampType RampType;
        #endregion Public Readonly Fields

        #region Private Fields
        /// <summary>Inner radius of the most inner curved lane if applies.</summary>
        private double innerRadius;
        /// <summary>Ramp inner radius of the off- or on-ramp if applies.</summary>
        private double rampRadius;
        /// <summary>Ramp curve angle of the off- or on-ramp if applies.</summary>
        private double rampCurveAngle;

        /// <summary>Width of stop lines in mm.</summary>
        private double stopLineWidth;
        /// <summary>Offset of the stop lines from the crossing lane limits in mm.</summary>
        private double stopLineOffset;
        /// <summary>Color of the stop line.</summary>
        private Color stopLineColor;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the MultiLaneStreet class according to the given parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="StreetType">StreetType of this street.</param>
        /// <param name="LaneCountRight">Number of Lanes on the right side, or in our direction.</param>
        /// <param name="LaneCountCenter">Number of Lanes in the center used as birectional turn lanes.</param>
        /// <param name="LaneCountLeft">Number of Lanes on the left side, or in opposite direction.</param>
        /// <param name="DividerLine">LineType of the center divider lines between the opposite lanes or a center lane.</param>
        public MultiLaneStreet(AppSettings AppSettings, StreetType StreetType, int LaneCountRight, int LaneCountCenter, int LaneCountLeft, LineType DividerLine) : 
            this(AppSettings, StreetType, LaneCountRight, LaneCountCenter, LaneCountLeft, LineType.ShoulderLine, LineType.SingleWhiteDashed, DividerLine, DividerLine, 
                 LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius) { }

        /// <summary>
        /// Creates an instance of the MultiLaneStreet class according to the given parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="StreetType">StreetType of this street.</param>
        /// <param name="LaneCountRight">Number of Lanes on the right side, or in our direction.</param>
        /// <param name="LaneCountCenter">Number of Lanes in the center used as birectional turn lanes.</param>
        /// <param name="LaneCountLeft">Number of Lanes on the left side, or in opposite direction.</param>
        /// <param name="DividerLine">LineType of the center divider lines between the opposite lanes or a center lane.</param>
        /// <param name="DividerLine2">LineType of the center divider lines between the center lane and the left lanes.</param>
        /// <param name="LeftLaneLine">LineType of the left lines between the lanes.</param>
        /// <param name="LeftBorderLine">LineType of the left border line.</param>
        /// <param name="InnerRadius">Inner radius of the most inner curved lane if applies.</param>
        public MultiLaneStreet(AppSettings AppSettings, StreetType StreetType, int LaneCountRight, int LaneCountCenter, int LaneCountLeft, LineType RightBorderLine, LineType RightLaneLine,
                       LineType DividerLine, LineType DividerLine2, LineType LeftLaneLine, LineType LeftBorderLine, double InnerRadius) :
            this(AppSettings, StreetType, LaneCountRight, LaneCountCenter, LaneCountLeft, RightBorderLine, RightLaneLine,
                       DividerLine, DividerLine2, LeftLaneLine, LeftBorderLine, InnerRadius, RampType.None, AppSettings.MinInnerRadius, AppSettings.DefaultRampCurveAngle) {  }

        /// <summary>
        /// Creates an instance of the MultiLaneStreet class according to the given parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="StreetType">StreetType of this street.</param>
        /// <param name="LaneCountRight">Number of Lanes on the right side, or in our direction.</param>
        /// <param name="LaneCountCenter">Number of Lanes in the center used as birectional turn lanes.</param>
        /// <param name="LaneCountLeft">Number of Lanes on the left side, or in opposite direction.</param>
        /// <param name="DividerLine">LineType of the center divider lines between the opposite lanes or a center lane.</param>
        /// <param name="DividerLine2">LineType of the center divider lines between the center lane and the left lanes.</param>
        /// <param name="LeftLaneLine">LineType of the left lines between the lanes.</param>
        /// <param name="LeftBorderLine">LineType of the left border line.</param>
        /// <param name="InnerRadius">Inner radius of the most inner curved lane if applies.</param>
        /// <param name="RampType">Type of off-ramp or on-ramp at the most right lane, if any.</param>
        /// <param name="RampRadius">Ramp inner radius of the off- or on-ramp if applies.</param>
        /// <param name="RampCurveAngle">Ramp curve angle of the off- or on-ramp if applies.</param>
        public MultiLaneStreet(AppSettings AppSettings, StreetType StreetType, int LaneCountRight, int LaneCountCenter, int LaneCountLeft, LineType RightBorderLine, LineType RightLaneLine,
                               LineType DividerLine, LineType DividerLine2, LineType LeftLaneLine, LineType LeftBorderLine, double InnerRadius, RampType RampType, double RampRadius, double RampCurveAngle) : base(AppSettings)
        {
            if ((LaneCountRight > AppSettings.MaxLaneCountLeftRight) || (LaneCountCenter > AppSettings.MaxLaneCountCenter) || (LaneCountLeft > AppSettings.MaxLaneCountLeftRight))
                throw new Exception("Invalid Parameter creating MultiLaneStreet!");

            if ((RampType != RampType.None) && (StreetType != StreetType.Straight))
                throw new Exception("Invalid Parameter Combination creating MultiLaneStreet!");

            this.StreetType = StreetType;
            this.LaneCountRight = LaneCountRight;
            this.LaneCountCenter = LaneCountCenter;
            this.LaneCountLeft = LaneCountLeft;
            this.RightBorderLine = RightBorderLine;
            this.RightLaneLine = RightLaneLine;
            this.DividerLine = DividerLine;
            this.DividerLine2 = DividerLine2;
            this.LeftLaneLine = LeftLaneLine;
            this.LeftBorderLine = LeftBorderLine;
            this.LaneCount = LaneCountRight + LaneCountCenter + LaneCountLeft + (RampType == RampType.None ? 0 : 1);
            this.innerRadius = InnerRadius;
            this.RampType = RampType;
            this.rampRadius = RampRadius;
            this.rampCurveAngle = RampCurveAngle;

            stopLineWidth = AppSettings.StopLineWidth;
            stopLineOffset = AppSettings.StopLineOffset;
            stopLineColor = AppSettings.StopLineColor;

            lanes = new LaneElement[LaneCount];
            ShapeType shapeRight = ShapeType.Straight;
            ShapeType shapeCenter = ShapeType.Straight;
            ShapeType shapeLeft = ShapeType.Straight;
            double curveAngle = AppSettings.DefaultCurveAngle;
            double startRadius = InnerRadius; // AppSettings.MinInnerRadius;
            double radiusStep = AppSettings.LaneWidth;
            double sOffsetRight = 0;
            double sOffsetStepRight = 0;
            double sOffsetLeft = 0;
            double sOffsetStepLeft = 0;
            double innerRadius;
            double x = 0;
            double y = LaneCount * AppSettings.LaneWidth;
            double yStepRight = AppSettings.LaneWidth;
            double yStepCenterPre = 0;
            double yStepCenter = AppSettings.LaneWidth;
            double yStepCenterPost = 0;
            double yStepCenter0 = 0;
            double yStepLeft = AppSettings.LaneWidth;
            LineType lineTypeRightCenter = DividerLine;
            LineType lineTypeCenterRight = DividerLine;
            LineType lineTypeCenterLeft = DividerLine2;
            LineType lineTypeLeftCenter = DividerLine2;

            ConnectorMode connectorModeCenter00 = ConnectorMode.NoDir;
            ConnectorMode connectorModeCenter01 = ConnectorMode.NoDir;
            ConnectorMode connectorModeCenter10 = ConnectorMode.NoDir;
            ConnectorMode connectorModeCenter11 = ConnectorMode.NoDir;

            switch (StreetType)
            {
                case StreetType.Straight:
                    break;

                case StreetType.CurveRight:
                    shapeRight = ShapeType.Curve;
                    shapeCenter = ShapeType.Curve;
                    shapeLeft = ShapeType.Curve;
                    curveAngle = AppSettings.DefaultCurveAngle;
                    break;

                case StreetType.CurveLeft:
                    shapeRight = ShapeType.Curve;
                    shapeCenter = ShapeType.Curve;
                    shapeLeft = ShapeType.Curve;
                    curveAngle = -AppSettings.DefaultCurveAngle;
                    startRadius = InnerRadius + (LaneCount - 1) * AppSettings.MinInnerRadius;
                    radiusStep = -AppSettings.LaneWidth;
                    break;

                case StreetType.S_Right:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = AppSettings.LaneWidth / 2;
                    sOffsetLeft = sOffsetRight;
                    break;

                case StreetType.S_Left:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = -AppSettings.LaneWidth / 2;
                    sOffsetLeft = sOffsetRight;
                    break;

                case StreetType.Lane_Split_Right:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape; 
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = AppSettings.LaneWidth;
                    sOffsetLeft = 0;
                    connectorModeCenter00 = ConnectorMode.Hidden;
                    connectorModeCenter01 = ConnectorMode.Out;
                    connectorModeCenter10 = ConnectorMode.Hidden;
                    connectorModeCenter11 = ConnectorMode.In;
                    break;

                case StreetType.Lane_Union_Right:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape; 
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = -AppSettings.LaneWidth;
                    sOffsetLeft = 0;
                    connectorModeCenter00 = ConnectorMode.In;
                    connectorModeCenter01 = ConnectorMode.Hidden;
                    connectorModeCenter10 = ConnectorMode.Out;
                    connectorModeCenter11 = ConnectorMode.Hidden;
                    break;

                case StreetType.Lane_Split_Both:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = AppSettings.LaneWidth;
                    sOffsetLeft = 0;
                    connectorModeCenter00 = ConnectorMode.Hidden;
                    connectorModeCenter01 = ConnectorMode.Out;
                    connectorModeCenter10 = ConnectorMode.Hidden;
                    connectorModeCenter11 = ConnectorMode.In;
                    break;

                case StreetType.Lane_Union_Both:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = -AppSettings.LaneWidth;
                    sOffsetLeft = 0;
                    connectorModeCenter00 = ConnectorMode.In;
                    connectorModeCenter01 = ConnectorMode.Hidden;
                    connectorModeCenter10 = ConnectorMode.Out;
                    connectorModeCenter11 = ConnectorMode.Hidden;
                    break;

                case StreetType.Center_Split:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape; // ShapeType.Straight;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = AppSettings.LaneWidth / 2;
                    sOffsetLeft = -sOffsetRight;
                    yStepCenterPre = AppSettings.LaneWidth / 2;
                    yStepCenterPost = -AppSettings.LaneWidth / 2;
                    lineTypeCenterRight = LineType.None;
                    lineTypeCenterLeft = LineType.None;
                    break;

                case StreetType.Center_Union:
                    shapeRight = ShapeType.S_Shape;
                    shapeCenter = ShapeType.S_Shape; // ShapeType.Straight;
                    shapeLeft = ShapeType.S_Shape;
                    sOffsetRight = -AppSettings.LaneWidth / 2;
                    sOffsetLeft = -sOffsetRight;
                    yStepCenter0 = AppSettings.LaneWidth;
                    lineTypeCenterRight = LineType.None;
                    lineTypeCenterLeft = LineType.None;
                    break;
            }

            int i;
            for (i = 0; i < LaneCountRight; i++)
            {
                innerRadius = startRadius + i * radiusStep;
                LaneElement newLaneRight = new LaneElement(this, i, AppSettings, shapeRight, innerRadius, curveAngle, sOffsetRight);
                if (i == 0)
                    newLaneRight.RightLine = new LineElement(AppSettings, shapeRight, RightBorderLine);
                else
                    newLaneRight.RightLine = new LineElement(AppSettings, shapeRight, RightLaneLine);

                if (i < LaneCountRight - 1)
                    newLaneRight.LeftLine = new LineElement(AppSettings, shapeRight, RightLaneLine);
                else
                {
                    if (lineTypeRightCenter != LineType.None)
                        newLaneRight.LeftLine = new LineElement(AppSettings, shapeRight, lineTypeRightCenter);
                }
                newLaneRight.Connectors[0].CenterP = new PointF((float)x, (float)y);
                newLaneRight.Connectors[0].SetMode(AppSettings, ConnectorMode.In);
                newLaneRight.Connectors[1].SetMode(AppSettings, ConnectorMode.Out);
                lanes[i] = newLaneRight;

                if ((StreetType == StreetType.Lane_Split_Right) || (StreetType == StreetType.Lane_Split_Both))
                {
                    sOffsetRight = 0;
                    if (i == 0)
                    {
                        newLaneRight.LeftLine = null;
                        newLaneRight.Connectors[0].SetMode(AppSettings, ConnectorMode.Hidden);
                    }
                    else
                        y -= yStepRight;
                }
                else if ((StreetType == StreetType.Lane_Union_Right) || (StreetType == StreetType.Lane_Union_Both))
                {
                    sOffsetRight = 0;
                    if (i == 0)
                    {
                        newLaneRight.LeftLine = null;
                        newLaneRight.Connectors[1].SetMode(AppSettings, ConnectorMode.Hidden);
                    }
                    
                    y -= yStepRight;
                }
                else
                {
                    y -= yStepRight;
                    sOffsetRight += sOffsetStepRight;
                }
            }


            if (LaneCountCenter > 0)
            {
                y += yStepCenterPre;
                for (int j = 0; j < LaneCountCenter; j++, i++)
                {
                    innerRadius = startRadius + i * radiusStep;
                    LaneElement newLaneCenter = new LaneElement(this, i, AppSettings, shapeCenter, innerRadius, curveAngle, 0);

                    if (j == 0)
                    {
                        if (lineTypeCenterRight != LineType.None)
                            newLaneCenter.RightLine = new LineElement(AppSettings, shapeCenter, lineTypeCenterRight);

                        if (lineTypeCenterLeft != LineType.None)
                            newLaneCenter.LeftLine = new LineElement(AppSettings, shapeCenter, lineTypeCenterLeft);

                        newLaneCenter.Connectors[0].SetMode(AppSettings, connectorModeCenter00);
                        newLaneCenter.Connectors[1].SetMode(AppSettings, connectorModeCenter01);
                    }
                    else
                    {
                        if (lineTypeCenterRight != LineType.None)
                            newLaneCenter.LeftLine = new LineElement(AppSettings, shapeCenter, lineTypeCenterRight);

                        if (lineTypeCenterLeft != LineType.None)
                            newLaneCenter.RightLine = new LineElement(AppSettings, shapeCenter, lineTypeCenterLeft);

                        newLaneCenter.Connectors[0].SetMode(AppSettings, connectorModeCenter10);
                        newLaneCenter.Connectors[1].SetMode(AppSettings, connectorModeCenter11);
                    }
                    newLaneCenter.Connectors[0].CenterP = new PointF((float)x, (float)y);
                    lanes[i] = newLaneCenter;
                    y -= yStepCenter;
                }
                y -= yStepCenterPost;
            }
            else y -= yStepCenter0;


            for (int j=0; j < LaneCountLeft; j++,i++)
            {
                innerRadius = startRadius + i * radiusStep;
                LaneElement newLaneLeft = new LaneElement(this, i, AppSettings, shapeLeft, innerRadius, curveAngle, sOffsetLeft);
                if (j == 0)
                {
                    if (lineTypeLeftCenter != LineType.None)
                        newLaneLeft.RightLine = new LineElement(AppSettings, shapeLeft, lineTypeLeftCenter);
                }
                else
                    newLaneLeft.RightLine = new LineElement(AppSettings, shapeLeft, LeftLaneLine);

                if (j < LaneCountLeft - 1)
                    newLaneLeft.LeftLine = new LineElement(AppSettings, shapeLeft, LeftLaneLine);
                else
                    newLaneLeft.LeftLine = new LineElement(AppSettings, shapeLeft, LeftBorderLine);

                newLaneLeft.Connectors[0].CenterP = new PointF((float)x, (float)y);
                newLaneLeft.Connectors[0].SetMode(AppSettings, ConnectorMode.Out);
                newLaneLeft.Connectors[1].SetMode(AppSettings, ConnectorMode.In);
                lanes[i] = newLaneLeft;

                if (StreetType == StreetType.Lane_Split_Both) 
                {
                    if (j < LaneCountLeft - 2)
                        y -= yStepLeft;
                    else
                        sOffsetLeft = -AppSettings.LaneWidth;
                    if (j == LaneCountLeft - 1)
                    {
                        newLaneLeft.RightLine = null;
                        newLaneLeft.Connectors[0].SetMode(AppSettings, ConnectorMode.Hidden);
                    }
                }
                else if (StreetType == StreetType.Lane_Union_Both)
                {
                    y -= yStepLeft;
                    if (j >= LaneCountLeft - 2)
                        sOffsetLeft = AppSettings.LaneWidth;
                    if (j == LaneCountLeft - 1)
                    {
                        newLaneLeft.RightLine = null;
                        newLaneLeft.Connectors[1].SetMode(AppSettings, ConnectorMode.Hidden);
                    }
                }
                else
                {
                    sOffsetLeft += sOffsetStepLeft;
                    y -= yStepLeft;
                }
            }

            lanes[0].RightLine.Shared = false; // in case, it is not a ShoulderLine type
            lanes[LaneCountRight + LaneCountCenter + LaneCountLeft - 1].LeftLine.Shared = false; // in case, it is not a ShoulderLine type

            activeConnector = lanes[(lanes.Length - 1) / 2].Connectors[0];

            if (RampType == RampType.ExitRamp)
            {
                LaneElement newLaneExit = new LaneElement(this, i, AppSettings, ShapeType.Curve, RampRadius, RampCurveAngle, 0);
                newLaneExit.LeftLine = new LineElement(AppSettings, ShapeType.Curve, RightBorderLine);
                newLaneExit.RightLine = new LineElement(AppSettings, ShapeType.Curve, RightBorderLine);
                newLaneExit.LeftLine.Shared = false;  // in case, it is not a ShoulderLine type
                newLaneExit.RightLine.Shared = false;  // in case, it is not a ShoulderLine type
                newLaneExit.Connectors[0].CenterP = lanes[0].Connectors[0].CenterP;
                newLaneExit.Connectors[0].SetMode(AppSettings, ConnectorMode.Hidden);
                newLaneExit.Connectors[1].SetMode(AppSettings, ConnectorMode.Out);
                lanes[i] = newLaneExit;
            }
            else if (RampType == RampType.Entrance)
            {
                LaneElement newLaneEntrance = new LaneElement(this, i, AppSettings, ShapeType.Curve, RampRadius, RampCurveAngle, 0);
                newLaneEntrance.LeftLine = new LineElement(AppSettings, ShapeType.Curve, RightBorderLine);
                newLaneEntrance.RightLine = new LineElement(AppSettings, ShapeType.Curve, RightBorderLine);
                newLaneEntrance.LeftLine.Shared = false;  // in case, it is not a ShoulderLine type
                newLaneEntrance.RightLine.Shared = false;  // in case, it is not a ShoulderLine type
                newLaneEntrance.Connectors[0].Angle = Utils.ToRadian(RampCurveAngle);

                float dx = newLaneEntrance.Connectors[1].CenterP.X - lanes[0].Connectors[1].CenterP.X;
                float dy = newLaneEntrance.Connectors[1].CenterP.Y - lanes[0].Connectors[1].CenterP.Y;
                newLaneEntrance.Connectors[0].CenterP -= new SizeF(dx,dy);
                newLaneEntrance.Connectors[0].SetMode(AppSettings, ConnectorMode.In);
                newLaneEntrance.Connectors[1].SetMode(AppSettings, ConnectorMode.Hidden);
                lanes[i] = newLaneEntrance;
            }
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Draw method of this class. It performs specific drawing related to ramps or splits and unions, but calls base.Draw for all other cases.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            if (DrawAbort())
                return;

            if ((DrawMode == DrawMode.TopLayer) && (RampType != RampType.None))
            {
                lanes[LaneCount - 1].Draw(grfx, ScaleFactor, DrawMode);
                lanes[0].Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                for (int i = 0; i < LaneCount - 1; i++)
                    lanes[i].Draw(grfx, ScaleFactor, DrawMode);

                double c = RampRadius + lanes[LaneCount - 1].Connectors[0].DrawWidth - lanes[LaneCount - 1].RightLine.Connectors[0].DrawWidth;
                double b = RampRadius;
                double a1 = Math.Sqrt(c * c - b * b);
                b += lanes[LaneCount - 1].RightLine.Connectors[0].Width;
                double a2 = Math.Sqrt(c * c - b * b);

                PointF[] poly = new PointF[5];
                if (RampType == RampType.ExitRamp)
                {
                    poly[0] = Utils.GetPoint(lanes[0].RightLine.Connectors[0].DrawP0, lanes[0].RightLine.Connectors[0].Angle,-0.25);
                    poly[1] = lanes[0].RightLine.Connectors[0].DrawP1;
                    poly[2] = Utils.GetPoint(poly[1], lanes[0].RightLine.Connectors[0].Angle + Utils.RIGHT_ANGLE_RADIAN, a1);
                    poly[3] = Utils.GetPoint(poly[0], lanes[0].RightLine.Connectors[0].Angle + Utils.RIGHT_ANGLE_RADIAN, a2);
                    poly[4] = poly[0];
                }
                else if (RampType == RampType.Entrance)
                {
                    poly[0] = Utils.GetPoint(lanes[0].RightLine.Connectors[1].DrawP0, lanes[0].RightLine.Connectors[1].Angle,-0.25);
                    poly[1] = lanes[0].RightLine.Connectors[1].DrawP1;
                    poly[2] = Utils.GetPoint(poly[1], lanes[0].RightLine.Connectors[1].Angle + Utils.RIGHT_ANGLE_RADIAN, -a1);
                    poly[3] = Utils.GetPoint(poly[0], lanes[0].RightLine.Connectors[1].Angle + Utils.RIGHT_ANGLE_RADIAN, -a2);
                    poly[4] = poly[0];
                }

                if ((lanes[0].ColorMode == ColorMode.ImageColor) || (lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir))
                    grfx.FillPolygon(new SolidBrush(lanes[0].GetDrawColor()), Utils.Scale(poly, ScaleFactor));
                else
                    grfx.FillPolygon(new SolidBrush(lanes[LaneCount - 1].GetDrawColor()), Utils.Scale(poly, ScaleFactor));

                if (RampType == RampType.Entrance)
                {
                    double angle = lanes[0].RightLine.Connectors[1].Angle + Utils.RIGHT_ANGLE_RADIAN;
                    poly[0] = poly[3];
                    poly[1] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
 
                    double dist = a2 / 1.2;
                    double w = stopLineWidth * 2 / 3;
                    double w2 = w / 2;
                    int n = (int)(dist / (w + w2)) + 1;

                    if (lanes[0].ColorMode == ColorMode.ImageColor)
                    {
                        double ofs = 0;// (dist - (n * w) - ((n - 1) * w2)) / 2;

                        PointF[] p = new PointF[3];
                        for (int j = 0; j < n; j++, ofs += w + w2)
                        {
                            p[0] = Utils.GetPoint(poly[0], angle, ofs);
                            p[1] = Utils.GetPoint(p[0], angle, w);
                            p[2] = Utils.GetPoint(poly[1], angle, ofs + w2);
                            grfx.FillPolygon(new SolidBrush(stopLineColor), Utils.Scale(p, ScaleFactor));
                        }
                    }
                    else // ColorMode.ClassCode or ColorMode.ClassColor
                    {
                        if (SegmClassDefs.IsDriveLeftOrRight(lanes[LaneCount - 1].SegmClassDef))
                        {
                            PointF[] pp = new PointF[5];
                            pp[0] = poly[3];
                            pp[2] = lanes[LaneCountRight - 1].LeftLine == null ? lanes[LaneCountRight - 1].Connectors[1].EndP0 : lanes[LaneCountRight - 1].LeftLine.Connectors[1].EndP1;
                            pp[3] = lanes[LaneCount - 1].RightLine == null ? lanes[LaneCount - 1].Connectors[1].EndP1 : lanes[LaneCount - 1].RightLine.Connectors[1].EndP0;
                            pp[1] = Utils.GetPoint(poly[3], angle - Utils.RIGHT_ANGLE_RADIAN, -Utils.GetDistance(pp[2], pp[3]));
                            pp[1] = Utils.GetPoint(pp[1], angle, dist*0.75);
                            pp[4] = pp[0];
                            grfx.FillPolygon(new SolidBrush(lanes[LaneCount - 1].GetDrawColor()), Utils.Scale(pp, ScaleFactor));

                            for (int i = 0; i < LaneCountRight; i++)
                            {
                                if ((i > 0) && (lanes[i].RightLine != null))
                                    lanes[i].RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);

                                if ((i < LaneCountRight - 1) && (lanes[i].LeftLine != null))
                                    lanes[i].LeftLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                            }
                        }

                        if (((Utils.GetDistance(CameraPoint,poly[0])<MaxDetailDist) || (Utils.GetDistance(CameraPoint, poly[1]) < MaxDetailDist)) && 
                            ((DrawWrongDirItems==true) || (lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir) || SegmClassDefs.IsDriveLeftOrRight(lanes[LaneCount - 1].SegmClassDef)))
                        {
                            dist = n * (w + w2) - w2;
                            poly[2] = Utils.GetPoint(poly[1], angle, dist);
                            poly[3] = Utils.GetPoint(poly[0], angle, dist);
                            poly[4] = poly[0];
                            SegmClassDef scdYielLine = SegmClassDefs.GetSegmClassDef(StopYieldType.YieldLine);
                            Color color = lanes[0].ColorMode == ColorMode.ClassColor ? scdYielLine.DrawColor : scdYielLine.ClassCodeColor;
                            grfx.FillPolygon(new SolidBrush(color), Utils.Scale(poly, ScaleFactor));
                        }
                    }
                }

                lanes[LaneCount - 1].RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
            }
            else
            {
                if (DrawMode == DrawMode.BaseLayer)
                {
                    if ((StreetType == StreetType.Lane_Split_Right) || (StreetType == StreetType.Lane_Split_Both))
                    {
                        if ((LaneCountRight >= 2) && (lanes[1].RightLine != null))
                            lanes[1].RightLine.Connectors[0].EndP1 = lanes[1].Connectors[0].EndP1;

                        if ((StreetType == StreetType.Lane_Split_Both) && (LaneCountLeft >= 2) && (lanes[lanes.Length - 2].LeftLine != null))
                            lanes[lanes.Length-2].LeftLine.Connectors[0].EndP0 = lanes[lanes.Length - 2].Connectors[0].EndP0;
                    }
                    else if ((StreetType == StreetType.Lane_Union_Right) || (StreetType == StreetType.Lane_Union_Both))
                    {
                        if ((LaneCountRight >= 2) && (lanes[1].RightLine != null))
                            lanes[1].RightLine.Connectors[1].EndP1 = lanes[1].Connectors[1].EndP1;

                        if ((StreetType == StreetType.Lane_Union_Both) && (LaneCountLeft >= 2) && (lanes[lanes.Length - 2].LeftLine != null))
                            lanes[lanes.Length - 2].LeftLine.Connectors[1].EndP0 = lanes[lanes.Length - 2].Connectors[1].EndP0;
                    }
                }
                base.Draw(grfx, ScaleFactor, DrawMode);

                if (lanes[0].ColorMode != ColorMode.ImageColor)
                {
                    if ((StreetType == StreetType.Center_Split) || (StreetType == StreetType.Center_Union))
                    {
                        lanes[LaneCountRight - 1].Draw(grfx, ScaleFactor, DrawMode);
                        lanes[LaneCount - LaneCountLeft].Draw(grfx, ScaleFactor, DrawMode);
                    }
                    else if ((StreetType == StreetType.Lane_Split_Right) || (StreetType == StreetType.Lane_Split_Both) || 
                             (StreetType == StreetType.Lane_Union_Right) || (StreetType == StreetType.Lane_Union_Both))
                    {
                        if (lanes[0].RightLine != null)
                            lanes[0].RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                        if (lanes[LaneCount-1].LeftLine != null)
                            lanes[LaneCount - 1].LeftLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                    }
                }
            }
        }

        /// <summary>
        /// Increase the size of this multilane object by adding one step size to its length or its curve angle.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="DrawingSize">Size of the drawing area to limit the maximum length.</param>
        public override void SizeIncreaseStep(AppSettings AppSettings, SizeF DrawingSize)
        {
            if (CanRotate() == false)
                return;

            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Shape == ShapeType.Curve)
                {
                    double angleStep = Utils.ToRadian(AppSettings.AngleStep);
                    if (lanes[i].CurveAngle >= 0)
                    {
                        if (lanes[i].CurveAngle + angleStep <= Utils.ToRadian(AppSettings.MaxCurveAngle))
                            lanes[i].CurveAngle += angleStep;
                    }
                    else
                    {
                        if (lanes[i].CurveAngle - angleStep >= -Utils.ToRadian(AppSettings.MaxCurveAngle))
                            lanes[i].CurveAngle -= angleStep;
                    }
                }
                else
                {
                    if (lanes[i].Length + AppSettings.LengthStep < DrawingSize.Width)
                        lanes[i].Length += AppSettings.LengthStep;
                }
            }
        }

        /// <summary>
        /// Decrease the size of this multilane object by subtracting one step size from its length or its curve angle.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        public override void SizeDecreaseStep(AppSettings AppSettings)
        {
            if (CanRotate() == false)
                return;

            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Shape == ShapeType.Curve)
                {
                    double angleStep = Utils.ToRadian(AppSettings.AngleStep);
                    if (lanes[i].CurveAngle >= 0)
                    {
                        if (lanes[i].CurveAngle - angleStep >= Utils.ToRadian(AppSettings.MinCurveAngle))
                            lanes[i].CurveAngle -= angleStep;
                    }
                    else
                    {
                        if (lanes[i].CurveAngle + angleStep <= -Utils.ToRadian(AppSettings.MinCurveAngle))
                            lanes[i].CurveAngle += angleStep;
                    }
                }
                else
                {
                    if (lanes[i].Length - AppSettings.LengthStep >= AppSettings.MinStraightLength)
                        lanes[i].Length -= AppSettings.LengthStep;
                }
            }
        }

        /// <summary>
        /// Sizes the multi lane street, so the ActiveConnector.CenterP selected before will be moved as close as possible to the TargetPoint. In base mode, only one parameter will be changed (length or curve angle)
        /// in extended mode, two parameters will be adjusted at once when possible to get directly to the TargetPoint.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="DrawingSize">Size of the drawing area to limit the maximum length.</param>
        /// <param name="TargetPoint">New target point for the ActiveConnector.CenterP</param>
        /// <param name="ExtMode">When false, only one parameter (length or curve angle) will be adjusted to get close to the target point. When true, length and s-offset or curve angle and radius will be adjusted. </param>
        public override void Size(AppSettings AppSettings, SizeF DrawingSize, PointF TargetPoint, bool ExtMode)
        {
            if (ActiveConnector == null)
                return;

            LaneElement activeLane = (LaneElement)ActiveConnector.Owner;
            bool c0move = ActiveConnector.OwnerIdx == 0;

            if ((RampType == RampType.ExitRamp) && (c0move==true))
                return;

            if ((RampType == RampType.Entrance) && (c0move==false))
                return;

            double pointDist = Utils.GetDistance(ActiveConnector.CenterP, TargetPoint);
            double pointAngle = Utils.GetAngle(ActiveConnector.CenterP, TargetPoint);
            double moveAngle = Utils.LimitRadian(ActiveConnector.Angle + Utils.RIGHT_ANGLE_RADIAN);
            double deltaAngle = Utils.LimitRadian(pointAngle - moveAngle);
            double moveDist = pointDist * Math.Cos(deltaAngle);
            double moveOffs = pointDist * Math.Sin(deltaAngle);
            SizeF deltaXY = new SizeF(TargetPoint.X - ActiveConnector.CenterP.X, TargetPoint.Y - ActiveConnector.CenterP.Y);

            if (activeLane.Shape == ShapeType.Curve)
            {
                double centerDist = Utils.GetDistance(activeLane.CircleCenter, TargetPoint);
                if (ExtMode || (centerDist < activeLane.InnerRadius + ActiveConnector.Width))
                {
                    double da = moveDist / activeLane.InnerRadius;
                    if (c0move)
                        da = -da;

                    double ca = Math.Min(Math.Max(Math.Abs(activeLane.CurveAngle) + da, Utils.ToRadian(AppSettings.MinCurveAngle)), Utils.ToRadian(AppSettings.MaxCurveAngle));
                    double phase = Math.PI;
                    if (activeLane.CurveAngle < 0)
                    {
                        ca = -ca;
                        phase = 0;
                        moveOffs = -moveOffs;
                    }

                    moveOffs = moveOffs / 2;

                    bool ramp = (RampType == RampType.ExitRamp) || (RampType == RampType.Entrance);
                    double maxRadius = Math.Max(DrawingSize.Width, DrawingSize.Height) + lanes.Length * AppSettings.LaneWidth;

                    for (int i = 0; i < lanes.Length; i++)
                    {
                        if (lanes[i].Shape == activeLane.Shape)
                        {
                            lanes[i].SuspendEvents();
                            double minInnerRadius = AppSettings.MinInnerRadius;  
                            if (ExtMode && (ramp==false))
                                 minInnerRadius = activeLane.CurveAngle >= 0? AppSettings.MinInnerRadius + i * AppSettings.LaneWidth : AppSettings.MinInnerRadius + (lanes.Length - 1 - i) * AppSettings.LaneWidth;

                            if (c0move)
                            {
                                if (ExtMode)
                                {
                                    if (activeLane.CurveAngle >= 0)
                                    {
                                        lanes[i].InnerRadius = Math.Min(Math.Max(lanes[i].InnerRadius + moveOffs, minInnerRadius), maxRadius);
                                        lanes[i].CircleCenter = Utils.GetPoint(lanes[i].Connectors[1].CenterP, Utils.LimitRadian(lanes[i].Connectors[1].Angle), lanes[i].InnerRadius + ActiveConnector.Width / 2);
                                    }
                                    else
                                    {
                                        lanes[i].InnerRadius = Math.Min(Math.Max(lanes[i].InnerRadius + moveOffs, minInnerRadius), maxRadius);
                                        lanes[i].CircleCenter = Utils.GetPoint(lanes[i].Connectors[1].CenterP, Utils.LimitRadian(lanes[i].Connectors[1].Angle - Math.PI), lanes[i].InnerRadius + ActiveConnector.Width / 2);
                                    }
                                }

                                PointF p0 = Utils.GetPoint(lanes[i].CircleCenter, Utils.LimitRadian((lanes[i].Connectors[1].Angle + ca) - phase), lanes[i].InnerRadius + ActiveConnector.Width / 2);
                                lanes[i].Connectors[0].UpdateAngleAndCenterP(Utils.LimitRadian(lanes[i].Connectors[1].Angle + ca), p0);
                                lanes[i].CurveAngle = ca;
                            }
                            else
                            {
                                if (ExtMode)
                                {
                                    if (activeLane.CurveAngle >= 0)
                                        lanes[i].InnerRadius = Math.Min(Math.Max(lanes[i].InnerRadius + moveOffs, minInnerRadius), maxRadius);
                                    else
                                        lanes[i].InnerRadius = Math.Min(Math.Max(lanes[i].InnerRadius + moveOffs, minInnerRadius), maxRadius);
                                }
                                lanes[i].CurveAngle = ca;
                            }
                            lanes[i].ResumeEvents();
                            lanes[i].Update();

                            double dist = Utils.GetDistance(ActiveConnector.CenterP, TargetPoint);

#if DEBUG_WRITE_LINES
                            Debug.WriteLine("X:" + TargetPoint.X.ToString("F3") + "  Y:" + TargetPoint.X.ToString("F3") + "  dX:" + deltaXY.Width.ToString("F3") + "  dY:" + deltaXY.Height.ToString("F3") + "  pointDist:" + pointDist.ToString("F3") + "  dist:" + dist.ToString("F6") + "  pointAngle:" + Utils.ToDegree(pointAngle).ToString("F3") + "  moveAngle:" + Utils.ToDegree(moveAngle).ToString("F3") + "  deltaAngle" + Utils.ToDegree(deltaAngle).ToString("F3") + "  moveDist:" + moveDist.ToString("F3") + "  moveOffs:" + moveOffs.ToString("F3") + "  InnerRadius:" + activeLane.InnerRadius.ToString("F3") + "  ca:" + Utils.ToDegree(ca).ToString("F3"));
#endif
                        }
                    }
                }
            }
            else
            {
                if (c0move)
                    moveDist = -moveDist;

                double l = Math.Min(Math.Max(activeLane.Length + moveDist, AppSettings.MinStraightLength), DrawingSize.Width);
                double o = Math.Min(Math.Max(activeLane.SOffset + moveOffs, -l), l) - activeLane.SOffset;

                if (double.IsNaN(l) || double.IsNaN(o))
                    Debug.WriteLine("l:" + l.ToString() + "  o:" + o.ToString());

                for (int i = 0; i < lanes.Length; i++)
                {
                    if (lanes[i].Shape == activeLane.Shape)
                    {
                        lanes[i].SuspendEvents();
                        if (c0move)
                        {
                            PointF p = Utils.GetPoint(lanes[i].Connectors[1].CenterP, moveAngle, -l);
                            if (lanes[i].Shape == ShapeType.S_Shape)
                            {
                                if (ExtMode)
                                    lanes[i].SOffset += o;
                                p = Utils.GetPoint(p, moveAngle - Utils.RIGHT_ANGLE_RADIAN, -lanes[i].SOffset);
                            }
                            lanes[i].Connectors[0].CenterP = p;
                        }
                        else
                        {
                            if ((lanes[i].Shape == ShapeType.S_Shape) && ExtMode)
                                lanes[i].SOffset -= o;
                        }
                        lanes[i].Length = l;

                        lanes[i].ResumeEvents();
                        lanes[i].Update();
                    }
                }
            }

            if ((StreetType == StreetType.CurveLeft) || (StreetType == StreetType.CurveRight))
                innerRadius = Math.Min(lanes[0].InnerRadius, lanes[lanes.Length - 1].InnerRadius);

            else if ((RampType == RampType.ExitRamp) || (RampType == RampType.Entrance))
            {
                rampRadius = lanes[lanes.Length - 1].InnerRadius;
                rampCurveAngle = Utils.ToDegree(lanes[lanes.Length - 1].CurveAngle);
            }
        }

        /// <summary>
        /// Set the length of all straight lanes to the given value. Curve shaped lanes are ignored.
        /// </summary>
        /// <param name="Length">Length in mm to set to.</param>
        public void SetLength(double Length)
        {
            if ((StreetType == StreetType.CurveLeft) || (StreetType == StreetType.CurveRight))
                return;

            for (int i = 0; i < lanes.Length; i++)
            {
                if (lanes[i].Shape != ShapeType.Curve)
                    lanes[i].Length = Length;
            }

            if (RampType == RampType.Entrance)
            {
                float dx = lanes[LaneCount-1].Connectors[1].CenterP.X - lanes[0].Connectors[1].CenterP.X;
                float dy = lanes[LaneCount - 1].Connectors[1].CenterP.Y - lanes[0].Connectors[1].CenterP.Y;
                lanes[LaneCount - 1].Connectors[0].CenterP -= new SizeF(dx, dy);
            }
        }

        /// <summary>
        /// Set the curve angle of of a curved shape to the given value. Straight shapes are ignored.
        /// </summary>
        /// <param name="CurveAngle">Curve angle in degrees to set to.</param>
        public void SetCurveAngle(double CurveAngle)
        {
            if ((StreetType == StreetType.CurveLeft) || (StreetType == StreetType.CurveRight))
            {
                CurveAngle = Math.Abs(CurveAngle);
                if (StreetType == StreetType.CurveLeft)
                    CurveAngle = -CurveAngle;
                double rCurveAngle = Utils.ToRadian(CurveAngle);

                for (int i = 0; i < lanes.Length; i++)
                {
                    if (lanes[i].Shape == ShapeType.Curve)
                        lanes[i].CurveAngle = rCurveAngle;
                }
            }
        }

        /// <summary>
        /// Set the S-offset of an s-shape to the given value.
        /// </summary>
        /// <param name="SOffset">S-offset in mm to set to.</param>
        public void SetSOffset(double SOffset)
        {
            if ((StreetType == StreetType.Straight) || (StreetType == StreetType.CurveLeft) || (StreetType == StreetType.CurveRight))
                return;

            if ((StreetType == StreetType.S_Left) || (StreetType == StreetType.S_Right))
            {
                for (int i = 0; i < lanes.Length; i++)
                {
                    if (lanes[i].Shape == ShapeType.S_Shape)
                        lanes[i].SOffset = SOffset;
                }
            }
            else
            {
                double delta = SOffset - lanes[0].SOffset;
                for (int i = 0; i < lanes.Length; i++)
                {
                    if (lanes[i].Shape == ShapeType.S_Shape)
                        lanes[i].SOffset += delta;
                }

            }
        }

        /// <summary>
        /// Set the 3 additional parameters at once: length, curve angle and s-offset.
        /// This method calls the three individual methods: SetLength, SetCurveAngle and SetSOffset.
        /// </summary>
        /// <param name="Length">Length in mm to set to.</param>
        /// <param name="CurveAngle">Curve angle in degrees to set to.</param>
        /// <param name="SOffset">S-offset in mm to set to.</param>
        public void SetParameter(double Length, double CurveAngle, double SOffset)
        {
            SetLength(Length);
            SetCurveAngle(CurveAngle);
            SetSOffset(SOffset);
        }

        /// <summary>
        /// Go through the lanes left, right and center and ramps to summarize the possible SegmClassDef objects and then call the base class method to update the lanes' classes.
        /// </summary>
        public override void UpdateUseCounts()
        {
            for (int i = 0; i < LaneCountRight; i++)
                SegmClassDefs.IncUseCount( SegmClassDefs.ScdDrivingDir);

            for (int i = 0; i < LaneCountCenter; i++)
                SegmClassDefs.IncUseCount(SegmClassDefs.ScdCenterLane);

            for (int i = 0; i < LaneCountLeft; i++)
                SegmClassDefs.IncUseCount(SegmClassDefs.ScdWrongDir);

            if (RampType == RampType.ExitRamp)
                SegmClassDefs.IncUseCount(SegmClassDefs.ScdRightTurnDir);

            if (RampType == RampType.Entrance)
                SegmClassDefs.IncUseCount(StopYieldType.YieldLine);

            base.UpdateUseCounts();
        }


#region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public override void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));
            nodeItem.AppendChild(Doc.CreateElement("element_type")).AppendChild(Doc.CreateTextNode(XML_TYPE_STR));

            nodeItem.AppendChild(Doc.CreateElement("street_type")).AppendChild(Doc.CreateTextNode(StreetType.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("lane_count_right")).AppendChild(Doc.CreateTextNode(LaneCountRight.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("lane_count_center")).AppendChild(Doc.CreateTextNode(LaneCountCenter.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("lane_count_left")).AppendChild(Doc.CreateTextNode(LaneCountLeft.ToString()));

            nodeItem.AppendChild(Doc.CreateElement("right_border_line")).AppendChild(Doc.CreateTextNode(RightBorderLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("right_lane_line")).AppendChild(Doc.CreateTextNode(RightLaneLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("divider_line")).AppendChild(Doc.CreateTextNode(DividerLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("divider_line2")).AppendChild(Doc.CreateTextNode(DividerLine2.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("left_lane_line")).AppendChild(Doc.CreateTextNode(LeftLaneLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("left_border_line")).AppendChild(Doc.CreateTextNode(LeftBorderLine.ToString()));

            nodeItem.AppendChild(Doc.CreateElement("length")).AppendChild(Doc.CreateTextNode(Lanes[0].Length.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("s_offset")).AppendChild(Doc.CreateTextNode(Lanes[0].SOffset.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("inner_radius")).AppendChild(Doc.CreateTextNode(InnerRadius.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("curve_angle")).AppendChild(Doc.CreateTextNode(Math.Abs(Utils.ToDegree(Lanes[0].CurveAngle)).ToString()));

            nodeItem.AppendChild(Doc.CreateElement("ramp_type")).AppendChild(Doc.CreateTextNode(RampType.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("ramp_radius")).AppendChild(Doc.CreateTextNode(RampRadius.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("ramp_curve_angle")).AppendChild(Doc.CreateTextNode(Math.Abs(RampCurveAngle).ToString()));

            nodeItem.AppendChild(Doc.CreateElement("angle")).AppendChild(Doc.CreateTextNode(Utils.ToDegree(Lanes[0].Connectors[0].Angle).ToString()));
            nodeItem.AppendChild(Doc.CreateElement("location_x")).AppendChild(Doc.CreateTextNode(Lanes[0].Connectors[0].CenterP.X.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("location_y")).AppendChild(Doc.CreateTextNode(Lanes[0].Connectors[0].CenterP.Y.ToString()));

            base.WriteToXml(Doc, nodeItem);
        }

        /// <summary>
        /// Reads the contents for one StreetElement class instance from an XML document at the specified node and returns the StreetElement object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <returns>Reference to the StreetElement created from the XML file contents.</returns>
        public static StreetElement LoadFromXml(XmlDocument Doc, XmlNode nodeItem, AppSettings AppSettings)
        {
            try
            {
                string typeStr = nodeItem.SelectSingleNode("element_type").InnerText;
                if (typeStr != XML_TYPE_STR)
                    return null;

                StreetType streetType = (StreetType)Enum.Parse(typeof(StreetType), nodeItem.SelectSingleNode("street_type").InnerText);
                int laneCountRight = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_right").InnerText);
                int laneCountCenter = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_center").InnerText);
                int laneCountLeft = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_left").InnerText);

                LineType rightBorderLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("right_border_line").InnerText);
                LineType rightLaneLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("right_lane_line").InnerText);
                LineType dividerLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("divider_line").InnerText);
                LineType dividerLine2 = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("divider_line2").InnerText);
                LineType leftLaneLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("left_lane_line").InnerText);
                LineType leftBorderLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("left_border_line").InnerText);

                double length = Convert.ToDouble(nodeItem.SelectSingleNode("length").InnerText);
                double soffset = Convert.ToDouble(nodeItem.SelectSingleNode("s_offset").InnerText);
                double innerRadius = Convert.ToDouble(nodeItem.SelectSingleNode("inner_radius").InnerText);
                double curveAngle = Convert.ToDouble(nodeItem.SelectSingleNode("curve_angle").InnerText);

                RampType rampType = (RampType)Enum.Parse(typeof(RampType), nodeItem.SelectSingleNode("ramp_type").InnerText);
                double rampRadius = Convert.ToDouble(nodeItem.SelectSingleNode("ramp_radius").InnerText);
                double rampCurveAngle = Convert.ToDouble(nodeItem.SelectSingleNode("ramp_curve_angle").InnerText);

                double angle = Convert.ToDouble(nodeItem.SelectSingleNode("angle").InnerText);
                float x = Convert.ToSingle(nodeItem.SelectSingleNode("location_x").InnerText);
                float y = Convert.ToSingle(nodeItem.SelectSingleNode("location_y").InnerText);

                MultiLaneStreet multiLaneStreet = new MultiLaneStreet(AppSettings, streetType, laneCountRight, laneCountCenter, laneCountLeft, rightBorderLine,
                    rightLaneLine, dividerLine, dividerLine2, leftLaneLine, leftBorderLine, innerRadius, rampType, rampRadius, rampCurveAngle);

                multiLaneStreet.SetParameter(length, curveAngle, soffset);
                multiLaneStreet.SetAngleAndLocation(angle, new PointF(x, y));

                multiLaneStreet.ReadFromXml(Doc, nodeItem, AppSettings);

                return multiLaneStreet;
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
        /// Gets the inner radius of the most inner lane.
        /// </summary>
        public double InnerRadius
        {
            get { return innerRadius; }
        }

        /// <summary>
        /// Gets the radius of a possible ramp in mm.
        /// </summary>
        public double RampRadius
        {
            get { return rampRadius; }
        }

        /// <summary>
        /// Gets the curve angle of a possible ramp in Degrees.
        /// </summary>
        public double RampCurveAngle
        {
            get { return rampCurveAngle; }
        }

#endregion Public Properties
    }

}
