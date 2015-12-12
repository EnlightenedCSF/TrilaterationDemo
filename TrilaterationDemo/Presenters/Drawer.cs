using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TrilaterationDemo.Model;

namespace TrilaterationDemo.Presenters
{
    public class Drawer : AbstractDrawer
    {
        private List<Beacon> _beacons;
        private event EventHandler NeedToRecalculate;

        private bool _isBeaconRadiusSelected;
        private bool _changingAccuracy;
        
        private const float AccuracyRadiusLineWidth = 2;
        private const float SelectedAccuracyRadiusLineWidth = 4;
        
        private const float UserPointSize = 8;
        private const float UserExtraPointSize = 12;

        public Drawer(Image bitmap) : base(bitmap)
        {
            _beacons = Floor.GetFloor().Beacons;
            
            NeedToRecalculate += (sender, args) => Floor.GetFloor().CalculateUserPosition();
            Reset();
            Draw();
        }

        protected override sealed void Reset()
        {
            _isBeaconRadiusSelected = false;
            _changingAccuracy = false;

            Floor.GetFloor().Reset();
            _beacons = Floor.GetFloor().Beacons;

            base.Reset();
        }

        #region Events

        public override void SetNeedsDisplay()
        {
            base.SetNeedsDisplay();
            NeedToRecalculate.Invoke(this, null);
        }

        public override void OnMouseDown(double x, double y)
        {
            if (_isBeaconRadiusSelected && (SelectedPoint != null))
            {
                _changingAccuracy = true;
            }
            base.OnMouseDown(x, y);
        }

        public override void OnMouseMove(float x, float y)
        {
            if ((!IsPointSelected) && (!_isBeaconRadiusSelected))
            {
                foreach (var beacon in _beacons.Where(beacon => IsOverPoint(beacon, x, y)))
                {
                    IsPointSelected = true;
                    SelectedPoint = beacon;
                    break;
                }
                
                foreach (var beacon in _beacons.Where(beacon => IsOverBeaconAccuracyRadius(beacon, x, y)))
                {
                    _isBeaconRadiusSelected = true;
                    SelectedPoint = beacon;
                    break;
                }
            }
            else
            {
                if (Dragging)
                {
                    SelectedPoint.RefreshPosition(x - ShiftX, y - ShiftY, MetersToPixels);
                    NeedToRecalculate.Invoke(this, null);
                }
                else if (_changingAccuracy)
                {
                    var beacon = SelectedPoint as Beacon;
                    if (beacon != null)
                        beacon.RefreshAccuracy(x - ShiftX, y - ShiftY, MetersToPixels);
                    NeedToRecalculate.Invoke(this, null);
                }

                if (IsNotOverSelectedPoint(x, y))
                {
                    IsPointSelected = false;
                }

                if (IsNotOverSelectedBeaconAccuracyRadius(x, y))
                {
                    _isBeaconRadiusSelected = false;
                }
            }

            Draw();
        }

        public override void OnMouseUp()
        {
            _changingAccuracy = false;
            base.OnMouseUp();
        }
        #endregion

        #region Drawing

        protected override sealed void Draw()
        {
            base.Draw();

            foreach (var beacon in _beacons)
            {
                #region Drawing beacons
                if (IsPointSelected && SelectedPoint == beacon)
                {
                    Graphics.FillEllipse(new SolidBrush(Color.Blue),
                        new RectangleF
                        {
                            Location = new PointF(ShiftX + beacon.X*MetersToPixels - SelectedPointSize/2,
                                ShiftY + beacon.Y*MetersToPixels - SelectedPointSize/2),
                            Size = new SizeF(SelectedPointSize, SelectedPointSize)
                        });
                }
                else
                {
                    Graphics.FillEllipse(new SolidBrush(Color.CornflowerBlue),
                       new RectangleF
                       {
                           Location = new PointF(ShiftX + beacon.X * MetersToPixels - PointSize / 2,
                               ShiftY + beacon.Y * MetersToPixels - PointSize / 2),
                           Size = new SizeF(PointSize, PointSize)
                       });
                }
                #endregion

                #region Drawing beacon radiuses
                if (_isBeaconRadiusSelected && SelectedPoint == beacon)
                {
                    Graphics.DrawEllipse(new Pen(Color.Salmon, SelectedAccuracyRadiusLineWidth),
                    new RectangleF
                    {
                        Location = new PointF(ShiftX + (beacon.X - beacon.Accuracy) * MetersToPixels,
                                              ShiftY + (beacon.Y - beacon.Accuracy) * MetersToPixels),
                        Size = new SizeF((2 * beacon.Accuracy) * MetersToPixels, (2 * beacon.Accuracy) * MetersToPixels),
                    });
                }
                else
                {
                    Graphics.DrawEllipse(new Pen(Color.Salmon, AccuracyRadiusLineWidth),
                    new RectangleF
                    {
                        Location = new PointF(ShiftX + (beacon.X - beacon.Accuracy) * MetersToPixels,
                                              ShiftY + (beacon.Y - beacon.Accuracy) * MetersToPixels),
                        Size = new SizeF((2 * beacon.Accuracy) * MetersToPixels, (2 * beacon.Accuracy) * MetersToPixels),
                    });
                }
                #endregion
            }

            if (!Floor.GetFloor().IsUsingBothStrategies)
            {
                #region Draw single user position
                Graphics.DrawEllipse(new Pen(Color.Teal, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels*Floor.GetFloor().UserPositions[0].X + ShiftX - UserExtraPointSize/2,
                                MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                Graphics.FillEllipse(new SolidBrush(Color.LimeGreen),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                #endregion
            }
            else
            {
                #region Trilateration
                Graphics.DrawEllipse(new Pen(Color.Teal, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                Graphics.FillEllipse(new SolidBrush(Color.LimeGreen),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                Graphics.DrawString("CI", Font, new SolidBrush(Color.LimeGreen), 
                    MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - LabelShift, 
                    CenterStringFormat);
                #endregion

                #region Ray tracing
                Graphics.DrawEllipse(new Pen(Color.DarkBlue, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                Graphics.FillEllipse(new SolidBrush(Color.DeepSkyBlue),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                Graphics.DrawString("RT", Font, new SolidBrush(Color.DarkBlue),
                    MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - LabelShift, 
                    CenterStringFormat);
                #endregion

                #region Power center
                Graphics.DrawEllipse(new Pen(Color.DarkOrange, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                Graphics.FillEllipse(new SolidBrush(Color.Orange),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                Graphics.DrawString("PC", Font, new SolidBrush(Color.DarkOrange),
                    MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - LabelShift, 
                    CenterStringFormat);
                #endregion
            }
        }

        #endregion

        #region Extra Methods
        private static bool IsOverBeaconAccuracyRadius(Beacon beacon, float x, float y)
        {
            var dx = MetersToPixels * beacon.X + ShiftX - x;
            var dy = MetersToPixels * beacon.Y + ShiftY - y;
            var dist = Math.Sqrt(dx*dx + dy*dy);
            var acc = MetersToPixels*beacon.Accuracy;
            return Math.Abs(dist - acc) < 12;
        }

        private bool IsNotOverSelectedBeaconAccuracyRadius(float x, float y)
        {
            if (SelectedPoint == null)
                return true;
            var beacon = SelectedPoint as Beacon;
            if (beacon != null)
                return !IsOverBeaconAccuracyRadius(beacon, x, y);
            return false;
        }
        #endregion
    }
}