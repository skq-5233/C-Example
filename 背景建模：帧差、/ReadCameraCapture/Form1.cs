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

namespace ReadCameraCapture
{
    public partial class Form1 : Form
    {
        Capture g_Capture;
        Int32 width = 640;
        Int32 height = 480;
        Boolean captureInProcess;
        Image<Bgr, Byte> BackGround;
        Boolean FrameDiffFlag = false;

        //成员        
        private Image<Bgr, Single> imageAccSum;          //累计图像       
        private Image<Bgr, Single> imageAccDiff;         //累计差值图像       
        private int frameCount;                          //已经累计的背景帧数        
        private Image<Bgr, Single> previousFrame;        //在背景建模时使用的前一帧图像
        private double scale;                            //计算背景时所使用的缩放系数，大于平均值*scale倍数的像素认为是前景       
        private Image<Gray, Byte> backgroundMask;        //计算得到的背景图像        
        private Image<Bgr, Single> imageTemp;            //临时图像            
        private Image<Gray, Single>[] imagesHi;          //背景模型中各通道的最大值图像        
        private Image<Gray, Single>[] imagesLow;         //背景模型中各通道的最小值图像
        private double alpha;                               //计算均值漂移时使用的权值

        public enum BackgroundStatModelType
        {
            FrameDiff = 1,      //帧差     
            AccAvg,         //平均背景      
            RunningAvg,     //均值漂移         
            MultiplyAcc,    //计算协方差        
            SquareAcc       //计算方差    
        }
        BackgroundStatModelType BackgroundStatModel = 0;
       
        public Form1()
        {
            InitializeComponent();
            captureInProcess = false;  
            BackGround = new Image<Bgr, Byte>(width, height);
            frameCount = 0;
            scale = 6d;

        }

         
        //开启相机
        private void button1_Click(object sender, EventArgs e)
        {
            g_Capture = new Capture(0);
            g_Capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, width);
            g_Capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, height);
            if (captureInProcess)
            {
                button1.Text = "Start";
                Application.Idle -= ProcessFrame;
                pictureBox1.Image = null;
                button2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
            }
            else
            {
                button1.Text = "Stop";
                Application.Idle += ProcessFrame;
                button2.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
            }
            captureInProcess = !captureInProcess;
        }

        //相机显示
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> imageFrame1 = null;
            if (g_Capture != null)
            {
                imageFrame1 = g_Capture.QueryFrame();
            }
            else
            {
                g_Capture = new Capture(0);
                g_Capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, width);
                g_Capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, height);
            }
                       
            if (imageFrame1 != null)
            {
                pictureBox1.Image = imageFrame1.ToBitmap();
                BackgroundStat(imageFrame1);          
                imageFrame1.Dispose();

            }            
            
        }

        //背景模型处理函数
        private void BackgroundStat(Image<Bgr,Byte> Frame)
        {
            switch (BackgroundStatModel)
            {
                case BackgroundStatModelType.FrameDiff:
                    if (FrameDiffFlag)
                    {
                        FrameDiffFlag = false;
                        BackGround = Frame.Copy();
                        pictureBox2.Image = BackGround.ToBitmap();
                    }
                    var backgroundMask1 = new Image<Gray, byte>(BackGround.Size);
                    var imageTemp1 = BackGround.AbsDiff(Frame);
                    Image<Gray, Byte>[] images = imageTemp1.Split();
                    backgroundMask1.SetValue(0d);
                    foreach (Image<Gray, Byte> image in images) 
                        backgroundMask1._Or(image.ThresholdBinary(new Gray(50d), new Gray(255d)));
                    backgroundMask1._Not();
                    pictureBox3.Image = backgroundMask1.ToBitmap();//背景
                    backgroundMask1._Not();
                    pictureBox4.Image = backgroundMask1.ToBitmap();//前景
                    //释放资源            
                    for (int i = 0; i < images.Length; i++)               
                        images[i].Dispose();
                    break;

                case BackgroundStatModelType.AccAvg:
                    if (frameCount==0)            
                    {                
                        imageAccSum = new Image<Bgr, Single>(Frame.Size);           
                        imageAccSum.SetValue(0d);                
                        imageAccDiff = new Image<Bgr, float>(Frame.Size);              
                        imageAccDiff.SetValue(0d);           
                    }           
                    imageTemp = Frame.ConvertScale<Single>(1d, 0d); //将图像转换成浮点型          
                    imageAccSum.Acc(imageTemp);           
                    if (previousFrame != null)               
                        imageAccDiff.Acc(imageTemp.AbsDiff(previousFrame));           
                    previousFrame = imageTemp.Copy();            
                    frameCount++;
                    pictureBox2.Image = imageAccSum.ToBitmap();
                    //计算出最高及最低阀值图像            
                    Image<Bgr, Single> imageAvg = imageAccSum.ConvertScale<Single>(1d / frameCount, 0d);  
                    Image<Bgr, Single> imageAvgDiff = imageAccDiff.ConvertScale<Single>(1d / frameCount, 1d);    //将平均值加1，为了确保总是存在差异           
                    Image<Bgr, Single> imageHi = imageAvg.Add(imageAvgDiff.ConvertScale<Single>(scale, 0d));    
                    Image<Bgr, Single> imageLow = imageAvg.Sub(imageAvgDiff.ConvertScale<Single>(scale, 0d));  
                    imagesHi = imageHi.Split();           
                    imagesLow = imageLow.Split();
                    //释放资源            
                    imageAvg.Dispose();            
                    imageAvgDiff.Dispose();            
                    imageHi.Dispose();            
                    imageLow.Dispose();
               

                    imageTemp = Frame.ConvertScale<Single>(1d, 0d);           
                    Image<Gray, Single>[] images1 = imageTemp.Split();
                    if (backgroundMask == null)
                        backgroundMask = new Image<Gray, byte>(Frame.Size);
                    backgroundMask.SetZero();
                    for (int i = 0; i < Frame.NumberOfChannels; i++)                
                        backgroundMask._Or(images1[i].InRange(imagesLow[i], imagesHi[i]));           
                    //释放资源           
                    for (int i = 0; i < images1.Length; i++)               
                        images1[i].Dispose();
                    pictureBox3.Image = backgroundMask.ToBitmap();
                    backgroundMask._Not();
                    pictureBox4.Image = backgroundMask.ToBitmap();
                    break;

                case BackgroundStatModelType.RunningAvg:

                    break;

                case BackgroundStatModelType.MultiplyAcc:

                    break;

                case BackgroundStatModelType.SquareAcc:

                    break;

                default:
                    break;
            }
        }
        //选择帧差背景模型
        private void button2_Click(object sender, EventArgs e)
        {
            BackgroundStatModel = BackgroundStatModelType.FrameDiff;
            button2.ForeColor = Color.Red;
            button4.ForeColor = Color.Black;
            button5.ForeColor = Color.Black;
            button6.ForeColor = Color.Black;
            button7.ForeColor = Color.Black;
            FrameDiffFlag = true;
        }
    

        //选择平均背景模型
        private void button4_Click(object sender, EventArgs e)
        {
            BackgroundStatModel = BackgroundStatModelType.AccAvg;
            button2.ForeColor = Color.Black;
            button4.ForeColor = Color.Red;
            button5.ForeColor = Color.Black;
            button6.ForeColor = Color.Black;
            button7.ForeColor = Color.Black;

            imageAccSum = null; 
            imageAccDiff = null; 
            frameCount = 0; 
            previousFrame = null; 
            scale = 6d; 
            backgroundMask = null; 
            imagesHi = null;
            imagesLow = null;
        }

        //均值漂移背景模型
        private void button5_Click(object sender, EventArgs e)
        {
            BackgroundStatModel = BackgroundStatModelType.RunningAvg;
            button2.ForeColor = Color.Black;
            button4.ForeColor = Color.Black;
            button5.ForeColor = Color.Red;
            button6.ForeColor = Color.Black;
            button7.ForeColor = Color.Black;

            imageAccSum = null;
            imageAccDiff = null;
            frameCount = 0;
            previousFrame = null;
            scale = 6d;
            alpha = 0.5d;
            backgroundMask = null;
            imagesHi = null;
            imagesLow = null;
        }

      
    }
}
