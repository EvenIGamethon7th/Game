using Sirenix.OdinInspector;
using UnityEngine;

namespace Cargold.Effect
{
    public partial struct EffectKey
    {
        public const int None = -1;
        private static string typeStr;

        [ValueDropdown("GetIEnumerable"), SerializeField, HideLabel]
        public int ID;

        public string ToString_Func()
        {
            return ToString_Func(this.ID);
        }

        public static string ToString_Func(int _id)
        {
            if (typeStr.IsNullOrWhiteSpace_Func() == true)
                typeStr = typeof(EffectKey).Name;

            return StringBuilder_C.Append_Func(typeStr, _id.ToString());
        }

        public static implicit operator int(EffectKey value)
        {
            return value.ID;
        }
        public static implicit operator EffectKey(int value)
        {
            EffectKey _data = new EffectKey();
            _data.ID = value;

            return _data;
        }
    } 
}