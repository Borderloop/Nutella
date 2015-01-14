import urllib2
import urllib
import time

try:
    import xml.etree.cElementTree as ET
except ImportError:
    import xml.etree.ElementTree as ET

from ConfigParser import SafeConfigParser

from CrawlerHelpScripts import logger


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    key = ''
    filter = ''
    feedPath = ''

    prevUrl = ""  # Used to check if the previous campaign is from the same company.
    x = 2  # Used to name files of companies that use multiple xml files

    log = logger.createLogger("EffiliationLogger", "Effiliation")

    # This procedure controls the main flow of the program.
    def main(self):
        root = self.getXMLFeed()

        if root != None:
            self.gatherData(root)

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.key = parser.get('Effiliation', 'key')
        self.filter = parser.get('Effiliation', 'filter')
        self.feedPath = parser.get('Effiliation', 'feedPath')

    # This procedure gets the xml feed, which contains all programs and links to product feeds.
    def getXMLFeed(self):
        xmlFeedUrl = 'http://apiv2.effiliation.com/apiv2/productfeeds.xml?key=' + self.key + '&filter=mines&fields=' + self.filter

        tries = 0
        while True:
            try:
                xmlfile = urllib2.urlopen(xmlFeedUrl)
            except IOError:
                tries += 1
                time.sleep(1)
                if tries == 5:
                    return None

        #Create a string from the data
        data = xmlfile.read()
        xmlfile.close()
        return ET.fromstring(data)

    # Gathers the website URL and feed URL for each website and calls the save method for it.
    def gatherData(self, root):
        # Each record is an affiliate, so for each one get the url of the website and URL of the feed.
        # Each child is a element in the xml for the given website.
        for record in root:
            for child in record:
                
                if child.tag == 'url_affilieur':  # Website URL
                    websiteURL = child.text.replace('http://', '')
                if child.tag == 'code':  # Product feed URL
                    if 'priceminister' not in websiteURL:
                        feedURL = child.text

                        # If the previous url equals the current url, this is a nth file of the same company.
                        # Add a number to the end to make the filename unique and so it doesnt get overwritten.
                        if self.prevUrl == websiteURL:
                            websiteURL = websiteURL + " " + str(self.x)
                            self.x += 1

                        self.save(websiteURL, feedURL)

    # Downloads and saves the xml file under the correct name
    def save(self, websiteURL, feedURL):
        print 'Crawling ' + websiteURL

        tries = 0
        while True:
            #If the save fails, catch the error.
            try:
                xmlFeed = urllib.URLopener()
                xmlFeed.retrieve(feedURL, self.feedPath + websiteURL + ".xml")
                print 'Done crawling ' + websiteURL

                break
            except IOError:
                tries += 1
                time.sleep(1)
                if tries == 5:
                    break
            except Exception as e:
                self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + str(e))

                break

        self.prevUrl = websiteURL