using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Typocrypha
{
    public class KeyboardSwitcher : MonoBehaviour
    {
        public Keyboard keyboard;
        public KeyboardBuilder mainBuilder;
        public KeyboardBuilder altBuilder;

        public CastBar castBar;
        public CastBar altCastBar;

        private bool inAltMode = false;

        public KeyCode debugKey = KeyCode.LeftBracket;

        public void Switch()
        {
            keyboard.Clear();
            if(inAltMode = !inAltMode)
            {
                mainBuilder.ClearKeyboard();
                altBuilder.BuildKeyboard();
                keyboard.castBar = altCastBar;
                keyboard.Initialize(altBuilder.Keys);
            }
            else
            {
                altBuilder.ClearKeyboard();
                mainBuilder.BuildKeyboard();
                keyboard.castBar = castBar;
                keyboard.Initialize(mainBuilder.Keys);
            }
            castBar.gameObject.SetActive(!inAltMode);
            altCastBar.gameObject.SetActive(inAltMode);
        }

        [Conditional("DEBUG")]
        private void Update()
        {
            if(Input.GetKeyDown(debugKey))
            {
                Switch();
            }
        }
    }
}
