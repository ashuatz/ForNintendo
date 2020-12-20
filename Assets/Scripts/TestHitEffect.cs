using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHitEffect : MonoBehaviour
{
    [Serializable]
    public class HitEffectData
    {
        public TestEntity.EntityType AttackerEntityType;
        public StructureType AttackerStructureType;
        public EnemyType AttackerEnemyType;

        public ParticleSystem HitEffect;
        public AudioClip clip;
        public float clipVolume;
    }


    [SerializeField]
    private AudioSource source;

    [SerializeField]
    private TestEntity entitiy;

    [SerializeField]
    private List<HitEffectData> Effects;
    
    void Start()
    {
        entitiy.OnHit += Entitiy_OnHit;
    }

    private void Entitiy_OnHit(HitInfo obj)
    {
        //피격자가 적
        if(obj.Destination.Type == TestEntity.EntityType.Enemy)
        {
            switch (obj.Origin)
            {
                case TestPlayer player:
                {
                    var target = Effects.Find((data) => data.AttackerEntityType == TestEntity.EntityType.Player && data.AttackerStructureType == StructureType.NotStructure);
                    if (target == null)
                        return;

                    target.HitEffect.transform.LookAt(transform.position - obj.hitDir);
                    target.HitEffect.Play();

                    source.clip = target.clip;
                    source.volume = target.clipVolume;
                    source.Play();
                    break;
                }

                case TestStructure structure:
                {
                    var target = Effects.Find((data) => data.AttackerEntityType == TestEntity.EntityType.Player && data.AttackerStructureType == structure.MyStructureType);
                    if (target == null)
                        return;

                    target.HitEffect.transform.LookAt(transform.position - obj.hitDir);
                    target.HitEffect.Play();

                    source.clip = target.clip;
                    source.volume = target.clipVolume;
                    source.Play();
                    break;
                }
            }
        }

        //피격자가 아군
        else if (obj.Destination.Type == TestEntity.EntityType.Player)
        {
            switch (obj.Origin)
            {
                case TestEnemy enemy:
                {
                    var target = Effects.Find((data) => data.AttackerEntityType == TestEntity.EntityType.Enemy && data.AttackerEnemyType == enemy.MyEnemyType);
                    if (target == null)
                        return;

                    target.HitEffect.transform.LookAt(transform.position - obj.hitDir);
                    target.HitEffect.Play();

                    source.clip = target.clip;
                    source.volume = target.clipVolume;
                    source.Play();
                    break;
                }

                case TestPlayer player:
                {
                    var target = Effects.Find((data) => data.AttackerEntityType == TestEntity.EntityType.Player && data.AttackerStructureType == StructureType.NotStructure);
                    if (target == null)
                        return;

                    target.HitEffect.transform.LookAt(transform.position - obj.hitDir);
                    target.HitEffect.Play();

                    source.clip = target.clip;
                    source.volume = target.clipVolume;
                    source.Play();
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        entitiy.OnHit -= Entitiy_OnHit;
    }
}
