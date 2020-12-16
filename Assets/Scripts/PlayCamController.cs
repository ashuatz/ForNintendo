using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCamController : MonoBehaviour
{
    [SerializeField] Transform _target; //추적 대상
    [SerializeField] float _moveSpeed = 10.0f;

    [SerializeField] Vector3 _centerPos; //추적 모드일 경우 타겟으로부터의 위치

    public enum ControlMode
    {
        FOLLOWTARGET, //캐릭터 추적 모드
        FREE //자유시점 모드
    }

    [SerializeField] ControlMode _controlMode = ControlMode.FOLLOWTARGET;

    [SerializeField] KeyCode _changeKey = KeyCode.Space; //모드 바꾸는 키
    [SerializeField] Vector2 _mapSize = new Vector2(100.0f, 100.0f); //맵 크기

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        switch(_controlMode)
        {
            case ControlMode.FOLLOWTARGET:
                transform.position = _target.position + _centerPos;
                break;

            case ControlMode.FREE:
                ControlCamPos();
                break;
        }

        if (Input.GetKeyDown(_changeKey))
            ChangeControlMode();
    }

    void ChangeControlMode()
    {
        transform.position = _target.position + _centerPos;
        _controlMode = _controlMode == ControlMode.FOLLOWTARGET ? ControlMode.FREE : ControlMode.FOLLOWTARGET;
    }

    void ControlCamPos()
    {
        float x = 0, z = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || Screen.width * 0.05f > Input.mousePosition.x) x--;
        if (Input.GetKey(KeyCode.RightArrow) || Screen.width * 0.95f < Input.mousePosition.x) x++;
        if (Input.GetKey(KeyCode.DownArrow) || Screen.height * 0.05f > Input.mousePosition.y) z--;
        if (Input.GetKey(KeyCode.UpArrow) || Screen.height * 0.95f < Input.mousePosition.y) z++;

        x = x * _moveSpeed * Time.deltaTime;
        z = z * _moveSpeed * Time.deltaTime;
        Vector3 origin = transform.position;

        transform.position = new Vector3(Mathf.Clamp(origin.x + x, _mapSize.x * -0.5f + _centerPos.x, _mapSize.x * 0.5f + _centerPos.x), origin.y,
            Mathf.Clamp(z + origin.z, _mapSize.y * -0.5f + _centerPos.z, _mapSize.y * 0.5f + _centerPos.z));
    }
}
