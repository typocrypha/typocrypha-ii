using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Target reticule and scouter during battle.
/// </summary>
public class Scouter : MonoBehaviour, IPausable
{
    #region IPausable
    public PauseHandle PH { get; private set; }
    // Stop target reticule movement and animation.
    public void OnPause(bool pause)
    {
        enabled = !pause;
    }
    #endregion

    [SerializeField] private TargetReticle targetReticle;

    public GameObject spellModeDisplay; // Scouter display object.
    [SerializeField] private Button firstSpellInList;
    [SerializeField] private Thesaurus thesaurus;

    private IReadOnlyList<SpellWord> listOfSpells = null;
    private int currentSpell = -1;
    private int currentPage = -1;
    private int pageCount = -1;

    public bool ScouterActive // Is the scouter active (i.e. display is active)?
    {
        get => spellModeDisplay.activeSelf;
        set => spellModeDisplay.SetActive(value);
    }

    void Awake()
    {
        PH = new PauseHandle(OnPause);
    }

    void Start()
    {
        PH.SetParent(BattleManager.instance.PH);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle scouter
        if (ScouterKeyPressed() && (!Battlefield.instance.PH.Pause || ScouterActive))
        {
            ToggleScouter();
        }

        if (!ScouterActive) return;

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            currentSpell = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell], currentPage = 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && currentPage < pageCount - 1)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], ++currentPage);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentPage > 0)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], --currentPage);
        }
    }

    private bool ScouterKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    private void ToggleScouter()
    {
        if (ScouterActive)
        {
            ScouterActive = false;
            targetReticle.PH.Pause = false;
            Battlefield.instance.PH.Pause = false;
            Typocrypha.Keyboard.instance.PH.Pause = false;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        ScouterActive = true;
        targetReticle.PH.Pause = true;
        Battlefield.instance.PH.Pause = true;
        Typocrypha.Keyboard.instance.PH.Pause = true;
        firstSpellInList.Select();
        listOfSpells = SpellCooldownManager.instance.GetSpells();
        for (int i = 0; i < 8; i++)
            firstSpellInList.transform.parent.GetChild(i).gameObject.SetActive(i < listOfSpells.Count);
        pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell = 0], currentPage = 0);
    }
}
 