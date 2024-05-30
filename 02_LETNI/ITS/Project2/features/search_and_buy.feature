Feature: Searching and Buing goods

    Scenario: Searching product (1)
        Given User is on the homepage with match
        When User click into the search bar with match
        And User types 'iphone' with match
        And User clicks the search button with match
        Then User should be directed to the Search page with match
        And List of products related to 'iphone' should be displayed with match


    Scenario: Search product without match (2)
        Given User is on the homepage without match
        When User clicks into the search bar without match
        And User types <keyword> 
        And User clicks the search button without match
        But There are no products related to "keyword"
        Then User should see a message "There is no product that matches the search criteria."
           
    Scenario: Search using Categories (3)
        Given User is on the homepage
        When User hovers over <categories>
        And User chooses one and clicks on it
        Then User should be directed to Categories page
        And Products of chosen category should be displayed

    Scenario: View product details (4)
        Given User is on the search results page
        When User clicks on a product
        Then User should be directed to the product details page
        And Detailed information about the product should be displayed

    Scenario: Add product to cart (5)
        Given User is on the product details page
        When User clicks on the "Add to Cart" button
        Then Popup success message should be displayed

    Scenario: Remove product from cart (6)
        Given User has added a product to his shopping cart
        And Has his cart displayed
        When User clicks the "Remove" button
        Then The product should be removed from his cart
