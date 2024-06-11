using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Infinite
{
    public class InfiniteSystemManager
    {
        protected static InfiniteSystemManager instance;
        public static InfiniteSystemManager Instance
        {
            get
            {
                if (instance == null)
                    new InfiniteSystemManager(new DefaultSetting());

                return instance;
            }
        }

        private Infinite intMaxInf;
        private DefaultSetting defaultSetting;
        
        public int DefaultPoint { get; protected set; }
        public LongData LongDataValue { get; protected set; }

        private InfiniteSystemManager(DefaultSetting systemSetting)
        {
            instance = this;

            instance.intMaxInf = new Infinite(int.MaxValue);
            instance.defaultSetting = systemSetting;

            instance.DefaultPoint = systemSetting.GetDefaultPoint();
            instance.LongDataValue = systemSetting.GetLongData();
        }

        public bool IsBiggerThanIntMax(ref Infinite value, int setDigit = 1)
        {
            this.intMaxInf.SetDigit(setDigit + 3);

            return this.intMaxInf < value;
        }

        public string GetDigitUnit(int unitID)
        {
            return this.defaultSetting.GetDigitUnit(unitID);
        }

        public static void Initialize(DefaultSetting systemSetting)
        {
            new InfiniteSystemManager(systemSetting);
        }

        public class DefaultSetting
        {
            protected string[] units;
            
            public DefaultSetting()
            {
                this.units = this.GetDigitUnitArr();
            }

            /// <summary>
            /// 몇번째 소수자리 표기를 기본으로 할 것인가?
            /// </summary>
            public virtual int GetDefaultPoint()
            {
                return 1;
            }

            /// <summary>
            /// 어느 값까지를 Long에서 Full로 출력할 것인가?
            /// </summary>
            public virtual LongData GetLongData()
            {
                LongData _longValue = new LongData();
                _longValue.longDigit = 3;
                _longValue.longValue = 999;

                return _longValue;
            }

            /// <summary>
            /// 축약 단위 명칭
            /// </summary>
            /// <returns>Z를 초과하면 자동으로 AA로 반환함</returns>
            public virtual string[] GetDigitUnitArr()
            {
                return new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            }

            public string GetDigitUnit(int unitID)
            {
                string _unit = string.Empty;

                this.GetDigitUnit(unitID, ref _unit);

                return _unit;
            }
            private void GetDigitUnit(int unitID, ref string unitStr)
            {
                int _quotientValue = unitID / this.units.Length;
                
                if (0 < _quotientValue)
                    this.GetDigitUnit(_quotientValue - 1, ref unitStr);

                int _reminderValue = unitID - (_quotientValue * this.units.Length);
                unitStr += this.units[_reminderValue];
            }
        }

        public struct LongData
        {
            public int longDigit;
            public int longValue;

            public LongData(int longDigit, int longValue)
            {
                this.longDigit = longDigit;
                this.longValue = longValue;
            }
        }
    }
}
