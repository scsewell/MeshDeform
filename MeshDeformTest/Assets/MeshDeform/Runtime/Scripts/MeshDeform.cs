using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshDeformation
{
    public class MeshDeform : MonoBehaviour
    {
        struct DeformationData
        {
            public Vector3 offset;
        }

        static readonly int k_DeformationDataSize = Marshal.SizeOf<DeformationData>();

        static readonly List<MeshDeform> s_instances = new List<MeshDeform>();

        /// <summary>
        /// The enabled <see cref="MeshDeform"/> components.
        /// </summary>
        internal static IEnumerable<MeshDeform> instances => s_instances;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            s_instances.Clear();
        }

        Renderer m_Renderer;
        SkinnedMeshRenderer m_SkinnedRenderer;
        MeshFilter m_MeshFilter;
        Material m_Material;
        ComputeBuffer m_DeformationData;

        void OnEnable()
        {
            m_Renderer = GetComponent<Renderer>();
            m_SkinnedRenderer = GetComponent<SkinnedMeshRenderer>();
            m_MeshFilter = GetComponent<MeshFilter>();

            s_instances.Add(this);
        }

        void OnDisable()
        {
            s_instances.Remove(this);

            Dispose(ref m_DeformationData);
        }

        void LateUpdate()
        {
            var mesh = GetMesh();
            var bindingsDirty = false;

            // ensure the per vertex deformation data buffer is large enough for the current mesh
            if (m_DeformationData == null || m_DeformationData.count < mesh.vertexCount)
            {
                Dispose(ref m_DeformationData);

                m_DeformationData = new ComputeBuffer(mesh.vertexCount, k_DeformationDataSize, ComputeBufferType.Structured)
                {
                    name = $"DeformationData ({name})",
                };

                ClearDeformation();

                bindingsDirty = true;
            }

            if (m_Material != m_Renderer.sharedMaterial)
            {
                bindingsDirty = true;
            }
                m_Material = m_Renderer.material;

            // assign the deformation data buffer to the material
            if (bindingsDirty)
            {
            }
            m_Material.SetBuffer(Properties._DeformationData, m_DeformationData);
        }

        /// <summary>
        /// Reinitializes the deformation data.
        /// </summary>
        public void ClearDeformation()
        {
            var mesh = GetMesh();
            var data = new NativeArray<DeformationData>(mesh.vertexCount, Allocator.Temp);
            m_DeformationData.SetData(data, 0, 0, data.Length);
            data.Dispose();
        }

        /// <summary>
        /// Draws the mesh deform pass.
        /// </summary>
        /// <param name="cmdBuffer">The command buffer to draw using.</param>
        internal void DoDeformPass(CommandBuffer cmdBuffer)
        {
            // bind the deformation write target
            cmdBuffer.SetRandomWriteTarget(Constants.DeformUavIndex, m_DeformationData);

            // only draw the first pass and write to a garbage render target, we don't actually want to do anything but output to the UAV buffer
            cmdBuffer.GetTemporaryRT(Properties._DeformTarget, 1, 1, 24);
            cmdBuffer.SetRenderTarget(Properties._DeformTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.DontCare);
            cmdBuffer.SetGlobalInt(Properties._IsDeformPass, 1);

            cmdBuffer.DrawRenderer(m_Renderer, m_Material, 0, 0);

            cmdBuffer.ReleaseTemporaryRT(Properties._DeformTarget);
            cmdBuffer.SetGlobalInt(Properties._IsDeformPass, 0);

            // clear the deformation write target
            cmdBuffer.ClearRandomWriteTargets();
        }

        Mesh GetMesh()
        {
            return m_MeshFilter != null ? m_MeshFilter.sharedMesh : m_SkinnedRenderer.sharedMesh;
        }

        static void Dispose(ref ComputeBuffer buffer)
        {
            if (buffer != null)
            {
                buffer.Release();
                buffer = null;
            }
        }
    }
}

