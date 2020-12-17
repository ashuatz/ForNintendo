using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildResourceManager : MonoBehaviour
{
    public static BuildResourceManager _BuildResourceManager { get; private set; }

    [SerializeField] BuildResourceData[] _resourceData;
    public BuildResourceData[] _ResourceData { get { return _resourceData; } }

    public bool _IsPlaying = true;

    private void Awake()
    {
        _BuildResourceManager = this;
    }

    void Update()
    {
        if (_IsPlaying)
            TowerRefill();
    }

    void TowerRefill()
    {
        for(int i=0;i<_resourceData.Length;i++)
        {
            BuildResourceData data = _resourceData[i];
            if (data._haveTower<data._maxHaveTower)
            {
                _resourceData[i]._nowCool = Mathf.Max(0, data._nowCool - Time.deltaTime);
                if (_resourceData[i]._nowCool<=0)
                {
                    _resourceData[i]._nowCool = data._refillCool;
                    _resourceData[i]._haveTower++;
                }
            }
        }
    }


    public bool IsHaveTower(int towerIndex)
    {
        return _resourceData[towerIndex]._haveTower > 0 ? true : false;
    }

    public void UseTower(int towerIndex) => _resourceData[towerIndex]._haveTower--;
}

[System.Serializable]
public class BuildResourceData
{
    public int _haveTower = 1;
    public int _maxHaveTower = 5;
    public float _refillCool;
    [HideInInspector] public float _nowCool;
}
