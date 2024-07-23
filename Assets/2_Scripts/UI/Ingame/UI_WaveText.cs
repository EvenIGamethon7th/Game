using System.Collections;
using System.Collections.Generic;
using _2_Scripts.Utils;
using TMPro;
using UniRx;
using UnityEngine;

public class UI_WaveText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI mWaveText;
    void Start()
    {
        MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange && message.Value != 0)
            .Subscribe(message => mWaveText.text = $"WAVE {message.Value}").AddTo(this);
    }

}
