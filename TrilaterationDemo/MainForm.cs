using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TrilaterationDemo.Properties;

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
            Floor.GetFloor().ChangeTrilaterationMethod(0);
            
            picBoxTrilateration.MouseDown += (o, args) => { _drawer.OnMouseDown(args.X, args.Y); picBoxTrilateration.Invalidate(); };
            picBoxTrilateration.MouseMove += (o, args) => { _drawer.OnMouseMove(args.X, args.Y); picBoxTrilateration.Invalidate(); };
            picBoxTrilateration.MouseUp += (o, args) => {_drawer.OnMouseUp(); picBoxTrilateration.Invalidate(); };

            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _drawer.Reset();
            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }

        private void rBtnMethod_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var radio in panelTrilaterationMethods.Controls.OfType<RadioButton>()
                .Select(control => control)
                .Where(radio => radio.Checked && radio.Text != Resources.Both))
            {
                Floor.GetFloor().ChangeTrilaterationMethod(radio.TabIndex);
                _drawer.SetNeedsDisplay();
                picBoxTrilateration.Invalidate();
            }
        }

        private void rBtnBoth_CheckedChanged(object sender, EventArgs e)
        {
            Floor.GetFloor().UseBothMethods();
            _drawer.SetNeedsDisplay();
            picBoxTrilateration.Invalidate();
        }
    }
}
