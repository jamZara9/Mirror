using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class EyeBlinkPass : ScriptableRenderPass
    {
        private static readonly int Blink = Shader.PropertyToID("_Blink");
        private static readonly int VignetteOuterRing = Shader.PropertyToID("_VignetteOuterRing");
        private static readonly int VignetteInnerRing = Shader.PropertyToID("_VignetteInnerRing");
        private static readonly int VignetteAspectRatio = Shader.PropertyToID("_VignetteAspectRatio");

        private readonly Material m_Material;
        private RTHandle m_CameraTempColor;

        public EyeBlinkPass(RenderPassEvent renderPassEvent, Material material)
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
            EyeBlink eyeBlinkVolume = stack.GetComponent<EyeBlink>();
            if (!eyeBlinkVolume.IsActive()) return;

            m_Material.SetFloat(Blink, eyeBlinkVolume.Blink.value);
            m_Material.SetFloat(VignetteOuterRing, eyeBlinkVolume.VignetteOuterRing.value);
            m_Material.SetFloat(VignetteInnerRing, eyeBlinkVolume.VignetteInnerRing.value);
            m_Material.SetFloat(VignetteAspectRatio, eyeBlinkVolume.VignetteAspectRatio.value);

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("EyeBlink")))
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