using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysIdentity : MonoBehaviour
{
    [SerializeField]
    private Vector3 OverrideEuler = Vector3.zero;

    void Update() => transform.rotation = Quaternion.Euler(OverrideEuler);
}
