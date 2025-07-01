using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// plays effect animations
public class AnimationPlayer : MonoBehaviour
{
	public static AnimationPlayer instance = null; // global static ref
	[SerializeField] private GameObject animationHolderPrefab; // object prefab that holds the animations
    [SerializeField] private Canvas animationCanvas;

    private PrefabPool<AnimationHolder> animationHolderPool;

    void Awake() {
		
		if (instance == null)
        {
            instance = this;
            Initialize();
        }          
		else Destroy (gameObject); // avoid multiple copies
	}

    private void Initialize()
    {
        animationHolderPool = new PrefabPool<AnimationHolder>(animationHolderPrefab, animationCanvas.transform, 10);
    }
	
    //Plays any one-shot animation clip and returns the play time as a float
    public System.Func<bool> Play(AnimationClip clip, Vector2 pos, float speed = 1f)
    {
        if (clip == null)
        {
            Debug.LogError("Animation player was requested to play null animation clip");
            return () => true;
        }
        //Create animation holder
        var animationHolder = animationHolderPool.Get();
        void Release()
        {
            animationHolderPool.Release(animationHolder);
        }
        animationHolder.Play(clip, pos, speed, Release);
        return animationHolder.IsCompleted;
    }
}
