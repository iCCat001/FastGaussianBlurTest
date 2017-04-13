using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        int R = 7;
        double MaxText = 160;
        int MM=200;
        PrivateBitmap Source = new PrivateBitmap("D:\\1.png");
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

                            double t = RC > GC ? RC : GC;
                            t = t > BC ? t : BC;
                            MaxText = MaxText > t ? MaxText : t;

                            count++;
                        }
                    }
                   Output[x, y] = Color.FromArgb(255, (int)RC / MM, (int)GC / MM, (int)BC / MM);

                }
            MaxText = MaxText / 255;
            MM = (int)MaxText+1;
            return Output;
        }
        Color[,] myGaussBlur1(Color[,] Input,int r)
        {
            int l1 = Input.GetLength(0);
            int l2 = Input.GetLength(1);
            Color[,] Output = new Color[l1, l2];

            //正四棱锥模型（长=宽=高），模拟单位面积下使用积分，对高进行求值，用于分权
            double Height = r; //正四棱锥
            double S = r * r;
            double Volume = (r * r * r) / 3; //体积
            int[] Levels = new int[r];

            Levels[0] = 1;
            for(int i=1;i<r;i++)
            {
                Levels[i] = i * 4 + 4;  //每层单位数
            }
            double[] Heights = new double[r+1];
            for(int i=0;i<r;i++)
            {
                Heights[i] = (((((i+1)*(i+1)+i*i)+(Height/r)
                    + Math.Pow(((i + 1.0) * (i + 1.0) + i * i) * (Height / r),0.5)) /3)
                    - (i*i* (Height/r)))/Levels[i] + (Height / r) * (r-i-1); //每层高度（近似）（权值）
            }
            //Heights[r] = 0;
            double V=0;
            for (int i = 0; i < r; i++)
            {
                V += Heights[i] * Levels[i];
            }

            for (int x = 0; x < l1; x++)
                for (int y = 0; y < l2; y++)
                {
                    int count = 0;
                    double RC = 0, GC = 0, BC = 0;
                    for (int ix = x - r; ix < x + r + 1; ix++)
                    {
                        for (int iy = y - r; iy < y + r + 1; iy++)
                        {
                            if (ix < 0) ix = 0;
                            if (iy < 0) iy = 0;
                            if (ix >= l1) break;
                            if (iy >= l2) break;
                            int Floor = ((Math.Abs(ix - x)) >= (Math.Abs(iy - y)) ? Math.Abs(ix - x) : Math.Abs(iy - y));
                            RC += ((Input[ix, iy].R) * Heights[r-Floor]);
                            GC += ((Input[ix, iy].G) * Heights[r-Floor]);
                            BC += ((Input[ix, iy].B) * Heights[r-Floor]);
                            count++;
                        }
                    }
                    Output[x, y] = Color.FromArgb(255, (int)(RC / (V)), (int)(GC / (V)), (int)(BC / (V)));

                }

            return Output;
        }
        int ImgDrawBefore(Color[,] Msgs)
        {
            panel3.Refresh();
            GF = panel3.CreateGraphics();
            label2.Refresh();
            /*
            for (int i = 0; i < iSize; i++)
            {
                for (int ii = 0; ii < iSize; ii++)
                {
                    GF.DrawRectangle(new Pen(Msgs[i, ii]), new Rectangle(i, ii, 1, 1));
                }
                label2.Text = "正在渲染原图......" + (Single)i/iSize * 100 + "%";
                label2.Refresh();
            }
            */
            GF.DrawImage(Source.ImgData, new PointF(0, 0));
            return 1;
            
        }
        int ImgDrawAfter(Color[,] Msgs)
        {
            panel4.Refresh();
            GF = panel4.CreateGraphics();
            label2.Refresh();
            /*
            for (int i = 0; i < iSize; i++)
            {
                for (int ii = 0; ii < iSize; ii++)
                {
                    GF.DrawRectangle(new Pen(Msgs[i, ii]), new Rectangle(i, ii, 1, 1));
                }
                label2.Text = "正在渲染处理后的图像......" + (Single)i / iSize * 100 + "%";
                label2.Refresh();
            }
            */
            GF.DrawImage(Source.ImgData,new PointF(0,0));

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
            label3.Text = "类高斯模糊算法测试                         像素点数量:250000 像素"+"          模糊半径:"+ R +" 像素";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            int count = 50;
            Random Ran = new Random();
            Color[,] Test = new Color[iSize, iSize];
            Color[,] TestO = new Color[Test.GetLength(0), Test.GetLength(1)];
            Source.LockBits();
            for (int i = 0; i < Test.GetLength(0); i++)
                for (int ii = 0; ii < Test.GetLength(1); ii++)
                {
                    //TestO[i, ii] = Color.FromArgb(Ran.Next(1, 254), Ran.Next(1, 254), Ran.Next(1, 254));
                    Test[i, ii] = Source.GetPixel(i, ii);
                    //Test[i, ii] = Color.FromArgb(count+= +Ran.Next(1, 50), count+ Ran.Next(1, 50), count + Ran.Next(1, 50));
                    if (count >= 154) count = 1;
                }
            Source.UnLockBits();
            ImgDrawBefore(Test);
            Source.LockBits();
            DateTime DTS2;
            DateTime DTS = DateTime.Now;
            DTS = DateTime.Now;

            //TestO = myGaussBlur1(Test, R);

            TestO = myGaussBlur(Test, R);

            DTS2 = DateTime.Now;
            label2.Text = "算法耗时："+ (DTS2-DTS).TotalSeconds.ToString()+ "(秒/s)";
            for (int i = 0; i < TestO.GetLength(0); i++)
                for (int ii = 0; ii < TestO.GetLength(1); ii++)
                {
                    Source.SetPixel(i, ii, TestO[i, ii]);
                }
            Source.UnLockBits();
            //Source.LockBits();
            label3.Refresh();
            ImgDrawAfter(TestO);
            label2.Refresh();
        }
    }
}
