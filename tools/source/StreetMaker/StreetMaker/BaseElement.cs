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
    /// Base class for lane and street classes introducing connections and drawing.
    /// </summary>
    public class BaseElement
    {
        /// <summary>Draw offset to extend the length of the base element. The value should be just a fraction to avoid rounding artefacts.</summary>
        public const double DRAW_OFFS_LENGTH = 0.25;
        /// <summary>Draw offset to extend the width of the base element. The value should be just a fraction to avoid rounding artefacts.</summary>
        public const double DRAW_OFFS_WIDTH = 0.1;

        #region Protected Fields
        /// <summary>Used to block re-entrance of event handlers. Normally false, it will be set to true after entering the handler and false before leaving</summary>
        protected bool eventsBlocked;
        #endregion Protected Fields

        #region Public Fields
        /// <summary>Color of this object used for drawing.</summary>
        public Color Color;
        /// <summary>Color for outlining this object only.</summary>
        public Color OutlineColor;
        /// <summary>Background color to be used to restore the background.</summary>
        public Color BackgroundColor;
        /// <summary>Pen width of the outline lines.</summary>
        public double OutlineLineWidth;
        /// <summary>True, if this object is in the mode of moving the whole object. False means sizing.</summary>
        public bool EditModeMove;
        /// <summary>Normally false to use the object Color in the Draw method. When set to true, the ClassCodeColor is used instead.</summary>
        public ColorMode ColorMode;

        /// <summary>Array of connector objects owned by this object.</summary>
        public readonly Connector[] Connectors;

        /// <summary>Refernce to the SegmClassDef object to be used with this instance.</summary>
        public SegmClassDef SegmClassDef;

        #endregion Public Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the BaseElement object.
        /// </summary>
        /// <param name="Width">Width of this element.</param>
        /// <param name="Color">Color of this object used for drawing.</param>
        /// <param name="OutlineColor">Color for outlining this object only.</param>
        /// <param name="OutlineLineWidth">Pen width of the outline lines.</param>
        /// <param name="BackgroundColor">Background color to be used to restore the background.</param>
        public BaseElement(double Width, Color Color, Color OutlineColor, double OutlineLineWidth, Color BackgroundColor)
        {
            this.Color = Color;
            this.OutlineColor = OutlineColor;
            this.OutlineLineWidth = OutlineLineWidth;
            this.BackgroundColor = BackgroundColor;
            this.EditModeMove = true;
            this.ColorMode = ColorMode.ImageColor;
            this.SegmClassDef = SegmClassDefs.GetSegmClassDef(null);

            Connectors = new Connector[2];
            eventsBlocked = false;
            for(int i = 0; i<Connectors.Length; i++)
            {
                Connectors[i] = new Connector(this, i, Width);
                Connectors[i].DrawOffsL = i == 0 ? DRAW_OFFS_LENGTH : -DRAW_OFFS_LENGTH;
                Connectors[i].DrawOffsW = DRAW_OFFS_WIDTH;
                Connectors[i].Changed += BaseConnectorChangeEvent;
            }
        }
        #endregion Constructor

        #region Protected Methods
        /// <summary>
        /// Outer part of the connector change event handler blocking re-entrance before calling the inner event handler HandleConnectorEvent().
        /// </summary>
        /// <param name="sender">Connector object that sent the event.</param>
        protected void BaseConnectorChangeEvent(Connector sender)
        {
            if (eventsBlocked)
                return;
            eventsBlocked = true;
            HandleConnectorChangeEvent(sender);
            eventsBlocked = false;
        }


        /// <summary>
        /// Virtual inner event handler to be overwritten by inherited classes. The method in this class is empty.
        /// </summary>
        /// <param name="sender">Connector object that sent the event.</param>
        protected virtual void HandleConnectorChangeEvent(Connector sender)
        {

        }
        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Returns the color to be used for drawing depending on the ColorMode.
        /// </summary>
        /// <returns>Color for drawing.</returns>
        public Color GetDrawColor()
        {
            switch (ColorMode)
            {
                case ColorMode.ImageColor: return Color;
                case ColorMode.ClassCode: return SegmClassDef.ClassCodeColor;
                case ColorMode.ClassColor: return SegmClassDef.DrawColor;
            }
            return Color;
        }

        /// <summary>
        /// Suspend calls to HandleConnectorEvent() until ResumeEvents() is called.
        /// </summary>
        public void SuspendEvents()
        {
            eventsBlocked = true;
        }

        /// <summary>
        /// Resume calls to HandleConnectorEvent() after suspension from SuspendEvents() call.
        /// </summary>
        public void ResumeEvents()
        {
            eventsBlocked = false;
        }


        /// <summary>
        /// Virtual draw method of this class that will be overwritten by inherited classes. In this class it only calls the connector draw methods.
        /// </summary>
        /// <param name="grfx">Reference to the graphics object to be used for drawing.</param>
        /// <param name="ScaleFactor">Scale factor in both dimensions.</param>
        /// <param name="DrawMode">Determines what part or layer to draw. </param>
        public virtual void Draw(Graphics grfx, SizeF ScaleFactor, DrawMode DrawMode)
        {
            for (int i = 0; i < Connectors.Length; i++)
                Connectors[i].Draw(grfx, ScaleFactor, DrawMode);
        }


        /// <summary>
        /// Returns true, if the given point is inside of this object area. This method has to be overwritten in an inherited class, since it always retunrs false here.
        /// </summary>
        /// <param name="P">Point to check.</param>
        /// <returns>True, if inside.</returns>
        public virtual object IsInside(PointF P)
        {
            return null;
        }

        #endregion Public Methods

        #region Public Properties
        #endregion Public Properties


    }
}
