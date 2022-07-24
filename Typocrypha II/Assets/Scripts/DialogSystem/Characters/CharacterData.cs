using UnityEngine;

// Container for data about specific characters (read only)
[CreateAssetMenu]
[System.Serializable]
public class CharacterData : ScriptableObject
{
    public string mainAlias;
    public NameSet aliases; // Different aliases/names for this character
    public PoseMap poses; // Different body poses
    public NameMap expressions; // Different facial expressions
    public NameMap bodies; // Different base bodies
    public NameMap clothes; // Different clothes
    public NameMap hair; // Different hair
    public NameMap codecs; // Different codec sprites
    public Sprite chat_icon; // Chat mode sprite
    public AudioClip talk_sfx; // Talking sound effect
    public Color characterColorDark = Color.white;
    public Color characterColorLight = Color.white;
    public Color characterHighlightColorLeft = Color.white;
    public Color characterHighlightColorRight = Color.white;

    [System.Serializable]
    public class PoseData
    {
        public Sprite pose;
        public float xCenterNormalized;
        public float yHeadTopNormalized;
    }

    public bool IsNamed(string alias)
    {
        return aliases.Contains(alias);
    }
}

// Serializable wrapper for dictionaries
[System.Serializable]
public class NameMap : SerializableDictionary<string, Sprite> { [System.NonSerialized] public string addField; }

// Serializable wrapper for sets
[System.Serializable]
public class NameSet : SerializableSet<string> { [System.NonSerialized] public string addField; }

// Serializable wrapper for dictionaries
[System.Serializable]
public class PoseMap : SerializableDictionary<string, CharacterData.PoseData> {[System.NonSerialized] public string addField; }

