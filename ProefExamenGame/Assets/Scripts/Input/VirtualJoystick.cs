using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float handleRange = 50f;
    [SerializeField] private float deadzone = 0.2f;
    
    private Vector2 inputVector;
    
    // Reference to your player controller
    public PlayerController playerController;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, 
            eventData.position, 
            eventData.pressEventCamera, 
            out position
        );

        position = Vector2.ClampMagnitude(position, handleRange);
        Vector2 rawInput = position / handleRange;
        float magnitude = rawInput.magnitude;

        if (magnitude <= deadzone)
        {
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = rawInput * handleRange;
        }
        else
        {
            Vector2 direction = rawInput / magnitude;
            float remappedMagnitude = Mathf.InverseLerp(deadzone, 1f, Mathf.Clamp01(magnitude));
            inputVector = direction * remappedMagnitude;
            joystickHandle.anchoredPosition = inputVector * handleRange;
        }
        
        // Send input to player controller
        if (playerController != null)
        {
            playerController.SetMovementInput(inputVector);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
        
        // Stop movement
        if (playerController != null)
        {
            playerController.SetMovementInput(Vector2.zero);
        }
    }
}
