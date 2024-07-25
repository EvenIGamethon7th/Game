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
            //TODO: ����ĳ���͸� �Ŵ������� �޾ƿ� �� �ڽĵ� �ʱ�ȭ
            if (GameManager.Instance.IsTest) return;
            var mainData = GameManager.Instance.CurrentMainCharacter;
            string name = mainData.CharacterEvolutions[1].GetData.characterData;

            //Sprite[] s = new Sprite[mSpriteAtlas.spriteCount];
            //mSpriteAtlas.GetSprites(s);
            //foreach (var sprite in s)
            //{
            //    if (sprite.name.Contains(name))
            //    {
            //        var st = sprite.name;
            //    }
            //}
            
            mButton.Init(mSpriteAtlas.GetSprite($"{name}_SkillIcon(Clone)"), mainData.SkillList[0].CoolTime);
            mCoolTime.Init(mSpriteAtlas.GetSprite($"{name}(Clone)"));
            mInfoBubble.Init(mainData.CharacterEvolutions[1].GetData);
        }
    }
}