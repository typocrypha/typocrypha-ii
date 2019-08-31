using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCaster : MonoBehaviour
{
    public InputField field;
    public Player caster;
    public void Cast()
    {
        caster.CastString(field.text);
    }
}
