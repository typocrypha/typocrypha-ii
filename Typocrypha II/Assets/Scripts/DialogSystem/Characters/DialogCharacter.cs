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
    public SpriteRenderer poseSprite; // Base sprite renderer (the pose). Redundant if body/clothes/hair set.
    public SpriteRenderer exprSprite; // Expression sprite renderer (face).
    public SpriteRenderer bodySprite; // Body sprite renderer (naked body).
    public SpriteRenderer clothesSprite; // Clothes sprite renderer (clothes).
    public SpriteRenderer hairSprite; // Hair sprite renderer (hair).
    public SpriteRenderer outlineSprite; // Outline renderer.
    public Animator animator; // Animator for character.
    public Shader outlineShader; // Shader for outlining dialog characters.
    public Material OutlineMaterial
    {
        get => outlineSprite.material;
    }
    [HideInInspector]public AnimatorOverrideController overrideAnimator; // Override animator.
    [HideInInspector]public DialogCharacterManager.CharacterSave saveData; // Serializable state.

    public const string idleAnimatorState = "Idle";
    public const string onceAnimatorState = "Once";
    public const string idleAnimationClip = "DialogCharacterIdle";
    public const string onceAnimationClip = "DialogCharacterOnce";

    static int layerSeparator = 0; // Keeps track of layers to put characters on own layers.
    const int layerSeparation = 10; // Amount of layer separation.

    readonly Color defaultOutlineColor = Color.white * 0.5f;
    readonly Color highlightColor = Color.white;
    readonly Color noHighlightColor = Color.gray;

    void Awake()
    {
        overrideAnimator = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideAnimator;
        outlineSprite.material = new Material(outlineShader);
        // Separate character's layers from other characters
        // More recently added characters go on top by default
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.sortingOrder += layerSeparator;
        layerSeparator += layerSeparation;
        // Initialize outline
        OutlineThickness = 0.004f;
        OutlineColor = defaultOutlineColor;
        Highlight(false);
    }

    void Start()
    {

    }

    public Sprite PoseSprite // Set/get the character's pose.
    {
        get => poseSprite.sprite;
        set => poseSprite.sprite = value;
    }

    public Sprite ExprSprite // Set/get the character's expression.
    {
        get => exprSprite.sprite;
        set => exprSprite.sprite = value;
    }

    public Sprite BodySprite // Set/get the character's body.
    {
        get => bodySprite.sprite;
        set
        {
            bodySprite.sprite = value;
            outlineSprite.material.SetTexture("_BodyTex", value.texture);
        }
    }

    public Sprite ClothesSprite // Set/get the character's clothes.
    {
        get => clothesSprite.sprite;
        set
        {
            clothesSprite.sprite = value;
            outlineSprite.material.SetTexture("_ClothesTex", value.texture);
        }
    }

    public Sprite HairSprite // Set/get the character's expression.
    {
        get => hairSprite.sprite;
        set
        {
            hairSprite.sprite = value;
            outlineSprite.material.SetTexture("_HairTex", value.texture);
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
    /// Thickness of outline.
    /// </summary>
    public float OutlineThickness
    {
        get => outlineSprite.material.GetFloat("_OutlineSize");
        set => outlineSprite.material.SetFloat("_OutlineSize", value);
    }

    /// <summary>
    /// Color of outline.
    /// </summary>
    public Color OutlineColor
    {
        get => outlineSprite.material.GetColor("_OutlineColor");
        set
        {
            outlineColor = value;
            outlineSprite.material.SetColor("_OutlineColor", value);
        }
    }
    Color outlineColor;

    /// <summary>
    /// Highlight a character on or off.
    /// </summary>
    /// <param name="on">Whether to to turn on highlight.</param>
    public void Highlight(bool on)
    {
        // Highlight base character sprite
        SpriteRenderer[] allSR = GetComponentsInChildren<SpriteRenderer>();
        foreach(var sr in allSR)
        {
            if (on)
            {
                sr.color = highlightColor;
            }
            else
            {
                sr.color = noHighlightColor;
            }
        }
        // Highlight outline
        if (on) outlineSprite.material.SetColor("_OutlineColor", outlineColor);
        else    outlineSprite.material.SetColor("_OutlineColor", outlineColor * 0.3f);
        // Save highlight state
        saveData.highlight = on;
    }
}
