using _2_Scripts.Game.Map.Tile;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace _2_Scripts.Game.Unit
{
    public class UnitGroup : MonoBehaviour
    {
        /* TODO speed와 같이 데이터 부분은 따로 DataSheet에서 관리할 예정.*/
        private float mSpeed = 1.0f;
        private CancellationTokenSource mToken = new CancellationTokenSource();

        private readonly int nMaxUnitCount = 3;
        private int mCurrentUnitCount = 0;

        private CUnit[] mUnits;

        private void Awake()
        {
            mUnits = new CUnit[nMaxUnitCount];
        }

        public void MoveGroup(TileSlot destinationTileSlot)
        {
            mToken.Cancel();
            mToken.Dispose();
            mToken = new();
            MoveGroupAsync(destinationTileSlot.transform.position).Forget();
        }

        private async UniTask MoveGroupAsync(Vector3 dstPos, float time = 1f)
        {
            Vector3 originPos = transform.position;

            int flip = dstPos.x > originPos.x ? 1 : -1;
            for (int i = 0; i < mUnits.Length; ++i)
            {
                if (mUnits[i] == null)
                    break;
                mUnits[i].UpdateState(EUnitStates.Move);
                mUnits[i].transform.localScale = new Vector3(Mathf.Abs(mUnits[i].transform.localScale.x) * flip, mUnits[i].transform.localScale.y, 1);
            }
            
            while (time >= 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mToken.Token);
                transform.position = Vector3.Lerp(dstPos, originPos, Mathf.Max(time, 0));
                time -= Time.deltaTime;
            }
            transform.position = dstPos;
            for (int i = 0; i < mUnits.Length; ++i)
                mUnits[i]?.UpdateState(EUnitStates.Idle);
        } 

        public bool CanAddUnit()
        {
            if (mCurrentUnitCount == 1 && mUnits[0].CurrentUnitRank == EUnitRank.Unique)
                return false;

            return mCurrentUnitCount < nMaxUnitCount;
        }

        public void AddUnit(CUnit newUnit)
        {
            if (mCurrentUnitCount == 1 && mUnits[0].CurrentUnitRank == EUnitRank.Unique) return;
                if (mCurrentUnitCount >= nMaxUnitCount) return;

            newUnit.transform.parent = transform;
            mUnits[mCurrentUnitCount] = newUnit;
            switch (mCurrentUnitCount)
            {
                case 1:
                    newUnit.transform.position += new Vector3(0.5f, 0.5f, 0);
                    break;

                case 2:
                    newUnit.transform.position -= new Vector3(0.5f, 0.5f, 0);
                    break;
            }
            ++mCurrentUnitCount;
        }
    }
}