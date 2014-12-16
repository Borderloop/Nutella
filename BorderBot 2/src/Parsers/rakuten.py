import re
import math
from ConfigParser import SafeConfigParser

from Handlers import Database


class Parser():
    def __init__(self, soup, website, url):
        self.parseConfigFile()
        self.soup = soup
        self.website = website
        self.url = url

    db = Database.Queries()

    # Procedure that controls the main flow of the parser.
    def main(self):
        self.parseProductPage()

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')
        self.disallowedCategories = parser.get('getgoods', 'disallowedcategories').split(',')

    # This procedure parses the product page. If it gets here it means it's a new product,
    # so it always saves the product which is parsed here to the database.
    def parseProductPage(self):
        title = self.soup.find('h1', {'itemprop': 'name'}).text
        price = self.soup.find('span', {'itemprop': 'price'}).text
        price = self.cleanPrice(price)
        availability = self.soup.find('link', {'itemprop': 'availability'}).nextSibling.nextSibling.text
        category = self.soup.find('div', {'id': 'breadbrumb'}).find_all('div', recursive=False)[-1].find('div', {'itemprop': 'title'}).text
        imageLocation = self.soup.find('div', {'id': 'product_image_dv4'}).find('a')['href']

        data = self.soup.find('div', {'id': 'tab_description'}).find_all('td')
        for item in data:
            if 'Artikel' in item.text:
                productID = item.nextSibling.nextSibling.text
            elif 'Hersteller' in item.text:
                sku = item.nextSibling.nextSibling.text
            elif 'EAN' in item.text:
                ean = item.nextSibling.nextSibling.text

        if self.checkIfParsedBefore(productID, self.website) == ():  # Product hasn't been parsed before, insert it.
            self.db.openConnection()
            self.db.insertNewArticle(self.website, productID, title, price, self.url, ean=ean, category=category, availability=availability, imageLocation=imageLocation, sku=sku)
            self.db.closeConnection()
        else:  # Known product, update only
            self.db.openConnection()
            self.db.updateArticle(productID, self.website, price)
            self.db.closeConnection()

    # This procedure checks if a product has been parsed before.
    def checkIfParsedBefore(self, productID, website):
        self.db.openConnection()
        result = self.db.selectProduct(productID, website)
        self.db.closeConnection()

        return result

    # This method gets rid of all other characters in a price.
    def cleanPrice(self, price):
        price = price.strip().replace('.', '').replace(',', '.')
        non_decimal = re.compile(r'[^\d.]+')
        return non_decimal.sub('', price)