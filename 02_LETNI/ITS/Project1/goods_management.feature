Feature: Admin Management of Goods and Stock

    Background: I am logged as an admin

    Scenario: Inspect the list of products (12)
        Given User is on the Homepage
        When User navigates to the Catalog
        And User click on the Product tab
        Then List of available products should be displayed
        And User should see their respective stock levels

    Scenario: Add new product (13)
        Given User is on the Products page
        When User clicks the "Add New" button
        And User fills in the details for a new product
        And User clicks on the "Save" button
        Then The new product should be added to the system
        And The initial stock level should be set

    Scenario: Update product details (14)
        Given There is an existing product in the system
        And User is on the Products page
        When User clicks the "Edit" button next to the product
        And User updates the details of the product
        And User saves it the by clicking the "Save" button
        Then The product details should be updated accordingly

    Scenario: Search for a product (15)
        Given There are existing products in the system
        When User searches for a specific product using filter options
        Then The search results should be displayed matching the search criteria

    Scenario: Delete product (16)
        Given User is on the Products page
        And There is an existing product in the system
        When User checks the checkbox next to the product
        And User clicks the "Delete" button
        Then The product should be removed from the system