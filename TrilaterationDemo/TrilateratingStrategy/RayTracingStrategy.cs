using System;
using System.Drawing;

namespace TrilaterationDemo.TrilateratingStrategy
{
    public class RayTracingStrategy : AbstractStrategy
    {
        public RayTracingStrategy(Floor floor) : base(floor)
        {
        }

        public override PointF CalculateUserPosition()
        {
            var wasIntersectionOfAllThree = false;
            var floor = Floor.GetFloor();
            var xMin = floor.Beacons[0].X;
            var xMax = xMin;            
            var yMax = floor.Beacons[0].Y;
            var yMin = yMax;

            foreach (var beacon in floor.Beacons)
            {
                var x = beacon.X;
                var y = beacon.Y;
                var r = beacon.Accuracy;

                if (x - r < xMin)
                {
                    xMin = x - r;
                }
                if (x + r > xMax)
                {
                    xMax = x + r;
                }
                if (y - r < yMin)
                {
                    yMin = y - r;
                }
                if (y + r > yMax)
                {
                    yMax = y + r;
                }
            }

            var xMinUser = 1e10f;
            var xMaxUser = -1e10f;
            var yMinUser = 1e10f;
            var yMaxUser = -1e10f;
            for (var i = xMin; i < xMax; i += 0.07f)
            {
                for (var j = yMin; j < yMax; j += 0.07f)
                {
                    if (IsRayIntersectWithBeaconAccuracy(new PointF(i, j), floor.Beacons[0]) &&
                        IsRayIntersectWithBeaconAccuracy(new PointF(i, j), floor.Beacons[1]) &&
                        IsRayIntersectWithBeaconAccuracy(new PointF(i, j), floor.Beacons[2]))
                    {
                        wasIntersectionOfAllThree = true;
                        if (i < xMinUser)
                        {
                            xMinUser = i;
                        }
                        if (i > xMaxUser)
                        {
                            xMaxUser = i;
                        }

                        if (j < yMinUser)
                        {
                            yMinUser = j;
                        }
                        if (j > yMaxUser)
                        {
                            yMaxUser = j;
                        }
                    }
                }
            }
            return wasIntersectionOfAllThree ? 
                new PointF((xMinUser + xMaxUser) / 2.0f, (yMinUser + yMaxUser) / 2.0f) : 
                new PointF(-100, -100);
        }

        private static bool IsRayIntersectWithBeaconAccuracy(PointF ray, Beacon beacon)
        {
            if (beacon == null)
                return false;

            var x = beacon.X;
            var y = beacon.Y;
            var r = beacon.Accuracy;
            return r > Math.Sqrt((ray.X - x) * (ray.X - x) + (ray.Y - y) * (ray.Y - y));
        }
    }
}
