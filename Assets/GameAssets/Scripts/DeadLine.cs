using System;
using UnityEngine;

public class DeadLine : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SpriteRenderer lineRenderer;
    [SerializeField] private LayerMask droppedItemLayer;

    [SerializeField] private float timeToLose = 5f;
    [SerializeField] private Color normalColor = new Color(1f, 0.3f, 0.43f, 0.4f);
    [SerializeField] private Color warningColor = Color.white;
    [SerializeField] private float blinkSpeed = 12f;

    public event Action<bool> OnWarningStateChanged;

    private BoxCollider2D col;
    private float dangerTimer;
    private bool isWarning;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        if (lineRenderer != null) lineRenderer.color = normalColor;
    }
    private void Update()
    {
        if (gameManager.CurrentState != GameState.Playing) return;

        if (IsItemCrossingLine())
        {
            SetWarningState(true);
            dangerTimer += Time.deltaTime;

            if(lineRenderer != null)
            {
                float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                lineRenderer.color = Color.Lerp(normalColor, warningColor, t);
            }
            if(dangerTimer >= timeToLose)
            {
                SetWarningState(false);
                gameManager.TriggerGameOver();
            }
        }
        else
        {
            dangerTimer = 0f;
            SetWarningState(false);
            if (lineRenderer != null) lineRenderer.color = normalColor;
        }
    }
    private bool IsItemCrossingLine()
    {
        Vector2 center = (Vector2)transform.position + col.offset;
        Vector2 size = Vector2.Scale(col.size, transform.lossyScale);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, droppedItemLayer);
        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i].TryGetComponent(out Item item) && item.CanDetector && !item.IsMerging)
            {
                return true;
            }
        }
        return false;
    }
    private void SetWarningState(bool warning)
    {
        if (isWarning == warning) return;
        isWarning = warning;
        OnWarningStateChanged?.Invoke(isWarning);
    }
}
