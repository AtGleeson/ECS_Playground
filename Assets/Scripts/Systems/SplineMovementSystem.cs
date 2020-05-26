using Unity.Collections;
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
        BezierSpline[] splines = EnemyFactory.instance.GetSplines();

        Entities.ForEach((ref Translation translation, ref Rotation rotation, ref SplineMovementData splineData) => {
            BezierSpline spline = splines[splineData.splineIndex];
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
