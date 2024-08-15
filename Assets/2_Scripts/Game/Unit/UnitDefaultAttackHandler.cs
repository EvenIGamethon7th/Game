
using System;
using System.Threading;
using _2_Scripts.Game.ScriptableObject.Skill;
using _2_Scripts.Game.Sound;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public class UnitDefaultAttackHandler : MonoBehaviour
    {
        private CUnit mUnit;
        private CancellationTokenSource mCancellationToken;

        private void OnEnable()
        {
            mCancellationToken = new CancellationTokenSource();
            mUnit = GetComponent<CUnit>();
            Transaction().Forget();
        }

        private async UniTaskVoid Transaction()
        {
            //간혹가다가 유닛 판매 아카데미 보내기 시, 캔슬토큰이 안잡혀서 추가함
            while (!mCancellationToken.Token.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() => mUnit.CharacterDatas.IsActive,cancellationToken:mCancellationToken.Token);
                await UniTask.WaitForFixedUpdate(cancellationToken:mCancellationToken.Token);
                if(mUnit == null)
                    continue;
                float delayAttack = 1 / mUnit.CharacterDatas.GetTotalAtkSpeed();
                // 공격 캔슬 토큰이 계속 잡힘...
                await UniTask.WaitForSeconds(delayAttack,cancellationToken: mCancellationToken.Token);    
           
                if(EUnitStates.Move == mUnit.CurrentState)
                    continue;
                bool isAttack = mUnit.DefaultAttack();
                EUnitStates updateState = !isAttack
                        ? EUnitStates.Idle : EUnitStates.Attack;

                mUnit.UpdateState(updateState);
                if (isAttack)
                {
                    if (mUnit.CharacterDatas.academyClass == 1)
                    {
                        SoundManager.Instance.Play2DSound(AddressableTable.Sound_Attack_Knight);
                    }
                    else if (mUnit.CharacterDatas.academyClass == 2)
                    {
                        SoundManager.Instance.Play2DSound(AddressableTable.Sound_Archer_Attack);
                    }
                    else
                    {
                        SoundManager.Instance.Play2DSound(AddressableTable.Sound_Attack_Magician);
                    }
                }
                
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