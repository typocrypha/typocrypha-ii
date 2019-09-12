using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages character sprites during dialog.
/// </summary>
public class DialogCharacterManager : MonoBehaviour, ISavable
{
    #region ISavable
    [System.Serializable]
    public struct CharacterSave
    {
        public string characterName;
        public string baseSprite;
        public string expression;
        public float xpos;
        public float ypos;
        public bool highlight;
    }

    public void Save()
    {
        var characters = new List<CharacterSave>();
        foreach(var kvp in characterMap) characters.Add(kvp.Value.saveData);
        SaveManager.instance.loaded.characters = characters;
    }

    public void Load()
    {
        foreach (var cs in SaveManager.instance.loaded.characters)
        {
            var data = characterDataBundle.LoadAsset<CharacterData>(cs.characterName);
            AddCharacter(data, cs.baseSprite, cs.expression, new Vector2(cs.xpos, cs.ypos));
            HighlightCharacter(data, cs.highlight);
        }
    }
    #endregion

    public static DialogCharacterManager instance = null;
    public GameObject characterPrefab; // Prefab of dialog character object
    public GameObject characterPrefabAyinCodec; // Prefab for ayin's codec object

    Dictionary<string, DialogCharacter> characterMap; // Map of string ids to characters in scene.
    static AssetBundle characterDataBundle; // All character data assets.

    const string defaultPose = "base";
    const string defaultExpr = "normal";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        characterMap = new Dictionary<string, DialogCharacter>();
        if (characterDataBundle == null)
            characterDataBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "characterdata"));
    }

    /// <summary>
    /// Adds a new character to the scene (given a CharacterData reference).
    /// </summary>
    /// <param name="data">CharacterData of character.</param>
    /// <param name="baseSprite">Starting pose.</param>
    /// <param name="expression">Starting expression.</param>
    /// <param name="pos">Starting position.</param>
    /// <returns>Reference to created character.</returns>
    public DialogCharacter AddCharacter(CharacterData data, string baseSprite, string expression, Vector2 pos)
    {
        GameObject go;
        if (data.aliases.Contains("Ayin")) // Codec position for ayin's character
        {
            go = Instantiate(characterPrefabAyinCodec, transform);
            pos = new Vector2(-5.5f, -4.75f);
        }
        else
        {
            go = Instantiate(characterPrefab, transform);
        }
        go.transform.position = pos;
        DialogCharacter dc = go.GetComponent<DialogCharacter>();
        characterMap[data.name] = dc;
        dc.TargetPosition = pos;
        ChangePose(data, baseSprite);
        ChangeExpression(data, expression);
        dc.saveData.characterName = data.name;
        return dc;
    }

    /// <summary>
    /// Adds a new character to the scene (given a CharacterData reference).
    /// </summary>
    /// <param name="data">CharacterData of character.</param>
    /// <param name="pos">Starting position.</param>
    /// <returns>Reference to created character.</returns>
    public DialogCharacter AddCharacter(CharacterData data, Vector2 pos)
    {
        return AddCharacter(data, defaultPose, defaultExpr, pos);
    }

    /// <summary>
    /// Removes character from scene.
    /// </summary>
    /// <param name="data">Id of character to remove.</param>
    public void RemoveCharacter(CharacterData data)
    {
        Destroy(characterMap[data.name].gameObject);
        characterMap.Remove(data.name);
    }

    /// <summary>
    /// Moves character immediately to new position.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="pos">Target position.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter TeleportCharacter(CharacterData data, Vector2 pos)
    {
        DialogCharacter dc = characterMap[data.name];
        dc.TargetPosition = pos;
        dc.transform.position = pos;
        return dc;
    }

    /// <summary>
    /// Linearly move character to position in a certain amount of time.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="pos">Target position.</param>
    /// <param name="time">Time to get to position.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter LerpCharacter(CharacterData data, Vector2 pos, float time)
    {
        DialogCharacter dc = characterMap[data.name];
        dc.TargetPosition = pos;
        StartCoroutine(LerpCharacterCR(dc, pos, time));
        return dc;
    }

    IEnumerator LerpCharacterCR(DialogCharacter dc, Vector2 targetPos, float time)
    {
        float currTime = 0f;
        Vector2 start = dc.transform.position;
        while (currTime < time)
        {
            dc.transform.position = Vector2.Lerp(start, targetPos, currTime / time);
            yield return new WaitForFixedUpdate();
            currTime += Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Smoothly move character with dampening to target position.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="pos">Target position.</param>
    /// <param name="time">Time to get to position.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter SmoothDampCharacter(CharacterData data, Vector2 pos, float time)
    {
        DialogCharacter dc = characterMap[data.name];
        dc.TargetPosition = pos;
        StartCoroutine(SmoothDampCharacterCR(dc, pos, time));
        return dc;
    }

    IEnumerator SmoothDampCharacterCR(DialogCharacter dc, Vector2 targetPos, float time)
    {
        Vector2 vel = Vector2.zero;
        while (Vector2.Distance(dc.transform.position, targetPos) > 0.01f)
        {
            dc.transform.position = Vector2.SmoothDamp(dc.transform.position, targetPos, ref vel, time);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Changes the pose of a character.
    /// Generally, should be accompanied by a change in expression.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="baseSprite">New pose/base sprite.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter ChangePose(CharacterData data, string baseSprite)
    {
        // FIXED POSE
        //characterMap[data.name].PoseSprite = data.poses[baseSprite];
        //characterMap[data.name].saveData.baseSprite = baseSprite;
        // TEMP: Uses the default of each element (e.g. Illyia's base body, base clothes, and base hair)
        // LATER: string manip? "base_1", "base_halloween"
        ChangeBCH(data, baseSprite, baseSprite, baseSprite);
        characterMap[data.name].saveData.baseSprite = baseSprite;
        return characterMap[data.name];
    }

    /// <summary>
    /// Changes the expression of a character.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="expression">New expression.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter ChangeExpression(CharacterData data, string expression)
    {
        string expr = expression.ToLower().Replace("default", "normal");
        if (data.expressions.ContainsKey(expr))
        {
            characterMap[data.name].ExprSprite = data.expressions[expr];
            characterMap[data.name].saveData.expression = expr;
        }
        else
        {
            Debug.LogError("No such expression:" + expr);
        }
        return characterMap[data.name];
    }

    /// <summary>
    /// Changes the Body, Clothes, and Hair of a character.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="body">Body id (no change if blank).</param>
    /// <param name="clothes">Clothes id (no change if blank).</param>
    /// <param name="hair">Hair id (no change if blank).</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter ChangeBCH(CharacterData data, string body, string clothes, string hair)
    {
        try
        {
            if (body != "") characterMap[data.name].BodySprite = data.bodies[body];
            if (clothes != "") characterMap[data.name].ClothesSprite = data.clothes[clothes];
            if (hair != "") characterMap[data.name].HairSprite = data.hair[hair];
        }
        catch
        {
            Debug.LogError("No such pose:" + body);
        }
        return characterMap[data.name];
    }

    /// <summary>
    /// Turns highlight on or off on a character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="on">Whether to turn on highlight.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter HighlightCharacter(CharacterData data, bool on)
    {
        Debug.Log(data.name);
        characterMap[data.name].Highlight(on);
        return characterMap[data.name];
    }

    /// <summary>
    /// Turns highlight on or off for all characters.
    /// </summary>
    /// <param name="on">Whether to turn on highlight.</param>
    public void HighlightAllCharacters(bool on)
    {
        foreach (var kvp in characterMap)
        {
            kvp.Value.Highlight(on);
        }
    }

    /// <summary>
    /// Highlight one character and unhighlight all others.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter SoloHighlightCharacter(CharacterData data)
    {
        HighlightAllCharacters(false);
        characterMap[data.name].Highlight(true);
        return characterMap[data.name];
    }

    /// <summary>
    /// Apply animation clip to character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="clip">Animation clip to apply.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public DialogCharacter AnimateCharacter(CharacterData data, AnimationClip clip)
    {
        if (clip.isLooping) // Set idle animation to looped clip.
        {
            characterMap[data.name].overrideAnimator[DialogCharacter.idleAnimationClip] = clip;
            characterMap[data.name].animator.Play(DialogCharacter.idleAnimatorState);
        }
        else // Play animation once. 
        {
            characterMap[data.name].animator.ResetTrigger("Reset");
            characterMap[data.name].overrideAnimator[DialogCharacter.onceAnimationClip] = clip;
            characterMap[data.name].animator.Play(DialogCharacter.onceAnimatorState);
        }
        return characterMap[data.name];
    }

    /// <summary>
    /// Reset certain dynamic properties of dialog characters.
    /// Resets animation state to idle animation.
    /// </summary>
    public void ResetCharacters()
    {
        StopAllCoroutines();
        foreach (var dc in characterMap.Values)
        {
            dc.transform.position = dc.TargetPosition;
            dc.PivotPosition = Vector2.zero;
            dc.animator.SetTrigger("Reset");
        }
    }

    // Used for memoizing character data 
    Dictionary<string, CharacterData> memoizeCD = new Dictionary<string, CharacterData>();

    /// <summary>
    /// Finds character data (in scene) that matches given alias.
    /// Will return null if alias exists, but that character isn't in the scene.
    /// </summary>
    /// <param name="alias">Alias of character you want to find.</param>
    /// <returns>CharacterData found. 'null' if none found.</returns>
    public CharacterData CharacterDataByName(string alias)
    {
        if (memoizeCD.ContainsKey(alias))
        {
            return memoizeCD[alias];
        }
        else
        {
            foreach(var cd in characterDataBundle.LoadAllAssets<CharacterData>())
            {
                if (cd.aliases.Contains(alias))
                {
                    memoizeCD[alias] = cd;
                    return cd;
                }
            }
        }
        return null;
    }
}
