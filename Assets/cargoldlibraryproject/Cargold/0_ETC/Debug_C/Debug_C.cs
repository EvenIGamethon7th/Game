namespace Cargold
{
    public static partial class Debug_C
    {
        [System.Flags]
        public enum PrintLogType
        {
            None = 0,

            Design_Upgrade = 1 << 1,
            Design_StageClearGold = 1 << 2,

            HeroAni = 1 << 3,
            MonsterSpawn = 1 << 4,
            Rush = 1 << 5,
            Matchable = 1 << 6,
            TouchPos = 1 << 7,
            Tile = 1 << 8,
            Combo = 1 << 9,
            Event = 1 << 10,
            Dialogue = 1 << 11,
            PropAction = 1 << 12,
            Event_Begin = 1 << 13,
            Event_Condition = 1 << 14,
            Event_Action = 1 << 15,
            Direction = 1 << 16,

            #region FrameWork
            Common = 1 << 30,
            Save = 1 << 29,
            TileSystem_C = 1 << 28,

            DB_ExcelToString = 1 << 27,
            DB_StringToJson = 1 << 26,
            DB_Generate = 1 << 25,
            DB_ExcelToString_IgnoreCol = 1 << 24,
            DB_GoogleAccess = 1 << 23,

            DB_Json = DB_ExcelToString | DB_StringToJson | DB_ExcelToString_IgnoreCol,
            #endregion

            All = ~None,
            EventAll = Event | Event_Begin | Event_Condition | Event_Action,
        }
    } 
}