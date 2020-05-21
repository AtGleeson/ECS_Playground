using Unity.Entities;

[GenerateAuthoringComponent]
public struct HorizontalWaveMovementModifier : IComponentData
{
    public float distance;
    public float frequency;
    public float currentPhase;
}