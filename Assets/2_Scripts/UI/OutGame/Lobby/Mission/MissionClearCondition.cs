using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public abstract class MissionClearCondition :ScriptableObject ,IClearMission
    {

        public abstract bool IsClear();
        public abstract int GetCurrentProgress();
        public abstract int GetMaxProgress();
    }
}