from behave import *
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.action_chains import ActionChains

def scroll_to_element(driver, element):
    driver.execute_script("arguments[0].scrollIntoView(true);", element)

############ Purchase of product that is out of stock (7) ############

@given(u'User has added product(s) to the shopping cart')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/samsung-galaxy-tab-10-1")
    context.driver.find_element(By.ID, "button-cart").click()

@given(u'The product is out of stock')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, ".col-sm li:nth-child(3)").text == "Availability: Pre-Order"

@when(u'User clicks on the "Shopping cart" button')
def step_impl(context):
    time.sleep(5)
    context.driver.find_element(By.CSS_SELECTOR, ".list-inline-item:nth-child(4) .d-none").click()

@then(u'User should be directed to the Shopping cart page')
def step_impl(context):
    assert context.driver.current_url == "http://opencart:8080/en-gb?route=checkout/cart"

@then(u'User should receive a message that the product is out of stock')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, ".alert").text == "Products marked with *** are not available in the desired quantity or not in stock!"
    context.driver.find_element(By.CSS_SELECTOR, ".btn:nth-child(4) > .fa-solid").click()
    time.sleep(.2)

############### Inspect content of Shopping cart (8) ###############

@given(u'User has added product(s) to the shopping cart that is/are avaliable')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")
    context.driver.find_element(By.ID, "button-cart").click()
    context.driver.execute_script("window.scrollTo(0, 0);")
    time.sleep(5)

@when(u'User clicks on the "Shopping cart" button at the top')
def step_impl(context):
    btn_cart = context.driver.find_element(By.CSS_SELECTOR, ".list-inline-item:nth-child(4) .d-none")
    scroll_to_element(context.driver, btn_cart)
    time.sleep(.2)
    btn_cart.click()

@then(u'User should be directed straight to the Shopping cart page')
def step_impl(context):
    assert context.driver.current_url == "http://opencart:8080/en-gb?route=checkout/cart"

@then(u'All products that have been added to the cart should be displayed')
def step_impl(context):
    assert context.driver.find_element(By.LINK_TEXT, "iPhone").text == "iPhone"
    assert context.driver.find_element(By.CSS_SELECTOR, "tbody .text-start:nth-child(3)").text == 'product 11'

############### Proceed to checkout from Cart (9) ###############

@given(u'User is on the Shopping cart page')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")
    context.driver.find_element(By.ID, "button-cart").click()
    time.sleep(.2)
    context.driver.get("http://opencart:8080/en-gb?route=checkout/cart")



@given(u'User has product(s) in the shopping cart that is/are avaliable')
def step_impl(context):
    assert context.driver.find_element(By.LINK_TEXT, "iPhone").text != None
    context.driver.execute_script("window.scrollBy(0, 500);")
    time.sleep(2)

@when(u'User clicks on the "Checkout" button')
def step_impl(context):
    
    element = WebDriverWait(context.driver, 2).until(EC.element_to_be_clickable((By.XPATH, "//a[contains(text(),'Checkout')]")))
    element.click()

@then(u'User should be directed to the checkout page')
def step_impl(context):
    time.sleep(1)
    context.driver.get("http://opencart:8080/en-gb?route=checkout/checkout")

@then(u'The summary of the order should be displayed')
def step_impl(context):
    pass


############### Proceed to checkout from Product Details (10) ###############

@given(u'There is avaliable product that User added to the shopping cart')
def step_impl(context):
    pass


@when(u'User clicks on the "Checkout" button after clicking the Cart Button')
def step_impl(context):
    pass


@then(u'User should be directed straight to the checkout page')
def step_impl(context):
    pass


@then(u'The summary of the order should be displayed as in previous test')
def step_impl(context):
    pass

##################### Checkout Process (11) #####################

@given(u'User is on the checkout page')
def step_impl(context):
    context.driver.get("http://opencart:8080/en-gb/product/iphone?search=iphone")
    context.driver.find_element(By.ID, "button-cart").click()
    time.sleep(.2)
    context.driver.get("http://opencart:8080/en-gb?route=checkout/checkout")
    time.sleep(.2)

@given(u'User fills in user details')
def step_impl(context):
    input_register = context.driver.find_element(By.ID, "input-guest")
    scroll_to_element(context.driver, input_register)
    input_register.click()
    time.sleep(.2)

    input_firstname = context.driver.find_element(By.ID, "input-firstname")
    scroll_to_element(context.driver, input_firstname)
    time.sleep(.2)
    input_firstname.click()
    input_firstname.send_keys("John")

    input_lastname = context.driver.find_element(By.ID, "input-lastname")
    scroll_to_element(context.driver, input_lastname)
    time.sleep(.2)
    input_lastname.click()
    input_lastname.send_keys("Doe")

    input_email = context.driver.find_element(By.ID, "input-email")
    scroll_to_element(context.driver, input_email)
    time.sleep(.2)
    input_email.click()
    input_email.send_keys("john.doe@example.com")


    input_shipping_address = context.driver.find_element(By.ID, "input-shipping-address-1")
    scroll_to_element(context.driver, input_shipping_address)
    time.sleep(.2)
    input_shipping_address.click()
    input_shipping_address.send_keys("NewStreet 21")

    input_shipping_city = context.driver.find_element(By.ID, "input-shipping-city")
    scroll_to_element(context.driver, input_shipping_city)
    time.sleep(.2)
    input_shipping_city.click()
    input_shipping_city.send_keys("Brno")

    input_shipping_country = context.driver.find_element(By.ID, "input-shipping-country")
    scroll_to_element(context.driver, input_shipping_country)
    time.sleep(.2)
    dropdown_country = context.driver.find_element(By.ID, "input-shipping-country")
    time.sleep(.2)
    dropdown_country.find_element(By.XPATH, "//option[. = 'Czech Republic']").click()
    time.sleep(.2)

    input_shipping_postcode = context.driver.find_element(By.ID, "input-shipping-postcode")
    scroll_to_element(context.driver, input_shipping_postcode)
    time.sleep(.2)
    input_shipping_postcode.click()
    time.sleep(.2)
    input_shipping_postcode.send_keys("65432")

    input_shipping_zone = context.driver.find_element(By.ID, "input-shipping-zone")
    scroll_to_element(context.driver, input_shipping_zone)
    time.sleep(.2)
    # context.driver.find_element(By.ID, "input-shipping-zone").click()
    dropdown = context.driver.find_element(By.ID, "input-shipping-zone")
    dropdown.find_element(By.XPATH, "//option[. = 'Jihomoravský']").click()
    time.sleep(.5)
    


@given(u'User clicks on the continue button')
def step_impl(context):
    wait = WebDriverWait(context.driver, 5)  # Adjust the timeout as needed
    btn = wait.until(EC.element_to_be_clickable((By.ID, "button-register")))
    btn.click()
    time.sleep(5)


@then(u'User should be able to select payment and delivery options')
def step_impl(context):
    # wait = WebDriverWait(context.driver, 10)  # Adjust the timeout as needed
    # shipping_button = wait.until(EC.element_to_be_clickable((By.ID, "button-shipping-methods")))
    # actions = ActionChains(context.driver)
    # actions.move_to_element(shipping_button).perform()

    shipping_button = context.driver.find_element(By.ID, "button-shipping-methods")
    scroll_to_element(context.driver, shipping_button)


    time.sleep(.5)
    shipping_button.click()
    time.sleep(.5)
    context.driver.find_element(By.ID, "input-shipping-method-flat-flat").click()
    time.sleep(.5)
    context.driver.find_element(By.ID, "button-shipping-method").click()
    time.sleep(.5)
    payment = context.driver.find_element(By.ID, "button-payment-methods")
    # scroll_to_element(context.driver, payment)
    payment.click()
    time.sleep(.5)
    context.driver.find_element(By.ID, "input-payment-method-cod-cod").click()
    time.sleep(.5)
    context.driver.find_element(By.ID, "button-payment-method").click()
    time.sleep(.5)

@then(u'User confirms the order')
def step_impl(context):
    wait = WebDriverWait(context.driver, 10)  # Adjust the timeout as needed
    confirm_btn = wait.until(EC.element_to_be_clickable((By.ID, "button-confirm")))
    actions = ActionChains(context.driver)
    actions.move_to_element(confirm_btn).perform()
    confirm_btn.click()
    time.sleep(.2)


@then(u'User should be directed to the order confirmation page')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, "h1").text == "Your order has been placed!"
    # assert context.driver.current_url == "http://opencart:8080/en-gb?route=checkout/success"

