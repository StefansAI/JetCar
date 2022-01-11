using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StreetMaker
{
    public class S_Element : BaseElement
    {
        protected double length;
        protected double s_Offset;

        public S_Element(double Width, Color Color, Color OutlineColor, double OutlinePenWidth, Color BackgroundColor, double Length, double S_Offset) : base(2, Width, Color, OutlineColor, OutlinePenWidth, BackgroundColor)
        {
            this.length = Length;
            this.s_Offset = S_Offset;
        }

        protected override void HandleConnectorEvent(Connector sender)
        {

            Connector updateConnector = sender == Connectors[0] ? Connectors[1] : Connectors[0];

            if (EditModeMove == true)
            {
                PointF p = Utils.GetPoint(sender.CenterP, sender.Angle, +s_Offset);
                updateConnector.UpdateAngleAndCenterP(sender.Angle, Utils.GetPoint(p, sender.Angle + Utils.RIGHT_ANGLE, Length));
            }
            else
            {
                double h = Utils.GetDistance(Connectors[0].CenterP, Connectors[1].CenterP);
                double alpha = Utils.GetAngle(Connectors[0].CenterP, Connectors[1].CenterP);
                length = h * Math.Sin(alpha);
                s_Offset = h * Math.Cos(alpha);
            }
        }


        protected GraphicsPath GetScaledSPath(SizeF ScaleFactor)
        {
            return GetScaledSPath(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
        }

        protected GraphicsPath GetScaledSPath(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            GraphicsPath path = new GraphicsPath();
            double angle_00_01 = Utils.LimitRadian(Utils.GetAngle(P00, P01));
            double angle_10_11 = Utils.LimitRadian(Utils.GetAngle(P10, P11));
            double offs = Length / 3;
            PointF p00 = Utils.Scale(P00, ScaleFactor);
            PointF p01 = Utils.Scale(P01, ScaleFactor);
            PointF p10 = Utils.Scale(P10, ScaleFactor);
            PointF p11 = Utils.Scale(P11, ScaleFactor);
            PointF p00_o = Utils.Scale(Utils.GetPoint(P00, angle_00_01 + Utils.RIGHT_ANGLE, offs), ScaleFactor);
            PointF p01_o = Utils.Scale(Utils.GetPoint(P01, angle_00_01 + Utils.RIGHT_ANGLE, offs), ScaleFactor);
            PointF p10_o = Utils.Scale(Utils.GetPoint(P10, angle_10_11 + Utils.RIGHT_ANGLE, -offs), ScaleFactor);
            PointF p11_o = Utils.Scale(Utils.GetPoint(P11, angle_10_11 + Utils.RIGHT_ANGLE, -offs), ScaleFactor);
            //Debug.WriteLine("Connectors[0].Angle:" + Connectors[0].Angle.ToString("F3") + "   angle_00_01:" + angle_00_01.ToString("F3") + "   Connectors[1].Angle:" + Connectors[1].Angle.ToString("F3") + "   angle_10_11:" + angle_10_11.ToString("F3") + "   angle_delta:" + angle_delta.ToString("F3"));

            path.AddLine(p00, p01);
            path.AddBezier(p01, p01_o, p11_o, p11);
            path.AddLine(p11, p10);
            path.AddBezier(p10, p10_o, p00_o, p00);
            return path;
        }

        protected PointF[] GetScaledSPolygon(SizeF ScaleFactor)
        {
            return GetScaledSPolygon(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
        }

        protected PointF[] GetScaledSPolygon(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            double angle_00_01 = Utils.LimitRadian(Utils.GetAngle(P00, P01));
            double angle_10_11 = Utils.LimitRadian(Utils.GetAngle(P10, P11));
            double offs = Length / 3;
            PointF p00 = Utils.Scale(P00, ScaleFactor);
            PointF p01 = Utils.Scale(P01, ScaleFactor);
            PointF p10 = Utils.Scale(P10, ScaleFactor);
            PointF p11 = Utils.Scale(P11, ScaleFactor);
            PointF p00_o = Utils.Scale(Utils.GetPoint(P00, angle_00_01 + Utils.RIGHT_ANGLE, offs), ScaleFactor);
            PointF p01_o = Utils.Scale(Utils.GetPoint(P01, angle_00_01 + Utils.RIGHT_ANGLE, offs), ScaleFactor);
            PointF p10_o = Utils.Scale(Utils.GetPoint(P10, angle_10_11 + Utils.RIGHT_ANGLE, -offs), ScaleFactor);
            PointF p11_o = Utils.Scale(Utils.GetPoint(P11, angle_10_11 + Utils.RIGHT_ANGLE, -offs), ScaleFactor);

            PointF[] polygon = new PointF[9];
            polygon[0] = p00;
            polygon[1] = p01;
            polygon[2] = p01_o;
            polygon[3] = p11_o;
            polygon[4] = p11;
            polygon[5] = p10;
            polygon[6] = p10_o;
            polygon[7] = p00_o;
            polygon[8] = p00;
            return polygon;
        }



        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            switch (DrawMode)
            {
                case DrawMode.Outline:
                    grfx.DrawPath(new Pen(OutlineColor, (float)OutlinePenWidth), GetScaledSPath(ScaleFactor));
                    break;

                case DrawMode.Background:
                    grfx.FillPath(new SolidBrush(BackgroundColor), GetScaledSPath(ScaleFactor));
                    break;

                case DrawMode.BaseLayer:
                    grfx.FillPath(new SolidBrush(Color), GetScaledSPath(ScaleFactor));
                    break;

                case DrawMode.TopLayer:
                    break;
            }
        }

        public override bool IsInside(PointF P)
        {
            return Utils.IsInPolygon(P, GetScaledSPolygon(new SizeF(1, 1)));
        }

        public double Length
        {
            get { return length; }
         }

        public double S_Offset
        {
            get { return s_Offset; }
        }

    }
}
