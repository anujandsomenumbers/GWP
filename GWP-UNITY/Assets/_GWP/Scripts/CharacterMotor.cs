using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    // Initialization properties
    public LevelController Level { get; set; }
    public Dijkstra<SearchNode<Vector3Int>, Vector3Int> Dijkstra { get; set; }

    // Other public properties
    public BuildTask Task { get; set; }

    // Serialized fields
    public float brakeDistance = 0.05f;
    public float moveSpeed = 2;
    public LineRenderer pathLine;
    
    // Privates
    private float actionTimer = 0;
    private readonly List<SearchNode<Vector3Int>> path = new List<SearchNode<Vector3Int>>();

    public void Tick(float deltaTime)
    {
        if (null == Task) return;

        if (PathfindToward(Task.position, deltaTime)) return;
        if (Construct(deltaTime)) return;
        Task = null;
    }

    private bool PathfindToward(Vector3Int position, float deltaTime)
    {
        // Don't move if already close enough.
        Vector3 worldPosition = Level.CellToWorld(position);
        if (Vector3.Distance(transform.position, worldPosition) < brakeDistance) return false;

        // No path available, try to get one.
        if (0 == path.Count)
        {
            var start = new SearchNode<Vector3Int>(Vector3Int.RoundToInt(transform.position));
            var goal = new SearchNode<Vector3Int>(position);
            Level.searchGraph.alwaysAllowed.Clear();
            Level.searchGraph.alwaysAllowed.Add(start);
            Dijkstra.BudgetedSearch(Level.searchGraph, start, 10000, goal);
            if (!Dijkstra.GetPath(start, goal, path))
            {
                Debug.Log("No path found");
                Task = null;
                return false;
            }

            int lastIndex = path.Count - 1;
            path.RemoveAt(lastIndex);
        }
        
        return MoveAlongPath(deltaTime);
    }

    private bool MoveAlongPath(float deltaTime)
    {
        // Reached end of path, don't move.
        if (0 == path.Count) return false;

        int lastIndex = path.Count - 1;
        Vector3Int nextPosition = path[lastIndex];
        Vector3 worldPosition = Level.CellToWorld(nextPosition);

        // Close enough, try to advance next position.
        if (Vector3.Distance(transform.position, worldPosition) < brakeDistance)
        {
            path.RemoveAt(lastIndex);

            // Reached end of path, don't move.
            if (0 == path.Count) return false;

            // Advance next position.
            lastIndex = path.Count - 1;
            nextPosition = path[lastIndex];
            worldPosition = Level.CellToWorld(nextPosition);
        }

        // Move
        transform.position = Vector3.MoveTowards(transform.position, worldPosition, deltaTime * moveSpeed);
        UpdatePathLine();
        return true;
    }

    private void UpdatePathLine()
    {
        pathLine.positionCount = path.Count + 1;
        for (int i = 0; i < path.Count; i++)
        {
            pathLine.SetPosition(i, Level.CellToWorld(path[i]));
        }
        pathLine.SetPosition(path.Count, transform.position);
    }

    private bool Construct(float deltaTime)
    {
        actionTimer += deltaTime;

        // TODO: Don't hardcode action duration
        if (actionTimer >= 1)
        {
            var item = new Item(Task.itemToBuild)
            {
                stackAmount = 1,
                gridPosition = Task.position
            };

            Level.SetItem(item);
            actionTimer = 0;
            return false;
        }

        return true;
    }
}