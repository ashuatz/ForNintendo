using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Util;



public class TestStructure : TestEntity
{

    [SerializeField]
    private StructureType myType;

    [SerializeField]
    private SphereCollider Detector;

    [SerializeField]
    private Collider SpaceCollider;
    public Collider StructureCollider { get => SpaceCollider; }

    [SerializeField]
    private StructureData data;
    private List<StructureData.Structure> Datas { get => data.Structures; }


    private StructureData.Structure CurrentData;

    private List<TestEntity> Targets = new List<TestEntity>();

    private CoroutineWrapper RotationWrapper;

    [SerializeField]
    Animator _animator;
    

    private void Awake()
    {
        RotationWrapper = new CoroutineWrapper(this);
        CurrentData = Datas.Find(data => data.Type == myType);

        HP.CurrentData = CurrentData.DefaultHP;
        var riser = Detector.GetComponent<CollisionEventRiser>();
        riser.OnTriggerEnterEvent += OnTriggerEnterListener;
        riser.OnTriggerExitEvent += OnTriggerExitListener;

        Detector.radius = CurrentData.AttackRange;
        Detector.isTrigger = true;
    }

    protected override void Dead()
    {
        WorldData.Instance.RemoveStructure(this);
        enabled = false;
        gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        List<TestEntity> removeList = new List<TestEntity>();

        while (enabled)
        {
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
                info.Amount = CurrentData.AttackDamage;
                info.Origin = this;
                info.Destination = target;
                info.hitDir = dir.ToVector3FromXZ();

                var targetQuaternion = Quaternion.LookRotation(dir.ToVector3FromXZ()) * Quaternion.Euler(-90, 0, 0);
                var runtime = myType == StructureType.MeleeTurret ? 8f / 24 : 8f / 30;
                RotationWrapper.StartSingleton(RotateToTarget(targetQuaternion, runtime)).SetOnComplete(() =>
                {
                    target.TakeDamage(info);
                });

                _animator.SetTrigger("Atk");

                target.OnDead += OnTargetDead;

                //local Function
                IEnumerator RotateToTarget(Quaternion targetRotation, float _runtime)
                {
                    float t = 0;
                    var defaultRotation = _animator.transform.GetChild(1).rotation;

                    while (t < _runtime)
                    {
                        _animator.transform.GetChild(1).rotation = Quaternion.Lerp(defaultRotation, targetRotation, t / _runtime);
                        t += Time.deltaTime;
                        yield return null;
                    }

                    _animator.transform.GetChild(1).rotation = targetRotation;
                }

                void OnTargetDead(TestEntity t)
                {
                    Targets.Remove(t);
                    t.OnDead -= OnTargetDead;
                }
                break;
            }

            foreach (var removeItem in removeList)
            {
                Targets.Remove(removeItem);
            }

            removeList.Clear();

            yield return YieldInstructionCache.WaitForSeconds(1 / CurrentData.AttackPerSecond);
        }
    }

    private void OnTriggerEnterListener(Collider other)
    {
        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (entity.Type == EntityType.Enemy)
                Targets.Add(entity);
        }
    }


    private void OnTriggerExitListener(Collider other)
    {
        if (other.TryGetComponent<TestEntity>(out var entity))
        {
            if (entity.Type == EntityType.Enemy)
                Targets.Remove(entity);
        }
    }

    private void OnDestroy()
    {
        var riser = Detector.GetComponent<CollisionEventRiser>();
        riser.OnTriggerEnterEvent -= OnTriggerEnterListener;
        riser.OnTriggerExitEvent -= OnTriggerExitListener;
    }
}