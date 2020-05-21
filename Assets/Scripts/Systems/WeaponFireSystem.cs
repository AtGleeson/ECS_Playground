using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections.Generic;
using System;

[UpdateAfter(typeof(PlayerInputSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class WeaponFireSystem : SystemBase
{
    /* 
     * https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/master/ECSSamples/Assets/HelloCube/5.%20SpawnFromEntity/SpawnerSystem_FromEntity.cs
     */
    // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
    // when that barrier system executes.
    // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
    // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
    // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
    // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
    // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
    BeginInitializationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to perform such changes on the main thread after the Job has finished.
        //Command buffers allow you to perform any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and deletions for later.
        var commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
    
        // Schedule the Entities.ForEach lambda job that will add Instantiate commands to the EntityCommandBuffer.
        // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance (3rd parameter of WithBurst)
        // The actual job will be cached once it is compiled (it will only get Burst compiled once).
        Entities.WithName("WeaponFireSystem")
            .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
            .ForEach((Entity entity, int entityInQueryIndex, in PlayerInputData inputData, in WeaponFireData weaponData, in LocalToWorld location) => {
                if (inputData.state.IsJumping) {
                   // List<Entity> entities = new List<Entity>();                    
                    
                    float halfAngle = weaponData.hSpreadAngle / 2;
                    float direction = -1.0f;
                    bool countIsOdd = weaponData.hCount % 2 != 0;
                    int horizontalCount = countIsOdd ? (weaponData.hCount - 1) : weaponData.hCount;
    
                    for (int i = 1; i <= horizontalCount; ++i) {
                        var bullet = commandBuffer.Instantiate(entityInQueryIndex, weaponData.bulletPrefab);
                        commandBuffer.SetComponent(entityInQueryIndex, bullet, new Translation { Value = location.Position });
                        //entities.Add(bullet);

                        int angleSpacing = 1 + (i / 2);
                        float newAngle = (halfAngle * direction * angleSpacing);
                        quaternion newRotation = quaternion.EulerXYZ(math.rotate(location.Value, newAngle));
                        commandBuffer.SetComponent(entityInQueryIndex, bullet, new Rotation { Value = newRotation });
                        direction *= -1.0f;
                    }

                    if (countIsOdd) { // spawn the default if we're an odd count
                        var bullet = commandBuffer.Instantiate(entityInQueryIndex, weaponData.bulletPrefab);
                        commandBuffer.SetComponent(entityInQueryIndex, bullet, new Translation { Value = location.Position });
                        //entities.Add(bullet);
                    }
                }
            }).ScheduleParallel();

        // SpawnJob runs in parallel with no sync point until the barrier system executes.
        // When the barrier system executes we want to complete the SpawnJob and then play back the commands (Creating the entities and placing them).
        // We need to tell the barrier system which job it needs to complete before it can play back the commands.
        entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
