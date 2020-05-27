using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class SquadronSpawner : MonoBehaviour
{
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private Entity shipEntity;

    [SerializeField]
    private GameObject shipPrefab;

    [SerializeField]
    private BezierSpline spline;

    [SerializeField]
    private SplineWalkerMode mode = SplineWalkerMode.Once;

    [SerializeField]
    private int shipCount = 10;

    [SerializeField]
    [Tooltip("Time in seconds between each ship moving")]
    private float shipDelay;

    [SerializeField]
    private float splineDuration = 5.0f;

    private int splineIndex;

    private void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        shipEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(shipPrefab, settings);
    }

    private void Start()
    {
        splineIndex = SplineManager.AddSpline(spline);
        OnActivate();
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    public void OnActivate()
    {        
        NativeArray<Entity> enemies = new NativeArray<Entity>(shipCount, Allocator.Temp);
        entityManager.Instantiate(shipEntity, enemies);
        int i = 0;
        foreach (Entity e in enemies) {
            // TODO: custom conversion logic to set these?
            entityManager.SetComponentData(e, new SplineMovementData {
                splineIndex = splineIndex,
                goingForward = true,
                lookForward = true,
                duration = splineDuration,
                movementDelay = shipDelay * i,
                mode = mode
            });
            ++i;
        }
        enemies.Dispose();
    }
}
