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
        MessageBroker.Default.Receive<GameMessage<int>>().Where(message => message.Message == EGameMessage.StageChange)
            .Subscribe(message => mWaveText.text = $"Wave {message.Value}");
    }

}
