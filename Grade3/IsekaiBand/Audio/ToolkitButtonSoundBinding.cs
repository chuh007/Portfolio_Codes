using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ToolkitButton = UnityEngine.UIElements.Button;

namespace _Work.CHUH.Code.Audio
{
    internal sealed class ToolkitButtonSoundBinding
    {
        private readonly Dictionary<UIDocument, VisualElement> _boundRoots = new();
        private readonly HashSet<UIDocument> _foundDocuments = new();
        private readonly List<UIDocument> _staleDocuments = new();
        private readonly Action _playClick;

        public ToolkitButtonSoundBinding(Action playClick) => _playClick = playClick;

        public void Refresh()
        {
            _foundDocuments.Clear();
            UIDocument[] documents = UnityEngine.Object.FindObjectsByType<UIDocument>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (UIDocument document in documents)
            {
                VisualElement root = document.rootVisualElement;
                _foundDocuments.Add(document);
                if (_boundRoots.TryGetValue(document, out VisualElement boundRoot))
                {
                    if (boundRoot == root) continue;
                    boundRoot?.UnregisterCallback<ClickEvent>(HandleClick);
                }

                if (root == null)
                {
                    _boundRoots.Remove(document);
                    continue;
                }
                root.RegisterCallback<ClickEvent>(HandleClick);
                _boundRoots[document] = root;
            }
            RemoveStaleRoots();
        }

        private void RemoveStaleRoots()
        {
            _staleDocuments.Clear();
            foreach (KeyValuePair<UIDocument, VisualElement> pair in _boundRoots)
            {
                if (pair.Key == null || !_foundDocuments.Contains(pair.Key))
                    _staleDocuments.Add(pair.Key);
            }

            foreach (UIDocument document in _staleDocuments)
            {
                if (_boundRoots.TryGetValue(document, out VisualElement root))
                    root?.UnregisterCallback<ClickEvent>(HandleClick);
                _boundRoots.Remove(document);
            }
        }

        private void HandleClick(ClickEvent evt)
        {
            VisualElement current = evt.target as VisualElement;
            while (current != null)
            {
                if (current is ToolkitButton button)
                {
                    if (button.enabledInHierarchy)
                        _playClick();
                    return;
                }
                current = current.parent;
            }
        }

        public void Clear()
        {
            foreach (VisualElement root in _boundRoots.Values)
                root?.UnregisterCallback<ClickEvent>(HandleClick);
            _boundRoots.Clear();
            _foundDocuments.Clear();
            _staleDocuments.Clear();
        }
    }
}
