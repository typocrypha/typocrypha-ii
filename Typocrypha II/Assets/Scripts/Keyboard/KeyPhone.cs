using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Typocrypha
{
    public class KeyPhone : Key
    {
        [SerializeField] private TextMeshProUGUI tooltip;
        public override void SetLetter(char c)
        {
            base.SetLetter(c);
            tooltip.text = CastBarPhone.GetToolTipText(c);
        }
    }
}
