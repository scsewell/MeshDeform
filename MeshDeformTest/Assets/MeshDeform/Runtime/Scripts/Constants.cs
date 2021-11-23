using UnityEngine;

namespace MeshDeformation
{
    static class Constants
    {
        public const int DeformUavIndex = 7;
    }

    static class Properties
    {
        public static readonly int _IsDeformPass = Shader.PropertyToID("_IsDeformPass");
        public static readonly int _DeformationData = Shader.PropertyToID("_DeformationData");
        public static readonly int _DeformTarget = Shader.PropertyToID("_DeformTarget");

        public static readonly int _MousePosition = Shader.PropertyToID("_MousePosition");
        public static readonly int _MousePressure = Shader.PropertyToID("_MousePressure");
        public static readonly int _MouseRadius = Shader.PropertyToID("_MouseRadius");
    }
}
