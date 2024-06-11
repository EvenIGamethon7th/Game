using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Vast
{
    [System.Serializable]
    public class VastSystemManager
    {
        private static PoolingSystem poolingSystem = new PoolingSystem();
        protected static VastSystemManager instance;
        public static VastSystemManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new VastSystemManager(new BaseDefault());

                return instance;
            }
        }
        public static Vast MaxIntVast = null;

        public string[] DigitUnits { get; protected set; }
        public LongData LongDataValue { get; protected set; }
        public int DefaultPoint { get; protected set; }
        public int DefaultDigitOperationAccuracy { get; protected set; }
        private BaseDefault baseVastSystem;

        public int DigitSize { get { return DigitUnits.Length; } }

        public VastSystemManager(BaseDefault baseDefault)
        {
            instance = this;

            instance.baseVastSystem = baseDefault;

            instance.DigitUnits = baseDefault.GetDigitUnit();
            instance.LongDataValue = baseDefault.GetLongData();
            instance.DefaultPoint = baseDefault.GetDefaultPoint();
            if (instance.DefaultPoint < 0 && 3 < instance.DefaultPoint)
            {
                Debug.LogWarning("DefaultPoint의 범위를 초과했습니다.");
                instance.DefaultPoint = 3;
            }
            instance.DefaultDigitOperationAccuracy = baseDefault.GetDigitOperationAccuracy();

            MaxIntVast = new Vast(int.MaxValue);
        }
        public static void Initialize(BaseDefault baseDefault)
        {
            instance = new VastSystemManager(baseDefault);
        }

        public static Vast Spawn()
        {
            return poolingSystem.Spawn();
        }
        public static Vast Spawn(Vast item)
        {
            Vast _vast = poolingSystem.Spawn();
            _vast.SetValue(item);

            return _vast;
        }
        public static void Despawn(Vast item)
        {
            poolingSystem.Despawn(item);
        }
        public static int PoolCount()
        {
            return poolingSystem.Count();
        }

        public class PoolingSystem
        {
            private List<Vast> vastList;
            private static int poolCount = 1000;

            public PoolingSystem()
            {
                vastList = new List<Vast>();
            }
            public Vast Spawn()
            {
                if (0 < vastList.Count)
                {
                    Vast _vast = null;
                    lock (vastList)
                    {
                        _vast = vastList[0];
                        vastList.RemoveAt(0);
                    }
                    
                    VastSystemManager.Instance.baseVastSystem.OnNotifyPoolCount(vastList.Count);

                    _vast.Clear();
                    return _vast;
                }
                else
                    return new Vast();
            }
            public void Despawn(Vast item)
            {
                lock (vastList)
                {
                    vastList.Add(item);
                }

                VastSystemManager.Instance.baseVastSystem.OnNotifyPoolCount(vastList.Count);

                if (poolCount <= vastList.Count)
                {
                    Debug.LogWarning(poolCount + "개 초과");
                    poolCount += 1000;
                }
            }
            public int Count() => vastList.Count;
        }

        public struct LongData
        {
            public int longDigit;
            public int longValue;
        }
        public enum UnitSystem
        {
            Alphabet,
            MetricUnits,
            Dollar,
        }

        public class BaseDefault
        {
            public virtual string[] GetDigitUnit()
            {
                UnitSystem _digitSystem = this.GetDigitSystem();

                switch (_digitSystem)
                {
                    default:
                    case UnitSystem.Alphabet:
                        return new string[]
                        {
                        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                        "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2", "I2", "J2", "K2", "L2", "M2", "N2", "O2", "P2", "Q2", "R2", "S2", "T2", "U2", "V2", "W2", "X2", "Y2", "Z2",
                        };

                    case UnitSystem.MetricUnits:
                        return new string[]
                        {
                        "k", "m", "g", "t", "p", "e", "z", "y"
                        };
                }
            }
            public virtual UnitSystem GetDigitSystem()
            {
                return UnitSystem.Alphabet;
            }
            public virtual LongData GetLongData()
            {
                LongData _longValue = new LongData();
                _longValue.longDigit = 3;
                _longValue.longValue = 999;

                return _longValue;
            }
            public virtual int GetDefaultPoint()
            {
                return 1;
            }
            public virtual void OnNotifyPoolCount(int count) { }

            public virtual int GetDigitOperationAccuracy()
            {
                return 2;
            }
        }
    }
}