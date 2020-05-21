using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class RotationSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in RotationData rotationData) => {
            float3 normalized = math.normalize(rotationData.direction) * rotationData.speed * deltaTime;
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(normalized.x)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(normalized.y)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(normalized.z)));
        }).Run();

        return default;
    }
}
