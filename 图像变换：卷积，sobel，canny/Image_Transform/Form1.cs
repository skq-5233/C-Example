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

        // 卷积边界
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                var image2 = new Image<Bgr, Byte>(600, 400);
                CvInvoke.cvCopyMakeBorder(image1, image2, new Point(2, 2), BORDER_TYPE.REPLICATE, new MCvScalar(0));
                pictureBox2.Image = image2.ToBitmap();
            }
        }

        //sobel导数
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                //var image3 = image1.Sobel(1, 0, 3);
                var image3 = image1.Sobel(0, 1, 3);
                pictureBox3.Image = image3.ToBitmap();
            }
        }

        //边缘检测Canny
        private void button4_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {               
                var image4 = image1.Canny(150, 100, 3);
                pictureBox4.Image = image4.ToBitmap();
            }
        }
    }
}
