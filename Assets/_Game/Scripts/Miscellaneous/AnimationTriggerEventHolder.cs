using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationTriggerEventHolder : MonoBehaviour
{
    [SerializeField] private UnityEvent[] eventsToInvoke;

    public void Invoke(int index)
    {
        if (eventsToInvoke.Length == 0) return;
        index = Mathf.Clamp(index, 0, eventsToInvoke.Length);
        eventsToInvoke[index].Invoke();
    }
}
