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
    /// Class to support marking a steering direction in an image.
    /// Currently not used.
    /// </summary>
    public class SteeringDirection
    {
        #region Private Fields
        /// <summary>Angle in Degrees of the direction from the center front of the car to the center of the street.</summary>
        private float angle;
        /// <summary>Angle in Radian of the direction from the center front of the car to the center of the street.</summary>
        private float radian;
        /// <summary>The 2 points of the direction line in normalized coordinates between 0 and 1.</summary>
        private PointF[] normalizedPoints;
        /// <summary>The 2 points of the direction line in drawing size coordinates.</summary>
        private PointF[] drawingPoints;
        /// <summary>The current size of the bitmap to draw on.</summary>
        private Size drawingSize;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Default length of the direction line normalized.</summary>
        public static float NormalizedLineLength = 0.5f;
        /// <summary>Color of the line to be drawn on the bitmap.</summary>
        public Color DrawColor;
        /// <summary>Name associated with the direction.</summary>
        public readonly string Name;
        #endregion Public Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the SteeringDirection class populating fields with the passed arguments.
        /// </summary>
        /// <param name="Name">Name associated with the direction.</param>
        /// <param name="DrawColor">Color of the line to be drawn on the bitmap.</param>
        /// <param name="Angle"Angle in Degrees of the direction from the center front of the car to the center of the street.></param>
        public SteeringDirection(string Name, Color DrawColor, float Angle)
        {
            this.Name = Name;
            this.DrawColor = DrawColor;
            this.normalizedPoints = new PointF[2];
            this.normalizedPoints[0] = new PointF(0.5f, 1.0f);
            this.drawingPoints = new PointF[2];

            this.Angle = Angle;
            this.DrawingSize = new Size(1, 1);
        }
        #endregion Constructor

        #region Public Methods
        /// <summary>
        /// Method to write the contents of this object to XML file.
        /// </summary>
        /// <param name="Doc">XML document object reference to write to.</param>
        /// <param name="Node">XML node to append a sub node with the contents.</param>
        public void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));
            nodeItem.AppendChild(Doc.CreateElement("name")).AppendChild(Doc.CreateTextNode(Name));
            nodeItem.AppendChild(Doc.CreateElement("angle")).AppendChild(Doc.CreateTextNode(Angle.ToString("F6")));
            nodeItem.AppendChild(Doc.CreateElement("draw_color")).AppendChild(Doc.CreateTextNode(DrawColor.ToArgb().ToString("X8")));
        }


        /// <summary>
        /// Create in instance of the steering direction class from XML file.
        /// </summary>
        /// <param name="Doc">XML document object reference to read from.</param>
        /// <param name="nodeItem">XML node to read directly the contents from.</param>
        /// <returns>Reference to the created object.</returns>
        public static SteeringDirection ReadFromXml(XmlDocument Doc, XmlNode nodeItem)
        {
            string name = nodeItem.SelectSingleNode("name").InnerText;
            float angle = Convert.ToInt32(nodeItem.SelectSingleNode("angle").InnerText);
            Color color = Color.FromArgb(Convert.ToInt32(nodeItem.SelectSingleNode("draw_color").InnerText, 16));

            return new SteeringDirection(name, color, angle);
        }
        #endregion Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the angle in Degrees of the direction from the center front of the car to the center of the street.
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                radian = (float)(Math.PI * angle / 180.0f);
                float dy = NormalizedLineLength * (float)Math.Cos(radian);
                float dx = NormalizedLineLength * (float)Math.Sin(radian);
                normalizedPoints[1] = new PointF(normalizedPoints[0].X + dx, normalizedPoints[0].Y - dy);
                drawingPoints[1] = new PointF(normalizedPoints[1].X * drawingSize.Width, normalizedPoints[1].Y * drawingSize.Height);
            }
        }

        /// <summary>
        /// Gets the angle in Radian of the direction from the center front of the car to the center of the street.
        /// </summary>
        public float Radian
        {
            get { return radian; }
        }

        /// <summary>
        /// Gets or sets the current size of the bitmap to draw on. Setting the DrawSize will convert the line points from normalized version to the drawing version.
        /// </summary>
        public Size DrawingSize
        {
            get { return drawingSize; }
            set
            {
                if ((value.Width != drawingSize.Width) || (value.Height != drawingSize.Height))
                {
                    drawingSize = value;
                    drawingPoints[0] = new PointF(normalizedPoints[0].X * drawingSize.Width, normalizedPoints[0].Y * drawingSize.Height);
                    drawingPoints[1] = new PointF(normalizedPoints[1].X * drawingSize.Width, normalizedPoints[1].Y * drawingSize.Height);
                }
            }
        }

        /// <summary>
        /// Gets the 2 points of the direction line in normalized coordinates between 0 and 1.
        /// </summary>
        public PointF[] NormalizedPoints
        {
            get { return normalizedPoints; }
        }

        /// <summary>
        /// Gets the 2 points of the direction line in drawing size coordinates.
        /// </summary>
        public PointF[] DrawingPoints
        {
            get { return drawingPoints; }
        }

        /// <summary>
        /// Gets or sets the end point of the line in drawing coordinates. Setting will cause the angle to be recalculated.
        /// </summary>
        public PointF DrawingEndPoint
        {
            get { return drawingPoints[1]; }
            set
            {
                float dx = value.X -drawingPoints[0].X;
                float dy = drawingPoints[0].Y - value.Y;
                float radian = (float)Math.Atan(dx / dy);
                Angle = radian * 180 / (float)Math.PI;
            }
        }
        #endregion Public Properties


    }
}
