using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSOAttribute : PropertyAttribute
{
    public string name;
    public SubSOAttribute(string name)
    {
        this.name = name;
    }
}
