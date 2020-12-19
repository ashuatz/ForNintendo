using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class TestMinion : TestEntity
{
    [SerializeField]
    private float DefaultHP;

    [SerializeField]
    private float DefaultSpeed;

    //[SerializeField]
    //private NavMeshAgent Agent;

    private float Speed;

    [SerializeField]
    private Transform PlayerProbe;

    private CoroutineWrapper BuildWrapper;

    [SerializeField]
    private List<GameObject> BuildObjects;

    [SerializeField]
    private Animator animator;


    public bool CC { get; private set; }
    public bool isBuilding { get; private set; }

    public bool isOrderAvailable { get => !CC && !isBuilding; }

    public event System.Action<TestStructure> OnBuildOnce;

    public NotifierClass<Transform> MoveTargetNotifier { get; private set; }

    private void Awake()
    {
        HP.CurrentData = DefaultHP;

        animator.speed = Random.Range(0.95f, 1.05f);

        BuildWrapper = new CoroutineWrapper(this);
        MoveTargetNotifier = new NotifierClass<Transform>();
        MoveTargetNotifier.CurrentData = PlayerProbe;
        MoveTargetNotifier.OnDataChanged += MoveTargetNotifier_OnDataChanged;
    }

    private void Update()
    {
        Speed = Mathf.Clamp(Vector2.Distance(PlayerProbe.position.ToXZ(), transform.position.ToXZ()) * DefaultSpeed, 1, 10);

        if (MoveTargetNotifier.CurrentData != null)
        {
            transform.position = Vector3.Lerp(transform.position, MoveTargetNotifier.CurrentData.position.ToXZ().ToVector3FromXZ(1.5f), 0.05f);
        }
    }

    private void MoveTargetNotifier_OnDataChanged(Transform obj)
    {
        //Agent.SetDestination(obj.position);
    }

    public void Build(int index, Vector2 position, float time)
    {
        BuildWrapper.Start(BuildTarget(index, position, time));

        IEnumerator BuildTarget(int idx, Vector2 pos, float t)
        {
            isBuilding = true;

            var preview = TestBuildPreview.Instance.GetPreviewFormPool(idx - 1);
            preview.transform.position = pos.ToVector3FromXZ().Round(1);
            preview.SetActive(true);

            var obj = Instantiate(BuildObjects[idx - 1]);
            var Component = obj.GetComponent<TestStructure>();
            obj.transform.position = pos.ToVector3FromXZ().Round(1);
            obj.SetActive(false);

            MoveTargetNotifier.CurrentData = obj.transform;

            OnBuildOnce?.Invoke(Component);
            OnBuildOnce = null;

            bool isSuccess = false;

            yield return this.WaitforTimeWhileCondition(3, () => Vector2.Distance(pos, transform.position.ToXZ()) > 2, (complete) => { isSuccess = complete; });

            yield return new WaitForSeconds(t);

            MoveTargetNotifier.CurrentData = PlayerProbe;

            TestBuildPreview.Instance.AddToPool(preview);

            obj.SetActive(true);
            isBuilding = false;
        }
    }


}
