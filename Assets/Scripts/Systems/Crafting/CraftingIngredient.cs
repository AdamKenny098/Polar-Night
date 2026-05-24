using UnityEngine;

[System.Serializable]
public class CraftingIngredient
{
    public Item item;
    [Min(1)] public int amount = 1;
}