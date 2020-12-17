﻿using UnityEngine;
using System;
using System.Collections;

using Util;


public class TestEntity : MonoBehaviour
{
    public enum EntityType
    {
        None,
        Player,
        Enemy,

    }

    [SerializeField]
    protected EntityType MyType;
    public EntityType Type { get => MyType; }
    public Notifier<float> HP = new Notifier<float>();

    public event Action<HitInfo> OnHit;
    public event Action<TestEntity> OnDead;

    private bool isDead = false;

    public virtual void TakeDamage(HitInfo info)
    {
        HP.CurrentData -= info.Amount;
        OnHit?.Invoke(info);

        if (HP.CurrentData <= 0)
        {
            if (isDead)
                return;

            isDead = true;

            Dead();
            OnDead?.Invoke(this);
        }
        else
        {
            isDead = false;
        }
    }

    protected virtual void Dead()
    {

    }
}
