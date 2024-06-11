using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold
{
    public static class RichText_C
    {
        private const string BoldStart = "<b>";
        private const string BoldEnd = "</b>";
        private const string SizeStart = "<size=";
        private const string SizeEnd = "</size>";
        private const string ColorStart = "<color=#";
        private const string ColorEnd = "</color>";
        

        private const string StartCut = ">";

        public static string SetBold_Func(string _str)
        {
            return StringBuilder_C.Append_Func(BoldStart, _str, BoldEnd);
        }
        public static string SetSize_Func(string _str, int _size)
        {
            string _sizeStr = _size.ToString();
            return StringBuilder_C.Append_Func(SizeStart, _sizeStr, StartCut, _str, SizeEnd);
        }
        public static string SetColor_Func(string _str, Color _color)
        {
            string _colorStr = ColorUtility.ToHtmlStringRGBA(_color);
            return StringBuilder_C.Append_Func(ColorStart, _colorStr, StartCut, _str, ColorEnd);
        }
    }
}