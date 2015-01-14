import urllib
import urllib2
import time
import traceback

try:
    import xml.etree.cElementTree as ET
except ImportError:
    import xml.etree.ElementTree as ET

from ConfigParser import SafeConfigParser
from threading import Thread, Lock

from CrawlerHelpScripts import logger


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    key = ''
    feedPath = ''

    amountOfThreads = ''
    activeThreads = 0

    log = logger.createLogger("DaisyconLogger", "Daisycon")

    lock = Lock()

    def main(self):
        self.getAdvertisers()

     # This procedure parses the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.key = parser.get('PepperjamNetwork', 'key')
        self.feedPath = parser.get('PepperjamNetwork', 'feedpath')
        self.amountOfThreads = int(parser.get('General', 'amountofthreads'))

    # This procedure gets id's and website urls from all joined advertiser programs.
    def getAdvertisers(self):
        url = "http://api.pepperjamnetwork.com/20120402/publisher/advertiser?apiKey=%s&format=xml&status=joined" % self.key
        advertiserXML = urllib2.urlopen(url)

        data = advertiserXML.read()
        advertiserXML.close()
        root = ET.fromstring(data)

        for record in root:
            if record.tag == 'data':  # Advertiser record
                for child in record:
                    if child.tag == 'id':
                        advertiserID = child.text
                    if child.tag == 'website':
                        websiteURL = child.text.replace('http://', '')

                        self.startThread(advertiserID, websiteURL)

    # Starts a thread to save feed.
    def startThread(self, advertiserID, websiteURL):
        while True:
            if self.activeThreads < self.amountOfThreads:
                self.lock.acquire()
                try:
                    t = Thread(target=self.save, args=(advertiserID, websiteURL))
                    t.start()

                    self.activeThreads += 1
                finally:
                    self.lock.release()
                break

    # Downloads and saves the csv file under the correct name
    def save(self, advertiserID, websiteURL):
        print 'Crawling ' + websiteURL

        feedURL = "http://api.pepperjamnetwork.com/20120402/publisher/creative/product?apiKey=%s&format=csv&programIds=%s" % (self.key, advertiserID)

        # If the save fails, something is wrong with the file or directory name. Catch this error
        tries = 0
        while True:
            try:
                csvFile = urllib.URLopener()
                csvFile.retrieve(feedURL, self.feedPath + websiteURL + ".csv")
                print 'Done crawling ' + websiteURL

                break
            except IOError:
                tries += 1
                time.sleep(1)
                if tries == 5:
                    break
            except:
                self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + traceback.format_exc())
                self.log.info(str(time.asctime(time.localtime(time.time()))) + ": Failed:" + websiteURL)

                break

        self.lock.acquire()
        try:
            self.activeThreads -= 1
        finally:
            self.lock.release()