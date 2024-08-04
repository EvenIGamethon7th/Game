using _2_Scripts.Utils;
using Cargold;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace _2_Scripts.UI.Ingame
{
    public class UI_MainCharacter : SerializedMonoBehaviour
    {
        private UI_MainCharacterButton mButton;
        private UI_MainCharacterCoolTime mCoolTime;
        private UI_MainCharcterInfoBubble mInfoBubble;

        [SerializeField]
        private SpriteAtlas mSpriteAtlas;

        [SerializeField]
        private Dictionary<string, Sprite> mButtonSpriteDict = new();

        private void Awake()
        {
            mButton = GetComponentInChildren<UI_MainCharacterButton>(true);
            mCoolTime = GetComponentInChildren<UI_MainCharacterCoolTime>(true);
            mInfoBubble = GetComponentInChildren<UI_MainCharcterInfoBubble>(true);
        }

        private void Start()
        {
            //TODO: 메인캐릭터를 매니저에서 받아온 후 자식들 초기화
            mInfoBubble.Init(null, null);
            if (GameManager.Instance.IsTest) return;
            var mainData = GameManager.Instance.CurrentMainCharacter;
            string name = mainData.CharacterEvolutions[1].GetData.characterData;

            Sprite[] s = new Sprite[mSpriteAtlas.spriteCount];
            mSpriteAtlas.GetSprites(s);
            foreach (var sprite in s)
            {
                if (sprite.name.Contains(name))
                {
                    var x = sprite.name;
                }
            }

            mButton.Init(mButtonSpriteDict[name], mainData.SkillList[0].CoolTime);

            mCoolTime.Init(s.FirstOrDefault(x => x.name == $"{name}(Clone)"));
            mInfoBubble.Init(mainData.CharacterEvolutions[1].GetData, s.FirstOrDefault(x => x.name == $"{name}_Info(Clone)"));
        }
    }
}