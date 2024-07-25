using _2_Scripts.Game.ScriptableObject.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_SelectMainCharacter : MonoBehaviour
    {
        [SerializeField]
        private string[] mMainCharacters;
        [SerializeField]
        private Sprite mOffSprite;
        [SerializeField]
        private Sprite mOnSprite;

        private Button[] mSelectButtons;
        private Image[] mSelectImages;

        [SerializeField]
        private Sprite[] mCharacterImage;

        [SerializeField]
        private Image mSelectCharacterImage;

        [SerializeField]
        private TextMeshProUGUI mCharacterName;

        private void Awake()
        {
            mSelectButtons = GetComponentsInChildren<Button>(true);
            mSelectImages = new Image[mSelectButtons.Length];

            for (int i = 0; i < mSelectButtons.Length; i++)
            {
                int num = i;
                mSelectButtons[num].onClick.AddListener(() => SelectButton(num));
                mSelectImages[num] = mSelectButtons[num].GetComponent<Image>();
            }
        }

        private void SelectButton(int num)
        {
            for (int i = 0; i < mSelectButtons.Length; ++i)
            {
                mSelectImages[i].sprite = mOffSprite;
            }

            mSelectImages[num].sprite = mOnSprite;

            mSelectCharacterImage.sprite = mCharacterImage[num];

            GameManager.Instance.CurrentMainCharacter = ResourceManager.Instance.Load<MainCharacterInfo>(mMainCharacters[num]);
            mCharacterName.text = GameManager.Instance.CurrentMainCharacter.CharacterEvolutions[1].GetData.GetCharacterName();
        }
    }
}