﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB2
{
    // Events sent from allies
    public partial class StateManager : MonoBehaviour
    {
        // Start ally's charging of mana
        // Called when Start state is exited

        //public void allyStartMana(StateEventArgs args)
        //{
        //    ((Ally)args.actor).startMana();
        //}

        // Opens ally's cast menu; also checks cast conditions
        // Sent when ally cast is triggered

        //public void allyMenu(StateEventArgs args)
        //{
        //    if (soloStack.Count == 0 || args.actor.isCast)
        //        return;
        //    Ally ally = (Ally)args.actor;
        //    bool cast = false;
        //    if (soloStack.Peek().isCurrentState("BeforeCast"))
        //        cast = true;
        //    if (soloStack.Peek().isCurrentState("AfterCast"))
        //        cast = true;
        //    //Check mana cost
        //    if (ally.mana < ally.manaCost)
        //        return;
        //    if (cast)
        //    {
        //        enterSolo(ally);
        //        CastBar.enterSolo(ally.castBar);
        //        ally.castBar.focus = true;
        //        ally.isCast = true;
        //        args.actor.stateMachine.Play("AllyMenu");
        //    }
        //}

        // Starts ally's cast sequence
        // Sent when ally enters into the cast bar

        //public void allyStartCast(StateEventArgs args)
        //{
        //    Ally ally = (Ally)args.actor;
        //    ally.castBar.focus = false;
        //    ally.stateMachine.Play("BeforeCast");
        //}

        // Apply Ally's cast effects
        // Sent when ally enters Cast state

        //public void allyCast(StateEventArgs args)
        //{
        //    Ally ally = (Ally)args.actor;
        //    ally.mana -= ally.manaCost;
        //    Debug.Log(args.actor.gameObject.name + " has cast a spell");
        //}

        // End ally's cast sequence
        // Send when ally endtes CastDisabled state

        //public void allyEndCast(StateEventArgs args)
        //{
        //    Ally ally = (Ally)args.actor;
        //    exitSolo(ally);
        //    CastBar.exitSolo(ally.castBar);
        //    ally.castBar.clear();
        //}
    }
}

