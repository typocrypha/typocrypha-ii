using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchData : MonoBehaviour
{
    [SerializeField] private DecodeData[] editorData;
    private readonly Dictionary<string, float> values = new Dictionary<string, float>();
    private readonly HashSet<string> decoded = new HashSet<string>();
    private readonly HashSet<string> decodeInProgress = new HashSet<string>();
    private readonly Dictionary<string, DecodeData> data = new Dictionary<string, DecodeData>();

    private void Awake()
    {
        for(int i = 0; i < editorData.Length; ++i)
        {
            var decodeData = editorData[i];
            if (!data.ContainsKey(decodeData.key))
            {
                data.Add(decodeData.key, decodeData);
            }
        }
    }

    public void Add(string key, float amount)
    {
        if (values.ContainsKey(key))
        {
            values[key] += amount;
        }
        else
        {
            values.Add(key, amount);
        }
    }

    public float Value(string key) => values.ContainsKey(key) ? values[key] : 0;

    public bool ReadyToDecode(string key) => Value(key) >= 1 && !IsDecodeInProgress(key) && !IsDecoded(key);

    public bool IsDecoded(string key) => decoded.Contains(key);

    public void SetDecoded(string key)
    {
        if (IsDecoded(key))
            return;
        CancelDecodeInProgress(key);
        decoded.Add(key);
    }

    public bool IsDecodeInProgress(string key) => decodeInProgress.Contains(key);

    public void SetDecodeInProgress(string key)
    {
        decodeInProgress.Add(key);
    }

    public void CancelDecodeInProgress(string key)
    {
        if (!IsDecodeInProgress(key))
            return;
        decodeInProgress.Remove(key);
    }

    public DecodeData GetData(string key)
    {
        return data.ContainsKey(key) ? data[key] : null;
    }
}
