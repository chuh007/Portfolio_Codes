using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace _Work.CHUH.Code.Visual
{
    internal static class BossAuraParticles
    {
        private const string EntitySortingLayer = "Entity";

        public static void Configure(ParticleSystem particles, float emissionRate, Vector3 emitterSize,
            Color baseColor, Color tipColor, Func<Material> getMaterial)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.duration = 2f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1.35f, 2.15f);
            main.startSpeed = 0f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.24f, 0.62f);
            main.startRotation = new ParticleSystem.MinMaxCurve(-0.35f, 0.35f);
            main.startColor = Color.white;
            main.maxParticles = 72;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            emission.rateOverTime = Mathf.Max(0f, emissionRate);

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = emitterSize;

            ParticleSystem.VelocityOverLifetimeModule velocity = particles.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = 0f;
            velocity.y = 1.75f;
            velocity.z = 0f;

            ParticleSystem.NoiseModule noise = particles.noise;
            noise.enabled = true;
            noise.quality = ParticleSystemNoiseQuality.Medium;
            noise.strength = new ParticleSystem.MinMaxCurve(0.12f, 0.32f);
            noise.frequency = 0.55f;
            noise.scrollSpeed = 0.25f;
            noise.damping = true;

            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(CreateAuraGradient(baseColor, tipColor));

            ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = particles.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0f, 0.25f),
                new Keyframe(0.28f, 1f),
                new Keyframe(0.78f, 0.72f),
                new Keyframe(1f, 0f)));

            ParticleSystemRenderer particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
            particleRenderer.renderMode = ParticleSystemRenderMode.Billboard;
            particleRenderer.alignment = ParticleSystemRenderSpace.View;
            particleRenderer.sortingLayerName = EntitySortingLayer;
            particleRenderer.sortingOrder = -2;
            particleRenderer.minParticleSize = 0.01f;
            particleRenderer.maxParticleSize = 0.3f;
            particleRenderer.sharedMaterial = getMaterial();
        }

        private static Gradient CreateAuraGradient(Color baseColor, Color tipColor)
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(baseColor, 0f),
                    new GradientColorKey(Color.Lerp(baseColor, tipColor, 0.45f), 0.55f),
                    new GradientColorKey(tipColor, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(0.72f, 0.18f),
                    new GradientAlphaKey(0.4f, 0.72f),
                    new GradientAlphaKey(0f, 1f)
                });
            return gradient;
        }
    }
}
