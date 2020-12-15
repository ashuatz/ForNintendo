using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "NewStructureData", menuName = "StructureData", order = 1)]
#endif

public class StructureData : ScriptableObject
{
    [Serializable]
    public class Structure
    {

        [SerializeField] private StructureType type;

        [SerializeField] private float defaultHP;

        [SerializeField] private float attackRange;
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackPerSecond;


        public StructureType Type { get => type; }
        public float DefaultHP { get => defaultHP; }
        public float AttackRange { get => attackRange; }
        public float AttackDamage { get => attackDamage; }
        public float AttackPerSecond { get => attackPerSecond; }
    }

    public List<Structure> Structures = new List<Structure>();
}
