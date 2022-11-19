using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItamiAPI;
using ItamiAPI.Configuration;
using KeyAuth;
using Newtonsoft.Json;
using Daedalus.Input.Hooks;
using Daedalus.Misc;

namespace Itami
{
    public partial class Form1 : Form
    {
        public static api KeyAuthApp = new api(
           name: "Itami",
           ownerid: "YUwTrOV29Y",
           secret: "8db075827a10f0deafe440e3e9f5805d0c3e3478a4da1fcdfc200ef7bc793d45",
           version: "4.1"

       );
        Form formie = new Form();
        private Userinfo user;
        private ItamiMain itami;
        private const uint WDA_NONE = 0x0;
        private const uint WDA_MONITOR = 0x1;
        private espoverlay espoverlay;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);
        KeyboardHook hk = new KeyboardHook();
        private Config config;
        private Clarity cla;
        private espoverlay esp;
        private Load loadF;
        public Form1(Userinfo userinf, ItamiMain itamiL, Clarity clarity, Load loadFrm)
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            cla = clarity;
            itami = itamiL;
            InitializeComponent();

            loadF = loadFrm;
            user = userinf;
            config = itami.Itamiconfig;

            esp = espoverlay;
        }
        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;

        }
        private async void LoadExternConfig()
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists("Configs"))
                {
                    Directory.CreateDirectory("Configs");
                }
                DirectoryInfo directory = new DirectoryInfo("Configs");
                FileInfo[] files = directory.GetFiles("*.json");
                Invoke((MethodInvoker)delegate
                {
                    configList.Items.Clear();
                });
                foreach (FileInfo info in files)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        configList.Items.Add(info.Name.Replace(".json", ""));
                    });

                }
            });
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();
            ctrpage.SetPage(1);

            hk.Hook();





            if (KeyAuthApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(KeyAuthApp.app_data.downloadLink))
                {
                    DialogResult dialogResult = MessageBox.Show("There are newer version avalible", "Aut", MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            //Process.Start(KeyAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case DialogResult.No:

                            Environment.Exit(0);

                            break;
                        default:
                            MessageBox.Show("Invalid option");
                            Environment.Exit(0);
                            break;
                    }
                }
                MessageBox.Show("Version of this program does not match the one online. Furthermore, the download link online isn't set. You will need to manually obtain the download link from the developer");
                Thread.Sleep(2500);
                Environment.Exit(0);
                if (!KeyAuthApp.response.success)
                {
                    MessageBox.Show(KeyAuthApp.response.message);
                    Environment.Exit(0);
                }
                // if(KeyAuthApp.checkblack())
                // {
                //     MessageBox.Show("user is blacklisted");
                // }
                // else
                // {
                //     MessageBox.Show("user is not blacklisted");
                // }
                KeyAuthApp.check();
            }
            Invoke((MethodInvoker)delegate
            {
                loadF.Close();
            });
            this.Text = random_string();
            CustomTit.Text = this.Text;
            LoadExternConfig();
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            cla.ExitClarity();

        }
        bool showSwitch = true;

        private void Hk_OnKeyPressed1(Daedalus.Misc.VK vk, bool isInjected)
        {
            if (vk == VK.KEY_DELETE)
            {
                showSwitch = !showSwitch;


                if (showSwitch == true)
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                }
            }
        }




        private void Minimizebutton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        int i = 3;
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            await Task.Run(() => //trans.ShowSync(navPanel, false, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.HorizSlide); //512, 42
            {
                Invoke((MethodInvoker)delegate
                {
                    Notification.Show(this, "Logging in...", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Warning);
                    LoginButton.Enabled = false;
                });
                KeyAuthApp.login(username.Text, password.Text);
                if (KeyAuthApp.response.success)
                {
                    Invoke((MethodInvoker)delegate
                   {

                       Notification.Show(this, "Logged as " + KeyAuthApp.user_data.username, Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);

                       ctrpage.AllowTransitions = true;

                       ctrpage.SetPage(3);
                       ctrpage.AllowTransitions = true;
                       navPanel.Visible = true;



                       //SessionIsVaild.Enabled = true;

                       Form f = new Form();
                       f.Size = new System.Drawing.Size(514, 662);


                   });
                }
                else
                {
                    Invoke((MethodInvoker)delegate
                    {
                        Notification.Show(this, "Incorrect username or password", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);
                        i--;
                        if (i <= 0)
                        {
                            Environment.Exit(0);
                        }
                        LoginButton.Enabled = true;

                    });

                }
            });


        }



        private async void EnlightenButton_Click(object sender, EventArgs e)
        {


            ctrpage.SetPage(0);

        }
        private void ArSt()
        {
            itami.ArChanger.ChangeAR(ArRate.Value);
            RateDisplay.Text = ArRate.Value.ToString();
        }
        private void ArRate_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private async void ArChangerToggle(object sender, EventArgs e)
        {
            cla.enableARCheat = ArChangerSwitch.Checked;
        }

        private async void CSChangerToggle_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableCSChanger = CSChangerToggle.Checked;
        }
        private void CzSt()
        {
            itami.circleSizeChange.ChangeCircleSize(CircleSize.Value);
            CircleSizeDisplay.Text = CircleSize.Value.ToString();
        }
        private void CircleSize_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private async void HPTG(object sender, EventArgs e)
        {
            cla.enableHPChanger = HPDrainRateChangerToggle.Checked;
        }
        private void HPSt()
        {
            itami.HPdrainChanger.ChangeHPDrRate(HPDrainrateValue.Value);
            HPDrainRateDisplay.Text = HPDrainrateValue.Value.ToString();
        }
        private void HPDrainrateValue_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void timer9_Tick(object sender, EventArgs e)
        {

        }

        private void a_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TopMostToggle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CustomTit_TextChanged(object sender, EventArgs e)
        {
            this.Text = CustomTit.Text;
        }

        private async void Settings_Click(object sender, EventArgs e)
        {

            ctrpage.SetPage(4);
        }
        private string IconPath;
        private void CustomAppIco_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Icon file|*.ico";

            var set = fileDialog.ShowDialog();
            if (set == DialogResult.OK)
            {
                IconPath = fileDialog.FileName;

            }
        }

        private void ScreenShareProtectToggle_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (ScreenShareProtectToggle.Checked)
                {
                    ShowInTaskbar = false;
                    SetWindowDisplayAffinity(this.Handle, WDA_MONITOR);

                }
                else
                {
                    ShowInTaskbar = true;
                    SetWindowDisplayAffinity(this.Handle, WDA_NONE);

                }
            }
            catch (Exception ex)
            {
                ScreenShareProtectToggle.Checked = false;
            }
        }

        private async void RelaxButton_Click(object sender, EventArgs e)
        {



            ctrpage.SetPage(3);


        }
        private async void Str()
        {


        }


        private async void RelaxToggle_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableRelax = RelaxToggle.Checked;

        }
        private void setPlayStyle(PlayStyles styles)
        {
            if (styles == PlayStyles.Singletap)
            {
                PlayStyleDropDown.SelectedIndex = 0;
            }
            if (styles == PlayStyles.Alternate)
            {
                PlayStyleDropDown.SelectedIndex = 1;
            }
            if (styles == PlayStyles.TapX)
            {
                PlayStyleDropDown.SelectedIndex = 2;
            }
        }
        private void PlayStyleDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PlayStyleDropDown.SelectedIndex == 0)
            {
                itami.Itamiconfig.PlayStyle = ItamiAPI.Configuration.PlayStyles.Singletap;
            }
            if (PlayStyleDropDown.SelectedIndex == 1)
            {
                itami.Itamiconfig.PlayStyle = ItamiAPI.Configuration.PlayStyles.Alternate;
            }
            if (PlayStyleDropDown.SelectedIndex == 2)
            {
                itami.Itamiconfig.PlayStyle = ItamiAPI.Configuration.PlayStyles.TapX;
            }
        }
        private void setPrimarykey(OsuKeys osuKeys)
        {
            if (osuKeys == OsuKeys.K1)
            {
                PrimaryKeyDrop.SelectedIndex = 0;
            }
            if (osuKeys == OsuKeys.K2)
            {
                PrimaryKeyDrop.SelectedIndex = 1;
            }
            if (osuKeys == OsuKeys.M1)
            {
                PrimaryKeyDrop.SelectedIndex = 2;
            }
            if (osuKeys == OsuKeys.M2)
            {
                PrimaryKeyDrop.SelectedIndex = 3;
            }
        }
        private void setSecoundarykey(OsuKeys osuKeys)
        {
            if (osuKeys == OsuKeys.K1)
            {
                PrimaryKeyDrop.SelectedIndex = 0;
            }
            if (osuKeys == OsuKeys.K2)
            {
                PrimaryKeyDrop.SelectedIndex = 1;
            }
            if (osuKeys == OsuKeys.M1)
            {
                PrimaryKeyDrop.SelectedIndex = 2;
            }
            if (osuKeys == OsuKeys.M2)
            {
                PrimaryKeyDrop.SelectedIndex = 3;
            }
        }
        private void PrimaryKeyDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            var sl = PrimaryKeyDrop.SelectedIndex;
            if (sl == 0)
            {
                itami.Itamiconfig.PrimaryKey = ItamiAPI.Configuration.OsuKeys.K1;

            }
            if (sl == 1)
            {
                itami.Itamiconfig.PrimaryKey = ItamiAPI.Configuration.OsuKeys.K2;
            }
            if (sl == 2)
            {
                itami.Itamiconfig.PrimaryKey = ItamiAPI.Configuration.OsuKeys.M1;
            }
            if (sl == 3)
            {
                itami.Itamiconfig.PrimaryKey = ItamiAPI.Configuration.OsuKeys.M2;
            }
        }

        private void MaxSingleTapBPMN_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.MaxSingletapBPM = (int)MaxSingleTapBPMN.Value;
        }

        private async void TimewarpToggle_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableTimewarp = TimewarpToggle.Checked;

        }

        private void ChangeTW(object sender, ScrollEventArgs e)
        {
            itami.Itamiconfig.TimewarpRate = TimwWarpRate.Value;
            TimewarpRateDisplay.Text = (itami.Itamiconfig.TimewarpRate * 100).ToString();
        }

        private void TimewarpButton_Click(object sender, EventArgs e)
        {
            AimAssistButton.FillColor = Color.FromArgb(31, 22, 43);
            EnlightenButton.FillColor = Color.FromArgb(31, 22, 43);
            RelaxButton.FillColor = Color.FromArgb(31, 22, 43);

            LmaoBtn.FillColor = Color.FromArgb(31, 22, 43);
            Settings.FillColor = Color.FromArgb(31, 22, 43);
            ctrpage.SetPage(2);
        }
        private bool isStart = false;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private async void checker_Tick(object sender, EventArgs e)
        {
            Thread thread = new Thread((ThreadStart)async delegate
            {
                try
                {
                    var beatmap = itami.GetOsu().Player.Beatmap;
                    while (!itami.GetOsu().CanLoad && RelaxToggle.Checked)
                        Thread.Sleep(5);

                    if (!RelaxToggle.Checked)
                        RelaxA.Enabled = false;


                    var RelaxTas = Task.Factory.StartNew(() =>
                    {
                        itami.RelaxAPI.Start(beatmap);
                    });
                    await Task.WhenAll(RelaxTas);

                }
                catch (Exception ex)
                {

                }
            });
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();

        }
        private bool isReseted = false;
        private async void TimewarpAsync_Tick(object sender, EventArgs e)
        {

            try
            {




            }
            catch (OverflowException er)
            {
                Thread.Sleep(3000);
            }

        }

        private async void ARChangerAsync_Tick(object sender, EventArgs e)
        {

            try
            {


            }
            catch (OutOfMemoryException t)
            {

            }


        }

        private async void CSChangerAsync_Tick(object sender, EventArgs e)
        {

            try
            {


            }
            catch (OutOfMemoryException et)
            {

            }

        }

        private async void HPChangerAsync_Tick(object sender, EventArgs e)
        {

            try
            {


            }
            catch (Exception et)
            {

            }

        }

        private async void AimAssistA_Tick(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (itami.GetOsu().Player.Beatmap != null)
                        itami.aimAssist.Start(itami.GetOsu().Player.Beatmap);
                }
                catch (Exception ex)
                {

                }
            });
        }
        private void setAo()
        {
            itami.aimAssist.setAimSpeed(AimSpeedD.Value);
            AimSpeedDisplay.Text = AimSpeedD.Value.ToString();
        }
        private void AimSpeedD_Scroll(object sender, ScrollEventArgs e)
        {
            setAo();
        }
        private void setAb()
        {
            itami.aimAssist.setAimStartingDistance(AimStrDist.Value);
            AimStartDistDisplay.Text = AimStrDist.Value.ToString();
        }
        private void AimStrDist_Scroll(object sender, ScrollEventArgs e)
        {
            setAb();
        }
        private void setAc()
        {
            itami.aimAssist.setAimStoppingDistance((AimStopDist.Value));
            AimStopDistDisplay.Text = AimStopDist.Value.ToString();
        }
        private void AimStopDist_Scroll(object sender, ScrollEventArgs e)
        {
            setAc();
        }

        private void AimAssistToggle_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableAA = AimAssistToggle.Checked;
        }

        private async void AimAssistButton_Click(object sender, EventArgs e)
        {




            ctrpage.SetPage(2);
        }

        private void UIAnimateToggle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void SecChange(object sender, EventArgs e)
        {
            var t = SecoundaryKey.SelectedIndex;
            if (t == 0)
            {
                itami.Itamiconfig.SecondaryKey = OsuKeys.K1;
            }
            if (t == 1)
            {
                itami.Itamiconfig.SecondaryKey = OsuKeys.K2;
            }
            if (t == 2)
            {
                itami.Itamiconfig.SecondaryKey = OsuKeys.M1;
            }
            if (t == 3)
            {
                itami.Itamiconfig.SecondaryKey = OsuKeys.M2;
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://claritycheat.wtf");
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {


        }

        private async void LmaoBtn_Click(object sender, EventArgs e)
        {

            key.Text = KeyAuthApp.user_data.username;
            subscriptionDaysLabel.Text = KeyAuthApp.expirydaysleft();
            version.Text = KeyAuthApp.app_data.version;

            ctrpage.SetPage(5);
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void navPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }



        private void HoldTime_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitTimingsMinHoldTime = 0;
            itami.Itamiconfig.HitTimingsMaxHoldTime = (int)HoldTime.Value;
        }

        private void OffsetMax_ValueChanged(object sender, EventArgs e)
        {

            itami.Itamiconfig.HitTimingsAlternateMaxOffset = (int)OffsetMax.Value;
        }

        private void OffsetMin_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitTimingsMinOffset = (int)OffsetMin.Value;
        }

        private void Predictiondirectionangletolerance_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanPredictionDirectionAngleTolerance = (int)Predictiondirectionangletolerance.Value;
        }

        private void PredictionRadiusScale_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanPredictionRadiusScale = (int)PredictionRadiusScale.Value;
        }

        private void PredictionMaxAngle_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanPredictionMaxDistance = (int)PredictionMaxDistance.Value;
        }

        private void MissRadius_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanMissRadius = (int)MissRadius.Value;
        }

        private void MissChance_ValueChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanMissChance = (int)MissChance.Value;
        }

        private void MissAfterHit50_CheckedChanged(object sender, EventArgs e)
        {
            itami.Itamiconfig.HitScanMissAfterHitWindow50 = MissAfterHit50.Checked;
        }

        private void PredictionToggle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void BlantanrModeToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (BlantanrModeToggle.Checked)
            {
                AimType.Items.Add("Aimbot");
                AimType.Items.Add("AimCorrection");
                TimwWarpRate.Minimum = 0;
            }
            else
            {
                AimType.Items.Remove("Aimbot");
                AimType.Items.Remove("AimCorrection");
                AimType.SelectedIndex = 0;
                TimwWarpRate.Minimum = 70;
            }
        }

        private void PredictionToggle_CheckedChanged_1(object sender, EventArgs e)
        {
            if (PredictionToggle.Checked)
            {
                relaxsettings.Visible = false;
                itami.Itamiconfig.EnableHitScanPrediction = PredictionToggle.Checked;

            }
            else
            {

            }

        }

        private void AimType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AimType.SelectedIndex == 0)
            {
                cla.enableAimSmooth = false;
                cla.enableAimbot = false;
            }
            else if (AimType.SelectedIndex == 1)
            {
                cla.enableAimSmooth = false;
                cla.enableAimbot = true;
            }
            else if (AimType.SelectedIndex == 2)
            {
                cla.enableAimbot = false;
                cla.enableAimSmooth = true;
            }

        }

        private void RainbowToggle_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ar54_Tick(object sender, EventArgs e)
        {

        }

        private async void esprog_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableESP = esprog.Checked;

        }
        private void setAd()
        {
            itami.aimAssist.AimCorrectionSmooth = smoothA.Value;
            smoothAL.Text = itami.aimAssist.AimCorrectionSmooth.ToString();
        }
        private void smoothA_Scroll(object sender, ScrollEventArgs e)
        {
            setAd();
        }

        private void guna2Panel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void status_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableTimewarp = TimewarpToggle.Checked;

        }

        private void guna2TrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            itami.Itamiconfig.TimewarpRate = TimwWarpRate.Value;
            TimewarpRateDisplay.Text = (itami.Itamiconfig.TimewarpRate * 100).ToString();
        }

        private void dllPath_TextChanged(object sender, EventArgs e)
        {

        }


        private void guna2Panel6_Paint(object sender, PaintEventArgs e)
        {

        }



        private void RelaxToggle_CheckedChanged_1(object sender, EventArgs e)
        {
            predsettings.Visible = false;
            cla.enableRelax = RelaxToggle.Checked;

        }

        private void ScreenShareProtectToggle_CheckedChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (ScreenShareProtectToggle.Checked)
                {

                    ShowInTaskbar = false;
                    SetWindowDisplayAffinity(this.Handle, WDA_MONITOR);

                }
                else
                {

                    ShowInTaskbar = true;
                    SetWindowDisplayAffinity(this.Handle, WDA_NONE);

                }
            }
            catch (Exception ex)
            {
                ScreenShareProtectToggle.Checked = false;
            }
        }

        private void TopMostToggle_CheckedChanged_1(object sender, EventArgs e)
        {


        }

        private void UIAnimateToggle_CheckedChanged_1(object sender, EventArgs e)
        {

        }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void BlantanrModeToggle_CheckedChanged_1(object sender, EventArgs e)
        {
            if (BlantanrModeToggle.Checked)
            {

                AimType.Items.Add("Aimbot");
                AimType.Items.Add("AimCorrection");
                TimwWarpRate.Minimum = 0;
            }
            else
            {

                AimType.Items.Remove("Aimbot");
                AimType.Items.Remove("AimCorrection");
                AimType.SelectedIndex = 0;
                TimwWarpRate.Minimum = 70;
            }
        }

        private void AimAssistToggle_CheckedChanged_1(object sender, EventArgs e)
        {


            cla.enableAA = AimAssistToggle.Checked;
        }

        private void CSChangerToggle_CheckedChanged_1(object sender, EventArgs e)
        {
            cla.enableCSChanger = CSChangerToggle.Checked;
        }

        private void HPDrainRateChangerToggle_CheckedChanged(object sender, EventArgs e)
        {

            cla.enableHPChanger = HPDrainRateChangerToggle.Checked;

        }

        private void ArChangerSwitch_CheckedChanged(object sender, EventArgs e)
        {


            cla.enableARCheat = ArChangerSwitch.Checked;

        }

        private void esprog_CheckedChanged_1(object sender, EventArgs e)
        {

            cla.enableESP = esprog.Checked;
        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void guna2ToggleSwitch1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void TimewarpToggle_CheckedChanged_1(object sender, EventArgs e)
        {

            cla.enableTimewarp = TimewarpToggle.Checked;
        }

        private void SessionIsVaild_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            var randomNum = random.Next(0, 6);
            string[] txt = { "NotCheck", "Check", "Not Check", "Check", "NotCheck", "Check", "NotCheck" };
            if (txt[randomNum] == "Check")
            {
                Thread thread = new Thread((ThreadStart)delegate
                {
                    KeyAuthApp.check();
                });
                thread.Start();
            }
        }

        private void navPanel_Click(object sender, EventArgs e)
        {

        }

        private void ESPClolorChose_Click(object sender, EventArgs e)
        {
            var t = colorDialog1.ShowDialog();
            if (t == DialogResult.OK)
            {
                cla.ESPColor = colorDialog1.Color;
                ESPClolorChose.FillColor = colorDialog1.Color;
            }
        }

        private void ESPTypeChoose_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ESPTypeChoose.SelectedIndex == 0)
            {
                cla.ESPType = 1;
            }
            else
            {
                cla.ESPType = 2;
            }
        }

        private void InjectButton_Click(object sender, EventArgs e)
        {

        }



        private void aimWhen0_SelectedIndexChanged(object sender, EventArgs e)
        {
            cla.itami.aimAssist.AimWhen = aimWhen0.SelectedIndex;
        }

        private void savBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists("Configs"))
                {
                    Directory.CreateDirectory("Configs");
                }
                List<ExterConfig> exters = new List<ExterConfig>();
                exters.Add(new ExterConfig()
                {
                    AimAssist_aimSpeed = AimSpeedD.Value,
                    AimAssist_aimStDist = AimStrDist.Value,
                    AimAssist_aimStopDist = AimStopDist.Value,
                    AimCorrection_aimCorrectionSmooth = smoothA.Value,
                    Aim_aimType = AimType.SelectedIndex,
                    AimAssist_aimWhen = aimWhen0.SelectedIndex,
                    Relax_PlayStyle = itami.Itamiconfig.PlayStyle,
                    Relax_PrimaryKey = itami.Itamiconfig.PrimaryKey,
                    Relax_SecoundaryKey = itami.Itamiconfig.SecondaryKey,
                    Relax_MaxSingleTapBPM = itami.Itamiconfig.MaxSingletapBPM,
                    Relax_HoldTime = (int)HoldTime.Value,
                    Relax_MaxOffset = OffsetMax.Value,
                    Relax_MinOffset = OffsetMin.Value,
                    Relax_EnablePrediction = PredictionToggle.Checked,
                    Relax_PredictionAngle = (int)Predictiondirectionangletolerance.Value,
                    Relax_PredictionRadius = (int)PredictionRadiusScale.Value,
                    Relax_PreditcionDist = (int)PredictionMaxDistance.Value,
                    Relax_MissRadius = (int)MissRadius.Value,
                    Relax_MissAfterHit50 = MissAfterHit50.Checked,
                    Relax_MissChance = (int)MissChance.Value,
                    Enlighteen_ApporachRate = ArRate.Value,
                    Enlighteen_CircleSize = CircleSize.Value,
                    Enlighteen_ESPType = ESPTypeChoose.SelectedIndex,
                    Enlighteen_HPDrainRate = HPDrainrateValue.Value,
                    Settings_BlatanMode = BlantanrModeToggle.Checked,
                    Settings_ScreenshareProtection = ScreenShareProtectToggle.Checked,
                });
                var tuy = JsonConvert.SerializeObject(exters.ToArray());
                File.WriteAllText("Configs/" + configName.Text + ".json", tuy);
                Notification.Show(this, "Saved as " + configName.Text, Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);
            }
            catch (Exception er)
            {
                Notification.Show(this, "Save config fail", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);
            }
        }

        private void cfg_Click(object sender, EventArgs e)
        {
            LoadExternConfig();
            ctrpage.SetPage(7);

        }

        private void LoadConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists("Configs/" + configList.Text + ".json"))
                {
                    Notification.Show(this, "Config not found", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);
                }
                else
                {
                    var Desnilayzer = JsonConvert.DeserializeObject<List<ExterConfig>>(File.ReadAllText("Configs/" + configList.Text + ".json"));
                    foreach (ExterConfig ext in Desnilayzer)
                    {
                        AimSpeedD.Value = ext.AimAssist_aimSpeed;
                        itami.Itamiconfig.AimAssistSpeed = ext.AimAssist_aimSpeed;
                        AimStrDist.Value = ext.AimAssist_aimStDist;

                        AimStopDist.Value = ext.AimAssist_aimStopDist;
                        smoothA.Value = ext.AimCorrection_aimCorrectionSmooth;
                        aimWhen0.SelectedIndex = ext.AimAssist_aimWhen;
                        AimType.SelectedIndex = ext.Aim_aimType;
                        setPlayStyle(ext.Relax_PlayStyle);
                        setPrimarykey(ext.Relax_PrimaryKey);
                        setSecoundarykey(ext.Relax_SecoundaryKey);
                        MaxSingleTapBPMN.Value = ext.Relax_MaxSingleTapBPM;
                        HoldTime.Value = ext.Relax_HoldTime;
                        OffsetMax.Value = ext.Relax_MaxOffset;
                        OffsetMin.Value = ext.Relax_MinOffset;
                        PredictionToggle.Checked = ext.Relax_EnablePrediction;
                        Predictiondirectionangletolerance.Value = ext.Relax_PredictionAngle;
                        PredictionMaxDistance.Value = ext.Relax_PreditcionDist;
                        PredictionRadiusScale.Value = ext.Relax_PredictionRadius;
                        MissRadius.Value = ext.Relax_MissRadius;
                        MissChance.Value = ext.Relax_MissChance;
                        MissAfterHit50.Checked = ext.Relax_MissAfterHit50;
                        ArRate.Value = (int)ext.Enlighteen_ApporachRate;
                        CircleSize.Value = (int)ext.Enlighteen_CircleSize;
                        HPDrainrateValue.Value = (int)ext.Enlighteen_HPDrainRate;
                        ESPTypeChoose.SelectedIndex = ext.Enlighteen_ESPType;
                        BlantanrModeToggle.Checked = ext.Settings_BlatanMode;
                        ScreenShareProtectToggle.Checked = ext.Settings_ScreenshareProtection;
                    }
                    Notification.Show(this, "Loaded" + configName.Text, Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Success);
                }
            }
            catch (Exception ex)
            {

                Notification.Show(this, "Fail to load", Bunifu.UI.WinForms.BunifuSnackbar.MessageTypes.Error);
            }
        }


        private void bunifuPanel4_Click(object sender, EventArgs e)
        {

        }

        private void s(object sender, EventArgs e)
        {
            setAo();
        }

        private void b(object sender, EventArgs e)
        {
            setAd();
        }

        private void c(object sender, EventArgs e)
        {
            setAb();
        }

        private void f(object sender, EventArgs e)
        {
            setAc();
        }

        private void CS_Value(object sender, EventArgs e)
        {
            CzSt();
        }

        private void HP_Value(object sender, EventArgs e)
        {
            HPSt();
        }

        private void Ar_Value(object sender, EventArgs e)
        {
            ArSt();
        }

        private void ReplayBotToggle_CheckedChanged(object sender, EventArgs e)
        {
            cla.enableReplayBot = ReplayBotToggle.Checked;
        }

        private void FlipToggle_CheckedChanged(object sender, EventArgs e)
        {
            if (FlipToggle.Checked)
            {
                itami.replayPlayer.setReplayFrames(itami.replayPlayer.flipReplay());
                label51.ForeColor = Color.FromArgb(255, 255, 255);
            }
            else
            {
                label51.ForeColor = Color.FromArgb(45, 45, 45);
            }
        }

        private void ChooseReplayPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "osu! replay|*.osr";
            var g = openFile.ShowDialog();
            if (g == DialogResult.OK)
            {
                itami.replayPlayer.selectReplay(openFile.FileName);
                replayPath.Text = itami.replayPlayer.getReplayPath();
            }
        }

        private void replayBtn_Click(object sender, EventArgs e)
        {




            ctrpage.SetPage(8);
        }

        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
            dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            return dtDateTime;
        }

        private void guna2CustomCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (guna2CustomCheckBox1.Checked)
            {
                hwid.Text = KeyAuthApp.user_data.hwid;
                ip.Text = KeyAuthApp.user_data.ip;
                label52.Text = "" + UnixTimeToDateTime(long.Parse(KeyAuthApp.user_data.createdate));
            }
            else
            {
                hwid.Text = "...";
                ip.Text = "...";
                label52.Text = "...";
            }
        }



        private void toka_Click(object sender, EventArgs e)
        {

        }

        private void eka_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {



            LoadExternConfig();
            ctrpage.SetPage(7);
        }

        private void bunifuPanel8_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {


        }

        private void bunifuPanel6_Click(object sender, EventArgs e)
        {

        }

        private void replayPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void MissAfterHit50_CheckedChanged_1(object sender, EventArgs e)
        {
            if (MissAfterHit50.Checked)
            {
                itami.Itamiconfig.HitScanMissAfterHitWindow50 = MissAfterHit50.Checked;
                label35.ForeColor = Color.FromArgb(255, 255, 255);
            }
            else
            {
                label35.ForeColor = Color.FromArgb(35, 35, 35);
            }
        }

        private void configName_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {


        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            cla.ExitClarity();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            espsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(espsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            espsettings.BringToFront();
            espsettings.Visible = true;

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            espsettings.Visible = false;
        }

        private void version_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            aimsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(aimsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            aimsettings.BringToFront();
            aimsettings.Visible = true;
        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {

            aimsettings.Visible = false;

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            relaxsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(relaxsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            relaxsettings.BringToFront();
            relaxsettings.Visible = true;
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {


            relaxsettings.Visible = false;

        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            predsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(predsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            predsettings.BringToFront();
            predsettings.Visible = true;
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {

            predsettings.Visible = false;

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click_1(object sender, EventArgs e)
        {
            titlesettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(titlesettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            titlesettings.BringToFront();
            titlesettings.Visible = true;
        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            titlesettings.Visible = false;
        }

        private void guna2Button15_Click(object sender, EventArgs e)
        {
            warpsettings.Visible = false;
        }

        private void guna2Button5_Click_1(object sender, EventArgs e)
        {
            accsettings.Visible = false;
            warpsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(warpsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            warpsettings.BringToFront();
            warpsettings.Visible = true;
        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            accsettings.Visible = false;
        }

        private void guna2Button16_Click(object sender, EventArgs e)
        {
            warpsettings.Visible = false;
            accsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(accsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            accsettings.BringToFront();
            accsettings.Visible = true;
        }

        private void guna2Panel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button18_Click(object sender, EventArgs e)
        {
            ctrpage.SetPage(0);

            navPanel.Size = new Size(512, 42);
            ctrpage.Size = new Size(528, 451);
            ctrpage.Location = new System.Drawing.Point(1, 167);
            navPanel.Visible = true;
            guna2Button2.Visible = true;
            Settings.Visible = true;
            trans.ShowSync(navPanel, false, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.HorizSlide);

            //1, 167
        }

        private void guna2Panel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button20_Click(object sender, EventArgs e)
        {
            configsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(configsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            configsettings.BringToFront();
            configsettings.Visible = true;
        }

        private void guna2Button21_Click(object sender, EventArgs e)
        {
            configsettings.Visible = false;
        }

        private void guna2Button24_Click(object sender, EventArgs e)
        {
            repsettings.Visible = false;
        }

        private void guna2Button22_Click(object sender, EventArgs e)
        {

            repsettings.Location = new System.Drawing.Point(38, 85);
            trans1.Show(repsettings, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            repsettings.BringToFront();
            repsettings.Visible = true;
        }

        private void guna2Button19_Click(object sender, EventArgs e)
        {
            guna2Button2.Visible = true;
            Settings.Visible = true;

            navPanel.Size = new Size(512, 42);
            ctrpage.Size = new Size(528, 451);
            ctrpage.Location = new System.Drawing.Point(1, 167);
            navPanel.Visible = true;
            guna2Button2.Visible = true;
            Settings.Visible = true;
            trans.ShowSync(navPanel, false, Bunifu.UI.WinForms.BunifuAnimatorNS.Animation.HorizSlide);
        }

        private void guna2Button23_Click(object sender, EventArgs e)
        {



        }

        private void guna2Button25_Click(object sender, EventArgs e)
        {


        }

        private void featureslabel_Click(object sender, EventArgs e)
        {

        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void warpsettings_Click(object sender, EventArgs e)
        {

        }
    }
}
