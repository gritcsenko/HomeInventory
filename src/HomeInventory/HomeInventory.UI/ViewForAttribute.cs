using Avalonia.Controls;
using HomeInventory.UI.ViewModels;

namespace HomeInventory.UI;

/// <summary>
/// Associates decorated <see cref="Control"/> with the specified <typeparamref name="TViewModel"/>.
/// Apply multiple times to map a single view to several view-models.
/// </summary>
/// <typeparam name="TViewModel">The view-model type represented by the view.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ViewForAttribute<TViewModel> : Attribute, IViewModelTypeProvider 
    where TViewModel : BaseViewModel
{
    /// <inheritdoc cref="ViewModelType" />
    public Type ViewModelType => typeof(TViewModel);
}