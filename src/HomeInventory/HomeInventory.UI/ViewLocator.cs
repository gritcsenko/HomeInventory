using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using HomeInventory.UI.ViewModels;

namespace HomeInventory.UI;

/// <summary>
/// Resolves a view (<see cref="Control"/>) for a given <see cref="BaseViewModel"/> using explicit
/// registrations or <see cref="ViewForAttribute{TViewModel}"/> annotations.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public class ViewLocator : IDataTemplate, IDisposable
{
    private readonly ConcurrentDictionary<Type, Type> _viewModelToViewMap = new();
    private readonly IDisposable _subscription = Disposable.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLocator"/> class.
    /// </summary>
    /// <param name="options">Optional configuration for the locator.</param>
    public ViewLocator(ViewLocatorOptions? options = null)
    {
        options ??= new();

        InternalScan(options.InitialAssemblies);
        if (options.AutoSubscribeAssemblyLoad)
        {
            Subscribe();
            _subscription = Disposable.Create(Unsubscribe);
        }
    }
    
    /// <summary>
    /// Registers a mapping between a view‑model type and a view type, overriding any existing mapping.
    /// </summary>
    /// <typeparam name="TViewModel">The view‑model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    public bool Register<TViewModel, TView>()
        where TViewModel : BaseViewModel
        where TView : Control
        => _viewModelToViewMap.TryAdd(typeof(TViewModel), typeof(TView));

    /// <summary>
    /// Scans the specified assemblies for types decorated with <see cref="ViewForAttribute{TViewModel}"/>.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    public void Scan(params Assembly[] assemblies) => InternalScan(assemblies);

    /// <summary>
    /// Scans the assembly that contains the specified view type.
    /// </summary>
    /// <typeparam name="TView">A view type whose assembly should be scanned.</typeparam>
    public void Scan<TView>() where TView : Control => InternalScan([typeof(TView).Assembly]);
    
    /// <inheritdoc/>
    public bool Match(object? data) => data is BaseViewModel && _viewModelToViewMap.ContainsKey(data.GetType());

    /// <inheritdoc/>
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        if (data is not BaseViewModel)
        {
            return new TextBlock { Text = "(Unsupported data type)" };
        }

        var vmType = data.GetType();
        if (!_viewModelToViewMap.TryGetValue(vmType, out var viewType))
        {
            return new TextBlock { Text = $"(View not found for {vmType.Name})" };
        }

        try
        {
            var view = (Control)Activator.CreateInstance(viewType)!;
            view.DataContext = data;
            return view;
        }
        catch
        {
            return new TextBlock { Text = $"(Error creating {viewType.Name})" };
        }
    }

    private void InternalScan(IEnumerable<Assembly> assemblies)
    {
        var baseViewType = typeof(Control);
        var viewTypes = assemblies.SelectMany(GetLoadableTypes).Where(baseViewType.IsAssignableFrom);
        foreach (var type in viewTypes)
        {
            foreach (var attr in type.GetCustomAttributes(inherit: false).OfType<IViewModelTypeProvider>())
            {
                _viewModelToViewMap.TryAdd(attr.ViewModelType, type);
            }
        }
    }

    private void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs e) => InternalScan([e.LoadedAssembly]);

    private static IEnumerable<Type> GetLoadableTypes(Assembly asm)
    {
        try
        {
            return asm.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t is not null)!;
        }
        catch
        {
            return [];
        }
    }

    private void Subscribe() => AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

    private void Unsubscribe() => AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscription.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}