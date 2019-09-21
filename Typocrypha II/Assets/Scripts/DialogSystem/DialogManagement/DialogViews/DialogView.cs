using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A different dialog view style (chat/normal vn/audio novel/etc).
/// </summary>
public abstract class DialogView : MonoBehaviour
{
    public GameObject dialogBoxPrefab; // Dialog box prefab specific to view
    public GameObject dialogInputPrefab; // Dialog input prefab specific to view

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

    /// <summary>
    /// Display input prompt.
    /// SHOULD BE ABSTRACT AND IMPLEMENTED IN CHILD CLASSES.
    /// </summary>
    /// <param name="data">Information about input event.</param>
    public void DisplayInput(DialogInputItem data)
    {
        GameObject go = Instantiate(dialogInputPrefab, transform);
    }

    /// <summary>
    /// Cleans up dialog view (e.g. removing dialog boxes)
    /// </summary>
    public abstract void CleanUp();
}
