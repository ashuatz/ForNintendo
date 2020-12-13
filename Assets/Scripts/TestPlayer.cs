using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Util;

public class TestPlayer : TestEntity
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform Target;

    [SerializeField]
    private NavMeshAgent Agent;

    [SerializeField]
    private GameObject Marker;
    
    [SerializeField]
    private List<TestMinion> Minions;
    public TestMinion GetCurrentMinion { get => Minions.Find(minion => minion.isOrderAvailable); }

    public Notifier<int> BuildIndex = new Notifier<int>();

    private CoroutineWrapper markerRoutine;
    private float time = 0;

    private void Start()
    {
        animator.speed = 0.85f;
        markerRoutine = CoroutineWrapper.Generate(this);

        //GetComponent<NavMeshAgent>().SetDestination(Target.position);
        time = Time.time;
    }

    private bool TryGetWorldPosition(Vector3 MousePosition, out Vector2 WorldXZPosition)
    {
        WorldXZPosition = Vector2.zero;

        var ray = Camera.main.ScreenPointToRay(MousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            WorldXZPosition = hit.point.ToXZ();
            return true;
        }

        return false;
    }

    void Update()
    {
        PlayerInput();
        Animation();

        void Animation()
        {
            if (Agent.velocity.magnitude > 0.1f)
            {

            }
        }
    }


    private void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //build Test
            BuildIndex.CurrentData = 1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            BuildIndex.CurrentData = 2;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            BuildIndex.CurrentData = 3;
        }

        //우클릭 이동
        if (Input.GetMouseButtonDown(1))
        {
            var position = InputManager.Instance.MouseWorldPosition.CurrentData;

            Agent.SetDestination(position);
            Marker.transform.position = position;
            Marker.SetActive(true);

            markerRoutine.StartSingleton(MarkerOff());


            if (BuildIndex.CurrentData != 0) BuildIndex.CurrentData = 0;

        }

        if (Input.GetMouseButton(0))
        {
            if (BuildIndex.CurrentData != 0)
            {
                var position = InputManager.Instance.MouseWorldXZ.CurrentData;
                var pos = Input.mousePosition;

                var currentMinion = GetCurrentMinion;
                if (currentMinion != null)
                {
                    GetCurrentMinion.Build(BuildIndex.CurrentData, position, 1f + BuildIndex.CurrentData);
                    BuildIndex.CurrentData = 0;
                }
            }
        }

        IEnumerator MarkerOff()
        {
            yield return new WaitForSeconds(0.5f);
            Marker.SetActive(false);
        }
    }
}
