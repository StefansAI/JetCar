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
using System.Xml;

namespace StreetMaker
{
    /// <summary>
    /// The LaneElement class is inherited from ShapedArea. It represents a piece of a lane straight, curved or s-shaped with border line objects
    /// </summary>
    public class LaneElement:ShapedArea
    {
        #region Private Fields
        /// <summary>Border line of this lane on the left side in driving direction.</summary>
        private LineElement leftLine;
        /// <summary>Border line of this lane on the right side in driving direction.</summary>
        private LineElement rightLine;
        /// <summary>Spacing of double lines.</summary>
        private double lineSpace;
        /// <summary>Index of the connector in the Connectors array to synchronize the line dashes.</summary>
        private int dashSyncConnectorIdx;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Reference to the owner object of this lane.</summary>
        public readonly StreetElement Owner;
        /// <summary>Index of this instance in the owner's lane array.</summary>
        public readonly int OwnerIdx;
        /// <summary>List of overlay objects belonging to this lane.</summary>
        public readonly List<Overlay> Overlays;
        #endregion Public Fields

        #region Constructors
        /// <summary>
        /// Creates an instance of the LaneElement class using defaults from AppSettings.
        /// </summary>
        /// <param name="Owner">Reference to the owner object of this lane.</param>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="Shape">Shape of this lane, straight, curved or s-shaped.</param>
        public LaneElement(StreetElement Owner, int OwnerIdx, AppSettings AppSettings, ShapeType Shape) : this(Owner, OwnerIdx, AppSettings, Shape, AppSettings.MinInnerRadius, AppSettings.DefaultCurveAngle, AppSettings.LaneWidth/2) { }

        /// <summary>
        /// Creates an instance of the LaneElement class using defaults from AppSettings.
        /// </summary>
        /// <param name="Owner">Reference to the owner object of this lane.</param>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="Shape">Shape of this lane, straight, curved or s-shaped.</param>
        /// <param name="Radius">The inner radius of the lane. This field is only used for the curved shape.</param>
        /// <param name="CurveAngle">The sweeping angle of the curve piece. This field is only used for the curved shape.</param>
        /// <param name="SOffset">The offset of an s-shaped center points from the straight one. This field is only used for the s-shape.</param>
        public LaneElement(StreetElement Owner, int OwnerIdx, AppSettings AppSettings, ShapeType Shape, double Radius, double CurveAngle, double SOffset):base(AppSettings, Shape, AppSettings.LaneWidth, Radius, CurveAngle, SOffset) 
        {
            this.Owner = Owner;
            this.OwnerIdx = OwnerIdx;
            this.lineSpace = AppSettings.LineSpace;
            this.eventsBlocked = true;
            this.Connectors[0].CatchDistance = AppSettings.PointCatchDistance;
            this.Connectors[1].CatchDistance = -AppSettings.PointCatchDistance;
            this.eventsBlocked = false;
            this.dashSyncConnectorIdx = 0;
            this.Connectors[0].Connect += HandleConnectorConnectEvent;
            this.Connectors[1].Connect += HandleConnectorConnectEvent;
            this.Overlays = new List<Overlay>();
        }
        #endregion Constructors

        #region Protected Methods
        /// <summary>
        /// Overwritten event handler for connector changes. Changes from the connectors are mainly from moving, rotating the element or from sizing it. The differentiation is made by the state of EditModeMove.
        /// The inherited handler is executed first. After that the LaneElement adjusts the owened lines.
        /// </summary>
        /// <param name="sender">Connector object that sent the event.</param>
        protected override void HandleConnectorChangeEvent(Connector sender)
        {
            base.HandleConnectorChangeEvent(sender);

            if (LeftLine != null)
            {
                LeftLine.Connectors[0].Angle = Connectors[0].Angle;
                LeftLine.Connectors[1].Angle = Connectors[1].Angle;

                if (LeftLine.Shared == true)
                {
                    LeftLine.Connectors[0].CenterP = Connectors[0].EndP0;
                    LeftLine.Connectors[1].CenterP = Connectors[1].EndP0;
                }
                else if (LeftLine.Doubled == true)
                {
                    LeftLine.Connectors[0].EndP0 = Utils.GetPoint(Connectors[0].EndP0, Connectors[0].Angle, +lineSpace / 2);
                    LeftLine.Connectors[1].EndP0 = Utils.GetPoint(Connectors[1].EndP0, Connectors[1].Angle, +lineSpace / 2);
                }
                else
                {
                    LeftLine.Connectors[0].EndP0 = Connectors[0].EndP0;
                    LeftLine.Connectors[1].EndP0 = Connectors[1].EndP0;
                }

                switch (shape)
                {
                    case ShapeType.Straight:
                        LeftLine.Length = length;
                        break;

                    case ShapeType.Curve:
                        LeftLine.CircleCenter = circleCenter;
                        if (curveAngle>=0)
                            LeftLine.InnerRadius = Utils.GetDistance(circleCenter, LeftLine.Connectors[0].EndP1);
                        else
                            LeftLine.InnerRadius = Utils.GetDistance(circleCenter, LeftLine.Connectors[0].EndP0);
                        LeftLine.CurveAngle = curveAngle;
                        break;

                    case ShapeType.S_Shape:
                        LeftLine.Length = length;
                        LeftLine.SOffset = sOffset;
                        break;
                }
            }

            if (RightLine != null)
            {
                RightLine.Connectors[0].Angle = Connectors[0].Angle;
                RightLine.Connectors[1].Angle = Connectors[1].Angle;
                if (RightLine.Shared == true)
                {
                    RightLine.Connectors[0].CenterP = Connectors[0].EndP1;
                    RightLine.Connectors[1].CenterP = Connectors[1].EndP1;
                }
                else if (RightLine.Doubled == true)
                {
                    RightLine.Connectors[0].EndP1 = Utils.GetPoint(Connectors[0].EndP1, Connectors[0].Angle, -lineSpace / 2);
                    RightLine.Connectors[1].EndP1 = Utils.GetPoint(Connectors[1].EndP1, Connectors[1].Angle, -lineSpace / 2);
                }
                else
                {
                    RightLine.Connectors[0].EndP1 = Connectors[0].EndP1;
                    RightLine.Connectors[1].EndP1 = Connectors[1].EndP1;
                }

                switch (shape)
                {
                    case ShapeType.Straight:
                        RightLine.Length = Length;
                        break;

                    case ShapeType.Curve:
                        RightLine.CircleCenter = circleCenter;
                        if (curveAngle >= 0)
                            RightLine.InnerRadius = Utils.GetDistance(circleCenter, RightLine.Connectors[0].EndP1);
                        else
                            RightLine.InnerRadius = Utils.GetDistance(circleCenter, RightLine.Connectors[0].EndP0);
                        RightLine.CurveAngle = curveAngle;
                        break;

                    case ShapeType.S_Shape:
                        RightLine.Length = length;
                        RightLine.SOffset = sOffset;
                        break;
                }
            }
        }

        /// <summary>
        /// Connector Connect event handler, executed whenever the connection status of the connectors changes.
        /// A disconnect leaves the sender connection reference as null. A connect event leaves it with a new reference.
        /// </summary>
        /// <param name="sender">Connector that changed the connection status.</param>
        protected virtual void HandleConnectorConnectEvent(Connector sender)
        {
            if (sender.Connection == null)
            {
                // at disconnect of the lane connector, disconnect the related line connectors
                if (LeftLine != null)
                    LeftLine.Connectors[sender.OwnerIdx].Connection = null;

                if (RightLine != null)
                    RightLine.Connectors[sender.OwnerIdx].Connection = null;
            }
            else 
            {
                // at connect of the lane connector, try to connect the related line connectors
                LaneElement connectionLane = sender.ConnectionOwner as LaneElement;
                if (connectionLane != null)
                {
                    if (LeftLine != null)
                    {
                        double dist = double.MaxValue;
                        if (connectionLane.LeftLine != null)
                            dist = Utils.GetDistance(LeftLine.Connectors[sender.OwnerIdx].CenterP,connectionLane.LeftLine.Connectors[sender.Connection.OwnerIdx].CenterP);

                        if (dist < AppSettings.MAX_CONN_DIST_TOL)
                            LeftLine.Connectors[sender.OwnerIdx].Connection = connectionLane.LeftLine.Connectors[sender.Connection.OwnerIdx];
                        else
                        {
                            if (connectionLane.RightLine != null)
                                dist = Utils.GetDistance(LeftLine.Connectors[sender.OwnerIdx].CenterP, connectionLane.RightLine.Connectors[sender.Connection.OwnerIdx].CenterP);

                            if (dist < AppSettings.MAX_CONN_DIST_TOL)
                                LeftLine.Connectors[sender.OwnerIdx].Connection = connectionLane.RightLine.Connectors[sender.Connection.OwnerIdx];
                        }
                    }

                    if (RightLine != null)
                    {
                        double dist = double.MaxValue;
                        if (connectionLane.RightLine != null)
                            dist = Utils.GetDistance(RightLine.Connectors[sender.OwnerIdx].CenterP, connectionLane.RightLine.Connectors[sender.Connection.OwnerIdx].CenterP);

                        if (dist < AppSettings.MAX_CONN_DIST_TOL)
                            RightLine.Connectors[sender.OwnerIdx].Connection = connectionLane.RightLine.Connectors[sender.Connection.OwnerIdx];
                        else
                        {
                            if (connectionLane.LeftLine != null)
                                dist = Utils.GetDistance(RightLine.Connectors[sender.OwnerIdx].CenterP, connectionLane.LeftLine.Connectors[sender.Connection.OwnerIdx].CenterP);

                            if (dist < AppSettings.MAX_CONN_DIST_TOL)
                                RightLine.Connectors[sender.OwnerIdx].Connection = connectionLane.LeftLine.Connectors[sender.Connection.OwnerIdx];
                        }
                    }

                }
            }
        }
        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Draw method of this class which is overwritten from the ShapedArea class. It calls the base method first and then performs drawing of this shape depending on the DrawMode.
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
                    Connectors[0].Draw(grfx, ScaleFactor, DrawMode);
                    Connectors[1].Draw(grfx, ScaleFactor, DrawMode);
                    break;

                case DrawMode.Background:
                    break;

                case DrawMode.BaseLayer:
                    break;

                case DrawMode.TopLayer:
                    if (LeftLine != null)
                    {
                        LeftLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);

                        if (LeftLine.Doubled && (ColorMode != ColorMode.ImageColor))
                        {
                            ShapedArea leftSpace = new ShapedArea(LeftLine, lineSpace / 2);

                            leftSpace.Connectors[0].EndP0 = Connectors[0].EndP0;
                            leftSpace.Connectors[1].EndP0 = Connectors[1].EndP0;

                            if (shape == ShapeType.Curve)
                            {
                                leftSpace.CircleCenter = circleCenter;
                                if (curveAngle >= 0)
                                    leftSpace.InnerRadius = Utils.GetDistance(circleCenter, leftSpace.Connectors[0].EndP1);
                                else
                                    leftSpace.InnerRadius = Utils.GetDistance(circleCenter, leftSpace.Connectors[0].EndP0);
                                leftSpace.CurveAngle = curveAngle;
                            }
                            //                            leftSpace.Color = Color.Red;
                            leftSpace.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                        }
                    }

                    if (RightLine != null)
                    {
                        RightLine.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                        if (RightLine.Doubled && (ColorMode != ColorMode.ImageColor))
                        {
                            ShapedArea rightSpace = new ShapedArea(RightLine, lineSpace / 2);

                            rightSpace.Connectors[0].EndP1 = Connectors[0].EndP1;
                            rightSpace.Connectors[1].EndP1 = Connectors[1].EndP1;

                            if (shape == ShapeType.Curve)
                            {
                                rightSpace.CircleCenter = circleCenter;
                                if (curveAngle >= 0)
                                    rightSpace.InnerRadius = Utils.GetDistance(circleCenter, rightSpace.Connectors[0].EndP1);
                                else
                                    rightSpace.InnerRadius = Utils.GetDistance(circleCenter, rightSpace.Connectors[0].EndP0);
                                rightSpace.CurveAngle = curveAngle;
                            }
                            rightSpace.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
                        }
                    }
                    break;

                case DrawMode.Overlay:
                    if (ColorMode == ColorMode.ImageColor)
                    {
                        foreach (Overlay overlay in Overlays)
                            overlay.Draw(grfx, ScaleFactor, DrawMode);
                    }
                    else
                    {
                        if (Owner != null)
                        {
                            if ((Owner.DrawWrongDirItems == true) || ((SegmClassDef != SegmClassDefs.ScdWrongDir) && (SegmClassDef != SegmClassDefs.ScdNothing)))
                            {
                                foreach (Overlay overlay in Overlays)
                                    if ((Utils.GetDistance(Owner.CameraPoint, overlay.StartPoint) < Owner.MaxDetailDist) || 
                                        (Utils.GetDistance(Owner.CameraPoint, overlay.EndPoint) < Owner.MaxDetailDist))
                                        overlay.Draw(grfx, ScaleFactor, DrawMode);
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Checks, if the point is inside of the area of any overlay belonging to this lane. If it is, this method returns the reference to that object or null if not.
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <returns>Reference to the overlay object if the point was inside an overlay area or null if not.</returns>
        public object IsInsideOverlay(PointF P)
        {
            foreach (Overlay ol in Overlays)
                if (ol.IsInside(P))
                    return ol;
            return null;
        }

        /// <summary>
        /// Checks if the given point is inside of the area of this lane or inside an overlay belonging to this lane.
        /// If an overlay is found, the refrence to the overlay is returned. Otherwise
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <param name="IncludeOverlays">If true, overlays will be checked first and the reference to a found overlay will be returned.</param>
        /// <returns>Reference to this object one of it's overlay, if inside.</returns>
        public object IsInside(PointF P, bool IncludeOverlays)
        {
            if (IncludeOverlays)
            {
                object obj = IsInsideOverlay(P);
                if (obj != null)
                    return obj;
            }
            return base.IsInside(P);
        }

        /// <summary>
        /// Adjusts the given referenced point to the center of the lane snaping to the step size distance form the 0 connector. 
        /// It also adjusts the direction angle to match the direction angle at that center point.
        /// </summary>
        /// <param name="P">Point to adjust to the center in a multiple of the DistanceStepSize from the 0 connector.</param>
        /// <param name="DirectionAngle">Angle to adjust to the true direction at the adjusted point.</param>
        /// <param name="RefPointDistance">Resulting distance of the adjusted point from the 0 connector.</param>
        /// <param name="DistanceStepSize">Step size to use for the RefPointDistance adjustment. </param>
        public void AdjustCenter(ref PointF P, ref double DirectionAngle, ref double RefPointDistance, double DistanceStepSize)
        {
            double dirAngle = 0;
            switch (shape)
            {
                case ShapeType.Straight:
                    {
                        double centerAngle = Utils.GetAngle(Connectors[0].CenterP, Connectors[1].CenterP);
                        double pointAngle = Utils.GetAngle(Connectors[0].CenterP, P);
                        double pointDist = Utils.GetDistance(Connectors[0].CenterP, P);
                        double centerDist = pointDist * Math.Cos(pointAngle - centerAngle);
                        int n = (int)Math.Round(centerDist / DistanceStepSize);
                        RefPointDistance = n * DistanceStepSize;

                        P = Utils.GetPoint(Connectors[0].CenterP, centerAngle, RefPointDistance);
                        dirAngle = centerAngle;
                    }
                    break;

                case ShapeType.Curve:
                    {
                        double centerAngle = Utils.GetAngle(circleCenter, P);
                        double midRadius = innerRadius + Connectors[0].Width / 2;
                        PointF p = Utils.GetPoint(circleCenter, centerAngle, midRadius);
                        double pointDist = Utils.GetDistance(Connectors[0].CenterP, p);
                        int n = (int)Math.Round(pointDist / DistanceStepSize);
                        pointDist = n * DistanceStepSize;

                        centerAngle = Utils.GetAngle(circleCenter, p);
                        P = Utils.GetPoint(circleCenter, centerAngle, midRadius);
                        RefPointDistance = Utils.GetDistance(Connectors[0].CenterP, P);
                        dirAngle = Utils.LimitRadian(centerAngle - Utils.RIGHT_ANGLE_RADIAN);
                    }
                    break;

                case ShapeType.S_Shape:
                    {
                        double pointDist = Utils.GetDistance(Connectors[0].CenterP, P);
                        int n = (int)Math.Round(pointDist / DistanceStepSize);
                        RefPointDistance = n * DistanceStepSize;

                        double l = GetCenterPathLength();
                        PointF[] triple = GetBezierPoints(GetSPathFitCenterPoints(), (RefPointDistance-1)/l, (RefPointDistance+1)/l, 1/l);

                        dirAngle = Utils.GetAngle(triple[0], triple[2]);
                        P = triple[1];

                    }
                    break;
            }

            if ((Connectors[0].Mode == ConnectorMode.In) || (Connectors[1].Mode == ConnectorMode.Out))
                DirectionAngle = Utils.LimitRadian(dirAngle+Math.PI);
            else if ((Connectors[1].Mode == ConnectorMode.In) || (Connectors[0].Mode == ConnectorMode.Out))
                DirectionAngle = dirAngle; 
            else if (Math.Abs(DirectionAngle - dirAngle) < Utils.RIGHT_ANGLE_RADIAN)
                DirectionAngle = dirAngle;
            else
                DirectionAngle = Utils.LimitRadian(dirAngle + Math.PI);
        }


        /// <summary>
        /// Calculate the coordinates of a point in the center of the lane at the passed fraction of the lane length.
        /// </summary>
        /// <param name="LengthFraction">Value between 0 and 1.0 as a fraction of the lane length, like a precantage value.</param>
        /// <param name="DirectionAngle">Output of the direction of the lane at the lane center point.</param>
        /// <returns>Calculated point at the center of the lane.</returns>
        public PointF GetLaneCenter(double LengthFraction, ref double DirectionAngle)
        {
            PointF laneCenterPoint= Connectors[0].CenterP;
            double centerPathLength = GetCenterPathLength();
            double centerLengthFraction = centerPathLength * Math.Min(LengthFraction,1);

            switch (Shape)
            {
                case ShapeType.Straight:
                    double alpha = Utils.GetAngle(Connectors[0].CenterP, Connectors[1].CenterP);
                    laneCenterPoint = Utils.GetPoint(Connectors[0].CenterP, alpha, centerLengthFraction);
                    break;

                case ShapeType.Curve:
                    if (curveAngle > 0)
                        laneCenterPoint = Utils.GetPoint(circleCenter, Connectors[0].CenterP, centerLengthFraction);
                    else
                        laneCenterPoint = Utils.GetPoint(circleCenter, Connectors[0].CenterP, -centerLengthFraction);
                    break;

                case ShapeType.S_Shape:
                    PointF[] fitPoints = GetSPathFitCenterPoints();
                    PointF[] bezierP = GetBezierPoints(fitPoints, LengthFraction, LengthFraction, 1);
                    laneCenterPoint = bezierP[0];
                    break;

            }
    
            double refPointDistance = centerLengthFraction;
            AdjustCenter(ref laneCenterPoint, ref DirectionAngle, ref refPointDistance, 1);
            return laneCenterPoint;
        }

        /// <summary>
        /// Set the UseClassCodeColor flag of all owned objects to the desired value.
        /// </summary>
        /// <param name="Value">Set to false to use the object Color in the Draw method. When set to true, the ClassCodeColor is used instead.</param>
        public void SetColorMode(ColorMode Value)
        {
            this.ColorMode = Value;

            if (LeftLine != null)
                LeftLine.ColorMode = Value;

            if (RightLine != null)
                RightLine.ColorMode = Value;

            foreach (Overlay overlay in Overlays)
                overlay.ColorMode = Value;
        }

        /// <summary>
        /// Checks LeftLine and RightLine objects and all Overlay objects for their Segmentation Class Definitions and increments the UseCounts of these.
        /// </summary>
        public void UpdateUseCounts()
        {
            if (LeftLine != null)
                SegmClassDefs.IncUseCount(LeftLine.SegmClassDef);

            if (RightLine != null)
                SegmClassDefs.IncUseCount(RightLine.SegmClassDef);

            foreach (Overlay ovly in Overlays)
                SegmClassDefs.IncUseCount(ovly.SegmClassDef);
        }

        #region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public virtual void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));
            XmlNode nodeOverlay = nodeItem.AppendChild(Doc.CreateElement("overlay"));
            foreach (Overlay overlay in Overlays)
                overlay.WriteToXml(Doc, nodeOverlay);


        }

        /// <summary>
        /// Reads the contents for one StreetElement class instance from an XML document at the specified node and returns the StreetElement object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <returns>Reference to the StreetElement created from the XML file contents.</returns>
        public virtual void ReadFromXml(XmlDocument Doc, XmlNode nodeItem, AppSettings AppSettings)
        {
            XmlNode nodeObject = nodeItem.SelectSingleNode("overlay");
            XmlNodeList nodeItems = nodeObject.SelectNodes("item");

            foreach (XmlNode node in nodeItems)
            {
                Overlay overlay = Overlay.LoadFromXml(Doc, node, AppSettings);
                overlay.Owner = this;
            }
        }

        #endregion XML File Support


        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the LeftLine object, the border line of this lane on the left side in driving direction.
        /// </summary>
        public LineElement LeftLine
        {
            get { return leftLine;  }
            set
            {
                leftLine = value;
                if (leftLine != null)
                    leftLine.SetOwner(this);
            }
        }

        /// <summary>
        /// Gets or sets the RightLine object, the border line of this lane on the right side in driving direction.
        /// </summary>
        public LineElement RightLine
        {
            get { return rightLine; }
            set
            {
                rightLine = value;
                if (rightLine != null)
                    rightLine.SetOwner(this);
            }
        }

        /// <summary>
        /// Gets or sets the index of the connector in the Connectors array to synchronize the line dashes.
        /// </summary>
        public int DashSyncConnectorIdx
        {
            get { return dashSyncConnectorIdx; }
            set
            {
                if (dashSyncConnectorIdx != value)
                {
                    dashSyncConnectorIdx = value;
                }
            }
        }

        #endregion Public Properties


    }




}

