using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using UnityEngine.UI;

public class TextBox : MonoSingleton<TextBox>
{
    [SerializeField]
    private Text targetText;
    [SerializeField]
    private Transform root;

    private CoroutineWrapper textoutput;

    protected override void Awake()
    {
        base.Awake();
        textoutput = CoroutineWrapper.Generate(this);
    }

    public void Input(string text, float time)
    {
        targetText.text = string.Empty;
        root.gameObject.SetActive(true);
        textoutput.StartSingleton(run(time)).OnCompleteOnce += TextOff;

        IEnumerator run(float runtime)
        {
            float t = 0;
            while (t < runtime)
            {
                t += Time.deltaTime;

                if (text.Length > 0)
                {
                    targetText.text += text.Substring(0, 1);
                    text = text.Remove(0, 1);
                }

                yield return null;
            }
        }
    }

    public void TextOff()
    {
        root.gameObject.SetActive(false);
    }

}
