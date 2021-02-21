using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    public class KeyPhone : Key
    {
        public TextMeshPro tooltip;
        public override void SetText(char c)
        {
            base.SetText(c);
            tooltip.text = CastBarPhone.GetToolTipText(c);
        }
    }
}
