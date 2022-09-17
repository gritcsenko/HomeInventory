namespace HomeInventory.Tests.Acceptance.Support;

public class AvailableProductData
{
    public AvailableProductData(string storeName, string productName, decimal price, DateOnly date, decimal volume)
    {
        StoreName = storeName;
        ProductName = productName;
        Price = price;
        AbsoluteExpiration = date;
        Gallons = volume;
    }

    public string StoreName { get; }

    public string ProductName { get; }

    public decimal Price { get; }

    public DateOnly AbsoluteExpiration { get; }

    public decimal Gallons { get; }
}
