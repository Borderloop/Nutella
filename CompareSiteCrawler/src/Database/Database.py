import MySQLdb as mdb
import types
from CrawlerHelpScripts import logger
import time

log = logger.createLogger("databaseLogger", "database")

class Connection:

    def __init__(self):
        self.host = "localhost"
        self.port = 3307
        self.user = "root"
        self.password = "Hoppa123" 
        self.database = "productsdb"
        
    
    def select(self, com, vals):
        try:
            self.cur = self.con.cursor()
            self.cur.execute(com,vals)
        
            ver = self.cur.fetchall()
            return ver
        except  mdb.Error as e:
            log.error(str(time.asctime( time.localtime(time.time()) )) + " " + str(e))
    
    def insert(self, com, vals):
        try:
            self.cur.execute(com, vals)
            self.con.commit()
        except mdb.Error as e:
            log.error(str(time.asctime( time.localtime(time.time()) )) + " " + str(e))
       
    def openConnection(self):
        log.info(str(time.asctime( time.localtime(time.time()) ))+ " opening connection")
        self.con = mdb.connect(host = self.host, port = self.port, user = self.user, passwd = self.password, db = self.database)
        self.cur = self.con.cursor()

    def closeConnection(self):
        if self.con:
            log.info(str(time.asctime( time.localtime(time.time()) ))+ " closing connection")
            self.con.close()
        
class Queries:
    db = Connection()
        
    #Set info needed to insert/select/update database
    def setInfo(self, title, brand, price, availability, stock, ean, producturl, weburl, image):
        self.title = title
        self.brand = brand
        self.price = price
        self.availability = availability
        self.stock = stock
        self.producturl = producturl
        self.weburl = weburl #used for foreign key assignments
        self.ean = ean #used for foreign key assignments
        self.image = image

    def saveProducts(self):#Save the product data to the database
        com = "INSERT IGNORE INTO Products (ean, brand, title, image_url) VALUES ((%s), (%s), (%s), (%s))"
        vals = [self.ean, self.brand, self.title, self.image]
        self.db.insert(com,vals)

    def saveDeliveryInfo(self):#Save the deliveryinfo to the database     
        com = "INSERT INTO deliveryinfo (price, availability, stock, url, website_url, product_ean) VALUES ((%s), (%s), (%s), (%s), (%s), (%s))" 
        vals = [str(self.price),str(self.availability), self.stock, str(self.producturl), self.weburl, self.ean]
        self.db.insert(com, vals);
        
    def getDeliveryInfo(self): #Procedure to get the delivery info from the database, used to check if anything changed.
        com = "SELECT price, availability, url FROM deliveryinfo WHERE website_url = (%s) AND product_ean = (%s)"
        vals = [self.weburl, self.ean]
        result = self.db.select(com, vals)
        li = []
        #Add all values to a list for easy comparison later on in the Comparator module.
        for row in result:
            li.append(row[0])
            li.append(row[1])
            li.append(row[2])
        
        return li
    
    def update(self): #Procedure to update the info
        com = "UPDATE deliveryinfo SET price = (%s), availability = (%s), url = (%s) WHERE website_url = (%s) AND product_ean = (%s)"
        vals = [self.price, self.availability, self.producturl, self.weburl, self.ean]
        self.db.insert(com, vals)
        
    def getZanoxProgrammes(self): #Procedures to get the Zanox programmes to use for parsing data from the Zanox API
        com = "SELECT website_url, feedurl FROM zanox ORDER BY website_url"
        result = self.db.select(com, None)
        return result
    
    def saveWebsiteInfo(self, weburl, name, country, shipping_price, sender, mark, payment_method): #Used to save websites to the database from WebsiteParser module
        #First save basic data to websites table
        com = "INSERT IGNORE INTO websites (url, name, country, shipping_price) VALUES ((%s), (%s), (%s), (%s))"
        vals = [weburl, name, country, shipping_price]
        self.db.insert(com, vals)
        
        #Save payment_method, sender & mark info
        self.savePaymentMethods(payment_method, weburl)
        self.saveMarks(mark, weburl)
        self.saveSenders(sender, weburl)
        
    def savePaymentMethods(self, payment_method, weburl):
        com = "INSERT IGNORE INTO payment_methods (method, website_url) VALUES ((%s), (%s))"
        if payment_method != None and payment_method != "None": 
            if isinstance(payment_method, types.ListType): #If the type is list, multiple values need to be saved
                for method in payment_method:
                    vals = [method, weburl]
                    self.db.insert(com, vals)
            else: #Else, it's a single value that needs to be saved
                vals = [payment_method, weburl]
                self.db.insert(com, vals)
                
    def saveMarks(self, mark, weburl):
        com = "INSERT IGNORE INTO marks (mark, website_url) VALUES ((%s), (%s))"
        if mark != None and mark != "None":
            if isinstance(mark, types.ListType): #If the type is list, multiple values need to be saved
                for record in mark:
                    vals = [record, weburl]
                    self.db.insert(com, vals)
            else: #Else, it's a single value that needs to be saved
                vals = [mark, weburl]
                self.db.insert(com, vals)
                
    def saveSenders(self, sender, weburl):
        com = "INSERT IGNORE INTO senders (sender, website_url) VALUES ((%s), (%s))"
        if sender != None and sender != "None":
            if isinstance(sender, types.ListType): #If the type is list, multiple values need to be saved
                for record in sender:
                    vals = [record, weburl]
                    self.db.insert(com, vals)
            else:  #Else, it's a single value that needs to be saved
                vals = [sender, weburl]
                self.db.insert(com, vals)
    
    def closeConnection(self):
        self.db.closeConnection()
        
    def openConnection(self):
        self.db.openConnection()
        
