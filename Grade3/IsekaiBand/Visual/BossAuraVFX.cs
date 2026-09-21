using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    [DisallowMultipleComponent]
    public sealed class BossAuraVFX : MonoBehaviour
    {
        private const string AuraObjectName = "BossAuraParticles";

        [SerializeField] private Vector3 localPosition = new Vector3(0f, -0.65f, 0f);
        [SerializeField] private Vector3 emitterSize = new Vector3(1.8f, 0.25f, 0.1f);
        [SerializeField, Min(0f)] private float emissionRate = 20f;
        [SerializeField] private Color baseColor = new Color(0.42f, 0.05f, 0.72f, 1f);
        [SerializeField] private Color tipColor = new Color(1f, 0.24f, 0.08f, 1f);

        private ParticleSystem _particles;
        private readonly BossAuraMaterial _material = new();

        private void Awake()
        {
            EnsureAura();
        }

        private void OnEnable()
        {
            EnsureAura();
            if (_particles == null)
                return;

            _particles.Clear(true);
            _particles.Play(true);
        }

        private void OnDisable()
        {
            if (_particles != null)
                _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void EnsureAura()
        {
            if (_particles == null)
            {
                Transform existing = transform.Find(AuraObjectName);
                if (existing != null)
                    _particles = existing.GetComponent<ParticleSystem>();
            }

            if (_particles == null)
            {
                var auraObject = new GameObject(AuraObjectName);
                auraObject.SetActive(false);
                auraObject.transform.SetParent(transform, false);
                _particles = auraObject.AddComponent<ParticleSystem>();
                BossAuraParticles.Configure(_particles, emissionRate, emitterSize, baseColor, tipColor,
                    _material.GetOrCreateMaterial);
                auraObject.SetActive(true);
            }
            else
            {
                BossAuraParticles.Configure(_particles, emissionRate, emitterSize, baseColor, tipColor,
                    _material.GetOrCreateMaterial);
            }

            _particles.transform.localPosition = localPosition;
        }

        private void OnDestroy() => _material.Dispose();
    }
}
