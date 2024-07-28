using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Typocrypha
{
    /// <summary>
    /// Shocked key effect.
    /// Affected key's output is swapped with another key.
    /// To apply, just apply to a single key. Then, that key will pick a random key to set.
    /// TODO: allow for manually setting shock pairs.
    /// </summary>
    public class KeyEffectShockedSub : MonoBehaviour
    {
        public float FillAmount { get => radialFill.fillAmount; set => radialFill.fillAmount = value; }

        [SerializeField] private Image radialFill;
    }
}

