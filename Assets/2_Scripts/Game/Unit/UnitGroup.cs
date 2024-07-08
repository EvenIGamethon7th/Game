using _2_Scripts.Game.Map.Tile;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace _2_Scripts.Game.Unit
{
    public class UnitGroup : MonoBehaviour
    {
        public bool IsSelect
        {
            get
            {
                return mIsSelect;
            }
            set
            {
                mIsSelect = value;
                mAcademyButton.gameObject.SetActive(mIsSelect);
            }
        }

        private bool mIsSelect = false;
        private CancellationTokenSource mToken = new CancellationTokenSource();

        private readonly int mMaxUnitCount = 3;
        private int mCurrentUnitCount = 0;

        public float GroupRange;
        
        private List<CUnit> mUnits;
        private Button mAcademyButton;

        private EUnitStates mCurrentState;
        private int mFlip = 1;

        private void Awake()
        {
            mUnits = new List<CUnit>(mMaxUnitCount);
            mAcademyButton = GetComponentInChildren<Button>();
            var canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = Camera.main;
            mAcademyButton.onClick.AddListener(() =>
            {
                CUnit unit = mUnits.Where(x => x.CharacterDatas.isAlumni == false).FirstOrDefault();
                if (unit == null)
                {
                    UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 전부 졸업했습니다");
                    return;
                }

                bool canEnterAcademy = MapManager.Instance.GoAcademy(this, unit);
                if (!canEnterAcademy) return; 

                RemoveUnit(unit);
                unit.Clear();
            });
            mAcademyButton.gameObject.SetActive(false);
        }

        private void MoveUnitOtherGroup(UnitGroup otherGroup)
        {
            var newUnit = otherGroup.mUnits.Last();
            otherGroup.RemoveUnit(newUnit);
            AddUnit(newUnit);
        }

        public bool CanFusion()
        {
            if (mCurrentUnitCount != mMaxUnitCount || mUnits[0]?.CharacterDatas.rank == (int)EUnitRank.Unique)
                return false;
            return true;
        }

        public void Fusion()
        {
            CharacterData alumniData = mUnits[0].CharacterDatas + mUnits[1].CharacterDatas + mUnits[2].CharacterDatas;
            var newData = mUnits[0].CharacterDataInfo.CharacterEvolutions[mUnits[0].CharacterDatas.rank + 1].GetData;
            newData = global::Utils.DeepCopy(newData);
            newData.AddAlumniInfo(alumniData);
            MapManager.Instance.ClearTile(this);
            MapManager.Instance.CreateUnit(newData);
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

            for (int i = 0; i < mUnits.Count; ++i)
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
            for (int i = 0; i < mUnits.Count; ++i)
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

        private void RemoveUnit(CUnit unit)
        {
            mUnits.Remove(unit);
            --mCurrentUnitCount;
            SetUnitPos();
            //가지고 있는 유닛이 없으므로
            if (mUnits.Count == 0) MapManager.Instance.ClearTile(this);
            //타일 중 같은 유닛이 있는지 확인해야 한다
            else if (mUnits.Count == 2)
            {
                var otherGroup = MapManager.Instance.CheckOccupantSameUnit(this);
                if (otherGroup == null) return;
                MoveUnitOtherGroup(otherGroup);
            }
        }

        public void AddUnit(CUnit newUnit)
        {
            if (mCurrentUnitCount == 1 && mUnits[0].CharacterDatas.rank == (int)EUnitRank.Unique) return;
            if (mCurrentUnitCount >= mMaxUnitCount) return;

            newUnit.transform.parent = transform;
            newUnit.UpdateState(mCurrentState);

            mUnits.Add(newUnit);
            GroupRange = newUnit.CharacterDatas.range;
            newUnit.transform.localScale = new Vector3(Mathf.Abs(newUnit.transform.localScale.x) * mFlip, newUnit.transform.localScale.y, newUnit.transform.localScale.z);
            SetUnitPos();
            ++mCurrentUnitCount;
       }

        private void SetUnitPos()
        {
            for (int i = 0; i < mUnits.Count; ++i)
            {
                switch (i)
                {
                    case 0:
                        mUnits[i].transform.position = transform.position;
                        break;

                    case 1:
                        mUnits[i].transform.position = transform.position + new Vector3(0.5f, 0.5f, 0);
                        break;

                    case 2:
                        mUnits[i].transform.position = transform.position - new Vector3(0.5f, 0.5f, 0);
                        break;
                }
            }
        }

        public void Clear()
        {
            mToken.Cancel();
            mToken.Dispose();
            mCurrentUnitCount = 0;
            for (int i = 0; i < mUnits.Count; ++i)
            {
                mUnits[i].Clear();
            }

            mUnits.Clear();

            gameObject.SetActive(false);
        }
    }
}