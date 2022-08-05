//using NUnit.Framework;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BlobTest
{

    internal class Class1
    {
        public static void Invake(Action method,int milliseconds)
        {
            Thread thdToKill = null;
            Action invokemethod = new Action(() =>
            {
                thdToKill = Thread.CurrentThread;
                method();
            }
            );
            IAsyncResult ar = invokemethod.BeginInvoke(null, null);
            if(!ar.AsyncWaitHandle.WaitOne(milliseconds))
            {
                thdToKill?.Abort();
                throw new Exception($"操作失败，原因:超时{milliseconds}毫秒");
            }
            invokemethod.EndInvoke(ar);
        }
   
        internal void Test1()
        {
            Mat src = Cv2.ImRead(@"D:\Image\T231019222660366\POS1.png", ImreadModes.Unchanged);
            //Mat gray = new Mat();
            Mat img_edge = new Mat();
            //Cv2.CvtColor(src, gray, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(src, img_edge, 168, 255, ThresholdTypes.Otsu);

            Mat labels = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();
            int nccomps = Cv2.ConnectedComponentsWithStats(img_edge, labels, stats, centroids);
             //nccomps = Cv2.ConnectedComponents(img_edge, labels);

          Console.WriteLine($"stats.size:{stats.Size()},stats.type:{stats.Type()}");

            //轮廓点提取
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(img_edge, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            //
            Mat img_color4 = Mat.Zeros(img_edge.Size(), MatType.CV_8UC3);
            Cv2.CvtColor(src, img_color4, ColorConversionCodes.GRAY2RGB);

            List<Point[]> points = new List<Point[]>(contours);
            points.Sort((ps1, ps2)=>-ps1.Length.CompareTo(ps2.Length));


            Render_Counter(ref img_color4, points[0]);
            Cv2.ImWrite(@"E:\Opencv TestImage\img_color4.png", img_color4);

            //Console.WriteLine($"nccomps:{nccomps}");

            Vec3b[] colors = new Vec3b[nccomps+1 ];
            colors[0] = new Vec3b(0, 0, 0);
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 1; i <= nccomps; i++)
            {
                colors[i] = new Vec3b((byte)(r.Next(0, 255) % 256), (byte)(r.Next(0, 255) % 256), (byte)(r.Next(0, 255) % 256));
            }
            Mat region = new Mat();
            Cv2.CvtColor(src, region, ColorConversionCodes.GRAY2RGB);

            //for(int h=0; h < centroids.Height; h++)
            //{
            //    double x = centroids.At<double>(h, 0);
            //    double y = centroids.At<double>(h, 1);
            //    Point p=new Point(x, y);
            //    Cv2.Circle(region, p, 20, new Scalar(0, 0, 255));
            //}

            for (int h = 0; h < stats.Height; h++)
            {
                int x = stats.At<int>(h, 0);
                int y = stats.At<int>(h, 1);
                int width = stats.At<int>(h, 2);
                int height = stats.At<int>(h, 3);
                int area = stats.At<int>(h, 4);
                Point p = new Point(x, y);
                Cv2.PutText(region, $"{h}", p, HersheyFonts.Italic, 2, new Scalar(0, 255, 0));
                Console.WriteLine($"index:{h},area:{area}");
                //Cv2.Circle(region, p, 20, new Scalar(0, 0, 255));
                Cv2.Rectangle(region, new Rect(x, y, width, height),new Scalar(0,0,255),2);
            }

            //for (int i = 0; i < nccomps; i++)
            //{
            //    //CC_STAT_AREA
            //    double left = stats.At<int>(i, ConnectedComponentsTypes.Left);
            //    //Cv2.DrawContours
            //}

            List<int> labelnums = new List<int>();

            //Mat img_color = Mat.Zeros(img_edge.Size(), MatType.CV_8UC3);
            //Cv2.CvtColor(img_edge, img_color, ColorConversionCodes.GRAY2BGR);
            //Render(ref img_color, contours, labels, nccomps);
     
            Mat img_color1 = Mat.Zeros(img_edge.Size(), MatType.CV_8UC3);
            Cv2.CvtColor(src, img_color1, ColorConversionCodes.GRAY2RGB);
            for (int y = 0; y < img_edge.Rows; y++)
            {
                for (int x = 0; x < img_edge.Rows; x++)
                {
                    int label = labels.At<int>(y, x);

                    if(label!=0)
                    {
                        img_color1.At<Vec3b>(y, x) = colors[label];
                    }
                 
                    //if (!labelnums.Contains(label))
                    //{
                    //    labelnums.Add(label);
                    //    Cv2.PutText(img_color1, label.ToString(), new Point(x, y), HersheyFonts.Italic, 2, new Scalar(0, 255, 0));
                    //}
                    //Assert.IsTrue(0 <= label && label <= nccomps);
                    //img_color1.At<Vec3b>(y, x) = colors[label];

                }
            }

            Vec3b back_vaec3d = new Vec3b(0, 0, 0);
            Vec3b blob_vaec3d = new Vec3b(255, 255, 255);
            //List<int> ints = new List<int>();
            Mat img_color2 = Mat.Zeros(img_edge.Size(), MatType.CV_8UC3);
            for (int y = 0; y < img_edge.Rows; y++)
            {
                for (int x = 0; x < img_edge.Rows; x++)
                {
                    int label = labels.At<int>(y, x);
                    Vec3b vec3B = new Vec3b(0, 0, 0);
                    //if (!ints.Contains(label))
                    //{
                    //    ints.Add(label);
                    //    //Console.WriteLine($"label:{label}");
                    //}

                    if (label != 0)
                    {
                        vec3B = new Vec3b(255, 255, 255);                      
                    }

                    //Assert.IsTrue(0 <= label && label <= nccomps);
                    img_color2.At<Vec3b>(y, x) = vec3B;

                }
            }

            
            //Cv2.ImWrite(@"E:\Opencv TestImage\img_edge.png", img_edge);
            Cv2.ImWrite(@"E:\Opencv TestImage\region.png", region);
            //Cv2.ImWrite(@"E:\Opencv TestImage\img_color1.png", img_color1);
            //Cv2.ImWrite(@"E:\Opencv TestImage\img_color.png", img_color);
            //Cv2.ImWrite(@"E:\Opencv TestImage\img_color2.png", img_color2);

            //labelnums.Clear();
            Cv2.NamedWindow("原图", WindowFlags.GuiExpanded);
            Cv2.ResizeWindow("原图", 480, 320);
            Cv2.ImShow("原图", src);

            Cv2.NamedWindow("img_color2", WindowFlags.GuiExpanded);
            Cv2.ResizeWindow("img_color2", 480, 320);
            Cv2.ImShow("img_color2", img_color2);

            //Cv2.NamedWindow("img_edge", WindowFlags.Normal);
            //Cv2.ResizeWindow("img_edge", 480, 320);
            //Cv2.ImShow("img_edge", img_edge);

            Cv2.NamedWindow("region", WindowFlags.GuiExpanded);
            Cv2.ResizeWindow("region", 480, 320);
            Cv2.ImShow("region", region);

            //Cv2.NamedWindow("img_edge1", WindowFlags.Normal);
            //Cv2.ResizeWindow("img_edge1", 480, 320);
            //Cv2.ImShow("img_edge1", img_edge1);


            //Cv2.NamedWindow("img_color", WindowFlags.Normal);
            //Cv2.ResizeWindow("img_color", 480, 320);
            //Cv2.ImShow("img_color", img_color);
            //Cv2.ImWrite(@"E:\Opencv TestImage\1.png", img_edge);

            Cv2.WaitKey();
        }

        public static void Render_Counter(ref Mat src, Point[] counter)
        {
            Scalar color = new Scalar(0, 255, 0);
            int count = counter.Length;

            int fx = counter[count - 1].X;
            int fy = counter[count - 1].Y;

            foreach (Point p in counter)
            {
                Cv2.Line(src, fx, fy, p.X, p.Y, color);
                fx = p.X;
                fy = p.Y;
            }

        }

        public static void Render(ref Mat src, Point[][] counters, Mat labels, int nccomps)
        {
            Scalar[] colors = new Scalar[nccomps + 1];
            colors[0] = new Scalar(0, 0, 0);
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 1; i <= nccomps; i++)
            {
                int red = r.Next(255) % 256;
                int green = r.Next(255) % 256;
                int blue = r.Next(255) % 256;
                //if (i < 10)
                //    Console.WriteLine("red:{0},green:{1},blue:{2}", red, green, blue);
                colors[i] = new Scalar(red, green, blue);
            }

            for (int i = 0; i < counters.Length; i++)
            {
                Point[] counter = counters[i];

                int count = counters[i].Length;

                int fx = counter[count - 1].X;
                int fy = counter[count - 1].Y;

                int label = labels.At<int>(fy, fx);

                Scalar color = colors[label];

                //Console.WriteLine("label:{0}", label);

                foreach (Point p in counter)
                {
                    Cv2.Line(src, fx, fy, p.X, p.Y, color);
                    fx = p.X;
                    fy = p.Y;
                }
            }
        }
        public static void Render(Mat src, Point[][] counters)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            Scalar[] colors = new Scalar[counters.Length];
            for (int i = 0; i < counters.Length; i++)
            {
                int red = r.Next(255) % 256;
                int green = r.Next(255) % 256;
                int blue = r.Next(255) % 256;
                //if (i < 10)
                //    Console.WriteLine("red:{0},green:{1},blue:{2}", red, green, blue);
                colors[i] = new Scalar(red, green, blue);
            }

            for (int i = 0; i < counters.Length; i++)
            {
                Point[] counter = counters[i];
                int count = counters[i].Length;
                Scalar color = colors[i];
                int fx = counter[count - 1].X;
                int fy = counter[count - 1].Y;
                foreach (Point p in counter)
                {
                    Cv2.Line(src, fx, fy, p.X, p.Y, color);
                    fx = p.X;
                    fy = p.Y;
                }
            }
        }

        internal void Test()
        {
            Mat src = Cv2.ImRead(@"D:\Image\T231019222660366\POS1.png", ImreadModes.Unchanged);

            SimpleBlobDetector.Params p = new SimpleBlobDetector.Params();

            //阈值控制
            p.ThresholdStep = 5;//搜索阈值步长
            p.MinThreshold = 168;//最小阈值
            p.MaxThreshold = 255;//最大阈值

            p.FilterByColor = true;
            p.BlobColor = 220;
            p.FilterByArea = false;
            p.FilterByCircularity = false;
            p.FilterByConvexity = false;
            p.FilterByInertia = false;

            //p.MinRepeatability = 2;
            //p.MinDistBetweenBlobs = 10;
            //p.FilterByColor = true;

            ////像素面积大小控制
            //p.FilterByArea = true;
            //p.MinArea = 1000;
            ////形状（凸）
            //p.FilterByCircularity = false;
            //p.MinCircularity = 0.7f;
            ////形状（园）
            //p.FilterByInertia = false;
            //p.MinInertiaRatio = 0.5f;


            SimpleBlobDetector detector = SimpleBlobDetector.Create(p);
            //Mat mark= new Mat(src.Rows, src.Cols, src.Type());
            KeyPoint[] keyPoints = detector.Detect(src);
            //Mat dst = new Mat();
            Mat dst = new Mat(src.Size(), MatType.CV_8SC3);
            Cv2.DrawKeypoints(src, keyPoints, dst, new Scalar(0, 0, 255), DrawMatchesFlags.DrawRichKeypoints);

            Cv2.NamedWindow("原图", WindowFlags.Normal);
            Cv2.ResizeWindow("原图", 480, 320);
            Cv2.ImShow("原图", src);

            //奇奇怪怪显示不出来
            //Cv2.NamedWindow("效果图", WindowFlags.Normal);
            //Cv2.ResizeWindow("效果图", 480, 320);
            //Cv2.ImShow("效果图", dst);

            //奇怪的能保存
            Cv2.ImWrite(@"E:\Opencv TestImage\1.png", dst);

            Cv2.WaitKey();
        }
    }
}
