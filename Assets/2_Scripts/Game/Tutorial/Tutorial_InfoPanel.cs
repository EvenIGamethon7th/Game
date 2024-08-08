using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _2_Scripts.UI
{
    public class Tutorial_InfoPanel : MonoBehaviour
    {
        private TextMeshProUGUI mText;

        [SerializeField]
        private string[] mInfoString;

        private int mCount;

        private void Awake()
        {
            mText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            mText.text = mInfoString[mCount];
            ++mCount;
        }
    }
}