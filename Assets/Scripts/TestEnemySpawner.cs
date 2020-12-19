using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class TestEnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Vector4 SpawnRangeXZ;
    [SerializeField]
    private Vector3 SpawnRate;

    [SerializeField]
    private int MaxSpawnCount;

    [SerializeField]
    private float SpawnPerSecond;

    [SerializeField]
    private Transform FirstAttackTarget;

    [SerializeField]
    private List<TestEnemy> EnemyOrigin;

    [SerializeField]
    private SpecialEnemyTypeA SpecialEnemyAOrigin;

    private List<TestEnemy> Instances = new List<TestEnemy>();
    private List<SpecialEnemyTypeA> SpecialEnemyInstances = new List<SpecialEnemyTypeA>();

    private Dictionary<int, List<TestEnemy>> Pool = new Dictionary<int, List<TestEnemy>>();
    private List<SpecialEnemyTypeA> SpecialEnemyPool = new List<SpecialEnemyTypeA>();


    private void OnEnable()
    {
        StartCoroutine(StartSpawn());
    }

    IEnumerator StartSpawn()
    {
        yield return null;
        yield return null;

        while (enabled)
        {
            if (Instances.Count + SpecialEnemyInstances.Count < MaxSpawnCount)
            {
                var tester = UnityEngine.Random.Range(0, SpawnRate.x + SpawnRate.y + SpawnRate.z);
                if (tester > SpawnRate.x + SpawnRate.y)
                {
                    Spawn(2);
                }
                else if (tester > SpawnRate.x)
                {
                    SpawnSpecialEnemyA();
                }
                else
                {
                    Spawn(0);
                }

            }
            yield return YieldInstructionCache.WaitForSeconds(1 / SpawnPerSecond);
        }
    }

    private void SpawnSpecialEnemyA()
    {
        var instance = SpecialEnemyPool.Find((seta) => !seta.gameObject.activeInHierarchy);
        if (instance == null)
        {
            instance = Instantiate(SpecialEnemyAOrigin);
        }
        else
        {
            SpecialEnemyPool.Remove(instance);
        }

        instance.transform.position = transform.position + SpawnRangeXZ.GetRandom().ToVector3FromXZ().Round(1);
        instance.Initialize(FirstAttackTarget);
        instance.OnDead += SpecialEnemyInstance_Ondead;
        instance.gameObject.SetActive(true);

        SpecialEnemyInstances.Add(instance);
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

    private void SpecialEnemyInstance_Ondead(TestEntity entity)
    {
        var instance = entity as SpecialEnemyTypeA;
        SpecialEnemyInstances.Remove(instance);

        instance.OnDead -= SpecialEnemyInstance_Ondead;

        AddToPool(instance);
    }

    private void Instance_OnDead(TestEntity entity)
    {
        var instance = entity as TestEnemy;
        Instances.Remove(instance);

        instance.OnDead -= Instance_OnDead;

        AddToPool(instance);
    }

    private void AddToPool(SpecialEnemyTypeA enemy)
    {
        SpecialEnemyPool.Add(enemy);
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
        if (index == 1)
            return null;

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
