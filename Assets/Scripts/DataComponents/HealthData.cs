using Unity.Entities;

[GenerateAuthoringComponent]
public struct HealthData : IComponentData
{
    public float current;
    //float max;
}
