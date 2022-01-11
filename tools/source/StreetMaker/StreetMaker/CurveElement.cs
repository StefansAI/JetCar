using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StreetMaker
{
    public class CurveElement:BaseElement
    {
        protected double innerRadius;
        protected double curveAngle;
        protected double curveAngleStep;
        protected double maxCurveAngle;
        internal PointF circleCenter;

        public CurveElement(double Width, Color Color, Color OutlineColor, double OutlinePenWidth, Color BackgroundColor, double InnerRadius, double CurveAngle, double CurveAngleStep, double MaxCurveAngle) : base(2, Width, Color, OutlineColor, OutlinePenWidth, BackgroundColor)
        {
            this.innerRadius = InnerRadius;
            this.curveAngle = CurveAngle; 
            this.curveAngleStep = CurveAngleStep;
            this.maxCurveAngle = MaxCurveAngle;
        }

    protected override void HandleConnectorEvent(Connector sender)
        {
            Connector updateConnector = sender == Connectors[0] ? Connectors[1] : Connectors[0];

            if (EditModeMove == true)
            {
                if (curveAngle >= 0)
                {
                    circleCenter = Utils.GetPoint(sender.EndP1, sender.Angle, innerRadius);
                    PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((sender.Angle - curveAngle) - Math.PI), (innerRadius + updateConnector.Width / 2));
                    updateConnector.UpdateAngleAndCenterP(sender.Angle - curveAngle, p);
                }
                else
                {
                    circleCenter = Utils.GetPoint(sender.EndP0, Utils.LimitRadian(sender.Angle - Math.PI), innerRadius);
                    PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((sender.Angle - curveAngle)), (innerRadius + updateConnector.Width / 2));
                    updateConnector.UpdateAngleAndCenterP(sender.Angle - curveAngle, p);
                }

            }
            else
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
                    PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((updateConnector.Angle - curveAngle) - Math.PI), (innerRadius + sender.Width / 2));
                    sender.UpdateAngleAndCenterP(updateConnector.Angle - curveAngle, p);

                }
                else
                {
                    curveAngle = -delta;
                    PointF p = Utils.GetPoint(circleCenter, Utils.LimitRadian((updateConnector.Angle - curveAngle)), (innerRadius + sender.Width / 2));
                    sender.UpdateAngleAndCenterP(updateConnector.Angle - curveAngle, p);
                }
            }

        }

        protected GraphicsPath GetScaledCurvePath(SizeF ScaleFactor)
        {
            return GetScaledCurvePath(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
        }

        protected GraphicsPath GetScaledCurvePath(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            GraphicsPath path = new GraphicsPath();
            double angle_00_01 = Utils.LimitRadian(Utils.GetAngle(P00, P01));
            double angle_10_11 = Utils.LimitRadian(Utils.GetAngle(P10, P11));
            double angle_delta = Utils.LimitRadian(angle_00_01 - angle_10_11);
            //Debug.WriteLine("Connectors[0].Angle:" + Connectors[0].Angle.ToString("F3") + "   angle_00_01:" + angle_00_01.ToString("F3") + "   Connectors[1].Angle:" + Connectors[1].Angle.ToString("F3") + "   angle_10_11:" + angle_10_11.ToString("F3") + "   angle_delta:" + angle_delta.ToString("F3"));

            RectangleF innerRect = new RectangleF(Utils.Scale(new PointF((float)(circleCenter.X - InnerRadius), (float)(circleCenter.Y - InnerRadius)), ScaleFactor), new SizeF((float)(2 * InnerRadius * ScaleFactor.Width), (float)(2 * InnerRadius * ScaleFactor.Height)));
            double outerRadius = InnerRadius + Connectors[0].Width;
            RectangleF outerRect = new RectangleF(Utils.Scale(new PointF((float)(circleCenter.X - outerRadius), (float)(circleCenter.Y - outerRadius)), ScaleFactor), new SizeF((float)(2 * outerRadius * ScaleFactor.Width), (float)(2 * outerRadius * ScaleFactor.Height)));
            if (angle_delta >= 0)
            {
                path.AddLine(Utils.Scale(P00, ScaleFactor), Utils.Scale(P01, ScaleFactor));
                path.AddArc(innerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_00_01 + Utils.RIGHT_ANGLE)), (float)Utils.ToDegree(angle_delta));
                path.AddLine(Utils.Scale(P11, ScaleFactor), Utils.Scale(P10, ScaleFactor));
                path.AddArc(outerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_10_11 + Utils.RIGHT_ANGLE)), -(float)Utils.ToDegree(angle_delta));
            }
            else
            {
                path.AddLine(Utils.Scale(P01, ScaleFactor), Utils.Scale(P00, ScaleFactor));
                path.AddArc(outerRect, -(float)Utils.ToDegree(Utils.LimitRadian(angle_00_01 - Utils.RIGHT_ANGLE)), (float)Utils.ToDegree(angle_delta));
                path.AddLine(Utils.Scale(P11, ScaleFactor), Utils.Scale(P10, ScaleFactor));
                path.AddArc(innerRect, (float)Utils.ToDegree(Utils.LimitRadian(Utils.RIGHT_ANGLE - angle_10_11)), -(float)Utils.ToDegree(angle_delta));
            }

            return path;
        }

        protected PointF[] GetScaledCurvePolygon(SizeF ScaleFactor)
        {
            return GetScaledCurvePolygon(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
        }

        protected PointF[] GetScaledCurvePolygon(PointF P00, PointF P01, PointF P10, PointF P11, SizeF ScaleFactor)
        {
            double angle_00_01 = Utils.GetAngle(P00, P01);
            double angle_10_11 = Utils.GetAngle(P10, P11);
            double angle_delta = angle_00_01 - angle_10_11;

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
            for (int i = 0; i < 10; i++)
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

            double outerRadius = innerRadius + Connectors[0].Width;
            for (int i = 0; i < 10; i++)
            {
                alpha += sweep;
                polygon[14 + i] = Utils.Scale(Utils.GetPoint(circleCenter, alpha, outerRadius), ScaleFactor);
            }
            polygon[24] = polygon[0];
            return polygon;
        }

        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            switch (DrawMode)
            {
                case DrawMode.Outline:
                    grfx.DrawPath(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledCurvePath(ScaleFactor));
                    //grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlineLineWidth), GetScaledCurvePolygon(ScaleFactor));
                    break;

                case DrawMode.Background:
                    grfx.FillPath(new SolidBrush(BackgroundColor), GetScaledCurvePath(ScaleFactor));
                    break;

                case DrawMode.BaseLayer:
                    grfx.FillPath(new SolidBrush(Color), GetScaledCurvePath(ScaleFactor));
                    break;

                case DrawMode.TopLayer:
                    break;
            }
        }

        public override bool IsInside(PointF P)
        {
            return Utils.IsInPolygon(P, GetScaledCurvePolygon(new SizeF(1, 1)));
        }

        public double InnerRadius
        {
            get { return innerRadius; }
            //set
            //{
            //    innerRadius = value;
            //}
        }

        public double CurveAngle
        {
            get { return curveAngle; }
            //set
            //{
            //    curveAngle = value;
            //}
        }

        public PointF CircleCenter
        {
            get { return circleCenter; }
            //set
            //{
            //    circleCenter = value;
            //}
        }


    }
}
