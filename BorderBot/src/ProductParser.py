from xml.etree.ElementTree import ElementTree, Element, SubElement
from ConfigParser import SafeConfigParser

from Handlers import Database


class Parser():
    def __init__(self):
        self.parseConfigFile()

    columns = ['Title', 'Brand', 'Category', 'EAN', 'SKU', 'Price', 'Availability', 'Deeplink', 'Image', 'ProductID']

    db = Database.Queries()

    websites = ''
    feedPath = ''

    def main(self):
        self.getWebsites()

        # Save product information for each website
        for website in self.websites:
            website = website[0]

            self.db.openConnection()
            records = self.db.selectProductsForWebsite(website)
            self.db.closeConnection()

            root = Element('Products')

            for record in records:
                product = SubElement(root, 'Product')

                i = 0
                for column in self.columns:
                    child = SubElement(product, column)
                    if record[i] == None:
                        child.text = ''
                    else:
                        try:
                            child.text = record[i].decode('ISO-8859-1')
                        except AttributeError:
                            child.text = str(record[i])
                    i += 1

            ElementTree(root).write(self.feedPath + website.replace('http://', '') + '.xml', encoding='UTF-8')

    # Parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
        self.feedPath = parser.get('General', 'feedpath')

    def getWebsites(self):
        self.db.openConnection()
        self.websites = self.db.selectWebsites()
        self.db.closeConnection()

Parser().main()