// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================


// Activate to visualize the point arrays recalculated in the CornerRound.Update method. To vizualize, DEBUG_CORNER_ROUND_UPDATE_POINTS has to be enabled in both: Intersection.cs and CornerRound.sc
//#define DEBUG_CORNER_ROUND_UPDATE_POINTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StreetMaker
{
    /// <summary>
    /// Class to handle corner rounds in intersections. These objects are attached between 2 perpendicular lanes to smooth the right angle. 
    /// It not only adds the quarter round pavement and a round line, but also draws over the right angle lines to create a good transistion.
    /// </summary>
    public class CornerRound
    {
        private const int OFFSET = 0;

        #region Private Fields
        /// <summary>Coordinates of the intersection point between the perpendicular lanes to round.</summary>
        private PointF cornerP;
        /// <summary>Rotational angle of this object in Radian.</summary>
        private double angle;
        /// <summary>Radius of the outer rounding between the perpendicular lanes.</summary>
        private double radius;
        /// <summary>Width of the lanes.</summary>
        private double laneWidth;
        /// <summary>Width of the lane limit line.</summary>
        private double lineWidth;

        /// <summary>Circle center point of the rounding.</summary>
        private PointF centerP;
        /// <summary>Inner rectangle around the inner radius of the shoulder line. Used for creating the inner graphics path.</summary>
        private RectangleF innerRect;
        /// <summary>Outer rectangle around the outer radius of the shoulder line. Used for creating the outer graphics path.</summary>
        private RectangleF outerRect;
        /// <summary>Open polygon containing the points for the base layer drawing graphics path.</summary>
        private PointF[] baseP;
        /// <summary>First part of the open polygon containing the points for the top layer drawing graphics path.</summary>
        private PointF[] topP0;
        /// <summary>Second part of the open polygon containing the points for the top layer drawing graphics path.</summary>
        private PointF[] topP1;
        #endregion Private Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the CornerRound class to the passed parameter.
        /// </summary>
        /// <param name="Radius">Radius of the outer rounding between the perpendicular lanes.</param>
        /// <param name="LaneWidth">Width of the lanes.</param>
        /// <param name="LineWidth">Width of the lane limit line.</param>
        public CornerRound(double Radius, double LaneWidth, double LineWidth)
        {
            radius = Radius;
            laneWidth = LaneWidth;
            lineWidth = LineWidth;
            baseP = new PointF[5];
            topP0 = new PointF[4];
            topP1 = new PointF[2];
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Updates internally used fields from the passed parameter and calculating geometry values. This method pre-calculates polygons for drawing the base layer and for the top layer.
        /// </summary>
        /// <param name="CornerP">Coordinates of the intersection point between the perpendicular lanes to round.</param>
        /// <param name="Angle">Rotational angle of this object.</param>
        /// <param name="OuterBorder">Outer border polygon of the intersection object to limit drawing to.</param>
        public void Update(PointF CornerP, double Angle, PointF LineEndP)
        {
            cornerP = CornerP;
            angle = Angle;
            centerP = Utils.GetPoint(cornerP, Utils.LimitRadian(angle + Utils.RIGHT_ANGLE_RADIAN / 2), -radius * Math.Sqrt(2));

            float iR = (float)(radius - BaseElement.DRAW_OFFS_WIDTH);
            float iD = 2 * iR;
            innerRect = new RectangleF(new PointF(centerP.X - iR, centerP.Y - iR), new SizeF(iD, iD));
            float oR = (float)(radius + lineWidth + BaseElement.DRAW_OFFS_WIDTH);
            float oD = 2 * oR;
            outerRect = new RectangleF(new PointF(centerP.X - oR, centerP.Y - oR), new SizeF(oD,oD));

            baseP[0] = Utils.GetPoint(cornerP, angle - Utils.RIGHT_ANGLE_RADIAN, radius);
            baseP[0] = Utils.GetPoint(baseP[0], angle, -BaseElement.DRAW_OFFS_WIDTH);
            baseP[1] = Utils.GetPoint(baseP[0], angle, lineWidth + OFFSET + 2 * BaseElement.DRAW_OFFS_WIDTH+1);
            baseP[2] = Utils.GetPoint(cornerP, Utils.LimitRadian(angle + Utils.RIGHT_ANGLE_RADIAN / 2), (lineWidth + OFFSET + BaseElement.DRAW_OFFS_WIDTH+1)*Math.Sqrt(2));
            baseP[4] = Utils.GetPoint(cornerP, angle, -radius);
            baseP[4] = Utils.GetPoint(baseP[4], angle + Utils.RIGHT_ANGLE_RADIAN, -BaseElement.DRAW_OFFS_WIDTH);
            baseP[3] = Utils.GetPoint(baseP[4], angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + OFFSET + 2 * BaseElement.DRAW_OFFS_WIDTH+1);

            topP0[3] = Utils.GetPoint(cornerP, angle, -(radius + BaseElement.DRAW_OFFS_WIDTH));
            topP0[2] = Utils.GetPoint(LineEndP, angle, BaseElement.DRAW_OFFS_WIDTH);
            topP0[0] = Utils.GetPoint(topP0[3], angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + BaseElement.DRAW_OFFS_WIDTH);
            topP0[1] = Utils.GetPoint(topP0[2], angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + BaseElement.DRAW_OFFS_WIDTH);

            topP1[1] = Utils.GetPoint(cornerP, angle - Utils.RIGHT_ANGLE_RADIAN, radius);
            topP1[0] = Utils.GetPoint(topP1[1], angle, lineWidth + 2 * BaseElement.DRAW_OFFS_WIDTH);
        }

#if DEBUG_CORNER_ROUND_UPDATE_POINTS
        public void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            Utils.DrawPointCircle(grfx, baseP[0], ScaleFactor, Color.Red, 2, 2);
            Utils.DrawPointCircle(grfx, baseP[1], ScaleFactor, Color.Green, 2, 2);
            Utils.DrawPointCircle(grfx, baseP[2], ScaleFactor, Color.Blue, 2, 2);
            Utils.DrawPointCircle(grfx, baseP[3], ScaleFactor, Color.Brown, 2, 2);
            Utils.DrawPointCircle(grfx, baseP[4], ScaleFactor, Color.Aqua, 2, 2);
        }
#endif

        /// <summary>
        /// Returns a drawing path of the base layer sclaed to the scaling factor.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>GraphicsPath object for the base layer.</returns>
        public GraphicsPath GetScaledBasePath(SizeF ScaleFactor)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] p = Utils.Scale(baseP, ScaleFactor);
            path.AddLine(p[0], p[1]);
            path.AddLine(p[1], p[2]);
            path.AddLine(p[2], p[3]);
            path.AddLine(p[3], p[4]);
            path.AddArc(Utils.Scale(innerRect, ScaleFactor), (float)Utils.ToDegree(Utils.LimitRadian(-angle)), 90);

            return path;
        }


        /// <summary>
        /// Returns a drawing path of the top layer sclaed to the scaling factor.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>GraphicsPath object for the top layer.</returns>
        public GraphicsPath GetScaledTopPath(SizeF ScaleFactor)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] p0 = Utils.Scale(topP0, ScaleFactor);
            PointF[] p1 = Utils.Scale(topP1, ScaleFactor);

            path.AddLine(p0[0], p0[1]);
            path.AddLine(p0[1], p0[2]);
            path.AddLine(p0[2], p0[3]);
            path.AddArc(Utils.Scale(innerRect, ScaleFactor), (float)Utils.ToDegree(Utils.LimitRadian(-angle)), 90);
            path.AddLine(p1[0], p1[1]);
            path.AddArc(Utils.Scale(outerRect, ScaleFactor), (float)Utils.ToDegree(Utils.LimitRadian(Utils.RIGHT_ANGLE_RADIAN - angle)), -90);

            return path;
        }

        /// <summary>
        /// Caclulate the left point of the curved shoulder line extending from the passed reference point. This method is used to extend for instance a stop line directly to the round shoulder line.
        /// </summary>
        /// <param name="RefPoint">Reference point on the line to be extended.</param>
        /// <returns>Left limit line point extended from the reference point.</returns>
        public PointF GetLeftLinePoint(PointF RefPoint)
        {
            double dist = Utils.GetDistance(RefPoint, cornerP);

            if (dist > radius)
                return Utils.GetPoint(RefPoint, angle, lineWidth);

            if (dist < 1e-4) 
                return Utils.GetPoint(cornerP, angle, -(radius- lineWidth));

            double a = radius - dist;
            double b = Math.Sqrt(radius * radius - a * a);
            
            return Utils.GetPoint(RefPoint, angle, -(radius-b-lineWidth));
        }

        /// <summary>
        /// Caclulate the right point of the curved shoulder line extending from the passed reference point. This method is used to extend for instance a stop line directly to the round shoulder line.
        /// </summary>
        /// <param name="RefPoint">Reference point on the line to be extended.</param>
        /// <returns>Right limit line point extended from the reference point.</returns>
        public PointF GetRightLinePoint(PointF RefPoint)
        {
            double dist = Utils.GetDistance(RefPoint, cornerP);

            if (dist > radius)
                return Utils.GetPoint(RefPoint, angle - Utils.RIGHT_ANGLE_RADIAN, -lineWidth);

            if (dist < 1e-4)
                return Utils.GetPoint(cornerP, angle-Utils.RIGHT_ANGLE_RADIAN, +(radius - lineWidth));

            double a = radius - dist;
            double b = Math.Sqrt(radius * radius - a * a);

            return Utils.GetPoint(RefPoint, angle - Utils.RIGHT_ANGLE_RADIAN, +(radius - b - lineWidth));
        }

        /// <summary>
        /// Gets a scaled polygon of the left line of the corner round that covers the area where the shoulder line of the lane was.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon to cover the shoulder line left of the corner round.</returns>
        public PointF[] GetScaledBaseLeftLine(SizeF ScaleFactor)
        {
            PointF[] poly = new PointF[5];

            poly[0] = baseP[2];
            poly[1] = baseP[3];
            poly[2] = baseP[4];
            poly[3] = Utils.GetPoint(poly[0], angle + Utils.RIGHT_ANGLE_RADIAN, -(lineWidth + OFFSET));
            poly[4] = poly[0];

            return Utils.Scale(poly, ScaleFactor);
        }

        /// <summary>
        /// Gets a scaled polygon of the left line of the corner round that covers the area where the shoulder line of the lane was.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon to cover the shoulder line left of the corner round.</returns>
        public PointF[] GetScaledBaseTopLine(SizeF ScaleFactor)
        {
            PointF[] poly = new PointF[5];

            poly[0] = baseP[2];
            poly[1] = baseP[1];
            poly[2] = baseP[0];
            poly[3] = Utils.GetPoint(poly[0], angle, -(lineWidth + OFFSET));
            poly[4] = poly[0];

            return Utils.Scale(poly, ScaleFactor);
        }
#endregion Public Methods

#region Public Properties

        /// <summary>
        /// Coordinates of the intersection point between the perpendicular lanes to round.
        /// </summary>
        public PointF CornerP
        {
            get { return cornerP; }
        }

        /// <summary>
        /// Rotational angle of this object in Radian.
        /// </summary>
        public double Angle
        {
            get { return angle; }
        }

        /// <summary>
        /// Circle center point of the rounding.
        /// </summary>
        public PointF CenterP
        {
            get { return centerP; }
        }
#endregion Public Properties

    }
}
