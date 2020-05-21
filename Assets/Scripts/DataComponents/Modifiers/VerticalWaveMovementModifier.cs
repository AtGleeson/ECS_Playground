using Unity.Entities;

[GenerateAuthoringComponent]
public struct VerticalWaveMovementModifier : IComponentData
{
    public float distance;
    public float frequency;
    public float currentPhase;
}