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

    public bool IsHidden => !isActiveAndEnabled;
    public virtual bool ReadyToContinue => true;
    public bool IsReadyToContinue() => ReadyToContinue;

    public virtual bool ShowImmediately => true;
    public virtual bool DeactivateOnEndSceneHide => true;

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
        Instantiate(dialogInputPrefab, transform);
    }

    /// <summary>
    /// Cleans up dialog view (e.g. removing dialog boxes)
    /// </summary>
    public abstract void CleanUp();

    /// <summary>
    /// Clears all dialog boxes from the current view
    /// </summary>
    public virtual Coroutine Clear()
    {
        CleanUp();
        return null;
    }

    protected bool IsDialogItemCorrectType<T>(DialogItem item, out T itemT) where T : DialogItem
    {
        itemT = item as T;
        if (itemT != null)
            return true;
        Debug.LogError($"Dialog item is not of type {typeof(T)}. Incorrect type for this view mode");
        return false;
    }
    public void SetLocationText(string location)
    {
        SetLocation(TextMacros.SubstituteMacros(location));
    }
    protected virtual void SetLocation(string location) { }

    public void SetDateTimeText(string dateTime)
    {
        SetDateTime(TextMacros.SubstituteMacros(dateTime));
    }
    protected virtual void SetDateTime(string dateTime) { }

    public virtual IEnumerator PlayEnterAnimation()
    {
        yield break;
    }

    public virtual IEnumerator PlayExitAnimation(DialogManager.EndType endType)
    {
        yield break;
    }

    // Character control

    public virtual bool AddCharacter(AddCharacterArgs args)
    {
        return false;
    }

    public virtual bool RemoveCharacter(CharacterData data)
    {
        return false;
    }

    public virtual void SetExpression(CharacterData data, string expression)
    {

    }

    public virtual void SetPose(CharacterData data, string pose)
    {

    }

    public class AddCharacterArgs
    {
        public CharacterData CharacterData { get; }
        public DialogViewVNPlus.CharacterColumn Column { get; }
        public Vector2 AbsolutePosition { get; }
        public string InitialPose { get; }
        public string InitialExpression { get; }
        public AddCharacterArgs(CharacterData data, DialogViewVNPlus.CharacterColumn column, Vector2 pos, string pose, string expr)
        {
            CharacterData = data;
            Column = column;
            AbsolutePosition = pos;
            InitialPose = pose;
            InitialExpression = expr;
        }
    }
}
