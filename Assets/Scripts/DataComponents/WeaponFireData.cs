using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WeaponFireData : IComponentData
{
    public Entity bulletPrefab;

    // TODO: Modifiers or just data here?
    public int hCount;
    public float hSpreadAngle;
    public int vCount;
    public float vSpreadAngle;
}
