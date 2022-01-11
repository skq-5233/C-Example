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


        //图像修复 (有问题）mask需要比较重要
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var mask = new Image<Gray, Byte>(image1.Size);
                mask.SetValue(255);
                var image2 = image1.InPaint(mask, 3);
                pictureBox2.Image = image2.ToBitmap();
              
            }
        }

        //均值漂移分割分割
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var imageDest = new Image<Bgr,Byte>(image1.Size);
                CvInvoke.cvPyrMeanShiftFiltering(image1.Ptr, imageDest.Ptr, 2, 40, 2, new MCvTermCriteria(5, 1));
                pictureBox3.Image = imageDest.ToBitmap();
            }
        }       

       
    }
}
