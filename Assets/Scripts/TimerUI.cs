using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    private static event Action<bool> OnPause;

    [SerializeField]
    private Text TimerText;

    [SerializeField]
    private AnimationCurve TicTok;

    [SerializeField]
    private string format_tic;
    [SerializeField]
    private string format_tok;

    private float currentTime;

    private bool isPause;

    private void Awake()
    {
        OnPause += TimerUI_OnPause;

        SetStart();
    }

    private void TimerUI_OnPause(bool obj)
    {
        isPause = obj;
    }

    private void Update()
    {
        if (isPause)
            return;

        currentTime += Time.deltaTime;

        var format = TicTok.Evaluate(Mathf.Repeat(Time.time, 1)) > 0 ? format_tic : format_tok;
        TimerText.text = string.Format(format, Mathf.FloorToInt(currentTime / 60), Mathf.FloorToInt(currentTime % 60));
    }

    private void OnDestroy()
    {
        OnPause -= TimerUI_OnPause;
    }

    public static void SetStart()
    {
        OnPause?.Invoke(false);
    }

    public static void SetEnd()
    {
        OnPause?.Invoke(true);
    }

}
