using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class PickupSystem : JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        bufferSystem.Enabled = false;
        buildPhysicsWorld.Enabled = false;
        stepPhysicsWorld.Enabled = false;
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        PickupTriggerJob triggerJob = new PickupTriggerJob {
            pickUpEntities = GetComponentDataFromEntity<PickupData>(),
            playerEntities = GetComponentDataFromEntity<PlayerMovementData>(),
            deletableEntities = GetComponentDataFromEntity<DeleteTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer()
        };
        inputDependencies = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        bufferSystem.AddJobHandleForProducer(inputDependencies);
        return inputDependencies;
    }
    
    private struct PickupTriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer commandBuffer;
        [ReadOnly] public ComponentDataFromEntity<PlayerMovementData> playerEntities;
        [ReadOnly] public ComponentDataFromEntity<PickupData> pickUpEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> deletableEntities;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (!TestTrigger(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB)) {
                TestTrigger(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            }
        }

        public bool TestTrigger(Entity entityA, Entity entityB)
        {
            if (playerEntities.HasComponent(entityA) && pickUpEntities.HasComponent(entityB)) {
                if (!deletableEntities.HasComponent(entityB)) {
                    commandBuffer.AddComponent(entityB, new DeleteTag());
                    return true;
                }
            }
            return false;
        }
    }
}
