using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_AcademyInfo : MonoBehaviour
    {
        private readonly Color Bonanza = new Color(0.996f, 0.859f, 0.373f);
        private readonly Color Success = new Color(0.227f, 0.635f, 0.447f);
        private readonly Color Fail = new Color(0.996f, 0.169f, 0.153f);

        [SerializeField]
        private GameObject mBubble;
        [SerializeField]
        private TextMeshProUGUI[] mInfoText;

        private Button mButton;

        public void Init()
        {
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(OnClick);
            mBubble = transform.GetChild(0).gameObject;
            mInfoText = mBubble?.GetComponentsInChildren<TextMeshProUGUI>(true);
            mInfoText[0].color = Success;
            mInfoText[1].color = Bonanza;
            mInfoText[2].color = Fail;
            mInfoText[0].text = "성공: ";
            mInfoText[1].text = "대성공: ";
            mInfoText[2].text = "실패: ";
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