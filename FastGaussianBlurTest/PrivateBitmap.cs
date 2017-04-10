using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace FastGaussianBlurTest
{
    public class PrivateBitmap
    {
        //Lrean form http://www.cnblogs.com/xiashengwang/p/4225848.html

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Deepth { get; private set; }
        public byte[] Pixels { get; set; }

        BitmapData pBitmapData = null;
        public Bitmap ImgData = null;
        IntPtr IntP = IntPtr.Zero;

        //构造函数
        public PrivateBitmap(Bitmap Source)
        {
            ImgData = Source;
        }
        public PrivateBitmap(string URL)
        {
            Bitmap Source = new Bitmap(Image.FromFile(URL));
            ImgData = Source;
        }
        public PrivateBitmap(Image Source)
        {
            ImgData = (Bitmap)Source;
        }

        /// <summary>
        /// Create(Save) & Lock Data to RAM
        /// </summary>
        public void LockBits()
        {
            //try
            //{
                Width = ImgData.Width;
                Height = ImgData.Height;
                Deepth = System.Drawing.Bitmap.GetPixelFormatSize(ImgData.PixelFormat);
                Rectangle ImgRect = new Rectangle(0, 0, Width, Height);

                if ((Deepth != 8) && (Deepth != 24) && (Deepth != 32))
                {
                    throw new ArgumentException("Only suport the picture whos Deepth is 8,24 or 32");
                }

                pBitmapData = ImgData.LockBits(ImgRect, ImageLockMode.ReadWrite, ImgData.PixelFormat);

                Pixels = new byte[Width * Height * (Deepth / 8)];

                IntP = pBitmapData.Scan0;
                System.Runtime.InteropServices.Marshal.Copy(IntP, Pixels, 0, Pixels.Length);
            //}
            //catch (Exception EX)
            //{
                //throw EX;
            //}
        }
        /// <summary>
        /// Read & UnLock Data
        /// </summary>
        public void UnLockBits()
        {
            //try
            //{
                System.Runtime.InteropServices.Marshal.Copy(Pixels, 0, IntP, Pixels.Length);

                ImgData.UnlockBits(pBitmapData);

            //}
            //catch (Exception EX)
            //{
                //throw EX;
            //}
        }
        /// <summary>
        /// Get the Color of Point(X,Y)
        /// </summary>
        /// <param name = "x"></param>
        /// <param name = "y"></param>
        public Color GetPixel(int x, int y)
        {
            Color ColorTemp = new Color();
            try
            {
                int Count = (y * Width + x) * (Deepth / 8);
                if (Count > Pixels.Length - Deepth / 8)
                    throw new IndexOutOfRangeException("Out of Data Range");
                if (x < 0 || y < 0 || x > Width || y > Height)
                    throw new IndexOutOfRangeException("x & y must be bigger than 0");
                switch (Deepth)
                {
                    case 8:
                        ColorTemp = Color.FromArgb(Pixels[Count], Pixels[Count], Pixels[Count]);
                        break;
                    case 24:
                        ColorTemp = Color.FromArgb(Pixels[Count + 2], Pixels[Count + 1], Pixels[Count]);
                        break;
                    case 32:
                        ColorTemp = Color.FromArgb(Pixels[Count + 3], Pixels[Count + 2], Pixels[Count + 1]
                            , Pixels[Count]);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
            return ColorTemp;
        }
        /// <summary>
        /// Set the Color of Point(X,Y)
        /// </summary>
        /// <param name = "x"></param>
        /// <param name = "y"></param>
        /// <param name = "Source"></param>
        public void SetPixel(int x, int y, Color Source)
        {
            try
            {
                int Count = (y * Width + x) * (Deepth / 8);
                if (x < 0 || y < 0 || x > Width || y > Height)
                    throw new IndexOutOfRangeException("x & y must be bigger than 0");
                switch (Deepth)
                {
                    case 8:
                        Pixels[Count] = Source.B;
                        break;
                    case 24:
                        Pixels[Count] = Source.B;
                        Pixels[Count + 1] = Source.G;
                        Pixels[Count + 2] = Source.R;
                        break;
                    case 32:
                        Pixels[Count] = Source.B;
                        Pixels[Count + 1] = Source.G;
                        Pixels[Count + 2] = Source.R;
                        Pixels[Count + 3] = Source.A;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
    }
}
