﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellFxData
{
    public enum EffectType
    {
        None,
        Single,
        Sequence,
        Parallel,
        Prefab
    }
    public EffectType effectType = EffectType.Single;
    public GameObject effectPrefab = null;
    public List<AnimationPacket> effectPackets = new List<AnimationPacket>() { new AnimationPacket() };

    public IEnumerator Play(Vector2 pos)
    {
        if (effectType == EffectType.None)
            yield break;
        if (effectType == EffectType.Single)
        {
            SoundController.instance.PlaySingle(effectPackets[0].sfx);
            yield return new WaitUntilAnimComplete(AnimationPlayer.instance.Play(effectPackets[0].clip, pos));
        }           
        else if (effectType == EffectType.Sequence)
        {
            foreach (var packet in effectPackets)
            {
                SoundController.instance.PlaySingle(packet.sfx);
                yield return new WaitUntilAnimComplete(AnimationPlayer.instance.Play(packet.clip, pos));
            }
                
        }
        else if(effectType == EffectType.Parallel)
        {
            for (int i = 1; i < effectPackets.Count; ++i)
            {
                SoundController.instance.PlaySingle(effectPackets[i].sfx);
                AnimationPlayer.instance.Play(effectPackets[i].clip, pos);
            }
            if (effectPackets.Count > 0)
                yield return new WaitUntilAnimComplete(AnimationPlayer.instance.Play(effectPackets[0].clip, pos));
        }
        else if(effectType == EffectType.Prefab)
        {
            if (effectPrefab != null)
            {
                var effectVisual = GameObject.Instantiate(effectPrefab);
                effectVisual.transform.position = pos;
                var fxComponent = effectVisual.GetComponent<SpellFxBase>();
                if (fxComponent != null)
                    yield return fxComponent.StartCoroutine(fxComponent.PlayEffect());
            }
        }
    }
}
