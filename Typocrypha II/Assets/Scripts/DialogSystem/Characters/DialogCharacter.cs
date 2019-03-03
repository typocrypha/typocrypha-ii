using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a character sprite in a dialog scene.
/// </summary>
public class DialogCharacter : MonoBehaviour
{
    public SpriteRenderer baseSprite; // Base sprite renderer (the pose).
    public SpriteRenderer exprSprite; // Expression sprite renderer (face).
    public Animator animator; // Animator for character.
    [HideInInspector]public AnimatorOverrideController overrideAnimator; // Override animator.

    public const string baseAnimatorState = "Base";
    public const string baseAnimationClip = "DialogCharacterBase";

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
    }
}
