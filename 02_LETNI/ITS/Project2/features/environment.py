from selenium import webdriver
from selenium.common.exceptions import WebDriverException
from utils import *
import time

def get_driver():
    """Get Chrome/Firefox driver from Selenium Hub"""
    try:
        driver = webdriver.Remote(
            command_executor='http://localhost:4444/wd/hub',
            options=webdriver.ChromeOptions()
        )
        
        
    except WebDriverException:
        driver = webdriver.Remote(
                command_executor='http://localhost:4444/wd/hub',
                options=webdriver.FirefoxOptions())

    driver.implicitly_wait(15)
    return driver


# ---------------- BEHAVE PRESETS ---------------
def before_all(context):
    """Gets driver at the start of all test run"""
    time.sleep(15)
    # context.driver = get_driver()
    # context.base_url = "http://opencart:8080/"

def before_scenario(context, scenario):
    """Resets driver at the start of each scenario"""
    context.driver = get_driver()


#Uncomment to inspect each step of the testing

# def after_step(_, __):
#     """Pauses test after each step"""
#     time.sleep(1)

def after_scenario(context, scenario):
    """Releases driver at the end of each scenario"""
    if context.driver:
        context.driver.quit()

def after_all(context):
    """Releases driver at the end of all test run"""
    # if context.driver:
    #     context.driver.quit()

