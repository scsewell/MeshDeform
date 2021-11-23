using System;
using UnityEngine;

namespace MeshDeformation
{
    public class DeformationController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Controls if the deformation cursor be visible.")]
        bool m_ShowCursor = true;

        [SerializeField]
        [Tooltip("The cursor texture.")]
        Texture2D m_Cursor = null;

        [SerializeField]
        [Tooltip("The radius of the brush in pixels.")]
        [Range(0f, 500f)]
        float m_Radius = 50f;

        [SerializeField]
        [Tooltip("How quickly the brush radius changes when using the scroll wheel.")]
        [Range(0.5f, 0.99f)]
        float m_RadiusStepSize = 0.9f;

        void LateUpdate()
        {
            m_Radius *= Mathf.Pow(m_RadiusStepSize, -Input.mouseScrollDelta.y);

            // TODO: tablet pressure support?
            var pressure = (Input.GetMouseButton(0) ? 1f : 0f) + (Input.GetMouseButton(1) ? -1f : 0f);

            Shader.SetGlobalVector(Properties._MousePosition, Input.mousePosition);
            Shader.SetGlobalFloat(Properties._MousePressure, pressure);
            Shader.SetGlobalFloat(Properties._MouseRadius, m_Radius);
        }

        void OnGUI()
        {
            if (!m_ShowCursor)
            {
                return;
            }

            var mousePos = Input.mousePosition;

            var rect = new Rect
            {
                size = m_Radius * Vector2.one,
                center = new Vector2(mousePos.x, Screen.height - mousePos.y),
            };

            GUI.DrawTexture(rect, m_Cursor);
        }
    }
}
