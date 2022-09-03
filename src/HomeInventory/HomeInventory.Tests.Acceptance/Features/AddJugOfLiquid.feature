Feature: AddJugOfLiquid
    As a registered user
    I would like to register that:
    - I bought a jug of liquid at some non-future date in some store and payed some price
    - Liquid that is stored in a jug will expire at some future date, relative to manufactured date or absolute
    - The jug is stored in the fridge storage area
    So that I know how much liquid I have and when I need to buy more

Background:
    Given That today is 12/02/2022 and following environment
        | Store   | Product | Price | Expiration | UnitVolume |
        | Walmart | Milk    | 2.99  | 12/12/2022 | 1          |
    And Following context
        | Area   |
        | Fridge |

@WI21
Scenario: User bought a gallon jug of milk from store
    Given User bought a 1 gallon jug of "Milk" at 12/02/2022 in "Walmart"
    When User stores jug in to the "Fridge" storage area
    Then The "Fridge" storage area should contain 1 gallon jug of "Milk" that will expire at 12/12/2022
    And A transaction was registered: User bought 1 gallon jug of "Milk" at 12/02/2022 in "Walmart" and payed $2.99
