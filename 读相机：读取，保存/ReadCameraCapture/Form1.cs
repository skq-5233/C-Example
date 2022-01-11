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
        Boolean g_SaveIndex;
        String FileName1;
        VideoWriter writer;
        public Form1()
        {
            InitializeComponent();
            captureInProcess = false;
            g_SaveIndex = false;
        }

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
            }
            else
            {
                button1.Text = "Stop";
                Application.Idle += ProcessFrame;                
            }
            captureInProcess = !captureInProcess;
        }

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
                if (g_SaveIndex)
                {
                    writer.WriteFrame(imageFrame1);    //保存视频
                }
                imageFrame1.Dispose();
            }
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!g_SaveIndex)
            {                
                var SaveVideo = new SaveFileDialog();
                SaveVideo.Filter = "AVI|*.avi";
                SaveVideo.Title = "保存视频";
                //SaveVideo.ShowDialog();
                if ((SaveVideo.FileName != null) && (SaveVideo.ShowDialog() == DialogResult.OK))
                {
                    g_SaveIndex = true;
                    FileName1 = SaveVideo.FileName;
                    int fps = Convert.ToInt32(g_Capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS));
                    //writer = new VideoWriter(FileName1,CvInvoke.CV_FOURCC('M','J','P','G'), 25, width, height,true);  
                    writer = new VideoWriter(FileName1, 15, width, height, true);//生成AVI视频
                    button2.Text = "Stop";
                }
            }
            else
            {
                g_SaveIndex = false;
                button2.Text = "Save";
                writer.Dispose();
            }
        }
    }
}
