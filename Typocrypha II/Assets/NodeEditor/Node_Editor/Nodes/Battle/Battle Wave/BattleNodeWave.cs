using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(false, "Battle/Wave", new System.Type[] { typeof(BattleCanvas) })]
    public class BattleNodeWave : BaseNodeIO
    {
        public const string ID = "Battle Wave Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Battle Wave"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 60); } }

        public Battlefield.ClearOptions clearFieldOptions;
        public string waveTitle;
        public string waveNumberOverride;
        public AudioClip music;
        public GOMatrix2D battleField;

        private RListGUI<GameObject> eventGUI;
        public List<GameObject> battleEvents;

        private RListGUI<GameObject> reinforcementsGUI;
        public List<GameObject> reinforcements;

        #region Tooltip Strings
        private static string tooltip_music = "Music to play. Leave as None to keep the previous music playing";
        #endregion

        protected override void OnCreate()
        {
            battleField = new GOMatrix2D(2, 3);
            battleEvents = new List<GameObject>();
            reinforcements = new List<GameObject>();
            clearFieldOptions = Battlefield.ClearOptions.ClearEnemies | Battlefield.ClearOptions.ClearObjects;
        }

        public override void NodeGUI()
        {

            #region Wave Transition GUI
            GUILayout.Space(4);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Wave Transition", "TODO: Tooltip"), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Title"), GUI.skin.label, GUILayout.Width(50));
            waveTitle = GUILayout.TextField(waveTitle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Num Override"), GUI.skin.label, GUILayout.Width(80));
            waveNumberOverride = GUILayout.TextField(waveNumberOverride);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Music", tooltip_music), GUI.skin.label, GUILayout.Width(50));
            music = RTEditorGUI.ObjectField(music, false) as AudioClip;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion

            #region Battlefield Data GUI
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Battlefield Data", "TODO: Tooltip"), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Clear"), GUILayout.Width(45));
            clearFieldOptions = (Battlefield.ClearOptions)RTEditorGUI.EnumFlagsPopup(clearFieldOptions);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            float objHeight = RTEditorGUI.lineHeight * 1.125f;
            float objWidth = rect.width * 0.33f - 7;
            battleField[0, 0] = RTEditorGUI.ObjectField(battleField[0, 0], false, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            battleField[0, 1] = RTEditorGUI.ObjectField(battleField[0, 1], false, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            battleField[0, 2] = RTEditorGUI.ObjectField(battleField[0, 2], false, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            battleField[1, 0] = RTEditorGUI.ObjectField(battleField[1, 0], false, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            GUILayout.Label(new GUIContent("Player"), NodeEditorGUI.nodeLabelCentered, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            battleField[1, 2] = RTEditorGUI.ObjectField(battleField[1, 2], false, GUILayout.Width(objWidth), GUILayout.Height(objHeight));
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            GUILayout.EndVertical();
            #endregion

            #region Battle Events GUI
            if(eventGUI == null)
            {
                void EltGUI(GameObject element, int ind, Rect rect)
                {
                    element = RTEditorGUI.ObjectFieldRect(rect, element, false);
                    battleEvents[ind] = element;
                }
                GameObject NewItem() => null;
                eventGUI = new RListGUI<GameObject>(battleEvents, new GUIContent("Battle Event Prefabs"), EltGUI, (e) => RTEditorGUI.lineHeight, NewItem);
            }
            eventGUI.DoLayoutList();
            #endregion

            #region Reinforcements GUI
            if(reinforcementsGUI == null)
            {
                void EltGUI(GameObject element, int ind, Rect rect)
                {
                    element = RTEditorGUI.ObjectFieldRect(rect, element, false);
                    reinforcements[ind] = element;
                }
                GameObject NewItem() => null;
                reinforcementsGUI = new RListGUI<GameObject>(reinforcements, new GUIContent("Reinforcements"), EltGUI, (e) => RTEditorGUI.lineHeight, NewItem);
            }
            reinforcementsGUI.DoLayoutList();
            #endregion

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }

    }
}
