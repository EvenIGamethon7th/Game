using _2_Scripts.Game.ScriptableObject.Skill;
using Sirenix.OdinInspector;
using _2_Scripts.Game.Unit.Data;
using _2_Scripts.Utils;
using UnityEngine;
using Cargold.FrameWork.BackEnd;

// 카라리 테이블 임포터에 의해 생성된 스크립트입니다.

public partial class CharacterData : IPoolable
{
    [LabelText("아카데미 졸업 여부")] public bool isAlumni;
    [LabelText("졸업 공격력")] public float alumniAtk;
    [LabelText("졸업 공격속도")] public float alumniAtkSpeed;
    [LabelText("졸업 마법공격력")] public float alumniMatk;

    public BuffData Buff { get; private set; }

    public bool IsActive { get => mIsActive; set => mIsActive = value; }
    private bool mIsActive;

    private string skill1Key;
    private string skill2Key;

    public string SkillName { get; private set;}
    public string SkillType { get; private set; }
    public string SkillDesc { get; private set; }

    private EEnchantClassType mEnchant;

    public EEnchantClassType GetCharacterClass()
    {
        return Utils.GetEnumFromDescription<EEnchantClassType>(ClassType);
    }
    
    public CharacterData()
    {

    }
    
    public void Init(CharacterData data, BuffData buff)
    {
        mIsActive = true;
        Buff = buff;
        atk = data.atk;
        matk = data.matk;
        atkSpeed = data.atkSpeed;
        rank = data.rank;
        characterPack = data.characterPack;
        range = data.range;
        nameKey = data.nameKey;
        Key = data.Key;
        isAlumni = data.isAlumni;
        alumniAtk = data.alumniAtk;
        alumniAtkSpeed = data.alumniAtkSpeed;
        alumniMatk = data.alumniMatk;
        ClassType = data.ClassType;
        Skill1 = data.Skill1;
        Skill2 = data.Skill2;
        skill1Key = data.Skill1;
        skill2Key = data.Skill2;
        characterData = data.characterData;
        cost = data.cost;
        academyClass = data.academyClass;
        Icon = data.Icon;
        illustration = data.illustration;
        if (!string.IsNullOrEmpty(ClassType))
            mEnchant = Utils.GetEnumFromDescription<EEnchantClassType>(ClassType);
    }

    public Sprite GetSkillIconOrNull(string skillKey)
    {
        if (skillKey != "-1")
        {
           return DataBase_Manager.Instance.GetSkill.GetData_Func(skillKey).icon;
        }

        return null;
    }
    public float GetTotalAtk()
    {
        float enchant = 1;
        if (EEnchantClassType.Mage != mEnchant)
            enchant = BackEndManager.Instance.GetEnchantData(mEnchant).GetEnchantStat();
        return Buff.ATKRate * 0.01f * (Buff.ATK + alumniAtk + atk * enchant);
    }

    public float GetTotalAtkSpeed()
    {
        return Buff.ATKSpeedRate * 0.01f * (Buff.ATKSpeed + alumniAtkSpeed + atkSpeed);
    }

    public float GetTotalMAtk()
    {
        float enchant = 1;
        EEnchantClassType type = Utils.GetEnumFromDescription<EEnchantClassType>(ClassType);
        if (EEnchantClassType.Mage == mEnchant)
            enchant = BackEndManager.Instance.GetEnchantData(mEnchant).GetEnchantStat();
        return Buff.MATKRate * 0.01f * (Buff.MATK + alumniMatk + matk * enchant);
    }

    public float GetTotalDamageToType(Define.EAttackType type)
    {
        if (type == Define.EAttackType.Physical)
            return GetTotalAtk();
        else
            return GetTotalMAtk();
    }

    public Sprite GetCharacterSprite()
    {
        return Icon;
    }
    public string GetCharacterName()
    {
        return LocalizeSystem_Manager.Instance.GetLcz_Func(nameKey);
    }

    public string GetCharacterClassName()
    {
        return LocalizeSystem_Manager.Instance.GetLcz_Func(ClassType);
    }

    public string GetSkillDataLoc(int rank)
    {
        string skillKey = rank == 1 ? Skill1 : Skill2; 
        return DataBase_Manager.Instance.GetSkill.GetData_Func(skillKey).Description;
    }
    public void SetSkillDataLoc(int rank)
    {
        string skillKey = rank == 1 ? Skill1 : Skill2; 
        SkillData skillData = DataBase_Manager.Instance.GetSkill.GetData_Func(skillKey);
        SkillName = skillData.Name;
        SkillDesc = skillData.Description;
        SkillType = skillData.SkillType == ESkillType.Active ? "액티브" : "패시브";
    }

    public SkillData GetSkillData(int rank)
    {
        string skillKey = rank == 1 ? Skill1 : Skill2;
        return DataBase_Manager.Instance.GetSkill.GetData_Func(skillKey);
    }

    protected override void Init_Project_Func()
    {
        base.Init_Project_Func();

        /* 런타임 즉시 이 함수가 호출됩니다.
         * 이 스크립트는 덮어쓰이지 않습니다.
         * 임의의 데이터 재가공을 원한다면 이 밑으로 코드를 작성하시면 됩니다.
         */
    }

    public static CharacterData operator +(CharacterData data1, CharacterData data2)
    {
        CharacterData newData = new();
        newData.atk = data1.atk + data2.atk;
        newData.matk = data1.matk + data2.matk;
        newData.atkSpeed = data1.atkSpeed + data2.atkSpeed;
        newData.rank = data1.rank;
        newData.characterPack = data1.characterPack;
        newData.range = data1.range;
        newData.nameKey = data1.nameKey;
        newData.Key = data1.Key;
        newData.isAlumni = data1.isAlumni || data2.isAlumni;
        newData.alumniAtk = data1.alumniAtk + data2.alumniAtk;
        newData.alumniAtkSpeed = data1.alumniAtkSpeed + data2.alumniAtkSpeed;
        newData.alumniMatk = data1.alumniMatk + data2.alumniMatk;
        newData.ClassType = data1.ClassType;
        newData.skill1Key = data1.Skill1;
        newData.skill2Key = data1.Skill2;
        newData.characterData = data1.characterData;

        return newData;
    }

    public void AddAlumniInfo(CharacterData data)
    {
        isAlumni = isAlumni || data.isAlumni;
        alumniAtk += data.alumniAtk;
        alumniAtkSpeed +=  data.alumniAtkSpeed;
        alumniMatk +=  data.alumniMatk;
    }

    public void Clear()
    {
        mIsActive = false;
        atk = 0;
        matk = 0;
        atkSpeed = 0;
        rank = 0;
        characterPack = "";
        range = 0;
        nameKey = "";
        Key = "";
        isAlumni = false;
        alumniAtk = 0;
        alumniAtkSpeed = 0;
        alumniMatk = 0;
        Buff = null;
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