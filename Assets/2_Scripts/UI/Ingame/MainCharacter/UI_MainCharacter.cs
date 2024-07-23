using _2_Scripts.Utils;
using Cargold;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacter : MonoBehaviour
    {
        UI_MainCharacterButton mButton;
        UI_MainCharacterCoolTime mCoolTime;
        UI_MainCharcterInfoBubble mInfoBubble;

        private void Awake()
        {
            mButton = GetComponentInChildren<UI_MainCharacterButton>(true);
            mCoolTime = GetComponentInChildren<UI_MainCharacterCoolTime>(true);
            mInfoBubble = GetComponentInChildren<UI_MainCharcterInfoBubble>(true);
        }

        private void Start()
        {
            //TODO: ����ĳ���͸� �Ŵ������� �޾ƿ� �� �ڽĵ� �ʱ�ȭ
            //mButton.Init();
            //mCoolTime.Init();
            mInfoBubble.Init(null);
        }
    }
}