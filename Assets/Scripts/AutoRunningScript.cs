using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRunningScript : MonoBehaviour
{
    [SerializeField]
    private WayPointData data;

    private IEnumerator Start()
    {
        float t = 0;

        foreach (var message in data.Messages)
        {
            if (t < message.time)
                yield return new WaitForSeconds(message.time - t);
            t += message.time;

            ShowMessage(message.desc, message.duration);

            yield return new WaitForSeconds(message.duration);
            t += message.duration;
        }
    }

    private void ShowMessage(string message, in float time)
    {
        TextBox.Instance.Input(message, time);
    }
}
