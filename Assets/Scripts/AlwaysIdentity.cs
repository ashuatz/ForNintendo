using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysIdentity : MonoBehaviour
{
    void Update() => transform.rotation = Quaternion.identity;
}
