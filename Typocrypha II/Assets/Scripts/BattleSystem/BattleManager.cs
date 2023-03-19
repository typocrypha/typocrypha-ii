using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

[RequireComponent(typeof(BattleGraphParser))]
public class BattleManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }
    public void OnPause(bool pauseState) // Pauses battle events and battlefield
    {
        SetBattleEventPause(pauseState);
        Battlefield.instance.PH.Pause = pauseState;
    }
    #endregion

    public static BattleManager instance = null;
    public BattleWave CurrWave { get; private set; }
    public GameObject[] stdBattleEvents;
    public string totalWaves = string.Empty;
    public Canvas waveTransitionCanvas;
    public GameObject defaultWaveTransitionPrefab = null;
    [Header("Default Spawn FX")]
    public SpellFxData defualtSpawnFx = new SpellFxData();

    private BattleGraphParser graphParser;
    private readonly List<BattleEvent> currEvents = new List<BattleEvent>();
    private int waveNum = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        graphParser = GetComponent<BattleGraphParser>();
        ph = new PauseHandle(OnPause);
    }

    public bool startOnStart = true; // Should battle start when scene starts?

    private void Start()
    {
        if (startOnStart)
        {
            LoadBattle();
            StartBattle();
        }
    }

    private void LoadBattle()
    {
        var startNode = graphParser.Init();
        totalWaves = startNode.totalWaves;
        // Initialize Player
        var player = Instantiate(startNode.player, transform).GetComponent<FieldObject>();
        Battlefield.instance.Add(player, new Battlefield.Position(1, 1));
        Typocrypha.Keyboard.instance?.castBar.onCast.AddListener(player.GetComponent<Player>().CastString);
        // Initialize ally character
        AllyBattleBoxManager.instance.SetBattleAllyData(startNode.initialAllyData, startNode.initialAllyExpr, startNode.initialAllyPose);
        AllyBattleBoxManager.instance.SetBattleAllyCharacterInstant();
    }

    public void LoadBattle(BattleCanvas graph)
    {
        graphParser.Graph = graph;
        LoadBattle();
    }

    /// <summary>
    /// Start new battle graph. Uses graph set in parser by "LoadBattle"
    /// </summary>
    public void StartBattle()
    {
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
        // Add the standard battle events for this battle to the list
        if (addStd)
        {
            foreach (var e in stdBattleEvents)
            {
                currEvents.Add(Instantiate(e).GetComponent<BattleEvent>());
            }
        }
        // Add the new battle events
        foreach (var e in eventObjects)
        {
            currEvents.Add(Instantiate(e).GetComponent<BattleEvent>());
        }
        // Pause all of the new battle events so they don't activate during the tarnsition
        //PH.Pause = pause;
    }

    public void AddBattleEvent(BattleEvent battleEvent)
    {
        currEvents.Add(battleEvent);
        // Start it paused if the battleManager is in a paused state
    }

    public void SetBattleEventPause(bool pause)
    {
        foreach (var e in currEvents)
            e.PH.Pause = pause;
    }

    public void NextWave()
    {
        Battlefield.instance.PH.Pause = true;
        ++waveNum;
        CurrWave = graphParser.NextWave();
        if (CurrWave == null) return;

        // Destroy battle events from previous wave
        foreach (var e in currEvents)
            Destroy(e.gameObject);
        currEvents.Clear();
        // Clear the battlefield according to the clear options in the new wave
        Battlefield.instance.ClearAndDestroy(CurrWave.fieldOptions);
        // Clear player cooldowns
        SpellCooldownManager.instance.ResetAllCooldowns();
        StartCoroutine(StartWaveCR(CurrWave));
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
            yield return StartCoroutine(AddFieldObject(fieldData[row, col], row, col));
            if (++col >= fieldData.Columns)
            {
                col = 0;
                ++row;
            }
        }
        yield return StartCoroutine(WaveTransition(waveData));
        // Set and pause the battle events
        SetBattleEvents(waveData.battleEvents);
        //DEBUG, actually sequence after transition later
        PH.Pause = false; // Unpause battle events
        Battlefield.instance.PH.Pause = false;
        if(waveData.music != null)
        {
            AudioManager.instance.PlayBGM(waveData.music);
            AudioManager.instance.BGMVolume = 0.6f;
        }
          
    }

    public IEnumerator AddFieldObject(GameObject fieldObjectPrefab, int row, int col, bool unPause = false)
    {
        FieldObject e = Instantiate(fieldObjectPrefab).GetComponent<FieldObject>();
        if (e == null)
            yield break;
        Battlefield.instance.Add(e, new Battlefield.Position(row, col));
        // Use the default FX if the prefab doesn't have a SpawnFX componetn
        var fx = e.GetComponent<SpawnFX>()?.fx ?? defualtSpawnFx;
        // Play and wait for spawn effects
        yield return StartCoroutine(fx.Play(e.transform.position));
        if(unPause)
        {
            var actor = e.GetComponent<ATB3.ATBActor>();
            if (actor != null)
                actor.Pause = false;
        }
    }

    private IEnumerator WaveTransition(BattleWave waveData)
    { 
        var wT = Instantiate(defaultWaveTransitionPrefab, waveTransitionCanvas.transform).GetComponent<WaveTransitionBanner>();
        wT.TitleText = TextMacros.SubstituteMacros(waveData.waveTitle);
        if (!string.IsNullOrEmpty(waveData.waveNumberOverride))
        {
            wT.NumberText = TextMacros.SubstituteMacros(waveData.waveNumberOverride);
        }
        else
        {
            wT.NumberText = "Wave " + waveNum + "/" + totalWaves;
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(0.5f);
    }
}
