using System;
using System.Drawing;
using TrilaterationDemo.Model;

namespace TrilaterationDemo.Presenters
{
    public abstract class AbstractDrawer
    {
        protected Graphics Graphics; 

        protected bool IsPointSelected;
        protected LocationPoint SelectedPoint;
        protected bool Dragging;
        
        protected const float ShiftX = 150;  
        protected const float ShiftY = 70;   
        protected const float MetersToPixels = 100;   
        protected const float PointSize = 10;    
        protected const float SelectedPointSize = 14;

        protected const float LabelShift = 10;

        protected readonly Font Font;
        protected readonly StringFormat CenterStringFormat;

        protected AbstractDrawer(Image image)
        {
            Font = new Font("Arial", 11f, FontStyle.Bold);
            CenterStringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            Graphics = Graphics.FromImage(image);            
        }

        protected PointF PositionInPixels(PointF posInMeters)
        {
            return new PointF(posInMeters.X * MetersToPixels + ShiftX, posInMeters.Y * MetersToPixels + ShiftY);
        }

        protected virtual void Reset()
        {
            IsPointSelected = false;
            Dragging = false;
            Draw();
        }

        protected virtual void Draw()
        {
            Graphics.Clear(Color.White);
        }

        #region Event handling
        public virtual void SetNeedsDisplay()
        {
            Draw();
        }

        public virtual  void OnMouseDown(double x, double y)
        {
            if (IsPointSelected && (SelectedPoint != null))
            {
                Dragging = true;
            }
            Draw();
        }

        public abstract void OnMouseMove(float x, float y);

        public virtual void OnMouseUp()
        {
            Dragging = false;
            Draw();
        }
        #endregion

        #region Extra methods
        protected static bool IsOverPoint(LocationPoint beacon, float x, float y)
        {
            var dx = MetersToPixels * beacon.X + ShiftX - x;
            var dy = MetersToPixels * beacon.Y + ShiftY - y;
            return Math.Sqrt(dx * dx + dy * dy) < SelectedPointSize;
        }

        protected bool IsNotOverSelectedPoint(float x, float y)
        {
            if (SelectedPoint == null)
                return true;
            return !IsOverPoint(SelectedPoint, x, y);
        }
        #endregion

    }
}
