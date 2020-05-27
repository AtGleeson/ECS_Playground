using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem]
public class SplineMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref Translation translation, ref Rotation rotation, ref SplineMovementData splineData) => {
            splineData.movementDelay -= deltaTime;
            if (!splineData.isActive && splineData.movementDelay <= 0) {
                splineData.isActive = true;
                splineData.progress -= splineData.movementDelay; // subtract movementDelay from progress to make sure we start in the right position  and don't lose time
            }

            if (!splineData.isActive) {
                translation.Value.y = -1.0f;    // hide below the level for now
                return;
            }

            BezierSpline spline = SplineManager.GetSpline(splineData.splineIndex);
            if (splineData.goingForward) {
                splineData.progress += deltaTime / splineData.duration;
                if (splineData.progress > 1f) {
                    if (splineData.mode == SplineWalkerMode.Once) {
                        splineData.progress = 1f;
                    } else if (splineData.mode == SplineWalkerMode.Loop) {
                        splineData.progress -= 1f;
                    } else {
                        splineData.progress = 2f - splineData.progress;
                        splineData.goingForward = false;
                    }
                }
            } else {
                splineData.progress -= deltaTime / splineData.duration;
                if (splineData.progress < 0f) {
                    splineData.progress = -splineData.progress;
                    splineData.goingForward = true;
                }
            }
            Vector3 position = spline.GetPoint(splineData.progress);
            translation.Value = position;
            if (splineData.lookForward) {
                rotation.Value = quaternion.LookRotation(spline.GetDirection(splineData.progress), Vector3.up);
            }
        }).WithoutBurst().Run();

        return default;
    }
}
