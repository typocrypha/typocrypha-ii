using UnityEngine;

public class TIPSControlGuideSetter : MonoBehaviour
{
    [SerializeField] ControlGuide guide;
    private string[] standard = new string[] {
        "<sprite name=keyboard_tab> Exit",
        "<sprite name=keyboard_semicolon> Log",
        "",
    };

    private string[] search = new string[] {
        "",
        "<sprite name=keyboard_arrow_down> Entries",
        "<sprite name=keyboard_enter> Search",
    };

    private string[] topics = new string[] {
        "",
        "<sprite name=keyboard_arrow_left> Back",
        "<sprite name=keyboard_arrow_right> Enter",
    };

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            guide.SetControl(i, standard[i]);
        }
    }

    public void SetContextSearch()
    {
        for (int i = 3; i < 6; i++)
        {
            guide.SetControl(i, search[i-3]);
        }
    }

    public void SetContextTopics()
    {
        for (int i = 3; i < 6; i++)
        {
            guide.SetControl(i, topics[i-3]);
        }
    }
}
