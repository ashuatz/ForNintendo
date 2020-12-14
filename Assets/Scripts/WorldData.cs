using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Util;

public class WorldData : Singleton<WorldData>
{
    public List<TestStructure> BuildedStructure = new List<TestStructure>();

    public Dictionary<Vector2, TestStructure> BuildedStructureDict = new Dictionary<Vector2, TestStructure>();

    public void AddStructure(TestStructure target)
    {
        var xz = target.transform.position.Round(1).ToXZ();

        BuildedStructure.Add(target);
        BuildedStructureDict[xz] = target;
    }

    public bool IsExist(Vector2 xz)
    {
        //check with this function
        //Physics.ComputePenetration()

        var key = xz.ToVector3FromXZ().Round(1).ToXZ();

        return BuildedStructureDict.ContainsKey(key);
    }

    public void RemoveStructure(TestStructure target)
    {
        BuildedStructure.Remove(target);
        if (BuildedStructureDict.TryGetValue(target.transform.position.ToXZ(), out var check))
        {
            if (check == target)
            {
                BuildedStructureDict.Remove(target.transform.position.ToXZ());
            }
        }
    }
}
