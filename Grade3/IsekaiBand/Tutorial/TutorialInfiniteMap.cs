using _Code.LCH._02.Scripts.Player;
using _Work.CHUH.Code.Core.Events;
using _Work.CHUH.Code.StageSystem;
using Chuh007Lib.Bus;
using UnityEngine;

namespace _Work.CHUH.Code.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class TutorialInfiniteMap : MonoBehaviour
    {
        private Player _player;
        private StageDataSO _mapData;
        private InfiniteMapChunkRenderer _mapRenderer;

        public void Initialize(Player player, SpriteRenderer background, float chunkSize)
        {
            if (_mapRenderer != null) return;

            _player = player;
            _mapData = ScriptableObject.CreateInstance<StageDataSO>();
            _mapData.StageSize = Vector2.one * Mathf.Max(16f, chunkSize);
            _mapData.BackGroundSprite = background != null ? background.sprite : null;
            _mapRenderer = new InfiniteMapChunkRenderer(transform, background, ResolvePlayer, 1);
            _mapRenderer.Setup(_mapData);

            Bus<MapSizeSetEvent>.Raise(new MapSizeSetEvent(_mapData.StageSize.x, _mapData.StageSize.y, true));
        }

        private Transform ResolvePlayer() => _player != null ? _player.transform : null;

        private void Update() => _mapRenderer?.Refresh();

        private void OnDestroy()
        {
            if (_mapData != null)
                Destroy(_mapData);
        }
    }
}
