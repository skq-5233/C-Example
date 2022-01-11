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
namespace Show_Capture
{
    public partial class Form1 : Form
    {
        Capture One_Capture;
        Boolean Start_Index;
        int FramePosition;
        int FrameTotal;
        int VideoFps;
        public Form1()
        {            
            InitializeComponent();
            Start_Index = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var New_Capture = new OpenFileDialog();
            New_Capture.Filter = "*.AVI|*.AVI|ALL|*.*";
            if (New_Capture.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    One_Capture = new Capture(New_Capture.FileName);
                    //FramePosition = Convert.ToInt32(One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES)) * timer1.Interval / 1000;
                    //FrameTotal = Convert.ToInt32(One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_COUNT)) * timer1.Interval / 1000;
                    FramePosition = (Int32)One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES);
                    FrameTotal = (Int32)One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_COUNT);
                    VideoFps = (int)One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS);//读取帧频
                    trackBar1.Maximum = FrameTotal;
                    label1.Text = FramePosition.ToString() + "s/" + FrameTotal.ToString() + "s";
                    One_Capture.ImageGrabbed += One_Capture_ImageGrabbed;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("载入文件格式错误！");
                }
            }
        }

        private void One_Capture_ImageGrabbed(object sender, EventArgs e)
        {
            var ImageFrame = One_Capture.RetrieveBgrFrame();
            //FramePosition = Convert.ToInt32(One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES)) * timer1.Interval / 1000;
            FramePosition = Convert.ToInt32(One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES));
            System.Threading.Thread.Sleep((int)(1000 / VideoFps - 5));
            this.trackBar1.Invoke(new EventHandler(delegate
            {
                trackBar1.Value = FramePosition;
            }));
            //label1.Text = FramePosition.ToString() + "s/" + FrameTotal.ToString() + "s";
            this.label1.Invoke(new EventHandler(delegate
            {
                label1.Text = FramePosition.ToString() + "s/" + FrameTotal.ToString() + "s"; ;
            }));
            this.pictureBox1.Invoke(new EventHandler(delegate
            {
                pictureBox1.Image = ImageFrame.ToBitmap();
            }));
            ImageFrame.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {     
            Image<Bgr,Byte> Image_Smooth;
            var ImageFrame = One_Capture.QueryFrame();
            //Image_Smooth = ImageFrame.SmoothGaussian(ImageFrame.Width, ImageFrame.Height, 3, 3);  //高斯平滑处理         
            FramePosition = Convert.ToInt32(One_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES)) * timer1.Interval / 1000;
            label1.Text = FramePosition.ToString() + "s/" + FrameTotal.ToString() + "s";
            if (ImageFrame != null)
            {
                this.pictureBox1.Invoke(new EventHandler(delegate
                {
                    //pictureBox1.Image = Image_Smooth.ToBitmap();
                    pictureBox1.Image = ImageFrame.ToBitmap();
                }));                
                ImageFrame.Dispose();
            }
            else
            {
                timer1.Enabled = false;
                button2.Text = "开始";
                Start_Index = false;
            }            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (One_Capture != null)
            {
                if (!Start_Index)
                {
                    //timer1.Enabled = true;
                    One_Capture.Start();
                    button2.Text = "暂停";
                    Start_Index = true;
                }
                else
                {
                    //timer1.Enabled = false;
                    One_Capture.Stop();
                    button2.Text = "开始";
                    Start_Index =false;
                }
            }
                        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var Current_Frame = Convert.ToInt32(textBox1.Text) < FrameTotal ? Convert.ToInt32(textBox1.Text) : FramePosition;
            One_Capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, Current_Frame);
        }
    }
}
