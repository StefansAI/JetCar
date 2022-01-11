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
using System.Drawing.Imaging;



namespace StreetMaker
{
    /// <summary>
    /// A class to create and process virtual camera images as it would been seen from the car camera.
    /// </summary>
    public class VirtualCamera
    {
        /// <summary>The maximum allowed angle up to the horizon when to stop trying to sample the StreetBitmap.</summary>
        public const double MAX_ALLOWED_ANGLE = Utils.RIGHT_ANGLE_RADIAN - 0.001;

        /// <summary>
        /// Result class for the TakeImage method.
        /// </summary>
        public class TakeImgResult
        {
            /// <summary>Virtual camera street view bitmap.</summary>
            public Bitmap StreetView;
            /// <summary>Mask of class codes for the StreetView.</summary>
            public Bitmap ClassMask;
            /// <summary>Colored class image for the StreetView.</summary>
            public Bitmap ClassImg;

            /// <summary>
            /// Creates and fills the result class with the passed references.
            /// </summary>
            /// <param name="StreetView">Virtual camera street view bitmap.</param>
            /// <param name="ClassMask">Mask of class codes for the StreetView.</param>
            /// <param name="ClassImg">Colored class image for the StreetView.</param>
            public TakeImgResult(Bitmap StreetView, Bitmap ClassMask, Bitmap ClassImg)
            {
                this.StreetView = StreetView;
                this.ClassMask = ClassMask;
                this.ClassImg = ClassImg;
            }
        }


        /// <summary>
        /// Take an image as it would be seen from the car camera in the height and angle forward as defined in the AppSettings. 
        /// The wide angle distortion is approximated to give a more realistic view.
        /// </summary>
        /// <param name="AppSettings">Storage class containing all settings for the camera view.</param>
        /// <param name="StreetBitmap">Bitmap of the full street map as input for the street view camera output</param>
        /// <param name="ClassBitmap">Bitmap of the segmentation class codes of the street map as input for the code view.</param>
        /// <param name="P0">Camera position in the StreetBitmap.</param>
        /// <param name="DirectionAngle">Angle of the view direction.</param>
        /// <returns>Array of 3 camera view images for the 3 ColorModes.</returns>
        public static TakeImgResult TakeImage(AppSettings AppSettings, Bitmap StreetBitmap, Bitmap ClassBitmap, PointF P0, double DirectionAngle)
        {
            // Create output bitmaps in oversampling sizes
            int imageWidth = AppSettings.CameraOutputWidth * AppSettings.CameraOversampling;
            int imageHeight = AppSettings.CameraOutputHeight * AppSettings.CameraOversampling;

            int ovrsmplOffs = AppSettings.CameraOversampling / 2;
            int ovrsmplStep = AppSettings.CameraOversampling;

            Bitmap bmStreetOut = new Bitmap(imageWidth, imageHeight);
            Bitmap bmClassOut = new Bitmap(AppSettings.CameraOutputWidth, AppSettings.CameraOutputHeight, PixelFormat.Format8bppIndexed);

            // calculate the center coordinates and ranges for x and y
            double xc = (imageWidth - 1.0) / 2.0;
            double dx_max = (imageWidth - 1.0) / 2.0;

            double yc = (imageHeight - 1.0) / 2.0;
            double dy_max = (imageHeight - 1.0) / 2.0;

            // angle of the camera center in radian
            double cameraAngle = Utils.ToRadian(AppSettings.CameraAxisAngle);

            // create an image plane from the camera point out at the distance where the closest point intersects with the street map plane
            double plane_dist = AppSettings.CameraHeight * Math.Cos(cameraAngle);
            double plane_xmax = plane_dist * Math.Tan(Utils.ToRadian(AppSettings.CameraHFOV / 2));
            double plane_ymax = plane_dist * Math.Tan(Utils.ToRadian((AppSettings.CameraHFOV / AppSettings.CameraImageRatio) / 2));
            double plane_xstep = plane_xmax / dx_max;
            double plane_ystep = plane_ymax / dy_max;

            // prepare for wide angle distortion calculations
            double diag = Math.Sqrt(plane_xmax * plane_xmax + plane_ymax * plane_ymax);
            double k1 = AppSettings.CameraLensDistortion1 / (Math.Pow(plane_xmax, 2));
            double k2 = AppSettings.CameraLensDistortion2 / (Math.Pow(plane_xmax, 4));
            double dist_center = Math.Abs(AppSettings.CameraHeight * Math.Tan(cameraAngle));
            double hypo_center = Math.Sqrt(AppSettings.CameraHeight * AppSettings.CameraHeight + dist_center * dist_center);
            PointF pc = Utils.GetPoint(P0, DirectionAngle, -dist_center);

            // use "unsafe" for faster pointer access rather than slow GetPixel and SetPixel
            unsafe
            {
                // lock the bitmap data once
                BitmapData bmdStreetIn = StreetBitmap.LockBits(new Rectangle(0, 0, StreetBitmap.Width, StreetBitmap.Height), ImageLockMode.ReadOnly, StreetBitmap.PixelFormat);
                BitmapData bmdCodeIn = ClassBitmap.LockBits(new Rectangle(0, 0, ClassBitmap.Width, ClassBitmap.Height), ImageLockMode.ReadOnly, ClassBitmap.PixelFormat);
                BitmapData bmdStreetOut = bmStreetOut.LockBits(new Rectangle(0, 0, bmStreetOut.Width, bmStreetOut.Height), ImageLockMode.ReadWrite, bmStreetOut.PixelFormat);
                BitmapData bmdClassOut = bmClassOut.LockBits(new Rectangle(0, 0, bmClassOut.Width, bmClassOut.Height), ImageLockMode.ReadWrite, bmClassOut.PixelFormat);

                // get pointers for the inputs and outputs
                byte* ptrBaseStreetIn = (byte*)bmdStreetIn.Scan0;
                byte* ptrBaseCodeIn = (byte*)bmdCodeIn.Scan0;
                byte* ptrBaseStreetOut = (byte*)bmdStreetOut.Scan0;
                byte* ptrBaseClassOut = (byte*)bmdClassOut.Scan0;

                // get bytes per pixel values individually, since there can be differences 
                int bppStreetIn = System.Drawing.Bitmap.GetPixelFormatSize(StreetBitmap.PixelFormat) / 8;
                int bppCodeIn = System.Drawing.Bitmap.GetPixelFormatSize(ClassBitmap.PixelFormat) / 8;
                int bppStreetOut = System.Drawing.Bitmap.GetPixelFormatSize(bmStreetOut.PixelFormat) / 8;

                // loop through y coordinates of the camera image
                for (int y = 0; y < imageHeight; y++)
                {
                    // calculate distance from optical axis
                    double dy_pixel = y - yc;
                    // prepare for distortion calculation
                    double dy_plane = dy_pixel * plane_ystep;
                    double dy_plane2 = dy_plane * dy_plane;
                    // Added trick to make it look more like real camera view by creating less distortion closer to camera
                    if (dy_pixel < 0)
                        dy_plane2 /= 4;

                    // calculate pointers to the current output lines
                    byte* ptrLineStreetOut = ptrBaseStreetOut + ((imageHeight - 1) - y) * bmdStreetOut.Stride;
                    byte* ptrLineClassOut = ptrBaseClassOut + ((bmClassOut.Height - 1) - y / ovrsmplStep) * bmdClassOut.Stride;
                    bool sampleY = ((y - ovrsmplOffs) % ovrsmplStep) == 0;

                    // loop through x coordinates of the camera image
                    for (int x = 0; x < imageWidth; x++)
                    {
                        // calculate distance from optical axis
                        double dx_pixel = x - xc;
                        double dx_plane = dx_pixel * plane_xstep;
                        double r = Math.Sqrt(dx_plane * dx_plane + dy_plane2);

                        // calculate distortion values
                        double distortion = k1 * Math.Pow(r, 2) + k2 * Math.Pow(r, 4);
                        double dx_plane_corr = dx_plane / (1 + Math.Max(distortion, -0.99));
                        double dy_plane_corr = dy_plane / (1 + Math.Max(distortion, -0.99));

                        // get the angles from camera point to the virtual plane including distortion
                        double dx_angle = Math.Atan(dx_plane_corr / plane_dist);
                        double dy_angle = Math.Atan(dy_plane_corr / plane_dist);

                        // project down to street map as deltas to current position
                        double y_angle = Math.Min(dy_angle + cameraAngle, MAX_ALLOWED_ANGLE);
                        double dy_p0 = -AppSettings.CameraHeight * Math.Tan(y_angle);
                        double dy_eff = dy_p0 * Math.Cos(dy_angle);
                        double c = Math.Sqrt(AppSettings.CameraHeight * AppSettings.CameraHeight + dy_eff * dy_eff);
                        double dx_pc = -c * Math.Tan(dx_angle);

                        double dist_p0 = Utils.GetDistance(0, 0, dx_pc, dy_p0);
                        double angle_p0 = -Utils.GetAngle(0, 0, dx_pc, dy_p0);
                        double dxy_angle = Math.Atan(dist_p0 / AppSettings.CameraHeight);

                        // preset color RGB values to black 
                        byte[] streetColor = new byte[] { 0, 0, 0 };
                        byte classCode = 0;
                        bool sampleX = ((x - ovrsmplOffs) % ovrsmplStep) == 0;

                        // only angles under the horizon are valid
                        if (Math.Abs(dxy_angle) < MAX_ALLOWED_ANGLE)
                        {
                            // calculate the projection point from the camera down to the ground
                            PointF p = Utils.GetPoint(P0, DirectionAngle + angle_p0, dist_p0);
                            int xx = (int)Math.Round(p.X);
                            int yy = (int)Math.Round(p.Y);

                            // Make sure to hit the street map area
                            if ((xx >= 0) && (xx < StreetBitmap.Width) && (yy >= 0) && (yy < StreetBitmap.Height))
                            {
                                //streetColor = StreetBitmap.GetPixel(xx,yy);
                                byte* ptrPixelStreetIn = ptrBaseStreetIn + (yy * bmdStreetIn.Stride) + (xx * bppStreetIn);
                                streetColor[0] = *ptrPixelStreetIn++;
                                streetColor[1] = *ptrPixelStreetIn++;
                                streetColor[2] = *ptrPixelStreetIn++;

                                if (sampleX && sampleY)
                                {
                                    double limitC = AppSettings.MarkLaneMaxDistSide + (AppSettings.MarkLaneMaxDistFront - AppSettings.MarkLaneMaxDistSide) * Math.Max(Math.Cos(dxy_angle), 0);
                                    if (dist_p0 < limitC)
                                    {
                                        //codeColor = ClassBitmap.GetPixel(xx,yy);
                                        byte* ptrPixelCodeIn = ptrBaseCodeIn + (yy * bmdCodeIn.Stride) + (xx * bppCodeIn);
                                        classCode = *ptrPixelCodeIn++;
                                    }
                                    else
                                        classCode = 0;
                                }
                            }
                            else
                            {
                                // make it gray for areas outside the map and 0 as codes
                                streetColor = new byte[] { 128, 128, 128 };
                                classCode = 0;
                            }
                        }
                        else
                        {
                            // make it gray for areas above the horizon and 0 as codes
                            streetColor = new byte[] { 128, 128, 128 };
                            classCode = 0;
                        }

                        //bmStreetOut.SetPixel(x, (imageHeight - 1) - y, codeColor);
                        byte* ptrPixelStreetOut = ptrLineStreetOut + (x * bppStreetOut);
                        *ptrPixelStreetOut++ = streetColor[0];
                        *ptrPixelStreetOut++ = streetColor[1];
                        *ptrPixelStreetOut++ = streetColor[2];
                        if (bppStreetOut > 3)
                            *ptrPixelStreetOut++ = 255;

                        if (sampleX && sampleY)
                        {
                            //bmClassOut.SetPixel(x, (imageHeight - 1) - y, codeColor);
                            byte* ptrPixelCodeOut = ptrLineClassOut + x / ovrsmplStep;
                            *ptrPixelCodeOut = classCode;
                        }

                    }
                }

                // unlock all bitmap data at the end
                StreetBitmap.UnlockBits(bmdStreetIn);
                ClassBitmap.UnlockBits(bmdCodeIn);
                bmStreetOut.UnlockBits(bmdStreetOut);
                bmClassOut.UnlockBits(bmdClassOut);

            }

            Bitmap bmStreetOutSampled = Process.DownSample(bmStreetOut, AppSettings.CameraOutputWidth, AppSettings.CameraOutputHeight);
            bmStreetOut.Dispose();

            // The 8bit version needs a new color palette lookup table with 1:1 translation, so it's altered with the standard color lookup table
            ColorPalette pal = bmClassOut.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = Color.FromArgb(i, i, i);
            bmClassOut.Palette = pal;

            Bitmap bmClassImg = (Bitmap)bmClassOut.Clone();
            pal = bmClassImg.Palette;
            int j = 0;
            for (int i = 0; i < SegmClassDefs.Defs.Length; i++)
                if (SegmClassDefs.Defs[i].UseCount > 0)
                    pal.Entries[j++] = SegmClassDefs.Defs[i].DrawColor;
            bmClassImg.Palette = pal;

            return new TakeImgResult( bmStreetOutSampled, bmClassOut, bmClassImg );
        }

    }
}
