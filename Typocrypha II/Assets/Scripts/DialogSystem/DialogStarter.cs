using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogStarter : MonoBehaviour
{
    [SerializeField] private DialogManager manager;
    void Start()
    {
        manager.StartDialog(false);
    }
}
