using System;
using System.Collections.Generic;
using Cargold;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_LockButton : MonoBehaviour
    {
        [SerializeField] private Button mButton;

        [SerializeField] private List<UI_SummonButton> SummonButtons;

        [SerializeField] private Image mImage;

        [SerializeField] private TextMeshProUGUI mText;
        
        [SerializeField]
        private Sprite[] mSprites;
        [SerializeField]
        private string[] mTexts;

        public bool IsLock { get; private set; } = false;

        private void Start()
        {
            mButton.onClick.AddListener(OnLockButton);
        }
        
        private void OnLockButton()
        {
            IsLock = !IsLock;
            Tween_C.OnPunch_Func(transform);
            foreach (var button in SummonButtons)
            {
                button.OnLockButton(IsLock);
            }
            int index = IsLock ? 1 : 0;
            mImage.sprite = mSprites[index];
            mText.text = mTexts[index];
        }
        
    }
}