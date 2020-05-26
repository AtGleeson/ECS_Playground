using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory instance;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    [SerializeField]
    private GameObject enemyPrefab;
    private Entity enemyEntity;
    [SerializeField]
    private BezierSpline[] enemySplines;
    [SerializeField]
    private int enemyCount;

    public BezierSpline[] GetSplines()
    {
        return enemySplines;
    }

    void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        // convert enemyPrefab to Entity
        enemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, settings);
    }

    void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

    void Start()
    {
        NativeArray<Entity> enemies = new NativeArray<Entity>(enemyCount, Allocator.Temp);
        entityManager.Instantiate(enemyEntity, enemies);
        float progressSplit = 1.0f / enemyCount;
        int i = 0;
        foreach (Entity e in enemies) {
            // TODO: custom conversion logic to set these?
            entityManager.SetComponentData(e, new SplineMovementData {
                splineIndex = 0, // TODO: make this controlable
                goingForward = true,
                lookForward = true,
                duration = 5.0f,
                progress = progressSplit * i,
                mode = SplineWalkerMode.Loop
            });
            ++i;
        }
        enemies.Dispose();
    }
}
