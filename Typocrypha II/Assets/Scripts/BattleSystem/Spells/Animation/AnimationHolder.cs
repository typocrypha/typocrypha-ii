using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHolder : MonoBehaviour 
{
    private const string oneShotStateName = "OneShot";

    [SerializeField] private Animator animator;

    public bool IsCompleted() => completed;
    private bool completed = false;
    private AnimatorOverrideController overrideController;
    private System.Action onComplete;

    private void Awake()
    {
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    public void AnimationComplete()
    {
        completed = true;

    }

    public void OnComplete()
    {
        onComplete?.Invoke();
        onComplete = null;
    }

    public void Play(AnimationClip clip, Vector2 pos, float speed, System.Action onFinished)
    {
        this.onComplete = onFinished;
        transform.position = pos;
        //Set animation speed
        animator.speed = speed;
        completed = false;
        overrideController[oneShotStateName] = clip;
        animator.Play(oneShotStateName, 0, 0f);
    }
}
