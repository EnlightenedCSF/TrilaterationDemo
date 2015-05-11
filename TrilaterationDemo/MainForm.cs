using System;
using System.Drawing;
using System.Windows.Forms;
using TrilaterationDemo.TrilateratingStrategy;

namespace TrilaterationDemo
{
    public partial class MainForm : Form
    {
        private Drawer _drawer;
        private Bitmap _bitmap;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _bitmap = new Bitmap(picBoxTrilateration.Size.Width, picBoxTrilateration.Size.Height);
            picBoxTrilateration.Image = _bitmap;

            _drawer = new Drawer(_bitmap);
            Floor.GetFloor().ChangeTrilaterationMethod(new TrilaterationStrategy(Floor.GetFloor()));
            
            picBoxTrilateration.MouseDown += (o, args) => { _drawer.OnMouseDown(args.X, args.Y); picBoxTrilateration.Invalidate(); };
            picBoxTrilateration.MouseMove += (o, args) => { _drawer.OnMouseMove(args.X, args.Y); picBoxTrilateration.Invalidate(); };
            picBoxTrilateration.MouseUp += (o, args) => {_drawer.OnMouseUp(); picBoxTrilateration.Invalidate(); };

            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _drawer.Reset();
            picBoxTrilateration.Invalidate();
        }

        private void rBtnRayTracing_CheckedChanged(object sender, EventArgs e)
        {
            Floor.GetFloor().ChangeTrilaterationMethod(new RayTracingStrategy(Floor.GetFloor()));
            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }

        private void rBtnTrilateration_CheckedChanged(object sender, EventArgs e)
        {
            Floor.GetFloor().ChangeTrilaterationMethod(new TrilaterationStrategy(Floor.GetFloor()));
            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }

        private void rBtnPowerCenter_CheckedChanged(object sender, EventArgs e)
        {
            Floor.GetFloor().ChangeTrilaterationMethod(new PowerCenterStrategy(Floor.GetFloor())); 
            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }
    }
}
