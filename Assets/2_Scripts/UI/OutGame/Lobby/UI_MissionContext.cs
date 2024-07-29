using System;
using System.Linq;
using Cargold.FrameWork.BackEnd;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_MissionContext : MonoBehaviour
    {
        [SerializeField] private UI_MissionGrid mMissionGrid;
        private ReactiveProperty<int> mIdx = new ReactiveProperty<int>(0);
        private void Start()
        {
            mMissionGrid.UpdateContents(BackEndManager.Instance.SpawnMissions());
            mMissionGrid.OnCellClicked(idx =>  mIdx.Value = idx);


            mIdx.Subscribe(_ =>
            {
                SelectCell();
            });
        }
        
        private void SelectCell()
        {
            if (mMissionGrid.DataCount == 0)
            {
                return;
            }
            mMissionGrid.UpdateSelection(mIdx.Value);
        }
    }
}