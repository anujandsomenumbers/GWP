using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : LevelEntity
{
    public float angle;
    public int stackAmount;

    public Item(string id) : base(id) { }

    public override IReadOnlyList<Vector3Int> GetPositions(List<Vector3Int> shape)
    {
        positions.Clear();
        var rotation = Quaternion.AngleAxis(angle, Vector3Int.up);
        foreach (var position in shape)
        {
            Vector3 rotated = rotation * new Vector3(position.x, position.y, position.z);
            Vector3 translated = rotated + gridPosition;
            positions.Add(Vector3Int.RoundToInt(translated));
        }
        return positions;
    }
}