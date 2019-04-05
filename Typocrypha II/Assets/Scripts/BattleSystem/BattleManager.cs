using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameflow;

[RequireComponent(typeof(BattleGraphParser))]
public class BattleManager : MonoBehaviour
{
    public static BattleManager instance = null;
    public GameObject[] stdBattleEvents;
    // DEBUG (come up with something better later)
    public string totalWaves = string.Empty;
    public bool leaveOldEnemies = true;
    public Canvas waveTransitionCanvas;
    public GameObject defaultWaveTransitionPrefab = null;
    [Header("Default Spawn FX")]
    public SpellFxData defualtSpawnFx = new SpellFxData();

    private BattleGraphParser graphParser;
    private BattleEvent[] currEvents;
    private BattleWave currWave;
    private int waveNum = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        graphParser = GetComponent<BattleGraphParser>();
    }

    private void Start()
    {
        StartBattle();
    }

    /// <summary>
    /// Start new dialog graph.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartBattle(BattleCanvas graph)
    {
        graphParser.Graph = graph;
        graphParser.Init();
        waveNum = 0;
        NextWave();
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    public void StartBattle()
    {
        graphParser.Init();
        waveNum = 0;
        NextWave();
    }

    public void NextWave()
    {
        ++waveNum;
        var wave = graphParser.NextWave();
        if (!leaveOldEnemies)
            Battlefield.instance.DestroyAllAndClear();
        StartCoroutine(StartWaveCR(wave));
    }

    private IEnumerator StartWaveCR(BattleWave waveData)
    {
        var fieldData = waveData.battleField;
        int row = 0;
        int col = 0;
        while (row < fieldData.Rows)
        {
            if (fieldData[row, col] == null)
            {
                if (++col >= fieldData.Columns)
                {
                    col = 0;
                    ++row;
                }
                continue;
            }
            FieldObject e = Instantiate(fieldData[row, col]).GetComponent<FieldObject>();
            Battlefield.instance.Add(e, new Battlefield.Position(row, col));
            // Use the default FX if the prefab doesn't have a SpawnFX componetn
            var fx = e.GetComponent<SpawnFX>()?.fx ?? defualtSpawnFx;
            // Play and wait for spawn effects
            yield return StartCoroutine(fx.Play(e.transform.position));
            if (++col >= fieldData.Columns)
            {
                col = 0;
                ++row;
            }
        }
        WaveTransition(waveData);
    }

    private void WaveTransition(BattleWave waveData)
    {
        GameObject wT = Instantiate(defaultWaveTransitionPrefab, waveTransitionCanvas.transform);
        wT.transform.Find("WaveBanner").GetComponentInChildren<Text>().text = DialogParser.instance.SubstituteMacros(waveData.waveTitle);
        wT.transform.Find("WaveTitle").GetComponentInChildren<Text>().text = "Wave " + waveNum + "/ " + totalWaves;
    }
}
