// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Drawing;
using System.Xml;

namespace ImageSegmenter
{
    /// <summary>
    /// Basic property definitions for segmentation classes. There should be one for each segmentation class in a project, stored in the XML file. 
    /// </summary>
    public class SegmClassDef
    {
        #region Public Readonly Fields
        /// <summary>Identifier or code of the class.</summary>
        public readonly int ID;
        /// <summary>Human friendly name of the class, used for the GUI.</summary>
        public readonly string Name;
        /// <summary>Defines the order of drawing the filled polygon in the mask. Lower orders are drawn first and higher orders can draw over lower orders, replacing the codes in the mask.</summary>
        public readonly int DrawOrder;
        /// <summary>Color to be used in the GUI when displaying the polygon on the image or the filled area in the mask.</summary>
        public readonly Color DrawColor;
        #endregion Public Readonly Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the segmentation class definition, loading the passed parameter into its fields.
        /// </summary>
        /// <param name="ID">Identifier or code of the class.</param>
        /// <param name="Name">Human friendly name of the class, used for the GUI.</param>
        /// <param name="DrawOrder">Defines the order of drawing the filled polygon in the mask. Lower orders are drawn first and higher orders can draw over lower orders, replacing the codes in the mask.</param>
        /// <param name="DrawColor">Color to be used in the GUI when displaying the polygon on the image or the filled area in the mask.</param>
        public SegmClassDef(int ID, string Name, int DrawOrder, Color DrawColor)
        {
            this.ID = ID;
            this.Name = Name;
            this.DrawOrder = DrawOrder;
            this.DrawColor = DrawColor;
        }
        #endregion Constructor

    }

    /// <summary>
    /// Handles a specific segmentation class instance for an image. Segmentation class areas in an image are outlined with a polygon in the GUI. 
    /// A segmentation mask is then created by drawing a filled polygon area containing the segmentation class ID or code on a mask bitmap for model training.
    /// This class holds the polygon as normalized version and drawing version scaled to the current image size. It also supports reading and writing from/to XML files.
    /// </summary>
    public class SegmClass
    {
        #region Private Fields
        /// <summary>Reference of the definition instance to be used for this segmentation class object.</summary>
        private SegmClassDef def;
        /// <summary>The normalized polygon is scaled between 0 and 1 for image width=1 and height=1. This allows scaling it to any image size used in the GUI.</summary>
        private PointF[] normalizedPolygon;
        /// <summary>Represents the outline rectangle of the normalizedPolygon.</summary>
        private RectangleF normalizedBounds;
        /// <summary>The top left point is one of the normalizedPolygon points to be used for placing the segmentation class name text.</summary>
        private PointF topLeftPoint;


        /// <summary>Currently used size of the image to draw to. The normalized polygon will be scaled to the drawPolygon for drawing.</summary>
        private Size drawSize;
        /// <summary>Calculated from the topLeftPoint, this point provides the text location in the drawing space.</summary>
        private PointF drawTextLocation;
        /// <summary>Segmentation class outline polygon scaled from the normalizedPolygon to the current image size via drawSize.</summary>
        private PointF[] drawPolygon;
        /// <summary>Represents the outline rectangle of the drawPolygon.</summary>
        private RectangleF drawBounds;
        /// <summary>Instance of the pen for drawing the polygon.</summary>
        private Pen drawPen;
        /// <summary>Instance of the brush for drawing the filled polygon area.</summary>
        private Brush drawBrush;
        #endregion Private Fields

        #region Constructors
        /// <summary>
        /// Creates the object instance of the segmentation class object. The DrawSize will be initialized to width=1 and height=1, making normalized polygon and draw polygon the same.
        /// </summary>
        /// <param name="Def">Reference of the segmentation class definition object for accessing ID, name etc.</param>
        /// <param name="NormalizedPolygon">The normalized polygon is scaled between 0 and 1 for image width and height.</param>
        public SegmClass(SegmClassDef Def, PointF[] NormalizedPolygon)
        {
            this.def = Def;
            this.normalizedPolygon = NormalizedPolygon;
            GetBounds();
            this.DrawSize = new Size(1, 1);

            this.drawPen = new Pen(Def.DrawColor);
            this.drawBrush = new SolidBrush(Def.DrawColor);
        }

        /// <summary>
        /// Creates the object instance of the segmentation class object. The normalized polygon  will be created internally by dividing the draw polygon values by the draw size values.
        /// </summary>
        /// <param name="Def">Reference of the segmentation class definition object for accessing ID, name etc.</param>
        /// <param name="DrawPolygon">Polygon scaled to the draw size of an image.</param>
        /// <param name="DrawSize">Size of the image as width and height.</param>
        public SegmClass(SegmClassDef Def, PointF[] DrawPolygon, Size DrawSize)
        {
            this.def = Def;
            this.drawPolygon = DrawPolygon;
            this.drawSize = DrawSize;

            normalizedPolygon = GetNormalizedPolygon(DrawPolygon, DrawSize);
            GetBounds();

            drawBounds = new RectangleF(normalizedBounds.X * drawSize.Width, normalizedBounds.Y * drawSize.Height, normalizedBounds.Width * drawSize.Width, normalizedBounds.Height * drawSize.Height);
            drawTextLocation = new PointF(topLeftPoint.X * drawSize.Width, topLeftPoint.Y * drawSize.Height - 12);

            this.drawPen = new Pen(Def.DrawColor);
            this.drawBrush = new SolidBrush(Def.DrawColor);
        }
        #endregion Constructors

        #region Private Methods
        /// <summary>
        /// Determine the outline rectangle of the normalized polygon and find the top left point for the text location.
        /// </summary>
        private void GetBounds()
        {
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float xmax = float.MinValue;
            float ymax = float.MinValue;
            topLeftPoint = new PointF(normalizedPolygon[0].X, normalizedPolygon[0].Y);
            float d0 = topLeftPoint.X * topLeftPoint.X + topLeftPoint.Y * topLeftPoint.Y;

            for (int i = 0; i < normalizedPolygon.Length; i++)
            {
                xmin = Math.Min(xmin, normalizedPolygon[i].X);
                ymin = Math.Min(ymin, normalizedPolygon[i].Y);
                xmax = Math.Max(xmax, normalizedPolygon[i].X);
                ymax = Math.Max(ymax, normalizedPolygon[i].Y);

                float d1 = (normalizedPolygon[i].X * normalizedPolygon[i].X + normalizedPolygon[i].Y * normalizedPolygon[i].Y);
                if (d1 < d0)
                {
                    topLeftPoint = normalizedPolygon[i];
                    d0 = d1;
                }
            }
            normalizedBounds = new RectangleF(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        /// <summary>
        /// Sets the drawSize field and updates the drawPolygon, drawBounds and drawTextLocation scaled up to the NewDrawSize width and height.
        /// </summary>
        /// <param name="NewDrawSize">New draw size width and height.</param>
        private void SetDrawSize(Size NewDrawSize)
        {
            drawSize = NewDrawSize;
            drawPolygon = GetDrawPolygon(normalizedPolygon, drawSize);

            drawBounds = new RectangleF(normalizedBounds.X * drawSize.Width, normalizedBounds.Y * drawSize.Height, normalizedBounds.Width * drawSize.Width, normalizedBounds.Height * drawSize.Height);
            drawTextLocation = new PointF(topLeftPoint.X * drawSize.Width, topLeftPoint.Y * drawSize.Height - 12);
        }
        #endregion Private Methods

        #region Public Static Methods
        /// <summary>
        /// Normalize a polygon given in image size space to coordinate values between 0 and 1.
        /// </summary>
        /// <param name="DrawPolygon">Polygon in image space of a current DrawSize.</param>
        /// <param name="DrawSize">Image draw size as width and height of the image.</param>
        /// <returns>Normalized polygon.</returns>
        public static PointF[] GetNormalizedPolygon(PointF[] DrawPolygon, Size DrawSize)
        {
            float factX = DrawSize.Width - 1;
            float factY = DrawSize.Height - 1;
            PointF[] normalizedPolygon = new PointF[DrawPolygon.Length];
            for (int i = 0; i < DrawPolygon.Length; i++)
                normalizedPolygon[i] = new PointF(DrawPolygon[i].X / factX, DrawPolygon[i].Y / factY);

            return normalizedPolygon;
        }

        /// <summary>
        /// Scale up a normalized polygon to the requested image size given by DrawSize.
        /// </summary>
        /// <param name="NormalizedPolygon">Normalized polygon with values between 0 and 1. </param>
        /// <param name="DrawSize">Image size in width and height for drawing.</param>
        /// <returns>Polygon scaled to the DrawSize.</returns>
        public static PointF[] GetDrawPolygon(PointF[] NormalizedPolygon, Size DrawSize)
        {
            float factX = DrawSize.Width - 1;
            float factY = DrawSize.Height - 1;
            PointF[] drawPolygon = new PointF[NormalizedPolygon.Length];
            for (int i = 0; i < NormalizedPolygon.Length; i++)
                drawPolygon[i] = new PointF(NormalizedPolygon[i].X * factX, NormalizedPolygon[i].Y * factY);

            return drawPolygon;
        }

        /// <summary>
        /// Normalize a given point in image draw size space.
        /// </summary>
        /// <param name="Point">Point coordinates in drawing space.</param>
        /// <param name="DrawSize">Image draw size.</param>
        /// <returns>Normalized point.</returns>
        public static PointF GetNormalizedPoint(PointF Point, Size DrawSize)
        {
            float factX = DrawSize.Width - 1;
            float factY = DrawSize.Height - 1;
            return new PointF(Point.X / factX, Point.Y / factY);
        }

        /// <summary>
        /// Scales a normalized point to the draw size of an image.
        /// </summary>
        /// <param name="Point">Normalized point.</param>
        /// <param name="DrawSize">Image darw size.</param>
        /// <returns>Scaled up point coordinates.</returns>
        public static PointF GetDrawPoint(PointF Point, Size DrawSize)
        {
            float factX = DrawSize.Width - 1;
            float factY = DrawSize.Height - 1;
            return new PointF(Point.X * factX, Point.Y * factY);
        }
        #endregion Public Static Methods

        #region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));

            nodeItem.AppendChild(Doc.CreateElement("id")).AppendChild(Doc.CreateTextNode(Def.ID.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("name")).AppendChild(Doc.CreateTextNode(Def.Name));
            nodeItem.AppendChild(Doc.CreateElement("draw_order")).AppendChild(Doc.CreateTextNode(Def.DrawOrder.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("draw_color")).AppendChild(Doc.CreateTextNode(Def.DrawColor.ToArgb().ToString("X8")));

            XmlNode nodePolygon = nodeItem.AppendChild(Doc.CreateElement("polygon"));
            for (int i=0; i<normalizedPolygon.Length; i++)
            {
                XmlNode nodePoint = Doc.CreateElement("point");
                nodePolygon.AppendChild(nodePoint);
                nodePoint.Attributes.Append(Doc.CreateAttribute("idx")).Value = i.ToString();
                nodePoint.Attributes.Append(Doc.CreateAttribute("x")).Value = normalizedPolygon[i].X.ToString("F7");
                nodePoint.Attributes.Append(Doc.CreateAttribute("y")).Value = normalizedPolygon[i].Y.ToString("F7");
            }
        }

        /// <summary>
        /// Reads the contents for one segmentation class instance from an XML document at the specified node and returns the SegmClass object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <returns>Reference to the SegmClass created from the XML file contents.</returns>
        public static SegmClass ReadFromXml(XmlDocument Doc, XmlNode nodeItem)
        {
            int id = Convert.ToInt32(nodeItem.SelectSingleNode("id").InnerText);
            string name = nodeItem.SelectSingleNode("name").InnerText;
            int draw_order = Convert.ToInt32(nodeItem.SelectSingleNode("draw_order").InnerText);
            Color draw_color = Color.FromArgb(Convert.ToInt32(nodeItem.SelectSingleNode("draw_color").InnerText,16));
 
            XmlNode nodePolygon = nodeItem.SelectSingleNode("polygon");
            XmlNodeList pointPoints = nodePolygon.SelectNodes("point");
            PointF[] poly = new PointF[pointPoints.Count];
            for (int i = 0; i < pointPoints.Count; i++)
            {
                int idx = Convert.ToInt32(pointPoints[i].Attributes["idx"].Value);
                float x = Convert.ToSingle(pointPoints[i].Attributes["x"].Value);
                float y = Convert.ToSingle(pointPoints[i].Attributes["y"].Value);
                poly[idx] = new PointF(x, y);
            }

            return new SegmClass(new SegmClassDef(id, name, draw_order, draw_color), poly);
        }
        #endregion XML File Support

        #region Public Properties
        /// <summary>
        /// The DrawSize property is used to scale the normalized polygon, bounds and text location to real image size. Setting this property will automatically scale these fields to the new settings.
        /// </summary>
        public Size DrawSize
        {
            get { return drawSize; }
            set
            {
                if ((drawSize.Width != value.Width) || (drawSize.Height != value.Height))
                    SetDrawSize(value);
            }
        }

        /// <summary>
        /// Reference to the segmentation class definition object to provide the ID, name color and draw order. Changing this reference is a re-classification of the ploygon area.
        /// </summary>
        public SegmClassDef Def
        {
            get { return def; }
            set
            {
                def = value;
                drawPen = new Pen(def.DrawColor);
                drawBrush = new SolidBrush(def.DrawColor);
            }
        }

        /// <summary>
        /// The normalized polygon is scaled between 0 and 1 for image width=1 and height=1. This allows scaling it to any image size used in the GUI.
        /// Assigning a new polygon will automatically update the bounds and calculate the scaled draw representations. 
        /// </summary>
        public PointF[] NormalizedPolygon
        {
            get { return normalizedPolygon; }
            set
            {
                normalizedPolygon = value;
                GetBounds();
                SetDrawSize(drawSize);
            }
        }

        /// <summary>
        /// Returns the coordinates for the text at the top left of the draw polygon.
        /// </summary>
        public PointF DrawTextLocation
        {
            get { return drawTextLocation; }
        }

        /// <summary>
        /// Returns the segmentation class outlining polygon scaled to the image size. 
        /// </summary>
        public PointF[] DrawPolygon
        {
            get { return drawPolygon; }
        }

        /// <summary>
        /// Returns the bounds rectangle of the area in the drawing space.
        /// </summary>
        public RectangleF DrawBounds
        {
            get { return drawBounds; }
        }

        /// <summary>
        /// Returns the drawing pen for the polygon.
        /// </summary>
        public Pen DrawPen
        {
            get { return drawPen; }
        }

        /// <summary>
        /// Returns the brush for filling the polygon.
        /// </summary>
        public Brush DrawBrush
        {
            get { return drawBrush; }
        }
        #endregion Public Properties


    }

}

