using _2_Scripts.UI.OutGame.Lobby.Enchant;
using _2_Scripts.Utils;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpController : MonoBehaviour
    {
        [SerializeField] private GameObject mPopUpBackGround;
        [SerializeField] private UI_EnchantPopUpContainer mEnchantPopUpContainer;
        [SerializeField] private UI_RewardPopUpContainer mRewardPopUpContainer;
        private void Start()
        {
            MessageBroker.Default.Receive<GameMessage<Define.EnchantMainCharacterEvent>>()
                .Where(message => message.Message == EGameMessage.EnchantOpenPopUp).Subscribe(
                    data =>
                    {
                        mPopUpBackGround.SetActive(true);
                        mEnchantPopUpContainer.gameObject.SetActive(true);
                        mEnchantPopUpContainer.OnPopUp(data.Value);
                    }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<List<Define.RewardEvent>>>()
                .Where(message => message.Message == EGameMessage.RewardOpenPopUp).Subscribe(
                    data =>
                    {
                        mPopUpBackGround.SetActive(true);
                        mRewardPopUpContainer.gameObject.SetActive(true);
                        mRewardPopUpContainer.OnPopUp(data.Value);
                    }).AddTo(this);
        }
    }
}