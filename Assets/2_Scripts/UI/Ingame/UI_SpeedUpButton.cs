using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI.Ingame
{
    public class UI_SpeedUpButton : MonoBehaviour
    {
        private TextMeshProUGUI mSpeedText;
        private Button mSpeedUpButton;

        [SerializeField]
        private string[] mTexts;

        private int mSpeed;

        private void Awake()
        {
            mSpeedText = GetComponentInChildren<TextMeshProUGUI>();
            mSpeedUpButton = GetComponent<Button>();
            mSpeedText.text = mTexts[0];
            mSpeedUpButton.onClick.AddListener(SpeedChange);
        }

        private void SpeedChange()
        {
            mSpeed = (mSpeed + 1) % mTexts.Length;
            mSpeedText.text = mTexts[mSpeed];
            Time.timeScale = 1 + 0.5f * mSpeed;
        }
    }
}