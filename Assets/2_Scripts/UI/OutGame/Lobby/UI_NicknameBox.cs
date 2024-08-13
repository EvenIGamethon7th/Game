using Cargold.FrameWork.BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_NicknameBox : MonoBehaviour,ISortPopUp
    {
        [SerializeField] private TextMeshProUGUI mErrorText;
        [SerializeField] private TMP_InputField mInputField;
        [SerializeField] private Button mConfirmButton;
        [SerializeField] private TextMeshProUGUI mNickNameText;
        private void Start()
        {
            mConfirmButton.onClick.AddListener(OnConfirmButton);
        }

        private void OnConfirmButton()
        {
           BackEndManager.Instance.ChangeDisplayName(mInputField.text, OnSuccess, OnError);
        }

        private void OnSuccess()
        {
            IsPopUpEnd = true;
            gameObject.SetActive(false);
            mNickNameText.text = PlayFabAuthService.NickName;
        }
        private void OnError(string text)
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func(text);
            mErrorText.text = text;
        }

        public int SortIndex { get; set; } = 1;
        public bool IsPopUpEnd { get; set; } = false;

        public void OnPopUp()
        {
            this.gameObject.SetActive(true);
        }
    }
}