using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StreetMaker
{
    public class StraightElement:BaseElement
    {
        protected double length;

        public StraightElement(double Width, Color Color, Color OutlineColor, double OutlinePenWidth, Color BackgroundColor, double Length) :base(2, Width, Color, OutlineColor, OutlinePenWidth, BackgroundColor)
        {
            this.length = Length;
        }

        protected override void HandleConnectorEvent(Connector sender)
        {
            Connector updateConnector = sender == Connectors[0] ? Connectors[1] : Connectors[0];

            if (EditModeMove == true)
                updateConnector.UpdateAngleAndCenterP(sender.Angle, Utils.GetPoint(sender.CenterP, sender.Angle + Utils.RIGHT_ANGLE, Length));
            else
                length = Math.Max(Utils.GetDistance(Connectors[0].CenterP, Connectors[1].CenterP), Connectors[0].Width / 2);

        }


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

        protected PointF[] GetScaledStraightPolygon(SizeF ScaleFactor)
        {
            return GetScaledStraightPolygon(Connectors[0].EndP0, Connectors[0].EndP1, Connectors[1].EndP0, Connectors[1].EndP1, ScaleFactor);
        }

        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            switch (DrawMode)
            {
                case DrawMode.Outline:
                    grfx.DrawPolygon(new Pen(OutlineColor, (float)OutlinePenWidth), GetScaledStraightPolygon(ScaleFactor));
                    break;

                case DrawMode.Background:
                    grfx.FillPolygon(new SolidBrush(BackgroundColor), GetScaledStraightPolygon(ScaleFactor));
                    break;

                case DrawMode.BaseLayer:
                    grfx.FillPolygon(new SolidBrush(Color), GetScaledStraightPolygon(ScaleFactor));
                    break;

                case DrawMode.TopLayer:
                    break;
            }
        }

        public override bool IsInside(PointF P)
        {
            return Utils.IsInPolygon(P, GetScaledStraightPolygon(new SizeF(1, 1)));
        }

        public double Length
        {
            get { return length; }
        }

    }
}
