using CharacterInfo = _2_Scripts.Game.ScriptableObject.Character.CharacterInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _2_Scripts.Utils;
using UniRx;

namespace _2_Scripts.UI
{
    public class UI_SpawnPannel : MonoBehaviour
    {
        private UI_SummonButton[] mButtons;

        [SerializeField]
        private CharacterInfo[] mInfos;

        void Start()
        {
            mButtons = GetComponentsInChildren<UI_SummonButton>(true);
            SetButton(0);
            MessageBroker.Default.Receive<GameMessage<int>>()
                .Where(message => message.Message == EGameMessage.StageChange)
                .Subscribe(message => SetButton(message.Value)).AddTo(this);
        }

        private void SetButton(int num)
        {
            int count = num << 2;
            for (int i = 0; i < mButtons.Length; i++)
            {
                mButtons[i].Reroll();
                if (mInfos.Length <= count) continue;

                else mButtons[i].UpdateCharacter(mInfos[i + count], 1);
            }
        }
    }
}