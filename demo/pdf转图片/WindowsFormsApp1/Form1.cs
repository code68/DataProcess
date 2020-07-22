using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using O2S.Components.PDFRender4NET;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private delegate void SetPos(int ipos);

        private void SetTextMessage(int ipos)
        {
            if (this.InvokeRequired)
            {
                SetPos setpos = new SetPos(SetTextMessage);
                this.Invoke(setpos, new object[] { ipos});
            }
            else
            {
                this.label1.Text = ipos.ToString() + "/100";
                this.progressBar1.Value = Convert.ToInt32(ipos);
            }
        }

        public enum Definition
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10
        }

        /// <summary>
        /// 将PDF文档转换为图片的方法
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">设置所需图片格式</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public  void ConvertPDF2Image(string pdfInputPath, string imageOutputPath,
            string imageName, int startPageNum, int endPageNum, ImageFormat imageFormat, Definition definition)
        {
            PDFFile pdfFile = PDFFile.Open(pdfInputPath);
            if (!Directory.Exists(imageOutputPath))
            {
                Directory.CreateDirectory(imageOutputPath);
            }
            // validate pageNum
            if (startPageNum <= 0)
            {
                startPageNum = 1;
            }
            if (endPageNum > pdfFile.PageCount)
            {
                endPageNum = pdfFile.PageCount;
            }
            if (startPageNum > endPageNum)
            {
                int tempPageNum = startPageNum;
                startPageNum = endPageNum;
                endPageNum = startPageNum;
            }
            // start to convert each page
            for (int i = startPageNum; i <= endPageNum; i++)
            {
                Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                pageImage.Save(imageOutputPath + imageName + i.ToString() + "." + imageFormat.ToString(), imageFormat);
                pageImage.Dispose();
                SetTextMessage(100 * i / endPageNum);
            }
            pdfFile.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.AllowDrop = true;
        }
        private void Form1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    e.Effect = DragDropEffects.Link;
            //}
            //else
            //{
            //    e.Effect = DragDropEffects.None;
            //}
        }

        

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string filePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            FileInfo fi = new FileInfo(filePath.ToString());
            string dirName = fi.Name.Replace(".pdf", "");
            saveDirName = System.Environment.CurrentDirectory + "\\" + dirName + "\\";
            Thread fThread = new Thread(new ParameterizedThreadStart(SleepT) );//开辟一个新的线程
            fThread.Start(filePath);
        }


         void SleepT(object filePath)
        {
            FileInfo fi = new FileInfo(filePath.ToString());
            string dirName = fi.Name.Replace(".pdf", "");
            Directory.CreateDirectory(dirName);
            ConvertPDF2Image(filePath.ToString(), saveDirName, "NImage", 1, 500, ImageFormat.Png, Definition.Five);
            MessageBox.Show("转换成功！");
            btnConvert.Enabled = true;
            if (saveDirName == "")
            {
                System.Diagnostics.Process.Start("explorer.exe",
                    System.Environment.CurrentDirectory + "\\" + dirName + "\\");
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", saveDirName);
            }
        }


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }

        public string pdfFileName;
        public string saveDirName;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择PDF文件";
            dialog.Filter = "Pdf Files|*.pdf";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pdfFileName= dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择保存文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                saveDirName = dialog.SelectedPath+"\\";
            }

        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(pdfFileName))
            {
                MessageBox.Show(this, "请选择转换的文件", "提示");
                return;
            }
            if (string.IsNullOrEmpty(saveDirName))
            {
                MessageBox.Show(this, "请选择转换后保存的文件夹", "提示");
                return;
            }
            btnConvert.Enabled = false;
            Thread fThread = new Thread(new ParameterizedThreadStart(SleepT));//开辟一个新的线程
            fThread.Start(pdfFileName);

        }
    }
}
