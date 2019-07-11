using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpellTag))]
[CanEditMultipleObjects]
public class SpellTagInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Set internal name to asset name"))
        {
            foreach(var t in targets)
            {
                var tag = t as SpellTag;
                tag.internalName = tag.name;
            }
        }
    }
}
