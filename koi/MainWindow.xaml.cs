using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Rectangle = System.Drawing.Rectangle;

namespace koi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Circle? fishFood;
        static double mouseX;
        static double mouseY;
        Pool pool;
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            pool = new Pool(Image.Height, Image.Width, 100);
            timer = new();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            pool.Advance(fishFood != null);
            Image.Source = BmpImageFromBmp(RenderPool(pool));
        }

        public static System.Drawing.Point[] fishOutline =
        {
            new System.Drawing.Point(0, 0),
            new System.Drawing.Point(0, 10),
            new System.Drawing.Point(-1, 8),
            new System.Drawing.Point(-1, 0),
            new System.Drawing.Point(0, -3),
            new System.Drawing.Point(-2, -5),
            new System.Drawing.Point(2, -5),
            new System.Drawing.Point(0, -3),
            new System.Drawing.Point(1, 0),
            new System.Drawing.Point(1, 8),
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

        public static void RenderFishFood(Graphics gfx)
        {
            if (fishFood == null) return;
            using (var brush = new SolidBrush(System.Drawing.Color.Bisque))
            {
                gfx.FillEllipse(brush, x: (int)fishFood.CenterX, y: (int)fishFood.CenterY, width: (int)fishFood.Radius, height: (int)fishFood.Radius);
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
                RenderFishFood(gfx);
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


        //protected override async void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);

        //    pool.predaX = e.GetPosition(Image).X;
        //    pool.predaY = e.GetPosition(Image).Y;

        //    if (fishFood != null)
        //    {
        //        fishFood.CenterX = (double)pool.predaX;
        //        fishFood.CenterY = (double)pool.predaY;
        //    }

        //}

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            pool.predaX = e.GetPosition(Image).X;
            pool.predaY = e.GetPosition(Image).Y;

            if (fishFood != null)
            {
                fishFood.CenterX = (double)pool.predaX;
                fishFood.CenterY = (double)pool.predaY;
            }

        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (fishFood != null)
            {
                fishFood = null;
            }
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            pool.predaX = pool.predaY = -1;
            fishFood = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Canvas.ActualHeight is not double.NaN)
            {
                Image.Width = Canvas.ActualWidth;
                Image.Height = Canvas.ActualHeight;
                pool.Height = Canvas.ActualHeight;
                pool.Width = Canvas.ActualWidth;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                fishFood = new(Mouse.GetPosition(Canvas).X, Mouse.GetPosition(Canvas).Y,  20);
            }
        }




    }
}
