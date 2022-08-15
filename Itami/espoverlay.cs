using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ItamiAPI;
using osu;
using System.Numerics;
using osu.Memory.Objects.Player.Beatmaps.Objects;
using System.Threading;


namespace Itami
{
    public partial class espoverlay : Form
    {
        private Clarity cla;
        public espoverlay(Clarity clarity)
        {
            InitializeComponent();
            
            cla = clarity;
        }

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

    

        private static bool IsForegroundFullScreen(Screen screen)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            RECT rect = new RECT();
            GetWindowRect(new HandleRef(null, GetForegroundWindow()), ref rect);
            return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top).Contains(screen.Bounds);
        }
        private Graphics graphics;
        public void setOpc(double op)
        {
            Invoke((MethodInvoker)delegate
            {
                Opacity = op;
            });
        }
        protected async override void OnShown(EventArgs e)
        {
           
                base.OnShown(e);
                int wl = GetWindowLong(this.Handle, GWL.ExStyle);
                wl = wl | 0x80000 | 0x20;
                SetWindowLong(this.Handle, GWL.ExStyle, wl);
              //  SetLayeredWindowAttributes(this.Handle, 0, 128, LWA.Alpha);
           
           
        }
        public async void DrawESP(Vector2 vec,string text,int type,Color LineColor)
        {
           
                using (Pen p = new Pen(LineColor, 2))
                using (SolidBrush soild = new SolidBrush(LineColor))
                {
                if (type == 2)
                {
                    graphics.DrawEllipse(p, vec.X - 50, vec.Y - 50, 100, 100);
                    graphics.FillEllipse(soild, vec.X - 50, vec.Y - 50, 100, 100);
                    
                }

                if (type == 1)
                {
                    graphics.DrawLine(p, Cursor.Position.X, Cursor.Position.Y, vec.X, vec.Y);
                    graphics.DrawString(text, new Font("Microsoft Sans Serif", 15f, FontStyle.Bold), soild, Cursor.Position);
                   
                }
                }
            
        }
        public async void ClearEsp()
        {
            try
            {
                graphics.Clear(Color.Black);
            }catch (Exception ex)
            {

            }
        }
        private void espoverlay_Load(object sender, EventArgs e)
        {


            
            ShowInTaskbar = false;
            WindowState = FormWindowState.Maximized;
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            graphics = this.CreateGraphics();
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
        }
        public void SetApplicStatus(string st)
        {
            label1.Text = st;
        }
        private void ClosingFunc(object sender, FormClosingEventArgs e)
        {
            cla.ExitClarity();
        }
       
        private void timer1_Tick(object sender, EventArgs e)
        {
            TopMost = true;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Maximized;
         
        }
    }
}
