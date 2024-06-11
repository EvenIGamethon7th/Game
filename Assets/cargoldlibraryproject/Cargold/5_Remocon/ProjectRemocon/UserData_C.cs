using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.FrameWork
{
    [System.Serializable]
    public class UserData_C
    {
        public int playtime;
        public bool isBgmOn;
        public bool isSfxOn;
        public int langTypeID;
        public DateTimeTick lastOffTime; // 게임 종료 시간
        public int version;

        public UserData_C()
        {
            this.playtime = 0;
            this.isBgmOn = true;
            this.isSfxOn = true;
            this.langTypeID = (int)SystemLanguage.English;
            this.lastOffTime = default;
            this.version = 0;
        }
    }
}