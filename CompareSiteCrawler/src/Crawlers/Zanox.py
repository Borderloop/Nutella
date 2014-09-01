import urllib2
from CrawlerHelpScripts import Comparator
from Database import Database
try:
    import xml.etree.cElementTree as ET
except ImportError:
    import xml.etree.ElementTree as ET
import re
import time
from CrawlerHelpScripts import logger
from threading import Thread

log = logger.createLogger("zanoxLogger", "zanox")

class Crawler():
    '''
    Create the lists that will be passed trough to the comparator. 
    This is so the data will be saved in batches
    '''
    titles = []
    brands = []
    prices = []
    availabilitys = []
    stocks = []
    eans = []
    producturls = []
    weburls = []
    images = []
    
    def main(self):
        start_time = time.time()
        log.info('starting crawler')
        #First get the programmes with download links from the database and the ean numbers of products to search for
        db = Database.Queries()
        db.openConnection()
        programmes = db.getZanoxProgrammes()
        db.closeConnection()
        #Download xml file for each programme 
        for programme in programmes:
            self.weburl = programme[0] #Used later for database to find website id
            log.info(str(time.asctime( time.localtime(time.time()) ))+ ' busy with'  +programme[0])
            xmlfile = urllib2.urlopen(programme[1])
            #convert to string:
            data = xmlfile.read()
            xmlfile.close()
            #create the root from data
            root = ET.fromstring(data)
            #find all records
            self.records = root.findall("data/record")
            self.i = 0 #Used to check if the last record is reached.
            self.getInfo()
            
        log.info(str(time.asctime( time.localtime(time.time()) ))+ ' completed in: ' + str((time.time() - start_time)))
    
    def getInfo(self):
        self.noTitle = False #Chech if the title is false or not
        #For each item in the list, go trough the child elements, which is the product data.   
        for index in self.records:
            #Gather all the product data
            for child in index.iterfind('column[@name="ean"]'):
                try:
                    ean = str(child.text)
                except:#If unicode error appears, assign 'Error' to ean so it will not pass validation
                    ean = 'Error'
                    
            #Validate ean
            match = re.match('[0-9]{10,13}', ean)
            if match and self.is_number(ean) == True:#If the ean code is not valid, don't execute remaining code
                match =  None #reset match so not every ean will validate
                self.eans.append(ean)
                
                for child in index.iterfind('column[@name="title"]'):
                    if child.text != None:
                        self.titles.append(child.text.encode('utf-8'))
                    else:
                        self.noTitle=True

                for child in index.iterfind('column[@name="vendor"]'):
                    if child.text != None and self.noTitle == False:
                        self.brands.append(child.text.encode('utf-8'))
                    else:
                        self.brands.append("")
                    
                for child in index.iterfind('column[@name="price"]'):
                    if child.text != None and self.noTitle == False:
                        price = child.text.encode('utf-8')
                        #Replace , with a '.' to avoid truncated data
                        self.prices.append(price.replace(',', '.'))
            
                        decimalFinder = self.prices[-1].find('.') 
                        if decimalFinder == -1: #If it is a round number, add decimals so the db won't be updated unnecessary
                            self.prices[-1] = self.prices[-1] + '.00'
                    else:
                        self.prices.append("")
                        
                for child in index.iterfind('column[@name="timetoship"]'):
                    if child.text != None and self.noTitle == False:
                        self.availabilitys.append(child.text.encode('utf-8'))
                    else:
                        self.availabilitys.append("")
                    
                for child in index.iterfind('column[@name="stock"]'):
                    if child.text != None and self.noTitle == False:
                        self.stocks.append(child.text.encode('utf-8'))
                    else:
                        self.stocks.append("")
                    
                for child in index.iterfind('column[@name="url"]'):
                    if child.text != None and self.noTitle == False:
                        self.producturls.append(child.text.encode('utf-8'))
                    else:
                        self.producturls.append("")
                    
                for child in index.iterfind('column[@name="image"]'):
                    if child.text != None and self.noTitle == False:
                        self.images.append(child.text.encode('utf-8'))
                    else:
                        self.images.append("")

                self.noTitle = False#Reset the noTitle variable, so not every record gets skipped.
                if self.i == len(self.records) - 1:
                    log.info("Last record reached for " + self.weburl)
                if len(self.titles) == 1000 or self.i == len(self.records) -1:#Start comparing when 1000 products have been crawled or when the last record of the current website has been reached
                    log.info(str(time.asctime( time.localtime(time.time()) ))+ ' starting comparison')
                    comparator = Comparator.Comparator(self.titles, self.brands, self.prices, self.availabilitys, self.stocks, self.eans, self.producturls, self.weburl, self.images)
                    comparator.main()
                    
                    #Reset lists
                    self.titles = []
                    self.brands = []
                    self.prices = []
                    self.availabilitys = []
                    self.stocks = []
                    self.eans = []
                    self.producturls = []
                    self.images = []
    
            self.i = self.i + 1 
    
    def is_number(self, ean):#Used to check whether the ean contains only numbers
        try:
            float(ean)
            return True
        except ValueError:
            return False
    
class Thread(Thread):
 
    def run(self):
        Crawler().main()