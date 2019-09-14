using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// Manages transitions between scenes.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance = null;
    public GameObject loadingScreenPrefab; // Loading screen prefab
    public GameObject currLoadingScreen; // Current loading screen
    public UnityEvent onStartScene; // What should be called when scene starts

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Transition to new scene. Plays loading screen.
    /// </summary>
    /// <param name="sceneName">Name of scene to transition to.</param>
    public void TransitionScene(string sceneName, GameObject loadingScreen = null)
    {
        loadingScreenPrefab = loadingScreen ?? loadingScreenPrefab;
        StartCoroutine(TransitionSceneCR(sceneName));
    }

    // Displays loading progress and switches scenes when done loading.
    IEnumerator TransitionSceneCR(string sceneName)
    {
        currLoadingScreen = Instantiate(loadingScreenPrefab, this.transform);
        LoadingScreen loadingScreen = currLoadingScreen.GetComponent<LoadingScreen>();
        yield return new WaitUntil(() => loadingScreen.ReadyToLoad);
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
        loadOp.allowSceneActivation = false;
        // Internal progress will stop at 0.9 when done loading
        while (loadOp.progress < 0.9f) 
        {
            loadingScreen.Progress = loadOp.progress;
            yield return null;
        }
        loadingScreen.Progress = 1.0f;
        yield return new WaitUntil(() => loadingScreen.DoneLoading);
        // Set animator flags (animator also triggers start of scene)
        yield return new WaitUntil(() => loadingScreen.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        loadingScreen.animator.SetBool("DoneLoading", true);
        loadOp.allowSceneActivation = true;
    }
}
