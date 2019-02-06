using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameflow
{
    /// <summary>
    /// Types of dialog views.
    /// </summary>
    public enum DialogViewType
    {
        VN, // Visual novel style (standard dialogue mode)
        AN, // Audio novel style (Text scrolling accross fullscreen background)
        Chat, // Chat style (Text boxed as if chatting online with someone)
    }

    /// <summary>
    /// Starts and manages dialog sequences.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager instance = null;
        public bool startOnAwake = true; // Should dialog start when scene starts up?

        // DIALOG GRAPH FIELD 

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            if (startOnAwake)
            {
                StartDialog();
            }
        }

        /// <summary>
        /// Starts dialog from beginning of graph.
        /// </summary>
        public void StartDialog()
        {

        }
    }
}
