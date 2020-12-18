using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Util;

public class TestNPC : TestEntity
{
    [SerializeField]
    private float DefaultHP;

    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform TargetWaypointProbe;

    private NavMeshPath path;


    public Notifier<Vector3> ProbePosition;

    private void Awake()
    {
        path = new NavMeshPath();
        ProbePosition = new Notifier<Vector3>();
        ProbePosition.OnDataChanged += ProbePosition_OnDataChanged;
        HP.CurrentData = DefaultHP;
    }

    private void ProbePosition_OnDataChanged(Vector3 obj)
    {
        if (Agent.CalculatePath(obj.ToXZ().ToVector3FromXZ(), path))
        {
            Agent.SetPath(path);
        }
    }

    private void Update()
    {
        ProbePosition.CurrentData = TargetWaypointProbe.position;

        Animation();

        void Animation()
        {
            animator.SetBool("Run", Agent.velocity.magnitude > 0.1f);
        }
    }

    private void OnDestroy()
    {
        ProbePosition.OnDataChanged -= ProbePosition_OnDataChanged;
    }
}