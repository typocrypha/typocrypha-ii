using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A different dialog view style (chat/normal vn/audio novel/etc).
/// </summary>
public abstract class DialogView : MonoBehaviour
{
    public abstract void SetEnabled(bool e);
    public abstract DialogBox NewDialog(DialogItem data);
}
