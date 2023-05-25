Feature: UserRegistration
    As an unregistered user
    I would like to be registered
    So that I can have an account

    Link to a feature: [GetArea](HomeInventory.Tests.Acceptance/Features/UserRegistration.feature)

Background:
    Given That today is 05/25/2023

@WI287
Scenario: Register new user
    Given User e-mail "Causal.User@someorg.email"
    And User password "Pa$$w0rd"
    When User registers new account
    Then User should get an ID as a confirmation of the successful registration
