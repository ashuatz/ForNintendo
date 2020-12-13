using UnityEngine;
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

    public Notifier<float> HP = new Notifier<float>();

    public event Action<float> OnHit;
    public event Action OnDead;

    protected virtual void TakeDamage(float Amount)
    {
        HP.CurrentData -= Amount;
        OnHit?.Invoke(Amount);

        if (HP.CurrentData < 0)
        {
            Dead();
            OnDead?.Invoke();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    protected virtual void Dead()
    {

    }
}
