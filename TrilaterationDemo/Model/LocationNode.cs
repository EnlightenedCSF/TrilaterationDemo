using System.Drawing;

namespace TrilaterationDemo.Model
{
    public class LocationNode : LocationPoint
    {
        public string Label { get; set; }

        public PointF AsPointF()
        {
            return new PointF(X, Y);
        }

        public override string ToString()
        {
            return "X: " + X + ";  Y: " + Y;
        }
    }
}
