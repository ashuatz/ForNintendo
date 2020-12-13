using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Util;

public class WayPoint : MonoBehaviour
{
    [Serializable]
    public class WayPair
    {
        public WayPoint TargetPoint;
        public CollisionEventRiser Riser;
    }

    [SerializeField]
    private Transform Probe;

    [SerializeField]
    private Transform NPC;

    [SerializeField]
    private List<WayPair> Targets;

    [SerializeField]
    private Transform Player;

    [SerializeField]
    private CollisionEventRiser Activator;


    private WayPoint LastTarget = null;

    private void Awake()
    {
        Activator.OnTriggerEnterEvent += Activator_OnTriggerEnterEvent;
        foreach (var target in Targets)
        {
            target.Riser.OnTriggerEnterEvent += (collider) => OnEnter(collider, target.TargetPoint);
            target.Riser.gameObject.SetActive(false);
        }
    }

    private void Activator_OnTriggerEnterEvent(Collider other)
    {
        if (other.TryGetComponent<TestPlayer>(out var player))
        {
            foreach (var target in Targets)
            {
                target.Riser.gameObject.SetActive(true);
            }
        }
    }

    private void OnEnter(Collider other, WayPoint target)
    {
        if (other.TryGetComponent<TestPlayer>(out var player))
        {
            LastTarget = target;
        }
    }


    private void Update()
    {
        if (Vector2.Distance(Probe.position.ToXZ(), transform.position.ToXZ()) > 1f)
            return;

        if (Vector2.Distance(NPC.position.ToXZ(), transform.position.ToXZ()) > 1f)
            return;

        if (LastTarget == null)
            return;


        //setup
        Probe.transform.position = LastTarget.transform.position;

        foreach (var target in Targets)
        {
            target.Riser.gameObject.SetActive(false);
        }
        LastTarget = null;

    }
}
