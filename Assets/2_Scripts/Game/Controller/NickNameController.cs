using System;
using _2_Scripts.UI.OutGame.Lobby;
using Cargold.FrameWork.BackEnd;
using TMPro;
using UnityEngine;

namespace _2_Scripts.Game.Controller
{
    public class NickNameController : MonoBehaviour
    {
        [SerializeField]
        private UI_NicknameBox mNickNameBox;
        [SerializeField]
        private TextMeshProUGUI mNickNameText;
        
        private void Start()
        {
            string userNickName = BackEndManager.Instance.GetUserNickName();
            if ( userNickName == null)
            {
                mNickNameBox.gameObject.SetActive(true);
            }
            else
            {
                mNickNameText.text = userNickName;
            }
        }
        
        //당장 넣을 controlle가 없어서 임시
        public void OnReadyButton()
        {
            UI_Toast_Manager.Instance.Activate_WithContent_Func("준비중 입니다.");
        }
    }
}