#pragma use_dxc

struct DeformationData
{
    float3 offset;
};

int _IsDeformPass;
StructuredBuffer<DeformationData> _DeformationData;
RWStructuredBuffer<DeformationData> _DeformationDataWritable : register(u7);

void GetDeformationData_float(in float vertexId, out float3 offset)
{
    DeformationData data;

    if (_IsDeformPass)
    {
        data = _DeformationDataWritable[vertexId];
    }
    else
    {
        data = _DeformationData[vertexId];
    }

    offset = data.offset;
}

void SetDeformationData_float(in float vertexId, in float3 offsetIn, out float3 offsetOut)
{
    DeformationData data;

    data.offset = offsetIn;

    if (_IsDeformPass)
    {
        _DeformationDataWritable[vertexId] = data;
    }

    // we need pass something out of the function for it to be included in the graph
    offsetOut = data.offset;
}

void WorldPosToScreenPos_float(in float3 worldPos, out float2 screenPos)
{
    float4 screen = ComputeScreenPos(TransformWorldToHClip(worldPos), _ProjectionParams.x);
    screenPos = _ScreenParams.xy * (screen.xy / screen.w);
}
