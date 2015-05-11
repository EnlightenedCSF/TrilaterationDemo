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

        public Drawer(Image bitmap)
        {
            _graphics = Graphics.FromImage(bitmap);
            _beacons = Floor.GetFloor().Beacons;
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
            }

            _graphics.DrawEllipse(new Pen(Color.Teal, 2), 
                new RectangleF
                {
                    Location = new PointF(MetersToPixels * Floor.GetFloor().UserPosition.X + ShiftX - UserExtraPointSize / 2,
                                          MetersToPixels * Floor.GetFloor().UserPosition.Y + ShiftY - UserExtraPointSize / 2),
                    Size = new SizeF(UserExtraPointSize, UserExtraPointSize)
                });
            _graphics.FillEllipse(new SolidBrush(Color.LimeGreen),
                new RectangleF
                {
                    Location = new PointF(MetersToPixels * Floor.GetFloor().UserPosition.X + ShiftX - UserPointSize/2, 
                                          MetersToPixels * Floor.GetFloor().UserPosition.Y + ShiftY - UserPointSize/2),
                    Size = new SizeF(UserPointSize, UserPointSize)
                });
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