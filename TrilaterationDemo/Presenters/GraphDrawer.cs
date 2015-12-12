using System.Drawing;
using System.Linq;
using TrilaterationDemo.Model;

namespace TrilaterationDemo.Presenters
{
    public class GraphDrawer : AbstractDrawer
    {
        private readonly Graph _graph;
        private readonly Attractor _attractor;

        //private float _modifiedX, _modifiedY;
        private PointF _modifiedPos, _mousePos;
        //private float _mouseX, _mouseY;
        private PointF _nearestBeacon;

        private readonly Pen _bluePen;
        private readonly Pen _grayPen; 
        private readonly Pen _dodgerPen;
        private readonly Pen _dashedPen;

        private const float CrossSize = 4;

        public GraphDrawer(Image image, Attractor attractor): base(image)
        {
            _graph = attractor.Graph;
            _attractor = attractor;
            _bluePen = new Pen(Color.Blue, 3);
            _grayPen = new Pen(Color.SlateGray, 2);
            _dodgerPen = new Pen(Color.DodgerBlue, 2);
            _dashedPen = new Pen(Color.LightGray) {DashPattern = new[] {1f, 1f, 1f}};
        }


        public override void OnMouseMove(float x, float y)
        {
            x -= ShiftX;
            y -= ShiftY;
            var userPosition = new PointF(x/MetersToPixels, y/MetersToPixels);
            var p = _attractor.UpdateUserPosition(userPosition);
            _nearestBeacon = _attractor.GetLocationOfNearestBeacon(userPosition);
            _modifiedPos = PositionInPixels(p);
            _mousePos = new PointF(x + ShiftX, y + ShiftY);
            Draw();
        }

        protected override void Draw()
        {
            base.Draw();

            var list = _graph.AdjacencyList;
            foreach (var nodeList in list)
            {
                var node = nodeList[0];

                #region Drawing edges
                foreach (var anotherNode in nodeList.Where(anotherNode => anotherNode != node))
                {
                    Graphics.DrawLine(_bluePen,
                        new PointF(ShiftX + node.X * MetersToPixels,
                            ShiftY + node.Y * MetersToPixels),
                        new PointF(ShiftX + anotherNode.X * MetersToPixels,
                            ShiftY + anotherNode.Y * MetersToPixels));
                }
                #endregion

                #region Drawing points

                var color = Color.Orange;

                var location = new PointF(ShiftX + node.X * MetersToPixels - PointSize / 2,
                        ShiftY + node.Y * MetersToPixels - PointSize / 2);

                var size = new SizeF(PointSize, PointSize);

                Graphics.FillEllipse(new SolidBrush(color),
                    new RectangleF
                    {
                        Location = location,
                        Size = size
                    });
                #endregion

                #region Draw Labels
                location.X += 13;
                location.Y -= 10;
                Graphics.DrawString(node.Label, Font, new SolidBrush(Color.Navy), location, CenterStringFormat);
                #endregion
            }

            #region Drawing mouse cursor
            Graphics.DrawLine(_grayPen, _modifiedPos.X - CrossSize, _modifiedPos.Y - CrossSize, _modifiedPos.X + CrossSize, _modifiedPos.Y + CrossSize);
            Graphics.DrawLine(_grayPen, _modifiedPos.X - CrossSize, _modifiedPos.Y + CrossSize, _modifiedPos.X + CrossSize, _modifiedPos.Y - CrossSize);

            Graphics.DrawLine(_dodgerPen, _mousePos.X - CrossSize, _mousePos.Y - CrossSize, _mousePos.X + CrossSize, _mousePos.Y + CrossSize);
            Graphics.DrawLine(_dodgerPen, _mousePos.X - CrossSize, _mousePos.Y + CrossSize, _mousePos.X + CrossSize, _mousePos.Y - CrossSize);

            Graphics.DrawLine(_dashedPen, _mousePos, PositionInPixels(_nearestBeacon));
            #endregion
        }
    }
}
