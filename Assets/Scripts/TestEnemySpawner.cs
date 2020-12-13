using System.Collections;
using System.Collections.Generic;
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
        var instance = Instantiate(EnemyOrigin[index]);
        instance.transform.position = transform.position + SpawnRangeXZ.GetRandom().ToVector3FromXZ().Round(1);
        instance.Initialize(FirstAttackTarget);

        Instances.Add(instance);
    }
}
