using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;

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
        
        //模板匹配 
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var mask = new Image<Gray, Byte>(image1.Size);
                mask.SetValue(255);
                var image2 = image1.InPaint(mask,2);
                pictureBox2.Image = image2.ToBitmap();
            }
        }

        //轮廓
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image4 = image1.Copy();
                var image2 = image1.Convert<Gray,Byte>(); 
                var image3 =  image2.ThresholdAdaptive(new Gray(255), ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                    THRESH.CV_THRESH_BINARY, 199, new Gray(-18));;
                Contour<Point> ContourBufPre = image3.FindContours(
                    CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_EXTERNAL);                         //寻找轮廓
                
                image4.Draw(ContourBufPre, new Bgr(0, 0, 255), new Bgr(255, 0, 255), 2, 1, new Point(0, 0));
                //矩形
                image4.Draw(ContourBufPre.BoundingRectangle, new Bgr(0, 255, 255), 1);
                //最小矩形
                MCvBox2D box = ContourBufPre.HNext.GetMinAreaRect();
                PointF[] pointfs = box.GetVertices();
                Point[] ps = new Point[pointfs.Length];
                for (int i = 0; i < pointfs.Length; i++)
                {
                    ps[i] = new Point((int)pointfs[i].X, (int)pointfs[i].Y);
                    //image4.Draw(new LineSegment2D(ps[i], ps[(i + 1) % 4]), new Bgr(0, 0, 255), 1);
                }
                //for (int i = 0; i < pointfs.Length; i++)
                //{
                //    //ps[i] = new Point((int)pointfs[i].X, (int)pointfs[i].Y);
                //    image4.Draw(new LineSegment2D(ps[i], ps[(i + 1) % 4]), new Bgr(0, 0, 255), 1);
                    
                //}
                image4.DrawPolyline(ps, true, new Bgr(0, 0, 255), 1);
                //圆形
                PointF center;
                float radius;
                CvInvoke.cvMinEnclosingCircle(ContourBufPre.Ptr, out center, out radius);
                image4.Draw(new CircleF(center, radius), new Bgr(255, 0, 255), 2);
                //椭圆               
                if (ContourBufPre.Total >= 6) //轮廓点数小于6，不能创建外围椭圆
                {
                    MCvBox2D box1 = CvInvoke.cvFitEllipse2(ContourBufPre.Ptr);
                    image4.Draw(new Ellipse(box), new Bgr(255, 255, 255), 2);
                }
                pictureBox3.Image = image4.ToBitmap();
            }
        }

        //多边形逼近轮廓
        private void button4_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image4 = image1.Copy();
                var image2 = image1.Convert<Gray,Byte>(); 
                var image3 =  image2.ThresholdAdaptive(new Gray(255), ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C,
                    THRESH.CV_THRESH_BINARY, 199, new Gray(-18));;
                Contour<Point> ContourBufPre = image3.FindContours(
                    CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE, RETR_TYPE.CV_RETR_EXTERNAL);                         //寻找轮廓

                var Contour1 = ContourBufPre.ApproxPoly(0, 0, new MemStorage());
                image4.Draw(Contour1, new Bgr(0, 0, 255), new Bgr(255, 0, 255), 2, 1, new Point(0, 0));
                pictureBox4.Image = image4.ToBitmap();
            }
        }

       
    }
}
