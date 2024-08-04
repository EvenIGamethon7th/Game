using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Dialog
{
    public class UI_ChoosPanel : MonoBehaviour
    {
        private List<UI_ChoiceButton> mButtons = new List<UI_ChoiceButton>();

        public void SetChoiceCard(IEnumerable<StoryData> datas, Action<int> action)
        {
            gameObject.SetActive(true);
            int count = 0;
            foreach (StoryData data in datas)
            {
                int temp = count;
                var button = ObjectPoolManager.Instance.CreatePoolingObject("ChoiceButton", transform.position).GetComponent<UI_ChoiceButton>();
                button.transform.SetParent(transform);

                var rect = button.GetComponent<RectTransform>();
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(855, 100);
                mButtons.Add(button);
                button.Init(data.Description);
                button.onClick.AddListener(() => WaitButton(temp, action));
                ++count;
            }
        }

        private void WaitButton(int count, Action<int> action)
        {
            action?.Invoke(count);
            foreach (var button in mButtons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }
}