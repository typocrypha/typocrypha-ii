﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundElementAnimator : MonoBehaviour
{

    public enum AnimationType
    {
        Position,
        Rotation,
        Scale
    }

    // axis of movement
    public enum MovementAxis
    {
        X,
        Y
    }

    // collection of animations
    [SerializeField]
    AnimatedElement[] animations;


    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        foreach (AnimatedElement animation in animations) StartAnimation(animation);
    }

    void StartAnimation(AnimatedElement anim)
    {
        switch(anim.animationType)
        {
            case AnimationType.Position:
                
                Vector3 movement = Vector3.zero;
                if (anim.movementOrScaleAxis == MovementAxis.X) movement.x = anim.targetPositionOffset;
                else movement.y = anim.targetPositionOffset;
                
                if (!anim.useCustomCurve)
                {
                    anim.transformToAnimate.DOBlendableMoveBy(movement, anim.animationSpeed, false)
                        .SetEase(anim.animationCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }
                else
                {
                    anim.transformToAnimate.DOBlendableMoveBy(movement, anim.animationSpeed, false)
                        .SetEase(anim.customCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }

                break;

            case AnimationType.Rotation:

                Vector3 rotation = new Vector3(0f, 0f, Mathf.Deg2Rad * anim.targetRotation);
                if (!anim.useCustomCurve)
                {
                    anim.transformToAnimate.DOBlendableLocalRotateBy(rotation, anim.animationSpeed, RotateMode.LocalAxisAdd)
                        .SetEase(anim.animationCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }
                else
                {
                    anim.transformToAnimate.DOBlendableRotateBy(rotation, anim.animationSpeed, RotateMode.LocalAxisAdd)
                        .SetEase(anim.customCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }

                break;

            case AnimationType.Scale:

                Vector3 scale = Vector3.zero;
                if (anim.movementOrScaleAxis == MovementAxis.X) scale.x = anim.targetPositionOffset;
                else scale.y = anim.targetPositionOffset;

                if (!anim.useCustomCurve)
                {
                    anim.transformToAnimate.DOBlendableMoveBy(scale, anim.animationSpeed, false)
                        .SetEase(anim.animationCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }
                else
                {
                    anim.transformToAnimate.DOBlendableMoveBy(scale, anim.animationSpeed, false)
                        .SetEase(anim.customCurve)
                        .SetLoops(-1, anim.loopBehavior)
                        .SetDelay(anim.animationStartOffset);
                }

                break;

            default:
                break;
        }
    }

    [System.Serializable]
    struct AnimatedElement
    {
        public Transform transformToAnimate;
        public AnimationType animationType;
        public MovementAxis movementOrScaleAxis;
        [Range(0.1f, 1000.0f)] public float targetScale;
        [Range(-10f, 10f)] public float targetPositionOffset;
        [Range(0f, 360f)] public float targetRotation;

        public Ease animationCurve;
        public bool useCustomCurve;
        public AnimationCurve customCurve;
        [Range(0.1f, 25.0f)] public float animationSpeed;
        [Range(-10.0f, 10.0f)] public float animationStartOffset;
        public LoopType loopBehavior;

    }

    
}