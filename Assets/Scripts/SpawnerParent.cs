using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerParent : MonoBehaviour
{
    [SerializeField] TestEnemySpawner_New[] _spawners;
    static SpawnerParent _SpawnerParent;
    public static TestEnemySpawner_New[] _Spawners { get { return _SpawnerParent._spawners; } }

    private void Awake()
    {
        _SpawnerParent = this;
    }
}
