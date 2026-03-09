using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace _01Scripts.FSM.Editor
{
    [UnityEditor.CustomEditor(typeof(StateDataSO))]
    public class StateDataEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset uiAsset = default;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            uiAsset.CloneTree(root);

            DropdownField classField = root.Q<DropdownField>("ClassDropDownField");

            classField.choices.Clear();
            Assembly fsmAssembly = Assembly.GetAssembly(typeof(EntityState));

            List<Type> stateTypes = fsmAssembly.GetTypes()
                .Where(type => type.IsAbstract == false
                                && type.IsSubclassOf(typeof(EntityState)))
                .ToList();

            stateTypes.ForEach(type => classField.choices.Add(type.FullName));

            return root;
        }
    }
}
#endif
