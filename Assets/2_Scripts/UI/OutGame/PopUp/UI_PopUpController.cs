using _2_Scripts.UI.OutGame.Lobby.DailyReward;
using _2_Scripts.UI.OutGame.Lobby.Enchant;
using _2_Scripts.UI.OutGame.PopUp;
using _2_Scripts.Utils;
using Cargold.FrameWork.BackEnd;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpController : MonoBehaviour
    {
        [SerializeField] private GameObject mPopUpBackGround;
        [SerializeField] private UI_EnchantPopUpContainer mEnchantPopUpContainer;
        [SerializeField] private UI_RewardPopUpContainer mRewardPopUpContainer;
        [SerializeField] private UI_ProductDetailPopUp mProductDetailContainer;
        [SerializeField] private UI_DailyContainer mDailyContainer;
        [SerializeField] private UI_MainCharacterSelectPopUp mMainCharacterSelectPopUp;
        [SerializeField] private UI_EncyclopediaPopUp mEncyclopediaPopUp;

        private void Start()
        {
            if (BackEndManager.Instance.UserDailyReward < 7 && BackEndManager.Instance.UserCurrency[ECurrency.DailyReward].Value >= 1)
            {
                MessageBroker.Default.Publish(new GameMessage<ISortPopUp>(EGameMessage.SortPopUp, mDailyContainer.GetComponent<ISortPopUp>()));
            }

            if (BackEndManager.Instance.IsSelectMainCharacter == false)
            {
                MessageBroker.Default.Publish(new GameMessage<ISortPopUp>(EGameMessage.SortPopUp, mMainCharacterSelectPopUp.GetComponent<ISortPopUp>()));
            }
            
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
            MessageBroker.Default.Receive<GameMessage<Define.RewardEvent>>()
                .Where(message => message.Message == EGameMessage.RewardOpenPopUp).Subscribe(
                    data =>
                    {
                        mPopUpBackGround.SetActive(true);
                        mRewardPopUpContainer.gameObject.SetActive(true);
                        mRewardPopUpContainer.OnPopUp(data.Value);
                    }).AddTo(this);
            
            MessageBroker.Default.Receive<GameMessage<ProductDetailsData>>().Where(message => message.Message == EGameMessage.ProductDetailPopUp).Subscribe(
                data =>
                {
                    mPopUpBackGround.SetActive(true);
                    mProductDetailContainer.gameObject.SetActive(true);
                    mProductDetailContainer.UpdateContent(data.Value);
                }).AddTo(this);

            MessageBroker.Default.Receive<GameMessage<CharacterInfo>>().Where(message => message.Message == EGameMessage.ProductDetailPopUp).Subscribe(
                data =>
                {
                    mPopUpBackGround.SetActive(true);
                    mEncyclopediaPopUp.gameObject.SetActive(true);
                    mEncyclopediaPopUp.OnPopUp(data.Value);
                }).AddTo(this);
        }
    }
}