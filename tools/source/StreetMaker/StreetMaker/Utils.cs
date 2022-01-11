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

using System.Diagnostics;

namespace StreetMaker
{
    /// <summary>
    /// Collection of static calculation methods, mostly simple trigonometry.
    /// </summary>
    public class Utils
    {
        #region Public Constants
        /// <summary>Constant definition of a right angle in degrees.</summary>
        public const double RIGHT_ANGLE_DEGREE = 90;
        /// <summary>Constant definition of a right angle in radian.</summary>
        public const double RIGHT_ANGLE_RADIAN = Math.PI / 2;
        #endregion Public Constants

        #region Public Static Methods
        #region Angle Conversions
        /// <summary>
        /// Converts the given value in degree into radian. 
        /// </summary>
        /// <param name="Degree">Value to be converted.</param>
        /// <returns>Conversion result in radian.</returns>
        public static double ToRadian(double Degree)
        {
            return Degree * Math.PI / 180;
        }

        /// <summary>
        /// Converts the given value in radian into degree. 
        /// </summary>
        /// <param name="Radian">Value to be converted.</param>
        /// <returns>Conversion result in degree.</returns>
        public static double ToDegree(double Radian)
        {
            return Radian * 180 / Math.PI;
        }

        /// <summary>
        /// Limit an angle in radian to the range -Pi to +Pi by remapping exceeding values back into this range.
        /// </summary>
        /// <param name="Radian">Angle in radian in full range.</param>
        /// <returns>Radian value between -Pi and +Pi</returns>
        public static double LimitRadian(double Radian)
        {
            if (Radian > Math.PI)
                return Radian - 2 * Math.PI;
            else if (Radian <= -Math.PI)
                return Radian + 2 * Math.PI;
            return Radian;
        }
        #endregion Angle Conversions

        #region Scale
        /// <summary>
        /// Scales the given point by multiplying its coordinates with the scale factor values.
        /// </summary>
        /// <param name="P">Point coordinates to be scaled.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled point.</returns>
        public static PointF Scale(PointF P, SizeF ScaleFactor)
        {
            return new PointF(P.X * ScaleFactor.Width, P.Y * ScaleFactor.Height);
        }

        /// <summary>
        /// Scales the given size by multiplying its width and height with the scale factor values.
        /// </summary>
        /// <param name="S">Size to be scaled.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled size.</returns>
        public static SizeF Scale(SizeF S, SizeF ScaleFactor)
        {
            return new SizeF(S.Width * ScaleFactor.Width, S.Height * ScaleFactor.Height);
        }

        /// <summary>
        /// Scales the given rectangle by multiplying its coordinates and width and height with the scale factor values.
        /// </summary>
        /// <param name="R">Rectangle to be scaled.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled rectangle.</returns>
        public static RectangleF Scale(RectangleF R, SizeF ScaleFactor)
        {
            return new RectangleF(Utils.Scale(R.Location, ScaleFactor), Utils.Scale(R.Size, ScaleFactor) );
        }


        /// <summary>
        /// Scales the given polygon by multiplying all its point coordinates with the scale factor values.
        /// </summary>
        /// <param name="Poly">Polygon to be scaled</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled polygon.</returns>
        public static PointF[] Scale(PointF[] Poly, SizeF ScaleFactor)
        {
            PointF[] polygon = new PointF[Poly.Length];
            for (int i = 0; i < Poly.Length; i++)
                polygon[i] = Utils.Scale(Poly[i], ScaleFactor);
            return polygon;
        }
        #endregion Scale

        #region GetDistance
        /// <summary>
        /// Calculate the distance between two points.
        /// </summary>
        /// <param name="X0">X coordinates of the first point.</param>
        /// <param name="Y0">Y coordinates of the first point.</param>
        /// <param name="X1">X coordinates of the second point.</param>
        /// <param name="Y1">Y coordinates of the second point.</param>
        /// <returns>Calculated distance between the 2 points.</returns>
        public static double GetDistance(double X0, double Y0, double X1, double Y1)
        {
            double dx = X1 - X0;
            double dy = Y1 - Y0;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculate the distance between two points.
        /// </summary>
        /// <param name="P0">Point coordinates of the first point.</param>
        /// <param name="P1">Point coordinates of the second point.</param>
        /// <returns>Calculated distance between the 2 points.</returns>
        public static double GetDistance(PointF P0, PointF P1)
        {
            return GetDistance(P0.X, P0.Y, P1.X, P1.Y);
            //double dx = P1.X - P0.X;
            //double dy = P1.Y - P0.Y;
            //return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calculate the sum of all the distances of neihboring points. 
        /// </summary>
        /// <param name="Points"></param>
        /// <returns>Sum of distances.</returns>
        public static double GetDistance(PointF[] Points)
        {
            double dist = 0;
            for (int i = 0; i < Points.Length - 1; i++)
                dist += GetDistance(Points[i], Points[i + 1]);
            return dist;
        }
        #endregion GetDistance

        #region GetPoint
        /// <summary>
        /// Calculates the coordinates of the point from a reference point, an angle and the distance from the reference point.
        /// </summary>
        /// <param name="RefPoint">Reference point to start from.</param>
        /// <param name="Angle">Angle of the direction to calculate the second point.</param>
        /// <param name="Distance">Distance from the reference point.</param>
        /// <returns>Calculated point coordinates.</returns>
        public static PointF GetPoint(PointF RefPoint, double Angle, double Distance)
        {
            double dx = (Distance * Math.Sin(Angle));
            double dy = (Distance * Math.Cos(Angle));
            return new PointF((float)(RefPoint.X + dx), (float)(RefPoint.Y + dy));
        }

        /// <summary>
        /// Caclulates the coordinates of a point on the circumference of a circle given by center point, a starting point on the circumference and a distance on the circumference.
        /// </summary>
        /// <param name="CircleCenter">Center point of the circle.</param>
        /// <param name="RefPoint">Reference point on the circumference of the circle.</param>
        /// <param name="Distance">Distance from the reference point on the circumference.</param>
        /// <returns>Calculated point coordinates.</returns>
        public static PointF GetPoint(PointF CircleCenter, PointF RefPoint, double Distance)
        {
            double radius = GetDistance(CircleCenter, RefPoint);
            double alpha = GetAngle(CircleCenter, RefPoint);
            double beta = Distance / radius;
            return GetPoint(CircleCenter, alpha - beta, radius);
        }
        #endregion GetPoint

        #region GetAngle
        /// <summary>
        /// Calculates the angle between the 2 given points.
        /// </summary>
        /// <param name="X0">X coordinates of the first point.</param>
        /// <param name="Y0">Y coordinates of the first point.</param>
        /// <param name="X1">X coordinates of the second point.</param>
        /// <param name="Y1">Y coordinates of the second point.</param>
        /// <returns>Angle between these points in radian</returns>
        public static double GetAngle(double X0, double Y0, double X1, double Y1)
        {
            double dx = X1 - X0;
            double dy = Y1 - Y0;

            if ((dx == 0) && (dy == 0))
                return 0;

            if (Math.Abs(dx) < Math.Abs(dy))
            {
                double atan = Math.Atan(dx / dy);
                if (dy>0)
                    return atan;
                if (dx > 0)
                    return (atan + Math.PI);
                else
                    return (atan - Math.PI);
            }
            else
            {
                double atan = Math.Atan(dy / dx);
                if (dx > 0)
                    return -(atan - Math.PI / 2);
                else
                    return -(atan + Math.PI / 2);
            }
        }

        /// <summary>
        /// Calculates the angle between the 2 given points.
        /// </summary>
        /// <param name="P0">Point coordinates of the first point.</param>
        /// <param name="P1">Point coordinates of the second point.</param>
        /// <returns>Angle between these points in radian</returns>
        public static double GetAngle(PointF P0, PointF P1)
        {
            return GetAngle(P0.X,P0.Y,P1.X,P1.Y);
        }
        #endregion GetAngle


        /// <summary>
        /// Calculates the size between the 2 given points.
        /// </summary>
        /// <param name="P0">Point coordinates of the first point.</param>
        /// <param name="P1">Point coordinates of the second point.</param>
        /// <returns>Size between these points.</returns>
        public static SizeF GetSize(PointF P0, PointF P1)
        {
            return new SizeF(P0.X - P1.X, P0.Y - P1.Y);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
        /// Returns true, if the given point is inside of the polygon.
        /// </summary>
        /// <param name="P">Point to be checked.</param>
        /// <param name="Polygon">Polygon to be checked against.</param>
        /// <returns>True, if the point is located inside the ploygon. </returns>
        public static bool IsInPolygon(PointF P, PointF[] Polygon)
        {
            bool result = false;
            int j = Polygon.Count() - 1;
            for (int i = 0; i < Polygon.Count(); i++)
            {
                if (Polygon[i].Y < P.Y && Polygon[j].Y >= P.Y || Polygon[j].Y < P.Y && Polygon[i].Y >= P.Y)
                {
                    if (Polygon[i].X + (P.Y - Polygon[i].Y) / (Polygon[j].Y - Polygon[i].Y) * (Polygon[j].X - Polygon[i].X) < P.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Draw a circle around a point.
        /// </summary>
        /// <param name="grfx">Graphics object to draw with.</param>
        /// <param name="P">Point coordinates to draw a circle around.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="Color">Color of the circle.</param>
        /// <param name="PenWidth">Width of the circle line.</param>
        /// <param name="Radius">Radius of the circle.</param>
        public static void DrawPointCircle(Graphics grfx, PointF P, SizeF ScaleFactor, Color Color, float PenWidth, float Radius)
        {
            PointF p = Scale(P, ScaleFactor);
            SizeF r = new SizeF(Radius * ScaleFactor.Width, Radius * ScaleFactor.Height);
            grfx.DrawEllipse(new Pen(Color, PenWidth), p.X - r.Width, p.Y - r.Height, 2 * r.Width, 2 * r.Height);
        }




        /// <summary>
        ///  Draw a rotated string at a particular position with center alignment.
        ///  see: http://csharphelper.com/blog/2014/07/draw-rotated-text-in-c/
        /// </summary>
        /// <param name="grfx">Graphics object to draw with.</param>
        /// <param name="Text">String to draw</param>
        /// <param name="CenterP">Center point of the string to draw.</param>
        /// <param name="RadianAngle">Angle of the text in Radian.</param>
        /// <param name="Font"></param>
        /// <param name="Color"></param>
        public static void DrawText(Graphics grfx, string Text, PointF CenterP, double RadianAngle, Font Font, Color Color)
        {
            // Save the graphics state.
            GraphicsContainer container = grfx.BeginContainer();
            //GraphicsState state = grfx.Save();
            //grfx.ResetTransform();

            // Rotate.
            grfx.RotateTransform((float)Utils.ToDegree(Utils.RIGHT_ANGLE_RADIAN-RadianAngle), MatrixOrder.Append);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            grfx.TranslateTransform(CenterP.X, CenterP.Y, MatrixOrder.Append);

            // setup center alignments
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            // Draw the text at the origin.
            grfx.DrawString(Text, Font, new SolidBrush(Color), 0, 0, format);

            // Restore the graphics state.
            //grfx.Restore(state);
            grfx.EndContainer(container);
        }

        #endregion Public Static Methods

    }
}
