Feature: GetArea
    As a registered user
    I would like to see available areas:
    So that I know where I can add inventory

    Link to a feature: [GetArea](HomeInventory.Tests.Acceptance/Features/GetArea.feature)
 
Background:
    Given That today is 12/02/2022
    And Following environment
        | Store   | Product | Price | Expiration | UnitVolume |
        | Walmart | Milk    | 2.99  | 12/12/2022 | 1          |
        | Lidl    | Water   | 0.99  | 12/12/2032 | 1          |
    And Following areas
        | Area   |
        | Fridge |
        | Pantry |
    And Registered users
        | FirstName | LastName | Email                     | Password |
        | Casual    | User     | Causal.User@someorg.email | Pa$$w0rd |

@WI32
Scenario: Get areas
    Given User "Causal.User@someorg.email"
    When User gets all available areas
    Then List of areas should contain
        | Area   |
        | Fridge |
        | Pantry |
