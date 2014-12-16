import MySQLdb as mdb
from time import strftime
import Logger


class Connection:

    host = "localhost"
    user = "root"
    password = "border1!LOOP"
    database = "borderbot"

    con = (mdb.connect(host, user, password, database, charset='utf8'))

    cur = ''

    def select(self, com, vals=None):
        try:
            self.cur.execute(com, vals)

            ver = self.cur.fetchall()
            return ver
        except Exception as e:
            print e

    def insert(self, com, vals):
        try:
            queryTime = strftime("%H:%M:%S")
            self.cur.execute(com, vals)
            Logger.logQuery(com, vals, queryTime)
            self.con.commit()
        except mdb.DataError as e:
            print e
        except mdb.IntegrityError as e:
            print e

    def openConnection(self):
        try:
            self.cur = self.con.cursor()
        except Exception as e:
            print e

    def closeConnection(self):
        if self.cur:
            self.cur.close()


class Queries():
    db = Connection()

    def selectProduct(self, productID, website):
        com = "SELECT title FROM product WHERE product_id = (%s) AND website_url = (%s)"
        vals = [productID, website]
        return self.db.select(com, vals)

    def selectProductByTitle(self, title, website):
        com = "SELECT product_id FROM product WHERE title = (%s) AND website_url = (%s)"
        vals = [title, website]
        return self.db.select(com, vals)

    def insertNewArticle(self, website, productID, title, price, deeplink, brand=None, availability=None, category=None, ean=None, sku=None, imageLocation=None):
        com = "INSERT INTO product VALUES ((%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s))"
        vals = [website, productID, title, brand, category, ean, sku, price, availability, deeplink, imageLocation]
        self.db.insert(com, vals)

    def updateArticle(self, productID, website, price, availability=None):
        com = "UPDATE product SET price = (%s), availability = (%s) WHERE product_id = (%s) AND website_url = (%s)"
        vals = [price, availability, productID, website]
        self.db.insert(com, vals)

    def selectWebsites(self):
        com = "SELECT DISTINCT website_url FROM product"
        return self.db.select(com)

    def selectProductsForWebsite(self, website):
        com = "SELECT title, brand, category, ean, sku, price, availability, deeplink, " \
              "image_loc, product_id FROM product WHERE website_url = (%s)"
        vals = [website]
        return self.db.select(com, vals)

    def closeConnection(self):
        self.db.closeConnection()

    def openConnection(self):
        self.db.openConnection()