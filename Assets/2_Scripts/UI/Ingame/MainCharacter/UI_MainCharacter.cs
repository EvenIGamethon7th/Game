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

        private void Awake()
        {
            mButton = GetComponentInChildren<UI_MainCharacterButton>();
        }

        private void Start()
        {
            //TODO: ����ĳ���͸� �Ŵ������� �޾ƿ� �� �ڽĵ� �ʱ�ȭ
            //mButton.Init()
        }
    }
}