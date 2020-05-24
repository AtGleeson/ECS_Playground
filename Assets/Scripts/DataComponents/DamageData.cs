using Unity.Entities;

[GenerateAuthoringComponent]
public struct DamageData : IComponentData
{
    public float damageAmount;
    public bool destroyOnCollision;
}
