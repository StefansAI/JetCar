// ================================================
//
// SPDX-FileCopyrightText: 2020 Stefan Warnke
//
// SPDX-License-Identifier: BeerWare
//
//=================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageSegmenter
{
    /// <summary>
    /// Class to provide static methods for some general image processing and augmentations. This class requires the project compilation with the checkbox "Allow unsafe code" checked, 
    /// since it uses pointers for faster execution. The direct use of pointer is seen as unsafe in C#, but possible fortunately.
    /// </summary>
    public class Process
    {
        /// <summary>
        /// Blurs an area outlined by a polygon and returns a new Bitmap object with the changes. The blurring itself is controlled by the WindowSize and the StepSize.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object with the image to be blurred</param>
        /// <param name="NormalizedPolygon">Normalized polygon with values between 0 and 1 outlining the area to be blurred.</param>
        /// <param name="WindowSize">Size of the window to be averaged to one pixel value. It can be 15x15, 50x50 or more. Bu the processing time will go up drastically.</param>
        /// <param name="StepSize">The StepSize defines the increments inside the WindowSize and can help reducing the processing time even with large WindowSizes. 
        /// For instance 2x2 will only average every other value in each dimension in the window.</param>
        /// <returns>New Bitmap object with the original image but the blurred area.</returns>
        public static Bitmap BlurArea(Bitmap SourceBitmap, PointF[] NormalizedPolygon, Size WindowSize, Size StepSize)
        {
            // if something is wrong with the polygon, just return a copy of the SourceBitmap
            if ((NormalizedPolygon == null) || (NormalizedPolygon.Length < 3))
                return new Bitmap(SourceBitmap);

            // go half the WindowSize left and right and up and down
            int dx2 = WindowSize.Width / 2;
            int dy2 = WindowSize.Height / 2;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap);
            Bitmap bmDraw = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmDraw).Clear(Color.Black);
            Graphics.FromImage(bmDraw).FillPolygon(new SolidBrush(Color.White), SegmClass.GetDrawPolygon(NormalizedPolygon,SourceBitmap.Size));

            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataDraw = bmDraw.LockBits(new Rectangle(0, 0, bmDraw.Width, bmDraw.Height), ImageLockMode.ReadWrite, bmDraw.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelDraw = System.Drawing.Bitmap.GetPixelFormatSize(bmDraw.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelDraw = (byte*)bitmapDataDraw.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineDraw = PtrFirstPixelDraw + (y * bitmapDataDraw.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        if (currentLineDraw[x * bytesPerPixelDraw] > 0)
                        {
                            int r = 0;
                            int g = 0;
                            int b = 0;
                            int n = 0;

                            for (int ys = Math.Max(y - dy2, 0); ys <= Math.Min(y + dy2, SourceBitmap.Height-1); ys+= StepSize.Height)
                            {
                                byte* currentLineDraw2 = PtrFirstPixelDraw + (ys * bitmapDataDraw.Stride);
                                byte* currentLineSource2 = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                                for (int xs = Math.Max(x - dx2, 0); xs <= Math.Min(x + dx2, SourceBitmap.Width-1); xs+= StepSize.Width)
                                {
                                    if (currentLineDraw2[xs * bytesPerPixelDraw] > 0)
                                    {
                                        byte* pxsSource = &currentLineSource2[xs * bytesPerPixelSource];
                                        b += *pxsSource++;
                                        g += *pxsSource++;
                                        r += *pxsSource++;
                                        n++;
                                    }
                                }
                            }
                            if (n > 0)
                            {
                                byte* pxResult = &currentLineResult[x * bytesPerPixelResult];
                                *pxResult++ = (byte)Math.Min(Math.Max(b / n, 0), 255);      // B
                                *pxResult++ = (byte)Math.Min(Math.Max(g / n, 0), 255);      // G
                                *pxResult++ = (byte)Math.Min(Math.Max(r / n, 0), 255);      // R
                            }
                        }
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmDraw.UnlockBits(bitmapDataDraw);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// Converts a color into a gray value via averaging the color values.
        /// </summary>
        /// <param name="InputColor">Color to be converted.</param>
        /// <returns>Gray value representation of the color.</returns>
        public static int GetGrayValue(Color InputColor)
        {
            return (InputColor.R + InputColor.G + InputColor.B) / 3;
        }

        /// <summary>
        /// Downsamples the input bitmap to the request output size by averaging pixel values together.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be downsampled.</param>
        /// <param name="OutWidth">Width of the downsampled Bitmap</param>
        /// <param name="OutHeight">Height of the downsampled Bitmap</param>
        /// <returns>New Bitmap object downsampled to the requested size.</returns>
        public static Bitmap DownSample(Bitmap SourceBitmap, int OutWidth, int OutHeight)
        {
            // If input size and output size are the same, just return a copy.
            if ((SourceBitmap.Width == OutWidth) && (SourceBitmap.Height == OutHeight))
                return new Bitmap(SourceBitmap);

            // sample steps
            float dx = (float)SourceBitmap.Width / OutWidth;
            float dy = (float)SourceBitmap.Height / OutHeight;

            Bitmap bmResult = new Bitmap(OutWidth, OutHeight);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        int r = 0;
                        int g = 0;
                        int b = 0;
                        int n = 0;

                        for (int ys = Math.Max((int)(y * dy), 0); ys <= Math.Min((int)((y * dy) +dy), SourceBitmap.Height-1); ys++)
                        {
                            byte* currentLineSource = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                            for (int xs = Math.Max((int)(x * dx), 0); xs <= Math.Min((int)((x * dx) + dx), SourceBitmap.Width-1); xs++)
                            {
                                byte* pxsSource = &currentLineSource[xs * bytesPerPixelSource];
                                b += *pxsSource++;
                                g += *pxsSource++;
                                r += *pxsSource++;
                                n++;
                            }
                        }
                        if (n > 0)
                        {
                            byte* pxResult = &currentLineResult[x * bytesPerPixelResult];
                            *pxResult++ = (byte)Math.Min(Math.Max(b / n, 0), 255);      // B
                            *pxResult++ = (byte)Math.Min(Math.Max(g / n, 0), 255);      // G
                            *pxResult++ = (byte)Math.Min(Math.Max(r / n, 0), 255);      // R
                        }

                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }

            return bmResult;
        }

        /// <summary>
        /// Rotate the input Bitmap around the center and apply zoom. The new Bitmap has the exact same size as the input, 
        /// but rotation and zoom will cut of areas outside and fill in black pixel where the original didn't hvae any information.
        /// Since rotation and zoom is based on interpolation, there will be some loss of sharpness in the results.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="TiltAngle">Positve or negative angle in Degrees. Must be in the range from -90 to +90.</param>
        /// <param name="ZoomFactor">Additional enlarging or shrinking of the input.</param>
        /// <returns>New Bitmap object rotated and zoomed.</returns>
        public static Bitmap TiltImage(Bitmap SourceBitmap, int TiltAngle, float ZoomFactor)
        {
            // When tilt is 0 and zoom is 1, just return a copy of the origginal bitmap
            if ((TiltAngle == 0) && (ZoomFactor == 1))
                return new Bitmap(SourceBitmap);

            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            // convert from Tilt in Degree to tilt in Radian
            float tilt = (float)(Math.PI * TiltAngle / 180.0f);
            // This is the point to rotate around
            PointF center = new PointF((SourceBitmap.Width - 1) / 2.0f, (SourceBitmap.Height - 1) / 2.0f);

            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    float dyc = y - center.Y;                               // y distance from center y
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        byte r = 0;                                         // preload r,g,b with 0 for black
                        byte g = 0;
                        byte b = 0;

                        float dxc = x - center.X;                           // x distance from center x
                        float h = (float)Math.Sqrt(dxc * dxc + dyc * dyc);  // length of the hypertenuse and also radius from the center
                        h = h / ZoomFactor;                                 // scaled radius
                        float beta = (float)(Math.Atan(dxc / dyc));         // angle of the current point to the center ccordinates
                        float gamma = beta + tilt;                          // add tilt angle to get coordinates for interpolation
                        float dy = h * (float)Math.Cos(gamma);              // get delta x and delta y from center
                        float dx = h * (float)Math.Sin(gamma);             
                        if (dyc > 0)                                        // inverse to correct in lower half of the image
                        {
                            dx = -dx;
                            dy = -dy;
                        }
                        float xf = center.X - dx;                           // calculate floating point coordinates for interpolation
                        float yf = center.Y - dy;

                        // make sure, only results in the correct bounds are used for interpolations
                        if (((int)xf >= 0) && ((int)xf < SourceBitmap.Width - 2) && ((int)yf >= 0) && ((int)yf < SourceBitmap.Height - 2))
                        {
                            int xn0 = Math.Min(Math.Max((int)xf, 0), SourceBitmap.Width - 1);       // integer conversion provides rounded down coordinates
                            int yn0 = Math.Min(Math.Max((int)yf, 0), SourceBitmap.Height - 1);

                            float dxf = xf - xn0;                                                   // get the fractional parts
                            float dyf = yf - yn0;

                            int xn1 = Math.Min(Math.Max((int)xn0 + 1, 0), SourceBitmap.Width - 1);  // get the upper integer coordinates
                            int yn1 = Math.Min(Math.Max((int)yn0 + 1, 0), SourceBitmap.Height - 1);

                            // now just sample r,g and b of the 4 corner points
                            byte* yn0LineSource = PtrFirstPixelSource + (yn0 * bitmapDataSource.Stride);
                            byte* pxn0Source = &yn0LineSource[xn0 * bytesPerPixelSource];
                            byte b00 = *pxn0Source++;
                            byte g00 = *pxn0Source++;
                            byte r00 = *pxn0Source++;
                            byte* pxn1Source = &yn0LineSource[xn1 * bytesPerPixelSource];
                            byte b01 = *pxn1Source++;
                            byte g01 = *pxn1Source++;
                            byte r01 = *pxn1Source++;

                            byte* yn1LineSource = PtrFirstPixelSource + (yn1 * bitmapDataSource.Stride);
                            pxn0Source = &yn1LineSource[xn0 * bytesPerPixelSource];
                            byte b10 = *pxn0Source++;
                            byte g10 = *pxn0Source++;
                            byte r10 = *pxn0Source++;
                            pxn1Source = &yn1LineSource[xn1 * bytesPerPixelSource];
                            byte b11 = *pxn1Source++;
                            byte g11 = *pxn1Source++;
                            byte r11 = *pxn1Source++;

                            // interpolate inside both lines in x-direction
                            float r0 = r00 * (1 - dxf) + r01 * dxf;
                            float g0 = g00 * (1 - dxf) + g01 * dxf;
                            float b0 = b00 * (1 - dxf) + b01 * dxf;

                            float r1 = r10 * (1 - dxf) + r11 * dxf;
                            float g1 = g10 * (1 - dxf) + g11 * dxf;
                            float b1 = b10 * (1 - dxf) + b11 * dxf;

                            // now interpolate between both line results
                            float rf = (r0 * (1 - dyf) + r1 * dyf);
                            float gf = (g0 * (1 - dyf) + g1 * dyf);
                            float bf = (b0 * (1 - dyf) + b1 * dyf);

                            // limit to valid r,g,b values
                            r = (byte)Math.Min(Math.Max((int)rf, 0), 255);
                            g = (byte)Math.Min(Math.Max((int)gf, 0), 255);
                            b = (byte)Math.Min(Math.Max((int)bf, 0), 255);
                        }

                        // just store the resulting r,g,b values
                        byte* pxResult = &currentLineResult[x * bytesPerPixelResult];
                        *pxResult++ = b;          // B
                        *pxResult++ = g;          // G
                        *pxResult++ = r;          // R
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// Apply tilt and zoom to a list of segmentation classes and return a new list.
        /// </summary>
        /// <param name="SourceSegmClasses">Reference to the original SegmClass list object.</param>
        /// <param name="TiltAngle">Positve or negative angle in Degrees. Must be in the range from -90 to +90.</param>
        /// <param name="ZoomFactor">Additional enlarging or shrinking of the input.</param>
        /// <returns></returns>
        public static List<SegmClass> TiltPolygons(List<SegmClass> SourceSegmClasses, int TiltAngle, float ZoomFactor)
        {
            // if tilt is 0 and zoom is 1, just return the reference to the original list
            if ((TiltAngle == 0) && (ZoomFactor == 1))
                return SourceSegmClasses;

            List<SegmClass> resultList = new List<SegmClass>();         // create the new list for the results
            float tilt = -(float)(Math.PI * TiltAngle / 180.0f);        // convert from Tilt in Degree to tilt in Radian

            for (int i = 0; i < SourceSegmClasses.Count; i++)           // go through the original list and create tilted and zoomed equivalents
            {
                PointF[] newPolygon = new PointF[SourceSegmClasses[i].NormalizedPolygon.Length];
                double centerX = 0.5;                                   // center of normalized coordinates
                double centerY = 0.5;
                for (int j = 0; j < SourceSegmClasses[i].NormalizedPolygon.Length; j++)
                {
                    double dyc = SourceSegmClasses[i].NormalizedPolygon[j].Y - centerY;     // calculate x and y deltas to center
                    double dxc = SourceSegmClasses[i].NormalizedPolygon[j].X - centerX;
                    double h = Math.Sqrt(dxc * dxc + dyc * dyc);                            // calculate hypertenuse or radius
                    h = h * ZoomFactor;                                                     // scale it
                    double beta = Math.Atan(dxc / dyc);                                     // calculate angle from point to center
                    double gamma = beta + tilt;                                             // add tilt
                    double dy = h * Math.Cos(gamma);                                        // calculate new deltas for x and y
                    double dx = h * Math.Sin(gamma);
                    if (dyc > 0)                                                            // correct for lower half of image
                    {
                        dx = -dx;
                        dy = -dy;
                    }
                    float xf = (float)(centerX - dx);                                       // calculate new floating point coordinates
                    float yf = (float)(centerY - dy);
                    newPolygon[j] = new PointF(xf, yf);                                     // and store these
                }
                SegmClass newEntry = new SegmClass(SourceSegmClasses[i].Def, newPolygon);
                resultList.Add(newEntry);
            }
            return resultList;
        }

        /// <summary>
        /// Enhance or reduce brightness of the image.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="BrightnessFactor">Factor to apply to RGB values.</param>
        /// <returns>New Bitmap object reference with applied changes.</returns>
        public static Bitmap ImageBrightness(Bitmap SourceBitmap, float BrightnessFactor)
        {
            // if brightness is 1, just return a copy of the original bitmap
            if (BrightnessFactor == 1)
                return new Bitmap(SourceBitmap);

            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            // just plow through and apply the BrightnessFactor to R, G and B
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bitmapDataSource.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bitmapDataSource.Width; x++)
                    {
                        currentLineResult[x * bytesPerPixelResult] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource] * BrightnessFactor), 0), 255);            // B
                        currentLineResult[x * bytesPerPixelResult + 1] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource + 1] * BrightnessFactor), 0), 255);    // G
                        currentLineResult[x * bytesPerPixelResult + 2] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource + 2] * BrightnessFactor), 0), 255);    // R
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }

            return bmResult;
        }

        /// <summary>
        /// Enhance the contrast of an image. The contrast enhancement uses histogram linearization. Since this algorithm results in very high contrasts, 
        /// the ContrastEnhancement argument is used as a blend value between original and full contrast values.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="ContrastEnhancement">Blend between original and full contrast enhanced version with 0 meaning no enhancement and 1 full enhancement.</param>
        /// <returns>New Bitmap object with enhancements applied.</returns>
        public static Bitmap ImageContrast(Bitmap SourceBitmap, float ContrastEnhancement)
        {
            // if enhancement is 0, just return a copy of the original bitmap
            if (ContrastEnhancement == 0)
                return new Bitmap(SourceBitmap);

            // create a histogram vector
            const int HISTOGRAM_SIZE = 256;
            int[] histogram = new int[HISTOGRAM_SIZE];
            for (int i = 0; i < HISTOGRAM_SIZE; i++)
                histogram[i] = 0;

            // first: build a histogram of gray values
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        int grayValue = (currentLineSource[x * bytesPerPixelSource] + currentLineSource[x * bytesPerPixelSource + 1] + currentLineSource[x * bytesPerPixelSource + 2]) / 3;
                        histogram[grayValue]++;
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
            }

            // accumulate the histogram values
            int accSum = 0;
            int[] accHist = new int[HISTOGRAM_SIZE];
            for (int i = 0; i < HISTOGRAM_SIZE; i++)
            {
                accSum += histogram[i];
                accHist[i] = accSum;
            }

            // scale and convert to factors, histogram linearization normally uses the scaled accumulated histogram as lookup table for B&W values, but we want to apply factors on RGB
            float[] contrastFactors = new float[HISTOGRAM_SIZE];
            for (int i = 1; i < HISTOGRAM_SIZE; i++)
                contrastFactors[i] = ((float)accHist[i] / accSum) * (HISTOGRAM_SIZE - 1) / (float)i;

            float oneMinusContrast = (1 - ContrastEnhancement);
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        // get the gray equivalent value, lookup the factor for full contrast and mix it.
                        int grayValue = (currentLineSource[x * bytesPerPixelSource] + currentLineSource[x * bytesPerPixelSource + 1] + currentLineSource[x * bytesPerPixelSource + 2]) / 3;
                        float fullContrast = contrastFactors[grayValue];
                        float mixFactor = oneMinusContrast + ContrastEnhancement * fullContrast;

                        currentLineResult[x * bytesPerPixelResult] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource] * mixFactor), 0), 255);             // B
                        currentLineResult[x * bytesPerPixelResult + 1] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource + 1] * mixFactor), 0), 255);      // G
                        currentLineResult[x * bytesPerPixelResult + 2] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bytesPerPixelSource + 2] * mixFactor), 0), 255);      // R
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }

            return bmResult;
        }

        /// <summary>
        /// Apply random noise to the original image.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="NoiseAdder">Adds random values in the range from -NoiseAdder to +NoiseAdder to R, G and B.</param>
        /// <returns>New Bitmap object with random noise added.</returns>
        public static Bitmap ImageNoise(Bitmap SourceBitmap, int NoiseAdder)
        {
            // if noise range is 0, just return a copy of the original bitmap
            if (NoiseAdder == 0)
                return new Bitmap(SourceBitmap);

            // seed the time as random number
            Random rnd = new Random(DateTime.Now.Millisecond);
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        currentLineResult[x * bytesPerPixelResult] = (byte)Math.Min(Math.Max(currentLineSource[x * bytesPerPixelSource] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);              // B
                        currentLineResult[x * bytesPerPixelResult + 1] = (byte)Math.Min(Math.Max(currentLineSource[x * bytesPerPixelSource + 1] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);      // G
                        currentLineResult[x * bytesPerPixelResult + 2] = (byte)Math.Min(Math.Max(currentLineSource[x * bytesPerPixelSource + 2] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);      // R
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }

            return bmResult;
        }

        /// <summary>
        /// Converts a bitmap read from a monochrome PNG mask to a color coded RGB bitmap using the segmentation class color definitions.
        /// The available Image.FromFile method reads the monochrome PNG file as 24bit RGB bitmap with the PNG gray value distributed in R, G and B, which is normally ok.
        /// For comparison with the own created masks, these gray images need to be re-colored, using just one of the pixel values as lookup into the segmentation class definition array to extract the DrawColor.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap read from a monochrome PNG mask file.</param>
        /// <param name="SegmClassDefs">Array of segmentation class definitions to access the DrawColor</param>
        /// <param name="Transparency">Transparency value to assign to each pixel.</param>
        /// <returns>New Bitmap object re-colored.</returns>
        public static Bitmap ImageColorMap(Bitmap SourceBitmap, SegmClassDef[] SegmClassDefs, int Transparency)
        {
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);

            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bytesPerPixelSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bytesPerPixelResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bitmapDataSource.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bitmapDataSource.Width; x++)
                    {
                        Color color = Color.White;
                        int id = currentLineSource[x * bytesPerPixelSource];                    // just one of the color values is enough to fetch
                        if (id < SegmClassDefs.Length)
                            color = SegmClassDefs[id].DrawColor;
                        
                        currentLineResult[x * bytesPerPixelResult] = (byte)color.B;             // B
                        currentLineResult[x * bytesPerPixelResult + 1] = (byte)color.G;         // G
                        currentLineResult[x * bytesPerPixelResult + 2] = (byte)color.R;         // R
                        currentLineResult[x * bytesPerPixelResult + 3] = (byte)Transparency;    // A
                    }
                };
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }

            return bmResult;
        }

    }
}
