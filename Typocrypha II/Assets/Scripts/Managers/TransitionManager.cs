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
    [SerializeField] private List<SceneData> sceneData;
    [SerializeField] private SceneData titleSceneData;

    public int SceneIndex { get; private set; } = -1;
    public string SceneName => SceneIndex >= 0 && SceneIndex < sceneData.Count ? sceneData[SceneIndex].Name : string.Empty;
    private int loadedIndex = -1;


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

    public void TransitionToMainMenu()
    {
        SaveManager.instance.Save();
        SceneIndex = -1;
        StartCoroutine(PlayLoadingScreen(titleSceneData.loadingScreenOverride, titleSceneData, titleSceneData.SceneName, false));
    }

    public void TransitionToNextScene()
    {
        if (SceneIndex + 1 >= sceneData.Count)
        {
            Debug.LogError("Already at the last scene of the game!");
            return;
        }
        TransitionToScene(SceneIndex + 1);
    }

    public void TransitionToScene(int newIndex)
    {
        if (SceneIndex == newIndex)
            return;
        var currScene = SceneIndex >= 0 ? sceneData[SceneIndex].SceneName : string.Empty;
        var nextSceneData = sceneData[newIndex];
        var nextScene = nextSceneData.SceneName;
        if (nextScene == currScene)
        {
            nextScene = string.Empty;
        }
        SceneIndex = newIndex;
        StartCoroutine(PlayLoadingScreen(nextSceneData.loadingScreenOverride, nextSceneData, nextScene, true));
    }

    public void Continue()
    {
        TransitionToScene(loadedIndex);
    }

    public void LoadIndex(string savedName, int savedIndex)
    {
        for (int i = 0; i < sceneData.Count; ++i)
        {
            var data = sceneData[i];
            if(savedName == data.Name)
            {
                loadedIndex = i;
                return;
            }
        }
        if(savedIndex < sceneData.Count)
        {
            loadedIndex = savedIndex;
            return;
        }
        loadedIndex = 0;
    }

    private IEnumerator PlayLoadingScreen(LoadingScreen loadingScreenOverride, SceneData data, string sceneName, bool save)
    {
        loadingScreenCanvas.enabled = true;
        // Play loading screen ON
        LoadingScreen loadingScreen = loadingScreenOverride ?? defaultLoadingScreen;
        loadingScreen.Progress = 0;
        yield return loadingScreen.StartLoading();
        if (save)
        {
            SaveManager.instance.Save();
        }
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

        // Reset Camera
        CameraManager.instance.ResetCamera();
        if (data.sceneData is BattleCanvas battleCanvas)
        {
            // Initialize Battle
            BattleManager.instance.LoadBattle(battleCanvas);
        }
        else if(data.sceneData is DialogCanvas dialogCanvas)
        {
            // Initialize Dialog
            DialogManager.instance.LoadDialog(dialogCanvas, true);
        }
        // Finish loading
        yield return loadingScreen.FinishLoading();
        // Start the scene
        if (data.sceneData is BattleCanvas)
        {
            BattleManager.instance.StartBattle();
        }
        else if (data.sceneData is DialogCanvas dialogCanvas)
        {
            DialogManager.instance.Loading = false;
            DialogManager.instance.NextDialog(false, false);
            //yield return null;
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
