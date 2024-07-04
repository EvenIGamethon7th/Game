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

        private readonly int mMaxUnitCount = 3;
        private int mCurrentUnitCount = 0;

        public float GroupRange;
        
        private CUnit[] mUnits;

        private EUnitStates mCurrentState;
        private int mFlip = 1;

        private void Awake()
        {
            mUnits = new CUnit[mMaxUnitCount];
        }

        public bool CanFusion()
        {
            if (mCurrentUnitCount != mMaxUnitCount || mUnits[0].CharacterDatas.rank == (int)EUnitRank.Unique)
                return false;
            return true;
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

            mFlip = dstPos.x > originPos.x ? 1 : -1;
            mCurrentState = EUnitStates.Move;

            for (int i = 0; i < mUnits.Length; ++i)
            {
                if (mUnits[i] == null)
                    break;
                
                mUnits[i].UpdateState(mCurrentState);
                mUnits[i].transform.localScale = new Vector3(Mathf.Abs(mUnits[i].transform.localScale.x) * mFlip, mUnits[i].transform.localScale.y, mUnits[i].transform.localScale.z);
            }
            
            while (time >= 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mToken.Token);
                transform.position = Vector3.Lerp(dstPos, originPos, Mathf.Max(time, 0));
                time -= Time.deltaTime;
            }
            transform.position = dstPos;

            mCurrentState = EUnitStates.Idle;
            for (int i = 0; i < mUnits.Length; ++i)
                mUnits[i]?.UpdateState(mCurrentState);
        } 

        public CharacterData GetCharacterData()
        {
            return mUnits[0]?.CharacterDatas;
        }

        public bool CanAddUnit()
        {
            if (mCurrentUnitCount == 1 && mUnits[0].CharacterDatas.rank == (int)EUnitRank.Unique)
                return false;

            return mCurrentUnitCount < mMaxUnitCount;
        }

        public void AddUnit(CUnit newUnit)
        {
            if (mCurrentUnitCount == 1 && mUnits[0].CharacterDatas.rank == (int)EUnitRank.Unique) return;
            if (mCurrentUnitCount >= mMaxUnitCount) return;

            newUnit.transform.parent = transform;
            newUnit.transform.position = transform.position;
            newUnit.UpdateState(mCurrentState);

            mUnits[mCurrentUnitCount] = newUnit;
            GroupRange = newUnit.CharacterDatas.range;
            newUnit.transform.localScale = new Vector3(Mathf.Abs(newUnit.transform.localScale.x) * mFlip, newUnit.transform.localScale.y, newUnit.transform.localScale.z);
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

        public void Clear()
        {
            mToken.Cancel();
            mToken.Dispose();
            mCurrentUnitCount = 0;
            for (int i = 0; i < mUnits.Length; ++i)
            {
                mUnits[i].Clear();
            }

            mUnits.Initialize();

            gameObject.SetActive(false);
        }
    }
}