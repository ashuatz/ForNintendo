using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Util;



public class TestStructure : TestEntity
{
    [Serializable]
    public class StructureData
    {
        public StructureType Type;

        public float DefaultHP;

        public float AttackRange;
        public float AttackDamage;
        public float AttackPerSecond;
    }

    public enum StructureType
    {
        A,
        B,
        Turret,
    }

    [SerializeField]
    private StructureType myType;

    [SerializeField]
    private SphereCollider Detector;

    [SerializeField]
    private List<StructureData> Datas;


    private StructureData CurrentData;

    private List<TestEntity> Targets = new List<TestEntity>();

    private void Awake()
    {
        CurrentData = Datas.Find(data => data.Type == myType);

        HP.CurrentData = CurrentData.DefaultHP;

        var riser = Detector.GetComponent<CollisionEventRiser>();
        riser.OnTriggerEnterEvent += OnTriggerEnterListener;
        riser.OnTriggerExitEvent += OnTriggerExitListener;

        Detector.radius = CurrentData.AttackRange;
        Detector.isTrigger = true;
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

                target.TakeDamage(info);
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
                Targets.Add(entity);
        }
    }

    private void OnDestroy()
    {
        var riser = Detector.GetComponent<CollisionEventRiser>();
        riser.OnTriggerEnterEvent -= OnTriggerEnterListener;
        riser.OnTriggerExitEvent -= OnTriggerExitListener;
    }
}