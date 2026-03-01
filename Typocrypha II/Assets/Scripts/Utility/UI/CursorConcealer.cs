using UnityEngine;

public class CursorConcealer : MonoBehaviour
{
#if !UNITY_EDITOR
    void Start()
    {
        // Hide mouse cursor
        Cursor.visible = false;
    }
#endif
}