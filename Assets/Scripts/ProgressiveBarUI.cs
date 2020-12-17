using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressiveBarUI : MonoBehaviour
{
    [SerializeField] Image[] _progressiveBar;

    TestEnemySpawner_New[] _spawner;

    int[] _maxEnemy;
    int[] _killEnemy;

    int _barMany;

    public bool _onBar = true;

    void Start()
    {
        _spawner = SpawnerParent._Spawners;

        _barMany = _spawner.Length;
        _maxEnemy = new int[_barMany];

        for (int i = 0; i < _barMany; i++)
        {
            _maxEnemy[i] = 0;
            for (int j = 0; j < _spawner[i]._WaveData.Length; j++)
                _maxEnemy[i] += _spawner[i]._WaveData[j]._maxSpawnCount;
        }
    }

    void Update()
    {
        if (_onBar)
            DrawProgressiveBar();
    }

    void DrawProgressiveBar()
    {
        for (int i = 0; i < _barMany; i++)
            _progressiveBar[i].fillAmount = (float)_spawner[i]._killEnemy / _maxEnemy[i];
    }
}
