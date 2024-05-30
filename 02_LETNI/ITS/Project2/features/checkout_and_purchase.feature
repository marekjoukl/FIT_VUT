Feature: Checkout and purchase of goods

    Scenario: Purchase of product that is out of stock (7)
        Given User has added product(s) to the shopping cart
        But The product is out of stock
        When User clicks on the "Shopping cart" button
        Then User should be directed to the Shopping cart page
    And User should receive a message that the product is out of stock

    Scenario: Inspect content of Shopping cart (8)
        Given User has added product(s) to the shopping cart that is/are avaliable
        When User clicks on the "Shopping cart" button at the top
        Then User should be directed straight to the Shopping cart page
        And All products that have been added to the cart should be displayed

    Scenario: Proceed to checkout from Cart (9)
        Given User is on the Shopping cart page
        And User has product(s) in the shopping cart that is/are avaliable
        When User clicks on the "Checkout" button
        Then User should be directed to the checkout page
        And The summary of the order should be displayed

    Scenario: Proceed to checkout from Product Details (10)
        Given User is on the Product details page
        And There is avaliable product that User added to the shopping cart
        When User clicks on the "Checkout" button after clicking the Cart Button
        Then User should be directed straight to the checkout page
        And The summary of the order should be displayed as in previous test

    Scenario: Checkout Process (11)
        Given User is on the checkout page
        And User fills in user details
        And User clicks on the continue button
        Then User should be able to select payment and delivery options
        And User confirms the order
        And User should be directed to the order confirmation page