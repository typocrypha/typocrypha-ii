using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using System;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(true, "Event/Audio Control", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public abstract class AudioControlNode : BaseNodeIO
    {
        //public const string ID = "Audio Control Node";
        //public override string GetID { get { return ID; } }

        //public override string Title { get { return "Audio Control"; } }
        //public override Vector2 MinSize { get { return new Vector2(200, 20); } }



        //[SerializeField]
        //List<EventData> _events;
        ////public ReorderableSOList<EventData> events = null;
        //protected override void OnCreate()
        //{
        //    _events = new List<EventData>();
        //}

        //public override ScriptableObject[] GetScriptableObjects()
        //{
        //    return _events.ToArray();
        //}

        //protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        //{
        //    for (int i = 0; i < _events.Count; ++i)
        //        _events[i] = (EventData)replaceSO(_events[i]);
        //}

        //public override void NodeGUI()
        //{
        //    GUILayout.Space(5);
        //    GUILayout.BeginVertical("box");
        //    //ReoderableListGUI.DoLayoutList(_events);
        //    GUILayout.EndVertical();
        //}


        //public abstract class EventData : ScriptableObject// : ReorderableSOList<EventData>.ListItem
        //{
        //    //public override float Height
        //    //{
        //    //    get
        //    //    {
        //    //        return EditorGUIUtility.singleLineHeight + 1;
        //    //    }
        //    //}
        //}
    }
}
