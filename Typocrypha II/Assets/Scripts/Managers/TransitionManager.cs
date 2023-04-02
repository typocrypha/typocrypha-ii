using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Gameflow;

/// <summary>
/// Manages transitions between scenes.
/// </summary>
public class TransitionManager : MonoBehaviour
{
    private const string battleSceneName = "Battle";
    private const string dialogSceneName = "Dialog";
    public static TransitionManager instance = null;
    [SerializeField] private LoadingScreen defaultLoadingScreen; // Loading screen prefab
    [SerializeField] private Canvas loadingScreenCanvas;
    [SerializeField] private int debugContinueIndex;
    [SerializeField] private List<SceneData> sceneData;

    private int sceneIndex = -1;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;

        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToNextScene()
    {
        if (sceneIndex + 1 >= sceneData.Count)
        {
            Debug.LogError("Already at the last scene of the game!");
            return;
        }
        TransitionToScene(sceneIndex + 1);
    }

    public void TransitionToScene(int newIndex)
    {
        if (sceneIndex == newIndex)
            return;
        var currScene = sceneIndex >= 0 ? sceneData[sceneIndex].SceneName : string.Empty;
        var nextSceneData = sceneData[newIndex];
        var nextScene = nextSceneData.SceneName;
        if (nextScene == currScene)
        {
            nextScene = string.Empty;
        }
        sceneIndex = newIndex;
        StartCoroutine(PlayLoadingScreen(nextSceneData.loadingScreenOverride, nextSceneData, nextScene));
    }

    public void Continue()
    {
#if DEBUG
        if(debugContinueIndex > 0)
        {
            TransitionToScene(debugContinueIndex);
            return;
        }
#endif
        // TODO: get scene number from saving
        TransitionToScene(0);
    }

    private IEnumerator PlayLoadingScreen(LoadingScreen loadingScreenOverride, SceneData data, string sceneName)
    {
        loadingScreenCanvas.enabled = true;
        // Play loading screen ON
        LoadingScreen loadingScreen = loadingScreenOverride ?? defaultLoadingScreen;
        loadingScreen.Progress = 0;
        yield return loadingScreen.StartLoading();
        // Start loading screen idle
        // (if new scene) actually load scene
        if(sceneName != string.Empty)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);
            loadOp.allowSceneActivation = true;
            // Internal progress will stop at 0.9 when done loading
            while (loadOp.progress < 0.9f)
            {
                loadingScreen.Progress = loadOp.progress;
                yield return null;
            }
            yield return new WaitUntil(() => loadOp.isDone);
        }
        else // fake load scene
        {
            // Cleanup singleton data (may not work for battle currently)
            if (DialogManager.instance != null)
            {
                DialogManager.instance.CleanUp();
            }
            var waitForFixed = new WaitForFixedUpdate();
            // Internal progress will stop at 0.9 when done loading
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.fixedDeltaTime;
                loadingScreen.Progress = Mathf.Round(progress * 100f) / 100f;
                yield return waitForFixed;
            }
        }
        loadingScreen.Progress = 1.0f;
        yield return null;
        // Initialize Scene
        if (data.sceneData is DialogCanvas dialogCanvas)
        {
            DialogManager.instance.StartDialog(dialogCanvas, true);
            yield return null;
        }
        // Initialize Battle
        if (data.sceneData is BattleCanvas battleCanvas)
        {
            BattleManager.instance.LoadBattle(battleCanvas);
        }
        yield return loadingScreen.FinishLoading();
        if (data.sceneData is BattleCanvas)
        {
            BattleManager.instance.StartBattle();
        }
        loadingScreenCanvas.enabled = false;
    }

    [System.Serializable]
    public class SceneData
    {
        public string Name => useCustomScene ? customSceneName : sceneData.name;
        public string SceneName => useCustomScene ? customSceneName : (sceneData is BattleCanvas ? battleSceneName : dialogSceneName);
        public NodeEditorFramework.NodeCanvas sceneData;
        public bool useCustomScene;
        public string customSceneName;
        public LoadingScreen loadingScreenOverride;
    }
}
