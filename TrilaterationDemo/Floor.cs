using System.Collections.Generic;
using System.Drawing;
using TrilaterationDemo.TrilateratingStrategy;

namespace TrilaterationDemo
{
    public class Floor
    {
        #region Singleton
        private static Floor _instance;

        public static Floor GetFloor()
        {
            return _instance ?? (_instance = new Floor());
        }

        private Floor()
        {
            Reset();
        }
        #endregion

        private AbstractStrategy _strategy;
        public List<Beacon> Beacons { get; set; }
        public PointF UserPosition { get; set; }

        public void Reset()
        {
            Beacons = new List<Beacon>
            {
                new Beacon {Accuracy = 1, X = 0.5f, Y = 0.5f},
                new Beacon {Accuracy = 1, X = 2.5f, Y = 0.5f},
                new Beacon {Accuracy = 1, X = 0.5f, Y = 2.5f}
            };
            UserPosition = new PointF(-100, -100);
        }

        public void ChangeTrilaterationMethod(AbstractStrategy strategy)
        {
            _strategy = strategy;
        }

        public void CalculateUserPosition()
        {
            _strategy.CalculateUserPosition();
        }
    }
}
