using Cargold.DB.TableImporter;
using Cargold.FrameWork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cargold.Example
{
    public class DB_로컬라이즈
    {
        public abstract class DBM : Cargold.FrameWork.DataBase_Manager
        {
            public IDB_Localize 재정의;

            /**/
            public override Cargold.FrameWork.IDB_Localize GetLocalize_C => this.재정의;
            /**/
        }

        public class DB_이름 : DB.TableImporter.DataGroup_C<이름Data>/**/, Cargold.FrameWork.IDB_Localize/**/
        {
            /**/
            bool ILibraryDataGroup<Cargold.FrameWork.ILczData>.TryGetData_Func(string _key, out Cargold.FrameWork.ILczData _lczData)
            {
                if (base.dataDic != null && base.dataDic.ContainsKey(_key) == true)
                {
                    _lczData = base.dataDic[_key];
                    return true;
                }
                else
                {
                    _lczData = null;
                    return false;
                }
            }
            /**/
        }

        public class 이름Data : Cargold.DB.TableImporter.Data_C/**/, Cargold.FrameWork.ILczData/**/
        {
            /**/
            string Cargold.FrameWork.ILczData.GetLczStr_Func(SystemLanguage _langType)
            {
                switch (_langType)
                {
                    default:
                    /*0*/

                    /*1*/
                    /*2*/
                    /*3*/
                    /*4*/
                    /*5*/
                    /*6*/
                    /*7*/
                    /*8*/
                    /*9*/
                    /*10*/
                    /*11*/
                    /*12*/
                    /*13*/
                    /*14*/
                    /*15*/
                    /*16*/
                    /*17*/
                    /*18*/
                    /*19*/
                    /*20*/
                    /*21*/
                    /*22*/
                    /*23*/
                    /*24*/
                    /*25*/
                    /*26*/
                    /*27*/
                    /*28*/
                    /*29*/
                    /*30*/

                    case SystemLanguage.Unknown:
                        Debug_C.Error_Func("다음 언어는 지원하지 않습니다. : " + _langType);
                        return default;
                }
            }
            /**/

#if UNITY_EDITOR
            public override void CallEdit_OnDataImport_Func(string[] _cellDataArr)
            {
                throw new System.NotImplementedException();
            } 
#endif
        }
    }
}
