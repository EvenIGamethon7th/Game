using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_AcademyInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject mBubble;
        [SerializeField]
        private TextMeshProUGUI[] mInfoText;

        private Button mButton;

        void Awake()
        {
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(OnClick);
            mBubble = transform.GetChild(0).gameObject;
            mInfoText = mBubble?.GetComponents<TextMeshProUGUI>();
        }

        public void SetText(float[] rateArr)
        {
            mInfoText[0].text = $"성공: {rateArr[0]}%";
            mInfoText[1].text = $"대성공: {rateArr[1]}%";
            mInfoText[2].text = $"실패: {rateArr[2]}%";
        }

        private void OnClick()
        {
            mBubble.SetActive(!mBubble.activeSelf);
        }

        private void OnDestroy()
        {
            mButton.onClick.RemoveAllListeners();
        }
    }
}