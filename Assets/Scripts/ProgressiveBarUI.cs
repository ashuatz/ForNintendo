using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class ProgressiveBarUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> DefenseProgressBars;
    [SerializeField]
    private List<Image> ClearMarkers;

    [SerializeField]
    private List<Image> MovementProgressBars;

    public Dictionary<int, Transform> DefenseSectors = new Dictionary<int, Transform>();

    public int ClearIndex { get; private set; }

    private Sector[] sectors;

    private List<int> maxCount = new List<int>();

    public bool _onBar = true;

    private void Awake()
    {
        ClearIndex = -1;
    }

    void Start()
    {
        DataContainer.Instance.ProgressBarUI.CurrentData = this;

        sectors = SpawnerParent._Sectors;
        foreach (var sector in sectors)
        {
            maxCount.Add(sector._maxEnemy);
        }

        foreach (var progress in DefenseProgressBars) progress.fillAmount = 0;
        foreach (var progress in MovementProgressBars) progress.fillAmount = 0;
        foreach (var marker in ClearMarkers) marker.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_onBar)
        {
            DrawDefenseProgressBar();
        }

        if (ClearIndex >= 0)
        {
            DrawMoveProgressBar();
        }
    }

    void DrawDefenseProgressBar()
    {
        for (int i = ClearIndex + 1; i < DefenseProgressBars.Count; i++)
        {
            var kill = sectors[i]._spawners.Sum(new System.Func<TestEnemySpawner_New, int>((spawner) => spawner._killEnemy));

            var amount = (float)kill / maxCount[i];
            DefenseProgressBars[i].fillAmount = amount;
            if (amount == 1 || Mathf.Approximately(amount, 1))
            {
                ClearIndex = Mathf.Max(ClearIndex, i);
                ClearMarkers[ClearIndex].gameObject.SetActive(true);
                DefenseSectors[ClearIndex].gameObject.SetActive(false);
            }
        }
    }

    void DrawMoveProgressBar()
    {
        if (ClearIndex >= MovementProgressBars.Count)
            return;

        var targetProgressBar = MovementProgressBars[ClearIndex];

        if (targetProgressBar.fillAmount > 0.92f)
        {
            targetProgressBar.fillAmount = 1f;
            return;
        }

        var prev = DefenseSectors[ClearIndex];
        var next = DefenseSectors[ClearIndex + 1];

        var amount =
            Vector2.Distance(DataContainer.Instance.Player.CurrentData.transform.position.ToXZ(), next.transform.position.ToXZ()) /
            Vector2.Distance(prev.transform.position.ToXZ(), next.transform.position.ToXZ());

        targetProgressBar.fillAmount = 1 - amount;
    }
}
