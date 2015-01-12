import urllib
import time

from ConfigParser import SafeConfigParser

from CrawlerHelpScripts import logger


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    feedPath = ''
    csvURL = ''

    log = logger.createLogger("JacobElektronikLogger", "JacobElektronik")

    def main(self):
        self.save()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.feedPath = parser.get('JacobElektronik', 'feedpath')
        self.csvURL = parser.get('JacobElektronik', 'csvurl')

    def save(self):
        print 'Crawling Jacob Elektronik'

        #If the save fails, catch the error.
        try:
            csvFeed = urllib.URLopener()
            csvFeed.retrieve(self.csvURL, self.feedPath + 'www.jacob-computer.de' + ".txt")
            print 'Done crawling Jacob Elektronik'
        except Exception as e:
            self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + str(e))