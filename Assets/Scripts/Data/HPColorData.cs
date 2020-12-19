using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CreateAssetMenu(fileName = "NewColorData", menuName = "ColorData", order = 0)]
#endif


public class HPColorData : ScriptableObject
{
    public enum HPType
    {
        None,

        Player,
        NPC,
        Minion,

        Structure,

        Enemy,
    }

    [Serializable]
    public class HPColor
    {
        public HPType myType;

        public Color BackgroundColor = Color.gray;
        public Color Phase0 = Color.white;
        public Color Phase1 = Color.red;
        public Color Phase2 = Color.red;

        public Color GetColor(int phaseIdx)
        {
            switch (phaseIdx)
            {
                case 0: return Phase0;
                case 1: return Phase1;
                case 2: return Phase2;
            }

            return Color.white;
        }
    }


    public List<HPColor> colors;
}
