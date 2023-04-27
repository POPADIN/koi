using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace koi
{
    public class Pool
    {
        public double Width;
        public double Height;
        public double predaX = 0;
        public double predaY = 0;
        public readonly List<Fish> School = new();
        private readonly Random random = new();

        public Pool(double height, double width, int fishCount = 100)
        {
            Width = width;
            Height = height;
            for (int i = 0; i < fishCount; i++)
                School.Add(new Fish(random, width, height));
        }

        public void Advance(bool attract = false)
        {
            double predaFactor = 1;
            if (attract) predaFactor = -1;
            foreach (var fish in School)
            {
                (double flockXvel, double flockYvel) = Flock(fish, 50, 0.0002);
                (double alignXvel, double alignYvel) = Align(fish, 250, 0.0002);
                (double avoidXvel, double avoidYvel) = Avoid(fish, 100, 0.00001);
                (double predaXvel, double predaYvel) = Preda(fish, 150, 0.0004);
                fish.Xvel += flockXvel + alignXvel + avoidXvel + predaXvel * predaFactor;
                fish.Yvel += flockYvel + alignYvel + avoidYvel + predaYvel * predaFactor;
            }

            foreach (var fish in School)
            {
                fish.MoveForward();
                BounceOffWall(fish);
            }
        }

        public (double xvel, double yvel) Flock(Fish fish, double distance, double power)
        {
            var neighbors = School.Where(x => x.GetDistance(fish) < distance);
            double meanX = neighbors.Sum(x => x.X) / neighbors.Count();
            double meanY = neighbors.Sum(x => x.Y) / neighbors.Count();
            double deltaCenterX = meanX - fish.X;
            double deltaCenterY = meanY - fish.Y;
            return (deltaCenterX * power, deltaCenterY * power);
        }

        public (double xvel, double yvel) Align(Fish fish, double distance, double power)
        {
            var neighbors = School.Where(x => x.GetDistance(fish) < distance);
            double meanXvel = neighbors.Sum(x => x.Xvel) / neighbors.Count();
            double meanYvel = neighbors.Sum(x => x.Yvel) / neighbors.Count();
            double deltaXvel = meanXvel - fish.X;
            double deltaYvel = meanYvel - fish.Y;
            return (deltaXvel * power, deltaYvel * power);
        }

        public (double xvel, double yvel) Avoid(Fish fish, double distance, double power)
        {
            var neighbors = School.Where(x => x.GetDistance(fish) < distance);
            (double sumClosenessX, double sumClosenessY) = (0, 0);
            foreach (var neighbor in neighbors)
            {
                double closeness = distance - neighbor.GetDistance(fish);
                sumClosenessX += (fish.X - neighbor.X) * closeness;
                sumClosenessY += (fish.Y - neighbor.Y) * closeness;
            }
            return (sumClosenessX * power, sumClosenessY * power);
        }

        public (double xvel, double yvel) Preda(Fish fish, double distance, double power)
        {
            (double sumClosenessX, double sumClosenessY) = (0, 0);
            if (predaX < 0 || predaY < 0) return (0, 0);

            double distanceAway = fish.GetDistance((double)predaX, (double)predaY);

            if (distanceAway < distance)
            {
                double closeness = distance - distanceAway;
                sumClosenessX += (fish.X - (double)predaX) * closeness;
                sumClosenessY += (fish.Y - (double)predaY) * closeness;
            }

            return (sumClosenessX * power, sumClosenessY * power);
        }

        public void BounceOffWall(Fish fish, double pad = 40, double turn = .75)
        {
            if (fish.X < pad)           fish.Xvel += turn;
            if (fish.X > Width - pad)   fish.Xvel -= turn;
            if (fish.Y < pad)           fish.Yvel += turn;
            if (fish.Y > Height - pad)  fish.Yvel -= turn;
        }
    }
}
