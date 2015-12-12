using System;
using System.Drawing;
using TrilaterationDemo.TrilateratingStrategy;

namespace TrilaterationDemo.Model
{
    public class Attractor
    {
        public Graph Graph { get; set; }
        public float AttractionPower { get; set; }
        public float DeadZone { get; set; }

        public Attractor()
        {
            AttractionPower = 0.1f;
            DeadZone = 0.2f; //meters
        }

        public PointF UpdateUserPosition(PointF userPosition)
        {
            var rootNodes = Graph.RootNodes;
            var totalShiftX = 0.0d;
            var totalShiftY = 0.0d;
            foreach (var node in rootNodes)
            {
                var distance = EptaStrategy.GetDistanceBetween(userPosition, node.AsPointF());
                if (distance < DeadZone)
                    continue;
                var shift = (1/(distance * distance * 100)) * 100 * AttractionPower;
                var dx = (userPosition.X - node.X) / distance * shift;
                var dy = (userPosition.Y - node.Y) / distance * shift;
                var length = EptaStrategy.GetDistanceBetween(new PointF(0, 0), new PointF((float) dx, (float) dy));
                if (length > distance)
                {
                    dx /= length / distance;
                    dy /= length / distance;
                }   
                totalShiftX += dx;
                totalShiftY += dy;
            }
            return new PointF((float)(userPosition.X - totalShiftX), (float)(userPosition.Y - totalShiftY));
        }

        public PointF GetLocationOfNearestBeacon(PointF userPosition)
        {
            var distance = float.MaxValue;
            var selectedNode = new LocationNode{X = int.MinValue, Y = int.MinValue};
            foreach (var node in Graph.RootNodes)
            {
                var newDist = EptaStrategy.GetDistanceBetween(userPosition, node.AsPointF());
                if (!(newDist < distance)) 
                    continue;
                distance = (float) newDist;
                selectedNode = node;
            }
            return selectedNode.AsPointF();
        }
    }
}
