// ================================================
//
// SPDX-FileCopyrightText: 2021 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;

namespace StreetMaker
{
    /// <summary>
    /// Main form of the Street Maker application containg the menu, tool buttons and the drawing area. All GUI event handler are placed here.
    /// </summary>
    public partial class frmStreetMakerMain : Form
    {
        #region Private Constants
        /// <summary>File name definition for the application settings XML file.</summary>
        private const string APP_SETTINGS_FILE_NAME = "StreetMaker.Cfg";
        #endregion Private Constants

        #region Private Fields
        /// <summary>Mouse coordinates converted to bitmap coordinates.</summary>
        private PointF BitmapCoord;
        /// <summary>Current display zoom factor used as scale factor for drawing to the street bitmap.</summary>
        private SizeF ZoomFactor;
        /// <summary>Minimum limit of the zoom factor.</summary>
        private SizeF ZoomFactorMin;
        /// <summary>Maximum limit of the zoom factor.</summary>
        private SizeF ZoomFactorMax;
        /// <summary>Current street edit mode.</summary>
        private StreetEditMode StreetEditMode;
        /// <summary>Bitmap object to draw the street map on.</summary>
        private Bitmap StreetBitmap;
        /// <summary>Currently pressed key or Keys.None if no key is pressed.</summary>
        private Keys PressedKey;
        /// <summary>Full file name of the last used street map XML file to be used as default for the next save request.</summary>
        private string StreetMapFileName;
        /// <summary>Equivalent of a sempahore counter used in SetEnabled to allow nested calls.</summary>
        private int enableCount;
        /// <summary>Set to true when starting to create a dataset and fals when done or to abort.</summary>
        private bool CreatingDataSet;
        #endregion Private Fields

        #region Public Fields
        /// <summary>Reference to the application settings object holding all customizable parameter.</summary>
        public readonly AppSettings AppSettings;
        /// <summary>Reference to the street map object handling the complete map.</summary>
        public readonly StreetMap StreetMap;
        /// <summary>Reference to the connection issue form.</summary>
        public frmConnectionIssues frmConnectionIssues = null;
        /// <summary>Reference to the intersection or multi lane street settings form.</summary>
        public Form frmStreetElementSettings = null;
        /// <summary>Reference to the Camera View form, if open.</summary>
        public frmCameraView frmCameraView = null;
        #endregion Public Fields

        #region Constructor
        /// <summary>
        /// Creates the instance of this main form of the application.
        /// </summary>
        public frmStreetMakerMain()
        {
            InitializeComponent();
            enableCount = 0;
            CreatingDataSet = false;

            AppSettings = new AppSettings(Application.StartupPath + "\\" + APP_SETTINGS_FILE_NAME);

            ofdLoadStreetMap.InitialDirectory = AppSettings.PathToDataStorage;
            sfdSaveStreetMap.InitialDirectory = AppSettings.PathToDataStorage;

            StreetMap = new StreetMap(ref AppSettings);
            StreetMap.ConnectionIssueChange += StreetMap_ConnectionIssueChange;
            StreetMap.RedrawRequest += StreetMap_RedrawRequest;
            StreetMap.InvalidateRequest += StreetMap_InvalidateRequest;
            StreetMap.DisplayFileName += StreetMap_DisplayFileName;
            StreetMap.NewBitmapsUpdate += StreetMap_NewBitmapsUpdate;

            InitializeDrawing();

            pbDrawingArea.MouseWheel += new MouseEventHandler(pbDrawingArea_MouseWheel);
            pbDrawingArea_MouseWheel(null, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, -1000));
        }


        #endregion Constructor

        #region Private Methods

        #region StreetMap Event Handler
        /// <summary>
        /// Event handler of invalidate request from the StreetMap object. It causes the picture box with the street bitmap to update on the screen.
        /// </summary>
        private void StreetMap_InvalidateRequest()
        {
            pbDrawingArea.Invalidate();
        }

        /// <summary>
        /// Event handler of the redraw request from the StreetMap object. It causes to clear and completely redraw the bitmap.
        /// </summary>
        private void StreetMap_RedrawRequest()
        {
            RedrawStreetBitmap();
        }

        /// <summary>
        /// Event handler indicating the ConnectionIssues list had been changed in the StreetMap object. This handler will bring up or update the frmConnectionIssues.
        /// </summary>
        private void StreetMap_ConnectionIssueChange()
        {
            if (StreetMap.ConnectionIssues.Count > 0)
            {
                bool createNew = frmConnectionIssues == null;
                if (frmConnectionIssues == null)
                    frmConnectionIssues = new frmConnectionIssues(this);
                else
                    frmConnectionIssues.Hide();
               
                frmConnectionIssues.LoadIssues();
                frmConnectionIssues.Show(this);
            }
            else
            {
                if (frmConnectionIssues != null)
                    frmConnectionIssues.Hide();
            }
            StreetMap.SetActiveConnectionIssue(-1);
            pbDrawingArea.Invalidate();
        }

        /// <summary>
        /// Event handler to display the file name in the status bar.
        /// </summary>
        /// <param name="Text">File name to display.</param>
        private void StreetMap_DisplayFileName(string Text)
        {
            string fileName = Path.GetFileName(Text);
            tsslFileName.Text = "Saving " + fileName;
            ssMainStatus.Refresh();
            if (frmCameraView != null)
                frmCameraView.AddFileName(Text);
            Application.DoEvents();
        }

        /// <summary>
        /// Event handler to display the current view bitmaps created.
        /// </summary>
        /// <param name="ViewBitmap">Bitmap of the camera view</param>
        /// <param name="ClassBitmap">Related class color bitmap</param>
        private void StreetMap_NewBitmapsUpdate(Bitmap ViewBitmap, Bitmap ClassBitmap)
        {
            if (frmCameraView != null)
            {
                frmCameraView.BitmapCameraView = new Bitmap(ViewBitmap);
                frmCameraView.BitmapMaskImage = new Bitmap(ClassBitmap);
                frmCameraView.Refresh();
                Application.DoEvents();
            }
        }


        #endregion StreetMap Event Handler

        /// <summary>
        /// Initializes a new clean drawing and defaluts all related parameter. The street bitmap is created to the drawing size and the displa is adjusted to teh size ratios.
        /// </summary>
        private void InitializeDrawing()
        {
            BitmapCoord = new Point(-1, -1);
            StreetEditMode = StreetEditMode.Nothing;
            AdjustDrawingSizes();
            StreetBitmap = new Bitmap((int)StreetMap.DrawingSize.Width, (int)StreetMap.DrawingSize.Height);
            ClearStreetBitMap();
            pbDrawingArea.Image = StreetBitmap;
            ZoomFactorMin = new SizeF((float)(pnDrawingArea.Width / StreetMap.DrawingSize.Width), (float)(pnDrawingArea.Height / StreetMap.DrawingSize.Height));
            ZoomFactor = ZoomFactorMin;
            ZoomFactorMax = new SizeF(4, 4);
            PressedKey = Keys.None;

            pbDrawingArea.Size = pnDrawingArea.Size;
            pbDrawingArea.Location = new Point(0, 0);
        }


        /// <summary>
        /// Event handler to adjust the drawing sizes on the screen and the ZoomFactor.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tscTools_ContentPanel_Resize(object sender, EventArgs e)
        {
            if (AppSettings != null)
            {
                AdjustDrawingSizes();
                ZoomFactorMin = new SizeF((float)(pnDrawingArea.Width / StreetMap.DrawingSize.Width), (float)(pnDrawingArea.Height / StreetMap.DrawingSize.Height));
                ZoomFactor = ZoomFactorMin;
            }
        }

        /// <summary>
        /// Calculates the width/height ratios of the drawing size and the visiable panel size. It then adjusts the panel size and location.
        /// </summary>
        private void AdjustDrawingSizes()
        {
            try
            {
                double drawingRatio = (double)StreetMap.DrawingSize.Width / StreetMap.DrawingSize.Height;
                double panelRatio = (double)tscTools.ContentPanel.Width / tscTools.ContentPanel.Height;

                if (panelRatio > drawingRatio)
                {
                    pnDrawingArea.Height = tscTools.ContentPanel.Height;
                    pnDrawingArea.Width = (int)(pnDrawingArea.Height * drawingRatio);
                    pnDrawingArea.Location = new Point((tscTools.ContentPanel.Width - pnDrawingArea.Width) / 2, 0);
                }
                else
                {
                    pnDrawingArea.Width = tscTools.ContentPanel.Width;
                    pnDrawingArea.Height = (int)(pnDrawingArea.Width / drawingRatio);
                    pnDrawingArea.Location = new Point(0, (tscTools.ContentPanel.Height - pnDrawingArea.Height) / 2);
                }
            }
            catch { }
        }

        /// <summary>
        /// Clears the street bitmap with the background color.
        /// </summary>
        private void ClearStreetBitMap()
        {
            Graphics.FromImage(StreetBitmap).Clear(AppSettings.BackgroundColor);
        }

        /// <summary>
        /// Clears the street bitmap and re-draws the complete street map.
        /// </summary>
        private void RedrawStreetBitmap()
        {
            ClearStreetBitMap();
            StreetMap.AssignDashSyncOrders();
            StreetMap.Draw(Graphics.FromImage(StreetBitmap), new Size(1, 1));
            pbDrawingArea.Invalidate();
        }

        /// <summary>
        /// Paint event handler calls the Paint method of the StreetMap object.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_Paint(object sender, PaintEventArgs e)
        {
            StreetMap.Paint(e.Graphics, ZoomFactor);
        }



        #region Drawing Area Mouse Events

        /// <summary>
        /// PictureBox mouse wheel is used to zoom in and out keeping the current cursor position in the same place.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pbDrawingArea.Image == null) return;

            int idxX = (int)(e.X / ZoomFactor.Width);
            int idxY = (int)(e.Y / ZoomFactor.Height);
            float delta = e.Delta / 500.0f; // 240.0f;
            SizeF zoomLast = ZoomFactor;
            ZoomFactor.Width = Math.Max(Math.Min(ZoomFactor.Width * (1 + delta), ZoomFactorMax.Width), ZoomFactorMin.Width);
            ZoomFactor.Height = Math.Max(Math.Min(ZoomFactor.Height * (1 + delta), ZoomFactorMax.Height), ZoomFactorMin.Height);
            if (ZoomFactor.Width > ZoomFactorMin.Width)
            {
                int width = (int)(pbDrawingArea.Image.Width * ZoomFactor.Width + 0.5f);
                int height = (int)(pbDrawingArea.Image.Height * ZoomFactor.Height + 0.5f);
                int x = Math.Min(pbDrawingArea.Left + e.X - (int)(idxX * ZoomFactor.Width + 0.5f), 0);
                int y = Math.Min(pbDrawingArea.Top + e.Y - (int)(idxY * ZoomFactor.Height + 0.5f), 0);
                pbDrawingArea.Size = new Size(width, height);
                pbDrawingArea.Location = new Point(x, y);
            }
            else
            {
                pbDrawingArea.Size = pnDrawingArea.Size;
                pbDrawingArea.Location = new Point(0, 0);
            }
            tsslLocation.Text = "Zoom:"+ZoomFactor.ToString()+ "  Location: " + pbDrawingArea.Location.ToString();
        }

        /// <summary>
        /// PictureBox mouse down event handler used for different activities depending on left or right mouse button down.
        /// The left mouse button starts or finishes moving or sizing an element. The activities are depending on the current 
        /// StreetEditMode and selected elements. 
        /// In case the mode is currently StreetEditMode.Nothing and the mouse curser selected an ActiveElement (StreetElement),
        /// the mode will change to StreetEditMode.MoveActiveStreetElement or StreetEditMode.SizeActiveStreetElement for the 
        /// following mouse move events. If an overlay had been selected, the mode will change to StreetEditMode.MoveActiveOverlay.
        /// If the StreetEditMode is AddNewStreetElement or AddNewOverlay, the mouse down event finishes up the movement and 
        /// tries to snap if in place when possible.
        /// The right mouse button opens the property page for the active street element.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (StreetEditMode == StreetEditMode.Nothing)
                {
                    if (StreetMap.ActiveElement != null)
                    {
                        if (StreetMap.ActiveElement.SetActiveConnector(BitmapCoord))
                        {
                            StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1), DrawMode.Background);

                            if ((PressedKey == AppSettings.HotkeySizeModeBase) || (PressedKey == AppSettings.HotkeySizeModeExt))
                                StreetEditMode = StreetEditMode.SizeActiveStreetElement;
                            else
                                StreetEditMode = StreetEditMode.MoveActiveStreetElement;
                        }
                    }
                    else if (StreetMap.ActiveOverlay != null)
                    {
                        StreetMap.ActiveOverlay.StartMove(BitmapCoord);
                        StreetEditMode = StreetEditMode.MoveActiveOverlay;
                    }
                }
                else if ((StreetEditMode == StreetEditMode.AddNewStreetElement) && (StreetMap.ActiveElement != null))
                {
                    StreetMap.Add(StreetMap.ActiveElement);
                    bool moveMode = (StreetEditMode == StreetEditMode.AddNewStreetElement) || (StreetEditMode == StreetEditMode.MoveActiveStreetElement);
                    bool sizeMove = (StreetEditMode == StreetEditMode.SizeActiveStreetElement);
                    bool extSizeMode = PressedKey == AppSettings.HotkeySizeModeExt;
                    StreetMap.CheckConnectionSnap(moveMode, sizeMove, extSizeMode);
                    StreetMap.ActiveElement.Draw(Graphics.FromImage(StreetBitmap), new SizeF(1, 1));
                    StreetMap.ActiveElement = null;
                    StreetEditMode = StreetEditMode.Nothing;
                    pbDrawingArea.Invalidate();
                }
                else if ((StreetEditMode == StreetEditMode.SizeActiveStreetElement) && (StreetMap.ActiveElement != null))
                {
                    StreetMap.ActiveElement.SetActiveConnector(BitmapCoord);
                }
                else if ((StreetEditMode == StreetEditMode.AddNewOverlay) && (StreetMap.ActiveOverlay != null))
                {
                    LaneElement le = StreetMap.GetLaneElement(BitmapCoord);
                    if (le != null)
                    {
                        StreetMap.ActiveOverlay.EndMove(le);
                        StreetMap.ActiveOverlay = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        RedrawStreetBitmap();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
                OpenPropertyPage(e.Location);
        }

        /// <summary>
        /// PictureBox mouse move event handler to move or size an active element or overlay.
        /// When moving a active StreetElement the method StreetMap.CheckConnectionDistance is called to 
        /// check and then display possible connections to snap to. Also StreetMap.CheckConnectionRotation 
        /// is called to rotate the active element towards the closest possible connection.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                BitmapCoord = new PointF((e.X / ZoomFactor.Width), (e.Y / ZoomFactor.Height));
                Color color = ((Bitmap)pbDrawingArea.Image).GetPixel((int)BitmapCoord.X, (int)BitmapCoord.Y);
                tsslCursorValues.Text = "X: " + BitmapCoord.X.ToString("F1") + "  Y: " + BitmapCoord.Y.ToString("F1") + " Color: " + color.ToString();
                tsslCursorValues.Invalidate();

                if (((StreetEditMode == StreetEditMode.AddNewStreetElement) || (StreetEditMode == StreetEditMode.MoveActiveStreetElement))&& (StreetMap.ActiveElement != null))
                {
                    StreetMap.ActiveElement.MoveToLocation(BitmapCoord);
                    StreetMap.CheckConnectionDistance();
                    if (PressedKey != AppSettings.HotkeyBlockAutoRotate)
                        StreetMap.CheckConnectionRotation();
                    pbDrawingArea.Invalidate();
                }
                else if ((StreetEditMode == StreetEditMode.SizeActiveStreetElement) && (StreetMap.ActiveElement != null))
                {
                    StreetMap.ActiveElement.Size(AppSettings, StreetMap.DrawingSize, BitmapCoord, PressedKey == AppSettings.HotkeySizeModeExt);
                    StreetMap.CheckConnectionDistance();
                    pbDrawingArea.Invalidate();
                }
                else if (((StreetEditMode == StreetEditMode.AddNewOverlay) || (StreetEditMode == StreetEditMode.MoveActiveOverlay)) && (StreetMap.ActiveOverlay != null))
                {
                    StreetMap.ActiveOverlay.Move(BitmapCoord);
                    pbDrawingArea.Invalidate();
                }
                else if (StreetEditMode == StreetEditMode.Nothing)
                {
                    StreetMap.SelectObject(BitmapCoord, true);
                    pbDrawingArea.Invalidate();
                }
            }
            catch
            {
                tsslCursorValues.Text = "X: " + BitmapCoord.X.ToString("F1") + "  Y: " + BitmapCoord.Y.ToString("F1") + " Color: ----";
            }
        }

        /// <summary>
        /// PictureBox mouse up event handler to finish any active StreetEditMode. If a StreetElement had been moved or sized,
        /// StreetMap.CheckConnectionSnap is called to check for possible connections and snap to them if possible.
        /// In case of an active Overlay move, the overlay will be assigned to the lane it is hovering over.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (StreetMap.ActiveElement != null)
                {
                    bool moveMode = (StreetEditMode == StreetEditMode.AddNewStreetElement) || (StreetEditMode == StreetEditMode.MoveActiveStreetElement);
                    bool sizeMode = (StreetEditMode == StreetEditMode.SizeActiveStreetElement);
                    bool extSizeMode = PressedKey == AppSettings.HotkeySizeModeExt;
                    if (moveMode || sizeMode)
                    {
                        StreetMap.CheckConnectionSnap(moveMode, sizeMode, extSizeMode);
                        StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1));
                        StreetMap.ActiveElement = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        pbDrawingArea.Invalidate();
                    }
                }
                else if (StreetMap.ActiveOverlay != null)
                {
                    if (StreetEditMode == StreetEditMode.MoveActiveOverlay)
                    {
                        LaneElement le = StreetMap.GetLaneElement(BitmapCoord);
                        if (le != null)
                        {
                            StreetMap.ActiveOverlay.EndMove(le);
                            StreetMap.ActiveOverlay = null;
                            StreetEditMode = StreetEditMode.Nothing;
                            RedrawStreetBitmap();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PictureBox mouse leave event handler to turn off any active outline highlighting.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void pbDrawingArea_MouseLeave(object sender, EventArgs e)
        {
            StreetMap.ActiveElement = null;
            StreetMap.ActiveOverlay = null;
            pbDrawingArea.Invalidate();
        }

        #endregion Drawing Area Mouse Events


        #region Hotkey Handling

        /// <summary>
        /// KeyDown handler of the form with frmStreetMakerMain.KeyPreview=true. This allows to capture all key down and key up events on the form level.
        /// All activated key codes are stored into the PressedKey field to process at other places, like in mouse events.
        /// All hot key codes to react to are stored in the related AppSettings fields. ALl key codes, that require immediate responses are handled directly here. 
        /// This includes deleting an element or aborting adding it, increase or decrease the size by one step or rotate an element by one angle step.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void frmStreetMakerMain_KeyDown(object sender, KeyEventArgs e)
        {
            PressedKey = e.KeyCode;

            if (StreetMap.ActiveElement != null)
            {
                if ((e.KeyCode == AppSettings.HotkeyRotateLeft) || (e.KeyCode == AppSettings.HotkeyRotateRight))
                {
                    if (StreetMap.ActiveElement.CanRotate())
                    {
                        if (StreetEditMode != StreetEditMode.AddNewStreetElement)
                            StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1), DrawMode.Background);

                        if (e.KeyCode == AppSettings.HotkeyRotateLeft)
                            StreetMap.ActiveElement.Rotate(AppSettings.AngleStep);
                        else
                            StreetMap.ActiveElement.Rotate(-AppSettings.AngleStep);

                        if (StreetEditMode != StreetEditMode.AddNewStreetElement)
                            StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1));
                    }

                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                }
                else if ((e.KeyCode == AppSettings.HotkeySizeIncrease) || (e.KeyCode == AppSettings.HotkeySizeDecrease))
                {
                    if (e.KeyCode == AppSettings.HotkeySizeIncrease)
                    {
                        StreetMap.ActiveElement.SizeIncreaseStep(AppSettings, StreetMap.DrawingSize);
                        if (StreetEditMode != StreetEditMode.AddNewStreetElement)
                            StreetMap.ActiveElement.Draw(Graphics.FromImage(StreetBitmap), new Size(1, 1));
                        pbDrawingArea.Invalidate();
                    }
                    else
                    {
                        StreetMap.ActiveElement.SizeDecreaseStep(AppSettings);
                        if (StreetEditMode != StreetEditMode.AddNewStreetElement)
                            RedrawStreetBitmap();
                        else
                            pbDrawingArea.Invalidate();
                    }

                    e.Handled = true;
                }
                else if ((e.KeyCode == AppSettings.HotkeyAbort))
                {
                    if (CreatingDataSet == true)
                        tsmiAbort_Click(null, null);
                    else if (StreetEditMode == StreetEditMode.AddNewStreetElement)
                    {
                        StreetMap.ActiveElement = null;
                        StreetMap.ActiveOverlay = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        RedrawStreetBitmap();
                    }
                    else if ((StreetEditMode == StreetEditMode.MoveActiveStreetElement) || (StreetEditMode == StreetEditMode.SizeActiveStreetElement))
                    {
                        StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1), DrawMode.Background);
                        StreetMap.ActiveElement.DrawGroup(Graphics.FromImage(StreetBitmap), new SizeF(1, 1));
                        StreetMap.ActiveElement = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        RedrawStreetBitmap();
                    }
                 
                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                }
                else if (e.KeyCode == AppSettings.HotkeyDelete)
                {
                    if (StreetEditMode == StreetEditMode.Nothing)
                    {
                        StreetElement activeElement = StreetMap.ActiveElement;
                        if (MessageBox.Show("Do you really want to delete this street element from the map?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SetEnabled(false);
                            StreetMap.ActiveElement = activeElement;
                            StreetMap.DeleteActiveElement();
                            StreetEditMode = StreetEditMode.Nothing;
                            // RedrawStreetBitmap();
                            SetEnabled(true);
                        }
                    }
                    e.Handled = true;
                }
                else if (e.KeyCode == AppSettings.HotkeyDisconnect)
                {
                    SetEnabled(false);
                    StreetMap.DisconnectActiveElement();
                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                    SetEnabled(true);
                }
                else if (e.KeyCode == AppSettings.HotkeyPropertyPage)
                {
                    e.Handled = true;
                    PressedKey = Keys.None;
                    PointF p = Utils.Scale(StreetMap.ActiveElement.Lanes[0].Connectors[0].CenterP, ZoomFactor);
                    OpenPropertyPage(new Point((int)p.X, (int)p.Y));
                }
            }
            else if (StreetMap.ActiveOverlay != null)
            {
                if (e.KeyCode == AppSettings.HotkeyRotateLeft)
                {
                    StreetMap.ActiveOverlay.Rotate(AppSettings.AngleStep);
                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                }
                else if (e.KeyCode == AppSettings.HotkeyRotateRight)
                {
                    StreetMap.ActiveOverlay.Rotate(-AppSettings.AngleStep);
                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                }
                else if ((e.KeyCode == AppSettings.HotkeyAbort))
                {
                    if (StreetEditMode == StreetEditMode.AddNewOverlay)
                    {
                        StreetMap.ActiveElement = null;
                        StreetMap.ActiveOverlay = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        RedrawStreetBitmap();
                    }
                    else if (StreetEditMode == StreetEditMode.MoveActiveOverlay)
                    {
                        StreetMap.ActiveOverlay = null;
                        StreetEditMode = StreetEditMode.Nothing;
                        RedrawStreetBitmap();
                    }
                    e.Handled = true;
                    pbDrawingArea.Invalidate();
                }
                else if (e.KeyCode == AppSettings.HotkeyDelete)
                {
                    if (StreetEditMode == StreetEditMode.Nothing)
                    {
                        Overlay activeOverlay = StreetMap.ActiveOverlay;
                        if (MessageBox.Show("Do you really want to delete this overlay element from the map?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            StreetMap.ActiveOverlay = activeOverlay;
                            StreetMap.DeleteActiveElement();
                            StreetEditMode = StreetEditMode.Nothing;
                            //RedrawStreetBitmap();
                        }
                    }
                    e.Handled = true;
                }
            }
            else if (StreetEditMode != StreetEditMode.Nothing)
            {
                if ((e.KeyCode == AppSettings.HotkeyAbort))
                {
                    StreetMap.ActiveElement = null;
                    StreetMap.ActiveOverlay = null;
                    StreetEditMode = StreetEditMode.Nothing;
                }
            }
        }

        /// <summary>
        /// KeyUp handler of the form with frmStreetMakerMain.KeyPreview=true. This allows to capture all key down and key up events on the form level.
        /// This handler only resets the PressedKey field to Keys.None, so mouse events won't use any already released key code.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void frmStreetMakerMain_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKey = Keys.None;
        }

        /// <summary>
        /// Opens a form showing the properties of the element and allowing to change those.
        /// There are 2 different descendents of the StreetElement class, the MultiLaneStreet class and the Intersection class.
        /// This method checks the StreetMap.ActiveElement and creates the settings form for one or the other.
        /// </summary>
        /// <param name="Position">Currnt mouse position used to place the form close to that.</param>
        private void OpenPropertyPage(Point Position)
        {
            if (StreetMap.ActiveElement != null)
            {
                if (frmStreetElementSettings != null)
                    frmStreetElementSettings.Close();

                frmStreetElementSettings = null;
                if (StreetMap.ActiveElement is Intersection)
                    frmStreetElementSettings = new frmIntersectionSettings(this, (StreetMap.ActiveElement as Intersection));
                else
                    frmStreetElementSettings = new frmMultiStreetSettings(this, (StreetMap.ActiveElement as MultiLaneStreet));

                Point p = pbDrawingArea.PointToScreen(Position);
                int dx = Math.Max((p.X + frmStreetElementSettings.Width) - (Screen.FromPoint(Position).Bounds.Width - 64), 0);
                int dy = Math.Max((p.Y + frmStreetElementSettings.Height) - (Screen.FromPoint(Position).Bounds.Height - 64), 0);
                frmStreetElementSettings.Location = new Point(p.X - dx, p.Y - dy);
                frmStreetElementSettings.Show();
            }
    }
        #endregion Hotkey Handling

        #region Menu Handler
        /// <summary>
        /// ToolStripMenuItem "New Map" click event handler to bring up the frmNewMap allowing the user to change the drawing size for a new map.
        /// In result the StreetMap object is re-initialized with the new drawing size and the form's drawing parameter and bitmap are initialized accordingly.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiNewMap_Click(object sender, EventArgs e)
        {
            frmNewMap frmNewMap = new frmNewMap(StreetMap.DrawingSize, AppSettings.DisplayMeasurementUnit);
            if (frmNewMap.ShowDialog() == DialogResult.OK)
            {
                StreetMap.Init(frmNewMap.DrawingSize);
                InitializeDrawing();
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Load" click event handler to bring up the open file dialog and then load the StreetMap object with the contents of the selected XML file.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiLoad_Click(object sender, EventArgs e)
        {
            if (ofdLoadStreetMap.ShowDialog() == DialogResult.OK)
            {
                SetEnabled(false);
                if (StreetMap.LoadFromXml(ofdLoadStreetMap.FileName))
                {
                    InitializeDrawing();
                    RedrawStreetBitmap();
                }
                StreetMapFileName = ofdLoadStreetMap.FileName;
                SetEnabled(true);
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Save" click event handler to save the current StreetMap contents to the StreetMapFileName. 
        /// If StreetMapFileName is empty, the SaveAs event hanadler is called.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiSave_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            if ((StreetMapFileName != null) && (File.Exists(StreetMapFileName)))
                StreetMap.SaveToXml(StreetMapFileName);
            else
                tsmiSaveAs_Click(sender, e);
            SetEnabled(true);
        }

        /// <summary>
        /// ToolStripMenuItem "Save As" click event handler to open up the save file dialog an then save the current StreetMap contents to the newly assigned StreetMapFileName. 
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiSaveAs_Click(object sender, EventArgs e)
        {
            if (sfdSaveStreetMap.ShowDialog() == DialogResult.OK)
            {
                SetEnabled(false);
                StreetMap.SaveToXml(sfdSaveStreetMap.FileName);
                StreetMapFileName = sfdSaveStreetMap.FileName;
                SetEnabled(true);
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Print" click event handler to open the print dialog and print the street map to the selected printer.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiPrintSeup_Click(object sender, EventArgs e)
        {
            //if (pageSetupDialog1.PageSettings == null)
            //    pageSetupDialog1.PageSettings = new PageSettings();

            if (pageSetupDialog1.PrinterSettings == null)
                pageSetupDialog1.PrinterSettings = new PrinterSettings();

            pageSetupDialog1.PageSettings = (PageSettings)AppSettings.PrintPageSettings.Clone();

            if (pageSetupDialog1.ShowDialog() == DialogResult.OK)
            {
                AppSettings.PrintPageSettings = (PageSettings)pageSetupDialog1.PageSettings.Clone();
                AppSettings.SaveSettings();
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Print" click event handler to open the print dialog and print the street map to the selected printer.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiPrint_Click(object sender, EventArgs e)
        {
            PrintStreetMap printMap = new PrintStreetMap(StreetMap, AppSettings, sender == tsmiTestPrint);
            pdPrintStreepMap.Document = printMap.PrintDocument;

            if (pdPrintStreepMap.ShowDialog() == DialogResult.OK)
            {
                SetEnabled(false);
                printMap.Print();
                SetEnabled(true);
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Reconnect All" click event handler to call the StreetMap.ReconnectAll method 
        /// that will try to connect all connectors that are close enough to eachother.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiReconnectAll_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            StreetMap.ReconnectAll();
            SetEnabled(true);
        }

        /// <summary>
        /// ToolStripMenuItem "Clear All" click event handler to clear the StreetMap contents completely and update the street bitmap after confirmation.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiClearAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete all?", "Confirmation") == DialogResult.OK)
            {
                SetEnabled(false);
                StreetMap.Clear();
                RedrawStreetBitmap();
                SetEnabled(true);
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Redraw" click event handler to redraw the street bitmap completely new.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiRedraw_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            RedrawStreetBitmap();
            SetEnabled(true);
        }

        /// <summary>
        /// ToolStripMenuItem "Settings" click event handler to open the AppSettings form for editing.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiSettings_Click(object sender, EventArgs e)
        {
            frmAppSettingsSetup frmAppSettingsSetup = new frmAppSettingsSetup(this);
            if (frmAppSettingsSetup.ShowDialog() == DialogResult.OK)
                AppSettings.SaveSettings();
        }

        /// <summary>
        /// ToolStripMenuItem "Show Item Numbers" click event handler to enable or disable showing streetmap item numbers.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiShowItemNumbers_CheckStateChanged(object sender, EventArgs e)
        {
            StreetMap.DrawItemNumbers = tsmiShowItemNumbers.Checked;
            pbDrawingArea.Invalidate();
        }

        /// <summary>
        /// ToolStripMenuItem "Show Lane Numbers" click event handler to enable or disable showing streetmap lane numbers.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiShowLaneNumbers_CheckStateChanged(object sender, EventArgs e)
        {
            StreetMap.DrawLaneNumbers = tsmiShowLaneNumbers.Checked;
            pbDrawingArea.Invalidate();
        }

        /// <summary>
        /// ToolStripMenuItem "Create Dataset" click event handler to start creating virtual images and masks as dataset for Jupyter notebook.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiCreateDataset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to start the very time consuming process of creating a new DataSet deleting any previous one?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SetEnabled(false);
                tsmiRedraw_Click(null, null);
                if (frmCameraView != null)
                    frmCameraView.Close();
                frmCameraView = new frmCameraView(this);
                frmCameraView.Show();
                tsmiAbort.Visible = true;
                StreetMap.GenerateDataset(ref CreatingDataSet, AppSettings.PathToDataStorage, StreetBitmap);
                tsmiAbort.Visible = false;
                tsslFileName.Text = "";
                if (frmCameraView != null)
                {
                    frmCameraView.ColorPalette = StreetMap.GetCurrentColorMap();
                    frmCameraView.SwitchToNavigation();
                }
                SetEnabled(true);
            }
        }

        /// <summary>
        /// ToolStripMenuItem "Display Test And Pred" click event handler to display previously created camera view images and class code masks in the test folders and available predictions.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiDisplayTestAndPred_Click(object sender, EventArgs e)
        {
            if (frmCameraView != null)
                frmCameraView.Close();
            
            frmCameraView = new frmCameraView(this, new string[] { AppSettings.SubDirTest });
            frmCameraView.Show();
        }

        /// <summary>
        /// ToolStripMenuItem "Display Train and Val" click event handler to display previously created camera view images and class code masks in the train and val sub folders.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiDisplayTrainVal_Click(object sender, EventArgs e)
        {
            if (frmCameraView != null)
                frmCameraView.Close();

            frmCameraView = new frmCameraView(this, new string[] { AppSettings.SubDirTrain, AppSettings.SubDirVal });
            frmCameraView.Show();
        }


        /// <summary>
        /// ToolStripMenuItem "Display Train" click event handler to display previously created camera view images and class code masks in the train sub folders.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiDisplayTrain_Click(object sender, EventArgs e)
        {
            if (frmCameraView != null)
                frmCameraView.Close();

            frmCameraView = new frmCameraView(this, new string[] { AppSettings.SubDirTrain });
            frmCameraView.Show();
        }

        /// <summary>
        /// ToolStripMenuItem "Display Val" click event handler to display previously created camera view images and class code masks in the val sub folders.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiDisplayVal_Click(object sender, EventArgs e)
        {
            if (frmCameraView != null)
                frmCameraView.Close();

            frmCameraView = new frmCameraView(this, new string[] { AppSettings.SubDirVal });
            frmCameraView.Show();
        }

        /// <summary>
        /// ToolStripMenuItem "Abort" click event handler to abort a running dataset creation.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsmiAbort_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to abort creating the DataSet?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                CreatingDataSet = false;

            if (frmCameraView != null)
                frmCameraView.BringToFront();
        }

        #endregion Menu Handler

        #region Add New Elements

        #region Add Straight Elements
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one straight lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }


        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one straight lane each direction with a yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLaneYellowDashedLine_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 1, LineType.SingleYellowDashed);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one straight lane each direction with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLaneYellowSingleLine_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1,0, 1, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one straight lane each direction with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLaneDoubleYellowLine_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1,0, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }


        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two straight lanes each direction with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLanesDoubleYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two straight lanes each direction and one center lane with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLanesCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 2, 1, 2, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Straight Elements

        #region Add Curve Right Elements
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one curved lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneCurveRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 1, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one curved lane each direction with a yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveRightDashedYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 1, 0, 1, LineType.SingleYellowDashed);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one curved lane each direction with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveRightSolidYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 1, 0, 1, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with just one curved lane each direction with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveRightDoubleYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 1, 0, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two curved lanes each direction with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneCurveRightDoubleYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two curved lanes each direction and a center lane with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneCurveRightCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveRight, 2, 1, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        #endregion Add Curve Right Elements

        #region Add Curve Left Elements

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one curved lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneCurveLeft_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 1, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one curved lane each direction with a yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveLeftDashedYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 1, 0, 1, LineType.SingleYellowDashed);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one curved lane each direction with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveLeftSolidYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 1, 0, 1, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one curved lane each direction with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneCurveLeftDoubleYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 1, 0, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two curved lanes each direction with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneCurveLeftDoubleYellow_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one curved lanes each direction and a center lane with a double yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneCurveLeftCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.CurveLeft, 2, 1, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Curve Left Elements

        #region Add S-Shaped Elements
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one s-shaped lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneSRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Right, 1, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one s-shaped lane each direction with a yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneSRightYellowSolid_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Right, 1, 0, 1, LineType.SingleYellowDashed);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }


        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two s-shaped lane each direction with a double yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneSRightYellowSolid_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Right, 2, 0, 2, LineType.SingleYellowDashed);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one s-shaped lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneSLeft_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Left, 1, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one s-shaped lane each direction with a yellow solid line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneSLeftYellowSolid_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Left, 1, 0, 1, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two s-shaped lane each direction with a double yellow dashed line in between.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneSLeftYellowSolid_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.S_Left, 2, 0, 2, LineType.SingleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        #endregion Add S-Shaped Elements

        #region Add Lane Split Elements
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two opposite s-shaped lanes split with a lane space in the middle.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneSplit_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Split, 1, 0, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two opposite s-shaped lanes split adding a center lane in the middle.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneSplitCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Split, 1, 1, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two by two opposite s-shaped lanes split with a lane space in the middle.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneSplit_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Split, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }

        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two by two opposite s-shaped lanes split adding a center lane in the middle.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneSplitCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Split, 2, 1, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Lane Split Elements

        #region Add Lane Union Elements
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two opposite s-shaped lanes with a lane space in the middle coming together.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneUnion_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Union, 1, 0, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two opposite s-shaped lanes with a center lane in the middle coming together losing the center lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneUnionCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Union, 1, 1, 1, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two by two opposite s-shaped lanes with a lane space in the middle coming together.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneUnion_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Union, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two by two opposite s-shaped lanes with a center lane in the middle coming together losing the center lane.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaaneUnionCenterLane_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Center_Union, 2, 1, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Lane Union Elements

        #region Add Lane Split Elements and Unions
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object splitting one lane into two, one straight and one s-shaped.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneSplitRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Lane_Split_Right, 2, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object joining two lanes into one, one straight and one s-shaped.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneUnion_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Lane_Union_Right, 2, 0, 0, LineType.ShoulderLine);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object splitting one lane into two in one direction and joining them in the other.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualToQuadDoubleYellowLine_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Lane_Split_Both, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object joining two lanes into one in one direction and spiltting them in the other.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadToDualDoubleYellowLine_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Lane_Union_Both, 2, 0, 2, LineType.DoubleYellowSolid);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Lane Split Elements and Unions

        #region Add On/Off Ramps
        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one straight lane and splitting off a curved lane like a highway exit or off-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneExitRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 0, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.ShoulderLine, LineType.ShoulderLine, 
                    LineType.SingleWhiteDashed,LineType.ShoulderLine, AppSettings.MinInnerRadius,RampType.ExitRamp, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with one straight lane and joining a curved lane like a highway entrance or on-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbSingleLaneJunctionRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 0, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.ShoulderLine, LineType.ShoulderLine,
                    LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius, RampType.Entrance, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two straight lanes and splitting off a curved lane like a highway exit or off-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneExitRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 1, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.SingleYellowSolid, LineType.SingleYellowSolid,
                    LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius, RampType.ExitRamp, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with two straight lane and joining a curved lane like a highway entrance or on-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDualLaneJunctionRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 1, 0, 1, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.SingleYellowSolid, LineType.SingleYellowSolid,
                    LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius, RampType.Entrance, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with four straight lanes and splitting off a curved lane like a highway exit or off-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneExitRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 2, 0, 2, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.SingleYellowSolid, LineType.SingleYellowSolid,
                    LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius, RampType.ExitRamp, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new MultiLaneStreet object with four straight lanes and joining a curved lane like a highway entrance or on-ramp.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbQuadLaneJunctionRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new MultiLaneStreet(AppSettings, StreetType.Straight, 2, 0, 2, LineType.ShoulderLine, LineType.SingleWhiteDashed, LineType.SingleYellowSolid, LineType.SingleYellowSolid,
                    LineType.SingleWhiteDashed, LineType.ShoulderLine, AppSettings.MinInnerRadius, RampType.Entrance, AppSettings.DefaultRampRadius, AppSettings.DefaultRampCurveAngle);
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        #endregion Add On/Off Ramps

        #region Add Intersection Elements

        /// <summary>
        /// ToolStripButton click event handler to create a new Intersection object as a T-intersection type with one street going through and a side street joining.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLaneTintersection_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new Intersection(AppSettings, new StreetDescriptor[] {
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.None, LineType.SingleWhiteDashed, LineType.SingleYellowDashed),     //LineType.SingleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.SingleYellowSolid) });
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new Intersection object as a 3 way intersection with all 3 streets meeting in the center.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLane3wayIntersection_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new Intersection(AppSettings, new StreetDescriptor[] {
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),     
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid) });
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new Intersection object as a 4 way intersection with all 4 streets meeting in the center.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLane4wayIntersection_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new Intersection(AppSettings, new StreetDescriptor[] {
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid) });
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new Roundabout object.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbDoubleLaneRoundabout_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveElement = new Intersection(AppSettings, new StreetDescriptor[] {
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid),
                    new StreetDescriptor(AppSettings, 1, 0, 1, CrosswalkType.None, StopYieldType.StopLineText, LineType.SingleWhiteSolid, LineType.DoubleYellowSolid) });
                StreetEditMode = StreetEditMode.AddNewStreetElement;
            }
        }
        #endregion Add Intersection Elements

        #endregion Add New Elements

        #region Add Overlays
        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow straight and left only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowStraightLeft_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowStraightLeft);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow straight only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowStraightOnly_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowStraightOnly);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow straight and right only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowStraightRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowStraightRight);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow left only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowLeftOnly_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowLeftOnly);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow left and right only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowLeftRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowLeftRight);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow right only.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbArrowRightOnly_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowRightOnly);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow merge left.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbMergeLeft_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowMergeLeft);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as prking sign.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbParking_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ParkingSign);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }
        }

        /// <summary>
        /// ToolStripButton click event handler to create a new overlay object as an arrow merge right.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void tsbMergeRight_Click(object sender, EventArgs e)
        {
            if (StreetEditMode == StreetEditMode.Nothing)
            {
                StreetMap.ActiveOverlay = new Overlay(AppSettings, OverlayType.ArrowMergeRight);
                StreetEditMode = StreetEditMode.AddNewOverlay;
            }

        }


        #endregion Add Overlays

        #endregion Private Methods

        #region Public Methods
        /// <summary>
        /// Method to enable or disable the GUI elements of this form and switch the cursor between default and wait cursor.
        /// Calling this method can be nested, since enableCount keeps track of the number of disable calls and enable calls. 
        /// Only the last enable call will really enable again.
        /// </summary>
        /// <param name="Enable">If true, the GUI should be enabled.</param>
        public void SetEnabled(bool Enable)
        {
            if (Enable)
                enableCount = Math.Max(--enableCount, 0);
            else
                enableCount++;

            bool enabled = enableCount == 0;
            tsmiFile.Enabled = enabled;
            tsmiEdit.Enabled = enabled;
            tsmiView.Enabled = enabled;
            tsmiProcess.Enabled = enabled;
            tscTools.Enabled = enabled;

            if (enabled)
                Cursor = Cursors.Default;
            else
                Cursor = Cursors.WaitCursor;
        }


        #endregion Public Methods

    }
}
