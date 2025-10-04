using System.Collections;
using HomeInventory.Core;
using HomeInventory.Modules.Interfaces;
using LanguageExt;

namespace HomeInventory.Modules;

internal sealed class ModuleMetadataCollection(IEnumerable<IModule> modules) : IReadOnlyCollection<ModuleMetadata>
{
    private readonly IReadOnlyCollection<ModuleMetadata> _metadata = [.. modules.Select(m => new ModuleMetadata(m))];

    public int Count => _metadata.Count;

    public IEnumerator<ModuleMetadata> GetEnumerator() => _metadata.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async Task<DirectedAcyclicGraph<ModuleMetadata, Type>> CreateDependencyGraphAsync(Func<ModuleMetadata, CancellationToken, Task<bool>> allowedToLoadAsync, CancellationToken cancellationToken = default)
    {
        var loadableModules = await GetLoadableModulesAsync(allowedToLoadAsync, cancellationToken);

        var graph = new DirectedAcyclicGraph<ModuleMetadata, Type>();
        foreach (var meta in loadableModules)
        {
            var source = graph.GetOrAddNode(meta, static (n, v) => n.Value == v);
            foreach (var reference in meta.GetDependencies(loadableModules))
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

    private async Task<List<ModuleMetadata>> GetLoadableModulesAsync(Func<ModuleMetadata, CancellationToken, Task<bool>> allowedToLoadAsync, CancellationToken cancellationToken)
    {
        var loadable = _metadata.ToList();
        var canLoadCache = new Dictionary<Type, bool>();
        int initialCount;

        do
        {
            initialCount = loadable.Count;
            foreach (var meta in loadable.Memo())
            {
                if (!await AllowedToLoadAsync(meta) || !await meta.GetDependencies(_metadata).AllAsync(async o => o.IsSome && await AllowedToLoadAsync((ModuleMetadata)o)))
                {
                    loadable.Remove(meta);
                }
            }
        } while (loadable.Count != initialCount);

        return loadable;

        ValueTask<bool> AllowedToLoadAsync(ModuleMetadata metadata) => canLoadCache.GetOrAddAsync(metadata.ModuleType, _ => allowedToLoadAsync(metadata, cancellationToken));
    }
}
