using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Code.SkillSystem
{
    [CreateAssetMenu(fileName = "SkillUpgradeSO", menuName = "SO/Skill/PerkUpgrade", order = 0)]
    public class SkillPerkUpgradeSO : SkillUpgradeDataSO
    {
        public enum UpgradeType
        {
            Boolean, Integer, Float, Method
        }

        [HideInInspector] public string targetSkill;
        public List<FieldInfo> boolFields = new List<FieldInfo>();
        public List<FieldInfo> floatFields = new List<FieldInfo>();
        public List<FieldInfo> intFields = new List<FieldInfo>();
        
        [HideInInspector] public UpgradeType upgradeType;
        [HideInInspector] public string selectFieldName;
        [HideInInspector] public int intValue;
        [HideInInspector] public float floatValue;

        private Type _skillType;
        private FieldInfo _selectedField;
        
        private void OnEnable()
        {
            GetFieldsFromTargetSkill();
            SetSelectedField();
        }
        
        public void GetFieldsFromTargetSkill()
        {
            if (string.IsNullOrEmpty(targetSkill))
            {
                Debug.LogWarning($"No target skill selected! : {this.name}");
                return;
            }
            
            _skillType = Type.GetType(targetSkill);
            if (_skillType == null)
            {
                Debug.LogWarning($"Target skill not found : {targetSkill}");
                return;
            }
            
            FieldInfo[] fields = _skillType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            boolFields.Clear();
            floatFields.Clear();
            intFields.Clear();
            
            foreach (FieldInfo field in fields)
            {
                if(field.FieldType == typeof(bool))
                    boolFields.Add(field);
                else if(field.FieldType == typeof(float))
                    floatFields.Add(field);
                else if(field.FieldType == typeof(int))
                    intFields.Add(field);
            }
        }
        
        public void SetSelectedField()
        {
            if (_skillType == null) return;
            _selectedField = _skillType.GetField(selectFieldName);
            Debug.Assert(_selectedField != null, $"Selected field is null {selectFieldName}");
        }


        public override void UpgradeSkill(Skill skill)
        {
            switch (upgradeType)
            {
                case UpgradeType.Boolean:
                    _selectedField.SetValue(skill, true);
                    break;
                case UpgradeType.Integer:
                {
                    int oldValue = (int)_selectedField.GetValue(skill);
                    _selectedField.SetValue(skill, oldValue + intValue);
                    break;
                }
                case UpgradeType.Float:
                {
                    float oldValue = (float)_selectedField.GetValue(skill);
                    _selectedField.SetValue(skill, oldValue + floatValue);
                    break;
                }
                case UpgradeType.Method:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void RollbackUpgrade(Skill skill)
        {
            switch (upgradeType)
            {
                case UpgradeType.Boolean:
                    _selectedField.SetValue(skill, false);
                    break;
                case UpgradeType.Integer:
                {
                    int oldValue = (int)_selectedField.GetValue(skill);
                    _selectedField.SetValue(skill, oldValue - intValue);
                    break;
                }
                case UpgradeType.Float:
                {
                    float oldValue = (float)_selectedField.GetValue(skill);
                    _selectedField.SetValue(skill, oldValue - floatValue);
                    break;
                }
                case UpgradeType.Method:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}