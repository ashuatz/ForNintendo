using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public static EndingManager _EndingManager { get; private set; }

    [SerializeField] TimerUI _timer;

    [SerializeField] Image _background;
    [SerializeField] float _fadeSpeed = 1.0f;

    [SerializeField] Image[] _clearImg;
    [SerializeField] float _imgFadeSpeed = 1.0f;

    [SerializeField] Text _timeName;
    [SerializeField] Text _timeText;

    [SerializeField] Text _rankName;
    [SerializeField] Image _rankImg;
    [SerializeField] Sprite[] _rankImgs;

    [SerializeField] int[] _rankTime;

    public bool _IsEnding { get; private set; }

    void Awake()
    {
        _EndingManager = this;
        _IsEnding = false;
    }

    public IEnumerator Ending()
    {
        _IsEnding = true;

        float t = 0;

        while (t < 1)
        {
            t = Mathf.Min(1, t + Time.deltaTime * _fadeSpeed);
            _background.color = new Color(0, 0, 0, t * 0.7f);

            yield return true;
        }

        t = 0;

        while(t<1)
        {
            t = Mathf.Min(1, t + Time.deltaTime * _imgFadeSpeed);
            for (int i = 0; i < _clearImg.Length; i++)
                _clearImg[i].color = new Color(1, 1, 1, t);

            yield return true;
        }

        _timeName.gameObject.SetActive(true);
        _timeText.gameObject.SetActive(true);
        int sec = 0;
        while (sec < _timer.CurrentTime)
        {
            sec++;
            _timeText.text = Mathf.FloorToInt(sec / 60).ToString() + ":" + sec % 60;

            yield return true;
        }

        t = 0;
        while(t<1)
        {
            t += Time.deltaTime;

            yield return true;
        }

        _rankName.gameObject.SetActive(true);
        _rankImg.gameObject.SetActive(true);
        for (int i = 0; i < _rankTime.Length; i++)
        {
            if (sec <= _rankTime[i])
            {
                _rankImg.sprite = _rankImgs[i];
                break;
            }
            else if (i >= _rankTime.Length - 1)
                _rankImg.sprite = _rankImgs[i];

        }
    }
}