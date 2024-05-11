using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class FearTentanclesPass : ScriptableRenderPass
    {
        private static readonly int EffectTime = Shader.PropertyToID("_EffectTime");
        private static readonly int EffectFade = Shader.PropertyToID("_EffectFade");
        private static readonly int TentaclesPosition = Shader.PropertyToID("_TentaclesPosition");
        private static readonly int LayerPosition = Shader.PropertyToID("_LayerPosition");
        private static readonly int VignetteStrength = Shader.PropertyToID("_VignetteStrength");
        private static readonly int TentaclesNum = Shader.PropertyToID("_NumOfTentacles");
        private static readonly int TopLayer = Shader.PropertyToID("_ShowLayer");

        private readonly Material m_Material;
        private RTHandle m_CameraTempColor;
        private float effectTime;

        public FearTentanclesPass(RenderPassEvent renderPassEvent, Material material)
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
            FearTentancles fearTent = stack.GetComponent<FearTentancles>();

            if (!fearTent.IsActive())
            {
                effectTime = 0f;
                return;
            }

            m_Material.SetFloat(EffectTime, effectTime);
            m_Material.SetFloat(EffectFade, fearTent.EffectFade.value);
            m_Material.SetFloat(TentaclesPosition, fearTent.TentaclesPosition.value);
            m_Material.SetFloat(LayerPosition, fearTent.LayerPosition.value);
            m_Material.SetFloat(VignetteStrength, fearTent.VignetteStrength.value);
            m_Material.SetFloat(TentaclesNum, fearTent.Tentacles.value);
            m_Material.SetInteger(TopLayer, fearTent.TopLayer.value ? 1 : 0);
            effectTime += Time.deltaTime * fearTent.TentaclesSpeed.value;

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("FearTentacles")))
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