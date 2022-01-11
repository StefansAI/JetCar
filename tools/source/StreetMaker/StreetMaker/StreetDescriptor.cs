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
using System.Xml;
using System.Windows.Forms;


namespace StreetMaker
{
    /// <summary>
    /// Class to provide to describe a street junction at an intersection. It defines the number lanes in and out, crosswalk markings, stop or yield markings etc.
    /// </summary>
    public class StreetDescriptor
    {
        #region Public Readonly Fields
        /// <summary>Number of Lanes on the right side, or in our direction.</summary>
        public readonly int LaneCountRight;
        /// <summary>Number of Lanes in the center used as birectional turn lanes.</summary>
        public readonly int LaneCountCenter;
        /// <summary>Number of Lanes on the left side, or in opposite direction.</summary>
        public readonly int LaneCountLeft;
        /// <summary>CrosswalkType type definition for this street descriptor.</summary>
        public readonly CrosswalkType CrosswalkType;
        /// <summary>Stop or Yield type definition for this street descriptor.</summary>
        public readonly StopYieldType StopYieldType;
        /// <summary>LineType of the right border line.</summary>
        public readonly LineType RightBorderLine;
        /// <summary>LineType of the right lines between the lanes.</summary>
        public readonly LineType RightLaneLine;
        /// <summary>LineType of the center divider lines between the opposite lanes or a center lane.</summary>
        public readonly LineType DividerLine;
        /// <summary>LineType of the center divider lines between the center lane and the left lanes.</summary>
        public readonly LineType DividerLine2;
        /// <summary>LineType of the left lines between the lanes.</summary>
        public readonly LineType LeftLaneLine;
        /// <summary>LineType of the left border line.</summary>
        public readonly LineType LeftBorderLine;
        /// <summary>Total lane count.</summary>
        public readonly int LaneCount;
        /// <summary>Length of this street junction.</summary>
        public readonly double Length;
        /// <summary>References to all lanes in this object.</summary>
        public readonly LaneElement[] Lanes;
        #endregion Public Readonly Fields

        #region Constructor
        /// <summary>
        /// Creates an instance of the StreetDescriptor class from all passed parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="LaneCountRight">Number of Lanes on the right side, or in our direction.</param>
        /// <param name="LaneCountCenter">Number of Lanes in the center used as birectional turn lanes.</param>
        /// <param name="LaneCountLeft">Number of Lanes on the left side, or in opposite direction.</param>
        /// <param name="CrosswalkType">CrosswalkType type definition for this street descriptor.</param>
        /// <param name="StopYieldType">Stop or Yield type definition for this street descriptor.</param>
        /// <param name="RightBorderLine">LineType of the right border line.</param>
        /// <param name="RightLaneLine">LineType of the right lines between the lanes.</param>
        /// <param name="DividerLine">LineType of the center divider lines between the opposite lanes or a center lane.</param>
        /// <param name="DividerLine2">LineType of the center divider lines between the center lane and the left lanes.</param>
        /// <param name="LeftLaneLine">LineType of the left lines between the lanes.</param>
        /// <param name="LeftBorderLine">LineType of the left border line.</param>
        /// <param name="Length">Length of this street junction.</param>
        public StreetDescriptor(AppSettings AppSettings, int LaneCountRight, int LaneCountCenter, int LaneCountLeft, CrosswalkType CrosswalkType, StopYieldType StopYieldType, 
                                LineType RightBorderLine, LineType RightLaneLine, LineType DividerLine, LineType DividerLine2, LineType LeftLaneLine, LineType LeftBorderLine, double Length)
        {
            if ((LaneCountRight > AppSettings.MaxLaneCountLeftRight) || (LaneCountCenter > AppSettings.MaxLaneCountCenter) || (LaneCountLeft > AppSettings.MaxLaneCountLeftRight))
                throw new Exception("Invalid Parameter creating StreetDescriptor!");

            this.LaneCountRight = LaneCountRight;
            this.LaneCountCenter = LaneCountCenter;
            this.LaneCountLeft = LaneCountLeft;
            this.CrosswalkType = CrosswalkType;
            this.StopYieldType = StopYieldType;
            this.RightLaneLine = RightLaneLine;
            this.DividerLine = DividerLine;

            this.RightBorderLine = RightBorderLine;
            this.RightLaneLine = RightLaneLine;
            this.DividerLine = DividerLine;
            this.DividerLine2 = DividerLine2;
            this.LeftLaneLine = LeftLaneLine;
            this.LeftBorderLine = LeftBorderLine;
            this.Length = Length;
            this.LaneCount = LaneCountRight + LaneCountCenter + LaneCountLeft;
            this.Lanes = new LaneElement[this.LaneCount];
        }


        /// <summary>
        /// Creates an instance of the StreetDescriptor class from all passed parameter.
        /// </summary>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        /// <param name="LaneCountRight">Number of Lanes on the right side, or in our direction.</param>
        /// <param name="LaneCountCenter">Number of Lanes in the center used as birectional turn lanes.</param>
        /// <param name="LaneCountLeft">Number of Lanes on the left side, or in opposite direction.</param>
        /// <param name="CrosswalkType">CrosswalkType type definition for this street descriptor.</param>
        /// <param name="StopYieldType">Stop or Yield type definition for this street descriptor.</param>
        /// <param name="RightLaneLine">LineType of the right lines between the lanes.</param>
        /// <param name="DividerLine">LineType of the center divider lines between the opposite lanes or a center lane.</param>
        public StreetDescriptor(AppSettings AppSettings, int LaneCountRight, int LaneCountCenter, int LaneCountLeft, CrosswalkType CrosswalkType, StopYieldType StopYieldType, LineType RightLaneLine, LineType DividerLine) :
            this(AppSettings, LaneCountRight, LaneCountCenter, LaneCountLeft, CrosswalkType, StopYieldType,  LineType.ShoulderLine, RightLaneLine, DividerLine, DividerLine, LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.DefaultJunctionLength)
        {

        }
        #endregion Constructor

        #region Public Methods

        #region XML File Support
        /// <summary>
        /// Write the object contents to the XML document at the specified node.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="Node">Open node to append the contents of this object to as new child.</param>
        public void WriteToXml(XmlDocument Doc, XmlNode Node)
        {
            XmlNode nodeItem = Node.AppendChild(Doc.CreateElement("item"));

            nodeItem.AppendChild(Doc.CreateElement("lane_count_right")).AppendChild(Doc.CreateTextNode(LaneCountRight.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("lane_count_center")).AppendChild(Doc.CreateTextNode(LaneCountCenter.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("lane_count_left")).AppendChild(Doc.CreateTextNode(LaneCountLeft.ToString()));

            nodeItem.AppendChild(Doc.CreateElement("crosswalk")).AppendChild(Doc.CreateTextNode(CrosswalkType.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("stop_yield")).AppendChild(Doc.CreateTextNode(StopYieldType.ToString()));

            nodeItem.AppendChild(Doc.CreateElement("right_border_line")).AppendChild(Doc.CreateTextNode(RightBorderLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("right_lane_line")).AppendChild(Doc.CreateTextNode(RightLaneLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("divider_line")).AppendChild(Doc.CreateTextNode(DividerLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("divider_line2")).AppendChild(Doc.CreateTextNode(DividerLine2.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("left_lane_line")).AppendChild(Doc.CreateTextNode(LeftLaneLine.ToString()));
            nodeItem.AppendChild(Doc.CreateElement("left_border_line")).AppendChild(Doc.CreateTextNode(LeftBorderLine.ToString()));

            nodeItem.AppendChild(Doc.CreateElement("length")).AppendChild(Doc.CreateTextNode(Length.ToString()));

        }

        /// <summary>
        /// Reads the contents for one StreetElement class instance from an XML document at the specified node and returns the StreetElement object created from that contents.
        /// </summary>
        /// <param name="Doc">Reference to the open XML document object.</param>
        /// <param name="nodeItem">Open node directly to the contents for this object.</param>
        /// <returns>Reference to the StreetElement created from the XML file contents.</returns>
        public static StreetDescriptor LoadFromXml(XmlDocument Doc, XmlNode nodeItem, AppSettings AppSettings)
        {
            try
            {
                int laneCountRight = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_right").InnerText);
                int laneCountCenter = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_center").InnerText);
                int laneCountLeft = Convert.ToInt32(nodeItem.SelectSingleNode("lane_count_left").InnerText);
                CrosswalkType crosswalk = (CrosswalkType)Enum.Parse(typeof(CrosswalkType), nodeItem.SelectSingleNode("crosswalk").InnerText);
                StopYieldType stopYield = (StopYieldType)Enum.Parse(typeof(StopYieldType), nodeItem.SelectSingleNode("stop_yield").InnerText);

                LineType rightBorderLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("right_border_line").InnerText);
                LineType rightLaneLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("right_lane_line").InnerText);
                LineType dividerLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("divider_line").InnerText);
                LineType dividerLine2 = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("divider_line2").InnerText);
                LineType leftLaneLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("left_lane_line").InnerText);
                LineType leftBorderLine = (LineType)Enum.Parse(typeof(LineType), nodeItem.SelectSingleNode("left_border_line").InnerText);

                double length = Convert.ToDouble(nodeItem.SelectSingleNode("length").InnerText);

                StreetDescriptor streetDescriptor = new StreetDescriptor(AppSettings, laneCountRight, laneCountCenter, laneCountLeft, crosswalk, stopYield,
                                                                        rightBorderLine, rightLaneLine, dividerLine, dividerLine2, leftLaneLine, leftBorderLine, length);

                return streetDescriptor;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading XML file:" + ex.Message);
            }
            return null;
        }

        #endregion XML File Support

        #endregion Public Methods

    }

}
