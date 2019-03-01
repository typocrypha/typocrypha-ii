using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell
{
    // Spell tag dictionary
    // Accessible spelleffects (already modified or list of modifications)
    
    // Constructor(list of spell word object)
        //Search for modifiers. apply modifiers

    public void Cast (Caster caster, Caster target)
    {
        //Foreach root?
        //Foreach spell effect
            //Target the effect (return list of positions)
            //Foreach target position
                //if no target
                    //no target visualization
                //else
                    //Apply effect to target (log effect data)
                    //Spawn visualization
                    //Break if enemy killed?
        //Do total visualization?
        //When do enemies die?
    }
}
