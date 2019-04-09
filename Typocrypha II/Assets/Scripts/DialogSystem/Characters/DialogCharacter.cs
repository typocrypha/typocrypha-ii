using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a character sprite in a dialog scene.
/// </summary>
public class DialogCharacter : MonoBehaviour
{
    public Transform pivotTr; // Transform of pivot position.
    public Vector2 PivotPosition
    {
        get => pivotTr.localPosition;
        set => pivotTr.localPosition = value;
    }
    public SpriteRenderer baseSprite; // Base sprite renderer (the pose).
    public SpriteRenderer exprSprite; // Expression sprite renderer (face).
    public Animator animator; // Animator for character.
    [HideInInspector]public AnimatorOverrideController overrideAnimator; // Override animator.
    [HideInInspector]public DialogCharacterManager.CharacterSave saveData; // Serializable state.

    public const string idleAnimatorState = "Idle";
    public const string onceAnimatorState = "Once";
    public const string idleAnimationClip = "DialogCharacterIdle";
    public const string onceAnimationClip = "DialogCharacterOnce";

    void Awake()
    {
        overrideAnimator = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideAnimator;
    }

    public Sprite Pose // Set/get the character's pose.
    {
        get
        {
            return baseSprite.sprite;
        }
        set
        {
            baseSprite.sprite = value;
        }
    }

    public Sprite Expr // Set/get the character's expression.
    {
        get
        {
            return exprSprite.sprite;
        }
        set
        {
            exprSprite.sprite = value;
        }
    }

    // If moving, position character is going to; Otherwise, just current position. 
    // Used for resetting position when skipping.
    Vector2 targetPosition;
    public Vector2 TargetPosition
    {
        get => targetPosition;
        set
        {
            targetPosition = value;
            saveData.xpos = value.x;
            saveData.ypos = value.y;
        }
    }

    /// <summary>
    /// Highlight a character on or off.
    /// </summary>
    /// <param name="on">Whether to to turn on highlight.</param>
    public void Highlight(bool on)
    {
        SpriteRenderer[] allSR = GetComponentsInChildren<SpriteRenderer>();
        foreach(var sr in allSR)
        {
            if (on)
            {
                sr.color = Color.white;
            }
            else
            {
                sr.color = DialogCharacterManager.instance.hideTint;
            }
        }
        saveData.highlight = on;
    }
}
