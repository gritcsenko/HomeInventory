namespace HomeInventory.Tests.Acceptance.Support;

public class AvailableProductData(string storeName, string productName, decimal price, DateOnly date, decimal volume)
{
    public string StoreName { get; } = storeName;

    public string ProductName { get; } = productName;

    public decimal Price { get; } = price;

    public DateOnly AbsoluteExpiration { get; } = date;

    public decimal Gallons { get; } = volume;
}
