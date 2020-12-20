using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Util;

using static BuildResourceManager;

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

    [SerializeField]
    private float ActionDistance = 20f;

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

    [SerializeField]
    private Transform ProbeRoot;

    [SerializeField]
    private LineRenderer Bullet;
    [SerializeField]
    private Gradient DefaultBulletGradient;

    [SerializeField]
    private ParticleSystem ProbeMuzzleEffect;



    private CoroutineWrapper probeRotationWrapper;

    private Notifier<bool> OnclickShot = new Notifier<bool>();

    public TestMinion GetCurrentMinion { get => Minions.Find(minion => minion.isOrderAvailable); }

    public Notifier<int> BuildIndex = new Notifier<int>();


    public event Action OnShot;
    private float ClickTime = 0f;

    private NavMeshPath path;

    private CoroutineWrapper BulletWrapper;
    private CoroutineWrapper attackDelayWrapper;
    private CoroutineWrapper markerRoutine;

    private void Start()
    {
        Bullet.gameObject.SetActive(false);
        path = new NavMeshPath();
        DataContainer.Instance.Player.CurrentData = this;

        foreach (var minion in Minions)
        {
            minion.OnDead += Minion_OnDead;
        }

        probeAnimator.SetFloat("AttackSpeed", 12f / (30 * AttackDelay));

        OnclickShot.OnDataChanged += OnclickShot_OnDataChanged;

        HP.CurrentData = DefaultHP;
        animator.speed = 0.85f;
        probeAnimator.speed = 0.85f;

        BulletWrapper = CoroutineWrapper.Generate(this);
        attackDelayWrapper = CoroutineWrapper.Generate(this);
        markerRoutine = CoroutineWrapper.Generate(this);
        probeRotationWrapper = CoroutineWrapper.Generate(this);
    }

    protected override void Dead()
    {
        base.Dead();
        GlobalFadeCanvas.Instance.On(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
    }

    private void Minion_OnDead(TestEntity obj)
    {
        var minion = obj as TestMinion;
        Minions.Remove(minion);
        minion.gameObject.SetActive(false);

        DeadEffectManager.Instance.PlayMinion(obj, Vector3.one * 2f);
    }

    private void OnclickShot_OnDataChanged(bool isShot)
    {
        probeAnimator.SetBool("Click", isShot);

        if (isShot)
        {
            probeAnimator.SetTrigger("Attack_Ready");
            ClickTime = Time.time;

            probeRotationWrapper.Stop();
        }
        else
        {
            probeRotationWrapper.StartSingleton(resetRotation((22f / 30 + AttackDelay), 0.2f));
            //ProbeRoot.localRotation = Quaternion.Euler(0, 90, 0);

            IEnumerator resetRotation(float firstDelay, float runtime)
            {
                float t = 0;
                yield return YieldInstructionCache.WaitForSeconds(firstDelay);

                Quaternion defaultLotation = ProbeRoot.localRotation;
                while (t < runtime)
                {
                    ProbeRoot.localRotation = Quaternion.Lerp(defaultLotation, Quaternion.Euler(0, 90, 0), t / runtime);
                    t += Time.deltaTime;
                    yield return null;
                }

                ProbeRoot.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }
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
        if (Input.GetKeyDown(KeyCode.Q) && _BuildResourceManager.IsHaveTower(0))
        {
            //build Test
            BuildIndex.CurrentData = 1;
        }
        if (Input.GetKeyDown(KeyCode.W) && _BuildResourceManager.IsHaveTower(1))
        {
            BuildIndex.CurrentData = 2;
        }
        if (Input.GetKeyDown(KeyCode.E) && _BuildResourceManager.IsHaveTower(2))
        {
            BuildIndex.CurrentData = 3;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            BuildIndex.CurrentData = -1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Agent.isStopped = true;
        }

        bool isOverrideKeyForTest = false;
        Vector3 overrideMoveForTest = Vector3.zero;
        //if(Input.GetKey(KeyCode.W))
        //{
        //    isOverrideKey = true;
        //    overrideMove = Vector2.up.ToVector3FromXZ() * 3;

        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    isOverrideKey = true;
        //    overrideMove = Vector2.left.ToVector3FromXZ() * 3;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    isOverrideKey = true;
        //    overrideMove = Vector2.right.ToVector3FromXZ() * 3;

        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    isOverrideKey = true;

        //    overrideMove = Vector2.down.ToVector3FromXZ() * 3;
        //}


        //우클릭 이동
        if (Input.GetMouseButton(1) || isOverrideKeyForTest)
        {
            //InputManager.Instance.MouseWorldPosition.OnDataChangedOnce += OnDataChanged;
            Vector3 position;

            if (isOverrideKeyForTest)
            {
                position = transform.position.ToXZ().ToVector3FromXZ() + overrideMoveForTest;
            }
            else
            {
                position = InputManager.Instance.MouseWorldPosition.CurrentData;
            }


            if (Agent.CalculatePath(position, path))
            {
                Agent.SetPath(path);
                Agent.isStopped = false;
            }


            //Agent.SetDestination(pos);
            //Agent.isStopped = false;

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

                if (Vector2.Distance(position, transform.position.ToXZ()) > ActionDistance)
                {
                    BuildIndex.CurrentData = 0;
                    return;
                }

                if (BuildIndex.CurrentData < 0)
                {
                    OnclickShot.CurrentData = false;

                    if (Viewer.TryGetStructure(position, out var structure))
                    {
                        var dir = (structure.transform.position.ToXZ() - transform.position.ToXZ()).normalized;
                        var info = new HitInfo();
                        info.Amount = 2020;
                        info.Origin = this;
                        info.Destination = structure;
                        info.hitDir = dir.ToVector3FromXZ();

                        structure.TakeDamage(info);
                    }
                }
                else
                {
                    OnclickShot.CurrentData = false;

                    if (Viewer.CheckBuildAllow(BuildIndex.CurrentData, position))
                    {
                        var currentMinion = GetCurrentMinion;
                        if (currentMinion != null)
                        {
                            currentMinion.OnBuildOnce += WorldData.Instance.AddStructure;
                            currentMinion.Build(BuildIndex.CurrentData, position, 1f + BuildIndex.CurrentData);
                            _BuildResourceManager.UseTower(BuildIndex.CurrentData - 1);
                            BuildIndex.CurrentData = 0;
                        }
                    }
                    else
                    {
                        //Not allowed
                    }
                }
            }
            else
            {
                OnclickShot.CurrentData = true;

                var position = InputManager.Instance.MouseWorldXZ.CurrentData;
                var dir = position - transform.position.ToXZ();

                ProbeRoot.rotation = Quaternion.Euler(Quaternion.LookRotation(dir.ToVector3FromXZ().normalized).eulerAngles + Quaternion.Euler(0, 90, 0).eulerAngles);

                if (LastAttackTime + AttackDelay < Time.time)
                {
                    LastAttackTime = Time.time;

                    var hits = Physics.RaycastAll(transform.position.ToXZ().ToVector3FromXZ(), dir.ToVector3FromXZ().normalized, 50, 1 << LayerMask.NameToLayer("Default"));
                    var entities = hits
                        .Select(new Func<RaycastHit, TestEntity>(hit => hit.transform.GetComponent<TestEntity>()))
                        .Where(entity => entity != null && entity.Type == EntityType.Enemy && entity.HP.CurrentData > 0).ToList();

                    if (entities != null && entities.Count > 0)
                    {
                        var target = entities.First();

                        if (Vector2.Distance(target.transform.position.ToXZ(), transform.position.ToXZ()) > ActionDistance)
                            return;

                        var info = new HitInfo();
                        info.Amount = Damage;
                        info.Origin = this;
                        info.Destination = target;
                        info.hitDir = dir.ToVector3FromXZ();

                        if (ClickTime == Time.time)
                        {
                            attackDelayWrapper.Start(SimpleDelay(0.733f, target, info));
                        }
                        else
                        {
                            target.TakeDamage(info);
                            ProbeMuzzleEffect.Play();
                            BulletWrapper.StartSingleton(BulletEffect(0.1f, info));
                        }
                    }

                    OnShot?.Invoke();
                    if (ClickTime + 0.733f < Time.time)
                    {
                        probeAnimator.SetTrigger("Attack_Trigger");
                    }
                }

            }
        }
        else
        {
            OnclickShot.CurrentData = false;
        }

        IEnumerator MarkerOff()
        {
            yield return new WaitForSeconds(0.5f);
            Marker.SetActive(false);
        }

        //local Function
        IEnumerator SimpleDelay(float delay, TestEntity target, HitInfo info)
        {
            yield return YieldInstructionCache.WaitForSeconds(delay);
            probeAnimator.SetTrigger("Attack_Trigger");

            yield return YieldInstructionCache.WaitForSeconds(AttackDelay);
            target.TakeDamage(info);

            ProbeMuzzleEffect.Play();

            BulletWrapper.StartSingleton(BulletEffect(0.1f, info));
        }

        IEnumerator BulletEffect(float moveDelay, HitInfo info)
        {
            Bullet.positionCount = 2;
            Bullet.SetPosition(0, ProbeMuzzleEffect.transform.position);
            Bullet.SetPosition(1, info.Destination.transform.position);

            Bullet.gameObject.SetActive(true);

            float t = 0;
            while (t < moveDelay)
            {
                var gradient = Bullet.colorGradient;
                var keys = DefaultBulletGradient.alphaKeys.Select(new Func<GradientAlphaKey, GradientAlphaKey>((key) => new GradientAlphaKey(key.alpha * t / moveDelay, key.time))).ToArray();
                gradient.SetKeys(gradient.colorKeys, keys);
                t += Time.deltaTime;
                yield return null;
            }

            Bullet.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        OnclickShot.OnDataChanged -= OnclickShot_OnDataChanged;
    }
}