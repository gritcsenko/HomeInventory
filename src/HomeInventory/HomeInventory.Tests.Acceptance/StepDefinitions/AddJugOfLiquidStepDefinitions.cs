namespace HomeInventory.Tests.Acceptance.StepDefinitions;

[Binding]
public class AddJugOfLiquidStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    [Given(@"Registered user")]
    public void GivenRegisteredUser()
    {
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method.
        throw new PendingStepException();
    }

    [Given(@"Today's date is (\d{2}/\d{2}/\d{4})")]
    public void GivenTodaysDateIs(DateOnly todayDate)
    {
        throw new PendingStepException();
    }

    [Given(@"Store ""([^""]*)"" to buy products from")]
    public void GivenStoreToBuyProductsFrom(string storeName)
    {
        throw new PendingStepException();
    }

    [Given(@"User bought a (\d+(\.\d+)?) gallon jug of ""([^""]*)"" at (\d{2}/\d{2}/\d{4})")]
    public void GivenUserBoughtJugToday(decimal volume, string productName, DateOnly buyDate)
    {
        throw new PendingStepException();
    }

    [Given(@"User payed \$(\d+\.\d{2}) price in ""([^""]*)""")]
    public void GivenUserPayedPriceAtStore(decimal price, string storeName)
    {
        throw new PendingStepException();
    }

    [Given(@"Jug has absolute expiration date (\d{2}/\d{2}/\d{4})")]
    public void GivenJugHasAbsoluteExpirationDate(DateOnly expirationDate)
    {
        throw new PendingStepException();
    }

    [When(@"User stores jug in to the ""([^""]*)"" storage area")]
    public void WhenUserStoresJugInToTheStorageArea(string storageeAreaName)
    {
        throw new PendingStepException();
    }

    [Then(@"The ""([^""]*)"" storage area should contain (\d+(\.\d+)?) gallon jug of ""([^""]*)"" that will expire at (\d{2}/\d{2}/\d{4})")]
    public void ThenTheStorageAreaShouldContainGallonJugOf(string storageeAreaName, decimal volume, string productName, DateOnly expirationDate)
    {
        throw new PendingStepException();
    }

    [Then(@"Accounting has transaction registered: User bought (\d+(\.\d+)?) gallon jug of ""([^""]*)"" at (\d{2}/\d{2}/\d{4}) in ""([^""]*)"" and payed \$(.*)")]
    public void ThenAccountingHasTransactionRegisteredUserBoughtGallonJugOfAtInAndPayed(decimal volume, string productName, DateOnly buyDate, string storeName, decimal price)
    {
        throw new PendingStepException();
    }
}
