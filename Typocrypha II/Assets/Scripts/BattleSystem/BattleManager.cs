using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameflow;

[RequireComponent(typeof(BattleGraphParser))]
public class BattleManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }
    public void OnPause(bool b) // Pauses battle events and battlefield
    {
        foreach (var e in currEvents)
            e.PH.Pause = b;
        Battlefield.instance.PH.Pause = b;
    }
    #endregion
    public static BattleManager instance = null;
    public GameObject[] stdBattleEvents;
    // DEBUG (come up with something better later)
    public string totalWaves = string.Empty;
    public Canvas waveTransitionCanvas;
    public GameObject defaultWaveTransitionPrefab = null;
    [Header("Default Spawn FX")]
    public SpellFxData defualtSpawnFx = new SpellFxData();

    private BattleGraphParser graphParser;
    private List<BattleEvent> currEvents = new List<BattleEvent>();
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
        ph = new PauseHandle(OnPause);
    }

    public bool startOnStart = true; // Should battle start when scene starts?

    private void Start()
    {
        // Set so that battle waits for transition
        TransitionManager.instance?.onStartScene.AddListener(StartBattle); 
        if (startOnStart) StartBattle();
    }

    /// <summary>
    /// Start new dialog graph.
    /// </summary>
    /// <param name="graph">Graph object to start.</param>
    public void StartBattle(BattleCanvas graph)
    {
        graphParser.Graph = graph;
        StartBattle();
    }

    /// <summary>
    /// Start new dialog graph. Implicitly uses graph already in parser.
    /// </summary>
    public void StartBattle()
    {
        var startNode = graphParser.Init();
        var player = Instantiate(startNode.player, transform).GetComponent<FieldObject>();
        Battlefield.instance.Add(player, new Battlefield.Position(1, 1));
        // TEMP CASTBAR HOOKUP
        Typocrypha.Keyboard.instance?.castBar.onCast.AddListener(player.GetComponent<DummyCaster>().CastString);
        waveNum = 0;
        NextWave();
    }

    /// <summary>
    /// Delete the currently active battle events and add a new collection
    /// if pause is true, pause all the events immediately after they are added
    /// if addStd is true, add the standard battle events to the collection
    /// </summary>
    public void SetBattleEvents(IEnumerable<GameObject> eventObjects, bool pause = true, bool addStd = true)
    {
        // Destroy battle events from previous wave
        foreach (var e in currEvents)
            Destroy(e.gameObject);
        currEvents.Clear();
        // Add the standard battle events for this battle to the list
        if(addStd)
            foreach (var e in stdBattleEvents)
                currEvents.Add(Instantiate(e).GetComponent<BattleEvent>());
        // Add the new battle events
        foreach (var e in eventObjects)
            currEvents.Add(Instantiate(e).GetComponent<BattleEvent>());
        // Pause all of the new battle events so they don't activate during the tarnsition
        PH.Pause = pause;
    }

    public void AddBattleEvent(BattleEvent battleEvent)
    {
        currEvents.Add(battleEvent);
        // Start it paused if the battleManager is in a paused state
    }

    public void NextWave()
    {
        Battlefield.instance.PH.Pause = true;
        ++waveNum;
        var wave = graphParser.NextWave();
        if (wave == null) return;
        // Set and pause the battle events
        SetBattleEvents(wave.battleEvents);
        // Clear the battlefield according to the clear options in the new wave
        Battlefield.instance.ClearAndDestroy(wave.fieldOptions);

        StartCoroutine(StartWaveCR(wave));
    }

    private IEnumerator StartWaveCR(BattleWave waveData)
    {
        var fieldData = waveData.battleField;
        int row = 0, col = 0;
        // Spawn all battlefield members
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
        yield return StartCoroutine(WaveTransition(waveData));
        //DEBUG, actually sequence after transition later
        PH.Pause = false; // Unpause battle events
        Battlefield.instance.PH.Pause = false; 
    }

    private IEnumerator WaveTransition(BattleWave waveData)
    { 
        GameObject wT = Instantiate(defaultWaveTransitionPrefab, waveTransitionCanvas.transform);
        wT.transform.Find("WaveBanner").GetComponentInChildren<Text>().text = DialogParser.instance.SubstituteMacros(waveData.waveTitle);
        wT.transform.Find("WaveTitle").GetComponentInChildren<Text>().text = "Wave " + waveNum + "/ " + totalWaves;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(0.5f);
    }
}
