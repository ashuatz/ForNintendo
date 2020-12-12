using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class TestBuildPreview : MonoBehaviour
{
    [SerializeField]
    private TestPlayer player;

    [SerializeField]
    private Transform PreviewRoot;

    [SerializeField]
    private List<GameObject> BuildObjects;

    private void Awake()
    {
        player.BuildIndex.OnDataChanged += BuildIndex_OnDataChanged;
        InputManager.Instance.MouseWorldXZ.OnDataChanged += MouseWorldXZ_OnDataChanged;

    }

    private void MouseWorldXZ_OnDataChanged(Vector2 xz)
    {
        if (player.BuildIndex.CurrentData == 0)
            return;

        PreviewRoot.position = xz.ToVector3FromXZ().Round(1);
    }

    private void BuildIndex_OnDataChanged(int obj)
    {
        for (int i = 0; i < BuildObjects.Count; ++i)
        {
            BuildObjects[i].SetActive(i == (obj - 1));
        }
    }

    private void OnDestroy()
    {
        player.BuildIndex.OnDataChanged -= BuildIndex_OnDataChanged;
        InputManager.Instance.MouseWorldXZ.OnDataChanged -= MouseWorldXZ_OnDataChanged;
    }

}
