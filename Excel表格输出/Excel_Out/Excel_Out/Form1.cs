using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Excel_Out
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog CSVSave = new SaveFileDialog();
            CSVSave.Filter = "*.csv|*.csv";
            CSVSave.DefaultExt = ".csv";
            if (CSVSave.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(CSVSave.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(fs, Encoding.Default);
                writer.WriteLine("111" + "," + "是哈哈" + "\n");
                writer.WriteLine("111" + "," + "是哈哈" + "\n\r");
                writer.WriteLine("111" + "," + "是哈哈");
                writer.WriteLine("111" + "," + "是哈哈" + "\r");
                writer.WriteLine("111" + "," + "是哈哈");

                writer.Close();
                fs.Close();

                
            }
        }
    }
}
