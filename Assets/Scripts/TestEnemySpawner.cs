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

    private List<TestEnemy> Instances = new List<TestEnemy>();

    private Dictionary<int, List<TestEnemy>> Pool = new Dictionary<int, List<TestEnemy>>();

    IEnumerator Start()
    {
        while (enabled)
        {
            if (Instances.Count < MaxSpawnCount)
            {
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

            }
            yield return YieldInstructionCache.WaitForSeconds(1 / SpawnPerSecond);
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

        instance.OnDead -= Instance_OnDead;

        AddToPool(instance);
    }

    private void AddToPool(TestEnemy enemy)
    {
        if (!Pool.TryGetValue(enemy.MyEnemyType, out var list))
        {
            list = new List<TestEnemy>();
            Pool.Add(enemy.MyEnemyType, list);
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

        if (Pool[index].Count <= 0)
            return Instantiate(EnemyOrigin[index]);

        var instance = list.First();
        list.Remove(instance);

        return instance;
    }
}
