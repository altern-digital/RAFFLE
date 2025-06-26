using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    [AutoProperty]
    public Image image;
    private bool isDragging;
    float time = 0f;

    public Color color;
    public Sprite sprite;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        Vector3 targetScale = isDragging ? Vector3.one * 1.5f : Vector3.one;
        float targetRotation = isDragging ? 15f : (Mathf.Sin(time * 5f) * 5f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 15f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * 15f);

        time += Time.deltaTime;
        time = Mathf.Repeat(time, 2 * Mathf.PI);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == transform.root && originalParent != null)
        {
            transform.SetParent(originalParent);
        }
    }

    public void SetColor(Color c)
    {
        color = c;
        image.color = c;
    }

    public void SetSprite(Sprite s)
    {
        sprite = s;
        image.sprite = s;
    }
}