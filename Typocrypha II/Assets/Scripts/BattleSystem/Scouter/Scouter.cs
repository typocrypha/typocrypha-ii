using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    [SerializeField] private VerticalLayoutGroup cursor;

    const float CURSOR_SLIDE_DURATION = 0.2f;
    const float CURSOR_WIGGLE_DISTANCE = 10f;
    const float CURSOR_WIGGLE_DURATION = 0.35f;


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
        DOTween.To(()=>cursor.spacing, (v)=>cursor.spacing = v, CURSOR_WIGGLE_DISTANCE, CURSOR_WIGGLE_DURATION)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
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

        //spell selection
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            currentSpell = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell], currentPage = 0);
            UpdateCursorPosition(currentSpell);
        }

        //next thesaurus page
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentPage < pageCount - 1)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], ++currentPage);
        }

        //previous thesaurus page
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentPage > 0)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], --currentPage);
        }
    }

    private void UpdateCursorPosition(int currentSpell)
    {
        cursor.transform.DOMoveY(EventSystem.current.currentSelectedGameObject.transform.position.y, CURSOR_SLIDE_DURATION);
    }

    private bool ScouterKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    private void ToggleScouter()
    {
        if (ScouterActive)
        {
            //exit scanner state
            ScouterActive = false;
            targetReticle.PH.Pause = false;
            Battlefield.instance.PH.Pause = false;
            Typocrypha.Keyboard.instance.PH.Pause = false;
            foreach (var c in Battlefield.instance.Casters) c.ui.onScouterHide.Invoke();
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        //pause
        ScouterActive = true;
        targetReticle.PH.Pause = true;
        Battlefield.instance.PH.Pause = true;
        Typocrypha.Keyboard.instance.PH.Pause = true;

        //show enemy description
        foreach (var c in Battlefield.instance.Casters) c.ui.onScouterShow.Invoke();

        //spell navigation
        firstSpellInList.Select();
        listOfSpells = SpellCooldownManager.instance.GetSpells();
        for (int i = 0; i < 8; i++)
            firstSpellInList.transform.parent.GetChild(i).gameObject.SetActive(i < listOfSpells.Count);
        pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell = 0], currentPage = 0);
    }
}
 