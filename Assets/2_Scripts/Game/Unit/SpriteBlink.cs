using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    using Monster = _2_Scripts.Game.Monster.Monster;
    /// <summary>
    /// 왜 material을 사용하지 않는가? mat은 Draw Call이 따로 잡히는데 sprite color 변경은 따로 안잡힘 
    /// </summary>
    public class SpriteBlink : MonoBehaviour,IDamagebleAction
    {
        [SerializeField]
        private Color mBlinkColor = Color.red;

        [Title(" 깜박일 횟 수")]
        [SerializeField] 
        private int blinkCount = 3;
        
        [Title("깜박이는 시간")]
        [SerializeField]
        private float  blinkDuration = 0.05f;
        
        private Color mOriginColor;
        private SpriteRenderer mSpriteRenderer;
        
        private Sequence blinkSequence;
        private void Awake()
        {
            mSpriteRenderer = GetComponent<SpriteRenderer>();
            mOriginColor = mSpriteRenderer.color;
            blinkSequence = DOTween.Sequence();
            for (int i = 0; i < blinkCount; i++)
            {
                blinkSequence.Append(mSpriteRenderer.DOColor(mBlinkColor, blinkDuration));
                blinkSequence.Append(mSpriteRenderer.DOColor(mOriginColor, blinkDuration));
            }
            blinkSequence.SetAutoKill(false);
        }

        private void BlinkSprite()
        {
            blinkSequence.Restart();
        }
        public Action DamageAction()
        {
            return BlinkSprite;
        }
    }
}