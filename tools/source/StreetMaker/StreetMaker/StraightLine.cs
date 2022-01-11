using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace StreetMaker
{
    public class StraightLine:StraightElement
    {
        private bool dashed;
        private bool doubled;
        private bool shared;

        private double dashLength;
        private double dashPhase;

        public StraightLine(double Width, Color Color, Color OutlineColor, double OutlinePenWidth, Color BackgroundColor, double Length) : base(Width, Color, OutlineColor, OutlinePenWidth, BackgroundColor, Length)
        {
            this.dashed = false;
            this.doubled = false;
            this.shared = true;
            this.dashLength = AppSettings.DashLength;

            switch (LineType)
            {
                case LineType.Transparent:
                    this.Color = Color.Transparent;
                    break;

                case LineType.SingleWhiteSolid:
                    this.Color = AppSettings.LineColorWhite;
                    this.shared = false;
                    break;


                case LineType.SingleWhiteDashed:
                    this.Color = AppSettings.LineColorWhite;
                    this.dashed = true;
                    break;

                case LineType.SingleYellowSolid:
                    this.Color = AppSettings.LineColorYellow;
                    break;

                case LineType.SingleYellowDashed:
                    this.Color = AppSettings.LineColorYellow;
                    this.dashed = true;
                    break;

                case LineType.DoubleWhiteSolid:
                    this.Color = AppSettings.LineColorWhite;
                    this.doubled = true;
                    this.shared = false;
                    break;

                case LineType.DoubleYellowSolid:
                    this.Color = AppSettings.LineColorYellow;
                    this.doubled = true;
                    this.shared = false;
                    break;
            }

            Connectors[0].SuspendEvents();
            Connectors[1].SuspendEvents();


        }


    }
}
