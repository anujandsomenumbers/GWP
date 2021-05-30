using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelEntity
{
    public string entityId; // Unique id for the type of entity
    public Vector3Int gridPosition;

    protected readonly List<Vector3Int> positions = new List<Vector3Int>();

    public LevelEntity(string id) => entityId = id;

    public virtual IReadOnlyList<Vector3Int> GetPositions(List<Vector3Int> shape)
    {
        if (null == shape)
        {
            if (0 == positions.Count) positions.Add(gridPosition);
            return positions;
        }
        return shape;
    }
}
