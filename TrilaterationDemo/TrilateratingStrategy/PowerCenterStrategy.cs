using System;
using System.Drawing;

namespace TrilaterationDemo.TrilateratingStrategy
{
    public class PowerCenterStrategy : AbstractStrategy
    {
        public PowerCenterStrategy(Floor floor)
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

            var k1 = (p1.X*p1.X + p1.Y*p1.Y - distA*distA)/2f;
            var k2 = (p2.X*p2.X + p2.Y*p2.Y - distB*distB)/2f;
            var k3 = (p3.X*p3.X + p3.Y*p3.Y - distC*distC)/2f;

            var d = (p1.X - p2.X)*(p2.Y - p3.Y) - (p1.Y - p2.Y)*(p2.X - p3.X);
            if (Math.Abs(d) < 1e-7)
            {
                return new PointF(-1, -1);
            }

            var x = (k1 - k2)*(p2.Y - p3.Y) - (p1.Y - p2.Y)*(k2 - k3);
            var y = (p1.X - p2.X)*(k2 - k3) - (k1 - k2)*(p2.X - p3.X);
            //Floor.UserPosition = new PointF(x/d, y/d);
            return new PointF(x/d, y/d);
        }
    }
}
