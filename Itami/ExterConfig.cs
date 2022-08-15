using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osu.Enums;
using ItamiAPI.Configuration;

namespace Itami
{
    public class ExterConfig
    {
        public int Aim_aimType { get; set; }
        public int AimAssist_aimStDist { get; set; }
        public int AimAssist_aimStopDist { get; set; }
        public int AimAssist_aimSpeed { get; set; }
        public int AimCorrection_aimCorrectionSmooth { get; set; }
        public int AimAssist_aimWhen { get; set; }
        public PlayStyles Relax_PlayStyle { get; set; }
        public OsuKeys Relax_PrimaryKey { get; set; }
        public OsuKeys Relax_SecoundaryKey { get; set; }
        public int Relax_MaxSingleTapBPM { get; set; }
        public int Relax_HoldTime { get; set; }
        public decimal Relax_MaxOffset { get; set; }
        public decimal Relax_MinOffset { get; set; }
        public bool Relax_EnablePrediction { get; set; }
        public int Relax_PredictionAngle { get; set; }
        public int Relax_PredictionRadius { get; set; }
        public int Relax_PreditcionDist { get; set; }
        public int Relax_MissRadius { get; set; }
        public int Relax_MissChance { get; set; }
        public bool Relax_MissAfterHit50 { get; set; }
        public float Enlighteen_CircleSize { get; set; }
        public float Enlighteen_ApporachRate { get; set; }
        public float Enlighteen_HPDrainRate { get; set; }
        public int Enlighteen_ESPType { get; set; }
        public bool Settings_BlatanMode { get; set; }
        public bool Settings_ScreenshareProtection { get; set; }


    }
}
