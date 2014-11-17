import MySQLdb as mdb


class Connection:

    host = "localhost"
    user = "root"
    password = "border1!LOOP"
    database = "borderbot"

    con = (mdb.connect(host, user, password, database))

    cur = ''

    def select(self, com, vals):
        try:
            self.cur.execute(com, vals)

            ver = self.cur.fetchone()
            return ver
        except Exception as e:
            print e

    def insert(self, com, vals):
        try:
            self.cur.execute(com, vals)
            self.con.commit()
        except mdb.DataError as e:
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

    def selectTitle(self, title, website):
        com = "SELECT title FROM product WHERE title = (%s) AND website_url = (%s)"
        vals = [title, website]
        return self.db.select(com, vals)

    def insertNewArticle(self, title, website, brand, price, availability, deeplink, category=None, ean=None, sku=None, imageLocation=None):
        com = "INSERT INTO product VALUES ((%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s), (%s))"
        vals = [title, website, brand, category, ean, sku, price, availability, deeplink, imageLocation]
        self.db.insert(com, vals)

    def updateArticle(self, title, website, price, availability):
        com = "UPDATE product SET price = (%s), availability = (%s) WHERE title = (%s) AND website_url = (%s)"
        vals = [price, availability, title, website]
        self.db.insert(com, vals)

    def closeConnection(self):
        self.db.closeConnection()

    def openConnection(self):
        self.db.openConnection()