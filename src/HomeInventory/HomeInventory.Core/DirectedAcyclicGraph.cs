﻿namespace HomeInventory.Core;

public sealed class DirectedAcyclicGraph<TNode, TEdge>
{
    private readonly System.Collections.Generic.HashSet<Node> _nodes = [];
    private readonly System.Collections.Generic.HashSet<Edge> _edges = [];

    public IReadOnlyCollection<Node> Nodes => _nodes;

    public IReadOnlyCollection<Edge> Edges => _edges;

    public Node AddNode(TNode nodeValue)
    {
        var node = new Node(nodeValue);
        _nodes.Add(node);
        return node;
    }

    public Node GetOrAdd(TNode nodeValue, Func<Node, TNode, bool> filter) =>
        Nodes.FirstOrDefault(n => filter(n, nodeValue)) ?? AddNode(nodeValue);

    public Edge AddEdge(Node from, Node to, TEdge edgeValue)
    {
        var edge = new Edge(edgeValue)
        {
            Source = from,
            Destination = to,
        };
        from.OnOutgoing(edge);
        to.OnIncoming(edge);
        _edges.Add(edge);
        return edge;
    }

    public IReadOnlyCollection<Node> KahnTopologicalSort()
    {
        var sorted = new List<Node>();
        var nodesToSort = new Queue<Node>();
        var lookup = Nodes.ToLookup(n => n.Incoming.Count);
        var inDegree = lookup.SelectMany(g => g.Select(node => (g.Key, Node: node))).ToDictionary(x => x.Node, x => x.Key);

        foreach (var n in lookup[0])
        {
            nodesToSort.Enqueue(n);
            inDegree.Remove(n);
        }

        while (nodesToSort.Count > 0)
        {
            var node = nodesToSort.Dequeue();
            sorted.Add(node);

            foreach (var outgoing in node.Outgoing.Select(e => e.Destination))
            {
                if (inDegree[outgoing]-- == 0)
                {
                    nodesToSort.Enqueue(outgoing);
                    inDegree.Remove(outgoing);
                }
            }
        }

        return sorted;
    }

    public TopologicalSortResult DeepFirstTraversalKahnTopologicalSort() => DfsVisitor.Visit(Nodes);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
    public sealed class Node(TNode value)
    {
        private readonly System.Collections.Generic.HashSet<Edge> _incoming = [];
        private readonly System.Collections.Generic.HashSet<Edge> _outgoing = [];

        public IReadOnlyCollection<Edge> Incoming => _incoming;
        public IReadOnlyCollection<Edge> Outgoing => _outgoing;

        public TNode Value { get; } = value;

        internal void OnIncoming(Edge edge)
        {
            if (edge.Destination != this)
            {
                throw new ArgumentException("Invalid edge's destination");
            }

            _incoming.Add(edge);
        }

        internal void OnOutgoing(Edge edge) => _incoming.Add(edge);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
    public sealed class Edge(TEdge value)
    {
        required public Node Source { get; init; }
        required public Node Destination { get; init; }

        public TEdge Value { get; } = value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
    public sealed record class TopologicalSortResult(IReadOnlyCollection<Node> Sorted, IReadOnlyCollection<IReadOnlyCollection<Node>> DetectedCycles)
    {
        public bool IsAcrylic => DetectedCycles.Count == 0;
    }

    private static class DfsVisitor
    {
        public static TopologicalSortResult Visit(IReadOnlyCollection<Node> nodes)
        {
            var state = new State([], [], []);
            foreach (var node in nodes.Where(state.IsNotVisited))
            {
                Visit(state with { Path = [node] });
            }

            return state.CreareResult();
        }

        private static void Visit(State state)
        {
            var node = state.Path[^1];
            foreach (var destination in node.Outgoing.Select(e => e.Destination))
            {
                if (state.PathContains(destination))
                {
                    state.AddCycle(destination);
                    continue;
                }

                if (state.IsNotVisited(destination))
                {
                    state.Path.Add(destination);
                    Visit(state);
                }
            }

            state.MakeSorted(node);
        }

        private sealed record class State(List<Node> Path, List<Node> Sorted, List<IReadOnlyCollection<Node>> Cycles)
        {
            public bool PathContains(Node node) => Path.Contains(node);

            public void AddCycle(Node node)
            {
                int index = Path.IndexOf(node);
                var cycle = Path.GetRange(index, Path.Count - index);
                Cycles.Add(cycle);
            }

            public bool IsNotVisited(Node node) => !Sorted.Contains(node);

            public void MakeSorted(Node node)
            {
                Sorted.Add(node);
                if (!Path[^1].Equals(node))
                {
                    throw new InvalidOperationException("The node to remove is not at the end of the path.");
                }

                Path.RemoveAt(Path.Count - 1);
            }

            public TopologicalSortResult CreareResult()
            {
                if (Cycles.Count > 0)
                {
                    return new([], Cycles);
                }

                Sorted.Reverse();
                return new(Sorted, Cycles);
            }
        }
    }
}
