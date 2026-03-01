using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Audio")]
public class AudioClipBundle : BetterBundle<AudioClip>
{
    public ClipDictionary clips;

    public override void Add(AudioClip item)
    {
        clips[item.name] = item;
    }

    public override void Clear()
    {
        clips.Clear();
    }

    [System.Serializable] public class ClipDictionary : SerializableDictionary<string, AudioClip> { };
}
