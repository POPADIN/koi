using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace koi
{
    public class Fish
    {
        public double X;
        public double Y;
        public double Xvel;
        public double Yvel;

        public Fish(double x, double y, double xvel, double yvel)
        {
            X = x;
            Y = y;
            Xvel = xvel;
            Yvel = yvel;
        }

        public Fish(Random random, double width, double height)
        {
            X = random.NextDouble() * width;
            Y = random.NextDouble() * height;
            Xvel = (random.NextDouble() - 0.5);
            Yvel = (random.NextDouble() - 0.5);
        }

        public double GetDistance(Fish f)
        {
            return Math.Sqrt(Math.Pow((f.Y - this.Y), 2) + Math.Pow((f.X - this.X), 2)); 
        }

        public double GetDistance(double x, double y)
        {
            return Math.Sqrt(Math.Pow((y - this.Y), 2) + Math.Pow((x - this.X), 2));
        }

        public double GetSpeed()
        {
            return Math.Sqrt(Xvel * Xvel + Yvel * Yvel);
        }

        public double GetAngle()
        {
            if (double.IsNaN(Xvel) || double.IsNaN(Yvel))
                return 0;
            if (Xvel == 0 && Yvel == 0)
                return 0;
            double angle = Math.Atan(Yvel / Xvel) * 180 / Math.PI - 90;
            if (Xvel < 0)
                angle += 180;
            return angle;
        }

        public void MoveForward(double minSpeed = 1, double maxSpeed = 6)
        {
            X += Xvel;
            Y += Yvel;

            var speed = GetSpeed();
            if (speed > maxSpeed)
            {
                Xvel = (Xvel / speed) * maxSpeed;
                Yvel = (Yvel / speed) * maxSpeed;
            }
            else if (speed < minSpeed)
            {
                Xvel = (Xvel / speed) * minSpeed;
                Yvel = (Yvel / speed) * minSpeed;
            }

            if (double.IsNaN(Xvel)) { Xvel = 0; }
            if (double.IsNaN(Yvel)) { Yvel = 0; }
        }


    }
}
