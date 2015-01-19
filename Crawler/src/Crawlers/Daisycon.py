import subprocess
import urllib
import time

from ConfigParser import SafeConfigParser
from threading import Thread, Lock

from CrawlerHelpScripts import logger


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    feedPath = ''
    amountOfThreads = ''
    urlFilesPath = ''
    phpPath = ''

    amountOfThreads = ''
    activeThreads = 0

    log = logger.createLogger("DaisyconLogger", "Daisycon")

    lock = Lock()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.feedPath = parser.get('Daisycon', 'feedpath')
        self.amountOfThreads = int(parser.get('General', 'amountofthreads'))
        self.urlFilesPath = parser.get('Daisycon', 'urlfilespath')
        self.phpPath = parser.get('Daisycon', 'phppath')

    def main(self):
        self.fillUrlFile()
        feedData = self.loadFeedData()

        for feed in feedData:
            data = feed.split(';')
            websiteURL = data[0]
            feedURL = data[1]

            if websiteURL.find('/') != -1:
                websiteURL = websiteURL[:websiteURL.find('/')]

            if feedURL != '':
                self.startThread(websiteURL, feedURL)

    # This procedure clears the url txt file and fills it by calling the php script for it.
    def fillUrlFile(self):
        f = open(self.urlFilesPath, 'w')
        f.close()

        subprocess.call("php " + self.phpPath)

    # This procedure loads all data from the txt file, which contains website URL and its corresponding feed URL.
    def loadFeedData(self):
        f = open(self.urlFilesPath)
        feedData = f.read().splitlines()
        f.close()

        return feedData

    # Starts a thread to save feed.
    def startThread(self, websiteURL, feedURL):
        while True:
            if self.activeThreads < self.amountOfThreads:
                self.lock.acquire()
                try:
                    t = Thread(target=self.save, args=(websiteURL, feedURL,))
                    t.start()

                    self.activeThreads += 1
                finally:
                    self.lock.release()
                break

    def save(self, websiteURL, feedURL):
        print 'Crawling ' + websiteURL

        tries = 0
        while True:
            #If the save fails, catch the error.
            try:
                xmlFeed = urllib.URLopener()
                xmlFeed.retrieve(feedURL, self.feedPath + websiteURL + ".xml")
                xmlFeed.close()
                print 'Done crawling ' + websiteURL

                break
            except IOError:
                print 'Daisycon timed out'
                tries += 1
                time.sleep(7)
                if tries == 20:
                    break
            except Exception as e:
                self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + str(e))

                break

        self.lock.acquire()
        try:
            self.activeThreads -= 1
        finally:
            self.lock.release()