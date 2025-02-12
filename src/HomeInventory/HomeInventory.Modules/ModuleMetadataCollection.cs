using System.Collections;
using HomeInventory.Core;
using HomeInventory.Modules.Interfaces;
using LanguageExt;

namespace HomeInventory.Modules;

public sealed class ModuleMetadataCollection : IReadOnlyCollection<ModuleMetadata>
{
    private readonly List<ModuleMetadata> _metadata = [];

    public int Count => _metadata.Count;

    public void Add(IModule module) => _metadata.Add(new(module));

    public IEnumerator<ModuleMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<DirectedAcyclicGraph<ModuleMetadata, Type>> CreateDependencyGraphAsync(Func<ModuleMetadata, CancellationToken, Task<bool>> canLoadAsync, CancellationToken cancellationToken = default)
    {
        var graph = new DirectedAcyclicGraph<ModuleMetadata, Type>();
        var loadable = await GetLoadableModulesAsync(canLoadAsync, cancellationToken);

        foreach (var meta in loadable)
        {
            var source = graph.GetOrAddNode(meta, static (n, v) => n.Value == v);
            foreach (var reference in meta.GetDependencies(loadable))
            {
                reference.Do(r =>
                {
                    var target = graph.GetOrAddNode(r, static (n, v) => n.Value == v);
                    graph.AddEdge(source, target, r.ModuleType);
                });
            }
        }

        return graph;
    }

    private async Task<List<ModuleMetadata>> GetLoadableModulesAsync(Func<ModuleMetadata, CancellationToken, Task<bool>> canLoadAsync, CancellationToken cancellationToken)
    {
        var loadable = _metadata.ToList();
        var canLoadCache = new Dictionary<Type, bool>();
        while (true)
        {
            var initialCount = loadable.Count;
            foreach (var meta in loadable.Memo())
            {
                if (!await CanLoadAsync(meta))
                {
                    loadable.Remove(meta);
                    continue;
                }

                foreach (var optional in meta.GetDependencies(_metadata))
                {
                    if (optional.IsSome && await CanLoadAsync((ModuleMetadata)optional))
                    {
                        continue;
                    }

                    loadable.Remove(meta);
                    break;
                }
            }

            if (loadable.Count == initialCount)
            {
                return loadable;
            }
        }

        ValueTask<bool> CanLoadAsync(ModuleMetadata metadata) => canLoadCache.GetOrAddAsync(metadata.ModuleType, _ => canLoadAsync(metadata, cancellationToken));
    }
}