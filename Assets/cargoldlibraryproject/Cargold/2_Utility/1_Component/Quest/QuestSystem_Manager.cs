using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Cargold.Observer;

namespace Cargold.QuestSystem
{
    public abstract class QuestSystem<ValueType> : MonoBehaviour
    {
        private Dictionary<string, List<IUesrQuestData>> questDic;
        private IQuestDataManager iQuestDataManager;
        private Action<ValueType> additionResultDel;
        private Observer_Action<bool, ValueType, ValueType> progressObs;

        public void Init_Func()
        {
            this.questDic = new Dictionary<string, List<IUesrQuestData>>();

            this.Deactivate_Func(true);
        }

        public void Activate_Func(IQuestDataManager _iQuestDataManager)
        {
            this.iQuestDataManager = _iQuestDataManager;
        }

        public void SetQuest_Func(string _conditionType, IUesrQuestData _iQuest)
        {
            if(this.questDic.TryGetValue(_conditionType, out List<IUesrQuestData> _list) == false)
            {
                _list = new List<IUesrQuestData>();
                this.questDic.Add(_conditionType, _list);
            }

            _list.Add(_iQuest);
        }

        public void OnProgress_Func(string _conditionType, ValueType _value)
        {
            this.OnProgress_Func(_conditionType, _value, (IUesrQuestData _iAreaQuest, IQuestData _questData) =>
            {
                return true;
            });
        }
        private void OnProgress_Func(string _conditionType, ValueType _value, Func<IUesrQuestData, IQuestData, bool> _del)
        {
            List<IUesrQuestData> _iQuestElemList = null;
            if (this.questDic.TryGetValue(_conditionType, out _iQuestElemList) == true)
            {
                foreach (IUesrQuestData _iQuest in _iQuestElemList)
                {
                    string _questDataKey = _iQuest.GetQuestDataKey;
                    IQuestData _iQuestData = this.iQuestDataManager.GetQuestData_Func(_questDataKey);

                    if (_del(_iQuest, _iQuestData) == true)
                        this.OnProgressResult_Func(_iQuest, _value);
                }
            }
        }
        private void OnProgressResult_Func(IUesrQuestData _iUserQuestData, ValueType _value)
        {
            string _questDataKey = _iUserQuestData.GetQuestDataKey;
            IQuestData _iQuestData = this.iQuestDataManager.GetQuestData_Func(_questDataKey);
            ValueType _goalValue = _iQuestData.GetGoalValue;
            QuestStackType _stackType = _iQuestData.GetQuestStackType;
            string _conditionType = _iQuestData.GetConditionType;
            ValueType _currentValue = default;

            if (_stackType == QuestStackType.Constant)
            {
                _currentValue = this.GetConstantCurrentValue_Func(_conditionType, _iQuestData);
            }
            else if (_stackType == QuestStackType.Moment)
            {
                _currentValue = _iUserQuestData.GetCurrentValue;
            }
            else
            {
                Debug_C.Error_Func("_stackType : " + _stackType);
            }

            ValueType _additionValue = this.GetAddition_Func(_value, _currentValue);
            bool _isGoal = this.IsGoal_Func(_additionValue, _goalValue);
            _iUserQuestData.SetCurrentValue_Func(_additionValue, _isGoal);

            this.progressObs.Notify_Func(_isGoal, _additionValue, _goalValue);
        }
        protected abstract ValueType GetConstantCurrentValue_Func(string _conditionType, IQuestData _iQuestData);
        protected abstract ValueType GetAddition_Func(ValueType _leftValue, ValueType _rightValue);
        protected abstract bool IsGoal_Func(ValueType _currentValue, ValueType _goalValue);

        public void Subscribe_Progress_Func(Action<bool, ValueType, ValueType> _del)
        {
            this.progressObs.Subscribe_Func(_del);
        }
        public void Unsubscribe_Progress_Func(Action<bool, ValueType, ValueType> _del)
        {
            this.progressObs.Unsubscribe_Func(_del);
        }

        public void Deactivate_Func(bool _isInit = false)
        {
            if(_isInit == false)
            {
                this.progressObs.UnsubscribeAll_Func();

                foreach (var item in this.questDic)
                {
                    item.Value.Clear();
                }
            }
        }

        public interface IQuestData
        {
            string GetConditionType { get; }
            ValueType GetGoalValue { get; }
            QuestStackType GetQuestStackType { get; }
        }

        public interface IQuestDataManager
        {
            IQuestData GetQuestData_Func(string _questDataKey);
        }

        public interface IUesrQuestData
        {
            string GetQuestDataKey { get; }
            ValueType GetCurrentValue { get; }

            void SetCurrentValue_Func(ValueType _currentValue, bool _isGoal);
        }
    }

    public enum QuestStackType
    {
        None = 0,

        Constant,
        Moment,
    }
}