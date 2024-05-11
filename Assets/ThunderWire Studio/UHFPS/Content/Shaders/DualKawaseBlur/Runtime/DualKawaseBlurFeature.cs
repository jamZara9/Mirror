using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class DualKawaseBlurFeature : ScriptableRendererFeature
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material BlurMaterial;

        private DualKawaseBlurPass pass;

        public override void Create()
        {
            pass = new DualKawaseBlurPass(RenderPassEvent, BlurMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (pass != null) renderer.EnqueuePass(pass);
        }
    }
}