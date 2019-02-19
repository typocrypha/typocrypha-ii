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
}
