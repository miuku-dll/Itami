using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu;
using osu.Enums;
using osu.Memory.Objects.Player.Beatmaps.Objects;
using osu.Memory.Objects.Player.Beatmaps;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Threading;

namespace Itami
{
    public class ESPCheat
    {
        private class ac_Display_a
        {
       
            public int index;
        
            public OsuHitObject currentHitObj;
            public Vector2 vec;
            public Point CursorPoint = Cursor.Position;
            public OsuBeatmap osubeatmap;
            public string ObjectType;
        }
        private ac_Display_a ar;
        private Vector2 lastMousePos;
        private OsuManager osu;
        private bool enable = false;
        public ESPCheat(OsuManager osuManager)
        {
            ar = new ac_Display_a();
            lastMousePos = new Vector2(0f, 0f);
            osu = osuManager;
        }
        public void Start(OsuBeatmap beatmap,espoverlay espoverlay,bool enableESP,Color ESPClr,int Typ = 1)
        {
            enable = enableESP;
            ar.osubeatmap = beatmap;
            ar.index = osu.Player.HitObjectManager.CurrentHitObjectIndex;
            if (ar.index > ar.osubeatmap.HitObjects.Count) return;
           
           
            while (enable)
            {
                Thread.Sleep(10);
                
                if (beatmap == null)
                {
                    espoverlay.ClearEsp();
                    return;
                }
                if (String.IsNullOrWhiteSpace(beatmap.Title))
                {
                    espoverlay.ClearEsp();
                    return;
                }
                if (!osu.CanPlay)
                {
                    return;
                }
                
                ar.currentHitObj = ar.osubeatmap.HitObjects[ar.index];
               
                if (osu.IsPaused)
                {
                    continue;
                }
                int CurrentTime = osu.CurrentTime;

                if (CurrentTime <= ar.currentHitObj.EndTime)
                {
                    ar.vec = (ar.currentHitObj is OsuSlider) ? (ar.currentHitObj as OsuSlider).PositionAtTime(CurrentTime) : ar.currentHitObj.Position;
                    if (ar.currentHitObj is OsuSlider)
                    {
                        ar.ObjectType = "Slider";
                        espoverlay.ClearEsp();
                    }
                    if (ar.currentHitObj is OsuSpinner)
                    {
                        ar.ObjectType = "Spinner";
                    }
                    if (ar.currentHitObj is OsuHitCircle)
                    {
                        ar.ObjectType = "Circle";
                    }
                    if (osu.Player.Ruleset.MousePosition != lastMousePos)
                    {
                        espoverlay.ClearEsp();
                    }
                    
                    ac__DisplayAC_1(osu.WindowManager.PlayfieldToScreen(ar.vec), espoverlay,ar.ObjectType,ESPClr,Typ);
                    lastMousePos = osu.Player.Ruleset.MousePosition;
                }
                else if (CurrentTime > ar.currentHitObj.EndTime)
                {
                    espoverlay.ClearEsp();
                    ar.index++;
                }
                
                while (osu.CanPlay && ar.index >= ar.osubeatmap.HitObjects.Count && enable)
                {
                    Thread.Sleep(5);
                }
            }
        }
        private void ac__DisplayAC_1(Vector2 vector,espoverlay es,string ObjectName,Color ESPClt,int Type)
        {
            if (osu.Player.ReplayMode) return;
            es.DrawESP(vector,ObjectName,Type,ESPClt);
        }
    }
}
