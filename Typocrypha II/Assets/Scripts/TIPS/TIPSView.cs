using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class TIPSView : MonoBehaviour
{
    public const int TOPIC_PANEL_SIZE = 7;

    public void SelectFirstTopic()
    {

    }

    public void PopulateTopics(string[] topics, bool subMenu, int page)
    {
        int pageCount = topics.Length / TOPIC_PANEL_SIZE + 1;
        SetPageCounter(subMenu, page, pageCount);
        if (!subMenu) HideSubMenu();
    }

    private void SetPageCounter(bool subMenu, int pageNumber, int pageCount)
    {

    }

    private void HideSubMenu()
    {
        throw new NotImplementedException();
    }
}
