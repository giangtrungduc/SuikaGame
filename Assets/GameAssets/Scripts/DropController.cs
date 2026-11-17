using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class DropController : MonoBehaviour
{
    [Header("AimLine")]
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float maxDistance = 12f;

    [Header("Di chuyển")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference pressAction;
    [SerializeField] private float leftWallX = -3.2f;
    [SerializeField] private float rightWallX = 3.2f;
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float currentItemRadius = 0.5f;

    [Header("Thả")]
    [SerializeField] private ItemManager itemManager;

    private LineRenderer lr;
    private Camera mainCamera;
    private bool isDragging;
    private bool canDrop = true;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (pressAction != null) pressAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (pressAction != null) pressAction.action.Disable();
    }

    private void Update()
    {
        HandleInputAndMovement();

        UpdateAimLine();
    }

    private void HandleInputAndMovement()
    {
        if (!canDrop) return;

        // Bắt đầu nhấn (chuột hoặc chạm) ngay trong Update để tránh lỗi UI của EventSystem
        if (pressAction.action.WasPressedThisFrame())
        {
            if (IsPointerOverUI()) return;

            isDragging = true;
            transform.position = GetClampedTargetPosition();
        }

        // Đang giữ và kéo
        if (isDragging && pressAction.action.IsPressed())
        {
            Vector3 targetPos = GetClampedTargetPosition();
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }

        // Nhả tay để thả vật thể
        if (isDragging && pressAction.action.WasReleasedThisFrame())
        {
            isDragging = false;
            transform.position = GetClampedTargetPosition();
            DropCurrentItem();
            Debug.Log("Tha");
        }
    }

    private void UpdateAimLine()
    {
        if (!lr.enabled) return;

        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + Vector2.down * maxDistance;

        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, maxDistance, targetLayers);
        if (hit.collider != null)
        {
            endPos = hit.point;
        }

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    private Vector3 GetClampedTargetPosition()
    {
        Vector2 screenPos = moveAction.action.ReadValue<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));

        float minX = leftWallX + currentItemRadius;
        float maxX = rightWallX - currentItemRadius;
        float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);

        return new Vector3(clampedX, transform.position.y, 0f);
    }

    private void DropCurrentItem()
    {
        canDrop = false;
        SetVisible(false);

        itemManager.ReleaseCurrentItem();
    }

    public void SetCurrentItemRadius(float newRadius)
    {
        currentItemRadius = newRadius;
    }

    public void SetVisible(bool isVisible)
    {
        lr.enabled = isVisible;
    }
    public void EnableDropping()
    {
        canDrop = true;
        SetVisible(true);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            return EventSystem.current.IsPointerOverGameObject(touchId);
        }

        return EventSystem.current.IsPointerOverGameObject();
    }
}