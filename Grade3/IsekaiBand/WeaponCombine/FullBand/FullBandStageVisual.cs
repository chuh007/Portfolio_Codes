using _Code.LCH._02.Scripts.Core;
using _Code.LCH._02.Scripts.Player.Attack;
using UnityEngine;

namespace _Work.CHUH.Code.WeaponCombine.FullBand
{
    internal sealed class FullBandStageVisual
    {
        private const string BandVisualPrefabPath = "LCH/RuntimePrefabs/FullBandInstruments";
        private readonly FullBandAttack _source;
        private GameObject _bandVisual;
        private BassPrototypeAttack.BassLowZoneLoopVisualRuntime _bassZoneVisual;
        public float Elapsed { get; private set; }
        public Vector3 PianoPosition => _bandVisual != null
            ? _bandVisual.transform.position : _source.Position + Vector3.up * 2.25f;

        public FullBandStageVisual(FullBandAttack source)
        {
            _source = source;
        }

        public void Begin()
        {
            Elapsed = 0f;
            Create();
        }

        public void Dispose()
        {
            if (_bandVisual != null) Object.Destroy(_bandVisual);
            if (_bassZoneVisual != null) Object.Destroy(_bassZoneVisual.gameObject);
            _bandVisual = null;
            _bassZoneVisual = null;
        }

        private void Create()
        {
            if (_bandVisual == null)
            {
                GameObject prefab = Resources.Load<GameObject>(BandVisualPrefabPath);
                _bandVisual = prefab != null
                    ? Object.Instantiate(prefab, _source.OwnerTransform, false)
                    : new GameObject("FullBandInstruments");
                _bandVisual.name = "FullBandInstruments";
                _bandVisual.transform.SetParent(_source.OwnerTransform, false);
                _bandVisual.transform.localScale = Vector3.one * 0.9f;
                ProjectileRenderLayer.ApplyTo(_bandVisual);
            }

            if (_bassZoneVisual == null)
            {
                var obj = new GameObject("FullBandRhythmSectionZoneLoopFx");
                _bassZoneVisual = obj.AddComponent<
                    BassPrototypeAttack.BassLowZoneLoopVisualRuntime>();
            }
        }

        public void Tick(float deltaTime)
        {
            Elapsed += deltaTime;
            if (_source.OwnerTransform == null)
                return;

            Create();
            if (_bandVisual != null)
            {
                float bob = Mathf.Sin(Elapsed * 3.6f) * 0.1f;
                _bandVisual.transform.localPosition = new Vector3(0f, 2.35f + bob, 0f);
                _bandVisual.transform.localRotation = Quaternion.Euler(
                    0f,
                    0f,
                    Mathf.Sin(Elapsed * 2.2f) * 2.5f);
            }

            _bassZoneVisual?.InitOrUpdate(
                _source.OwnerTransform,
                _source.VenueRange,
                new Color(0.14f, 0.9f, 0.54f, 0.46f),
                46);
        }
    }
}
