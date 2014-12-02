import re
import math
from ConfigParser import SafeConfigParser

from Handlers import Database


class Parser():
    def __init__(self, soup, page, website, url):
        self.parseConfigFile()
        self.soup = soup
        self.page = page
        self.website = website
        self.url = url

    db = Database.Queries()
    urls = []
    disallowedCategories = []

    # Procedure that controls the main flow of the parser.
    def main(self):
        self.urls = []

        if self.page == 'home':
            self.parseHomePage()
        elif self.page == 'subCategory':
            self.parseSubCategoryPage()
        elif self.page == 'products':
            self.parseProductsPage()
        elif self.page == 'product':
            self.parseProductPage()

        return self.urls

    # Procedure to parse the config file
    def parseConfigFile(self):
        parser = SafeConfigParser()
        parser.read('C:/BorderSoftware/BorderBot/settings/borderbot.ini')

    # This procedure parses the home page
    def parseHomePage(self):
        mainCategories = self.soup.find('ul', {'id': 'menu-sidebar-drop'}).find_all('li', recursive=False)
        for category in mainCategories:
            subCategories = category.find('ul').find_all('li')

            for subCategory in subCategories:
                if subCategory != subCategories[0] and subCategory != subCategories[1] and subCategory != subCategories[2]:
                    self.urls.append(self.url + subCategory.find('a')['href'])


    # This procedure parses the subcategory page
    def parseSubCategoryPage(self):
        categories = self.soup.find('ul', {'id': 'menu-sidebar-drop'}).find_all('li')
        for category in categories:
            try:
                self.urls.append(self.website + category.find('a')['href'])
            except TypeError:
                pass

    # This procedure parses the products page, the page which enlists multiple products.
    def parseProductsPage(self):
        products = self.soup.find('div', {'id': 'vmMainPage'}).find_all('div', {'class': 'box-content float-left'})
        for product in products:
            self.urls.append(self.website + product.find('a')['href'])

        self.checkForNextPage()

    # This procedure parses the product page. If it gets here it means it's a new product,
    # so it always saves the product which is parsed here to the database.
    def parseProductPage(self):
        title = self.soup.find('h1', {'class': 'title'}).text.strip()
        title = " ".join(title.split())
        price = self.soup.find('span', {'class': 'productPrice'}).text
        price = self.cleanPrice(price)


        try:
            ean = self.soup.find('b', text='EAN').parent.text
            ean = ean[3:].strip()
        except AttributeError:  # No EAN present
            ean = None

        category = self.soup.find('div', {'class': 'breadcrumbs'}).find('strong').text
        productID = self.soup.find('div', {'id': 'vmMainPage'}).find('span').text
        productID = productID[productID.find(':')+1:].strip()

        if self.checkIfParsedBefore(productID, self.website) == ():  # Product hasn't been parsed before, insert it.
            self.db.openConnection()
            self.db.insertNewArticle(self.website, productID, title, price, self.url, ean=ean, category=category)
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

    # Procedure to check if there's a next page and if so, add it to the urls.
    def checkForNextPage(self):
        pagination = self.soup.find('div', {'class': 'pagination'})
        try:
            currentPage = int(pagination.find('strong').text)
        except AttributeError:  # Only one page
            return

        try:
            self.urls.append(self.website + pagination.find('a', text=str(currentPage+1))['href'])
        except TypeError:  # Reached last page
            pass