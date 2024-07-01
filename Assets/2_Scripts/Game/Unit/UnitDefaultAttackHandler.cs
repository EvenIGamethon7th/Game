using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _2_Scripts.Game.ScriptableObject.Skill;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public class UnitDefaultAttackHandler : MonoBehaviour
    {
        private Skill mDefaultAttack;
        private CharacterData mCharacterData;
        private Action mAttackAction;
        private CancellationTokenSource mCancellationToken = new CancellationTokenSource();
        public void SetAttack(Skill attack,CharacterData characterData,Action attackAction)
        {
            mDefaultAttack = attack;
            mCharacterData = characterData;
            mAttackAction = attackAction;
            CancelAndDisposeToken();
            TryAttack().Forget();
        }

        private void OnDisable()
        {
            CancelAndDisposeToken();
        }

        private void CancelAndDisposeToken()
        {
            if (mCancellationToken != null)
            {
                if (!mCancellationToken.IsCancellationRequested)
                {
                    mCancellationToken.Cancel();
                }
                mCancellationToken.Dispose();
                mCancellationToken = null;
            }
            else
            {
                mCancellationToken = new CancellationTokenSource();
            }
        }

        private async UniTaskVoid TryAttack()
        {
            while (!mCancellationToken.IsCancellationRequested)
            {
                await UniTask.WaitForFixedUpdate();
                if (!mDefaultAttack.CanCastAttack(this.transform, mCharacterData.range))
                {
                    continue;
                }
                mDefaultAttack.CastAttack(this.transform, mCharacterData);
                mAttackAction();
                await UniTask.WaitForSeconds(mCharacterData.atkSpeed, cancellationToken: mCancellationToken.Token);
            }
        } 
        
    }
}