using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util;

public class TestBuildPreview : MonoSingleton<TestBuildPreview>
{
    private TestPlayer player { get => DataContainer.Instance.Player.CurrentData; }

    [SerializeField]
    private Transform PreviewRoot;

    [SerializeField]
    private List<GameObject> BuildObjects;

    [SerializeField]
    private GameObject Remover;


    [SerializeField]
    private List<Collider> BuildTester;


    [SerializeField]
    private GameObject BuildPreviewOrigins;


    private List<GameObject> Pool = new List<GameObject>();

    public void AddToPool(GameObject instance)
    {
        for (int i = 0; i < 3; ++i) instance.transform.GetChild(i).gameObject.SetActive(false);
        instance.gameObject.SetActive(false);
        Pool.Add(instance);
    }

    public GameObject GetPreviewFormPool(int index)
    {
        var instance = Pool.Find((_instace) => !_instace.gameObject.activeInHierarchy);

        if (instance == null)
        {
            instance = Instantiate(BuildPreviewOrigins);
        }

        instance.transform.GetChild(index).gameObject.SetActive(true);
        Pool.Remove(instance);
        return instance;
    }



    protected override void Awake()
    {
        base.Awake();

        if (DataContainer.Instance.Player.CurrentData != null)
        {
            player.BuildIndex.OnDataChanged += BuildIndex_OnDataChanged;
        }
        else
        {
            DataContainer.Instance.Player.OnDataChangedOnce += player => player.BuildIndex.OnDataChanged += BuildIndex_OnDataChanged;
        }

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

    public bool TryGetStructure(Vector2 position,out TestStructure instance)
    {
        instance = null;

        var target = BuildTester[2];
        target.transform.position = position.ToVector3FromXZ().Round(1);
        target.gameObject.SetActive(true);

        if(WorldData.Instance.TryGetStructure(target,out var structure))
        {
            instance = structure;
            return true;
        }

        return false;


    }

    private void MouseWorldXZ_OnDataChanged(Vector2 xz)
    {
        if (player.BuildIndex.CurrentData == 0)
            return;

        PreviewRoot.position = xz.ToVector3FromXZ().Round(1);
    }

    private void BuildIndex_OnDataChanged(int obj)
    {
        Remover.SetActive(obj == -1);

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
