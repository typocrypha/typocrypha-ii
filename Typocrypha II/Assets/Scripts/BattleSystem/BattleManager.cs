using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;
using Typocrypha;

[RequireComponent(typeof(BattleGraphParser))]
public class BattleManager : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    public void OnPause(bool pauseState) // Pauses battle events and battlefield
    {
        SetBattleEventPause(pauseState);
        Battlefield.instance.PH.Pause = pauseState;
        Keyboard.instance.PH.Pause = pauseState;
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

    [Header("Victory")]
    [SerializeField] VictoryResultsScreen victoryScreen;

    [Header("Game Over")]
    public UnityEngine.Events.UnityEvent OnGameOver = default;
    public AudioClip GameOverAudioClip = default;

    [Header("Equipment Menu")]
    [SerializeField] private EquipmentMenu equipmentMenu;

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
        PH = new PauseHandle(OnPause);
    }

    [SerializeField] private bool startOnStart = true; // Should battle start when scene starts?
    [SerializeField] private bool loadDebugSave = false; // Should battle start when scene starts?

#if DEBUG
    private void Start()
    {
        if(loadDebugSave)
        {
            SaveManager.instance.DebugLoadCampaign();
        }
        if (startOnStart)
        {
            LoadBattle();
            StartBattle();
        }
    }
#endif

    private void LoadBattle()
    {
        var startNode = graphParser.Init();
        totalWaves = startNode.totalWaves;
        // Initialize Player
        var player = Instantiate(startNode.player, transform).GetComponent<Caster>();
        Battlefield.instance.Add(player, new Battlefield.Position(1, 1));
        PlayerDataManager.instance.equipment.ReapplyEquippedBadgeWords(player);
        // Initialize ally character
        if(startNode.initialAllyData != null)
        {
            var expr = string.IsNullOrEmpty(startNode.initialAllyExpr) ? CharacterData.defaultExpr : startNode.initialAllyExpr;
            var pose = string.IsNullOrEmpty(startNode.initialAllyPose) ? CharacterData.defaultPose : startNode.initialAllyPose;
            AllyBattleBoxManager.instance.SetBattleAllyData(startNode.initialAllyData, expr, pose);
        }
        else
        {
            AllyBattleBoxManager.instance.SetBattleAllyData(null, "", "");
        }
        AllyBattleBoxManager.instance.SetBattleAllyCharacterInstant();
        // Initialize proxy casters
        AddProxyCaster(startNode.proxyCaster1);
        AddProxyCaster(startNode.proxyCaster2);
        AddProxyCaster(startNode.proxyCaster3);
    }

    private void AddProxyCaster(GameObject prefab)
    {
        if (prefab == null)
            return;
        var proxyCaster = Instantiate(prefab, transform).GetComponent<Caster>();
        Battlefield.instance.AddProxyCaster(proxyCaster);
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

    public void SetBattleEvents(IEnumerable<GameObject> eventObjects, bool addStd)
    {
        // Add the standard battle events for this battle to the list
        if (addStd)
        {
            foreach (var e in stdBattleEvents)
            {
                AddBattleEvent(Instantiate(e).GetComponent<BattleEvent>());
            }
        }
        // Add the new battle events
        foreach (var e in eventObjects)
        {
            AddBattleEvent(Instantiate(e).GetComponent<BattleEvent>());
        }
    }

    public void AddBattleEvent(BattleEvent battleEvent)
    {
        currEvents.Add(battleEvent);
        // Start it paused if the battleManager is in a paused state
        if (PH.Pause)
        {
            battleEvent.PH.Pause = true;
        }
    }

    public void SetBattleEventPause(bool pause)
    {
        foreach (var e in currEvents)
            e.PH.Pause = pause;
    }

    public void ClearReinforcements()
    {
        CurrWave.reinforcementPrefabs.Clear();
    }

    public void NextWave()
    {
        PH.Pause = true;
        ++waveNum;
        CurrWave = graphParser.NextWave();
        if (CurrWave == null) return;

        // Destroy battle events from previous wave
        foreach (var e in currEvents)
            Destroy(e.gameObject);
        currEvents.Clear();
        // Clear the battlefield according to the clear options in the new wave
        Battlefield.instance.Clear(CurrWave.fieldOptions);
        // Clear player cooldowns
        SpellCooldownManager.instance.ResetAllCooldowns();
        Typocrypha.Keyboard.instance.Clear();
        Rule.ActiveRule = null;
        Battlefield.instance.Player.TargetPos = new Battlefield.Position(0, 1);
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
            yield return StartCoroutine(AddCaster(fieldData[row, col], row, col));
            if (++col >= fieldData.Columns)
            {
                col = 0;
                ++row;
            }
        }
        yield return StartCoroutine(WaveTransition(waveData));
        // Set and pause the battle events
        SetBattleEvents(waveData.battleEvents, waveData.useStdEvents);
        if (waveData.music != null)
        {
            AudioManager.instance.PlayBGM(waveData.music);
            AudioManager.instance.BGMVolume = 0.6f;
        }
        if (waveData.openingScene != null)
        {
            // Play opening scene
            DialogManager.instance.StartDialog(waveData.openingScene, true);
        }
        PH.Pause = false; // Unpause if no dialog scene, else remove extra pause
    }

    public class ReinforcementData
    {
        public ReinforcementData(GameObject prefab, Battlefield.Position pos)
        {
            Prefab = prefab;
            Pos = pos;
        }

        public ReinforcementData(GameObject prefab, int row, int col) : this(prefab, new Battlefield.Position(row, col))
        {
        }

        public GameObject Prefab { get; set; }
        public Battlefield.Position Pos { get; set; }
    }

    public IEnumerator AddCasters(IReadOnlyList<ReinforcementData> data, bool unPause = false)
    {
        for (int i = 0; i < data.Count; i++)
        {
            var casterData = data[i];
            yield return StartCoroutine(AddCaster(casterData.Prefab, casterData.Pos.Row, casterData.Pos.Col, unPause));
        }
    }

    public IEnumerator AddCaster(GameObject casterPrefab, int row, int col, bool unPause = false)
    {
        var caster = Instantiate(casterPrefab).GetComponent<Caster>();
        if (caster == null)
            yield break;
        Battlefield.instance.Add(caster, new Battlefield.Position(row, col));
        // Use the default FX if the prefab doesn't have a SpawnFX componetn
        var fx = caster.GetComponent<SpawnFX>()?.fx ?? defualtSpawnFx;
        // Play and wait for spawn effects
        yield return StartCoroutine(fx.Play(caster.transform.position, true));
        if(unPause)
        {
            var actor = caster.GetComponent<ATB3.ATBActor>();
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
        if (waveData.allowEquipment)
        {
            equipmentMenu.Enable();
        }
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && !equipmentMenu.IsShowing);
        equipmentMenu.Disable();
        wT.ContinueAnimation();
        yield return new WaitForSeconds(0.5f);
    }

    public void Victory(VictoryScreenNode results)
    {
        victoryScreen.DisplayResults(results);
    }

    public void GameOver()
    {
        PH.Pause = true;
        AudioManager.instance.PlayBGM(GameOverAudioClip);
        OnGameOver.Invoke();
    }

    public void Reload()
    {
        Battlefield.instance.Remove(1, 1, true); //removes player if already exists
        AudioManager.instance.StopBGM();
        PH.Pause = false;
        LoadBattle();
        StartBattle();
    }
}
