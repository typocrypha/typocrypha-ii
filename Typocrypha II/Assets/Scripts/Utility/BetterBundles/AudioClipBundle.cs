using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bundle/Audio")]
public class AudioClipBundle : BetterBundle<AudioClip>
{
    public BadgeDictionary clips;

    public override void Add(AudioClip item)
    {
        clips[item.name] = item;
    }

    public override void Clear()
    {
        clips.Clear();
    }

    [System.Serializable] public class BadgeDictionary : SerializableDictionary<string, AudioClip> { };
}
