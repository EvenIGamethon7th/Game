using System;
using Cargold.FrameWork.BackEnd;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.OutGame.Lobby
{
    public class UI_PlayPopUp : MonoBehaviour
    {
        [SerializeField]
        private Button mPlayButton;

        public void Start()
        {
            mPlayButton.onClick.AddListener(OnClickPlay);
        }

        private void OnClickPlay()
        {
            if (BackEndManager.Instance.UserCurrency[ECurrency.Father].Value <= 0)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("깃털이 부족합니다.");
                return;
            }
            
            SceneLoadManager.Instance.SceneChange("Main");
        }
    }
}