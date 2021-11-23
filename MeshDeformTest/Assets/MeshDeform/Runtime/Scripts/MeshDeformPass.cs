using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace MeshDeformation
{
    class MeshDeformPass : CustomPass
    {
        CommandBuffer m_ComputeBuffer;

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            m_ComputeBuffer = new CommandBuffer
            {
                name = "MeshDeform",
            };
        }

        protected override void Execute(CustomPassContext ctx)
        {
            foreach (var deformer in MeshDeform.instances)
            {
                deformer.DoDeformPass(m_ComputeBuffer);
            }

            ctx.renderContext.ExecuteCommandBuffer(m_ComputeBuffer);
            m_ComputeBuffer.Clear();
        }

        protected override void Cleanup()
        {
            if (m_ComputeBuffer != null)
            {
                m_ComputeBuffer.Release();
                m_ComputeBuffer = null;
            }
        }
    }
}
