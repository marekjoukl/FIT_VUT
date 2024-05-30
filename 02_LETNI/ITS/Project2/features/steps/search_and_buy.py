from behave import *
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time

######################### Searching product (1) #########################

@given(u'User is on the homepage with match')
def step_impl(context):
    context.driver.get("http://opencart:8080/")


@when(u'User click into the search bar with match')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").click()


@when(u'User types \'iphone\' with match')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").send_keys("iphone")


@when(u'User clicks the search button with match')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").send_keys(Keys.ENTER)


@then(u'User should be directed to the Search page with match')
def step_impl(context):
    context.driver.get("http://opencart:8080/index.php?route=product/search&language=en-gb&search=iphone")


@then(u'List of products related to \'iphone\' should be displayed with match')
def step_impl(context):
    assert context.driver.find_element(By.LINK_TEXT, "iPhone").text == "iPhone"

######################### Search product without match (2) #########################

@given(u'User is on the homepage without match')
def step_impl(context):
    context.driver.get("http://opencart:8080/")


@when(u'User clicks into the search bar without match')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").click()


@when(u'User types <keyword>')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").send_keys("nothing")


@when(u'User clicks the search button without match')
def step_impl(context):
    context.driver.find_element(By.NAME, "search").send_keys(Keys.ENTER)


@when(u'There are no products related to "keyword"')
def step_impl(context):
    pass


@then(u'User should see a message "There is no product that matches the search criteria."')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, "p:nth-child(7)").text == "There is no product that matches the search criteria."

######################### Search using Categories (3) #########################

@given(u'User is on the homepage')
def step_impl(context):
    context.driver.get("http://opencart:8080/")

@when(u'User hovers over <categories>')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Desktops").click()

@when(u'User chooses one and clicks on it')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Mac (1)").click()

@then(u'User should be directed to Categories page')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/catalog/desktops/mac")

@then(u'Products of chosen category should be displayed')
def step_impl(context):
    assert context.driver.find_element(By.LINK_TEXT, "iMac").text == "iMac"

######################### View product details (4) #########################

@given(u'User is on the search results page')
def step_impl(context):
    context.driver.get("http://opencart:8080/index.php?route=product/search&language=en-gb&search=iphone")

@when(u'User clicks on a product')
def step_impl(context):
    context.driver.find_element(By.CSS_SELECTOR, ".image .img-fluid").click()


@then(u'User should be directed to the product details page')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")


@then(u'Detailed information about the product should be displayed')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, ".intro").text != None
    assert context.driver.find_element(By.CSS_SELECTOR, "h1").text == "iPhone"


######################### Add product to cart (5) #########################

@given(u'User is on the product details page')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")


@when(u'User clicks on the "Add to Cart" button')
def step_impl(context):
    context.driver.find_element(By.ID, "button-cart").click()


@then(u'Popup success message should be displayed')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, ".alert").text == "Success: You have added iPhone to your shopping cart!"


######################### Remove product from cart (6) #########################

@given(u'User has added a product to his shopping cart')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")
    context.driver.find_element(By.ID, "button-cart").click()


@given(u'Has his cart displayed')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb?route=checkout/cart")


@when(u'User clicks the "Remove" button')
def step_impl(context):
    time.sleep(1)
    context.driver.find_element(By.CSS_SELECTOR, ".btn:nth-child(4) > .fa-solid").click()
    time.sleep(1)

@then(u'The product should be removed from his cart')
def step_impl(context):
    WebDriverWait(context.driver, 3).until(EC.invisibility_of_element_located((By.CSS_SELECTOR, '.table-responsive')))
    
    cart_items = context.driver.find_elements(By.CSS_SELECTOR, '.table-responsive tbody tr')
    assert len(cart_items) == 0, "Product still present in the cart"
    