using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(RootWordEffect), true)]
public class RootWordEffectInspector : Editor
{
    protected RListGUI<SpellAnimationPacket> rList;
    protected void InitList()
    {
        var effect = target as RootWordEffect;
        RListGUI<SpellAnimationPacket>.ElementGUI eGUI = (elem, ind, rect) =>
        {
            Rect UIRect = new Rect(rect) { height = EditorGUIUtility.singleLineHeight, width = 32 };
            EditorGUI.PrefixLabel(UIRect, new GUIContent("Anim"));
            UIRect.x += 33;
            UIRect.width = 110;
            elem.clip = EditorGUI.ObjectField(UIRect, elem.clip, typeof(AnimationClip), false) as AnimationClip;
            UIRect.x += 111;
            UIRect.width = 30;
            EditorGUI.PrefixLabel(UIRect, new GUIContent("Sfx"));
            UIRect.x += 31;
            UIRect.width = 110;
            elem.sfx = EditorGUI.ObjectField(UIRect, elem.sfx, typeof(AudioClip), false) as AudioClip;
        };
        rList = new RListGUI<SpellAnimationPacket>(effect.effectPackets, new GUIContent("Sfx/Vfx"), eGUI, (elem, ind) => EditorGUIUtility.singleLineHeight, () => new SpellAnimationPacket());
    }
    private void OnEnable()
    {
        InitList();
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        var effect = target as RootWordEffect;
        EditorGUILayout.LabelField(new GUIContent("Animation and Sfx"));
        EditorUtils.Separator();
        effect.effectType = EditorUtils.EnumPopup(new GUIContent("EffectType"), effect.effectType);
        switch (effect.effectType)
        {
            case RootWordEffect.EffectType.Single:
                if (effect.effectPackets.Count <= 0)
                    effect.effectPackets.Add(new SpellAnimationPacket());
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Anim"), GUILayout.Width(32));
                effect.effectPackets[0].clip = EditorUtils.ObjectField(effect.effectPackets[0].clip, false);
                EditorGUILayout.LabelField(new GUIContent("Sfx"), GUILayout.Width(30));
                effect.effectPackets[0].sfx = EditorUtils.ObjectField(effect.effectPackets[0].sfx, false);
                EditorGUILayout.EndHorizontal();
                break;
            case RootWordEffect.EffectType.Sequence:
                rList.DoLayoutList();
                break;
            case RootWordEffect.EffectType.Parallel:
                rList.DoLayoutList();
                break;
            case RootWordEffect.EffectType.Prefab:
                effect.effectPrefab = EditorUtils.ObjectField(effect.effectPrefab, false);
                break;
        }
        EditorUtils.Separator();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pattern"));
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(this);
    }
}
