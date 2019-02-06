using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Base class for FXText
    // Attach child classes to an object with a Text component to apply effect
    public abstract class FXTextBase : BaseMeshEffect
    {
        public List<int> ind; // List of text indices to apply effect on: in between consecutive pairs
        protected Text textComp; // Text component attached

        // Base initialization
        new void Start()
        {
            base.Start();
            textComp = GetComponent<Text>();
            StartCoroutine(effectCR());
        }

        // Force update mesh
        void FixedUpdate()
        {
            textComp.SetVerticesDirty();
        }

        // Called when graphic is redrawn (forced update every fixed update frame)
        public override void ModifyMesh(VertexHelper vh)
        {
            if (ind == null || ind.Count == 0 || ind.Count % 2 != 0) return;
            List<UIVertex> stream = new List<UIVertex>();
            vh.GetUIVertexStream(stream);

            // Apply effect to each individual letter rect
            for (int i = 0; i < ind.Count; i += 2)
            {
                for (int j = ind[i]; j < ind[i + 1]; j++)
                {
                    onEffect(vh, j);
                }
            }
        }

        // Coroutine to initialize and animate (if animated) effect
        protected abstract IEnumerator effectCR();

        // Called to update the single letter rect at index 'pos'
        protected abstract void onEffect(VertexHelper vh, int pos);
    }
}

