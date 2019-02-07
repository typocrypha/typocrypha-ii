using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A different dialog view style (chat/normal vn/audio novel/etc).
/// </summary>
public abstract class DialogView : MonoBehaviour
{
    public GameObject dialogBoxPrefab; // Dialog box prefab specific to view

    /// <summary>
    /// Enable/Disable this dialog view. Does not disable others.
    /// </summary>
    /// <param name="e">true:enable, false:disable.</param>
    public abstract void SetEnabled(bool e);

    /// <summary>
    /// Creates/prepares dialog box and starts line of dialog.
    /// </summary>
    /// <param name="data">Dialog line data.</param>
    /// <returns>DialogBox object created/prepared.</returns>
    public abstract DialogBox PlayDialog(DialogItem data);
}
