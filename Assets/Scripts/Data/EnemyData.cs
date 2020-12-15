using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "EnemyData", order = 0)]
#endif

public class EnemyData : ScriptableObject
{
    [Serializable]
    public class Enemy
    {
        [SerializeField]
        private EnemyType myType;

        [SerializeField]
        private float defaultHP;

        [SerializeField]
        private float attackRange;
        [SerializeField]
        private float attackDamage;
        [SerializeField]
        private float attackPerSecond;

        public EnemyType MyType { get => myType; }
        public float DefaultHP { get => defaultHP; }
        public float AttackRange { get => attackRange; }
        public float AttackDamage { get => attackDamage; }
        public float AttackPerSecond { get => attackPerSecond; }
    }

    public List<Enemy> Enemies;
}

public enum EnemyType
{
    Normal,
    SpecialA,
    SpecialB,
}