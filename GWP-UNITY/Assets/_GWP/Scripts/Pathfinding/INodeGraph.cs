using System.Collections.Generic;

public interface ISearchGraph<TNode, TCoord> where TNode : SearchNode<TCoord>
{
    int Neighbours(TNode node, List<TNode> neighbours);
    int Cost(TNode current, TNode next);
    float Heuristic(TNode a, TNode b);
}
