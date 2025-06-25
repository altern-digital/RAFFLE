using UnityEngine;
using UnityEngine.EventSystems;

public class NiceUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float initialScale = 0f;
    public float normalScale = 1.0f;
    public float pressedScale = 0.9f;
    public float scaleDuration = 0.1f;
    public float sineAmplitude = 0.1f;
    public float delay = 0f;

    bool isPressed = false;
    float time = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    void Awake()
    {
        transform.localScale = Vector3.one * initialScale;

        gameObject.SetActive(false);

        Invoke(nameof(Active), delay);
    }

    void Active()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one * initialScale;
    }

    void Update()
    {
        float targetScale = isPressed ? pressedScale : (normalScale + Mathf.Sin(time * 5f) * sineAmplitude);

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, scaleDuration);

        time += Time.deltaTime;
        time = Mathf.Repeat(time, 2 * Mathf.PI); // Reset time to avoid overflow
    }
}
