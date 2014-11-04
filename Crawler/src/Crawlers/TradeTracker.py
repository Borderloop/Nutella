import suds
import urllib
from CrawlerHelpScripts import logger
import time
import traceback
from ConfigParser import SafeConfigParser


class Crawler():
    def __init__(self):
        self.parseConfigFile()

    prevUrl = ""  # Used to check if the previous campaign is from the same company.
    x = 2  # Used to name files of companies that use multiple xml files

    log = logger.createLogger("TradeTrackerLogger", "TradeTracker")
    
    # Authentication and feed data
    wsdlurl = ''
    customerId = ''
    key = ''
    aid = ''
    feedPath = ''
    
    def main(self):
        # Create client and authenticate
        client = suds.client.Client(self.wsdlurl)
        client.service.authenticate(self.customerId, self.key)

        self.gatherData(client)

    # Procedure to parse the config file
    def parseConfigFile(self):
        # Get credentials
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/Boris/settings/boris.ini')
        self.customerId = parser.get('TradeTracker', 'customerId')
        self.key = parser.get('TradeTracker', 'key')
        self.aid = parser.get('TradeTracker', 'aid')
        self.wsdlurl = parser.get('TradeTracker', 'wsdlurl')
        self.feedPath = parser.get('TradeTracker', 'feedpath')

    # Gathers the data needed for downloading and correct file name
    def gatherData(self, client):
        # Return all the feeds available for our account. Save the id's for the assigned
        # campaigns and get the xml files for them.
        for feed in client.service.getFeeds(self.aid):

            if feed.assignmentStatus == 'accepted':  # Campaign needs to be accepted
                
                if feed.name.lower() == "algemeen (csv)" or "OUD" in feed.name:  # We don't want these
                    continue

                if 'losse toestellen' in feed.name.lower() or ('algemeen' in feed.name.lower() and feed.campaign.name.lower() != 'belsimpel.nl' and feed.campaign.name.lower() != 'phoneshop.nl') or feed.name.lower() == 'shop' or feed.name.lower() == 'tablets' or feed.name.lower() == 'gehele aanbod':
                    
                    if feed.campaign.name.lower() == 'digidingen.nl' and feed.name != 'Algemeen + EAN': #Don't want this one either
                        continue
                    
                    print 'Crawling ' + feed.campaign.name

                    self.log.info(str(time.asctime( time.localtime(time.time()) ))+": " + feed.campaign.name + " - " + feed.name)
                    
                    campaignUrl = feed.campaign.URL
                    if campaignUrl[len(campaignUrl)-3:] == '/nl':  # Prevent 'no such directory' error
                        campaignUrl = campaignUrl[:-3]
                    elif campaignUrl[len(campaignUrl)-1:] == '/':
                        campaignUrl = campaignUrl[:len(campaignUrl)-1]
                        
                    # Don't use the 'http://' for the file name.
                    campaignUrl = campaignUrl[campaignUrl.find("//") + 2:]
                    
                    # Replace remaining slashes with dollar signs, which are converted into slashes again by BOB.
                    campaignUrl = campaignUrl.replace('/', '$')
                    
                    # If the previous url equals the current url, this is a nth file of the same company.
                    # Add a number to the end to make the filename unique and so it doesnt get overwritten.
                    if self.prevUrl == campaignUrl:
                        self.log.info("nth child for: " + campaignUrl)
                        campaignUrl = campaignUrl + " " + str(self.x)
                        self.x += 1
                    else:
                        self.x = 2  # Always reset x if a new company is found
                        
                    self.save(campaignUrl, feed.URL)
                    
    # Downloads and saves the xml file under the correct name
    def save(self, campaignUrl, feedURL):

        # If the save fails, something is wrong with the file or directory name. Catch this error
        try:
            xmlFile = urllib.URLopener()
            xmlFile.retrieve(feedURL, self.feedPath + campaignUrl + ".xml")
            print campaignUrl + " Saved."
        except Exception:
            self.log.error(str(time.asctime(time.localtime(time.time()))) + ": " + traceback.format_exc())
            self.log.info(str(time.asctime(time.localtime(time.time()))) + ": Failed:" + campaignUrl)
        
        self.prevUrl = campaignUrl