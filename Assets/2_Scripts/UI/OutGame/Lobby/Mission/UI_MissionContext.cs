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
        private SpawnMission mSelectedMission;
        private void Start()
        {
            mMissionGrid.UpdateContents(BackEndManager.Instance.SpawnMissions());

            mSelectedMission = BackEndManager.Instance.SpawnMissions().DefaultIfEmpty(null).FirstOrDefault(x => x.IsEquip == true);

            MessageBroker.Default.Receive<GameMessage<SpawnMission>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter)
                .Subscribe(data =>
                {
                    if (mSelectedMission != null)
                        mSelectedMission.IsEquip = false;

                    mSelectedMission = data.Value;
                    mSelectedMission.IsEquip = true;

                    mMissionGrid.UpdateContents(BackEndManager.Instance.SpawnMissions());
                }).AddTo(this);
        }

        public void OnExitButton()
        {
            // BackEndManager.Instance.SaveMissionCharacterCardChange();
            this.gameObject.SetActive(false);
        }
    }
}