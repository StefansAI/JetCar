// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

// Activate to vizualise the assigned dash sync order number of each street element and marks connector[0] and the connector synchronized.
//#define DEBUG_PAINT_DASH_SYNC_ORDER

// Activate to generate class color images of the street map for each generated camera view to be saved to the file system.
//#define DEBUG_CLASS_STREET_IMAGES

// Activate experimental erode and dilate functions in the camera view generation. Goal of this experimental implementation was the removal of smaller fractions of the masks far away. 
// However, the computation time increase outweighted the benefits of the artefact reduction
//#define ERODE_AND_DILATE_MASK
//#define DEBUG_MASK_IMAGES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using System.Windows.Forms;
using System.Xml;
using System.IO;


namespace StreetMaker
{
    /// <summary>
    /// Class to hold all elements of a street map and manage drawing and APIs for the GUI and XML file handling.
    /// </summary>
    public class StreetMap
    {
        #region Public Constants
        /// <summary>Identifer string that is stored into the XML file.</summary>
        public const string XML_FILE_ID_STR = "StreetMaker StreetMap";
        #endregion Public Constants

        #region Private Fields
        /// <summary>Reference to the active street element of this object, which was previously selected by the GUI.</summary>
        private StreetElement activeElement;
        /// <summary>Reference to the active overlay of this object, which was previously selected by the GUI.</summary>
        private Overlay activeOverlay;
        /// <summary>List of possible connections between the ActiveElement and any other element in the map.</summary>
        private List<ConnectorLine> PossibleConnections;
        /// <summary>Index of the activated connection issue in the issue list.</summary>
        private int ConnectionIssueIndex = -1;
        /// <summary>Reference to the application settings object to get parameter from.</summary>
        private AppSettings AppSettings;
        /// <summary>Size of the drawing area in mm.</summary>
        private SizeF drawingSize;
        #endregion Private Fields

        #region Public Fields
        /// <summary>If true, the index numbers of the street elements are drawn into the street bitmap.</summary>
        public bool DrawItemNumbers = false;
        /// <summary>If true, the numbers of the lanes of each street element are drawn into the street bitmap.</summary>
        public bool DrawLaneNumbers = false;

        /// <summary>List of all StreetElements belonging to this map.</summary>
        public readonly List<StreetElement> Items;
        /// <summary>List of all ConnectionIssues discovered at connect attemps.</summary>
        public readonly List<ConnectionIssue> ConnectionIssues;
        #endregion Public Fields

        #region Public Events
        /// <summary>Delegate definition for general events without any arguments. </summary>
        public delegate void GeneralEvent();
        /// <summary>Delegate definition for text update events. </summary>
        public delegate void TextEvent(string Text);
        /// <summary>Delegate definition for bitmaps update events. </summary>
        public delegate void NewBitmapsEvent(Bitmap ViewBitmap, Bitmap ClassBitmap);

        /// <summary>Event definition for connection issue changes.</summary>
        public event GeneralEvent ConnectionIssueChange;
        /// <summary>Event definition for a request to redraw the GUI bitmap.</summary>
        public event GeneralEvent RedrawRequest;
        /// <summary>Event definition for a request to invalidate the GUI bitmap.</summary>
        public event GeneralEvent InvalidateRequest;
        /// <summary>Event definition for displaying the file name text in the GUI.</summary>
        public event TextEvent DisplayFileName;
        /// <summary>Event definition for displaying new bitmaps in the GUI.</summary>
        public event NewBitmapsEvent NewBitmapsUpdate;
        #endregion Public Events

        #region Constructor
        /// <summary>
        /// Creates an instance of the StreetMap class.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        public StreetMap(ref AppSettings AppSettings)
        {
            this.AppSettings = AppSettings;
            drawingSize = new SizeF((float)AppSettings.DefaultDrawingWidth, (float)AppSettings.DefaultDrawingHeight);

            Items = new List<StreetElement>();
            PossibleConnections = new List<ConnectorLine>();
            ConnectionIssues = new List<ConnectionIssue>();
        }
        #endregion Constructor

        #region Public Methods
        #region Drawing related
        /// <summary>
        /// Initializes a new map with the given draw size.
        /// </summary>
        /// <param name="NewDrawingSize">New x- and y-size of the StreetMap in millimeter.</param>
        public void Init(SizeF NewDrawingSize)
        {
            Clear();
            drawingSize = NewDrawingSize;
        }

        /// <summary>
        /// Determines the minimum size for the current map that can be applied without cutting off parts.
        /// </summary>
        /// <returns>Minimum size of the current map</returns>
        public SizeF GetMapMinimumSize()
        {
            float xmax = 2*(float)AppSettings.LaneWidth;
            float ymax = 2*(float)AppSettings.LaneWidth;
            foreach (StreetElement se in Items)
                foreach (LaneElement lane in se.Lanes)
                    foreach (Connector con in lane.Connectors)
                    {
                        xmax = Math.Max(xmax, con.EndP0.X);
                        ymax = Math.Max(ymax, con.EndP0.Y);
                        xmax = Math.Max(xmax, con.EndP1.X);
                        ymax = Math.Max(ymax, con.EndP1.Y);
                    }
            return new SizeF((float)Math.Round(xmax + AppSettings.LaneWidth/2), (float)Math.Round(ymax + AppSettings.LaneWidth/2));
        }

        /// <summary>
        /// Apply new size to the current map. Any new size smaller than the current occupied space will be restricted to that size.
        /// </summary>
        /// <param name="NewDrawingSize">New size to set to.</param>
        public void ResizeMap(SizeF NewDrawingSize)
        {
            SizeF minSize = GetMapMinimumSize();
            drawingSize = new SizeF((float)Math.Round(Math.Max(minSize.Width, NewDrawingSize.Width)), (float)Math.Round( Math.Max(minSize.Height, NewDrawingSize.Height)));
        }


        /// <summary>
        /// Add the new street element to the items list of the map.
        /// </summary>
        /// <param name="NewStreetElement">Reference to the new StreetMap to be added.</param>
        public void Add(StreetElement NewStreetElement)
        {
            Items.Add(NewStreetElement);
        }

        /// <summary>
        /// Clear the item list, the issue list and reset active references.
        /// </summary>
        public void Clear()
        {
            Items.Clear();
            ConnectionIssues.Clear();
            ActiveElement = null;
            ActiveOverlay = null;
        }

        /// <summary>
        /// Delete the currently ActiveElement or ActiveOverlay. An ActiveElement is disconnected first and then removed from the Items list.
        /// </summary>
        public void DeleteActiveElement()
        {
            if (ActiveElement != null)
            {
                DisconnectActiveElement();
                Items.Remove(ActiveElement);
                ActiveElement = null;
                RedrawRequest?.Invoke();
            }
            else if (ActiveOverlay != null)
            {
                ActiveOverlay.Owner = null;
                ActiveOverlay = null;
                RedrawRequest?.Invoke();
            }
        }

        /// <summary>
        /// Replace a given existing StreetElement in the Items list with a new one at the same place in the list. This method is used to exchange an element after editing.
        /// The CurrentStreetElement is disconnected first, then replaced and reconnected at the end. Overlays will be removed from and added to the new one where possible. 
        /// </summary>
        /// <param name="CurrentStreetElement">Reference to the current StreetElement that needs to be replaced in the Items list.</param>
        /// <param name="NewStreetElement">Reference to the new StreetElement that will have to take the place in the Items list.</param>
        public void ReplaceStreetElement(StreetElement CurrentStreetElement, StreetElement NewStreetElement)
        {
            DisconnectElement(CurrentStreetElement);
            int idx = Items.IndexOf(CurrentStreetElement);
            if ((idx >= 0) && (idx < Items.Count))
                Items[idx] = NewStreetElement;
            else
                Items.Add(NewStreetElement);

            for (int i = 0; i < Math.Min(CurrentStreetElement.Lanes.Length, NewStreetElement.Lanes.Length); i++)
            {
                NewStreetElement.Lanes[i].Overlays.Clear();
                for (int j=0; j< CurrentStreetElement.Lanes[i].Overlays.Count; j++)
                {
                    CurrentStreetElement.Lanes[i].Overlays[j].Owner = NewStreetElement.Lanes[i];
                }

            }
            
            ActiveElement = NewStreetElement;
            ReconnectAll();
            RedrawRequest?.Invoke();
        }

        /// <summary>
        /// Checks all connectors of the ActiveElement against all other connectors in the map and creates a new list of ConnectorLine objects in PossibleConnections. 
        /// The goal is to find only the closest and compatible connector lines to be displayed while moving the ActiveElement. It helps to snap it into a correct position.
        /// </summary>
        public void CheckConnectionDistance()
        {
            if (ActiveElement != null)
            {
                PossibleConnections.Clear();
                foreach (LaneElement lane in ActiveElement.Lanes)
                    foreach (Connector connector in lane.Connectors)
                    {
                        double minDistance = AppSettings.ConnectionDrawDistance;
                        Connector minDistConnector = null;

                        foreach (StreetElement se in Items)
                            se.FindClosestPossibleConnector(connector, ref minDistance, ref minDistConnector);

                        if (minDistConnector != null)
                            PossibleConnections.Add(new ConnectorLine(connector, minDistConnector, minDistance, minDistance < AppSettings.ConnectionSnapDistance ? AppSettings.ConnectionSnapColor : AppSettings.ConnectionDrawColor));
                    }
            }
        }

        /// <summary>
        /// Checks if the ActiveElement can be rotated to fit the closest possible connector angle. If it can be rotated, this method will automatically rotate it to fit that angle.
        /// </summary>
        public void CheckConnectionRotation()
        {
            if (ActiveElement != null) 
            {
                ConnectorLine closest = null;
                foreach (ConnectorLine cl in PossibleConnections)
                {
                    if (cl.Distance <= AppSettings.ConnectionSnapDistance)
                    {
                        if (closest == null)
                            closest = cl;
                        else if (cl.Distance < closest.Distance)
                            closest = cl;
                    }
                }

                if (closest != null)
                {
                    double angle = Utils.ToDegree(closest.StreetConnector.Angle - closest.ActiveConnector.Angle);
                    if (closest.StreetConnector.OwnerIdx != closest.ActiveConnector.OwnerIdx)
                        ActiveElement.Rotate(angle);
                    else
                        ActiveElement.Rotate(180 + angle);

                }
            }
        }

        /// <summary>
        /// Checks, if the ActiveELement is close enough to any other street element in the map that it can be snaped and connected to that.
        /// It checks the closest possible connections, then moves or sizes the ActiveElement into the best possition and tries to connect all lanes
        /// </summary>
        /// <param name="MoveMode">If true, the ActiveElement will be just moved and not sized.</param>
        /// <param name="SizeMode">If true, the ActiveElement will be sized, meaning one side is fixed in place while the dragged one will be snaped.</param>
        /// <param name="ExtSizeMode">If true, 2 parameters might be adjusted (radius and curve angle ro length and s-offset) to fit it to the other element. If false, only one parameter (curve angle or length) will be adjusted. </param>
        public void CheckConnectionSnap(bool MoveMode, bool SizeMode, bool ExtSizeMode)
        {
            if ((ActiveElement != null) && (ActiveElement.ActiveConnector != null))
            {
                ConnectorLine closest = null;
                foreach (ConnectorLine cl in PossibleConnections)
                {
                    if (cl.Distance <= AppSettings.ConnectionSnapDistance)
                    {
                        if (MoveMode || (SizeMode && (ActiveElement.ActiveConnector == cl.ActiveConnector)))
                        {
                            if (closest == null)
                                closest = cl;
                            else if (cl.Distance < closest.Distance)
                                closest = cl;
                        }
                    }
                }

                if (closest != null)
                {
                    if (MoveMode)
                        ActiveElement.Move(Utils.GetSize(closest.StreetConnector.CenterP, closest.ActiveConnector.CenterP));
                    else if (SizeMode)
                    {
                        int n = 0;
                        double dist = double.MaxValue;
                        do
                        {
                            ActiveElement.Size(AppSettings, DrawingSize, closest.StreetConnector.CenterP, ExtSizeMode); // PressedKey == AppSettings.HotkeySizeModeExt);
                            dist = Utils.GetDistance(closest.StreetConnector.CenterP, ActiveElement.ActiveConnector.CenterP);
                        } while ((dist > AppSettings.MAX_CONN_DIST_TOL / 2) && (++n < 1000));
                    }

                    foreach (ConnectorLine cl in PossibleConnections)
                    {
                        if (Utils.GetDistance(cl.ActiveConnector.CenterP, cl.StreetConnector.CenterP) <= Math.Abs(cl.ActiveConnector.CatchDistance))
                            cl.ActiveConnector.Connection = cl.StreetConnector;
                    }
                    PossibleConnections.Clear();
                    ActiveElement.UpdateStreetGroup();
                    CheckConnectionIssues();
                    AssignDashSyncOrders();
                }
            }
        }

        /// <summary>
        /// Sets the ConnectionIssueIndex to the given value and triggers the InvalidateRequest to display the now active Connection Issue.
        /// </summary>
        /// <param name="Index">Index of the ConnectionIssue todisplay as active.</param>
        public void SetActiveConnectionIssue(int Index)
        {
            ConnectionIssueIndex = Index;
            InvalidateRequest?.Invoke();
        }

        /// <summary>
        /// Checks, if the angles of two connectors are close enough for a good connection and returns true if they are in tolerance.
        /// Phase shifts of +-180 degrees are taken into account.
        /// </summary>
        /// <param name="C0"></param>
        /// <param name="C1"></param>
        /// <returns></returns>
        private bool ConnectorAngleInTol(Connector C0, Connector C1)
        {
            double delta = C1.Angle - C0.Angle;
            if (delta > 1.5*Math.PI)
                delta -= 2*Math.PI;
            else if (delta > Math.PI / 2)
                delta -= Math.PI;
            else if (delta < -1.5*Math.PI)
                delta += 2*Math.PI;
            else if (delta < -Math.PI / 2)
                delta += Math.PI;
            return (Math.Abs(delta) < AppSettings.MAX_CONN_ANGLE_TOL);
        }

        /// <summary>
        /// Goes through all StreetElements in the Items list, checking all close proximity connector pairs and tries to connect all of them if possible.
        /// In the end the method CheckConnectionIssues is called to update the issue list.
        /// </summary>
        public void ReconnectAll()
        {
            foreach (StreetElement se in Items)
                foreach (LaneElement le in se.Lanes)
                    foreach (Connector con in le.Connectors)
                        if (con.Connection == null)
                        {
                            foreach (StreetElement sx in Items)
                                if (se != sx)
                                {
                                    foreach (LaneElement lx in sx.Lanes)
                                        foreach (Connector cx in lx.Connectors)
                                            if (con.CanConnect(cx) &&
                                                (Utils.GetDistance(con.CenterP, cx.CenterP) < AppSettings.MAX_CONN_DIST_TOL) && ConnectorAngleInTol(con, cx))
                                                con.Connection = cx;
                                }
                        }
            CheckConnectionIssues();
        }

        /// <summary>
        /// Add a ConnectionIssue to the ConnectionIssues list that occured trying to connect the 2 given connectors.
        /// </summary>
        /// <param name="C0">Reference to the first Connector object.</param>
        /// <param name="C1">Reference to the second Connector object.</param>
        private void AddIssue(Connector C0, Connector C1)
        {
            foreach (ConnectionIssue ci in ConnectionIssues)
                if (((ci.C0 == C0) && (ci.C1 == C1)) || ((ci.C0 == C1) && (ci.C1 == C0)))
                    return;

            ConnectionIssues.Add(new ConnectionIssue(C0, C1));
        }

        /// <summary>
        /// Goes through all StreetElements in the Items list, checking all connected connector pairs and those that could be connected.  
        /// Any pair that doesn't fit well enough will be added to the ConnectionIssue list via AddIssue, so it can be displayed by the GUI.
        /// The ConnectionIssues list is cleared first, so the list is correctly representing the current connection state.
        /// </summary>
        public void CheckConnectionIssues()
        {
            ConnectionIssues.Clear();
            SetActiveConnectionIssue(-1);

            foreach (StreetElement se in Items)
                foreach (LaneElement le in se.Lanes)
                    foreach (Connector con in le.Connectors)
                        if (con.Connection == null)
                        {
                            foreach (StreetElement sx in Items)
                                if (se != sx)
                                {
                                    foreach (LaneElement lx in sx.Lanes)
                                        foreach (Connector cx in lx.Connectors)
                                            if (con.CanConnect(cx) && (Utils.GetDistance(con.CenterP, cx.CenterP) < AppSettings.ConnectionSnapDistance))
                                                AddIssue(con, cx);
                                }
                        }
                        else
                        {
                            if ((Utils.GetDistance(con.CenterP, con.Connection.CenterP) >= AppSettings.MAX_CONN_DIST_TOL) || (!ConnectorAngleInTol(con, con.Connection)))
                                AddIssue(con, con.Connection);
                        }


            foreach (StreetElement se in Items)
                se.UpdateStreetGroup();

            ConnectionIssueChange?.Invoke();
        }

        /// <summary>
        /// Select the object at the given location and make it the active one. The method goes through all StreetElements in the Items list to query their IsInside methods.
        /// Each of the elements will check all their lanes and also their overlays if requested. If a lane is found, the reference to owning StreetElement is assigned to ActiveElement.
        /// If IncludeOverlays is true, the returned object might also be an overlay. In this case, ActiveOverlay is set instead of ActiveElement. 
        /// If nothing is found, ActiveElement and ActiveOverlay are set to null.
        /// </summary>
        /// <param name="P">Coordinates to check.</param>
        /// <param name="IncludeOverlays">If true, overlays will be checked first and the reference to a found overlay will be returned.</param>
        /// <returns>Reference to the object found, if inside or null if not.</returns>
        public object SelectObject(PointF P, bool IncludeOverlays)
        {
            foreach (StreetElement se in Items)
            {
                Object obj = se.IsInside(P, IncludeOverlays);
                if (obj != null)
                {
                    activeOverlay = obj as Overlay;                   
                    activeElement = activeOverlay == null ? se : null;
                    return obj;
                }
            }

            activeElement = null;
            activeOverlay = null;
            return null;
        }

        /// <summary>
        /// Search through all StreetElements in the Items list and find the LaneElement at the given location.
        /// This method returns the LaneElement, even if the location is inside an overlay of that lane.
        /// </summary>
        /// <param name="P">Coordinates to check.</param>
        /// <returns>LaneElement found at the location or null, if nothing is found.</returns>
        public LaneElement GetLaneElement(PointF P)
        {
            foreach (StreetElement se in Items)
            {
                Object obj = se.IsInside(P, false);
                if (obj is LaneElement)
                    return (obj as LaneElement);
            }

            return null;
        }

        /// <summary>
        /// Disconnects all Connectors of the ActiveElement, so it can be moved, sized, replaced or deleted.
        /// </summary>
        public void DisconnectActiveElement()
        {
            if (ActiveElement != null)
                DisconnectElement(ActiveElement);
        }

        /// <summary>
        /// Disconnect all Connectors of the given StreetElement in the Items list, so it can be moved, sized, replaced or deleted.
        /// </summary>
        /// <param name="StreetElement">Reference to the StreetElement object to be disconnected.</param>
        public void DisconnectElement(StreetElement StreetElement)
        {
            StreetElement.UpdateStreetGroup();
            StreetElement[] streets = StreetElement.StreetGroup.ToArray();
            foreach (LaneElement lane in StreetElement.Lanes)
                foreach (Connector connector in lane.Connectors)
                    connector.Connection = null;

            foreach (StreetElement se in streets)
                se.UpdateStreetGroup();

        }

        /// <summary>
        /// Paint method to draw temporary information on top of the bitmap. It is called from the pbDrawingArea_Paint event handler, passing its Graphics object reference.
        /// This method draws outlines of the ActiveElement and its connected group, or the ActiveOverlay and a ConnectionIssue.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        public void Paint(Graphics grfx, SizeF ScaleFactor)
        {
            if (ActiveElement != null)
            {
                ActiveElement.DrawGroup(grfx, ScaleFactor, DrawMode.Outline);
                if ((PossibleConnections != null) && (PossibleConnections.Count != 0))
                    foreach (ConnectorLine cl in PossibleConnections)
                        grfx.DrawLine(new Pen(cl.DrawColor, 1), Utils.Scale(cl.ActiveConnector.CenterP, ScaleFactor), Utils.Scale(cl.StreetConnector.CenterP, ScaleFactor));
            }
            else if (ActiveOverlay != null)
            {
                ActiveOverlay.Draw(grfx, ScaleFactor, DrawMode.Outline);
            }

            if ((ConnectionIssueIndex >= 0) && (ConnectionIssueIndex < ConnectionIssues.Count))
            {
                Utils.DrawPointCircle(grfx, ConnectionIssues[ConnectionIssueIndex].C0.CenterP, ScaleFactor, Color.IndianRed, 3, 30);
                float l = 20;
                float w = 10;
                PointF[] marker = ConnectionIssues[ConnectionIssueIndex].C0.MarkerPolygon;
                PointF[] points = new PointF[4];
                points[0] = Utils.GetPoint(marker[2], Utils.RIGHT_ANGLE_RADIAN, -w / 2);
                points[1] = Utils.GetPoint(marker[2], Utils.RIGHT_ANGLE_RADIAN, w / 2);
                if (ConnectionIssues[ConnectionIssueIndex].dy > 0)
                    points[2] = Utils.GetPoint(marker[2], 0, +l);
                else
                    points[2] = Utils.GetPoint(marker[2], 0, -l);
                points[3] = points[0];
                grfx.FillPolygon(new SolidBrush(Color.LightGreen), Utils.Scale(points, ScaleFactor));

                points[0] = Utils.GetPoint(marker[3], 0, -w / 2);
                points[1] = Utils.GetPoint(marker[3], 0, w / 2);
                if (ConnectionIssues[ConnectionIssueIndex].dx > 0)
                    points[2] = Utils.GetPoint(marker[3], Utils.RIGHT_ANGLE_RADIAN, +l);
                else
                    points[2] = Utils.GetPoint(marker[3], Utils.RIGHT_ANGLE_RADIAN, -l);
                points[3] = points[0];
                grfx.FillPolygon(new SolidBrush(Color.LightGreen), Utils.Scale(points, ScaleFactor));
            }

#if DEBUG_PAINT_DASH_SYNC_ORDER
            float fontSize = 30 * ScaleFactor.Height;
            Font font = new Font("Impact", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);

            foreach (StreetElement se in Items)
            {
                PointF p = new PointF((se.Lanes[0].Connectors[0].CenterP.X + se.Lanes[0].Connectors[1].CenterP.X) / 2, (se.Lanes[0].Connectors[0].CenterP.Y + se.Lanes[0].Connectors[1].CenterP.Y) / 2);
                Utils.DrawText(grfx, se.DashSyncOrder.ToString(), Utils.Scale(p,ScaleFactor), Utils.RIGHT_ANGLE_RADIAN, font, Color.Red);

                foreach (LaneElement le in se.Lanes)
                {
                    Utils.DrawPointCircle(grfx, le.Connectors[0].MarkerPolygon[3], ScaleFactor, Color.Blue, 3, 3);
                    Utils.DrawPointCircle(grfx, le.Connectors[le.DashSyncConnectorIdx].MarkerPolygon[2], ScaleFactor, Color.Green, 3, 3);
                }
            }
#else
            if (DrawItemNumbers || DrawLaneNumbers)
            {
                float fontSize = 30 * ScaleFactor.Height;
                Font font = new Font("Impact", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                int i = 0;
                foreach (StreetElement se in Items)
                {
                    if (DrawItemNumbers)
                    {
                        PointF p = new PointF((se.Lanes[0].Connectors[0].CenterP.X + se.Lanes[0].Connectors[1].CenterP.X) / 2, (se.Lanes[0].Connectors[0].CenterP.Y + se.Lanes[0].Connectors[1].CenterP.Y) / 2);
                        Utils.DrawText(grfx, i.ToString(), Utils.Scale(p, ScaleFactor), Utils.RIGHT_ANGLE_RADIAN, font, Color.Red);
                    }

                    if (DrawLaneNumbers)
                    {
                        foreach (LaneElement le in se.Lanes)
                        {
                            PointF p = le.Connectors[0].MarkerPolygon[3];
                            Utils.DrawText(grfx, le.OwnerIdx.ToString(), Utils.Scale(p, ScaleFactor), Utils.RIGHT_ANGLE_RADIAN, font, Color.Blue);
                        }
                    }
                    i++;
                }
            }

#endif
        }


        /// <summary>
        /// Draws all StreetElements as BaseLayer, TopLayer and Overlay.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        public void Draw(Graphics grfx, SizeF ScaleFactor)
        {
            foreach (StreetElement se in Items)
                se.Draw(grfx, ScaleFactor, DrawMode.BaseLayer);
            foreach (StreetElement se in Items)
                se.Draw(grfx, ScaleFactor, DrawMode.TopLayer);
            foreach (StreetElement se in Items)
                se.Draw(grfx, ScaleFactor, DrawMode.Overlay);
        }

        /// <summary>
        /// Draws the outlines only. Used for test prints.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        public void DrawOutlines(Graphics grfx, SizeF ScaleFactor)
        {
            foreach (StreetElement se in Items)
                se.Draw(grfx, ScaleFactor, DrawMode.Outline);
        }

        /// <summary>
        /// Goes through all StreetElements in the Items list and assigns a DashSyncOrder number to each.
        /// The method starts with the first element in the items list and assigns 0 to it. 
        /// It goes then through all of its connected elements and assigns incrementing numbers to them, collecting a list of all connected eleements of each. 
        /// This continues until the DashSyncOrder of all elements is assigned. This number is used to synchronize the dash phases of connected lanes. 
        /// A streetElement with a higher DashSyncOrder synchronizes to the connected one with lower DashSyncOrder.
        /// </summary>
        public void AssignDashSyncOrders()
        {
            if (Items.Count == 0)
                return;

            foreach (StreetElement se in Items)
                se.DashSyncOrder = -1;

            List<StreetElement> elements = new List<StreetElement>();
            elements.Add(Items[0]);
            int n = 0;
            while ((n < Items.Count) && (elements.Count>0))
            {
                List<StreetElement> nextLevel = new List<StreetElement>();

                foreach (StreetElement se in elements)
                {
                    if (se.DashSyncOrder < 0)
                    {
                        se.DashSyncOrder = n++;
                        foreach(LaneElement le in se.Lanes)
                        {
                            foreach (Connector cn in le.Connectors)
                            {
                                LaneElement connle = cn.ConnectionOwner as LaneElement;
                                if ((connle != null) && (connle.Owner != null) && (connle.Owner.DashSyncOrder < 0))
                                {
                                    if (nextLevel.Contains(connle.Owner) == false)
                                        nextLevel.Add(connle.Owner);
                                }
                            }
                        }
                    }
                }

                elements = nextLevel;
            }

            foreach (StreetElement se in Items)
                foreach (LaneElement le in se.Lanes)
                    foreach (Connector cn in le.Connectors)
                    {
                        LaneElement connle = cn.ConnectionOwner as LaneElement;
                        if ((connle != null) && (connle.Owner != null))
                        {
                            if (connle.Owner.DashSyncOrder < se.DashSyncOrder)
                                le.DashSyncConnectorIdx = cn.OwnerIdx;
                        }
                    }

            foreach (StreetElement se in Items)
                foreach (LaneElement le in se.Lanes)
                {
                    if ((le.LeftLine != null) && (le.LeftLine.Dashed))
                        le.LeftLine.UpdateGeometries();

                    if ((le.RightLine != null) && (le.RightLine.Dashed))
                        le.RightLine.UpdateGeometries();
                }
        }
        #endregion Drawing related

        #region XML File Support
        /// <summary>
        /// Save the complete street map to an XML file. An XmlDocument is created and initialized and the DrawingSize values are written to it.
        /// Then each individual StreetElement will be written via its method WriteToXml.
        /// </summary>
        /// <param name="FullXmlFilename">Full file name to save the XML contents to.</param>
        public void SaveToXml(string FullXmlFilename)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlNode nodeAnnotation = doc.AppendChild(doc.CreateElement("annotation"));
            nodeAnnotation.AppendChild(doc.CreateElement("identifier")).AppendChild(doc.CreateTextNode(XML_FILE_ID_STR));

            XmlNode nodeSize = nodeAnnotation.AppendChild(doc.CreateElement("size"));
            nodeSize.AppendChild(doc.CreateElement("drawing_width")).AppendChild(doc.CreateTextNode(DrawingSize.Width.ToString()));
            nodeSize.AppendChild(doc.CreateElement("drawing_height")).AppendChild(doc.CreateTextNode(DrawingSize.Height.ToString()));

            XmlNode nodeObject = nodeAnnotation.AppendChild(doc.CreateElement("object"));
            foreach (StreetElement se in Items)
                se.WriteToXml(doc, nodeObject);

            doc.Save(FullXmlFilename);
        }

        /// <summary>
        /// Loads the contents of the XML file and creates a new list of StreetElements from it.
        /// </summary>
        /// <param name="FullXmlFilename">Full file name to load the XML contents from.</param>
        /// <returns>True, if everything was loaded successfully.</returns>
        public bool LoadFromXml(string FullXmlFilename)
        {
            Items.Clear();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(FullXmlFilename);
                XmlNode nodeAnnotation = doc.SelectSingleNode("annotation");

                //string id = nodeAnnotation.SelectSingleNode("identifier").InnerText;
                //if (id != XML_FILE_ID_STR)
                //    throw new Exception("XML file identifier error!");

                XmlNode nodeSize = nodeAnnotation.SelectSingleNode("size");
                float width = Convert.ToSingle(nodeSize.SelectSingleNode("drawing_width").InnerText);
                float height = Convert.ToSingle(nodeSize.SelectSingleNode("drawing_height").InnerText);
                drawingSize = new SizeF(width, height);

                XmlNode nodeObject = nodeAnnotation.SelectSingleNode("object");
                XmlNodeList nodeItems = nodeObject.SelectNodes("item");

                foreach (XmlNode node in nodeItems)
                {
                    StreetElement newStreetElement = null;
                    newStreetElement = MultiLaneStreet.LoadFromXml(doc, node, AppSettings);
                    if (newStreetElement == null)
                        newStreetElement = Intersection.LoadFromXml(doc, node, AppSettings);

                    if (newStreetElement != null)
                        Items.Add(newStreetElement);
                }

                ReconnectAll();
                AssignDashSyncOrders();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading segmentation classes");
            }
            return false;
        }
        #endregion XML File Support

        #region Dataset Generation
        /// <summary>
        /// Delete all files in the requested folder or creates the folder if doesn't exist.
        /// </summary>
        /// <param name="Path">Full path to the folder.</param>
        private void ClearPath(string Path)
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    string[] files = Directory.GetFiles(Path);
                    foreach (string fileName in files)
                        File.Delete(fileName);
                }
                else
                    Directory.CreateDirectory(Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ClearPath:" + Path + "\n\r" + ex.Message);
            }
        }

        /// <summary>
        /// Clears the data path and the train, val and test folders underneath or creates this folder structure if doesn't exist.
        /// </summary>
        /// <param name="Path">Full path to the folder.</param>
        private void PrepareDataSetPath(string Path)
        {
            ClearPath(Path);
            ClearPath(Path + AppSettings.SubDirTrain);
            ClearPath(Path + AppSettings.SubDirVal);
            ClearPath(Path + AppSettings.SubDirTest);
        }

        /// <summary>
        /// Writes first a list constant names and code assignements and then a list of name strings into a text file to be copied into Python code.
        /// </summary>
        /// <param name="FileName">Full path and file name of the text file to write to</param>
        /// <returns>Number of classes used in the current street map.</returns>
        public int WriteClassText(string FileName)
        {
            int n = GetUsedClassCount();
            StreamWriter sw = new StreamWriter(FileName);

            sw.WriteLine("Copy the lines below between the ---- lines into 'jetcar_definitions.py' to update the class definitions");
            sw.WriteLine("---------------------------------------------------------------------------------");
            sw.WriteLine("# Scene Parsing has N classes including nothing=0");
            sw.WriteLine("N_CLASSES = " + n);
            sw.WriteLine();
            sw.WriteLine("# Definition of all segmentation class names and values");
            sw.WriteLine("class SegmClass(enum.Enum):");
            foreach (SegmClassDef scd in SegmClassDefs.Defs)
                if (scd.UseCount > 0)
                    sw.WriteLine("\t"+scd.Name.ToLower() + " = " + scd.ClassCode);

            sw.WriteLine("\t#Unused:");

            int i = 100;
            foreach (SegmClassDef scd in SegmClassDefs.Defs)
                if (scd.UseCount <= 0)
                    sw.WriteLine("\t" + scd.Name.ToLower() + " = " + (i++));
            /*
            sw.WriteLine();
            sw.WriteLine();
            sw.WriteLine("Names only:");

            foreach (SegmClassDef scd in SegmClassDefs.Defs)
                if (scd.UseCount > 0)
                    sw.WriteLine("'"+scd.Name.ToLower()+"', \\");
            */
            sw.WriteLine();
            sw.WriteLine("---------------------------------------------------------------------------------");
            sw.WriteLine();
            sw.WriteLine("Copy the lines below into AppSettings.xml for ImageSegmenter to update the class definitions if needed");
            sw.WriteLine("  <segm_class_defs>");
            foreach (SegmClassDef scd in SegmClassDefs.Defs)
                if (scd.UseCount > 0)
                    sw.WriteLine("    <item id=\"" + scd.ClassCode.ToString() + "\" draw_order=\"" +scd.DrawOrder.ToString() + "\" draw_color=\"" + scd.DrawColor.ToArgb().ToString("X8") + "\">"+scd.Name+"</item>");
            sw.WriteLine("  </segm_class_defs>");

            sw.Close();
            return n;
        }

        /// <summary>
        /// Creates a color lookup table from the currently used classes.
        /// </summary>
        /// <returns>Array of colors as current color map</returns>
        public Color[] GetCurrentColorMap()
        {
            Color[] colorMap = new Color[256];
            int n = 0;
            for (int i = 0; i < SegmClassDefs.Defs.Length; i++)
                if (SegmClassDefs.Defs[i].UseCount > 0)
                    colorMap[n++] = SegmClassDefs.Defs[i].DrawColor;

            for (int i = n; i < colorMap.Length; i++)
                colorMap[i] = Color.White;

            return colorMap;
        }

        /// <summary>
        /// Helper function to save a color map to a CSV file, that can be used in the Jupyter notebook later.
        /// </summary>
        /// <param name="FileName">Full name of the file to save to.</param>
        public void SaveColorMapCsv(string FileName)
        {
            Color[] colorMap = GetCurrentColorMap();

            StreamWriter sw = new StreamWriter(FileName);
            for (int i = 0; i < colorMap.Length; i++)
                sw.WriteLine(colorMap[i].B.ToString() + ',' + colorMap[i].G.ToString() + ',' + colorMap[i].R.ToString() + ",255");
            sw.Close();
        }


#if DEBUG_CLASS_STREET_IMAGES
        /// <summary>
        /// Creates a colored class mask representation of the full street map adding outlines and a small triangle at the spot of the camera pointing in the direction of the view.
        /// </summary>
        /// <param name="ClassCodeBitmap">A reference bitmap to use width and height from.</param>
        /// <param name="ViewPoint">Location of the camera.</param>
        /// <param name="ViewDirection">The direction the camera is pointed to.</param>
        /// <param name="FileName">Full file name to store the resulting bitmap to.</param>
        private void SaveDebugClassStreetImg(Bitmap ClassCodeBitmap, PointF ViewPoint, double ViewDirection, string FileName )
        {
            // create a same sized bitmap for drawing the classes.
            Bitmap bmClassColor = new Bitmap(ClassCodeBitmap.Width, ClassCodeBitmap.Height);
            Graphics grfx = Graphics.FromImage(bmClassColor);
            grfx.Clear(Color.FromArgb(0, 0, 0));

            // Now draw the street map as class colors
            SetColorMode(ColorMode.ClassColor);
            Draw(grfx, new SizeF(1, 1));
            // Outline maybe helpful too
            foreach (StreetElement se in Items) se.Draw(grfx, new SizeF(1, 1), DrawMode.Outline);
            SetColorMode(ColorMode.ImageColor);

            // draw the view point triangle
            PointF[] p = new PointF[3];
            p[0] = Utils.GetPoint(ViewPoint, ViewDirection, -30);
            p[1] = Utils.GetPoint(ViewPoint, ViewDirection + Utils.RIGHT_ANGLE_RADIAN, 10);
            p[2] = Utils.GetPoint(ViewPoint, ViewDirection - Utils.RIGHT_ANGLE_RADIAN, 10);
            grfx.FillPolygon(new SolidBrush(Color.White), p);
            grfx.DrawPolygon(new Pen(Color.Black), p);
            bmClassColor.Save(FileName);
            bmClassColor.Dispose();
        }
#endif

        /// <summary>
        /// Deletes all files in the datset or creates the folder structure if not exists.
        /// </summary>
        /// <param name="DatasetPath">Full path to the dataset root.</param>
        public void ClearDataset(string DatasetPath)
        {
            if (!Directory.Exists(DatasetPath))
                Directory.CreateDirectory(DatasetPath);

            string ImgPath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirImg;
            string ClassCodePath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirMask;
            string ClassImgPath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirClassImg;

            // delete all previous data
            PrepareDataSetPath(ImgPath);
            PrepareDataSetPath(ClassCodePath);
            PrepareDataSetPath(ClassImgPath);

#if DEBUG_CLASS_STREET_IMAGES
            string ClassStreetImgPath = DatasetPath + "\\StreetClassImgs\\";
            ClearPath(ClassStreetImgPath);
#endif
#if ERODE_AND_DILATE_MASK
            string ErodedMaskPath = DatasetPath + "\\Eroded\\";
            ClearPath(ErodedMaskPath);
            string DilatedMaskPath = DatasetPath + "\\Dilated\\";
            ClearPath(DilatedMaskPath);
            string DiffMaskPath = DatasetPath + "\\Diff\\";
            ClearPath(DiffMaskPath);
#endif

            if ((AppSettings.IncludeClassImages == false) && (Directory.Exists(ClassImgPath)))
                Directory.Delete(ClassImgPath, true);

        }

        /// <summary>
        /// Write the 2 output files containing the used class definitions and the color map.
        /// </summary>
        /// <param name="DatasetPath"></param>
        public void WriteClassFileAndColorMap(string DatasetPath)
        {
            WriteClassText(DatasetPath + AppSettings.ClassTextFileName);
            SaveColorMapCsv(DatasetPath + AppSettings.ColorMapFileName);
        }

        /// <summary>
        /// Generates a dataset of virtual images of a camera like view and related class images from the current street map.
        /// This method iterates through all lanes of all street elements in the possible driving directions to create virtual 
        /// images similar to the camera view of a car driving through the map. Simultanously classification maps are generated for each view.
        /// </summary>
        /// <param name="DatasetPath">Full path to the dataset root.</param>
        /// <param name="ImgFolder">Full path to the virtual camera view images of the resulting dataset.</param>
        /// <param name="ClassImgPath">Full path to the virtual camera view class colored images of the resulting dataset.</param>
        /// <param name="ClassCodePath">Full path to the virtual camera view class code images of the resulting dataset.</param>
        /// <param name="StreetBitmap">Bitmap of the street map.</param>
        public void GenerateDataset(ref bool CreatingDataSet, string DatasetPath, Bitmap StreetBitmap)
        {
            // return when nothing to do
            if (Items.Count == 0)
                return;

            ClearDataset(DatasetPath);

            string ImgPath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirImg;
            string ClassCodePath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirMask;
            string ClassImgPath = DatasetPath + AppSettings.SubDirDataSet + AppSettings.SubDirClassImg;


#if DEBUG_CLASS_STREET_IMAGES
            string ClassStreetImgPath = DatasetPath + "\\StreetClassImgs\\";
#endif
#if ERODE_AND_DILATE_MASK
            string ErodedMaskPath = DatasetPath + "\\Eroded\\";
            string DilatedMaskPath = DatasetPath + "\\Dilated\\";
            string DiffMaskPath = DatasetPath + "\\Diff\\";
#endif

            WriteClassFileAndColorMap(DatasetPath);
            
            Random rnd = new Random(DateTime.Now.Millisecond);
            CreatingDataSet = true;

            int trainValCount = 0;
            int testOutCount = 0;

            // go through each item in the street element item list
            for (int itemIdx = 0; itemIdx < Items.Count; itemIdx++)
            {
                // go through each lane of the current street element
                for (int laneIdx = 0; laneIdx < Items[itemIdx].Lanes.Length; laneIdx++)
                {
                    // calculate the number of steps in the lane depending on the settings in AppSettings
                    double pathLength = Items[itemIdx].Lanes[laneIdx].GetCenterPathLength();
                    int nSteps = Math.Max((int)(pathLength / AppSettings.ImageStepSize),1);
                    double stp = 1.0 / nSteps;
                    double stpOffs = stp/4;

                    // go through the number of steps along the lane
                    for (int stepIdx = 0; stepIdx <= nSteps; stepIdx++)
                    {
                        // determine the number of directional views for the same point, if in a center lane that can be used in both directions
                        int nDirections = 1;
                        double directionAngle = 0;
                        double stepPos = Math.Min(stpOffs + stepIdx * stp, 0.99);
                        PointF pLaneCenter = Items[itemIdx].Lanes[laneIdx].GetLaneCenter(stepPos, ref directionAngle);
                        CameraPoint = pLaneCenter;

                        if (Items[itemIdx] is MultiLaneStreet)
                        {
                            MultiLaneStreet mls = Items[itemIdx] as MultiLaneStreet;
                            if ((mls.LaneCountCenter > 0) && (laneIdx >= mls.LaneCountRight) && ((laneIdx < mls.LaneCountRight + mls.LaneCountCenter)))
                                nDirections = 2;
                        }
                        else if (Items[itemIdx] is Intersection)
                        {
                            Intersection isn = Items[itemIdx] as Intersection;
                            int descrLaneIdx;
                            int descrIdx = isn.GetDescrIndex(Items[itemIdx].Lanes[laneIdx], out descrLaneIdx);
                            if ((isn.StreetDescriptors[descrIdx].LaneCountCenter > 0) && (descrLaneIdx >= isn.StreetDescriptors[descrIdx].LaneCountRight) &&
                                (descrLaneIdx < isn.StreetDescriptors[descrIdx].LaneCountRight + isn.StreetDescriptors[descrIdx].LaneCountCenter))
                                nDirections = 2;
                        }

                        // go through the number of directions for this point. There is normally only one view per lane in the driving direction except for center lanes, that can be used in both directions
                        for (int dirIdx = 0; dirIdx < nDirections; dirIdx++)
                        {
                            string baseName = "i" + itemIdx.ToString().PadLeft(3, '0') + "_l" + laneIdx.ToString().PadLeft(3, '0') + "_s" + stepIdx.ToString().PadLeft(3, '0') + "_d" + dirIdx.ToString();

                            //Use this to debug a specific view
                            //if ((itemIdx == 0) && (laneIdx == 0) && (stepIdx == 10) && (dirIdx == 0)) //!!!!!!!!!!!!!!!!!
                            {
                                // Create a bitmap with class code colors around that point with the viewing direction.
                                Bitmap bmClassCode = DrawClassColorBitmap(StreetBitmap, pLaneCenter, directionAngle);
                                //Bitmap bmClassCodeRaw = DrawClassColorBitmap(StreetBitmap, pLaneCenter, directionAngle);
                                //Bitmap bmClassCode = Process.Median(bmClassCodeRaw, new Size(3, 3));
                                //bmClassCodeRaw.Dispose();
#if DEBUG_CLASS_STREET_IMAGES
                                SaveDebugClassStreetImg(bmClassCode, pLaneCenter, directionAngle, ClassStreetImgPath + "StreetClassImg_" + baseName + ".jpg");
#endif
                                // go through the number of side steps in the lane
                                for (int sideIdx = 0; sideIdx < AppSettings.SideSteps.Length; sideIdx++)
                                {
                                    PointF pSide = Utils.GetPoint(pLaneCenter, directionAngle + Utils.RIGHT_ANGLE_RADIAN, AppSettings.SideSteps[sideIdx]);

                                    // go through the number of viewing angle variations
                                    for (int angleIdx = 0; angleIdx < AppSettings.AngleSteps.Length; angleIdx++)
                                    {
                                        // Create the virtual view as camera picture and class masks
                                        double viewDir = directionAngle + Utils.ToRadian(AppSettings.AngleSteps[angleIdx]);
                                        VirtualCamera.TakeImgResult CameraImgs = VirtualCamera.TakeImage(AppSettings, StreetBitmap, bmClassCode, pSide, viewDir);                                        
                                        Bitmap bmMask = Process.CreateBitmapMask(CameraImgs.ClassMask);
#if ERODE_AND_DILATE_MASK
                                        Bitmap bmEroded = Process.BitmapErode(CameraImgs.ClassMask, bmMask, new Size(21, 21));
                                        Bitmap bmDilated = Process.BitmapDilate(bmEroded, bmMask, new Size(31, 31));
                                        Bitmap bmMasked = Process.BitmapAnd(CameraImgs.ClassMask,bmDilated);
                                        Bitmap bmDiff = Process.BitmapDiff(CameraImgs.ClassMask, bmMasked);
                                        //Bitmap bmDiff = Process.BitmapDiff(bmDilated, bmEroded);
#endif
                                        // go through brightness variations
                                        for (int brightIdx = 0; brightIdx < AppSettings.BrightnessFactors.Length; brightIdx++)
                                        {
                                            float brightness = (float)AppSettings.BrightnessFactors[brightIdx];

                                            // go through the color factors
                                            for (int colorIdx = 0; colorIdx < AppSettings.ColorFactors.Length; colorIdx++)
                                            {
                                                float colorFactor = (float)AppSettings.ColorFactors[colorIdx];
                                                int rgbN = colorFactor == 1 ? 1 : 3;

                                                // apply the color factors to each color individually
                                                for (int rgbIdx = 0; rgbIdx < rgbN; rgbIdx++)
                                                {
                                                    float[] brightnessFactors = new float[] { brightness * (float)AppSettings.CameraColorCorrRed, 
                                                                                              brightness * (float)AppSettings.CameraColorCorrGreen, 
                                                                                              brightness * (float)AppSettings.CameraColorCorrBlue };
                                                    brightnessFactors[rgbIdx] *= colorFactor;

                                                    Bitmap bmBrightOut = Process.ImageBrightness(CameraImgs.StreetView, brightnessFactors, AppSettings.CenterBrightnessResults);

                                                    // go through the different noise level applications
                                                    for (int noiseIdx = 0; noiseIdx < AppSettings.NoiseLevels.Length; noiseIdx++)
                                                    {
                                                        Bitmap bmNoised = Process.ImageNoise(bmBrightOut, AppSettings.NoiseLevels[noiseIdx]);
                                                    
                                                        // choose target directory randomly with the train/val-ration as target
                                                        string trainVal = AppSettings.SubDirTrain;
                                                        trainValCount++;
                                                        bool valOut = AppSettings.ValidateCenterViewsOnly == true ?
                                                            (trainValCount >= AppSettings.TrainValRatio) && (AppSettings.SideSteps[sideIdx] == 0) && (AppSettings.AngleSteps[angleIdx] == 0) :
                                                            (rnd.Next(AppSettings.TrainValRatio) == 0) || (trainValCount > 2*AppSettings.TrainValRatio);

                                                        if (valOut == true)
                                                        {
                                                            trainVal = AppSettings.SubDirVal;
                                                            trainValCount = 0;
                                                        }

                                                        // build filename with all indices included for identification
                                                        string extBaseName = baseName + "_s" + sideIdx.ToString() + "_a" + angleIdx.ToString() + "_b" + brightIdx.ToString() + "_c" + colorIdx.ToString() + "_r" + rgbIdx.ToString() + "_n" + noiseIdx.ToString();
                                                        string imgFileName = AppSettings.PrefixImg + extBaseName + ".jpg";
                                                        string fullImgFileName = ImgPath + trainVal + imgFileName;

                                                        DisplayFileName?.Invoke(fullImgFileName);
                                                        NewBitmapsUpdate?.Invoke(bmNoised, CameraImgs.ClassImg);

                                                        // save the files
                                                        bmNoised.Save(fullImgFileName);
#if ERODE_AND_DILATE_MASK && DEBUG_MASK_IMAGES
                                                        bmMasked.Save(ClassCodePath + trainVal + AppSettings.PrefixMask + extBaseName + ".png");
#else
                                                        CameraImgs.ClassMask.Save(ClassCodePath + trainVal + AppSettings.PrefixMask + extBaseName + ".png");
#endif
                                                        if (AppSettings.IncludeClassImages == true)
                                                            CameraImgs.ClassImg.Save(ClassImgPath + trainVal + AppSettings.PrefixClassImg + extBaseName + ".jpg");

#if ERODE_AND_DILATE_MASK
                                                        bmEroded.Save(ErodedMaskPath + "Erode_" + extBaseName + ".png");
                                                        bmDilated.Save(DilatedMaskPath + "Dilate_" + extBaseName + ".png");
                                                        bmDiff.Save(DiffMaskPath + "Diff_" + extBaseName + ".png");
#endif
                                                        // create additional test images in the requested ratio
                                                        testOutCount++;
                                                        bool testOut = AppSettings.TestCenterViewsOnly == true ?
                                                            (testOutCount >= AppSettings.TestOutRatio) && (AppSettings.SideSteps[sideIdx] == 0) && (AppSettings.AngleSteps[angleIdx] == 0) :
                                                            (rnd.Next(AppSettings.TestOutRatio) == 0) || (testOutCount > 2*AppSettings.TestOutRatio);

                                                        if (testOut == true)
                                                        {
                                                            bmNoised.Save(ImgPath + AppSettings.SubDirTest + imgFileName);
                                                            CameraImgs.ClassMask.Save(ClassCodePath + AppSettings.SubDirTest + AppSettings.PrefixMask + extBaseName + ".png");

                                                            if (AppSettings.IncludeClassImages == true)
                                                                CameraImgs.ClassImg.Save(ClassImgPath + AppSettings.SubDirTest + AppSettings.PrefixClassImg + extBaseName + ".jpg");

                                                            testOutCount = 0;
                                                        }
                                                        // dispose unused bitmaps to free memory for next loops
                                                        bmNoised.Dispose();
                                           
                                                        if (CreatingDataSet == false)
                                                            return;
                                                    }
                                                    // dispose unused bitmaps to free memory for next loops
                                                    bmBrightOut.Dispose();
                                                }
                                            }
                                        }
                                        // dispose unused bitmaps to free memory for next loops
                                        CameraImgs.StreetView.Dispose();
                                        CameraImgs.ClassMask.Dispose();
                                        CameraImgs.ClassImg.Dispose();
                                        bmMask.Dispose();
                                        #if ERODE_AND_DILATE_MASK
                                        bmEroded.Dispose();
                                        bmDilated.Dispose();
                                        bmMasked.Dispose();
                                        bmDiff.Dispose();
                                        #endif
                                    }
                                }
                                // dispose unused bitmaps to free memory for next loops
                                bmClassCode.Dispose();
                            }
                            // flip the direction 180 degrees for a next view from the same spot in case of nDirections == 2
                            directionAngle += Math.PI;
                        }
                    }
                }
            }
            CreatingDataSet = false;
        }


        /// <summary>
        /// Draw a class colored street map equivalent of the StreetBitmap with the point P0 as starting point and looking to the DirectionAngle. 
        /// The map is only partially drawn to match a limited camera view from that point.
        /// </summary>
        /// <param name="StreetBitmap">Reference to the current StreetBitmap used for creating a same sized class bitmap.</param>
        /// <param name="P0">Point of a virtual car camera looking in the direction of DirectionAngle.</param>
        /// <param name="DirectionAngle">Viewing direction from the point P0.</param>
        /// <returns>Bitmap with the class information drawn on.</returns>
        private Bitmap DrawClassColorBitmap(Bitmap StreetBitmap, PointF P0, double DirectionAngle)
        {
            ClearLaneSegmClasses();

            // create a same sized bitmap for drawing the classes.
            Bitmap bmClassCode = new Bitmap(StreetBitmap.Width, StreetBitmap.Height);
            Graphics grfx = Graphics.FromImage(bmClassCode);
            grfx.Clear(Color.FromArgb(0, 0, 0));

            // get the lane element where the point is on
            LaneElement leStart = GetLaneElement(P0);
            // create the list with an initial entry to start with
            List<NextLaneEntry> leNext = new List<NextLaneEntry>();
            leNext.Add(new NextLaneEntry(leStart, SegmClassDefs.ScdDrivingDir, leStart.Owner));

            // process all list elements until done
            while (leNext.Count > 0)
            {
                NextLaneEntry next = leNext[0];
                leNext.RemoveAt(0);
                // mark the lane and all lanes of this street element and add connected lanes to the list
                leNext.AddRange(MarkLane(P0, DirectionAngle, next.LE, next.SCD, next.SourceSE));
                // sort it to keep processing all 3 driving directions first
                // leNext = SortNextLaneList(leNext);
            }

            // Now draw the street map as classes
            SetColorMode(ColorMode.ClassCode);
            Draw(grfx, new SizeF(1, 1));
            SetColorMode(ColorMode.ImageColor);

            return bmClassCode;
        }

        /// <summary>
        /// Returns a sorted version of the input list, where straight, left and right driving directions are moved to beginning of the list to be proceesed before others
        /// </summary>
        /// <param name="NextLaneList">List to be sorted.</param>
        /// <returns>Sorted list from the input list.</returns>
        private List<NextLaneEntry> SortNextLaneList(List<NextLaneEntry> NextLaneList)
        {
            if (NextLaneList.Count <= 1)
                return NextLaneList;

            List<NextLaneEntry> leSort = new List<NextLaneEntry>();
            SegmClassDef[] scdTest = { SegmClassDefs.ScdDrivingDir, SegmClassDefs.ScdRightTurnDir, SegmClassDefs.ScdLeftTurnDir };
            //SegmClassDef[] scdTest = { SegmClassDefs.ScdRightTurnDir, SegmClassDefs.ScdLeftTurnDir, SegmClassDefs.ScdDrivingDir };
            for (int i = 0; i < scdTest.Length; i++)
            {
                int j = 0;
                while (j < NextLaneList.Count)
                {
                    if (NextLaneList[j].SCD == scdTest[i])
                    {
                        leSort.Add(NextLaneList[j]);
                        NextLaneList.RemoveAt(j);
                    }
                    else j++;
                }
            }
            leSort.AddRange(NextLaneList);
            return leSort;
        }



        /// <summary>
        /// Clear all lane class codes by assigning the code for Nothing to all.
        /// </summary>
        private void ClearLaneSegmClasses()
        {
            foreach (StreetElement se in Items)
                foreach (LaneElement le in se.Lanes)
                    le.SegmClassDef = SegmClassDefs.ScdNothing;
        }

        /// <summary>
        /// A simple storage class to store a lane element reference and the segmentation class definition for it.
        /// It will be used in a list to add connected lanes.
        /// </summary>
        private class NextLaneEntry
        {
            /// <summary>Reference to the lane element object to work on later.</summary>
            public readonly LaneElement LE;
            /// <summary>Segementation class definition for that lane element.</summary>
            public readonly SegmClassDef SCD;
            /// <summary>Reference to the source street element object from where this one was tagged.</summary>
            public readonly StreetElement SourceSE;

            /// <summary>
            /// Create an instance of the storage class.
            /// </summary>
            /// <param name="LE">Reference to the lane element object to work on later.</param>
            /// <param name="SCD">Segementation class definition for that lane element.</param>
            /// <param name="SourceSE">Reference to the source street element object from where this one was tagged.</param>
            public NextLaneEntry(LaneElement LE, SegmClassDef SCD, StreetElement SourceSE)
            {
                this.LE = LE;
                this.SCD = SCD;
                this.SourceSE = SourceSE;
            }
        }

        /// <summary>
        /// Add the reference of the connected lane to the list together with the segmentation class definition.
        /// </summary>
        /// <param name="LeNext">List object to add a new entry to.</param>
        /// <param name="LE">Reference to the lane element to check for a connected lane to add</param>
        /// <param name="ConnIdx">Index of the connector to check for a connected lane.</param>
        /// <param name="LaneSegmClassDef">Segmentation class definition to store with the connected lane.</param>
        private void AddConnectionToList(List<NextLaneEntry> LeNext, LaneElement LE, int ConnIdx, SegmClassDef LaneSegmClassDef, StreetElement SourceSE)
        {
            if ((LE.Connectors[ConnIdx].ConnectionOwner != null) && (LE.Connectors[ConnIdx].ConnectionOwner is LaneElement))
                LeNext.Add(new NextLaneEntry(LE.Connectors[ConnIdx].ConnectionOwner as LaneElement, LaneSegmClassDef, SourceSE));
        }

        /// <summary>
        /// Determines, if a given ConnectorPoint is outside of the limits for marking from the current StartPoint with the view direction StartDir.
        /// AppSettings.MarkLaneMaxDistFront gives a half circle in front as limit, AppSettings.MarkLaneMaxDistSide limits to the side.
        /// A line from side to front with AppSettings.MarkLaneMaxDistSideAngle connects both left and right, see "graph" below. 
        /// a is the angle from viewing direction, b is AppSettings.MarkLaneMaxDistSideAngle and s is AppSettings.MarkLaneMaxDistSide.
        /// 
        /// 	|\ \ 
        /// 	| \  \
        /// 	|  \   \C
        /// 	|   \    \
        ///    A|    \     \  
        /// 	|     \b|    \ a|
        /// 	|      \|  S   \|
        /// 	-----------------
        ///            B
        ///
        ///   sin(a)= B/C
        ///   tan(a)= B/A
        ///   tan(b)= (B-S)/A
        ///   
        ///   B = A * tan(a)
        ///   A=(B-S)/tan(b)
        ///
        ///  B=(B-S)*tan(a)/tan(b)
        ///  B=B* tan(a)/tan(b)-S* tan(a)/tan(b)
        ///  B* tan(a)/tan(b)-B = S* tan(a)/tan(b)
        ///  B* (tan(a)/tan(b)-1)= S* tan(a)/tan(b)
        ///  B= S* tan(a)/tan(b)/(tan(a)/tan(b)-1)
        /// </summary>
        /// <param name="StartPoint">Starting point on the initial lane this view had been originated.</param>
        /// <param name="StartDir">Starting direction angle of the view at StartPoint.</param>
        /// <param name="ConnectorPoint">Connector point to use for angle and distance calculations.</param>
        /// <param name="Behind">True, when the connector point is behind the camera view.</param>
        /// <returns>True, when connector is out of limits for marking.</returns>
        private bool OutOfLimits(PointF StartPoint, double StartDir, PointF ConnectorPoint, ref bool Behind)
        {
            double limit = AppSettings.MarkLaneMaxDistFront;
            double angleDiff = Utils.LimitRadian(StartDir - Utils.GetAngle(ConnectorPoint, StartPoint));
            double dist = Utils.GetDistance(ConnectorPoint, StartPoint);

            if (Math.Abs(Utils.ToDegree(angleDiff)) >= AppSettings.MarkLaneMaxDistSideAngle + 1)
            {
                double a = angleDiff;
                double b = Utils.ToRadian(AppSettings.MarkLaneMaxDistSideAngle);
                double B0 = AppSettings.MarkLaneMaxDistFront * Math.Sin(a);

                double ratio = Math.Tan(a) / Math.Tan(b);
                double B = AppSettings.MarkLaneMaxDistSide * ratio / (ratio - 1);

                if (B < B0)
                    limit = B / Math.Sin(a);
            }

            Behind = Behind && (Math.Abs(angleDiff) > Utils.RIGHT_ANGLE_RADIAN);
            return dist > limit;
        }

        /// <summary>
        /// Marks the passed lane element with the segementation class definition and go through the other lanes of the owning street element to assign classes accordingly.
        /// Depending on the lane type and the direction, classes may be changed from the original assignment. 
        /// If the distance of a lane is too far out to front or side or even in the back, no action might be taken at all.
        /// The method returns a list of the other lanes of this street element and connected lanes to be processed afterwards.
        /// </summary>
        /// <param name="StartPoint">Starting point on the initial lane this view had been originated.</param>
        /// <param name="StartDir">Starting direction angle of the view at StartPoint.</param>
        /// <param name="leCurrent">Current lane element to assign the LaneSegmClassDef to.</param>
        /// <param name="LaneSegmClassDef">Class definition to assign to the current lane element.</param>
        /// <param name="SourceSE">Reference to the source street element object from where this one was tagged.</param>
        /// <returns>List of more lanes to assign classes to.</returns>
        private List<NextLaneEntry> MarkLane(PointF StartPoint, double StartDir, LaneElement leCurrent, SegmClassDef LaneSegmClassDef, StreetElement SourceSE)
        {
            // create the result list
            List<NextLaneEntry> leNext = new List<NextLaneEntry>();

            // just in case a null pointer had been passed, exit here
            if (leCurrent == null)
                return leNext;

            StreetElement se = leCurrent.Owner;
            PointF pointC = se.GetClosestConnector(StartPoint).CenterP;

            bool behind=true;
            bool outOfLimits = OutOfLimits(StartPoint, StartDir, leCurrent.Connectors[0].CenterP, ref behind);
            outOfLimits = outOfLimits && OutOfLimits(StartPoint, StartDir, leCurrent.Connectors[1].CenterP, ref behind);
            outOfLimits = outOfLimits && OutOfLimits(StartPoint, StartDir, pointC, ref behind);

            bool startPointInsideCurrent = (se.IsInside(StartPoint, false) != null);
            bool startPointInsideSource = (SourceSE.IsInside(StartPoint, false) != null);

            // Exit if it is behind or outside of region of interest
            if ((outOfLimits && !startPointInsideCurrent) || (behind && !startPointInsideCurrent && !startPointInsideSource))
                return leNext;

            // determine the direction of this lane, starting with the connector modes
            ConnectorMode leCurrentC0mode = leCurrent.Connectors[0].Mode;
            ConnectorMode leCurrentC1mode = leCurrent.Connectors[1].Mode;

            // Correct hidden modes for C0
            if (leCurrentC0mode == ConnectorMode.Hidden)
            {
                if (leCurrentC1mode == ConnectorMode.In)
                    leCurrentC0mode = ConnectorMode.Out;
                else if (leCurrentC1mode == ConnectorMode.Out)
                    leCurrentC0mode = ConnectorMode.In;
            }
            // Correct hidden modes for C1
            if (leCurrentC1mode == ConnectorMode.Hidden)
            {
                if (leCurrentC0mode == ConnectorMode.In)
                    leCurrentC1mode = ConnectorMode.Out;
                else if (leCurrentC0mode == ConnectorMode.Out)
                    leCurrentC1mode = ConnectorMode.In;
            }

            // When no direction is given as in a center lane, use direction from starting point to assign directions
            if ((leCurrentC0mode == ConnectorMode.NoDir) && (leCurrentC0mode == ConnectorMode.NoDir))
            {
                double angleDiff0 = Utils.LimitRadian(StartDir - Utils.GetAngle(leCurrent.Connectors[0].CenterP, StartPoint));

                if (Math.Abs(angleDiff0) < Utils.RIGHT_ANGLE_RADIAN)
                {
                    leCurrentC0mode = ConnectorMode.Out;
                    leCurrentC1mode = ConnectorMode.In;
                }
                else
                {
                    leCurrentC0mode = ConnectorMode.In;
                    leCurrentC1mode = ConnectorMode.Out;
                }
            }

            // define the connector index for in and out direction
            int outIdx = leCurrentC0mode == ConnectorMode.Out ? 0 : 1;
            int inIdx = outIdx ^ 1; 

            // Now handle the lane passed as argument and set it to the requested class if not assigned yet
            // ===============================================================================================
            if (leCurrent.SegmClassDef == SegmClassDefs.ScdNothing) 
            {
                bool addNext = true;
                // assign the class to the current lane
                leCurrent.SegmClassDef = LaneSegmClassDef;

                // Now check special cases for multi lane streets and intersections
                if (se is MultiLaneStreet)
                {
                    MultiLaneStreet mls = se as MultiLaneStreet;

                    // handle center lanes
                    if ((mls.LaneCountCenter > 0) && (leCurrent.OwnerIdx >= mls.LaneCountRight) && (leCurrent.OwnerIdx < mls.LaneCountRight + mls.LaneCountCenter))
                    {
                        // Set the center lane in a split or union as wrong direction, because it is not allowed to drive there
                        if ((mls.StreetType == StreetType.Center_Split) || (mls.StreetType == StreetType.Center_Union))
                            leCurrent.SegmClassDef = SegmClassDefs.ScdWrongDir;
                        else
                            leCurrent.SegmClassDef = SegmClassDefs.ScdCenterLane;

                        addNext = false;
                    }

                    // handle an entrance, when entering, the other lanes in the same direction should be wrong in to the left and driving dir to the right
                    // this is a bit inconsistent, since it is a right turn, but on the other side, the curvature just continues in the driving dir and there is no left turn allowed here
                    if ((mls.RampType == RampType.Entrance) && (leCurrent == mls.Lanes[mls.LaneCount - 1]) && (LaneSegmClassDef == SegmClassDefs.ScdDrivingDir))
                    {
                        // list both connected ends differently
                        for (int i = 0; i < mls.LaneCountRight; i++)
                        {
                            AddConnectionToList(leNext, mls.Lanes[i], outIdx, SegmClassDefs.ScdDrivingDir, se);
                            AddConnectionToList(leNext, mls.Lanes[i], inIdx, SegmClassDefs.ScdWrongDir, se);
                        }
                        addNext = false;
                    }
                }
                else if (se is Intersection)
                {
                    // when on a center lane of an intersection, it has to be blocked as wrong direction except a T-intersecion with lanes going through
                    Intersection isn = se as Intersection;
                    int descrLaneIdx;
                    int descrIdx = isn.GetDescrIndex(leCurrent, out descrLaneIdx);
                    if ((isn.StreetDescriptors[descrIdx].LaneCountCenter > 0) && (descrLaneIdx >= isn.StreetDescriptors[descrIdx].LaneCountRight) &&
                        (descrLaneIdx < isn.StreetDescriptors[descrIdx].LaneCountRight + isn.StreetDescriptors[descrIdx].LaneCountCenter))
                    {
                        if ((isn.StreetDescriptors.Length == 2) && (descrIdx == 0))
                        {
                            // In a T-intersection on the through lanes, correct for the pointed direction 
                            leCurrent.SegmClassDef = SegmClassDefs.ScdCenterLane;
                        }
                        else
                        {
                            leCurrent.SegmClassDef = SegmClassDefs.ScdWrongDir;
                            leCurrentC0mode = ConnectorMode.In;
                            leCurrentC1mode = ConnectorMode.Out;
                            inIdx = 0;
                            outIdx = 1;
                            addNext = false;
                        }
                    }
                }

                // Now add out connections to the list to process later
                if (addNext == true)
                {
                    AddConnectionToList(leNext, leCurrent, outIdx, leCurrent.SegmClassDef, se);
                    AddConnectionToList(leNext, leCurrent, inIdx, leCurrent.SegmClassDef, se);
                }
            }

            // The current lane is processed, now the other lanes of the owner need to be processed
            // ===============================================================================================
            if (se is MultiLaneStreet)
            {
                MultiLaneStreet mls = se as MultiLaneStreet;
                // Go through all lanes of the multi lane street and set to correct classes depending on conditions
                foreach (LaneElement le in se.Lanes)
                {
                    // Exclude all already marked lanes
                    if (le.SegmClassDef == SegmClassDefs.ScdNothing)
                    {
                        // preset idx to outIdx for driving direction and change for others later
                        int idx = outIdx;
                        // Check if it is in the same direction and mark those lanes with the same class
                        if ((leCurrentC0mode == le.Connectors[0].Mode) || (leCurrentC1mode == le.Connectors[1].Mode))
                        {
                            if (mls.RampType == RampType.ExitRamp)
                            {
                                // An exit ramp in same direction is a right turn
                                if (le.OwnerIdx == mls.LaneCount - 1)
                                {
                                    if (se.Lanes[0].SegmClassDef == SegmClassDefs.ScdWrongDir)
                                        le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                    else
                                        le.SegmClassDef = SegmClassDefs.ScdRightTurnDir;
                                }
                                // Or a wrong direction, if we are on the exit ramp
                                else if (leCurrent.OwnerIdx == mls.LaneCount - 1)
                                    le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                else
                                    le.SegmClassDef = LaneSegmClassDef;
                            }
                            else if (mls.RampType == RampType.Entrance)
                            {
                                // an on ramp is a wrong direction seen from the through lanes
                                if (le.OwnerIdx == mls.LaneCount - 1)
                                {
                                    le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                    idx = inIdx;
                                }
                                // seen from the on-ramp, all through lanes are wrong directions
                                else if (mls.Lanes[mls.LaneCount - 1].SegmClassDef == SegmClassDefs.ScdDrivingDir)
                                    le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                else
                                    le.SegmClassDef = LaneSegmClassDef;
                            }
                            else le.SegmClassDef = LaneSegmClassDef;

                            // add connected lanes to the list for further processing
                            AddConnectionToList(leNext, le, idx, le.SegmClassDef, se);

                            // if in first street element extend to other side too
                            if (startPointInsideCurrent)
                                AddConnectionToList(leNext, le, idx ^ 1, le.SegmClassDef, se);

                        }
                        // Now mark opposite directions as wrong directions
                        else if ((leCurrentC0mode == le.Connectors[1].Mode) || (leCurrentC1mode == le.Connectors[0].Mode))
                        {
                            le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                            AddConnectionToList(leNext, le, idx, le.SegmClassDef, se);

                            // if in first street element extend to other side too
                            if (startPointInsideCurrent)
                                AddConnectionToList(leNext, le, idx ^ 1, le.SegmClassDef, se);
                        }
                        // check for a center lane
                        else if ((mls.LaneCountCenter > 0) && (le.OwnerIdx >= mls.LaneCountRight) && (le.OwnerIdx < (mls.LaneCountRight + mls.LaneCountCenter)))
                        {
                            if ((mls.StreetType == StreetType.Center_Split) || (mls.StreetType == StreetType.Center_Union))
                                le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                            else
                            {
                                le.SegmClassDef = SegmClassDefs.ScdCenterLane;
                                AddConnectionToList(leNext, le, idx, le.SegmClassDef, se);

                                // if in first street element extend to other side too
                                if (startPointInsideCurrent)
                                    AddConnectionToList(leNext, le, idx ^ 1, le.SegmClassDef, se);
                            }
                        }
                    }
                }
            }
            else if (se is Intersection)
            {
                // Handle intersection lanes
                Intersection isn = se as Intersection;
                int descrLaneIdx;
                int descrIdx = isn.GetDescrIndex(leCurrent, out descrLaneIdx);

                // go through all lanes of the street descriptor the current lane belongs to
                foreach (LaneElement le in isn.StreetDescriptors[descrIdx].Lanes)
                {
                    if (le.SegmClassDef == SegmClassDefs.ScdNothing)
                    {
                        // mark the ones with same direction as driving dir
                        if ((leCurrentC0mode == le.Connectors[0].Mode) || (leCurrentC1mode == le.Connectors[1].Mode))
                        {
                            //le.SegmClassDef = SegmClassDefs.ScdDrivingDir;
                            le.SegmClassDef = LaneSegmClassDef;
                            AddConnectionToList(leNext, le, outIdx, le.SegmClassDef, se);

                            // if in first street element extend to other side too
                            if (startPointInsideCurrent)
                                AddConnectionToList(leNext, le, inIdx, le.SegmClassDef, se);
                        }
                        // all opposite directions are wrong dir
                        else if ((leCurrentC0mode == le.Connectors[1].Mode) || (leCurrentC1mode == le.Connectors[0].Mode))
                        {
                            le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                            AddConnectionToList(leNext, le, inIdx, le.SegmClassDef, se);

                            // if in first street element extend to other side too
                            if (startPointInsideCurrent)
                                AddConnectionToList(leNext, le, outIdx, le.SegmClassDef, se);
                        }
                        // center lanes are also wrong directions
                        else if (le.Connectors[0].Mode == ConnectorMode.NoDir)
                        {
                            if ((isn.StreetDescriptors.Length == 2) && (descrIdx == 0))
                                le.SegmClassDef = SegmClassDefs.ScdCenterLane;
                            else
                                le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                        }
                    }
                }

                // Check, if this intersection had been reached via driving dir code or not, so intersections reached by left or right are excluded
                bool assignRemaining = false;
                foreach (LaneElement le in isn.Lanes)
                    if (le.SegmClassDef == SegmClassDefs.ScdDrivingDir)
                    {
                        double d0 = Utils.GetDistance(StartPoint, le.Connectors[0].CenterP);
                        double d1 = Utils.GetDistance(StartPoint, le.Connectors[1].CenterP);
                        double da0 = Utils.LimitRadian(StartDir - Utils.GetAngle(le.Connectors[0].CenterP, StartPoint));
                        double da1 = Utils.LimitRadian(StartDir - Utils.GetAngle(le.Connectors[1].CenterP, StartPoint));

                        bool leBehind =  (Math.Abs(da0) > Utils.RIGHT_ANGLE_RADIAN) && (Math.Abs(da1) > Utils.RIGHT_ANGLE_RADIAN);
                        assignRemaining = !leBehind || (d0<100) || (d1<100);
                        break;
                    }

                // assign all other directions, since this intersection had been reached via driving dir
                if (assignRemaining)
                {
                    // Handle a T-intersection separately 
                    if (isn.StreetDescriptors.Length == 2)
                    {
                        // index 0 is for the lanes going through
                        if (descrIdx == 0)
                        {
                            // from the through lanes determine if the T goes left or right
                            SegmClassDef scdX = SegmClassDefs.ScdNothing;
                            if ((leCurrentC0mode == ConnectorMode.In) && ((LaneSegmClassDef == SegmClassDefs.ScdDrivingDir) || ((LaneSegmClassDef == SegmClassDefs.ScdCenterLane))))
                                scdX = SegmClassDefs.ScdRightTurnDir;
                            else if ((leCurrentC0mode == ConnectorMode.Out) && ((LaneSegmClassDef == SegmClassDefs.ScdDrivingDir) || ((LaneSegmClassDef == SegmClassDefs.ScdCenterLane))))
                                scdX = SegmClassDefs.ScdLeftTurnDir;

                            // it should always be left or right here, but
                            if (scdX != SegmClassDefs.ScdNothing)
                            {
                                // go through the lanes on the side and set them to the correct classes
                                foreach (LaneElement le in isn.StreetDescriptors[1].Lanes)
                                {
                                    if (le.SegmClassDef == SegmClassDefs.ScdNothing)
                                    {
                                        if (le.Connectors[0].Mode == ConnectorMode.Out)
                                        {
                                            // set outgoing lanes to left or right turn
                                            le.SegmClassDef = scdX;
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                        }
                                        else if (le.Connectors[0].Mode == ConnectorMode.In)
                                        {
                                            // all incoming lanes are wrong way
                                            le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                        }
                                        else
                                        {
                                            throw new Exception("else if (le.Connectors[0].Mode == ConnectorMode.In) else in isn");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //                            throw new Exception("if (scdX != SegmClassDefs.ScdNothing) else in isn");
                            }
                        }
                        else
                        {
                            // Coming from the side street of a T-intersection looking towards intersection
                            if ((leCurrentC0mode == ConnectorMode.In) && (LaneSegmClassDef == SegmClassDefs.ScdDrivingDir))
                            {
                                foreach (LaneElement le in isn.StreetDescriptors[0].Lanes)
                                {
                                    if (le.SegmClassDef == SegmClassDefs.ScdNothing)
                                    {
                                        // mark through lanes left and right as turns
                                        if (le.Connectors[1].Mode == ConnectorMode.Out)
                                        {
                                            le.SegmClassDef = SegmClassDefs.ScdRightTurnDir;
                                            AddConnectionToList(leNext, le, 1, le.SegmClassDef, se);
                                            AddConnectionToList(leNext, le, 0, SegmClassDefs.ScdWrongDir, se);
                                        }
                                        else if (le.Connectors[0].Mode == ConnectorMode.Out)
                                        {
                                            le.SegmClassDef = SegmClassDefs.ScdLeftTurnDir;
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                            AddConnectionToList(leNext, le, 1, SegmClassDefs.ScdWrongDir, se);
                                        }
                                        else
                                        {
//                                            throw new Exception("else if (le.Connectors[0].Mode == ConnectorMode.Out) else in isn");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //                            throw new Exception("if ((leCurrentC0mode == ConnectorMode.In) && (LaneSegmClassDef == SegmClassDefs.ScdDrivingDir)) else in isn");
                            }
                        }
                    }
                    // It is not a t-intersection, check if looking towards intersection
                    else if (leCurrent.Connectors[outIdx].Mode == ConnectorMode.Hidden)
                    {
                        // 3-way or 4-way looking towards intersection, define the street descriptor indices for the other directions
                        int oppositeIdx = descrIdx >= 2 ? descrIdx - 2 : descrIdx + 2;
                        int leftIdx = descrIdx > 0 ? descrIdx - 1 : descrIdx + 3;
                        int rightIdx = descrIdx < isn.StreetDescriptors.Length - 1 ? descrIdx + 1 : descrIdx - 3;

                        int[] dirIdx = { oppositeIdx, leftIdx, rightIdx };
                        SegmClassDef[] scd = { SegmClassDefs.ScdDrivingDir, SegmClassDefs.ScdLeftTurnDir, SegmClassDefs.ScdRightTurnDir };

                        // go through all other 3 directions to mark them
                        for (int i = 0; i < dirIdx.Length; i++)
                        {
                            // make sure, this index is valid
                            if ((dirIdx[i] >= 0) && (dirIdx[i] < isn.StreetDescriptors.Length))
                            {
                                //now go through all lanes here
                                foreach (LaneElement le in isn.StreetDescriptors[dirIdx[i]].Lanes)
                                {
                                    if (le.SegmClassDef == SegmClassDefs.ScdNothing)
                                    {
                                        // outgoing lanes are driving dir, left or right
                                        if (le.Connectors[0].Mode == ConnectorMode.Out)
                                        {
                                            le.SegmClassDef = scd[i];
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                        }
                                        // incoming lanes are wrong dir
                                        else if (le.Connectors[0].Mode == ConnectorMode.In)
                                        {
                                            le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                        }
                                        // center lanes are also wrong dir, but connections are marked as center lanes
                                        else if (le.Connectors[0].Mode == ConnectorMode.NoDir)
                                        {
                                            le.SegmClassDef = SegmClassDefs.ScdWrongDir;
                                            AddConnectionToList(leNext, le, 0, le.SegmClassDef, se);
                                        }
                                        else
                                        {
                                            throw new Exception("else if (le.Connectors[0].Mode == ConnectorMode.NoDir) else in isn");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //                   throw new Exception("else if (leCurrent.Connectors[outIdx].Mode == ConnectorMode.Hidden) else in isn");
                    }
                }
            }

            return SortNextLaneList(leNext);
        }

        /// <summary>
        /// Set the color mode in all street elements of this street map.
        /// </summary>
        /// <param name="ColorMode">Mode to set to.</param>
        public void SetColorMode(ColorMode ColorMode)
        {
            foreach (StreetElement se in Items)
                se.SetColorMode(ColorMode);
        }

        /// <summary>
        /// Determines the classes used in this StreetMap and returns the number of used classes.
        /// </summary>
        /// <returns>Number of used classes.</returns>
        public int GetUsedClassCount()
        {
            SegmClassDefs.ClearUseCounts();

            foreach (StreetElement se in Items)
                se.UpdateUseCounts();

            return SegmClassDefs.OptClassCodes();
        }
        #endregion Dataset Generation
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets the item at the given index.
        /// </summary>
        /// <param name="idx">Index of the item in the list.</param>
        /// <returns></returns>
        public StreetElement this[int idx]
        {
            get { return Items[idx]; }
        }

        /// <summary>
        /// Gets or sets the reference to the ActiveElement. Setting it will automatically set the ActiveOverlay to null.
        /// </summary>
        public StreetElement ActiveElement
        {
            get { return activeElement; }
            set 
            { 
                activeElement = value;
                activeOverlay = null;
            }
        }
        /// <summary>
        /// Gets or sets the reference to the ActiveOverlay. Setting it will automatically set the ActiveElement to null.
        /// </summary>
        public Overlay ActiveOverlay
        {
            get { return activeOverlay;  }
            set 
            { 
                activeOverlay = value;
                activeElement = null;
            }
        }

        /// <summary>
        /// Gets the current DrawingSize of the StreetMap.
        /// </summary>
        public SizeF DrawingSize
        {
            get { return drawingSize; }
        }


        public PointF CameraPoint
        {
            set
            {
                foreach (StreetElement se in Items)
                    se.CameraPoint = value;
            }
        }
        #endregion Public Properties

    }
}
