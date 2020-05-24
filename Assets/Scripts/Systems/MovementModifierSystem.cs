using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class MovementModifierSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float maxRadians = 2 * math.PI;

        // Basic Movement
        Entities.ForEach((ref Translation location, in LocalToWorld matrix, in BasicMovementModifierData modifier) => {
            //float2 normDirection = math.normalize(modifier.direction);
            //Vector3 direction = new Vector3(modifier.direction.x, 0, modifier.direction.y);
            //direction = Vector3.Project(direction, matrix.Forward);
            float3 normDirection = math.normalize(matrix.Forward);
            location.Value.xyz += (float3)(normDirection * modifier.speed * deltaTime);
        }).Run();

        // HorizontalWaveMovementModifier
        Entities.ForEach((ref Translation location, ref HorizontalWaveMovementModifier data) => {
            data.currentPhase += deltaTime * data.frequency;
            if (data.currentPhase >= maxRadians) {
                data.currentPhase = -maxRadians;
            }
            location.Value.x += math.cos(data.currentPhase) * data.distance * deltaTime;
        }).Run();

        // VerticalWaveMovementModifier
        Entities.ForEach((ref Translation location, ref VerticalWaveMovementModifier data) => {
            data.currentPhase += deltaTime * data.frequency;
            if (data.currentPhase >= maxRadians) {
                data.currentPhase = -maxRadians;
            }
            location.Value.y += math.sin(data.currentPhase) * data.distance * deltaTime;
        }).Run();

        return default;
    }
}
