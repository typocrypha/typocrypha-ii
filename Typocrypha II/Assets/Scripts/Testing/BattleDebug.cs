using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDebug : MonoBehaviour
{
#if !DEBUG
    private void Awake()
    {
        Destroy(gameObject);
    }
#else

    [Range(-1, 1f)]
    public float gameSpeed = 0f;

    [SerializeField] bool lockWindow = false;
    [SerializeField] bool logUISelection = true;
    [SerializeField] int _width = 240;
    [SerializeField] int _height = 140;

    [SerializeField, Range(1, 32)] int fontSize = 16;

    private Rect windowRect = new Rect(20, 20, 0, 0); //window starts at 20,20
    private Vector2 _scroll;
    private bool debugButtonHeld;

    readonly Vector2 nativeSize = new Vector2(1280, 720);


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && !DialogManager.instance.PH.Paused)
        {
            DialogManager.instance.StopAllCoroutines();
            DialogManager.instance.Hide(DialogManager.EndType.DialogEnd, DialogManager.instance.CleanUp);
        }

        debugButtonHeld = Input.GetKey(KeyCode.LeftAlt);
        var debugButtonPressed = Input.GetKeyDown(KeyCode.LeftAlt);
        Time.timeScale = Mathf.Pow(10, gameSpeed);
        if (!debugButtonHeld) return;
        gameSpeed = Mathf.Clamp(gameSpeed + Input.GetAxisRaw("Horizontal") / 100f, -1, 1);
        if (Input.GetKeyDown(KeyCode.LeftControl)) lockWindow = !lockWindow;
        if (debugButtonPressed && logUISelection) Debug.Log(
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject,
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void OnGUI()
    {
        if (!lockWindow && !debugButtonHeld) return;
        Vector3 scale = new Vector3(Screen.width / nativeSize.x, Screen.height / nativeSize.y, 1.0f);
        GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, scale);

        GUI.skin.GetStyle("Button").fontSize = fontSize;
        GUI.skin.GetStyle("Label").fontSize = fontSize;
        GUI.skin.GetStyle("Toggle").fontSize = fontSize;
        GUI.skin.GetStyle("Window").fontSize = fontSize;

        windowRect = new Rect(windowRect.x, windowRect.y, _width, _height);
        windowRect = GUI.Window(0, windowRect, DebugWindow, "Debug");
    }

    void DebugWindow(int windowId)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
        lockWindow = GUILayout.Toggle(lockWindow, "Lock");
        _scroll = GUILayout.BeginScrollView(_scroll, "Box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time Scale");
        GUILayout.Label(Time.timeScale.ToString("n2") + 'x');
        GUILayout.EndHorizontal();
        gameSpeed = GUILayout.HorizontalSlider(gameSpeed, -1, 1);
        if (GUILayout.Button("Reset Speed")) gameSpeed = 0;

        logUISelection = GUILayout.Toggle(logUISelection, "Log UI Selection");

        GUILayout.EndScrollView();
    }
#endif
}
