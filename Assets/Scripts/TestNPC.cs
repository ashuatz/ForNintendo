using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class TestNPC : TestEntity
{
    [SerializeField]
    private float DefaultHP;

    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Transform TargetWaypointProbe;

    private void Awake()
    {
        HP.CurrentData = DefaultHP;
    }

    private void Update()
    {
        Agent.SetDestination(TargetWaypointProbe.position);
    }
}