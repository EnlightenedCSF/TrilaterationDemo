using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TrilaterationDemo
{
    public class Drawer
    {
        private readonly Graphics _graphics;
        private List<Beacon> _beacons;
        private event EventHandler NeedToRecalculate;

        private bool _isBeaconSelected, _isBeaconRadiusSelected;
        private Beacon _selectedBeacon;
        private bool _dragging, _changingAccuracy;

        private const float ShiftX = 150;
        private const float ShiftY = 70;
        private const float MetersToPixels = 100;
        private const float BeaconSize = 10;
        private const float SelectedBeaconSize = 14;
        private const float AccuracyRadiusLineWidth = 2;
        private const float SelectedAccuracyRadiusLineWidth = 4;
        private const float UserPointSize = 8;
        private const float UserExtraPointSize = 12;
        private const float LabelShift = 10;
        private readonly Font _font;
        private readonly StringFormat _centerStringFormat;

        public Drawer(Image bitmap)
        {
            _graphics = Graphics.FromImage(bitmap);
            _beacons = Floor.GetFloor().Beacons;
            _font = new Font("Arial", 10f);
            _centerStringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            NeedToRecalculate += (sender, args) => Floor.GetFloor().CalculateUserPosition();

            Reset();
            Draw();
        }

        public void Reset()
        {
            _isBeaconSelected = false;
            _isBeaconRadiusSelected = false;

            _dragging = false;
            _changingAccuracy = false;

            Floor.GetFloor().Reset();
            _beacons = Floor.GetFloor().Beacons;

            Draw();
        }

        #region Events

        public void SetNeedsDisplay()
        {
            NeedToRecalculate.Invoke(this, null);
            Draw();
        }

        public void OnMouseDown(double x, double y)
        {
            if (_isBeaconSelected && (_selectedBeacon != null))
            {
                _dragging = true;
            }
            if (_isBeaconRadiusSelected && (_selectedBeacon != null))
            {
                _changingAccuracy = true;
            }
            Draw();
        }

        public void OnMouseMove(float x, float y)
        {
            if ((!_isBeaconSelected) && (!_isBeaconRadiusSelected))
            {
                foreach (var beacon in _beacons.Where(beacon => IsOverBeacon(beacon, x, y)))
                {
                    _isBeaconSelected = true;
                    _selectedBeacon = beacon;
                    break;
                }
                
                foreach (var beacon in _beacons.Where(beacon => IsOverBeaconAccuracyRadius(beacon, x, y)))
                {
                    _isBeaconRadiusSelected = true;
                    _selectedBeacon = beacon;
                    break;
                }
            }
            else
            {
                if (_dragging)
                {
                    _selectedBeacon.RefreshPosition(x - ShiftX, y - ShiftY, MetersToPixels);
                    NeedToRecalculate.Invoke(this, null);
                }
                else if (_changingAccuracy)
                {
                    _selectedBeacon.RefreshAccuracy(x - ShiftX, y - ShiftY, MetersToPixels);
                    NeedToRecalculate.Invoke(this, null);
                }

                if (IsNotOverSelectedBeacon(x, y))
                {
                    _isBeaconSelected = false;
                }

                if (IsNotOverSelectedBeaconAccuracyRadius(x, y))
                {
                    _isBeaconRadiusSelected = false;
                }
            }

            Draw();
        }

        public void OnMouseUp()
        {
            _dragging = false;
            _changingAccuracy = false;
            Draw();
        }
        #endregion

        #region Drawing
        private void Draw()
        {
            _graphics.Clear(Color.White);

            foreach (var beacon in _beacons)
            {
                #region Drawing beacons
                if (_isBeaconSelected && _selectedBeacon == beacon)
                {
                    _graphics.FillEllipse(new SolidBrush(Color.Blue),
                        new RectangleF
                        {
                            Location = new PointF(ShiftX + beacon.X*MetersToPixels - SelectedBeaconSize/2,
                                ShiftY + beacon.Y*MetersToPixels - SelectedBeaconSize/2),
                            Size = new SizeF(SelectedBeaconSize, SelectedBeaconSize)
                        });
                }
                else
                {
                    _graphics.FillEllipse(new SolidBrush(Color.CornflowerBlue),
                       new RectangleF
                       {
                           Location = new PointF(ShiftX + beacon.X * MetersToPixels - BeaconSize / 2,
                               ShiftY + beacon.Y * MetersToPixels - BeaconSize / 2),
                           Size = new SizeF(BeaconSize, BeaconSize)
                       });
                }
                #endregion

                #region Drawing beacon radiuses
                if (_isBeaconRadiusSelected && _selectedBeacon == beacon)
                {
                    _graphics.DrawEllipse(new Pen(Color.Salmon, SelectedAccuracyRadiusLineWidth),
                    new RectangleF
                    {
                        Location = new PointF(ShiftX + (beacon.X - beacon.Accuracy) * MetersToPixels,
                                              ShiftY + (beacon.Y - beacon.Accuracy) * MetersToPixels),
                        Size = new SizeF((2 * beacon.Accuracy) * MetersToPixels, (2 * beacon.Accuracy) * MetersToPixels),
                    });
                }
                else
                {
                    _graphics.DrawEllipse(new Pen(Color.Salmon, AccuracyRadiusLineWidth),
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
                _graphics.DrawEllipse(new Pen(Color.Teal, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels*Floor.GetFloor().UserPositions[0].X + ShiftX - UserExtraPointSize/2,
                                MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                _graphics.FillEllipse(new SolidBrush(Color.LimeGreen),
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
                _graphics.DrawEllipse(new Pen(Color.Teal, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                _graphics.FillEllipse(new SolidBrush(Color.LimeGreen),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                _graphics.DrawString("CI", _font, new SolidBrush(Color.LimeGreen), 
                    MetersToPixels * Floor.GetFloor().UserPositions[0].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[0].Y + ShiftY - LabelShift, 
                    _centerStringFormat);
                #endregion

                #region Ray tracing
                _graphics.DrawEllipse(new Pen(Color.DarkBlue, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                _graphics.FillEllipse(new SolidBrush(Color.DeepSkyBlue),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                _graphics.DrawString("RT", _font, new SolidBrush(Color.DarkBlue),
                    MetersToPixels * Floor.GetFloor().UserPositions[1].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[1].Y + ShiftY - LabelShift, 
                    _centerStringFormat);
                #endregion

                #region Power center
                _graphics.DrawEllipse(new Pen(Color.DarkOrange, 2),
                    new RectangleF
                    {
                        Location =
                            new PointF(MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX - UserExtraPointSize / 2,
                                MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - UserExtraPointSize / 2),
                        Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                    });
                _graphics.FillEllipse(new SolidBrush(Color.Orange),
                    new RectangleF
                    {
                        Location = new PointF(MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX - UserPointSize / 2,
                            MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - UserPointSize / 2),
                        Size = new SizeF(UserPointSize, UserPointSize)
                    });
                _graphics.DrawString("PC", _font, new SolidBrush(Color.DarkOrange),
                    MetersToPixels * Floor.GetFloor().UserPositions[2].X + ShiftX + LabelShift,
                    MetersToPixels * Floor.GetFloor().UserPositions[2].Y + ShiftY - LabelShift, 
                    _centerStringFormat);
                #endregion
            }
        }
        #endregion

        #region Extra Methods
        private static bool IsOverBeacon(Beacon beacon, float x, float y)
        {
            var dx = MetersToPixels*beacon.X + ShiftX - x;
            var dy = MetersToPixels*beacon.Y + ShiftY - y;
            return Math.Sqrt(dx*dx + dy*dy) < SelectedBeaconSize;
        }

        private bool IsNotOverSelectedBeacon(float x, float y)
        {
            if (_selectedBeacon == null)
                return true;
            return !IsOverBeacon(_selectedBeacon, x, y);
        }

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
            if (_selectedBeacon == null)
                return true;
            return !IsOverBeaconAccuracyRadius(_selectedBeacon, x, y);
        }
        #endregion
    }
}