using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class BloodDisortionPass : ScriptableRenderPass
    {
        private static readonly int BlendColor = Shader.PropertyToID("_BlendColor");
        private static readonly int OverlayColor = Shader.PropertyToID("_OverlayColor");
        private static readonly int BlendTexture = Shader.PropertyToID("_BlendTex");
        private static readonly int BumpTexture = Shader.PropertyToID("_BumpMap");
        private static readonly int BloodAmount = Shader.PropertyToID("_BloodAmount");
        private static readonly int BlendAmount = Shader.PropertyToID("_BlendAmount");
        private static readonly int EdgeSharpness = Shader.PropertyToID("_EdgeSharpness");
        private static readonly int Distortion = Shader.PropertyToID("_Distortion");

        private readonly Material m_Material;
        private RTHandle m_CameraTempColor;

        public BloodDisortionPass(RenderPassEvent renderPassEvent, Material material)
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
            BloodDisortion bloodDisortion = stack.GetComponent<BloodDisortion>();
            if (bloodDisortion == null || !bloodDisortion.IsActive()) return;

            m_Material.SetColor(BlendColor, bloodDisortion.BlendColor.value);
            m_Material.SetColor(OverlayColor, bloodDisortion.OverlayColor.value);

            m_Material.SetTexture(BlendTexture, bloodDisortion.BlendTexture.value);
            m_Material.SetTexture(BlendTexture, bloodDisortion.BlendTexture.value);
            m_Material.SetTexture(BumpTexture, bloodDisortion.BumpTexture.value);
            m_Material.SetFloat(EdgeSharpness, bloodDisortion.EdgeSharpness.value);
            m_Material.SetFloat(Distortion, bloodDisortion.Distortion.value);

            float minBlood = bloodDisortion.MinBloodAmount.value;
            float maxBlood = bloodDisortion.MaxBloodAmount.value;

            float bloodAmount = bloodDisortion.BloodAmount.value;
            m_Material.SetFloat(BloodAmount, bloodAmount);

            float blendAmount = Mathf.Clamp01(bloodAmount * (maxBlood - minBlood) + minBlood);
            m_Material.SetFloat(BlendAmount, blendAmount);

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("BloodDisortion")))
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