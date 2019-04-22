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
    [HideInInspector]public AnimatorOverrideController overrideAnimator; // Override animator.
    [HideInInspector]public DialogCharacterManager.CharacterSave saveData; // Serializable state.

    Material outlineMat; // Material for outline.

    public const string idleAnimatorState = "Idle";
    public const string onceAnimatorState = "Once";
    public const string idleAnimationClip = "DialogCharacterIdle";
    public const string onceAnimationClip = "DialogCharacterOnce";

    void Awake()
    {
        overrideAnimator = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideAnimator;
        outlineMat = new Material(outlineShader);
        outlineSprite.material = outlineMat;
    }

    void Start()
    {
        // Initialize outline
        BodySprite = BodySprite;
        ClothesSprite = ClothesSprite;
        HairSprite = HairSprite;
        OutlineThickness = 0.004f;
        OutlineColor = Color.white;
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
            outlineMat.SetTexture("_BodyTex", value.texture);
        }
    }

    public Sprite ClothesSprite // Set/get the character's clothes.
    {
        get => clothesSprite.sprite;
        set
        {
            clothesSprite.sprite = value;
            outlineMat.SetTexture("_ClothesTex", value.texture);
        }
    }

    public Sprite HairSprite // Set/get the character's expression.
    {
        get => hairSprite.sprite;
        set
        {
            hairSprite.sprite = value;
            outlineMat.SetTexture("_HairTex", value.texture);
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
        get => outlineMat.GetFloat("_OutlineSize");
        set => outlineMat.SetFloat("_OutlineSize", value);
    }

    /// <summary>
    /// Color of outline.
    /// </summary>
    public Color OutlineColor
    {
        get => outlineMat.GetColor("_OutlineColor");
        set => outlineMat.SetColor("_OutlineColor", value);
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
