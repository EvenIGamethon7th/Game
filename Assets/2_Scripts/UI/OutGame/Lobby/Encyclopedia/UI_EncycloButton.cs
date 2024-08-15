using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using _2_Scripts.Game.Sound;

namespace _2_Scripts.UI.OutGame.Lobby.Encyclopedia
{
    public class UI_EncycloButton : MonoBehaviour
    {
        private Button mButton;

        [SerializeField]
        private int mIndex;

        [SerializeField]
        private Image mImage;

        [SerializeField]
        private TextMeshProUGUI mText;

        private GameMessage<CharacterInfo> mMessage;

        private void Awake()
        {
            mButton = GetComponent<Button>();
            mText = GetComponentInChildren<TextMeshProUGUI>();
            var info = GameManager.Instance.GetCharacterInfo(mIndex);
            mMessage = new GameMessage<CharacterInfo>(EGameMessage.ProductDetailPopUp, info);
            mImage.sprite = info.CharacterEvolutions[1].GetData.GetCharacterSprite();
            mText.text = info.CharacterEvolutions[1].GetData.GetCharacterName();
            mButton.onClick.AddListener(Click);
        }

        private void Click()
        {
            SoundManager.Instance.Play2DSound(AddressableTable.Sound_Button);
            MessageBroker.Default.Publish(mMessage);
        }

        private void OnDestroy()
        {
            mButton.onClick.RemoveAllListeners();
        }
    }
}