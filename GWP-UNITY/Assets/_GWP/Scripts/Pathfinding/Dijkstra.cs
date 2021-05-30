using System.Collections.Generic;
using Priority_Queue;

public class Dijkstra<TNode, TCoord> where TNode : SearchNode<TCoord>
{
    public IEnumerable<TNode> Visited => cameFrom.Keys;
    public IReadOnlyDictionary<TNode, TNode> CameFrom => cameFrom;

    private readonly List<TNode> neighbours;
    private readonly FastPriorityQueue<TNode> frontier;
    private readonly Dictionary<TNode, TNode> cameFrom = new Dictionary<TNode, TNode>();
    private readonly Dictionary<TNode, int> costSoFar = new Dictionary<TNode, int>();

    private readonly System.Func<TCoord, TNode> coordToNode;

    public Dijkstra(int maxFrontierSize, int maxNeighbours, System.Func<TCoord, TNode> coordToNode)
    {
        frontier = new FastPriorityQueue<TNode>(maxFrontierSize);
        neighbours = new List<TNode>(maxNeighbours);
        this.coordToNode = coordToNode;
    }

    public void BudgetedSearch(ISearchGraph<TNode, TCoord> graph, TNode start, int budget, TNode goal = null)
    {
        Clear();
        
        frontier.Enqueue(start, 0);
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);

        while (frontier.Count != 0)
        {
            TNode current = frontier.Dequeue();
            if (current == goal) break;

            int neighbourCount = graph.Neighbours(current, neighbours);
            TNode neighbour;
            for (int i = 0; i < neighbourCount; i++)
            {
                neighbour = neighbours[i];

                // Stop searching when out of budget.
                if (costSoFar[current] > budget)
                {
                    break;
                }

                int newCost = costSoFar[current] + graph.Cost(current, neighbour);

                // Don't add to frontier if too expensive.
                if (newCost > budget) break;

                // Don't add to frontier unless cost is lower than existing path cost (if there's an existing path).
                if (!costSoFar.ContainsKey(neighbour) || (newCost < costSoFar[neighbour]))
                {
                    if (frontier.Contains(neighbour)) { frontier.Remove(neighbour); }
                    costSoFar[neighbour] = newCost;
                    float heuristic = (null == goal) ? 0 : graph.Heuristic(goal, neighbour);
                    frontier.Enqueue(neighbour, newCost + heuristic);
                    cameFrom[neighbour] = current;
                }
            }
        }
    }

    public int GetPathCost(TNode goal) => costSoFar[goal];

    public bool GetPath(TCoord startCoord, TCoord goalCoord, List<TNode> path) 
        => GetPath(coordToNode(startCoord), coordToNode(goalCoord), path);

    public bool GetPath(TNode start, TNode goal, List<TNode> path)
    {
        if (!cameFrom.ContainsKey(goal)) return false;
        path.Clear();
        TNode current = goal;
        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);

        return true;
    }

    public int GetPathLength(TNode start, TNode goal)
    {
        int path = 0;
        TNode current = goal;
        while (current != start)
        {
            ++path;
            current = cameFrom[current];
        }
        ++path;
        return path;
    }

    public bool HasPathTo(TNode node)
    {
        return cameFrom.ContainsKey(node);
    }

    public void Clear()
    {
        frontier.Clear();
        cameFrom.Clear();
        costSoFar.Clear();
    }
}
