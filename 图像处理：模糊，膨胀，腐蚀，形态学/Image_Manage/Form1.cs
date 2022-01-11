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

namespace Image_Manage
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

        //模糊处理(高斯）
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                int a = 15;
                //var image2 = image1.SmoothGaussian(a, a, a / 4, a / 4);
                var image2 = image1.Copy();
                CvInvoke.cvSmooth(image1, image2, SMOOTH_TYPE.CV_GAUSSIAN, a, a, a / 4, a / 4);
                pictureBox2.Image = image2.ToBitmap();
            }
        }

        //腐蚀
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image3 = image1.Erode(10);
                pictureBox3.Image = image3.ToBitmap();
            }
        }

        //膨胀
        private void button4_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image3 = image1.Dilate(3);
                pictureBox3.Image = image3.ToBitmap();
            }
        }

        //更多地处理（类似膨胀，腐蚀）
        private void button5_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var element = new StructuringElementEx(3,3,1,1,CV_ELEMENT_SHAPE.CV_SHAPE_RECT);//自定义核
                var image4 = image1.MorphologyEx(element, CV_MORPH_OP.CV_MOP_OPEN, 3);//开运算（运行3次）
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var element = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
                var image4 = image1.MorphologyEx(element, CV_MORPH_OP.CV_MOP_CLOSE, 3);//闭运算
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var element = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
                var image4 = image1.MorphologyEx(element, CV_MORPH_OP.CV_MOP_GRADIENT, 3);//形态学梯度运算
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var element = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
                var image4 = image1.MorphologyEx(element, CV_MORPH_OP.CV_MOP_TOPHAT, 3);//礼帽运算
                pictureBox4.Image = image4.ToBitmap();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var element = new StructuringElementEx(3, 3, 1, 1, CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
                var image4 = image1.MorphologyEx(element, CV_MORPH_OP.CV_MOP_BLACKHAT, 3);//黑帽运算
                pictureBox4.Image = image4.ToBitmap();
            }
        }
    }
}
