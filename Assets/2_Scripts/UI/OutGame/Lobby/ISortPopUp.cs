using System;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public interface ISortPopUp : IComparable<ISortPopUp>
    {
        int SortIndex { get; set; }
        
        bool IsPopUpEnd { get; set; }

        void OnPopUp();
        int IComparable<ISortPopUp>.CompareTo(ISortPopUp other)
        {
            if (other == null) return 1;
            return SortIndex.CompareTo(other.SortIndex);
        }
    }
}