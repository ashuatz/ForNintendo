using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class TestEnemySpawner_New : MonoBehaviour
{
    [SerializeField]
    private Vector4 SpawnRangeXZ;
    //[SerializeField]
    //private Vector3 SpawnRate;

    //[SerializeField]
    //private int MaxSpawnCount;

    [SerializeField]
    private WaveData[] _waveData;
    public WaveData[] _WaveData { get { return _waveData; } }

    [SerializeField]
    private float SpawnPerSecond;

    [SerializeField]
    private Transform FirstAttackTarget;

    [SerializeField]
    private List<TestEnemy> EnemyOrigin;

    private List<TestEnemy> Instances = new List<TestEnemy>();

    private Dictionary<int, List<TestEnemy>> Pool = new Dictionary<int, List<TestEnemy>>();

    private int _nowWave = 0;
    private int _nowSpawn = 0;

    public int _killEnemy { get; private set; }

    private void Start()
    {
        _killEnemy = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(StartSpawn());
    }

    IEnumerator StartSpawn()
    {
        while (_waveData.Length > _nowWave)
        {
            _nowSpawn = 0;
            for (int i = 0; i < _waveData[_nowWave]._maxSpawnCount; i++)
            {
                Vector3 SpawnRate = _waveData[_nowWave]._spawnRate;

                _nowSpawn++;
                var tester = UnityEngine.Random.Range(0, SpawnRate.x + SpawnRate.y + SpawnRate.z);
                if (tester > SpawnRate.x + SpawnRate.y)
                {
                    Spawn(2);
                }
                else if (tester > SpawnRate.x)
                {
                    Spawn(1);
                }
                else
                {
                    Spawn(0);
                }
                yield return YieldInstructionCache.WaitForSeconds(1 / SpawnPerSecond);
            }

            yield return YieldInstructionCache.WaitForSeconds(_waveData[_nowWave]._waitTimeForNextWave);
            _nowWave++;
        }
    }

    private void Spawn(in int index)
    {
        var instance = GetObjectFormPool(index);
        instance.transform.position = transform.position + SpawnRangeXZ.GetRandom().ToVector3FromXZ().Round(1);
        instance.Initialize(FirstAttackTarget);
        instance.OnDead += Instance_OnDead;
        instance.gameObject.SetActive(true);

        Instances.Add(instance);
    }

    private void Instance_OnDead(TestEntity entity)
    {
        var instance = entity as TestEnemy;
        Instances.Remove(instance);
        _killEnemy++;
        SpawnerParent._SpawnerParent.CheckSectorClear();

        instance.OnDead -= Instance_OnDead;

        AddToPool(instance);
    }

    private void AddToPool(TestEnemy enemy)
    {
        if (!Pool.TryGetValue((int)enemy.MyEnemyType, out var list))
        {
            list = new List<TestEnemy>();
            Pool.Add((int)enemy.MyEnemyType, list);
        }
        list.Add(enemy);
    }

    private TestEnemy GetObjectFormPool(int index)
    {
        if (!Pool.TryGetValue(index, out var list))
        {
            list = new List<TestEnemy>();
            Pool.Add(index, list);

            return Instantiate(EnemyOrigin[index]);
        }

        if (Pool[index].Count((enemy) => !enemy.gameObject.activeInHierarchy) <= 0)
            return Instantiate(EnemyOrigin[index]);

        var instance = list.First((enemy) => !enemy.gameObject.activeInHierarchy);
        list.Remove(instance);

        return instance;
    }
}

[System.Serializable]
public class WaveData
{
    public int _maxSpawnCount;
    public Vector3 _spawnRate;
    public float _waitTimeForNextWave;
}