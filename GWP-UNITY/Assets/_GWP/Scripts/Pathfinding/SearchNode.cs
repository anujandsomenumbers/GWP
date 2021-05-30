using Priority_Queue;
using System.Collections.Generic;

public class SearchNode<T> : FastPriorityQueueNode
{
    public T Coord { get; private set; }

    public SearchNode(T coord) => Coord = coord;

    public static List<T> ToCoordsList(IEnumerable<SearchNode<T>> nodeRange)
    {
        var ret = new List<T>();

        foreach (var node in nodeRange)
        {
            ret.Add(node.Coord);
        }

        return ret;
    }

    public static bool operator ==(SearchNode<T> a, SearchNode<T> b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }
    public static bool operator !=(SearchNode<T> a, SearchNode<T> b) => !(a == b);

    public static implicit operator T(SearchNode<T> node)
    {
        return node.Coord;
    }

    public override bool Equals(object obj)
    {
        var other = obj as SearchNode<T>;
        if (null == other) { return false; }
        return Coord.Equals(other.Coord);
    }

    public override int GetHashCode()
    {
        return Coord.GetHashCode();
    }
}
