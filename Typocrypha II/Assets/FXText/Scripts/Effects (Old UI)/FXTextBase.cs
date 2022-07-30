using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace FXText
{
    // Base class for FXText
    // Attach child classes to an object with a Text component to apply effect
    [RequireComponent(typeof(Text))]
    public abstract class FXTextBase : BaseMeshEffect
    {
        public List<int> ind; // List of text indices to apply effect on: in between consecutive pairs
        protected Text textComp; // Text component attached

        protected virtual int vertsInQuad => 6; // Number of vertices in a single text quad.

        // Base initialization
        protected override void Awake()
        {
            base.Awake();
            textComp = GetComponent<Text>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StopAllCoroutines();
            StartCoroutine(EffectCR());
        }

        // Force update mesh
        void FixedUpdate()
        {
            textComp.SetVerticesDirty();
        }

        // Called when graphic is redrawn (forced update every fixed update frame)
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || ind == null || ind.Count == 0 || ind.Count % 2 != 0) return;
            List<UIVertex> stream = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(stream);
            string text = Regex.Replace(textComp.text, @"<.*?>", ""); // Remove rich text tags
            int len = text.Length * vertsInQuad;
            int wspace = text.Count(a => a == ' ') + text.Count(a => a == '\n');
            bool shadow = (gameObject.GetComponent<Shadow>() != null) && (gameObject.GetComponent<Shadow>().enabled); // Is there an active shadow?
            // Check if first FXText (dont double add spaces)
            if (GetComponents<FXTextBase>()[0] == this)
            {
                int shift = 0;
                // Add dummy vertices for spaces
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ' || text[i] == '\n')
                    {
                        shift++;
                        int pos = i * vertsInQuad;
                        for (int k = 0; k < vertsInQuad; k++)
                        {
                            stream.Insert(pos, new UIVertex());
                            if (shadow)
                            {
                                int spos = pos + ((text.Length - wspace + shift - 1) * vertsInQuad) + k + 1;
                                stream.Insert(spos, new UIVertex());
                            }
                        }
                    }
                }
            }

            // Apply effect to each individual letter rect
            // Also applies to shadows
            for (int i = 0; i < ind.Count; i += 2)
            {
                for (int j = ind[i]; j < ind[i + 1]; j++)
                {
                    if (j >= text.Length) break;
                    // Apply effect on character and its shadow
                    for (int pos = j * vertsInQuad; pos < stream.Count; pos += len)
                    {
                        List<UIVertex> buffer = ListPool<UIVertex>.Get();
                        for (int k = 0; k < vertsInQuad; k++) buffer.Add(stream[pos + k]);
                        OnEffect(buffer, j);
                        for (int k = 0; k < vertsInQuad; k++) stream[pos + k] = buffer[k];
                        ListPool<UIVertex>.Release(buffer);
                    }
                }
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(stream);
            ListPool<UIVertex>.Release(stream);
        }

        // Coroutine to initialize and animate (if animated) effect
        protected abstract IEnumerator EffectCR();

        // Called to update the single letter rect at index 'pos'
        protected abstract void OnEffect(List<UIVertex> buf, int pos);
    }
}

