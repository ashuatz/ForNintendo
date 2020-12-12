using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class TestMinion : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Transform PlayerProbe;

    private CoroutineWrapper BuildWrapper;

    [SerializeField]
    private List<GameObject> BuildObjects;

    public bool CC { get; private set; }
    public bool isBuilding { get; private set; }

    public bool isOrderAvailable { get => !CC && !isBuilding; }


    public NotifierClass<Transform> MoveTargetNotifier { get; private set; }

    private void Awake()
    {
        BuildWrapper = new CoroutineWrapper(this);
        MoveTargetNotifier = new NotifierClass<Transform>();
        MoveTargetNotifier.CurrentData = PlayerProbe;
        MoveTargetNotifier.OnDataChanged += MoveTargetNotifier_OnDataChanged;
    }

    private void Update()
    {
        if (MoveTargetNotifier.CurrentData != null)
        {
            Agent.SetDestination(MoveTargetNotifier.CurrentData.position);
        }
    }

    private void MoveTargetNotifier_OnDataChanged(Transform obj)
    {
        Agent.SetDestination(obj.position);
    }

    public void Build(Vector2 position, float time)
    {
        BuildWrapper.Start(BuildTarget(position, time));
    }

    IEnumerator BuildTarget(Vector2 position, float time)
    {
        var obj = Instantiate(BuildObjects[0]);
        obj.transform.position = position.ToVector3FromXZ().Round(1);
        obj.SetActive(false);
        MoveTargetNotifier.CurrentData = obj.transform;
        isBuilding = true;

        yield return new WaitForSeconds(time);
        MoveTargetNotifier.CurrentData = PlayerProbe;
        obj.SetActive(true);
        isBuilding = false;
    }

}
