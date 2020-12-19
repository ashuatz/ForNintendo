using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStayEventRiser : CollisionEventRiser
{
    private void OnTriggerStay(Collider other)
    {
        base.CallOnTriggerStay(other);
    }
}
