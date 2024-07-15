
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

        private void Start()
        {
            mUnit = GetComponent<CUnit>();
        }

        private void OnEnable()
        {
            mCancellationToken = new CancellationTokenSource();
            Transaction().Forget();
        }

        private async UniTaskVoid Transaction()
        {
            while (true)
            {
                await UniTask.WaitForFixedUpdate();
                if(mUnit == null)
                    continue;
                float delayAttack = 1 / mUnit.CharacterDatas.GetTotalAtkSpeed();
                await UniTask.WaitForSeconds(delayAttack,cancellationToken: mCancellationToken.Token);
                if(EUnitStates.Move == mUnit.CurrentState)
                    continue;
                EUnitStates updateState = !mUnit.DefaultAttack()
                        ? EUnitStates.Idle : EUnitStates.Attack;
                    mUnit.UpdateState(updateState);
                
            }
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