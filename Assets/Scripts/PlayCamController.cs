using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamController : MonoBehaviour
{
    Transform _target; //추적 대상
    [SerializeField] float _moveSpeed = 3.0f;

    public enum ControlMode
    {
        FOLLOWTARGET, //캐릭터 추적 모드
        FREE //자유시점 모드
    }

    [SerializeField] ControlMode _controlMode = ControlMode.FOLLOWTARGET;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        switch(_controlMode)
        {
            case ControlMode.FOLLOWTARGET:
                transform.position = _target.position;
                break;

            case ControlMode.FREE:
                ControlCamPos();
                break;
        }
    }

    void ControlCamPos()
    {
        int x = 0, y = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || Screen.width * 0.05f > Input.mousePosition.x) x--;
        if (Input.GetKey(KeyCode.RightArrow) || Screen.width * 0.95f < Input.mousePosition.x) x++;
        if (Input.GetKey(KeyCode.DownArrow) || Screen.height * 0.05f > Input.mousePosition.y) y--;
        if (Input.GetKey(KeyCode.UpArrow) || Screen.height * 0.95f < Input.mousePosition.y) y++;

        transform.position += new Vector3(x, 0, y) * _moveSpeed * Time.deltaTime;
    }
}
