using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

public partial class MonsterData : IPoolable
{
    public float MaxHp;

    private HashSet<float> mSlowSet = new HashSet<float>();
    private float mCurrentSlow;

    [field:SerializeField]
    public float hp { get ; private set;}
    [field: SerializeField]
    public float def{ get ; private set;}
    [field: SerializeField]
    public float mdef{ get ; private set;}
    [field: SerializeField]
    public float speed{ get ; private set;}
    [field: SerializeField]
    public float atk { get ; private set;}

    public Dictionary<string, int> rewardList = new Dictionary<string, int>()
    {
        { "Money_1001", 0 },
        { "Money_1002", 0 },
    };
    [SerializeField]
    private bool mIsActive;
    public bool IsActive { get => mIsActive; set => mIsActive = value; }
    public void Clear()
    {
        mIsActive = false;
        hp = 0;
        MaxHp = 0;
        def = 0;
        mdef = 0;
        speed = 0;
        atk = 0;
        rewardList["Money_1001"] = 0;
        rewardList["Money_1002"] = 0;
    }

    public void Init(WaveStatData waveStatData,float weight)
    {
        mSlowSet.Clear();
        mCurrentSlow = 0;
        mIsActive = true;
        hp = waveStatData.hp * weight;
        atk = waveStatData.atk;
        def = waveStatData.def;
        mdef = waveStatData.mdef;
        speed = waveStatData.speed;
        atk = waveStatData.atk;
        rewardList["Money_1001"] = waveStatData.reward_count1;
        rewardList["Money_1002"] = waveStatData.reward_count2;
        MaxHp = hp;
    }
    
    

    public void DamageHp(float damage)
    {
        this.hp -= damage;
    }
    
    public void AddDefenceStat(float def)
    {
        this.def += def;
    }
    
    public void AddMagicDefenceStat(float mdef)
    {
        this.mdef += mdef;
    }
    
    public void SetSpeed(float percent, bool isRemove = false)
    {
        if (mSlowSet.Contains(percent) && !isRemove) return;

        if (mCurrentSlow < percent && !isRemove)
        {
            mSlowSet.Add(percent);
            speed /= (1f - mCurrentSlow * 0.01f);
            mCurrentSlow = percent;
            speed *= (1f - percent * 0.01f);
        }

        else if (isRemove && mSlowSet.Contains(percent))
        {
            mSlowSet.Remove(percent);
            if (mCurrentSlow == percent)
            {
                speed /= (1f - percent * 0.01f);
                float next = mSlowSet.OrderBy(x => x).LastOrDefault();
                speed *= (1f - next * 0.01f);
                mCurrentSlow = next;
            }
        }
    }
    protected override void Init_Project_Func()
    {
        base.Init_Project_Func();
        /* 런타임 즉시 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }

#if UNITY_EDITOR
    public override void CallEdit_OnDataImportDone_Func()
    {
        base.CallEdit_OnDataImportDone_Func();

        /* 테이블 임포트가 모두 마무리된 뒤 마지막으로 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }
#endif


}