using System;
using _Code.LCH._02.Scripts.Player.WeaponStyle;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.UI
{
    internal sealed class CraftingInstrumentDrag
    {
        private readonly VisualElement _root;
        private readonly VisualElement _ghost;
        private readonly VisualElement _target;
        private readonly CraftingBuildContext _context;
        private readonly Action<WeaponType> _select;
        private readonly Action<WeaponType> _place;
        private VisualElement _source;
        private WeaponType _type;
        private Vector2 _start;
        private int _pointerId;
        private bool _dragging;

        public CraftingInstrumentDrag(VisualElement root, CraftingBuildContext context,
            Action<WeaponType> select, Action<WeaponType> place)
        {
            _root = root;
            _ghost = root.Q("drag-ghost");
            _target = root.Q("craft-panel");
            _context = context;
            _select = select;
            _place = place;
        }

        public void Bind(VisualElement item, WeaponType type)
        {
            item.focusable = true;
            item.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;
                Cancel();
                _source = item;
                _type = type;
                _pointerId = evt.pointerId;
                _start = evt.position;
                item.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            });
            item.RegisterCallback<PointerMoveEvent>(HandleMove);
            item.RegisterCallback<PointerUpEvent>(HandleUp);
            item.RegisterCallback<PointerCancelEvent>(_ => Cancel());
            item.RegisterCallback<PointerCaptureOutEvent>(_ => Cancel());
            item.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.Space) return;
                _select(type);
                evt.StopPropagation();
            });
        }

        private void HandleMove(PointerMoveEvent evt)
        {
            if (_source == null || evt.pointerId != _pointerId || !_context.CanPlace(_type)) return;
            if (!_dragging && Vector2.Distance(_start, evt.position) < 8f) return;
            _dragging = true;
            _context.ApplyIcon(_ghost, _type);
            _ghost.style.display = DisplayStyle.Flex;
            Vector2 local = _root.WorldToLocal(evt.position);
            _ghost.style.left = local.x - 36f;
            _ghost.style.top = local.y - 36f;
            _target.EnableInClassList("craft-panel--drag-over", _target.worldBound.Contains(evt.position));
            evt.StopPropagation();
        }

        private void HandleUp(PointerUpEvent evt)
        {
            if (_source == null || evt.pointerId != _pointerId) return;
            WeaponType type = _type;
            bool place = _dragging && _target.worldBound.Contains(evt.position);
            bool select = !_dragging && Vector2.Distance(_start, evt.position) < 8f;
            Cancel();
            if (place) _place(type);
            else if (select) _select(type);
            evt.StopPropagation();
        }

        public void Cancel()
        {
            VisualElement source = _source;
            _source = null;
            _dragging = false;
            _ghost.style.display = DisplayStyle.None;
            _target.RemoveFromClassList("craft-panel--drag-over");
            if (source != null && source.HasPointerCapture(_pointerId)) source.ReleasePointer(_pointerId);
        }
    }
}
