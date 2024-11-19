using System.Collections;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Modules;

public sealed class ModuleMetadataCollection : IReadOnlyCollection<ModuleMetadata>
{
    private readonly List<ModuleMetadata> _metadata = [];

    public int Count => _metadata.Count;

    public void Add(IModule module) => _metadata.Add(new(module));

    public IEnumerator<ModuleMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<DirectedAcyclicGraph<ModuleMetadata, Type>> CreateDependencyGraph(Func<ModuleMetadata, Task<bool>> canLoadAsync)
    {
        var graph = new DirectedAcyclicGraph<ModuleMetadata, Type>();
        var loadable = await GetLoadableModules(canLoadAsync);

        foreach (var meta in loadable)
        {
            var source = graph.GetOrAdd(meta, static (n, v) => n.Value == v);
            foreach (var reference in meta.GetDependencies(loadable))
            {
                reference.Do(r =>
                {
                    var target = graph.GetOrAdd(r, static (n, v) => n.Value == v);
                    graph.AddEdge(source, target, r.ModuleType);
                });
            }
        }

        return graph;
    }

    private async Task<List<ModuleMetadata>> GetLoadableModules(Func<ModuleMetadata, Task<bool>> canLoadAsync)
    {
        var loadable = _metadata.ToList();
        var canLoadCache = new Dictionary<Type, bool>();
        while (true)
        {
            var count = loadable.Count;
            foreach (var meta in loadable.Memo())
            {
                if (!await CanLoadAsync(meta))
                {
                    loadable.Remove(meta);
                    continue;
                }

                foreach (var optional in meta.GetDependencies(_metadata))
                {
                    if (optional.IsNone || !await CanLoadAsync((ModuleMetadata)optional))
                    {
                        loadable.Remove(meta);
                        break;
                    }
                }
            }

            if (loadable.Count == count)
            {
                return loadable;
            }
        }

        ValueTask<bool> CanLoadAsync(ModuleMetadata metadata) => canLoadCache.GetOrAddAsync(metadata.ModuleType, _ => canLoadAsync(metadata));
    }
}