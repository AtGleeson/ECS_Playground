using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[AlwaysSynchronizeSystem]
[UpdateBefore(typeof(EntityGarbageCollectionSystem))]
public class LifetimeSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        ComponentDataFromEntity<DeleteTag> deletableEntities = GetComponentDataFromEntity<DeleteTag>(true);
        EntityCommandBuffer commandBuffer = entityCommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity entity, ref LifetimeData lifetime) => {
            lifetime.timeRemaining -= deltaTime;
            if (lifetime.timeRemaining <= 0) {
                if (!deletableEntities.HasComponent(entity)) {
                    commandBuffer.AddComponent(entity, new DeleteTag());
                }
            }
        }).Run();

        return default;
    }
}
