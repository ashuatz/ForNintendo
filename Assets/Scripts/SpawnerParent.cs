using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerParent : MonoBehaviour
{
    [SerializeField] Sector[] _sectors;

    [SerializeField] 
    static SpawnerParent _SpawnerParent;
    public static Sector[] _Sectors { get { return _SpawnerParent._sectors; } }

    private void Awake()
    {
        _SpawnerParent = this;
    }
}

[System.Serializable]
public class Sector
{
    public TestEnemySpawner_New[] _spawners;
}