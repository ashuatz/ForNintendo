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

    [SerializeField]
    private List<Collider> BuildTester;

    private void Awake()
    {
        player.BuildIndex.OnDataChanged += BuildIndex_OnDataChanged;
        InputManager.Instance.MouseWorldXZ.OnDataChanged += MouseWorldXZ_OnDataChanged;
    }

    public bool CheckBuildAllow(int index, Vector2 position)
    {
        var target = BuildTester[index - 1];
        target.transform.position = position.ToVector3FromXZ().Round(1);
        target.gameObject.SetActive(true);

        var allowed = !WorldData.Instance.IsExist(target);
        target.gameObject.SetActive(false);

        return allowed;
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

        if (!InputManager.ApplicationIsQuitting)
            InputManager.Instance.MouseWorldXZ.OnDataChanged -= MouseWorldXZ_OnDataChanged;
    }

}
