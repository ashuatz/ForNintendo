using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class WorldData : MonoSingleton<WorldData>
{
    /*
     * Physics.ComputePenetration
     * 물리 레이어 콜라이더 행렬과 관계없음
     * 꺼져있는 콜라이더는 충돌처리되지않음
     * 트리거든 아니든 상관없음
     */
    
    [SerializeField]
    private List<Collider> StaticColliders;

    private List<Collider> DynamicColliders = new List<Collider>();

    private List<Collider> StructureColliders = new List<Collider>();

    public List<TestStructure> BuildedStructure = new List<TestStructure>();

    public Dictionary<Vector2, TestStructure> BuildedStructureDict = new Dictionary<Vector2, TestStructure>();

    [SerializeField]
    private BoxCollider TesterA;
    [SerializeField]
    private BoxCollider TesterB;

    protected override void Awake()
    {
        base.Awake();
    }

    public void AddStructure(TestStructure target)
    {
        var xz = target.transform.position.Round(1).ToXZ();
        StructureColliders.Add(target.StructureCollider);

        BuildedStructure.Add(target);
        BuildedStructureDict[xz] = target;
    }

    public bool IsExist(Collider tester)
    {
        foreach (var staticCollider in StaticColliders)
        {
            if (Physics.ComputePenetration(tester, tester.transform.position, tester.transform.rotation, staticCollider, staticCollider.transform.position, staticCollider.transform.rotation, out var dir, out var dis))
            {
                return true;
            }
        }

        foreach (var structureCollider in StructureColliders)
        {
            int type = 0;

            if (!tester.gameObject.activeInHierarchy && tester is BoxCollider)
            {
                TesterA.transform.position = tester.transform.position;
                TesterA.transform.rotation = tester.transform.rotation;
                TesterA.transform.localScale = tester.transform.lossyScale;

                TesterA.size = (tester as BoxCollider).size;
                TesterA.center = (tester as BoxCollider).center;

                TesterA.gameObject.SetActive(true);
                type += 1;
            }

            if (!structureCollider.gameObject.activeInHierarchy && structureCollider is BoxCollider)
            {
                TesterB.transform.position = structureCollider.transform.position;
                TesterB.transform.rotation = structureCollider.transform.rotation;
                TesterB.transform.localScale = structureCollider.transform.lossyScale;

                TesterB.size = (structureCollider as BoxCollider).size;
                TesterB.center = (structureCollider as BoxCollider).center;

                TesterB.gameObject.SetActive(true);
                type += 2;
            }

            if (type == 0)
            {
                if (Physics.ComputePenetration(tester, tester.transform.position, tester.transform.rotation, structureCollider, structureCollider.transform.position, structureCollider.transform.rotation, out var dir, out var dis))
                    return true;
            }
            else if (type == 1)
            {
                if (Physics.ComputePenetration(TesterA, TesterA.transform.position, TesterA.transform.rotation, structureCollider, structureCollider.transform.position, structureCollider.transform.rotation, out var dir, out var dis))
                    return true;
            }
            else if (type == 2)
            {
                if (Physics.ComputePenetration(tester, tester.transform.position, tester.transform.rotation, TesterB, TesterB.transform.position, TesterB.transform.rotation, out var dir, out var dis))
                    return true;
            }
            else
            {
                if (Physics.ComputePenetration(TesterA, TesterA.transform.position, TesterA.transform.rotation, TesterB, TesterB.transform.position, TesterB.transform.rotation, out var dir, out var dis))
                    return true;
            }

            TesterA.gameObject.SetActive(false);
            TesterB.gameObject.SetActive(false);
        }

        foreach (var dynamicCollider in DynamicColliders)
        {
            if (Physics.ComputePenetration(tester, tester.transform.position, tester.transform.rotation, dynamicCollider, dynamicCollider.transform.position, dynamicCollider.transform.rotation, out var dir, out var dis))
            {
                return true;
            }
        }

        return false;
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
        StructureColliders.Remove(target.StructureCollider);

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
