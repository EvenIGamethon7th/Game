using System;
using _2_Scripts.Game.Sound;
using _2_Scripts.Utils;
using Cargold.FrameWork;
using Cargold.FrameWork.BackEnd;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace _2_Scripts.UI.OutGame.Title
{
    public class UI_LoginBtn : MonoBehaviour
    {
        [SerializeField]
        private Button mButton;

        [SerializeField]
        private TextMeshProUGUI mTapToStartText;

        private int mLoadCount;
        private void Start()
        {
            mButton.interactable = false;
            mTapToStartText.text = "이세계 아카데미 진입 중...!";
            MessageBroker.Default.Receive<TaskMessage>().Where(message =>
                message.Task == ETaskList.DefaultResourceLoad
            ).Subscribe(_ =>
            {
                SetCount();
            }).AddTo(this);

            BackEndManager.Instance.OnLogin(() =>
            {
                SetCount();
            });
        }

        private void SetCount()
        {
            ++mLoadCount;
            if (mLoadCount >= 2)
            {
                OnButton();
            }
        }

        private void OnButton()
        {
            mButton.interactable = true;
            mTapToStartText.text = "Tap to Start";
            mButton.onClick.AddListener(OnClickLoginBtn);
        }

        private void OnClickLoginBtn()
        {
            mButton.interactable = false;
            GameManager.Instance.NotTestMode();
            if (!GameManager.Instance.IsFirstConnect)
                SceneLoadManager.Instance.SceneChange("LobbyScene");
            else
                SceneLoadManager.Instance.SceneChange("PrologueScene");
        }
    }
}