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
 
################ Inspect the list of products (12) ################

@given(u'User is on the admin Homepage')
def step_impl(context):
    context.driver.get("http://opencart:8080/administration/")
    time.sleep(1)
    context.driver.find_element(By.ID, "input-username").click()
    context.driver.find_element(By.ID, "input-username").send_keys("user")
    context.driver.find_element(By.ID, "input-password").click()
    context.driver.find_element(By.ID, "input-password").send_keys("bitnami")
    context.driver.find_element(By.CSS_SELECTOR, ".btn").click()

@when(u'User navigates to the Catalog')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Catalog").click()

@when(u'User click on the Product tab')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Products").click()

@then(u'List of available products should be displayed')
def step_impl(context):
    assert context.driver.find_element(By.LINK_TEXT, "Products").is_displayed()
    context.driver.find_element(By.CSS_SELECTOR, "#nav-logout .d-none").click()
    time.sleep(1.5)

@then(u'User should see their respective stock levels')
def step_impl(context):
    pass

################ Add new product (13) ################

@given(u'User is on the Products page - Add new product')
def step_impl(context):
    
    context.driver.get("http://opencart:8080/administration/")
    time.sleep(1)
    context.driver.find_element(By.ID, "input-username").click()
    context.driver.find_element(By.ID, "input-username").send_keys("user")
    context.driver.find_element(By.ID, "input-password").click()
    context.driver.find_element(By.ID, "input-password").send_keys("bitnami")
    context.driver.find_element(By.CSS_SELECTOR, ".btn").click()
    context.driver.find_element(By.LINK_TEXT, "Catalog").click()
    context.driver.find_element(By.LINK_TEXT, "Products").click()


@when(u'User clicks the "Add New" button')
def step_impl(context):
    time.sleep(.2)
    context.driver.find_element(By.CSS_SELECTOR, ".float-end > .btn-primary").click()
    time.sleep(.2)


@when(u'User fills in the details for a new product')
def step_impl(context):
    context.driver.find_element(By.ID, "input-name-1").click()
    context.driver.find_element(By.ID, "input-name-1").send_keys("Iphone 20X")
    input1 = context.driver.find_element(By.ID, "input-meta-title-1")
    scroll_to_element(context.driver, input1)
    time.sleep(1)
    input1.click()
    input1.send_keys("phone")
    time.sleep(1)

    data = context.driver.find_element(By.LINK_TEXT, "Data")
    scroll_to_element(context.driver, data)
    time.sleep(1)
    data.click()
    time.sleep(1)

    context.driver.find_element(By.ID, "input-model").click()
    context.driver.find_element(By.ID, "input-model").send_keys("Model 20")
    time.sleep(1)
    context.driver.find_element(By.LINK_TEXT, "SEO").click()
    context.driver.find_element(By.ID, "input-keyword-0-1").click()
    context.driver.find_element(By.ID, "input-keyword-0-1").send_keys("Iphone_20")
    time.sleep(1)


@when(u'User clicks on the "Save" button')
def step_impl(context):
    context.driver.find_element(By.CSS_SELECTOR, ".float-end > .btn-primary").click()
    time.sleep(1)
    context.driver.find_element(By.LINK_TEXT, "Catalog").click()
    context.driver.find_element(By.LINK_TEXT, "Products").click()
    time.sleep(1)


@then(u'The new product should be added to the system')
def step_impl(context):
    context.driver.find_element(By.ID, "input-name").click()
    context.driver.find_element(By.ID, "input-name").send_keys("Iphone 20X")
    context.driver.find_element(By.ID, "button-filter").click()
    time.sleep(1)
    assert 'Iphone 20X' in context.driver.find_element(By.CSS_SELECTOR, "tbody .text-start:nth-child(3)").text
    context.driver.find_element(By.CSS_SELECTOR, "#nav-logout .d-none").click()
    time.sleep(1.5)


@then(u'The initial stock level should be set')
def step_impl(context):
    pass

# ################ Update product details (14) ################

@given(u'There is an existing product in the system - Update product')
def step_impl(context):
    context.driver.get("http://opencart:8080/administration/")
    time.sleep(1)
    context.driver.find_element(By.ID, "input-username").click()
    context.driver.find_element(By.ID, "input-username").send_keys("user")
    context.driver.find_element(By.ID, "input-password").click()
    context.driver.find_element(By.ID, "input-password").send_keys("bitnami")
    context.driver.find_element(By.CSS_SELECTOR, ".btn").click()
    context.driver.find_element(By.LINK_TEXT, "Catalog").click()
    context.driver.find_element(By.LINK_TEXT, "Products").click()
    time.sleep(1)



@given(u'User is on the Products page - Update product')
def step_impl(context):
    pass


@when(u'User clicks the "Edit" button next to the product')
def step_impl(context):
    context.driver.find_element(By.CSS_SELECTOR, "tr:nth-child(1) .btn:nth-child(1)").click()
    time.sleep(1)


@when(u'User updates the details of the product')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Data").click()
    time.sleep(1)
    input1 = context.driver.find_element(By.ID, "input-price")
    scroll_to_element(context.driver, input1)
    time.sleep(1)

    context.driver.execute_script("arguments[0].value = '';", input1)
    time.sleep(1)

    input1.click()

    input1.send_keys("120.0000")
    time.sleep(1)
    


@when(u'User saves it the by clicking the "Save" button')
def step_impl(context):
    btn = context.driver.find_element(By.CSS_SELECTOR, ".fa-floppy-disk")
    scroll_to_element(context.driver, btn)
    time.sleep(2)
    btn.click()
    time.sleep(1)


@then(u'The product details should be updated accordingly')
def step_impl(context):
    context.driver.find_element(By.LINK_TEXT, "Products").click()
    time.sleep(1)
    assert context.driver.find_element(By.CSS_SELECTOR, "tr:nth-child(1) > .text-end:nth-child(5) > span").text == '$120.00'
    context.driver.find_element(By.CSS_SELECTOR, "#nav-logout .d-none").click()
    time.sleep(1.5)

# ################ Search for a product (15) ################

# (already tested in test 13)
@given(u'There are existing products in the system')
def step_impl(context):
    pass


@when(u'User searches for a specific product using filter options')
def step_impl(context):
    pass


@then(u'The search results should be displayed matching the search criteria')
def step_impl(context):
    pass

# ################ Delete product (16) ################

@given(u'User is on the Products page - Delete product')
def step_impl(context):
    context.driver.get("http://opencart:8080/administration/")
    time.sleep(1)
    context.driver.find_element(By.ID, "input-username").click()
    context.driver.find_element(By.ID, "input-username").send_keys("user")
    context.driver.find_element(By.ID, "input-password").click()
    context.driver.find_element(By.ID, "input-password").send_keys("bitnami")
    context.driver.find_element(By.CSS_SELECTOR, ".btn").click()
    context.driver.find_element(By.LINK_TEXT, "Catalog").click()
    context.driver.find_element(By.LINK_TEXT, "Products").click()
    time.sleep(1)


@given(u'There is an existing product in the system')
def step_impl(context):
    pass


@when(u'User checks the checkbox next to the product')
def step_impl(context):
    context.driver.find_element(By.NAME, "selected[]").click()
    time.sleep(1)


@when(u'User clicks the "Delete" button')
def step_impl(context):
    context.driver.find_element(By.CSS_SELECTOR, ".btn-danger").click()
    time.sleep(1)
    assert context.driver.switch_to.alert.text == "Are you sure?"
    time.sleep(1)
    context.driver.switch_to.alert.accept()
    time.sleep(1)


@then(u'The product should be removed from the system')
def step_impl(context):
    assert context.driver.find_element(By.CSS_SELECTOR, ".alert").text == "Success: You have modified products!" 
    time.sleep(5)
    context.driver.find_element(By.CSS_SELECTOR, "#nav-logout .d-none").click()
    time.sleep(1.5)
 