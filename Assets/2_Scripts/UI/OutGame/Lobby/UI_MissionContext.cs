using System;
using System.Linq;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils;
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
        private UI_MissionCharacterCard mSelectedMission;
        private void Start()
        {
            mMissionGrid.UpdateContents(BackEndManager.Instance.SpawnMissions());
            MessageBroker.Default.Receive<GameMessage<UI_MissionCharacterCard>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter)
                .Subscribe(data =>
                {
                    if(mSelectedMission != null)
                        mSelectedMission.SelectBorder(EStateCard.Unlock);
                    
                    mSelectedMission = data.Value;
                    mSelectedMission.SelectBorder(EStateCard.Equip);
                });
        }
    }
}