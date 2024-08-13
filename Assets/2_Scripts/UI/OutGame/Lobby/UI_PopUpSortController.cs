using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpSortController : SerializedMonoBehaviour
    {
        private Queue<ISortPopUp> sortPopup = new ();
        private bool mbIsAlready = false;
        private ISortPopUp currentPopUp;
        public void Update()
        {
            if (sortPopup.Count != 0 && !mbIsAlready)
            {
                mbIsAlready = true;
                currentPopUp = sortPopup.Dequeue();
                currentPopUp.OnPopUp();
            }            
        }
    }
}