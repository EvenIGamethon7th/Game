using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MonsterHpCanvas : UI_Base
    {
        private void Awake()
        {
            MessageBroker.Default.Receive<GameMessage<Monster>>()
                .Subscribe(message =>
                {
                    GameObject canvas = ObjectPoolManager.Instance.CreatePoolingObject("DefaultMonsterHpSlider", message.Value.transform.position);
                    if (canvas == null) return;
                    var rect = canvas.GetComponent<RectTransform>();
                    rect.parent = transform;
                    rect.localScale = Vector3.one;
                    rect.localPosition = Vector3.zero;
                    rect.sizeDelta = new Vector2(1, 0.25f);
                    if (canvas.TryGetComponent<IMonsterHpUI>(out var monsterHpUI))
                    {
                        message.Value.CurrentHpCanvas = monsterHpUI;
                    }
                })
                .AddTo(this);
        }
    }
}