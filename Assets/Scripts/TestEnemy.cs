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
    private EnemyType myEnemyType;

    [SerializeField]
    private EnemyData data;

    private EnemyData.Enemy currentData;

    [SerializeField]
    private Transform PlayerTransformForTest;

    [SerializeField]
    private CollisionEventRiser Detector;

    public EnemyType MyEnemyType { get => myEnemyType; }

    private CoroutineWrapper HitWrapper;
    private CoroutineWrapper InitWrapper;

    private Transform FirstTarget;

    private List<TestEntity> Targets = new List<TestEntity>();

    private EntityComparer comparer = new EntityComparer();

    public NotifierClass<Transform> AttackTarget = new NotifierClass<Transform>();

    private void Awake()
    {

        currentData = data.Enemies.Find(e => e.MyType == MyEnemyType);

        base.OnHit += TestEnemy_OnHit;

        HitWrapper = CoroutineWrapper.Generate(this);
        InitWrapper = CoroutineWrapper.Generate(this);

        HP.CurrentData = currentData.DefaultHP;

        AttackTarget.CurrentData = PlayerTransformForTest;

        AttackTarget.OnDataChanged += AttackTarget_OnDataChanged;

        Detector.OnTriggerEnterEvent += OnTriggerEnterListener;
        Detector.OnTriggerExitEvent += OnTriggerExitListener;
    }
    public void Initialize(Transform target)
    {
        FirstTarget = target;
        HP.CurrentData = currentData.DefaultHP;
        AttackTarget.CurrentData = target;
        Targets.Clear();

        InitWrapper.StartSingleton(AgentInit());

        switch (MyEnemyType)
        {
            case EnemyType.Normal:
                Animator.Play("Zombi_Run");
                break;

            case EnemyType.SpecialB:
                Animator.Play("Zombi_Crawling");
                break;

        }

        IEnumerator AgentInit()
        {
            yield return new WaitUntil(() => Agent.isOnNavMesh);
            Agent.isStopped = false;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(AttackRoutine());
    }

    protected override void Dead()
    {
        base.Dead();

        Agent.isStopped = true;

        Animator.SetTrigger("Die");

        StartCoroutine(DeleayRelease());

        IEnumerator DeleayRelease()
        {
            yield return YieldInstructionCache.WaitForSeconds(1.5f);

            gameObject.SetActive(false);
        }
    }

    private IEnumerator AttackRoutine()
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
                info.Amount = currentData.AttackDamage;
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

            yield return YieldInstructionCache.WaitForSeconds(1 / currentData.AttackPerSecond);
        }
    }

    private void TestEnemy_OnHit(HitInfo info)
    {
        if (HP.CurrentData <= 0)
            return;

        HitWrapper.StartSingleton(HitEffect(0.2f));

        if (AttackTarget.CurrentData == null)
        {
            AttackTarget.CurrentData = info.Origin.transform;
        }

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


    private void AttackTarget_OnDataChanged(Transform obj)
    {
        if (obj == null)
            return;

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

            Animator.SetBool("IsMoving", true);
        }
    }

    private void OnDestroy()
    {
        AttackTarget.OnDataChanged -= AttackTarget_OnDataChanged;

        Detector.OnTriggerEnterEvent -= OnTriggerEnterListener;
        Detector.OnTriggerExitEvent -= OnTriggerExitListener;
    }

}
