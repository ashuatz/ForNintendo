using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHPDisplayer : MonoBehaviour
{
    [SerializeField]
    private Image bar;
    [SerializeField]
    private Text desc;
    [SerializeField]
    private TestEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        entity.HP.OnDataChanged += HP_OnDataChanged;
        HP_OnDataChanged(entity.HP.CurrentData);
    }

    private void HP_OnDataChanged(float obj)
    {
        desc.text = "HP:" + obj;
    }

    private void OnDestroy()
    {
        entity.HP.OnDataChanged -= HP_OnDataChanged;
    }
}
