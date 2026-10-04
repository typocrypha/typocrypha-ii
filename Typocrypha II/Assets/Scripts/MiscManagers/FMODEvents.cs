using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.IO;

[ExecuteInEditMode]
public class FMODEvents : MonoBehaviour {
    public static FMODEvents instance { get; private set; }

    [field: SerializeField] public static Dictionary<string, EventReference> references { get; private set; }

    private static void GenerateReferenceList()
    {
        if (!Application.isEditor) return;

        references = new Dictionary<string, EventReference>();
        foreach (var e in EventManager.Events)
        {
            references.Add(Path.GetFileNameWithoutExtension(e.Path), new EventReference { Path = e.Path, Guid = e.Guid });
        }
    }

    public static EventReference ClipToRef(AudioClip a) => references[a.name];
    public static EventReference NameToRef(string name) => references[name];

    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;

        GenerateReferenceList();
    }
}
