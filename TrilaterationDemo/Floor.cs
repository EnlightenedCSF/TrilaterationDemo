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
            _strategies = new List<AbstractStrategy>(3)
            {
                new TrilaterationStrategy(this),
                new RayTracingStrategy(this),
                new PowerCenterStrategy(this)
            };
            Reset();
        }
        #endregion

        private readonly List<AbstractStrategy> _strategies;
        public bool IsUsingBothStrategies { get; private set; }
        private int _usingStrategyIndex;
        public List<Beacon> Beacons { get; private set; }
        public List<PointF> UserPositions { get; private set; }

        public void Reset()
        {
            Beacons = new List<Beacon>
            {
                new Beacon {Accuracy = 1, X = 0.5f, Y = 0.5f},
                new Beacon {Accuracy = 1, X = 2.5f, Y = 0.5f},
                new Beacon {Accuracy = 1, X = 0.5f, Y = 2.5f}
            };
            UserPositions = new List<PointF>(3) {new PointF(-100, -100), new PointF(-100, -100), new PointF(-100, -100)};
        }

        public void ChangeTrilaterationMethod(int index)
        {
            IsUsingBothStrategies = false;
            _usingStrategyIndex = index;
        }

        public void UseBothMethods()
        {
            IsUsingBothStrategies = true;
        }

        public void CalculateUserPosition()
        {
            if (!IsUsingBothStrategies)
            {
                UserPositions[0] = _strategies[_usingStrategyIndex].CalculateUserPosition();
            }
            else
            {
                for (var i = 0; i < _strategies.Count; i++)
                {
                    UserPositions[i] = _strategies[i].CalculateUserPosition();
                }
            }
        }
    }
}
