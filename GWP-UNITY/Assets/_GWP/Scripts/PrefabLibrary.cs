using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "GWP/Prefab Library")]
public class PrefabLibrary : ScriptableObject
{
    public Tile zoneTile;

    [Tooltip("Floor prefabs and corresponding Id.")]
    public List<FloorPrefabInfo> FloorPrefabInfos;
    [Tooltip("Item prefabs and corresponding Id.")]
    public List<ItemPrefabInfo> ItemPrefabInfos;
    [Tooltip("Prefab to clone when creating characters.")]
    public List<CharacterPrefabInfo> CharacterPrefabInfos;

    private Dictionary<string, FloorPrefabInfo> floorPrefabInfoById;
    private Dictionary<string, ItemPrefabInfo> itemPrefabInfoById;
    private Dictionary<string, CharacterPrefabInfo> characterPrefabInfoById;

    public FloorPrefabInfo GetFloorInfo(string entityId)
    {
        if (null == floorPrefabInfoById)
        {
            floorPrefabInfoById = FloorPrefabInfos.ToDictionary(info => info.Id);
        }
        floorPrefabInfoById.TryGetValue(entityId, out var result);
        return result;
    }

    public ItemPrefabInfo GetItemInfo(string entityId)
    {
        if (null == itemPrefabInfoById)
        {
            itemPrefabInfoById = ItemPrefabInfos.ToDictionary(info => info.Id);
        }
        itemPrefabInfoById.TryGetValue(entityId, out var result);
        return result;
    }

    public CharacterPrefabInfo GetCharacterInfo(string entityId)
    {
        if (null == characterPrefabInfoById)
        {
            characterPrefabInfoById = CharacterPrefabInfos.ToDictionary(info => info.Id);
        }
        return characterPrefabInfoById[entityId];
    }

    private void OnValidate()
    {
        ItemPrefabInfos.ForEach(info => info.OnValidate());
    }

    [Serializable]
    public class EntityInfo<T> : IButtonInfo where T : UnityEngine.Object
    {
        public string Label => DisplayName;
        public Sprite Icon => UIIcon;
        string IButtonInfo.Id => Id;

        [Tooltip("The Name of the Item shown to the user.")]
        public string DisplayName;
        [Tooltip("Unique identifier used to map saved data to preauthored data.")]
        [Delayed] public string Id;
        [Tooltip("The prefab to clone.")]
        public T Prefab;
        [Tooltip("The sprite used to represent this object in the menu")]
        public Sprite UIIcon;
    }
    [Serializable] public class FloorPrefabInfo : EntityInfo<Tile> 
    {
        [Tooltip("The speed of movement on this tile.")]
        [Range(0, 1)] public float SpeedMultiplier = 0.5f;
    }

    [Serializable] public class ItemPrefabInfo : EntityInfo<Tile> 
    {
        [Tooltip("The maximum amount of items of this type that can share a spot.")]
        [Min(1)] public int StackLimit = 1;
        [Tooltip("The tiles the item occupies relative to its position and rotation.")]
        [Min(1)] public List<Vector3Int> Shape = new List<Vector3Int>();
        [Tooltip("Does this item block pathfinding and prevent characters from standing on it.")]
        public bool IsBlocker;

        public void OnValidate()
        {
            if (!Shape.Contains(Vector3Int.zero)) Shape.Add(Vector3Int.zero);
        }
    }
    [Serializable] public class CharacterPrefabInfo : EntityInfo<CharacterMotor> { }
}