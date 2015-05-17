using System.Drawing;

namespace TrilaterationDemo.TrilateratingStrategy
{
    public abstract class AbstractStrategy
    {
        protected Floor Floor { get; set; }

        protected AbstractStrategy(Floor floor)
        {
            Floor = floor;
        }

        public abstract PointF CalculateUserPosition();
    }
}
