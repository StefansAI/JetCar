// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

// Activate to vizualize the corner points as different color circles
//#define DEBUG_DRAW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace StreetMaker
{
    /// <summary>
    /// The shaped area is inherited from BaseElement. It expands to handle shapes like straight lane or line, a curved one or an s-shaped.
    /// </summary>
    public class ShapedArea:BaseElement
    {
        #region Protected and Internal Fields
        /// <summary>The shape of this instance.</summary>
        protected ShapeType shape;
        /// <summary>The length of a straight lane or line. This field is only used for the straight shape.</summary>
        protected double length;
        /// <summary>The inner radius of a curved shape. This field is only used for the curved shape.</summary>
        protected double innerRadius;
        /// <summary>The sweeping angle of the curve piece. This field is only used for the curved shape.</summary>
        protected double curveAngle;
        /// <summary>The offset of an s-shaped center points from the straight one. This field is only used for the s-shape.</summary>
        protected double sOffset;
        /// <summary>The angle increment or decrement of a curved element that can be added to the current angle. This field is only used for the curved shape.</summary>
        protected double curveAngleStep;
        /// <summary>The maximum angle  that a curved element can assume. This field is only used for the curved shape.</summary>
        protected double maxCurveAngle;
        /// <summary>The center point of the circle of a curved element. This field is only used for the curved shape.</summary>
        internal PointF circleCenter;
        #endregion Protected and Internal Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the ShapedArea class, which is inherited from the BaseElement class.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="Shape">Shape of this element, straight, curved or s-shaped.</param>
        /// <param name="Width">Width of the element.</param>
        /// <param name="InnerRadius">The inner radius of a curved shape. This field is only used for the curved shape.</param>
        /// <param name="CurveAngle">The sweeping angle of the curve piece. This field is only used for the curved shape.</param>
        /// <param name="SOffset">The offset of an s-shaped center points from the straight one. This field is only used for the s-shape.</param>
        public ShapedArea(AppSettings AppSettings, ShapeType Shape, double Width, double InnerRadius, double CurveAngle, double SOffset):base(Width, AppSettings.LaneColor, AppSettings.StreetOutlineColor, AppSettings.StreetOutlineLineWidth, AppSettings.BackgroundColor)
        {
            this.shape = Shape;
            this.length = AppSettings.DefaultStraightLength;
            this.curveAngleStep = Utils.ToRadian(AppSettings.AngleStep);
            this.maxCurveAngle = Utils.ToRadian(AppSettings.MaxCurveAngle);
            this.innerRadius = InnerRadius;
            this.curveAngle = Utils.ToRadian(CurveAngle);
            this.sOffset = SOffset;
        }

        public ShapedArea(ShapedArea Source, double Width) : base(Width, Source.Color, Source.OutlineColor, Source.OutlineLineWidth, Source.BackgroundColor)
        {
            eventsBlocked = true;
            this.shape = Source.shape;
            this.length = Source.length;
            this.curveAngleStep = Source.curveAngleStep;
            this.maxCurveAngle = Source.maxCurveAngle;
            this.innerRadius = Source.innerRadius;
            this.curveAngle = Source.curveAngle;
            this.sOffset = Source.sOffset;
            this.circleCenter = Source.circleCenter;
            this.EditModeMove = Source.EditModeMove;
            this.ColorMode = Source.ColorMode;
            this.SegmClassDef = Source.SegmClassDef;

            this.Connectors[0].Angle = Source.Connectors[0].Angle;
            this.Connectors[1].Angle = Source.Connectors[1].Angle;
        }
        #endregion Constructor

        #region Private and Protected Methods
        /// <summary>
        /// Overwritten event handler for connector changes. Changes from the connectors are mainly from moving, rotating the element or from sizing it. 
        /// </summary>
        /// <param name="sender">Connector object that sent the event.</param>
        protected override void HandleConnectorChangeEvent(Connector sender)
        {
            //if (sender == Connectors[1])
            //    return;

            Connector updateConnector = sender == Connectors[0] ? Connectors[1] : Connectors[0];

            if (this.EditModeMove == true)
            {
                switch (shape)
                {
                    case ShapeType.Straight:
                        updateConnector.UpdateAngleAndCenterP(sender.Angle, Utils.GetPoint(sender.CenterP, sender.Angle + Utils.RIGHT_ANGLE_RADIAN, Length));
                        break;

                    case ShapeType.Curve:
                        if (curveAngle >= 0)
                        {
                            circleCenter = Utils.GetPoint(sender.EndP1, Utils.LimitRadian(sender.Angle), InnerRadius);
                            PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((sender.Angle - curveAngle) - Math.PI), (InnerRadius + updateConnector.Width / 2));
                            updateConnector.UpdateAngleAndCenterP(Utils.LimitRadian(sender.Angle - curveAngle), p);
                        }
                        else
                        {
                            circleCenter = Utils.GetPoint(sender.EndP0, Utils.LimitRadian(sender.Angle - Math.PI), InnerRadius);
                            PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((sender.Angle - curveAngle)), (InnerRadius + updateConnector.Width / 2));
                            updateConnector.UpdateAngleAndCenterP(sender.Angle - curveAngle, p);
                        }
                        break;

                    case ShapeType.S_Shape:
                        {
                            PointF p = Utils.GetPoint(sender.CenterP, sender.Angle, +sOffset);
                            updateConnector.UpdateAngleAndCenterP(sender.Angle, Utils.GetPoint(p, sender.Angle + Utils.RIGHT_ANGLE_RADIAN, Length));
                        }
                        break;
                }
            }
            else
            {
                switch (shape)
                {
                    case ShapeType.Straight:
                        {
                            length = Math.Max(Utils.GetDistance(Connectors[0].CenterP, Connectors[1].CenterP), Connectors[0].Width / 2);
                        }
                        break;

                    case ShapeType.Curve:
                        {
                            double alpha = Utils.GetAngle(Connectors[0].CenterP, circleCenter);
                            double beta = Utils.GetAngle(sender.CenterP, circleCenter);
                            double gamma = Utils.LimitRadian(alpha - beta);

                            int n = (int)Math.Round(gamma / curveAngleStep);
                            int m = (int)Math.Round(maxCurveAngle / curveAngleStep);
                            double delta = Math.Min(Math.Max(Math.Abs(n), 1), m) * curveAngleStep;
                            if (Math.Abs(n) != 4)
                                alpha = Utils.GetAngle(updateConnector.CenterP, circleCenter);

                            if (curveAngle > 0)
                            {
                                curveAngle = delta;
                                PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((updateConnector.Angle - curveAngle) - Math.PI), (InnerRadius + sender.Width / 2));
                                sender.UpdateAngleAndCenterP(updateConnector.Angle - curveAngle, p);

                            }
                            else
                            {
                                curveAngle = -delta;
                                PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((updateConnector.Angle - curveAngle)), (InnerRadius + sender.Width / 2));
                                sender.UpdateAngleAndCenterP(updateConnector.Angle - curveAngle, p);
                            }

                        }
                        break;

                    case ShapeType.S_Shape:
                        {
                            double h = Utils.GetDistance(Connectors[0].CenterP, Connectors[1].CenterP);
                            double alpha = Utils.GetAngle(Connectors[0].CenterP, Connectors[1].CenterP);
                            length = h * Math.Sin(alpha);
                            sOffset = h * Math.Cos(alpha);
                        }
                        break;
                }
            }
        }

        #region Draw Support Straight Shape
        /// <summary>
        /// Get a scaled polygon from the given points. The 4 corner points of the straight shape will be converted to a 5 point closed polygon after scaling.
        /// </summary>
        /// <param name="P00">First corner point, for instance upper left in initial state.</param>
        /// <param name="P01">Second corner point, for instance lower left in initial state.</param>
        /// <param name="P10">Third corner point, for instance upper right in initial state.</param>
        /// <param name="P11">Fourth corner point, for instance lower right in initial state.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected PointF[] GetScaledStraightPolygon(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            PointF[] polygon = new PointF[5];
            polygon[0] = Utils.Scale(P00, ScaleFactor);
            polygon[1] = Utils.Scale(P10, ScaleFactor);
            polygon[2] = Utils.Scale(P11, ScaleFactor);
            polygon[3] = Utils.Scale(P01, ScaleFactor);
            polygon[4] = polygon[0];
            return polygon;
        }

        /// <summary>
        /// Get a scaled polygon from the connector end points. The 4 corner points of the straight shape will be converted to a 5 point closed polygon after scaling.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected PointF[] GetScaledStraightPolygon(SizeF ScaleFactor)
        {
            //return GetScaledStraightPolygon(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
            return GetScaledStraightPolygon(Connectors[0].DrawP0, Connectors[0].DrawP1, Connectors[1].DrawP0, Connectors[1].DrawP1, ScaleFactor);
        }
        #endregion Draw Support Straight Shape

        #region Draw Support Curve Shape
        /// <summary>
        /// Get a scaled graphics path from the given points. The 4 corner points of the curved shape will be converted to a path of 2 arcs and 2 lines after scaling.
        /// </summary>
        /// <param name="P00">First corner point, for instance upper left in initial state.</param>
        /// <param name="P01">Second corner point, for instance lower left in initial state.</param>
        /// <param name="P10">Third corner point, for instance upper right in initial state.</param>
        /// <param name="P11">Fourth corner point, for instance lower right in initial state.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected GraphicsPath GetScaledCurvePath(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            GraphicsPath path = new GraphicsPath();
            double angle_00_01 = Utils.LimitRadian(Utils.GetAngle(P00, P01));
            double angle_10_11 = Utils.LimitRadian(Utils.GetAngle(P10, P11));
            double angle_delta = Utils.LimitRadian(angle_00_01 - angle_10_11);
            if (curveAngle >= 0)
            {
                if (angle_delta < 0)
                    angle_delta += 2 * Math.PI;
            }
            else
            {
                if (angle_delta > 0)
                    angle_delta -= 2 * Math.PI;
            }

            //Debug.WriteLine("Connectors[0].Angle:" + Connectors[0].Angle.ToString("F3") + "   angle_00_01:" + angle_00_01.ToString("F3") + "   Connectors[1].Angle:" + Connectors[1].Angle.ToString("F3") + "   angle_10_11:" + angle_10_11.ToString("F3") + "   angle_delta:" + angle_delta.ToString("F3") + "   CurveAngle:" + CurveAngle.ToString("F3") + "   offset:" + (CurveAngle - angle_delta).ToString("F3"));

            float iR = (float)(InnerRadius - Connectors[0].DrawOffsW);
            RectangleF innerRect = new RectangleF(Utils.Scale(new PointF(circleCenter.X - iR, circleCenter.Y - iR), ScaleFactor), new SizeF(2 * iR * ScaleFactor.Width, 2 * iR * ScaleFactor.Height));
            float oR = (float)(iR + Connectors[0].DrawWidth);
            RectangleF outerRect = new RectangleF(Utils.Scale(new PointF(circleCenter.X - oR, circleCenter.Y - oR), ScaleFactor), new SizeF(2 * oR * ScaleFactor.Width, 2 * oR * ScaleFactor.Height));
            if (angle_delta >= 0)
            {
                path.AddLine(Utils.Scale(P00, ScaleFactor), Utils.Scale(P01, ScaleFactor));
                path.AddArc(innerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_00_01 + Utils.RIGHT_ANGLE_RADIAN)), (float)Utils.ToDegree(angle_delta));
                path.AddLine(Utils.Scale(P11, ScaleFactor), Utils.Scale(P10, ScaleFactor));
                path.AddArc(outerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_10_11 + Utils.RIGHT_ANGLE_RADIAN)), -(float)Utils.ToDegree(angle_delta));
            }
            else
            {
                path.AddLine(Utils.Scale(P00, ScaleFactor), Utils.Scale(P01, ScaleFactor));
                path.AddArc(outerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_00_01 - Utils.RIGHT_ANGLE_RADIAN)), (float)Utils.ToDegree(angle_delta));
                path.AddLine(Utils.Scale(P11, ScaleFactor), Utils.Scale(P10, ScaleFactor));
                path.AddArc(innerRect, (float)Utils.ToDegree(Utils.LimitRadian(Utils.RIGHT_ANGLE_RADIAN - angle_10_11)), -(float)Utils.ToDegree(angle_delta));
            }

            return path;
        }

        /// <summary>
        /// Get a scaled graphics path from the connector end points. The 4 corner points of the curved shape will be converted to a path of 2 arcs and 2 lines after scaling.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected GraphicsPath GetScaledCurvePath(SizeF ScaleFactor)
        {
            //return GetScaledCurvePath(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
            return GetScaledCurvePath(Connectors[0].DrawP0, Connectors[0].DrawP1, Connectors[1].DrawP0, Connectors[1].DrawP1, ScaleFactor);
        }

        /// <summary>
        /// Get a scaled polygon from the given points. The 4 corner points of the curved shape will be converted to a polygon approximating the curved shape after scaling.
        /// </summary>
        /// <param name="P00">First corner point, for instance upper left in initial state.</param>
        /// <param name="P01">Second corner point, for instance lower left in initial state.</param>
        /// <param name="P10">Third corner point, for instance upper right in initial state.</param>
        /// <param name="P11">Fourth corner point, for instance lower right in initial state.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected PointF[] GetScaledCurvePolygon(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            double angle_00_01 = Utils.LimitRadian(Utils.GetAngle(P00, P01));
            double angle_10_11 = Utils.LimitRadian(Utils.GetAngle(P10, P11));
            double angle_delta = Utils.LimitRadian(angle_00_01 - angle_10_11);
         
            if (curveAngle >= 0)
            {
                if (angle_delta < 0)
                    angle_delta += 2 * Math.PI;
            }
            else
            {
                if (angle_delta > 0)
                    angle_delta -= 2 * Math.PI;
            }

            PointF[] polygon = new PointF[25];
            if (angle_delta >= 0)
            {
                polygon[0] = Utils.Scale(P00, ScaleFactor);
                polygon[1] = Utils.Scale(P01, ScaleFactor);
            }
            else
            {
                polygon[0] = Utils.Scale(P01, ScaleFactor);
                polygon[1] = Utils.Scale(P00, ScaleFactor);
            }
            double sweep = angle_delta / 11;
            double alpha = Utils.GetAngle(circleCenter, P01) - sweep;
            for (int i=0; i<10; i++)
            {
                polygon[2 + i] = Utils.Scale(Utils.GetPoint(circleCenter, alpha, InnerRadius), ScaleFactor);
                alpha -= sweep;
            }
            if (curveAngle >= 0)
            {
                polygon[12] = Utils.Scale(P11, ScaleFactor);
                polygon[13] = Utils.Scale(P10, ScaleFactor);
            }
            else
            {
                polygon[12] = Utils.Scale(P10, ScaleFactor);
                polygon[13] = Utils.Scale(P11, ScaleFactor);
            }

            double outerRadius = InnerRadius + Connectors[0].Width;
            for (int i = 0; i < 10; i++)
            {
                alpha += sweep;
                polygon[14 + i] = Utils.Scale(Utils.GetPoint(circleCenter, alpha, outerRadius), ScaleFactor);
            }
            polygon[24] = polygon[0];
            return polygon;
        }

        /// <summary>
        /// Get a scaled polygon from the connector end points. The 4 corner points of the curved shape will be converted to a polygon approximating the curved shape after scaling.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon</returns>
        protected PointF[] GetScaledCurvePolygon(SizeF ScaleFactor)
        {
            //return GetScaledCurvePolygon(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
            return GetScaledCurvePolygon(Connectors[0].DrawP0, Connectors[0].DrawP1, Connectors[1].DrawP0, Connectors[1].DrawP1, ScaleFactor);
        }
        #endregion Draw Support Curve Shape

        #region Draw Support S-Shape

        /// <summary>
        /// Calculate 2 additional points between the 2 given connector points for creating an s-shaped path via Bezier.
        /// </summary>
        /// <param name="P0">Point of the first connector, CenterP, EndP0 or EndP1</param>
        /// <param name="P1">Point of the second connector, CenterP, EndP0 or EndP1</param>
        /// <returns>Point array with the 4 points in it, the given points and the 2 in between.</returns>
        protected PointF[] GetSPathFitPoints(PointF P0, PointF P1)
        {
            double offs = Length / 3;
            PointF p0_o = Utils.GetPoint(P0, Connectors[0].Angle + Utils.RIGHT_ANGLE_RADIAN, offs);
            PointF p1_o = Utils.GetPoint(P1, Connectors[1].Angle + Utils.RIGHT_ANGLE_RADIAN, -offs);

            return new PointF[] { P0, p0_o, p1_o, P1 };
        }

        /// <summary>
        /// Calculates 2 additional points in between the connector center points for creating an s-shaped path via Bezier by calling GetSPathFitPoints.
        /// </summary>
        /// <returns>Point array with the 4 points in it, the center points and the 2 in between.</returns>
        protected PointF[] GetSPathFitCenterPoints()
        {
            return GetSPathFitPoints(Connectors[0].CenterP, Connectors[1].CenterP);
        }

        /// <summary>
        /// Calculates the 2 point arrays for both end point sides for creating an s-shaped path via Bezier by calling GetSPathFitPoints.
        /// </summary>
        /// <returns>Point arrays with the 4 points in it, the end points and the 2 in between.</returns>
        protected PointF[][] GetSPathFitEndPoints()
        {
            PointF[][] result = new PointF[2][];
            //result[0] = GetSPathFitPoints(Connectors[0].EndP0, Connectors[1].EndP0);
            //result[1] = GetSPathFitPoints(Connectors[0].EndP1, Connectors[1].EndP1); ;
            result[0] = GetSPathFitPoints(Connectors[0].DrawP0, Connectors[1].DrawP0);
            result[1] = GetSPathFitPoints(Connectors[0].DrawP1, Connectors[1].DrawP1); ;
            return result;
        }

        /// <summary>
        /// Calculates the bezier curve point array between Start and End using the FitPoints provided by GetSPathFitPoints or similar.
        /// The curves are calculated between 0 and 1, so Start, End need to be normalized to this range and Step has to be a fraction of the difference.
        /// </summary>
        /// <param name="FitPoints">4 point array from GetSPathFitPoints or similar with a start point, an end point and 2 in between.</param>
        /// <param name="Start">Normalized start value between 0.0 and End value. 0 corresponds to FitPoints[0].</param>
        /// <param name="End">Normalized end value between Start and 1.0. 1 corresponds to FitPoints[3].</param>
        /// <param name="Step">Iteration step as fraction of the difference between Start and End. It determines the number of points in the resulting array. </param>
        /// <returns>Point array calculated along the bezier curve from Start to End.</returns>
        protected PointF[] GetBezierPoints(PointF[] FitPoints, double Start, double End, double Step)
        {
            List<PointF> points = new List<PointF>();
            for (double t = Start; t < End+Step/2; t += Step)
            {
                double t2 = t * t;
                double t3 = t2 * t;
                double o = 1 - t;
                double o2 = o * o;
                double o3 = o2 * o;
                double x = FitPoints[0].X * o3 + FitPoints[1].X * 3 * t * o2 + FitPoints[2].X * 3 * o * t2 + FitPoints[3].X * t3;
                double y = FitPoints[0].Y * o3 + FitPoints[1].Y * 3 * t * o2 + FitPoints[2].Y * 3 * o * t2 + FitPoints[3].Y * t3;
                points.Add(new PointF((float)x, (float)y));
            }
            return points.ToArray();
        }

        /// <summary>
        /// Build a scaled graphics path from the connector end points. The 4 corner points of the s-shape will be converted to a path of 2 beziers and 2 lines after scaling.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled GraphicsPath to draw the s-shape.</returns>
        protected GraphicsPath GetScaledSPath(SizeF ScaleFactor)
        {
            PointF[][] p = GetSPathFitEndPoints();
            PointF[] p0 = Utils.Scale(p[0], ScaleFactor);
            PointF[] p1 = Utils.Scale(p[1], ScaleFactor);

            GraphicsPath path = new GraphicsPath();

            path.AddLine(p0[0], p1[0]);
            path.AddBezier(p1[0], p1[1], p1[2], p1[3]);
            path.AddLine(p1[3], p0[3]);
            path.AddBezier(p0[3], p0[2], p0[1], p0[0]);

            return path;
        }

        /// <summary>
        /// Calculate a scaled s-path polygon of with the connectors widths from Start to End.
        /// </summary>
        /// <param name="FitPoints">Dual fit points array from the connectors end points calculated via GetSPathFitEndPoints.</param>
        /// <param name="Start">Normalized start value between 0.0 and End value. 0 corresponds to FitPoints[0].</param>
        /// <param name="End">Normalized end value between Start and 1.0. 1 corresponds to FitPoints[3].</param>
        /// <param name="Step">Iteration step as fraction of the difference between Start and End. It determines the number of points in the resulting array. </param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon of the partial or full s-shape.</returns>
        protected PointF[] GetScaledSPolygon(PointF[][] FitPoints, double Start, double End, double Step, SizeF ScaleFactor)
        {
            PointF[] p0=GetBezierPoints(FitPoints[0], Start, End, Step);
            PointF[] p1=GetBezierPoints(FitPoints[1], Start, End, Step);
            Array.Reverse(p1);

            PointF[] poly = new PointF[p0.Length + p1.Length + 1];
            Array.Copy(p0, poly, p0.Length);
            Array.Copy(p1, 0, poly, p0.Length, p1.Length);
            poly[poly.Length - 1] = poly[0];

            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Calculate a scaled full s-path polygon of with the connectors widths.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon of the full s-shape.</returns>
        protected PointF[] GetScaledSPolygon(SizeF ScaleFactor)
        {
            return GetScaledSPolygon(GetSPathFitEndPoints(), 0, 1, 0.1, ScaleFactor);
        }

        #endregion Draw Support S-Shape

        #endregion Private and Protected Methods

        #region Public Methods
        /// <summary>
        /// Draw method of this class that is overwritten from the BaseElement class. It calls the method first and then performs drawing of this shape depending on the DrawMode.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            base.Draw(grfx, ScaleFactor, DrawMode);

            switch (DrawMode)
            {
                case DrawMode.Outline:
                    switch (shape)
                    {
                        case ShapeType.Straight:
                            grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledStraightPolygon(ScaleFactor));
                            break;

                        case ShapeType.Curve:
#if DEBUG_DRAW
                            grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledCurvePolygon(ScaleFactor));
                            Utils.DrawPointCircle(grfx, circleCenter, ScaleFactor, Color.Black, 3, 10);
#else
                            grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledCurvePath(ScaleFactor));
#endif
                            break;

                        case ShapeType.S_Shape:
                            grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledSPath(ScaleFactor));
                            break;
                    }
#if DEBUG_DRAW
                    Utils.DrawPointCircle(grfx, Connectors[0].EndP0, ScaleFactor, Color.White, 3, 10);
                    Utils.DrawPointCircle(grfx, Connectors[0].EndP1, ScaleFactor, Color.DarkGray, 3, 10);
                    Utils.DrawPointCircle(grfx, Connectors[1].EndP0, ScaleFactor, Color.DarkRed, 3, 10);
                    Utils.DrawPointCircle(grfx, Connectors[1].EndP1, ScaleFactor, Color.DarkBlue, 3, 10);
#endif
                    break;

                case DrawMode.Background:
                    switch (shape)
                    {
                        case ShapeType.Straight:
                            grfx.FillPolygon(new SolidBrush(BackgroundColor), GetScaledStraightPolygon(ScaleFactor));
                            break;

                        case ShapeType.Curve:
                            grfx.FillPath(new SolidBrush(BackgroundColor), GetScaledCurvePath(ScaleFactor));
                            break;

                        case ShapeType.S_Shape:
                            grfx.FillPath(new SolidBrush(BackgroundColor), GetScaledSPath(ScaleFactor));
                            break;
                    }
                    break;

                case DrawMode.BaseLayer:
                    {
                        Color drawColor = GetDrawColor();

                        switch (shape)
                        {
                            case ShapeType.Straight:
                                grfx.FillPolygon(new SolidBrush(drawColor), GetScaledStraightPolygon(ScaleFactor));
                                break;

                            case ShapeType.Curve:
                                grfx.FillPath(new SolidBrush(drawColor), GetScaledCurvePath(ScaleFactor));
                                break;

                            case ShapeType.S_Shape:
                                grfx.FillPath(new SolidBrush(drawColor), GetScaledSPath(ScaleFactor));
                                break;
                        }
                    }
                    break;


                case DrawMode.TopLayer:
                    break;
            }
        }

        /// <summary>
        /// Calculates the path length in the center of the shape between the 2 connector center points.
        /// </summary>
        /// <returns>Path length in the center of the shape in mm.</returns>
        public double GetCenterPathLength()
        {
            switch (Shape)
            {
                case ShapeType.Straight:
                    return Utils.GetDistance(Connectors[1].CenterP, Connectors[0].CenterP);

                case ShapeType.Curve:
                    return Math.Abs((InnerRadius + Connectors[0].Width / 2) * curveAngle);

                case ShapeType.S_Shape:
                    return Utils.GetDistance(GetBezierPoints(GetSPathFitCenterPoints(), 0, 1, 0.1f));
            }
            return 0;
        }

        /// <summary>
        /// Returns the reference to this object, if the given point is inside of its area or null if outside.
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <returns>Reference to this object, if inside.</returns>
        public override object IsInside(PointF P)
        {
            switch (shape)
            {
                case ShapeType.Straight:
                    return Utils.IsInPolygon(P, GetScaledStraightPolygon(new SizeF(1, 1))) ? this : null;

                case ShapeType.Curve:
                    return Utils.IsInPolygon(P, GetScaledCurvePolygon(new SizeF(1, 1))) ? this : null;

                case ShapeType.S_Shape:
                    return Utils.IsInPolygon(P, GetScaledSPolygon(new SizeF(1, 1))) ? this : null;
            }
            return null;
        }


        /// <summary>
        /// Recalculates all geometrical parameter of this shape by calling BaseConnectorChangeEvent.
        /// </summary>
        public void Update()
        {
            BaseConnectorChangeEvent(Connectors[0]);
        }


        #endregion Public Methods


        #region Public Properties
        /// <summary>
        /// Gets or sets the length of a straight lane or line. This field is only used for the straight shape.
        /// </summary>
        public double Length
        {
            get { return length; }
            set 
            { 
                length = value;
                Update();
            }
        }

        /// <summary>
        /// Gets the shape of this instance.
        /// </summary>
        public ShapeType Shape
        {
            get { return shape; }
        }

        /// <summary>
        /// Gets or sets the inner radius of a curved shape. This field is only used for the curved shape.
        /// </summary>
        public double InnerRadius
        {
            get { return innerRadius; }
            set 
            { 
                innerRadius = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the sweeping angle of the curve piece. This field is only used for the curved shape.
        /// </summary>
        public double CurveAngle
        {
            get { return curveAngle; }
            set 
            { 
                curveAngle = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the center point of the circle of a curved element. This field is only used for the curved shape.
        /// </summary>
        public PointF CircleCenter
        {
            get { return circleCenter;  }
            set
            {
                circleCenter = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the offset of an s-shaped center points from the straight one. This field is only used for the s-shape.
        /// </summary>
        public double SOffset
        {
            get { return sOffset;  }
            set
            {
                sOffset = value;
                Update();
            }
        }

        #endregion Public Properties

    }



}
