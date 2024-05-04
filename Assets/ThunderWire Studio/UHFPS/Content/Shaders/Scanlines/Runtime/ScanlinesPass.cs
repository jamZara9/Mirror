using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class ScanlinesPass : ScriptableRenderPass
    {
        private static readonly int ScanlinesStrength = Shader.PropertyToID("_ScanlinesStrength");
        private static readonly int ScanlinesSharpness = Shader.PropertyToID("_ScanlinesSharpness");
        private static readonly int ScanlinesScroll = Shader.PropertyToID("_ScanlinesScroll");
        private static readonly int ScanlinesFrequency = Shader.PropertyToID("_ScanlinesFrequency");
        private static readonly int GlitchIntensity = Shader.PropertyToID("_GlitchIntensity");
        private static readonly int GlitchFrequency = Shader.PropertyToID("_GlitchFrequency");

        private readonly Material m_Material;
        private RTHandle m_CameraTempColor;

        public ScanlinesPass(RenderPassEvent renderPassEvent, Material material)
        {
            this.renderPassEvent = renderPassEvent;
            m_Material = material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var colorDesc = renderingData.cameraData.cameraTargetDescriptor;
            colorDesc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref m_CameraTempColor, colorDesc, name: "_TemporaryColorTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null) return;
            if (renderingData.cameraData.isSceneViewCamera) return;
            if (!renderingData.cameraData.postProcessEnabled) return;

            VolumeStack stack = VolumeManager.instance.stack;
            Scanlines scanlinesVolume = stack.GetComponent<Scanlines>();
            if (!scanlinesVolume.IsActive()) return;

            m_Material.SetFloat(ScanlinesStrength, scanlinesVolume.ScanlinesStrength.value);
            m_Material.SetFloat(ScanlinesSharpness, scanlinesVolume.ScanlinesSharpness.value);
            m_Material.SetFloat(ScanlinesScroll, scanlinesVolume.ScanlinesScroll.value);
            m_Material.SetFloat(ScanlinesFrequency, scanlinesVolume.ScanlinesFrequency.value);
            m_Material.SetFloat(GlitchIntensity, scanlinesVolume.GlitchIntensity.value);
            m_Material.SetFloat(GlitchFrequency, scanlinesVolume.GlitchFrequency.value);

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Scanlines")))
            {
                RTHandle camTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
                Blitter.BlitCameraTexture(cmd, camTarget, m_CameraTempColor, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, m_CameraTempColor, camTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }
    }
}