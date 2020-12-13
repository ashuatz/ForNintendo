using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Util;


public class TestEnemy : TestEntity
{
    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private float AttackRange;

    [SerializeField]
    private Transform PlayerTransformForTest;

    public NotifierClass<Transform> AttackTarget = new NotifierClass<Transform>();

    private void Awake()
    {
        AttackTarget.CurrentData = PlayerTransformForTest;

        AttackTarget.OnDataChanged += AttackTarget_OnDataChanged;
    }

    public void Initialize(Transform target)
    {
        AttackTarget.CurrentData = target;
    }

    private void AttackTarget_OnDataChanged(Transform obj)
    {
        Agent.SetDestination(obj.position);
    }

    private void Update()
    {
        if (AttackTarget.CurrentData != null)
        {
            Agent.SetDestination(AttackTarget.CurrentData.position);
        }
    }

    private void OnDestroy()
    {
        AttackTarget.OnDataChanged -= AttackTarget_OnDataChanged;
    }

}
