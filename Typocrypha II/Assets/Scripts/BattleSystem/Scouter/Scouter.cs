using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private VerticalLayoutGroup spellCursor;
    [SerializeField] private HorizontalLayoutGroup thesaurusCursor;

    const float CURSOR_SLIDE_DURATION = 0.1f;
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
        DOTween.To(()=>spellCursor.spacing, (v)=>spellCursor.spacing = v, spellCursor.spacing + CURSOR_WIGGLE_DISTANCE, CURSOR_WIGGLE_DURATION)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);

        DOTween.To(() => thesaurusCursor.spacing, (v) => thesaurusCursor.spacing = v, thesaurusCursor.spacing + CURSOR_WIGGLE_DISTANCE, CURSOR_WIGGLE_DURATION)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle scouter
        if (ScouterKeyPressed() && (!Battlefield.instance.PH.Paused || ScouterActive))
        {
            ToggleScouter();
        }

        if (!ScouterActive) return;

        //spell selection
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            currentSpell = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
            pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell], currentPage = 0);
            UpdateSpellCursor();
        }

        //next thesaurus page
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentPage < pageCount - 1)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], ++currentPage);
            UpdateThesaurusCursor();
        }

        //previous thesaurus page
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentPage > 0)
        {
            thesaurus.DisplaySynonyms(listOfSpells[currentSpell], --currentPage);
            UpdateThesaurusCursor();
        }
    }

    private void UpdateSpellCursor()
    {
        GameObject target = EventSystem.current.currentSelectedGameObject ?? firstSpellInList.gameObject;
        spellCursor.transform.DOMoveY(target.transform.position.y, CURSOR_SLIDE_DURATION);
        spellCursor.transform.GetChild(0).GetComponent<Image>().enabled = currentSpell > 0;
        spellCursor.transform.GetChild(1).GetComponent<Image>().enabled = currentSpell < listOfSpells.Count - 1;
        UpdateThesaurusCursor();
    }

    private void UpdateThesaurusCursor()
    {
        thesaurusCursor.transform.GetChild(0).GetComponent<Image>().enabled = currentPage > 0;
        thesaurusCursor.transform.GetChild(1).GetComponent<Image>().enabled = currentPage < pageCount - 1;
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
            targetReticle.PH.Unpause(PauseSources.Scouter);
            Battlefield.instance.PH.Unpause(PauseSources.Scouter);
            InputManager.Instance.PH.Unpause(PauseSources.Scouter);
            BattleDimmer.instance.SetDimmer(false);
            foreach (var c in Battlefield.instance.Enemies) c.ui.onScouterHide.Invoke();
            EventSystem.current.SetSelectedGameObject(null);
            UpdateSpellCursor();
            return;
        }

        //enter scanner state
        ScouterActive = true;
        targetReticle.PH.Pause(PauseSources.Scouter);
        Battlefield.instance.PH.Pause(PauseSources.Scouter);
        InputManager.Instance.PH.Pause(PauseSources.Scouter);
        BattleDimmer.instance.SetDimmer(true);
        BattleDimmer.instance.DimCasters(Battlefield.instance.Enemies);
        foreach (var c in Battlefield.instance.Enemies) c.ui.onScouterShow.Invoke();

        //spell navigation
        firstSpellInList.Select();
        listOfSpells = SpellCooldownManager.instance.GetSpells();
        for (int i = 0; i < 8; i++)
            firstSpellInList.transform.parent.GetChild(i).gameObject.SetActive(i < listOfSpells.Count);
        pageCount = thesaurus.DisplaySynonyms(listOfSpells[currentSpell = 0], currentPage = 0);
        UpdateSpellCursor();
    }
}
 