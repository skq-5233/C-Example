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

namespace Image_Manage2
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

        //漫水填充算法
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image2 = image1.Copy();
                MCvConnectedComp mo = new MCvConnectedComp();
                CvInvoke.cvFloodFill(image2, new Point(100, 100), new MCvScalar(0, 255, 0), new MCvScalar(7, 7, 7), new MCvScalar(7, 7, 7), out mo, 4, IntPtr.Zero);
                CvInvoke.cvFloodFill(image2, new Point(100, 300), new MCvScalar(0, 0, 255), new MCvScalar(7, 7, 7), new MCvScalar(7, 7, 7), out mo, 4, IntPtr.Zero);
                pictureBox2.Image = image2.ToBitmap();
            }
        }


        //尺寸调整
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image3 = image1.Resize(400, 200, INTER.CV_INTER_LINEAR);
                pictureBox3.Image = image3.ToBitmap();
                image3.Save("E:\\李志\\C#Test\\图片\\gougou1.jpg");
            }
        }

        //图像金字塔UP
        private void button4_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image4 = image1.PyrUp();
               // CvInvoke.cvPyrUp(image1, image4,FILTER_TYPE.CV_GAUSSIAN_5x5);
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        //图像金字塔Down
        private void button5_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image4 = image1.PyrDown();
                // CvInvoke.cvPyrUp(image1, image4,FILTER_TYPE.CV_GAUSSIAN_5x5);
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        //拉普拉斯金字塔 （图像的宽度和高度必须能被2整除）
        private void button6_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                image1.ROI = new Rectangle(0, 0, 400, 320);
                Image<Bgr, Byte> image4 = image1.Copy();
                image4.ROI = image1.ROI;
                var storage = new MemStorage();
                IntPtr comp;
                CvInvoke.cvPyrSegmentation(image1, image4, storage, out comp, 1, 100, 50);
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        //阈值处理
        private void button7_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                //var image5 = image1.Copy();
                //image1.ROI = new Rectangle(30, 30, 100, 100);
                //image5.ROI = image1.ROI;
                //CvInvoke.cvThreshold(image1, image5, 180, 255, THRESH.CV_THRESH_BINARY);
                //image1.ROI = Rectangle.Empty;
                //image5.ROI = Rectangle.Empty;
                //var image5 = image1.ThresholdBinary(new Bgr(180, 180, 180), new Bgr(0, 0, 200));//二值阈值化
                //var image5 = image1.ThresholdBinaryInv(new Bgr(180, 180, 180), new Bgr(0, 0, 200));//反向二值阈值化
                //var image5 = image1.ThresholdToZero(new Bgr(180, 180, 180));//低于阈值被置零
                //var image5 = image1.ThresholdToZeroInv(new Bgr(180, 180, 180));//超过阈值被置零
                //var image5 = image1.ThresholdTrunc(new Bgr(180, 180, 180));//截断阈值化
                var image5 = image1.ThresholdAdaptive(new Bgr(255, 255, 255), ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C, THRESH.CV_THRESH_BINARY, 21, new Bgr(5, 5, 5));
                pictureBox1.Image = image5.ToBitmap();
            }
        }
    }
}
