using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class DeadEffectManager : MonoSingleton<DeadEffectManager>
{
    [SerializeField]
    private ParticleSystem Origin;

    private List<ParticleSystem> Pool = new List<ParticleSystem>();

    public void Play(TestEntity entity, in Vector3 scale)
    {
        var instance = Pool.Find(particle => !particle.isPlaying);
        if (instance == null)
            instance = Instantiate(Origin);

        instance.transform.position = entity.transform.position;
        instance.transform.localScale = scale;
        instance.Play();
    }
}
