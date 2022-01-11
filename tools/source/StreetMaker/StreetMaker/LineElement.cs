// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

// Activate to enable many Debug.WriteLine instructions for debugging the dashed line calculations
//#define DEBUG_WRITE_LINES


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
    /// The LineElement class is inherited from ShapedArea. It represents a lane limit line in the shape straight, curved or s-shaped. 
    /// The line can have different colors like yellow or white, it can be solid or dashed and even doubled.
    /// </summary>
    public class LineElement:ShapedArea
    {
        #region Private Fields
        /// <summary>Type of the line, like yellow or white, solid or dashed, single or doubled.</summary>
        private LineType lineType;
        /// <summary>When set to true, the line is dashed. Otherwise the line is solid.</summary>
        private bool dashed;
        /// <summary>When set to true, the line is a double line. Otherwise the line is just a single line.</summary>
        private bool doubled;
        /// <summary>When set to true, the line is centered to the edge of the lane. Otherwise the line edge will be aligned with the lane edge.</summary>
        private bool shared;

        /// <summary>Determines the length of the dashes when dashed is true.</summary>
        private double dashLength;
        /// <summary>Determines the starting phase of the dashes when dashed is true. This field asures that dashes may continue correctly from one elemenet to the next.</summary>
        private double dashPhase;
        /// <summary>Reference to the owner of this line object.</summary>
        private LaneElement owner;
        /// <summary>Calculated path length between the 2 CenterP in the center of the line </summary>
        private double pathLength;

        #endregion Private Fields

        #region Constructors
        /// <summary>
        /// Creates an instance of the LineElement class using defaults from AppSettings.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="Shape">Shape of this lane, straight, curved or s-shaped.</param>
        /// <param name="LineType">Type of the line, like yellow or white, solid or dashed, single or doubled.</param>
        public LineElement(AppSettings AppSettings, ShapeType Shape, LineType LineType) : this(AppSettings, Shape, LineType, AppSettings.MinInnerRadius, AppSettings.DefaultCurveAngle, AppSettings.LaneWidth/2) { }

        /// <summary>
        /// Creates an instance of the LineElement class using defaults from AppSettings.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="Shape">Shape of this lane, straight, curved or s-shaped.</param>
        /// <param name="LineType">Type of the line, like yellow or white, solid or dashed, single or doubled.</param>
        /// <param name="Radius">The inner radius of a curved shape. This field is only used for the curved shape.</param>
        /// <param name="CurveAngle">The sweeping angle of the curve piece. This field is only used for the curved shape.</param>
        /// <param name="SOffset">The offset of an s-shaped center points from the straight one. This field is only used for the s-shape.</param>
        public LineElement(AppSettings AppSettings, ShapeType Shape, LineType LineType, double Radius, double CurveAngle, double SOffset):base(AppSettings, Shape, AppSettings.LineWidth, Radius, CurveAngle, SOffset)
        {
            this.lineType = LineType;
            this.dashed = false;
            this.doubled = false;
            this.shared = true;
            this.dashLength = AppSettings.DashLength;
            this.SegmClassDef = SegmClassDefs.GetSegmClassDef(LineType);

            switch (LineType)
            {
                case LineType.None:
                    this.Color = Color.Transparent;
                    break;

                case LineType.Transparent:
                    this.Color = Color.Transparent;
                    break;

                case LineType.ShoulderLine:
                    this.Color = AppSettings.LineColorWhite;
                    this.shared = false;
                    break;

                case LineType.SingleWhiteSolid:
                    this.Color = AppSettings.LineColorWhite;
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

                case LineType.DoubleYellowSolidDashed:
                    this.Color = AppSettings.LineColorYellow;
                    this.doubled = true;
                    this.shared = false;
                    
                    break;

                case LineType.DoubleYellowDashedSolid:
                    this.Color = AppSettings.LineColorYellow;
                    this.doubled = true;
                    this.shared = false;
                    break;
            }

            Connectors[0].SuspendEvents();
            Connectors[1].SuspendEvents();

            Connectors[0].Connect += HandleConnectorConnectEvent;
            Connectors[1].Connect += HandleConnectorConnectEvent;
        }
        #endregion Constructors

        #region Private Methods
        /// <summary>
        /// Overwritten event handler for connector changes. Changes from the connectors are mainly from moving, rotating the element or from sizing it. 
        /// </summary>
        /// <param name="sender">Connector object that sent the event.</param>
        protected override void HandleConnectorChangeEvent(Connector sender)
        {
#if DEBUG_WRITE_LINES
            Debug.WriteLine("Connector Change:  dashed:" + dashed.ToString());
#endif
            base.HandleConnectorChangeEvent(sender);
            UpdateGeometries();
        }


        /// <summary>
        /// Connector Connect event handler, executed whenever the connection status of the connectors changes.
        /// A disconnect leaves the sender connection reference as null. A connect event leaves it with a new reference.
        /// </summary>
        /// <param name="sender">Connector that changed the connection status.</param>
        protected void HandleConnectorConnectEvent(Connector sender)
        {
#if DEBUG_WRITE_LINES
            Debug.WriteLine("Connect Event:  dashed:"+dashed.ToString());
#endif
            UpdateGeometries();
        }

        /// <summary>
        /// Returns the DashSyncConnectorIdx of the owner StreetElement, if all references can be resolved or 0, if otherwise.
        /// </summary>
        /// <returns>StreetElement.DashSyncConnectorIdx if possible or 0.</returns>
        private int GetDashPhaseSourceIdx()
        {
            if (owner == null)
                return 0;

            if ((owner.DashSyncConnectorIdx < 0) && (owner.DashSyncConnectorIdx >= Connectors.Length))
                return 0;

            if (Connectors[owner.DashSyncConnectorIdx].Connection == null)
                return 0;

            return owner.DashSyncConnectorIdx;
        }

        /// <summary>
        /// Returns an adjusted phase to avoid resulting gaps.
        /// </summary>
        /// <param name="x">A dash phase to be checked.</param>
        /// <returns>DashLength adjusted phase.</returns>
        private double AdjustPhase(double x)
        {
            if (x > DashLength)
                x -= 2 * DashLength;
            else if (x < -DashLength)
                x += 2 * DashLength;
            return x;
        }

        /// <summary>
        /// Gets the LineELement that owns the connected line if exists.
        /// </summary>
        /// <param name="ConnectorIdx">Index of the connector in the Connectors array.</param>
        /// <returns>Returns the reference to the connected LineElement or null.</returns>
        private LineElement GetConnectionLineOwner(int ConnectorIdx)
        {
            if ((ConnectorIdx >= Connectors.Length) || (Connectors[ConnectorIdx].Connection == null))
                return null;
            return Connectors[ConnectorIdx].Connection.Owner as LineElement;
        }

        /// <summary>
        /// Returns the DashSyncOrder of the connected LineELement by calling GetConnectionLineOwner or -1 if doesn't exist.
        /// </summary>
        /// <param name="ConnectorIdx">Index of the connector in the Connectors array.</param>
        /// <returns>DashSyncOrder of the connection line owner or -1.</returns>
        private int GetConnectionlineDashSyncOrder(int ConnectorIdx)
        {
            LineElement le = GetConnectionLineOwner(ConnectorIdx);
            if (le != null)
                return le.DashSyncOrder;
            else
                return -1;
        }

        #endregion Private Methods


        #region Public Methods

        /// <summary>
        /// Updates all geometrical parameter for correct dash phases to connected street elements.
        /// </summary>
        public void UpdateGeometries()
        {
            pathLength = GetCenterPathLength();

            dashPhase = 0;
            double endPhase = 0;

            if (dashed)
            {
                int connectorIdx = GetDashPhaseSourceIdx();
                double connectorPhase = 0;

                int n = (int)(pathLength / (2 * dashLength));
                double totalDashedLength = (n + 1) * (2 * dashLength);

                int thisOrder = DashSyncOrder;
                if (Connectors[connectorIdx].Connection != null)
                {
                    int connOrder = GetConnectionlineDashSyncOrder(connectorIdx);
                    if ((connOrder >= 0) && (thisOrder > connOrder))
                    {
                        connectorPhase = Connectors[connectorIdx].Connection.DashPhase;
                        if ((connectorIdx == Connectors[connectorIdx].Connection.OwnerIdx) && (connectorIdx == 0))
                            connectorPhase += dashLength;
                    }
                }
                else
                {
                    LineElement le = NeighborLine;
                    if (le != null)
                    {
                        connectorIdx = le.GetDashPhaseSourceIdx();
                        int connOrder = le.GetConnectionlineDashSyncOrder(connectorIdx);
                        if ((connOrder >= 0) && (thisOrder > connOrder))
                        {
                            connectorPhase = le.Connectors[connectorIdx].Connection.DashPhase;
                            if ((connectorIdx == le.Connectors[connectorIdx].Connection.OwnerIdx) && (connectorIdx == 0))
                                connectorPhase += dashLength;
                        }
                    }
                }

                if (connectorIdx == 0)
                {
                    dashPhase = AdjustPhase(connectorPhase);
                    endPhase = AdjustPhase(totalDashedLength - pathLength + dashPhase);
                }
                else
                {
                    dashPhase = AdjustPhase(connectorPhase - totalDashedLength + pathLength);
                    endPhase = AdjustPhase(-connectorPhase);
                }

#if DEBUG_WRITE_LINES
                Debug.WriteLine("UpdateGeometries   se:" + Owner.Owner.DashSyncOrder.ToString() + " le:" + Owner.OwnerIdx.ToString() + (this == Owner.LeftLine ? " left" : " right") + " connectorIdx:" + connectorIdx.ToString() + " connectorPhase:" + connectorPhase.ToString("F1") + " pathLength:" + pathLength.ToString("F1") + " totalDashedLength:" + totalDashedLength.ToString("F1") + " endPhase0:" + (totalDashedLength - pathLength + dashPhase).ToString("F1") + " dashPhase:" + dashPhase.ToString("F1") + " endPhase:" + endPhase.ToString("F1"));
#endif
            }
            Connectors[0].DashPhase = dashPhase;
            Connectors[1].DashPhase = endPhase;

        }

        /// <summary>
        /// Sets the owner field in this instance and other fields related to this update.
        /// </summary>
        /// <param name="Owner">LaneElement that owns this line.</param>
        public void SetOwner(LaneElement Owner)
        {
            this.owner = Owner;
            if ((lineType == LineType.DoubleYellowSolidDashed) && IsRight)
                this.dashed = true;
            else if ((lineType == LineType.DoubleYellowDashedSolid) && IsLeft)
                this.dashed = true;
        }

 
        /// <summary>
        /// Draw method of this class which is overwritten from the ShapedArea class. Here it only handles dashed lines in BaseLayer mode while all other drawing uses base.Draw method.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public override void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            if ((dashed == true) && (DrawMode == DrawMode.BaseLayer) && (ColorMode == ColorMode.ImageColor))
            {
                double alpha,l, length = pathLength;
                PointF p00, p01, p10, p11;

                switch (Shape)
                {
                    case ShapeType.Straight:
                        alpha = Utils.GetAngle(Connectors[0].EndP0, Connectors[1].EndP0);

                        if (dashPhase <= 0)
                        {
                            p00 = Connectors[0].DrawP0;
                            p01 = Connectors[0].DrawP1;
                            l = dashLength + dashPhase;
                        }
                        else
                        {
                            p00 = Utils.GetPoint(Connectors[0].DrawP0, alpha, dashPhase);
                            p01 = Utils.GetPoint(Connectors[0].DrawP1, alpha, dashPhase);
                            l = dashLength;
                            length -= dashPhase;
                        }
#if DEBUG_WRITE_LINES
                        Debug.WriteLine("Draw Straight:   se:" + Owner.Owner.DashSyncOrder.ToString() + " le:" + Owner.OwnerIdx.ToString() + (this == Owner.LeftLine ? " left" : " right") + " dashPhase:" + dashPhase.ToString("F1") + " pathLength:" + pathLength.ToString("F1") + " dashLength:" + dashLength.ToString("F1") + " l:" + l.ToString("F1")+" length:"+length.ToString("F1"));
#endif
                        do
                        {
                            p10 = Utils.GetPoint(p00, alpha, l);
                            p11 = Utils.GetPoint(p01, alpha, l);

                            grfx.FillPolygon(new SolidBrush(Color), GetScaledStraightPolygon(p00, p01, p10, p11, ScaleFactor));

                            length -= l + dashLength;
#if DEBUG_WRITE_LINES
                            Debug.WriteLine("Draw Straight loop:  length:" + length.ToString("F1") + "  l:" + l.ToString("F1"));
#endif
                            if (length > 0)
                            {
                                p00 = Utils.GetPoint(p10, alpha, dashLength);
                                p01 = Utils.GetPoint(p11, alpha, dashLength);
                                l = Math.Min(dashLength, length);
                                if (l < dashLength / 10)
                                    length = 0;

#if DEBUG_WRITE_LINES
                                Debug.WriteLine("Draw Straight length>0:   l:" + l.ToString("F1"));
#endif 
                            }

                        } while (length > 0);
#if DEBUG_WRITE_LINES
                        Debug.WriteLine("Draw Straight done:  length:" + length.ToString("F1"));
#endif
                        break;

                    case ShapeType.Curve:
                        if (curveAngle > 0)
                        {
                            if (dashPhase <= 0)
                            {
                                p00 = Connectors[0].DrawP0;
                                p01 = Connectors[0].DrawP1;
                                l = dashLength + dashPhase;
                            }
                            else
                            {
                                p00 = Utils.GetPoint(circleCenter, Connectors[0].DrawP0, dashPhase);
                                p01 = Utils.GetPoint(p00, Utils.GetAngle(p00, circleCenter), Connectors[0].DrawWidth);
                                l = dashLength;
                                length -= dashPhase;
                            }

                            do
                            {
                                p10 = Utils.GetPoint(circleCenter, p00, l);
                                p11 = Utils.GetPoint(p10, Utils.GetAngle(p10, circleCenter), Connectors[0].DrawWidth);

                                grfx.FillPath(new SolidBrush(Color), GetScaledCurvePath(p00, p01, p10, p11, ScaleFactor));

                                length -= l + dashLength;

                                if (length > 0)
                                {
                                    p00 = Utils.GetPoint(circleCenter, p10, dashLength);
                                    p01 = Utils.GetPoint(p00, Utils.GetAngle(p00, circleCenter), Connectors[0].DrawWidth);
                                    l = Math.Min(dashLength, length);
                                    if (l < dashLength / 10)
                                        length = 0;
                                }

                            } while (length > 0);
                        }
                        else
                        {
                            if (dashPhase <= 0)
                            {
                                p00 = Connectors[0].DrawP0;
                                p01 = Connectors[0].DrawP1;
                                l = dashLength + dashPhase;
                            }
                            else
                            {
                                p01 = Utils.GetPoint(circleCenter, Connectors[0].DrawP1, -dashPhase);
                                p00 = Utils.GetPoint(p01, Utils.GetAngle(p01, circleCenter), Connectors[0].DrawWidth);
                                l = dashLength;
                                length -= dashPhase;
                            }

                            do
                            {
                                p11 = Utils.GetPoint(circleCenter, p01, -l);
                                p10 = Utils.GetPoint(p11, Utils.GetAngle(p11, circleCenter), Connectors[0].DrawWidth);

                                //grfx.FillPolygon(new SolidBrush(Color), GetScaledStraightPolygon(p00, p01, p10, p11, ScaleFactor));

                                length -= l + dashLength;

                                if (length > 0)
                                {
                                    p01 = Utils.GetPoint(circleCenter, p11, -dashLength);
                                    p00 = Utils.GetPoint(p01, Utils.GetAngle(p01, circleCenter), Connectors[0].DrawWidth);
                                    l = Math.Min(dashLength, length);
                                    if (l < dashLength / 10)
                                        length = 0;
                                }

                            } while (length > 0);
                        }
                        break;

                    case ShapeType.S_Shape:
                        PointF[][] fitPoints = GetSPathFitEndPoints();
                        double delta = dashLength / length;
                        double step = delta / 10;
                        double pos = 0;

                        if (dashPhase <= 0)
                            l = dashLength + dashPhase;
                        else
                        {
                            l = dashLength;
                            pos += dashPhase;
                        }
#if DEBUG_WRITE_LINES
                        Debug.WriteLine("Draw S_Shape:   se:" + Owner.Owner.DashSyncOrder.ToString() + " le:" + Owner.OwnerIdx.ToString() + (this == Owner.LeftLine ? " left" : " right") + " dashPhase:" + dashPhase.ToString("F1") + " pathLength:" + pathLength.ToString("F1") + " dashLength:" + dashLength.ToString("F1") + " l:" + l.ToString("F1") + " length:" + length.ToString("F1"));
#endif
                        do
                        {
                            grfx.FillPolygon(new SolidBrush(Color), GetScaledSPolygon(fitPoints, pos / length, (pos + l) / length, step, ScaleFactor));

                            pos += l + dashLength;
#if DEBUG_WRITE_LINES
                            Debug.WriteLine("Draw S_Shape loop:  pos:" + pos.ToString("F1") + "  l:" + l.ToString("F1"));
#endif 
                            if (pos < length)
                            {
                                l = Math.Min(dashLength, length - pos);
                                if (l < dashLength / 10)
                                    pos = length;
#if DEBUG_WRITE_LINES
                                Debug.WriteLine("Draw S_Shape pos<length:   l:" + l.ToString("F1"));
#endif
                            }

                        } while (pos < length);
#if DEBUG_WRITE_LINES
                        Debug.WriteLine("Draw S_Shape done:  pos:" + pos.ToString("F1"));
#endif
                        break;
                }

            }
            else base.Draw(grfx, ScaleFactor, DrawMode);
        }
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the Dashed property. When set to true, the line is dashed. Otherwise the line is solid.
        /// </summary>
        public bool Dashed
        {
            get { return dashed; }
            set { dashed = value; }
        }

        /// <summary>
        /// Gets or sets the Doubled property. When set to true, the line is a double line. Otherwise the line is just a single line.
        /// </summary>
        public bool Doubled
        {
            get { return doubled; }
            set
            {
                doubled = value;
            }
        }

        /// <summary>
        /// Gets or sets the Shared property. When set to true, the line is centered to the edge of the lane. Otherwise the line edge will be aligned with the lane edge.
        /// </summary>
        public bool Shared
        {
            get { return shared; }
            set
            {
                shared = value;

            }
        }

        /// <summary>
        /// Gets or sets the DashLength property. Determines the length of the dashes when dashed is true.
        /// </summary>
        public double DashLength
        {
            get { return dashLength; }
            set { dashLength = value; }
        }

        /// <summary>
        /// Gets the owner LaneElement of this line.
        /// </summary>
        public LaneElement Owner
        {
            get { return owner;  }
        }

        /// <summary>
        /// Gets the owning StreetElement, the owner of the lane that owns this object or null if none is assigned.
        /// </summary>
        public StreetElement OwnerStreet
        {
            get
            {
                if (owner != null)
                    return owner.Owner;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the close LineElement of the neighboring lane owned by the same StreetElement or nul if not available.
        /// </summary>
        public LineElement NeighborLine
        {
            get
            {
                if (OwnerStreet == null)
                    return null;

                if (IsLeft)
                {
                    int idx = Owner.OwnerIdx + 1;
                    if ((idx < Owner.Owner.Lanes.Length) && (Owner.Owner.Lanes[idx] != null))
                        return Owner.Owner.Lanes[idx].RightLine;
                }
                else
                {
                    int idx = Owner.OwnerIdx - 1;
                    if ((idx >= 0) && (Owner.Owner.Lanes[idx] != null))
                        return Owner.Owner.Lanes[idx].LeftLine;
                 }
                return null;
            }
        }

        /// <summary>
        /// Gets the DashSyncOrder of the OwnerStreet or -1 if not resolvable.
        /// </summary>
        public int DashSyncOrder
        {
            get
            {
                StreetElement se = OwnerStreet;
                if (se != null)
                    return se.DashSyncOrder;
                else
                    return -1;
            }
        }
        /// <summary>
        /// Returns true, if this line is the left line of the owning lane object.
        /// </summary>
        public bool IsLeft
        {
            get 
            {
                if (owner == null) return false;
                return this == owner.LeftLine; 
            }
        }

        /// <summary>
        /// Returns true, if this line is the right line of the owning lane object.
        /// </summary>
        public bool IsRight
        {
            get
            {
                if (owner == null) return false;
                return this == owner.RightLine;
            }
        }


        #endregion Public Properties

    }
}
