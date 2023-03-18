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
using System.Drawing.Printing;
using System.Windows.Forms;

namespace StreetMaker
{
    /// <summary>
    /// Class to print the street map. It splits up the large bitmap into pieces of one page to send the complete street map page by page to the printer.
    /// Besides the standard color print mode, a black on white outline test mode can help setting up correct margins or test the geometrical accuracy without wasting colors.
    /// </summary>
    public class PrintStreetMap
    {
        #region Private Fields
        /// <summary>Reference to the StreetMap object used for drawing.</summary>
        private StreetMap StreetMap;
        /// <summary>Reference to the application settings object to get parameter from.</summary>
        private AppSettings AppSettings;

        /// <summary>Total width of the print area with 100 DPI resolution</summary>
        private double totalWidth;
        /// <summary>Total height of the print area with 100 DPI resolution</summary>
        private double totalHeight;

        /// <summary>Scale factor in both dimensions to translate internal bitmaps in mm and 100dpi used by the printer. The conversion takes into account that Bitmaps objects are defaulted to 96dpi.</summary>
        private SizeF scaleFactor;
        /// <summary>Page number counter in x-direction.</summary>
        private int pageX;
        /// <summary>Page number counter in y-direction.</summary>
        private int pageY;

        /// <summary>Printing offset in the virtual large bitmap in x-direction.</summary>
        private double offsX;
        /// <summary>Printing offset in the virtual large bitmap in y-direction.</summary>
        private double offsY;

        private int pageCount;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Reference to the PrintDocument object to print to.</summary>
        public PrintDocument PrintDocument;
        /// <summary>If set to true, only outlines will be printed.</summary>
        public bool OutlinesOnly;
        #endregion Public Fields

        #region Constructor

        /// <summary>
        /// Creates an instance of the PrintStreetMap class for printing. The constructor creates the PrintDocument.
        /// </summary>
        /// <param name="StreetMap">Reference to the StreetMap object used for drawing.</param>
        /// <param name="AppSettings">Reference to the application settings object to get parameter from.</param>
        public PrintStreetMap(StreetMap StreetMap, AppSettings AppSettings, bool OutlinesOnly)
        {
            this.StreetMap = StreetMap;
            this.AppSettings = AppSettings;
            this.OutlinesOnly = OutlinesOnly;

            totalWidth = AppSettings.ToHundredsOfInch(StreetMap.DrawingSize.Width);
            totalHeight = AppSettings.ToHundredsOfInch(StreetMap.DrawingSize.Height);

            scaleFactor = new SizeF((float)(96 / AppSettings.INCH_TO_MM), (float)(96 / AppSettings.INCH_TO_MM));

            PrintDocument = new PrintDocument();
            PrintDocument.DefaultPageSettings = (PageSettings)AppSettings.PrintPageSettings.Clone();
            
            PrintDocument.BeginPrint += PrintDocument_BeginPrint;
            PrintDocument.PrintPage += PrintDocument_PrintPage;
        }
        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Begin Print event handler of the PrintDocument called before starting the print. It is used to reset the page counters and offsets.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PrintDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            pageX = 0;
            pageY = 0;
            offsX = 0;
            offsY = 0;
            pageCount = 0;
        }

        /// <summary>
        /// Print Page event handler of the PrintDocument. Since creating a huge bitmap for the complete StreetMap with 100dpi resolution is not possible, 
        /// a smaller bitmap is created for each page with the origin moved to offsetX and offsetY. The complete StreetMap is printed to it,
        /// so the bitmap will contain the correct partial image.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int w = e.MarginBounds.Width;
            int h = e.MarginBounds.Height;
            int x = e.MarginBounds.X;
            int y = e.MarginBounds.Y;

            Bitmap bmPage = new Bitmap(w, h);
            Graphics grfx = Graphics.FromImage(bmPage);
            grfx.Clear(AppSettings.BackgroundColor);

            grfx.TranslateTransform((float)(-offsX), (float)(-offsY), MatrixOrder.Prepend);
            StreetMap.Draw(grfx, scaleFactor, false);

            if (OutlinesOnly)
            {
                Bitmap bmPage1 = Process.OutlineBW(bmPage, new Size(3, 3), Color.FromArgb(1, 1, 1));
                bmPage.Dispose();
                bmPage = bmPage1;
            }

            e.Graphics.DrawImageUnscaled(bmPage/*bmPageCleaned*/, x, y);
            //e.Graphics.DrawString("Page:" + pageX.ToString() + "." + pageY.ToString(), new Font("System", 8), new SolidBrush(Color.Red), 50, 10);
            //bmPage.Save(Application.StartupPath + "\\Page_" + pageCount.ToString() + ".bmp");
            bmPage.Dispose();

            pageCount++;
            pageX++;
            offsX += w;
            if (offsX >= totalWidth)
            {
                pageX = 0;
                offsX = 0;
                pageY++;
                offsY += h;
            }

            e.HasMorePages = (offsY < totalHeight);
        }
        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Creates and shows the PrintPreviewDialog and assignes it's PrintDocument to it.
        /// </summary>
        public void Preview()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = PrintDocument;
            printPreviewDialog.Show();
        }

        /// <summary>
        /// Prints the StreetMap contents by calling the Print method of the PrintDocument./
        /// </summary>
        public void Print()
        {
            PrintDocument.Print();
        }
        #endregion Public Methods


    }
}
