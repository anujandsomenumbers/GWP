using System.Collections.Generic;
using UnityEngine;

public class LevelSearch : ISearchGraph<SearchNode<Vector3Int>, Vector3Int>
{
    public readonly HashSet<Vector3Int> alwaysAllowed = new HashSet<Vector3Int>();
    private readonly LevelController level;

    public LevelSearch(LevelController level) => this.level = level;

    public int Cost(SearchNode<Vector3Int> current, SearchNode<Vector3Int> next)
    {
        if (level.levelData.GetEntity(next, out Floor floor))
        {
            return Mathf.RoundToInt(level.prefabLibrary.GetFloorInfo(floor.entityId).SpeedMultiplier*100);
        }
        return 1000;
    }

    public float Heuristic(SearchNode<Vector3Int> a, SearchNode<Vector3Int> b)
    {
        return Vector2.Distance(
            new Vector2(a.Coord.x, a.Coord.y), 
            new Vector2(b.Coord.x, b.Coord.y));
    }

    public int Neighbours(SearchNode<Vector3Int> node, List<SearchNode<Vector3Int>> neighbours)
    {
        neighbours.Clear();

        void tryAddNeighbour(Vector3Int neighbour)
        {
            bool isNextOpen = IsTraversable(neighbour, node);
            if (isNextOpen) neighbours.Add(new SearchNode<Vector3Int>(neighbour));
        }

        tryAddNeighbour(node + Vector3Int.up);
        tryAddNeighbour(node + Vector3Int.right);
        tryAddNeighbour(node + Vector3Int.down);
        tryAddNeighbour(node + Vector3Int.left);
        return neighbours.Count;
    }

    private bool IsTraversable(Vector3Int next, Vector3Int start)
    {
        if (!level.levelData.IsInRange(next)) return false;
        if (level.levelData.GetEntity(next, out Item item))
        {
            if (level.prefabLibrary.GetItemInfo(item.entityId).IsBlocker
                && !alwaysAllowed.Contains(start)) return false;
        }
        return true;
    }
}
