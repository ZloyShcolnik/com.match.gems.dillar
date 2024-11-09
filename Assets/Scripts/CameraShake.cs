using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private const float shakeDuration = 0.1f;
    private const float shakeIntensity = 0.06f;

    private Vector3 originalPosition;
    private static float shakeTimer = 0f;
	
	[SerializeField] float size;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        Camera.main.orthographicSize = size * Screen.height / Screen.width * 0.5f;

        if (shakeTimer > 0)
        {
            Vector3 randomPosition = Random.insideUnitSphere * shakeIntensity;
            randomPosition.z = 0;

            transform.localPosition = originalPosition + randomPosition;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }

    public static void Make()
    {
        shakeTimer = shakeDuration;
    }
}
