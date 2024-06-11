using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Cargold;

namespace Cargold.FrameWork
{
    public partial class DataBase_Manager // C - Lcz
    {
        public abstract IDB_Localize GetLocalize_C { get; } 
    }

    public interface IDB_Localize : DB.TableImporter.ILibraryDataGroup<ILczData>
    {

    }

    public interface ILczData
    {
        string GetLczStr_Func(SystemLanguage _langType);
    }
}