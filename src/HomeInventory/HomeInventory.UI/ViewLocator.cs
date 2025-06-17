using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using HomeInventory.UI.ViewModels;

namespace HomeInventory.UI;

/// <summary>
/// Resolves a view (<see cref="Control"/>) for a given <see cref="BaseViewModel"/> using explicit
/// registrations or <see cref="ViewForAttribute{TViewModel}"/> annotations.
/// </summary>
[SuppressMessage("Design", "CA1031:Do not catch general exception types")]
public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Type> _viewModelToViewMap = [];
    
    /// <summary>
    /// Registers a mapping between a view‑model type and a view type, overriding any existing mapping.
    /// </summary>
    /// <typeparam name="TViewModel">The view‑model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    public bool Register<TViewModel, TView>()
        where TViewModel : BaseViewModel
        where TView : Control
        => _viewModelToViewMap.TryAdd(typeof(TViewModel), typeof(TView));
   
    /// <inheritdoc/>
    public bool Match(object? data) => data is BaseViewModel && _viewModelToViewMap.ContainsKey(data.GetType());

    /// <inheritdoc/>
    public Control? Build(object? param) =>
        param switch
        {
            null => null,
            _ when _viewModelToViewMap.TryGetValue(param.GetType(), out var viewType) => CreateView(param, viewType),
            _ => new TextBlock { Text = "(Unmatched data type)" },
        };

    private static Control CreateView(object dataContext, Type viewType)
    {
        try
        {
            var view = (Control)Activator.CreateInstance(viewType)!;
            view.DataContext = dataContext;
            return view;
        }
        catch
        {
            return new TextBlock { Text = $"(Error creating {viewType.Name})" };
        }
    }
}