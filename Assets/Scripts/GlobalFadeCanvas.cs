using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Util;


public class GlobalFadeCanvas : MonoSingleton<GlobalFadeCanvas>
{
    [SerializeField]
    private Image FadeImage;

    private CoroutineWrapper wrapper;

    protected override void Awake()
    {
        base.Awake();

        wrapper = new CoroutineWrapper(this);
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        Off();
    }

    public void On(Action onComplete = null)
    {
        if (onComplete != null)
        {
            wrapper.StartSingleton(Run(0, 1, 1f)).SetOnComplete(onComplete);
        }
        else
        {
            wrapper.StartSingleton(Run(0, 1, 1f));
        }
    }


    public void Off()
    {
        wrapper.StartSingleton(Run(1, 0, 1f));
    }


    private IEnumerator Run(float start, float end, float runtime)
    {
        float t = 0;
        while (t < runtime)
        {
            var color = FadeImage.color;
            color.a = (t / runtime).Remap((0, 1), (start, end));
            FadeImage.color = color;
            t += Time.deltaTime;
            yield return null;
        }

        var finalColor = FadeImage.color;
        finalColor.a = end;
        FadeImage.color = finalColor;

    }
}
