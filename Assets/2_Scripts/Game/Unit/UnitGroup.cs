using _2_Scripts.Game.Map.Tile;
using _2_Scripts.UI;
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
        public bool IsSelect { get; set; } = false;

        private CancellationTokenSource mToken = new CancellationTokenSource();

        private readonly int mMaxUnitCount = 3;

        public float GroupRange { get; private set; }
        
        public List<CUnit> Units { get; private set; }

        private EUnitStates mCurrentState;
        private Vector3 mDstPos;

        [SerializeField]
        private float mSpeed = 10f;
        

        private void Awake()
        {
            Units = new List<CUnit>(mMaxUnitCount);
        }

        public bool CanFusion()
        {
            return Units.Count == mMaxUnitCount && Units[0]?.CharacterDatas.rank != (int)EUnitRank.Unique;
        }

        //문제의 코드 어떻게 해야 하는가?
        public void Fusion()
        {
            CharacterData alumniData = Units[0].CharacterDatas + Units[1].CharacterDatas + Units[2].CharacterDatas;
            var newData = MemoryPoolManager<CharacterData>.CreatePoolingObject();
            newData.Init(Units[0].CharacterDataInfo.CharacterEvolutions[Units[0].CharacterDatas.rank + 1].GetData, null);
            newData.AddAlumniInfo(alumniData);

            var masterSlot = MapManager.Instance.GetTileSlotAboutGroup(this);
            MapManager.Instance.ClearTile(masterSlot);
            MapManager.Instance.CreateUnit(newData, assignSlot: masterSlot);
        }

        public void MoveGroup(TileSlot destinationTileSlot)
        {
            mToken.Cancel();
            mToken.Dispose();
            mToken = new();
            var pos = new Vector3(destinationTileSlot.transform.position.x, destinationTileSlot.transform.position.y, destinationTileSlot.TileZ);
            MoveGroupAsync(pos).Forget();
        }

        public CharacterData GetCharacterData()
        {
            return Units.OrderBy(x => x.CharacterDatas.isAlumni).LastOrDefault().CharacterDatas;
        }

        public bool CanAddUnit()
        {
            return Units.Count < mMaxUnitCount;
        }

        public void AddUnit(CUnit newUnit)
        {
            if (Units.Count >= mMaxUnitCount) return;

            newUnit.transform.parent = transform;
            newUnit.UpdateState(mCurrentState);

            Units.Add(newUnit);
            GroupRange = newUnit.CharacterDatas.range;
            if (mCurrentState == EUnitStates.Move)
                newUnit.SetFlipUnit(mDstPos);
            SetUnitPos();
        }

        public void RemoveUnit(CUnit unit)
        {
            Units.Remove(unit);
            SetUnitPos();
            //가지고 있는 유닛이 없으므로
            if (Units.Count == 0) MapManager.Instance.ClearTile(MapManager.Instance.GetTileSlotAboutGroup(this));
            //타일 중 같은 유닛이 있는지 확인해야 한다
            else if (Units.Count == 2)
            {
                var otherGroup = MapManager.Instance.CheckOccupantSameUnit(this);
                if (otherGroup == null) return;
                MoveUnitOtherGroup(otherGroup);
            }
        }

        public void Clear(bool isDestroy = false)
        {
            mToken.Cancel();
            mToken.Dispose();
            if (!isDestroy)
                mToken = new();

            for (int i = 0; i < Units.Count; ++i)
            {
                Units[i].Clear();
            }

            Units.Clear();

            gameObject.SetActive(false);
        }

        private async UniTask MoveGroupAsync(Vector3 dstPos)
        {
            Vector3 originPos = transform.position;

            mDstPos = dstPos;
            mCurrentState = EUnitStates.Move;

            for (int i = 0; i < Units.Count; ++i)
            {
                if (Units[i] == null)
                    break;

                Units[i].UpdateState(mCurrentState);
                Units[i].SetFlipUnit(dstPos);
            }

            Vector3 normal = Vector3.Normalize(dstPos - transform.position);
            float length = Vector3.Distance(dstPos, originPos);

            while (length > Vector3.Distance(transform.position, originPos))
            {
                await UniTask.DelayFrame(1, cancellationToken: mToken.Token);
                transform.position += normal * mSpeed * Time.deltaTime;
            }
            transform.position = dstPos;

            mCurrentState = EUnitStates.Idle;
            for (int i = 0; i < Units.Count; ++i)
                Units[i]?.UpdateState(mCurrentState);
        }

        private void MoveUnitOtherGroup(UnitGroup otherGroup)
        {
            var newUnit = otherGroup.Units.Last();
            otherGroup.RemoveUnit(newUnit);
            AddUnit(newUnit);
        }

        private void SetUnitPos()
        {
            for (int i = 0; i < Units.Count; ++i)
            {
                switch (i)
                {
                    case 0:
                        Units[i].transform.position = transform.position;
                        break;

                    case 1:
                        Units[i].transform.position = transform.position + new Vector3(0.5f, 0.5f, 0.0001f);
                        break;

                    case 2:
                        Units[i].transform.position = transform.position - new Vector3(0.5f, 0.5f, 0.0001f);
                        break;
                }
            }
        }
    }
}