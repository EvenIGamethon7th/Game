using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_ManagePanel : MonoBehaviour
    {
        private Button[] mButtons;
        [SerializeField]
        private GameObject[] mCanvas;

        void Awake()
        {
            mButtons = GetComponentsInChildren<Button>();
            for (int i = 0; i < mCanvas.Length; ++i)
            {
                int num = i;
                mButtons[num].onClick.AddListener(() => ChangePannel(num));
            }
        }

        private void ChangePannel(int num)
        {
            bool isActive = mCanvas[num].activeInHierarchy;
            for (int i = 0; i < mCanvas.Length; ++i)
            {
                mCanvas[i].SetActive(false);
            }
            
            mCanvas[num].SetActive(!isActive);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < mCanvas.Length; ++i)
            {
                mButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
}