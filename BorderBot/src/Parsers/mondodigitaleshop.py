import time
from ConfigParser import SafeConfigParser
from time import strftime

import requests
from requests.exceptions import ChunkedEncodingError
from bs4 import BeautifulSoup

from Handlers import Database
from Handlers import Logger


class Crawler():
    def __init__(self, website, url):
        self.parseConfigFile()
        self.website = website
        self.url = url

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

    # Procedure to control the main flow of the crawler
    def main(self):
        print 'Crawling ' + self.url
        self.start_time = time.time()

        r = self.getRequest()

        # If the returned value is a list, something went wrong.
        if isinstance(r, list):
            return r

        data = r.text

        self.soup = BeautifulSoup(data, 'html.parser')
        self.checkPage()

        return [self.result, time.time() - self.start_time]

    # Parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
        self.agent = parser.get('General', 'agent')

    # This procedure makes the request to the server and checks if it's a valid response.
    def getRequest(self):
        try:
            reqTime = strftime("%H:%M:%S")
            r = requests.get(self.url, headers=self.headers, timeout=7)

            #Log the request details
            Logger.logRequest(self.websiteName, reqTime, self.url, r.status_code, r.elapsed.total_seconds(), r.headers['content-type'])

        except (requests.ConnectionError, ChunkedEncodingError, requests.exceptions.ReadTimeout) as e:
            Logger.logError(self.websiteName, reqTime, self.url, r.status_code, e)
            return [[self.url], (time.time() - self.start_time)]

        # Check for bad response.
        try:
            r.raise_for_status()
        except requests.exceptions.HTTPError as e:
            Logger.logError(self.websiteName, reqTime, self.url, r.status_code, e)
            return [[self.url], (time.time() - self.start_time) * 5]

        return r

    # Procedure to check which page of the website is being crawled
    def checkPage(self):

        if self.url == self.website:  # Home page
            #self.result = Parser(self.soup, 'home', self.website, self.url).main()
            return

        # Identifiers contain tags, elements and contents.
        identifiers = dict(products=['div', 'class', 'box-content float-left'],
                           category=['div', 'class', 'module mod-black'],
                           subCategory=['div', 'class', 'module mod-box mod-box-header deepest'],
                           product=['table', 'id', 'item_specification'])

        for identifier in identifiers:
            if self.soup.find(identifiers[identifier][0], {identifiers[identifier][1]: identifiers[identifier][2]}) is not None:
                print "It's the " + identifier + " page!"
                # If it's the product page, add extension to the url for more products per page.
                # Then crawl the site again.
                if identifier == 'products' and self.url.find('?n=') == -1:
                    self.url += '?n=48'

                    r = self.getRequest()

                    # If the returned value is a list, something went wrong.
                    if isinstance(r, list):
                        return r

                    data = r.text
                    self.soup = BeautifulSoup(data, 'html.parser')

                #self.result = Parser(self.soup, identifier, self.website, self.url).main()
                break