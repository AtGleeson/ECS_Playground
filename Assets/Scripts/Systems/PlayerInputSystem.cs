using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PlayerInputSystem : JobComponentSystem
{
    private PlayerInputState[] inputStates;

    protected override void OnCreate()
    {
        base.OnCreate();
        inputStates = new PlayerInputState[4];
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        // Grab all player inputs
        inputStates[0].Movement.x = Input.GetAxis("Horizontal");
        inputStates[0].Movement.y = Input.GetAxis("Vertical");
        inputStates[0].IsJumping = Input.GetAxis("Jump") >= 0.2f;

        NativeArray<PlayerInputState> stupidInputCopy = new NativeArray<PlayerInputState>(inputStates, Allocator.Temp);
        Entities.ForEach((ref PlayerInputData inputData) => {
            inputData.state = stupidInputCopy[inputData.id];
        }).Run();
        stupidInputCopy.Dispose();

        return default;
    }
}
