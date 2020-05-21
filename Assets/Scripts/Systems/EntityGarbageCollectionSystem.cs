using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

[AlwaysSynchronizeSystem]
[UpdateAfter(typeof(PickupSystem))]
public class EntityGarbageCollectionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<DeleteTag>().ForEach((ref Entity entity) => {
            commandBuffer.DestroyEntity(entity);
        }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        return default;
    }
}
