using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RotationData : IComponentData
{
    public float3 offset;
    public float3 direction;
    public float speed;
}
