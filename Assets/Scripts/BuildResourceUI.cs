using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static BuildResourceManager;

public class BuildResourceUI : MonoBehaviour
{
    [SerializeField] GameObject[] _slots;

    Image[] _coolImage;
    Text[] _haveText;

    bool _settingOk = false;

    private void Start()
    {
        _coolImage = new Image[_slots.Length];
        _haveText = new Text[_slots.Length];

        if (_slots.Length>_BuildResourceManager._ResourceData.Length)
        {
            Debug.LogError("실제 타워 종류보다 많은 UI 슬롯이 셋팅되어있습니다. \n BuildResourceManager 의 ResourceData 수와 BuildResourceUI 의 _slot 수를 맞춰주십시오");
            return;
        }

        for(int i=0;i<_slots.Length;i++)
        {
            _coolImage[i] = _slots[i].transform.GetChild(0).GetComponent<Image>();
            _haveText[i] = _slots[i].transform.GetChild(2).GetComponent<Text>();
        }
        _settingOk = true;
    }

    private void Update()
    {
        if (_settingOk)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                DrawCool(i);
                DrawHaveText(i);
            }
        }
    }

    void DrawCool(int n)
    {
        BuildResourceData data = _BuildResourceManager._ResourceData[n];
        if (data._haveTower >= data._maxHaveTower)
            _coolImage[n].fillAmount = 0;
        else _coolImage[n].fillAmount = data._nowCool / data._refillCool;
    }

    void DrawHaveText(int n)
    {
        BuildResourceData data = _BuildResourceManager._ResourceData[n];
        _haveText[n].text = data._haveTower.ToString();
    }
}
