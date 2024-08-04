using _2_Scripts.Game.Monster;
using _2_Scripts.Utils;
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
                    GameObject canvas;

                    if (message.Value.IsBoss)
                    {
                        canvas = ObjectPoolManager.Instance.CreatePoolingObject("BossHpSlider", Vector3.zero);
                    }

                    else
                    {
                        canvas = ObjectPoolManager.Instance.CreatePoolingObject("DefaultMonsterHpSlider", Vector3.zero);
                    }

                    if (canvas == null) return;
                    CreateInterface(canvas, message.Value);
                })
                .AddTo(this);
        }

        private void CreateInterface(GameObject canvas, Monster monster)
        {
            var rect = canvas.GetComponent<RectTransform>();
            rect.SetParent(transform);
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(1, 0.25f);
            rect.localPosition = monster.transform.position + Vector3.up * (monster.IsBoss ? 2 : 1);
            if (canvas.TryGetComponent<IMonsterHpUI>(out var monsterHpUI))
            {
                monster.CurrentHpCanvas = monsterHpUI;
            }
        }
    }
}