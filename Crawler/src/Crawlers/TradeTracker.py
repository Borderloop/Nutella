import suds
import urllib
from CrawlerHelpScripts import logger
import time

class Crawler():

    url = "http://ws.tradetracker.com/soap/affiliate?wsdl"
    
    prevName = "" #Used to check if the previous campaign is from the same company.
    x = 2 #Used to name files of companys that use multiple xml files
        
    log = logger.createLogger("TradeTrackerLogger", "TradeTracker")
    
    #Authentication and feed data
    id = ''
    key = ''
    customerId = ''
    
    def main(self):
        start_time = time.time()
        
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Starting with TradeTracker")
        
        #Create client and authenticate
        client = suds.client.Client(self.url)
        client.service.authenticate(self.id, self.key)
        
        self.gatherData(client)
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Finished crawling TradeTracker in: " + str((time.time() - start_time)))
        
    #Gathers the data needed for downloading and correct file name    
    def gatherData(self, client):
        #Return all the feeds available for our account. Save the id's for the assigned 
        #campaigns and get the xml files for them.
        for feed in client.service.getFeeds(self.customerId):
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
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Saving:      C:/Product Feeds/TradeTracker/" + campaignName + ".xml")
        xmlFile = urllib.URLopener()
        xmlFile.retrieve(feedURL, "C:/Product Feeds/TradeTracker/" + campaignName + ".xml")
        self.log.info(str(time.asctime( time.localtime(time.time()) ))+": Saved:       C:/Product Feeds/TradeTracker/" + campaignName + ".xml")
        
        self.prevName = campaignName
                