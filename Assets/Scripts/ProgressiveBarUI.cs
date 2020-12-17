using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressiveBarUI : MonoBehaviour
{
    [SerializeField] Image[] _progressiveBar;

    Sector[] _sector;

    int[] _maxEnemy;
    int[] _killEnemy;

    int _sectorMany;

    public bool _onBar = true;

    void Start()
    {
        _sector = SpawnerParent._Sectors;

        _sectorMany = _sector.Length;
        _maxEnemy = new int[_sectorMany];

        for (int i = 0; i < _sectorMany; i++)
            _maxEnemy[i] = _sector[i]._maxEnemy;
    }

    void Update()
    {
        if (_onBar)
            DrawProgressiveBar();
    }

    void DrawProgressiveBar()
    {
        for (int i = 0; i < _sectorMany; i++)
        {
            int kill = 0;
            for (int j = 0; j < _sector[i]._spawners.Length; j++)
                kill += _sector[i]._spawners[j]._killEnemy;
            _progressiveBar[i].fillAmount = (float)kill / _maxEnemy[i];
        }
    }
}
