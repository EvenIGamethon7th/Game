using System;
using Cargold.FrameWork.BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace _2_Scripts.UI.OutGame.Title
{
    public class UI_LoginBtn : MonoBehaviour
    {
        private Button mButton;

        private void Start()
        {
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(OnClickLoginBtn);   
        }

        private void OnClickLoginBtn()
        {
            mButton.interactable = false;
            BackEndManager.Instance.OnLogin(() =>
            {
                SceneManager.LoadScene("1_Scenes/Main");
            });
        }
    }
}