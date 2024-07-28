using System;
using Cargold.FrameWork.BackEnd;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_MissionContext : MonoBehaviour
    {
        [SerializeField] private UI_MissionGrid mMissionGrid;
        private void Start()
        {
            mMissionGrid.UpdateContents(BackEndManager.Instance.SpawnMissions());
        }
    }
}