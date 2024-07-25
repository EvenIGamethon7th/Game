using _2_Scripts.Utils;
using Cargold;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacter : MonoBehaviour
    {
        private UI_MainCharacterButton mButton;
        private UI_MainCharacterCoolTime mCoolTime;
        private UI_MainCharcterInfoBubble mInfoBubble;

        [SerializeField]
        private SpriteAtlas mSpriteAtlas;

        private void Awake()
        {
            mButton = GetComponentInChildren<UI_MainCharacterButton>(true);
            mCoolTime = GetComponentInChildren<UI_MainCharacterCoolTime>(true);
            mInfoBubble = GetComponentInChildren<UI_MainCharcterInfoBubble>(true);
        }

        private void Start()
        {
            //TODO: 메인캐릭터를 매니저에서 받아온 후 자식들 초기화
            if (!GameManager.Instance.IsTest) return;
            var mainData = GameManager.Instance.CurrentMainCharacter;
            string name = mainData.CharacterEvolutions[1].GetData.characterData;
            
            mButton.Init(mSpriteAtlas.GetSprite($"{name}_SkillIcon"), mainData.SkillList[0].CoolTime);
            mCoolTime.Init(mSpriteAtlas.GetSprite(name));
            mInfoBubble.Init(mainData.CharacterEvolutions[1].GetData);
        }
    }
}