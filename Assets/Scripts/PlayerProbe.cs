using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class PlayerProbe : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private ParticleSystem.MinMaxCurve curve;


    [SerializeField]
    private Transform PlayerTransform;

    private float Runtime;

    private void Awake()
    {
        Runtime = Random.Range(1.0f, 2.0f);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerTransform.TransformPoint(offset), Time.deltaTime * curve.Evaluate(Mathf.Repeat(Time.time * Runtime, 1), Random.Range(0, 1)))
            .ToXZ()
            .ToVector3FromXZ(curve.Evaluate(Mathf.Repeat(Time.time * Runtime, 1)));
    }

}
