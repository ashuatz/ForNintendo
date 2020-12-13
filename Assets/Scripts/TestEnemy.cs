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
    private float DefaultHP;

    [SerializeField]
    private float AttackRange;

    [SerializeField]
    private Transform PlayerTransformForTest;

    private CoroutineWrapper HitWrapper;

    public NotifierClass<Transform> AttackTarget = new NotifierClass<Transform>();

    private void Awake()
    {
        base.OnHit += TestEnemy_OnHit;

        HitWrapper = CoroutineWrapper.Generate(this);

        HP.CurrentData = DefaultHP;

        AttackTarget.CurrentData = PlayerTransformForTest;

        AttackTarget.OnDataChanged += AttackTarget_OnDataChanged;
    }

    private void TestEnemy_OnHit(HitInfo info)
    {
        HitWrapper.StartSingleton(HitEffect(0.2f));

        IEnumerator HitEffect(float runtime)
        {
            Agent.velocity = Vector3.zero;
            float t = 0;
            while (t < runtime)
            {
                t += Time.deltaTime;
                Agent.velocity *= t / runtime;

                yield return null;
            }
        }
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
