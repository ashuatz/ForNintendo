using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "NewWayPointData", menuName = "WayPointData", order = 0)]
#endif

public class WayPointData : ScriptableObject
{
    [Serializable]
    public class Message
    {
        public float time;
        public string desc;
        public float duration;
    }

    public List<Message> Messages;
}

