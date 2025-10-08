using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;


// I created this in Visual Studio 2010 for quick building.
namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.Text = "osu!Hex2RGB";
            this.MaximizeBox = false;

            richTextBox2.Text = "hex";
            richTextBox1.Text = "output";

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.Load += (s, e) => ApplyDarkTheme(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // DLLImport works in C# 4?
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            try
            {
                int useDark = 1;

                DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDark, sizeof(int));
                DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
            }
            catch
            {
            }
        }

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
                        Color color = ColorTranslator.FromHtml("#" + hex);
                        sb.AppendLine(string.Format("Combo{0}: {1}, {2}, {3}",
                            i + 1, color.R, color.G, color.B));
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
                richTextBox1.Text = "Error";
            }
        }
        private void ApplyDarkTheme(Control parent)
        {
            parent.BackColor = Color.FromArgb(30, 30, 30);
            parent.ForeColor = Color.WhiteSmoke;

            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button)
                {
                    ctrl.BackColor = Color.FromArgb(50, 50, 50);
                    ctrl.ForeColor = Color.WhiteSmoke;
                    (ctrl as Button).FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is TextBox || ctrl is RichTextBox)
                {
                    ctrl.BackColor = Color.FromArgb(20, 20, 20);
                    ctrl.ForeColor = Color.WhiteSmoke;
                    ctrl.Font = new Font("Consolas", 10f);
                }
                else if (ctrl is Label)
                {
                    ctrl.ForeColor = Color.Gainsboro;
                }
                if (ctrl.HasChildren)
                    ApplyDarkTheme(ctrl);
            }
        }
    }
}
