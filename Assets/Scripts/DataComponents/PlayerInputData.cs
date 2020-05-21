using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct PlayerInputData : IComponentData
{
    [UnityEngine.Range(0, 3)]
    public int id;
    public PlayerInputState state;
}

public struct PlayerInputState
{
    public float2 Movement;
    public bool IsJumping;
}