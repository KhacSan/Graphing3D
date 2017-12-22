
using DevExpress.Utils.Svg;
using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Graphing3D
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        private double xMin2D, xMax2D, xMin3D, xMax3D, yMin, yMax;
        SvgBitmap btm;
        private int check;
        Plotting3D paint3D = null;
        Plotting2D paint2D = null;
        private double xoayQuanhOy = 33.0, xoayQuanhOz = 24.0;
        Bitmap bitmap = null;
        Bitmap bitmap2d = null;
        public Form1()
        {
            InitializeComponent();
            listView2D.View = View.Details;
            listView2D.Columns.Add("X", 70);
            listView2D.Columns.Add("Y", 70);
            listView3D.View = View.Details;
            listView3D.Columns.Add("X", 50);
            listView3D.Columns.Add("Y", 50);
            listView3D.Columns.Add("Z", 50);
            Double.TryParse(textBoxXMin2D.Text,out xMin2D);
            Double.TryParse(textBoxXMax2D.Text, out xMax2D);
            Double.TryParse(textBoxXMin3D.Text, out xMin3D);
            Double.TryParse(textBoxXMax3D.Text,out xMax3D);
            Double.TryParse(textBoxYMin3D.Text, out yMin);
            Double.TryParse(textBoxYMax3D.Text, out yMax);
            paint2D = new Plotting2D();
            paint3D = new Plotting3D();
            check = 0;
        }

        private void textBoxXMax2D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxXMax2D.Text, out xMax2D) && !textBoxXMax2D.Text.Equals(""))
            {
                textBoxXMax2D.Focus();
                textBoxInfo2D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxXMax2D.Text.Equals("")) textBoxInfo2D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxXMin2D.Text.Equals("") && xMin2D >= xMax2D)
                    {
                        textBoxXMax2D.Focus();
                        textBoxInfo2D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo2D.Text = "XMax = " + xMax2D.ToString();
                }
            }
        }

        private void SaveImage(uint dpi, string file)
        {
            var converter = new ImageSvgConverter(null);
            string temporary = file + ".tmp.png";
            string convertedFileName = file.Replace(".svg", "").Replace(".SVG", "") + ".png";
            converter.Convert(file, temporary);
            using (Bitmap bitmap = (Bitmap)Image.FromFile(temporary))
            {
                using (Bitmap newBitmap = new Bitmap(bitmap))
                {
                    newBitmap.SetResolution(dpi, dpi);          //dpi could be 96, 300, 600, 1200, whatever floats your boat  
                    newBitmap.Save(convertedFileName, ImageFormat.Png);
                }
            }
            File.Delete(temporary);
        }

        private void textBoxXMin2D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxXMin2D.Text, out xMin2D) && !textBoxXMin2D.Text.Equals(""))
            {
                textBoxXMin2D.Focus();
                textBoxInfo2D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxXMin2D.Text.Equals("")) textBoxInfo2D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxXMax2D.Text.Equals("") && xMin2D >= xMax2D)
                    {
                        textBoxXMin2D.Focus();
                        textBoxInfo2D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo2D.Text = "XMin = " + xMin2D.ToString();
                }
            }
        }

        private void textBoxXMin3D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxXMin3D.Text, out xMin3D) && !textBoxXMin3D.Text.Equals(""))
            {
                textBoxXMin3D.Focus();
                textBoxInfo3D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxXMin3D.Text.Equals("")) textBoxInfo3D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxXMax3D.Text.Equals("") && xMin3D >= xMax3D)
                    {
                        textBoxXMin3D.Focus();
                        textBoxInfo3D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo3D.Text = "XMin = " + xMin3D.ToString();
                }
            }
        }

        public void renderPicture2D()
        {
            if (bitmap2d != null)
            {
                bitmap2d.Dispose();
            }
            SaveImage(600, Application.StartupPath + "\\demo2d");
            try
            {
                bitmap2d = (Bitmap)Image.FromFile(Application.StartupPath + "\\demo2d.png", true);
                pictureBox2D.Image = bitmap2d;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }

        }

        public void renderPicture3D()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
            }
            SaveImage(600, Application.StartupPath + "\\demo3d");
            try
            {
                bitmap = new Bitmap(Application.StartupPath + "\\demo3d.png", true);
                pictureBox3D.Image = bitmap;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("There was an error." +
                    "Check the path to the image file.");
            }
        }

        private void buttonPaint2D_Click(object sender, EventArgs e)
        {
            if (textBoxFxy.Text.Equals("") || textBoxXMin2D.Text.Equals("") || textBoxXMax2D.Text.Equals(""))
            {
                const string message = "Vui lòng nhập đủ thông tin!";
                const string caption = "Error";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    if (textBoxFxy.Text.Equals(""))
                    {
                        textBoxFxy.Focus();
                    }
                    else
                    {
                        if (textBoxXMin2D.Text.Equals(""))
                        {
                            textBoxXMin2D.Focus();
                        }
                        else
                        {
                            textBoxXMax2D.Focus();
                        }
                    }
                }
            }
            else
            {
                if (!Double.TryParse(textBoxXMin2D.Text, out xMin2D) || !Double.TryParse(textBoxXMax2D.Text, out xMax2D))
                {
                    const string message = "Vui lòng nhập số!";
                    const string caption = "Error";
                    var result = MessageBox.Show(message, caption,
                                                 MessageBoxButtons.OK,
                                                 MessageBoxIcon.Error);
                    if (result == DialogResult.OK)
                    {
                        if (!Double.TryParse(textBoxXMin2D.Text, out xMin2D))
                        {
                            textBoxXMin2D.Focus();
                            textBoxXMin2D.Text = "";
                        }
                        else
                        {
                            textBoxXMax2D.Focus();
                            textBoxXMax2D.Text = "";
                        }
                    }
                }
                else
                {
                    if (xMin2D >= xMax2D)
                    {
                        MessageBox.Show("Vui lòng nhập X: Min < Max !");
                        textBoxXMin2D.Focus();
                    }
                    else
                    {
                        listView2D.Items.Clear();
                        paint2d();
                    }
                }
            }
        }

        private void xoayXNguoc_Click(object sender, EventArgs e)
        {
            if (check == 1)
            {
                xoayQuanhOz -= 20.0;
                if (xoayQuanhOz <= 0.0) xoayQuanhOz = 360.0;
                paint3D.Paint3D(textBoxFxyz.Text, xoayQuanhOy, xoayQuanhOz);
                renderPicture3D();
            }
        }

        private void xoayYNguoc_Click(object sender, EventArgs e)
        {
            if (check == 1)
            {
                xoayQuanhOy -= 10.0;
                if (xoayQuanhOy <= 0.0) xoayQuanhOy = 1.0;
                paint3D.Paint3D(textBoxFxyz.Text, xoayQuanhOy, xoayQuanhOz);
                renderPicture3D();
            }
        }

        private void xoayYXuoi_Click(object sender, EventArgs e)
        {
            if (check == 1)
            {
                xoayQuanhOy += 10.0;
                if (xoayQuanhOy >= 90.0) xoayQuanhOy = 89.0;
                paint3D.Paint3D(textBoxFxyz.Text, xoayQuanhOy, xoayQuanhOz);
                renderPicture3D();
            }
        }


        private void xoayXXuoi_Click(object sender, EventArgs e)
        {
            if (check == 1)
            {
                xoayQuanhOz += 20.0;
                if (xoayQuanhOz >= 360.0) xoayQuanhOz = 0.0;
                paint3D.Paint3D(textBoxFxyz.Text, xoayQuanhOy, xoayQuanhOz);
                renderPicture3D();
            }
        }

        private void paint2d()
        {
            paint2D.Paint2D(xMin2D, xMax2D, listView2D, textBoxFxy.Text);
            renderPicture2D();
        }

        private void paint3d()
        {
            paint3D.funtion3D(xMin3D, xMax3D, yMin, yMax, listView3D, textBoxFxyz.Text);
            paint3D.Paint3D(textBoxFxyz.Text, xoayQuanhOy, xoayQuanhOz);
            renderPicture3D();
            check = 1;
        }

        private void buttonPaint3D_Click(object sender, EventArgs e)
        {

            if (textBoxFxyz.Text.Equals("") || textBoxXMin3D.Text.Equals("") || textBoxXMax3D.Text.Equals("")
                || textBoxYMin3D.Text.Equals("") || textBoxYMax3D.Text.Equals(""))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin !");
            }
            else
            {
                if (!Double.TryParse(textBoxXMin3D.Text, out xMin3D) || !Double.TryParse(textBoxXMax3D.Text, out xMax3D)
                   || !Double.TryParse(textBoxYMin3D.Text, out yMin) || !Double.TryParse(textBoxYMax3D.Text, out yMax))
                {
                    MessageBox.Show("Lỗi nhập sai kiểu dữ liệu !");
                }
                else
                {
                    if (xMin3D >= xMax3D || yMin >= yMax)
                    {
                        MessageBox.Show("Lỗi nhập giá trị Min >= Max !");
                    }
                    else
                    {
                        check = 0;
                        listView3D.Items.Clear();
                        paint3d();
                    }
                }
            }
        }

        private void textBoxXMax3D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxXMax3D.Text, out xMax3D) && !textBoxXMax3D.Text.Equals(""))
            {
                textBoxXMax3D.Focus();
                textBoxInfo3D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxXMax3D.Text.Equals("")) textBoxInfo3D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxXMin3D.Text.Equals("") && xMin3D >= xMax3D)
                    {
                        textBoxXMax3D.Focus();
                        textBoxInfo3D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo3D.Text = "XMax = " + xMax3D.ToString();
                }
            }
        }

        private void textBoxYMin3D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxYMin3D.Text, out yMin) && !textBoxYMin3D.Text.Equals(""))
            {
                textBoxYMin3D.Focus();
                textBoxInfo3D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxYMin3D.Text.Equals("")) textBoxInfo3D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxYMax3D.Text.Equals("") && yMin >= yMax)
                    {
                        textBoxYMin3D.Focus();
                        textBoxInfo3D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo3D.Text = "YMin = " + yMin.ToString();
                }
            }
        }

        private void textBoxYMax3D_TextChanged(object sender, EventArgs e)
        {
            if (!Double.TryParse(textBoxYMax3D.Text, out yMax) && !textBoxYMax3D.Text.Equals(""))
            {
                textBoxYMax3D.Focus();
                textBoxInfo3D.Text = "Lỗi nhập sai kiểu dữ liệu, vui lòng nhập số!";
            }
            else
            {
                if (textBoxYMax3D.Text.Equals("")) textBoxInfo3D.Text = "Vui lòng nhập số!";
                else
                {
                    if (!textBoxYMin3D.Text.Equals("") && yMin >= yMax)
                    {
                        textBoxYMax3D.Focus();
                        textBoxInfo3D.Text = "Lỗi giá trị Min >= Max!";
                    }
                    else textBoxInfo3D.Text = "YMax = " + yMax.ToString();
                }
            }
        }
    }
}
