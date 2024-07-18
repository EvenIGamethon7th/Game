using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cargold;
using Sirenix.OdinInspector;
using Cargold.DB.TableImporter;
using System.Linq;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

public partial class MonsterData
{
    public float MaxHp;

    private HashSet<float> mSlowSet = new HashSet<float>();
    private float mCurrentSlow;

    public void SetSpeed(float percent, bool isRemove = false)
    {
        if (mSlowSet.Contains(percent) && !isRemove) return;

        mSlowSet.Add(percent);

        if (mCurrentSlow < percent && !isRemove)
        {
            speed /= (1f - mCurrentSlow * 0.01f);
            mCurrentSlow = percent;
            speed *= (1f - percent * 0.01f);
        }

        else if (isRemove)
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