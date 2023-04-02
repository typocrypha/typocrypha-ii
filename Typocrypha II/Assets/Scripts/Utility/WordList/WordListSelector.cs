using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;

public class WordListSelector
{
    private readonly IReadOnlyList<string> fullList;
    private readonly List<string> currentList;

    public WordListSelector(IReadOnlyList<string> options)
    {
        fullList = options;
        currentList = new List<string>(fullList);
    }

    public void Reset()
    {
        currentList.Clear();
        currentList.AddRange(fullList);
    }

    public string Get()
    {
        if(currentList.Count <= 0)
        {
            Reset();
        }
        var output =  RandomU.instance.Choice(currentList, out int index);
        currentList.RemoveAt(index);
        return output;
    }
}
