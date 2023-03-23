using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// plays effect animations
public class AnimationPlayer : MonoBehaviour
{
	public static AnimationPlayer instance = null; // global static ref
	public GameObject animationHolderPrefab; // object prefab that holds the animations

    void Awake() {
		
		if (instance == null)
        {
            instance = this;
        }          
		else Destroy (gameObject); // avoid multiple copies
	}
	
    //Plays any one-shot animation clip and returns the play time as a float
    public CompletionData Play(AnimationClip clip, Vector2 pos, float speed = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Null animation clip, returning empty completion data");
            return new CompletionData();
        }
        //Create animation holder
        GameObject display = Instantiate(animationHolderPrefab);
        display.transform.position = pos;
        //Set animation speed
        var animator = display.GetComponent<Animator>();
        animator.speed = speed;
        //Override animation clip
        var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);       
        overrideController["OneShot"] = clip;
        animator.runtimeAnimatorController = overrideController;
        animator.Play("OneShot", 0, 0f);
        //Set and return completion data
        CompletionData data = display.GetComponent<AnimationHolder>().completionData;
        data.time = (clip.length * 1/speed);
        return data;
    }

    //Contains a time and a completion trigger
    public class CompletionData
    {
        public float time = 0;
        public bool keepPlaying = true;
    }
}

//Wait until the given CompletionData's time has elapsed, or the end trigger is set, whichever comes first
public class WaitUntilAnimComplete : CustomYieldInstruction
{
    AnimationPlayer.CompletionData data;
    private float elapsedTime = 0;
    public override bool keepWaiting
    {
        get
        {
            return data.keepPlaying && ((elapsedTime += Time.deltaTime) < data.time);
        }
    }
    public WaitUntilAnimComplete(AnimationPlayer.CompletionData data)
    {
        this.data = data;
    }
}
