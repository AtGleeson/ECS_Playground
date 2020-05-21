using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(PlayerInputSystem))]
public class PlayerMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Translation vel, in PlayerInputData inputData, in PlayerMovementData speedData) => {
            float2 newVelocity = vel.Value.xz;
            newVelocity += (inputData.state.Movement * speedData.speed * deltaTime);
            //vel.Linear.xz = newVelocity;
            vel.Value.xz = newVelocity;
        }).Run();

        return default;
    }
}
