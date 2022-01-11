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

namespace StreetMaker
{
    /// <summary>
    /// Storage class for a possible connection between 2 connectors of different elements. The GUI will draw a line between the 2 connectors indicating that they might be able to connect.
    /// </summary>
    public class ConnectorLine
    {
        #region Public Fields
        /// <summary>Reference to the connector of the currently active element that is moved.</summary>
        public readonly Connector ActiveConnector;
        /// <summary>Reference to the connector of the already placed street element.</summary>
        public readonly Connector StreetConnector;
        /// <summary>Distance between the 2 connectors. Used to check for snap distance.</summary>
        public readonly double Distance;
        /// <summary>Color of the drawing line indicating just a possible connection or a possible snap.</summary>
        public readonly Color DrawColor;
        #endregion Public Fields

        /// <summary>
        /// Creates an instance of the ConnectorLine class from the passed values.
        /// </summary>
        /// <param name="ActiveConnector">Reference to the connector of the currently active element that is moved.</param>
        /// <param name="StreetConnector">Reference to the connector of the already placed street element.</param>
        /// <param name="Distance">Distance between the 2 connectors. Used to check for snap distance.</param>
        /// <param name="DrawColor">Color of the drawing line indicating just a possible connection or a possible snap.</param>
        public ConnectorLine(Connector ActiveConnector, Connector StreetConnector, double Distance, Color DrawColor)
        {
            this.ActiveConnector = ActiveConnector;
            this.StreetConnector = StreetConnector;
            this.Distance = Distance;
            this.DrawColor = DrawColor;
        }
    }
}
