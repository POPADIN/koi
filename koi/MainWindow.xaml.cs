using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace koi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Pool pool;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            pool = new Pool(Image.Height, Image.Width);
            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            pool.Advance();
            Image.Source = BmpImageFromBmp(RenderPool(pool));
        }

        public static System.Drawing.Point[] fishOutline =
        {
            new System.Drawing.Point(0, 0),
            new System.Drawing.Point(0, 10),
            new System.Drawing.Point(-2, 8),
            new System.Drawing.Point(-2, 0),
            new System.Drawing.Point(-1, -3),
            new System.Drawing.Point(-3, -6),
            new System.Drawing.Point(3, -6),
            new System.Drawing.Point(1, -3),
            new System.Drawing.Point(2, 0),
            new System.Drawing.Point(2, 8),
            new System.Drawing.Point(0, 10),
        };


        public static void RenderFish(Graphics gfx, Fish fish)
        {
            using (var brush = new SolidBrush(System.Drawing.Color.IndianRed))
            {
                gfx.TranslateTransform((float)fish.X, (float)fish.Y);
                gfx.RotateTransform((float)fish.GetAngle());

                gfx.FillClosedCurve(brush, fishOutline);

                gfx.ResetTransform();
            }
        }

        public static Bitmap RenderPool(Pool pool)
        {
            Bitmap bmp = new Bitmap((int)pool.Width, (int)pool.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.Clear(System.Drawing.Color.CornflowerBlue);
                foreach (var fish in pool.School)
                {
                    RenderFish(gfx, fish);
                }
            }
            return bmp;

        }

        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            pool.predaX = e.GetPosition(Image).X;
            pool.predaY = e.GetPosition(Image).Y;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            pool.predaX = pool.predaY = null;
        }
    }
}
