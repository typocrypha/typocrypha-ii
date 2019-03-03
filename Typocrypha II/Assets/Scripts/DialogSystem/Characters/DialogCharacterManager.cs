using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages character sprites during dialog.
/// </summary>
public class DialogCharacterManager : MonoBehaviour
{
    public static DialogCharacterManager instance = null;
    public GameObject characterPrefab; // Prefab of dialog character object

    public Color hideTint; // Tint when character is not highlighted.
    Dictionary<CharacterData, DialogCharacter> characterMap; // Map of string ids to characters in scene.

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

        characterMap = new Dictionary<CharacterData, DialogCharacter>();
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
        GameObject go = Instantiate(characterPrefab, transform);
        go.transform.position = pos;
        DialogCharacter dc = go.GetComponent<DialogCharacter>();
        dc.Pose = data.poses[baseSprite];
        dc.Expr = data.expressions[expression];
        characterMap[data] = dc;
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
        GameObject go = Instantiate(characterPrefab, transform);
        go.transform.position = pos;
        DialogCharacter dc = go.GetComponent<DialogCharacter>();
        dc.Pose = data.poses[defaultPose];
        dc.Expr = data.expressions[defaultExpr];
        characterMap[data] = dc;
        return dc;
    }

    /// <summary>
    /// Removes character from scene.
    /// </summary>
    /// <param name="data">Id of character to remove.</param>
    public void RemoveCharacter(CharacterData data)
    {
        Destroy(characterMap[data].gameObject);
        characterMap.Remove(data);
    }

    /// <summary>
    /// Moves character immediately to new position.
    /// </summary>
    /// <param name="data">CharacterData of character.</param>
    /// <param name="pos">Target position.</param>
    /// <returns></returns>
    public DialogCharacter TeleportCharacter(CharacterData data, Vector2 pos)
    {
        DialogCharacter dc = characterMap[data];
        dc.transform.position = pos;
        return dc;
    }

    /// <summary>
    /// Changes the pose of a character.
    /// Generally, should be accompanied by a change in expression.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="pose">New pose/base sprite.</param>
    /// <returns>CharacterDialog component of selected character.</returns>
    public DialogCharacter ChangePose(CharacterData data, string pose)
    {
        characterMap[data].Pose = data.poses[pose];
        return characterMap[data];
    }

    /// <summary>
    /// Changes the expression of a character.
    /// </summary>
    /// <param name="data">Id of character selected.</param>
    /// <param name="expression">New expression.</param>
    /// <returns>CharacterDialog component of selected character.</returns>
    public DialogCharacter ChangeExpression(CharacterData data, string expression)
    {
        characterMap[data].Expr = data.expressions[expression];
        return characterMap[data];
    }

    /// <summary>
    /// Turns highlight on or off on a character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="on">Whether to turn on highlight.</param>
    /// <returns>CharacterDialog component of selected character.</returns>
    public DialogCharacter HighlightCharacter(CharacterData data, bool on)
    {
        characterMap[data].Highlight(on);
        return characterMap[data];
    }

    /// <summary>
    /// Highlight one character and unhighlight all others.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <returns>CharacterDialog component of selected character.</returns>
    public DialogCharacter SoloHighlightCharacter(CharacterData data)
    {
        foreach(var kvp in characterMap)
        {
            kvp.Value.Highlight(false);
        }
        characterMap[data].Highlight(true);
        return characterMap[data];
    }

    /// <summary>
    /// Apply animation clip to character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="clip">Animation clip to apply.</param>
    /// <returns>CharacterDialog component of selected character.</returns>
    public DialogCharacter AnimateCharacter(CharacterData data, AnimationClip clip)
    {
        if (clip.isLooping) // Set idle animation to looped clip.
        {
            characterMap[data].overrideAnimator[DialogCharacter.idleAnimationClip] = clip;
            characterMap[data].animator.Play(DialogCharacter.idleAnimatorState);
        }
        else // Play animation once. 
        {
            characterMap[data].animator.ResetTrigger("Reset");
            characterMap[data].overrideAnimator[DialogCharacter.onceAnimationClip] = clip;
            characterMap[data].animator.Play(DialogCharacter.onceAnimatorState);
        }
        return characterMap[data];
    }

    /// <summary>
    /// Reset certain dynamic properties of dialog characters.
    /// Resets animation state to idle animation.
    /// </summary>
    public void ResetCharacters()
    {
        foreach (var dc in characterMap.Values)
        {
            dc.PivotPosition = Vector2.zero;
            dc.animator.SetTrigger("Reset");
        }
    }

    /// <summary>
    /// Finds character data (in scene) that matches given alias.
    /// </summary>
    /// <param name="alias">Alias of character you want to find.</param>
    /// <returns>CharacterData found. 'null' if none found.</returns>
    public CharacterData CharacterDataByName(string alias)
    {
        foreach(var kvp in characterMap)
        {
            if (kvp.Key.aliases.Contains(alias))
            {
                return kvp.Key;
            }
        }
        return null;
    }
}
