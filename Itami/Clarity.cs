using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItamiAPI;
using System.Diagnostics;
using osu;
using osu.Memory.Objects.Player.Beatmaps;
using System.Drawing;
using System.Numerics;
using osu.Memory.Objects.Player.Beatmaps.Objects;
using System.Runtime.InteropServices;
using osu.Enums;

namespace Itami
{
    public class Clarity
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr dc);
        [StructLayout(LayoutKind.Auto)]
        public struct lazy
        {
            public OsuBeatmap beatmap;
        }
        public ItamiMain itami = new ItamiMain();
        private Load loadFrom;
        private espoverlay espoverlay;
      
        public async void Start()
        {



            Thread threadC = new Thread((ThreadStart)delegate
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                loadFrom = new Load();
                Application.Run(loadFrom);

            });
            threadC.SetApartmentState(ApartmentState.STA);
            threadC.Start();

            Thread.Sleep(2000);
            Process[] proc = Process.GetProcessesByName("osu!");
            if (proc.Length == 0)
            {
                Thread.Sleep(2000);
                loadFrom.SetStatus("Please open osu! first, auto-close in 3 secounds");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            Thread.Sleep(2000);
            loadFrom.SetStatus("Initializing...");
            Thread.Sleep(2000);
            var status = await itami.InitializeAPI("YhHeFZXYSe4zdVurVyrceI15XCC7Mcwe");


            if (!status)
            {
                await Task.Run(() =>
                {
                    loadFrom.SetStatus("An error occured,please report at discord.Exit in 3 secounds");
                    Thread.Sleep(3000);
                    Environment.Exit(0);

                });

            }
            else
            {
                loadFrom.SetStatus("Initializing overlay...");
                Thread.Sleep(2000);
                espoverlay = new espoverlay(this);
                Thread thread1 = new Thread((ThreadStart)delegate
                {
                    Application.EnableVisualStyles();
                    Application.Run(espoverlay);
                });
                thread1.SetApartmentState(ApartmentState.STA);
                thread1.Start();
                loadFrom.SetStatus("Initializing main UI...");
                Thread.Sleep(2000);
                Thread thread = new Thread((ThreadStart)delegate
                {
                    Application.EnableVisualStyles();
                    Application.Run(new Form1(new Userinfo(), itami, this, loadFrom));
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                loadFrom.SetStatus("Done,have fun!");
                StartTask();
            }
        }
        private IntPtr desktopPtr = GetDC(IntPtr.Zero);
        public Graphics graphics;
        public bool enableSongInfo;
       
        public void ExitClarity()
        {
            Application.Exit();
            Environment.Exit(0);
        }
        public async void StartTask()
        {
            lazy lazy = new lazy();
            graphics = espoverlay.CreateGraphics();
            bool flg = true;
           
            while (flg)
            {
                try
                {
                    if (!flg)
                        break;
                    while (!itami.GetOsu().CanLoad)
                    {
                        Thread.Sleep(5);
                    }
                    lazy.beatmap = itami.GetOsu().Player.Beatmap;
                    
                    cb cb = new cb(this, espoverlay,lazy);
                    var task1 = Task.Factory.StartNew(()=> {
                        if (enableAA && lazy.beatmap != null && !String.IsNullOrEmpty(lazy.beatmap.Title))
                            itami.aimAssist.Start(lazy.beatmap);
                    });
                    var task2 = Task.Factory.StartNew(()=> {
                        if (enableRelax && lazy.beatmap != null && !String.IsNullOrEmpty(lazy.beatmap.Title))
                            itami.RelaxAPI.Start(lazy.beatmap);
                    });
                    var task3 = Task.Factory.StartNew(new Action(cb.ARCheat));
                    var task4 = Task.Factory.StartNew(new Action(cb.CSCheat));
                    var task5 = Task.Factory.StartNew(new Action(cb.HPCheat));
                    var task9 = Task.Factory.StartNew(new Action(cb.ESP));
                    var task6 = Task.Factory.StartNew(new Action(cb.TimewarpCheat));
                    var task7 = Task.Factory.StartNew(new Action(cb.SuperHardMode));
                    var task8 = Task.Factory.StartNew(new Action(cb.NegativeAR));
                    var task10 = Task.Factory.StartNew(new Action(cb.ReplayPlayerCheat));

                    Task.WaitAll(new Task[] {
                        task3,
                        task4,
                        task5,
                        task1,
                        task2,
                        task6,
                        task7,
                        task8,
                        task9,
                        task10,
                    });
                }
                catch (Exception ex)
                {

                }
            }

        }
        public bool enableRelax = false;
        public bool enableAA = false;
        public bool enableTimewarp = false;
        public bool enableARCheat = false;
        public bool enableCSChanger = false;
        public bool enableHPChanger = false;
        public bool enableSuperhardMode = false;
        public bool enableNegativeAR = false;
        public bool enableAimbot = false;
        public bool enableESP = false;
        public bool enableReplayBot = false;
        public Vector2 vec;
        public Color ESPColor = Color.Red;
        public int ESPType = 1;
        public bool enableAimSmooth = false;


        internal sealed class cb
        {

            public cb(Clarity clarity, espoverlay es,lazy lay)
            {
                cla = clarity;
                itam = clarity.itami;
                tbeatmap = lay.beatmap;
                osu = clarity.itami.GetOsu();
                espoverlay = es;
                g = cla.graphics;
                ar = new ESPCheat(osu);

            }
            private bool isShowed = false;


            internal void RelaxCheat()
            {
                try
                {

                    if (cla.enableRelax && tbeatmap != null && !String.IsNullOrEmpty(tbeatmap.Title))
                    {

                        itam.RelaxAPI.Start(tbeatmap);

                    }
                    else
                    {
                        itam.RelaxAPI.Stop();
                    }

                }
                catch (Exception ex)
                {

                }
            }
            internal void AimAssistCheat()
            {
                try
                {

                    if (cla.enableAA && tbeatmap != null && !String.IsNullOrEmpty(tbeatmap.Title))
                    {
                        itam.aimAssist.Start(tbeatmap);

                    }
                    else
                    {
                        itam.aimAssist.Stop();
                    }
                    itam.aimAssist.enableAimBot = cla.enableAimbot;
                    itam.aimAssist.enableAimCorrection = cla.enableAimSmooth;
                    //  itam.aimAssist.enableAimCorrection = cla.enableAimSmooth;
                }
                catch (Exception ex)
                {

                }
            }
            internal void ReplayPlayerCheat()
            {
                try
                {
                    if (cla.enableReplayBot && tbeatmap != null && !String.IsNullOrEmpty(tbeatmap.Title))
                    {
                        itam.replayPlayer.Start(tbeatmap);
                    }
                    else
                    {
                        itam.replayPlayer.Stop();
                    }
                }catch (Exception ex)
                {

                }
            }
            internal void TimewarpCheat()
            {
                if (cla.enableTimewarp)
                {
                    itam.timewarp.Start();
                }
                else
                {
                    itam.timewarp.Stop();
                }

            }
            internal void ARCheat()
            {
               
                  if (cla.enableARCheat)
                {
                    itam.ArChanger.Start();
                }
                else
                {
                    itam.ArChanger.Stop();
                }
                
            }
            internal void CSCheat()
            {
               
                    if (cla.enableCSChanger)
                    {
                        itam.circleSizeChange.Start();
                    }
                    else
                    {
                        itam.circleSizeChange.Stop();
                    }
               
               

            }
            internal void HPCheat()
            {
               if (cla.enableHPChanger)
                {
                    itam.HPdrainChanger.Start();
                }
                else
                {
                    itam.HPdrainChanger.Stop();
                }
                  
                
            }
            internal void SuperHardMode()
            {
                //I think we don't need it
                try
                {
                    if (cla.enableSuperhardMode)
                    {
                        if (cla.enableARCheat && cla.enableCSChanger)
                        {
                            MessageBox.Show("Disable AR Changer and CirleSize Changer to enable this module");
                            cla.enableSuperhardMode = false;
                        }
                        else
                        {
                            itam.lmao.SuperhardMode();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            internal void NegativeAR()
            {
                //I think we don't need it
                try
                {
                    if (!cla.enableSuperhardMode)
                    {
                        if (cla.enableNegativeAR)
                        {
                            itam.lmao.NegativeAR();
                        }
                    }
                    else
                    {

                    }
                }
                catch (Exception EX)
                {

                }
            }

            internal async void ESP()
            {
                if (cla.enableESP)
                {
                    try
                    {

                        ar.Start(osu.Player.Beatmap, espoverlay, cla.enableESP,cla.ESPColor,cla.ESPType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {

                }
            }
            private ItamiMain itam;
            private Clarity cla;
            private OsuBeatmap tbeatmap;
            private espoverlay espoverlay;
            private OsuManager osu;
            private Graphics g;
            private ESPCheat ar;
        }
    }
}
