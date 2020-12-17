using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerParent : MonoBehaviour
{
    [SerializeField] Sector[] _sectors;
    [SerializeField] Transform _wayPointProbe;

    [SerializeField] UnityEvent _clearEvt;

    public static SpawnerParent _SpawnerParent { get; private set; }
    public static Sector[] _Sectors { get { return _SpawnerParent._sectors; } }

    int _nowSector = 0;

    private void Awake()
    {
        _SpawnerParent = this;

        for(int i=0;i<_sectors.Length;i++)
        {
            _sectors[i]._maxEnemy = 0;
            for (int j = 0; j < _sectors[i]._spawners.Length; j++)
                for (int k = 0; k < _sectors[i]._spawners[j]._WaveData.Length; k++)
                    _sectors[i]._maxEnemy += _sectors[i]._spawners[j]._WaveData[k]._maxSpawnCount;
        }
    }

    public void CheckSectorClear()
    {
        if (GetKillEnemyMany(_nowSector) >= _sectors[_nowSector]._maxEnemy)
        {
            _nowSector++;
            if (_nowSector < _sectors.Length)
                _clearEvt.Invoke();
        }
    }

    int GetKillEnemyMany(int sectorNum)
    {
        int many = 0;
        for (int i = 0; i < _sectors[sectorNum]._spawners.Length; i++)
            many += _sectors[sectorNum]._spawners[i]._killEnemy;

        return many;
    }

    public void ChangeWaypointPos() => _wayPointProbe.position = _sectors[_nowSector]._wayPointSetPos;
}

[System.Serializable]
public class Sector
{
    public Vector3 _wayPointSetPos;
    public TestEnemySpawner_New[] _spawners;
    [HideInInspector] public int _maxEnemy = 999; 
}