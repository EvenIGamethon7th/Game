using System.Text;

namespace Cargold
{
    public static class StringBuilder_C
    {
        public const string Percent = "%";

        private static StringBuilder staticBuilder;
        public static StringBuilder AccessCarefully
        {
            get
            {
                if (staticBuilder == null)
                    staticBuilder = new StringBuilder(1024);

                return staticBuilder;
            }
        }

        public static string Append_Func(params string[] _valueArr)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            for (int i = 0; i < _valueArr.Length; i++)
                staticBuilder.Append(_valueArr[i]);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5, string _value6)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);
            staticBuilder.Append(_value6);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5, string _value6, string _value7)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);
            staticBuilder.Append(_value6);
            staticBuilder.Append(_value7);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5, string _value6, string _value7, string _value8)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);
            staticBuilder.Append(_value6);
            staticBuilder.Append(_value7);
            staticBuilder.Append(_value8);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5, string _value6, string _value7, string _value8, string _value9)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);
            staticBuilder.Append(_value6);
            staticBuilder.Append(_value7);
            staticBuilder.Append(_value8);
            staticBuilder.Append(_value9);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, string _value5, string _value6, string _value7, string _value8, string _value9, string _value10)
        {
            if (staticBuilder == null)
                staticBuilder = new StringBuilder(1024);

            staticBuilder.Append(_value1);
            staticBuilder.Append(_value2);
            staticBuilder.Append(_value3);
            staticBuilder.Append(_value4);
            staticBuilder.Append(_value5);
            staticBuilder.Append(_value6);
            staticBuilder.Append(_value7);
            staticBuilder.Append(_value8);
            staticBuilder.Append(_value9);
            staticBuilder.Append(_value10);

            string _resultStr = staticBuilder.ToString();

            staticBuilder.RemoveAll_Func();

            return _resultStr;
        }

        public static string Append_Func(string _value1, int _value2)
        {
            return Append_Func(_value1, _value2.ToString_Func());
        }

        public static string Append_Func(int _value1, int _value2)
        {
            return Append_Func(_value1.ToString_Func(), _value2.ToString_Func());
        }
        public static string Append_Func(int _value1, string _value2, int _value3)
        {
            return Append_Func(_value1.ToString_Func(), _value2, _value3.ToString_Func());
        }

        public static string Append_Func(string _value1, string _value2, string _value3, int _value4, string _value5, string _value6)
        {
            return Append_Func(_value1, _value2, _value3, _value4.ToString_Func(), _value5, _value6);
        }
        public static string Append_Func(string _value1, string _value2, string _value3, string _value4, int _value5, string _value6, bool _isComma = true)
        {
            string _value5Str = _isComma == true ? _value5.ToString_Func() : _value5.ToString();
            return Append_Func(_value1, _value2, _value3, _value4, _value5Str, _value6);
        }

        public static string GetTensionTime_Func(float _remainTime)
        {
            if (10f <= _remainTime)
                return _remainTime.ToString_Func(0);
            else if(1f <= _remainTime)
                return _remainTime.ToString_Func(1);
            else
                return _remainTime.ToString_Func(2);
        }

        public static string GetPath_Func(string _separatorStr, params string[] _strArr)
        {
            return GetPath_Func(_separatorStr, 0, _strArr: _strArr);
        }
        public static string GetPath_Func(string _separatorStr, int _minPath, params string[] _strArr)
        {
            if (_strArr == null || _strArr.Length <= 0)
                return default;

            string _fullStr = _strArr[0];
            int _pathCnt = _minPath < _strArr.Length ? _strArr.Length : _minPath;

            for (int i = 1; i < _pathCnt; i++)
            {
                if(_strArr.TryGetItem_Func(i, out string _str) == true)
                {
                    _fullStr = StringBuilder_C.Append_Func(_fullStr, _separatorStr, _str);
                }
                else
                {
                    _fullStr = StringBuilder_C.Append_Func(_fullStr, _separatorStr);
                }
            }

            return _fullStr;
        }
    } 
}