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

namespace StreetMaker
{    
    /// <summary>
    /// Spezialized storage class for one specific connection issue detected.
    /// </summary>
    public class ConnectionIssue
    {
        #region Public Fields
        /// <summary>Reference to the first connector object of the issue.</summary>
        public readonly Connector C0;
        /// <summary>Reference to the second connector object of the issue.</summary>
        public readonly Connector C1;
        /// <summary>Distance between the 2 connector senter points.</summary>
        public readonly double Distance;
        /// <summary>Angle differenc between the 2 connector.</summary>
        public readonly double Angle;
        /// <summary>Delta x between the 2 connector points</summary>
        public readonly double dx;
        /// <summary>Delta y between the 2 connector points</summary>
        public readonly double dy;
        /// <summary>True, if the 2 connectors had been reported a s connected.</summary>
        public readonly bool Connected;
        #endregion Public Fields

        /// <summary>
        /// Creates an instance of the ConnectionIssue class filling in the fields from the given 2 connector objects.
        /// </summary>
        /// <param name="C0">Reference to the first connector object of the issue.</param>
        /// <param name="C1">Reference to the second connector object of the issue.</param>
        public ConnectionIssue(Connector C0, Connector C1)
        {
            this.C0 = C0;
            this.C1 = C1;
            this.Distance = Utils.GetDistance(C0.CenterP, C1.CenterP);
            this.Angle = Utils.ToDegree(Utils.GetAngle(C0.CenterP, C1.CenterP));
            this.dx = C1.CenterP.X - C0.CenterP.X;
            this.dy = C1.CenterP.Y - C0.CenterP.Y;
            this.Connected = C0.Connection == C1;
        }
    }

}
