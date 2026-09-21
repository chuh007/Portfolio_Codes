using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.EntityPlus.Effect.Editor
{
    public class EffectDataSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            visualTreeAsset.CloneTree(root);
            
            TextField nameField = root.Q<TextField>("EffectNameField");
            nameField.RegisterValueChangedCallback(HandleAssetNameChange);

            return root;
        }
        
        private void HandleAssetNameChange(ChangeEvent<string> evt)
        {
            
        }
    }
}