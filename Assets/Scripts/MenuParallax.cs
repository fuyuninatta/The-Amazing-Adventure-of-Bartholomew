using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 40f; //How far the UI element moves
    public float smoothTime = .3f; //How long it takes to reach the target position

    private Vector3 startLocalPosition;
    private Vector3 velocity; //required by SmoothDamp to track the current movement speed

    private void Start()
    {
        // Capture the LOCAL position relative to the Canvas, not the World
        //make sure that the parallax do not break if you move the parent object
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        //to get the mouse coordinate
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        //to convert pixel to percentage of the screen
        Vector2 viewportPos = Camera.main.ScreenToViewportPoint(mousePosition);
        Vector3 offset = new Vector3(viewportPos.x - 0.5f, viewportPos.y - 0.5f, 0f);

        // Smoothly move using localPosition instead of world position
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition, //current position
            startLocalPosition + (offset * offsetMultiplier), //target position
            ref velocity, //current speed
            smoothTime //easing duration
        );
    }
}