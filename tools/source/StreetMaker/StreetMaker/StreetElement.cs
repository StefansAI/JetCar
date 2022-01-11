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
using System.Xml;
using System.Diagnostics;

namespace StreetMaker
{
    /// <summary>
    /// The StreetElement is a base class that handles a number of lanes and adds editing capabilities. 
    /// </summary>
    public class StreetElement
    {
        #region Private and Protected Fields
        /// <summary>Minimum length of the element. Resizing a straight shape will cut off at this length.</summary>
        protected double MinLength;
        /// <summary>Array of lanes fo this street element.</summary>
        protected LaneElement[] lanes;
        /// <summary>Reference to the currently active connector when editing (moving or resizing). It can be null, when no lane is selected/active. </summary>
        protected Connector activeConnector;
        /// <summary>List of StreetElements that are connected together in this group.</summary>
        protected List<StreetElement> streetGroup;
        #endregion Private and Protected Fields

        #region Public Fields
        /// <summary>Order number assigned to all StreetElements to synchronize the dashed lines according to the order. 
        /// Streets with higher order number synchronize to the connected one with a lower order number. </summary>
        public int DashSyncOrder;
        /// <summary>If true, overlays and intersection items are drawn in wrong direction.</summary>
        public bool DrawWrongDirItems = false;
        /// <summary>Point of the camera to be used together with MaxDetailDist to check for marking class details in Draw method.</summary>
        public PointF CameraPoint;
        /// <summary>Maximum distance from CameraPoint for marking class details in Draw method.</summary>
        public double MaxDetailDist;
        #endregion Public Fields

        #region Constructors
        /// <summary>
        /// Creates an instance of the StreetElement class.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        public StreetElement(AppSettings AppSettings)
        {
            MinLength = AppSettings.DefaultStraightLength;
            MaxDetailDist = AppSettings.MarkMaxDetailDistance;
            streetGroup = new List<StreetElement>();
            streetGroup.Add(this);
            DashSyncOrder = -1;
        }
        #endregion Constructors

        #region Private and Protected Methods
        /// <summary>
        /// Moves all lanes of this street element including their overlays by the given delta.
        /// </summary>
        /// <param name="Delta">X- and Y- delta to move all lanes.</param>
        protected virtual void MoveStreet(SizeF Delta)
        {
            foreach (LaneElement lane in Lanes)
                lane.Connectors[0].CenterP += Delta;
        }

        /// <summary>
        /// Adds the given StreetElement to the list after checking that it is not in the list yet. Then it checks all connected StreetElements and calls itself recursively adding not yet listed streets.
        /// </summary>
        /// <param name="Street">StreetElemeent to add and to checkconnected streets.</param>
        protected void AddStreetToGroup(StreetElement Street)
        {
            if (streetGroup.Contains(Street) == true)
                return;
                
            streetGroup.Add(Street);

            foreach (LaneElement lane in Street.Lanes)
                foreach (Connector con in lane.Connectors)
                {
                    if ((con.ConnectionOwner != null) && (con.ConnectionOwner is LaneElement) && ((con.ConnectionOwner as LaneElement).Owner != null))
                    {
                        StreetElement se = (con.ConnectionOwner as LaneElement).Owner as StreetElement;
                        if ((se != null) && (se != Street))
                            AddStreetToGroup(se);
                    }
                }

        }

        /// <summary>
        /// Trace all connected StreetElements and add them to the StreetGroup list, which allows moving the complete group together.
        /// </summary>
        protected void BuildStreetGroup()
        {
            streetGroup.Clear();
            AddStreetToGroup(this);
        }

        /// <summary>
        /// Allows aborting the Draw method in case of drawing classes but all lanes are marked as Nothing class.
        /// </summary>
        /// <returns>Ture if drawing can be aborted or false if drawing needs to proceed.</returns>
        protected bool DrawAbort()
        {
            if (lanes[0].ColorMode == ColorMode.ImageColor)
                return false;
            SegmClassDef scdNothing = SegmClassDefs.GetSegmClassDef(null);
            foreach (LaneElement lane in lanes)
                if (lane.SegmClassDef != scdNothing)
                    return false;
            return true;
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Draw method of this class. It calls the draw methods of all owned lane objects passing the parameters.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public virtual void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            if (lanes[0].ColorMode != ColorMode.ImageColor)
            {
                bool draw = false;
                SegmClassDef scdNothing = SegmClassDefs.GetSegmClassDef(null);
                foreach (LaneElement lane in lanes)
                    if (lane.SegmClassDef != scdNothing)
                    {
                        draw = true;
                        break;
                    }
                if (draw == false)
                    return;
            }
            foreach (LaneElement lane in lanes)
                lane.Draw(grfx, ScaleFactor, DrawMode);
        }

        /// <summary>
        /// Draw method of this class. It calls the draw methods of all owned lane objects to draw the base layer first and then the top layer.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        public virtual void Draw(Graphics grfx, SizeF ScaleFactor)
        {
            Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
            Draw(grfx, ScaleFactor, DrawMode.TopLayer);
            Draw(grfx, ScaleFactor, DrawMode.Overlay);
        }

        /// <summary>
        /// Draw the street group. It calls the draw methods of all connected StreetElements passing the parameters.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public void DrawGroup(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            foreach (StreetElement se in streetGroup)
                se.Draw(grfx, ScaleFactor, DrawMode);
        }

        /// <summary>
        /// Draw the street group. It calls the draw methods of all connected StreetElements passing the parameters.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        public void DrawGroup(Graphics grfx, SizeF ScaleFactor)
        {
            DrawGroup(grfx, ScaleFactor, DrawMode.BaseLayer);
            DrawGroup(grfx, ScaleFactor, DrawMode.TopLayer);
            DrawGroup(grfx, ScaleFactor, DrawMode.Overlay);
        }

        /// <summary>
        /// Finds and returns the reference to the closest connector of this street element to the provided connector, if the distance is less than the minimum distance. 
        /// In that case, MinDistance and ClosestConnector will be updated.
        /// </summary>
        /// <param name="RefConnector">Reference connector to chack the distance to. </param>
        /// <param name="MinDistance">Minimum distance threshold to update the ClosestConnector reference. It will be updated with the distance of the found connector.</param>
        /// <param name="ClosestConnector">This reference will be only updated if a connector had been found, whose distance from RefConnector is less than MinDistance.</param>
        public void FindClosestPossibleConnector(Connector RefConnector, ref double MinDistance, ref Connector ClosestConnector)
        {
            foreach (LaneElement lane in lanes)
                foreach (Connector connector in lane.Connectors)
                    if ((RefConnector.Owner != connector.Owner) && ((RefConnector.Owner as LaneElement).Owner != this) && (RefConnector.CanConnect(connector) == true))
                    {
                        double dist = Utils.GetDistance(RefConnector.CenterP, connector.CenterP);
                        if (dist < MinDistance)
                        {
                            MinDistance = dist;
                            ClosestConnector = connector;
                        }
                    }
        }

        /// <summary>
        /// Returns the reference to the connecto that is closes to the point.
        /// </summary>
        /// <param name="P">Point to check the distance to.</param>
        /// <returns>Reference to the closest connector to the point.</returns>
        public Connector GetClosestConnector(PointF P)
        {
            Connector closest = null;
            double minDist = double.MaxValue;
            foreach (LaneElement lane in lanes)
                foreach (Connector connector in lane.Connectors)
                {
                    double dist = Utils.GetDistance(P, connector.CenterP);
                    if (dist<minDist)
                    {
                        minDist = dist;
                        closest = connector;
                    }
                }
            return closest;
        }

        /// <summary>
        /// Get the reference to the connector, that is selected via the given point. This method checks all connectors to find the one, 
        /// where the point is inside the center point marker. If none matches, null is returned.
        /// </summary>
        /// <param name="P">Point to check against.</param>
        /// <returns>Reference to the connector where the point is inside the center point marker or null if none is found.</returns>
        public Connector GetSelectedConnector(PointF P)
        {
            foreach (LaneElement lane in lanes)
                foreach (Connector connector in lane.Connectors)
                    if (connector.InMarkerArea(P))
                        return connector;
            return null;        
        }


        /// <summary>
        /// Find the connector that can be selected via the given coordinates via GetSelectedConnector and assign the reference to ActiveConnector.
        /// </summary>
        /// <param name="P">Point to check against.</param>
        /// <returns>True, if a connector had been found and could be assigned.</returns>
        public bool SetActiveConnector(PointF P)
        {
            activeConnector = GetSelectedConnector(P);
            return activeConnector != null;
        }

        /// <summary>
        /// Move this object to the requested location by calling Move() with the delta between ActiveConnector.CenterP and the given point coordinates.
        /// No move will be performed if ActiveConnector is not assigned.
        /// </summary>
        /// <param name="P">Location coordinates to move the ActiveConnector.CenterP to.</param>
        public void MoveToLocation(PointF P)
        {
            if (activeConnector != null)
                Move(Utils.GetSize(P, activeConnector.CenterP));
         }

        /// <summary>
        /// Moves this object with all lanes by the given delta.
        /// </summary>
        /// <param name="Delta">Delta in x and y to move this object.</param>
        public virtual void Move(SizeF Delta)
        {
            foreach (StreetElement se in streetGroup)
                se.MoveStreet(Delta);
        }

        /// <summary>
        /// Update the StreetGroup list of all connected StreetElements for this object and then build the StreetGroups of all the StreetElements in this group. so all of them are up-to-date.
        /// </summary>
        public void UpdateStreetGroup()
        {
            BuildStreetGroup();
            foreach (StreetElement se in streetGroup)
            {
                if (se != this)
                    se.BuildStreetGroup();
            }
        }

        /// <summary>
        /// Checks all lanes belonging to this object, if the point coordinates are inside one of the lanes or 
        /// one of its possible overlays and returns the reference to the found object or null.
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <param name="IncludeOverlays">If true, overlays will be checked first and the reference to a found overlay will be returned.</param>
        /// <returns>Reference to the object found, if inside or null if not.</returns>
        public virtual object IsInside(PointF P, bool IncludeOverlays)
        {
            foreach (LaneElement lane in lanes)
            {
                object obj = lane.IsInside(P, IncludeOverlays);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        /// <summary>
        /// Returns true, if the object is not connected to anything else and thus can be rotated.
        /// </summary>
        /// <returns>True, if rotation is possible.</returns>
        public virtual bool CanRotate()
        {
            foreach (LaneElement lane in lanes)
                if ((lane.Connectors[0].Connection != null) || (lane.Connectors[1].Connection != null))
                    return false;
            return true;

        }

        /// <summary>
        /// Perform the rotation of this object by the amount requested.
        /// </summary>
        /// <param name="Degrees">Angle in Degree to rotate.</param>
        public virtual void Rotate(double Degrees)
        {
            if (activeConnector != null)
            {
                double dAngle = Utils.ToRadian(Degrees);

                for (int i = 0; i < lanes.Length; i++)
                {
                    double dist = Utils.GetDistance(activeConnector.CenterP, lanes[i].Connectors[0].CenterP); 
                    if (dist > 0)
                    {
                        double angle = Utils.GetAngle(activeConnector.CenterP, lanes[i].Connectors[0].CenterP); 
                        PointF p = Utils.GetPoint(activeConnector.CenterP, angle + dAngle, dist);
                        lanes[i].Connectors[0].UpdateAngleAndCenterP(lanes[i].Connectors[0].Angle + dAngle, p);
                    }
                    else 
                        lanes[i].Connectors[0].Angle = Utils.LimitRadian(lanes[i].Connectors[0].Angle + dAngle);
                }
            }
        }


        /// <summary>
        /// Increase the size of this multilane object by adding one step size to its length or its curve angle.
        /// To be overwritten in inherited classes.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="DrawingSize">Size of the drawing area to limit the maximum length.</param>
        public virtual void SizeIncreaseStep(AppSettings AppSettings, SizeF DrawingSize)
        {
        }

        /// <summary>
        /// Decrease the size of this multilane object by subtracting one step size from its length or its curve angle.
        /// To be overwritten in inherited classes.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        public virtual void SizeDecreaseStep(AppSettings AppSettings)
        {
        }


        /// <summary>
        /// Sizes the multi lane street, so the ActiveConnector.CenterP selected before will be moved as close as possible to the TargetPoint. In base mode, only one parameter will be changed (length or curve angle)
        /// in extended mode, two parameters will be adjusted at once when possible to get directly to the TargetPoint.
        /// To be overwritten in inherited classes.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="DrawingSize">Size of the drawing area to limit the maximum length.</param>
        /// <param name="TargetPoint">New target point for the ActiveConnector.CenterP</param>
        /// <param name="ExtMode">When false, only one parameter (length or curve angle) will be adjusted to get close to the target point. When true, length and s-offset or curve angle and radius will be adjusted. </param>
        public virtual void Size(AppSettings AppSettings, SizeF DrawingSize, PointF TargetPoint, bool ExtMode)
        {
        }

        /// <summary>
        /// Assign angle and location via one method.
        /// </summary>
        /// <param name="Angle">Angle to set Lanes[0].Connectors[0].Angle to.</param>
        /// <param name="Location">Location to move Lanes[0].Connectors[0].CenterP to.</param>
        public void SetAngleAndLocation(double Angle, PointF Location)
        {
            Rotate(Angle - Utils.ToDegree(Lanes[0].Connectors[0].Angle));
            Move(new SizeF(Location.X - Lanes[0].Connectors[0].CenterP.X, Location.Y - Lanes[0].Connectors[0].CenterP.Y));
        }

        /// <summary>
        /// Set the UseClassCodeColor flag of all elements to the desired value.
        /// </summary>
        /// <param name="Value">Set to false to use the object Color in the Draw method. When set to true, the ClassCodeColor is used instead.</param>
        public virtual void SetColorMode(ColorMode Value)
        {
            foreach (LaneElement lane in lanes)
                lane.SetColorMode(Value);
        }

        /// <summary>
        /// Goes through all lanes and calls their UpdateUseCounts method to increment the UseCounts for the SegmClassDef objects used.
        /// </summary>
        public virtual void UpdateUseCounts()
        {
            foreach (LaneElement lane in lanes)
                lane.UpdateUseCounts();
        }


        #region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public virtual void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("lane"));
            foreach (LaneElement le in Lanes)
                le.WriteToXml(Doc, nodeItem);

        }

        /// <summary>
        /// Reads the contents for one StreetElement class instance from an XML document at the specified node and returns the StreetElement object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <returns>Reference to the StreetElement created from the XML file contents.</returns>
        public virtual void ReadFromXml(XmlDocument Doc, XmlNode nodeItem, AppSettings AppSettings)
        {
            XmlNode nodeLane = nodeItem.SelectSingleNode("lane");
            if (nodeLane == null) return;

            XmlNodeList nodeItems = nodeLane.SelectNodes("item");
            for (int i = 0; i < Math.Min(nodeItems.Count, Lanes.Length); i++)
                lanes[i].ReadFromXml(Doc, nodeItems[i], AppSettings);
        }

        #endregion XML File Support

        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the array of lane objects.
        /// </summary>
        public LaneElement[] Lanes
        {
            get { return lanes; }
        }

        /// <summary>
        /// Gets the reference to the ActiveConnector if assigned or null if not.
        /// </summary>
        public Connector ActiveConnector
        {
            get { return activeConnector;  }
        }

        /// <summary>
        /// List of StreetElement objects to add connected streets.
        /// </summary>
        public List<StreetElement> StreetGroup
        {
            get { return streetGroup;  }
        }

        #endregion Public Properties
    }

}



