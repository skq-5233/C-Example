using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;


namespace DownLoad
{  
    public partial class DownLoad : Form
    {
        Byte[] ReceData = new Byte[16];
        Byte[] SendData = new Byte[16];
        int a = 0;


        public DownLoad()
        {
            InitializeComponent();
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
            }
            SendData[0] = 0x31;
            serialPort1.Write(SendData, 0, 1);
            timer1.Enabled = false;
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            serialPort1.Read(ReceData, 0, 1);
            
            label2.Text = ReceData[0].ToString("x");
            if (ReceData[0] == 0x31)
            {

                button2.Text = "软件升级中......";
                Process.Start("hypertrm.exe");
                button2.Text = "软件升级成功！！！";
            }
            else
            {
                button2.Text = "软件升级失败！！！";
            }
            serialPort1.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (a == 0)
            {
                SendData[0] = 0x13;
                SendData[1] = 0x00;
                SendData[2] = 0x13;
                serialPort1.Write(SendData, 0, 3);
                //serialPort1.Write(SendData, 1, 1);
                //serialPort1.Write(SendData, 2, 1);
                timer1.Enabled = true;
                a++;
                serialPort1.Close();
            }
        }
      
    }
}