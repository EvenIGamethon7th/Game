using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Enchant
{
    public class UI_TabButton : MonoBehaviour
    {
        [SerializeField] private Image mContainerImage;
        [SerializeField] private Sprite[] mTabSprites;
        [SerializeField] private GameObject[] mBottomDeco;
        [SerializeField] private GameObject mShowObject;
        [SerializeField] private Button mButton;
        public bool IsSelected { get; private set; } = false;

        public void InitButton(Action<UI_TabButton> action)
        {
            mButton.onClick.AddListener(()=>action.Invoke(this));
        }

        public void OnSelectButton()
        {
            IsSelected = true;
            mShowObject.SetActive(true);
            mContainerImage.sprite = mTabSprites[1];
            mBottomDeco[0].SetActive(false);
            mBottomDeco[1].SetActive(true);
        }

        public void OnDeselectButton()
        {
            IsSelected = false;
            mShowObject.SetActive(false);
            mContainerImage.sprite = mTabSprites[0];
            mBottomDeco[0].SetActive(true);
            mBottomDeco[1].SetActive(false);
        }
    }
}