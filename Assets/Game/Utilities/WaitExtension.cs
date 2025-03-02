using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class WaitExtension
{
    public static void Wait(this MonoBehaviour mono, float delay, UnityAction action)
    {
        mono.StartCoroutine(Execute(delay, action));
    }

    private static IEnumerator Execute(float delay, UnityAction action)
    {
        yield return new WaitForSecondsRealtime(delay);
        action.Invoke();
    }
}
