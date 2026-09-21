using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace _Work.CHUH.Code.Visual
{
    public sealed class FullScreenPixelizeRendererFeature : ScriptableRendererFeature
    {
        public Material material;
        [Min(1)] public int pixelSize = 4;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;

        private PixelizePass _pass;

        public override void Create()
        {
            _pass = new PixelizePass
            {
                renderPassEvent = renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
            {
                return;
            }

            if (renderingData.cameraData.camera.targetTexture != null)
            {
                return;
            }

            if (material == null)
            {
                Debug.LogWarning($"{nameof(FullScreenPixelizeRendererFeature)} material is null and will be skipped.", this);
                return;
            }

            _pass.renderPassEvent = renderPassEvent;
            _pass.Setup(material, pixelSize);
            renderer.EnqueuePass(_pass);
        }

        private sealed class PixelizePass : ScriptableRenderPass
        {
            private static readonly int PixelSizeId = Shader.PropertyToID("_PixelSize");

            private Material _material;
            private int _pixelSize;

            public void Setup(Material material, int pixelSize)
            {
                _material = material;
                _pixelSize = Mathf.Max(1, pixelSize);
                ConfigureInput(ScriptableRenderPassInput.Color);
                requiresIntermediateTexture = true;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                if (resourceData.isActiveTargetBackBuffer)
                {
                    Debug.LogWarning("Skipping full screen pixelize pass because the active color target is the back buffer.");
                    return;
                }

                TextureHandle source = resourceData.activeColorTexture;
                if (!source.IsValid())
                    return;

                TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = "CameraColor-FullScreenPixelize";
                destinationDesc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                _material.SetFloat(PixelSizeId, _pixelSize);

                RenderGraphUtils.BlitMaterialParameters parameters = new(source, destination, _material, 0);
                renderGraph.AddBlitPass(parameters, "Full Screen Pixelize");

                renderGraph.AddCopyPass(destination, source, "Copy Full Screen Pixelize To Camera Color");
            }
        }
    }
}
