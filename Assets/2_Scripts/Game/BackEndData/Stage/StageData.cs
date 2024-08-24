using System;

namespace _2_Scripts.Game.BackEndData.Stage
{
    public enum StageType
    {
        Normal,
        Survive
    }
    [Serializable]
    public class StageData
    {
        public int ChapterNumber;
        public int StageNumber;
        public int Star;
        public bool IsClear;
        public bool IsLastStage;
        public StageType StageType { get; set; } = StageType.Normal;
    }
}