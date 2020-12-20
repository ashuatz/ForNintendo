using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class DeadEffectManager : MonoSingleton<DeadEffectManager>
{
    [SerializeField]
    private ParticleSystem StructureOrigin;


    private List<ParticleSystem> StructurePool = new List<ParticleSystem>();

    public void PlayStructure(TestEntity entity, in Vector3 scale)
    {
        var instance = StructurePool.Find(particle => !particle.isPlaying);
        if (instance == null)
            instance = Instantiate(StructureOrigin);

        instance.transform.position = entity.transform.position;
        instance.transform.localScale = scale;
        instance.Play();
    }

    [SerializeField]
    private ParticleSystem MinionOrigin;

    private List<ParticleSystem> MinionPool = new List<ParticleSystem>();

    public void PlayMinion(TestEntity entity, in Vector3 scale)
    {
        var instance = MinionPool.Find(particle => !particle.isPlaying);
        if (instance == null)
            instance = Instantiate(MinionOrigin);

        instance.transform.position = entity.transform.position;
        instance.transform.localScale = scale;
        instance.Play();
    }


    [SerializeField]
    private ParticleSystem NPCOrigin;

    private List<ParticleSystem> NPCPool = new List<ParticleSystem>();

    public void PlayNPC(TestEntity entity, in Vector3 scale)
    {
        var instance = NPCPool.Find(particle => !particle.isPlaying);
        if (instance == null)
            instance = Instantiate(NPCOrigin);

        instance.transform.position = entity.transform.position;
        instance.transform.localScale = scale;
        instance.Play();
    }
}
