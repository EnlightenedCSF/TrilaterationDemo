using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using TrilaterationDemo.Model;

namespace TrilaterationDemo.TrilateratingStrategy
{
    //TODO: продебажить ситуацию с 8О
    /// <summary>
    /// Enhanced Positioning Trilateration Algorithm
    /// </summary>
    public class EptaStrategy : AbstractStrategy
    {
        private const double KStep = 0.05f; 

        public EptaStrategy(Floor floor) : base(floor)
        {
        }

        private static List<Beacon> GetCopy(IEnumerable<Beacon> beacons)
        {
            return beacons.Select(beacon => new Beacon
            {
                Accuracy = beacon.Accuracy, X = beacon.X, Y = beacon.Y
            }).ToList();
        }

        List<Beacon> _beacons = new List<Beacon>
            {
                new Beacon{X = 0.5f, Y = 0.5f, Accuracy = 1},
                new Beacon{X = 0.5f, Y = 2.5f, Accuracy = 1},
                new Beacon{X = 2.4f, Y = 1.42f, Accuracy = 2.65f}
            };

        public override PointF CalculateUserPosition()
        {
            var beacons = GetCopy(Floor.GetFloor().Beacons);

            //var beacons = _beacons;
            
            var center = new PointF(-100, -100);
            var isCenterInitialized = false;
            var iterations = 0;
            while (true)
            {
                iterations++;
                var intersectionPoints = GetIntersectionPoints(beacons);
                var innerPoints = intersectionPoints.Where(intersection => IsPointBelongToAllCircles(intersection, beacons))
                    .ToList();

                if (innerPoints.Count == 3)
                {
                    if (innerPoints.Count >= 2)
                    {
                        center = GetCenter(innerPoints);
                        isCenterInitialized = true;
                    }
                    else {
                        break;
                    }

                    foreach (var beacon in beacons)
                    {
                        beacon.Accuracy *= (float)(1 - KStep);
                    }
                }
                else
                {
                    if (isCenterInitialized)
                    {
                        break;
                    }
                    if (innerPoints.Count == 2 && IsOneIsTooBig(beacons))
                    {
                        center = GetCenter(innerPoints);
                        break;
                    }

                    foreach (var beacon in beacons)
                    {
                        beacon.Accuracy *= (float)(1 + 2*KStep);
                    }
                }
            }
            return center;
        }

        private static bool IsOneIsTooBig(List<Beacon> beacons)
        {
            beacons.Sort((b1, b2) => b1.Accuracy > b2.Accuracy ? -1 : 1);

            var result = true;
            foreach (var beacon in beacons)
            {
                var isAllIn = true;
                foreach (var innerBeacon in beacons)
                {
                    if (innerBeacon == beacon)
                        continue;
                    if (GetDistanceBetween(innerBeacon.PositionAsPoint(), beacon.PositionAsPoint()) >= beacon.Accuracy)
                    {
                        isAllIn = false;
                        break;
                    }
                }
                result &= isAllIn;
                if (result)
                    return true;
            }
            return result;
        }

        private static IEnumerable<PointF> GetIntersectionPoints(IReadOnlyList<Beacon> beacons)
        {
            var res = new List<PointF>();
            for (var i = 0; i < beacons.Count; i++)
            {
                for (var j = i+1; j < beacons.Count; j++)
                {
                    var intersect = GetCircleCircleIntersections(beacons[i], beacons[j]);
                    res.AddRange(intersect);
                }
            }
            return res;
        }

        private static IEnumerable<PointF> GetCircleCircleIntersections(Beacon b1, Beacon b2)
        {
            var p1 = new PointF(b1.X, b1.Y);
            var p2 = new PointF(b2.X, b2.Y);
            var d = GetDistanceBetween(p1, p2);
            var r1 = b1.Accuracy;
            var r2 = b2.Accuracy;

            // if to far away, or self contained - can't be done
            if ((d >= (r1 + r2)) || (d <= Math.Abs(r1 - r2))) {
                return new List<PointF>();
            }

            var a = (r1*r1 - r2*r2 + d*d)/(2*d);
            var h = Math.Sqrt(r1*r1 - a*a);
            var x0 = p1.X + a*(p2.X - p1.X)/d;
            var y0 = p1.Y + a*(p2.Y - p1.Y)/d;
            var rx = -(p2.Y - p1.Y)*(h/d);
            var ry = -(p2.X - p1.X)*(h/d);

            return new List<PointF>
            {
                new PointF((float) (x0 + rx), (float) (y0 - ry)),
                new PointF((float) (x0 - rx), (float) (y0 + ry))
            };
        }

        public static double GetDistanceBetween(PointF p1, PointF p2)
        {
            var d = Math.Sqrt((p1.X - p2.X)*(p1.X - p2.X) + (p1.Y - p2.Y)*(p1.Y - p2.Y));
            return d;
        }

        private static bool IsPointBelongToAllCircles(PointF p, IEnumerable<Beacon> beacons)
        {
            return beacons.All(b => !(GetDistanceBetween(p, new PointF(b.X, b.Y)) > (b.Accuracy + 1e-2)));
        }

        private static PointF GetCenter(IReadOnlyCollection<PointF> points)
        {
            var center = new PointF(0, 0);
            foreach (var intersect in points)
            {
                center.X += intersect.X;
                center.Y += intersect.Y;
            }
            center.X /= points.Count;
            center.Y /= points.Count;

            return center;
        }
    }
}