using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public PrefabLibrary Library { get; set; }
    public BoundsInt Bounds { get; private set; }

    [SerializeField] private BoundsInt bounds;
    public List<Floor> floors = new List<Floor>();
    public List<Item> items = new List<Item>();
    public List<Character> characters = new List<Character>();

    private Dictionary<Vector3Int, Floor> floorByPosition = new Dictionary<Vector3Int, Floor>();
    private Dictionary<Vector3Int, Item> itemByPosition = new Dictionary<Vector3Int, Item>();

    public LevelData(Vector3Int size)
    {
        Bounds = new BoundsInt(-size / 2, size);
    }

    public IEnumerable<T> GetLevelEntities<T>() where T : LevelEntity => GetList<T>();

    public void OnBeforeSerialize() { } // no-op

    public void OnAfterDeserialize()
    {
        floorByPosition = floors.ToDictionary(floor => floor.gridPosition);
        itemByPosition = items.ToDictionary(item => item.gridPosition);
    }

    public bool GetEntity<T>(Vector3Int gridPosition, out T result) where T : LevelEntity
    {
        Dictionary<Vector3Int, T> contents = GetDictionary<T>();
        return contents.TryGetValue(gridPosition, out result);
    }

    public bool IsSlotTypeTaken<T>(Vector3Int gridPosition) where T : LevelEntity
    {
        Dictionary<Vector3Int, T> contents = GetDictionary<T>();
        return contents.ContainsKey(gridPosition);
    }

    public bool CanPlaceFloor(Floor floor, bool willOverwrite = false)
    {
        return !IsSlotTypeTaken<Floor>(floor.gridPosition) || willOverwrite;
    }

    public int ItemPlacementRoom(Item item, out bool areAllSpotsEmpty, bool willOverwrite = false)
    {
        areAllSpotsEmpty = false;
        int stackLimit = Library.GetItemInfo(item.entityId).StackLimit;
        
        if (willOverwrite)
        {
            areAllSpotsEmpty = true;
            return stackLimit;
        }

        // All spots are empty. Place a new item.
        IReadOnlyList<Vector3Int> newPositions = GetAllPositions(item);
        if (!newPositions.Any(position => itemByPosition.ContainsKey(position)))
        {
            areAllSpotsEmpty = true;
            return stackLimit;
        }

        // Target spot does not contain an item. Can't stack.
        if (!itemByPosition.TryGetValue(item.gridPosition, out Item current))
        {
            return 0;
        }

        // Target spot contains a different kind of item. Can't stack.
        if (item.entityId != current.entityId)
        {
            return 0;
        }

        // Stack room left over on item at target spot.
        return stackLimit - current.stackAmount;
    }

    public bool PlaceFloor(Floor floor, bool willOverwrite = false)
    {
        if (!CanPlaceFloor(floor, willOverwrite)) return false;
        Add(floor);
        return true;
    }

    // Returns leftover stack amount after placing.
    public int PlaceItem(Item item, bool willOverwrite = false)
    {
        int room = ItemPlacementRoom(item, out bool isSpotEmpty, willOverwrite);

        if (isSpotEmpty)
        {
            Add(item);
        }
        else
        {
            Item current = itemByPosition[item.gridPosition];
            if (room >= item.stackAmount)
            {
                current.stackAmount += item.stackAmount;
                item.stackAmount = 0;
            }
            else
            {
                item.stackAmount -= room;
                current.stackAmount += room;
                return item.stackAmount;
            }
        }

        return 0;
    }

    public bool Delete<T>(T entity) where T : LevelEntity
    {
        Dictionary<Vector3Int, T> contents = GetDictionary<T>();
        if (!contents.ContainsKey(entity.gridPosition)) return false;
        foreach (var position in GetAllPositions(entity)) contents.Remove(position);
        GetList<T>().Remove(entity);
        return true;
    }

    public void AllEntitiesAt(Vector3Int position, List<LevelEntity> result)
    {
        result.Clear();

        if (floorByPosition.TryGetValue(position, out Floor floor)) result.Add(floor);
        result.AddRange(characters.FindAll(c => c.gridPosition == position));
        if (itemByPosition.TryGetValue(position, out Item item)) result.Add(item);
    }

    private Dictionary<Vector3Int, T> GetDictionary<T>() where T : LevelEntity
    {
        if (typeof(T) == typeof(Floor)) return floorByPosition as Dictionary<Vector3Int, T>;
        else if (typeof(T) == typeof(Item)) return itemByPosition as Dictionary<Vector3Int, T>;

        throw new System.NotSupportedException(
            $"{nameof(GetDictionary)} does not support grid objects of type '{typeof(T).Name}'");
    }

    public bool IsInRange(Vector3Int gridPosition)
    {
        bool isInRange = true;
        for (var i = 0; i < 3; i++)
        {
            isInRange &= Bounds.min[i] <= gridPosition[i];
            isInRange &= gridPosition[i] < Bounds.max[i];
        }
        return isInRange;
    }

    private void Add<T>(T entity) where T : LevelEntity
    {
        Dictionary<Vector3Int, T> contents = GetDictionary<T>();
        List<T> contentList = GetList<T>();
        bool isPositionTaken = contents.ContainsKey(entity.gridPosition);
        IReadOnlyList<Vector3Int> newPositions = GetAllPositions(entity);

        if (isPositionTaken)
        {
            IEnumerable<T> overlaps = newPositions.Select(pos => contents[pos]).Distinct();

            foreach (var overlap in overlaps)
            {
                IReadOnlyList<Vector3Int> currentPositions = GetAllPositions(overlap);
                contentList.Remove(overlap);
                foreach (var position in currentPositions) contents.Remove(position);
            }

            foreach (var position in newPositions) contents.Add(position, entity);
            contentList.Add(entity);
        }
        else
        {
            foreach (var position in newPositions) contents.Add(position, entity);
            contentList.Add(entity);
        }
    }

    private List<T> GetList<T>() where T : LevelEntity
    {
        if (typeof(T) == typeof(Floor)) return floors as List<T>;
        else if (typeof(T) == typeof(Character)) return characters as List<T>;
        else if (typeof(T) == typeof(Item)) return items as List<T>;

        throw new System.NotSupportedException(
            $"{nameof(GetList)} does not support grid objects of type '{nameof(T)}'");
    }


    private IReadOnlyList<Vector3Int> GetAllPositions<T>(T entity) where T : LevelEntity
    {
        if (entity is Item item)
        {
            return item.GetPositions(Library.GetItemInfo(item.entityId).Shape);
        }
        else
        {
            return entity.GetPositions(null);
        }
    }
}
