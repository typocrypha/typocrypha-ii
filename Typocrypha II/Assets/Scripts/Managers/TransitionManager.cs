using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages transitions between scenes.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance = null;
    public GameObject loadingScreenPrefab; // Loading screen prefab

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
        GameObject obj = Instantiate(loadingScreenPrefab, this.transform);
        LoadingScreen loadingScreen = obj.GetComponent<LoadingScreen>();
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
        loadOp.allowSceneActivation = true;
    }
}
