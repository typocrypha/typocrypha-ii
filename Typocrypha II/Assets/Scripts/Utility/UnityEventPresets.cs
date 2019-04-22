using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Container class for commonly used UnityEvents.

[System.Serializable]
public class UnityEvent_int : UnityEvent<int> { }
[System.Serializable]
public class UnityEvent_float : UnityEvent<float> { }
[System.Serializable]
public class UnityEvent_string : UnityEvent<string> { }
[System.Serializable]
public class UnityEvent_bool : UnityEvent<bool> { }
