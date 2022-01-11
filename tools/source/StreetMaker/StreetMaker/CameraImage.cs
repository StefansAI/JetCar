//#define DEBUG_WRITE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

#if DEBUG_WRITE
using System.Diagnostics;
#endif

namespace StreetMaker
{
    public class CameraImage
    {
        /// <summary>Horizontal Field of View of the camera lens in Degrees</summary>
        public const double CAMERA_HFOV = 110;
        /// <summary>Vertical Field of View of the camera lens in Degrees.</summary>
        public const double CAMERA_VFOV = CAMERA_HFOV * 3 / 4;
        /// <summary>Optical distortion of the camera lens.</summary>
        public const double CAMERA_LENS_DISTORTION = 0.15;
        /// <summary>Height of the camera above ground.</summary>
        public const double CAMERA_HEIGHT = 100;
        /// <summary>Angle of the optical axis of the camera.</summary>
        public const double CAMERA_AXIS_ANGLE = (CAMERA_VFOV / 2) +15;

        public const double MAX_ALLOWED_ANGLE = Utils.RIGHT_ANGLE_RADIAN - 0.001;

#if DEBUG_WRITE
        public const int OUTPUT_WIDTH = 5;
        public const int OUTPUT_HEIGHT = 5;
        public const int IMAGE_OVERSAMPLING = 1;
#else
        public const int OUTPUT_WIDTH = 224;
        public const int OUTPUT_HEIGHT = 224;
        public const int IMAGE_OVERSAMPLING = 4;
#endif
        public const int IMAGE_WIDTH = OUTPUT_WIDTH * IMAGE_OVERSAMPLING;
        public const int IMAGE_HEIGHT = OUTPUT_WIDTH * IMAGE_OVERSAMPLING;


        public Bitmap ViewBitmap;
        public CameraImage()
        {
            //Calculate();
//            Distortion2();
        }


        public void Calculate2(Bitmap StreetBitmap, PointF P0, double DirectionAngle, double Distortion, double CameraAxisAngle)
        {
#if DEBUG_WRITE
            Debug.WriteLine("");
            Debug.WriteLine("Calculate2 P0:" + P0.ToString() + ", DirectionAngle:" + DirectionAngle.ToString() + ", Distortion:" + Distortion.ToString() + ", CameraAxisAngle:" + CameraAxisAngle.ToString());
#endif
            ViewBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

            double xc = (IMAGE_WIDTH - 1.0) / 2.0;
            double dx_max = (IMAGE_WIDTH - 1.0) / 2.0;

            double yc = (IMAGE_HEIGHT - 1.0) / 2.0;
            double dy_max = (IMAGE_HEIGHT - 1.0) / 2.0;

            double cameraAngle = Utils.ToRadian(CameraAxisAngle);

            double plane_dist = CAMERA_HEIGHT * Math.Cos(cameraAngle);
            double plane_xmax = plane_dist * Math.Tan(Utils.ToRadian(CAMERA_HFOV / 2));
            double plane_ymax = plane_dist * Math.Tan(Utils.ToRadian(CAMERA_VFOV / 2));
            double plane_xstep = plane_xmax / dx_max;
            double plane_ystep = plane_ymax / dy_max;

            double diag = Math.Sqrt(plane_xmax * plane_xmax + plane_ymax * plane_ymax);
            double K1 = (4*Distortion) / (diag * diag);
            double dist_center = Math.Abs(CAMERA_HEIGHT * Math.Tan(cameraAngle));
            double hypo_center = Math.Sqrt(CAMERA_HEIGHT * CAMERA_HEIGHT + dist_center * dist_center);
            PointF pc = Utils.GetPoint(P0, DirectionAngle, -dist_center);

#if DEBUG_WRITE
            Debug.WriteLine("plane_dist:" + plane_dist.ToString("F1") + ", plane_xmax:" + plane_xmax.ToString("F1") + ", plane_ymax:" + plane_ymax.ToString("F1") + ", plane_xstep:" + plane_xstep.ToString("F3") + ", plane_ystep:" + plane_ystep.ToString("F3") + ", dist_center:" + dist_center.ToString("F1"));
#endif
            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                double dy_pixel = y - yc;
                double dy_plane = dy_pixel * plane_ystep;

#if DEBUG_WRITE
                //string s = "dy_pixel:" + dy_pixel.ToString("F1");
                string s = "";
#endif
                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    double dx_pixel = x - xc;
                    double dx_plane = dx_pixel * plane_xstep;
                    double r = Math.Sqrt(dx_plane * dx_plane + dy_plane * dy_plane);

                    double dx_plane_corr = dx_plane / (1 + K1 * r * r );
                    double dy_plane_corr = dy_plane / (1 + K1 * r * r );

                    double dx_angle = Math.Atan(dx_plane_corr / plane_dist);
                    double dy_angle = Math.Atan(dy_plane_corr / plane_dist);

                    double y_angle = Math.Min(dy_angle + cameraAngle, MAX_ALLOWED_ANGLE);
                    double dy_p0 = -CAMERA_HEIGHT * Math.Tan(y_angle);
                    double c = Math.Sqrt(CAMERA_HEIGHT * CAMERA_HEIGHT + dy_p0 * dy_p0);
                    double dx_pc = -c * Math.Tan(dx_angle);

                    double dist_p0 = Utils.GetDistance(0, 0, dx_pc, dy_p0);
                    double angle_p0 = -Utils.GetAngle(0, 0, dx_pc, dy_p0);
                    double dxy_angle = Math.Atan(dist_p0 / CAMERA_HEIGHT);

#if DEBUG_WRITE
                    //s += "   dx_pixel:" + dx_pixel.ToString("F1") + " dx_corr:" + dx_plane_corr.ToString("F1") + " dy_corr:" + dy_plane_corr.ToString("F1");
                    s += "||   dx/dy plane:"+((dx_plane_corr.ToString("F0") + "/").PadLeft(6) + dy_plane_corr.ToString("F0")).PadLeft(12) + "mm";
                    s += "  dx/dy angle:"+((Utils.ToDegree(dx_angle).ToString("F0") + "/").PadLeft(6) + Utils.ToDegree(dy_angle).ToString("F0")).PadLeft(12)+"deg";
                    s += "  y_angle,dxy_angle:" + ((Utils.ToDegree(y_angle).ToString("F0") + ",").PadLeft(6) + Utils.ToDegree(dxy_angle).ToString("F0")).PadLeft(12) + "deg";
                    s += "  dx_pc/dy_p0:" + ((dx_pc.ToString("F0") + "/").PadLeft(6) + dy_p0.ToString("F0")).PadLeft(12) + "mm";
                    s += "  dist_p0:" + (dist_p0.ToString("F0") + "").PadLeft(6) + "mm    ||  ";
#endif
                    Color color = Color.Black;

                    if (Math.Abs(dxy_angle) < MAX_ALLOWED_ANGLE)
                    {
                            //PointF p = Utils.GetPoint(P0, Utils.LimitRadian(DirectionAngle + angle), dist);
                            PointF p = Utils.GetPoint(P0, DirectionAngle + angle_p0, dist_p0);
                        int xx = (int)Math.Round(p.X);
                        int yy = (int)Math.Round(p.Y);
#if DEBUG_WRITE
                        s += "  dxx/dyy:" + (((xx-pc.X).ToString("F0") + "/").PadLeft(6) + (yy-pc.Y).ToString("F0")).PadLeft(12) + "xy";
#endif               
                        if ((xx >= 0) && (xx < StreetBitmap.Width) && (yy >= 0) && (yy < StreetBitmap.Height))
                        {
                            //if ((Math.Abs(xx - pc.X) < 10) && (Math.Abs(yy - pc.Y) < 10))
                            //    color = Color.Yellow;
                            //else if ((Math.Abs(xx - P0.X) < 10) && (Math.Abs(yy - P0.Y) < 10))
                            //    color = Color.Red;
                            //else
                                color = StreetBitmap.GetPixel(xx, yy);
                        }
                        else
                            color = Color.DarkGray;
                    }
                    else color = Color.LightGray;

                    ViewBitmap.SetPixel(x, (IMAGE_HEIGHT - 1) - y, color);
//                    ViewBitmap.SetPixel(x, y, color);
                }
#if DEBUG_WRITE
                Debug.WriteLine(s);
#endif
            }
        }


        public void Calculate2bak(Bitmap StreetBitmap, PointF P0, double DirectionAngle)
        {
            ViewBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

            double K1 = -CAMERA_LENS_DISTORTION / (Utils.ToRadian(CAMERA_HFOV / 2) * Utils.ToRadian(CAMERA_HFOV / 2));
            double dist_max = 3 * Math.Abs(CAMERA_HEIGHT * Math.Tan(Utils.ToRadian(CAMERA_VFOV / 2) + Utils.ToRadian(CAMERA_AXIS_ANGLE)));
            double dist_center = Math.Abs(CAMERA_HEIGHT * Math.Tan(Utils.ToRadian(CAMERA_AXIS_ANGLE)));
            PointF pc = Utils.GetPoint(P0, DirectionAngle, -dist_center);

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                double dy_angle = (y - ((double)(IMAGE_HEIGHT - 1) / 2)) * Utils.ToRadian(CAMERA_VFOV) / (IMAGE_HEIGHT - 1);

                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    double dx_angle = (x - ((double)(IMAGE_WIDTH - 1) / 2)) * Utils.ToRadian(CAMERA_HFOV) / (IMAGE_WIDTH - 1);
                    double r = Math.Sqrt(dx_angle * dx_angle + dy_angle * dy_angle);

                    double dx_angle_d = (dx_angle / (1 + K1 * r * r));
                    double dy_angle_d = (dy_angle / (1 + K1 * r * r));

                    double dy = -CAMERA_HEIGHT * Math.Tan(dy_angle_d + Utils.ToRadian(CAMERA_AXIS_ANGLE));
                    double c = Math.Sqrt(CAMERA_HEIGHT * CAMERA_HEIGHT + dy * dy);
                    double dx = c * Math.Tan(dx_angle_d);

                    double dist = Utils.GetDistance(0, 0, dx, dy);
                    double angle = Utils.GetAngle(0, 0, dx, dy);

                    Color color = Color.Black;

                    if ((dist < dist_max) && ((dy_angle_d + Utils.ToRadian(CAMERA_AXIS_ANGLE)) < Utils.RIGHT_ANGLE_RADIAN) && (Math.Abs(dx_angle_d) < Utils.RIGHT_ANGLE_RADIAN))
                    {
                        PointF p = Utils.GetPoint(P0, Utils.LimitRadian(DirectionAngle + angle), dist);
                        int xx = (int)Math.Round(p.X);
                        int yy = (int)Math.Round(p.Y);
                        if ((xx >= 0) && (xx < StreetBitmap.Width) && (yy >= 0) && (yy < StreetBitmap.Height))
                        {
                            if ((Math.Abs(xx - pc.X) < 10) && (Math.Abs(yy - pc.Y) < 10))
                                color = Color.Yellow;
                            else if ((Math.Abs(xx - P0.X) < 10) && (Math.Abs(yy - P0.Y) < 10))
                                color = Color.Red;
                            else
                                color = StreetBitmap.GetPixel(xx, yy);
                        }
                        else
                            color = Color.DarkGray;
                    }
                    else color = Color.DarkGray;
                    ViewBitmap.SetPixel(x, IMAGE_HEIGHT - y - 1, color);
                }
            }
        }


        private void Distortion2()
        {
            ViewBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

            double K1 = CAMERA_LENS_DISTORTION / (Utils.ToRadian(CAMERA_HFOV/2) * Utils.ToRadian(CAMERA_HFOV/2));
            double dist_max = 10*Math.Abs(CAMERA_HEIGHT * Math.Tan(Utils.ToRadian(CAMERA_VFOV / 2) + Utils.ToRadian(CAMERA_AXIS_ANGLE)));
            double dist_center = Math.Abs(CAMERA_HEIGHT * Math.Tan(Utils.ToRadian(CAMERA_AXIS_ANGLE)));

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                double dy_angle = (y - ((double)(IMAGE_HEIGHT - 1) / 2)) * Utils.ToRadian(CAMERA_VFOV) / (IMAGE_HEIGHT - 1);

                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    double dx_angle = (x - ((double)(IMAGE_WIDTH - 1) / 2)) * Utils.ToRadian(CAMERA_HFOV) / (IMAGE_WIDTH - 1);
                    double r = Math.Sqrt(dx_angle * dx_angle + dy_angle * dy_angle);

                    double dx_angle_d = (dx_angle / (1 + K1 * r * r));
                    double dy_angle_d = (dy_angle / (1 + K1 * r * r));

                    double dy = CAMERA_HEIGHT * Math.Tan(dy_angle_d + Utils.ToRadian(CAMERA_AXIS_ANGLE));
                    double c =  Math.Sqrt(CAMERA_HEIGHT * CAMERA_HEIGHT + dy * dy);
                    double dx = c * Math.Tan(dx_angle_d);

                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    Color color = Color.Black;

                    if ((dist < dist_max) && ((dy_angle_d + Utils.ToRadian(CAMERA_AXIS_ANGLE)) < Utils.RIGHT_ANGLE_RADIAN) && (Math.Abs(dx_angle_d) < Utils.RIGHT_ANGLE_RADIAN))
                    {
                        if (Check(dx, 100))
                            color = Color.White;

                        if (Math.Abs(dist - dist_center) <10)
                            color = Color.Red;
                        //if (Check(dy, 20))
                        //    color = Color.White;

                        if (Math.Abs(y- (IMAGE_WIDTH / 2))<2)
                            color = Color.Blue;
                    }
                    else color = Color.Gray;
                    ViewBitmap.SetPixel(x, IMAGE_HEIGHT - y - 1, color);
                }

            }

        }


        private void Distortion()
        {
            ViewBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

            double xc = (double)(IMAGE_WIDTH - 1) / 2;
            double yc = (double)(IMAGE_HEIGHT - 1) / 2;

            double K1 = 0.15 / (xc * xc);

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                double dy = y - yc;

                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    double dx = x - xc;
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    double xd = xc + (dx / (1 + K1 * r * r));
                    double yd = yc + (dy / (1 + K1 * r * r));

                    Color color = Color.Black;
                    if (Check(xd - xc, 5))
                        color = Color.White;

                    if (Check(yd - yc, 5))
                        color = Color.White;

                    ViewBitmap.SetPixel(x, IMAGE_HEIGHT - y - 1, color);
                }

            }
        }



        private bool Check(double delta, double cycle)
        {
            double dabs = Math.Abs(delta) - (cycle / 2);
            if (dabs < 0)
                return false;

            do
            {
                if (dabs < cycle)
                    return true;
                dabs -= 2 * cycle;
            } while (dabs > 0);

            return false;
        }

        private void Calculate()
        {
            ViewBitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                double alpha = (y - ((double)(IMAGE_HEIGHT - 1) / 2)) * Utils.ToRadian(CAMERA_VFOV) / (IMAGE_HEIGHT - 1) + Utils.ToRadian(CAMERA_AXIS_ANGLE);
                if (alpha < Utils.RIGHT_ANGLE_RADIAN)
                {
                    double dy = CAMERA_HEIGHT * Math.Tan(alpha);
                    double c = Math.Sqrt(CAMERA_HEIGHT * CAMERA_HEIGHT + dy * dy);
                    string s = "y:" + y.ToString() + "  alpha:" + Utils.ToDegree(alpha).ToString("F1") + "  dy:" + dy.ToString("F1") + "  c:" + c.ToString("F1");

                    for (int x = 0; x < IMAGE_WIDTH; x++)
                    {
                        double beta = (x - ((double)(IMAGE_WIDTH - 1) / 2)) * Utils.ToRadian(CAMERA_HFOV) / (IMAGE_WIDTH - 1);
                        double dx = c * Math.Tan(beta);
                        s += ", " + dx.ToString("F1") + "/" + Utils.ToDegree(beta).ToString("F1");

                        Color color = Color.Black;
                        if (Check(dx, 100))
                            color = Color.White;
                        ViewBitmap.SetPixel(x, IMAGE_HEIGHT - y - 1, color);
                    }
                }
                else
                    for (int x = 0; x < IMAGE_WIDTH; x++)
                        ViewBitmap.SetPixel(x, IMAGE_HEIGHT - y - 1, Color.Gray);

            }
        }

    }
}
