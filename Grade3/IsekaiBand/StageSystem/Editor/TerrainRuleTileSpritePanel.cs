using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class TerrainRuleTileSpritePanel
    {
        private readonly UnityEditor.Editor _editor;
        private Sprite _spriteToApply;

        public TerrainRuleTileSpritePanel(UnityEditor.Editor editor) => _editor = editor;

        public void Draw()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Sprite Actions", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _spriteToApply = (Sprite)EditorGUILayout.ObjectField("Sprite", _spriteToApply, typeof(Sprite), false);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(_spriteToApply == null))
                    {
                        if (GUILayout.Button("Set Default Sprite"))
                            TerrainRuleSpriteCommands.ApplyDefaultSprite(_editor.targets, _spriteToApply);

                        if (GUILayout.Button("Add Rule From Sprite"))
                            TerrainRuleSpriteCommands.AddSpriteRules(_editor.targets, new[] { _spriteToApply });
                    }
                }

                Sprite[] selectedSprites = TerrainRuleSpriteCommands.GetSpritesFromObjects(Selection.objects);
                using (new EditorGUI.DisabledScope(selectedSprites.Length == 0))
                {
                    string buttonLabel = selectedSprites.Length == 0
                        ? "Add Selected Sprites As Rules"
                        : $"Add Selected Sprites As Rules ({selectedSprites.Length})";
                    if (GUILayout.Button(buttonLabel))
                        TerrainRuleSpriteCommands.AddSpriteRules(_editor.targets, selectedSprites);
                }

                DrawSpriteDropArea();
            }
        }

        private void DrawSpriteDropArea()
        {
            Rect dropRect = GUILayoutUtility.GetRect(0f, 46f, GUILayout.ExpandWidth(true));
            GUI.Box(dropRect, "Drop sprites or sliced textures here to add blank rules", EditorStyles.helpBox);

            Event current = Event.current;
            if (!dropRect.Contains(current.mousePosition))
                return;

            if (current.type != EventType.DragUpdated && current.type != EventType.DragPerform)
                return;

            Sprite[] sprites = TerrainRuleSpriteCommands.GetSpritesFromObjects(DragAndDrop.objectReferences);
            DragAndDrop.visualMode = sprites.Length > 0 ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            if (current.type == EventType.DragPerform && sprites.Length > 0)
            {
                DragAndDrop.AcceptDrag();
                TerrainRuleSpriteCommands.AddSpriteRules(_editor.targets, sprites);
            }

            current.Use();
        }
    }
}
