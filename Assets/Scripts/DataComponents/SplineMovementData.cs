using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SplineMovementData : IComponentData
{
    public int splineIndex;
    public SplineWalkerMode mode;
    public float duration;
    public float movementDelay;
    public bool lookForward;

    [HideInInspector]
    public float progress;
    [HideInInspector]
    public bool goingForward;
    [HideInInspector]
    public bool isActive;
}
