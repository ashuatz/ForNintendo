using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHitEffect : MonoBehaviour
{
    [SerializeField]
    private TestEntity entitiy;

    [SerializeField]
    private ParticleSystem HitEffect;
    
    void Start()
    {
        entitiy.OnHit += Entitiy_OnHit;
    }

    private void Entitiy_OnHit(HitInfo obj)
    {
        HitEffect.transform.LookAt(transform.position - obj.hitDir);
        HitEffect.Play();
    }

    private void OnDestroy()
    {
        entitiy.OnHit -= Entitiy_OnHit;
    }
}
