using UnityEngine;
using UnityEngine.UI;

public class VNPlusDialogBoxUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image[] borderImages;
    public CanvasGroup CanvasGroup => canvasGroup;
    [SerializeField] private CanvasGroup canvasGroup;
    public void Bind(CharacterData character)
    {
        foreach(var image in borderImages)
        {
            image.color = character.characterColorLight;
        }
        iconImage.sprite = character.chat_icon;
    }
}
