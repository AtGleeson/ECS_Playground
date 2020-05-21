using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BasicMovementModifierData : IComponentData
{
    public float2 direction;
    public float speed;
}
