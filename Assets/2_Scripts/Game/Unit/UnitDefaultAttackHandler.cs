
using System;
using System.Threading;
using _2_Scripts.Game.ScriptableObject.Skill;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public class UnitDefaultAttackHandler : MonoBehaviour
    {
        private CUnit mUnit;
        private CancellationTokenSource mCancellationToken;
        private bool mbIsAttack = false;
        public void Start()
        {
            mUnit = GetComponent<CUnit>();
            mCancellationToken = new CancellationTokenSource();
            mUnit.AddActionState(EUnitStates.Attack, () =>
            {
                Attack().Forget();
            });
            Transaction().Forget();
        }

        private async UniTaskVoid Transaction()
        {
            while (!mCancellationToken.IsCancellationRequested)
            {
                await UniTask.WaitForFixedUpdate();
                if (mUnit.CurrentState == EUnitStates.Idle)
                {
                    EUnitStates updateState = !mUnit.CharacterDataInfo.DefaultAttack.CanCastAttack(this.transform,mUnit.CharacterDatas.range) ? EUnitStates.Idle : EUnitStates.Attack;
                    mUnit.UpdateState(updateState);
                }
            }
        }

        private async UniTaskVoid Attack()
        {
            if(mbIsAttack)
                return;
            mbIsAttack = true;
            mUnit.CharacterDataInfo.DefaultAttack.CastAttack(this.transform, mUnit.CharacterDatas);
            await UniTask.WaitForSeconds(mUnit.CharacterDatas.atkSpeed, cancellationToken: mCancellationToken.Token);
            mUnit.UpdateState(EUnitStates.Idle);
            mbIsAttack = false;
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
    }
}