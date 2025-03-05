using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static void Shake(float duration, float strength)
    {
        CameraShake camShake = FindObjectOfType<CameraShake>();

        if (camShake)
        {
            camShake.StartCoroutine(camShake.ShakeCamera(duration, strength));
        }
    }
    
    private IEnumerator ShakeCamera(float duration, float strength)
    {
        Vector3 startPosition = transform.localPosition;
        
        while (duration >= 0)
        {
            Vector2 shakeOffset = Random.insideUnitCircle * strength;
            transform.localPosition = startPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);

            duration -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = startPosition;
    }
}
