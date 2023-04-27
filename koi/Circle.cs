namespace koi
{
    public class Circle
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        
        public Circle(double centerX, double centerY, double radius = 4)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
        }
    }
}