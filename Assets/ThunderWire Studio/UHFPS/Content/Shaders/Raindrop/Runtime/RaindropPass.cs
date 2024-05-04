using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

namespace UHFPS.Rendering
{
    public class RaindropPass : ScriptableRenderPass
    {
        private static readonly int Raining = Shader.PropertyToID("_Raining");
        private static readonly int DropletsMask = Shader.PropertyToID("_DropletsMask");
        private static readonly int Tiling = Shader.PropertyToID("_Tiling");
        private static readonly int Distortion = Shader.PropertyToID("_Distortion");
        private static readonly int GlobalRotation = Shader.PropertyToID("_GlobalRotation");
        private static readonly int DropletsGravity = Shader.PropertyToID("_DropletsGravity");
        private static readonly int DropletsSpeed = Shader.PropertyToID("_DropletsSpeed");
        private static readonly int DropletsStrength = Shader.PropertyToID("_DropletsStrength");

        private readonly Material m_Material;
        private RTHandle m_CameraTempColor;

        public RaindropPass(RenderPassEvent renderPassEvent, Material material)
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
            Raindrop raindropVolume = stack.GetComponent<Raindrop>();
            if (!raindropVolume.IsActive()) return;

            Vector2 tiling = raindropVolume.Tiling.value;
            float tilingScale = raindropVolume.TilingScale.value;
            tiling *= tilingScale;

            m_Material.SetFloat(Raining, raindropVolume.Raining.value);
            m_Material.SetTexture(DropletsMask, raindropVolume.DropletsMask.value);
            m_Material.SetVector(Tiling, tiling);
            m_Material.SetFloat(Distortion, raindropVolume.Distortion.value);
            m_Material.SetFloat(GlobalRotation, raindropVolume.GlobalRotation.value);
            m_Material.SetFloat(DropletsGravity, raindropVolume.DropletsGravity.value);
            m_Material.SetFloat(DropletsSpeed, raindropVolume.DropletsSpeed.value);
            m_Material.SetFloat(DropletsStrength, raindropVolume.DropletsStrength.value);

            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("RaindropFX")))
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