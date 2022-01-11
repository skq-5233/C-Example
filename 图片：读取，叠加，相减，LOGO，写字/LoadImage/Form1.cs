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


namespace LoadImage
{
    public partial class Form1 : Form
    {
        Image<Bgr, Byte> One_picture;
        Image<Bgr, Byte> Two_picture;        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ChooseImage = new OpenFileDialog();
            ChooseImage.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
            if (ChooseImage.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    One_picture = new Image<Bgr, Byte>(ChooseImage.FileName);
                    pictureBox1.Image = One_picture.ToBitmap();

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("载入文件格式错误！");
                }

            }

        }

        //增加图像区域的蓝色通道值
        private void button2_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                CvInvoke.cvSetImageROI(One_picture, new Rectangle(100, 100, 100, 100));               
                var One_picture1 =  One_picture.Add(new Bgr(150, 0, 0));  
                
                CvInvoke.cvResetImageROI(One_picture);
                for (int i = 100; i < 200; i++)
                {
                    for (int j = 100; j < 200; j++)
                    {
                        One_picture[i, j] = One_picture1[i - 100, j - 100];                                              
                    }
                }
                pictureBox1.Image = One_picture.ToBitmap();
              
            }
            
        }

        //设置图像区域为红色
        private void button3_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                One_picture.ROI = new Rectangle(100, 100, 100, 100);
                One_picture.SetValue(new Bgr(Color.Red));
                One_picture.ROI = Rectangle.Empty;                   //取消ROI设置，否则操作都会在ROI中进行，包括图像显示
                pictureBox1.Image = One_picture.ToBitmap();
            }
        }

        //图像叠加(两张图像必须相同）
        private void button4_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                OpenFileDialog ChooseImage1 = new OpenFileDialog();
                ChooseImage1.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                if (ChooseImage1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Two_picture = new Image<Bgr, Byte>(ChooseImage1.FileName);
                        int width = (int)(Two_picture.Width > One_picture.Width ? One_picture.Width : Two_picture.Width);
                        int heigth = (int)(Two_picture.Height > One_picture.Height ? One_picture.Height : Two_picture.Height);
                        One_picture.ROI = new Rectangle(0, 0, width, heigth);
                        Two_picture.ROI = new Rectangle(0, 0, width, heigth);
                        One_picture = One_picture.Add(Two_picture);
                        pictureBox1.Image = One_picture.ToBitmap();

                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("载入文件格式错误！");
                    }

                }
            }
        }

        //图像叠加(两张图像必须相同）使用掩膜
        private void button5_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                OpenFileDialog ChooseImage1 = new OpenFileDialog();
                ChooseImage1.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                if (ChooseImage1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Two_picture = new Image<Bgr, Byte>(ChooseImage1.FileName);
                        int width = (int)(Two_picture.Width > One_picture.Width ? One_picture.Width : Two_picture.Width);
                        int heigth = (int)(Two_picture.Height > One_picture.Height ? One_picture.Height : Two_picture.Height);
                        One_picture.ROI = new Rectangle(0, 0, width, heigth);
                        Two_picture.ROI = new Rectangle(0, 0, width, heigth);
                        var rect = new Rectangle(0, 0, width / 2, heigth);
                        var mask = new Image<Gray, Byte>(One_picture.Size);
                        mask.SetZero();
                        mask.ROI = rect;
                        mask.SetValue(255);
                        mask.ROI = Rectangle.Empty;
                        //方法1：
                        //var resImage = One_picture.Add(Two_picture, mask);//mask(i) ==0时，目标图像默认是null，只进行mask（i）不等于0时进行操作
                        //mask._Not();//反转mask值(255->0,0->255)
                        //One_picture.Copy(resImage, mask);//在Mask(I) != 0 时，将One_picture图像拷贝到resImage！
                        //方法2：
                        var resImage = One_picture.Copy();
                        resImage.ROI = One_picture.ROI;
                        CvInvoke.cvAdd(One_picture, Two_picture, resImage, mask);//操作后resImage的ROI属性不变还是true
                        pictureBox1.Image = resImage.ToBitmap();

                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("载入文件格式错误！");
                    }

                }
            }
        }

        //减去图片
        private void button6_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                var colorImage = new Image<Bgr, Byte>(One_picture.Size);
                colorImage.SetValue(new Bgr(Color.Red));
                One_picture = One_picture.Sub(colorImage);
                pictureBox1.Image = One_picture.ToBitmap();
            }

        }

        //减去图片使用掩膜
        private void button7_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                var colorImage = new Image<Bgr, Byte>(One_picture.Size);
                colorImage.SetValue(new Bgr(Color.Red));
                var mask = new Image<Gray, Byte>(One_picture.Size);
                var rect = new Rectangle(0, 0, One_picture.Width / 2, One_picture.Height);
                mask.SetZero();
                mask.ROI = rect;
                mask.SetValue(255);
                mask.ROI = Rectangle.Empty;
                //One_picture = One_picture.Sub(colorImage, mask);
                //pictureBox1.Image = One_picture.ToBitmap();
                var resImage = One_picture.Sub(colorImage, mask);
                mask._Not();//反转mask值(255->0,0->255)
                One_picture.Copy(resImage, mask);//在Mask(I) != 0 时，将One_picture图像拷贝到resImage！
                pictureBox1.Image = resImage.ToBitmap();
                //CvInvoke.cvSub(One_picture, Two_picture, resImage, mask);
            }

        }

        //添加Logo
        private void button8_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                OpenFileDialog LogoImage = new OpenFileDialog();
                LogoImage.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                if (LogoImage.ShowDialog() == DialogResult.OK)
                {
                    var logo = new Image<Bgr, Byte>(LogoImage.FileName);
                    var resImage = One_picture.Copy();
                    One_picture.ROI = new Rectangle(new Point(0, 0), logo.Size);
                    resImage.ROI = One_picture.ROI;
                    if (logo != null)
                    {
                        OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                        OpenFileDialog1.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                        if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            var mask = new Image<Gray, Byte>(OpenFileDialog1.FileName);
                            var colorMask = mask.Convert<Bgr, Byte>();
                            CvInvoke.cvAdd(One_picture, colorMask, resImage, mask);
                                                    
                            CvInvoke.cvAnd(resImage, logo, resImage, mask);//
                            resImage.ROI = Rectangle.Empty;
                            pictureBox1.Image = resImage.ToBitmap();
                        }

                    }

                }
            }

        }

        //图像混合相加
        private void button9_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                OpenFileDialog LogoImage = new OpenFileDialog();
                LogoImage.Filter = "JPEG;BMP;PNG;JPG|*.JPEG;*.BMP;*.PNG;*.JPG|ALL|*.*";
                if (LogoImage.ShowDialog() == DialogResult.OK)
                {                    
                    Two_picture = new Image<Bgr, Byte>(LogoImage.FileName);
                    int width = (int)(Two_picture.Width > One_picture.Width ? One_picture.Width : Two_picture.Width);
                    int heigth = (int)(Two_picture.Height > One_picture.Height ? One_picture.Height : Two_picture.Height);
                
                    One_picture.ROI = new Rectangle(0, 0, width, heigth);
                    Two_picture.ROI = new Rectangle(0, 0, width, heigth);

                    var resImage = One_picture.AddWeighted(Two_picture, 0.5, 0.5, 0);
                   
                    pictureBox1.Image = resImage.ToBitmap();

                }
            }
        }

        //图片上写文字
        private void button10_Click(object sender, EventArgs e)
        {
            if (One_picture != null)
            {
                var font = new MCvFont(FONT.CV_FONT_HERSHEY_DUPLEX, 1, 1);
                One_picture.Draw("Hello Dog!", ref font, new Point(100, 100), new Bgr(255, 255, 255));
                pictureBox1.Image = One_picture.ToBitmap();
                //One_picture.Save("E:\李志\C#Test\图片");
               One_picture.Save("1.jpg");
            }
        }



    }
}
