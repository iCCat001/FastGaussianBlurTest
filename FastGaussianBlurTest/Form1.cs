using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastGaussianBlurTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Graphics GF;
        int iSize = 500;
        int R = 3;
        // source channel, target channel, width, height, radius
        int[] gaussBlur(int[] scl, int w, int h, int r)
        {
            int[] tcl = new int[10];
            int rs = (int)(r * 2.57);     // significant radius
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                {
                    int val = 0, wsum = 0;
                    for (int iy = i - rs; iy < i + rs + 1; iy++)
                        for (int ix = j - rs; ix < j + rs + 1; ix++)
                        {
                            int x = Math.Min(w - 1, Math.Min(0, ix));
                            int y = Math.Min(h - 1, Math.Min(0, iy));
                            int dsq = (ix - j) * (ix - j) + (iy - i) * (iy - i);
                            int wght = (int)(Math.Exp((-dsq / (2 * r * r))) / (Math.PI * 2 * r * r));
                            val += scl[y * w + x] * wght; wsum += wght;
                        }
                    tcl[i * w + j] = (int)(val / wsum);
                }
            return tcl;
        }
        Color[,] myGaussBlur(Color[,] Input,int r)
        {
            int l1 = Input.GetLength(0);
            int l2 = Input.GetLength(1);
            Color[,] Output = new Color[l1, l2];

            for (int x = 0; x < l1; x++)
                for (int y = 0; y < l2; y++)
                {
                    int count = 0;
                    Single RC = 0, GC = 0, BC = 0;
                    for (int ix = x - r; ix < x + r+1; ix++)
                    {
                        for (int iy = y - r; iy < y + r+1; iy++)
                        {
                            if (ix < 0) ix = 0;
                            if (iy < 0) iy = 0;
                            if (ix >= l1) break;
                            if (iy >= l2) break ;
                            RC += (Single)(Input[ix, iy].R * (1.0- ((Single)(Math.Abs(ix-x)* Math.Abs(iy - y))/ (r*r))));
                            GC += (Single)(Input[ix, iy].G * (1.0 - ((Single)(Math.Abs(ix - x) * Math.Abs(iy - y)) / (r * r))));
                            BC += (Single)(Input[ix, iy].B * (1.0 - ((Single)(Math.Abs(ix - x) * Math.Abs(iy - y)) / (r * r))));
                            count++;
                        }
                    }
                   Output[x, y] = Color.FromArgb(255, (int)RC / 90, (int)GC / 90, (int)BC / 90);

                }
            return Output;
        }

        int ImgDrawBefore(Color[,] Msgs)
        {
            panel3.Refresh();
            GF = panel3.CreateGraphics();
            label2.Text = "正在渲染原图";
            label2.Refresh();
            for (int i = 0; i < iSize; i++)
            {
                for (int ii = 0; ii < iSize; ii++)
                {
                    GF.DrawRectangle(new Pen(Msgs[i, ii]), new Rectangle(i, ii, 1, 1));
                }
                label2.Text = "正在渲染原图......" + (Single)i/iSize * 100 + "%";
                label2.Refresh();
            }
            return 1;
            
        }
        int ImgDrawAfter(Color[,] Msgs)
        {
            panel4.Refresh();
            GF = panel4.CreateGraphics();
            label2.Text = "正在渲染处理后的图像";
            label2.Refresh();
            for (int i = 0; i < iSize; i++)
            {
                for (int ii = 0; ii < iSize; ii++)
                {
                    GF.DrawRectangle(new Pen(Msgs[i, ii]), new Rectangle(i, ii, 1, 1));
                }
                label2.Text = "正在渲染处理后的图像......" + (Single)i / iSize * 100 + "%";
                label2.Refresh();
            }
            return 1;

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            /*
int[] scl = new int[10];
for (int i = 0; i < scl.Length; i++)
{
    scl[i] = i + 100;
}
int[] tcl = gaussBlur(scl, 10, 10, 1);
scl[1] = 1 + 100;

*/
            label3.Text = "类高斯模糊算法测试         模糊半径:"+ R +"                     点击开始          ";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            int count = 50;
            Image T1 = Bitmap.FromFile("D:\\1.png");
            Bitmap TT = new Bitmap(T1);
            Random Ran = new Random();
            Color[,] Test = new Color[iSize, iSize];
            Color[,] TestO = new Color[Test.GetLength(0), Test.GetLength(1)];
            for (int i = 0; i < Test.GetLength(0); i++)
                for (int ii = 0; ii < Test.GetLength(1); ii++)
                {
                    //TestO[i, ii] = Color.FromArgb(Ran.Next(1, 254), Ran.Next(1, 254), Ran.Next(1, 254));
                    Test[i, ii] = TT.GetPixel(i, ii);
                    //Test[i, ii] = Color.FromArgb(count+= +Ran.Next(1, 50), count+ Ran.Next(1, 50), count + Ran.Next(1, 50));
                    if (count >= 154) count = 1;
                }
            

            DateTime DTS = DateTime.Now;
            DTS = DateTime.Now;
            TestO = myGaussBlur(Test, 5);
            //label3.Text += "   算法耗时："+ (DateTime.Now-DTS).ToString();
            label3.Refresh();
            ImgDrawBefore(Test);
            ImgDrawAfter(TestO);
            label2.Text = "渲染完成";
            label2.Refresh();
        }
    }
}
