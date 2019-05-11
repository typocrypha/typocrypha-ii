using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestRule : Rule
{
    public GameObject popupPrefab;
    public SpellTag tagToBan;
    // Start is called before the first frame update
    void Start()
    {       
        Restrictions += BanTag;
        ActiveRule = this;
    }
    private bool BanTag(Spell s,Caster c,Battlefield.Position t, bool b)
    {
        var modRoots = SpellManager.instance.Modify(s);
        bool ret = modRoots.Any((root) => root.effects.Any((effect) => effect.tags.Contains(tagToBan)));
        if(ret)
        {
            var popper = GameObject.Instantiate(popupPrefab).GetComponent<PopupBase>();
            popper.PopText(s.ToString() + " is banned.", Battlefield.instance.GetSpace(c.FieldPos), 0.75f);
        }
        return ret;
    }
}
