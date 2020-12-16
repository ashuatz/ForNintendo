using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WayPointEvent : MonoBehaviour
{
    [SerializeField]
    private WayPointData CurrentData;

    [SerializeField]
    private UnityEvent targetEvent;

    public bool isEnd { get; private set; }
    public bool isRun{ get; private set; }
    private void Awake()
    {
        isEnd = false;
        isRun = false;
    }

    public void Run()
    {
        if (isRun)
            return;

        isRun = true;
        StartCoroutine(RunInternal());
    }

    private IEnumerator RunInternal()
    {
        float t = 0;

        foreach (var message in CurrentData.Messages)
        {
            if (t < message.time)
                yield return new WaitForSeconds(message.time - t);
            t += message.time;

            ShowMessage(message.desc);

            yield return new WaitForSeconds(message.duration);
            t += message.duration;
        }

        targetEvent.Invoke();
        isEnd = true;
    }

    private void ShowMessage(string message)
    {
        Debug.Log("message : " + message);
    }
}