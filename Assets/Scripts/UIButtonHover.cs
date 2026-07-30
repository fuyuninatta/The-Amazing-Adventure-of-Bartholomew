using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    private float transitionSpeed = 12f;
    private Color hoverColor = new Color(1f, 0.95f, 0.8f, 1f); 

    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;
    private Vector3 targetScale;
    private Color targetColor;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            targetColor = originalColor;
        }
    }

    private void Update()
    {
        // Smoothly interpolate scale and color
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
        if (buttonImage != null)
        {
            buttonImage.color = Color.Lerp(buttonImage.color, targetColor, Time.deltaTime * transitionSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * 1.1f; // Scale up 10%
        targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        targetColor = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Reset scale on click so it feels snappy
        transform.localScale = originalScale;
    }
}