using System;

namespace TrilaterationDemo
{
    public class Beacon
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Accuracy { get; set; }

        public void RefreshPosition(float x, float y, float ratio)
        {
            X = x/ratio;
            Y = y/ratio;
        }

        public void RefreshAccuracy(float x, float y, float ratio)
        {
            var xM = x/ratio - X;
            var yM = y/ratio - Y;
            Accuracy = (float) Math.Sqrt(xM*xM + yM*yM);
        }
    }
}
