import time
from ConfigParser import SafeConfigParser
from time import strftime
import importlib
from selenium import webdriver

import requests
from requests.exceptions import ChunkedEncodingError
from bs4 import BeautifulSoup

from Handlers import Logger
from Parsers import getgoods
from Parsers import mondodigitaleshop


class Crawler():
    def __init__(self, website, url, identifiers, javascriptCrawler):
        self.parseConfigFile()
        self.website = website
        self.url = url
        self.identifiers = identifiers
        self.javascriptCrawler = javascriptCrawler

        self.headers = {
            'User-Agent': self.agent,
            'Connection': 'close'
        }

        self.websiteName = website[website.find('.')+1:website.rfind('.')]

    start_time = 0

    agent = ''
    headers = {}

    soup = ''

    result = ''

    phantomJSPath = ''

    # Procedure to control the main flow of the crawler
    def main(self):
        print 'Crawling ' + self.url
        self.start_time = time.time()

        if self.javascriptCrawler == 'False':
            r = self.getRequest()

            # If the returned value is a list, something went wrong.
            if isinstance(r, list):
                return r

            data = r.text

            self.soup = BeautifulSoup(data, 'html.parser')
        elif self.javascriptCrawler == 'True':
            self.soup = self.getJavascriptRequest()

            # If the returned value is a list, something went wrong.
            if isinstance(self.soup, list):
                return self.soup

        self.checkPage()

        return self.result

    # Parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
        self.agent = parser.get('General', 'agent')
        self.phantomJSPath = parser.get('General', 'phantomjspath')

    # This procedure makes the request to the server and checks if it's a valid response.
    def getRequest(self):
        try:
            reqTime = strftime("%H:%M:%S")
            r = requests.get(self.url, headers=self.headers, timeout=7)

            #Log the request details
            Logger.logRequest(self.websiteName, reqTime, self.url, r.status_code, r.elapsed.total_seconds(), r.headers['content-type'])

        except (requests.ConnectionError, ChunkedEncodingError, requests.exceptions.ReadTimeout) as e:
            Logger.logError(self.websiteName, reqTime, self.url, error=e)
            return [self.url]

        # Check for bad response.
        try:
            r.raise_for_status()
        except requests.exceptions.HTTPError as e:
            Logger.logError(self.websiteName, reqTime, self.url, error=e, httpResponse=r.status_code)
            return [self.url]

        return r

    # This procedure gets a request from a site which requires execution of javascript code.
    def getJavascriptRequest(self):
        webdriver.DesiredCapabilities.PHANTOMJS['phantomjs.page.customHeaders.User-Agent'] = self.agent
        webdriver.DesiredCapabilities.PHANTOMJS['phantomjs.page.customHeaders.Connection'] = 'close'
        driver = webdriver.PhantomJS(executable_path=self.phantomJSPath)

        reqTime = strftime("%H:%M:%S")
        start_time = time.time()
        driver.get(self.url)
        reqDuration = time.time() - start_time

        # Get the HTTP response and content-type.
        log = driver.get_log("har")
        message = log[0]['message']

        statusIndex = message.find('status') + 8
        contentTypeIndex = message.find('Content-Type')
        contentType = message[contentTypeIndex:].find(':') + contentTypeIndex + 2

        try:
            responseStatus = int(message[statusIndex:message[statusIndex:].find(',') + statusIndex])
        except ValueError:
            responseStatus = None
        contentType = message[contentType:message[contentType:].find('}') + contentType - 1]

        if responseStatus != 200:  # Something went wrong
            Logger.logError(self.websiteName, reqTime, self.url, httpResponse=responseStatus)
            driver.quit()
            return [self.url]
        else:
            #Log the request details
            Logger.logRequest(self.websiteName, reqTime, self.url, responseStatus, reqDuration, contentType)

            soup = BeautifulSoup(driver.page_source)
            driver.quit()

            return soup

    # Procedure to check which page of the website is being crawled
    def checkPage(self):
        parser = importlib.import_module('Parsers.' + self.websiteName)

        if self.url == self.website:  # Home page
            self.result = parser.Parser(self.soup, 'home', self.website, self.url).main()
            return
        for identifier in self.identifiers:
            if self.soup.find(self.identifiers[identifier][0], {self.identifiers[identifier][1]: self.identifiers[identifier][2]}) is not None:

                self.result = parser.Parser(self.soup, identifier, self.website, self.url).main()
                break