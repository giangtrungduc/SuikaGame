using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] allItemDatas;
    [SerializeField] private Item itemPrefab;
    [SerializeField] private DropController dropController;
    [SerializeField] private int maxRandomSpawnLevel = 5;
    [SerializeField] private float spawnCooldown = 0.5f;


    // --- Các sự kiện phát ra cho UI, Score/Combo và Audio/VFX ---
    public event Action<Sprite> OnNextItemUpdated;
    public event Action<int, Vector3, int, float, Color> OnItemMerged;     
    public event Action<Vector3, int, float, Color> OnLevel12Cleared;  
    public event Action<Vector3, Color> OnItemDestroyedByPowerUp;

    private readonly List<Item> activeItems = new List<Item>();
    private Item currentHoldingItem;
    private ItemData nextItemData;
    private int droppedItemLayer;

    private void Awake()
    {
        droppedItemLayer = LayerMask.NameToLayer("DroppedItem");
    }
    private void Start()
    {
        RollNextItemData();
        SpawnHoldingItem();
    }
    private void RollNextItemData()
    {
        int maxIndex = Mathf.Min(maxRandomSpawnLevel, allItemDatas.Length);
        int randomIndex = UnityEngine.Random.Range(0, maxRandomSpawnLevel);
        nextItemData = allItemDatas[randomIndex];
    }
    private void SpawnHoldingItem()
    {
        if (nextItemData == null) RollNextItemData();

        currentHoldingItem = Instantiate(itemPrefab, dropController.transform.position, Quaternion.identity, dropController.transform);

        currentHoldingItem.Initialize(nextItemData, this);
        currentHoldingItem.SetHoldingState();

        dropController.SetCurrentItemRadius(nextItemData.colliderRadius);
        dropController.EnableDropping();

        RollNextItemData();
        OnNextItemUpdated?.Invoke(nextItemData.sprite);
    }
    public void ReleaseCurrentItem()
    {
        if (currentHoldingItem == null) return;

        currentHoldingItem.transform.SetParent(null);
        currentHoldingItem.SetDroppedState(droppedItemLayer);
        activeItems.Add(currentHoldingItem);

        currentHoldingItem = null;
        StartCoroutine(SpawnNextRoutine());
    }
    private IEnumerator SpawnNextRoutine()
    {
        yield return new WaitForSeconds(spawnCooldown);
        SpawnHoldingItem();
    }
}
