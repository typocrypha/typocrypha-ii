using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class DecodeBubble : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform keyContainer;

    private Dictionary<string, GameObject> keyDict;
    private string correctInput;
    private bool isCorrect;
    private DecodePopup popup;
    private bool triggeredOnCorrectKey;

    public void Show(List<char> keys, char correctChar, DecodePopup popup)
    {
        this.popup = popup;
        triggeredOnCorrectKey = false;
        keyDict = new Dictionary<string, GameObject>(keys.Count);
        correctInput = correctChar.ToString();
        isCorrect = false;
        foreach (char key in keys)
        {
            var keyText = Instantiate(keyPrefab, keyContainer).GetComponentInChildren<TextMeshProUGUI>();
            keyText.text = key.ToString();
            if(key == correctChar)
            {
                isCorrect = true;
                continue;
            }
            keyDict.Add(key.ToString(), keyText.gameObject);
        }
    }

    private void Update()
    {
        if(isCorrect)
        {
            if (Input.GetKeyDown(correctInput))
            {
                gameObject.SetActive(false);
                return;
            }
            foreach (var kvp in keyDict)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    kvp.Value.SetActive(false);
                    if (!triggeredOnCorrectKey)
                    {
                        triggeredOnCorrectKey = true;
                        popup.CleanupBubbles(gameObject);
                    }
                }
            }
        }
        else if(keyDict.Any(k => Input.GetKeyDown(k.Key)))
        {
            gameObject.SetActive(false);
        }
    }
}
