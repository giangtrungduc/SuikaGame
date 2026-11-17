using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Suika/Item Data")]
public class ItemData : ScriptableObject
{
    [Range(1, 12)]
    public int level = 1;
    public string itemName;
    public Sprite sprite;
    public Color themeColor = Color.white;
    public float colliderRadius = 0.22f;
    public float mass = 0.5f;
    public int mergeScore = 10;
    public float comboFillAmount = 5f; // max 100
}
