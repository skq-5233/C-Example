using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Image_Transform
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> image1;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image1 = new Image<Bgr, Byte>(OpenFileDialog1.FileName);
                    pictureBox1.Image = image1.ToBitmap();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("载入文件格式错误！");
                }

            }
        }

        
        //轮廓各种矩的计算 
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var sbContour = new StringBuilder();
                var image2 = image1.Convert<Gray, Byte>();
                var image3 = image2.ThresholdAdaptive(new Gray(255), ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                    THRESH.CV_THRESH_BINARY, 199, new Gray(-18)); ;
                Contour<Point> ContourBufPre = image3.FindContours(
                    CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_EXTERNAL);                         //寻找轮廓
                
                //矩            
                MCvMoments moments = ContourBufPre.GetMoments();
                //遍历各种情况下的矩、中心矩及归一化矩，必须满足条件：xOrder>=0; yOrder>=0; xOrder+yOrder<=3;            
                for (int xOrder = 0; xOrder <= 3; xOrder++)            
                {                
                    for (int yOrder = 0; yOrder <= 3; yOrder++)             
                    {                   
                        if (xOrder + yOrder <= 3)               
                        {                      
                            double spatialMoment = moments.GetSpatialMoment(xOrder, yOrder);             
                            double centralMoment = moments.GetCentralMoment(xOrder, yOrder);  
                            double normalizedCentralMoment = moments.GetNormalizedCentralMoment(xOrder, yOrder);   
                            sbContour.AppendFormat("矩（xOrder：{0}，yOrder：{1}），矩：{2:F09}，中心矩：{3:F09}，归一化矩：{4:F09}\r\n", xOrder, yOrder, spatialMoment, centralMoment, normalizedCentralMoment);   
                        }                
                    }           
                }
                //Hu矩           
                MCvHuMoments huMonents = moments.GetHuMoment();            
                sbContour.AppendFormat("Hu矩 h1：{0:F09}，h2：{1:F09}，h3：{2:F09}，h4：{3:F09}，h5：{4:F09}，h6：{5:F09}，h7：{6:F09}\r\n", huMonents.hu1, huMonents.hu2, huMonents.hu3, huMonents.hu4, huMonents.hu5, huMonents.hu6, huMonents.hu7);
                
            }
        }

        //凸包和凸缺陷
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image4 = image1.Copy();
                var image2 = image1.Convert<Gray, Byte>();
                var image3 = image2.ThresholdAdaptive(new Gray(255), ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                    THRESH.CV_THRESH_BINARY, 199, new Gray(-25));
               
                Contour<Point> ContourBufPre = image3.FindContours(
                    CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_CCOMP);                         //寻找轮廓
                image4.Draw(ContourBufPre, new Bgr(0, 0, 255), new Bgr(255, 0, 255), 2, 1, new Point(0, 0));
                //凸包                
                Seq<Point> convexHull = ContourBufPre.GetConvexHull(ORIENTATION.CV_CLOCKWISE);
                //缺陷                
                Seq<MCvConvexityDefect> defects = ContourBufPre.GetConvexityDefacts(new MemStorage(), ORIENTATION.CV_CLOCKWISE);
                Point[] points = new Point[convexHull.Total];
                for (int i = 0; i < convexHull.Total; i++) 
                { 
                    points[i] = convexHull[i];
                }
                image4.DrawPolyline(points, true, new Bgr(255, 0, 255), 2);
                MCvConvexityDefect defect; 
                for (int i = 0; i < defects.Total; i++) 
                { 
                    defect = defects[i]; 
                }
                pictureBox2.Image = image4.ToBitmap();
            }
        }

        //HU矩匹配
        private void button4_Click(object sender, EventArgs e)
        {
            double matchValue1 = 0, matchValue2=0, matchValue3=0;
            if (image1 != null)
            {
                OpenFileDialog OpenFileDialog2 = new OpenFileDialog();
                OpenFileDialog2.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                if (OpenFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var image2 = new Image<Bgr, Byte>(OpenFileDialog2.FileName);
                        pictureBox3.Image = image2.ToBitmap();
                        Image<Gray, Byte> imageGray1 = image1.Convert<Gray, Byte>();
                        Image<Gray, Byte> imageGray2 = image2.Convert<Gray, Byte>(); 
                        Image<Gray, Byte> imageThreshold1 = imageGray1.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
                        Image<Gray, Byte> imageThreshold2 = imageGray2.ThresholdBinaryInv(new Gray(128d), new Gray(255d));
                        Contour<Point> contour1 = imageThreshold1.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_EXTERNAL);
                        Contour<Point> contour2 = imageThreshold2.FindContours(CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, RETR_TYPE.CV_RETR_EXTERNAL);
                        //HU匹配
                        matchValue1 = contour1.MatchShapes(contour2, CONTOURS_MATCH_TYPE.CV_CONTOUR_MATCH_I1);
                        matchValue2 = contour1.MatchShapes(contour2, CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I2);
                        matchValue3 = contour1.MatchShapes(contour2, CONTOURS_MATCH_TYPE.CV_CONTOURS_MATCH_I3);
                        textBox1.Text = matchValue1.ToString("F2");
                        textBox2.Text = matchValue2.ToString("F2");
                        textBox3.Text = matchValue3.ToString("F2");

                        //轮廓树匹配
                        //IntPtr ptrTree1 = CvInvoke.cvCreateContourTree(contour1.Ptr, new MemStorage().Ptr, 0);          
                        //IntPtr ptrTree2 = CvInvoke.cvCreateContourTree(contour2.Ptr, new MemStorage().Ptr, 0);
                        //double matchValue4 = CvInvoke.cvMatchContourTrees(ptrTree1, ptrTree2, MATCH_CONTOUR_TREE_METHOD.CONTOUR_TREES_MATCH_I1, 1);
                        //textBox4.Text = matchValue4.ToString("F2");

                        //成对几何直方图匹配
                        Rectangle rect1 = contour1.BoundingRectangle;
                        float maxDist1 = (float)Math.Sqrt(rect1.Width * rect1.Width + rect1.Height * rect1.Height); //轮廓的最大距离：这里使用轮廓矩形边界框的对角线长度            
                        int[] bins1 = new int[] { 60, 20 };           
                        RangeF[] ranges1 = new RangeF[] { new RangeF(0f, 180f), new RangeF(0f, maxDist1) };     //直方图第0维为角度，范围在(0,180)，第2维为轮廓两条边缘线段的距离
                        DenseHistogram hist1 = new DenseHistogram(bins1, ranges1);
                        CvInvoke.cvCalcPGH(contour1.Ptr, hist1.Ptr); Rectangle rect2 = contour2.BoundingRectangle;
                        float maxDist2 = (float)Math.Sqrt(rect2.Width * rect2.Width + rect2.Height * rect2.Height); 
                        int[] bins2 = new int[] { 60, 20 };
                        RangeF[] ranges2 = new RangeF[] { new RangeF(0f, 180f), new RangeF(0f, maxDist2) };
                        DenseHistogram hist2 = new DenseHistogram(bins2, ranges2);
                        CvInvoke.cvCalcPGH(contour2.Ptr, hist2.Ptr);
                        double compareResult;
                        //将直方图转换成矩阵                
                        //Matrix<Single> matrix1 = hist1;               
                        //Matrix<Single> matrix2 = FormProcessHist.ConvertDenseHistogramToMatrix(hist2);                
                        //compareResult = CvInvoke.cvCalcEMD2(matrix1.Ptr, matrix2.Ptr, DIST_TYPE.CV_DIST_L2, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);                
                        //matrix1.Dispose();                
                        //matrix2.Dispose();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("载入文件格式错误！");
                    }

                }
            }
        }

       
    }
}
