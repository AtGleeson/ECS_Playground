using Unity.Entities;

[GenerateAuthoringComponent]
public struct SplineMovementData : IComponentData
{
    public int splineIndex;
    public SplineWalkerMode mode;
    public float duration;
    public bool lookForward;

    public float progress;
    public bool goingForward;
}
