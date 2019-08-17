using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugShowTags : MonoBehaviour
{
    public Text text;
    private Caster caster;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(caster == null)
        {
            caster = Battlefield.instance.Player;
            if (caster == null)
                return;
        }
        text.text = caster.TagDict.ToString();
    }
}
