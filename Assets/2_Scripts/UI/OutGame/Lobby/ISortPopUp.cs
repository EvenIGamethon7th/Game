namespace _2_Scripts.UI.OutGame.Lobby
{
    public interface ISortPopUp
    {
        public int SortIndex { get; set; }
        public void OnPopUp();
        public void OffPopUp();
    }
}