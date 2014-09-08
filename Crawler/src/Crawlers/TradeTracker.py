import suds
import urllib
from CrawlerHelpScripts import logger
import time
import traceback
from ConfigParser import SafeConfigParser

class Crawler():

    url = "http://ws.tradetracker.com/soap/affiliate?wsdl"
    
    prevName = "" #Used to check if the previous campaign is from the same company.
    x = 2 #Used to name files of companys that use multiple xml files
        
    log = logger.createLogger("TradeTrackerLogger", "TradeTracker")
    
    #Authentication and feed data
    customerId = ''
    key = ''
    aid = ''
    
    def main(self):
        start_time = time.time()
        
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Starting with TradeTracker")
        
        #Get credentials
        parser = SafeConfigParser()
        parser.read('C:/Crawler/crawler.ini')
        self.customerId = parser.get('TradeTracker', 'customerId')
        self.key = parser.get('TradeTracker', 'key')
        self.aid = parser.get('TradeTracker', 'aid')
        
        #Create client and authenticate
        client = suds.client.Client(self.url)
        client.service.authenticate(self.customerId, self.key)
        
        self.gatherData(client)
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Finished crawling TradeTracker in: " + str((time.time() - start_time)))
        
    #Gathers the data needed for downloading and correct file name    
    def gatherData(self, client):
        #Return all the feeds available for our account. Save the id's for the assigned 
        #campaigns and get the xml files for them.
        for feed in client.service.getFeeds(self.aid):
            if feed.assignmentStatus == 'accepted' and 'abonnement' not in feed.name.lower() and 'sim only' not in feed.name.lower() and 'simonly' not in feed.name.lower(): #Subscription feeds must be left out too.
                self.log.info(str(time.asctime( time.localtime(time.time()) ))+": " + feed.campaign.name + " - " + feed.name)
            
                campaignName = feed.campaign.name
                if campaignName[len(campaignName)-3:] == '/nl': #Prevent 'no such directory' error
                    campaignName = campaignName[:-3]
                
                #If the previous name equals the current name, this is a nth file of the same company. Add a number to the end to make
                #the filename unique and so it doesnt get overwritten.
                if self.prevName == campaignName:
                    self.log.info("nth child for: " + campaignName)
                    campaignName = campaignName + " " + str(self.x)
                    self.x = self.x+1
                else:
                    self.x = 2 #Always reset x if a new company is found 
                    
                self.save(campaignName, feed.URL)
        
    #Downloads and saves the xml file under the correct name      
    def save(self, campaignName, feedURL):
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Saving:      C:/Crawler/Product Feeds/TradeTracker/" + campaignName + ".xml")
        
        #If the save failes, something is wrong with the file or directory name. Catch this error
        try:
            xmlFile = urllib.URLopener()
            xmlFile.retrieve(feedURL, "C:/Crawler/Product Feeds/TradeTracker/" + campaignName + ".xml")
            self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Saved:       C:/Crawler/Product Feeds/TradeTracker/" + campaignName + ".xml")
        except:
            self.log.error(str(time.asctime( time.localtime(time.time()) ))+ ": " + traceback.format_exc())
            self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Failed:       C:/Crawler/Product Feeds/TradeTracker/" + campaignName + ".xml")
        
        self.prevName = campaignName
                