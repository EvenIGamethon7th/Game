using System;
using System.Linq;
using _2_Scripts.Game.BackEndData.Mission;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_CharacterBackGround : MonoBehaviour
    {
        [SerializeField] private Image mCharacterImage;

        private void Start()
        {
            OnChangeCharacter();
            MessageBroker.Default.Receive<GameMessage<SpawnMission>>()
                .Where(message => message.Message == EGameMessage.SelectCharacter).Subscribe(card  => OnChangeCharacter(card.Value)).AddTo(this);
        }

        private void OnChangeCharacter(SpawnMission card = null)
        {
            if(card != null){
                mCharacterImage.sprite = DataBase_Manager.Instance.GetCharacter.GetData_Func(card.CharacterKey).illustration;
                return;
            }
            
            var characterMission = BackEndManager.Instance.UserMission.FirstOrDefault(mission => mission.Value.IsEquip).Key;
            if (characterMission == null)
            {
                return;
            }
            mCharacterImage.sprite =
                DataBase_Manager.Instance.GetCharacter.GetData_Func(characterMission).illustration;

        }
    }
}