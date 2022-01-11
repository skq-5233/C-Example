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

        // 透视变换
        //仿射变换3个点，一个平面内，var mymat = new Matrix<float>(2, 3);      
        //mymat = CameraCalibration.GetAffineTransform(srcquad, dstquad);
        //var image2 = image1.WarpAffine(mymat, INTER.CV_INTER_NN, WARP.CV_WARP_FILL_OUTLIERS, new Bgr(255, 255, 0)); 
        private void button2_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
             
                PointF[] srcquad = new PointF[4];
                PointF[] dstquad = new PointF[4];

                srcquad[0].X = 0;
                srcquad[0].Y = 0;
                srcquad[1].X = image1.Width - 1;
                srcquad[1].Y = 0;
                srcquad[2].X = 0;
                srcquad[2].Y = image1.Height - 1;
                srcquad[3].X = image1.Width - 1;
                srcquad[3].Y = image1.Height - 1;


                dstquad[0].X = (float)(image1.Width * 0.05);
                dstquad[0].Y = (float)(image1.Height * 0.33);
                dstquad[1].X = (float)(image1.Width * 0.9);
                dstquad[1].Y = (float)(image1.Height * 0.25);
                dstquad[2].X = (float)(image1.Width * 0.2);
                dstquad[2].Y = (float)(image1.Height * 0.7);
                dstquad[3].X = (float)(image1.Width * 0.8);
                dstquad[3].Y = (float)(image1.Height * 0.9);

                var mymat = new Matrix<double>(3, 3);
                //mymat = CameraCalibration.GetPerspectiveTransform(srcquad, dstquad);                
                CvInvoke.cvGetPerspectiveTransform(srcquad, dstquad, mymat);             
                var image2 = image1.WarpPerspective(mymat, INTER.CV_INTER_NN, WARP.CV_WARP_FILL_OUTLIERS, new Bgr(255, 255, 0));            
                pictureBox2.Image = image2.ToBitmap();
            }
        }

        //直方图均值化
        private void button3_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {                
                var image3 = image1.Copy();
                image3._EqualizeHist();
                //CvInvoke.cvEqualizeHist(image1, image3);//有问题，不知道为什么
                pictureBox3.Image = image3.ToBitmap();
            }
        }

        //一维直方图显示
        private void button4_Click(object sender, EventArgs e)
        {
            if (image1 != null)
            {
                int h_max_range = 180;
                int s_max_range = 255;

                //创建IntPtr类型的图片
                IntPtr h_plane = CvInvoke.cvCreateImage(image1.Size, IPL_DEPTH.IPL_DEPTH_8U, 1);//色调
                IntPtr s_plane = CvInvoke.cvCreateImage(image1.Size, IPL_DEPTH.IPL_DEPTH_8U, 1);//饱和度
                IntPtr v_plane = CvInvoke.cvCreateImage(image1.Size, IPL_DEPTH.IPL_DEPTH_8U, 1);//亮度值
                IntPtr hsv = CvInvoke.cvCreateImage(image1.Size, IPL_DEPTH.IPL_DEPTH_8U, 3);

                //转换成Hsv色彩空间
                CvInvoke.cvCvtColor(image1, hsv, COLOR_CONVERSION.BGR2HSV);//转换成Hsv色彩空间（不加好像也可以）
                CvInvoke.cvSplit(hsv, h_plane, s_plane, v_plane, IntPtr.Zero);//分割图像B,G,R


                /** Hue的變化範圍 */
                //float[] h_ranges = new float[2] { 0, h_max_range }; //max_range  = 180
                IntPtr[] h_plane_ptr = new IntPtr[] { h_plane };
                //float[] s_ranges = new float[2] { 0, s_max_range };
                IntPtr[] s_plane_ptr = new IntPtr[] { s_plane };

                IntPtr[] planes = new IntPtr[2] { h_plane, s_plane };

                int[] HSbins = { 16, 16 }; //切割量化的數量{HBin,SBin}
                //int[] Hbins = { 16 };
                //int[] Sbins = { 16 };
                int Hbins = 16;
                int Sbins = 16;

                //配置Histogarm空间 計算Hue的色彩分布直方图
                RangeF hRange = new RangeF(0, h_max_range);       //H色調分量的變化範圍
                RangeF sRange = new RangeF(0, s_max_range);       //S飽和度分量的變化範圍
                DenseHistogram histDense = new DenseHistogram(HSbins, new RangeF[] { hRange, sRange });//初始化，配置空間（告知色相的範圍與切割的bin值）
                //DenseHistogram h_histDense = new DenseHistogram(Hbins,new RangeF[]{ hRange});
                //DenseHistogram s_histDense = new DenseHistogram(Sbins,new RangeF[]{sRange});
                DenseHistogram h_histDense = new DenseHistogram(Hbins, hRange);//
                DenseHistogram s_histDense = new DenseHistogram(Sbins, sRange);
                CvInvoke.cvCalcHist(planes, histDense, true, System.IntPtr.Zero);  //把色相的影像資料
                CvInvoke.cvCalcHist(h_plane_ptr, h_histDense, true, System.IntPtr.Zero);  //把色相的影像資料
                CvInvoke.cvCalcHist(s_plane_ptr, s_histDense, true, System.IntPtr.Zero);  //把色相的影像資料


                float max_value = 0;
                int[] a1 = new int[100];//最小值坐标（可以为null）
                int[] b1 = new int[100];//最大值坐标（可以为null）
                float min_value = 0;
                //int h_bins = h_histDense.BinDimension[0].Size;//取得我們之前計算時所設定的顏色區間設定區塊

                //取得直方圖的最大與最小累積值

                CvInvoke.cvGetMinMaxHistValue(h_histDense, ref min_value, ref max_value, a1, b1);
                int height = 300;
                int width = 400;
                IntPtr h_img = CvInvoke.cvCreateImage(new Size(width, height), IPL_DEPTH.IPL_DEPTH_8U, 3);
                CvInvoke.cvZero(h_img);
                int bin_w = width / (Hbins); //計算出顯示在圖像上的bin寬度

                for (int h = 0; h < Hbins; h++)
                {

                    /** 取得直方圖的統計資料，計算值方圖中的所有顏色中最高統計的數值，作為實際顯示時的圖像高 */
                    //取得值方圖的數值位置,以便之後存成檔案
                    double bin_val = CvInvoke.cvQueryHistValue_1D(h_histDense, h);

                    //計算取得的bin值要顯示在240高的圖像上時的位置
                    int intensity = (int)Math.Round(bin_val * height / max_value);

                    /** 取得現在抓取的直方圖的hue顏色，並為了顯示成圖像可觀看，轉換成RGB色彩 */
                    CvInvoke.cvRectangle(h_img, new Point(h * bin_w, height), new Point((h + 1) * bin_w, height - intensity),
                                     HueToBgr(h * 180/ Hbins), -1, LINE_TYPE.EIGHT_CONNECTED, 0);

                }

                var h_image = new Image<Bgr, Byte>(new Size(width, height));
                CvInvoke.cvCopy(h_img, h_image.Ptr, IntPtr.Zero);
                pictureBox4.Image = h_image.ToBitmap();

                width = Hbins * Sbins * 2;

                CvInvoke.cvGetMinMaxHistValue(histDense, ref min_value, ref max_value, a1, b1);
                //用來存放從Hsv轉回RGB圖像時用的空間
                IntPtr hsv_color = CvInvoke.cvCreateImage(new Size(1, 1), IPL_DEPTH.IPL_DEPTH_8U, 3);
                IntPtr rgb_color = CvInvoke.cvCreateImage(new Size(1, 1), IPL_DEPTH.IPL_DEPTH_8U, 3);

                IntPtr hs_img = CvInvoke.cvCreateImage(new Size(width, height), IPL_DEPTH.IPL_DEPTH_8U, 3);
                CvInvoke.cvZero(hs_img);

                int bin_hs = width / (Hbins * Sbins);
                for (int h = 0; h < Hbins; h++)
                {
                    for (int s = 0; s < Sbins; s++)
                    {
                        int i = h * Sbins + s;

                        /** 取得直方圖的統計資料，計算值方圖中的所有顏色中最高統計的數值，作為實際顯示時的圖像高 */
                        //取得值方圖的數值位置,以便之後存成檔案
                        double bin_val = CvInvoke.cvQueryHistValue_2D(histDense, h, s);
                        int intensity = (int)Math.Round(bin_val * height / max_value);

                        /** 取得現在抓取的直方圖的hue顏色，並為了顯示成圖像可觀看，轉換成RGB色彩 */
                        CvInvoke.cvSet2D(hsv_color, 0, 0, new MCvScalar(h * 180.0f / Hbins, s * 255.0f / Sbins, 255, 0)); //這邊用來計算色相與飽和度的統計資料轉換到圖像上 hsv_color的數值
                        CvInvoke.cvCvtColor(hsv_color, rgb_color, COLOR_CONVERSION.HSV2RGB);   //在把hsv顏色空間轉換為RGB
                        MCvScalar color = CvInvoke.cvGet2D(rgb_color, 0, 0);
                        CvInvoke.cvRectangle(hs_img, new Point(i * bin_w, height), new Point((i + 1) * bin_w, height - intensity), color, -1, LINE_TYPE.EIGHT_CONNECTED, 0);
                    }
                }

                var hs_image = new Image<Bgr, Byte>(new Size(width, height));
                CvInvoke.cvCopy(hs_img, hs_image.Ptr, IntPtr.Zero);
                pictureBox1.Image = hs_image.ToBitmap();              

            }
        }

        private static MCvScalar HueToBgr(double hue)
        {
            int[] rgb = new int[3];
            int p, sector;
            int[,] sector_data = { { 0, 2, 1 }, { 1, 2, 0 }, { 1, 0, 2 }, { 2, 0, 1 }, { 2, 1, 0 }, { 0, 1, 2 } };
            hue *= 0.033333333333333333333333333333333f;
            sector = (int)Math.Floor(hue);
            p = (int)Math.Round(255 * (hue - sector));
            //p ^= sector & 1 ? 255 : 0;
            if ((sector & 1) == 1) p ^= 255;
            else p ^= 0;
            rgb[sector_data[sector, 0]] = 255;
            rgb[sector_data[sector, 1]] = 0;
            rgb[sector_data[sector, 2]] = p;
            MCvScalar scalar = new MCvScalar(rgb[2], rgb[1], rgb[0], 0);
            return scalar;
        }
    }
}
