using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Colour = System.Drawing.Color;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;


// I created this in Visual Studio 2010 for quick building.
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        // I know that that topbar isn't exactly alligned.
        #region ctor
        public Form1()
        {
            InitializeComponent();
            this.Text = "osu!Hex2RGB";
            this.MaximizeBox = false;

            richTextBox2.Text = "Hex (separated by space)";
            richTextBox1.Text = "Output";

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Load += (s, e) => tLoad(this);
        }
        #endregion

        // I might as well remove this, we don't even have a topbar anymore.
        #region var
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        const int darkmode = 20;
        const int darkmode2 = 19;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        #endregion
        #region Handle
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            try
            {
                int useDark = 1;

                DwmSetWindowAttribute(this.Handle, darkmode2, ref useDark, sizeof(int));
                DwmSetWindowAttribute(this.Handle, darkmode, ref useDark, sizeof(int));
                Console.WriteLine("topbar darken success");
                this.FormBorderStyle = FormBorderStyle.None;
                Console.WriteLine("topbar remove success");
            }
            catch
            {
            }
        }
        #endregion

        #region Convert button
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                var hexList = richTextBox2.Text
                    .Replace("\r", "")
                    .Split(new[] { '\n', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                StringBuilder sb = new StringBuilder();
                int maxCombos = Math.Min(hexList.Length, 8);

                for (int i = 0; i < maxCombos; i++)
                {
                    string hex = hexList[i].Trim();

                    if (hex.StartsWith("#"))
                        hex = hex.Substring(1);

                    if (hex.Length == 6)
                    {
                        Colour colour = ColorTranslator.FromHtml("#" + hex);
                        sb.AppendLine(string.Format("Combo{0}: {1}, {2}, {3}",
                            i + 1, colour.R, colour.G, colour.B));
                    }
                    else
                    {
                        sb.AppendLine(string.Format("Combo{0}: Invalid", i + 1));
                    }
                }

                richTextBox1.Text = sb.ToString();
            }
            catch
            {
                richTextBox1.Text = "error";
            }
        }
        #endregion
        #region Load
        private void tLoad(Control parent)
        {
            parent.BackColor = Colour.FromArgb(30, 30, 30);
            pictureBox1.BackColor = Colour.Black;
            parent.ForeColor = Colour.WhiteSmoke;
            Console.WriteLine("dark theme initialized");
            label1.Font = new Font("Alan Sans", 9f);
            label1.BackColor = Colour.Black;
            label1.Text = "osu!HEX2RGB";
            // I will change this to a proper button. With icons.
            button2.Text = "X";
            button2.FlatStyle = FlatStyle.Flat;
            button2.Click += this.Exit;
            Console.WriteLine("set topbar properties");


            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.BackColor = Colour.FromArgb(50, 50, 50);
                   //  ctrl.ForeColor = Colour.WhiteSmoke;
                    (ctrl as Button).FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is TextBox || ctrl is RichTextBox)
                {
                    ctrl.BackColor = Colour.FromArgb(20, 20, 20);
                    ctrl.ForeColor = Colour.WhiteSmoke;
                    ctrl.Font = new Font("Consolas", 10f);
                }
                else if (ctrl is Label)
                {
                    ctrl.ForeColor = Colour.Gainsboro;
                }
                if (ctrl.HasChildren)
                    tLoad(ctrl);
            }
            Console.WriteLine("dark theme success");
        }
        #endregion

        #region Handlers
        private void Dwn(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            }
        }
        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
