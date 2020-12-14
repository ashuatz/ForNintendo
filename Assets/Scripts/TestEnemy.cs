using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Util;


public class TestEnemy : TestEntity
{

    public class EntityComparer : IComparer<TestEntity>
    {
        public int Compare(TestEntity x, TestEntity y)
        {
            return GetValueByType(x).CompareTo(GetValueByType(y));
        }

        private int GetValueByType(TestEntity entity)
        {
            switch (entity)
            {
                case TestNPC n: return 0;
                case TestStructure s: return 1;
                case TestPlayer p: return 2;
                case TestMinion m: return 3;

                default: return 5;
            }
        }
    }

    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private Animator Animator;

    [SerializeField]
    private int myType;

    [SerializeField]
    private float DefaultHP;

    [SerializeField]
    private float AttackRange;
    [SerializeField]
    private float AttackDamage;
    [SerializeField]
    private float AttackPerSecond;

    [SerializeField]
    private Transform PlayerTransformForTest;

    [SerializeField]
    private CollisionEventRiser Detector;

    public int MyEnemyType { get => myType; }

    private CoroutineWrapper HitWrapper;

    private Transform FirstTarget;

    private List<TestEntity> Targets = new List<TestEntity>();

    private EntityComparer comparer = new EntityComparer();

    public NotifierClass<Transform> AttackTarget = new NotifierClass<Transform>();

    private void Awake()
    {
        base.OnHit += TestEnemy_OnHit;

        HitWrapper = CoroutineWrapper.Generate(this);

        HP.CurrentData = DefaultHP;

        AttackTarget.CurrentData = PlayerTransformForTest;

        AttackTarget.OnDataChanged += AttackTarget_OnDataChanged;

        Detector.OnTriggerEnterEvent += OnTriggerEnterListener;
        Detector.OnTriggerExitEvent += OnTriggerExitListener;
    }

    protected override void Dead()
    {
        base.Dead();
        gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        List<TestEntity> removeList = new List<TestEntity>();

        while (HP.CurrentData > 0)
        {
            Targets.Sort(comparer);
            foreach (var target in Targets)
            {
                if (!target.gameObject.activeInHierarchy)
                {
                    removeList.Add(target);
                    continue;
                }
                if (target.HP.CurrentData <= 0)
                {
                    removeList.Add(target);
                    continue;
                }

                var dir = (target.transform.position.ToXZ() - transform.position.ToXZ()).normalized;
                var info = new HitInfo();
                info.Amount = AttackDamage;
                info.Origin = this;
                info.Destination = target;
                info.hitDir = dir.ToVector3FromXZ();

                AttackTarget.CurrentData = info.Destination.transform;

                target.TakeDamage(info);
                break;
            }

            foreach (var removeItem in removeList)
            {
                Targets.Remove(removeItem);
            }

            yield return YieldInstructionCache.WaitForSeconds(1 / AttackPerSecond);
        }
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
        FirstTarget = target;
        HP.CurrentData = DefaultHP;
        AttackTarget.CurrentData = target;
    }

    private void AttackTarget_OnDataChanged(Transform obj)
    {
        Agent.SetDestination(obj.position);
    }

    private void OnTriggerEnterListener(Collider other)
    {

        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (entity.Type == EntityType.Player)
                Targets.Add(entity);
        }
    }

    private void OnTriggerExitListener(Collider other)
    {
        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (entity.Type == EntityType.Player)
                Targets.Remove(entity);
        }
    }

    private void Update()
    {
        if (AttackTarget.CurrentData != null)
        {
            if (!AttackTarget.CurrentData.gameObject.activeInHierarchy)
                AttackTarget.CurrentData = FirstTarget;

            Agent.SetDestination(AttackTarget.CurrentData.position);
        }
    }

    private void OnDestroy()
    {
        AttackTarget.OnDataChanged -= AttackTarget_OnDataChanged;

        Detector.OnTriggerEnterEvent -= OnTriggerEnterListener;
        Detector.OnTriggerExitEvent -= OnTriggerExitListener;
    }

}
