using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [Serializable]
    public class HPGauge
    {
        public List<GameObject> Cells;
    }

    [SerializeField]
    private float HPPerCell;

    [SerializeField]
    private List<HPGauge> Gauge;

    [SerializeField]
    private TestEntity entity;

    private void Awake()
    {
        entity.HP.OnDataChanged += HP_OnDataChanged;

        HP_OnDataChanged(entity.HP.CurrentData);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void HP_OnDataChanged(float currentHP)
    {
        if (currentHP <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        var count = Mathf.CeilToInt(currentHP / HPPerCell);

        foreach (var phase in Gauge)
        {
            foreach (var cell in phase.Cells)
            {
                cell.SetActive(count > 0);
                count -= 1;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void OnDestroy()
    {
        entity.HP.OnDataChanged -= HP_OnDataChanged;
    }
}
