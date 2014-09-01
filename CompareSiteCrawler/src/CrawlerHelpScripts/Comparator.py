from Database import Database
import logger
import time

log = logger.createLogger("comparatorLogger", "comparator")

'''
The comparator is used to compare the crawler product data to the data stored in the database. If the data is not
the same, the database needs to be updated
'''
class Comparator():
    
    def __init__(self, titles, brands, prices, availabilitys, stocks, eans, producturls, weburl, images):
        self.titles = titles
        self.brands = brands
        self.prices = prices
        self.availabilitys = availabilitys
        self.stocks = stocks
        self.eans = eans
        self.producturls = producturls
        self.weburl = weburl
        self.images = images
        self.db = Database.Queries()

    def main(self):
        self.db.openConnection()#Open database connection before starting
        #For every product, compare and save data
        for self.title, self.brand, self.price, self.availability, self.stock, self.ean, self.producturl, self.image in zip(self.titles, self.brands, self.prices, self.availabilitys, self.stocks, self.eans, self.producturls, self.images):
            self.db.setInfo(self.title, self.brand, self.price, self.availability, self.stock, self.ean, self.producturl, self.weburl, self.image)
            self.db.saveProducts()
            self.compare()
        self.db.closeConnection() #Close connection when done
       
    def compare(self):#Procedure used to compare gathered data with data from the database and save changes.
        deliveryInfo = self.db.getDeliveryInfo() #Get the current price and availability from the database first.
        
        if deliveryInfo == []: #If result is empty, product data is not inserted in db yet so do this
            log.info(str(time.asctime( time.localtime(time.time()) ))+ ' no record, saving for ' +self.weburl+ ' for ' + self.title)
            self.db.saveDeliveryInfo()
        else: #Else if the result is not empty, check if the result are the same as the just crawled data. If not, update the database
            log.info(str(time.asctime( time.localtime(time.time()) ))+ ' checking for updates for ' +self.weburl+ ' for ' + self.title)
            if str(deliveryInfo[0]) != self.price or deliveryInfo[1] != str(self.availability) or deliveryInfo[2] != self.producturl:
                log.info(str(time.asctime( time.localtime(time.time()) ))+ ' updating db for ' +self.weburl)
                self.db.update()