namespace HomeInventory.UI;

public interface IViewModelTypeProvider
{
    /// <summary>
    /// Gets the associated view‑model type.
    /// </summary>
    Type ViewModelType { get; }
}