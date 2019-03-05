using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages interfacing with TIPS database.
/// </summary>
public class TIPSManager : MonoBehaviour, IPausable
{
    #region IPausable
    PauseHandle ph;
    public PauseHandle PH { get => ph; }

    public void OnPause(bool b)
    {
        enabled = !b;
    }
    #endregion
    public static TIPSManager instance = null;
    public TIPSEntryData currSearchable; // Current dialog line's searchable term.
    public TIPSEntryData CurrSearchable
    {
        get => currSearchable;
        set
        {
            currSearchable = value;
            if (currSearchable == null)
            {
                TIPSTab.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                TIPSTab.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
    }
    public GameObject TIPSTab; // TIPS tab to notify when new entries are added.
    public GameObject TIPSMenu; // TIPS menu object.
    public InputField TIPSsearch; // TIPS search bar.

    void Awake()
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
        ph = new PauseHandle(OnPause);
    }

    void Update()
    {
        if (Input.GetButtonDown("TIPS"))
        {
            TIPSMenu.SetActive(!TIPSMenu.activeSelf);
            PauseManager.instance.PauseAll(TIPSMenu.activeSelf);
            if (TIPSMenu.activeSelf) // Turning TIPS menu on.
            {
                PH.Pause = false; // Unpause self.
                if (CurrSearchable != null)
                    TIPSsearch.text = CurrSearchable.searchTerms.Items[0];
                else
                    TIPSsearch.text = "";
            }
        }
    }
}
