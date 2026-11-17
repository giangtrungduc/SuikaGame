using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData Data { get; private set; }
    public bool IsDropped { get; set; }
    public bool IsMerging { get; set; }

    [SerializeField] private SpriteRenderer spriteRenderer;

    private CircleCollider2D circleCollider;
    private Rigidbody2D rb;
    private int holdingLayer;
    private ItemManager manager;


    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        holdingLayer = LayerMask.NameToLayer("HoldingItem");
    }
    public void Initialize(ItemData data, ItemManager itemManager)
    {
        Data = data;
        manager = itemManager;
        IsMerging = false;

        spriteRenderer.sprite = data.sprite;
        circleCollider.radius = data.colliderRadius;
        rb.mass = data.mass;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsMerging) return;

        if(collision.gameObject.TryGetComponent(out Item otherItem))
        {
            if (otherItem.IsMerging) return;

            if(otherItem.Data.level == this.Data.level)
            {
                this.IsMerging = true;
                otherItem.IsMerging = true;
            }
        }
    }
    public void SetHoldingState()
    {
        IsDropped = false;
        gameObject.layer = holdingLayer;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        circleCollider.enabled = false;
    }
    public void SetDroppedState(int droppedLayer)
    {
        IsDropped = true;
        gameObject.layer = droppedLayer;
        circleCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}