// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

// Activate to visualize the outer rectangle of the intersection object with circels around the corner points 0 and 1.
//#define DEBUG_DRAW_OUTER_LINES

// Activate to visualize the inner rectangle of the intersection object with circels around the corner points 0 and 1.
//#define DEBUG_DRAW_INNER_LINES

// Activate to vizualize the indices of the corner rounds as number strings next to the corner
//#define DEBUG_DRAW_CORNER_IDX

// Activate to visualize the point arrays recalculated in the CornerRound.Update method. To vizualize, DEBUG_CORNER_ROUND_UPDATE_POINTS has to be enabled in both: Intersection.cs and CornerRound.cs
//#define DEBUG_CORNER_ROUND_UPDATE_POINTS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace StreetMaker
{ 
    /// <summary>
    /// Class to handle intersections. Intersections can be a T-intersection where one street is going through or 3-way or 4-way intersections. An array of StreetDescriptor objects defines 
    /// each multilane street meeting at the intersection. Crosswalks, Stop or Yield markings and the number of lanes are defined individually.
    /// </summary>
    public class Intersection : StreetElement
    {
        #region Public Constants
        /// <summary>String to identify this class in the XML file.</summary>
        public const string XML_TYPE_STR = "intersection";
        #endregion Public Constants

        #region Private Fields
        /// <summary>Width of the inner area of the intersection where all streets meet. Width refers to original orientation.</summary>
        private double innerWidth;
        /// <summary>Height of the inner area of the intersection where all streets meet. Height refers to original orientation.</summary>
        private double innerHeight;

        /// <summary>Width of the outer area of the intersection including all connecting streets. Width refers to original orientation.</summary>
        private double outerWidth;
        /// <summary>Height of the outer area of the intersection including all connecting streets. Height refers to original orientation.</summary>
        private double outerHeight;

        /// <summary>Length of the right street connecting to the intersection. Right refers to original orientation.</summary>
        private double rightStreetLength;
        /// <summary>Length of the top street connecting to the intersection. Top refers to original orientation.</summary>
        private double topStreetLength;

        /// <summary>Width of the lanes.</summary>
        private double laneWidth;
        /// <summary>Width of the lane limit lines.</summary>
        private double lineWidth;
        /// <summary>Recess of crosswalks or stop/yield lines from the inner intersection area limits.</summary>
        private double intersectionRecess;
        /// <summary>Width of a crosswalk in mm.</summary>
        private double crosswalkWidth;
        /// <summary>Width of the lines limiting a crosswalk in mm.</summary>
        private double crosswalkLineWidth;
        /// <summary>Width of the zebra strips of a crosswalk in mm.</summary>
        private double crosswalkStripeWidth;
        /// <summary>Border offset of a crosswalk in mm. This provides a gap between the crosswalk and a stop line or other elements.</summary>
        private double crosswalkBorder;

        /// <summary>Width of stop lines in mm.</summary>
        private double stopLineWidth;
        /// <summary>Offset of the stop lines from the crossing lane limits in mm.</summary>
        private double stopLineOffset;
        /// <summary>Color of the stop line.</summary>
        private Color stopLineColor;
        /// <summary>The font size of Stop and Yield markings in the intersections.</summary>
        private double intersectionFontSize;

        /// <summary>Polygon outlining the inner area of the intersection where all streets meet.</summary>
        private PointF[] innerArea;
        /// <summary>Polygon outlining the outer area of the intersection including all streets.</summary>
        private PointF[] outerBorder;
        /// <summary>Angle of street 0 as reference for all other parts of the intersection.</summary>
        private double lane0Angle;
        /// <summary>Array of CornerRound objects for this object.</summary>
        private CornerRound[] cornerRounds;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Array of StreetDescriptors for this object.</summary>
        public readonly StreetDescriptor[] StreetDescriptors;
        #endregion Public Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the Intersection class and initializes all internal parameter to draw and handle a complete intersection.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object holding all customizable parameter.</param>
        /// <param name="StreetDescriptors">Array of StreetDescriptors to describe this intersection.</param>
        public Intersection(AppSettings AppSettings, StreetDescriptor[] StreetDescriptors) : base(AppSettings)
        {
            if (((StreetDescriptors.Length < 2)) || (StreetDescriptors.Length > 4))
                throw new Exception("Invalid Parameter creating Intersection!");

            this.StreetDescriptors = StreetDescriptors;
            int totalLanes = 0;
            for (int i = 0; i < StreetDescriptors.Length; i++)
                totalLanes += StreetDescriptors[i].LaneCount;

            lanes = new LaneElement[totalLanes];
            innerArea = new PointF[5];
            outerBorder = new PointF[5];

            if (StreetDescriptors.Length == 4)
                cornerRounds = new CornerRound[4];
            else 
                cornerRounds = new CornerRound[2];

            for (int i = 0; i < cornerRounds.Length; i++)
                cornerRounds[i] = new CornerRound(AppSettings.CornerRadius, AppSettings.LaneWidth, AppSettings.LineWidth);

            int yLaneCount = StreetDescriptors.Length >= 3 ? Math.Max(StreetDescriptors[0].LaneCount, StreetDescriptors[2].LaneCount) : StreetDescriptors[0].LaneCount;
            int xLaneCount = StreetDescriptors.Length == 4 ? Math.Max(StreetDescriptors[1].LaneCount, StreetDescriptors[3].LaneCount) : StreetDescriptors[1].LaneCount;

            laneWidth = AppSettings.LaneWidth;
            lineWidth = AppSettings.LineWidth;
            intersectionRecess = AppSettings.IntersectionRecess;
            crosswalkWidth = AppSettings.CrosswalkWidth;
            crosswalkLineWidth = AppSettings.CrosswalkLineWidth;
            crosswalkStripeWidth = AppSettings.CrosswalkStripeWidth;
            crosswalkBorder = AppSettings.CrosswalkBorder;
            stopLineWidth = AppSettings.StopLineWidth;
            stopLineOffset = AppSettings.StopLineOffset;
            stopLineColor = AppSettings.StopLineColor;
            intersectionFontSize = AppSettings.IntersectionFontSize;

            innerWidth = xLaneCount * AppSettings.LaneWidth;
            innerHeight = yLaneCount * AppSettings.LaneWidth;


            rightStreetLength = StreetDescriptors.Length >= 3 ? StreetDescriptors[2].Length : StreetDescriptors[0].Length;
            topStreetLength = StreetDescriptors.Length == 4 ? StreetDescriptors[3].Length : 0;

            outerWidth = innerWidth + StreetDescriptors[0].Length + rightStreetLength;
            outerHeight = innerHeight + StreetDescriptors[1].Length + topStreetLength;

            int n = 0;
            double x = 0, y = 0, xStep = 0, yStep = 0;
            for (int i = 0; i < StreetDescriptors.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        x = 0;
                        y = outerHeight - StreetDescriptors[1].Length - AppSettings.LaneWidth / 2;
                        xStep = 0;
                        yStep = -AppSettings.LaneWidth;
                        break;

                    case 1:
                        x = outerWidth - rightStreetLength - AppSettings.LaneWidth / 2;
                        y = outerHeight;
                        xStep = -AppSettings.LaneWidth;
                        yStep = 0;
                        break;

                    case 2:
                        x = outerWidth;
                        y = topStreetLength + AppSettings.LaneWidth / 2;
                        xStep = 0;
                        yStep = AppSettings.LaneWidth;
                        break;

                    case 3:
                        x = StreetDescriptors[0].Length + AppSettings.LaneWidth / 2;
                        y = 0;
                        xStep = AppSettings.LaneWidth;
                        yStep = 0;
                        break;
                }

                for (int j = 0; j < StreetDescriptors[i].LaneCount; j++, n++)
                {
                    LaneElement newLane = new LaneElement(this, n, AppSettings, ShapeType.Straight);
                    if (j < StreetDescriptors[i].LaneCountRight)
                    {
                        if (j == 0)
                            newLane.RightLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].RightBorderLine);
                        else
                            newLane.RightLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].RightLaneLine);

                        if (j < StreetDescriptors[i].LaneCountRight - 1)
                            newLane.LeftLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].RightLaneLine);
                        else
                            newLane.LeftLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].DividerLine);

                        newLane.Connectors[0].SetMode(AppSettings, ConnectorMode.In);
                        newLane.Connectors[1].SetMode(AppSettings, ConnectorMode.Out);
                    }
                    else if (j < StreetDescriptors[i].LaneCountRight + StreetDescriptors[i].LaneCountCenter)
                    {
                        newLane.RightLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].DividerLine);
                        newLane.LeftLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].DividerLine2);
                        newLane.Connectors[0].SetMode(AppSettings, ConnectorMode.NoDir);
                        newLane.Connectors[1].SetMode(AppSettings, ConnectorMode.NoDir);
                    }
                    else
                    {
                        if (j == StreetDescriptors[i].LaneCountRight + StreetDescriptors[i].LaneCountCenter)
                            newLane.RightLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].DividerLine2);
                        else
                            newLane.RightLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].LeftLaneLine);

                        if (j == StreetDescriptors[i].LaneCount - 1)
                            newLane.LeftLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].LeftBorderLine);
                        else
                            newLane.LeftLine = new LineElement(AppSettings, ShapeType.Straight, StreetDescriptors[i].LeftLaneLine);

                        newLane.Connectors[0].SetMode(AppSettings, ConnectorMode.Out);
                        newLane.Connectors[1].SetMode(AppSettings, ConnectorMode.In);
                    }

                    newLane.Connectors[0].CenterP = new PointF((float)x, (float)y);
                    newLane.Connectors[0].Angle = i * Utils.RIGHT_ANGLE_RADIAN;

                    if (newLane.LeftLine != null)
                        newLane.LeftLine.Connectors[1].DrawOffsL = 0;

                    if (newLane.RightLine != null)
                        newLane.RightLine.Connectors[1].DrawOffsL = 0;

                    if ((i == 0) && (StreetDescriptors.Length == 2))
                        newLane.Length = outerWidth;
                    else
                    {
                        newLane.Length = StreetDescriptors[i].Length;
                        newLane.Connectors[1].SetMode(AppSettings, ConnectorMode.Hidden);
                    }

                    StreetDescriptors[i].Lanes[j] = newLane;
                    lanes[n] = newLane;
                    x += xStep;
                    y += yStep;
                }

                StreetDescriptors[i].Lanes[0].RightLine.Shared = false; // in case, it is not a ShoulderLine type
                StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCount - 1].LeftLine.Shared = false; // in case, it is not a ShoulderLine type
            }

            activeConnector = lanes[(StreetDescriptors[0].LaneCount - 1) / 2].Connectors[0];
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Calculates the innerArea and outerArea polygons and updates the cornerRound objects. This method is called after a move or rotation.
        /// </summary>
        private void UpdateGeometries()
        {
            lane0Angle = Utils.GetAngle(lanes[0].Connectors[0].CenterP, lanes[0].Connectors[1].CenterP);

            innerArea[0] = Utils.GetPoint(lanes[0].RightLine.Connectors[0].EndP1, lane0Angle, StreetDescriptors[0].Length); 
            innerArea[1] = Utils.GetPoint(lanes[0].RightLine.Connectors[0].EndP1, lane0Angle, StreetDescriptors[0].Length + innerWidth);
            innerArea[2] = Utils.GetPoint(innerArea[1], lane0Angle+Utils.RIGHT_ANGLE_RADIAN, innerHeight);
            innerArea[3] = Utils.GetPoint(innerArea[2], lane0Angle, -innerWidth);
            innerArea[4] = innerArea[0];

            outerBorder[0] = Utils.GetPoint(lanes[0].RightLine.Connectors[0].EndP1, lane0Angle - Utils.RIGHT_ANGLE_RADIAN, StreetDescriptors[1].Length);
            outerBorder[1] = Utils.GetPoint(outerBorder[0], lane0Angle, outerWidth);
            outerBorder[2] = Utils.GetPoint(outerBorder[1], lane0Angle + Utils.RIGHT_ANGLE_RADIAN, outerHeight);
            outerBorder[3] = Utils.GetPoint(outerBorder[2], lane0Angle, -outerWidth);
            outerBorder[4] = outerBorder[0];

            if (StreetDescriptors.Length == 2)
            {
                cornerRounds[0].Update(innerArea[0], lane0Angle, StreetDescriptors[0].Lanes[0].RightLine.Connectors[0].EndP1);
                cornerRounds[1].Update(innerArea[1], lane0Angle + Utils.RIGHT_ANGLE_RADIAN, StreetDescriptors[1].Lanes[0].RightLine.Connectors[0].EndP1);
            }
            else
            {
                double angle = lane0Angle;
                int n = StreetDescriptors[0].LaneCount;
                for (int i = 0; i < cornerRounds.Length; i++)
                {
                    if (i< 3)
                        n += StreetDescriptors[i+1].LaneCount;
                    else
                        n = StreetDescriptors[0].LaneCount;

                    cornerRounds[i].Update(lanes[n - 1].Connectors[1].EndP0, angle, StreetDescriptors[i].Lanes[0].RightLine.Connectors[0].EndP1);
                    angle = Utils.LimitRadian(angle + Utils.RIGHT_ANGLE_RADIAN);
                }
            }
        }

        /// <summary>
        /// Get scaled intersection inner area polygon.
        /// </summary>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <returns>Scaled inner polygon.</returns>
        private PointF[] GetInnerPolygon(SizeF ScaleFactor)
        {
            return Utils.Scale(innerArea, ScaleFactor);
        }

        /// <summary>
        /// Moves this complete intersection by the given delta.
        /// </summary>
        /// <param name="Delta">X- and Y- delta to move this intersection.</param>
        protected override void MoveStreet(SizeF Delta)
        {
            base.MoveStreet(Delta);
            UpdateGeometries();
        }

        /// <summary>
        /// Get the color for drawing depending on the current ColorMode.
        /// </summary>
        /// <param name="DefaultColor">Color to be used for ColorMode.ImageColor</param>
        /// <param name="SCD">SegmClassDef to be used for class colors</param>
        /// <returns>Draw color selected via ColorMode.</returns>
        private Color GetDrawColor(Color DefaultColor, SegmClassDef SCD)
        {
            switch (Lanes[0].ColorMode)
            {
                case ColorMode.ImageColor:
                    return DefaultColor;

                case ColorMode.ClassColor:
                    return SCD.DrawColor;

                case ColorMode.ClassCode:
                    return SCD.ClassCodeColor;
            }
            return DefaultColor;
        }

        /// <summary>
        /// Get the color for drawing depending on the current ColorMode. The default color for ColorMode.ImageColor is the lane color.
        /// </summary>
        /// <param name="SCD">SegmClassDef to be used for class colors</param>
        /// <returns>Draw color selected via ColorMode.</returns>
        private Color GetDrawColor(SegmClassDef SCD)
        {
            return GetDrawColor(Lanes[0].Color, SCD);
        }

        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Draw method of this class. It calls the draw methods of all owned lane objects passing the parameters.
        /// It then darws the inner area and the corner rounds. In the end crosswalks and stop or yield lines and texts are drawn.
        /// The trick to add corner rounds to the intersection is drawing corner round's base layer on top of the the street top layer, 
        /// so the corner round pavement draws over the shoulder lines of the corner and then to draw the round shoulder lines on top.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            if (DrawAbort())
                return;

            base.Draw(grfx, ScaleFactor, DrawMode);

            switch (DrawMode)
            {
                case DrawMode.Outline:
                    if (StreetDescriptors.Length > 2)
                        grfx.DrawPolygon(new Pen(lanes[0].OutlineColor, (float)lanes[0].OutlineLineWidth), GetInnerPolygon(ScaleFactor));

                    // prepare for drawing street indices
                    Font f = new Font("System", 24*ScaleFactor.Height);
                    Brush b = new SolidBrush(Color.Blue);
                    // setup center alignments
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    // draw the street indices
                    for (int i = 0; i < StreetDescriptors.Length; i++)
                        grfx.DrawString(i.ToString(), f, b, Utils.Scale(StreetDescriptors[i].Lanes[0].Connectors[1].CenterP,ScaleFactor), format);

#if DEBUG_DRAW_CORNER_IDX
                    // draw the corner indices for debugging
                    b = new SolidBrush(Color.LightBlue);
                    for (int i = 0; i < cornerRounds.Length; i++)
                    {
                        float x = (cornerRounds[i].CenterP.X + cornerRounds[i].CornerP.X)/2;
                        float y = (cornerRounds[i].CenterP.Y + cornerRounds[i].CornerP.Y)/2;
                        grfx.DrawString(i.ToString(), f, b, Utils.Scale(new PointF(x,y), ScaleFactor), format);
                    }
#endif
                    break;

                case DrawMode.Background:
                    if (StreetDescriptors.Length > 2)
                        grfx.FillPolygon(new SolidBrush(lanes[0].BackgroundColor), GetInnerPolygon(ScaleFactor));

                    foreach (CornerRound cr in cornerRounds)
                        grfx.FillPath(new SolidBrush(lanes[0].BackgroundColor), cr.GetScaledBasePath(ScaleFactor));

                    break;

                case DrawMode.BaseLayer:
                    if (StreetDescriptors.Length > 2)
                        grfx.FillPolygon(new SolidBrush(lanes[0].Color), GetInnerPolygon(ScaleFactor));

                    foreach (CornerRound cr in cornerRounds)
                        grfx.FillPath(new SolidBrush(lanes[0].Color), cr.GetScaledBasePath(ScaleFactor));

                    break;

                case DrawMode.TopLayer:
#if DEBUG_DRAW_OUTER_LINES
                    grfx.DrawPolygon(new Pen(Color.Red, (float)lanes[0].OutlineLineWidth), Utils.Scale(outerBorder, ScaleFactor));
                    Utils.DrawPointCircle(grfx, outerBorder[0], ScaleFactor, Color.LightBlue, 3, 10);
                    Utils.DrawPointCircle(grfx, outerBorder[1], ScaleFactor, Color.LightGreen, 3, 10);
#endif
                    int drivingDirIdx = -1;
                    // draw the corner round base layer on top of the street top layer, darwing over the existing lines
                    if (Lanes[0].ColorMode == ColorMode.ImageColor)
                        foreach (CornerRound cr in cornerRounds)
                            grfx.FillPath(new SolidBrush(lanes[0].Color), cr.GetScaledBasePath(ScaleFactor));
                    else
                    {   // ColorMode is set to draw classes, so corner rounds will be drawn differently depending to the class codes
                        // Assign correct class codes to the corner rounds
                        for (int i = 0; i < cornerRounds.Length; i++)
                        {
                            int j = (i + 1) % StreetDescriptors.Length;
                            SegmClassDef scdL = StreetDescriptors[i].Lanes[0].SegmClassDef; 
                            SegmClassDef scdR = StreetDescriptors[j].Lanes[StreetDescriptors[j].LaneCount - 1].SegmClassDef;

                            if (StreetDescriptors.Length > 2)
                            {
                                if (scdR == SegmClassDefs.ScdDrivingDir)
                                {
                                   int jj = (j + 2) % 4;
                                    if ((jj < StreetDescriptors.Length) && (StreetDescriptors[j].LaneCount < StreetDescriptors[jj].LaneCount))
                                        scdL = scdR;
                                }
                            }
                            else if (cornerRounds.Length == 2) 
                            {
                                if (i == 0)
                                {
                                    if ((scdR == SegmClassDefs.ScdDrivingDir) /*&& (StreetDescriptors[0].LaneCountLeft==0)*/ && (StreetDescriptors[1].LaneCountLeft == 0))
                                        scdL = SegmClassDefs.ScdWrongDir;
                                }
                                else
                                {
                                    if (StreetDescriptors[0].LaneCountRight == 0)
                                    {
                                        if (scdL == SegmClassDefs.ScdDrivingDir)
                                            scdR = SegmClassDefs.ScdWrongDir;
                                    }
                                    else
                                        scdR = StreetDescriptors[j].Lanes[0].SegmClassDef;
                                }
                            }

                            if (scdL == SegmClassDefs.ScdDrivingDir)
                            {
                                grfx.FillPath(new SolidBrush(GetDrawColor(scdR)), cornerRounds[i].GetScaledBasePath(ScaleFactor));
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(scdL)), cornerRounds[i].GetScaledBaseLeftLine(ScaleFactor));
                            }
                            else if (scdR == SegmClassDefs.ScdDrivingDir)
                            {
                                grfx.FillPath(new SolidBrush(GetDrawColor(scdL)), cornerRounds[i].GetScaledBasePath(ScaleFactor));
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(scdR)), cornerRounds[i].GetScaledBaseTopLine(ScaleFactor));
                            }
                            else if (scdR == SegmClassDefs.ScdLeftTurnDir) 
                            {
                                grfx.FillPath(new SolidBrush(GetDrawColor(scdL)), cornerRounds[i].GetScaledBasePath(ScaleFactor));
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(scdR)), cornerRounds[i].GetScaledBaseTopLine(ScaleFactor));
                            }
                            else if (scdL == SegmClassDefs.ScdLeftTurnDir) 
                            {
                                grfx.FillPath(new SolidBrush(GetDrawColor(scdR)), cornerRounds[i].GetScaledBasePath(ScaleFactor));
                                if ((StreetDescriptors.Length != 2) || (i > 0))
                                    grfx.FillPolygon(new SolidBrush(GetDrawColor(scdL)), cornerRounds[i].GetScaledBaseLeftLine(ScaleFactor));
                            }
                            else grfx.FillPath(new SolidBrush(GetDrawColor(scdR)), cornerRounds[i].GetScaledBasePath(ScaleFactor));

                        }
                        
                        // in class mode, draw wrong direction into inner area 
                        if (StreetDescriptors.Length > 2)
                        {
                            grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdWrongDir)), GetInnerPolygon(ScaleFactor));
                        }
                        else if ((StreetDescriptors[1].Lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir))//!!!!!!! && (DrawWrongDirItems == true)) // when StreetDescriptors.Length == 2
                        {
                            // the T-intersection has lanes going through and are now drawn with class codes left and right completely
                            // -> draw wrong dir over right turn code from left to center
                            PointF[] poly = new PointF[5];
                            if (StreetDescriptors[0].LaneCountRight > 0)
                            {
                                poly[0] = StreetDescriptors[1].Lanes[0].Connectors[1].EndP0;
                                poly[1] = StreetDescriptors[0].Lanes[0].Connectors[0].EndP1;
                                poly[2] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCountRight - 1].Connectors[0].EndP0;
                                double w = Utils.GetDistance(poly[1], poly[2]);
                                double a = Utils.GetAngle(poly[1], poly[2]);
                                poly[3] = Utils.GetPoint(poly[0], a, w);
                                poly[4] = poly[0];
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdWrongDir)), Utils.Scale(poly, ScaleFactor));
                            }
                            // draw wrong dir over left turn code from right to center
                            if (StreetDescriptors[0].LaneCountLeft > 0)
                            {
                                poly[0] = innerArea[2]; // innerPoly[2];
                                poly[1] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].Connectors[1].EndP0;
                                poly[2] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - StreetDescriptors[0].LaneCountLeft].RightLine.Connectors[1].EndP0;
                                double w = Utils.GetDistance(poly[1], poly[2]);
                                double a = Utils.GetAngle(poly[1], poly[2]);
                                poly[3] = Utils.GetPoint(poly[0], a, w);
                                poly[4] = poly[0];
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdWrongDir)), Utils.Scale(poly, ScaleFactor));
                            }
                        }
                       
                        // extend left turn from lane to center
                        for (int i = 0; i < StreetDescriptors.Length; i++)
                        {
                            if (StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCount - 1].SegmClassDef == SegmClassDefs.ScdLeftTurnDir)
                            {
                                int ii = (i + 1) % 4;
                                if ((StreetDescriptors.Length == 2) || ((ii<StreetDescriptors.Length) && (StreetDescriptors[ii].LaneCountRight > 0)))
                                {
                                    // extend left turn from lane to center
                                    PointF[] poly = new PointF[5];
                                    double d = (ii & 1) == 0 ? innerHeight : innerWidth;
                                    PointF p0, p1;
                                    poly[0] = StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCount - 1].Connectors[1].EndP0;
                                    if ((StreetDescriptors[i].LaneCountRight + StreetDescriptors[i].LaneCountCenter - 1) >= 0)
                                        poly[1] = StreetDescriptors[i].Lanes[Math.Max(StreetDescriptors[i].LaneCountRight + StreetDescriptors[i].LaneCountCenter - 1, 0)].Connectors[1].EndP0;
                                    else
                                        poly[1] = StreetDescriptors[i].Lanes[0].Connectors[1].EndP1;

                                    if (StreetDescriptors.Length == 2)
                                    {
                                        p0 = StreetDescriptors[0].Lanes[Math.Max(StreetDescriptors[0].LaneCount - StreetDescriptors[0].LaneCountLeft - StreetDescriptors[0].LaneCountCenter, 0)].RightLine.Connectors[1].EndP0;
                                        p1 = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].Connectors[1].EndP0;
                                    }
                                    else
                                    {
                                        p0 = StreetDescriptors[ii].Lanes[Math.Max(StreetDescriptors[ii].LaneCountRight - 1, 0)].Connectors[1].EndP0;
                                        p1 = StreetDescriptors[ii].Lanes[0].Connectors[1].EndP1;
                                    }
                                    double dist = Utils.GetDistance(p0, p1);
                                    double dirAngle = Utils.GetAngle(p0, p1);
                                    poly[2] = Utils.GetPoint(poly[1], dirAngle, d - dist);
                                    poly[3] = Utils.GetPoint(poly[0], dirAngle, d - dist);
                                    poly[4] = poly[0];
                                    grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdLeftTurnDir)), Utils.Scale(poly, ScaleFactor));
                                }
                            }
                        }

                        // draw driving dir over it from starting point of driving direction lanes
                        for (int i = 0; i < StreetDescriptors.Length; i++)
                        {
                            if (StreetDescriptors[i].Lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir)
                            {
                                if ((StreetDescriptors.Length > 2) || (i != 0))
                                {
                                    // draw driving dir over it from starting point of driving direction lanes
                                    PointF[] poly = GetInnerPolygon(new SizeF(1, 1));

                                    double dist = Utils.GetDistance(poly[i], poly[i + 1]);
                                    double dirAngle = Utils.GetAngle(poly[i], poly[i + 1]);
                                    poly[(i + 3) % 4] = StreetDescriptors[i].Lanes[Math.Max(StreetDescriptors[i].LaneCountRight - 1, 0)].Connectors[1].EndP0;
                                    poly[(i + 2) % 4] = Utils.GetPoint(poly[(i + 3) % 4], dirAngle, dist);
                                    poly[4] = poly[0];
                                    grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdDrivingDir)), Utils.Scale(poly, ScaleFactor));

                                    drivingDirIdx = i;
                                }
                            }
                        }
                            
                        if ((StreetDescriptors.Length == 2) && (StreetDescriptors[1].Lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir))//!!!!!!!! && (DrawWrongDirItems == true))
                        {
                            // Since lane codes had been drawn over the lines, restore them now on top
                            foreach (LaneElement le in StreetDescriptors[0].Lanes)
                            {
                                if ((le.RightLine != null) && (le != StreetDescriptors[0].Lanes[0]))
                                    le.RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);

                                if (le.LeftLine != null)
                                    le.LeftLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                            }
                        }
                    } // End of "ColorMode is set to draw classes"

                    // Continue normal drawing here
                    if (StreetDescriptors.Length == 2)
                    {
                        // disappear right shoulder line going through intersection
                        PointF pm = StreetDescriptors[1].Lanes[Math.Max(StreetDescriptors[1].LaneCountRight - 1, 0)].Connectors[1].EndP0;
                        PointF[] poly = new PointF[5];
                        poly[0] = Utils.Scale(pm, ScaleFactor);
                        poly[1] = Utils.Scale(Utils.GetPoint(pm, lane0Angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + BaseElement.DRAW_OFFS_WIDTH+1), ScaleFactor);
                        poly[2] = Utils.Scale(Utils.GetPoint(innerArea[1], lane0Angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + BaseElement.DRAW_OFFS_WIDTH+1), ScaleFactor);
                        poly[3] = Utils.Scale(innerArea[1], ScaleFactor);
                        poly[4] = poly[0];
                        grfx.FillPolygon(new SolidBrush(StreetDescriptors[1].Lanes[0].GetDrawColor()), poly);

                        if (StreetDescriptors[1].LaneCountLeft > 0)
                        {
                            poly[2] = Utils.Scale(Utils.GetPoint(innerArea[0], lane0Angle + Utils.RIGHT_ANGLE_RADIAN, lineWidth + BaseElement.DRAW_OFFS_WIDTH+1), ScaleFactor);
                            poly[3] = Utils.Scale(innerArea[0], ScaleFactor);
                            grfx.FillPolygon(new SolidBrush(StreetDescriptors[1].Lanes[StreetDescriptors[1].LaneCountRight].GetDrawColor()), poly);
                        }
                    }

                    // Go through street descriptors to add crosswalks and stop or yield lines
                    double angle = lane0Angle - Utils.RIGHT_ANGLE_RADIAN;
                    for (int i = 0; i < StreetDescriptors.Length; i++, angle += Utils.RIGHT_ANGLE_RADIAN)
                    {
                        double offset = intersectionRecess;
                        PointF[] poly = new PointF[5];

                        // descriptor 0 has corner 0 to the right and corner 3 to the left etc
                        int l = (i + 3) % 4;
                        PointF pl = l < cornerRounds.Length ? cornerRounds[l].CornerP : innerArea[l];
                        PointF pm = StreetDescriptors[i].Lanes[Math.Max(StreetDescriptors[i].LaneCountRight - 1, 0)].Connectors[1].EndP0;
                        PointF pr = innerArea[i];
                        PointF p0, p1;

                        // Handle Crosswalks
                        if (StreetDescriptors[i].CrosswalkType != CrosswalkType.None)
                        {
                            poly[0] = pm;
                            poly[1] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkWidth + 2 * crosswalkBorder);
                            poly[3] = Utils.GetPoint(pr, angle, -lineWidth);
                            poly[2] = Utils.GetPoint(poly[3], angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkWidth + 2 * crosswalkBorder);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(StreetDescriptors[i].Lanes[0].GetDrawColor()), Utils.Scale(poly, ScaleFactor));

                            poly[3] = Utils.GetPoint(pl, angle, lineWidth);
                            poly[2] = Utils.GetPoint(poly[3], angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkWidth + 2 * crosswalkBorder);
                            grfx.FillPolygon(new SolidBrush(StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCount - 1].GetDrawColor()), Utils.Scale(poly, ScaleFactor));

                            bool handleCrosswalks = true;
                            if (Lanes[0].ColorMode != ColorMode.ImageColor)
                                handleCrosswalks = (Utils.GetDistance(CameraPoint, pl) < MaxDetailDist) || (Utils.GetDistance(CameraPoint, pr) < MaxDetailDist) ||
                                                   (Utils.GetDistance(CameraPoint, poly[1]) < MaxDetailDist) || (Utils.GetDistance(CameraPoint, poly[2]) < MaxDetailDist);

                            if (handleCrosswalks)
                            {
                                Color drawColor = GetDrawColor(stopLineColor, SegmClassDefs.GetSegmClassDef(StreetDescriptors[i].CrosswalkType));

                                if ((StreetDescriptors[i].CrosswalkType == CrosswalkType.ParallelLines) || (Lanes[0].ColorMode != ColorMode.ImageColor))
                                {
                                    poly[0] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder);
                                    poly[1] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkLineWidth);
                                    if (l < cornerRounds.Length)
                                    {
                                        poly[0] = cornerRounds[l].GetLeftLinePoint(poly[0]);
                                        poly[1] = cornerRounds[l].GetLeftLinePoint(poly[1]);
                                    }
                                    poly[2] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkLineWidth);
                                    poly[3] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder);
                                    if (i < cornerRounds.Length)
                                    {
                                        poly[2] = cornerRounds[i].GetRightLinePoint(poly[2]);
                                        poly[3] = cornerRounds[i].GetRightLinePoint(poly[3]);
                                    }

                                    if (Lanes[0].ColorMode == ColorMode.ImageColor)
                                    {
                                        poly[4] = poly[0];
                                        grfx.FillPolygon(new SolidBrush(drawColor), Utils.Scale(poly, ScaleFactor));
                                    }

                                    p0 = poly[0];
                                    p1 = poly[3];

                                    poly[0] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkWidth - crosswalkLineWidth);
                                    poly[1] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkWidth);
                                    if (l < cornerRounds.Length)
                                    {
                                        poly[0] = cornerRounds[l].GetLeftLinePoint(poly[0]);
                                        poly[1] = cornerRounds[l].GetLeftLinePoint(poly[1]);
                                    }
                                    poly[2] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkWidth);
                                    poly[3] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkWidth - crosswalkLineWidth);
                                    if (i < cornerRounds.Length)
                                    {
                                        poly[2] = cornerRounds[i].GetRightLinePoint(poly[2]);
                                        poly[3] = cornerRounds[i].GetRightLinePoint(poly[3]);
                                    }

                                    if (Lanes[0].ColorMode != ColorMode.ImageColor)
                                    {
                                        poly[0] = p0;
                                        poly[3] = p1;
                                    }
                                    poly[4] = poly[0];
                                    grfx.FillPolygon(new SolidBrush(drawColor), Utils.Scale(poly, ScaleFactor));

                                }
                                else if (StreetDescriptors[i].CrosswalkType == CrosswalkType.ZebraStripes)
                                {
                                    p0 = poly[0] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder);
                                    p1 = poly[1] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset + crosswalkBorder + crosswalkWidth);

                                    double dist = Utils.GetDistance(pl, pr) - 2 * lineWidth;
                                    int n = (int)(dist / (2 * crosswalkStripeWidth));
                                    double delta = (dist - ((n * crosswalkStripeWidth) + ((n - 1) * crosswalkStripeWidth))) / 2;
                                    poly[0] = Utils.GetPoint(poly[0], angle, delta + lineWidth);
                                    poly[1] = Utils.GetPoint(poly[1], angle, delta + lineWidth);

                                    for (int j = 0; j < n; j++)
                                    {
                                        poly[2] = Utils.GetPoint(poly[1], angle, crosswalkStripeWidth);
                                        poly[3] = Utils.GetPoint(poly[0], angle, crosswalkStripeWidth);
                                        poly[4] = poly[0];
                                        grfx.FillPolygon(new SolidBrush(stopLineColor), Utils.Scale(poly, ScaleFactor));

                                        poly[0] = Utils.GetPoint(poly[0], angle, 2 * crosswalkStripeWidth);
                                        poly[1] = Utils.GetPoint(poly[1], angle, 2 * crosswalkStripeWidth);
                                    }

                                }
                                offset += crosswalkWidth + 2 * crosswalkBorder;
                            }
                        }
                        else if ((StreetDescriptors.Length != 2) || (i != 0))
                        {
                            if (Lanes[0].ColorMode == ColorMode.ImageColor)
                            {
                                // Draw the offset zone to recess lanes and stop or yield lines all in one here
                                poly[0] = Utils.GetPoint(pl, angle, lineWidth); 
                                poly[1] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                                poly[3] = Utils.GetPoint(pr, angle, -lineWidth);
                                poly[2] = Utils.GetPoint(poly[3], angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                                poly[4] = poly[0];
                                grfx.FillPolygon(new SolidBrush(StreetDescriptors[i].Lanes[0].Color), Utils.Scale(poly, ScaleFactor));
                            }
                            else
                            {
                                for (int k = 0; k < StreetDescriptors[i].LaneCount; k++)
                                {
                                    // Draw the offset zone to recess lanes and stop or yield lines for each lane separately with its code
                                    poly[0] = StreetDescriptors[i].Lanes[k].Connectors[1].EndP0;
                                    poly[1] = StreetDescriptors[i].Lanes[k].Connectors[1].EndP1;
                                    poly[2] = Utils.GetPoint(poly[1], angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                                    poly[3] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                                    poly[4] = poly[0];
                                    grfx.FillPolygon(new SolidBrush(StreetDescriptors[i].Lanes[k].GetDrawColor()), Utils.Scale(poly, ScaleFactor));
                                }
                            }
                        }

                        // Draw a closing line at end of a center lane
                        if (((StreetDescriptors.Length > 2) || (i > 0)) && (StreetDescriptors[i].LaneCountCenter > 0))
                        {
                            // determine the outlines for the line
                            pl = StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCountRight + StreetDescriptors[i].LaneCountCenter - 1].LeftLine.Connectors[1].EndP1;
                            pr = StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCountRight].RightLine.Connectors[1].EndP0;

                            poly[0] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                            poly[1] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, offset);
                            poly[2] = Utils.GetPoint(poly[1], angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
                            poly[3] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
                            poly[4] = poly[0];
                            Color color = StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCountRight].RightLine.GetDrawColor();
                            // draw the line or the code line 
                            grfx.FillPolygon(new SolidBrush(color), Utils.Scale(poly, ScaleFactor));
                        }

                        bool handleStopYield = (Lanes[0].ColorMode == ColorMode.ImageColor) || (DrawWrongDirItems == true) || (DrawWrongDirStopYield == true) || ((StreetDescriptors[i].Lanes[0].SegmClassDef != SegmClassDefs.ScdWrongDir) && (StreetDescriptors[i].Lanes[0].SegmClassDef != SegmClassDefs.ScdNothing));
                        if (Lanes[0].ColorMode != ColorMode.ImageColor)
                            handleStopYield &= (Utils.GetDistance(CameraPoint, pl) < MaxDetailDist) && (Utils.GetDistance(CameraPoint, pr) < MaxDetailDist);

                        // Handle Stop or Yield markings
                        if (handleStopYield && ((StreetDescriptors.Length > 2) || (i > 0)) && (StreetDescriptors[i].StopYieldType != StopYieldType.None) && (StreetDescriptors[i].LaneCountRight > 0))
                        {
                            // determine the outlines for the stop line or yield line
                            pl = StreetDescriptors[i].Lanes[StreetDescriptors[i].LaneCountRight - 1].LeftLine.Connectors[1].EndP1;
                            pr = Utils.GetPoint(innerArea[i], angle - Utils.RIGHT_ANGLE_RADIAN, offset);

                            if ((StreetDescriptors[i].LaneCountLeft == 0) && (l < cornerRounds.Length))
                            {
                                poly[0] = Utils.GetPoint(cornerRounds[l].CornerP, angle - Utils.RIGHT_ANGLE_RADIAN, offset); ;
                                poly[1] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
                                poly[0] = cornerRounds[l].GetLeftLinePoint(poly[0]);
                                poly[1] = cornerRounds[l].GetLeftLinePoint(poly[1]);
                            }
                            else
                            {
                                poly[0] = Utils.GetPoint(pl, angle - Utils.RIGHT_ANGLE_RADIAN, offset); ;
                                poly[1] = Utils.GetPoint(poly[0], angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
                            }
                            poly[2] = Utils.GetPoint(pr, angle - Utils.RIGHT_ANGLE_RADIAN, stopLineWidth);
                            poly[3] = pr;
                            if (i < cornerRounds.Length)
                            {
                                poly[2] = cornerRounds[i].GetRightLinePoint(poly[2]);
                                poly[3] = cornerRounds[i].GetRightLinePoint(poly[3]);
                            }
                            else
                            {
                                poly[2] = Utils.GetPoint(poly[2], angle, -lineWidth);
                                poly[3] = Utils.GetPoint(poly[3], angle, -lineWidth);
                            }
                            poly[4] = poly[0];

                            if ((StreetDescriptors[i].StopYieldType == StopYieldType.StopLine) || (StreetDescriptors[i].StopYieldType == StopYieldType.StopLineText))
                            {
                                // draw the stop line or the code line for stop
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(stopLineColor, SegmClassDefs.GetSegmClassDef(StopYieldType.StopLine))), Utils.Scale(poly, ScaleFactor));
                            }
                            else
                            {
                                // clear the background for the yield markings or draw code line for yield
                                grfx.FillPolygon(new SolidBrush(GetDrawColor(lanes[0].Color, SegmClassDefs.GetSegmClassDef(StopYieldType.YieldLine))), Utils.Scale(poly, ScaleFactor));

                                if (Lanes[0].ColorMode == ColorMode.ImageColor)
                                {
                                    // When not drawing code, then draw the triangles for the yield line
                                    double dist = Utils.GetDistance(poly[2], poly[1]);
                                    double w = stopLineWidth * 2 / 3;
                                    double w2 = w / 2;
                                    int n = (int)(dist / (w + w2));
                                    double ofs = (dist - (n * w) - ((n - 1) * w2)) / 2;
                                    PointF[] p = new PointF[3];
                                    for (int j = 0; j < n; j++, ofs += w + w2)
                                    {
                                        p[0] = Utils.GetPoint(poly[0], angle, ofs);
                                        p[1] = Utils.GetPoint(p[0], angle, w);
                                        p[2] = Utils.GetPoint(poly[1], angle, ofs + w2);
                                        grfx.FillPolygon(new SolidBrush(stopLineColor), Utils.Scale(p, ScaleFactor));
                                    }
                                }
                            }

                            if ((StreetDescriptors[i].StopYieldType == StopYieldType.StopLineText) || (StreetDescriptors[i].StopYieldType == StopYieldType.YieldLineText))
                            {
                                string txt = StreetDescriptors[i].StopYieldType == StopYieldType.StopLineText ? "STOP" : "YIELD";

                                // First get size for unscaled text
                                float fontSize = (float)intersectionFontSize;
                                Font font = new Font("Impact", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                                SizeF textSize = grfx.MeasureString(txt, font);
                                offset += stopLineWidth + textSize.Height / 2;

                                // Now get scaled font and size
                                fontSize = (float)intersectionFontSize * ScaleFactor.Height;
                                font = new Font("Impact", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                                textSize = grfx.MeasureString(txt, font);

                                for (int j = 0; j < StreetDescriptors[i].LaneCountRight; j++)
                                {
                                    pl = StreetDescriptors[i].Lanes[j].LeftLine.Connectors[1].EndP1;
                                    pr = StreetDescriptors[i].Lanes[j].RightLine.Connectors[1].EndP0;
                                    double dist = Utils.GetDistance(pl, pr);
                                    PointF p = Utils.GetPoint(pl, angle, dist / 2);
                                    p = Utils.Scale(Utils.GetPoint(p, angle - Utils.RIGHT_ANGLE_RADIAN, offset), ScaleFactor);
                                    
                                    if (Lanes[0].ColorMode == ColorMode.ImageColor)
                                        Utils.DrawText(grfx, txt, p, angle, font, stopLineColor);
                                    else //if ((DrawWrongDirItems == true) || ((StreetDescriptors[i].Lanes[j].SegmClassDef != SegmClassDefs.ScdWrongDir) && (StreetDescriptors[i].Lanes[j].SegmClassDef != SegmClassDefs.ScdNothing)))
                                    {
                                        double w = textSize.Width * 0.85;
                                        double h1 = textSize.Height * 0.75;
                                        double h2 = textSize.Height * 0.65;

                                        PointF[] pp = new PointF[5];
                                        pp[0] = Utils.GetPoint(p, angle, -w / 2);
                                        pp[0] = Utils.GetPoint(pp[0], angle - Utils.RIGHT_ANGLE_RADIAN, -h1 / 2);
                                        pp[1] = Utils.GetPoint(pp[0], angle - Utils.RIGHT_ANGLE_RADIAN, h2);
                                        pp[2] = Utils.GetPoint(pp[1], angle, w);
                                        pp[3] = Utils.GetPoint(pp[0], angle, w);
                                        pp[4] = pp[0];
                                        grfx.FillPolygon(new SolidBrush(GetDrawColor(stopLineColor, SegmClassDefs.GetSegmClassDef(StreetDescriptors[i].StopYieldType))), pp);
                                    }
                                }
                            }
                        }
                     }

                    if (StreetDescriptors.Length == 3)
                    {
                        // Add missing shoulder line at forth side,
                        PointF[] poly = new PointF[5];
                        // Only in this condition a straight line can be drawn from end to end
                        if (StreetDescriptors[0].LaneCount >= StreetDescriptors[2].LaneCount)
                        {
                            poly[0] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].LeftLine.Connectors[0].EndP1;
                            poly[1] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].LeftLine.Connectors[0].EndP0;
                            poly[2] = StreetDescriptors[2].Lanes[0].RightLine.Connectors[0].EndP1;
                            poly[3] = StreetDescriptors[2].Lanes[0].RightLine.Connectors[0].EndP0;
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(lanes[0].RightLine.GetDrawColor()), Utils.Scale(poly, ScaleFactor));
                        }
                        else
                        {   // If lane count left is smaller than on the right, there is a misalignment and the corner needs shoulder lines 
                            double w = StreetDescriptors[2].Lanes[0].RightLine.Connectors[0].Width;
                            poly[0] = StreetDescriptors[2].Lanes[0].RightLine.Connectors[0].EndP0;
                            poly[1] = StreetDescriptors[2].Lanes[0].RightLine.Connectors[0].EndP1;
                            poly[2] = Utils.GetPoint(innerArea[3], lane0Angle, -intersectionRecess) ;
                            poly[3] = Utils.GetPoint(poly[2], lane0Angle-Utils.RIGHT_ANGLE_RADIAN, w);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(lanes[0].RightLine.GetDrawColor()), Utils.Scale(poly, ScaleFactor));

                            double d = Utils.GetDistance(innerArea[3], StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].LeftLine.Connectors[1].EndP0);
                            poly[0] = poly[2];
                            poly[1] = Utils.GetPoint(poly[0], lane0Angle - Utils.RIGHT_ANGLE_RADIAN, d);
                            poly[2] = Utils.GetPoint(poly[1], lane0Angle, w);
                            poly[3] = Utils.GetPoint(poly[0], lane0Angle, w);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(lanes[0].RightLine.GetDrawColor()), Utils.Scale(poly, ScaleFactor));
                        }
                    }

                    // had the driving dir index been assigned before ? Then draw limit lines over the open space
                    if ((drivingDirIdx >= 0) && (StreetDescriptors[drivingDirIdx].LaneCountRight > 1))
                    {
                        // Draw imaginary lane limit lines over the intersection area
                        PointF[] poly = GetInnerPolygon(new SizeF(1, 1));
                        int i = drivingDirIdx;
                        double dist = Utils.GetDistance(poly[i], poly[i + 1]);
                        double dirAngle = Utils.GetAngle(poly[i], poly[i + 1]);

                        dist += 2 * intersectionRecess;
                        for (int k = 0; k < StreetDescriptors[i].LaneCountRight - 1; k++)
                        {
                            poly[0] = Utils.GetPoint(StreetDescriptors[i].Lanes[k].LeftLine.Connectors[1].EndP0, dirAngle, -intersectionRecess);
                            poly[1] = Utils.GetPoint(StreetDescriptors[i].Lanes[k].LeftLine.Connectors[1].EndP1, dirAngle, -intersectionRecess);
                            poly[2] = Utils.GetPoint(poly[1], dirAngle, dist);
                            poly[3] = Utils.GetPoint(poly[0], dirAngle, dist);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdLaneLimitLine)), Utils.Scale(poly, ScaleFactor));
                        }
                    }

                    // In the end, correct some special cases for driving dir
                    if (Lanes[0].ColorMode != ColorMode.ImageColor)
                    {
                        if ((StreetDescriptors.Length == 2) && /*(StreetDescriptors[0].LaneCountRight == 0) && */(StreetDescriptors[0].Lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir))
                        {
                            // draw driving dir over it from starting point of driving direction lanes
                            PointF[] poly = GetInnerPolygon(new SizeF(1, 1));

                            double dist = -(StreetDescriptors[0].Lanes[0].LeftLine.Connectors[0].Width+1); // bigger to avoid rounding gaps
                            double dirAngle = StreetDescriptors[0].Lanes[0].LeftLine.Connectors[0].Angle;
                            poly[2] = Utils.GetPoint(poly[1], dirAngle, dist);
                            poly[3] = Utils.GetPoint(poly[0], dirAngle, dist);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdDrivingDir)), Utils.Scale(poly, ScaleFactor));
                        }
                    }

                    // Now draw the new round shoulder lines on top
                    Color shoulderColor = lanes[0].RightLine.GetDrawColor(); 
                    foreach (CornerRound cr in cornerRounds)
                        grfx.FillPath(new SolidBrush(shoulderColor), cr.GetScaledTopPath(ScaleFactor));

#if DEBUG_DRAW_INNER_LINES
                    grfx.DrawPolygon(new Pen(Color.Red, (float)lanes[0].OutlineLineWidth), Utils.Scale(innerArea, ScaleFactor));
                    Utils.DrawPointCircle(grfx, innerArea[0], ScaleFactor, Color.LightBlue, 3, 10);
                    Utils.DrawPointCircle(grfx, innerArea[1], ScaleFactor, Color.LightGreen, 3, 10);
#endif
                    break;

                case DrawMode.Overlay:
                    // Overlays are normally handled correctly with the class code assignment to their owning lanes.
                    // But T-Intersections have through lanes marked as left and right, but then wrong dir drawn over half of them when coming from the side street.
                    // To correctly handle this, the wrong dir code is drawn over the already drawn overlays.
                    if ((Lanes[0].ColorMode != ColorMode.ImageColor) && (DrawWrongDirItems == false) && 
                        (StreetDescriptors.Length == 2) && (StreetDescriptors[1].Lanes[0].SegmClassDef == SegmClassDefs.ScdDrivingDir))
                    {
                        // the T-intersection has lanes going through and are now drawn with class codes left and right completely
                        // -> draw wrong dir over right turn code from left to center
                        PointF[] poly = new PointF[5];
                        if (StreetDescriptors[0].LaneCountRight > 0)
                        {
                            poly[0] = StreetDescriptors[1].Lanes[0].Connectors[1].EndP0;
                            poly[1] = StreetDescriptors[0].Lanes[0].Connectors[0].EndP1;
                           
                            // To avoid drawing over the outer lines on the left side, recess points inside the outer lines of wrong dir lanes
                            if (StreetDescriptors[0].Lanes[0].RightLine != null)
                            {
                                double a10 = Utils.GetAngle(poly[1], poly[0]);
                                double w10 = Utils.GetDistance(poly[1], poly[0]);
                                poly[1] = StreetDescriptors[0].Lanes[0].RightLine.Connectors[0].EndP0;
                                poly[0] = Utils.GetPoint(poly[1], a10, w10);
                            }

                            poly[2] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCountRight - 1].Connectors[0].EndP0;
                            if (StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCountRight - 1].LeftLine != null)
                                poly[2] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCountRight - 1].LeftLine.Connectors[0].EndP1;

                            double w = Utils.GetDistance(poly[1], poly[2]);
                            double a = Utils.GetAngle(poly[1], poly[2]);
                            poly[3] = Utils.GetPoint(poly[0], a, w);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdWrongDir)), Utils.Scale(poly, ScaleFactor));
                        }

                        // draw wrong dir over left turn code from right to center
                        if (StreetDescriptors[0].LaneCountLeft > 0)
                        {
                            poly[0] = innerArea[2]; // innerPoly[2];
                            poly[1] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - 1].Connectors[1].EndP0;
                            poly[2] = StreetDescriptors[0].Lanes[StreetDescriptors[0].LaneCount - StreetDescriptors[0].LaneCountLeft].RightLine.Connectors[1].EndP0;
                            double w = Utils.GetDistance(poly[1], poly[2]);
                            double a = Utils.GetAngle(poly[1], poly[2]);
                            poly[3] = Utils.GetPoint(poly[0], a, w);
                            poly[4] = poly[0];
                            grfx.FillPolygon(new SolidBrush(GetDrawColor(SegmClassDefs.ScdWrongDir)), Utils.Scale(poly, ScaleFactor));
                        }
                        //Since lane codes had been drawn over some of the lines, restore them now on top
                        foreach (LaneElement le in StreetDescriptors[0].Lanes)
                        {
                            if ((le.RightLine != null) && (le != StreetDescriptors[0].Lanes[0]))
                                le.RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);

                            if (le.LeftLine != null)
                                le.LeftLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                        }
                    }
                    break;
            }

#if DEBUG_CORNER_ROUND_UPDATE_POINTS
            foreach (CornerRound cr in cornerRounds)
                cr.Draw(grfx, ScaleFactor, DrawMode);
#endif
        }


        /// <summary>
        /// Returns true, if the given point is inside any of the lanes belonging to this intersection or the inner area.
        /// </summary>
        /// <param name="P">Point to check if inside.</param>
        /// <returns>True, if the point is inside of the intersection or false if not.</returns>
        public override object IsInside(PointF P, bool IncludeOverlays)
        {
            object obj = base.IsInside(P, IncludeOverlays);
            if (obj != null)
                return obj;

            if ((StreetDescriptors.Length > 2) && Utils.IsInPolygon(P, GetInnerPolygon(new SizeF(1, 1))))
                return this;

            return null;
        }

        /// <summary>
        /// Perform the rotation of this object by the amount requested.
        /// </summary>
        /// <param name="Degrees">Angle in Degree to rotate.</param>
        public override void Rotate(double Degrees)
        {
            base.Rotate(Degrees);
            UpdateGeometries();
        }

        /// <summary>
        /// Returns the index in the StreetDescriptors array, the lane object belongs to.
        /// </summary>
        /// <param name="Lane">Lane to check.</param>
        /// <returns>Index of the StreetDescriptor containing the lane or -1 if not found.</returns>
        public int GetDescrIndex(LaneElement Lane, out int DescrLaneIdx)
        {
            if (Lane.Owner == this)
            {
                int ownerIdx = Lane.OwnerIdx;
                int descrIdx, totalIdx = 0;
                for (descrIdx = 0; descrIdx < StreetDescriptors.Length; descrIdx++)
                {
                    totalIdx += StreetDescriptors[descrIdx].LaneCount;
                    if (ownerIdx < totalIdx)
                    {
                        DescrLaneIdx = ownerIdx - (totalIdx- StreetDescriptors[descrIdx].LaneCount);
                        return descrIdx;
                    }
                }
            }
            DescrLaneIdx = -1;
            return -1;
        }

        /// <summary>
        /// Goes through the StreetDescriptors to increment the UseCounts for the SegmClassDef objects used in the intersection and then call the base class method to update the lanes' classes.
        /// </summary>
        public override void UpdateUseCounts()
        {
            for (int i = 0; i < StreetDescriptors.Length; i++)
            {
                for (int j = 0; j < StreetDescriptors[i].LaneCountRight; j++)
                {
                    SegmClassDefs.IncUseCount(SegmClassDefs.ScdDrivingDir);

                    if (StreetDescriptors[i].CrosswalkType != CrosswalkType.None)
                        SegmClassDefs.IncUseCount(StreetDescriptors[i].CrosswalkType);

                    if (StreetDescriptors[i].StopYieldType != StopYieldType.None)
                    {
                        SegmClassDefs.IncUseCount(StreetDescriptors[i].StopYieldType);
                        if (StreetDescriptors[i].StopYieldType == StopYieldType.StopLineText)
                            SegmClassDefs.IncUseCount(StopYieldType.StopLine);
                        if (StreetDescriptors[i].StopYieldType == StopYieldType.YieldLineText)
                            SegmClassDefs.IncUseCount(StopYieldType.YieldLine);
                    }

                    if (StreetDescriptors[i].LaneCountRight>1)
                        SegmClassDefs.IncUseCount(SegmClassDefs.ScdLaneLimitLine);
                }

                for (int j = 0; j < StreetDescriptors[i].LaneCountCenter; j++)
                    SegmClassDefs.IncUseCount(SegmClassDefs.ScdWrongDir);

                for (int j = 0; j < StreetDescriptors[i].LaneCountLeft; j++)
                {
                    SegmClassDefs.IncUseCount(SegmClassDefs.ScdWrongDir);
                    SegmClassDefs.IncUseCount(SegmClassDefs.ScdRightTurnDir);
                    SegmClassDefs.IncUseCount(SegmClassDefs.ScdLeftTurnDir);

                    if (StreetDescriptors[i].LaneCountLeft > 1)
                        SegmClassDefs.IncUseCount(SegmClassDefs.ScdLaneLimitLine);
                }
            }
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

            XmlNode nodeStreetDescriptor = nodeItem.AppendChild(Doc.CreateElement("street_descriptor"));
            foreach (StreetDescriptor sd in StreetDescriptors)
                sd.WriteToXml(Doc, nodeStreetDescriptor);

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


                XmlNode nodeStreetDescriptor = nodeItem.SelectSingleNode("street_descriptor");
                XmlNodeList nodeItems = nodeStreetDescriptor.SelectNodes("item");
                StreetDescriptor[] streetDescriptors = new StreetDescriptor[nodeItems.Count];
                int i = 0;
                foreach (XmlNode node in nodeItems)
                {
                    streetDescriptors[i++] =  StreetDescriptor.LoadFromXml(Doc, node, AppSettings);
                }

                double angle = Convert.ToDouble(nodeItem.SelectSingleNode("angle").InnerText);
                float x = Convert.ToSingle(nodeItem.SelectSingleNode("location_x").InnerText);
                float y = Convert.ToSingle(nodeItem.SelectSingleNode("location_y").InnerText);

                Intersection intersection = new Intersection(AppSettings, streetDescriptors);
                intersection.SetAngleAndLocation(angle, new PointF(x, y));
                intersection.ReadFromXml(Doc, nodeItem, AppSettings);
                return intersection;
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
        /// Gets the Lane 0 angle as reference for the complete intersection rotation.
        /// </summary>
        public double Lane0Angle
        {
            get { return lane0Angle; }
        }
#endregion Public Properties

    }
}
