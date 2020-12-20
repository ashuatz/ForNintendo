using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [SerializeField] TimerUI _timer;

    [SerializeField] Image _background;
    [SerializeField] float _fadeSpeed = 1.0f;

    [SerializeField] Image[] _clearImg;
    [SerializeField] float _imgFadeSpeed = 1.0f;

    [SerializeField] Text _timeText;

    [SerializeField] Image _rankImg;
    [SerializeField] Image[] _rankImgs;

    [SerializeField] Rank[] _ranks;

    void Start()
    {
        
    }

    IEnumerator Ending()
    {
        float t = 0;

        while (t < 1)
        {
            t += Mathf.Min(1, t + Time.deltaTime * _fadeSpeed);
            _background.color = new Color(0, 0, 0, t * 0.7f);

            yield return true;
        }

        t = 0;

        while(t<1)
        {
            t += Mathf.Min(1, t + Time.deltaTime * _imgFadeSpeed);
            for (int i = 0; i < _clearImg.Length; i++)
                _clearImg[i].color = new Color(1, 1, 1, t);

            yield return true;
        }


    }
}

[System.Serializable]
public class Rank
{
    public string _rank;
    public string _time;
}