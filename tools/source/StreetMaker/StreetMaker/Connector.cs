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

namespace StreetMaker
{
    /// <summary>
    /// Class to connect between different shaped lanes or lines. The connector assumes the width of the shapes 
    /// it belongs to and the angle of that line. It can connect to another connecctor with the same width and angle.
    /// </summary>
    public class Connector
    {
        #region Private Fields
        /// <summary>Center point of the connection edge line.</summary>
        private PointF centerP;
        /// <summary>Angle of the connection edge line in radian.</summary>
        private double angle;
        /// <summary>Width of the connector.</summary>
        private double width;
        /// <summary>Reference to a connection object of another shaped owner.</summary>
        private Connector connection;
        /// <summary>Counter to enable event handlers similar to a counting semaphore used by SuspendEvents() and ResumeEvents(). Handlers will only called when this counter is 0. </summary>
        private int suspendCount;
        /// <summary>The catch distance is used to create a marker square around the center point to allow grabbing this point for moving ro sizing.</summary>
        private double catchDistance;
        /// <summary>Phase of dashed line to propagate to next element.</summary>
        private double dashPhase;

        /// <summary>First end point of the connection line.</summary>
        private PointF endP0;
        /// <summary>Secondt end point of the connection line.</summary>
        private PointF endP1;

        /// <summary>First draw point of the connection line a bit outside of endP0 to avoid rounding artefacts.</summary>
        private PointF drawP0;
        /// <summary>Second draw point of the connection line a bit outside of endP1 to avoid rounding artefacts.</summary>
        private PointF drawP1;

        /// <summary>Draw offset in direction of the length of the owning shape object. The value should be just a fraction to avoid rounding artefacts.</summary>
        private double drawOffsL;
        /// <summary>Draw offset to extend the width. The value should be just a fraction to avoid rounding artefacts.</summary>
        private double drawOffsW;


        /// <summary>Polygon of a marker around the center point.</summary>
        private PointF[] markerPolygon;

        /// <summary>Describes this connector as input or output of a lane, or something else. This mode will determine if a connection to another connector is possible. Only Out and In should connect for instance. </summary>
        private ConnectorMode mode;

        /// <summary>Color of the center point marker.</summary>
        private Color color;
        /// <summary>Pen width of the center point marker.</summary>
        private double penWidth;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Reference to the owner object of this connector.</summary>
        public readonly BaseElement Owner;
        /// <summary>Index of this instance in the owner's connector array.</summary>
        public readonly int OwnerIdx;

        /// <summary>
        /// Delegate definition for connector change events.
        /// </summary>
        /// <param name="sender">Reference to the connector that had sent this event.</param>
        public delegate void ChangeEvent(Connector sender);

        /// <summary>
        /// Event handler subscriptions for geometrical changes.
        /// </summary>
        public event ChangeEvent Changed;

        /// <summary>
        /// Event handler subscriptions for connection changes.
        /// </summary>
        public event ChangeEvent Connect;

        #endregion Public Fields

        #region Constructors
        /// <summary>
        /// Creates an instance of the connector class.
        /// </summary>
        /// <param name="Owner">Reference to the owner object of this connector.</param>
        /// <param name="OwnerIdx">Index of this instance in the owner's connector array.</param>
        /// <param name="CenterP">Center point of the connection edge line.</param>
        /// <param name="Angle">Angle of the connection edge line in radian.</param>
        /// <param name="Width">Width of the connector.</param>
        /// <param name="CatchDistance">When a connector object gets closer than this catch distance to another connector object, it will be allowed connect to it.</param>
        public Connector(BaseElement Owner, int OwnerIdx, PointF CenterP, double Angle, double Width, double CatchDistance)
        {
            this.Owner = Owner;
            this.OwnerIdx = OwnerIdx;
            this.centerP = CenterP;
            this.angle = Angle;
            this.width = Width;
            this.catchDistance = CatchDistance;
            this.drawOffsL = 0;
            this.drawOffsW = 0;
            this.dashPhase = 0;
            this.markerPolygon = new PointF[5];
            suspendCount = 0;
            mode = ConnectorMode.Hidden;
            connection = null;
            UpdateEndPoints();
        }

        /// <summary>
        /// Creates an instance of the connector class.
        /// </summary>
        /// <param name="Owner">Reference to the owner object of this connector.</param>
        /// <param name="OwnerIdx">Index of this instance in the owner's connector array.</param>
        /// <param name="Width">Width of the connector.</param>
        public Connector(BaseElement Owner, int OwnerIdx, double Width) : this(Owner, OwnerIdx, new PointF(0, 0), 0, Width, 0) { }
        #endregion Constructors

        #region Protected Methods
        /// <summary>
        /// To be callled on any change in this connector. It will invoke change events, if thesuspendCount is zero.
        /// </summary>
        protected void OnChanged()
        {
            if (suspendCount == 0)
                Changed?.Invoke(this);
        }

        /// <summary>
        /// Updates the end points of the connection edge line after a center point change. It also calls OnChanged().
        /// </summary>
        protected void UpdateEndPoints()
        {
            endP0 = Utils.GetPoint(centerP, angle, -width / 2);
            endP1 = Utils.GetPoint(centerP, angle, +width / 2);
            UpdateMarkerPolygon();
            OnChanged();
        }

        /// <summary>
        /// Updates the draw points a bit outside the end points. If the draw offsets are 0, the draw points will be set to the end points.
        /// </summary>
        protected void UpdateDrawPoints()
        {
            if ((drawOffsL == 0) && (drawOffsW == 0))
            {
                drawP0 = endP0;
                drawP1 = endP1;
            }
            else
            {
                PointF cp = Utils.GetPoint(centerP, angle - Utils.RIGHT_ANGLE_RADIAN, drawOffsL);
                drawP0 = Utils.GetPoint(cp, angle, -width / 2 - drawOffsW);
                drawP1 = Utils.GetPoint(cp, angle, +width / 2 + drawOffsW);
            }
        }


        /// <summary>
        /// Updates the marker polygon points around the center point.
        /// </summary>
        protected void UpdateMarkerPolygon()
        {
            if (catchDistance != 0)
            {
                markerPolygon[0] = Utils.GetPoint(centerP, angle, +catchDistance / 2);
                markerPolygon[1] = Utils.GetPoint(centerP, angle, -catchDistance / 2);
                markerPolygon[2] = Utils.GetPoint(markerPolygon[1], angle + Utils.RIGHT_ANGLE_RADIAN, catchDistance);
                markerPolygon[3] = Utils.GetPoint(markerPolygon[2], angle, +catchDistance);
                markerPolygon[4] = markerPolygon[0];
            }
            UpdateDrawPoints();
        }
        #endregion Protected Methods

        #region Public Methods
        /// <summary>
        /// Method to suspend all events until ResumeEvents() is called. It simply increments the suspendCount, blocking events until decrementd to zero again. This allws nested calls of suspend and resume pairs.
        /// </summary>
        public void SuspendEvents()
        {
            suspendCount++;
        }

        /// <summary>
        /// Methods to resume all events when suspendCount is decremented to zero. It is the counter part to the method SuspendEvents().
        /// </summary>
        public void ResumeEvents()
        {
            if (suspendCount > 0)
                suspendCount--;
        }

        /// <summary>
        /// Updates the connector's angle and center point coordinates.
        /// </summary>
        /// <param name="NewAngle">New angle to update to.</param>
        /// <param name="NewCenterP">New center point coordinates to update to.</param>
        public void UpdateAngleAndCenterP(double NewAngle, PointF NewCenterP)
        {
            angle = Utils.LimitRadian(NewAngle);
            centerP = NewCenterP;
            UpdateEndPoints();
        }

        /// <summary>
        /// Returns true, if the passed point is inside the marker polygon.
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <returns>True, if inside.</returns>
        public bool InMarkerArea(PointF P)
        {
            if (Utils.GetDistance(P,CenterP) < 1e-4)
                    return true;
            return Utils.IsInPolygon(P, MarkerPolygon);
        }

        /// <summary>
        /// Returns true, if the given connector could connect to this connector. Only in and out connectors can connect to eachother or no directional ones with eachother.
        /// </summary>
        /// <param name="Connector">Reference to another connector object to check.</param>
        /// <returns>True, if it can connect.</returns>
        public bool CanConnect(Connector Connector)
        {
            if ((connection == null) && (Connector.connection == null))
                switch (mode)
                {
                    case ConnectorMode.In:
                        return Connector.Mode == ConnectorMode.Out;
                    case ConnectorMode.Out:
                        return Connector.Mode == ConnectorMode.In;
                    case ConnectorMode.NoDir:
                        return Connector.Mode == ConnectorMode.NoDir;
                }
            return false;
        }

        /// <summary>
        /// Set the mode of this connector and with it the color representation of the marker.
        /// </summary>
        /// <param name="AppSettings">Reference to the application setting object to retrieve the colors from.</param>
        /// <param name="Mode">Mode for this connector as on, out or others.</param>
        public void SetMode(AppSettings AppSettings, ConnectorMode Mode)
        {
            mode = Mode;
            penWidth = AppSettings.StreetOutlineLineWidth;
            switch (Mode)
            {
                case ConnectorMode.In:
                    color = AppSettings.ConnectorInColor;
                    break;
                case ConnectorMode.Out:
                    color = AppSettings.ConnectorOutColor;
                    break;
                case ConnectorMode.NoDir:
                    color = AppSettings.ConnectorNoDirColor;
                    break;
                default:
                    color = Color.Transparent;
                    break;
            }
        }

        /// <summary>
        /// Draw the connector's marker around the center circle.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to use for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part to draw. The marker is only drawn in Outline mode.</param>
        public void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            switch (DrawMode)
            {
                case DrawMode.Outline:
                    if ((mode != ConnectorMode.Hidden) && (connection == null))
                        grfx.DrawPolygon(new Pen(color, (float)penWidth), Utils.Scale(markerPolygon, ScaleFactor));
                    break;
            }
        }
        #endregion Public Methods


        #region Public Properties
        /// <summary>
        /// Gets or sets the angle of the connection edge line in radian.
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set
            {
                double newValue = Utils.LimitRadian(value);
                if (angle != newValue)
                {
                    angle = Utils.LimitRadian(newValue);
                    UpdateEndPoints();
                }
            }
        }

        /// <summary>
        /// Gets or stes the width of the connector.
        /// </summary>
        public double Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    UpdateEndPoints();
                }
            }
        }

        /// <summary>
        /// Gets or sets the reference to another connector that links both connectors together.
        /// </summary>
        public Connector Connection
        {
            get { return connection; }
            set
            {
                if (connection != value)
                {
                    if (connection != null)
                        connection.connection = null;

                    connection = value;
                    Connect?.Invoke(this);

                    if (value != null)
                        value.Connection = this;

                }
            }
        }

        /// <summary>
        /// Gets the owner of the connected connector object or null if there is none.
        /// </summary>
        public BaseElement ConnectionOwner
        {
            get
            {
                if (connection == null)
                    return null;

                return connection.Owner;
            }
        }

        /// <summary>
        /// Gets or sets the center point of this connector. Setting this property will automaticlly update EndP0 and EndP1.
        /// </summary>
        public PointF CenterP
        {
            get { return centerP; }
            set
            {
                if ((centerP.X != value.X) || (centerP.Y != value.Y))
                {
                    centerP = value;
                    UpdateEndPoints();
                }
            }
        }

        /// <summary>
        /// Gets or sets the first end point of this connector. Setting this property will automaticlly update CenterP and EndP1.
        /// </summary>
        public PointF EndP0
        {
            get { return endP0; }
            set
            {
                if ((endP0.X != value.X) || (endP0.Y != value.Y))
                {
                    endP0 = value;
                    centerP = Utils.GetPoint(endP0, angle, +width / 2);
                    endP1 = Utils.GetPoint(endP0, angle, +width);
                    UpdateMarkerPolygon();
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the first second point of this connector. Setting this property will automaticlly update CenterP and EndP0.
        /// </summary>
        public PointF EndP1
        {
            get { return endP1; }
            set
            {
                if ((endP1.X != value.X) || (endP1.Y != value.Y))
                {
                    endP1 = value;
                    centerP = Utils.GetPoint(endP1, angle, -width / 2);
                    endP0 = Utils.GetPoint(endP1, angle, -width);
                    UpdateMarkerPolygon();
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the catch distance. The catch distance is used to create a marker square around the center point to allow grabbing this point for moving ro sizing. 
        /// </summary>
        public double CatchDistance
        {
            get { return catchDistance; }
            set
            {
                if (catchDistance != value)
                {
                    catchDistance = value;
                    UpdateMarkerPolygon();
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets the reference to the marker polygon.
        /// </summary>
        public PointF[] MarkerPolygon
        {
            get { return markerPolygon; }
        }

        /// <summary>
        ///  Gets the mode of this connector.
        /// </summary>
        public ConnectorMode Mode
        {
            get { return mode; }
        }

        /// <summary>
        /// Get or set phase of dashed line to propagate to next element.
        /// </summary>
        public double DashPhase
        {
            get { return dashPhase; }
            set 
            { 
                dashPhase = value;
                //if (connection != null)
                //    connection.OnChanged();
            }
        }

        /// <summary>
        /// Gets the first draw point of the connection line a bit outside of EndP0 to avoid rounding artefacts.
        /// </summary>
        public PointF DrawP0
        {
            get { return drawP0; }
        }

        /// <summary>
        /// Gets the second draw point of the connection line a bit outside of EndP1 to avoid rounding artefacts.
        /// </summary>
        public PointF DrawP1
        {
            get { return drawP1; }
        }

        /// <summary>
        /// Gets the draw width of this connector, which might be equal to the Width or a fraction larger.
        /// </summary>
        public double DrawWidth
        {
            get { return width + 2 * drawOffsW; }
        }

        /// <summary>
        /// Gets or sets the draw offset in direction of the length of the owning shape object. The value should be just a fraction to avoid rounding artefacts.
        /// </summary>
        public double DrawOffsL
        {
            get { return drawOffsL; }
            set
            {
                if (drawOffsL != value)
                {
                    drawOffsL = value;
                    UpdateMarkerPolygon();
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the draw offset to extend the width. The value should be just a fraction to avoid rounding artefacts.
        /// </summary>
        public double DrawOffsW
        {
            get { return drawOffsW; }
            set
            {
                if (drawOffsW != value)
                {
                    drawOffsW = value;
                    UpdateMarkerPolygon();
                    OnChanged();
                }
            }
        }

        #endregion Public Properties

    }

}
