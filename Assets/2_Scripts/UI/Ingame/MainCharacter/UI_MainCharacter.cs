using _2_Scripts.Game.ScriptableObject.Character;
using _2_Scripts.Utils;
using Cargold;
using Cargold.FrameWork.BackEnd;
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

        [SerializeField]
        private MainCharacterInfo mCharacterInfo;

        private void Awake()
        {
            mButton = GetComponentInChildren<UI_MainCharacterButton>(true);
            mCoolTime = GetComponentInChildren<UI_MainCharacterCoolTime>(true);
            mInfoBubble = GetComponentInChildren<UI_MainCharcterInfoBubble>(true);
        }

        private void Start()
        {
            int rank = 1;
            if (BackEndManager.Instance.IsUserTutorial)
            {
                mCharacterInfo = GameManager.Instance.CurrentMainCharacter;
                rank = BackEndManager.Instance.UserMainCharacterData[mCharacterInfo.name].rank;
            }
            string name = mCharacterInfo.CharacterEvolutions[rank].GetData.characterData;

            Sprite[] s = new Sprite[mSpriteAtlas.spriteCount];
            mSpriteAtlas.GetSprites(s);
            foreach (var sprite in s)
            {
                if (sprite.name.Contains(name))
                {
                    var x = sprite.name;
                }
            }

            mButton.Init(mButtonSpriteDict[name], mCharacterInfo.SkillList[rank - 1].CoolTime);

            mCoolTime.Init(s.FirstOrDefault(x => x.name == $"{name}(Clone)"));
            mInfoBubble.Init(mCharacterInfo.CharacterEvolutions[rank].GetData, s.FirstOrDefault(x => x.name == $"{name}_Info(Clone)"));
        }
    }
}