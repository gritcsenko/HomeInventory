using ArchUnitNET.Domain;

namespace HomeInventory.Tests.Architecture;

public static class LayerDependencies
{
    private static readonly DirectedAcyclicGraph<IObjectProvider<IType>, Unit> _dependencies = CreateDependencies();

    private static DirectedAcyclicGraph<IObjectProvider<IType>, Unit> CreateDependencies()
    {
        var graph = new DirectedAcyclicGraph<IObjectProvider<IType>, Unit>();
        var core = graph.AddNode(ApplicationLayers.Core);
        var modules = graph.AddNode(ApplicationLayers.ModulesSdk);
        var domain = graph.AddNode(ApplicationLayers.Domain);
        var application = graph.AddNode(ApplicationLayers.Application);
        
        graph.AddEdge(modules, core, Unit.Default);
        graph.AddEdge(domain, core, Unit.Default);
        graph.AddEdge(domain, modules, Unit.Default);
        graph.AddEdge(application, core, Unit.Default);
        graph.AddEdge(application, domain, Unit.Default);
        graph.AddEdge(application, modules, Unit.Default);

        return graph;
    }

    public static IEnumerable<IObjectProvider<IType>> GetAllLayers() => _dependencies.Nodes.Select(node => node.Value);
    
    public static IEnumerable<IObjectProvider<IType>> NotDependsOn(IObjectProvider<IType> source)
    {
        var option = _dependencies.GetNodeOptional(source, static (n, v) => ReferenceEquals(n.Value, v));
        var otherLayers = GetAllLayers().Except([source]);
        return option.Match<IEnumerable<IObjectProvider<IType>>>(
            Some: x => otherLayers.Except(x.Outgoing.Select(static e => e.Destination.Value)),
            None: () => otherLayers);
    }
}
