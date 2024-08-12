namespace _2_Scripts.UI.OutGame.Lobby
{
    public interface IClearMission
    {
        public bool IsClear();
        public int GetCurrentProgress();
        public int GetMaxProgress();
    }
}