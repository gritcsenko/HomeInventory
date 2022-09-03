namespace HomeInventory.Tests.Acceptance.Support;

public class AvailableProductData
{
    public AvailableProductData(string storeName, string productName, string priceText, string dateText, string volumeText)
    {
        StoreName = storeName;
        ProductName = productName;
        Price = priceText.ParseDecimal();
        AbsoluteExpiration = dateText.ParseDate();
        Gallons = volumeText.ParseDecimal();
    }

    public string StoreName { get; }

    public string ProductName { get; }

    public decimal Price { get; }

    public DateOnly AbsoluteExpiration { get; }

    public decimal Gallons { get; }
}
