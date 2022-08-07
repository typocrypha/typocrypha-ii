using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace FXText
{
    /// <summary>
    /// Base class for TMPro effects
    /// TODO: replace Linq
    /// </summary>
    public abstract class TMProEffect : MonoBehaviour
    {
        /// <summary>
        /// Enum for different priority groups.
        /// If multiple effects of the same priority group are applied to the same character, \
        /// only the effect instance with the highest priority is applied. Effects in different \
        /// groups may be applied at the same time (e.g. a color effect + a movement effect).
        /// </summary>
        public enum PriorityGroupEnum
        {
            DEFAULT,   // Default - shouldnt be used in general
            COLOR,     // Effects which modify the color of a character
            POSITION,  // Effects which modify the position of a character,
        }

        public bool done = false; // Remove self from allEffects lists when done
        public TextMeshProUGUI text; // Text component attached
        public List<int> ind; // List of text indices to apply effect on: in between consecutive pairs
        public int Priority = 0; // Priority of effect when multiple are working. Higher priority = displayed.
        public abstract PriorityGroupEnum PriorityGroup // Priority group of effect
        {
            get;
        }

        List<TMProEffect> allEffects; // All effects on this text.
        private readonly SharedMemory sharedMemory = new SharedMemory();

        protected const int vertsInQuad = 4; // Number of vertices in a single text quad.
        private bool firstUpdate = true;

        public void UpdateAllEffects()
        {
            firstUpdate = false;
            // Get all other TMProEffect components to determine priority
            allEffects = GetComponents<TMProEffect>().Where(a => a != null).OrderBy(a => a.Priority).ToList();
            sharedMemory.EnsureCapacity(allEffects.Count);
        }
        private void Start()
        {
            if (firstUpdate)
            {
                UpdateAllEffects();
            }
        }

        private void Update()
        {
            if (allEffects.Count <= 0)
                return;
            // All effects managed by highest priority instance
            allEffects.RemoveAll(a => a == null);
            if (allEffects[allEffects.Count - 1] == this)
            {
                UpdateMesh(text, allEffects, sharedMemory);
                sharedMemory.Clear();
            }
            allEffects.RemoveAll(a => a.done);
        }

        /// <summary>
        /// Update text mesh with all effects
        /// </summary>
        static void UpdateMesh(TextMeshProUGUI text, List<TMProEffect> allEffects, SharedMemory sharedMemory)
        {
            // Iterate through each character
            for (int charIndex = 0; charIndex < text.textInfo.characterCount && charIndex < text.text.Length; ++charIndex)
            {
                // Skip characters that aren't affected by effects
                if (SkipCharacters(text.text[charIndex])) continue;
                // Get mesh info for current character
                int meshIndex = text.textInfo.characterInfo[charIndex].materialReferenceIndex;
                int vertexIndex = text.textInfo.characterInfo[charIndex].vertexIndex;
                // Apply default effects (overridden if actually applied)
                foreach (var effect in allEffects)
                {
                    effect.ApplyDefaultEffect(text.textInfo.meshInfo[meshIndex], vertexIndex);
                }
                // Get effects on current character, and continue if no effects.
                var currEffects = GetApplicableEffects(charIndex, allEffects, sharedMemory.CurrEffects);
                if (currEffects.Count == 0) continue;
                // Split list of effects based on priority groups, and get highest priority for each group
                var splitEffects = SplitByPriorityGroup(currEffects, sharedMemory.SplitEffects, sharedMemory.HighestPriorityEffects);                
                // Apply effects
                foreach (var effect in splitEffects)
                {
                    effect.ApplyVertexEffect(text.textInfo.meshInfo[meshIndex], vertexIndex);
                }
            }
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

        /// <summary>
        /// Returns true if character should be skipped for effects.
        /// </summary>
        static bool SkipCharacters(char character)
        {
            if (char.IsWhiteSpace(character)) // Skip white space (not included in vertices)
            {
                return true;
            }
            // TODO: '-' character also causes issues
            return false;
        }

        /// <summary>
        /// Get all effects affecting current character (retains previous sorting)
        /// </summary>
        /// <param name="charIndex">Index of current character</param>
        /// <param name="effects">All effects on this text object</param>
        /// <returns>List of effects on current character</returns>
        static List<TMProEffect> GetApplicableEffects(int charIndex, List<TMProEffect> effects, List<TMProEffect> currEffects)
        {
            currEffects.Clear();
            for (int priority = 0; priority < effects.Count; priority++)
            {
                var effect = effects[priority];
                for (int pairIndex = 0; pairIndex < effect.ind.Count / 2; pairIndex += 2)
                {
                    // If effect includes current character, apply effect
                    if (charIndex >= effect.ind[pairIndex] && charIndex < effect.ind[pairIndex + 1])
                    {
                        currEffects.Add(effect);
                        break;
                    }
                }
            }
            return currEffects;
        }

        /// <summary>
        /// Split a list of effects by their priority group, and retain only highest priority for each group.
        /// </summary>
        /// <param name="effects">List of effects to split</param>
        /// <returns>Nested list of effects by priority group</returns>
        static List<TMProEffect> SplitByPriorityGroup(List<TMProEffect> effects, Dictionary<PriorityGroupEnum, List<TMProEffect>> split, List<TMProEffect> highestPriorityEffects)
        {
            // Clear shared memory
            foreach (var kvp in split)
            {
                kvp.Value.Clear();
            }
            // Go through all effects and split by priority group
            foreach (var effect in effects)
            {
                // If group is not in split, add it to the split
                if (!split.ContainsKey(effect.PriorityGroup))
                {
                    split.Add(effect.PriorityGroup, new List<TMProEffect>(effects.Count));
                }
                // Add effect to group
                split[effect.PriorityGroup].Add(effect);            
            }
            // Only retain highest priority
            highestPriorityEffects.Clear();
            foreach (var kvp in split)
            {
                var group = kvp.Value;
                if (group.Count <= 0)
                    continue;
                group.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                highestPriorityEffects.Add(group[group.Count - 1]);
            }
            return highestPriorityEffects;
        }

        /// <summary>
        /// Apply effect to specific character
        /// </summary>
        /// <param name="meshInfo">Text mesh data</param>
        /// <param name="vertexIndex">Starting vertex index for character</param>
        protected abstract void ApplyVertexEffect(TMP_MeshInfo meshInfo, int vertexIndex);

        /// <summary>
        /// Apply default if effect is not applied to character
        /// </summary>
        /// <param name="meshInfo">Text mesh data</param>
        /// <param name="vertexIndex">Starting vertex index for character</param>
        protected abstract void ApplyDefaultEffect(TMP_MeshInfo meshInfo, int vertexIndex);

        private class SharedMemory
        {
            // Reusable memory (to prevent continuous allocation)
            public List<TMProEffect> CurrEffects { get; } = new List<TMProEffect>(0);
            public List<TMProEffect> HighestPriorityEffects { get; } = new List<TMProEffect>(0);
            public Dictionary<PriorityGroupEnum, List<TMProEffect>> SplitEffects { get; } = new Dictionary<PriorityGroupEnum, List<TMProEffect>>(3);

            public void EnsureCapacity(int capacity)
            {
                if(CurrEffects.Capacity < capacity)
                {
                    CurrEffects.Capacity = capacity;
                }
                if(HighestPriorityEffects.Capacity < capacity)
                {
                    HighestPriorityEffects.Capacity = capacity;
                }
            }

            public void Clear()
            {
                CurrEffects.Clear();
                HighestPriorityEffects.Clear();
                foreach(var kvp in SplitEffects)
                {
                    kvp.Value.Clear();
                }
            }
        }
    }
}

