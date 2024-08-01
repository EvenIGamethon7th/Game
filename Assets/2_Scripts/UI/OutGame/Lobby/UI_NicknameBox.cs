using Cargold.FrameWork.BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_NicknameBox : MonoBehaviour
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
            gameObject.SetActive(false);
            mNickNameText.text = PlayFabAuthService.NickName;
        }
        private void OnError(string text)
        {
            mErrorText.text = text;
        }
    }
}