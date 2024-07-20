using _2_Scripts.Utils;
using Cargold;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.UI
{
    public class UI_EditWaveCanvas : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField mStage;
        [SerializeField]
        private TMP_InputField mWave;
        [SerializeField]
        private Button mButton;

        private void Awake()
        {
            mButton.onClick.AddListener(ClickButton);
        }

        private void ClickButton()
        {
            MessageBroker.Default.Publish(new EditMessage<int, int>(mStage.text.ToInt(), mWave.text.ToInt() - 1));
        }
    }
}