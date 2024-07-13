using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rito.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class MatController : SerializedMonoBehaviour
{
   private enum EMat
   {
      NORMAL,
      HIT,
      DISSOLVE
   }
   [SerializeField] 
   private Dictionary<EMat,Material> mMatList;
   private MaterialPropertyBlock mPropertyBlock;
   private SpriteRenderer mSpriteRenderer;

   private static readonly int MainTex = Shader.PropertyToID("_MainTex");
   private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");

   private void Awake()
   {
      mPropertyBlock = new MaterialPropertyBlock();
      mSpriteRenderer = GetComponent<SpriteRenderer>();
   }

   
   private void ChangeMat(EMat mat)
   {
      mSpriteRenderer.material = mMatList[mat];
   }


   public void RunDissolve(bool isApear = true, Action action = null)
   {
        ChangeMat(EMat.DISSOLVE);
        mPropertyBlock.SetTexture(MainTex,mSpriteRenderer.sprite.texture);
        float dissolveAmount = isApear ? 0 : 1;
        
        mSpriteRenderer.SetPropertyBlock(mPropertyBlock);
            
        DOTween.To(() => dissolveAmount, x =>
            {
                dissolveAmount = x;
                mPropertyBlock.SetFloat(DissolveAmount, dissolveAmount);
                mSpriteRenderer.SetPropertyBlock(mPropertyBlock);
            }, 1f - dissolveAmount, 1.5f)
            .OnComplete(() => { 
                action?.Invoke();
                ChangeMat(EMat.NORMAL);
            });
   }
   
   
}
