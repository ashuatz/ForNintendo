using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{

    [Header("init")]
    [SerializeField]
    private Image BackgroundImage;


    [Serializable]
    public class HPGauge
    {
        public List<Image> Cells;
    }

    [SerializeField]
    private HPColorData colorData;

    [SerializeField]
    private float HPPerCell;

    [SerializeField]
    private List<HPGauge> Gauge;

    [SerializeField]
    private TestEntity entity;

    private void Awake()
    {
        ColorInit();

        entity.HP.OnDataChanged += HP_OnDataChanged;

        HP_OnDataChanged(entity.HP.CurrentData);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void ColorInit()
    {
        HPColorData.HPColor colorInfo = new HPColorData.HPColor();

        switch (entity)
        {
            case TestPlayer p:
                colorInfo = colorData.colors.Find((data) => data.myType == HPColorData.HPType.Player);
                break;
            
            case TestMinion m:
                colorInfo = colorData.colors.Find((data) => data.myType == HPColorData.HPType.Minion);
                break;
            
            case TestEnemy e:
            case SpecialEnemyTypeA s:
                colorInfo = colorData.colors.Find((data) => data.myType == HPColorData.HPType.Enemy);
                break;
            
            case TestNPC n:
                colorInfo = colorData.colors.Find((data) => data.myType == HPColorData.HPType.NPC);
                break;

            default: return;
        }

        BackgroundImage.color = colorInfo.BackgroundColor;
        for (int i = 0; i < 3; i++)
        {
            foreach (var cell in Gauge[i].Cells)
            {
                cell.color = colorInfo.GetColor(i);
            }
        }
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
                cell.gameObject.SetActive(count > 0);
                count -= 1;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private void OnDestroy()
    {
        if (entity != null)
        {
            entity.HP.OnDataChanged -= HP_OnDataChanged;
        }
    }
}
