using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class InputManager : MonoSingleton<InputManager>
{
    public Notifier<Vector3> MouseWorldPosition = new Notifier<Vector3>();
    public Notifier<Vector2> MouseWorldXZ = new Notifier<Vector2>();


    private bool TryGetWorldPosition(Vector3 MousePosition, out Vector2 WorldXZPosition)
    {
        WorldXZPosition = Vector2.zero;

        var ray = Camera.main.ScreenPointToRay(MousePosition);

        if (Physics.Raycast(ray, out var hit, 50 ,1 << LayerMask.NameToLayer("Ground")))
        {
            Debug.Log("Hit : " + hit.transform.name);
            WorldXZPosition = hit.point.ToXZ();
            return true;
        }

        return false;
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (TryGetWorldPosition(mousePosition, out var xz))
        {
            MouseWorldXZ.CurrentData = xz;
            MouseWorldPosition.CurrentData = xz.ToVector3FromXZ();
        }
    }
}
