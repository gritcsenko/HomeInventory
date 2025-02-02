namespace HomeInventory.Core;

public sealed class DirectedAcyclicGraph<TNode, TEdge>
{
    private readonly System.Collections.Generic.HashSet<Node> _nodes = [];

    public Node GetOrAdd(TNode nodeValue, Func<Node, TNode, bool> filter) =>
        _nodes.FirstOrDefault(n => filter(n, nodeValue)) ?? AddNode(nodeValue);

    public void AddEdge(Node from, Node to, TEdge edgeValue)
    {
        var edge = new Edge(edgeValue)
        {
            Source = from,
            Destination = to,
        };
        from.OnOutgoing(edge);
        to.OnIncoming(edge);
    }

    public IReadOnlyCollection<Node> KahnTopologicalSort()
    {
        var sorted = new List<Node>();
        var nodesToSort = new Queue<Node>();
        var lookup = _nodes.ToLookup(n => n.Incoming.Count);
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
                if (--inDegree[outgoing] > 0)
                {
                    continue;
                }

                nodesToSort.Enqueue(outgoing);
                inDegree.Remove(outgoing);
            }
        }

        return sorted;
    }

    private Node AddNode(TNode nodeValue)
    {
        var node = new Node(nodeValue);
        _nodes.Add(node);
        return node;
    }

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

        internal void OnOutgoing(Edge edge)
        {
            if (edge.Source != this)
            {
                throw new ArgumentException("Invalid edge's source");
            }

            _outgoing.Add(edge);
        }

        public override string ToString() => $"[{Value}]";
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "By design")]
    public sealed class Edge(TEdge value)
    {
        private readonly TEdge _value = value;
        public required Node Source { get; init; }
        public required Node Destination { get; init; }

        public override string ToString() => $"{Source} -> [{_value}] -> {Destination}";
    }
}
