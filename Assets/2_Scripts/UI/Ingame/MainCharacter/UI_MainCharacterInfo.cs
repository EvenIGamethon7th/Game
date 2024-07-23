using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacterInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject mInfo;
        private Button mButton;

        private void Awake()
        {
            mInfo.SetActive(false);
            mButton = GetComponent<Button>();
            mButton.onClick.AddListener(SetActiveInfo);
        }

        private void SetActiveInfo()
        {
            mInfo.SetActive(true);
        }

        private void OnDestroy()
        {
            mButton?.onClick.RemoveAllListeners();
        }
    }
}