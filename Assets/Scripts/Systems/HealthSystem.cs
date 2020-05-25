using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[AlwaysSynchronizeSystem]
[UpdateBefore(typeof(EntityGarbageCollectionSystem))]
public class HealthSystem : JobComponentSystem
{
    private BeginSimulationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
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
        HealthTriggerJob triggerJob = new HealthTriggerJob {
            healthEntities = GetComponentDataFromEntity<HealthData>(),
            damageEntities = GetComponentDataFromEntity<DamageData>(true),
            deletableEntities = GetComponentDataFromEntity<DeleteTag>(true),
            commandBuffer = bufferSystem.CreateCommandBuffer()
        };
        JobHandle job = triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDependencies);
        job.Complete();

        return default;
    }

    [BurstCompile]
    private struct HealthTriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer commandBuffer;
        public ComponentDataFromEntity<HealthData> healthEntities;
        [ReadOnly] public ComponentDataFromEntity<DamageData> damageEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> deletableEntities;

        public void Execute(TriggerEvent triggerEvent)
        {
            if (!TestTrigger(triggerEvent.Entities.EntityA, triggerEvent.Entities.EntityB)) {
                TestTrigger(triggerEvent.Entities.EntityB, triggerEvent.Entities.EntityA);
            }
        }

        public bool TestTrigger(Entity healthEntity, Entity damageEntity)
        {
            if (healthEntities.HasComponent(healthEntity) && damageEntities.HasComponent(damageEntity)) {
                // these are readonly
                HealthData healthData = healthEntities[healthEntity];
                DamageData damageData = damageEntities[damageEntity];

                // Handle healthEntity update
                if (!deletableEntities.HasComponent(healthEntity)) {
                    commandBuffer.SetComponent(healthEntity, new HealthData { current = healthData.current - damageData.damageAmount });

                    if (healthData.current <= 0) {
                        commandBuffer.AddComponent(healthEntity, new DeleteTag());
                    }
                }

                // Handle damageEntity
                if (!deletableEntities.HasComponent(damageEntity)) {
                    if (damageData.destroyOnCollision) {
                        commandBuffer.AddComponent(damageEntity, new DeleteTag());
                    }
                }

                return true;                
            }
            return false;
        }
    }
}
