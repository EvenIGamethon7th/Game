using _2_Scripts.UI.OutGame.Lobby.Enchant;
using _2_Scripts.Utils;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PopUpController : MonoBehaviour
    {
        [SerializeField] private GameObject mPopUpBackGround;
        [SerializeField] private UI_EnchantPopUpContainer mEnchantPopUpContainer;
        
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
        }
    }
}