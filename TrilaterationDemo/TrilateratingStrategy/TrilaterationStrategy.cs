using System;
using System.Drawing;

namespace TrilaterationDemo.TrilateratingStrategy
{
    public class TrilaterationStrategy : AbstractStrategy
    {
        public TrilaterationStrategy(Floor floor)
            : base(floor)
        {
        }

        public override PointF CalculateUserPosition()
        {
            var floor = Floor.GetFloor();
            var p1 = floor.Beacons[0];
            var p2 = floor.Beacons[1];
            var p3 = floor.Beacons[2];

            var distA = floor.Beacons[0].Accuracy;
            var distB = floor.Beacons[1].Accuracy;
            var distC = floor.Beacons[2].Accuracy;

            var temp = (p2.X - p1.X)*(p2.X - p1.X) + (p2.Y - p1.Y)*(p2.Y - p1.Y);
            var ex = new PointF((float) ((p2.X - p1.X)/Math.Sqrt(temp)), (float) ((p2.Y - p1.Y)/Math.Sqrt(temp)));

            var p3P1 = new PointF(p3.X - p1.X, p3.Y - p1.Y);
            var ival = ex.X*p3P1.X + ex.Y*p3P1.Y;

            var p3P1I = (p3.X - p1.X - ex.X)*(p3.X - p1.X - ex.X) + (p3.Y - p1.Y - ex.Y)*(p3.Y - p1.Y - ex.Y);
            var ey = new PointF((float) ((p3.X - p1.X - ex.X)/Math.Sqrt(p3P1I)),
                (float) ((p3.Y - p1.Y - ex.Y)/Math.Sqrt(p3P1I)));

            var ez = new PointF(0, 0);
            var d = Math.Sqrt(temp);

            var jval = (ey.X*p3P1.X) + (ey.Y*p3P1.Y);

            var xval = (distA*distA - distB*distB + d*d)/(2*d);
            var yval = (distA*distA - distC*distC + ival*ival + jval*jval)/(2*jval) - (ival/jval)*xval;
            const int zval = 0;

            return new PointF((float)(p1.X + ex.X * xval + ey.X * yval + ez.X * zval),
                (float)(p1.Y + ex.Y * xval + ey.Y * yval + ez.Y * zval));
        }
    }
}
