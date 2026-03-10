using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.SkillSystem.Editor
{
    [UnityEditor.CustomEditor(typeof(SkillPerkUpgradeSO))]
    public class CustomSkillUpgradeEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset customTreeAsset;

        private SkillPerkUpgradeSO _skillPerkUpgradeSO; //타겟
        private Assembly _skillAssembly; //스킬 클래스가 속한 어셈블리
        private VisualElement _root;
        private DropdownField _fieldList;
        private IntegerField _intField;
        private FloatField _floatField;

        public override VisualElement CreateInspectorGUI()
        {
            _skillPerkUpgradeSO = target as SkillPerkUpgradeSO;

            _root = new VisualElement();
            //이건 내가 안만들고 기본으로 유니티 인스펙터가 보여주는 내용들을 그려줘.
            InspectorElement.FillDefaultInspector(_root, serializedObject, this);
            customTreeAsset.CloneTree(_root);
            MakeSkillDropDown();
            
            EnumField upgradeTypeSelector = _root.Q<EnumField>("UpgradeType");
            upgradeTypeSelector.RegisterValueChangedCallback(HandleUpgradeTypeChange);

            _intField = _root.Q<IntegerField>("IntegerValue");
            _floatField = _root.Q<FloatField>("FloatValue");
            _fieldList = _root.Q<DropdownField>("FieldList");
            UpdateFieldList();
            
            return _root;
        }


        private void HandleUpgradeTypeChange(ChangeEvent<Enum> evt)
        {
            UpdateFieldList();

            switch (evt.newValue)
            {
                case SkillPerkUpgradeSO.UpgradeType.Boolean:
                    _intField.style.display = DisplayStyle.None;
                    _floatField.style.display = DisplayStyle.None;
                    _fieldList.style.display = DisplayStyle.Flex;
                    break;
                case SkillPerkUpgradeSO.UpgradeType.Integer:
                    _intField.style.display = DisplayStyle.Flex;
                    _floatField.style.display = DisplayStyle.None;
                    _fieldList.style.display = DisplayStyle.Flex;
                    break;
                case SkillPerkUpgradeSO.UpgradeType.Float:
                    _intField.style.display = DisplayStyle.None;
                    _floatField.style.display = DisplayStyle.Flex;
                    _fieldList.style.display = DisplayStyle.Flex;
                    break;
                case SkillPerkUpgradeSO.UpgradeType.Method:
                    _intField.style.display = DisplayStyle.None;
                    _floatField.style.display = DisplayStyle.None;
                    _fieldList.style.display = DisplayStyle.None;
                    //이건 나중에 할께.
                    break;
            }
        }

        private void UpdateFieldList()
        {
            _fieldList.choices = _skillPerkUpgradeSO.upgradeType switch
            {
                SkillPerkUpgradeSO.UpgradeType.Boolean => _skillPerkUpgradeSO.boolFields.Select(field => field.Name).ToList(),
                SkillPerkUpgradeSO.UpgradeType.Integer => _skillPerkUpgradeSO.intFields.Select(field => field.Name).ToList(),
                SkillPerkUpgradeSO.UpgradeType.Float => _skillPerkUpgradeSO.floatFields.Select(field => field.Name).ToList(),
                SkillPerkUpgradeSO.UpgradeType.Method => new List<string>(), //나중에 정정함
                _ => new List<string>()
            };
            
            //스킬의 UpgradeType이 변경되었고, 현재 선택된 필드가 업그레이드 타입변경에 없는 거라면 현재 가져온것중 첫번째 걸로 변경해준다.
            if (_fieldList.choices.Count > 0 &&
                _fieldList.choices.Contains(_skillPerkUpgradeSO.selectFieldName) == false)
            {
                _skillPerkUpgradeSO.selectFieldName = _fieldList.choices[0];
            }else if (_fieldList.choices.Count == 0)
            {
                _skillPerkUpgradeSO.selectFieldName = string.Empty;
            }
        }
        
        private void MakeSkillDropDown()
        {
            DropdownField typeSelector = _root.Q<DropdownField>("TypeSelector");

            _skillAssembly = Assembly.GetAssembly(typeof(Skill)); //스킬이라는 클래스가 정의된 어셈블리를 가져와
            List<Type> derivedTypes = _skillAssembly.GetTypes()
                .Where(type => type.IsClass && type.IsAbstract == false && type.IsSubclassOf(typeof(Skill)))
                .ToList();
            
            derivedTypes.ForEach(type => typeSelector.choices.Add(type.FullName));

            typeSelector.RegisterValueChangedCallback(HandleTypeChange);
            typeSelector.SetValueWithoutNotify(_skillPerkUpgradeSO.targetSkill);
            
            _skillPerkUpgradeSO.targetSkill = typeSelector.value;
            _skillPerkUpgradeSO.GetFieldsFromTargetSkill(); //필드를 선택된 타입에 맞추어 새로 채운다. 
        }

        private void HandleTypeChange(ChangeEvent<string> evt)
        {
            _skillPerkUpgradeSO.targetSkill = evt.newValue;
            _skillPerkUpgradeSO.GetFieldsFromTargetSkill(); //선택한 스킬에 맞춰서 다시 필드 채우고
            UpdateFieldList();
        }
    }
}