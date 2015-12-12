namespace TrilaterationDemo.Model
{
    public abstract class LocationPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public void RefreshPosition(float x, float y, float ratio)
        {
            X = x / ratio;
            Y = y / ratio;
        }
    }
}
