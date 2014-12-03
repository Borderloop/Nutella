import urllib2
import urllib

from CrawlerHelpScripts import logger

try:
    import xml.etree.cElementTree as ET
except ImportError:
    import xml.etree.ElementTree as ET

import time
import traceback
from ConfigParser import SafeConfigParser


class Crawler():
    def __init__(self):
        self.parseConfigFile()
    
    token = ''
    feedPath = ''

    feedId = ''
    name = ''

    log = logger.createLogger("WehkampLogger", "Wehkamp")

    # Procedure to control the main flow of the program.
    def main(self):
        start_time = time.time()
        self.log.info(str(time.asctime(time.localtime(time.time())))+": Starting with Wehkamp")

        #Add token to the url. This url links to a xml file that contains program data
        url = "http://api.tradedoubler.com/1.0/productFeeds.xml?token=%s" % self.token
        xmlfile = urllib2.urlopen(url)

        #Create a string from the data
        data = xmlfile.read()
        xmlfile.close()

        root = ET.fromstring(data)
        #Each child is an affiliate, so for each one get feedID and name.
        for record in root:
            for child in record:
                self.gatherData(child)
        
        self.log.info(str(time.asctime(time.localtime(time.time())))+": Finished crawling Wehkamp in: " + str((time.time() - start_time)))

    # This procedure parses the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.token = parser.get('Wehkamp', 'token')
        self.feedPath = parser.get('Wehkamp', 'feedpath')
    
    #Gathers the data needed for downloading and correct file name            
    def gatherData(self, child):
        #Extract feedId, name and numberOfProducts from xml file
        if child.tag == "{urn:com:tradedoubler:pf:model:xml:common}feedId":
            self.feedId = child.text
            
        if child.tag == "{urn:com:tradedoubler:pf:model:xml:common}name":
            self.name = child.text

            if self.name == 'Wehkamp Algemene Feed':
                self.save()

    #Downloads and saves the xml file under the correct name      
    def save(self):
        print 'Saving Wehkamp'
        feedURL = "http://api.tradedoubler.com/1.0/productsUnlimited.xml;fid=%s?token=%s" % (self.feedId, self.token)

        #If the save fails, something is wrong with the file or directory name. Catch this error
        try:
            xmlFile = urllib.URLopener()
            xmlFile.retrieve(feedURL, self.feedPath + self.name + ".xml")
            print 'Done saving wehkamp'
        except Exception as e:
            self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + traceback.format_exc())
            self.log.info(str(time.asctime(time.localtime(time.time()))) + ": Failed:" + str(e))