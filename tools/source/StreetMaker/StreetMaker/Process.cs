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
    /// Collection of static image processing functions used in the application.
    /// </summary>
    public class Process
    {

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
                BitmapData bmdSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bmdResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bppSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bppResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;

                byte* ptrBaseSource = (byte*)bmdSource.Scan0;
                byte* ptrBaseResult = (byte*)bmdResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineResult = ptrBaseResult + (y * bmdResult.Stride);
                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        int r = 0;
                        int g = 0;
                        int b = 0;
                        int n = 0;

                        for (int ys = Math.Max((int)(y * dy), 0); ys <= Math.Min((int)((y * dy) + dy), SourceBitmap.Height - 1); ys++)
                        {
                            byte* currentLineSource = ptrBaseSource + (ys * bmdSource.Stride);
                            for (int xs = Math.Max((int)(x * dx), 0); xs <= Math.Min((int)((x * dx) + dx), SourceBitmap.Width - 1); xs++)
                            {
                                byte* pxsSource = &currentLineSource[xs * bppSource];
                                b += *pxsSource++;
                                g += *pxsSource++;
                                r += *pxsSource++;
                                n++;
                            }
                        }
                        if (n > 0)
                        {
                            byte* pxResult = &currentLineResult[x * bppResult];
                            *pxResult++ = (byte)Math.Min(Math.Max(b / n, 0), 255);      // B
                            *pxResult++ = (byte)Math.Min(Math.Max(g / n, 0), 255);      // G
                            *pxResult++ = (byte)Math.Min(Math.Max(r / n, 0), 255);      // R
                        }

                    }
                };
                SourceBitmap.UnlockBits(bmdSource);
                bmResult.UnlockBits(bmdResult);
            }

            return bmResult;
        }



        /// <summary>
        /// Enhance or reduce brightness of the image with one factor applied to all three colors equally.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="BrightnessFactor">Factor to apply to RGB values.</param>
        /// <param name="CenterRange">If true, the min/max range will be centered to 127.</param>
        /// <returns>New Bitmap object reference with applied changes.</returns>
        public static Bitmap ImageBrightness(Bitmap SourceBitmap, float BrightnessFactor, bool CenterRange)
        {
            return ImageBrightness(SourceBitmap, new float[] { BrightnessFactor, BrightnessFactor, BrightnessFactor }, CenterRange);
        }

        /// <summary>
        /// Enhance or reduce brightness of the image with individual RGB factors.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="BrightnessFactorsRGB">Factors to apply to RGB values.</param>
        /// <param name="CenterRange">If true, the min/max range will be centered to 127.</param>
        /// <returns>New Bitmap object reference with applied changes.</returns>
        public static Bitmap ImageBrightness(Bitmap SourceBitmap, float[] BrightnessFactorsRGB, bool CenterRange)
        {
            // if brightness is 1, just return a copy of the original bitmap
            if ((BrightnessFactorsRGB[0] == 1) && (BrightnessFactorsRGB[1] == 1) && (BrightnessFactorsRGB[2] == 1))
                return new Bitmap(SourceBitmap);

            int offs = 0;
            if (CenterRange == true )
            {
                float factAvg = (BrightnessFactorsRGB[0] + BrightnessFactorsRGB[1] + BrightnessFactorsRGB[2]) / 3;
//                if (factAvg < 1)
                    offs = (int)(127 * (1 - factAvg));
            }

            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);
            Graphics.FromImage(bmResult).Clear(Color.Black);

            // just plow through and apply the BrightnessFactor to R, G and B
            unsafe
            {
                BitmapData bmdSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bmdResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bppSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bppResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* ptrBaseSource = (byte*)bmdSource.Scan0;
                byte* ptrBaseResult = (byte*)bmdResult.Scan0;

                for (int y = 0; y < bmdSource.Height; y++)
                {
                    byte* currentLineSource = ptrBaseSource + (y * bmdSource.Stride);
                    byte* currentLineResult = ptrBaseResult + (y * bmdResult.Stride);
                    for (int x = 0; x < bmdSource.Width; x++)
                    {
                        currentLineResult[x * bppResult    ] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bppSource    ] * BrightnessFactorsRGB[2]) + offs, 0), 255);    // B
                        currentLineResult[x * bppResult + 1] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bppSource + 1] * BrightnessFactorsRGB[1]) + offs, 0), 255);    // G
                        currentLineResult[x * bppResult + 2] = (byte)Math.Min(Math.Max((int)(currentLineSource[x * bppSource + 2] * BrightnessFactorsRGB[0]) + offs, 0), 255);    // R
                    }
                };
                SourceBitmap.UnlockBits(bmdSource);
                bmResult.UnlockBits(bmdResult);
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
                BitmapData bmdSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bmdResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bppSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bppResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* ptrBaseSource = (byte*)bmdSource.Scan0;
                byte* ptrBaseResult = (byte*)bmdResult.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineSource = ptrBaseSource + (y * bmdSource.Stride);
                    byte* currentLineResult = ptrBaseResult + (y * bmdResult.Stride);
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        currentLineResult[x * bppResult] = (byte)Math.Min(Math.Max(currentLineSource[x * bppSource] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);              // B
                        currentLineResult[x * bppResult + 1] = (byte)Math.Min(Math.Max(currentLineSource[x * bppSource + 1] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);      // G
                        currentLineResult[x * bppResult + 2] = (byte)Math.Min(Math.Max(currentLineSource[x * bppSource + 2] + rnd.Next(-NoiseAdder, NoiseAdder), 0), 255);      // R
                    }
                };
                SourceBitmap.UnlockBits(bmdSource);
                bmResult.UnlockBits(bmdResult);
            }

            return bmResult;
        }


        /// <summary>
        /// Converts a bitmap read from a monochrome PNG mask to a color coded RGB bitmap using the segmentation class color definitions.
        /// The available Image.FromFile method reads the monochrome PNG file as 24bit RGB bitmap with the PNG gray value distributed in R, G and B, which is normally ok.
        /// For comparison with the own created masks, these gray images need to be re-colored, using just one of the pixel values as lookup into the segmentation class definition array to extract the DrawColor.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap read from a monochrome PNG mask file.</param>
        /// <param name="ColorPalette">Color palette to be used to color the class codes.</param>
        /// <param name="Transparency">Transparency value to assign to each pixel.</param>
        /// <returns>New Bitmap object re-colored.</returns>
        public static Bitmap ImageColorMap(Bitmap SourceBitmap, Color[] ColorPalette, int Transparency)
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
                        if (id < ColorPalette.Length)
                            color = ColorPalette[id];

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

        /// <summary>
        /// Creates an eroded binary bitmap from the source bitmap. The erosion is controlled by the WindowSize. The output pixel is 0, if the majority of pixel in the window is zero.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object. It must be 8 bit.</param>
        /// <returns>New Bitmap object containing an eroded mask.</returns>
        public static Bitmap CreateBitmapMask(Bitmap SourceBitmap)
        {
            if (SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                return null;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height, PixelFormat.Format8bppIndexed);
            SetGrayScale(ref bmResult);

            int xc = (SourceBitmap.Width - 1) / 2;
            int dx = 0;

            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    dx++;

                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        if ((x < (xc - dx / 2)) || (x > xc + dx / 2))
                        {
                            if (currentLineSource[x] == 0)
                                currentLineResult[x] = 0;
                            else
                                currentLineResult[x] = 255;
                        }
                        else
                            currentLineResult[x] = 0;
                    }
                }
                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }


        /// <summary>
        /// Creates an eroded binary bitmap from the source bitmap. The erosion is controlled by the WindowSize. The output pixel is 0, if the majority of pixel in the window is zero.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object. It must be 8 bit.</param>
        /// <param name="WindowSize">Size of the window for the erode operation.</param>
        /// <returns>New Bitmap object containing an eroded mask.</returns>
        public static Bitmap BitmapErode(Bitmap SourceBitmap, Bitmap MaskBitmap, Size WindowSize)
        {
            if (SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                return null;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height, PixelFormat.Format8bppIndexed);
            SetGrayScale(ref bmResult);

            // go half the WindowSize left and right and up and down
            int dx2 = WindowSize.Width / 2;
            int dy2 = WindowSize.Height / 2;

            bool low;

            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataMask = MaskBitmap.LockBits(new Rectangle(0, 0, MaskBitmap.Width, MaskBitmap.Height), ImageLockMode.ReadWrite, MaskBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelMask = (byte*)bitmapDataMask.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineMask = PtrFirstPixelMask + (y * bitmapDataMask.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);

                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        if (currentLineMask[x] == 0)
                            currentLineResult[x] = 255;
                        else
                        {
                            low = false;
                            for (int ys = Math.Max(y - dy2, 0); ys <= Math.Min(y + dy2, SourceBitmap.Height - 1); ys++)
                            {
                                byte* currentLineSource = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                                for (int xs = Math.Max(x - dx2, 0); xs <= Math.Min(x + dx2, SourceBitmap.Width - 1); xs++)
                                {
                                    if (currentLineSource[xs] == 0)
                                    {
                                        low = true;
                                        break;
                                    }
                                }
                                if (low)
                                    break;
                            }
                            if (low)
                                currentLineResult[x] = 0;
                            else
                                currentLineResult[x] = 255;
                        }
                    }
                }
                SourceBitmap.UnlockBits(bitmapDataSource);
                MaskBitmap.UnlockBits(bitmapDataMask);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }


        /// <summary>
        /// Creates an dilated binary bitmap from the source bitmap. The dilation is controlled by the WindowSize. The output pixel is 255, if the majority of pixel in the window is not zero.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object. It must be 8 bit.</param>
        /// <param name="WindowSize">Size of the window for the dilate operation.</param>
        /// <returns>New Bitmap object containing an dilation mask.</returns>
        public static Bitmap BitmapDilate(Bitmap SourceBitmap, Bitmap MaskBitmap, Size WindowSize)
        {
            if (SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                return null;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height, PixelFormat.Format8bppIndexed);
            SetGrayScale(ref bmResult);

            // go half the WindowSize left and right and up and down
            int dx2 = WindowSize.Width / 2;
            int dy2 = WindowSize.Height / 2;

            bool high;
            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataMask = MaskBitmap.LockBits(new Rectangle(0, 0, MaskBitmap.Width, MaskBitmap.Height), ImageLockMode.ReadWrite, MaskBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelMask = (byte*)bitmapDataMask.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineMask = PtrFirstPixelMask + (y * bitmapDataMask.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);

                    for (int x = 0; x < bmResult.Width; x++)
                    {
                        if (currentLineMask[x] == 0)
                            currentLineResult[x] = 255;
                        else
                        {
                            high = false;
                            for (int ys = Math.Max(y - dy2, 0); ys <= Math.Min(y + dy2, SourceBitmap.Height - 1); ys++)
                            {
                                byte* currentLineSource = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                                for (int xs = Math.Max(x - dx2, 0); xs <= Math.Min(x + dx2, SourceBitmap.Width - 1); xs++)
                                {
                                    if (currentLineSource[xs] != 0)
                                    {
                                        high = true;
                                        break;
                                    }
                                }
                                if (high)
                                    break;
                            }

                            if (high)
                                currentLineResult[x] = 255;
                            else
                                currentLineResult[x] = 0;
                        }
                    }
                }
                SourceBitmap.UnlockBits(bitmapDataSource);
                MaskBitmap.UnlockBits(bitmapDataMask);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// Creates an dilated binary bitmap from the source bitmap. The dilation is controlled by the WindowSize. The output pixel is 255, if the majority of pixel in the window is not zero.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object. It must be 8 bit.</param>
        /// <returns>New Bitmap object containing an dilation mask.</returns>
        public static Bitmap BitmapAnd(Bitmap SourceBitmap, Bitmap MaskBitmap)
        {
            if ((SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed) || (MaskBitmap.PixelFormat != PixelFormat.Format8bppIndexed) ||
                (SourceBitmap.Width != MaskBitmap.Width) || (SourceBitmap.Height != MaskBitmap.Height))
                return null;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height, PixelFormat.Format8bppIndexed);
            SetGrayScale(ref bmResult);

            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataMask = MaskBitmap.LockBits(new Rectangle(0, 0, MaskBitmap.Width, MaskBitmap.Height), ImageLockMode.ReadWrite, MaskBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelMask = (byte*)bitmapDataMask.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineMask = PtrFirstPixelMask + (y * bitmapDataMask.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bmResult.Width; x++)
                        currentLineResult[x] = (byte)(currentLineSource[x] & currentLineMask[x]);
                }
                SourceBitmap.UnlockBits(bitmapDataSource);
                MaskBitmap.UnlockBits(bitmapDataMask);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// Creates an dilated binary bitmap from the source bitmap. The dilation is controlled by the WindowSize. The output pixel is 255, if the majority of pixel in the window is not zero.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object. It must be 8 bit.</param>
        /// <returns>New Bitmap object containing an dilation mask.</returns>
        public static Bitmap BitmapDiff(Bitmap SourceBitmap, Bitmap MaskBitmap)
        {
            if ((SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed) || (MaskBitmap.PixelFormat != PixelFormat.Format8bppIndexed) ||
                (SourceBitmap.Width != MaskBitmap.Width) || (SourceBitmap.Height != MaskBitmap.Height))
                return null;

            // The draw bitmap serves the purpose of a mask, with the polygon drawn in there. White pixels will enable the blurring.
            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height, PixelFormat.Format8bppIndexed);
            SetGrayScale(ref bmResult);

            // prepare all pointers and just plow through
            unsafe
            {
                BitmapData bitmapDataSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bitmapDataMask = MaskBitmap.LockBits(new Rectangle(0, 0, MaskBitmap.Width, MaskBitmap.Height), ImageLockMode.ReadWrite, MaskBitmap.PixelFormat);
                BitmapData bitmapDataResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                byte* PtrFirstPixelSource = (byte*)bitmapDataSource.Scan0;
                byte* PtrFirstPixelMask = (byte*)bitmapDataMask.Scan0;
                byte* PtrFirstPixelResult = (byte*)bitmapDataResult.Scan0;

                for (int y = 0; y < bmResult.Height; y++)
                {
                    byte* currentLineSource = PtrFirstPixelSource + (y * bitmapDataSource.Stride);
                    byte* currentLineMask = PtrFirstPixelMask + (y * bitmapDataMask.Stride);
                    byte* currentLineResult = PtrFirstPixelResult + (y * bitmapDataResult.Stride);
                    for (int x = 0; x < bmResult.Width; x++)
                        currentLineResult[x] = (byte)Math.Min(Math.Max(128 + (currentLineSource[x] - currentLineMask[x]) * 16, 0), 255);
                }
                SourceBitmap.UnlockBits(bitmapDataSource);
                MaskBitmap.UnlockBits(bitmapDataMask);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }


        /// <summary>
        /// This function performs a gradient window operator to extract the outline contours as black on white image. The window operator is performed seperately 
        /// for each color to seperate contours of different colors but same gray levels.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object with the image to be ouylined</param>
        /// <param name="WindowSize">Size of the window of the gradient operator. The processing time will go up drastically with larger window sizes.</param>
        /// <param name="Threshold">Threshold values for each color element. A pixel is set to black if one of the color gradient results is above its color threshold.</param>
        /// <returns>New Bitmap object with the outlines from the original image.</returns>
        public static Bitmap OutlineBW(Bitmap SourceBitmap, Size WindowSize, Color Threshold)
        {
            // if something is wrong with the polygon, just return a copy of the SourceBitmap
            if ((WindowSize.Width < 3) || (WindowSize.Height < 3))
                return new Bitmap(SourceBitmap);

            // go half the WindowSize left and right and up and down
            int dx2 = WindowSize.Width / 2;
            int dy2 = WindowSize.Height / 2;

            // create result bitmap from source 
            Bitmap bmResult = new Bitmap(SourceBitmap);
            Graphics.FromImage(bmResult).Clear(Color.White);

            // prepare all pointers and just plow through
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
                        int r = 0, rc = 0;
                        int g = 0, gc = 0;
                        int b = 0, bc = 0;
                        int n = 0;

                        // inner loop for window operator
                        for (int ys = Math.Max(y - dy2, 0); ys <= Math.Min(y + dy2, SourceBitmap.Height - 1); ys += 1)
                        {
                            byte* currentLineSource2 = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                            for (int xs = Math.Max(x - dx2, 0); xs <= Math.Min(x + dx2, SourceBitmap.Width - 1); xs += 1)
                            {
                                if ((x == xs) && (y == ys))
                                {
                                    byte* pxsSource = &currentLineSource2[xs * bytesPerPixelSource];
                                    bc = *pxsSource++;
                                    gc = *pxsSource++;
                                    rc = *pxsSource++;
                                }
                                else
                                {
                                    byte* pxsSource = &currentLineSource2[xs * bytesPerPixelSource];
                                    b += *pxsSource++;
                                    g += *pxsSource++;
                                    r += *pxsSource++;
                                    n++;
                                }
                            }
                        }
                        if (n > 1)
                        {
                            r = Math.Abs(r - n * rc);
                            g = Math.Abs(g - n * gc);
                            b = Math.Abs(b - n * bc);
                            if ((r > Threshold.R) || (g > Threshold.G) || (b > Threshold.B))
                            {
                                byte* pxResult = &currentLineResult[x * bytesPerPixelResult];
                                *pxResult++ = 0;      // B
                                *pxResult++ = 0;      // G
                                *pxResult++ = 0;      // R
                            }
                        }

                    }
                }

                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// BubbleSort algorithm to rearrange the color array in order of the brightness. This method is used for Median operator.
        /// </summary>
        /// <param name="Colors">An array of colors to be sorted.</param>
        /// <param name="N">Maximum number of valid entries in the color array.</param>
        private static void BubbleSort(ref Color[] Colors, int N)
        {
            for (int n = Math.Min(Colors.Length, N) - 1; n > 0; n--)
                for (int i = 0; i < n; i++)
                {
                    if (Colors[i].GetBrightness() > Colors[i + 1].GetBrightness())
                    {
                        Color c = Colors[i];
                        Colors[i] = Colors[i + 1];
                        Colors[i + 1] = c;
                    }
                }
        }

        /// <summary>
        /// This function performs a median window operator to smooth over small defects. A 3x3 window takes out up to 4 pixels out of place.
        /// </summary>
        /// <param name="SourceBitmap">Reference to the original Bitmap object with the image to be ouylined</param>
        /// <param name="WindowSize">Size of the window of the gradient operator. The processing time will go up drastically with larger window sizes.</param>
        /// <returns>New Bitmap object with the outlines from the original image.</returns>
        public static Bitmap Median(Bitmap SourceBitmap, Size WindowSize)
        {
            // if something is wrong with the polygon, just return a copy of the SourceBitmap
            if ((WindowSize.Width < 3) || (WindowSize.Height < 3))
                return new Bitmap(SourceBitmap);

            // go half the WindowSize left and right and up and down
            int dx2 = WindowSize.Width / 2;
            int dy2 = WindowSize.Height / 2;

            Color[] colors = new Color[WindowSize.Width * WindowSize.Height];
            // create result bitmap from source 
            Bitmap bmResult = new Bitmap(SourceBitmap);
            Graphics.FromImage(bmResult).Clear(Color.White);

            // prepare all pointers and just plow through
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

                        int n = 0;

                        // inner loop for window operator
                        for (int ys = Math.Max(y - dy2, 0); ys <= Math.Min(y + dy2, SourceBitmap.Height - 1); ys += 1)
                        {
                            byte* currentLineSource2 = PtrFirstPixelSource + (ys * bitmapDataSource.Stride);
                            for (int xs = Math.Max(x - dx2, 0); xs <= Math.Min(x + dx2, SourceBitmap.Width - 1); xs += 1)
                            {
                                byte* pxsSource = &currentLineSource2[xs * bytesPerPixelSource];
                                int b = *pxsSource++;
                                int g = *pxsSource++;
                                int r = *pxsSource++;
                                colors[n] = Color.FromArgb(r, g, b);
                                n++;
                            }
                        }
                        if (n > 8)
                        {
                            BubbleSort(ref colors, n);
                            byte* pxResult = &currentLineResult[x * bytesPerPixelResult];
                            *pxResult++ = colors[n / 2].B;      // B
                            *pxResult++ = colors[n / 2].G;      // G
                            *pxResult++ = colors[n / 2].R;      // R
                        }
                    }
                }

                SourceBitmap.UnlockBits(bitmapDataSource);
                bmResult.UnlockBits(bitmapDataResult);
            }
            return bmResult;
        }

        /// <summary>
        /// Set the color palette conetents of the bitmap to a grey scale palette.
        /// </summary>
        /// <param name="SourceBitmap">Bitmap object to assign the grey scale palette to</param>
        public static void SetGrayScale(ref Bitmap SourceBitmap)
        {
            if (SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                return;

            ColorPalette pal = SourceBitmap.Palette;
            for (int i = 0; i < 256; i++)
                pal.Entries[i] = Color.FromArgb(i, i, i);
            SourceBitmap.Palette = pal;
        }


        /// <summary>
        /// Creates a copy with the transparency changed to the requested one.
        /// </summary>
        /// <param name="SourceBitmap">Source Bitmap to be processed.</param>
        /// <param name="Transparency">Transparency to set to.</param>
        /// <returns>New Bitmap object with set transparency.</returns>
        public static Bitmap SetTransparency(Bitmap SourceBitmap, byte Transparency)
        {

            Bitmap bmResult = new Bitmap(SourceBitmap.Width, SourceBitmap.Height);

            unsafe
            {
                BitmapData bmdSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);
                BitmapData bmdResult = bmResult.LockBits(new Rectangle(0, 0, bmResult.Width, bmResult.Height), ImageLockMode.ReadWrite, bmResult.PixelFormat);

                int bppSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                int bppResult = System.Drawing.Bitmap.GetPixelFormatSize(bmResult.PixelFormat) / 8;
                byte* ptrBaseSource = (byte*)bmdSource.Scan0;
                byte* ptrBaseResult = (byte*)bmdResult.Scan0;

                for (int y = 0; y < SourceBitmap.Height; y++)
                {
                    byte* currentLineSource = ptrBaseSource + (y * bmdSource.Stride);
                    byte* currentLineResult = ptrBaseResult + (y * bmdResult.Stride);
                    for (int x = 0; x < SourceBitmap.Width; x++)
                    {
                        currentLineResult[x * bppResult] = currentLineSource[x * bppSource];              // B
                        currentLineResult[x * bppResult + 1] = currentLineSource[x * bppSource + 1];      // G
                        currentLineResult[x * bppResult + 2] = currentLineSource[x * bppSource + 2];      // R
                        currentLineResult[x * bppResult + 3] = (byte)Transparency;  
                    }
                };
                SourceBitmap.UnlockBits(bmdSource);
                bmResult.UnlockBits(bmdResult);
            }

            return bmResult;
        }

        /// <summary>
        /// Gets the byte value of the SourceBitmap at the location of x,y.
        /// </summary>
        /// <param name="SourceBitmap">SourceBitmap in PixelFormat Format8bppIndexed</param>
        /// <param name="x">x-coordinate of the pixel to read.</param>
        /// <param name="y">y-coordinate of the pixel to read.</param>
        /// <returns>Byte valye read from the bitmap coordinates x,y</returns>
        public static byte Get8bitValue(Bitmap SourceBitmap, int x, int y)
        {
            if (SourceBitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new Exception("Get8bitValue  SourceBitmap.PixelFormat has to be Format8bppIndexed!");

            if ((x<0) || (x>= SourceBitmap.Width) || (y < 0) || (y >= SourceBitmap.Height))
                throw new Exception("Get8bitValue x,y parameter out of range");

            byte value=0;
            unsafe
            {
                BitmapData bmdSource = SourceBitmap.LockBits(new Rectangle(0, 0, SourceBitmap.Width, SourceBitmap.Height), ImageLockMode.ReadWrite, SourceBitmap.PixelFormat);

                int bppSource = System.Drawing.Bitmap.GetPixelFormatSize(SourceBitmap.PixelFormat) / 8;
                byte* ptrBaseSource = (byte*)bmdSource.Scan0;
                byte* currentLineSource = ptrBaseSource + (y * bmdSource.Stride);
                value = currentLineSource[x * bppSource];
                SourceBitmap.UnlockBits(bmdSource);
            }
            return value;
        }

    }
}
