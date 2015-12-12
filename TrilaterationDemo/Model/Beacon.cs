using System;
using System.Drawing;

namespace TrilaterationDemo.Model
{
    public class Beacon : LocationPoint
    {
        public float Accuracy { get; set; }

        public void RefreshAccuracy(float x, float y, float ratio)
        {
            var xM = x/ratio - X;
            var yM = y/ratio - Y;
            Accuracy = (float) Math.Sqrt(xM*xM + yM*yM);
        }

        public PointF PositionAsPoint()
        {
            return new PointF(X, Y);
        }
    }
}
