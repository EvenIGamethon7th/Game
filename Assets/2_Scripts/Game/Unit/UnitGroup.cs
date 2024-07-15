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

        public float GroupRange;
        
        public List<CUnit> Units { get; private set; }

        private EUnitStates mCurrentState;
        private int mFlip = 1;

        private int mInt;
        

        private void Awake()
        {
            Units = new List<CUnit>(mMaxUnitCount);
        }

        public bool CanFusion()
        {
            return Units.Count == mMaxUnitCount && Units[0]?.CharacterDatas.rank != (int)EUnitRank.Unique;
        }

        public void Fusion()
        {
            CharacterData alumniData = Units[0].CharacterDatas + Units[1].CharacterDatas + Units[2].CharacterDatas;
            var newData = Units[0].CharacterDataInfo.CharacterEvolutions[Units[0].CharacterDatas.rank + 1].GetData;
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

        public CharacterData GetCharacterData()
        {
            return Units[0]?.CharacterDatas;
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
            newUnit.transform.localScale = new Vector3(Mathf.Abs(newUnit.transform.localScale.x) * mFlip, newUnit.transform.localScale.y, newUnit.transform.localScale.z);
            SetUnitPos();
        }

        public void RemoveUnit(CUnit unit)
        {
            Units.Remove(unit);
            SetUnitPos();
            //가지고 있는 유닛이 없으므로
            if (Units.Count == 0) MapManager.Instance.ClearTile(this);
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

        private async UniTask MoveGroupAsync(Vector3 dstPos, float time = 1f)
        {
            Vector3 originPos = transform.position;

            mFlip = dstPos.x > originPos.x ? 1 : -1;
            mCurrentState = EUnitStates.Move;

            for (int i = 0; i < Units.Count; ++i)
            {
                if (Units[i] == null)
                    break;

                Units[i].UpdateState(mCurrentState);
                Units[i].transform.localScale = new Vector3(Mathf.Abs(Units[i].transform.localScale.x) * mFlip, Units[i].transform.localScale.y, Units[i].transform.localScale.z);
            }

            while (time >= 0)
            {
                await UniTask.DelayFrame(1, cancellationToken: mToken.Token);
                transform.position = Vector3.Lerp(dstPos, originPos, Mathf.Max(time, 0));
                time -= Time.deltaTime;
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
                        Units[i].transform.position = transform.position + new Vector3(0.5f, 0.5f, 0);
                        break;

                    case 2:
                        Units[i].transform.position = transform.position - new Vector3(0.5f, 0.5f, 0);
                        break;
                }
            }
        }

        private void AcademyButton()
        {
            CUnit unit = Units.Where(x => x.CharacterDatas.isAlumni == false).FirstOrDefault();
            if (unit == null)
            {
                UI_Toast_Manager.Instance.Activate_WithContent_Func("이미 전부 졸업했습니다");
                return;
            }

            bool canEnterAcademy = MapManager.Instance.GoAcademy(this, unit);
            if (!canEnterAcademy) return;

            RemoveUnit(unit);
            unit.Clear();
        }

        private void FusionButton()
        {

        }
    }
}