using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistWaypointMarker : MonoBehaviour
{
    [SerializeField]
    private int index;

    private void Awake()
    {
        if (DataContainer.Instance.ProgressBarUI.CurrentData == null)
        {
            DataContainer.Instance.ProgressBarUI.OnDataChangedOnce += ProgressBarUI_OnDataChangedOnce;
        }
        else
        {
            ProgressBarUI_OnDataChangedOnce(DataContainer.Instance.ProgressBarUI.CurrentData);
        }
    }

    private void ProgressBarUI_OnDataChangedOnce(ProgressiveBarUI obj)
    {
        obj.DefenseSectors[index] = transform;
    }
}
