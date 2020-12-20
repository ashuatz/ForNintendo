using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSpace : MonoBehaviour
{
    [SerializeField] TimerUI _timer;
    [SerializeField] ProgressiveBarUI _progressiveUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TestPlayer>(out var entity) && _progressiveUI.ClearIndex == SpawnerParent._Sectors.Length - 1)
        {
            TimerUI.SetEnd();
            EndingManager._EndingManager.StartCoroutine(EndingManager._EndingManager.Ending());
        }
    }
}
