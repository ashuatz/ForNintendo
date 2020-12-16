using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class TestPlayer : TestEntity
{


    [SerializeField]
    private float DefaultHP;

    [Header("Attak Property")]
    [SerializeField]
    private float Damage;
    [SerializeField]
    private float AttackDelay;
    private float LastAttackTime;

    [Header("Pre-defined Property")]
    [SerializeField]
    private TestBuildPreview Viewer;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator probeAnimator;

    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private GameObject Marker;

    [SerializeField]
    private List<TestMinion> Minions;

    public TestMinion GetCurrentMinion { get => Minions.Find(minion => minion.isOrderAvailable); }

    public Notifier<int> BuildIndex = new Notifier<int>();

    public event Action OnShot;



    private CoroutineWrapper markerRoutine;
    private float time = 0;

    private void Start()
    {
        HP.CurrentData = DefaultHP;
        animator.speed = 0.85f;
        markerRoutine = CoroutineWrapper.Generate(this);

        //GetComponent<NavMeshAgent>().SetDestination(Target.position);
        time = Time.time;
    }

    private bool TryGetWorldPosition(Vector3 MousePosition, out Vector2 WorldXZPosition)
    {
        WorldXZPosition = Vector2.zero;

        var ray = Camera.main.ScreenPointToRay(MousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            WorldXZPosition = hit.point.ToXZ();
            return true;
        }

        return false;
    }

    void Update()
    {
        PlayerInput();
        Animation();

        void Animation()
        {
            animator.SetBool("Run", Agent.velocity.magnitude > 0.1f);
            probeAnimator.SetBool("Run", Agent.velocity.magnitude > 0.1f);
        }
    }


    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //build Test
            BuildIndex.CurrentData = 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            BuildIndex.CurrentData = 2;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            BuildIndex.CurrentData = 3;
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            Agent.isStopped = true;
        }

        //우클릭 이동
        if (Input.GetMouseButton(1))
        {
            var position = InputManager.Instance.MouseWorldPosition.CurrentData;

            Agent.ResetPath();
            
            Agent.SetDestination(position);
            Agent.isStopped = false;

            Marker.transform.position = position;
            Marker.SetActive(true);

            markerRoutine.StartSingleton(MarkerOff());


            if (BuildIndex.CurrentData != 0) BuildIndex.CurrentData = 0;

        }

        if (Input.GetMouseButton(0))
        {
            if (BuildIndex.CurrentData != 0)
            {
                var position = InputManager.Instance.MouseWorldXZ.CurrentData;
                if (Viewer.CheckBuildAllow(BuildIndex.CurrentData, position))
                {
                    var currentMinion = GetCurrentMinion;
                    if (currentMinion != null)
                    {
                        currentMinion.OnBuildOnce += WorldData.Instance.AddStructure;
                        currentMinion.Build(BuildIndex.CurrentData, position, 1f + BuildIndex.CurrentData);
                        BuildIndex.CurrentData = 0;
                    }
                }
                else
                {
                    //Not allowed
                }
            }
            else
            {
                if (LastAttackTime + AttackDelay < Time.time)
                {
                    LastAttackTime = Time.time;

                    var position = InputManager.Instance.MouseWorldXZ.CurrentData;

                    var dir = position - transform.position.ToXZ();

                    var hits = Physics.RaycastAll(transform.position.ToXZ().ToVector3FromXZ(), dir.ToVector3FromXZ().normalized, 50, 1 << LayerMask.NameToLayer("Default"));
                    var entities = hits
                        .Select(new Func<RaycastHit, TestEntity>(hit => hit.transform.GetComponent<TestEntity>()))
                        .Where(entity => entity != null && entity.Type == EntityType.Enemy).ToList();

                    if (entities != null && entities.Count > 0)
                    {
                        var target = entities.First();
                        var info = new HitInfo();
                        info.Amount = Damage;
                        info.Origin = this;
                        info.Destination = target;
                        info.hitDir = dir.ToVector3FromXZ();

                        target.TakeDamage(info);
                    }

                    OnShot?.Invoke();
                }

            }
        }

        IEnumerator MarkerOff()
        {
            yield return new WaitForSeconds(0.5f);
            Marker.SetActive(false);
        }
    }
}
