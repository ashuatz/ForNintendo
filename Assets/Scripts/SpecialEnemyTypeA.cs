using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Util;


public class SpecialEnemyTypeA : TestEntity
{
    /*
     * 미니언 - 안함
     * NPC 안함
     * X || Z == 1 케이스
     */

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
                case TestNPC n: return 1;
                case TestStructure s: return 0;
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
    private EnemyType myEnemyType = EnemyType.SpecialA;

    [SerializeField]
    private EnemyData data;

    private EnemyData.Enemy currentData;


    [SerializeField]
    private Transform PlayerTransformForTest;

    [SerializeField]
    private CollisionEventRiser Detector;

    [Header("Special Attack Field")]

    private bool useRush = false;

    [SerializeField]
    private float RushTime;

    [SerializeField]
    private float RushDamage;

    [SerializeField]
    private CollisionEventRiser Searcher;

    [SerializeField]
    private float RushSpeed;
    private float DefaultSpeed;

    private Vector3 TargetPosition;

    private bool IsMoving;

    public EnemyType MyEnemyType { get => myEnemyType; }

    private CoroutineWrapper HitWrapper;

    private CoroutineWrapper RushWrapper;

    private Transform FirstTarget;

    private List<TestEntity> Targets = new List<TestEntity>();

    private EntityComparer comparer = new EntityComparer();

    public NotifierClass<Transform> AttackTarget = new NotifierClass<Transform>();

    private void Awake()
    {
        IsMoving = false;

        DefaultSpeed = Agent.speed;

        currentData = data.Enemies.Find(e => e.MyType == MyEnemyType);

        base.OnHit += TestEnemy_OnHit;

        HitWrapper = CoroutineWrapper.Generate(this);
        RushWrapper = CoroutineWrapper.Generate(this);

        HP.CurrentData = currentData.DefaultHP;

        AttackTarget.CurrentData = PlayerTransformForTest;

        AttackTarget.OnDataChanged += AttackTarget_OnDataChanged;

        Searcher.OnTriggerEnterEvent += OnSearchTriggerEnter;
        Searcher.OnTriggerStayEvent += Searcher_OnTriggerStayEvent;
        Searcher.OnTriggerExitEvent += OnSearchTriggerExit;

        Detector.OnTriggerEnterEvent += OnTriggerEnterListener;
        Detector.OnTriggerExitEvent += OnTriggerExitListener;
    }

    public void Initialize(Transform target)
    {
        useRush = false;
        FirstTarget = target;
        HP.CurrentData = currentData.DefaultHP;
        AttackTarget.CurrentData = target;
        Agent.isStopped = false;
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


    private void AttackTarget_OnDataChanged(Transform obj)
    {
        if (IsMoving)
            return;

        if (obj == null)
            return;

        Agent.SetDestination(obj.position);
    }

    private void Searcher_OnTriggerStayEvent(Collider other)
    {
        OnSearchTriggerEnter(other);
    }


    private void OnSearchTriggerEnter(Collider other)
    {
        if (useRush)
            return;

        if (IsMoving)
            return;

        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (entity.Type == EntityType.Player)
            {
                if (entity is TestNPC || entity is TestMinion)
                    return;

                var xEqual = entity.transform.position.Round(1).x == transform.position.Round(1).x;
                var zEqual = entity.transform.position.Round(1).z == transform.position.Round(1).z;
                if (!(xEqual ^ zEqual))
                    return;


                TargetPosition = entity.transform.position.ToXZ().ToVector3FromXZ();

                IsMoving = true;

                RushWrapper.StartSingleton(Rush());
            }
        }
    }

    private IEnumerator Rush()
    {
        useRush = true;

        Agent.speed = RushSpeed;
        //Agent.velocity = (TargetPosition.ToXZ() - transform.position.ToXZ()).ToVector3FromXZ().normalized * RushSpeed;
        Agent.SetDestination(TargetPosition);

        Animator.SetBool("Lock_ON", true);

        yield return this.WaitforTimeWhileCondition(RushTime,
            (/*condition*/) => Vector2.Distance(transform.position.Round(1).ToXZ(), TargetPosition.Round(1).ToXZ()) > 0.5f,
            (isHit) =>
            {
                Debug.Log("On End");
                IsMoving = false;
                Animator.SetBool("Lock_ON", false);
                Agent.speed = DefaultSpeed;
            });

    }



    private void OnSearchTriggerExit(Collider other)
    {
        if (IsMoving)
            return;
    }


    private void OnTriggerEnterListener(Collider other)
    {
        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (IsMoving)
            {
                var info = new HitInfo();
                info.Amount = RushDamage;
                info.Origin = this;
                info.Destination = entity;
                info.hitDir = Agent.velocity.ToXZ().ToVector3FromXZ();

                entity.TakeDamage(info);
            }

            if (entity.Type == EntityType.Player)
            {

                Targets.Add(entity);
            }
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
            if (!AttackTarget.CurrentData.gameObject.activeInHierarchy && FirstTarget != null)
                AttackTarget.CurrentData = FirstTarget;

            Agent.SetDestination(AttackTarget.CurrentData.position);
        }
    }

    private void OnDestroy()
    {
        AttackTarget.OnDataChanged -= AttackTarget_OnDataChanged;


        Searcher.OnTriggerEnterEvent -= OnSearchTriggerEnter;
        Searcher.OnTriggerExitEvent -= OnSearchTriggerExit;

        Detector.OnTriggerEnterEvent -= OnTriggerEnterListener;
        Detector.OnTriggerExitEvent -= OnTriggerExitListener;
    }
}
