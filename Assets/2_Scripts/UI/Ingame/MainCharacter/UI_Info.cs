using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_Info : MonoBehaviour
    {
        [SerializeField]
        private GameObject mInfo;
        private Button mButton;

        private void Awake()
        {
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(SetActiveInfo);
        }

        private void SetActiveInfo()
        {
            if (!mInfo.activeSelf)
                mInfo.SetActive(true);
        }

        private void OnDestroy()
        {
            mButton?.onClick.RemoveAllListeners();
        }
    }
}